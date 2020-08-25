using KProcess.KL2.JWT;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KProcess.KL2.WebAdmin.Identity
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedroles;

        public CustomAuthorizeAttribute(params string[] roles)
        {
            allowedroles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var cookie = httpContext.Request.Cookies.AllKeys.Contains("token") ? httpContext.Request.Cookies["token"] : null;
            if (cookie == null)
                return false;
            var token = cookie.Value;
            if (string.IsNullOrEmpty(token))
                return false;
            var tokenIsValid = JwtTokenProvider.IsValid(token);
            if (!tokenIsValid)
                return false;
            var userModel = JwtTokenProvider.GetUserModel(token);

            foreach (var role in allowedroles)
            {   
                if (userModel.Roles.Any(r => r == role))
                    return true;
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                var urlHelper = new UrlHelper(filterContext.RequestContext);
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        Error = "NotAuthorized"
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}