using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static KProcess.KL2.APIClient.Delegates;

namespace KProcess.KL2.APIClient
{
    public interface IAPIHttpClient
    {
        event Connecting OnConnecting;

        Task<T> ServiceFormDataAsync<T>(KL2_Server server, string service, string function, Dictionary<string, string> parameters, Dictionary<string, string> files, IProgress<double> progress = null);

        void SetUserId(KL2_Server server, int? id);

        string Token { get; set; }

        Task<bool> CheckConnectionAsync(KL2_Server server);

        int? UserId(KL2_Server server);

        Task<bool> Logon(string username, string password, string language = null);

        Task<bool> Relogon(string oldToken = null);

        Task<T> ServiceAsync<T>(KL2_Server server, string service, string function, dynamic param = null, string method = "POST");
        Task ServiceAsync(KL2_Server server, string service, string function, dynamic param = null, string method = "POST");
        T Service<T>(KL2_Server server, string service, string function, dynamic param = null, string method = "POST");

        ObservableDictionary<string, string> DownloadedFiles { get; }

        void RefreshDownloadedFiles(string downloadFolder = null);

        (string fileName, byte[] data) GetLog(KL2_Server server);
    }
}
