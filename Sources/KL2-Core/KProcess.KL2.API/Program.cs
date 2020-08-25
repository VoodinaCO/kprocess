using System;
using KProcess.KL2.API.App_Start;
using KProcess.Supervision.Log4net;
using Topshelf;

namespace KProcess.KL2.API
{
    class Program
    {
        public static int Main(string[] args)
        {
            ITraceManager traceManager = new Log4netTraceManager(new Log4netWrapper());

            var rc = HostFactory.New(x =>
            {
                x.Service<ApiService>(sc =>
                {
                    sc.ConstructUsing(s => new ApiService());
                    sc.WhenStarted((s, hostControl) => s.Start(hostControl));
                    sc.WhenStopped((s, hostControl) => s.Stop(hostControl));
                });

                x.RunAsLocalSystem();
                x.SetDescription(ApiService.Description);
                x.SetDisplayName(ApiService.DisplayName);
                x.SetServiceName(ApiService.ServiceName);

                x.EnableServiceRecovery(recoveryOption => recoveryOption.RestartService(0));
                x.StartAutomaticallyDelayed();
            });

            TopshelfExitCode exitCode = TopshelfExitCode.AbnormalExit;
            try
            {
                exitCode = rc.Run();
            }
            catch (Exception e)
            {
                traceManager.TraceError(e, "ERROR on service starting.");
            }
            return (int)exitCode;
        }
    }
}
