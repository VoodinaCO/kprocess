using KProcess.KL2.API.Authentication;
using KProcess.KL2.JWT;
using KProcess.Supervision.Log4net;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Practices.Unity;
using Owin;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UAParser;

namespace KProcess.KL2.API.App_Start
{
    public class OwinConfiguration
    {
        public void Configuration(IAppBuilder app)
        {
            // Dependency injection settings
            var container = new UnityContainer();
            var config = WebApiConfig.Register(container);

            app.Use<LoggingMiddleware>();
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            //enable cors origin requestsit 
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            /*var myProvider = new AuthorizationServerProvider();
            OAuthAuthorizationServerOptions options = new OAuthAuthorizationServerOptions
            {
                AllowInsecureHttp = true,
                TokenEndpointPath = new PathString("/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),   // OAuth token valid 1 day
                Provider = myProvider,
                RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };
            app.UseOAuthAuthorizationServer(options);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());*/
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                //AllowedAudiences = new[] { "KL²", "KL² Web Admin", "KL² Tablet" },
                TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = JwtTokenProvider._jwtSecret.ToSymmetricSecurityKey(),
                    ValidIssuer = JwtTokenProvider._appDomain,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(0)
                }
            });

            app.UseWebApi(config);
            app.MapSignalR(SignalRConfig.Register(container));

            // Setup Log4Net configuration (in App.config)
            log4net.Config.XmlConfigurator.Configure();
        }

        class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
        {
            private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();

            public Task CreateAsync(AuthenticationTokenCreateContext context) =>
                Task.Run(() => Create(context));

            public Task ReceiveAsync(AuthenticationTokenReceiveContext context) =>
                Task.Run(() => Receive(context));

            public void Create(AuthenticationTokenCreateContext context)
            {
                var guid = Guid.NewGuid().ToString().Replace("-", "");

                _refreshTokens.TryAdd(guid, context.Ticket);

                // hash??
                context.SetToken(guid);
                context.Ticket.Properties.ExpiresUtc = new DateTimeOffset(DateTime.UtcNow.AddYears(9));
            }

            public void Receive(AuthenticationTokenReceiveContext context)
            {
                if (_refreshTokens.TryRemove(context.Token, out AuthenticationTicket ticket))
                    context.SetTicket(ticket);
            }
        }
    }

    public class LoggingMiddleware : OwinMiddleware
    {
        readonly ITraceManager traceManager;

        static readonly string[] excludeLog =
        {
            "/Ping",
            "/token",
            "/CheckAuditorHaveActiveAudit"
        };

        public LoggingMiddleware(OwinMiddleware next)
            : base(next)
        {
            traceManager = new Log4netTraceManager(new Log4netWrapper());
        }

        public override async Task Invoke(IOwinContext context)
        {
            string RequestMethod = $"{context.Request.Method}";
            string RequestUri = $"{context.Request.Uri}";
            string Range = context.Request.Headers.ContainsKey("Range") ? context.Request.Headers["Range"] : null;
            string RequestInfo = $"{RequestMethod}{(string.IsNullOrEmpty(Range) ? null : $" Range : {Range}, ")} Request : {RequestUri}";

            if (!excludeLog.Any(RequestInfo.EndsWith))
            {
                string requestSource = context.Request.Headers.ContainsKey("RequestSource") ? context.Request.Headers["RequestSource"] : null;
                if (string.IsNullOrEmpty(requestSource)) // It's from Web
                {
                    var remoteAddr = context.Request.RemoteIpAddress;
                    if (remoteAddr == "::1")
                        remoteAddr = "localhost";
                    var uaParser = Parser.GetDefault();
                    ClientInfo c = null;
                    if (context.Request.Headers.ContainsKey("user-agent"))
                        c = uaParser.Parse(context.Request.Headers["user-agent"]);
                    try
                    {
                        var hostEntry = Dns.GetHostEntry(remoteAddr);
                        requestSource = $"Computer: {hostEntry.HostName}, IP: {context.Request.RemoteIpAddress}, App: Web, Navigator: {c?.UA.Family}";
                    }
                    catch (SocketException)
                    {
                        requestSource = $"Computer: Unknown, IP: {context.Request.RemoteIpAddress}, App: Web, Navigator: {c?.UA.Family}";
                    }
                }

                traceManager.TraceDebug($"--> Request : {RequestInfo}");
                traceManager.TraceDebug($"from {requestSource}");
            }
            try
            {
                if (context.Request.CallCancelled.IsCancellationRequested)
                {
                    if (!excludeLog.Any(RequestInfo.EndsWith))
                        traceManager.TraceDebug($"<-- Response : Cancelling {RequestInfo}");
                }
                else
                {
                    await Next.Invoke(context);
                    if (!excludeLog.Any(RequestInfo.EndsWith))
                        traceManager.TraceDebug($"<-- Response : {context.Response.StatusCode} {RequestInfo}");
                }
            }
            catch (OperationCanceledException)
            {
                if (!excludeLog.Any(RequestInfo.EndsWith))
                    traceManager.TraceDebug($"<-- Response : Cancelling {RequestInfo}");
            }
            catch (Exception ex)
            {
                if (!excludeLog.Any(RequestInfo.EndsWith))
                    traceManager.TraceError(ex, $"<-- {context.Response.StatusCode} {RequestInfo} with error");
            }
        }

    }
}
