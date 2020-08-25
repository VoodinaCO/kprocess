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
    /// Définit le comportement du modèle de vue de l'écran Validation - Comparer.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IValidateSimulateViewModel : ISimulateViewModel
    {

        /// <summary>
        /// Obtient une valeur indiquant si la sélection peut être changée.
        /// </summary>
        bool CanChange { get; }

        /// <summary>
        /// Obtient une valeur indiquant si le scénario courant n'est pas en lecture seule.
        /// </summary>
        bool IsNotReadOnly { get; }

        /// <summary>
        /// Obtient une valeur indiquant si le scénario courant est en lecture seule.
        /// </summary>
        bool IsReadOnly { get; }

    }
}
