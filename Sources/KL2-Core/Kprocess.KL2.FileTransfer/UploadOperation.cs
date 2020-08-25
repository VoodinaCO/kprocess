using System.Linq;
using usis.Net.Bits;

namespace Kprocess.KL2.FileTransfer
{
    public class UploadOperation : TransferOperation
    {
        public override JobType JobType =>
            JobType.Upload;

        public BackgroundCopyFile File =>
            _fileTransferManager._manager.EnumerateJobs()
                .SingleOrDefault(j => j.Id == _job.Id)
                ?.EnumerateFiles()
                ?.FirstOrDefault();

        internal UploadOperation(FileTransferManager fileTransferManager, BackgroundCopyJob job)
            : base(fileTransferManager, job)
        {
        }

        public UploadOperation Reset()
        {
            var remoteFilePath = File.RemoteName;
            var localFilePath = File.LocalName;
            Cancel();
            return _fileTransferManager.CreateUpload(Group, remoteFilePath, localFilePath);
        }
    }
}
