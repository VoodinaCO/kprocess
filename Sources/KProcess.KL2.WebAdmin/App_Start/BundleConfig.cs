using System.Web;
using System.Web.Optimization;

namespace KProcess.KL2.WebAdmin
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Css").Include(
                "~/Content/Site.css",
                "~/Content/twitter-bootstrap/bootstrap.min.css"));

            bundles.Add(new ScriptBundle("~/Js").Include(
                "~/Scripts/docready.min.js",
                "~/Content/jquery/jquery.min.js",
                "~/Content/twitter-bootstrap/bootstrap.min.js",
                "~/Content/jquery-easing/jquery.easing.min.js",
                "~/Scripts/element-closest.min.js",
                "~/Scripts/tus.min.js"));
        }
    }
}
