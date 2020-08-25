using KProcess.KL2.SignalRClient.Context;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Threading.Tasks;

namespace KProcess.KL2.SignalRClient.Hub
{
    [HubName(nameof(KLPublicationHub))]
    public class KLPublicationHub : BaseKL2Hub
    {
        public KLPublicationHub(IKLPublicationRepository publicationRepository)
        {
            var _publicationRepository = publicationRepository ?? throw new ArgumentNullException(nameof(IKLPublicationRepository));
            _publicationRepository.OnRefreshPublicationProgress -= OnRefreshPublicationProgress;
            _publicationRepository.OnRefreshPublicationProgress += OnRefreshPublicationProgress;
        }

        public void OnRefreshPublicationProgress(object sender, PublicationProgressEventArgs eventArgs)
        {
            Clients.All.RefreshPublicationProgress(eventArgs);
        }

        public Task RefreshPublicationProgress(PublicationProgressEventArgs mess)
        {
            return Task.Run(() => AddConnectionId(nameof(RefreshPublicationProgress)));
        }
    }
}
