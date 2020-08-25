using KProcess.Ksmed.Models;
using System.Collections.Generic;

namespace KProcess.Ksmed.Business.Dtos.Export
{
    /// <summary>
    /// Représente le projet à importer.
    /// </summary>
    public class ProjectImport
    {
        /// <summary>
        /// Obtient ou définit le projet exporté.
        /// </summary>
        public ProjectExport ExportedProject { get; set; }

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
