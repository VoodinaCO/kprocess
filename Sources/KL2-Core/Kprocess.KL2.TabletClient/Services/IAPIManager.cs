using Kprocess.KL2.TabletClient.Models;
using KProcess.Ksmed.Models;
using System;
using System.Threading.Tasks;

namespace Kprocess.KL2.TabletClient.Services
{
    public interface IAPIManager
    {
        bool? IsOnline { get; }

        Task<Publication> SyncPublicationOffline(OfflineFile offlineFile, Publication publication);

        Task<Publication> SyncPublicationOnline(OfflineFile offlineFile, Publication publication, bool saveFirst = true);

        event EventHandler<OnlineEventArgs> APIStatusChangedHandler;
    }
}
