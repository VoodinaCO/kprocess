using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API
{
    /// <summary>
    /// Definit le comprotement d'un service de gestion des états des écrans
    /// </summary>
    public interface IUISettingsServiceController
    {
        Task<IHttpActionResult> SaveColumnsInfoAsync(dynamic param);

        Task<IHttpActionResult> GetColumnsInfoAsync(dynamic param);
    }
}
