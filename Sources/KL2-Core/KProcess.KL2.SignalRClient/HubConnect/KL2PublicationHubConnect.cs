using KProcess.KL2.SignalRClient.Context;
using KProcess.KL2.SignalRClient.Hub;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace KProcess.KL2.SignalRClient
{
    public sealed class KL2PublicationHubConnect : BaseKL2HubConnect<KLPublicationHub>, IKL2PublicationHubConnect
    {
#pragma warning disable CS0067
        public event EventHandler<PublicationProgressEventArgs> OnRefreshPublicationProgress;
#pragma warning restore CS0067

        public KL2PublicationHubConnect(IEventSignalR eventSignalR) : base(eventSignalR)
        {

        }

        protected override void RegisterTakeNotificationEvent()
        {
            ProxyHub.On<PublicationProgressEventArgs>(nameof(IKLPublicationRepository.RefreshPublicationProgress), message =>
            {
                if (ConnectionHub != null)
                    EventSignalR?.PublishOnBackgroundThread(message);
            });
        }

        public Task RefreshPublicationProgress(PublicationProgressEventArgs mess)
        {
            return ProxyHub.Invoke(nameof(IKLPublicationRepository.RefreshPublicationProgress), mess);
        }

    }
}
