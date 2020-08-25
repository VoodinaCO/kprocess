using AForge.Video.DirectShow;
using KProcess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Kprocess.KL2.TabletClient.Models
{
    public class FFMEWebcam
    {
        const string _webcamVideoSize = "WebcamVideoSize";
        const string _webcam = "Webcam";

        readonly ITraceManager _traceManager;

        public FFMEWebcam(ITraceManager traceManager)
        {
            _traceManager = traceManager;

            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            CaptureDevices = new List<CaptureDeviceInfo>();
            foreach (FilterInfo filterInfo in videoDevices)
            {
                var device = new CaptureDeviceInfo(filterInfo);
                if (device.SupportedVideoResolutions.Any())
                    CaptureDevices.Add(device);
            }
            CaptureDevices.Sort();
        }

        public List<CaptureDeviceInfo> CaptureDevices { get; private set; }

        public Uri InputUriSource =>
            string.IsNullOrEmpty(InputSource) ? null : new Uri($"device://dshow/?video={InputSource}");

        public string InputSource
        {
            get => ConfigurationManager.AppSettings.AllKeys.Contains(_webcam) ? ConfigurationManager.AppSettings[_webcam] : null;
            set
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[_webcam] == null)
                    settings.Add(_webcam, value);
                else
                    settings[_webcam].Value = value;
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
        }

        public string InputVideoSize
        {
            get => ConfigurationManager.AppSettings.AllKeys.Contains(_webcamVideoSize) ? ConfigurationManager.AppSettings[_webcamVideoSize] : null;
            set
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[_webcamVideoSize] == null)
                    settings.Add(_webcamVideoSize, value);
                else
                    settings[_webcamVideoSize].Value = value;
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
        }

        public class ResolutionInfo : IComparable<ResolutionInfo>
        {
            public int Width { get; private set; }
            public int Height { get; private set; }
            public int MaximumFrameRate { get; private set; }

            public ResolutionInfo(VideoCapabilities videoCapabilities)
            {
                Width = videoCapabilities.FrameSize.Width;
                Height = videoCapabilities.FrameSize.Height;
                MaximumFrameRate = videoCapabilities.MaximumFrameRate;
            }

            public int CompareTo(ResolutionInfo other)
            {
                if (this == null && other == null)
                    return 0;
                if (this == null)
                    return -1;
                if (other == null)
                    return 1;

                int result1 = Width * Height;
                int result2 = other.Width * other.Height;

                if (result1 == result2)
                    return Width > other.Width ? 1 : -1;
                return result1 > result2 ? 1 : -1;

                throw new NotSupportedException();
            }

            public override string ToString() =>
                $"{Width}x{Height} {MaximumFrameRate}fps";

            public string ToString(bool withFps) =>
                $"{Width}x{Height}{(withFps ? $" {MaximumFrameRate}fps" : string.Empty)}";
        }

        public class CaptureDeviceInfo : IComparable<CaptureDeviceInfo>
        {
            public string Name { get; private set; }
            public string MonikerString { get; private set; }
            public List<ResolutionInfo> SupportedSnapshotResolutions { get; private set; } = new List<ResolutionInfo>();
            public List<ResolutionInfo> SupportedVideoResolutions { get; private set; } = new List<ResolutionInfo>();

            public CaptureDeviceInfo(FilterInfo filterInfo)
            {
                Name = filterInfo.Name;
                MonikerString = filterInfo.MonikerString;
                GetSupportedResolutions(filterInfo, SupportedSnapshotResolutions, SupportedVideoResolutions);
            }

            public int CompareTo(CaptureDeviceInfo other) =>
                Name.CompareTo(other.Name); 

            public override string ToString() =>
                Name;

            void GetSupportedResolutions(FilterInfo filterInfo, List<ResolutionInfo> snapshotResolutions, List<ResolutionInfo> videoResolutions)
            {
                snapshotResolutions.Clear();
                videoResolutions.Clear();

                VideoCaptureDevice videoSource = new VideoCaptureDevice(filterInfo.MonikerString);

                // SnapshotResolutions
                foreach (VideoCapabilities videoCapability in videoSource.SnapshotCapabilities)
                {
                    ResolutionInfo resolution = new ResolutionInfo(videoCapability);
                    if (!snapshotResolutions.Contains(resolution))
                        snapshotResolutions.Add(resolution);
                }

                // VideoResolutions
                foreach (VideoCapabilities videoCapability in videoSource.VideoCapabilities)
                {
                    ResolutionInfo resolution = new ResolutionInfo(videoCapability);
                    if (!videoResolutions.Contains(resolution))
                        videoResolutions.Add(resolution);
                }

                snapshotResolutions.Sort();
                videoResolutions.Sort();
            }

            public string MaxVideoResolutionString =>
                SupportedVideoResolutions.LastOrDefault()?.ToString(false);
        }
    }
}
