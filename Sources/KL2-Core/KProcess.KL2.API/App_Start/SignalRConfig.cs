using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity;

namespace KProcess.KL2.API.App_Start
{
    public static class SignalRConfig
    {
        public static HubConfiguration Register(IUnityContainer container)
        {
            var signalRResolver = new SignalRResolver(container);
            var hubConfig = new HubConfiguration()
            {
                EnableJSONP = true,
                EnableDetailedErrors = true,
                Resolver = signalRResolver
            };

            GlobalHost.DependencyResolver = hubConfig.Resolver;
            signalRResolver.RegisterHubConnect();

            return hubConfig;
        }
    }
}
