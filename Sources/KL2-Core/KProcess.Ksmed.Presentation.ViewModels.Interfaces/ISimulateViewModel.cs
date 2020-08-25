using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement du modèle de vue des écrans Simuler.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface ISimulateViewModel : IViewModel, IFrameContentViewModel
    {

        /// <summary>
        /// Obtient les scénarios originaux disponibles.
        /// </summary>
        Scenario[] AvailableOriginalScenarios { get; }

        /// <summary>
        /// Obtient ou définit l'élément courant de la grille.
        /// </summary>
        GanttChartItem CurrentTargetGridItem { get; set; }

        /// <summary>
        /// Obtient ou définit le temps actuel.
        /// </summary>
        long CurrentTimelinePosition { get; set; }

        /// <summary>
        /// Obtient les libellés des champs libres.
        /// </summary>
        CustomFieldsLabels CustomFieldsLabels { get; }

        /// <summary>
        /// Obtient le décalage dans les scroll viewers.
        /// </summary>
        double GanttHorizontalScrollOffset { get; set; }

        /// <summary>
        /// Obtient les filtres IES disponibles.
        /// </summary>
        IESFilter[] IESFilters { get; }

        /// <summary>
        /// Obtient ou définit le filtre IES défini.
        /// </summary>
        IESFilter SelectedIESFilter { get; set; }

        /// <summary>
        /// Obtient les éléments du gantt du scénario original sélectionné.
        /// </summary>
        BulkObservableCollection<GanttChartItem> SelectedOriginalActionItems { get; }

        /// <summary>
        /// Obtient ou définit le scénario d'origine sélectionné.
        /// </summary>
        Scenario SelectedOriginalScenario { get; set; }

        /// <summary>
        /// Obtient les éléments d'actions de la cible sélectionnée.
        /// </summary>
        BulkObservableCollection<GanttChartItem> SelectedTargetActionItems { get; }

        /// <summary>
        /// Obtient ou définit le scénario cible sélectionné.
        /// </summary>
        Scenario SelectedTargetScenario { get; set; }

    }
}
