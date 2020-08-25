using KProcess.Ksmed.Models;
using System.Collections.Generic;

namespace KProcess.Ksmed.Business.Dtos.Export
{
    /// <summary>
    /// Représente la décompo vidéo à importer.
    /// </summary>
    public class VideoDecompositionImport
    {
        /// <summary>
        /// Obtient ou définit la décomposition vidéo exportée.
        /// </summary>
        public VideoDecompositionExport ExportedVideoDecomposition { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels projets candidats à la fusion.
        /// </summary>
        public IDictionary<IActionReferentialProcess, IActionReferential> ProjectReferentialsMergeCandidates { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels standards candidats à la fusion.
        /// </summary>
        public IDictionary<IActionReferential, IActionReferential> StandardReferentialsMergeCandidates { get; set; }
    }
}
