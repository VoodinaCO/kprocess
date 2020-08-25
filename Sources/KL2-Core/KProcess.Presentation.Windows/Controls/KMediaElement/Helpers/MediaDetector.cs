using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// The MediaDetector class allows to query meta data from audio/video media files.
    /// This includes CODEC information and the ability to grab video snapshots.
    /// </summary>
    public static class MediaDetector
    {
        /// <summary>
        /// Get media info.
        /// </summary>
        /// <param name="uri">The full path of the media file to load</param>
        public static MediaInfo GetMediaInfo(string uri)
        {
            MediaInfo result = null;
            string json = "";
            using (var process = new Process())
            {
                process.StartInfo.FileName = @"ffme\ffprobe.exe";
                process.StartInfo.Arguments = $"-v quiet -print_format json -show_format -show_streams \"{uri}\"";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;

                process.OutputDataReceived += (s, e) =>
                {
                    json += e.Data;
                };

                try
                {
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }
                catch (Exception e)
                {
                    throw new Exception("An issue has occured during video packaging. It seems that some components are missing.", e);
                }
            }
            JObject jsonRoot = JObject.Parse(json);
            if (jsonRoot.ContainsKey("streams"))
            {
                result = new MediaInfo();
                var streams = jsonRoot["streams"];
                var audio = streams.FirstOrDefault(_ => (string)_.SelectToken("codec_type") == "audio");
                var video = streams.FirstOrDefault(_ => (string)_.SelectToken("codec_type") == "video");
                if (audio != null)
                {
                    result.HasAudio = true;
                    if (int.TryParse((string)audio.SelectToken("bit_rate"), out int audioBitrate))
                        result.AudioSampleRate = audioBitrate;
                    result.AudioCodec = (string)audio.SelectToken("codec_name");
                    if (!string.IsNullOrEmpty((string)audio.SelectToken("duration")))
                    {
                        if (double.TryParse((string)audio.SelectToken("duration"), NumberStyles.Any, CultureInfo.InvariantCulture, out double durationValue))
                            result.AudioLength = TimeSpan.FromSeconds(durationValue);
                    }
                    if (!string.IsNullOrEmpty((string)audio.SelectToken("bits_per_sample")))
                    {
                        if (int.TryParse((string)audio.SelectToken("bits_per_sample"), NumberStyles.Any, CultureInfo.InvariantCulture, out int bits_per_sample))
                            result.AudioBitsPerSample = bits_per_sample;
                    }
                    if (!string.IsNullOrEmpty((string)audio.SelectToken("channels")))
                    {
                        if (int.TryParse((string)audio.SelectToken("channels"), NumberStyles.Any, CultureInfo.InvariantCulture, out int channels))
                            result.AudioChannels = channels;
                    }
                }
                if (video != null)
                {
                    result.HasVideo = true;
                    if (int.TryParse((string)video.SelectToken("bit_rate"), out int videoBitrate))
                        result.VideoDataRate = videoBitrate;
                    result.VideoCodec = (string)video.SelectToken("codec_name");
                    if (!string.IsNullOrEmpty((string)video.SelectToken("duration")))
                    {
                        if (double.TryParse((string)video.SelectToken("duration"), NumberStyles.Any, CultureInfo.InvariantCulture, out double durationValue))
                            result.VideoLength = TimeSpan.FromSeconds(durationValue);
                    }
                    if (!string.IsNullOrEmpty((string)video.SelectToken("bits_per_raw_sample")))
                    {
                        if (int.TryParse((string)video.SelectToken("bits_per_raw_sample"), NumberStyles.Any, CultureInfo.InvariantCulture, out int bits_per_raw_sample))
                            result.VideoBitsPerPixel = bits_per_raw_sample;
                    }
                    if (!string.IsNullOrEmpty((string)video.SelectToken("avg_frame_rate")))
                    {
                        var splitFramerate = ((string) video.SelectToken("avg_frame_rate")).Split('/');
                        if (splitFramerate.Length == 2
                            && double.TryParse(splitFramerate[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double first)
                            && double.TryParse(splitFramerate[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double second))
                            result.VideoFrameRate = Math.Round(first / second, 1);
                    }
                    var rotate = (string)video["tags"]?.SelectToken("rotate");
                    double? NullableRotateValue = null;
                    if (!string.IsNullOrEmpty(rotate))
                    {
                        if (double.TryParse(rotate, out double rotateValue))
                            NullableRotateValue = rotateValue;
                    }
                    bool invertWidthHeight = NullableRotateValue.HasValue && NullableRotateValue != 0 && NullableRotateValue.Value % 90 == 0;
                    result.FrameWidth = invertWidthHeight ? int.Parse((string)video.SelectToken("height")) : int.Parse((string)video.SelectToken("width"));
                    result.FrameHeight = invertWidthHeight ? int.Parse((string)video.SelectToken("width")) : int.Parse((string)video.SelectToken("height"));
                }
            }

            return result;
        }
    }
}
