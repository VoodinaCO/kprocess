using usis.Net.Bits;

namespace Kprocess.KL2.FileTransfer
{
    public static class TransferStatusExtension
    {
        public static TransferStatus ToTranferStatus(this BackgroundCopyJobState state) =>
            (TransferStatus)state;
    }
}
