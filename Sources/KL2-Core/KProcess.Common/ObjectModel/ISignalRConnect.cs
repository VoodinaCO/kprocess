using System;

namespace KProcess
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISignalRConnect : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        void StartConnect();

        /// <summary>
        /// Raise event connected success
        /// </summary>
        event EventHandler OnConnected;

        /// <summary>
        /// Raise event disconnect or in reconnecting
        /// </summary>
        event EventHandler OnDisconnected;
    }
}
