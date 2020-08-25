using Microsoft.AspNet.SignalR.Client;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace KProcess.KL2.SignalRClient
{
    public abstract class BaseKL2HubConnect<HubName>
    {
        public const string ApiServerUriKey = "ApiServerUri";

        protected HubConnection ConnectionHub;
        protected IHubProxy ProxyHub;
        protected IEventSignalR EventSignalR;
        bool _isConnect;
        bool _isConnecting;

        DispatcherTimer _timer;

        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;

        protected BaseKL2HubConnect(IEventSignalR eventSignal)
        {
            InitTimer();
            EventSignalR = eventSignal;
            InitConnection();
        }

        void InitConnection()
        {
            try
            {
                if (ConnectionHub != null)
                {
                    ConnectionHub.Closed -= ConnectionHubOnClosed;
                    ConnectionHub.Reconnecting -= ConnectionHubOnReconnecting;
                    ConnectionHub.Reconnected -= ConnectionHubOnReconnected;
                }

                ConnectionHub = new HubConnection(ConfigurationManager.AppSettings[ApiServerUriKey], true);
                try
                {
                    var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                    ConnectionHub.Headers.Add("RequestSource", $"Computer: {Environment.MachineName}, IP: {hostEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)}, App: {Assembly.GetEntryAssembly().GetName().Name}");
                }
                catch (SocketException)
                {
                    ConnectionHub.Headers.Add("RequestSource", $"Computer: {Environment.MachineName}, IP: Unknown, App: {Assembly.GetEntryAssembly().GetName().Name}");
                }
                ConnectionHub.Closed += ConnectionHubOnClosed;
                ConnectionHub.Reconnecting += ConnectionHubOnReconnecting;
                ConnectionHub.Reconnected += ConnectionHubOnReconnected;

                ProxyHub = ConnectionHub.CreateHubProxy(typeof(HubName).Name);

                RegisterTakeNotificationEvent();
            }
            catch (Exception ex)
            {
                _isConnect = false;
                System.Diagnostics.Debug.WriteLine("Error " + ex.Message);

                OnDisconnected?.Invoke(this, EventArgs.Empty);

                throw new HubException($"Error to connect to Service. Check the service is online, and the ServiceAddress is correct. Error:{ex.Message}");
            }
        }

        void InitTimer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            _timer.Tick += TimerOnTick;
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            if (_isConnecting)
                return;

            if (_isConnect)
            {
                _timer.Stop();
                return;
            }
            _isConnecting = true;
            StartConnect();
        }

        public async void StartConnect()
        {
            try
            {
                await Task.WhenAll(ConnectionHub.Start());
                _timer.Stop();
                _isConnecting = false;

                if (ConnectionHub.State == ConnectionState.Connected)
                {
                    OnConnected?.Invoke(this, EventArgs.Empty);
                    await JoinGroup("JoinGroup");
                    _isConnect = true;
                }
            }
            catch (Exception e)
            {
                _isConnecting = false;
                Console.WriteLine(e);
            }
        }

        private void ConnectionHubOnReconnected()
        {
            OnConnected?.Invoke(this, EventArgs.Empty);
        }

        private void ConnectionHubOnReconnecting()
        {
            OnDisconnected?.Invoke(this, EventArgs.Empty);
        }

        private void ConnectionHubOnClosed()
        {
            if (_isConnecting)
                return;

            OnDisconnected?.Invoke(this, EventArgs.Empty);

            Thread.Sleep(5000);
            _isConnect = false;
            _timer.Start();
        }

        public virtual Task JoinGroup(string groupName)
        {
            return ProxyHub.Invoke(nameof(JoinGroup), groupName);
        }

        protected virtual void RegisterTakeNotificationEvent()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsAvailable()
        {
            return _isConnect && ConnectionHub != null && ConnectionHub.State == ConnectionState.Connected;
        }

        public void Dispose()
        {
            _isConnect = false;

            ConnectionHub?.Dispose();
            ConnectionHub = null;
            ProxyHub = null;
        }
    }
}
