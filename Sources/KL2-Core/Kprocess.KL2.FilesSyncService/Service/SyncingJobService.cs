using JKang.IpcServiceFramework;
using Kprocess.KL2.IpcContracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using Topshelf;

namespace Kprocess.KL2.FilesSyncService.Service
{
    class SyncingJobService : ServiceControl, IFilesSyncService
    {
        public const string ServiceName = "KL² Files sync service";
        public const string DisplayName = "KL² Files sync service";
        public const string Description = "KL² Files syncing job service";

        //ITraceManager traceManager;

        public bool Start(HostControl hostControl)
        {
            Trace.WriteLine($"Starting the service {ServiceName}");
            //traceManager = new Log4netTraceManager(new Log4netWrapper());

            // configure DI
            IServiceCollection services = ConfigureServices(new ServiceCollection());
            // build and run service host
            new IpcServiceHostBuilder(services.BuildServiceProvider())
                .AddNamedPipeEndpoint<IFilesSyncService>(name: "Server", pipeName: "KL2_FilesSyncService")
                //.AddTcpEndpoint<IFilesSyncService>(name: "endpoint2", ipEndpoint: IPAddress.Loopback, port: 45684)
                .Build()
                .Run();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            Trace.WriteLine($"{ServiceName} has been stopped.");
            return true;
        }

        static IServiceCollection ConfigureServices(IServiceCollection services) =>
            services.AddIpc(builder =>
            {
                builder
                    .AddNamedPipe(options =>
                    {
                        options.ThreadCount = Environment.ProcessorCount;
                    })
                    .AddService<IFilesSyncService, SyncingJobService>();
            });

        #region IFilesSyncService

        public bool Ping()
        {
            return true;
        }

        public bool Upload()
        {
            return true;
        }

        #endregion
    }
}
