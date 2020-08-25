using KProcess.KL2.WebAdmin.Models.Documentation;
using KProcess.KL2.WebAdmin.Serialization;
using KProcess.Ksmed.Models;
using KProcess.Supervision.Log4net;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace KProcess.KL2.WebAdmin
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            // Setup Log4Net configuration (in App.config)
            log4net.Config.XmlConfigurator.Configure();
            ITraceManager traceManager = new Log4netTraceManager(new Log4netWrapper());

            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTA3NjkwQDMxMzcyZTMxMmUzMGRpNUF0QThQTklxRFlMeDhXR3NRZFlHcnU4VjN5Z2tGS3FaT2sxM1hER1E9;MTA3NjkxQDMxMzcyZTMxMmUzMFVqQ0tFMGcxclFlNUVKVUJtdWpGcWlmQ1gvbE9PMGxZYWxHTFZYSmxHZDQ9;MTA3NjkyQDMxMzcyZTMxMmUzMEZMTENrQjNCYjUvNmtKTHFNRHBGRkxXVlFXNGZReDVrbWl2WXZsczFORlk9;MTA3NjkzQDMxMzcyZTMxMmUzME5MY3VCaXEydmZyS085RmhzdWtMK2pjNE4wQmY1RUZxMHorMHhHZGlpOFU9;MTA3Njk0QDMxMzcyZTMxMmUzMERPVCtOWHBlT0NZVkM5aExMOUhtL3JMdDFyMWcySEl1cGdlZjBubm1wQW89;MTA3Njk1QDMxMzcyZTMxMmUzMFM5YmYyTDhoL1N4Z2RIeEZ4S1p6KzF5eGxkdkp2VDNIMFpkZHBwYWVndFE9;MTA3Njk2QDMxMzcyZTMxMmUzMEtFZTNrcnBaelBHUXg4TC9kdGE0TjVQTHNIVVloTDZ3N1I4VHQ0NysvTEE9;MTA3Njk3QDMxMzcyZTMxMmUzMExlamxrNmhmL0RyR3Y2QzBoYitxOXhyazRnR0JZUUJBZThJZmhENG1oVGs9;MTA3Njk4QDMxMzcyZTMxMmUzMGRrTFo4clJIR29kZEZrdlRSUlFsQ3VvckJGRjNiZCtscFNYbE0zRThQSGc9;MTA3Njk5QDMxMzcyZTMxMmUzMERPVCtOWHBlT0NZVkM5aExMOUhtL3JMdDFyMWcySEl1cGdlZjBubm1wQW89");

            AreaRegistration.RegisterAllAreas();
            UnityMvcActivator.Start();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            // Refresh the file caches with all the files present in the folder ~/Files
            //APIHttpClient.RefreshDownloadedFiles(Server.MapPath("~/Files"));

            ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
            ValueProviderFactories.Factories.Add(new MyJsonValueProviderFactory());

            ModelBinders.Binders[typeof(DocumentationDraft)] = new GenericModelBinder<DocumentationDraft>();
            ModelBinders.Binders[typeof(UpdateDispositionDocumentationDraftModel)] = new GenericModelBinder<UpdateDispositionDocumentationDraftModel>();
            ModelBinders.Binders[typeof(SaveDocumentationDraftActionsModel)] = new GenericModelBinder<SaveDocumentationDraftActionsModel>();
            ModelBinders.Binders[typeof(UpdateActionDispositionDocumentationDraftModel)] = new GenericModelBinder<UpdateActionDispositionDocumentationDraftModel>();
            ModelBinders.Binders[typeof(UpdateVideoDocumentationDraftModel)] = new GenericModelBinder<UpdateVideoDocumentationDraftModel>();
            ModelBinders.Binders[typeof(UpdateReleaseNotesDocumentationDraftModel)] = new GenericModelBinder<UpdateReleaseNotesDocumentationDraftModel>();
            ModelBinders.Binders[typeof(GetActionModel)] = new GenericModelBinder<GetActionModel>();

            try
            {
                traceManager.TraceInfo($"Webadmin is starting with folder ~/Files={Server.MapPath("~/Files")}");
            }
            catch(Exception ) { }
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Allow https pages in debugging
            /*if (Request.IsLocal)
            {
                if (Request.Url.Scheme == "http")
                {
                    int localSslPort = 44327; // Your local IIS port for HTTPS

                    var path = "https://" + Request.Url.Host + ":" + localSslPort + Request.Url.PathAndQuery;

                    Response.Status = "301 Moved Permanently";
                    Response.AddHeader("Location", path);
                }
            }
            else
            {
                switch (Request.Url.Scheme)
                {
                    case "https":
                        Response.AddHeader("Strict-Transport-Security", "max-age=31536000");
                        break;
                    case "http":
                        var path = "https://" + Request.Url.Host + Request.Url.PathAndQuery;
                        Response.Status = "301 Moved Permanently";
                        Response.AddHeader("Location", path);
                        break;
                }
            }*/
        }

        public bool IsInContainer()
        {
            var baseKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control");
            return baseKey.GetValueNames().Contains("ContainerType");
        }
    }
}
