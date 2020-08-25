using KProcess.Supervision.Log4net;
using Microsoft.Owin.Hosting;
using Microsoft.Win32;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Topshelf;

namespace KProcess.KL2.API.App_Start
{
    public class ApiService : ServiceControl
    {
        public const string ServiceName = "KL2 API";
        public const string DisplayName = "KL2 API";
        public const string Description = "KL2 API Windows service";

        IDisposable _webApplication;
        ITraceManager traceManager;
        FileSystemWatcher configWatcher;

        public bool Start(HostControl hostControl)
        {
            traceManager = new Log4netTraceManager(new Log4netWrapper());

            // If in docker container, edit the config file
            if (IsInContainer())
            {
                traceManager.TraceDebug("Edit dynamically config file from docker container");
                var dataSource = Environment.GetEnvironmentVariable("DATASOURCE");
                var database = Environment.GetEnvironmentVariable("DATABASE");
                var applicationScheme = Environment.GetEnvironmentVariable("APPLICATIONSCHEME");
                var applicationPort = Environment.GetEnvironmentVariable("APPLICATIONPORT");
                var fileServerScheme = Environment.GetEnvironmentVariable("FILESERVERSCHEME");
                var fileServer = Environment.GetEnvironmentVariable("FILESERVER");
                var fileServerPort = Environment.GetEnvironmentVariable("FILESERVERPORT");

                traceManager.TraceDebug($"DATASOURCE : {dataSource}");
                traceManager.TraceDebug($"DATABASE : {database}");
                traceManager.TraceDebug($"APPLICATIONSCHEME : {applicationScheme}");
                traceManager.TraceDebug($"APPLICATIONPORT : {applicationPort}");
                traceManager.TraceDebug($"FILESERVERSCHEME : {fileServerScheme}");
                traceManager.TraceDebug($"FILESERVER : {fileServer}");
                traceManager.TraceDebug($"FILESERVERPORT : {fileServerPort}");

                using (var process = new Process())
                {
                    process.StartInfo.FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "kl2suiteapiconfig.exe");
                    process.StartInfo.Arguments = $"-ConfigFile KProcess.KL2.API.exe.config -DataSource {dataSource} -Database {database} -ApplicationScheme {applicationScheme} -ApplicationPort {applicationPort} -FileServerScheme {fileServerScheme} -FileServer {fileServer} -FileServerPort {fileServerPort}";
                    try
                    {
                        process.Start();
                        process.WaitForExit();
                        traceManager.TraceError("Config file correctly updated.");
                    }
                    catch
                    {
                        traceManager.TraceError("An issue has occured during config edition.");
                    }
                }
            }

            var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "KProcess.KL2.API.exe.config");
            configWatcher = new FileSystemWatcher(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "KProcess.KL2.API.exe.config");
            configWatcher.Changed += (sender, e) =>
            {
                Environment.Exit(1);
            };
            configWatcher.EnableRaisingEvents = true;

            traceManager.TraceDebug("Starting the service");

            string url = ConfigurationManager.AppSettings["ApplicationUrl"];
            if (string.IsNullOrEmpty(url))
                url = "http://*:8081";

            _webApplication = WebApp.Start<OwinConfiguration>(url);

            Console.WriteLine($"{ServiceName} has been started on {url}.");
            Console.WriteLine($"FileServerUri : {ConfigurationManager.AppSettings["FileServerUri"]}");
            traceManager.TraceDebug($"{ServiceName} has been started on {url}.");
            traceManager.TraceDebug($"FileServerUri : {ConfigurationManager.AppSettings["FileServerUri"]}");

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            traceManager.TraceDebug($"{ServiceName} has been stopped.");
            return true;
        }

        public bool IsInContainer()
        {
            var baseKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control");
            return baseKey.GetValueNames().Contains("ContainerType");
        }
    }
}
