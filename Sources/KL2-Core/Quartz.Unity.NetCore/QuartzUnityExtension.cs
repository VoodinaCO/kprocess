using Microsoft.Practices.Unity;

namespace Quartz.Unity.NetCore
{
    public class QuartzUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container.RegisterType<ISchedulerFactory, UnitySchedulerFactory>(new ContainerControlledLifetimeManager());
        }
    }
}
