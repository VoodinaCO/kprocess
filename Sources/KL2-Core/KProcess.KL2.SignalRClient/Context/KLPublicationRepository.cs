using KProcess.KL2.SignalRClient.Hub;
using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Threading.Tasks;

namespace KProcess.KL2.SignalRClient.Context
{
    public class KLPublicationRepository : BaseRespository<KLPublicationHub>, IKLPublicationRepository
    {
        public KLPublicationRepository(IConnectionManager connectionManager) :
            base(connectionManager)
        {
        }

        public Task RefreshPublicationProgress(PublicationProgressEventArgs mess)
        {
            return Task.Run(() =>
            {
                OnRefreshPublicationProgress?.Invoke(this, mess);
            });
        }

        public event EventHandler<PublicationProgressEventArgs> OnRefreshPublicationProgress;
    }
}
