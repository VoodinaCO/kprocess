using System;
using System.Threading.Tasks;

namespace KProcess.KL2.SignalRClient.Context
{
    public interface IKLAnalyzeRepository
    {
        Task RefreshProcess(AnalyzeEventArgs args);

        Task RaiseScenarioUpdated();

        event EventHandler<AnalyzeEventArgs> OnRefreshProcess;
    }
}
