using KProcess.KL2.SignalRClient.Context;
using KProcess.KL2.SignalRClient.Hub;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace KProcess.KL2.SignalRClient
{
    public sealed class KL2AnalyzeHubConnect : BaseKL2HubConnect<KLAnalyzeHub>, IKL2AnalyzeHubConnect
    {
#pragma warning disable CS0067
        public event EventHandler<AnalyzeEventArgs> OnRefreshProcess;
#pragma warning restore CS0067

        public KL2AnalyzeHubConnect(IEventSignalR eventSignalR) : base(eventSignalR)
        {

        }

        protected override void RegisterTakeNotificationEvent()
        {
            ProxyHub.On<AnalyzeEventArgs>(nameof(IKLAnalyzeRepository.RefreshProcess), message =>
            {
                var mess = new AnalyzeEventArgs(string.Empty);
                if (message != null && ConnectionHub != null)
                    mess = message;

                EventSignalR?.PublishOnBackgroundThread(mess);
            });
        }

        public Task RefreshProcess(AnalyzeEventArgs mess)
        {
            return ProxyHub.Invoke(nameof(IKLAnalyzeRepository.RefreshProcess), mess);
        }

        public Task RaiseScenarioUpdated()
        {
            return ProxyHub.Invoke(nameof(RaiseScenarioUpdated));
        }

    }
}
