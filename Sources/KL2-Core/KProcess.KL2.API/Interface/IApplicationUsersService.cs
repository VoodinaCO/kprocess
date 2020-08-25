using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API
{
    /// <summary>
    /// Definit le comportement d'un service de gestion des utilisateurs de l'application.
    /// </summary>
    public interface IApplicationUsersServiceController
    {
        Task<IHttpActionResult> GetUsersAndRolesAndLanguages();

        Task<IHttpActionResult> SaveUsers(dynamic param);
        Task<IHttpActionResult> SaveTeams(dynamic param);
    }
}