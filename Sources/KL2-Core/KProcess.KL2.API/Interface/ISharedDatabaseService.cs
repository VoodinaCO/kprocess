using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API
{
    /// <summary>
    /// Définit le comportement du service de gestion de verrous en base de données partagée.
    /// </summary>
    public interface ISharedDatabaseServiceController
    {
        Task<IHttpActionResult> IsLocked(dynamic param);

        Task<IHttpActionResult> UpdateLock(dynamic param);

        Task<IHttpActionResult> ReleaseLock(dynamic param);
    }
}