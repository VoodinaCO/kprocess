using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Décrit le comportement d'un VM de vue par ressource pour la synthèse.
    /// </summary>
    /// <typeparam name="TReferential">Le type du référentiel.</typeparam>
    public interface IRestitutionViewByResourceViewModel<TReferential> : ISubRestitutionViewModel
        where TReferential : IActionReferential
    {
        /// <summary>
        /// Obtient ou définit une valeur indiquant si la vue et les données sont relatives.
        /// </summary>
        RestitutionValueMode SelectedValueMode { get; set; }

        /// <summary>
        /// Obtient ou définit l'index de la vue sélectionnée.
        /// </summary>
        int SelectedViewIndex { get; set; }
        
        /// <summary>
        /// Obtient ou définit la ressource sélectionnée.
        /// </summary>
        Resource SelectedResource { get; set; }
    }
}
