using KProcess.KL2.JWT;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Security;
using KProcess.Ksmed.Security;
using System.Globalization;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace KProcess.KL2.API.Authentication
{
    /// <summary>
    /// Custom WEB API Action filter
    /// Executed before each action method to setup user context
    /// </summary>
    public class SettingUserContextFilter : ActionFilterAttribute
    { 
        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (!(actionContext.Request.GetDependencyScope().GetService(typeof(ISecurityContext)) is ISecurityContext securityContext))
                return Task.CompletedTask;

            var principal = actionContext.RequestContext.Principal;
            var identity = principal?.Identity as ClaimsIdentity;
            string username = identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Task.CompletedTask;

            if (actionContext.Request.Headers.Authorization == null)
                return Task.CompletedTask;
            var usermodel = JwtTokenProvider.GetUserModel(actionContext.Request.Headers.Authorization.Parameter);
            securityContext.CurrentUser = new SecurityUser(User.FromUserModel(usermodel));
            if (actionContext.Request.GetDependencyScope().GetService(typeof(ILocalizationManager)) is ILocalizationManager localizationManager)
                localizationManager.CurrentCulture = new CultureInfo(usermodel.CurrentLanguageCode ?? usermodel.DefaultLanguageCode);

            return Task.CompletedTask;
        }

    }
}
