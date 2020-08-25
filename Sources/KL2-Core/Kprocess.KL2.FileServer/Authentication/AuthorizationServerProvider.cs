using KProcess;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Business.Desktop;
using KProcess.Supervision.Log4net;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kprocess.KL2.FileServer.Authentication
{
    /// <summary>
    /// Custom OAuth provider
    /// </summary>
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly IServiceBus _serviceBus;
        public AuthorizationServerProvider(IServiceBus serviceBus)
        {
            _serviceBus = serviceBus;
        }


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
            var (User, Result) = await securityContext.TryLogonUser(context.UserName, context.Password, string.Empty);

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
            else
            {
                context.SetError("invalid_grant", "Provided username and password is incorrect");
                traceManager.TraceInfo($"Authentication failed for username={context.UserName}");
                return;
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            return Task.FromResult<object>(null);
        }
    }
}
