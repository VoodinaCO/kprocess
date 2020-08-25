using Kprocess.KL2.FilesSyncService.Service;
using System;
using System.Configuration;
using Topshelf;

namespace Kprocess.KL2.FilesSyncService
{
    class Program
    {
        static int Main(string[] args)
        {
            int syncInterval = Convert.ToInt16(ConfigurationManager.AppSettings["SyncInterval"]);

            var rc = HostFactory.New(x =>
            {
                x.Service<SyncingJobService>(sc =>
                {
                    sc.ConstructUsing(s => new SyncingJobService());
                    sc.WhenStarted((s, hostControl) => s.Start(hostControl));
                    sc.WhenStopped((s, hostControl) => s.Stop(hostControl));
                });

                x.RunAsLocalSystem()
                    .DependsOnEventLog()
                    .StartAutomatically()
                    .EnableServiceRecovery(src => src.RestartService(1));

                x.SetServiceName("KL2 Files sync service");
                x.SetDisplayName("KL2 Files sync service");
                x.SetDescription("KL2 Files syncing job service");
            });

            TopshelfExitCode exitCode = TopshelfExitCode.AbnormalExit;
            try
            {
                exitCode = rc.Run();
            }
            catch (Exception e)
            {
                //Trace.TraceError(e, "ERROR on service starting.");
            }
            return (int)exitCode;
        }
    }
}
