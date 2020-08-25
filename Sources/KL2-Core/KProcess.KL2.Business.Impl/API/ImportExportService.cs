using KProcess.Business;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos.Export;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.API
{
    /// <summary>
    /// Représente un service d'importation/exportation.
    /// </summary>
    public class ImportExportService : IBusinessService, IImportExportService
    {
        readonly IAPIHttpClient _apiHttpClient;
        readonly ITraceManager _traceManager;
        readonly ISecurityContext _securityContext;

        public ImportExportService(IAPIHttpClient apiHttpClient, ISecurityContext securityContext, ITraceManager traceManager)
        {
            _apiHttpClient = apiHttpClient;
            _securityContext = securityContext;
            _traceManager = traceManager;
        }

        #region Export projet

        /// <summary>
        /// Exporte un projet.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<Stream> ExportProject(int projectId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                return await _apiHttpClient.ServiceAsync<Stream>(KL2_Server.API, nameof(ImportExportService), nameof(ExportProject), param);
            });

        /// <summary>
        /// Prédit les référentiels potentiels à merger.
        /// </summary>
        /// <param name="data">Les données contenant l'export.</param>
        public virtual async Task<ProjectImport> PredictMergedReferentialsProject(byte[] data) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.data = data;
                return await _apiHttpClient.ServiceAsync<ProjectImport>(KL2_Server.API, nameof(ImportExportService), nameof(PredictMergedReferentialsProject), param);
            });

        /// <summary>
        /// Importe un projet.
        /// </summary>
        /// <param name="import">Les données à importer.</param>
        /// <param name="mergeReferentials"><c>true</c> pour fusionner les référentiels.</param>
        /// <param name="videosDirectory">Le dossier qui contient les vidéos.</param>
        public virtual async Task<Project> ImportProject(ProjectImport import, bool mergeReferentials, string videosDirectory) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.import = import;
                param.mergeReferentials = mergeReferentials;
                param.videosDirectory = videosDirectory;
                return await _apiHttpClient.ServiceAsync<Project>(KL2_Server.API, nameof(ImportExportService), nameof(ImportProject), param);
            });

        #endregion

        #region Export video decomposition

        /// <summary>
        /// Exporte la décomposition d'une vidéo d'un scénario.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="scenarioId">L'identifiant du scénario.</param>
        /// <param name="videoId">L'identifiant de la vidéo.</param>
        public virtual async Task<Stream> ExportVideoDecomposition(int projectId, int scenarioId, int videoId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                param.scenarioId = scenarioId;
                param.videoId = videoId;
                return await _apiHttpClient.ServiceAsync<Stream>(KL2_Server.API, nameof(ImportExportService), nameof(ExportVideoDecomposition), param);
            });

        /// <summary>
        /// Prédit les référentiels potentiels à merger.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="stream">Le flux de données contenant l'export.</param>
        public virtual async Task<VideoDecompositionImport> PredictMergedReferentialsVideoDecomposition(int projectId, Stream stream) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                param.stream = stream;
                return await _apiHttpClient.ServiceAsync<VideoDecompositionImport>(KL2_Server.API, nameof(ImportExportService), nameof(PredictMergedReferentialsVideoDecomposition), param);
            });

        /// <summary>
        /// Importe décomposition d'une vidéo d'un scénario dans le scénario initial existant d'un projet.
        /// </summary>
        /// <param name="videoDecomposition">Les données à importer.</param>
        /// <param name="mergeReferentials"><c>true</c> pour fusionner les référentiels.</param>
        /// <param name="videosDirectory">Le dossier qui contient les vidéos.</param>
        /// <param name="targetProjectId">L'identifiant du projet cible.</param>
        public virtual async Task<bool> ImportVideoDecomposition(VideoDecompositionImport videoDecomposition, bool mergeReferentials, string videosDirectory, int targetProjectId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.videoDecomposition = videoDecomposition;
                param.mergeReferentials = mergeReferentials;
                param.videosDirectory = videosDirectory;
                param.targetProjectId = targetProjectId;
                return await _apiHttpClient.ServiceAsync<bool>(KL2_Server.API, nameof(ImportExportService), nameof(ImportVideoDecomposition), param);
            });

        #endregion
    }
}