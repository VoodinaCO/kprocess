﻿using Kprocess.KL2.FileServer.Authentication;
using KProcess;
using KProcess.KL2.APIClient;
using KProcess.KL2.JWT;
using KProcess.Ksmed.Models;
using KProcess.Supervision.Log4net;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Practices.Unity;
using Murmur;
using Owin;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Tasks;
using tusdotnet;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Models.Configuration;
using UAParser;

namespace Kprocess.KL2.FileServer.App_Start
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
            //app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            /*var myProvider = new AuthorizationServerProvider(config.DependencyResolver as IServiceBus);
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

            // Branch the pipeline here for requests that start with "/signalr"
            app.Map("/signalr", map =>
            {
                // Setup the CORS middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);
                map.RunSignalR(SignalRConfig.Register(container));
            });

            //app.MapSignalR(SignalRConfig.Register(container));

            var corsPolicy = new System.Web.Cors.CorsPolicy
            {
                AllowAnyHeader = true,
                AllowAnyMethod = true,
                AllowAnyOrigin = true
            };

            // ExposedHeaders has a private setter for some reason so one must use reflection to set it.
            corsPolicy.GetType()
                .GetProperty(nameof(corsPolicy.ExposedHeaders))
                .SetValue(corsPolicy, tusdotnet.Helpers.CorsHelper.GetExposedHeaders());

            app.UseCors(new CorsOptions
            {
                PolicyProvider = new CorsPolicyProvider
                {
                    PolicyResolver = context => Task.FromResult(corsPolicy)
                }
            });


            app.UseTus(context => new DefaultTusConfiguration
            {
                Store = ((UnityResolver)config.DependencyResolver).Resolve<IFileProvider>().GetTusStore(),
                UrlPath = "/files",
                Events = new Events
                {
                    OnFileCompleteAsync = async ctx =>
                    {
                        if (ctx.Store is ITusReadableStore readableStore)
                        {
                            var fileProvider = ((UnityResolver)config.DependencyResolver).Resolve<IFileProvider>();

                            var file = await ctx.GetFileAsync();
                            if (file == null)
                                return;
                            //await readableStore.GetFileAsync(ctx.FileId, ctx.CancellationToken);
                            var metadata = await file.GetMetadataAsync(ctx.CancellationToken);
                            string filename = metadata?.ContainsKey("filename") == true ? metadata["filename"].GetString(System.Text.Encoding.UTF8) : file.Id;
                            if (metadata?.ContainsKey("computeHash") == true)
                            {
                                HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
                                using (var ms = await fileProvider.OpenRead(file.Id, DirectoryEnum.Uploaded))
                                {
                                    var streamHash = murmur128.ComputeHash(ms).ToHashString();
                                    filename = $"{streamHash}{Path.GetExtension(filename)}";
                                }
                            }
                            // On doit déplacer le fichier vers PublishedFiles en le renommant
                            await fileProvider.Complete(file.Id, filename);
                            // TODO : ajouter une metadata indiquant un traitement avec FFMPEG (encodage vidéo, traitement image)
                            // Supprimer les fichiers temp
                            var tempFiles = await fileProvider.EnumerateFiles(DirectoryEnum.Uploaded, $"{file.Id}.*");
                            foreach (string tmpFile in tempFiles)
                                await fileProvider.Delete(tmpFile, DirectoryEnum.Uploaded);

                            fileProvider.WriteAllText($"{fileProvider.UploadedFilesDirectory}/{file.Id}.name", filename);
                        }
                    }
                }
            });

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
            "/GetAllTranscodingProgress"
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
            catch (TaskCanceledException)
            {
                if (!excludeLog.Any(RequestInfo.EndsWith))
                    traceManager.TraceDebug($"<-- Response : Cancelling {RequestInfo}");
            }
            catch (OperationCanceledException)
            {
                if (!excludeLog.Any(RequestInfo.EndsWith))
                    traceManager.TraceDebug($"<-- Response : Cancelling {RequestInfo}");
            }
            /*try
            {
                await Next.Invoke(context);
                traceManager.TraceDebug($"<-- Response : {context.Response.StatusCode} {context.Request.Uri}");
            }
            catch (TaskCanceledException ex)
            {
                traceManager.TraceError(ex, $"<-- TaskCanceledException");
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                traceManager.TraceError(ex, $"<-- ObjectDisposedException");
                throw;
            }
            catch (Exception ex)
            {
                traceManager.TraceError(ex, $"<-- {context.Response.StatusCode} {context.Request.Uri} with error");
                throw;
            }*/
        }

    }
}