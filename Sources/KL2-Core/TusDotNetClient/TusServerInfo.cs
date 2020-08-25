﻿namespace TusDotNetClient
{
    public class TusServerInfo
    {
        public string Version { get; }
        public string SupportedVersions { get; }
        public string Extensions { get; }
        public long MaxSize { get; }

        public bool SupportsDelete => Extensions.Contains("termination");

        public TusServerInfo(string version, string supportedVersions, string extensions, long? maxSize)
        {
            Version = version ?? "";
            SupportedVersions = supportedVersions ?? "";
            Extensions = extensions ?? "";
            MaxSize = maxSize ?? 0;
        }
    }
}
