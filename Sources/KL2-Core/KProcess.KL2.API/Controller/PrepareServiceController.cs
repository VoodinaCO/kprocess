using KProcess.KL2.API.App_Start;
using KProcess.KL2.API.Authentication;
using KProcess.KL2.SignalRClient;
using KProcess.KL2.SignalRClient.Context;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API.Controller
{
    [SettingUserContextFilter]
    [RoutePrefix("Services/PrepareService")]
    public class PrepareServiceController : ApiController, IPrepareServiceController
    {
        private readonly ITraceManager _traceManager;
        readonly IPrepareService _prepareService;
        readonly ISecurityContext _securityContext;
        readonly IKLAnalyzeRepository _analyzeRepository;

        /// <summary>
        /// PrepareServiceController ctors
        /// </summary>
        /// <param name="prepareService"></param>
        /// <param name="securityContext"></param>
        /// <param name="analyzeRepository"></param>
        public PrepareServiceController(ITraceManager traceManager, IPrepareService prepareService, ISecurityContext securityContext, IKLAnalyzeRepository analyzeRepository)
        {
            _traceManager = traceManager;
            _prepareService = prepareService;
            _securityContext = securityContext;
            _analyzeRepository = analyzeRepository;
        }

        /// <summary>
        /// True si une publication existe pour le process.
        /// </summary>
        [HttpPost]
        [Route("PublicationExistsForProcess")]
        [Authorize]
        public async Task<IHttpActionResult> PublicationExistsForProcess([DynamicBody]dynamic param)
        {
            try
            {
                int processId = (int)param.processId;
                return Ok(await _prepareService.PublicationExistsForProcess(processId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// True si une publication existe pour le process en sync.
        /// </summary>
        [HttpPost]
        [Route("PublicationExistsForProcessSync")]
        [Authorize]
        public IHttpActionResult PublicationExistsForProcessSync([DynamicBody]dynamic param)
        {
            try
            {
                int processId = (int)param.processId;
                return Ok(_prepareService.PublicationExistsForProcessSync(processId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("GetPublication")]
        [Authorize]
        public async Task<IHttpActionResult> GetPublication([FromBody]dynamic param)
        {
            try
            {
                Guid publicationId = Guid.Parse((string)param.publicationId);
                return Ok(await _prepareService.GetPublication(publicationId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("GetTrainingPublication")]
        [Authorize]
        public async Task<IHttpActionResult> GetTrainingPublication([FromBody]dynamic param)
        {
            try
            {
                Guid evaluationPublicationId = Guid.Parse((string)param.evaluationPublicationId);
                return Ok(await _prepareService.GetTrainingPublication(evaluationPublicationId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("GetEvaluationPublication")]
        [Authorize]
        public async Task<IHttpActionResult> GetEvaluationPublication([FromBody]dynamic param)
        {
            try
            {
                Guid trainingPublicationId = Guid.Parse((string)param.trainingPublicationId);
                return Ok(await _prepareService.GetEvaluationPublication(trainingPublicationId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("GetTrainingPublications")]
        [Authorize]
        public async Task<IHttpActionResult> GetTrainingPublications([FromBody]dynamic param)
        {
            try
            {
                Guid[] evaluationPublicationIds = ((string[])param.evaluationPublicationIds).Select(_ => Guid.Parse(_)).ToArray();
                return Ok(await _prepareService.GetTrainingPublications(evaluationPublicationIds));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("GetEvaluationPublications")]
        [Authorize]
        public async Task<IHttpActionResult> GetEvaluationPublications([FromBody]dynamic param)
        {
            try
            {
                Guid[] trainingPublicationIds = ((string[])param.trainingPublicationIds).Select(_ => Guid.Parse(_)).ToArray();
                return Ok(await _prepareService.GetEvaluationPublications(trainingPublicationIds));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("GetLightPublication")]
        [Authorize]
        public async Task<IHttpActionResult> GetLightPublication([FromBody]dynamic param)
        {
            try
            {
                Guid publicationId = Guid.Parse((string)param.publicationId);
                return Ok(await _prepareService.GetLightPublication(publicationId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient la dernière publication d'un process.
        /// </summary>
        [HttpPost]
        [Route("GetLastPublication")]
        [Authorize]
        public async Task<IHttpActionResult> GetLastPublication([DynamicBody]dynamic param)
        {
            try
            {
                int processId = (int)param.processId;
                return Ok(await _prepareService.GetLastPublication(processId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient la dernière publication d'un process avec le mode de publication adéquat.
        /// </summary>
        [HttpPost]
        [Route("GetLastPublicationFiltered")]
        [Authorize]
        public async Task<IHttpActionResult> GetLastPublicationFiltered([DynamicBody]dynamic param)
        {
            try
            {
                int processId = (int)param.processId;
                int publishModeFilter = (int)param.publishModeFilter;
                return Ok(await _prepareService.GetLastPublicationFiltered(processId, publishModeFilter));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient si l'utilisateur donné possède un audit ouvert dans la publication donnée.
        /// </summary>
        [HttpPost]
        [Route("CheckAuditorHaveActiveAudit")]
        [Authorize]
        public async Task<IHttpActionResult> CheckAuditorHaveActiveAudit([DynamicBody]dynamic param)
        {
            try
            {
                int auditorId = (int)param.auditorId;
                Guid? publicationId = null;
                if (!string.IsNullOrEmpty((string)param.publicationId))
                    publicationId = Guid.Parse((string)param.publicationId);
                return Ok(await _prepareService.CheckAuditorHaveActiveAudit(auditorId, publicationId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient la publication d'un process avec un audit ouvert pour l'utilisateur donné.
        /// </summary>
        [HttpPost]
        [Route("GetPublicationToAudit")]
        [Authorize]
        public async Task<IHttpActionResult> GetPublicationToAudit([DynamicBody]dynamic param)
        {
            try
            {
                int auditorId = (int)param.auditorId;
                return Ok(await _prepareService.GetPublicationToAudit(auditorId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient un booléen qui indique si tous les process liés d'un scénario (si ils existent) ont été publiés.
        /// </summary>
        [HttpPost]
        [Route("AllLinkedProcessArePublished")]
        [Authorize]
        public async Task<IHttpActionResult> AllLinkedProcessArePublished([DynamicBody]dynamic param)
        {
            try
            {
                int scenarioId = (int)param.scenarioId;
                return Ok(await _prepareService.AllLinkedProcessArePublished(scenarioId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient un booléen qui indique si tous les process liés d'un scénario (si ils existent) ont été publiés. (SYNC)
        /// </summary>
        [HttpPost]
        [Route("AllLinkedProcessArePublishedSync")]
        [Authorize]
        public IHttpActionResult AllLinkedProcessArePublishedSync([DynamicBody]dynamic param)
        {
            try
            {
                int scenarioId = (int)param.scenarioId;
                return Ok(_prepareService.AllLinkedProcessArePublishedSync(scenarioId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("SetReadPublication")]
        [Authorize]
        public async Task<IHttpActionResult> SetReadPublication([DynamicBody]dynamic param)
        {
            try
            {
                Guid publicationId = Guid.Parse((string)param.publicationId);
                int UserId = (int)param.UserId;
                DateTime? ReadingDate = param.ReadingDate;
                await _prepareService.SetReadPublication(publicationId, UserId, ReadingDate);
                return Ok();
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient un projet
        /// </summary>
        [HttpPost]
        [Route("GetProject")]
        [Authorize]
        public async Task<IHttpActionResult> GetProject([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                return Ok(await _prepareService.GetProject(projectId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient un process
        /// </summary>
        [HttpPost]
        [Route("GetProcess")]
        [Authorize]
        public async Task<IHttpActionResult> GetProcess([DynamicBody]dynamic param)
        {
            try
            {
                int processId = (int)param.processId;
                bool includeAllInformations = param.includeAllInformations;
                return Ok(await _prepareService.GetProcess(processId, includeAllInformations));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient un projet en sync
        /// </summary>
        [HttpPost]
        [Route("GetProjectSync")]
        [Authorize]
        public IHttpActionResult GetProjectSync([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                return Ok(_prepareService.GetProjectSync(projectId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient l'arborescence des process ayant une publication
        /// </summary>
        [HttpPost]
        [Route("GetPublicationsTree")]
        [Authorize]
        public async Task<IHttpActionResult> GetPublicationsTree([DynamicBody]dynamic param)
        {
            try
            {
                PublishModeEnum filter = (PublishModeEnum)param.filter;
                return Ok(await _prepareService.GetPublicationsTree(filter));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient les projets et les objectifs
        /// </summary>
        [HttpPost]
        [Route("GetProjects")]
        [Authorize]
        public async Task<IHttpActionResult> GetProjects()
        {
            try
            {
                return Ok(await _prepareService.GetProjects());
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// List all videos to sync.
        /// </summary>
        [HttpPost]
        [Route("ListAllVideosToSync")]
        [Authorize]
        public async Task<IHttpActionResult> ListAllVideosToSync([DynamicBody]dynamic param)
        {
            try
            {
                int[] processIds = (int[])param.processIds;
                return Ok(await _prepareService.ListAllVideosToSync(processIds));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Get if a video can be unsynced.
        /// </summary>
        [HttpPost]
        [Route("CanBeUnSync")]
        [Authorize]
        public async Task<IHttpActionResult> CanBeUnSync([DynamicBody]dynamic param)
        {
            try
            {
                string videoHash = (string)param.videoHash;
                return Ok(await _prepareService.CanBeUnSync(videoHash));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient les dossiers
        /// </summary>
        [HttpPost]
        [Route("GetProjectDirs")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetProjectDirs()
        {
            try
            {
                return Ok(await _prepareService.GetProjectDirs());
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient les process
        /// </summary>
        [HttpPost]
        [Route("GetProcesses")]
        [Authorize]
        public async Task<IHttpActionResult> GetProcesses()
        {
            try
            {
                return Ok(await _prepareService.GetProcesses());
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient si un process est lié à une tâche.
        /// </summary>
        [HttpPost]
        [Route("ProcessIsLinkedToATask")]
        [Authorize]
        public async Task<IHttpActionResult> ProcessIsLinkedToATask([DynamicBody]dynamic param)
        {
            try
            {
                int processId = (int)param.processId;
                return Ok(await _prepareService.ProcessIsLinkedToATask(processId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient les noms avec extension d'une liste de fichiers.
        /// </summary>
        [HttpPost]
        [Route("GetFullName")]
        [Authorize]
        public async Task<IHttpActionResult> GetFullName(dynamic param)
        {
            try
            {
                string[] fileHashes = param.fileHashes;
                return Ok(await _prepareService.GetFullName(fileHashes));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Retrieve all the projects
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveProject")]
        [Authorize]
        public async Task<IHttpActionResult> SaveProject([DynamicBody]dynamic param)
        {
            try
            {
                Project project = param.project;
                var content = await _prepareService.SaveProject(project);
                await _analyzeRepository.RefreshProcess(new AnalyzeEventArgs(nameof(SaveProject), content, TargetAnalyze.Project));
                return Ok(content);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Retrieve all the projects
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveFolder")]
        [Authorize]
        public async Task<IHttpActionResult> SaveFolder([DynamicBody]dynamic param)
        {
            try
            {
                ProjectDir folder = param.folder;
                var content = await _prepareService.SaveFolder(folder);
                await _analyzeRepository.RefreshProcess(new AnalyzeEventArgs(nameof(SaveFolder), content, TargetAnalyze.ProjectDir));
                return Ok(content);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Retrieve all the projects
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveProcess")]
        [Authorize]
        public async Task<IHttpActionResult> SaveProcess([DynamicBody]dynamic param)
        {
            try
            {
                Procedure process = param.process;
                bool notifyChanges = param.notifyChanges;
                var content = await _prepareService.SaveProcess(process, notifyChanges);
                if (notifyChanges)
                    await _analyzeRepository.RefreshProcess(new AnalyzeEventArgs(nameof(SaveProcess), content, TargetAnalyze.Process));
                return Ok(content);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient les raisons possibles d'une non qualification.
        /// </summary>
        [HttpPost]
        [Route("GetQualificationReasons")]
        [Authorize]
        public async Task<IHttpActionResult> GetQualificationReasons()
        {
            try
            {
                return Ok(await _prepareService.GetQualificationReasons());
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Sauvegarde les raisons possibles d'une non qualification.
        /// </summary>
        [HttpPost]
        [Route("SaveQualificationReasons")]
        [Authorize]
        public async Task<IHttpActionResult> SaveQualificationReasons([DynamicBody]dynamic param)
        {
            try
            {
                IEnumerable<QualificationReason> reasons = (IEnumerable<QualificationReason>)param.reasons;
                return Ok(await _prepareService.SaveQualificationReasons(reasons));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Retrieve all members of a process
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetMembers")]
        [Authorize]
        public async Task<IHttpActionResult> GetMembers([DynamicBody]dynamic param)
        {
            try
            {
                int processId = (int)param.processId;
                return Ok(await _prepareService.GetMembers(processId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Save members
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveMember")]
        [Authorize]
        public async Task<IHttpActionResult> SaveMember([DynamicBody]dynamic param)
        {
            try
            {
                int processId = (int)param.processId;
                User member = param.member;
                return Ok(await _prepareService.SaveMember(processId, member));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Get referentials
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetReferentials")]
        [Authorize]
        public async Task<IHttpActionResult> GetReferentials([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                return Ok(await _prepareService.GetReferentials(projectId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Save referentials
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveReferentials")]
        [Authorize]
        public async Task<IHttpActionResult> SaveReferentials([DynamicBody]dynamic param)
        {
            try
            {
                Project project = param.project;
                return Ok(await _prepareService.SaveReferentials(project));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient toutes les ressources liées au process spécifié.
        /// </summary>
        /// <param name="param">L'identifiant du process.</param>
        [HttpPost]
        [Route("GetAllResources")]
        [Authorize]
        public async Task<IHttpActionResult> GetAllResources([DynamicBody]dynamic param)
        {
            try
            {
                int processId = (int)param.processId;
                return Ok(await _prepareService.GetAllResources(processId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient une vidéo ayant la même vidéo d'origine si elle existe.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetSameOriginalVideo")]
        [Authorize]
        public async Task<IHttpActionResult> GetSameOriginalVideo([DynamicBody]dynamic param)
        {
            try
            {
                string originalHash = (string)param.originalHash;
                return Ok(await _prepareService.GetSameOriginalVideo(originalHash));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient la vidéo.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetVideo")]
        [Authorize]
        public async Task<IHttpActionResult> GetVideo([DynamicBody]dynamic param)
        {
            try
            {
                int videoId = (int)param.videoId;
                return Ok(await _prepareService.GetVideo(videoId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Get videos
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetVideos")]
        [Authorize]
        public async Task<IHttpActionResult> GetVideos([DynamicBody]dynamic param)
        {
            try
            {
                int processId = (int)param.processId;
                return Ok(await _prepareService.GetVideos(processId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Save videos
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveVideo")]
        [Authorize]
        public async Task<IHttpActionResult> SaveVideo([DynamicBody]dynamic param)
        {
            try
            {
                Video video = param.video;
                return Ok(await _prepareService.SaveVideo(video));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Get scenarios
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetScenarios")]
        [Authorize]
        public async Task<IHttpActionResult> GetScenarios([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                return Ok(await _prepareService.GetScenarios(projectId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Get scenario
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("GetScenario")]
        [Authorize]
        public async Task<IHttpActionResult> GetScenario([DynamicBody]dynamic param)
        {
            try
            {
                int scenarioId = (int)param.scenarioId;
                return Ok(await _prepareService.GetScenario(scenarioId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Create initial scenarios
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateInitialScenario")]
        [Authorize]
        public async Task<IHttpActionResult> CreateInitialScenario([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                return Ok(await _prepareService.CreateInitialScenario(projectId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Create scenarios
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateScenario")]
        [Authorize]
        public async Task<IHttpActionResult> CreateScenario([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                Scenario sourceScenario = param.sourceScenario;
                bool keepVideoForUnchanged = param.keepVideoForUnchanged;
                return Ok(await _prepareService.CreateScenario(projectId, sourceScenario, keepVideoForUnchanged));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Delete scenario
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("DeleteScenario")]
        [Authorize]
        public async Task<IHttpActionResult> DeleteScenario([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                Scenario scenario = param.scenario;
                var content = await _prepareService.DeleteScenario(projectId, scenario);
                await _analyzeRepository.RefreshProcess(new AnalyzeEventArgs(nameof(SaveScenario), scenario, TargetAnalyze.Scenario));
                return Ok(content);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        ///// <summary>
        ///// Save scenario
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("SaveScenario")]
        //[Authorize]
        //public async Task<IHttpActionResult> SaveScenario()
        //{
        //    string content = await Request.Content.ReadAsStringAsync();
        //    JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        //    dynamic param = JsonConvert.DeserializeObject(content, settings);

        //    int projectId = (int)param.projectId;
        //    Scenario scenario = param.scenario;
        //    var result = await _prepareService.SaveScenario(projectId, scenario);
        //    return Ok(result);
        //}


        /// <summary>
        /// Save scenario
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveScenario")]
        [Authorize]
        public async Task<IHttpActionResult> SaveScenario([DynamicBody] dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                Scenario scenario = param.scenario;
                var content = await _prepareService.SaveScenario(projectId, scenario);
                await _analyzeRepository.RefreshProcess(new AnalyzeEventArgs(nameof(SaveScenario), scenario, TargetAnalyze.Scenario));
                return Ok(content);
            }
            catch (BLLException ex) 
            {
                _traceManager.TraceError(ex, ex.Message);

                var message = JsonConvert.SerializeObject(ex);

                return BadRequest(message);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Create a new scenario
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateNewProjectFromValidatedScenario")]
        [Authorize]
        public async Task<IHttpActionResult> CreateNewProjectFromValidatedScenario([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                Scenario validatedScenario = param.validatedScenario;
                var content = await _prepareService.CreateNewProjectFromValidatedScenario(projectId, validatedScenario);
                var newProject = await _prepareService.GetProject(content);
                await _analyzeRepository.RefreshProcess(new AnalyzeEventArgs(nameof(CreateNewProjectFromValidatedScenario), newProject, TargetAnalyze.Project));
                return Ok(content);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Update scenario
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateScenarioPublicationGuid")]
        [Authorize]
        public async Task<IHttpActionResult> UpdateScenarioPublicationGuid([DynamicBody]dynamic param)
        {
            try
            {
                int scenarioId = (int)param.scenarioId;
                Guid publicationGuid = param.publicationGuid;
                await _prepareService.UpdateScenarioPublicationGuid(scenarioId, publicationGuid);
                return Ok();
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Retrieve the draft documentation for this scenario id
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("HasDocumentationDraft")]
        [Authorize]
        public async Task<IHttpActionResult> HasDocumentationDraft([DynamicBody]dynamic param)
        {
            try
            {
                int scenarioId = (int)param.scenarioId;
                return Ok(await _prepareService.HasDocumentationDraft(scenarioId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Retrieve the draft documentation for this scenario id
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("HasDocumentationDraftSync")]
        [Authorize]
        public IHttpActionResult HasDocumentationDraftSync([DynamicBody]dynamic param)
        {
            try
            {
                int scenarioId = (int)param.scenarioId;
                return Ok(_prepareService.HasDocumentationDraftSync(scenarioId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Retrieve the draft documentation for this project id and this scenario id
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetDocumentationDraft")]
        [Authorize]
        public async Task<IHttpActionResult> GetDocumentationDraft([DynamicBody]dynamic param)
        {
            try
            {
                int documentationDraftId = (int)param.documentationDraftId;
                return Ok(await _prepareService.GetDocumentationDraft(documentationDraftId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Retrieve the draft documentations for this project id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetLastDocumentationDraft")]
        [Authorize]
        public async Task<IHttpActionResult> GetLastDocumentationDraft([DynamicBody]dynamic param)
        {
            try
            {
                int processId = (int)param.processId;
                int scenarioId = (int)param.scenarioId;
                return Ok(await _prepareService.GetLastDocumentationDraft(processId, scenarioId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }


        #region Publication
        /// <summary>
        /// Sauvegarde la publication
        /// </summary>
        [HttpPost]
        [Route("SavePublication")]
        [Authorize]
        public async Task<IHttpActionResult> SavePublication([DynamicBody]dynamic param)
        {
            try
            {
                Publication publication = (Publication)param.publication;
                return Ok(await _prepareService.SavePublication(publication));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }
        #endregion

        #region Training
        /// <summary>
        /// Obtient la formation d'un utilisateur pour une publication
        /// </summary>
        [HttpPost]
        [Route("GetTraining")]
        [Authorize]
        public async Task<IHttpActionResult> GetTraining([DynamicBody]dynamic param)
        {
            try
            {
                Guid publicationId = Guid.Parse(param.publicationId);
                int userId = (int)param.userId;
                return Ok(await _prepareService.GetTraining(publicationId, userId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Sauvegarde la liste des formations
        /// </summary>
        [HttpPost]
        [Route("SaveTrainings")]
        [Authorize]
        public async Task<IHttpActionResult> SaveTrainings([DynamicBody]dynamic param)
        {
            try
            {
                Training[] trainings = (Training[])param.trainings;
                return Ok(await _prepareService.SaveTrainings(trainings));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }
        #endregion

        #region Inspection

        /// <summary>
        /// Méthode permettant de récupérer la dernière inspection d'une publication
        /// </summary>
        [HttpPost]
        [Route("GetLastInspection")]
        [Authorize]
        public async Task<IHttpActionResult> GetLastInspection([DynamicBody]dynamic param)
        {
            try
            {
                Guid publicationId = Guid.Parse(param.publicationId);
                return Ok(await _prepareService.GetLastInspection(publicationId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Get all inspections
        /// </summary>
        [HttpPost]
        [Route("GetInspections")]
        [Authorize]
        public async Task<IHttpActionResult> GetInspections([DynamicBody]dynamic param)
        {
            try
            {
                if (Guid.TryParse(param.publicationId, out Guid publicationId))
                    return Ok(await _prepareService.GetInspections(publicationId));
                return Ok(await _prepareService.GetInspections());
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Get all inspections schedule exclude inspections completed
        /// </summary>
        [HttpPost]
        [Route("GetInspectionSchedules")]
        [Authorize]
        public async Task<IHttpActionResult> GetInspectionSchedules([DynamicBody]dynamic param)
        {
            try
            {
                int? InspectionScheduleId = (int?)param.InspectionScheduleId;
                return Ok(await _prepareService.GetInspectionSchedules(InspectionScheduleId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Get all inspections schedule
        /// </summary>
        [HttpPost]
        [Route("GetInspectionSchedulesNonFilter")]
        [Authorize]
        public async Task<IHttpActionResult> GetInspectionSchedulesNonFilter([DynamicBody]dynamic param)
        {
            try
            {
                int? InspectionScheduleId = (int?)param.InspectionScheduleId;
                return Ok(await _prepareService.GetInspectionSchedulesNonFilter(InspectionScheduleId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Get all inspections schedule for timeslot
        /// </summary>
        [HttpPost]
        [Route("GetInspectionsScheduleForTimeslot")]
        [Authorize]
        public async Task<IHttpActionResult> GetInspectionsScheduleForTimeslot([DynamicBody]dynamic param)
        {
            try
            {
                int timeslotId = (int)param.timeslotId;
                return Ok(await _prepareService.GetInspectionsScheduleForTimeslot(timeslotId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Sauvegarde les inspections
        /// </summary>
        [HttpPost]
        [Route("SaveInspections")]
        [Authorize]
        public async Task<IHttpActionResult> SaveInspections([DynamicBody]dynamic param)
        {
            try
            {
                Inspection[] inspections = (Inspection[])param.inspections;
                return Ok(await _prepareService.SaveInspections(inspections));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Get all anomalies of an inspection
        /// </summary>
        [HttpPost]
        [Route("GetAnomalies")]
        [Authorize]
        public async Task<IHttpActionResult> GetAnomalies([DynamicBody]dynamic param)
        {
            try
            {
                int inspectionId = (int)param.inspectionId;
                return Ok(await _prepareService.GetAnomalies(inspectionId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        #endregion
    }
}
