using KProcess.KL2.API.App_Start;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos.Export;
using KProcess.Ksmed.Security;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API.Controller
{
    [Authorize]
    [RoutePrefix("Services/ImportExportServiceController")]
    public class ImportExportServiceController : ApiController, IImportExportServiceController
    {
        private readonly ITraceManager _traceManager;
        private readonly IImportExportService _importExportService;
        private readonly ISecurityContext _securityContext;

        /// <summary>
        /// ImportExportServiceController ctors
        /// </summary>
        /// <param name="importExportService"></param>
        /// <param name="securityContext"></param>
        public ImportExportServiceController(ITraceManager traceManager, IImportExportService importExportService, ISecurityContext securityContext)
        {
            _traceManager = traceManager;
            _importExportService = importExportService;
            _securityContext = securityContext;
        }

        /// <summary>
        /// Exporte un projet.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("ExportProject")]
        public async Task<IHttpActionResult> ExportProject([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = param.projectId;
                var result = await _importExportService.ExportProject(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Prédit les référentiels potentiels à merger.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("PredictMergedReferentialsProject")]
        public async Task<IHttpActionResult> PredictMergedReferentialsProject([DynamicBody]dynamic param)
        {
            try
            {
                byte[] data = param.data;
                var result = await _importExportService.PredictMergedReferentialsProject(data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }


        /// <summary>
        /// Importe un projet.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("ImportProject")]
        public async Task<IHttpActionResult> ImportProject([DynamicBody]dynamic param)
        {
            try
            {
                ProjectImport import = param.allScenarios.ToObject<ProjectImport>();
                bool mergeReferentials = param.allScenarios.ToObject<bool>();
                string videosDirectory = param.allScenarios.ToObject<string>();

                await _importExportService.ImportProject(import, mergeReferentials, videosDirectory);
                return Ok();
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Exporte la décomposition d'une vidéo d'un scénario.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("ExportVideoDecomposition")]
        public async Task<IHttpActionResult> ExportVideoDecomposition([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = param.projectId;
                int scenarioId = param.scenarioId;
                int videoId = param.videoId;

                var result = await _importExportService.ExportVideoDecomposition(projectId, scenarioId, videoId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Prédit les référentiels potentiels à merger.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("PredictMergedReferentialsVideoDecomposition")]
        public async Task<IHttpActionResult> PredictMergedReferentialsVideoDecomposition([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = param.projectId;
                Stream stream = param.stream.ToObject<Stream>();
                var result = await _importExportService.PredictMergedReferentialsVideoDecomposition(projectId, stream);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Importe décomposition d'une vidéo d'un scénario dans le scénario initial existant d'un projet.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("ImportVideoDecomposition")]
        public async Task<IHttpActionResult> ImportVideoDecomposition([DynamicBody]dynamic param)
        {
            try
            {
                VideoDecompositionImport videoDecomposition = param.stream.ToObject<VideoDecompositionImport>();
                bool mergeReferentials = param.mergeReferentials;
                string videosDirectory = param.videosDirectory;
                int targetProjectId = param.targetProjectId;

                var result = await _importExportService.ImportVideoDecomposition(videoDecomposition, mergeReferentials, videosDirectory, targetProjectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }
    }
}
