using KProcess.Ksmed.Models.OlderVersions.v2200;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace KProcess.Ksmed.Business.Dtos.Export.OlderVersions.v2200
{
    /// <summary>
    /// Représente un projet exporté.
    /// </summary>
    [DataContract(IsReference = true, Namespace = Models.ModelsConstants.DataContractNamespace)]
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
        public object[] ReferentialsStandard { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels projet du projet.
        /// </summary>
        [DataMember]
        public object[] ReferentialsProject { get; set; }

    }
}
