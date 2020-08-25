using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement du modèle de vue de l'écran Publication - Scénario.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IPublishScenarioViewModel : IViewModel, IFrameContentViewModel
    {

        /// <summary>
        /// Obtient une valeur indiquant si la sélection peut être changée.
        /// </summary>
        bool CanChange { get; }

        /// <summary>
        /// Obtient ou définit le scénario courant.
        /// </summary>
        Scenario CurrentScenario { get; set; }

        /// <summary>
        /// Obtient les scénarios.
        /// </summary>
        BulkObservableCollection<Scenario> Scenarios { get; }

    }
}
