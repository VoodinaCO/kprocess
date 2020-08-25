using Quartz.Spi;
using System;
using Topshelf.HostConfigurators;

namespace Topshelf.Quartz
{
    public static class ScheduleJobHostConfiguratorExtensions
    {
        public static HostConfigurator UsingQuartzJobFactory<TJobFactory>(this HostConfigurator configurator, Func<TJobFactory> jobFactory)
            where TJobFactory : IJobFactory
        {
            ScheduleJobServiceConfiguratorExtensions.JobFactory = jobFactory();
            return configurator;
        }

        public static HostConfigurator UsingQuartzJobFactory<TJobFactory>(this HostConfigurator configurator)
            where TJobFactory : IJobFactory, new()
        {
            return UsingQuartzJobFactory(configurator, () => new TJobFactory());
        }


        public static HostConfigurator ScheduleQuartzJobAsService(this HostConfigurator configurator, Action<QuartzConfigurator> jobConfigurator, bool replaceJob = false)
        {
            configurator.Service<NullService>(s => s
                .ScheduleQuartzJob(jobConfigurator, replaceJob)
                .WhenStarted(p => p.Start())
                .WhenStopped(p => p.Stop())
                .ConstructUsing(settings => new NullService())
            );

            return configurator;
        }
    }
}
