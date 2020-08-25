using System;

namespace Kprocess.KL2.FileServer
{
    public class MediaInfo
    {
        public bool HasVideo { get; set; }
        public bool HasAudio { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int? VideoBitrate { get; set; }
        public int? AudioBitrate { get; set; }
        public string VideoCodec { get; set; }
        public string AudioCodec { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
