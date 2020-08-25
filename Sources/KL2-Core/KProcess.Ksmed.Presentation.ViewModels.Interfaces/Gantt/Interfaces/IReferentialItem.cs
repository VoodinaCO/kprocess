using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Définit le comportement d'un élément du Gantt ou de la grille associé à un référentiel
    /// </summary>
    public interface IReferentialItem : IGridItem
    {

        /// <summary>
        /// Obtient le référentiel.
        /// </summary>
        IActionReferential Referential { get; }

    }
}
