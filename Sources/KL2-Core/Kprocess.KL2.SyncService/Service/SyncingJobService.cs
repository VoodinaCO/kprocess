using System;
using System.IO;
using System.Reflection;

namespace Kprocess.KL2.SyncService.Service
{
    class SyncingJobService
    {
        public const string ServiceName = "KL2 FS sync service";
        public const string DisplayName = "KL2 FS sync service";
        public const string Description = "KL2 FS syncing job service";

        FileSystemWatcher configWatcher;

        public void OnStart()
        {
            configWatcher = new FileSystemWatcher(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Kprocess.KL2.SyncService.exe.config");
            configWatcher.Changed += (sender, e) =>
            {
                Environment.Exit(1);
            };
            configWatcher.EnableRaisingEvents = true;
        }
        public void OnStop()
        {
        }
    }
}
