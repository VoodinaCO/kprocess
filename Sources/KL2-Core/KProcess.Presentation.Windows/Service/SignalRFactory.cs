using KProcess.KL2.SignalRClient;
using MoreLinq;
using System;
using System.ComponentModel.Composition;

namespace KProcess.Presentation.Windows.Service
{
    [Export(typeof(ISignalRFactory))]
    public class SignalRFactory : ISignalRFactory
    {
        private bool _isConnected;

        public void Initialization(IEventSignalR eventSignalR)
        {
            Register<ISignalRConnect>(new KL2AnalyzeHubConnect(eventSignalR), nameof(IKL2AnalyzeHubConnect));
        }

        public TSignalRConnect GetSignalR<TSignalRConnect>() where TSignalRConnect : ISignalRConnect
        {
            if (IoC.IsRegistered<ISignalRConnect>(typeof(TSignalRConnect).Name))
                return (TSignalRConnect)IoC.Resolve<ISignalRConnect>(typeof(TSignalRConnect).Name);

            throw new ArgumentException(typeof(TSignalRConnect).Name + "is not Registed");
        }


        void Register<TService>(TService service, string name)
            where TService : ISignalRConnect
        {
            service.OnConnected += ServiceOnOnConnected;
            service.OnDisconnected += ServiceOnOnDisconnected;

            IoC.RegisterInstance(name, service);

            service.StartConnect();
        }

        private void ServiceOnOnDisconnected(object sender, EventArgs eventArgs)
        {
            if(!_isConnected)
                return;

            _isConnected = false;
            OnDisconnected?.Invoke(this, EventArgs.Empty);
        }

        private void ServiceOnOnConnected(object sender, EventArgs eventArgs)
        {
            if (_isConnected)
                return;

            _isConnected = true;
            OnConnected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            var list = IoC.ResolveAll<ISignalRConnect>();
            list.ForEach(x =>
            {
                x.Dispose();
            });
        }

        public event EventHandler OnConnected;

        public event EventHandler OnDisconnected;
    }
}
