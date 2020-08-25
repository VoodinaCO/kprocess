using DlhSoft.Windows.Controls;
using Kprocess.KL2.FileTransfer;
using KProcess.Globalization;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Shell.ViewModels;
using KProcess.Presentation.Windows;
using KProcess.Presentation.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Gère les actions et les référentiels dans une grille ou dans un Gantt.
    /// </summary>
    /// <typeparam name="TComponentItem">Le type du composant manipulé.</typeparam>
    /// <typeparam name="TActionItem">Le type du composant pour les actions.</typeparam>
    /// <typeparam name="TReferentialItem">Le type du composant pour les référentiels.</typeparam>
    public abstract class ActionsManager<TComponentItem, TActionItem, TReferentialItem>
        where TComponentItem : DataTreeGridItem
        where TActionItem : TComponentItem, IActionItem
        where TReferentialItem : TComponentItem, IReferentialItem
    {
        static readonly string RefLabelsSeparator = Environment.NewLine;

        #region Champs privés

        private KAction[] _actions;
        private bool _isActionsCollectionDirty = true;
        private Action<TComponentItem> _currentItemSetter;
        private Func<int, Video> _videoGetter;
        private KAction _updateActionTimingSource;
        private KAction _updatingLinkedAction = null;
        private ActionPath[] _lastCriticalPath;
        private static readonly WBSHelper.WBSComparer _wbsComparer = new WBSHelper.WBSComparer();
        private Dictionary<KAction, ActionTimingSnapshot> _groupsSavedTimings = new Dictionary<KAction, ActionTimingSnapshot>();
        private bool _isUpdatingPercentReduction = false;
        private bool _isUpdatingGroupsTimings = false;
        private INavigationService _navigationService;
        private ITimeTicksFormatService _timeService;
        private IVideoColorService _videoColorService;
        private long _maxStopValue = long.MaxValue;
        private long _minStopValue = long.MinValue;
        private KAction _lastMarkerMoved = null;
        private bool _isCheckingMarkers = false;

        #endregion

        #region Constructeurs

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ActionsManager&lt;TComponentItem, TActionItem, TReferentialItem&gt;"/>.
        /// </summary>
        /// <param name="currentItemSetter">Un délégué capable de définir l'élément sélectionné.</param>
        /// <param name="videoGetter">Un délégué capable d'obtenir une vidéo à partir de son identifiant.</param>
        public ActionsManager(Action<TComponentItem> currentItemSetter, Func<int, Video> videoGetter)
        {
            _currentItemSetter = currentItemSetter;
            _videoGetter = videoGetter;
            ReferentialsLoad = new BulkObservableCollection<ReferentialLoad>();
            this.AllowTimingsChange = true;
            this.AreMarkersLinked = false;
            this.IsCriticalPathEnabled = true;
            this.View = GanttGridView.WBS;
            if (!DesignMode.IsInDesignMode)
            {
                _timeService = IoC.Resolve<IServiceBus>().Get<ITimeTicksFormatService>();
                _videoColorService = IoC.Resolve<IServiceBus>().Get<IVideoColorService>();
            }
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient la vue courante.
        /// </summary>
        public GanttGridView View { get; private set; }

        /// <summary>
        /// Obtient la charge des ressources.
        /// </summary>
        public BulkObservableCollection<ReferentialLoad> ReferentialsLoad { get; private set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la charge des ressources doit être mise à jour automatiquement.
        /// </summary>
        public bool EnableRessourceLoad { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les temps des actions doivent être mise à jour automatiquement en fonction de leurs prédécesseurs.
        /// </summary>
        public bool EnablePredecessorTimingFix { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le pourcentage de réduction de chaque action doit être rafraîchi.
        /// </summary>
        public bool EnableReducedPercentageRefresh { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les changements de temps sont autorisés.
        /// </summary>
        public bool AllowTimingsChange { get; set; }

        private bool _allowTimingsDurationChange;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si les changements de Durée sont autorisés.
        /// </summary>
        public bool AllowTimingsDurationChange
        {
            get { return _allowTimingsDurationChange; }
            set
            {
                if (_allowTimingsDurationChange != value)
                {
                    _allowTimingsDurationChange = value;
                    OnAllowTimingsDurationChangeChanged();
                }
            }
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="AllowTimingsDurationChange"/> a changé.
        /// </summary>
        protected virtual void OnAllowTimingsDurationChangeChanged()
        {
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les markers doivent être liés lors de leurs déplacements.
        /// </summary>
        public bool AreMarkersLinked { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les markers ne doivent pas être vérifiés.
        /// </summary>
        public bool NotVerifMarkers { get; set; } = false;

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la gestion du chemin critique est activée.
        /// </summary>
        public bool IsCriticalPathEnabled { get; set; }

        #endregion

        #region Eléments protégés

        /// <summary>
        /// Obtient la collection des éléments.
        /// </summary>
        protected abstract BulkObservableCollection<TComponentItem> Items { get; }

        /// <summary>
        /// Obtient les éléments de type Action.
        /// </summary>
        protected IEnumerable<TActionItem> ItemsOfTypeAction
        {
            get { return this.Items.OfType<TActionItem>(); }
        }

        /// <summary>
        /// Obtient les éléments de type Référentiel.
        /// </summary>
        protected IEnumerable<TReferentialItem> ItemsOfTypeReferential
        {
            get { return this.Items.OfType<TReferentialItem>(); }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les temps des groupes est constament réévalué.
        /// </summary>
        protected bool EnableGroupsTimingCoercion { get; set; }

        /// <summary>
        /// Obtient le service de navigation.
        /// </summary>
        protected INavigationService NavigationService
        {
            get
            {
                if (_navigationService == null)
                    _navigationService = IoC.Resolve<IServiceBus>().Get<INavigationService>();
                return _navigationService;
            }
        }

        /// <summary>
        /// Crée un nouvel élément représentant une action.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <returns>L'élément créé.</returns>
        protected TActionItem CreateNewActionItem(KAction action)
        {
            var item = CreateNewActionItemImpl(action);
            return item;
        }

        /// <summary>
        /// Crée un nouvel élément représentant une action.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <returns>L'élément créé.</returns>
        protected abstract TActionItem CreateNewActionItemImpl(KAction action);

        /// <summary>
        /// Crée un nouvel élément représentant un référentiel.
        /// </summary>
        /// <param name="referential">Le référentiel.</param>
        /// <param name="label">Le libellé de du référentiel.</param>
        /// <returns>L'élément créé.</returns>
        protected TReferentialItem CreateNewReferentialItem(IActionReferential referential, string label)
        {
            var item = CreateNewReferentialItemImpl(referential, label);
            return item;
        }

        /// <summary>
        /// Crée un nouvel élément représentant un référentiel.
        /// </summary>
        /// <param name="referential">Le référentiel.</param>
        /// <param name="label">Le libellé de du référentiel.</param>
        /// <returns>L'élément créé.</returns>
        protected abstract TReferentialItem CreateNewReferentialItemImpl(IActionReferential referential, string label);

        /// <summary>
        /// Obtient toutes les actions triées par WBS.
        /// </summary>
        /// <returns>Une liste des actions.</returns>
        internal protected KAction[] GetActionsSortedByWBS()
        {
            if (_isActionsCollectionDirty)
            {
                _actions = this.ItemsOfTypeAction.Select(i => i.Action)
                    .Distinct()
                    .OrderBy(a => a.WBSParts, _wbsComparer)
                    .ToArray();
                _isActionsCollectionDirty = false;
            }
            return
                _actions;
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les versions managées des prédécesseurs et successeurs doivent être utilisées.
        /// </summary>
        protected bool UseManagedPredecessorsSuccessors { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si mes Pred/Succ timings sont en cours de mise à jour.
        /// </summary>
        protected bool IsFixingPredecessorsSuccessorsTiming { get; set; }

        /// <summary>
        /// Obtient le début process d'une action.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <returns>Le début</returns>
        protected virtual long GetBuildStart(KAction action)
        {
            return action.BuildStart;
        }

        /// <summary>
        /// Obtient la fin process d'une action.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <returns>La fin</returns>
        protected virtual long GetBuildFinish(KAction action)
        {
            return action.BuildFinish;
        }

        /// <summary>
        /// Obtient une valeur indiquant si la vue courange est par référentiel.
        /// </summary>
        protected bool IsViewByReferential()
        {
            return this.View != GanttGridView.WBS;
        }

        /// <summary>
        /// Obtient un délégue permettant de récupérer les référentiels d'une action, en fonction de la vue en cours.
        /// </summary>
        protected Func<KAction, IEnumerable<IActionReferential>> GetReferentialsDelegate { get; private set; }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Change de vue.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="currentSelection">La sélection actuelle, à récupérer dans la prochaine vue.</param>
        public void ChangeView(GanttGridView view, TActionItem currentSelection)
        {
            this.View = view;
            this.RebuildItems(currentSelection);
        }

        /// <summary>
        /// Supprime les abonnements aux évènements de tous les éléments.
        /// </summary>
        public void UnregisterAllItems()
        {
            foreach (var item in this.ItemsOfTypeAction)
                Unregister(item);
            foreach (var item in this.ItemsOfTypeReferential)
                Unregister(item);
        }

        /// <summary>
        /// Reconstruit la collection des éléments.
        /// Si previousSelection est spécifié, sélectionne le nouvel élément correspondant à previousSelection.
        /// </summary>
        /// <param name="previousSelection">La sélection précédente à réintégrerg.</param>
        public void RebuildItems(TActionItem previousSelection = null)
        {
            UnregisterAllItems();

            var actions = GetActionsSortedByWBS();

            this.Clear();
            this.RegisterInitialActions(actions);

            if (previousSelection != null)
            {
                var newSelection = this.ItemsOfTypeAction.FirstOrDefault(r => r.Action == ((TActionItem)previousSelection).Action);
                this.SetCurrentItemAsync(newSelection);
            }
        }

        /// <summary>
        /// Met à jour les ratios de gains sur les actions en les comparant avec des anciennes actions.
        /// </summary>
        /// <param name="originalActions">Les actions d'origine.</param>
        /// <param name="targetActions">Les actions cible.</param>
        public void UpdateActionsOriginalRatio(IEnumerable<KAction> originalActions, IEnumerable<KAction> targetActions)
        {
            foreach (var target in targetActions)
            {
                var original = target.Original;
                while (original != null)
                {
                    if (originalActions.Contains(original))
                    {
                        var item = this.ItemsOfTypeAction.FirstOrDefault(i => i.Action == original);
                        if (item != null)
                        {
                            item.OriginalGainPercentage = (1.0 - (double)target.BuildDuration / (double)original.BuildDuration) * 100;
                        }
                        break;
                    }
                    original = original.Original;
                }
            }

            foreach (var original in originalActions)
            {
                var item = this.ItemsOfTypeAction.FirstOrDefault(i => i.Action == original);
                if (item != null && !item.OriginalGainPercentage.HasValue)
                    item.OriginalGainPercentage = 100;
            }
        }

        #endregion

        #region Gestion du chemin critique

        /// <summary>
        /// Survient lorsque le chemin critique a changé.
        /// </summary>
        public event EventHandler<EventArgs<ActionPath[]>> CriticalPathChanged;

        /// <summary>
        /// Met à jour et obtient le chemin critique.
        /// </summary>
        /// <returns>Les actions représentant le chemin critique.</returns>
        public IEnumerable<ActionPath> UpdateCriticalPath()
        {
            if (!IsCriticalPathEnabled)
            {
                if (_lastCriticalPath != null)
                {
                    _lastCriticalPath = null;
                    OnCriticalPathChanged(null);
                }
                return null;
            }

            var actions = GetActionsSortedByWBS();

            foreach (var item in this.ItemsOfTypeAction)
                item.IsCritical = false;

            var newCriticalPath = GetCriticalPath(actions);

            foreach (var item in this.ItemsOfTypeAction.Where(i => newCriticalPath.Any(c => c.Action == i.Action)))
                item.IsCritical = true;

            if (!AreCriticalPathesEqual(_lastCriticalPath, newCriticalPath))
            {
                _lastCriticalPath = newCriticalPath;
                OnCriticalPathChanged(newCriticalPath);
            }

            return newCriticalPath;
        }

        /// <summary>
        /// Obtient le chemin critiqueà partir des actions spécifiées.
        /// </summary>
        /// <param name="actions">les actions.</param>
        /// <param name="getBuildStart">Un délégué capable d'obtenir le début process d'une action.</param>
        /// <param name="getBuildFinish">Un délégué capable d'obtenir la fin process d'une action.</param>
        /// <returns>Le chemin critique.</returns>
        protected ActionPath[] GetCriticalPath(IEnumerable<KAction> actions)
        {
            var criticalActions = new List<ActionPath>();

            var critical = CalculateCriticalTimes(actions);

            CriticalAction grappeStart = null;
            var current = critical.Any() ? critical.MinWithValue(c => GetBuildStart(c.Action)) : null;
            while (current != null)
            {
                if (grappeStart == null)
                    grappeStart = current;

                criticalActions.Add(new ActionPath(current.Action));

                if (current.Successors.Any())
                {
                    current = current.Successors.MaxWithValue(p => p.CriticalTime);
                }
                else
                {
                    // Rechercher des actions qui finissent après la courante mais qui chevauchent la courante
                    var overlapping = critical
                        .Where(c => !criticalActions.Any(a => a.Action == c.Action)
                            && GetBuildFinish(c.Action) > GetBuildFinish(current.Action)
                            && GetBuildStart(c.Action) <= GetBuildFinish(current.Action));

                    if (overlapping.Any())
                    {
                        current = overlapping.MaxWithValue(c => GetBuildFinish(c.Action));
                        grappeStart = current;
                    }
                    else
                    {
                        // Rechercher des actions qui commencent et finissent apres la courante
                        var finishingLater = critical
                            .Where(c => !criticalActions.Any(a => a.Action == c.Action) && GetBuildFinish(c.Action) > GetBuildFinish(current.Action));

                        if (finishingLater.Any())
                            current = finishingLater.MinWithValue(c => GetBuildStart(c.Action));
                        else
                            current = null;
                    }
                }
            }

            // Parcourir tous les éléments afin de mettre à jour le CriticalStart et le CriticalFinish
            long currentMaxFinish = -1;
            foreach (var acp in criticalActions)
            {
                acp.Start = GetBuildStart(acp.Action);

                acp.Finish = GetBuildFinish(acp.Action);
                currentMaxFinish = GetBuildFinish(acp.Action);
            }

            var newCriticalPath = criticalActions.ToArray();
            return newCriticalPath;
        }

        /// <summary>
        /// Obtient le chemin critique du référentiel de l'action spécifiée, en fonction de la vue en cours.
        /// </summary>
        /// <param name="item">L'élément.</param>
        /// <returns>Le chemin critique du référentiel.</returns>
        public ActionPath[] GetReferentialCriticalPath(TActionItem item)
        {
            var ressource = item.ParentReferentialItem;

            var actions = GetActionsSortedByWBS().Where(a => GetReferentialsDelegate(a).Contains(ressource.Referential));
            return GetCriticalPath(actions);
        }

        /// <summary>
        /// Calcule les temps critiques.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        /// <returns>Les temps critiques.</returns>
        protected virtual CriticalAction[] CalculateCriticalTimes(IEnumerable<KAction> actions)
        {
            var actionTimings = ActionsTimingsMoveManagement.CreateActionTimings(actions, this.UseManagedPredecessorsSuccessors,
                a => a.BuildStart, a => a.BuildFinish);
            return ActionsTimingsMoveManagement.CalculateCriticalTimes(actionTimings);
        }

        /// <summary>
        /// Appelé pour lever l'évènement CriticalPathChanged.
        /// </summary>
        /// <param name="criticalPath">Le nouveau chemin critique.</param>
        protected virtual void OnCriticalPathChanged(ActionPath[] criticalPath)
        {
            if (CriticalPathChanged != null)
                CriticalPathChanged(this, new EventArgs<ActionPath[]>(criticalPath));
        }

        /// <summary>
        /// Détermine si deux chemins critique sont équivalent.
        /// </summary>
        /// <param name="path1">Le premier chemin critique.</param>
        /// <param name="path2">Le deuxième chemin critique.</param>
        /// <returns><c>true</c> si les chemins critiques sont équivalent.</returns>
        private bool AreCriticalPathesEqual(ActionPath[] path1, ActionPath[] path2)
        {
            if (path1 == null && path2 != null || path1 != null && path2 == null)
                return false;

            if (path1.Length != path2.Length)
                return false;

            for (int i = 0; i < path1.Length; i++)
            {
                var action1 = path1[i];
                var action2 = path2[i];

                if (action1.Action != action2.Action)
                    return false;

                if (action1.Action != action2.Action ||
                    action1.Start != action2.Start ||
                    action1.Finish != action2.Finish ||
                    action1.HasVideo != action2.HasVideo ||
                    action1.VideoStart != action2.VideoStart ||
                    action1.VideoFinish != action2.VideoFinish)
                    return false;
            }

            return true;
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Crée un élément représentant une action, avec un référentiel associée.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="referential">Le référentiel.</param>
        /// <returns>L'élément créé.</returns>
        private TActionItem CreateActionItemWithReferential(KAction action, IActionReferential referential)
        {
            var item = CreateNewActionItem(action);
            item.ParentReferentialItem = this.ItemsOfTypeReferential.Single(i => i.Referential == referential);
            return item;
        }

        /// <summary>
        /// Obtient l'index d'insertion pour le parent d'une action.
        /// </summary>
        /// <param name="item">L'action parent.</param>
        /// <param name="referential">Le référentiel.</param>
        /// <param name="allActions">Les autres actions.</param>
        /// <returns>
        /// L'index
        /// </returns>
        private int GetParentInsertionIndex(TActionItem item, IActionReferential referential, IEnumerable<KAction> allActions)
        {
            var parents = WBSHelper.GetParents(item.Action, allActions);

            if (!parents.Any())
            {
                int insertionIndex = -1;

                // Chercher les frères avec sous le même groupe référentiel parent
                var siblingsWithSameReferential = this.ItemsOfTypeAction.Where(i =>
                    i.ParentReferentialItem.Referential == referential && i.Indentation == item.Indentation);
                TActionItem lastPrecedingSiblingWithSameReferential = null;
                foreach (var i in siblingsWithSameReferential)
                {
                    if (WBSHelper.Compare(i.Action.WBS, item.Action.WBS) < 0)
                    {
                        if (lastPrecedingSiblingWithSameReferential == null ||
                            WBSHelper.Compare(i.Action.WBS, lastPrecedingSiblingWithSameReferential.Action.WBS) > 0)
                        {
                            lastPrecedingSiblingWithSameReferential = i;
                        }
                    }
                }

                if (lastPrecedingSiblingWithSameReferential != null)
                {
                    var lastPSIndex = this.Items.IndexOf(lastPrecedingSiblingWithSameReferential) + 1;
                    insertionIndex = lastPSIndex;
                    var lastPSIndentation = lastPrecedingSiblingWithSameReferential.Indentation;

                    for (int i = lastPSIndex; i < this.Items.Count; i++)
                    {
                        if (this.Items[i].Indentation <= lastPSIndentation)
                            break;
                        insertionIndex = i + 1;
                    }
                }

                if (insertionIndex == -1)
                {
                    // Ajouter à la fin du référentiel
                    var minIndex = this.Items.IndexOf(this.ItemsOfTypeReferential.Single(i => i.Referential == referential)) + 1;
                    insertionIndex = minIndex;

                    for (int i = minIndex; i < this.Items.Count; i++)
                    {
                        if (this.Items[i].Indentation == 0)
                            break;
                        insertionIndex = i + 1;
                    }

                    insertionIndex = minIndex;
                }

                return insertionIndex;
            }
            else
            {
                // Dans le parent du parent, il faut l'ajouter à la fin
                var lastParentIndex = this.Items.IndexOf(this.ItemsOfTypeAction.Single(i => i.Action == parents.Last() && i.ParentReferentialItem.Referential == referential));
                int targetindex = -1;
                for (int i = lastParentIndex + 1; i < this.Items.Count; i++)
                {
                    if (this.Items[i] is TActionItem)
                    {
                        var actionitem = (TActionItem)this.Items[i];

                        if (actionitem.Indentation <= item.Indentation)
                        {
                            targetindex = i + 1;
                            break;
                        }
                        else if (_wbsComparer.Compare(actionitem.Action.WBSParts, item.Action.WBSParts) > 0)
                        {
                            targetindex = i;
                            break;
                        }
                    }
                    else
                        break;
                }
                if (targetindex == -1)
                    targetindex = lastParentIndex + 1;

                return targetindex;
            }
        }

        /// <summary>
        /// En vue par référentiel, vérifie que chaque feuille de la grille (partie appartenant au référentiel spécifiée) possède bien ses parents.
        /// </summary>
        private void CheckGroups(IReferentialItem referential)
        {
            if (IsViewByReferential())
            {
                // Sélectionner les feuilles
                var actions = GetActionsSortedByWBS();
                var leafs = this.ItemsOfTypeAction
                    .Where(i => i.ParentReferentialItem == referential && !WBSHelper.HasChildren(i.Action, actions))
                    .ToArray();

                foreach (var item in leafs)
                {
                    // Récupérer le dernier de ses parents
                    var parents = WBSHelper.GetParents(item.Action, actions);

                    foreach (var parent in parents)
                    {
                        // Vérifier s'il existe
                        var parentItem = this.ItemsOfTypeAction.FirstOrDefault(i =>
                            i.Action == parent
                            && i.ParentReferentialItem != null
                            && GetReferentialsDelegate(item.Action).Contains(i.ParentReferentialItem.Referential));

                        // S'il n'existe pas, le créer
                        if (parentItem == null)
                        {
                            parentItem = CreateActionItemWithReferential(parent, referential.Referential);
                            this.Items.Insert(GetParentInsertionIndex(parentItem, referential.Referential, actions), parentItem);

                            this.Register(parentItem);
                            UpdatePredecessorsString(parentItem.Action);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Définit l'élément courant de manière asynchrone.
        /// </summary>
        /// <param name="item">l'élément.</param>
        private void SetCurrentItemAsync(TActionItem item)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() =>
            {
                _currentItemSetter(null); // Dans certains cas, même si la propriété SelectedItem du DataGrid est conforme, c'est visuellement le mauvais élément qui est selectionné. Ce reset permet de focer le composant DataGrid à rafraichir sa selection.
                _currentItemSetter(item);

            }), DispatcherPriority.Background);
        }

        /// <summary>
        /// Met à jour la couleur de la vidéo sur l'élément.
        /// </summary>
        /// <param name="item">L'élément.</param>
        private void UpdateVideoColor(TActionItem item)
        {
            if (item.Action.VideoId.GetValueOrDefault() != default(int) && item.Action.Video != null && item.Action != null)
            {
                item.VideoColor = _videoColorService.GetColor(item.Action.Video);
                var video = item.Action.Video;
                item.VideoName = $"{(string.IsNullOrEmpty(video.CameraName) ? string.Empty : $"{video.CameraName} - ")}{(video.DefaultResource == null ? string.Empty : $"{video.DefaultResource.Label} - ")}{(video.ResourceView == null ? string.Empty : $"{video.View.Label} - ")}{video.ShootingDate}";
            }
            else
            {
                item.VideoColor = null;
                item.VideoName = null;
            }
        }

        /// <summary>
        /// Met à jour la vignette de l'action.
        /// </summary>
        /// <param name="action">L'action.</param>
        /*private void UpdateThumbnail(KAction action)
        {
            var items = this.ItemsOfTypeAction.Where(i => i.Action == action);
            if (items.Any())
            {
                var thumbailBitmap = CreateThumbnailBitmap(action);

                foreach (var item in items)
                    item.Thumbnail = thumbailBitmap;
            }
        }*/

        /// <summary>
        /// Restaure l'état plié/déplié de tous les éléments chargés.
        /// </summary>
        private void RestoreExpandedStates()
        {
            // Classer les éléments par ordre de profondeur descendant
            // Cela permet de ne pas se tromper dans l'affectation
            foreach (var item in this.Items.OrderByDescending(i => i.Indentation))
            {
                RestoreExpandedState(item);
            }
        }

        /// <summary>
        /// Restaure l'état plié/déplié d'un élément.
        /// </summary>
        /// <param name="item">L'élément.</param>
        private void RestoreExpandedState(TComponentItem item)
        {
            if (KProcess.Presentation.Windows.DesignMode.IsInDesignMode)
                return;

            if (_navigationService == null)
            {
                if (IoC.IsRegistered<IServiceBus>() && IoC.Resolve<IServiceBus>().IsAvailable<INavigationService>())
                    _navigationService = IoC.Resolve<IServiceBus>().Get<INavigationService>();
                else
                    return;
            }

            var action = item as TActionItem;
            if (action != null)
            {
                if (action.Action.ActionId != default(int))
                {
                    var isExpanded = NavigationService.Preferences.IsActionExpanded(action.Action.ActionId);
                    if (isExpanded.HasValue)
                        item.IsExpanded = isExpanded.Value;
                }
            }
            else
            {
                var referential = item as TReferentialItem;
                if (referential != null)
                {
                    int referentialId = referential.Referential != null ? referential.Referential.Id : 0;

                    var isExpanded = NavigationService.Preferences.IsReferentialExpanded(referentialId);
                    if (isExpanded.HasValue)
                        item.IsExpanded = isExpanded.Value;
                }
            }
        }

        #endregion

        #region Initialisation, Ajout, Suppression

        /// <summary>
        /// Enregistre des actions déjà existantes et construit les éléments.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        public void RegisterInitialActions(IEnumerable<KAction> actions)
        {
            RegisterInitialActionsImpl(actions);
        }

        /// <summary>
        /// Enregistre des actions déjà existantes et construit les éléments.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        protected virtual void RegisterInitialActionsImpl(IEnumerable<KAction> actions)
        {
            if (this.View == GanttGridView.WBS)
                RegisterInitialActionsWBS(actions);
            else
                RegisterInitialActionsReferentials(actions);

            _isActionsCollectionDirty = true;

            this.RestoreExpandedStates();
            this.UpdateIsGroup();
            this.UpdateAllPredecessorsString();
            this.UpdateResourcesLoad();
            this.UpdateCriticalPath();
            this.UpdateAllReducedReductionFromTimings(actions);
        }

        /// <summary>
        /// Enregistre des actions déjà existantes et construit les éléments en vue WBS.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        private void RegisterInitialActionsWBS(IEnumerable<KAction> actions)
        {
            this.GetReferentialsDelegate = null;

            // Trier par rapport au wbs
            actions = actions.OrderBy(a => a.WBSParts, _wbsComparer);

            foreach (var action in actions)
            {
                var item = CreateNewActionItem(action);

                this.EnqueueAdd(item);
            }

            this.FlushAdd();

            foreach (var item in this.ItemsOfTypeAction)
                UpdatePredecessorsString(item.Action);
        }

        private List<TComponentItem> _itemsQueue = new List<TComponentItem>();
        private void EnqueueAdd(TComponentItem item)
        {
            _itemsQueue.Add(item);
        }

        private void FlushAdd()
        {
            this.Items.ReplaceAll(_itemsQueue);

            foreach (var item in _itemsQueue.OfType<TActionItem>())
                this.Register(item);

            foreach (var item in _itemsQueue.OfType<TReferentialItem>())
                this.Register(item);

            _itemsQueue.Clear();
        }

        /// <summary>
        /// Créer un <see cref="TReferentialItem"/> et l'ajoute dans liste si c'est nécessaire.
        /// </summary>
        /// <param name="referential">Le référentiel associée.</param>
        private void CreateReferentialItemIfNecessary(IActionReferential referential)
        {
            if (this.IsViewByReferential())
            {
                var firstRes = this.ItemsOfTypeReferential.FirstOrDefault(r => r.Referential == referential);
                if (firstRes == null)
                {
                    string label = referential != null ? referential.Label : LocalizationManager.GetString("VMHelpers_ActionsManager_EmptyReferentialLabel");
                    var resItem = CreateNewReferentialItem(referential, label);
                    this.Register(resItem);

                    // Déterminer l'index où l'ajouter
                    int insertIndex = -1;

                    // Ajouter le sans référentiel à la fin
                    if (referential == null)
                        insertIndex = this.Items.Count;
                    else
                    {
                        var emptyReferential = this.ItemsOfTypeReferential.FirstOrDefault(i => i.Referential == null);

                        var itemsResQuery = this.ItemsOfTypeReferential
                            .Where(i => i.Referential != null)
                            .Concat(resItem)
                            .OrderBy(i => i.Content)
                            .AsEnumerable();

                        if (emptyReferential != null)
                            itemsResQuery = itemsResQuery.Concat(emptyReferential);

                        var itemsRes = itemsResQuery.ToArray();

                        var index = itemsRes.IndexOf(resItem);
                        if (index == 0)
                            insertIndex = 0;
                        else if (index == itemsRes.Length - 1)
                            insertIndex = this.Items.Count;
                        else
                            // Prendre l'index du suivant dans la liste
                            insertIndex = this.Items.IndexOf(itemsRes[index + 1]);
                    }

                    System.Diagnostics.Debug.Assert(insertIndex != -1);

                    this.Items.Insert(insertIndex, resItem);
                }
            }
        }

        /// <summary>
        /// Enregistre des actions déjà existantes et construit les éléments en vue par référentiels.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        private void RegisterInitialActionsReferentials(IEnumerable<KAction> actions)
        {
            switch (this.View)
            {
                case GanttGridView.Category:
                    this.GetReferentialsDelegate = a => EnumerableExt.Concat(a.Category);
                    break;
                case GanttGridView.Ref1:
                    this.GetReferentialsDelegate = a => a.Ref1.Count == 0 ? EnumerableExt.Concat<Ref1>((Ref1)null) : a.Ref1.Select(c => c.Ref1);
                    break;
                case GanttGridView.Ref2:
                    this.GetReferentialsDelegate = a => a.Ref2.Count == 0 ? EnumerableExt.Concat<Ref2>((Ref2)null) : a.Ref2.Select(c => c.Ref2);
                    break;
                case GanttGridView.Ref3:
                    this.GetReferentialsDelegate = a => a.Ref3.Count == 0 ? EnumerableExt.Concat<Ref3>((Ref3)null) : a.Ref3.Select(c => c.Ref3);
                    break;
                case GanttGridView.Ref4:
                    this.GetReferentialsDelegate = a => a.Ref4.Count == 0 ? EnumerableExt.Concat<Ref4>((Ref4)null) : a.Ref4.Select(c => c.Ref4);
                    break;
                case GanttGridView.Ref5:
                    this.GetReferentialsDelegate = a => a.Ref5.Count == 0 ? EnumerableExt.Concat<Ref5>((Ref5)null) : a.Ref5.Select(c => c.Ref5);
                    break;
                case GanttGridView.Ref6:
                    this.GetReferentialsDelegate = a => a.Ref6.Count == 0 ? EnumerableExt.Concat<Ref6>((Ref6)null) : a.Ref6.Select(c => c.Ref6);
                    break;
                case GanttGridView.Ref7:
                    this.GetReferentialsDelegate = a => a.Ref7.Count == 0 ? EnumerableExt.Concat<Ref7>((Ref7)null) : a.Ref7.Select(c => c.Ref7);
                    break;
                case GanttGridView.Resource:
                    this.GetReferentialsDelegate = a => EnumerableExt.Concat(a.Resource);
                    break;
                case GanttGridView.Skill:
                    this.GetReferentialsDelegate = a => EnumerableExt.Concat(a.Skill);
                    break;
                case GanttGridView.WBS:
                default:
                    throw new ArgumentOutOfRangeException();
            }
            // Trier tout d'abord par WBS
            actions = actions.OrderBy(a => a.WBSParts, _wbsComparer).ToList();

            foreach (var action in actions)
            {
                foreach (var referential in GetReferentialsDelegate(action))
                {
                    this.CreateReferentialItemIfNecessary(referential);

                    // Si l'action est un groupement, on ne l'ajoute pas explicitement
                    var hasChildren = WBSHelper.HasChildren(action, actions);

                    if (!hasChildren)
                    {
                        // Récupérer le dernier de ses parents
                        var parents = WBSHelper.GetParents(action, actions);

                        foreach (var parent in parents)
                        {
                            // Vérifier s'il existe
                            var parentItem = this.ItemsOfTypeAction.FirstOrDefault(i =>
                                i.Action == parent
                                && i.ParentReferentialItem != null
                                && i.ParentReferentialItem.Referential == referential);

                            // S'il n'existe pas, le créer
                            if (parentItem == null)
                            {
                                parentItem = CreateActionItemWithReferential(parent, referential);
                                this.Items.Insert(GetParentInsertionIndex(parentItem, referential, actions), parentItem);

                                this.Register(parentItem);
                                UpdatePredecessorsString(parentItem.Action);
                            }
                        }

                        // calculer l'index
                        TComponentItem lastParent;
                        if (parents.Any())
                            lastParent = this.ItemsOfTypeAction.Single(i => i.Action == parents.Last() && i.ParentReferentialItem.Referential == referential);
                        else
                            lastParent = this.ItemsOfTypeReferential.Single(i => i.Referential == referential);

                        // Après le dernier parent, chercher le dernier élément du niveau en dessous
                        var parentIndex = this.Items.IndexOf(lastParent);

                        int targetIndex = -1;
                        for (int i = parentIndex + 1; i < this.Items.Count; i++)
                        {
                            if (this.Items[i].Indentation <= lastParent.Indentation)
                            {
                                targetIndex = i;
                                break;
                            }
                        }

                        if (targetIndex == -1)
                        {
                            // Ajouter à la fin
                            targetIndex = this.Items.Count;
                        }

                        // Insérer l'action après
                        var item = CreateActionItemWithReferential(action, referential);
                        this.Items.Insert(targetIndex, item);

                        this.Register(item);
                    }
                }

            }

            foreach (var item in this.ItemsOfTypeAction)
                UpdatePredecessorsString(item.Action);

        }

        /// <summary>
        /// Ajoute une nouvelle action.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <returns>L'élément créé.</returns>
        public TActionItem AddAction(KAction action)
        {
            return this.AddAction(action, null);
        }

        /// <summary>
        /// Ajoute une nouvelle action.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="afterItem">L'élément après lequel l'action doit être ajoutée.</param>
        /// <returns>L'élément créé.</returns>
        public TActionItem AddAction(KAction action, TComponentItem afterItem)
        {
            this.TraceAllWBS();
            this.TraceCollection();

            if (afterItem != null && !this.Items.Contains(afterItem))
            {
                // Ca peut arriver car la collection a pu être reconstruite
                if (afterItem is TActionItem afterActionItem)
                    afterItem = this.ItemsOfTypeAction.FirstOrDefault(i =>
                        i.Action == afterActionItem.Action &&
                        (
                            (i.ParentReferentialItem == null && i.ParentReferentialItem == afterActionItem.ParentReferentialItem) ||
                            (i.ParentReferentialItem != null && afterActionItem.ParentReferentialItem != null && i.ParentReferentialItem.Referential == afterActionItem.ParentReferentialItem.Referential)
                        ));

                if (afterItem is TReferentialItem afterReferentialItem)
                    afterItem = this.ItemsOfTypeReferential.FirstOrDefault(i =>
                        i.Referential == afterReferentialItem.Referential);
            }

            var allActions = GetActionsSortedByWBS();

            int targetWBSValue = -1;
            KAction parentAction = null;

            // Positionne l'élément à la fin au niveau 0
            void putAtEnd()
            {
                parentAction = null;

                if (allActions.Any())
                {
                    targetWBSValue =
                        WBSHelper.GetNumberAtLevel(
                            WBSHelper.GetActionsAtIndentationLevel(0, allActions
                        ).Last().WBS, 0) + 1;
                }
                else
                    targetWBSValue = 1;
            }

            if (afterItem != null)
            {
                // Si l'élément sélectionné est un référentiel, on ajoute l'action :
                // Au premier niveau d'indentation
                // A la fin
                if (afterItem is TReferentialItem)
                {
                    putAtEnd();
                }
                // Si l'élément sélectionné est une action, on ajoute l'action :
                // Au même niveau d'indentation
                // Immédiatement à la suite de l'action associée
                else
                {
                    var afterAction = ((TActionItem)afterItem).Action;

                    parentAction = WBSHelper.GetParent(afterAction, allActions);
                    var insertIndentation = WBSHelper.IndentationFromWBS(afterAction.WBS);
                    targetWBSValue = WBSHelper.GetNumberAtLevel(afterAction.WBS, insertIndentation) + 1;

                    // Si l'élement après lequel on insère l'élement est dans un groupe, que cet élément est le dernier element du groupe
                    // alors la nouvelle action récupère les successeurs.
                    if (WBSHelper.IndentationFromWBS(afterAction.WBS) > 0)
                    {
                        var nextSibling = WBSHelper.GetNextSiblingWBS(afterAction.WBS);
                        var isLastInItsGroup = !allActions.Any(a => a.WBS == nextSibling);
                        if (isLastInItsGroup)
                        {
                            var parent = WBSHelper.GetParent(afterAction, allActions);
                            if (parent != null
                                && parent.IsGroup && afterAction.Start >= parent.Start
                                && afterAction.Finish <= parent.Finish)
                            {
                                action.Successors.AddRange(afterAction.Successors);
                                afterAction.Successors.Clear();
                            }
                        }
                    }
                }
            }
            else
            {
                putAtEnd();
            }

            ActionsTimingsMoveManagement.InsertUpdateWBS(allActions, action, parentAction, targetWBSValue);

            System.Diagnostics.Debug.Assert(targetWBSValue != -1);
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(action.WBS));

            TActionItem item;
            if (this.View == GanttGridView.WBS)
                item = CreateNewActionItem(action);
            else
            {
                // Créer un item par référentiel sur les feuilles
                // Utiliser uniquement le premier référentiel pour placer l'élément
                var firstReferential = this.GetReferentialsDelegate(action).FirstOrDefault();
                this.CreateReferentialItemIfNecessary(firstReferential);
                item = CreateActionItemWithReferential(action, firstReferential);
            }
            int finalInsertIndex = -1;

            // Ajouter l'item dans la collection
            if (afterItem != null)
            {
                var afterItemIndex = this.Items.IndexOf(afterItem);

                if (afterItem is TReferentialItem)
                {
                    // L'ajouter à la fin du référentiel
                    for (int i = afterItemIndex + 1; i < this.Items.Count; i++)
                    {
                        if (this.Items[i] is TReferentialItem)
                        {
                            finalInsertIndex = i;
                            break;
                        }
                    }
                    if (finalInsertIndex == -1)
                        finalInsertIndex = this.Items.Count;
                }
                else
                {
                    // L'ajouter juste après l'élément sélectionné et ses enfants
                    for (int i = afterItemIndex + 1; i < this.Items.Count; i++)
                    {
                        if (this.Items[i].Indentation <= afterItem.Indentation)
                        {
                            finalInsertIndex = i;
                            break;
                        }
                    }
                    if (finalInsertIndex == -1)
                        finalInsertIndex = this.Items.Count;
                }
            }
            else
            {
                finalInsertIndex = this.Items.Count;
            }

            System.Diagnostics.Debug.Assert(finalInsertIndex != -1);

            this.Items.Insert(finalInsertIndex, item);



            this.Register(item);
            UpdatePredecessorsString(item.Action);

            _isActionsCollectionDirty = true;
            this.DebugCheckAllWBS();
            this.UpdateIsGroup();
            this.UpdateAllPredecessorsString();
            this.UpdateResourcesLoad();
            return item;
        }

        public ActionGridItem DuplicateAction(KAction action)
        {
            return AddAction(action) as ActionGridItem;
        }

        public void DuplicateActionChild(KAction action, KAction parent, KAction afterItem)
        {
            var parentItem = CreateNewActionItem(parent);
            AddActionAsChild(action, parentItem,afterItem);
        }



        /// <summary>
        /// Ajoute une nouvelle action en tant qu'enfant.
        /// </summary>
        /// <param name="action">l'action.</param>
        /// <param name="parent">Le parent.</param>
        /// <returns>L'élément créé</returns>
        public TActionItem AddActionAsChild(KAction action, TActionItem parent, KAction afterItem)
        {
            Assertion.NotNull(parent, "parent");

            if (IsViewByReferential())
                throw new InvalidOperationException("Ne fonctionne qu'en vue par WBS");

            this.TraceAllWBS();
            this.TraceCollection();

            if (parent != null && !this.Items.Contains(parent))
            {
                // Ca peut arriver car la collection a pu être reconstruite
                var afterActionItem = parent as TActionItem;
                //if (afterActionItem != null)
                //    parent = this.ItemsOfTypeAction.FirstOrDefault(i => i.Action == afterActionItem.Action);
            }

            string targetWBS = null;
            var allActions = GetActionsSortedByWBS();

            var parentAction = parent.Action;

            int insertIndentation = WBSHelper.IndentationFromWBS(parentAction.WBS) + 1;
            int targetWBSValue = 1;
            var lastChild = WBSHelper.GetLastChild(parentAction, allActions);
            if (lastChild != null)
                targetWBSValue = WBSHelper.GetParts(lastChild.WBS).Last() + 1;
            else
                targetWBSValue = 1;

            ActionsTimingsMoveManagement.InsertUpdateWBS(GetActionsSortedByWBS(), action, parentAction, targetWBSValue);

            targetWBS = WBSHelper.LevelsToWBS(WBSHelper.GetParts(parentAction.WBS).Concat(new int[] { targetWBSValue }));

            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(targetWBS));

            action.WBS = targetWBS;

            var item = CreateNewActionItem(action);

            if (lastChild != null)
            {
                if (afterItem == null)
                    this.Items.Insert(this.Items.IndexOf(this.ItemsOfTypeAction.First(a => a.Action == lastChild)) + 1, item);
                else
                    this.Items.Insert(this.Items.IndexOf(this.ItemsOfTypeAction.First(a => a.Action == afterItem)) + 1, item);

            }
            else
                this.Items.Insert(this.Items.IndexOf(parent) + 1, item);

            this.Register(item);
            UpdatePredecessorsString(item.Action);

            _isActionsCollectionDirty = true;
            FixAllIndentations();
            FixAllWBS();
            this.DebugCheckAllWBS();
            this.UpdateIsGroup();
            this.UpdateAllPredecessorsString();
            this.UpdateResourcesLoad();
            return item;
        }

        /// <summary>
        /// Supprime une action.
        /// </summary>
        /// <param name="action">L'action.</param>
        public void DeleteAction(KAction action)
        {
            this.TraceAllWBS();
            this.TraceCollection();
            var actions = GetActionsSortedByWBS();

            ActionsTimingsMoveManagement.DeleteUpdateWBS(actions, action);

            // Rechercher les items correspondants
            var items = this.ItemsOfTypeAction.Where(i => i.Action == action).ToArray();
            foreach (var i in items)
            {
                this.Items.Remove(i);
                Unregister(i);
            }

            _isActionsCollectionDirty = true;
            this.DebugCheckAllWBS();
            this.UpdateIsGroup();
            this.UpdateAllPredecessorsString();
            this.FixPredecessorsSuccessorsTimings();
            this.UpdateVideoGroupsTiming();
            this.UpdateBuildGroupsTiming();
            this.UpdateResourcesLoad();
        }

        /// <summary>
        /// Vide les éléments des collections.
        /// </summary>
        public void Clear()
        {
            foreach (var item in this.ItemsOfTypeAction)
                this.Unregister(item);
            foreach (var item in this.ItemsOfTypeReferential)
                this.Unregister(item);

            this.Items.Clear();
            _isActionsCollectionDirty = true;
        }

        #endregion

        #region Gestion des déplacements

        private bool _isMovingItems;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si le gestionnaire est actuellement en train de déplacer des éléments.
        /// </summary>
        protected bool IsMovingItems
        {
            get { return _isMovingItems; }
            private set
            {
                if (_isMovingItems != value)
                {
                    _isMovingItems = value;
                    OnIsMovingItemsChanged();
                }
            }
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="IsMovingItems"/> a changé.
        /// </summary>
        protected virtual void OnIsMovingItemsChanged()
        {
        }

        /// <summary>
        /// Déplace l'action spécifiée vers le haut.
        /// </summary>
        /// <param name="item">L'action.</param>
        /// <returns><c>true</c> si le déplacement a été effectué correctement.</returns>
        public bool MoveUp(TActionItem item)
        {
            this.TraceAllWBS();
            this.TraceCollection();

            IsMovingItems = true;
            var ret = this.MoveUp(item, item);
            IsMovingItems = false;
            if (ret)
            {
                this.UpdateIsGroup();
                this.UpdateAllPredecessorsString();
            }
            return ret;
        }

        /// <summary>
        /// Déplace un élément et tous ses enfants vers le haut, en le positionnant au dessus de son frère le plus proche.
        /// </summary>
        /// <param name="item">L'action.</param>
        /// <returns><c>true</c> si le déplacement a été effectué correctement.</returns>
        /// <remarks>Ne fonctionne qu'en vue par WBS</remarks>
        public bool MoveUpAboveSibling(TActionItem item)
        {
            this.TraceAllWBS();
            this.TraceCollection();

            if (IsViewByReferential())
                throw new InvalidOperationException("Ne fonctionne qu'en vue par WBS");

            IsMovingItems = true;

            bool ret = false;

            var index = this.Items.IndexOf(item);
            var graph = ActionsGraphManipulator<TActionItem>.Virtualize(this.ItemsOfTypeAction);
            var node = graph.ResolveNodeByFlatIndex(index);
            if (ret = node.MoveUpCommand.CanExecute(null))
            {
                node.MoveUpCommand.Execute(null);
            }

            graph.Apply(this.Items, delete: Unregister);

            IsMovingItems = false;

            if (ret)
            {
                _isActionsCollectionDirty = true;
                FixAllIndentations();
                FixAllWBS();
                this.UpdateIsGroup();
                this.UpdateAllPredecessorsString();
                this.FixPredecessorsSuccessorsTimings();
                this.UpdateVideoGroupsTiming();
                this.UpdateBuildGroupsTiming();
                this.UpdateResourcesLoad();
                this.SetCurrentItemAsync(item);
            }

            return ret;
        }

        /// <summary>
        /// Déplace l'action spécifiée vers le haut puis sélectionne l'élément spécifié.
        /// </summary>
        /// <param name="item">L'action.</param>
        /// <param name="selection">La selection.</param>
        /// <returns><c>true</c> si le déplacement a été effectué correctement.</returns>
        private bool MoveUp(TActionItem item, TActionItem selection)
        {
            var index = this.Items.IndexOf(item);
            var previousIndex = index - 1;

            if (index > 0)
            {
                // L'élément précédent doit être une action
                var previousItem = this.Items[previousIndex] as TActionItem;
                if (previousItem == null)
                    return false;

                return MoveUp(item, previousItem, selection);
            }
            else
                return false;
        }

        /// <summary>
        /// Déplace l'action spécifiée au dessus de l'élément spécifié puis sélectionne l'élément spécifié.
        /// </summary>
        /// <param name="item">L'action.</param>
        /// <param name="upToThisOne">L'action au dessus de laquelle l'élément doit se positionner.</param>
        /// <param name="selection">La sélection.</param>
        /// <returns><c>true</c> si le déplacement a été effectué correctement.</returns>
        private bool MoveUp(TActionItem item, TActionItem upToThisOne, TActionItem selection)
        {
            var actions = GetActionsSortedByWBS();

            // Déterminer le décalage entre l'élément précédent et l'élément cible, à partir du WBS
            var previousItemWBS = upToThisOne.Action.WBS;
            var itemWBS = item.Action.WBS;

            var offset = WBSHelper.GetVerticalOffset(upToThisOne.Action, item.Action, actions);

            for (int i = 0; i < offset; i++)
            {
                MoveUpOnce(item, actions);
                // Rafraichir les actions et leur ordre
                actions = GetActionsSortedByWBS();
            }

            // Déterminer si l'action, une fois déplacée, doit être désindentée
            var newIndex = this.Items.IndexOf(item);
            if (item.Indentation > 0 && newIndex > 0)
            {
                var newPreviousItem = this.Items[this.Items.IndexOf(item) - 1];
                if (newPreviousItem.Indentation < item.Indentation - 1)
                {
                    Unindent(GetActionsSortedByWBS(), item.Action, UnindentationBehavior.PutBelow);
                    item.Indentation--;
                }
            }

            CheckGroups(item.ParentReferentialItem);
            SetCurrentItemAsync(selection);
            DebugCheckAllWBS();
            return true;
        }

        /// <summary>
        /// Déplace l'élément spécifié vers le haut, d'une case.
        /// </summary>
        /// <param name="item">L'élément spécifié.</param>
        /// <param name="actions">Les actions.</param>
        private void MoveUpOnce(TActionItem item, KAction[] actions)
        {
            var actionIndex = Array.IndexOf(actions, item.Action);
            var actionIndentation = WBSHelper.IndentationFromWBS(item.Action.WBS);

            // Déterminer si à l'index - 1, l'élément pourra garder son indentation
            bool shouldDecreaseIndentation;
            if (actionIndentation == 0)
                shouldDecreaseIndentation = false;
            else if (actionIndex - 2 < 0)
                shouldDecreaseIndentation = true;
            else
            {
                shouldDecreaseIndentation = true;
                for (int i = 0; i <= actionIndex - 2; i++)
                {
                    var indentation = WBSHelper.IndentationFromWBS(actions[i].WBS);
                    if (indentation == actionIndentation - 1)
                    {
                        shouldDecreaseIndentation = false;
                        break;
                    }
                }
            }

            if (shouldDecreaseIndentation)
            {
                // Ex. : on monte T1 dans :
                //RES1
                //    G1 1
                //        T1 1.1
                //        T2 1.2

                Unindent(actions, item.Action, UnindentationBehavior.PutAbove);
            }
            else
            {
                var previousAction = actions[actionIndex - 1];
                var previousActionIndentation = WBSHelper.IndentationFromWBS(previousAction.WBS);

                // Si le précédent est du même niveau, il faut augmenter son WBS
                if (actionIndentation == previousActionIndentation)
                {

                    // Ex. : on monte T2 dans :
                    //RES1
                    //    G1 1
                    //        T1 1.1
                    //        T2 1.2

                    previousAction.WBS = WBSHelper.MoveDown(previousAction.WBS, actionIndentation);
                    item.Action.WBS = WBSHelper.MoveUp(item.Action.WBS, actionIndentation);
                }

                // Si le précédent est à un niveau plus bas, il faut décaler son WBS à lui uniquement
                else if (actionIndentation < previousActionIndentation)
                {
                    // S'il y a déjà d'autres éléments en dessous de previousAction, les descendre
                    var children = WBSHelper.GetDescendants(item.Action, actions);
                    foreach (var child in children)
                        child.WBS = WBSHelper.MoveDown(child.WBS, actionIndentation + 1);

                    previousAction.WBS = WBSHelper.MoveDown(previousAction.WBS, actionIndentation);
                    previousAction.WBS = WBSHelper.SetNumberAtLevel(previousAction.WBS, actionIndentation + 1, 1);

                    // Désindenter jusqu'à ce que l'action soit au niveau +1
                    while (WBSHelper.IndentationFromWBS(previousAction.WBS) > actionIndentation + 1)
                        previousAction.WBS = WBSHelper.Unindent(previousAction.WBS);
                }
                else
                {
                    // Ex. : On monte T21
                    //        T1 1.1
                    //            T1 1.1.1
                    //        T2 1.2
                    //            T21 1.2.1

                    // Déterminer l'index du dernier enfant du précédent
                    KAction lastPreviousChild = null;
                    var lastPrecedingSibling = WBSHelper.GetLastPrecedingSibling(previousAction.WBS, actions);
                    if (lastPrecedingSibling != null)
                        lastPreviousChild = WBSHelper.GetLastChild(lastPrecedingSibling, actions);

                    // On décale tout ce qui était après 1.2.1
                    var successiveSiblings = WBSHelper.GetSucessiveSliblingsAndDescendents(item.Action, actions);
                    foreach (var a in successiveSiblings)
                        a.WBS = WBSHelper.MoveUp(a.WBS, actionIndentation);

                    // On passe 1.2.1 à 1.1.1
                    item.Action.WBS = WBSHelper.MoveUp(item.Action.WBS, actionIndentation - 1);

                    if (lastPreviousChild != null)
                    {
                        // On passe 1.1.1 à 1.1.2
                        item.Action.WBS = WBSHelper.SetNumberAtLevel(item.Action.WBS, actionIndentation,
                            WBSHelper.GetNumberAtLevel(lastPreviousChild.WBS, actionIndentation) + 1);
                    }
                }
            }

            // Tous ceux succédant à l'action au même niveau doivent être décalés
            var items = this.ItemsOfTypeAction.Where(i => i.Action == item.Action).ToArray();

            foreach (var i in items)
            {
                i.Indentation = WBSHelper.IndentationFromWBS(item.Action.WBS);
                if (IsViewByReferential())
                    i.Indentation++;

                var itemIndex = this.Items.IndexOf(i);
                var previousItem = this.Items[itemIndex - 1] as TActionItem;

                if (previousItem != null)
                {
                    // Déterminer si l'item doit être déplacé dans l'arbre visuel
                    if (_wbsComparer.Compare(i.Action.WBSParts, previousItem.Action.WBSParts) < 0)
                    {
                        this.Items.RemoveAt(itemIndex);
                        this.Items.Insert(itemIndex - 1, i);
                    }
                }
            }
            _isActionsCollectionDirty = true;
        }

        /// <summary>
        /// Déplace l'élément spécifié vers le bas.
        /// </summary>
        /// <param name="item">L'élément.</param>
        /// <returns><c>true</c> si l'opération a réussi.</returns>
        public bool MoveDown(TActionItem item)
        {
            this.TraceAllWBS();
            this.TraceCollection();

            // Déplacer item vers le bas revient à déplacer item + 1 vers le haut.

            var index = this.Items.IndexOf(item);
            var nextIndex = index + 1;

            if (index < this.Items.Count - 1)
            {
                var nextItem = this.Items[nextIndex] as TActionItem;
                if (nextItem != null)
                {
                    IsMovingItems = true;
                    var ret = MoveUp(nextItem, item);
                    IsMovingItems = false;
                    this.UpdateIsGroup();
                    this.UpdateAllPredecessorsString();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Obtient les descendants d'une action sur la collection des éléments affichés.
        /// </summary>
        /// <param name="item">l'action.</param>
        /// <returns>les descendants.</returns>
        private TActionItem[] GetDescendants(TActionItem item)
        {
            var items = new List<TActionItem>();
            var index = this.Items.IndexOf(item);
            for (int i = index + 1; i < this.Items.Count; i++)
            {
                var next = this.Items[i] as TActionItem;
                if (next == null)
                    break;

                if (next.Indentation <= item.Indentation)
                    break;

                items.Add(next);
            }
            return items.ToArray();
        }

        /// <summary>
        /// Recherche le frère le plus proche dans la directions spécifiée.
        /// </summary>
        /// <param name="item">L'élément.</param>
        /// <param name="direction">La direction.</param>
        /// <returns>Le frère le plus proche ou null s'il n'y a pas.</returns>
        private TActionItem GetNearestSibling(TActionItem item, VerticalDirection direction)
        {
            int index = this.Items.IndexOf(item);
            int i = direction == VerticalDirection.Down ? index + 1 : index - 1;

            TActionItem sibling = null;

            while (true)
            {
                if (direction == VerticalDirection.Up && i < 0)
                    break;
                else if (direction == VerticalDirection.Down && i >= this.Items.Count)
                    break;

                var nextItem = this.Items[i] as TActionItem;
                if (nextItem == null)
                    break;

                if (nextItem.Indentation < item.Indentation)
                    break;
                else if (nextItem.Indentation == item.Indentation)
                {
                    sibling = nextItem;
                    break;
                }

                if (direction == VerticalDirection.Up)
                    i--;
                else
                    i++;
            }

            return sibling;
        }

        /// <summary>
        /// Déplace un élément vers le bas, en le positionnant en dessous de son frère le plus proche.
        /// </summary>
        /// <param name="item">L'action.</param>
        /// <returns><c>true</c> si le déplacement a été effectué correctement.</returns>
        /// <remarks>Ne fonctionne qu'en vue par référentiel</remarks>
        public bool MoveDownBelowSibling(TActionItem item)
        {
            this.TraceAllWBS();
            this.TraceCollection();

            if (IsViewByReferential())
                throw new InvalidOperationException("Ne fonctionne qu'en vue par WBS");

            IsMovingItems = true;

            var ret = false;
            var index = this.Items.IndexOf(item);
            var graph = ActionsGraphManipulator<TActionItem>.Virtualize(this.ItemsOfTypeAction);
            var node = graph.ResolveNodeByFlatIndex(index);
            if (ret = node.MoveDownCommand.CanExecute(null))
            {
                node.MoveDownCommand.Execute(null);
            }

            graph.Apply(this.Items, delete: Unregister);

            IsMovingItems = false;
            if (ret)
            {
                _isActionsCollectionDirty = true;
                FixAllIndentations();
                FixAllWBS();
                this.UpdateIsGroup();
                this.UpdateAllPredecessorsString();
                this.FixPredecessorsSuccessorsTimings();
                this.UpdateVideoGroupsTiming();
                this.UpdateBuildGroupsTiming();
                this.UpdateResourcesLoad();
                this.SetCurrentItemAsync(item);
            }
            return ret;
        }

        /// <summary>
        /// Déplace l'élément spécifié vers la gauche.
        /// </summary>
        /// <param name="item">L'élément.</param>
        /// <returns><c>true</c> si l'opération a réussi.</returns>
        public bool MoveLeft(TActionItem item)
        {
            this.TraceAllWBS();
            this.TraceCollection();

            var actionIndentation = WBSHelper.IndentationFromWBS(item.Action.WBS);
            if (actionIndentation > 0)
            {
                Unindent(GetActionsSortedByWBS(), item.Action, UnindentationBehavior.PutInline);

                // Rafraichir l'indentation pour qu'elle corresponde 
                foreach (var i in this.ItemsOfTypeAction)
                {
                    i.Indentation = WBSHelper.IndentationFromWBS(i.Action.WBS);
                    if (IsViewByReferential())
                        i.Indentation++;
                }

                _isActionsCollectionDirty = true;
                DebugCheckAllWBS();
                this.UpdateIsGroup();
                this.UpdateAllPredecessorsString();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Déplace les éléments spécifiés vers la gauche.
        /// </summary>
        /// <param name="items">Les éléments.</param>
        /// <returns>
        ///   <c>true</c> si l'opération a réussi.
        /// </returns>
        /// <remarks>Ne fonctionne que vue par WBS.</remarks>
        public bool MoveLeft(IEnumerable<TActionItem> items)
        {
            this.TraceAllWBS();
            this.TraceCollection();

            if (IsViewByReferential())
                return false;

            TActionItem[] itemsWithMinIndentation;

            if (!AreItemsValidForMultipleAdjacentOperations(items, out itemsWithMinIndentation))
                return false;

            var itemsTomove = this.GetItemsWithDescendants(itemsWithMinIndentation);


            foreach (var item in itemsTomove)
            {
                if (item.Indentation > 0)
                {
                    var index = this.Items.IndexOf(item);
                    var nextItem = index < this.Items.Count - 1 ? this.Items[index + 1] : null;

                    if (nextItem == null || item.Indentation >= nextItem.Indentation)
                    {
                        item.Indentation--;
                    }
                }
            }

            _isActionsCollectionDirty = true;
            FixAllIndentations();
            FixAllWBS();
            this.UpdateIsGroup();
            this.UpdateAllPredecessorsString();
            return true;
        }

        /// <summary>
        /// Déplace l'élément spécifié vers la droite.
        /// </summary>
        /// <param name="item">L'élément.</param>
        /// <returns><c>true</c> si l'opération a réussi.</returns>
        public bool MoveRight(TActionItem item)
        {
            this.TraceAllWBS();
            this.TraceCollection();

            var actions = GetActionsSortedByWBS();
            var actionIndentation = WBSHelper.IndentationFromWBS(item.Action.WBS);
            var actionIndex = this.Items.IndexOf(item);
            if (actionIndex > 0)
            {
                var previousItem = this.Items[actionIndex - 1] as TActionItem;
                if (previousItem != null && item.Indentation <= previousItem.Indentation)
                {
                    Indent(item, actions);

                    // Rafraichir l'indentation pour qu'elle corresponde 
                    foreach (var i in this.ItemsOfTypeAction)
                    {
                        i.Indentation = WBSHelper.IndentationFromWBS(i.Action.WBS);
                        if (IsViewByReferential())
                            i.Indentation++;
                    }

                    _isActionsCollectionDirty = true;
                    this.UpdateIsGroup();
                    this.UpdateAllPredecessorsString();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Déplace les éléments spécifiés vers la droite.
        /// </summary>
        /// <param name="items">Les éléments.</param>
        /// <returns>
        ///   <c>true</c> si l'opération a réussi.
        /// </returns>
        /// <remarks>Ne fonctionne que vue par WBS.</remarks>
        public bool MoveRight(IEnumerable<TActionItem> items)
        {
            this.TraceAllWBS();
            this.TraceCollection();

            if (IsViewByReferential())
                return false;

            TActionItem[] itemsWithMinIndentation;

            if (!AreItemsValidForMultipleAdjacentOperations(items, out itemsWithMinIndentation))
                return false;

            var itemsTomove = this.GetItemsWithDescendants(itemsWithMinIndentation);


            foreach (var item in itemsTomove)
            {
                var index = this.Items.IndexOf(item);
                if (index > 0)
                {
                    var previousItem = this.Items[index - 1];

                    if (item.Indentation <= previousItem.Indentation)
                        item.Indentation++;
                }
            }

            _isActionsCollectionDirty = true;
            FixAllIndentations();
            FixAllWBS();
            this.UpdateIsGroup();
            this.UpdateAllPredecessorsString();
            return true;
        }

        /// <summary>
        /// Indente l'élément spécifié
        /// </summary>
        /// <param name="item">L'élément.</param>
        /// <param name="actions">Les actions.</param>
        private void Indent(TActionItem item, IEnumerable<KAction> actions)
        {
            var actionIndentation = WBSHelper.IndentationFromWBS(item.Action.WBS);

            var children = WBSHelper.GetDescendants(item.Action, actions).ToArray();
            var successiveSiblings = WBSHelper.GetSucessiveSliblingsAndDescendents(item.Action, actions).ToArray();

            int startIndex = 1;
            var nextParent = WBSHelper.GetLastPrecedingSibling(item.Action.WBS, actions);
            if (nextParent != null)
            {
                var lastChild = WBSHelper.GetLastChild(nextParent, actions);
                if (lastChild != null)
                    startIndex = WBSHelper.GetNumberAtLevel(lastChild.WBS, actionIndentation + 1) + 1;
            }

            item.Action.WBS = WBSHelper.Indent(item.Action.WBS);
            item.Action.WBS = WBSHelper.MoveUp(item.Action.WBS, actionIndentation);

            // On définit l'indentation de l'élément comme succesive à son prédécesseur

            item.Action.WBS = WBSHelper.SetNumberAtLevel(item.Action.WBS, actionIndentation + 1, startIndex);

            // Pour tous les enfants, modifier les premiers niveaux
            // Ex: l'indentation de T2 décale++ T21 & T22 et décale-- T3

            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            foreach (var child in children)
            {
                child.WBS = WBSHelper.CopyNumberAtLevel(item.Action.WBS, child.WBS, actionIndentation);
                child.WBS = WBSHelper.SetNumberAtLevel(child.WBS, actionIndentation + 1, WBSHelper.GetNumberAtLevel(child.WBS, actionIndentation + 1) + startIndex);
                //child.WBS = WBSHelper.MoveDown(child.WBS, actionIndentation + 1);
            }

            foreach (var sibling in successiveSiblings)
            {
                sibling.WBS = WBSHelper.MoveUp(sibling.WBS, actionIndentation);
            }
            _isActionsCollectionDirty = true;
        }

        /// <summary>
        /// Désindente l'action spécifié.
        /// Il n'est pas censé avoir d'enfants.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        /// <param name="action">L'action.</param>
        /// <param name="behavior">The behavior.</param>
        private void Unindent(KAction[] actions, KAction action, UnindentationBehavior behavior)
        {
            ActionsTimingsMoveManagement.Unindent(actions, action, behavior);
            _isActionsCollectionDirty = true;
        }

        /// <summary>
        /// Détermine si les éléments spécifiés peuvent être groupés.
        /// </summary>
        /// <param name="items">Les éléments.</param>
        /// <returns>
        ///   <c>true</c> si les éléments spécifiés peuvent être groupés.; sinon, <c>false</c>.
        /// </returns>
        public bool CanGroup(IEnumerable<TActionItem> items)
        {
            TActionItem[] itemsWithMinIndentation;
            return AreItemsValidForMultipleAdjacentOperations(items, out itemsWithMinIndentation);
        }

        /// <summary>
        /// Détermine si les éléments spécifiés sont valides pour une opération requérant plusieurs éléments contigus.
        /// </summary>
        /// <param name="items">Les éléments.</param>
        /// <param name="itemsWithMinIndentation">Les éléments qui ont l'indentation minimale.</param>
        /// <returns>
        ///   <c>true</c> si cette instance [can group private] le items spécifié; sinon, <c>false</c>.
        /// </returns>
        private bool AreItemsValidForMultipleAdjacentOperations(IEnumerable<TActionItem> items, out TActionItem[] itemsWithMinIndentation)
        {
            if (IsViewByReferential())
            {
                itemsWithMinIndentation = null;
                return false;
            }

            if (!items.Any())
            {
                itemsWithMinIndentation = null;
                return false;
            }

            var minIndentation = items.Min(i => WBSHelper.IndentationFromWBS(i.Action.WBS));

            itemsWithMinIndentation = items.Where(i => WBSHelper.IndentationFromWBS(i.Action.WBS) == minIndentation).ToArray();
            var hasMultipleElementOnMinIndentation = itemsWithMinIndentation.Length > 1;

            // Vérifier que les éléments avec minIndentation soient contigus
            if (hasMultipleElementOnMinIndentation)
            {
                var valuesAtIndentation = itemsWithMinIndentation
                    .Select(i => WBSHelper.GetNumberAtLevel(i.Action.WBS, minIndentation))
                    .OrderBy(v => v);

                int previous = -1;
                foreach (var value in valuesAtIndentation)
                {
                    if (previous != -1)
                        if (previous + 1 != value)
                            return false;

                    previous = value;
                }
            }

            return true;
        }

        /// <summary>
        /// Obtient une collection contenant les éléments spécifiés ainsi que tous leurs descendants.
        /// </summary>
        /// <param name="items">Les éléments.</param>
        /// <returns>Une collection contenant les éléments spécifiés ainsi que tous leurs descendants.</returns>
        /// <remarks>Ne fonctionne qu'en vue par WBS</remarks>
        private List<TActionItem> GetItemsWithDescendants(IEnumerable<TActionItem> items)
        {
            if (IsViewByReferential())
                throw new NotSupportedException("Ne fonctionne qu'en vue par WBS");

            var itemsToMove = new List<TActionItem>();

            // Déterminer tous les éléments impactés
            foreach (var actionItem in items)
            {
                itemsToMove.Add(actionItem);

                // Chercher tous les enfants
                for (int i = this.Items.IndexOf(actionItem) + 1; i < this.Items.Count; i++)
                {
                    var currentItem = (TActionItem)this.Items[i];
                    if (currentItem.Indentation <= actionItem.Indentation)
                        break;

                    itemsToMove.Add(currentItem);
                }
            }

            return itemsToMove;
        }

        /// <summary>
        /// Crée un groupe qui contiendra tous les éléments spécifié.
        /// </summary>
        /// <param name="items">Les éléments.</param>
        /// <returns>
        ///   L'élément groupant créé.
        /// </returns>
        /// <remarks>
        /// N'est disponible qu'en vue par WBS.
        /// </remarks>
        public TActionItem Group(IEnumerable<TActionItem> items)
        {
            this.TraceAllWBS();
            this.TraceCollection();

            TActionItem[] itemsWithMinIndentation;

            if (!AreItemsValidForMultipleAdjacentOperations(items, out itemsWithMinIndentation))
                throw new InvalidOperationException("Impossible de grouper les éléments spécifiés. Utiliser CanGroup avant d'appeler Group");

            // Créer l'action
            var action = new KAction();
            var groupItem = CreateNewActionItem(action);
            groupItem.Indentation = itemsWithMinIndentation.First().Indentation;

            // Positionner l'item
            var topItemWithMinIndentation = itemsWithMinIndentation.OrderBy(a => a.Action.WBSParts, _wbsComparer).First();
            var insertIndex = this.Items.IndexOf(topItemWithMinIndentation);

            this.Items.Insert(insertIndex, groupItem);

            var itemsToMove = GetItemsWithDescendants(itemsWithMinIndentation);

            // Décaler les éléments
            foreach (var actionItem in itemsToMove)
                actionItem.Indentation++;

            // Mettre à jour le WBS
            _isActionsCollectionDirty = true;
            FixAllWBS();

            this.Register(groupItem);

            SetCurrentItemAsync(groupItem);
            DebugCheckAllWBS();
            this.UpdateIsGroup();
            this.UpdateAllPredecessorsString();
            this.UpdateVideoGroupsTiming();
            this.UpdateBuildGroupsTiming();

            return groupItem;
        }

       public ActionGridItem GetItem(KAction action)
        {
            return CreateNewActionItem(action) as ActionGridItem;
        }
        /// <summary>
        /// Prédit les actions qui vont être modifiées par une action de dégroupage.
        /// </summary>
        /// <param name="groupItem">Le groupe à dégrouper.</param>
        /// <returns>Les actions modifiées.</returns>
        /// <remarks>
        /// Le groupe sera supprimé. Il n'est pas inclus dans le tableau de retour.
        /// </remarks>
        public KAction[] PredictUngroupModifiedAction(TActionItem groupItem)
        {
            return WBSHelper.GetSucessiveSliblingsAndDescendents(groupItem.Action, GetActionsSortedByWBS())
                .Union(WBSHelper.GetDescendants(groupItem.Action, GetActionsSortedByWBS()))
                .ToArray();
        }

        /// <summary>
        /// Dégroupe l'élément spécifié
        /// </summary>
        /// <param name="groupItem">Le groupe.</param>
        /// <remarks>
        /// N'est disponible qu'en vue par WBS.
        /// </remarks>
        public void Ungroup(TActionItem groupItem)
        {
            this.TraceAllWBS();
            this.TraceCollection();

            if (!groupItem.IsGroup.GetValueOrDefault())
                throw new InvalidOperationException("L'élément doit être un groupe");

            if (IsViewByReferential())
                throw new InvalidOperationException("Ne fonctionne qu'en vue par WBS");

            //var groupSuccessiveSiblings = WBSHelper.GetSucessiveSliblingsAndDescendents(groupItem.Action, GetActionsSortedByWBS()).ToArray();

            var descendants = WBSHelper.GetDescendants(groupItem.Action, GetActionsSortedByWBS()).ToArray();

            // Décaler les descendants vers la gauche
            foreach (var desc in descendants)
            {
                var item = this.ItemsOfTypeAction.First(i => i.Action == desc);
                item.Indentation--;
            }

            // Supprimer le groupe
            this.Items.Remove(groupItem);
            Unregister(groupItem);

            _isActionsCollectionDirty = true;
            this.FixAllWBS();
            this.DebugCheckAllWBS();
            this.UpdateIsGroup();
            this.UpdateAllPredecessorsString();
            this.UpdateResourcesLoad();
        }

        #endregion

        #region Gestion des évènements

        private Dictionary<TComponentItem, List<IDisposable>> _subscriptions = new Dictionary<TComponentItem, List<IDisposable>>();

        /// <summary>
        /// Survient lorsque le timing (début ou fin) vidéo d'une tâche a changé.
        /// </summary>
        public event EventHandler<EventArgs<KAction>> ActionVideoTimingChanged;

        /// <summary>
        /// Lève l'évènement <see cref="ActionVideoTimingChanged"/>.
        /// </summary>
        /// <param name="action">L'action.</param>
        protected virtual void OnActionVideoTimingChanged(KAction action)
        {
            if (ActionVideoTimingChanged != null)
                ActionVideoTimingChanged(this, new EventArgs<KAction>(action));
        }

        /// <summary>
        /// Survient lorsque la propriété "Approved" d'une action réduite a changé.
        /// </summary>
        public event EventHandler<EventArgs<KActionReduced>> ReducedApprovedChanged;

        /// <summary>
        /// Lève l'évènement <see cref="ActionVideoTimingChanged"/>.
        /// </summary>
        /// <param name="reduced">L'action réduite.</param>
        protected void OnReducedApprovedChanged(KActionReduced reduced)
        {
            if (ReducedApprovedChanged != null)
                ReducedApprovedChanged(this, new EventArgs<KActionReduced>(reduced));
        }

        /// <summary>
        /// S'abonne à des évènements sur l'élément spécifié.
        /// </summary>
        /// <param name="item">L'élément.</param>
        protected virtual void Register(TActionItem item)
        {
            item.Action.LabelChanged += new EventHandler<PropertyChangedEventArgs<string>>(OnActionLabelChanged);
            item.Action.WBSChanged += new EventHandler<PropertyChangedEventArgs<string>>(OnActionWBSChanged);
            item.Action.StartChanged += new EventHandler<PropertyChangedEventArgs<long>>(OnActionStartChanged);
            item.Action.FinishChanged += new EventHandler<PropertyChangedEventArgs<long>>(OnActionFinishChanged);
            item.Action.BuildStartChanged += new EventHandler<PropertyChangedEventArgs<long>>(OnBuildActionStartChangedThunk);
            item.Action.BuildFinishChanged += new EventHandler<PropertyChangedEventArgs<long>>(OnBuildActionFinishChangedThunk);
            item.Action.VideoChanged += new EventHandler<PropertyChangedEventArgs<Video>>(OnActionVideoChanged);

            item.Action.ThumbnailHashChanged += OnThumbnailHashChanged;
            item.Action.ResourceChanged += new EventHandler<PropertyChangedEventArgs<Resource>>(OnActionResourceChanged);

            item.Action.ReducedChanged += new EventHandler<PropertyChangedEventArgs<KActionReduced>>(OnActionReducedChanged);
            RegisterReduced(item.Action.Reduced);
            item.HierarchyChanged += new EventHandler(OnItemHierarchyChanged);
            item.PredecessorsStringChanged += new EventHandler(OnItemPredecessorsStringChanged);

            SubscribeToPropertyChanged(item, "IsExpanded", OnIsExpandedChanged);

            UpdateVideoColor(item);
            //UpdateThumbnail(item.Action);
            UpdateMultipleReferentialsLabels(item);
        }

        /// <summary>
        /// Met à jour les libellés des référentiels multiples.
        /// </summary>
        /// <param name="item">L'action</param>
        private void UpdateMultipleReferentialsLabels(TActionItem item)
        {
            item.Ref1Labels = GetMultiReferentialLabels(item.Action.Ref1, ProcessReferentialIdentifier.Ref1);
            item.Ref2Labels = GetMultiReferentialLabels(item.Action.Ref2, ProcessReferentialIdentifier.Ref2);
            item.Ref4Labels = GetMultiReferentialLabels(item.Action.Ref4, ProcessReferentialIdentifier.Ref4);
            item.Ref3Labels = GetMultiReferentialLabels(item.Action.Ref3, ProcessReferentialIdentifier.Ref3);
            item.Ref5Labels = GetMultiReferentialLabels(item.Action.Ref5, ProcessReferentialIdentifier.Ref5);
            item.Ref6Labels = GetMultiReferentialLabels(item.Action.Ref6, ProcessReferentialIdentifier.Ref6);
            item.Ref7Labels = GetMultiReferentialLabels(item.Action.Ref7, ProcessReferentialIdentifier.Ref7);
        }

        /// <summary>
        /// Obtient les libellés des référentiels de l'action concaténés, en fonction de leur utilisation et de leurs options.
        /// </summary>
        /// <param name="links">Les liens Référentiel - Action.</param>
        /// <param name="refeId">L'identifiant de chaque référentiel utilisé.</param>
        /// <returns>La chaîne concaténée.</returns>
        private string GetMultiReferentialLabels(IEnumerable<IReferentialActionLink> links, ProcessReferentialIdentifier refeId)
        {
            if (DesignMode.IsInDesignMode)
                return string.Join(RefLabelsSeparator, links.Select(al => al.Referential.Label));

            var service = IoC.Resolve<IReferentialsUseService>();
            var useRef = service.IsReferentialEnabled(refeId);
            if (useRef)
            {

                var useQuantity = service.Referentials[refeId].HasQuantity;
                IEnumerable<string> values;
                if (useQuantity)
                    values = links.Select(al => string.Format(LocalizationManager.GetString("Common_Referentials_QuantityDescription"), al.Quantity, al.Referential.Label));
                else
                    values = links.Select(al => al.Referential.Label);
                return string.Join(RefLabelsSeparator, values);
            }
            else
                return null;
        }

        /// <summary>
        /// S'abonne à ProeprtyChanged sur une propriété en particulier.
        /// </summary>
        /// <param name="item">L'élément sur lequel s'abonner.</param>
        /// <param name="propertyName">Le nom de la propriété.</param>
        /// <param name="eventHandler">L'event handler gérant l'abonnement.</param>
        private void SubscribeToPropertyChanged(TComponentItem item, string propertyName, EventHandler eventHandler)
        {
            var subscription = Observable.FromEventPattern<System.ComponentModel.PropertyChangedEventArgs>(item, "PropertyChanged")
                .Where(e => string.Compare(e.EventArgs.PropertyName, propertyName, true) == 0)
                .Subscribe(e => eventHandler(e.Sender, e.EventArgs));

            if (!_subscriptions.ContainsKey(item))
                _subscriptions[item] = new List<IDisposable>();

            _subscriptions[item].Add(subscription);
        }

        /// <summary>
        /// Se désabonne à tous les PropertyChanged sur l'élément.
        /// </summary>
        /// <param name="item">L'élément.</param>
        private void UnsubscribeToIsExpandedChanged(TComponentItem item)
        {
            if (_subscriptions.ContainsKey(item))
            {
                foreach (var subscription in _subscriptions[item])
                    subscription.Dispose();
            }
        }

        /// <summary>
        /// S'abonne à des évènements sur l'élément spécifié.
        /// </summary>
        /// <param name="item">L'élément.</param>
        protected virtual void Register(TReferentialItem item)
        {
            SubscribeToPropertyChanged(item, "IsExpanded", OnIsExpandedChanged);
        }

        /// <summary>
        /// Se désabonne à des évènements sur l'action réduite spécifiée.
        /// </summary>
        /// <param name="item">L'élément.</param>
        private void RegisterReduced(KActionReduced reduced)
        {
            if (reduced != null)
            {
                UpdateReductionDescription(reduced.Action);
                reduced.ReductionRatioChanged += new EventHandler<PropertyChangedEventArgs<double>>(OnActionReducedReductionRatioChangedChanged);
                reduced.ActionTypeChanged += new EventHandler<PropertyChangedEventArgs<ActionType>>(OnActionReducedActionTypeChanged);
                reduced.ApprovedChanged += new EventHandler<PropertyChangedEventArgs<bool>>(OnActionReducedApprovedChanged);
                reduced.ResourceChanged += new EventHandler<PropertyChangedEventArgs<bool>>(OnActionReducedResourceChanged);
            }
        }

        /// <summary>
        /// Se désabonne à des évènements sur l'élément spécifié.
        /// </summary>
        /// <param name="item">L'élément.</param>
        protected virtual void Unregister(TActionItem item)
        {
            item.Action.LabelChanged -= new EventHandler<PropertyChangedEventArgs<string>>(OnActionLabelChanged);
            item.Action.WBSChanged -= new EventHandler<PropertyChangedEventArgs<string>>(OnActionWBSChanged);
            item.Action.StartChanged -= new EventHandler<PropertyChangedEventArgs<long>>(OnActionStartChanged);
            item.Action.FinishChanged -= new EventHandler<PropertyChangedEventArgs<long>>(OnActionFinishChanged);
            item.Action.BuildStartChanged -= new EventHandler<PropertyChangedEventArgs<long>>(OnBuildActionStartChangedThunk);
            item.Action.BuildFinishChanged -= new EventHandler<PropertyChangedEventArgs<long>>(OnBuildActionFinishChangedThunk);
            item.Action.VideoChanged -= new EventHandler<PropertyChangedEventArgs<Video>>(OnActionVideoChanged);

            item.Action.ThumbnailHashChanged -= OnThumbnailHashChanged;
            item.Action.ResourceChanged -= new EventHandler<PropertyChangedEventArgs<Resource>>(OnActionResourceChanged);

            item.Action.ReducedChanged -= new EventHandler<PropertyChangedEventArgs<KActionReduced>>(OnActionReducedChanged);
            UnregisterReduced(item.Action.Reduced);
            item.HierarchyChanged -= new EventHandler(OnItemHierarchyChanged);
            item.PredecessorsStringChanged -= new EventHandler(OnItemPredecessorsStringChanged);
            UnsubscribeToIsExpandedChanged(item);
        }

        /// <summary>
        /// Se désabonne à des évènements sur l'élément spécifié.
        /// </summary>
        /// <param name="item">L'élément.</param>
        protected virtual void Unregister(TReferentialItem item)
        {
            UnsubscribeToIsExpandedChanged(item);
        }

        /// <summary>
        /// Se désabonne à des évènements sur l'action réduite spécifiée.
        /// </summary>
        /// <param name="item">L'élément.</param>
        private void UnregisterReduced(KActionReduced reduced)
        {
            if (reduced != null)
            {
                reduced.ReductionRatioChanged -= new EventHandler<PropertyChangedEventArgs<double>>(OnActionReducedReductionRatioChangedChanged);
                reduced.ActionTypeChanged -= new EventHandler<PropertyChangedEventArgs<ActionType>>(OnActionReducedActionTypeChanged);
                reduced.ApprovedChanged -= new EventHandler<PropertyChangedEventArgs<bool>>(OnActionReducedApprovedChanged);
                reduced.ResourceChanged -= new EventHandler<PropertyChangedEventArgs<bool>>(OnActionReducedResourceChanged);
            }
        }

        /// <summary>
        /// Appelé lorsque le libellé de l'action a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.EventArgs&lt;System.String&gt;"/> contenant les données de l'évènement.</param>
        private void OnActionLabelChanged(object sender, PropertyChangedEventArgs<string> e)
        {
            var action = (KAction)sender;
            foreach (var item in this.ItemsOfTypeAction.Where(i => i.Action == action))
                item.UpdateContent();
        }

        /// <summary>
        /// Appelé lorsque le WBS de l'action a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.EventArgs&lt;System.String&gt;"/> contenant les données de l'évènement.</param>
        private void OnActionWBSChanged(object sender, PropertyChangedEventArgs<string> e)
        {
            var action = (KAction)sender;
            foreach (var item in this.ItemsOfTypeAction.Where(i => i.Action == action))
                item.UpdateContent();
        }

        /// <summary>
        /// Appelé lorsque le début de l'action a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.EventArgs&lt;System.Int64&gt;"/> contenant les données de l'évènement.</param>
        private void OnActionStartChanged(object sender, PropertyChangedEventArgs<long> e)
        {
            var action = (KAction)sender;

            if (_updateActionTimingSource != action && !_isCheckingMarkers && !NotVerifMarkers)
            {
                CheckMarkerMovingConditions(action, true, e.OldValue, e.NewValue);
            }
            if (!_isCheckingMarkers)
            {
                OnActionVideoTimingChanged(action);
                UpdateVideoGroupsTiming();
                UpdateBuildGroupsTiming();
            }
        }

        /// <summary>
        /// Appelé lorsque la fin de l'action a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.EventArgs&lt;System.Int64&gt;"/> contenant les données de l'évènement.</param>
        private void OnActionFinishChanged(object sender, PropertyChangedEventArgs<long> e)
        {
            var action = (KAction)sender;

            if (_updateActionTimingSource != action && !_isCheckingMarkers)
            {
                CheckMarkerMovingConditions(action, false, e.OldValue, e.NewValue);
            }
            if (!_isCheckingMarkers)
            {
                OnActionVideoTimingChanged(action);
                UpdateVideoGroupsTiming();
                UpdateBuildGroupsTiming();
            }
        }

        private void CheckMarkerMovingConditions(KAction action, bool movedStart, long oldValue, long newValue)
        {
            _isCheckingMarkers = true;
            //Si les markers ne sont pas liés, on les déplace comme on veut ??? A vérifier
            if (this.AreMarkersLinked)
            {
                long min = oldValue - 1000000;
                long max = oldValue + 1000000;

                Func<KAction, bool> predicate;
                if (movedStart)
                {
                    predicate = a => a.Finish >= min && a.Finish <= max;
                }
                else
                {
                    predicate = a => a.Start >= min && a.Start <= max;
                }

                var linkedActions =
                    this.ItemsOfTypeAction.Select(i => i.Action)
                    .Distinct()
                    .Where(a =>
                        a != action &&
                        a.Video != null &&
                        a.Video == action.Video &&
                        WBSHelper.StartsWithSameExceptLast(a.WBS, action.WBS) &&
                        predicate(a))
                        .ToList();

                if (_lastMarkerMoved == null || _lastMarkerMoved.ActionId != action.ActionId)
                {
                    _maxStopValue = long.MaxValue;
                    _minStopValue = long.MinValue;


                }


                KAction linkedAction = null;
                if (linkedActions.Count > 0)
                {
                    if (movedStart)
                        linkedAction = linkedActions.MaxWithValue(a => a.Start);
                    else
                        linkedAction = linkedActions.MinWithValue(a => a.Finish);
                }

                //Si on déplace le marqueur de début 
                // ET si on atteint le marqueur de fin de la tache:
                // - on arrête le marqueur
                // -  ET on ne met pas à jour les autes marqueurs
                if (action.Start >= action.Finish && movedStart)
                {
                    action.Start = action.Finish;
                    if (newValue < _maxStopValue
                        && newValue > _minStopValue)
                    {
                        if (linkedAction != null)
                            linkedAction.Finish = action.Finish;
                        _maxStopValue = action.Finish;
                    }
                }

                // Si on déplace le marqueur de fin 
                // ET si on atteint le marqueur début de la tache
                // - on arrête le marqueur
                // -  ET on ne met pas à jour les autes markers
                else if (action.Start >= action.Finish && !movedStart)
                {
                    action.Finish = action.Start;
                    if (newValue < _maxStopValue
                         && newValue > _minStopValue)
                    {
                        if (linkedAction != null)
                            linkedAction.Start = action.Start;
                        _minStopValue = action.Start;
                    }
                }

                // Si on déplace le marqueur de fin 
                // ET si on atteint la fin de la tache suivante:
                // - on arrête le marqueur
                // -  ET on ne met pas à jour les autes markers
                else if (linkedAction != null
                    && !movedStart
                    && action.Finish > linkedAction.Finish)
                {
                    action.Finish = linkedAction.Finish;

                    if (linkedAction != null
                       && newValue < _maxStopValue
                       && newValue > _minStopValue)
                    {
                        linkedAction.Start = linkedAction.Finish;
                        _maxStopValue = linkedAction.Finish;
                    }
                }

                //Si on déplace le marqueur de début
                // ET Si on atteint le début de la tache précédente:
                // - on arrête le marqueur
                // - ET on ne met pas à jour les autes marqueurs
                else if (linkedAction != null
                    && movedStart
                    && action.Start < linkedAction.Start)
                {
                    action.Start = linkedAction.Start;

                    if (linkedAction != null
                       && newValue < _maxStopValue
                       && newValue > _minStopValue)
                    {
                        linkedAction.Finish = linkedAction.Start;
                        _minStopValue = linkedAction.Start;
                    }
                }

                //Si on n'a atteint aucune des bornes, on met à jours les marqueurs liés
                else if (_updatingLinkedAction != action
                        && newValue < _maxStopValue
                        && newValue > _minStopValue)
                {
                    UpdateLinkedAction(action, movedStart, oldValue, newValue, linkedAction);
                }

                if (newValue < _maxStopValue
                        && newValue > _minStopValue)
                {
                    _maxStopValue = long.MaxValue;
                    _minStopValue = long.MinValue;
                }
                _lastMarkerMoved = action;
            }
            else //Les marqueurs sont déliés
            {
                //Si on déplace le marqueur de début 
                // ET si on atteint le marqueur de fin de la tache:
                // - on arrête le marqueur
                // -  ET on ne met pas à jour les autes marqueurs
                if (action.Start >= action.Finish && movedStart)
                {
                    action.Start = action.Finish;
                    if (newValue < _maxStopValue
                        && newValue > _minStopValue)
                    {
                        _maxStopValue = action.Finish;
                    }
                }

                // Si on déplace le marqueur de fin 
                // ET si on atteint le marqueur début de la tache
                // - on arrête le marqueur
                // -  ET on ne met pas à jour les autes markers
                else if (action.Start >= action.Finish && !movedStart)
                {
                    action.Finish = action.Start;
                    if (newValue < _maxStopValue
                         && newValue > _minStopValue)
                    {
                        _minStopValue = action.Start;
                    }
                }
            }
            _isCheckingMarkers = false;
        }


        /// <summary>
        /// Appelé lorsque le début de l'action (dans le process) a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.EventArgs&lt;System.Int64&gt;"/> contenant les données de l'évènement.</param>
        private void OnBuildActionStartChangedThunk(object sender, PropertyChangedEventArgs<long> e)
        {
            var action = (KAction)sender;

            if (_updateActionTimingSource != action)
            {
                if (!IsFixingPredecessorsSuccessorsTiming)
                {
                    if (!this.AllowTimingsDurationChange)
                    {
                        // Si le changement de durée n'est pas autorisé, déplacer la fin
                        action.BuildFinish += e.NewValue - e.OldValue;
                    }

                    if (!action.IsGroup)
                        this.FixPredecessorsSuccessorsTimings();
                }
                OnBuildActionStartChanged(action);
            }
            UpdateBuildGroupsTiming();
            this.UpdateResourcesLoad();
        }

        /// <summary>
        /// Appelé lorsque le début de l'action (dans le process) a changé.
        /// </summary>
        /// <param name="action">L'action.</param>
        protected virtual void OnBuildActionStartChanged(KAction action)
        {
        }

        /// <summary>
        /// Appelé lorsque la fin de l'action (dans le process) a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.EventArgs&lt;System.Int64&gt;"/> contenant les données de l'évènement.</param>
        private void OnBuildActionFinishChangedThunk(object sender, PropertyChangedEventArgs<long> e)
        {
            var action = (KAction)sender;

            if (_updateActionTimingSource != action)
            {
                if (!action.IsGroup)
                    this.FixPredecessorsSuccessorsTimings();
                OnBuildActionFinishChanged(action);

                UpdateBuildGroupsTiming();
                UpdateReducedReductionFromTimings(action);
                UpdateResourcesLoad();
            }
        }

        /// <summary>
        /// Appelé lorsque la fin de l'action (dans le process) a changé.
        /// </summary>
        /// <param name="action">L'action.</param>
        protected virtual void OnBuildActionFinishChanged(KAction action)
        {
        }

        /// <summary>
        /// Définit le temps pour une action.
        /// </summary>
        /// <param name="start">Le début.</param>
        /// <param name="finish">La fin.</param>
        protected bool SetActionBuildTiming(KAction action, long start, long finish)
        {
            if (!AllowTimingsChange)
                return false;

            if (action.Predecessors.Any() && action.BuildStart != start)
            {
                // Il s'agit d'un déplacement
                // Lorsque l'on a des prédécesseurs, ce déplacement est impossible
                return false;
            }

            if (_updateActionTimingSource != action &&
                (action.BuildStart != start || action.BuildFinish != finish))
            {
                _updateActionTimingSource = action;

                action.BuildStart = start;
                action.BuildFinish = finish;

                _updateActionTimingSource = null;

                FixPredecessorsSuccessorsTimings();
                UpdateBuildGroupsTiming();
                UpdateReducedReductionFromTimings(action);
                UpdateResourcesLoad();
            }

            return true;
        }

        /// <summary>
        /// Appelé lorsque la vidéo de l'action a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.EventArgs&lt;KProcess.Ksmed.Models.Video&gt;"/> contenant les données de l'évènement.</param>
        private void OnActionVideoChanged(object sender, PropertyChangedEventArgs<Video> e)
        {
            var action = (KAction)sender;
            var items = this.ItemsOfTypeAction.Where(i => i.Action == action);
            foreach (var item in items)
                UpdateVideoColor(item);
        }


        /// <summary>
        /// Appelé lorsque la vignette de l'action a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les données de l'évènement.</param>
        private void OnThumbnailHashChanged(object sender, PropertyChangedEventArgs<string> e)
        {
            var action = (KAction)sender;
            //UpdateThumbnail(action);
        }

        /// <summary>
        /// Crée un <see cref="System.Windows.Media.ImageSource"/> depuis la vignette sous forme de tableau d'octets.
        /// </summary>
        /// <param name="action">l'action.</param>
        /// <returns>La source de l'image.</returns>
        System.Windows.Media.ImageSource CreateThumbnailBitmap(KAction action)
        {
            if (action.Thumbnail != null)
            {
                /*var memStream = new System.IO.MemoryStream(action.Thumbnail.Data); // Garbage collecté au changement de l'image

                var bi = new System.Windows.Media.Imaging.BitmapImage();
                bi.BeginInit();
                bi.StreamSource = memStream;
                bi.EndInit();*/
                try
                {
                    var bi = new BitmapImage();
                    WebRequest request = WebRequest.Create(new Uri($"{Preferences.FileServerUri}/GetFile/{action.Thumbnail.Hash}{action.Thumbnail.Extension}", UriKind.Absolute));
                    request.Timeout = -1;
                    WebResponse response = request.GetResponse();
                    using (Stream responseStream = response.GetResponseStream())
                    using (BinaryReader reader = new BinaryReader(responseStream))
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        int bytesRead = 0;
                        byte[] bytebuffer = new byte[StreamExtensions.BufferSize];
                        do
                        {
                            bytesRead = reader.Read(bytebuffer, 0, bytebuffer.Length);
                            memoryStream.Write(bytebuffer, 0, bytesRead);
                        } while (bytesRead > 0);

                        bi.BeginInit();
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        bi.StreamSource = memoryStream;
                        bi.EndInit();

                        bi.Freeze();
                    }

                    return bi;
                }
                catch
                {
                }
            }
            return null;
        }

        /// <summary>
        /// Appelé lorsque la ressource de l'action a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.EventArgs&lt;KProcess.Ksmed.Models.Video&gt;"/> contenant les données de l'évènement.</param>
        private void OnActionResourceChanged(object sender, PropertyChangedEventArgs<Resource> e)
        {
            var action = (KAction)sender;
            UpdateReducedResource(action);
            this.UpdateResourcesLoad();
        }

        /// <summary>
        /// Appelé lorsque la notion réduite a changé sur la vidéo.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.EventArgs&lt;KProcess.Ksmed.Models.Video&gt;"/> contenant les données de l'évènement.</param>
        private void OnActionReducedChanged(object sender, PropertyChangedEventArgs<KActionReduced> e)
        {
            var action = (KAction)sender;
            if (action.Reduced != null)
            {
                UpdateReducedReductionFromTimings(action);
                RegisterReduced(action.Reduced);
            }
        }

        /// <summary>
        /// Appelé lorsque le pourcentage de réduction de l'action réduite a changé.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Les <see cref="KProcess.EventArgs&lt;System.Int32&gt;"/> contenant les données de l'évènement.</param>
        private void OnActionReducedReductionRatioChangedChanged(object sender, PropertyChangedEventArgs<double> e)
        {
            var action = ((KActionReduced)sender).Action;
            UpdateTimingsFromReducedReduction(action);
            UpdateReductionDescription(action);
        }

        /// <summary>
        /// Appelé lorsque le type de l'action réduite a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.EventArgs&lt;KProcess.Ksmed.Models.ActionType&gt;"/> contenant les données de l'évènement.</param>
        private void OnActionReducedActionTypeChanged(object sender, PropertyChangedEventArgs<ActionType> e)
        {
            var action = ((KActionReduced)sender).Action;

            var oldCode = e.OldValue != null ? e.OldValue.ActionTypeCode : null;
            ApplyReducedType(action, oldCode);
            UpdateReductionDescription(action);
        }

        /// <summary>
        /// Appelé lorsque l'état approuvé de l'action réduite a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.Ksmed.Models.PropertyChangedEventArgs&lt;System.Boolean&gt;"/> contenant les données de l'évènement.</param>
        private void OnActionReducedApprovedChanged(object sender, PropertyChangedEventArgs<bool> e)
        {
            var action = ((KActionReduced)sender).Action;
            ApplyReducedApproved(action, true);
            UpdateReductionDescription(action, true);
            OnReducedApprovedChanged(action.Reduced);
        }

        /// <summary>
        /// Appelé lorsque l'état resource de l'action réduite a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.Ksmed.Models.PropertyChangedEventArgs&lt;System.Boolean&gt;"/> contenant les données de l'évènement.</param>
        private void OnActionReducedResourceChanged(object sender, PropertyChangedEventArgs<bool> e)
        {
        }

        /// <summary>
        /// Appelé lorsque l'indentation de l'élément a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.EventArgs"/> contenant les données de l'évènement.</param>
        private void OnItemHierarchyChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="IsExpanded"/> a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.EventArgs"/> contenant les données de l'évènement.</param>
        private void OnIsExpandedChanged(object sender, EventArgs e)
        {
            var action = sender as TActionItem;
            if (action != null)
            {
                if (action.Action.ActionId != default(int))
                    this.NavigationService.Preferences.SetActionExpanded(action.Action.ActionId, action.IsExpanded);
            }
            else
            {
                var referentialItem = sender as TReferentialItem;
                if (referentialItem != null)
                {
                    int referentialId = referentialItem.Referential != null ? referentialItem.Referential.Id : 0;
                    this.NavigationService.Preferences.SetReferentialExpanded(referentialId, referentialItem.IsExpanded);
                }
            }
        }



        #endregion

        #region Gestion des actions liées

        /// <summary>
        /// Met à jour les timings pour les actions liées (qui commencent ou terminent au même moment que l'action spécifiée).
        /// </summary>
        /// <param name="sourceAction">L'action source.</param>
        /// <param name="movedStart"><c>true</c> si c'est le début qui a changé.</param>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
        private void UpdateLinkedAction(KAction sourceAction, bool movedStart, long oldValue, long newValue, KAction linkedAction)
        {
            if (this.AreMarkersLinked)
            {
                Action<KAction, long> setter;
                if (movedStart)
                {
                    setter = (a, v) => a.Finish = v;
                }
                else
                {
                    setter = (a, v) => a.Start = v;
                }

                if (linkedAction != null)
                {
                    _updatingLinkedAction = linkedAction;
                    setter(linkedAction, newValue);
                    _updatingLinkedAction = null;
                }

            }
        }

        #endregion

        #region Gestion de la réduction

        /// <summary>
        /// Met à jour la valeur resource reduced
        /// </summary>
        /// <param name="action">L'action mise à jour</param>
        private static void UpdateReducedResource(KAction action)
        {
            if (action.Reduced != null)
            {
                action.Reduced.Resource = action.Resource;
            }
        }

        /// <summary>
        /// Applique les changements du type de l'action réduite.
        /// </summary>
        /// <param name="action">L'action.</param>
        private void ApplyReducedType(KAction action, string oldCode)
        {
            ActionsTimingsMoveManagement.ApplyReducedType(action, oldCode);
            OnReducedTypeApplied(action);
        }

        /// <summary>
        /// Appelé lorsque un type de réduction (IES) a été appliqué sur une action.
        /// </summary>
        /// <param name="action">L'action.</param>
        protected virtual void OnReducedTypeApplied(KAction action)
        {
        }

        /// <summary>
        /// Applique le changement de l'état approuvé de l'action
        /// </summary>
        /// <param name="action">The action.</param>
        private void ApplyReducedApproved(KAction action, bool refreshReducedType)
        {
            ActionsTimingsMoveManagement.ApplyReducedApproved(action, refreshReducedType);
        }

        /// <summary>
        /// Met à jour les pourcentage de réduction à partir des timings pour toutes les actions spécifiées.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        private void UpdateAllReducedReductionFromTimings(IEnumerable<KAction> actions)
        {
            if (!EnableReducedPercentageRefresh)
                return;

            foreach (var action in actions)
                UpdateReducedReductionFromTimings(action);
        }

        /// <summary>
        /// Met à jour le pourcentage de réduction à partir des timings.
        /// </summary>
        /// <param name="action">L'action.</param>
        private void UpdateReducedReductionFromTimings(KAction action)
        {
            if (!EnableReducedPercentageRefresh)
                return;

            _isUpdatingPercentReduction = true;

            ActionsTimingsMoveManagement.UpdateReducedReductionFromTimings(action);

            _isUpdatingPercentReduction = false;
        }

        /// <summary>
        /// Met à jour les timings à partir du pourcentage de réduction.
        /// </summary>
        /// <param name="action">L'action.</param>
        private void UpdateTimingsFromReducedReduction(KAction action)
        {
            if (!EnableReducedPercentageRefresh)
                return;

            if (!_isUpdatingPercentReduction && _updateActionTimingSource != action)
            {
                ActionsTimingsMoveManagement.UpdateTimingsFromReducedReduction(action);
            }
        }

        /// <summary>
        /// Met à jour la description de la réduction d'une action.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="useApprovedFromAction"><c>true</c> pour utiliser l'état Approved de l'action et non de la solution.</param>
        public void UpdateReductionDescription(KAction action, bool useApprovedFromAction = false)
        {
            if (action.IsGroup || !action.IsReduced)
            {
                action.AmeliorationDescription = null;
            }
            else if (action.IsReduced)
            {
                var solution = ActionsTimingsMoveManagement.GetSolution(action);
                if (solution != null && !(solution.IsEmpty && action.Reduced.ReductionRatio == 0.0d && action.Reduced.ActionTypeCode == KnownActionCategoryTypes.I))
                {
                    var typeCode = action.Reduced.ActionTypeCode ?? KnownActionCategoryTypes.I;
                    string ies;

                    switch (typeCode)
                    {
                        case KnownActionCategoryTypes.I:
                            ies = LocalizationManager.GetString("ActionType_I_Short");
                            break;

                        case KnownActionCategoryTypes.E:
                            ies = LocalizationManager.GetString("ActionType_E_Short");
                            break;

                        case KnownActionCategoryTypes.S:
                            ies = LocalizationManager.GetString("ActionType_S_Short");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("typeCode");
                    }

                    string reduc = string.Format("{0:0.0} %", action.Reduced.ReductionRatio * 100);

                    bool isApproved = useApprovedFromAction ?
                        action.Reduced.Approved :
                        ActionsTimingsMoveManagement.GetIsSolutionApproved(action).GetValueOrDefault(true);

                    string OK = isApproved ?
                        LocalizationManager.GetString("View_AnalyzeBuild_ActionImproved_Approved") :
                        LocalizationManager.GetString("View_AnalyzeBuild_ActionImproved_NotApproved");

                    string description = action.Reduced.Solution;

                    action.AmeliorationDescription = $"{ies} - {reduc} - {OK} - {description}";
                }
                else
                    action.AmeliorationDescription = null;
            }
            else
                action.AmeliorationDescription = null;
        }

        #endregion

        #region Gestion des prédécesseurs et successeurs

        private readonly Regex _predecessorsRegex = new Regex(@"((?<p>([0-9]+\.?)+),? *)+?");
        private bool _ignoreOnItemPredecessorsStringChanged;

        /// <summary>
        /// Ajoute un prédécesseur à l'élément.
        /// </summary>
        /// <param name="item">L'élément.</param>
        /// <param name="predecessor">Le prédécesseur.</param>
        /// <returns><c>true</c> si l'ajout est possible.</returns>
        public bool AddPredecessor(TActionItem item, TActionItem predecessor)
        {
            return AddPredecessor(item.Action, predecessor.Action, true);
        }

        /// <summary>
        /// Supprime une prédécesseur de l'élément spécifié.
        /// </summary>
        /// <param name="item">L'élément.</param>
        /// <param name="predecessor">Le prédécesseur.</param>
        public void RemovePredecessor(TActionItem item, TActionItem predecessor)
        {
            RemovePredecessor(item.Action, predecessor.Action, true);
        }

        /// <summary>
        /// Ajoute un prédécesseur à l'action.
        /// </summary>
        /// <param name="action">L'action successeur.</param>
        /// <param name="predecessor">Le prédécesseur.</param>
        /// <param name="refreshPredecessorsStringAndtimings"><c>true</c> pour rafraichir la chaîne représentant les prédécesseurs.</param>
        /// <returns><c>true</c> si l'ajout est possible.</returns>
        private bool AddPredecessor(KAction action, KAction predecessor, bool refreshPredecessorsStringAndtimings)
        {
            var ret = ActionsTimingsMoveManagement.AddPredecessor(GetActionsSortedByWBS(), action, predecessor);
            if (ret)
            {
                if (refreshPredecessorsStringAndtimings)
                {
                    UpdatePredecessorsString(action);
                    this.FixPredecessorsSuccessorsTimings();
                }
                OnPredecessorAdded(action, predecessor);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Appelé lorsqu'un prédecesseur a été ajouté.
        /// </summary>
        /// <param name="action">L'action successeur.</param>
        /// <param name="predecessor">Le prédécesseur.</param>
        protected virtual void OnPredecessorAdded(KAction action, KAction predecessor)
        {

        }

        /// <summary>
        /// Supprime une prédécesseur de l'action spécifiée.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="predecessor">Le prédécesseur.</param>
        /// <param name="refreshPredecessorsStringAndTimings"><c>true</c> pour rafraichir la chaîne représentant les prédécesseurs.</param>
        private void RemovePredecessor(KAction action, KAction predecessor, bool refreshPredecessorsStringAndTimings)
        {
            ActionsTimingsMoveManagement.RemovePredecessor(action, predecessor);
            if (refreshPredecessorsStringAndTimings)
            {
                UpdatePredecessorsString(action);
                this.FixPredecessorsSuccessorsTimings();
            }
            OnPredecessorRemoved(action, predecessor);
        }

        /// <summary>
        /// Appelé lorsqu'un prédecesseur a été supprimé.
        /// </summary>
        /// <param name="action">L'action successeur.</param>
        /// <param name="predecessor">Le prédécesseur.</param>
        protected virtual void OnPredecessorRemoved(KAction action, KAction predecessor)
        {

        }

        /// <summary>
        /// Supprime tous les prédécesseurs et successeurs de l'action.
        /// </summary>
        /// <param name="action">L'action.</param>
        private void ClearPredecessorsSuccessors(KAction action)
        {
            var predecessors = action.Predecessors.ToArray();
            foreach (var predecessor in predecessors)
                this.RemovePredecessor(action, predecessor, false);

            UpdatePredecessorsString(action);

            var successors = action.Successors.ToArray();
            foreach (var successor in successors)
                this.RemovePredecessor(successor, action, true);
        }

        /// <summary>
        /// Appelé lorsque la chaîne représentant les prédécesseurs a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.EventArgs"/> contenant les données de l'évènement.</param>
        protected virtual void OnItemPredecessorsStringChanged(object sender, EventArgs e)
        {
            if (!this.EnablePredecessorTimingFix || _ignoreOnItemPredecessorsStringChanged)
                return;

            List<string> predecessorsWBS = new List<string>();
            List<TActionItem> predecessorsItems = new List<TActionItem>();

            // Parser le contenu
            var item = (TActionItem)sender;
            try
            {
                var matches = _predecessorsRegex.Matches(item.PredecessorsString);
                foreach (Match match in matches)
                {
                    if (match.Groups["p"].Success)
                        predecessorsWBS.Add(match.Groups["p"].Value);
                }
            }
            catch (Exception)
            {
            }

            // rechercher les actions correspondantes
            foreach (var wbs in predecessorsWBS)
            {
                var predecessorItem = this.ItemsOfTypeAction.FirstOrDefault(i => _wbsComparer.Compare(WBSHelper.GetParts(wbs), i.Action.WBSParts) == 0);
                if (predecessorItem != null)
                    predecessorsItems.Add(predecessorItem);
            }

            // Mettre à jour les collections de prédecesseurs sur les actions
            var toDelete = item.Action.Predecessors.Except(predecessorsItems.Select(i => i.Action)).ToArray();
            var toAdd = predecessorsItems.Select(i => i.Action).Except(item.Action.Predecessors).ToArray();

            bool hasChanged = false;

            foreach (var predecessor in toDelete)
            {
                hasChanged = true;
                RemovePredecessor(item.Action, predecessor, false);
            }
            foreach (var predecessor in toAdd)
                hasChanged |= AddPredecessor(item.Action, predecessor, refreshPredecessorsStringAndtimings: true); // User Story 4333. refreshPredecessorsStringAndtimings passé à true pour répondre au besoin. Voir si pas de bug suite à cette modification

            if (hasChanged)
                item.Action.MarkAsModified();

            _ignoreOnItemPredecessorsStringChanged = true;

            // Reformater correctement PredecessorsString
            // La liste des prédécesseurs a pu changer si certains ajouts sont impossible
            UpdatePredecessorsString(item.Action);

            _ignoreOnItemPredecessorsStringChanged = false;
        }

        /// <summary>
        /// Met à jour toutes les PredecessorsString
        /// </summary>
        private void UpdateAllPredecessorsString()
        {
            _ignoreOnItemPredecessorsStringChanged = true;

            var actions = GetActionsSortedByWBS();
            foreach (var action in actions)
            {
                var items = this.ItemsOfTypeAction.Where(a => a.Action == action);
                foreach (var item in items)
                    item.PredecessorsString = string.Join(", ", GetPredecessorsHandleManaged(action).Select(a => a.WBS));
            }

            _ignoreOnItemPredecessorsStringChanged = false;
        }

        /// <summary>
        /// Actualise la chaîne représentant les prédécesseurs à partir des prédécesseurs spécifiés.
        /// </summary>
        /// <param name="action">L'action.</param>
        private void UpdatePredecessorsString(KAction action)
        {
            var predecessorsItems = new List<TActionItem>();

            foreach (var pred in GetPredecessorsHandleManaged(action))
            {
                var predItem = this.ItemsOfTypeAction.FirstOrDefault(i => i.Action == pred);
                if (predItem != null)
                    predecessorsItems.Add(predItem);
            }

            var items = this.ItemsOfTypeAction.Where(a => a.Action == action);
            foreach (var item in items)
                item.PredecessorsString = string.Join(", ", predecessorsItems.Select(i => i.Action.WBS));
        }

        /// <summary>
        /// Corrige les temps en fonction des prédécesseurs et successeurs de chaque action.
        /// </summary>
        public virtual void FixPredecessorsSuccessorsTimings()
        {
            if (!EnablePredecessorTimingFix || !AllowTimingsChange || IsFixingPredecessorsSuccessorsTiming)
                return;

            IsFixingPredecessorsSuccessorsTiming = true;
            _isUpdatingGroupsTimings = true;
            ActionsTimingsMoveManagement.FixPredecessorsSuccessorsTimings(
                GetActionsSortedByWBS(),
                this.UseManagedPredecessorsSuccessors);
            _isUpdatingGroupsTimings = false;
            IsFixingPredecessorsSuccessorsTiming = false;

            this.UpdateBuildGroupsTiming();
            this.UpdateCriticalPath();
        }

        /// <summary>
        /// Obtient les prédécesseurs Managé ou non de l'action, en fonction du mode de fonctionnement actuel.
        /// </summary>
        /// <param name="action">L'action</param>
        /// <returns>Les prédécesseurs</returns>
        protected IEnumerable<KAction> GetPredecessorsHandleManaged(KAction action)
        {
            return this.UseManagedPredecessorsSuccessors ? action.PredecessorsManaged.AsEnumerable() : action.Predecessors.AsEnumerable();
        }

        public void AddNodes(List<WBSTreeVirtualizer.Node> nodes)
        {
            foreach (WBSTreeVirtualizer.Node node in nodes)
            {
                AddAction(node.Action, null);
                if (node.Children.Count > 0)
                    AddNodes(node.Children);
            }
        }
        #endregion

        #region Gestion des groupes

        /// <summary>
        /// Met à jour la propriété IsGroup de tous les éléments
        /// </summary>
        private void UpdateIsGroup()
        {
            foreach (var item in this.ItemsOfTypeReferential)
                item.IsGroup = true;

            var actions = GetActionsSortedByWBS();

            EnableGroupsTimingCoercion = false;
            foreach (var item in this.ItemsOfTypeAction)
            {
                var wasGroup = item.IsGroup;

                item.IsGroup = WBSHelper.HasChildren(item.Action, actions);
                item.Action.IsGroup = item.IsGroup.Value;
                item.Action.IsLinkToProcess = item.Action.LinkedProcessId != null;

                if (wasGroup.HasValue)
                {
                    if (!wasGroup.Value && item.IsGroup.Value)
                    {
                        // Si l'élément devient un groupe, sauvegarder ses infos de temps
                        SaveTiming(item.Action);
                        ClearPredecessorsSuccessors(item.Action);
                        item.Action.Video = null;
                        item.Action.Category = null;

                        UpdateVideoGroupsTiming();
                        UpdateBuildGroupsTiming();
                    }
                    else if (wasGroup.Value && !item.IsGroup.Value)
                    {
                        // Si l'élément devient une action classique, lui réappliquer ses valeurs d'origine
                        RestoreTiming(item.Action, actions);

                        UpdateVideoGroupsTiming();
                        UpdateBuildGroupsTiming();
                    }
                }
            }
            EnableGroupsTimingCoercion = true;
        }

        /// <summary>
        /// Sauvegarde les timings de l'action spécifiée.
        /// </summary>
        /// <param name="action">L'action.</param>
        private void SaveTiming(KAction action)
        {
            var timing = new ActionTimingSnapshot(action);
            _groupsSavedTimings[action] = timing;
        }

        /// <summary>
        /// Rétablit les timings sauvegardés de l'action spécifiée.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="allActions">Toutes les actions.</param>
        private void RestoreTiming(KAction action, IEnumerable<KAction> allActions)
        {
            if (_groupsSavedTimings.ContainsKey(action))
            {
                var timing = _groupsSavedTimings[action];

                action.Start = timing.Start;
                action.Finish = timing.Finish;
                action.BuildStart = timing.BuildStart;
                action.BuildFinish = timing.BuildFinish;

                if (_videoGetter != null && timing.VideoId.HasValue)
                    action.Video = _videoGetter(timing.VideoId.Value);

                if (timing.Predecessors != null)
                {
                    foreach (var predecessorId in timing.Predecessors)
                    {
                        var predecessor = allActions.FirstOrDefault(a => a.ActionId == predecessorId);
                        if (ActionsTimingsMoveManagement.CheckCanAddPredecessor(GetActionsSortedByWBS(), action, predecessor))
                            this.AddPredecessor(action, predecessor, true);
                    }
                }

                if (timing.Successors != null)
                {
                    foreach (var successorId in timing.Successors)
                    {
                        var successor = allActions.FirstOrDefault(a => a.ActionId == successorId);
                        if (ActionsTimingsMoveManagement.CheckCanAddPredecessor(GetActionsSortedByWBS(), successor, action))
                            this.AddPredecessor(successor, action, true);
                    }
                }
            }
        }

        /// <summary>
        /// Met à jour le début, la durée et la fin VIDEO sur les éléments de groupe
        /// </summary>
        protected virtual void UpdateVideoGroupsTiming()
        {
            if (!EnableGroupsTimingCoercion || !AllowTimingsChange || _isUpdatingGroupsTimings)
                return;

            _isUpdatingGroupsTimings = true;
            ActionsTimingsMoveManagement.UpdateVideoGroupsTiming(GetActionsSortedByWBS());
            _isUpdatingGroupsTimings = false;
        }

        /// <summary>
        /// Met à jour le début, la durée et la fin PROCESS sur les éléments de groupe
        /// </summary>
        protected virtual void UpdateBuildGroupsTiming()
        {
            if (!EnableGroupsTimingCoercion || !AllowTimingsChange || _isUpdatingGroupsTimings)
                return;

            _isUpdatingGroupsTimings = true;
            ActionsTimingsMoveManagement.UpdateBuildGroupsTiming(GetActionsSortedByWBS());
            _isUpdatingGroupsTimings = false;
        }

        #endregion

        #region Gestion de la charge et de la surcharge des ressources

        /// <summary>
        /// Met à jour la charge des ressources.
        /// </summary>
        protected void UpdateResourcesLoad()
        {
            //if (!EnableRessourceLoad)
            //    return;

            var actions = GetActionsSortedByWBS().MapApprovedReducedResource();

            if (actions.Any())
            {
                // Exclure les groupes
                var distinctResources = actions
                    .Where(a => !a.IsGroup && a.GetApprovedResource() != null)
                    .Select(a => a.GetApprovedResource())
                    .Distinct()
                    .ToArray();

                var totalTime = actions.Max(a => GetBuildFinish(a)) - actions.Min(a => GetBuildStart(a));
                if (totalTime <= 0)
                    return;

                foreach (var resource in distinctResources)
                {
                    ReferentialLoad load = ReferentialsLoad.FirstOrDefault(rl => rl.Resource == resource);

                    var add = false;
                    if (load == null)
                    {
                        load = new ReferentialLoad(resource);
                        add = true;
                    }

                    // Exclure les groupes
                    var actionsTarget = actions.Where(a => a.GetApprovedResource() == resource && !a.IsGroup);
                    var ranges = actionsTarget.Union(GetBuildStart, GetBuildFinish);

                    var loadRaw = ranges.Select(r => r.End - r.Start).Sum();
                    var overloadRaw = actionsTarget.Sum(a => GetBuildFinish(a) - GetBuildStart(a)) - loadRaw;

                    load.Load = (double)loadRaw * 100 / (double)totalTime;
                    load.Overload = (double)overloadRaw * 100 / (double)totalTime;

                    if (_timeService != null) // Vaut null en mode design
                    {
                        load.Description = string.Format(LocalizationManager.GetString("VMHelpers_ActionsManager_ResourceLoadDescription"),
                          load.Resource.Label,
                          _timeService.TicksToString(loadRaw), load.Load,
                          _timeService.TicksToString(overloadRaw), load.Overload);
                    }

                    if (add)
                        ReferentialsLoad.Add(load);
                }

                var toDelete = ReferentialsLoad.Select(rl => rl.Resource).Except(distinctResources).ToArray();
                foreach (var resource in toDelete)
                    ReferentialsLoad.Remove(ReferentialsLoad.FirstOrDefault(rl => rl.Resource == resource));
            }
            else
                ReferentialsLoad.Clear();
        }

        #endregion

        #region Vérifications et corrections automatiques

        [System.Diagnostics.Conditional("DEBUG")]
        /// <summary>
        /// Vérifie l'intégrité de tous les WBS
        /// </summary>
        private void DebugCheckAllWBS()
        {
            var actions = GetActionsSortedByWBS();

            if (!actions.Any())
                return;
            var maxIndentation = actions.Select(a => WBSHelper.IndentationFromWBS(a.WBS)).Max();

            var currentLevels = new int[maxIndentation];

            bool areValid = true;
            string previous = null;
            foreach (var action in actions)
            {
                if (previous != null && !WBSHelper.CanBeSuccessive(previous, action.WBS))
                {
                    areValid = false;
                    break;
                }

                previous = action.WBS;
            }

            if (!areValid)
            {
                FixAllWBS();
                System.Diagnostics.Debug.Fail("Les WBS sont invalides. Ca ne devrait pas arriver. Ils vont être corrigés mais des données pourraient être perdues");
            }
        }

        /// <summary>
        /// Corrige les WBS. A n'utiliser que s'ils sont incorrects.
        /// </summary>
        internal void FixAllWBS()
        {
            if (IsViewByReferential())
            {
                // Corriger via les actions

                var actions = GetActionsSortedByWBS();
                var currentLevels = new int[actions.Any() ? actions.Select(a => WBSHelper.IndentationFromWBS(a.WBS)).Max() + 1 : 0];
                foreach (var action in actions)
                {
                    var indentation = WBSHelper.IndentationFromWBS(action.WBS);
                    var actionWBS = new int[indentation + 1];

                    for (int i = 0; i <= indentation; i++)
                    {
                        if (i == indentation)
                            currentLevels[i] = currentLevels[i] + 1;

                        actionWBS[i] = currentLevels[i];
                    }

                    for (int i = indentation + 1; i < currentLevels.Length; i++)
                    {
                        currentLevels[i] = 0;
                    }

                    action.WBS = WBSHelper.LevelsToWBS(actionWBS);
                }
            }
            else if (View == GanttGridView.WBS)
            {
                // Corriger par les items et leurs indentations
                var maxIndentation = this.Items.Any() ? this.Items.Select(i => i.Indentation).Max() + 1 : 0;
                var currentLevels = new int[maxIndentation];

                foreach (var item in this.ItemsOfTypeAction)
                {
                    var itemWBS = new int[item.Indentation + 1];

                    for (int i = 0; i <= item.Indentation; i++)
                    {
                        if (i == item.Indentation)
                            currentLevels[i] = currentLevels[i] + 1;

                        itemWBS[i] = currentLevels[i];
                    }

                    for (int i = item.Indentation + 1; i < currentLevels.Length; i++)
                    {
                        currentLevels[i] = 0;
                    }

                    item.Action.WBS = WBSHelper.LevelsToWBS(itemWBS);
                }
            }
            _isActionsCollectionDirty = true;
        }

        /// <summary>
        /// Trace le WBS des actions chargées.
        /// </summary>
        [Conditional("DEBUG")]
        private void TraceAllWBS()
        {
            var sb = new StringBuilder();
            foreach (var action in GetActionsSortedByWBS())
            {
                var indentation = WBSHelper.IndentationFromWBS(action.WBS);
                for (int i = 0; i < indentation; i++)
                    sb.Append("  ");
                sb.Append(action.Label ?? string.Empty);
                sb.Append(" ");
                sb.Append(action.WBS);
                sb.AppendLine();
            }
            this.TraceDebug(sb.ToString());
        }

        /// <summary>
        /// Trace la collection des items chargés.
        /// </summary>
        private void TraceCollection()
        {
            var sb = new StringBuilder();
            foreach (var item in this.Items)
            {
                var indentation = item.Indentation;
                for (int i = 0; i < indentation; i++)
                    sb.Append("  ");
                sb.Append(item.Content);
                if (item is TActionItem)
                {
                    sb.Append(" ");
                    sb.Append(((TActionItem)item).Action.WBS);
                }
                sb.AppendLine();
            }
            this.TraceDebug(sb.ToString());
        }

        /// <summary>
        /// Corrige toutes les indentations qui sont incorrectes.
        /// </summary>
        private void FixAllIndentations()
        {
            if (IsViewByReferential())
                throw new NotSupportedException("Ne fonctionne qu'en vue par WBS");

            TComponentItem previousItem = null;
            foreach (var item in this.Items)
            {
                if (previousItem == null)
                {
                    if (item.Indentation != 0)
                        item.Indentation = 0;
                }
                else
                {
                    if (item.Indentation > previousItem.Indentation + 1)
                        item.Indentation = previousItem.Indentation + 1;
                }

                previousItem = item;
            }
        }

        #endregion

        #region Types imbriqués

        /// <summary>
        /// Une direction verticale.
        /// </summary>
        private enum VerticalDirection
        {
            /// <summary>
            /// Vers le haut.
            /// </summary>
            Up,

            /// <summary>
            /// Vers le bas
            /// </summary>
            Down
        }

        /// <summary>
        /// Contient les informations de temps d'une action.
        /// </summary>
        private struct ActionTimingSnapshot
        {
            public ActionTimingSnapshot(KAction action)
                : this()
            {
                Start = action.Start;
                Finish = action.Finish;
                BuildStart = action.BuildStart;
                BuildFinish = action.BuildFinish;
                VideoId = action.Video != null ? action.Video.VideoId : (int?)null;
                Predecessors = action.Predecessors.Where(p => p.ActionId != default(int)).Select(p => p.ActionId).ToArray();
                Successors = action.Successors.Where(p => p.ActionId != default(int)).Select(p => p.ActionId).ToArray();
            }

            public long Start { get; private set; }
            public long Finish { get; private set; }
            public long BuildStart { get; private set; }
            public long BuildFinish { get; private set; }

            public int? VideoId { get; private set; }

            public int[] Predecessors { get; private set; }
            public int[] Successors { get; private set; }
        }


        #endregion
    }
}
