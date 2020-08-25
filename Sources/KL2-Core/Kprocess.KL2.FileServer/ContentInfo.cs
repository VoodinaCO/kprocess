using KProcess.KL2.APIClient;

namespace Kprocess.KL2.FileServer
{
    public class ContentInfo
    {
        public long From;
        public long To;
        public bool IsPartial;
        public long Length;

        public ContentInfo(bool isPartial, long? from, long? to)
        {
            IsPartial = isPartial;
            From = from ?? 0;
            To = to ?? From + StreamExtensions.BufferSize - 1;
            Length = To - From + 1;
        }
    }
}
