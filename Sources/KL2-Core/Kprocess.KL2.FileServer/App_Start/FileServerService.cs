using KProcess;
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

namespace Kprocess.KL2.FileServer.App_Start
{
    public class FileServerService : ServiceControl
    {
        public const string ServiceName = "KL2 File Server";
        public const string DisplayName = "KL2 File Server";
        public const string Description = "KL2 File Server Windows service";

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
                var fileProvider = Environment.GetEnvironmentVariable("FILEPROVIDER");
                var sFtp_Server = Environment.GetEnvironmentVariable("SFTP_SERVER");
                var sFtp_Port = Environment.GetEnvironmentVariable("SFTP_PORT");
                var sFtp_User = Environment.GetEnvironmentVariable("SFTP_USER");
                var sFtp_Password = Environment.GetEnvironmentVariable("SFTP_PASSWORD");
                var sFtp_PublishedFilesDirectory = Environment.GetEnvironmentVariable("SFTP_PUBLISHEDFILESDIRECTORY");
                var sFtp_UploadedFilesDirectory = Environment.GetEnvironmentVariable("SFTP_UPLOADEDFILESDIRECTORY");
                var local_PublishedFilesDirectory = Environment.GetEnvironmentVariable("LOCAL_PUBLISHEDFILESDIRECTORY");
                var local_UploadedFilesDirectory = Environment.GetEnvironmentVariable("LOCAL_UPLOADEDFILESDIRECTORY");

                traceManager.TraceDebug($"DATASOURCE : {dataSource}");
                traceManager.TraceDebug($"DATABASE : {database}");
                traceManager.TraceDebug($"APPLICATIONSCHEME : {applicationScheme}");
                traceManager.TraceDebug($"APPLICATIONPORT : {applicationPort}");
                traceManager.TraceDebug($"FILEPROVIDER : {fileProvider}");
                traceManager.TraceDebug($"SFTP_SERVER : {sFtp_Server}");
                traceManager.TraceDebug($"SFTP_PORT : {sFtp_Port}");
                traceManager.TraceDebug($"SFTP_USER : {sFtp_User}");
                traceManager.TraceDebug($"SFTP_PASSWORD : {sFtp_Password}");
                traceManager.TraceDebug($"SFTP_PUBLISHEDFILESDIRECTORY : {sFtp_PublishedFilesDirectory}");
                traceManager.TraceDebug($"SFTP_UPLOADEDFILESDIRECTORY : {sFtp_UploadedFilesDirectory}");
                traceManager.TraceDebug($"LOCAL_PUBLISHEDFILESDIRECTORY : {local_PublishedFilesDirectory}");
                traceManager.TraceDebug($"LOCAL_UPLOADEDFILESDIRECTORY : {local_UploadedFilesDirectory}");

                using (var process = new Process())
                {
                    process.StartInfo.FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "kl2suitefileserverconfig.exe");
                    process.StartInfo.Arguments = $"-ConfigFile Kprocess.KL2.FileServer.exe.config -DataSource {dataSource} -Database {database} -ApplicationScheme {applicationScheme} -ApplicationPort {applicationPort} -FileProvider {fileProvider} -SFtp_Server {sFtp_Server} -SFtp_Port {sFtp_Port} -SFtp_User {sFtp_User} -SFtp_Password {sFtp_Password} -SFtp_PublishedFilesDirectory {sFtp_PublishedFilesDirectory} -SFtp_UploadedFilesDirectory {sFtp_UploadedFilesDirectory} -Local_PublishedFilesDirectory {local_PublishedFilesDirectory} -Local_UploadedFilesDirectory {local_UploadedFilesDirectory}";
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

            configWatcher = new FileSystemWatcher(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Kprocess.KL2.FileServer.exe.config");
            configWatcher.Changed += (sender, e) =>
            {
                Environment.Exit(1);
            };
            configWatcher.EnableRaisingEvents = true;

            traceManager.TraceDebug("Starting the service");

            string url = ConfigurationManager.AppSettings["ApplicationUrl"];
            if (string.IsNullOrEmpty(url))
                url = "http://*:8082";

            _webApplication = WebApp.Start<OwinConfiguration>(url);

            Console.WriteLine($"{ServiceName} has been started on {url}.");
            traceManager.TraceDebug($"{ServiceName} has been started on {url}.");

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
