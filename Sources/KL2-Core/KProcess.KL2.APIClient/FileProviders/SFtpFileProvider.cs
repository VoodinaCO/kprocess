using KProcess.KL2.APIClient.Tus;
using Microsoft.VisualBasic.CompilerServices;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
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
    public class SFtpFileProvider : IFileProvider
    {
        const string SFtpServerKey = "SFtp_Server";
        const string SFtpPortKey = "SFtp_Port";
        const string SFtpUserKey = "SFtp_User";
        const string SFtpPasswordKey = "SFtp_Password";
        const string PublishedFilesDirectoryKey = "SFtp_PublishedFilesDirectory";
        const string UploadedFilesDirectoryKey = "SFtp_UploadedFilesDirectory";
        readonly object connectLock = new object();

        readonly ITraceManager _traceManager;

        public string Server { get; private set; } = @"127.0.0.1";
        public int Port { get; private set; } = 22;
        public string User { get; private set; }
        public string Password { get; private set; }
        public NetworkCredential Credentials { get; private set; }

        public string PublishedFilesDirectory { get; private set; } = @"/PublishedFiles";
        public string UploadedFilesDirectory { get; private set; } = @"/UploadedFiles";

        public bool SupportsRange =>
            true;

        SftpClient _sftpClient;

        public SFtpFileProvider(ITraceManager traceManager)
        {
            _traceManager = traceManager;

            ConfigurationManager.RefreshSection("appSettings");

            if (ConfigurationManager.AppSettings.AllKeys.Contains(SFtpServerKey))
                Server = ConfigurationManager.AppSettings[SFtpServerKey];

            if (ConfigurationManager.AppSettings.AllKeys.Contains(SFtpPortKey))
                Port = int.Parse(ConfigurationManager.AppSettings[SFtpPortKey]);

            if (ConfigurationManager.AppSettings.AllKeys.Contains(SFtpUserKey))
                User = ConfigurationManager.AppSettings[SFtpUserKey];

            if (ConfigurationManager.AppSettings.AllKeys.Contains(SFtpPasswordKey))
                Password = ConfigurationManager.AppSettings[SFtpPasswordKey];

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

            _sftpClient = new SftpClient(Server, Port, Credentials.UserName, Credentials.Password);

            // Format directory for all SFTP servers
            try
            {
                _traceManager.TraceInfo($"Test SFTP connection with Server : {Server}, Port : {Port}, UserName : {Credentials.UserName}, Password : {Credentials.Password}");
                _sftpClient.Connect();
                if (_sftpClient.Exists(Path.Combine(_sftpClient.WorkingDirectory, PublishedFilesDirectory)))
                    PublishedFilesDirectory = Path.Combine(_sftpClient.WorkingDirectory, PublishedFilesDirectory);
                else
                    _traceManager.TraceError($"Error when accessing to '{Path.Combine(_sftpClient.WorkingDirectory, PublishedFilesDirectory)}'");
                if (_sftpClient.Exists(Path.Combine(_sftpClient.WorkingDirectory, UploadedFilesDirectory)))
                    UploadedFilesDirectory = Path.Combine(_sftpClient.WorkingDirectory, UploadedFilesDirectory);
                else
                    _traceManager.TraceError($"Error when accessing to '{Path.Combine(_sftpClient.WorkingDirectory, UploadedFilesDirectory)}'");
            }
            catch (Exception e)
            {
                _traceManager.TraceError(e, "Error when testing SFTP connection");
            }
        }

        void Connect()
        {
            lock (connectLock)
            {
                int maxRetries = 10;
                int retriesCount = 0;

                if (_sftpClient == null)
                    _sftpClient = new SftpClient(Server, Port, Credentials.UserName, Credentials.Password);
                while (!_sftpClient.IsConnected && retriesCount < maxRetries)
                {
                    try
                    {
                        _sftpClient.Connect();
                    }
                    catch
                    {
                        retriesCount++;
                        Thread.Sleep(100);
                    }
                }
            }
        }

        public Task<bool> Exists(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published)
        {
            _traceManager.TraceInfo($"Test if file exists : {GetFilePath(fileName, directoryEnum)}");
            try
            {
                Connect();
                return Task.FromResult(_sftpClient.Exists(GetFilePath(fileName, directoryEnum)));
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> IsDirectory(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published)
        {
            try
            {
                Connect();
                SftpFile sftpFile = _sftpClient.Get(GetFilePath(fileName, directoryEnum));
                return Task.FromResult(sftpFile?.IsDirectory ?? false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public async Task<Stream> OpenRead(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published)
        {
            if (await Exists(fileName, directoryEnum))
                return _sftpClient.OpenRead(GetFilePath(fileName, directoryEnum)) as Stream;
            return null;
        }

        public Task<Stream> OpenWrite(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published)
        {
            Connect();
            return Task.FromResult(_sftpClient.OpenWrite(GetFilePath(fileName, directoryEnum)) as Stream);
        }

        public async Task<long> GetLengthAsync(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published)
        {
            if (await Exists(fileName))
            {
                SftpFile file = _sftpClient.Get(GetFilePath(fileName, directoryEnum));
                return file.Length;
            }
            return -1;
        }

        public async Task<bool> Delete(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published)
        {
            try
            {
                if (await Exists(fileName, directoryEnum))
                {
                    try
                    {
                        _sftpClient.DeleteFile(GetFilePath(fileName, directoryEnum));
                    }
                    catch (SftpPermissionDeniedException) // If file is in use, delete it later
                    {
                        var deleteTask = Task.Run(async () =>
                        {
                            await Task.Delay(TimeSpan.FromSeconds(5));
                            _sftpClient.DeleteFile(GetFilePath(fileName, directoryEnum));
                        });
                    }
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public ITusStore GetTusStore() =>
            new TusSFtpStore(this, UploadedFilesDirectory);

        public async Task Complete(string oldFileName, string newFileName = null)
        {
            _traceManager.TraceDebug($"SFTP Complete({oldFileName}, {newFileName ?? "null"})");
            string resultFileName = string.IsNullOrEmpty(newFileName) ? oldFileName : newFileName;

            string oldPath = GetFilePath(oldFileName, DirectoryEnum.Uploaded);
            string newPath = GetFilePath(resultFileName, DirectoryEnum.Published);

            if (await Exists(oldFileName, DirectoryEnum.Uploaded))
            {
                _traceManager.TraceDebug($"{oldFileName} exists in {DirectoryEnum.Uploaded}");
                if (await Exists(resultFileName, DirectoryEnum.Published))
                {
                    _traceManager.TraceDebug($"{resultFileName} exists in {DirectoryEnum.Published}, delete {oldPath}");
                    _sftpClient.DeleteFile(oldPath);
                    return;
                }
                _traceManager.TraceDebug($"{resultFileName} doesn't exist in {DirectoryEnum.Published}, rename {oldPath} to {newPath}");
                try
                {
                    _sftpClient.RenameFile(oldPath, newPath);
                }
                catch (Exception ex)
                {
                    _traceManager.TraceDebug(ex, ex.Message);
                }
            }
            _traceManager.TraceDebug($"SFTP Complete finished");
        }

        public async Task Upload(FileInfo fileInfo, string newFileName = null, DirectoryEnum directoryEnum = DirectoryEnum.Published, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            Connect();
            if (fileInfo.Exists)
            {
                using (var outputFile = _sftpClient.OpenWrite($"{(directoryEnum == DirectoryEnum.Published ? PublishedFilesDirectory : UploadedFilesDirectory)}/{(string.IsNullOrEmpty(newFileName) ? fileInfo.Name : newFileName)}"))
                using (var inputFile = File.OpenRead(fileInfo.FullName))
                {
                    await inputFile.CopyToAsync(outputFile, progress, cancellationToken);
                }
            }
        }

        public async Task Upload((FileInfo fileInfo, string newFileName)[] files, DirectoryEnum directoryEnum = DirectoryEnum.Published, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        {
            Connect();
            long totalLength = 0;
            long alreadyReadedLength = 0;
            foreach ((FileInfo fileInfo, string newFileName) in files)
                totalLength += fileInfo.Exists ? fileInfo.Length : 0;
            foreach ((FileInfo fileInfo, string newFileName) in files)
            {
                if (fileInfo.Exists)
                {
                    using (var outputFile = _sftpClient.OpenWrite($"{(directoryEnum == DirectoryEnum.Published ? PublishedFilesDirectory : UploadedFilesDirectory)}/{(string.IsNullOrEmpty(newFileName) ? fileInfo.Name : newFileName)}"))
                    using (var inputFile = File.OpenRead(fileInfo.FullName))
                    {
                        await inputFile.CopyToAsync(outputFile, progress, cancellationToken, totalLength, alreadyReadedLength);
                    }
                }
            }
        }

        public Task<IEnumerable<string>> EnumerateFiles(DirectoryEnum directoryEnum, string searchPattern)
        {
            Connect();
            return Task.FromResult(_sftpClient.ListDirectory(directoryEnum == DirectoryEnum.Published ? PublishedFilesDirectory : UploadedFilesDirectory)
                .Where(_ => Operators.LikeString(_.Name, searchPattern, Microsoft.VisualBasic.CompareMethod.Text))
                .Select(_ => _.Name));
        }

        string GetFilePath(string fileName, DirectoryEnum directoryEnum = DirectoryEnum.Published)
        {
            if (directoryEnum == DirectoryEnum.Published)
                return $"{PublishedFilesDirectory.TrimEnd('/', '\\')}{(PublishedFilesDirectory.Contains("/") ? "/" : "\\")}{fileName.TrimStart('/', '\\')}";
            else
                return $"{UploadedFilesDirectory.TrimEnd('/', '\\')}{(UploadedFilesDirectory.Contains("/") ? "/" : "\\")}{fileName.TrimStart('/', '\\')}";
        }

        #region Extensions

        public string ReadAllText(string path)
        {
            Connect();
            return _sftpClient.ReadAllText(path);
        }

        public void WriteAllText(string path, string contents)
        {
            Connect();
            _sftpClient.WriteAllText(path, contents);
        }

        public Stream GetStream(string path, FileMode mode, FileAccess access, FileShare share)
        {
            Connect();
            return _sftpClient.Open(path, mode, access);
        }

        public long GetLength(string path)
        {
            Connect();
            return _sftpClient.Get(path).Length;
        }

        public Stream Create(string path)
        {
            Connect();
            return _sftpClient.Create(path);
        }

        #endregion
    }
}
