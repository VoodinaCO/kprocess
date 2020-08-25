using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ReencodeTool
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisedPropertyChanged([CallerMemberName]string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        readonly IContext context;
        int transcodedFiles;

        Visibility _isLoadingVisibility = Visibility.Visible;
        public Visibility IsLoadingVisibility
        {
            get => _isLoadingVisibility;
            set
            {
                if (_isLoadingVisibility != value)
                {
                    _isLoadingVisibility = value;
                    RaisedPropertyChanged();
                }
            }
        }

        Visibility _usageVisibility = Visibility.Visible;
        public Visibility UsageVisibility
        {
            get => _usageVisibility;
            set
            {
                if (_usageVisibility != value)
                {
                    _usageVisibility = value;
                    RaisedPropertyChanged();
                }
            }
        }

        Visibility _progressListVisibility = Visibility.Collapsed;
        public Visibility ProgressListVisibility
        {
            get => _progressListVisibility;
            set
            {
                if (_progressListVisibility != value)
                {
                    _progressListVisibility = value;
                    RaisedPropertyChanged();
                }
            }
        }

        public ObservableDictionary<string, TranscodeTask> TranscodeTasks { get; private set; }

        CancellationTokenSource GlobalCancellationTokenSource { get; set; }

        public MainWindow()
        {
            GlobalCancellationTokenSource = new CancellationTokenSource();
            context = Application.Current as IContext;
            TranscodeTasks = new ObservableDictionary<string, TranscodeTask>();

            InitializeComponent();

            Loaded += (o, e) =>
            {
                DataContext = this;
                context.OriginalFiles.ForEach(fileInfo =>
                {
                    if (!TranscodeTasks.ContainsKey(Path.GetFileNameWithoutExtension(fileInfo.Name)))
                    {
                        var progress = new Progress<double>();
                        TranscodeTasks.Add(Path.GetFileNameWithoutExtension(fileInfo.Name), new TranscodeTask { FileInfo = fileInfo, Task = Transcode(fileInfo, progress, GlobalCancellationTokenSource.Token), Progress = progress });
                        UsageVisibility = Visibility.Collapsed;
                        ProgressListVisibility = Visibility.Visible;
                    }
                });
            };
        }

        ICommand _exitCommand;

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand != null)
                    return _exitCommand;
                _exitCommand = new DelegateCommand<object>(async o =>
                {
                    if (TranscodeTasks.Any(_ => _.Value.TranscodingTaskStatus == TranscodingTaskStatus.InProgress))
                    {
                        var result = MessageBox.Show(
                            "You have transcoding tasks running.\nDo you really want to exit and cancel all of them ?",
                            "Exit", MessageBoxButton.YesNo);
                        if (result != MessageBoxResult.Yes)
                            return;
                        IsLoadingVisibility = Visibility.Visible;
                        GlobalCancellationTokenSource.Cancel();
                        await Task.WhenAll(TranscodeTasks.Select(_ => _.Value.Task).ToList());
                    }
                    Application.Current.Shutdown();
                });
                return _exitCommand;
            }
        }

        void Grid_Drop(object sender, DragEventArgs e)
        {
            var test = e.Data.GetFormats();
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                var fileInfos = filePaths.Select(_ => new FileInfo(_)).Where(_ => _.Exists);
                foreach (var fileInfo in fileInfos)
                {
                    if (!TranscodeTasks.ContainsKey(Path.GetFileNameWithoutExtension(fileInfo.Name)))
                    {
                        var progress = new Progress<double>();
                        TranscodeTasks.Add(Path.GetFileNameWithoutExtension(fileInfo.Name), new TranscodeTask { FileInfo = fileInfo, Task = Transcode(fileInfo, progress, GlobalCancellationTokenSource.Token), Progress = progress });
                        UsageVisibility = Visibility.Collapsed;
                        ProgressListVisibility = Visibility.Visible;
                    }
                }
            }
        }

        MediaInfo GetMediaInfo(FileInfo file)
        {
            MediaInfo result = null;
            string json = "";
            using (var process = new Process())
            {
                process.StartInfo.FileName = context.FfprobePath;
                process.StartInfo.Arguments = $"-v quiet -print_format json -show_format -show_streams \"{file.FullName}\"";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;

                process.OutputDataReceived += (s, e) =>
                {
                    json += e.Data;
                };

                try
                {
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }
                catch (Exception e)
                {
                    throw new Exception("An issue has occured during video packaging. It seems that some components are missing.", e);
                }
            }
            JObject jsonRoot = JObject.Parse(json);
            if (jsonRoot.ContainsKey("streams"))
            {
                result = new MediaInfo();
                var streams = jsonRoot["streams"];
                var audio = streams.FirstOrDefault(_ => (string)_.SelectToken("codec_type") == "audio");
                var video = streams.FirstOrDefault(_ => (string)_.SelectToken("codec_type") == "video");
                if (audio != null)
                {
                    result.HasAudio = true;
                    if (int.TryParse((string)audio.SelectToken("bit_rate"), out int audioBitrate))
                        result.AudioBitrate = audioBitrate;
                    result.AudioCodec = (string)audio.SelectToken("codec_name");
                    if (!string.IsNullOrEmpty((string)audio.SelectToken("duration")))
                    {
                        if (double.TryParse((string)audio.SelectToken("duration"), NumberStyles.Any, CultureInfo.InvariantCulture, out double durationValue))
                            result.Duration = TimeSpan.FromSeconds(durationValue);
                    }
                }
                if (video != null)
                {
                    result.HasVideo = true;
                    if (int.TryParse((string)video.SelectToken("bit_rate"), out int videoBitrate))
                        result.VideoBitrate = videoBitrate;
                    result.VideoCodec = (string)video.SelectToken("codec_name");
                    if (!string.IsNullOrEmpty((string)video.SelectToken("duration")))
                    {
                        if (double.TryParse((string)video.SelectToken("duration"), NumberStyles.Any, CultureInfo.InvariantCulture, out double durationValue))
                            result.Duration = TimeSpan.FromSeconds(durationValue);
                    }
                    var rotate = (string)video["tags"]?.SelectToken("rotate");
                    double? NullableRotateValue = null;
                    if (!string.IsNullOrEmpty(rotate))
                    {
                        if (double.TryParse(rotate, out double rotateValue))
                            NullableRotateValue = rotateValue;
                    }
                    bool invertWidthHeight = NullableRotateValue.HasValue && NullableRotateValue != 0 && NullableRotateValue.Value % 90 == 0;
                    result.Width = invertWidthHeight ? int.Parse((string)video.SelectToken("height")) : int.Parse((string)video.SelectToken("width"));
                    result.Height = invertWidthHeight ? int.Parse((string)video.SelectToken("width")) : int.Parse((string)video.SelectToken("height"));
                }
            }

            return result;
        }

        public Task<bool> Transcode(FileInfo file, IProgress<double> progress, CancellationToken ct) =>
            Task.Run(() =>
            {
                string transcodeExt = ".mp4";
                int maxAudioBitRate = 96000; // 96k
                int maxVideoBitRate = 1500000; // 1500k
                int maxResolution = 720;
                TimeSpan videoDuration = new TimeSpan(0);
                TimeSpan progressDuration = new TimeSpan(0);

                var mediaInfo = GetMediaInfo(file);
                var processArgumentsBuilder = new StringBuilder($"-hide_banner -y -nostdin -i \"{file.FullName}\"");
                if (mediaInfo.HasAudio)
                    processArgumentsBuilder.Append($" -acodec mp3 -b:a {Math.Min(mediaInfo.AudioBitrate ?? maxAudioBitRate, maxAudioBitRate)}");
                if (mediaInfo.HasVideo)
                {
                    if (Math.Min(mediaInfo.Width, mediaInfo.Height) > maxResolution) // Needs downscaling
                        processArgumentsBuilder.Append($" -vf \"scale={(mediaInfo.Width > mediaInfo.Height ? $"-2:{maxResolution}" : $"{maxResolution}:-2")}\"");
                    else if (mediaInfo.Width % 2 != 0) // Needs rescaling (MP4 resolutions must be multiple of 2)
                    {
                        mediaInfo.Width++;
                        processArgumentsBuilder.Append($" -vf \"scale={mediaInfo.Width}:-2\"");
                    }
                    else if (mediaInfo.Height % 2 != 0) // Needs rescaling (MP4 resolutions must be multiple of 2)
                    {
                        mediaInfo.Height++;
                        processArgumentsBuilder.Append($" -vf \"scale=-2:{mediaInfo.Height}\"");
                    }
                    processArgumentsBuilder.Append($" -vcodec libx264 -preset fast -b:v {Math.Min(mediaInfo.VideoBitrate ?? maxVideoBitRate, maxVideoBitRate)}");
                }
                var outFile = Path.Combine(context.TranscodeFolder, $"{Path.GetFileNameWithoutExtension(file.FullName)}{transcodeExt}");
                processArgumentsBuilder.Append($" -f mp4 -movflags faststart \"{outFile}\"");

                using (var process = new Process())
                {
                    process.StartInfo.FileName = context.FfmpegPath;
                    process.StartInfo.Arguments = processArgumentsBuilder.ToString();
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.RedirectStandardOutput = true;

                    process.OutputDataReceived += (s, e) => Console.WriteLine(e.Data);
                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (e.Data != null && e.Data.Trim().StartsWith("Duration:"))
                        {
                            string duration = e.Data.Trim().Split(',').First().Split(' ')[1].Trim();
                            TimeSpan.TryParse(duration, out videoDuration);
                        }
                        if (e.Data != null && e.Data.Trim().StartsWith("frame="))
                        {
                            string duration = e.Data.Trim().Split(' ').Single(_ => _.StartsWith("time=")).Split('=')[1];
                            TimeSpan.TryParse(duration, out progressDuration);
                            double progressValue = videoDuration.Ticks == 0 ? 0 : Math.Round(progressDuration.TotalMilliseconds * 100 / videoDuration.TotalMilliseconds);
                            progress?.Report(progressValue);
                            if (ct.IsCancellationRequested)
                                process.Kill();
                        }
                        //Console.WriteLine(e.Data);
                    };

                    try
                    {
                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        process.WaitForExit();
                        if (process.ExitCode == 0)
                        {
                            transcodedFiles++;
                            return true;
                        }
                    }
                    catch (Win32Exception e)
                    {
                        Console.WriteLine($"Le splitter de video n'a pas été trouvé :\n{e.Message}");
                    }
                    catch (OperationCanceledException e)
                    {
                        Console.WriteLine($"L'export a été annulé lors du split des vidéos :\n{e.Message}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Une erreur non prévue s'est produite lors du split de la video :\n{e.Message}");
                    }

                    try
                    {
                        File.Delete(outFile);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Erreur lors de la suppression du fichier \"{outFile}\" :\n{e.Message}");
                    }

                    return false;
                }
            }, ct);

        async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            while (App.IsLoading)
                await Task.Delay(100);
            IsLoadingVisibility = Visibility.Hidden;
        }

        void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            ExitCommand.Execute(null);
            return;
        }
    }
}
