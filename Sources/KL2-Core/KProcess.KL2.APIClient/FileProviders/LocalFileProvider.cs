using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using tusdotnet.Interfaces;
using tusdotnet.Stores;
using System.Web;

namespace KProcess.KL2.APIClient
{
    public class LocalFileProvider : IFileProvider
    {
        const string PublishedFilesDirectoryKey = "Local_PublishedFilesDirectory";
        const string UploadedFilesDirectoryKey = "Local_UploadedFilesDirectory";

        public string PublishedFilesDirectory { get; private set; } = @"PublishedFiles";
        public string UploadedFilesDirectory { get; private set; } = @"UploadedFiles";

        public bool SupportsRange =>
            true;

        public LocalFileProvider()
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(PublishedFilesDirectoryKey))
                PublishedFilesDirectory = ConfigurationManager.AppSettings[PublishedFilesDirectoryKey];
            Uri publishedFilesDirectoryUri = new Uri(PublishedFilesDirectory, UriKind.RelativeOrAbsolute);
            PublishedFilesDirectory = publishedFilesDirectoryUri.IsAbsoluteUri ?
                HttpUtility.UrlDecode(publishedFilesDirectoryUri.AbsolutePath) :
                Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), publishedFilesDirectoryUri.OriginalString);

            if (ConfigurationManager.AppSettings.AllKeys.Contains(UploadedFilesDirectoryKey))
                UploadedFilesDirectory = ConfigurationManager.AppSettings[UploadedFilesDirectoryKey];
            Uri uploadedFilesDirectoryUri = new Uri(UploadedFilesDirectory, UriKind.RelativeOrAbsolute);
            UploadedFilesDirectory = uploadedFilesDirectoryUri.IsAbsoluteUri ?
                HttpUtility.UrlDecode(uploadedFilesDirectoryUri.AbsolutePath) :
                Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), uploadedFilesDirectoryUri.OriginalString);
        }

        public Task<bool> Exists(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published) =>
            Task.FromResult(Directory.GetFiles(directoryEnum == DirectoryEnum.Published ? PublishedFilesDirectory : UploadedFilesDirectory, Path.GetFileName(HttpUtility.UrlDecode(fileName)), SearchOption.TopDirectoryOnly).Any());

        public Task<bool> IsDirectory(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published) =>
            Task.FromResult(Directory.Exists(Path.Combine(directoryEnum == DirectoryEnum.Published? PublishedFilesDirectory : UploadedFilesDirectory, Path.GetFileName(HttpUtility.UrlDecode(fileName)))));

        public async Task<Stream> OpenRead(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published) =>
            await Exists(fileName, directoryEnum) ? File.OpenRead(GetFilePath(HttpUtility.UrlDecode(fileName), directoryEnum)) as Stream : null;

        public Task<Stream> OpenWrite(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published) =>
            Task.FromResult(File.OpenWrite(GetFilePath(fileName, directoryEnum)) as Stream);

        public async Task<long> GetLengthAsync(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published) =>
            await Exists(fileName, directoryEnum) ? new FileInfo(GetFilePath(fileName, directoryEnum)).Length : -1;

        public Task<bool> Delete(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published) =>
            Task.Run(() =>
            {
                try
                {
                    File.Delete(GetFilePath(fileName, directoryEnum));
                    return true;
                }
                catch
                {
                    return false;
                }
            });

        public ITusStore GetTusStore()
        {
            string directoryPath = UploadedFilesDirectory;
            return new TusDiskStore(directoryPath);
        }

        public Task Complete(string oldFileName, string newFileName = null) =>
            Task.Run(async () =>
            {
                var srcPath = GetFilePath(oldFileName, DirectoryEnum.Uploaded);
                var dstPath = GetFilePath(string.IsNullOrEmpty(newFileName) ? oldFileName : newFileName, DirectoryEnum.Published);
                if (File.Exists(dstPath))
                    File.Delete(dstPath);
                while (File.Exists(dstPath))
                    await Task.Delay(200);
                File.Move(srcPath, dstPath);
            });

        public async Task Upload(FileInfo fileInfo, string newFileName = null, DirectoryEnum directoryEnum = DirectoryEnum.Published, IProgress<double> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfo.Exists)
            {
                using (var outputFile = File.OpenWrite($"{(directoryEnum == DirectoryEnum.Published ? PublishedFilesDirectory : UploadedFilesDirectory)}/{(string.IsNullOrEmpty(newFileName) ? fileInfo.Name : HttpUtility.UrlDecode(newFileName))}"))
                using (var inputFile = File.OpenRead(fileInfo.FullName))
                {
                    await inputFile.CopyToAsync(outputFile, progress, cancellationToken);
                }
            }
        }

        public async Task Upload((FileInfo fileInfo, string newFileName)[] files, DirectoryEnum directoryEnum = DirectoryEnum.Published, IProgress<double> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            long totalLength = 0;
            long alreadyReadedLength = 0;
            foreach ((FileInfo fileInfo, string newFileName) in files)
                totalLength += fileInfo.Exists ? fileInfo.Length : 0;
            foreach ((FileInfo fileInfo, string newFileName) in files)
            {
                if (fileInfo.Exists)
                {
                    using (var outputFile = File.OpenWrite($"{(directoryEnum == DirectoryEnum.Published ? PublishedFilesDirectory : UploadedFilesDirectory)}/{(string.IsNullOrEmpty(newFileName) ? fileInfo.Name : HttpUtility.UrlDecode(newFileName))}"))
                    using (var inputFile = File.OpenRead(fileInfo.FullName))
                    {
                        await inputFile.CopyToAsync(outputFile, progress, cancellationToken, totalLength, alreadyReadedLength);
                    }
                }
            }
        }

        public Task<IEnumerable<string>> EnumerateFiles(DirectoryEnum directoryEnum, string searchPattern) =>
            Task.FromResult(Directory.EnumerateFiles(directoryEnum == DirectoryEnum.Published ? PublishedFilesDirectory : UploadedFilesDirectory, searchPattern));

        string GetFilePath(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published) =>
            Path.Combine(directoryEnum == DirectoryEnum.Published ? PublishedFilesDirectory : UploadedFilesDirectory, HttpUtility.UrlDecode(fileName).TrimStart('/','\\'));

        #region Extensions

        public string ReadAllText(string path) =>
            File.ReadAllText(HttpUtility.UrlDecode(path));

        public void WriteAllText(string path, string contents) =>
            File.WriteAllText(HttpUtility.UrlDecode(path), contents);

        public Stream GetStream(string path, FileMode mode, FileAccess access, FileShare share) =>
            File.Open(HttpUtility.UrlDecode(path), mode, access, share);

        public long GetLength(string path) =>
            new FileInfo(HttpUtility.UrlDecode(path)).Length;

        public Stream Create(string path) =>
            File.Create(HttpUtility.UrlDecode(path));

        #endregion
    }
}
