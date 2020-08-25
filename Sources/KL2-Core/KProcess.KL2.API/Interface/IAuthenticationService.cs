using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API
{
    /// <summary>
    /// Definit le comprotement d'un service d'authentification.
    /// </summary>
    public interface IAuthenticationServiceController
    {
        Task<IHttpActionResult> GetUser(dynamic param);

        Task<IHttpActionResult> IsUserValid(dynamic param);

        Task<IHttpActionResult> SaveUser(dynamic param);

    }

}
