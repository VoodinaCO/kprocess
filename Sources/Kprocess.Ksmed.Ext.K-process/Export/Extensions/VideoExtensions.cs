using KProcess.Ksmed.Ext.Kprocess.Enums;
using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Ext.Kprocess.Export
{
    public static class VideoExtensions
    {
        private const string VideoBufferDirectory = @"Video\Buffer\";
        private const int _MARKING_PADDING = 10;

#if DEBUG
        static VideoExtensions()
        {
            var bufferDirectoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), VideoBufferDirectory);
            if (!Directory.Exists(bufferDirectoryPath))
                Directory.CreateDirectory(bufferDirectoryPath);
        }
#endif

        public static IVideoTransformations CutForAction(this Video video, KAction action)
        {
            return new VideoCut(video.FilePath)
            {
                From = TimeSpan.FromTicks(action.Start),
                Duration = TimeSpan.FromTicks(action.Duration)
            };
        }

        public static string GetVideoBufferFolderPath() =>
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), VideoBufferDirectory);


        public static string GetInVideoBufferFolder(string file) =>
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), VideoBufferDirectory, Path.GetFileName(file));

        private class VideoCut : IVideoTransformations
        {
            private readonly string _input;

            public TimeSpan From { get; set; }

            public TimeSpan Duration { get; set; }

            public VideoCut(string input)
            {
                _input = input;
            }

            public string Save(string videoName, string overlayTextMarking, EHorizontalAlign markingHorizontalAlign, EVerticalAlign markingVerticalAlign, double? durationMini = null)
            {
                this.TraceDebug($"Saving video \"{videoName}\" in buffer");

                var videoFileName = $"{videoName}.mpeg";
               
                var exeFilePath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    @"Video\",
                    Environment.Is64BitOperatingSystem ? "videosplitter-64.exe" : "videosplitter-32.exe");
                var fontFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    @"Video\", "arial.ttf");
                var textFilePath = Path.GetTempFileName();
                if (!string.IsNullOrWhiteSpace(overlayTextMarking))
                {
                    using (var stream = File.OpenWrite(textFilePath))
                    using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                    {
                        writer.Write(overlayTextMarking.Replace(@"\",@"\\"));
                    }
                }

                string markingAlignment = string.Empty;

                switch(markingHorizontalAlign)
                {
                    case EHorizontalAlign.Center:
                        markingAlignment = $"x=(w-tw)/2";
                        break;
                    case EHorizontalAlign.Left:
                        markingAlignment = $"x={_MARKING_PADDING}";
                        break;
                    case EHorizontalAlign.Right:
                        markingAlignment = $"x=w-tw-{_MARKING_PADDING}";
                        break;
                }

                switch(markingVerticalAlign)
                {
                    case EVerticalAlign.Bottom:
                        markingAlignment += $":y=h-th-{_MARKING_PADDING}";
                        break;
                    case EVerticalAlign.Center:
                        markingAlignment += $":y=(h-th)/2";
                        break;
                    case EVerticalAlign.Top:
                        markingAlignment += $":y={_MARKING_PADDING}";
                        break;
                }

                double speed = 1;
                bool slowMotion = false;
                if (durationMini != null && Duration.TotalSeconds < durationMini)
                {
                    slowMotion = true;
                    speed = Duration.TotalMilliseconds / (durationMini.Value * 1000);
                }
                List<string> filters = new List<string>();
                if (slowMotion)
                    filters.Add($"setpts=(1/{speed.ToString().Replace(',', '.')})*PTS");
                if (!string.IsNullOrWhiteSpace(overlayTextMarking))
                {
                    string formattedFontFilePath = fontFilePath.Replace(@"\", @"\\").Replace(":", @"\:");
                    string formattedtextFilePath = textFilePath.Replace(@"\", @"\\").Replace(":", @"\:");
                    filters.Add($"drawtext=fontfile='{formattedFontFilePath}':textfile='{formattedtextFilePath}':fontcolor=white:fontsize=24:box=1:boxcolor=black@0.2:boxborderw=5:{markingAlignment}");
                }

                var processArgumentsBuilder = new StringBuilder("-y -nostdin");
                processArgumentsBuilder.Append($" -ss {From.ToString(@"hh\:mm\:ss")}.{From.Milliseconds}");
                processArgumentsBuilder.Append($" -t {Duration.ToString(@"hh\:mm\:ss")}.{Math.Max(Duration.Milliseconds, 200)}");
                processArgumentsBuilder.Append($" -i \"{_input}\"");
                if (filters.Count > 0)
                {
                    if (slowMotion)
                        processArgumentsBuilder.Append(" -an");
                    processArgumentsBuilder.Append($" -vf \"{string.Join(",", filters)}\"");
                }
                processArgumentsBuilder.Append($" -vcodec mpeg1video -b:v 4000k \"{videoFileName}\"");

                using (var process = new Process())
                {
                    process.StartInfo.FileName = exeFilePath;
                    process.StartInfo.Arguments = processArgumentsBuilder.ToString();
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.RedirectStandardOutput = true;

                    this.TraceDebug($"{exeFilePath} {processArgumentsBuilder}");

                    try
                    {
                        string errorOutput = null;
                        string standardOutput = null;

                        process.Start();

                        var outputStreamTasks = new[]
                        {
                            Task.Factory.StartNew(() => errorOutput = process.StandardError.ReadToEnd()),
                            Task.Factory.StartNew(() => standardOutput = process.StandardOutput.ReadToEnd())
                        };

                        var timeout = TimeSpan.FromMinutes(3); // TODO add to configuration
                        process.WaitForExit((int)timeout.TotalMilliseconds);
                        Task.WaitAll(outputStreamTasks, (int)timeout.TotalMilliseconds, KprocessExportWindow.CancellationToken);

                        this.TraceDebug(standardOutput);
                        Debug.WriteLine(standardOutput);

                        if (!string.IsNullOrWhiteSpace(errorOutput))
                        {
                            this.TraceError("Des erreurs ont été levées par le processus permettant de splitter les videos:");
                            this.TraceError(errorOutput);
                            Debug.WriteLine(errorOutput);
                        }
                    }
                    catch (Win32Exception e)
                    {
                        this.TraceError(e, "Le splitter de video n'a pas été trouvé");
                        throw new Exception("An issue has occured during video packaging. It seems that some components are missing.", e);
                    }
                    catch (OperationCanceledException e)
                    {
                        this.TraceError(e, "L'export a été annulé lors du split des vidéos");
                        throw;
                    }
                    catch (Exception e)
                    {
                        this.TraceError(e, "Une erreur non prévue s'est produite lors du split de la video");
                        throw; 
                    }
                    finally
                    {
                        try
                        {
                            File.Delete(textFilePath);
                        }
                        catch { }
                    }
                }

                return videoName;
            }
        }
    }

    public interface IVideoTransformations
    {
        string Save(string outputPath, string overlayTextMarking, EHorizontalAlign markingHorizontalAlign, EVerticalAlign markingVerticalAlign, double? durationMini = null);
    }
}
