using KProcess.KL2.SignalRClient.Hub;
using Microsoft.AspNet.SignalR.Infrastructure;
using System;
using System.Threading.Tasks;

namespace KProcess.KL2.SignalRClient.Context
{
    public class KLAnalyzeRepository : BaseRespository<KLAnalyzeHub>, IKLAnalyzeRepository
    {
        public KLAnalyzeRepository(IConnectionManager connectionManager) :
            base(connectionManager)
        {
        }

        public Task RefreshProcess(AnalyzeEventArgs mess)
        {
            return Task.Run(() =>
            {
                OnRefreshProcess?.Invoke(this, mess);
            });
        }

        public Task RaiseScenarioUpdated()
        {
            return Task.CompletedTask;
        }

        public event EventHandler<AnalyzeEventArgs> OnRefreshProcess;
    }
}
