using KProcess.KL2.SignalRClient.Context;
using KProcess.KL2.SignalRClient.Hub;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace Kprocess.KL2.FileServer.App_Start
{
    public class SignalRResolver : DefaultDependencyResolver
    {
        protected IUnityContainer container;

        public SignalRResolver(IUnityContainer container)
        {
            this.container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public void RegisterHubConnect()
        {
            container.RegisterInstance(GlobalHost.DependencyResolver.Resolve<IConnectionManager>());
            container.RegisterType<IKLPublicationRepository, KLPublicationRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<KLPublicationHub>(new ContainerControlledLifetimeManager(), new InjectionConstructor(container.Resolve<IKLPublicationRepository>()));

        }

        public override object GetService(Type serviceType)
        {
            try
            {
                if (container.IsRegistered(serviceType) || typeof(IHub).IsAssignableFrom(serviceType))
                    return container.Resolve(serviceType);
                return base.GetService(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                if (container.IsRegistered(serviceType) || typeof(IHub).IsAssignableFrom(serviceType))
                    return container.ResolveAll(serviceType);
                return base.GetServices(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var child = container.CreateChildContainer();
            return new UnityResolver(child);
        }

        public new void Dispose()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            container.Dispose();
        }
    }
}
