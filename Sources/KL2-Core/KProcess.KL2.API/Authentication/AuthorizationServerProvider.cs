using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Business.Desktop;
using KProcess.Supervision.Log4net;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KProcess.KL2.API.Authentication
{
    /// <summary>
    /// Custom OAuth provider
    /// </summary>
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            return Task.Run(() => context.Validated());
        }

        /// <summary>
        /// Grant credentials according to standard authentication method
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // Creation of service
            ITraceManager traceManager = new Log4netTraceManager(new Log4netWrapper());
            ISecurityContext securityContext = new IdentitySecurityContext(traceManager, new AuthenticationService());

            ClaimsIdentity identity = new ClaimsIdentity(context.Options.AuthenticationType);

            try
            {
                (User User, bool Result) = await securityContext.TryLogonUser(context.UserName, context.Password, "");

                if (Result)
                {
                    AuthenticationProperties props = new AuthenticationProperties(new Dictionary<string, string>
                    {
                        ["username"] = context.UserName
                    });
                    identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                    AuthenticationTicket ticket = new AuthenticationTicket(identity, props);
                    context.Validated(ticket);
                }
                else if (User != null && context.Password == User.Password.ToHashString())
                {
                    securityContext.ReconnectUser(User);
                    AuthenticationProperties props = new AuthenticationProperties(new Dictionary<string, string>
                    {
                        ["username"] = context.UserName
                    });
                    identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                    AuthenticationTicket ticket = new AuthenticationTicket(identity, props);
                    context.Validated(ticket);
                }
                else
                    throw new Exception("Provided username and password is incorrect");
            }
            catch (Exception ex)
            {
                context.SetError("invalid_grant", ex.Message);
                traceManager.TraceError($"Authentication failed for username={context.UserName} with msg={ex.Message}{(ex.InnerException == null ? string.Empty : $" and inner={ex.InnerException?.Message}")}", ex);
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            return Task.CompletedTask;
        }
    }
}
