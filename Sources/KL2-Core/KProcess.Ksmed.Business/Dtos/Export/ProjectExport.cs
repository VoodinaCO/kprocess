using KProcess.Ksmed.Models;
using System.Runtime.Serialization;

namespace KProcess.Ksmed.Business.Dtos.Export
{
    /// <summary>
    /// Représente un projet exporté.
    /// </summary>
    [DataContract(IsReference = true, Namespace = ModelsConstants.DataContractNamespace)]
    public class ProjectExport
    {

        /// <summary>
        /// Obtient ou définit la version de l'application.
        /// </summary>
        [DataMember]
        public string AppVersion { get; set; }

        /// <summary>
        /// Obtient ou définit le projet.
        /// </summary>
        [DataMember]
        public Project Project { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels standard utilisés pas le projet.
        /// </summary>
        [DataMember]
        public IActionReferential[] ReferentialsStandard { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels projet du projet.
        /// </summary>
        [DataMember]
        public IActionReferentialProcess[] ReferentialsProject { get; set; }

    }
}
