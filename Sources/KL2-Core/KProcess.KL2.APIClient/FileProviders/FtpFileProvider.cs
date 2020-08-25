using FluentFTP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using tusdotnet.Interfaces;

namespace KProcess.KL2.APIClient
{
    public class FtpFileProvider : IFileProvider
    {
        const string FtpServerKey = "Ftp_Server";
        const string FtpPortKey = "Ftp_Port";
        const string FtpUserKey = "Ftp_User";
        const string FtpPasswordKey = "Ftp_Password";
        const string PublishedFilesDirectoryKey = "Ftp_PublishedFilesDirectory";
        const string UploadedFilesDirectoryKey = "Ftp_UploadedFilesDirectory";

        public string Server { get; private set; } = @"127.0.0.1";
        public int Port { get; private set; } = 21;
        public string User { get; private set; }
        public string Password { get; private set; }
        public NetworkCredential Credentials { get; private set; }

        public string PublishedFilesDirectory { get; private set; } = @"/PublishedFiles";
        public string UploadedFilesDirectory { get; private set; } = @"/UploadedFiles";

        public bool SupportsRange =>
            false;

        public FtpFileProvider()
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(FtpServerKey))
                Server = ConfigurationManager.AppSettings[FtpServerKey];

            if (ConfigurationManager.AppSettings.AllKeys.Contains(FtpPortKey))
                Port = int.Parse(ConfigurationManager.AppSettings[FtpPortKey]);

            if (ConfigurationManager.AppSettings.AllKeys.Contains(FtpUserKey))
                User = ConfigurationManager.AppSettings[FtpUserKey];

            if (ConfigurationManager.AppSettings.AllKeys.Contains(FtpPasswordKey))
                Password = ConfigurationManager.AppSettings[FtpPasswordKey];

            if (ConfigurationManager.AppSettings.AllKeys.Contains(PublishedFilesDirectoryKey))
                PublishedFilesDirectory = ConfigurationManager.AppSettings[PublishedFilesDirectoryKey];
            Uri publishedFilesDirectoryUri = new Uri(PublishedFilesDirectory, UriKind.RelativeOrAbsolute);
            PublishedFilesDirectory = publishedFilesDirectoryUri.IsAbsoluteUri ?
                publishedFilesDirectoryUri.AbsolutePath :
                Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), publishedFilesDirectoryUri.OriginalString);

            if (ConfigurationManager.AppSettings.AllKeys.Contains(UploadedFilesDirectoryKey))
                UploadedFilesDirectory = ConfigurationManager.AppSettings[UploadedFilesDirectoryKey];
            Uri uploadedFilesDirectoryUri = new Uri(UploadedFilesDirectory, UriKind.RelativeOrAbsolute);
            UploadedFilesDirectory = uploadedFilesDirectoryUri.IsAbsoluteUri ?
                uploadedFilesDirectoryUri.AbsolutePath :
                Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), uploadedFilesDirectoryUri.OriginalString);

            Credentials = new NetworkCredential(User, Password);
        }

        public async Task<bool> Exists(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(GetFilePath(fileName, directoryEnum));
            request.Credentials = Credentials;
            request.UseBinary = true;
            request.KeepAlive = true;
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            FtpWebResponse response = null;
            try
            {
                response = (FtpWebResponse)(await request.GetResponseAsync());
                return true;
            }
            catch (WebException ex)
            {
                response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return false;
            }
            return false;
        }

        public Task<bool> IsDirectory(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published)
        {
            return Task.FromResult(false);
            /*FtpWebRequest request = (FtpWebRequest)WebRequest.Create(GetDirPath(directoryEnum));
            request.Credentials = Credentials;
            request.UseBinary = true;
            request.KeepAlive = true;
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            FtpWebResponse response = null;
            try
            {
                response = (FtpWebResponse)(await request.GetResponseAsync());
                return true;
            }
            catch (WebException ex)
            {
                response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return false;
            }
            return false;*/
        }

        public async Task<Stream> OpenRead(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published) =>
            await Exists(fileName) ?
                new SeekableFtpDataStream(new FtpClient
                {
                    Host = Server,
                    Port = Port,
                    Credentials = Credentials
                }, FileAccess.Read, $"{(directoryEnum == DirectoryEnum.Published ? PublishedFilesDirectory : UploadedFilesDirectory)}/{fileName}") as Stream :
                null;

        public Task<Stream> OpenWrite(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published) =>
            Task.FromResult(new SeekableFtpDataStream(new FtpClient
            {
                Host = Server,
                Port = Port,
                Credentials = Credentials
            }, FileAccess.Write, $"{(directoryEnum == DirectoryEnum.Published ? PublishedFilesDirectory : UploadedFilesDirectory)}/{fileName}") as Stream);

        public async Task<long> GetLengthAsync(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(GetFilePath(fileName, directoryEnum));
            request.Credentials = Credentials;
            request.UseBinary = true;
            request.KeepAlive = true;
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            FtpWebResponse response = null;
            try
            {
                response = (FtpWebResponse)(await request.GetResponseAsync());
                return response.ContentLength;
            }
            catch (WebException ex)
            {
                response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    return -1;
            }
            return -1;
        }

        public async Task<bool> Delete(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(GetFilePath(fileName, directoryEnum));
                request.Credentials = Credentials;
                request.UseBinary = true;
                request.KeepAlive = true;
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                await request.GetResponseAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public ITusStore GetTusStore() =>
            null;

        public Task Complete(string oldFileName, string newFileName = null) =>
            Task.CompletedTask;

        public Task Upload(FileInfo fileInfo, string newFileName = null, DirectoryEnum directoryEnum = DirectoryEnum.Published, IProgress<double> progress = null, CancellationToken cancellationToken = default(CancellationToken)) =>
            Task.FromException(new NotImplementedException());

        public Task Upload((FileInfo fileInfo, string newFileName)[] files, DirectoryEnum directoryEnum = DirectoryEnum.Published, IProgress<double> progress = null, CancellationToken cancellationToken = default(CancellationToken)) =>
            Task.FromException(new NotImplementedException());

        public Task<IEnumerable<string>> EnumerateFiles(DirectoryEnum directoryEnum, string searchPattern) =>
            Task.FromException<IEnumerable<string>>(new NotImplementedException());

        string GetFilePath(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published) =>
            $@"ftp://{Server}/{(directoryEnum == DirectoryEnum.Published ? PublishedFilesDirectory : UploadedFilesDirectory)}/{fileName.TrimStart('/', '\\')}";

        string GetDirPath(DirectoryEnum directoryEnum = DirectoryEnum.Published) =>
            $@"ftp://{Server}/{(directoryEnum == DirectoryEnum.Published ? PublishedFilesDirectory : UploadedFilesDirectory)}";

        #region Extensions

        public string ReadAllText(string path) =>
            throw new NotSupportedException();

        public void WriteAllText(string path, string contents) =>
            throw new NotSupportedException();

        public Stream GetStream(string path, FileMode mode, FileAccess access, FileShare share) =>
            throw new NotSupportedException();

        public long GetLength(string path) =>
            throw new NotSupportedException();

        public Stream Create(string path) =>
            throw new NotSupportedException();

        #endregion
    }
}
