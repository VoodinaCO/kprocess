using Kprocess.KL2.TabletClient.Extensions;
using Kprocess.KL2.TabletClient.Models;
using KProcess;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using System;
using System.Threading.Tasks;

namespace Kprocess.KL2.TabletClient.Services
{
    public class APIManager : IAPIManager
    {
        readonly ISignalRFactory _signalRFactory;
        readonly ITraceManager _traceManager;

        bool? _isOnline;
        public bool? IsOnline
        {
            get => _isOnline;
            private set
            {
                var oldValue = _isOnline;
                _isOnline = value;
                APIStatusChangedHandler?.Invoke(this, new OnlineEventArgs(oldValue, _isOnline));
            }
        }

        public event EventHandler<OnlineEventArgs> APIStatusChangedHandler;

        public Task<Publication> SyncPublicationOffline(OfflineFile offlineFile, Publication publication) =>
            Task.Run(() =>
            {
                if (publication == null)
                    return null;
                var hasChanges = publication.HasChanges();
                if (!hasChanges)
                    return publication;
                offlineFile.SaveToJson(publication);
                return publication;
            });

        public Task<Publication> SyncPublicationOnline(OfflineFile offlineFile, Publication publication, bool saveFirst = true) =>
            Task.Run(async () =>
            {
                if (publication == null)
                    return null;
                var hasChanges = publication.HasChanges();
                if (!hasChanges)
                    return publication;
                try
                {
                    if (saveFirst)
                        offlineFile.SaveToJson(publication);
                    Publication result = await Locator.GetService<IPrepareService>().SavePublication(publication);
                    if (result != null)
                        await result.UpdatePublication(false);
                    offlineFile.SaveToJson(result);
                    return result;
                }
                catch (Exception e)
                {
                    var syncException = new SyncingException("Error when syncing publication", e);
                    _traceManager.TraceError(syncException, syncException.Message);
                    throw syncException;
                }
            });

        public APIManager(ISignalRFactory signalRFactory, ITraceManager traceManager)
        {
            _signalRFactory = signalRFactory;
            _traceManager = traceManager;

            _signalRFactory.OnConnected += (sender, args) =>
            {
                _traceManager.TraceDebug("API connected.");
                IsOnline = true;
            };
            _signalRFactory.OnDisconnected += (sender, args) =>
            {
                _traceManager.TraceDebug("API disconnected.");
                IsOnline = false;
            };
        }
    }

    public class OnlineEventArgs : EventArgs
    {
        public bool? OldValue { get; }
        public bool? NewValue { get; }

        public OnlineEventArgs(bool? oldValue, bool? newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Syncing exception class 
    /// </summary>
    [Serializable]
    public class SyncingException : Exception
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        public SyncingException()
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message.</param>
        public SyncingException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="format">Formatting string.</param>
        /// <param name="args">Formatting arguments.</param>
        public SyncingException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message.</param>
        /// <param name="innerException">Inner exception.</param>
        public SyncingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="innerException">Inner exception.</param>
        /// <param name="format">Formatting string.</param>
        /// <param name="args">Formatting arguments.</param>
        public SyncingException(Exception innerException, string format, params object[] args)
            : base(string.Format(format, args), innerException)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info"><see cref="T:System.Runtime.Serialization.SerializationInfo" /> that contains serialized object infos concerning raised exception.</param>
        /// <param name="context"><see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual infos concerning source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="info" /> parameters is null.</exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">Class name is null or <see cref="P:System.Exception.HResult" /> equals zero (0). </exception>
        public SyncingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
