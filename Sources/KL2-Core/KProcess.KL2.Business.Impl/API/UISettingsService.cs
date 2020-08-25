using KProcess.Business;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Business;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.API
{
    /// <summary>
    /// Représente un service de gestion de l'état de l'IHM.
    /// </summary>
    public class UISettingsService : IBusinessService, IUISettingsService
    {
        readonly IAPIHttpClient _apiHttpClient;

        public UISettingsService(IAPIHttpClient apiHttpClient)
        {
            _apiHttpClient = apiHttpClient;
        }

        public Task SaveColumnsInfo(string uiPartCode, IDictionary<string, double> columnInfo)
        {
            dynamic param = new ExpandoObject();
            param.uiPartCode = uiPartCode;
            param.columnInfo = columnInfo;
            return _apiHttpClient.ServiceAsync(KL2_Server.API, nameof(UISettingsService), nameof(SaveColumnsInfo), param);
        }

        public Task<Dictionary<string, double>> GetColumnsInfo(string uiPartCode)
        {
            dynamic param = new ExpandoObject();
            param.uiPartCode = uiPartCode;
            return _apiHttpClient.ServiceAsync<Dictionary<string, double>>(KL2_Server.API, nameof(UISettingsService), nameof(GetColumnsInfo), param);
        }
    }
}