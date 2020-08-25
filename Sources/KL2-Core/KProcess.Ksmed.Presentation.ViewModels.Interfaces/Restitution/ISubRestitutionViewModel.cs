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
    /// Définit le comportement des modèles de vue des sous-écrans de restitution;
    /// </summary>
    [InheritedExportAsPerCall]
    public interface ISubRestitutionViewModel : IViewModel
    {
        /// <summary>
        /// Obtient ou définit le VM parent.
        /// </summary>
        IRestitutionViewModel ParentViewModel { get; set; }
        
        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la navigation est acceptée.
        /// </returns>
        bool OnNavigatingAway();

        /// <summary>
        /// Appelé lorsque la sélection des scénarios a changé.
        /// </summary>
        void OnScenariosSelectionChanged();
    }
}
