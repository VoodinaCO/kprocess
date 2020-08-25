using KProcess.KL2.SignalRClient.Context;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Threading.Tasks;

namespace KProcess.KL2.SignalRClient.Hub
{
    [HubName(nameof(KLAnalyzeHub))]
    public class KLAnalyzeHub : BaseKL2Hub
    {
        public KLAnalyzeHub(IKLAnalyzeRepository analyzeRepository)
        {
            var _analyzeRepository = analyzeRepository ?? throw new ArgumentNullException(nameof(IKLAnalyzeRepository));
            _analyzeRepository.OnRefreshProcess -= OnRefreshProcess;
            _analyzeRepository.OnRefreshProcess += OnRefreshProcess;
        }

        void OnRefreshProcess(object sender, AnalyzeEventArgs eventArgs)
        {
            var list = GetConnectionId(nameof(RefreshProcess));
            if (list == null)
                list = Array.Empty<string>();

            Clients.AllExcept(list).RefreshProcess(eventArgs);
        }

        public Task RefreshProcess(AnalyzeEventArgs mess)
        {
            return Task.Run(() => AddConnectionId(nameof(RefreshProcess)));
        }

        public Task RaiseDocumentationUpdated()
        {
            Clients.All.RefreshProcess(new AnalyzeEventArgs(nameof(RaiseDocumentationUpdated)));
            return Task.CompletedTask;
        }

        public Task RaiseScenarioUpdated()
        {
            Clients.Others.RefreshProcess(new AnalyzeEventArgs(nameof(RaiseScenarioUpdated)));
            return Task.CompletedTask;
        }
    }
}
