using KProcess.Business;
using KProcess.Ksmed.Business.Dtos.Export;
using KProcess.Ksmed.Models;
using System.IO;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Definit le comportement d'un service d'import/export.
    /// </summary>
    public interface IImportExportService : IBusinessService
    {

        /// <summary>
        /// Exporte un projet.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        Task<Stream> ExportProject(int projectId);

        /// <summary>
        /// Prédit les référentiels potentiels à merger.
        /// </summary>
        /// <param name="data">Les données contenant l'export.</param>
        Task<ProjectImport> PredictMergedReferentialsProject(byte[] data);

        /// <summary>
        /// Importe un projet.
        /// </summary>
        /// <param name="import">Les données à importer.</param>
        /// <param name="mergeReferentials"><c>true</c> pour fusionner les référentiels.</param>
        /// <param name="videosDirectory">Le dossier qui contient les vidéos.</param>
        Task<Project> ImportProject(ProjectImport import, bool mergeReferentials, string videosDirectory);



        /// <summary>
        /// Exporte la décomposition d'une vidéo d'un scénario.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="scenarioId">L'identifiant du scénario.</param>
        /// <param name="videoId">L'identifiant de la vidéo.</param>
        Task<Stream> ExportVideoDecomposition(int projectId, int scenarioId, int videoId);

        /// <summary>
        /// Prédit les référentiels potentiels à merger.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet</param>
        /// <param name="stream">Le flux de données contenant l'export.</param>
        Task<VideoDecompositionImport> PredictMergedReferentialsVideoDecomposition(int projectId, Stream stream);

        /// <summary>
        /// Importe décomposition d'une vidéo d'un scénario dans le scénario initial existant d'un projet.
        /// </summary>
        /// <param name="videoDecomposition">Les données à importer.</param>
        /// <param name="mergeReferentials"><c>true</c> pour fusionner les référentiels.</param>
        /// <param name="videosDirectory">Le dossier qui contient les vidéos.</param>
        /// <param name="targetProjectId">L'identifiant du projet cible.</param>
        Task<bool> ImportVideoDecomposition(VideoDecompositionImport videoDecomposition, bool mergeReferentials, string videosDirectory, int targetProjectId);

    }
}