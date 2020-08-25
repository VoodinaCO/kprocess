using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;
using usis.Net.Bits;

namespace Kprocess.KL2.FileTransfer
{
    public class FileTransferManager
    {
        internal readonly BackgroundCopyManager _manager;
        readonly DispatcherTimer _timerTransfer;

        readonly FileSystemWatcher _watcher;

        public event EventHandler<string> OnFileAdded;
        public event EventHandler<string> OnFileDeleted;
#pragma warning disable CS0067
        public event EventHandler<string> OnTransferFinished;
        public event EventHandler<string> OnTransferProgress;
#pragma warning restore CS0067

        readonly ObservableDictionary<string, ITransferOperation> _downloadOperations;
        public ObservableDictionary<string, ITransferOperation> DownloadOperations
        {
            get
            {
                IEnumerable<BackgroundCopyJob> dlJobs = _manager.EnumerateJobs()
                    .Where(_ => _.JobType == BackgroundCopyJobType.Download);

                // On met à jour les jobs
                dlJobs.AsParallel().ForAll(job =>
                {
                    if (_downloadOperations.Any(_ => _.Value.Group == job.DisplayName && _.Value.Guid != job.Id)) // On remplace les jobs ayant le même nom mais des GUID différents
                        _downloadOperations.UpdateWithNotification(job.DisplayName, new DownloadOperation(this, job));
                    else if (!_downloadOperations.Any(_ => _.Value.Guid == job.Id)) // On ajoute les nouveaux jobs
                        _downloadOperations.AddWithNotification(job.DisplayName, new DownloadOperation(this, job));
                });

                // On supprime les jobs annulés, terminés ou qui n'existent plus
                _downloadOperations.RemoveWithNotification(_ =>
                {
                    var dlJob = dlJobs.SingleOrDefault(job => job.Id == _.Value.Guid);
                    if (dlJob == null)
                        return true;
                    if (dlJob.State == BackgroundCopyJobState.Canceled)
                        return true;
                    if (dlJob.State == BackgroundCopyJobState.Acknowledged)
                        return true;
                    return false;
                });

                return _downloadOperations;
            }
        }

        readonly ObservableDictionary<string, ITransferOperation> _uploadOperations;
        public ObservableDictionary<string, ITransferOperation> UploadOperations
        {
            get
            {
                IEnumerable<BackgroundCopyJob> ulJobs = _manager.EnumerateJobs()
                    .Where(_ => _.JobType == BackgroundCopyJobType.Upload);

                // On met à jour les jobs
                ulJobs.AsParallel().ForAll(job =>
                {
                    if (_uploadOperations.Any(_ => _.Value.Group == job.DisplayName && _.Value.Guid != job.Id)) // On remplace les jobs ayant le même nom mais des GUID différents
                        _uploadOperations.UpdateWithNotification(job.DisplayName, new UploadOperation(this, job));
                    else if (!_uploadOperations.Any(_ => _.Value.Guid == job.Id)) // On ajoute les nouveaux jobs
                        _uploadOperations.AddWithNotification(job.DisplayName, new UploadOperation(this, job));
                });

                // On supprime les jobs annulés, terminés ou qui n'existent plus
                _uploadOperations.RemoveWithNotification(_ =>
                {
                    var ulJob = ulJobs.SingleOrDefault(job => job.Id == _.Value.Guid);
                    if (ulJob == null)
                        return true;
                    if (ulJob.State == BackgroundCopyJobState.Canceled)
                        return true;
                    if (ulJob.State == BackgroundCopyJobState.Acknowledged)
                        return true;
                    return false;
                });

                return _uploadOperations;
            }
        }

        public ConcatenatedObservableDictionary<string, ITransferOperation> TransferOperations { get; }

        public FileTransferManager()
        {
            _watcher = new FileSystemWatcher(Preferences.SyncDirectory);
            _watcher.Created += (o, e) => OnFileAdded?.Invoke(this, e.FullPath);
            _watcher.Renamed += (o, e) => OnFileAdded?.Invoke(this, e.FullPath);
            _watcher.Deleted += (o, e) => OnFileDeleted?.Invoke(this, e.FullPath);
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        _watcher.EnableRaisingEvents = true;
                        return;
                    }
                    catch { }
                    await Task.Delay(500);
                    _watcher.Path = Preferences.SyncDirectory;
                }
            });

            _manager = BackgroundCopyManager.Connect();

            if (ConfigurationManager.AppSettings.AllKeys.Contains(Preferences.FileServerUriKey))
                Preferences.FileServerUri = ConfigurationManager.AppSettings[Preferences.FileServerUriKey];

            _downloadOperations = new ObservableDictionary<string, ITransferOperation>();
            _uploadOperations = new ObservableDictionary<string, ITransferOperation>();
            TransferOperations = new ConcatenatedObservableDictionary<string, ITransferOperation>(_downloadOperations, _uploadOperations);

            _timerTransfer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _timerTransfer.Tick += TimerTransfer_Tick;
            _timerTransfer.Start();
        }

        void TimerTransfer_Tick(object sender, EventArgs e)
        {
            ResumeAllJobOnError();
        }

        public void ResumeAllJobOnError()
        {
            TransferOperations.Values.AsParallel()
                .ForAll(op =>
                {
                    if (op.State == TransferStatus.TransientError)
                        op.Resume();
                    op.Progress.RaisePropertyChanged(nameof(BackgroundTransferProgress.PercentBytesTransferred));
                });
        }

        BackgroundCopyNotifyCommandLine GetTransferredCmdLine(Guid jobId) =>
            new BackgroundCopyNotifyCommandLine("bitsadmin.exe", $"bitsadmin.exe /complete {{{jobId}}}");

        public DownloadOperation CreateDownload(string group, params (Uri remoteFileUri, string localFilePath)[] uris)
        {
            BackgroundCopyJob job = null;
            try
            {
                job = _manager.CreateJob(group, BackgroundCopyJobType.Download);
                job.Priority = BackgroundCopyJobPriority.Foreground;
                job.NotifyCommandLine = GetTransferredCmdLine(job.Id);
                foreach ((Uri remoteFileUri, string localFilePath) in uris)
                    job.AddFile(remoteFileUri, localFilePath);
                var result = new DownloadOperation(this, job);
                _downloadOperations.AddWithNotification(result.Group, result);
                return result;
            }
            catch (Exception e)
            {
                job?.Cancel();
                throw e;
            }
        }

        public DownloadOperation CreateDownload(string group, params (string remoteFilePath, string localFilePath)[] paths)
        {
            BackgroundCopyJob job = null;
            try
            {
                job = _manager.CreateJob(group, BackgroundCopyJobType.Download);
                job.Priority = BackgroundCopyJobPriority.Foreground;
                job.NotifyCommandLine = GetTransferredCmdLine(job.Id);
                foreach ((string remoteFilePath, string localFilePath) in paths)
                    job.AddFile(remoteFilePath, localFilePath);
                var result = new DownloadOperation(this, job);
                _downloadOperations.AddWithNotification(result.Group, result);
                return result;
            }
            catch (Exception e)
            {
                job?.Cancel();
                throw e;
            }
        }

        public UploadOperation CreateUpload(string group, Uri remoteFileUri, string localFilePath)
        {
            BackgroundCopyJob job = null;
            try
            {
                job = _manager.CreateJob(group, BackgroundCopyJobType.Upload);
                job.Priority = BackgroundCopyJobPriority.Foreground;
                job.NotifyCommandLine = GetTransferredCmdLine(job.Id);
                job.AddFile(remoteFileUri, localFilePath);
                var result = new UploadOperation(this, job);
                _uploadOperations.AddWithNotification(result.Group, result);
                return result;
            }
            catch (Exception e)
            {
                job?.Cancel();
                throw e;
            }
        }

        public UploadOperation CreateUpload(string group, string remoteFilePath, string localFilePath)
        {
            BackgroundCopyJob job = null;
            try
            {
                job = _manager.CreateJob(group, BackgroundCopyJobType.Upload);
                job.Priority = BackgroundCopyJobPriority.Foreground;
                job.NotifyCommandLine = GetTransferredCmdLine(job.Id);
                job.AddFile(remoteFilePath, localFilePath);
                var result = new UploadOperation(this, job);
                _uploadOperations.AddWithNotification(result.Group, result);
                return result;
            }
            catch (Exception e)
            {
                job?.Cancel();
                throw e;
            }
        }

        public static List<string> ListSyncFiles(string syncFolder = null)
        {
            string downloadedFilesDirectory = string.IsNullOrEmpty(syncFolder)
                ? Preferences.SyncDirectory
                : Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), syncFolder);
            DirectoryInfo syncDirectory = new DirectoryInfo(Preferences.SyncDirectory);
            return syncDirectory.GetFiles()
                .Select(_ => _.FullName)
                .ToList();
        }

        public static Task DeleteSyncFiles(IEnumerable<string> filesToDelete) =>
            Task.WhenAll(filesToDelete.Select(_ => Task.Run(() => File.Delete(_))));

        public static Task CleanSyncFiles(string syncFolder = null)
        {
            string downloadedFilesDirectory = string.IsNullOrEmpty(syncFolder)
                ? Preferences.SyncDirectory
                : Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), syncFolder);
            DirectoryInfo syncDirectory = new DirectoryInfo(Preferences.SyncDirectory);
            return DeleteSyncFiles(syncDirectory.GetFiles()
                .Select(_ => _.FullName));
        }
    }
}
