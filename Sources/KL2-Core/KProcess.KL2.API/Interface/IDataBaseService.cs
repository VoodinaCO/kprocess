using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API
{
    /// <summary>
    /// Définit le comportement d'un service de gestion de la base de données.
    /// </summary>
    public interface IDataBaseServiceController
    {
        Task<IHttpActionResult> Backup(dynamic param);

        Task<IHttpActionResult> Restore(dynamic param);

        Task<IHttpActionResult> GeBackupDir();

        Task<IHttpActionResult> GetVersion();

        Task<IHttpActionResult> Upgrade(dynamic param);

        Task<IHttpActionResult> SetDataBaseVersion(dynamic param);

        Task<IHttpActionResult> IsLocalDb();
    }
}
