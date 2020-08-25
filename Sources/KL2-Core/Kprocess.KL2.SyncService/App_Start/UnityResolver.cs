using Kprocess.KL2.SyncService.Jobs;
using Kprocess.KL2.SyncService.Service;
using KProcess;
using KProcess.KL2.APIClient;
using KProcess.KL2.Business.Impl.API;
using KProcess.Ksmed.Business;
using KProcess.Supervision.Log4net;
using Microsoft.Practices.Unity;

namespace Kprocess.KL2.SyncService.App_Start
{
    public class UnityResolver
    {
        public static UnityContainer RegisterServices(UnityContainer container)
        {
            container.RegisterType<IApplicationUsersService, ApplicationUsersService>(new HierarchicalLifetimeManager());
            container.RegisterType<IPrepareService, PrepareService>(new HierarchicalLifetimeManager());

            // Logging
            container.RegisterType<ITraceManager, Log4netTraceManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITraceWrapper, Log4netWrapper>(new ContainerControlledLifetimeManager());

            // API Client
            container.RegisterType<IAPIHttpClient, APIHttpClient>(new ContainerControlledLifetimeManager());

            // Jobs and service
            container.RegisterType<SyncingJobService>();
            container.RegisterType<SyncingJob>();

            return container;
        }
    }
}
