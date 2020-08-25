using KProcess.Supervision.Log4net;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace KProcess.KL2.Notification.Service
{
    public class NotificationJobService
    {
        public const string ServiceName = "KL2 Notification";
        public const string DisplayName = "KL2 Notification";
        public const string Description = "KL2 Notification Windows service";

        ITraceManager traceManager;
        FileSystemWatcher configWatcher;

        public void OnStart()
        {
            traceManager = new Log4netTraceManager(new Log4netWrapper());

            configWatcher = new FileSystemWatcher(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "KProcess.KL2.Notification.exe.config");
            configWatcher.Changed += (sender, e) =>
            {
                Environment.Exit(1);
            };
            configWatcher.EnableRaisingEvents = true;

            Trace.WriteLine("Starting the service");

            Console.WriteLine($"{ServiceName} has been started.");
            traceManager.TraceDebug($"{ServiceName} has been started.");
        }

        public void OnStop()
        {
            traceManager.TraceDebug($"{ServiceName} has been stopped.");
        }
    }
}
