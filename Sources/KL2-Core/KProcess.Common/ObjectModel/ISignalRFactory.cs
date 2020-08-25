using System;

namespace KProcess
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISignalRFactory : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventSignalR"></param>
        void Initialization(IEventSignalR eventSignalR);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSignalRConnect"></typeparam>
        /// <returns></returns>
        TSignalRConnect GetSignalR<TSignalRConnect>() where TSignalRConnect : ISignalRConnect;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler OnConnected;

        /// <summary>
        /// 
        /// </summary>
        event EventHandler OnDisconnected;
    }
}
