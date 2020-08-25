using System.Web.Mvc;
using System.Web.Routing;

namespace KProcess.KL2.WebAdmin
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Action",
                url: "action/{id}",
                defaults: new { controller = "Action", action = "Details" },
                constraints: new { id = @"\d+" }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Empty", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
