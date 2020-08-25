using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using System.Linq;
using System.Web.Http;

namespace KProcess.KL2.API.App_Start
{
    public static class WebApiConfig
    {
        public static HttpConfiguration Register(IUnityContainer container)
        {
            var config = new HttpConfiguration();

            var resolver = new UnityResolver(container);
            resolver.RegisterServices();
            container.RegisterInstance<IServiceBus>(resolver);
            resolver.RegisterControllers();

            config.DependencyResolver = resolver;

            // Returns JSON by default
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            // Routing
            // Routing is done using Attribute inside controller instead of default route
            config.MapHttpAttributeRoutes();

            config.Formatters.JsonFormatter.SerializerSettings.TypeNameHandling = TypeNameHandling.All;

            // Exception handler
            config.Filters.Add(new NotImplExceptionFilterAttribute());

            return config;
        }
    }
}
