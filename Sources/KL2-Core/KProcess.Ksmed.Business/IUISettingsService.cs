using KProcess.Business;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Definit le comprotement d'un service de gestion des états des écrans
    /// </summary>
    public interface IUISettingsService: IBusinessService
    {
        Task SaveColumnsInfo(string uiPartCode, IDictionary<string, double> columnInfo);

        Task<Dictionary<string, double>> GetColumnsInfo(string uiPartCode);
    }
}
