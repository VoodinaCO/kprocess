using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API
{
    /// <summary>
    /// Definit le comportement d'un service de gestion de la partie fonctionnelle "Valider".
    /// </summary>
    public interface IValidateServiceController
    {
        Task<IHttpActionResult> GetAcquireData(dynamic param);

        Task<IHttpActionResult> SaveAcquireData(dynamic param);

        Task<IHttpActionResult> GetBuildData(dynamic param);

        Task<IHttpActionResult> SaveBuildScenario(dynamic param);

        Task<IHttpActionResult> GetSimulateData(dynamic param);

        Task<IHttpActionResult> SaveSimulateData(dynamic param);

        Task<IHttpActionResult> GetRestitutionData(dynamic param);
    }
}