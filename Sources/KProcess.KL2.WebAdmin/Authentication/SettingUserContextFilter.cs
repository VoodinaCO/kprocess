using KProcess.KL2.JWT;
using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Mapper;
using KProcess.KL2.WebAdmin.Utils;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Security;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Business;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KProcess.KL2.WebAdmin.Authentication
{
    /// <summary>
    /// Custom WEB API Action filter
    /// Executed before each action method to setup user context
    /// </summary>
    public class SettingUserContextFilter : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ITraceManager traceManager = DependencyResolver.Current.GetService(typeof(ITraceManager)) as ITraceManager;

            try
            {
                IAuthenticationService authenticationService = DependencyResolver.Current.GetService(typeof(IAuthenticationService)) as IAuthenticationService;
                ISecurityContext securityContext = DependencyResolver.Current.GetService(typeof(ISecurityContext)) as ISecurityContext;
                ILocalizationManager localizationManager = DependencyResolver.Current.GetService(typeof(ILocalizationManager)) as ILocalizationManager;

                if (securityContext == null)
                {
                    traceManager.TraceWarning($"Security context can be retrieve from Unity. Skipping ..");
                    return;
                }

                /*IPrincipal principal = filterContext.HttpContext.User;
                ClaimsIdentity identity = principal?.Identity as ClaimsIdentity;
                string username = identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    traceManager.TraceWarning($"Claim username not found. Skipping ..");
                    return;
                }*/

                var user = JwtTokenProvider.GetUserModel(filterContext.HttpContext.Request.Cookies["token"].Value);
                securityContext.CurrentUser = new SecurityUser(new User
                {
                    Username = user.Username,
                    Firstname = user.Firstname,
                    Name = user.Name,
                    RoleCodes = user.Roles.ToList()
                });
                localizationManager.CurrentCulture = new CultureInfo(JwtTokenProvider.GetUserModelCurrentLanguage(filterContext.HttpContext.Request.Cookies["token"].Value));

                filterContext.Controller.ViewBag.LicenseExpired = Task.Run(() => LicenseMapper.CheckLicenseIsExpired()).Result;

                //Refresh cookie if not idle
                filterContext.HttpContext.Response.SetAuthorizationCookie(user);

                //traceManager.TraceDebug($"Current user set to {username} inside security context");

                base.OnActionExecuting(filterContext);
            }
            catch(Exception ex)
            {
                traceManager.TraceError(ex,  $"Error while setting action context...");
            }
        }
    }
}