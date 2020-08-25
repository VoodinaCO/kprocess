using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API
{
    /// <summary>
    /// Definit le comprotement d'un service de gestion de la partie fonctionnelle "Analyser".
    /// </summary>
    public interface IAnalyzeServiceController
    {
        Task<IHttpActionResult> PredictImpactedScenarios(dynamic param);

        Task<IHttpActionResult> GetAcquireData(dynamic param);

        Task<IHttpActionResult> SaveAcquireData(dynamic param);

        Task<IHttpActionResult> GetBuildData(dynamic param);

        Task<IHttpActionResult> SaveBuildScenario(dynamic param);

        Task<IHttpActionResult> GetSimulateData(dynamic param);

        Task<IHttpActionResult> GetRestitutionData(dynamic param);

        Task<IHttpActionResult> GetFullProjectDetails(dynamic param);

        Task<IHttpActionResult> UpdateScenarioIsShownInSummary(dynamic param);
    }
}