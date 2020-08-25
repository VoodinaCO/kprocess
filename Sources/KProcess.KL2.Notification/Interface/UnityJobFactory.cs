using Microsoft.Practices.Unity;
using Quartz;
using Quartz.Simpl;
using Quartz.Spi;
using System;

namespace KProcess.KL2.Notification.Interface
{
    public class UnityJobFactory : SimpleJobFactory
    {
        readonly IUnityContainer _container;

        public UnityJobFactory(IUnityContainer container)
        {
            _container = container;
        }

        public override IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {
                return _container.Resolve(bundle.JobDetail.JobType) as IJob;
            }
            catch (Exception e)
            {
                throw new SchedulerException($"Problem while instantiating job '{bundle.JobDetail.Key}' from the UnityJobFactory.", e);
            }
        }

        public override void ReturnJob(IJob job)
        {   
        }
    }
}
