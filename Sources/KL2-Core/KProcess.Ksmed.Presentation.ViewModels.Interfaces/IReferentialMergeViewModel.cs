using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement du modèle de vue de l'écran de fusion des référentiels.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IReferentialMergeViewModel : IViewModel
    {
        /// <summary>
        /// Obtient ou définit les autres référentiels avec lesquels le principal peut être fusionné.
        /// </summary>
        IActionReferential[] Referentials { get; set;}

        /// <summary>
        /// Obtient ou définit le référentiel principal.
        /// </summary>
        IActionReferential MainReferential { get; set; }

        /// <summary>
        /// Obtient ou définit les régles de regroupement et de tri.
        /// </summary>
        ReferentialsGroupSortDescription GroupSort { get; set; }

        /// <summary>
        /// Obtient le résultat de la fenêtre.
        /// </summary>
        bool Result { get; }

    }
}
