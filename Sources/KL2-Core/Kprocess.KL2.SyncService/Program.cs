using Kprocess.KL2.SyncService.App_Start;
using Kprocess.KL2.SyncService.Interface;
using Kprocess.KL2.SyncService.Jobs;
using Kprocess.KL2.SyncService.Service;
using Microsoft.Practices.Unity;
using Quartz;
using System;
using System.Configuration;
using Topshelf;
using Topshelf.Quartz;
using Topshelf.Unity.NetCore;

namespace Kprocess.KL2.SyncService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Registering dependencies ...");
            var container = UnityResolver.RegisterServices(new UnityContainer());
            int syncInterval = Convert.ToInt16(ConfigurationManager.AppSettings["SyncInterval"]);

            HostFactory.Run(x =>
            {
                x.UseUnityContainer(container);
                x.UsingQuartzJobFactory(() => new UnityJobFactory(container));
                x.Service<SyncingJobService>(s =>
                {
                    s.ConstructUsingUnityContainer();
                    s.WhenStarted(service => service.OnStart());
                    s.WhenStopped(service => service.OnStop());

                    s.ScheduleQuartzJob(q =>
                        q.WithJob(() =>
                            JobBuilder.Create<SyncingJob>().Build())
                        .AddTrigger(() =>
                            TriggerBuilder.Create()
                                .WithSimpleSchedule(b => b.WithIntervalInMinutes(syncInterval).RepeatForever()).Build())
                    );
                });

                x.RunAsLocalSystem()
                .DependsOnEventLog()
                .StartAutomatically()
                .EnableServiceRecovery(rc => rc.RestartService(0));

                x.SetServiceName(SyncingJobService.ServiceName);
                x.SetDisplayName(SyncingJobService.DisplayName);
                x.SetDescription(SyncingJobService.Description);
            });
        }
    }
}
