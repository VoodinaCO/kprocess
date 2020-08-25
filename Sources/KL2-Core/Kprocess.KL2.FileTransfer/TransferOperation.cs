using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using usis.Net.Bits;

namespace Kprocess.KL2.FileTransfer
{
    public abstract class TransferOperation : NotifiableObject, ITransferOperation
    {
        protected readonly FileTransferManager _fileTransferManager;
        protected readonly BackgroundCopyJob _job;
        readonly Progress<BackgroundCopyJobProgress> _jobProgress;
        readonly Progress<BackgroundCopyError> _jobError;

        public event EventHandler<JobType> OnTransferFinished;

        public virtual JobType JobType =>
            JobType.Download;

        public string Group =>
            _job.DisplayName;

        public Guid Guid =>
            _job.Id;

        BackgroundTransferProgress _progress;
        public BackgroundTransferProgress Progress
        {
            get => _progress;
            private set
            {
                if (_progress != value)
                {
                    _progress = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Error =>
            _fileTransferManager._manager.EnumerateJobs()
                .SingleOrDefault(j => j.Id == _job.Id)
                ?.RetrieveError()
                ?.Description;

        TransferStatus _state;
        public TransferStatus State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    RaisePropertyChanged();
                    CanResume = _state == TransferStatus.Suspended;
                    CanPause = _state == TransferStatus.Transferring;
                    CanCancel = _state != TransferStatus.Canceled;
                }
            }
        }

        bool _isFinished;
        public bool IsFinished
        {
            get => _isFinished;
            set
            {
                if (_isFinished != value)
                {
                    _isFinished = value;
                    if (_isFinished)
                    {
                        var job = _fileTransferManager._manager.EnumerateJobs()
                            .SingleOrDefault(j => j.Id == _job.Id);

                        if (job != null)
                        {
                            job.Transferred -= OnModified;
                            job.Modified -= OnModified;
                            job.Failed -= OnFailed;
                        }

                        OnTransferFinished?.Invoke(this, JobType);
                    }
                    RaisePropertyChanged();
                }
            }
        }

        bool _canResume;
        public bool CanResume
        {
            get => _canResume;
            set
            {
                if (_canResume != value)
                {
                    _canResume = value;
                    RaisePropertyChanged();
                }
            }
        }

        bool _canPause;
        public bool CanPause
        {
            get => _canPause;
            set
            {
                if (_canPause != value)
                {
                    _canPause = value;
                    RaisePropertyChanged();
                }
            }
        }

        bool _canCancel;
        public bool CanCancel
        {
            get => _canCancel;
            set
            {
                if (_canCancel != value)
                {
                    _canCancel = value;
                    RaisePropertyChanged();
                }
            }
        }

        internal TransferOperation(FileTransferManager fileTransferManager, BackgroundCopyJob job)
        {
            _fileTransferManager = fileTransferManager;
            _job = job;
            State = _job.State.ToTranferStatus();
            _jobProgress = new Progress<BackgroundCopyJobProgress>();
            _jobError = new Progress<BackgroundCopyError>();

            Progress = new BackgroundTransferProgress(_fileTransferManager, _job);
            var progressInfos = _job.RetrieveProgress();
            Progress.BytesTransferred = progressInfos.BytesTransferred;
            IsFinished = progressInfos.FilesTransferred == progressInfos.FilesTotal;
            Progress.PropertyChanged += (sender, e) => RaisePropertyChanged(nameof(Progress));
        }

        void OnModified(object sender, EventArgs e)
        {
            var progress = _fileTransferManager._manager.EnumerateJobs()
                .SingleOrDefault(j => j.Id == _job.Id)
                ?.RetrieveProgress();
            if (progress != null)
                (_jobProgress as IProgress<BackgroundCopyJobProgress>).Report(progress);
        }

        void OnFailed(object sender, BackgroundCopyErrorEventArgs e)
        {
            var error = _fileTransferManager._manager.EnumerateJobs()
                .SingleOrDefault(j => j.Id == _job.Id)
                ?.RetrieveError();
            if (error != null)
                (_jobError as IProgress<BackgroundCopyError>).Report(error);
        }

        public void Resume()
        {
            var job = _fileTransferManager._manager.EnumerateJobs()
               .SingleOrDefault(j => j.Id == _job.Id);
            if (job == null)
                return;

            _jobProgress.ProgressChanged += (sender, e) =>
            {
                var innerJob = _fileTransferManager._manager.EnumerateJobs()
                   .SingleOrDefault(j => j.Id == _job.Id);
                if (innerJob != null)
                    State = innerJob.State.ToTranferStatus();
                RaisePropertyChanged(nameof(Error));
                Progress.RaisePropertyChanged(nameof(BackgroundTransferProgress.PercentBytesTransferred));
                Progress.RaisePropertyChanged(nameof(BackgroundTransferProgress.BytesTotal));
                Progress.BytesTransferred = e.BytesTransferred;
                Progress.RaisePropertyChanged(nameof(BackgroundTransferProgress.FilesTotal));
                Progress.RaisePropertyChanged(nameof(BackgroundTransferProgress.FilesTransferred));

                IsFinished = Progress.FilesTransferred == Progress.FilesTotal;
            };

            _jobError.ProgressChanged += (sender, e) =>
            {
                var innerJob = _fileTransferManager._manager.EnumerateJobs()
                   .SingleOrDefault(j => j.Id == _job.Id);
                if (innerJob != null)
                    State = innerJob.State.ToTranferStatus();
                RaisePropertyChanged(nameof(Error));
                innerJob?.Cancel();
            };

            job.Transferred += OnModified;
            job.Modified += OnModified;
            job.Failed += OnFailed;

            job.Resume();
            Progress.stopWatch.Restart();
        }

        public void Pause()
        {
            var job = _fileTransferManager._manager.EnumerateJobs()
               .SingleOrDefault(j => j.Id == _job.Id);

            if (job == null)
                return;

            job.Transferred -= OnModified;
            job.Modified -= OnModified;
            job.Failed -= OnFailed;

            job.Suspend();
            Progress.Speed = 0;
        }

        public TaskResult Cancel()
        {
            var job = _fileTransferManager._manager.EnumerateJobs()
               .SingleOrDefault(j => j.Id == _job.Id);

            if (job == null)
                return TaskResult.Ok;

            job.Transferred -= OnModified;
            job.Modified -= OnModified;
            job.Failed -= OnFailed;

            try
            {
                switch (job.State)
                {
                    case BackgroundCopyJobState.Acknowledged:
                    case BackgroundCopyJobState.Transferred:
                        return TaskResult.Ok;
                    default:
                        job.Cancel();
                        return TaskResult.Cancelled;
                }
            }
            catch
            {
                return TaskResult.Nok;
            }
        }

        public async Task<TaskResult> WaitTransferFinished(CancellationTokenSource cts = null)
        {
            try
            {
                while (!IsFinished)
                {
                    if (cts?.IsCancellationRequested == true)
                        return Cancel();
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                }
                return TaskResult.Ok;
            }
            catch
            {
                return TaskResult.Nok;
            }
        }
    }

    public enum TaskResult
    {
        Ok,
        Nok,
        Cancelled
    }
}
