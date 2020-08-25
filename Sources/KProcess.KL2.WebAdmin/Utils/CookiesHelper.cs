using KProcess.KL2.JWT;
using System;
using System.Web;

namespace KProcess.KL2.WebAdmin.Utils
{
    public static class CookiesHelper
    {
        public static void SetAuthorizationCookie(this HttpResponseBase response, UserModel userModel)
        {
            var jwtToken = JwtTokenProvider.GenerateAccessToken(JwtTokenProvider.WebAudience, userModel);

            var cookie = new HttpCookie("token", jwtToken)
            {
                Expires = userModel.RememberMe ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMinutes(30)
            };
            response.SetCookie(cookie);
        }
    }
}