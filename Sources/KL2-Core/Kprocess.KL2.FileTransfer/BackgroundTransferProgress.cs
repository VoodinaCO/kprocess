using System;
using System.Diagnostics;
using System.Linq;
using usis.Net.Bits;

namespace Kprocess.KL2.FileTransfer
{
    public class BackgroundTransferProgress : NotifiableObject
    {
        public Stopwatch stopWatch = new Stopwatch();

        readonly FileTransferManager _fileTransferManager;
        readonly BackgroundCopyJob _job;

        public BackgroundTransferProgress(FileTransferManager fileTransferManager, BackgroundCopyJob job)
        {
            _fileTransferManager = fileTransferManager;
            _job = job;
        }

        public float PercentBytesTransferred =>
            _fileTransferManager._manager.EnumerateJobs()
                .SingleOrDefault(j => j.Id == _job.Id)
                ?.GetPercentBytesTransferred() ?? 0;

        public long BytesTotal =>
            _fileTransferManager._manager.EnumerateJobs()
                .SingleOrDefault(j => j.Id == _job.Id)
                ?.RetrieveProgress().BytesTotal ?? 0;

        long _bytesTransferred;
        public long BytesTransferred
        {
            get => _fileTransferManager._manager.EnumerateJobs()
                .SingleOrDefault(j => j.Id == _job.Id)
                ?.RetrieveProgress().BytesTotal ?? 0;
            set
            {
                if (_bytesTransferred != value)
                {
                    double seconds = stopWatch.IsRunning ? stopWatch.ElapsedMilliseconds / 1000 : 0;
                    long elapsedMs = stopWatch.ElapsedMilliseconds;
                    long speed = stopWatch.IsRunning && elapsedMs != 0 ? (value - _bytesTransferred) / elapsedMs * 1000 : 0;
                    stopWatch.Restart();
                    _bytesTransferred = value;
                    RaisePropertyChanged();
                    Speed = Math.Round((double)speed / 1024 / 1024, 2);
                }
            }
        }

        public int FilesTotal =>
            _fileTransferManager._manager.EnumerateJobs()
                .SingleOrDefault(j => j.Id == _job.Id)
                ?.RetrieveProgress().FilesTotal ?? 0;

        public int FilesTransferred =>
            _fileTransferManager._manager.EnumerateJobs()
                .SingleOrDefault(j => j.Id == _job.Id)
                ?.RetrieveProgress().FilesTransferred ?? 0;

        /// <summary>
        /// Speed in Mo/s
        /// </summary>
        double _speed;
        public double Speed
        {
            get => _speed;
            set
            {
                if (_speed != value)
                {
                    _speed = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
