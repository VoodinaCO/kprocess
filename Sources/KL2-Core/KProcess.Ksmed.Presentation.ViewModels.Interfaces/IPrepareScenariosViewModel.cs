using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement du modèle de vue de l'écran Preparation - Scénarios.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IPrepareScenariosViewModel : IViewModel, IFrameContentViewModel
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
        /// Obtient la commande permettant d'exporter la décomposition d'une vidéo.
        /// </summary>
        ICommand ExportVideoDecompositionCommand { get; }

        /// <summary>
        /// Obtient la commande permettant d'importer un projet.
        /// </summary>
        ICommand ImportVideoDecompositionCommand { get; }

        /// <summary>
        /// Obtient les différentes natures des scénarios.
        /// </summary>
        ScenarioNature[] Natures { get; }

        /// <summary>
        /// Obtient les scénarios.
        /// </summary>
        BulkObservableCollection<Scenario> Scenarios { get; }

        /// <summary>
        /// Obtient les états des scénarios.
        /// </summary>
        ScenarioState[] States { get; }

        /// <summary>
        /// Obtient la synthèse.
        /// </summary>
        ScenarioCriticalPath[] Summary { get; }

    }
}
