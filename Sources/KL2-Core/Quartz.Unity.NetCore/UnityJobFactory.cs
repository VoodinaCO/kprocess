using Microsoft.Practices.Unity;
using Quartz.Spi;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Quartz.Unity.NetCore
{
    public class UnityJobFactory : IJobFactory
    {
        readonly IUnityContainer _container;

        static UnityJobFactory()
        {
        }

        public UnityJobFactory(IUnityContainer container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;

            try
            {
                return new JobWrapper(bundle, _container);
            }
            catch (Exception ex)
            {
                throw new SchedulerException(string.Format(
                    CultureInfo.InvariantCulture,
                    "Problem instantiating class '{0}'", new object[] { jobDetail.JobType.FullName }), ex);
            }
        }

        public void ReturnJob(IJob job)
        {
            // Nothing here. Unity does not maintain a handle to container created instances.
        }


        #region Job Wrappers

        /// <summary>
        ///     Job execution wrapper.
        /// </summary>
        /// <remarks>
        ///     Creates nested lifetime scope per job execution and resolves Job from Autofac.
        /// </remarks>
        internal class JobWrapper : IJob
        {
            readonly TriggerFiredBundle bundle;
            readonly IUnityContainer unityContainer;

            /// <summary>
            ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
            /// </summary>
            public JobWrapper(TriggerFiredBundle bundle, IUnityContainer unityContainer)
            {
                this.bundle = bundle ?? throw new ArgumentNullException(nameof(bundle));
                this.unityContainer = unityContainer ?? throw new ArgumentNullException(nameof(unityContainer));
            }

            protected IJob RunningJob { get; private set; }

            /// <summary>
            ///     Called by the <see cref="T:Quartz.IScheduler" /> when a <see cref="T:Quartz.ITrigger" />
            ///     fires that is associated with the <see cref="T:Quartz.IJob" />.
            /// </summary>
            /// <remarks>
            ///     The implementation may wish to set a  result object on the
            ///     JobExecutionContext before this method exits.  The result itself
            ///     is meaningless to Quartz, but may be informative to
            ///     <see cref="T:Quartz.IJobListener" />s or
            ///     <see cref="T:Quartz.ITriggerListener" />s that are watching the job's
            ///     execution.
            /// </remarks>
            /// <param name="context">The execution context.</param>
            /// <exception cref="SchedulerConfigException">Job cannot be instantiated.</exception>
            public Task Execute(IJobExecutionContext context)
            {
                var childContainer = unityContainer.CreateChildContainer();
                try
                {
                    RunningJob = (IJob)childContainer.Resolve(bundle.JobDetail.JobType);
                    return RunningJob.Execute(context);
                }
                catch (JobExecutionException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new JobExecutionException(string.Format(CultureInfo.InvariantCulture,
                        "Failed to execute Job '{0}' of type '{1}'",
                        bundle.JobDetail.Key, bundle.JobDetail.JobType), ex);
                }
                finally
                {
                    RunningJob = null;
                    childContainer.Dispose();
                }
            }
        }

        #endregion
    }
}
