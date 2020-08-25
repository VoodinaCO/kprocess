using System.Threading;
using System.Threading.Tasks;

namespace KProcess.KL2.SignalRClient
{
    /// <summary>
    /// Extensions for <see cref="IEventSignalR"/>.
    /// </summary>
    public static class EventSignalRExtensions
    {
        /// <summary>
        /// Publishes a message on the current thread (synchrone).
        /// </summary>
        /// <param name="eventSignal">The event signal.</param>
        /// <param name = "message">The message instance.</param>
        public static void PublishOnCurrentThread(this IEventSignalR eventSignal, object message)
        {
            eventSignal.Publish(message, action => action());
        }

        /// <summary>
        /// Publishes a message on a background thread (async).
        /// </summary>
        /// <param name="eventSignal">The event signal.</param>
        /// <param name = "message">The message instance.</param>
        public static void PublishOnBackgroundThread(this IEventSignalR eventSignal, object message)
        {
            eventSignal.Publish(message, action =>
            {
                Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None,
                        TaskScheduler.Default);
            });
        }
    }
}
