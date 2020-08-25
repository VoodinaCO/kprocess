using System;
using System.Threading.Tasks;

namespace KProcess.KL2.SignalRClient.Context
{
    public interface IKLPublicationRepository
    {
        Task RefreshPublicationProgress(PublicationProgressEventArgs args);

        event EventHandler<PublicationProgressEventArgs> OnRefreshPublicationProgress;
    }
}
