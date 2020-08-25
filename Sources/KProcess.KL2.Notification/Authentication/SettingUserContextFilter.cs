using KProcess;
using KProcess.Ksmed.Models.Security;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Business;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Kprocess.KL2.Notification.Authentication
{
    /// <summary>
    /// Custom WEB API Action filter
    /// Executed before each action method to setup user context
    /// </summary>
    public class SettingUserContextFilter : ActionFilterAttribute
    {
        public SettingUserContextFilter()
        {
        }
 
        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (actionContext.Request.GetDependencyScope().GetService(typeof(ISecurityContext)) is ISecurityContext securityContext)
            {
                IPrincipal principal = actionContext.RequestContext.Principal;
                ClaimsIdentity identity = principal?.Identity as ClaimsIdentity;
                string username = identity?.Name;
                if (string.IsNullOrEmpty(username))
                    return;

                var authenticationService = actionContext.Request.GetDependencyScope().GetService(typeof(IAuthenticationService)) as IAuthenticationService;
                var traceManager = actionContext.Request.GetDependencyScope().GetService(typeof(ITraceManager)) as ITraceManager;

                KProcess.Ksmed.Models.User user = await authenticationService.GetUser(username);
                securityContext.CurrentUser = new SecurityUser(user);
            }
        }
    }
}
