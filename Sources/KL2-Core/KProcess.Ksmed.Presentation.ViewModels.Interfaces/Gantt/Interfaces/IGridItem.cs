using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Définit le comportement d'un élément d'une grille.
    /// </summary>
    public interface IGridItem
    {
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'item est sélectionné.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si is l'item est activé.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'élément est un groupe.
        /// </summary>
        bool? IsGroup { get; set; }

        /// <summary>
        /// Obtient la couleur de la vidéo.
        /// </summary>
        Brush VideoColor { get; set; }

        /// <summary>
        /// Obtient ou définit le nom de la vidéo.
        /// </summary>
        string VideoName { get; set; }
    }
}
