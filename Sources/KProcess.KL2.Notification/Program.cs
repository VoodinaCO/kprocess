using Kprocess.KL2.Notification.App_Start;
using KProcess.KL2.Notification.Jobs;
using KProcess.KL2.Notification.Service;
using KProcess.Supervision.Log4net;
using Microsoft.Win32;
using Quartz;
using Quartz.Unity.NetCore;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Topshelf;
using Topshelf.Quartz;
using Topshelf.Unity.NetCore;

namespace KProcess.KL2.Notification
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                var traceManager = new Log4netTraceManager(new Log4netWrapper());

                // If in docker container, edit the config file
                if (IsInContainer())
                {
                    traceManager.TraceDebug("Edit dynamically config file from docker container");
                    var dataSource = Environment.GetEnvironmentVariable("DATASOURCE");
                    var database = Environment.GetEnvironmentVariable("DATABASE");
                    var fileServerScheme = Environment.GetEnvironmentVariable("FILESERVERSCHEME");
                    var fileServer = Environment.GetEnvironmentVariable("FILESERVER");
                    var fileServerPort = Environment.GetEnvironmentVariable("FILESERVERPORT");
                    var sendNotificationInterval = Environment.GetEnvironmentVariable("SENDNOTIFICATIONINTERVAL");

                    traceManager.TraceDebug($"DATASOURCE : {dataSource}");
                    traceManager.TraceDebug($"DATABASE : {database}");
                    traceManager.TraceDebug($"FILESERVERSCHEME : {fileServerScheme}");
                    traceManager.TraceDebug($"FILESERVER : {fileServer}");
                    traceManager.TraceDebug($"FILESERVERPORT : {fileServerPort}");
                    traceManager.TraceDebug($"SENDNOTIFICATIONINTERVAL : {sendNotificationInterval}");

                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "kl2suitenotificationconfig.exe");
                        process.StartInfo.Arguments = $"-ConfigFile KProcess.KL2.Notification.exe.config -DataSource {dataSource} -Database {database} -FileServerScheme {fileServerScheme} -FileServer {fileServer} -FileServerPort {fileServerPort} -SendNotificationInterval {sendNotificationInterval}";
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

                Console.WriteLine("Registering dependencies ...");
                traceManager.TraceDebug("Registering dependencies ...");
                var container = UnityResolver.RegisterServices(IoC.Container);
                int emailInterval = Convert.ToInt16(ConfigurationManager.AppSettings["SendNotificationInterval"]);

                x.UseUnityContainer(container);
                x.UsingQuartzJobFactory(() => new UnityJobFactory(container));
                x.Service<NotificationJobService>(s =>
                {
                    s.ConstructUsingUnityContainer();
                    s.WhenStarted(service => service.OnStart());
                    s.WhenStopped(service => service.OnStop());

                    s.ScheduleQuartzJob(q =>
                        q.WithJob(() =>
                            JobBuilder.Create<SendEmailsJob>().Build())
                        .AddTrigger(() =>
                            TriggerBuilder.Create()
                                .WithSimpleSchedule(b => b.WithIntervalInMinutes(emailInterval).RepeatForever()).Build())
                    );

                    s.ScheduleQuartzJob(q =>
                        q.WithJob(() =>
                            JobBuilder.Create<CollectReportsJob>().Build())
                        .AddTrigger(() =>
                            TriggerBuilder.Create()
                                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(07, 00).InTimeZone(TimeZoneInfo.Local)).Build())
                                //.WithSimpleSchedule(b => b.WithIntervalInMinutes(30).RepeatForever()).Build())
                    );

                    //s.ScheduleQuartzJob(q =>
                    //    q.WithJob(() =>
                    //        JobBuilder.Create<CreateEmailJob>().Build())
                    //    .AddTrigger(() =>
                    //        TriggerBuilder.Create()
                    //            //.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(15, 37).InTimeZone(TimeZoneInfo.Local)).Build())
                    //            .WithSimpleSchedule(b => b.WithIntervalInMinutes(3).RepeatForever()).Build())
                    //);
                });

                x.RunAsLocalSystem()
                    .DependsOnEventLog()
                    .StartAutomatically()
                    .EnableServiceRecovery(rc => rc.RestartService(0));

                x.SetServiceName(NotificationJobService.ServiceName);
                x.SetDisplayName(NotificationJobService.DisplayName);
                x.SetDescription(NotificationJobService.Description);

                // TEST daily inspections reporting
                /*var job = container.Resolve<CollectReportsJob>();
                job.Execute(null);*/
            });
        }

        public static bool IsInContainer()
        {
            var baseKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control");
            return baseKey.GetValueNames().Contains("ContainerType");
        }
    }
}
