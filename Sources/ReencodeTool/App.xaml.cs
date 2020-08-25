using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace ReencodeTool
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application, IContext
    {
        public static bool IsLoading = true;

        readonly List<string> FFME_Resources = new List<string>
        {
            "avcodec-58.dll",
            "avdevice-58.dll",
            "avfilter-7.dll",
            "avformat-58.dll",
            "avutil-56.dll",
            "ffmpeg.exe",
            "ffplay.exe",
            "ffprobe.exe",
            "postproc-55.dll",
            "swresample-3.dll",
            "swscale-5.dll"
        };

        public DirectoryInfo DirFFME { get; private set; }
        public string FfmpegPath { get; private set; }
        public string FfprobePath { get; private set; }
        public string TranscodeFolder { get; private set; }

        public List<FileInfo> OriginalFiles { get; private set; } = new List<FileInfo>();

        public App()
        {
            //Extract ffme
            DirFFME = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "ffme"));

            FfmpegPath = Path.Combine(DirFFME.FullName, "ffmpeg.exe");
            FfprobePath = Path.Combine(DirFFME.FullName, "ffprobe.exe");
            TranscodeFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "transcoded");
            Directory.CreateDirectory(TranscodeFolder);
            var extractTask = ExtractFFME(DirFFME.FullName);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var files = e.Args.Select(_ => new FileInfo(_)).Where(_ => _.Exists);
            if (files.Any())
                OriginalFiles.AddRange(files);
        }

        async Task ExtractEmbeddedResource(string fileName, string outFilePath)
        {
            using (var resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
            using (var outFileStream = new FileStream(outFilePath, FileMode.Create, FileAccess.Write))
            {
                await resStream.CopyToAsync(outFileStream);
            }
        }

        async Task ExtractFFME(string basePath)
        {
            var appName = Assembly.GetExecutingAssembly().GetName().Name;
            var extractTasks = FFME_Resources.Select(async res => await ExtractEmbeddedResource(GetFFMEResourceFilePath(appName, res), Path.Combine(basePath, res))).ToList();
            await Task.WhenAll(extractTasks);
            IsLoading = false;
        }

        string GetFFMEResourceFilePath(string appName, string fileName) =>
            $"{appName}.ffme.{fileName}";
    }
}
