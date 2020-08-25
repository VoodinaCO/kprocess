using Kprocess.KL2.FileServer.App_Start;
using Topshelf;

namespace Kprocess.KL2.FileServer
{
    class Program
    {
        static int Main(string[] args)
        {
            var rc = HostFactory.New(x =>
            {
                x.Service<FileServerService>(sc =>
                {
                    sc.ConstructUsing(s => new FileServerService());
                    sc.WhenStarted((s, hostControl) => s.Start(hostControl));
                    sc.WhenStopped((s, hostControl) => s.Stop(hostControl));
                });

                x.RunAsNetworkService();//x.RunAsLocalSystem();
                x.SetDescription(FileServerService.Description);
                x.SetDisplayName(FileServerService.DisplayName);
                x.SetServiceName(FileServerService.ServiceName);

                x.EnableServiceRecovery(recoveryOption => recoveryOption.RestartService(0));
                x.StartAutomaticallyDelayed();
            });

            var exitCode = rc.Run();
            return (int)exitCode;
        }
    }
}
