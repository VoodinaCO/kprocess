using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.ViewModels.Restitution;
using KProcess.Presentation.Windows;
using KProcess.Presentation.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement du modèle de vue de l'écran de construction du scénario initial.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IBuildViewModel : IGanttGridViewModel<GanttChartItem, ActionGanttItem, ReferentialGanttItem>
    {

        /// <summary>
        /// Obtient le chemin à jouer dans le lecteur.
        /// </summary>
        ActionPath[] ActionsPath { get; }

        /// <summary>
        /// Obtient les types d'actions.
        /// </summary>
        ActionType[] ActionTypes { get; }

        /// <summary>
        /// Obtient les valorisations d'actions.
        /// </summary>
        ActionValue[] ActionValues { get; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les changements de durée sur les actions sont autorisés.
        /// </summary>
        bool AllowTimingsDurationChange { get; }

        /// <summary>
        /// Obtient une valeur indiquant si la vidéo de la ressource peut être jouée.
        /// </summary>
        bool CanPlayReferential { get; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'action sélectionnée peut être réduite.
        /// </summary>
        bool CanReduceCurrentActionItem { get; }

        /// <summary>
        /// Obtient la commande permettant de 
        /// </summary>
        ICommand ConvertToReducedCommand { get; }

        /// <summary>
        /// Obtient ou définit l'élément courant dans la lecture du chemin critique.
        /// </summary>
        ActionPath CurrentActionPath { get; set; }

        /// <summary>
        /// Obtient ou définit le scénario actuel.
        /// </summary>
        Scenario CurrentScenario { get; }

        /// <summary>
        /// Obtient ou définit le temps actuel.
        /// </summary>
        long CurrentTimelinePosition { get; set; }

        /// <summary>
        /// Obtient le validateur de création de liens.
        /// </summary>
        DependencyCreationValidator DependencyCreationValidator { get; }

        /// <summary>
        /// Obtient les filtres IES disponibles.
        /// </summary>
        IESFilter[] IESFilters { get; }

        /// <summary>
        /// Obtient la visibilité du filtre IES.
        /// </summary>
        Visibility IESFilterVisibility { get; }

        /// <summary>
        /// Détermine si le mode lier/délier les marquers est disponible
        /// </summary>
        bool IsMarkersLinkedModeEnabled { get; }

        /// <summary>
        /// Obtient les filtres sur le chart de charge.
        /// </summary>
        string[] LoadChartFilters { get; }

        /// <summary>
        /// Obtient la commande permettant de jouer le chemin critique.
        /// </summary>
        ICommand PlayCriticalPath { get; }

        /// <summary>
        /// Obtient la commande permettant de jouer l'action actuellement sélectionnée.
        /// </summary>
        ICommand PlayCurrentAction { get; }

        /// <summary>
        /// Obtient la commande permettant de lire les actions de la ressource sélectionnée.
        /// </summary>
        ICommand PlayReferentialCommand { get; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la réduction de l'action courante est visible.
        /// </summary>
        Visibility ReduceCurrentActionItemVisibility { get; }

        /// <summary>
        /// Obtient les niveaux de coûts des actions réduites.
        /// </summary>
        short[] ReducedActionCosts { get; }

        /// <summary>
        /// Obtient les niveaux de difficulté des actions réduites.
        /// </summary>
        short[] ReducedActionDifficulties { get; }

        /// <summary>
        /// Obtient les opérateurs.
        /// </summary>
        BulkObservableCollection<Operator> Operators { get; }

        /// <summary>
        /// Obtient les équipements.
        /// </summary>
        BulkObservableCollection<Equipment> Equipments { get; }

        /// <summary>
        /// Obtient la charge des ressources.
        /// </summary>
        BulkObservableCollection<ReferentialLoad> ResourcesLoad { get; }

        /// <summary>
        /// Obtient les états de scénario possibles.
        /// </summary>
        ScenarioState[] ScenarioStates { get; }

        /// <summary>
        /// Obtient ou définit le filtre IES défini.
        /// </summary>
        IESFilter SelectedIESFilter { get; set; }

        /// <summary>
        /// Obtient ou définit le filtre sur le chart de charge.
        /// </summary>
        string SelectedLoadChartFilter { get; set; }

        /// <summary>
        /// Obtient la commande permettant de sélectionner le GanttItem spécifié en argument.
        /// </summary>
        ICommand SelectGanttItemCommand { get; }

        /// <summary>
        /// Obtient ou définit les solutions disponibles.
        /// </summary>
        BulkObservableCollection<string> Solutions { get; set; }

        /// <summary>
        /// Obtient la visibilité des solutions.
        /// </summary>
        Visibility SolutionsVisibility { get; }

        /// <summary>
        /// Obtient les solutions.
        /// </summary>
        SolutionWrapper[] SolutionsWrappers { get; }

        /// <summary>
        /// Obtient la durée totale du process.
        /// </summary>
        long TotalDuration { get; }

    }
}
