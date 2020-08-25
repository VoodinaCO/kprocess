using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.Business
{
    /// <summary>
    /// Fournit des fabriques utilisées pour faire de l'asynchronisme.
    /// </summary>
    public static class AsyncFactory
    {

        private static Func<TaskScheduler> _taskSchedulerFactory = () =>
        {
            if (System.Threading.SynchronizationContext.Current != null)
                return TaskScheduler.FromCurrentSynchronizationContext();
            else
                return TaskScheduler.Default;
        };

        /// <summary>
        /// Obtient ou définit la fabrique de <see cref="TaskScheduler"/>.
        /// </summary>
        public static Func<TaskScheduler> TaskSchedulerFactory
        {
            get { return _taskSchedulerFactory; }
            set { _taskSchedulerFactory = value; }
        }

    }
}
