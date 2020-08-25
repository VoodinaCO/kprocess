using Kprocess.KL2.FileTransfer;
using System;

namespace KProcess.Ksmed.Models
{
    public partial class PublishedFile
    {
        public PublishedFile(CloudFile cloudFile)
        {
            Hash = cloudFile.Hash;
            Extension = cloudFile.Extension;
        }

        public Uri Uri =>
            new Uri($"{Preferences.FileServerUri}/GetFile/{Hash}{Extension}");
    }
}
