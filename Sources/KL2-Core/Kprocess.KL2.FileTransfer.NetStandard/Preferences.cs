using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Kprocess.KL2.FileTransfer
{
    public static class Preferences
    {
        public const string FileServerUriKey = "FileServerUri";

        public const int BufferSize = 81920; // 80KB

        public static string SyncDirectory
        {
            get
            {
                if (Assembly.GetEntryAssembly() == null)
                    return null;
                var syncFilesLocation = ConfigurationManager.AppSettings["SyncPath"];
                if (syncFilesLocation.StartsWith(@".\"))
                    syncFilesLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), syncFilesLocation.TrimStart('.', '\\'));
                if (!new Uri(syncFilesLocation).IsAbsoluteUri)
                    syncFilesLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), syncFilesLocation);
                Directory.CreateDirectory(syncFilesLocation);
                return syncFilesLocation;
            }
        }

        public static void SetSyncFilesLocation(string path, string exePath = null)
        {
            Configuration config = string.IsNullOrEmpty(exePath) ? ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None) : ConfigurationManager.OpenExeConfiguration(exePath);
            config.AppSettings.Settings["SyncPath"].Value = path;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static bool TestSyncDirectoryRights()
        {
            try
            {
                var testFile = Path.Combine(SyncDirectory, "testRights.tmp");
                using (var testFileStream = File.Create(testFile))
                {
                    // Its OK
                }
                File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string FileServerUri { get; set; } = "https://localhost:8082";
    }
}
