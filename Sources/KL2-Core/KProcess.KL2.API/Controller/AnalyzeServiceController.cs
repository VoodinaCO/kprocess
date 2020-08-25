using KProcess.KL2.API.App_Start;
using KProcess.KL2.API.Authentication;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API.Controller
{
    [Authorize]
    [SettingUserContextFilter]
    [RoutePrefix("Services/AnalyzeService")]
    public class AnalyzeServiceController : ApiController, IAnalyzeServiceController
    {
        private readonly ITraceManager _traceManager;
        private readonly IAnalyzeService _analyzeService;
        private readonly ISecurityContext _securityContext;

        /// <summary>
        /// AnalyzeServiceController ctors
        /// </summary>
        /// <param name="analyzeService"></param>
        /// <param name="securityContext"></param>
        public AnalyzeServiceController(ITraceManager traceManager, IAnalyzeService analyzeService, ISecurityContext securityContext)
        {
            _traceManager = traceManager;
            _analyzeService = analyzeService;
            _securityContext = securityContext;
        }

        /// <summary>
        /// Prédit les scénarios qui seront impactés par les modifications en attente de sauvegarde.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("PredictImpactedScenarios")]
        public async Task<IHttpActionResult> PredictImpactedScenarios([DynamicBody]dynamic param)
        {
            try
            {
                Scenario sourceModifiedScenario = param.sourceModifiedScenario;
                Scenario[] allScenarios = param.allScenarios;
                KAction[] actionsToDelete = param.actionsToDelete;
                KAction[] actionsWithUpdatedWBS = param.actionsWithUpdatedWBS;

                return Ok(await _analyzeService.PredictImpactedScenarios(sourceModifiedScenario, allScenarios, actionsToDelete, actionsWithUpdatedWBS));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient les données pour l'écran Acquérir.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("GetAcquireData")]
        public async Task<IHttpActionResult> GetAcquireData([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                return Ok(await _analyzeService.GetAcquireData(projectId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Sauvegarde les actions spécifiées.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("SaveAcquireData")]
        public async Task<IHttpActionResult> SaveAcquireData([DynamicBody]dynamic param)
        {
            try
            {
                Scenario[] allScenarios = param.allScenarios;
                Scenario updatedScenario = param.updatedScenario;

                return Ok(await _analyzeService.SaveAcquireData(allScenarios, updatedScenario));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient les données pour l'écran Construire.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("GetBuildData")]
        public async Task<IHttpActionResult> GetBuildData([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                return Ok(await _analyzeService.GetBuildData(projectId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Sauvegarde le scénario spécifié.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("SaveBuildScenario")]
        public async Task<IHttpActionResult> SaveBuildScenario([DynamicBody]dynamic param)
        {
            try
            {
                Scenario[] allScenarios = param.allScenarios;
                Scenario updatedScenario = param.updatedScenario;

                var result = await _analyzeService.SaveBuildScenario(allScenarios, updatedScenario);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient les données pour l'écran Simuler.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("GetSimulateData")]
        public async Task<IHttpActionResult> GetSimulateData([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                return Ok(await _analyzeService.GetSimulateData(projectId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Obtient les données pour l'écran Construire.
        /// </summary>
        /// <param name="param">Le login de l'utilisateur.</param>
        [HttpPost]
        [Route("GetRestitutionData")]
        public async Task<IHttpActionResult> GetRestitutionData([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                return Ok(await _analyzeService.GetRestitutionData(projectId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient toutes les données du projet spécifié.
        /// </summary>
        /// <param name="param">Le login de l'utilisateur.</param>
        [HttpPost]
        [Route("GetFullProjectDetails")]
        public async Task<IHttpActionResult> GetFullProjectDetails([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                return Ok(await _analyzeService.GetFullProjectDetails(projectId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Met à jour l'affichage dans la synthèse pour le scénario spécifié.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("UpdateScenarioIsShownInSummary")]
        public async Task<IHttpActionResult> UpdateScenarioIsShownInSummary([DynamicBody]dynamic param)
        {
            try
            {
                int scenarioId = (int)param.scenarioId;
                bool isShownInSummary = param.isShownInSummary;

                return Ok(await _analyzeService.UpdateScenarioIsShownInSummary(scenarioId, isShownInSummary));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }
    }
}
