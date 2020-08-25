using System.Collections.Generic;
using System.Linq;
using usis.Net.Bits;

namespace Kprocess.KL2.FileTransfer
{
    public class DownloadOperation : TransferOperation
    {
        public override JobType JobType =>
            JobType.Download;

        public IEnumerable<BackgroundCopyFile> Files =>
            _fileTransferManager._manager.EnumerateJobs()
                .SingleOrDefault(j => j.Id == _job.Id)
                ?.EnumerateFiles() ?? new List<BackgroundCopyFile>();

        public bool Contains(string localFilePath) =>
            Files.Any(_ => _.LocalName == localFilePath);

        public void AddFile(string remoteFilePath, string localFilePath) =>
            _fileTransferManager._manager.EnumerateJobs()
                .SingleOrDefault(j => j.Id == _job.Id)
                ?.AddFile(remoteFilePath, localFilePath);

        public void AddFiles(params (string remoteFilePath, string localFilePath)[] files) =>
            _fileTransferManager._manager.EnumerateJobs()
                .SingleOrDefault(j => j.Id == _job.Id)
                ?.AddFiles(files.Select(_ => new BackgroundCopyFileInfo { RemoteName = _.remoteFilePath, LocalName = _.localFilePath }).ToArray());

        internal DownloadOperation(FileTransferManager fileTransferManager, BackgroundCopyJob job)
            : base(fileTransferManager, job)
        {
        }
    }
}
