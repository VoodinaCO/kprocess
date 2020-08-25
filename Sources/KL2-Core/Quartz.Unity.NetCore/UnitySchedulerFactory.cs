using Quartz.Core;
using Quartz.Impl;

namespace Quartz.Unity.NetCore
{
    public class UnitySchedulerFactory : StdSchedulerFactory
    {
        readonly UnityJobFactory _unityJobFactory;

        public UnitySchedulerFactory(UnityJobFactory unityJobFactory)
        {
            _unityJobFactory = unityJobFactory;
        }

        protected override IScheduler Instantiate(QuartzSchedulerResources rsrcs, QuartzScheduler qs)
        {
            qs.JobFactory = _unityJobFactory;
            return base.Instantiate(rsrcs, qs);
        }
    }
}
