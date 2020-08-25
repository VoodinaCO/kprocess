using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using DlhSoft.Windows.Controls;
using KProcess.Globalization;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Gère les actions pour le Gantt
    /// </summary>
    class GanttActionsManager : ActionsManager<GanttChartItem, ActionGanttItem, ReferentialGanttItem>
    {
        private BulkObservableCollection<GanttChartItem> _items;
        private PredecessorItemCollection _currentUpdatingPredecessors;
        private bool _useGanttItemsTimingsOnly;

        #region Initialisation

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="GanttActionsManager"/>.
        /// </summary>
        /// <param name="items">La collection d'élélments.</param>
        /// <param name="currentItemSetter">Un délégué capable de définir l'élément sélectionné.</param>
        /// <param name="videoGetter">Un délégué capable d'obtenir une vidéo à partir de son identifiant.</param>
        public GanttActionsManager(BulkObservableCollection<GanttChartItem> items, Action<GanttChartItem> currentItemSetter, Func<int, Video> videoGetter)
            : base(currentItemSetter, videoGetter)
        {
            _items = items;
            base.EnableGroupsTimingCoercion = true;
            base.EnableRessourceLoad = false;
            base.EnablePredecessorTimingFix = true;
            base.EnableReducedPercentageRefresh = true;
        }

        /// <summary>
        /// Enregistre des actions déjà existantes et construit les éléments.
        /// </summary>
        /// <param name="allActions">Toutes les actions.</param>
        /// <param name="filter">Les filtre I/E/S.</param>
        public void RegisterInitialActions(IEnumerable<KAction> allActions, IESFilterValue filter)
        {
            Func<ActionsDisplayResults> displayedActions;

            bool isCriticalPathEnabled;
            bool useManagedPredSucc;
            bool useGanttItemsTimingsOnly;
            bool fixPredSuccTimings;
            bool refreshTimings;

            foreach (var action in allActions)
            {
                action.IsGroup = WBSHelper.HasChildren(action, allActions);
                action.IsLinkToProcess = action.LinkedProcessId != null;
            }

            switch (filter)
            {
                case IESFilterValue.IES:
                    useManagedPredSucc = false;
                    isCriticalPathEnabled = true;
                    displayedActions = () => new ActionsDisplayResults { Actions = allActions };
                    useGanttItemsTimingsOnly = false;
                    fixPredSuccTimings = true;
                    refreshTimings = true;
                    break;

                case IESFilterValue.I:
                    useManagedPredSucc = true;
                    isCriticalPathEnabled = true;
                    displayedActions = () => FilterActionsUpdatePredSuccManaged(allActions,
                        a => ActionsTimingsMoveManagement.IsActionExternal(a) || ActionsTimingsMoveManagement.IsActionDeleted(a),
                        a => ActionsTimingsMoveManagement.IsActionInternal(a));
                    useGanttItemsTimingsOnly = true;
                    fixPredSuccTimings = true;
                    refreshTimings = false;
                    break;

                case IESFilterValue.IE:
                    useManagedPredSucc = true;
                    isCriticalPathEnabled = false;
                    displayedActions = () => FilterActionsUpdatePredSuccManaged(allActions,
                        a => ActionsTimingsMoveManagement.IsActionDeleted(a),
                        a => ActionsTimingsMoveManagement.IsActionExternal(a) || ActionsTimingsMoveManagement.IsActionInternal(a));
                    useGanttItemsTimingsOnly = true;
                    fixPredSuccTimings = true;
                    refreshTimings = false;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("filter");
            }

            _useGanttItemsTimingsOnly = useGanttItemsTimingsOnly;

            this.IsCriticalPathEnabled = isCriticalPathEnabled;

            var results = displayedActions();

            this.UseManagedPredecessorsSuccessors = useManagedPredSucc;
            if (filter == IESFilterValue.IES && useManagedPredSucc)
                ActionsTimingsMoveManagement.DebugCheckAllWBS(results.Actions);

            this.RegisterInitialActionsImpl(results.Actions);

            if (results.NewTimings != null)
            {
                foreach (var timing in results.NewTimings)
                {
                    SetBuildStart(timing.Action, timing.Start);
                    SetBuildFinish(timing.Action, timing.Finish);
                }
            }

            if (refreshTimings)
            {
                foreach (var item in this.ItemsOfTypeAction)
                {
                    _updatingitem = item;
                    item.Start = GanttDates.ToDateTime(item.Action.BuildStart);
                    item.Finish = GanttDates.ToDateTime(item.Action.BuildFinish);
                    _updatingitem = null;
                }
            }

            if (fixPredSuccTimings)
            {
                this.FixPredecessorsSuccessorsTimings();
                this.UpdateResourcesLoad();
            }
        }

        /// <summary>
        /// Enregistre des actions déjà existantes et construit les éléments.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        protected override void RegisterInitialActionsImpl(IEnumerable<KAction> actions)
        {
            base.RegisterInitialActionsImpl(actions);
            this.AddAllMissingDependencies();
        }

        /// <summary>
        /// Filtre les actions et met à jour leurs prédécesseurs et succésseurs managés.
        /// </summary>
        /// <param name="allActions">Toutes les actions.</param>
        /// <param name="excludedActionsFilter">Le filtre excluant les actions.</param>
        /// <param name="includedActionsPredSuccFilter">Le filtre incluant les actions prédécesseurs et succ.</param>
        /// <returns>Les actions filtrées.</returns>
        private ActionsDisplayResults FilterActionsUpdatePredSuccManaged(IEnumerable<KAction> allActions,
            Func<KAction, bool> excludedActionsFilter,
            Func<KAction, bool> includedActionsPredSuccFilter)
        {
            var results = new ActionsDisplayResults();
            var newTimings = new List<ActionTiming>();

            var actionsSorted = allActions
                .OrderBy(a => a.WBSParts, new WBSHelper.WBSComparer())
                .ToArray();

#if DEBUG
            foreach (var action in allActions)
            {
                // S'assurer qu'une action soit un seul de : I/E/S/Group
                var isI = ActionsTimingsMoveManagement.IsActionInternal(action);
                var isE = ActionsTimingsMoveManagement.IsActionExternal(action);
                var isS = ActionsTimingsMoveManagement.IsActionDeleted(action);
                var isG = action.IsGroup;

                if (!(isI | isE | isS | isG))
                    throw new InvalidOperationException("Impossible qu'une action soit à la fois I ou E ou S ou G");
            }
#endif

            var actionsFinal = new List<KAction>();

            foreach (var action in actionsSorted)
            {
                action.PredecessorsManaged.Clear();
                action.SuccessorsManaged.Clear();
            }

            foreach (var action in actionsSorted)
            {
                if (excludedActionsFilter(action))
                {
                    // Déplacer tous les pred vers ses succ
                    ActionsTimingsMoveManagement.MapAllPredToSucc(action, includedActionsPredSuccFilter, newTimings, true);
                }
                else if (!action.IsGroup)
                {
                    foreach (var pred in action.Predecessors)
                    {
                        if (includedActionsPredSuccFilter(pred))
                        {
                            pred.SuccessorsManaged.Add(action);
                            action.PredecessorsManaged.Add(pred);
                        }
                    }
                    actionsFinal.Add(action);
                }
                else
                {
                    System.Diagnostics.Debug.Assert(action.IsGroup);
                    actionsFinal.Add(action);
                }
            }

            // Supprimer les liens pour les actions non présentes dans la collection
            foreach (var action in actionsFinal)
            {
                foreach (var pred in action.PredecessorsManaged.ToArray())
                {
                    if (!actionsFinal.Contains(pred))
                    {
                        action.PredecessorsManaged.Remove(pred);
                        pred.SuccessorsManaged.Remove(action);
                    }
                }

                foreach (var succ in action.SuccessorsManaged.ToArray())
                {
                    if (!actionsFinal.Contains(succ))
                    {
                        action.SuccessorsManaged.Remove(succ);
                        succ.PredecessorsManaged.Remove(action);
                    }
                }
            }

            // Il faut automatiquement supprimer les groupes qui ne contiennent plus d'enfants
            // Les groupes n'ayant ni précédesseurs ni successeurs, on peut les supprimer sans réel impact
            var copy = actionsFinal.ToArray();
            foreach (var action in copy)
            {
                if (action.IsGroup && !WBSHelper.HasChildren(action, copy))
                    actionsFinal.Remove(action);
            }

            results.Actions = actionsFinal;
            results.NewTimings = newTimings;

            return results;
        }

        /// <summary>
        /// Corrige les temps en fonction des prédécesseurs et successeurs de chaque action.
        /// </summary>
        public override void FixPredecessorsSuccessorsTimings()
        {
            if (!EnablePredecessorTimingFix || (!AllowTimingsChange && !_useGanttItemsTimingsOnly) || IsFixingPredecessorsSuccessorsTiming)
                return;

            IsFixingPredecessorsSuccessorsTiming = true;
            ActionsTimingsMoveManagement.FixPredecessorsSuccessorsTimings(
                GetActionsSortedByWBS(),
                this.UseManagedPredecessorsSuccessors,
                this.GetBuildStart,
                this.GetBuildFinish,
                this.SetBuildStart,
                this.SetBuildFinish);
            IsFixingPredecessorsSuccessorsTiming = false;

            this.UpdateBuildGroupsTiming();
            this.UpdateCriticalPath();
        }


        /// <summary>
        /// Appelé lorsque la chaîne représentant les prédécesseurs a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.EventArgs"/> contenant les données de l'évènement.</param>
        protected override void OnItemPredecessorsStringChanged(object sender, EventArgs e)
        {
            if (this.UseManagedPredecessorsSuccessors)
                return;

            base.OnItemPredecessorsStringChanged(sender, e);
        }

        #endregion

        #region Surchages

        /// <summary>
        /// Obtient les éléments.
        /// </summary>
        protected override BulkObservableCollection<GanttChartItem> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Crée un nouvel élément représentant une action.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <returns>L'élément créé.</returns>
        protected override ActionGanttItem CreateNewActionItemImpl(KAction action)
        {
            ActionGanttItem item;

            if (this.IsViewByReferential())
                item = new ActionGanttItem(action, WBSHelper.IndentationFromWBS(action.WBS) + 1);
            else
                item = new ActionGanttItem(action, WBSHelper.IndentationFromWBS(action.WBS));

            item.CanResize = base.AllowTimingsDurationChange;

            Brush fillBrush, strokeBrush;
            GetActionBrushes(action, out fillBrush, out strokeBrush);

            item.FillBrush = fillBrush;
            item.StrokeBrush = strokeBrush;
            item.OrangeHeaderVisibility = ActionsTimingsMoveManagement.IsActionExternal(action) ? Visibility.Visible : Visibility.Collapsed;
            item.OrangeHeaderToolTip = LocalizationManager.GetString("VM_ActionManager_ExternalToolTip");
            item.GreenHeaderToolTip = LocalizationManager.GetString("VM_ActionManager_NewToolTip");

            return item;
        }

        /// <inheritdoc />
        protected override ReferentialGanttItem CreateNewReferentialItemImpl(IActionReferential referential, string label)
        {
            return new ReferentialGanttItem(referential, label);
        }

        private bool _isUpdatingGroupsTimings;
        /// <summary>
        /// Met à jour le début, la durée et la fin sur les éléments de groupe
        /// </summary>
        protected override void UpdateBuildGroupsTiming()
        {
            if (!EnableGroupsTimingCoercion || (!AllowTimingsChange && !_useGanttItemsTimingsOnly) || _isUpdatingGroupsTimings)
                return;

            _isUpdatingGroupsTimings = true;
            ActionsTimingsMoveManagement.UpdateBuildGroupsTiming(GetActionsSortedByWBS(),
                GetBuildStart, GetBuildFinish, SetBuildStart, SetBuildFinish);
            _isUpdatingGroupsTimings = false;
        }

        /// <summary>
        /// S'abonne à des évènements sur l'élément spécifié.
        /// </summary>
        /// <param name="item">L'élément.</param>
        protected override void Register(ActionGanttItem item)
        {
            base.Register(item);
            item.TimingChanged += new EventHandler(OnItemTimingChanged);
            item.Predecessors.CollectionChanged += new NotifyCollectionChangedEventHandler(OnItemPredecessorsCollectionChanged);
        }

        /// <summary>
        /// Se désabonne à des évènements sur l'élément spécifié.
        /// </summary>
        /// <param name="item">L'élément.</param>
        protected override void Unregister(ActionGanttItem item)
        {
            base.Unregister(item);
            item.TimingChanged -= new EventHandler(OnItemTimingChanged);
            item.Predecessors.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnItemPredecessorsCollectionChanged);
        }

        private ActionGanttItem _updatingitem;

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="BuildActionStart"/> a changé.
        /// </summary>
        /// <param name="action">The action.</param>
        protected override void OnBuildActionStartChanged(KAction action)
        {
            foreach (var item in this.ItemsOfTypeAction.Where(i => i.Action == action))
            {
                _updatingitem = item;
                item.Start = GanttDates.ToDateTime(action.BuildStart);
                _updatingitem = null;
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="BuildActionFinish"/> a changé.
        /// </summary>
        /// <param name="action">The action.</param>
        protected override void OnBuildActionFinishChanged(KAction action)
        {
            foreach (var item in this.ItemsOfTypeAction.Where(i => i.Action == action))
            {
                _updatingitem = item;
                item.Finish = GanttDates.ToDateTime(action.BuildFinish);
                _updatingitem = null;
            }
        }

        private void OnItemTimingChanged(object sender, EventArgs e)
        {
            var item = (ActionGanttItem)sender;

            if (_updatingitem == item)
                return;

            if (_useGanttItemsTimingsOnly)
                return;

            if (!SetActionBuildTiming(item.Action, GanttDates.ToTicks(item.Start), GanttDates.ToTicks(item.Finish)))
            {
                item.Start = GanttDates.ToDateTime(item.Action.BuildStart);
                item.Finish = GanttDates.ToDateTime(item.Action.BuildFinish);
            }
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="AllowTimingsDurationChange"/> a changé.
        /// </summary>
        protected override void OnAllowTimingsDurationChangeChanged()
        {
            base.OnAllowTimingsDurationChangeChanged();

            foreach (var item in this.ItemsOfTypeAction)
                item.CanResize = base.AllowTimingsDurationChange;
        }

        /// <summary>
        /// Obtient le début process d'une action.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <returns>Le début</returns>
        protected override long GetBuildStart(KAction action)
        {
            if (_useGanttItemsTimingsOnly)
            {
                var first = this.ItemsOfTypeAction.First(i => i.Action == action);
                return GanttDates.ToTicks(first.Start);
            }
            else
            {
                return action.BuildStart;
            }
        }

        /// <summary>
        /// Obtient la fin process d'une action.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <returns>La fin</returns>
        protected override long GetBuildFinish(KAction action)
        {
            if (_useGanttItemsTimingsOnly)
            {
                var first = this.ItemsOfTypeAction.First(i => i.Action == action);
                return GanttDates.ToTicks(first.Finish);
            }
            else
            {
                return action.BuildFinish;
            }
        }

        private void SetBuildStart(KAction action, long value)
        {
            if (_useGanttItemsTimingsOnly)
            {
                foreach (var item in this.ItemsOfTypeAction.Where(i => i.Action == action))
                    item.Start = GanttDates.ToDateTime(value);
            }
            else
                action.BuildStart = value;
        }

        private void SetBuildFinish(KAction action, long value)
        {
            if (_useGanttItemsTimingsOnly)
            {
                foreach (var item in this.ItemsOfTypeAction.Where(i => i.Action == action))
                    item.Finish = GanttDates.ToDateTime(value);
            }
            else
                action.BuildFinish = value;
        }

        /// <summary>
        /// Calcule les temps critiques.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        /// <returns>Les temps critiques.</returns>
        protected override CriticalAction[] CalculateCriticalTimes(IEnumerable<KAction> actions)
        {
            var actionTimings = ActionsTimingsMoveManagement.CreateActionTimings(actions, this.UseManagedPredecessorsSuccessors,
                GetBuildStart, GetBuildFinish);
            return ActionsTimingsMoveManagement.CalculateCriticalTimes(actionTimings);
        }

        /// <summary>
        /// Appelé lorsque un type de réduction (IES) a été appliqué sur une action.
        /// </summary>
        /// <param name="action">L'action.</param>
        protected override void OnReducedTypeApplied(KAction action)
        {
            var item = this.ItemsOfTypeAction.FirstOrDefault(i => i.Action == action);
            if (item != null)
                item.OrangeHeaderVisibility = ActionsTimingsMoveManagement.IsActionExternal(action) ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Obtient des pinceaux à affecter aux items en fonction de leur catégorie.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="fillBrush">Le pinceau pour le contenu.</param>
        /// <param name="strokeBrush">Le pinceau pour la bordure.</param>
        private void GetActionBrushes(KAction action, out Brush fillBrush, out Brush strokeBrush)
        {
            BrushesHelper.GetBrush(action.Category != null ? action.Category.Color : null, true, out fillBrush, out strokeBrush);
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Met à jour l'affichage du header lorsque les actions n'ont pas d'originaux.
        /// </summary>
        /// <param name="targetActions">les actions cibles.</param>
        public void UpdateActionsHeaderWithNoOriginal(IEnumerable<KAction> targetActions)
        {
            foreach (var original in targetActions)
            {
                var item = this.ItemsOfTypeAction.FirstOrDefault(i => i.Action == original);
                if (item != null)
                {
                    item.GreenHeaderVisibility = original.Original == null ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Gestion prédecesseurs successeurs

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="IsMovingItems"/> a changé.
        /// </summary>
        protected override void OnIsMovingItemsChanged()
        {
            // Ré analyser tous les éléments et vérifier que les prédécesseurs soient corrects
            this.AddAllMissingDependencies();
        }

        /// <summary>
        /// Détermine si un lien peut être ajouté.
        /// </summary>
        /// <param name="item">L'élément.</param>
        /// <param name="predecessor">Le prédécesseur.</param>
        /// <returns><c>true</c> si le lien peut être ajouté.</returns>
        public bool DependencyCreationValidatorProvider(GanttChartItem item, GanttChartItem predecessor)
        {
            if (!DefaultDependencyCreationValidatorProvider(item, predecessor))
                return false;

            var actionItem = item as ActionGanttItem;
            var predecessorItem = predecessor as ActionGanttItem;

            if (actionItem == null || predecessorItem == null)
                return false;

            return ActionsTimingsMoveManagement.CheckCanAddPredecessor(base.GetActionsSortedByWBS(), actionItem.Action, predecessorItem.Action);
        }

        /// <summary>
        /// Détermine si un lien peut être ajouté.
        /// Cette implémentation correspond aux vérifications par défaut du contrôle.
        /// </summary>
        /// <param name="item">L'élément.</param>
        /// <param name="predecessor">Le prédécesseur.</param>
        /// <returns><c>true</c> si le lien peut être ajouté.</returns>
        private static bool DefaultDependencyCreationValidatorProvider(GanttChartItem item, GanttChartItem predecessor)
        {
            GanttChartView ganttChartView = item.GanttChartView as GanttChartView;
            if (ganttChartView == null || predecessor.GanttChartView != ganttChartView)
                return false;

            GanttChartItemCollection managedItems = ganttChartView.ManagedItems;
            if (managedItems == null)
                return false;

            return
                predecessor != item &&
                !managedItems.GetAllChildren(predecessor).Contains(item) &&
                !managedItems.GetAllChildren(item).Contains(predecessor) &&
                !predecessor.Predecessors.Any(p => (p.Item == item)) &&
                !item.Predecessors.Any(p => (p.Item == predecessor));
        }

        private void AddAllMissingDependencies()
        {
            Func<KAction, IEnumerable<KAction>> pred;

            if (this.UseManagedPredecessorsSuccessors)
                pred = a => a.PredecessorsManaged;
            else
                pred = a => a.Predecessors;

            foreach (var item in this.ItemsOfTypeAction)
            {
                var missingPredecessors = pred(item.Action)
                    .Where(predecessor =>
                        !item.Predecessors.Any(pi => ((ActionGanttItem)pi.Item).Action == predecessor) &&
                        this.ItemsOfTypeAction.Any(a => a.Action == predecessor)
                    ).ToArray();

                foreach (var p in missingPredecessors)
                    AddPredecessorItem(item, this.ItemsOfTypeAction.First(i => i.Action == p));
            }
        }

        /// <summary>
        /// Ajoute un prédecesseur à l'élément spécifié.
        /// </summary>
        /// <param name="item">L'élément successeur.</param>
        /// <param name="predecessorItem">L'élément prédecesseur.</param>
        private void AddPredecessorItem(ActionGanttItem item, ActionGanttItem predecessorItem)
        {
            _currentUpdatingPredecessors = item.Predecessors;
            item.Predecessors.Add(new PredecessorItem() { Item = predecessorItem, DependencyType = DependencyType.FinishStart });
            _currentUpdatingPredecessors = null;
        }

        /// <summary>
        /// Supprime un prédecesseur de l'élément.
        /// </summary>
        /// <param name="item">Le <see cref="PredecessorItem"/>.</param>
        private void DeletePredecessorItem(PredecessorItem item)
        {
            _currentUpdatingPredecessors = item.DependentItem.Predecessors;
            item.DependentItem.Predecessors.Remove(item);
            _currentUpdatingPredecessors = null;
        }

        /// <summary>
        /// Appelé lorsqu'un prédecesseur a été ajouté.
        /// </summary>
        /// <param name="action">L'action successeur.</param>
        /// <param name="predecessor">Le prédécesseur.</param>
        protected override void OnPredecessorAdded(KAction action, KAction predecessor)
        {
            var items = this.ItemsOfTypeAction.Where(i => i.Action == action).ToArray();
            var predecessorItems = this.ItemsOfTypeAction.Where(i => i.Action == predecessor).ToArray();

            foreach (var item in items)
            {
                if (_currentUpdatingPredecessors != item.Predecessors)
                    foreach (var pItem in predecessorItems)
                    {
                        AddPredecessorItem(item, pItem);
                    }
            }
        }

        /// <summary>
        /// Appelé lorsqu'un prédecesseur a été supprimé.
        /// </summary>
        /// <param name="action">L'action successeur.</param>
        /// <param name="predecessor">Le prédécesseur.</param>
        protected override void OnPredecessorRemoved(KAction action, KAction predecessor)
        {
            var items = this.ItemsOfTypeAction.Where(i => i.Action == action).ToArray();
            var predecessorItems = this.ItemsOfTypeAction.Where(i => i.Action == predecessor).ToArray();

            foreach (var item in items)
            {
                if (_currentUpdatingPredecessors != item.Predecessors)
                    foreach (var pItem in predecessorItems)
                    {
                        var predecessorItem = item.Predecessors.FirstOrDefault(pi => pi.Item == pItem);
                        if (_currentUpdatingPredecessors != item.Predecessors)
                            DeletePredecessorItem(predecessorItem);
                    }
            }
        }

        /// <summary>
        /// Appelé lorsque les prédecesseurs ont changé sur un élément.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.EventArgs"/> contenant les données de l'évènement.</param>
        private void OnItemPredecessorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var collection = (PredecessorItemCollection)sender;
            if (_currentUpdatingPredecessors != collection && !base.IsMovingItems)
            {
                if (e.NewItems != null)
                {
                    foreach (PredecessorItem pi in e.NewItems)
                    {
                        var item = (ActionGanttItem)pi.DependentItem;
                        var predecessor = (ActionGanttItem)pi.Item;

                        _currentUpdatingPredecessors = collection;
                        if (!base.AddPredecessor(item, predecessor))
                        {
                            throw new InvalidOperationException("Impossible d'ajouter un prédecesseur invalide. Utiliser CheckCanAddPredecessor() au préalable.");
                        }
                        _currentUpdatingPredecessors = null;
                    }
                }

                if (e.OldItems != null)
                {
                    foreach (PredecessorItem pi in e.OldItems)
                    {
                        var item = (ActionGanttItem)pi.DependentItem;
                        var predecessor = (ActionGanttItem)pi.Item;

                        _currentUpdatingPredecessors = collection;
                        base.RemovePredecessor(item, predecessor);
                        _currentUpdatingPredecessors = null;
                    }
                }
            }
        }

        #endregion

        #region Types imbriqués

        private class ActionsDisplayResults
        {
            public IEnumerable<KAction> Actions { get; set; }
            public IEnumerable<ActionTiming> NewTimings { get; set; }
        }

        #endregion

    }
}
