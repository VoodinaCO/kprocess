using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Définit le comportement d'un élément du Gantt ou de la grille associé à une action
    /// </summary>
    public interface IActionItem : IGridItem
    {
        /// <summary>
        /// Obtient ou définir le contenu.
        /// </summary>
        object Content { get; set; }

        /// <summary>
        /// Obtient l'action.
        /// </summary>
        KAction Action { get; }

        /// <summary>
        /// Obtient ou définit l'élément de réferentiel auquel appartient l'élément d'action.
        /// </summary>
        IReferentialItem ParentReferentialItem { get; set; }

        /// <summary>
        /// Obtient ou définit le prédécesseur définit lors de la création.
        /// </summary>
        IActionItem CreationPredecessor { get; set; }

        /// <summary>
        /// Obtient ou définit les prédécesseurs sous forme de chaîne de caractères.
        /// </summary>
        string PredecessorsString { get; set; }

        /// <summary>
        /// Survient lorsque la chaîne de prédécesseurs a changé.
        /// </summary>
        event EventHandler PredecessorsStringChanged;

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'action fait partie du chemin critique.
        /// </summary>
        bool IsCritical { get; set; }

        /// <summary>
        /// Obtient ou définit le gain de temps par rapport à l'original en pourcentage.
        /// </summary>
        double? OriginalGainPercentage { get; set; }

        /// <summary>
        /// Obtient ou définit la vignette de la vidéo.
        /// </summary>
        CloudFile Thumbnail { get; set; }

        /// <summary>
        /// Obtient les libellés concaténées des référentiels 1.
        /// </summary>
        string Ref1Labels { get; set; }

        /// <summary>
        /// Obtient les libellés concaténés des référentiels 2.
        /// </summary>
        string Ref2Labels { get; set; }

        /// <summary>
        /// Obtient les libellés concaténés des référentiels 3.
        /// </summary>
        string Ref3Labels { get; set; }

        /// <summary>
        /// Obtient les libellés concaténés des référentiels 4.
        /// </summary>
        string Ref4Labels { get; set; }

        /// <summary>
        /// Obtient les libellés concaténés des référentiels 5.
        /// </summary>
        string Ref5Labels { get; set; }

        /// <summary>
        /// Obtient les libellés concaténés des référentiels 6.
        /// </summary>
        string Ref6Labels { get; set; }

        /// <summary>
        /// Obtient les libellés concaténés des référentiels 7.
        /// </summary>
        string Ref7Labels { get; set; }

        /// <summary>
        /// Obtient le libellé ou le WBS de l'action associée.
        /// </summary>
        string LabelOrWBS { get; set; }

        /// <summary>
        /// Met à jour le contenu à partir de l'élément associé.
        /// </summary>
        void UpdateContent();
    }
}
