using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement d'un VM de Gantt (grille ou pas).
    /// </summary>
    /// <typeparam name="TComponentItem">Le type de composant utilisé pour le Gantt.</typeparam>
    /// <typeparam name="TActionItem">Le type de composant pour une action.</typeparam>
    /// <typeparam name="TResourceItem">Le type de composant pour une ressource.</typeparam>
    public interface IGanttGridViewModel<TComponentItem, TActionItem, TResourceItem> : IViewModel, IFrameContentViewModel
    {

        /// <summary>
        /// Obtient les éléments représentant des actions, utilisés par le Gantt.
        /// </summary>
        ObservableCollection<TComponentItem> ActionItems { get; }

        /// <summary>
        /// Obtient une valeur indiquant si la sélection peut être changée.
        /// </summary>
        bool CanChange { get; }

        /// <summary>
        /// Obtient les catégories.
        /// </summary>
        BulkObservableCollection<ActionCategory> Categories { get; }

        /// <summary>
        /// Obtient les consommables.
        /// </summary>
        BulkObservableCollection<Ref2> Ref2s { get; }

        /// <summary>
        /// Obtient ou définit l'action courante
        /// </summary>
        TActionItem CurrentActionItem { get; }

        /// <summary>
        /// Obtient ou définit l'élément courant de la grille.
        /// </summary>
        TComponentItem CurrentGridItem { get; set; }

        /// <summary>
        /// Obtient les libellés des champs libres.
        /// </summary>
        CustomFieldsLabels CustomFieldsLabels { get; }

        /// <summary>
        /// Obtient les documents.
        /// </summary>
        BulkObservableCollection<Ref4> Ref4s { get; }

        /// <summary>
        /// Obtient la visibilité de l'indicateur d'attente de la grille.
        /// </summary>
        Visibility GridWaitVisibility { get; }

        /// <summary>
        /// Obtient une valeur indiquant si le scénario courant n'est pas en lecture seule.
        /// </summary>
        bool IsNotReadOnly { get; }

        /// <summary>
        /// Obtient une valeur indiquant si le scénario courant est en lecture seule.
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// Obtient la commande permettant de déplacer l'élément sélectionné vers le bas.
        /// </summary>
        ICommand MoveDownCommand { get; }

        /// <summary>
        /// Obtient la commande permettant de déplacer l'élément sélectionné vers le haut.
        /// </summary>
        ICommand MoveUpCommand { get; }

        /// <summary>
        /// Obtient les lieux.
        /// </summary>
        BulkObservableCollection<Ref3> Ref3s { get; }

        /// <summary>
        /// Obtient la liste des éléments sélectionnés
        /// </summary>
        BulkObservableCollection<TComponentItem> SelectedItems { get; }

        /// <summary>
        /// Obtient les outils.
        /// </summary>
        BulkObservableCollection<Ref1> Ref1s { get; }

        /// <summary>
        /// Obtient la vue actuelle.
        /// </summary>
        GanttGridView View { get; }

        /// <summary>
        /// Obtient ou définit la vue sélectionnée.
        /// </summary>
        GanttGridViewContainer ViewContainer { get; set; }

        /// <summary>
        /// Obtient les vues disponibles.
        /// </summary>
        GanttGridViewContainer[] Views { get; }

    }
}
