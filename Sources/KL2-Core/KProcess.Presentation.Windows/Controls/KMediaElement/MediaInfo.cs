using System;

namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// Defines a model containing informations on a media
    /// </summary>
    public class MediaInfo
    {
        /// <summary>
        /// Gets or sets if the media has video.
        /// </summary>
        public bool HasVideo { get; set; }

        /// <summary>
        /// Gets or sets if the media has audio.
        /// </summary>
        public bool HasAudio { get; set; }

        /// <summary>
        /// Gets or sets the length of the video.
        /// </summary>
        /// <value>
        /// The length of the video.
        /// </value>
        public TimeSpan VideoLength { get; set; }

        /// <summary>
        /// Gets or sets the height of the frame (pixels).
        /// </summary>
        /// <value>
        /// The height of the frame.
        /// </value>
        public int FrameHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the frame (pixels).
        /// </summary>
        /// <value>
        /// The width of the frame.
        /// </value>
        public int FrameWidth { get; set; }

        /// <summary>
        /// Gets or sets the video data rate (kbps).
        /// </summary>
        /// <value>
        /// The video data rate.
        /// </value>
        public int VideoDataRate { get; set; }

        /// <summary>
        /// Gets or sets the video frame rate (frames/seconds).
        /// </summary>
        /// <value>
        /// The video frame rate.
        /// </value>
        public double VideoFrameRate { get; set; }

        /// <summary>
        /// Gets or sets the audio codec.
        /// </summary>
        /// <value>
        /// The audio codec.
        /// </value>
        public string AudioCodec { get; set; }

        /// <summary>
        /// Gets or sets the length of the audio.
        /// </summary>
        /// <value>
        /// The length of the audio.
        /// </value>
        public TimeSpan AudioLength { get; set; }

        /// <summary>
        /// Gets or sets the channels (2 mins stereo).
        /// </summary>
        /// <value>
        /// The channels.
        /// </value>
        public int AudioChannels { get; set; }

        /// <summary>
        /// Gets or sets the audio sample rate (kbps).
        /// </summary>
        /// <value>
        /// The audio sample rate.
        /// </value>
        public int AudioSampleRate { get; set; }

        /// <summary>
        /// Gets or sets the video codec.
        /// </summary>
        /// <value>
        /// The video codec.
        /// </value>
        public string VideoCodec { get; set; }

        /// <summary>
        /// Gets or sets the video bits per pixel.
        /// </summary>
        /// <value>
        /// The video bits per pixel.
        /// </value>
        public int VideoBitsPerPixel { get; set; }

        /// <summary>
        /// The number of bits per sample in the audio stream
        /// </summary>
        /// <value>
        /// The audio bits per sample.
        /// </value>
        public int AudioBitsPerSample { get; set; }
    }
}
