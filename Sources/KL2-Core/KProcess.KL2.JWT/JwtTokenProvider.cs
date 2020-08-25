using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace KProcess.KL2.JWT
{
    public static class JwtTokenProvider
    {
        public static readonly string _appDomain = "k-process.com";
        public static readonly string _jwtSecret = "c3W|T4&D(o#D0+:j<JN25agDP%46H)";

        public static readonly string WebAudience = "KL² Web Admin";
        public static readonly string TabletAudience = "KL² Tablet";
        public static readonly string TabletSyncServiceAudience = "KL² Tablet Sync Service";
        public static readonly string DesktopAudience = "KL²";

        /// <summary>
        /// Creates a JSW Bearer token for the user.
        /// </summary>
        /// <see cref="https://jwt.io/"/>
        /// <see cref="https://jonhilton.net/2017/10/11/secure-your-asp.net-core-2.0-api-part-1---issuing-a-jwt/"/>
        /// <see cref="http://bitoftech.net/2014/08/11/asp-net-web-api-2-external-logins-social-logins-facebook-google-angularjs-app/"/>
        /// <param name="user">User model.</param>
        /// <returns>A JWT bearer token.</returns>
        public static string GenerateAccessToken(string audience, UserModel user)
        {
            var jwtSecurityToken = new JwtSecurityToken
            (
                issuer: _appDomain,
                audience: audience,
                claims: CreateClaims(user),
                notBefore: DateTime.UtcNow,
                // Set expiry to whatever suits you, or implement short-lived 
                // sliding-tokens for better security
                expires: DateTime.UtcNow.AddYears(1),
                signingCredentials: _jwtSecret.ToIdentitySigningCredentials()
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }

        /// <summary>
        /// Regenerate a JSW Bearer token from an old (expired) token.
        /// </summary>
        /// <see cref="https://jwt.io/"/>
        /// <see cref="https://jonhilton.net/2017/10/11/secure-your-asp.net-core-2.0-api-part-1---issuing-a-jwt/"/>
        /// <see cref="http://bitoftech.net/2014/08/11/asp-net-web-api-2-external-logins-social-logins-facebook-google-angularjs-app/"/>
        /// <param name="user">User model.</param>
        /// <returns>A JWT bearer token.</returns>
        public static string RegenerateAccessToken(string audience, string token)
        {
            try
            {
                if (IsValid(token, false))
                    return GenerateAccessToken(audience, GetUserModel(token));
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Creates a collection of claims based on the the current user.
        /// </summary>
        /// <param name="user">UserModel which describes the current user.</param>
        /// <returns>A collection of Claims for the JWT token.</returns>
        private static IEnumerable<Claim> CreateClaims(UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.GivenName, user.Firstname ?? string.Empty),
                new Claim(ClaimTypes.Surname, user.Name ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
                new Claim(ClaimTypes.Country, user.DefaultLanguageCode),
                new Claim(ClaimTypes.Locality, user.CurrentLanguageCode),
                new Claim(ClaimTypes.IsPersistent, user.RememberMe.ToString()),
                // Add any aditional claims here
            };

            if (user.Roles != null)
            {
                foreach (var roleCode in user.Roles)
                    claims.Add(new Claim(ClaimTypes.Role, roleCode));
            }

            return claims;
        }

        /// <summary>
        /// Valid a JWT token
        /// </summary>
        public static bool IsValid(string token, bool validateLifeTime = true)
        {
            try
            {
                var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = _jwtSecret.ToSymmetricSecurityKey(),
                    ValidIssuer = _appDomain,
                    ValidateAudience = false,
                    ValidateLifetime = validateLifeTime,
                    ClockSkew = TimeSpan.FromMinutes(0)
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get UserModel from JWT token
        /// </summary>
        public static UserModel GetUserModel(string token)
        {
            UserModel userModel = null;

            try
            {
                var jsonToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                userModel = new UserModel
                {
                    UserId = int.Parse(jsonToken.Claims.First(_ => _.Type == ClaimTypes.NameIdentifier).Value),
                    Username = jsonToken.Claims.First(_ => _.Type == ClaimTypes.Name).Value,
                    Firstname = jsonToken.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.GivenName)?.Value,
                    Name = jsonToken.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Surname)?.Value,
                    Email = jsonToken.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Email)?.Value,
                    PhoneNumber = jsonToken.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.MobilePhone)?.Value,
                    DefaultLanguageCode = jsonToken.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Country)?.Value ?? "fr-FR",
                    CurrentLanguageCode = jsonToken.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Locality).Value ?? jsonToken.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Country)?.Value ?? "fr-FR",
                    Roles = jsonToken.Claims.Where(_ => _.Type == ClaimTypes.Role).Select(_ => _.Value),
                    RememberMe = bool.Parse(jsonToken.Claims.First(_ => _.Type == ClaimTypes.IsPersistent).Value)
                };
            }
            catch { }

            return userModel;
        }

        public static string GetUserModelCurrentLanguage(string token)
        {
            string currentLanguage = "";

            try
            {
                var jsonToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                currentLanguage = jsonToken.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Locality).Value ?? jsonToken.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Country)?.Value ?? "fr-FR";
            }
            catch { }

            return currentLanguage;
        }
    }
}
