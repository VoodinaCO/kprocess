using KProcess.Ksmed.Models;
using System.Runtime.Serialization;

namespace KProcess.Ksmed.Business.Dtos.Export
{
    /// <summary>
    /// Représente la décomposition d'une vidéo exportée.
    /// </summary>
    [DataContract(IsReference = true, Namespace = Models.ModelsConstants.DataContractNamespace)]
    public class VideoDecompositionExport
    {
        /// <summary>
        /// Obtient ou définit la version de l'application.
        /// </summary>
        [DataMember]
        public string AppVersion { get; set; }

        /// <summary>
        /// Obtient ou définit la vidéo.
        /// </summary>
        [DataMember]
        public Video Video { get; set; }

        /// <summary>
        /// Obtient ou définit les actions.
        /// </summary>
        [DataMember]
        public KAction[] Actions { get; set; }

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

        /// <summary>
        /// Obtient le libellé du champ libre texte n° 1.
        /// </summary>
        [DataMember]
        public string CustomTextLabel { get; set; }

        /// <summary>
        /// Obtient le libellé du champ libre texte n° 2.
        /// </summary>
        [DataMember]
        public string CustomTextLabel2 { get; set; }

        /// <summary>
        /// Obtient le libellé du champ libre texte n° 3.
        /// </summary>
        [DataMember]
        public string CustomTextLabel3 { get; set; }

        /// <summary>
        /// Obtient le libellé du champ libre texte n° 4.
        /// </summary>
        [DataMember]
        public string CustomTextLabel4 { get; set; }

        /// <summary>
        /// Obtient le libellé du champ libre numérique n° 1.
        /// </summary>
        [DataMember]
        public string CustomNumericLabel { get; set; }

        /// <summary>
        /// Obtient le libellé du champ libre numérique n° 2.
        /// </summary>
        [DataMember]
        public string CustomNumericLabel2 { get; set; }

        /// <summary>
        /// Obtient le libellé du champ libre numérique n° 3.
        /// </summary>
        [DataMember]
        public string CustomNumericLabel3 { get; set; }

        /// <summary>
        /// Obtient le libellé du champ libre numérique n° 4.
        /// </summary>
        [DataMember]
        public string CustomNumericLabel4 { get; set; }

    }
}
