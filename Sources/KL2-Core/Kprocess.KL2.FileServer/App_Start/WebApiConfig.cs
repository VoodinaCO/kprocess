using KProcess;
using KProcess.KL2.APIClient;
using Microsoft.Practices.Unity;
using System.Linq;
using System.Web.Http;

namespace Kprocess.KL2.FileServer.App_Start
{
    public static class WebApiConfig
    {
        public static HttpConfiguration Register(IUnityContainer container)
        {
            var config = new HttpConfiguration();

            // Dependency injection settings
            var resolver = new UnityResolver(container);
            resolver.RegisterServices();
            resolver.RegisterControllers();
            config.DependencyResolver = resolver;

            // Returns JSON by default
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            // Exception handler
            config.Filters.Add(new NotImplExceptionFilterAttribute());

            // HttpMessage handlers
            var traceManager = container.Resolve<ITraceManager>();
            var fileProvider = container.Resolve<IFileProvider>();
            config.MessageHandlers.Add(new MethodOverrideHandler(traceManager));
            config.MessageHandlers.Add(new SimpleBITSRequestHandler(fileProvider, traceManager));

            // Routing
            // Routing is done using Attribute inside controller instead of default route
            config.MapHttpAttributeRoutes();

            return config;
        }
    }
}
