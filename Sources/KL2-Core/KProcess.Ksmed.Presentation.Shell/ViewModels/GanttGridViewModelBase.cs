using DlhSoft.Windows.Controls;
using KProcess.Globalization;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente une classe de base pour les écrans qui contiennent une grille affichant des actions.
    /// </summary>
    public abstract class GanttGridViewModelBase<TViewModel, TIViewModel, TComponentItem, TActionItem, TResourceItem> : FrameContentExtensibleViewModelBase<TViewModel, TIViewModel>, IGanttGridViewModel<TComponentItem, TActionItem, TResourceItem>
        where TViewModel : GanttGridViewModelBase<TViewModel, TIViewModel, TComponentItem, TActionItem, TResourceItem>, TIViewModel
        where TIViewModel : IFrameContentViewModel
        where TComponentItem : DataTreeGridItem
        where TActionItem : TComponentItem, IActionItem
        where TResourceItem : TComponentItem, IReferentialItem
    {

        #region Champs privés

        private TActionItem _currentActionItem;
        private TComponentItem _currentGridItem;
        private bool _blockSelectionReentrancy = false;
        private BulkObservableCollection<TComponentItem> _selecteditems;
        private CustomFieldsLabels _customFieldsLabels;
        private bool _canChange = true;
        private Visibility _gridWaitVisibility = Visibility.Collapsed;
        private bool _isReadOnly;
        private GanttGridViewContainer[] _views;
        private GanttGridViewContainer _viewContainer;

        #endregion

        #region Surcharges

        /// <summary>
        /// Initialise le VM en vue du chargement
        /// </summary>
        protected override void Initialize()
        {
            this.SelectedItems = new BulkObservableCollection<TComponentItem>(true);

            _views = CreateViewContainersAndInitializeReferentialCollections().ToArray();

            this._viewContainer = _views[0];
        }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override Task OnInitializeDesigner()
        {
            Categories = DesignData.GenerateActionCategories().Categories.ToBulkObservableCollection();
            Skills = DesignData.GenerateSkills().ToBulkObservableCollection();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override async Task OnLoading()
        {
            await base.OnLoading();

            NavigationService = ServiceBus.Get<INavigationService>();

            if (NavigationService.Preferences.GanttGridView.HasValue)
                ChangeView(NavigationService.Preferences.GanttGridView.Value);
            else
                ChangeView(GanttGridView.WBS);
        }

        /// <summary>
        /// Méthode appelée dés que le chargement est terminé
        /// </summary>
        protected override void OnLoaded()
        {
            base.OnLoaded();
            base.IsScenarioPickerEnabled = true;

            base.EventBus.Subscribe<UIUpdatedNotSynchronizedEvent>(OnUIUpdatedNotSynchronized);
        }

        /// <summary>
        /// Appelé afin de nettoyer les ressources.
        /// </summary>
        protected override void OnCleanup()
        {
            base.OnCleanup();
            if (this.ActionsManager != null)
                this.ActionsManager.Clear();
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient une valeur indiquant si le sélectionneur de scénario est à afficher.
        /// </summary>
        public override bool ShowScenarioPicker
        {
            get { return true; }
        }

        /// <summary>
        /// Obtient ou définit le gestionnaire d'actions.
        /// </summary>
        protected ActionsManager<TComponentItem, TActionItem, TResourceItem> ActionsManager { get; set; }

        /// <summary>
        /// Obtient le service de navigation.
        /// </summary>
        protected INavigationService NavigationService { get; private set; }

        /// <summary>
        /// Obtient ou définit l'élément courant de la grille.
        /// </summary>
        public TComponentItem CurrentGridItem
        {
            get { return _currentGridItem; }
            set
            {
                if (_currentGridItem != value)
                {
                    var previous = _currentGridItem;
                    _currentGridItem = value;
                    OnCurrentGridItemChanged(previous, _currentGridItem);
                    OnPropertyChanged("CurrentGridItem");
                }
            }
        }

        /// <summary>
        /// Obtient ou définit l'action courante
        /// </summary>
        public TActionItem CurrentActionItem
        {
            get { return _currentActionItem; }
            protected set
            {
                if (_currentActionItem != value && !_blockSelectionReentrancy)
                {
                    var previous = _currentActionItem;
                    _currentActionItem = value;
                    OnCurrentActionChanged(previous, _currentActionItem);
                    OnPropertyChanged("CurrentActionItem");
                }
            }
        }

        /// <summary>
        /// Obtient la liste des éléments sélectionnés
        /// </summary>
        public BulkObservableCollection<TComponentItem> SelectedItems
        {
            get { return _selecteditems; }
            private set
            {
                if (_selecteditems != value)
                {
                    _selecteditems = value;
                    OnPropertyChanged("SelectedItems");
                }
            }
        }

        /// <summary>
        /// Obtient les éléments représentant des actions, utilisés par le Gantt.
        /// </summary>
        public ObservableCollection<TComponentItem> ActionItems { get; protected set; }

        /// <summary>
        /// Obtient les éléments représentant des actions correctement typés.
        /// </summary>
        internal protected IEnumerable<TActionItem> ActionGridItems
        {
            get { return ActionItems.OfType<TActionItem>(); }
        }

        /// <summary>
        /// Obtient ou définit le scénario courant.
        /// </summary>
        protected Scenario CurrentScenarioInternal { get; set; }

        /// <summary>
        /// Obtient la vue actuelle.
        /// </summary>
        public GanttGridView View
        {
            get { return ActionsManager.View; }
        }

        /// <summary>
        /// Obtient ou définit la vue sélectionnée.
        /// Version interne avec accesseurs simples.
        /// </summary>
        protected GanttGridViewContainer ViewContainerInternal
        {
            get { return _viewContainer; }
            set { _viewContainer = value; }
        }

        /// <summary>
        /// Obtient ou définit la vue sélectionnée.
        /// </summary>
        public virtual GanttGridViewContainer ViewContainer
        {
            get { return _viewContainer; }
            set
            {
                if (_viewContainer != value)
                {
                    _viewContainer = value;
                    OnPropertyChanged("ViewContainer");
                    this.ChangeView((GanttGridView)_viewContainer.View);
                }
            }
        }

        /// <summary>
        /// Obtient les catégories.
        /// </summary>
        public BulkObservableCollection<ActionCategory> Categories { get; private set; }

        /// <summary>
        /// Obtient les compétences.
        /// </summary>
        public BulkObservableCollection<Skill> Skills { get; private set; }

        /// <summary>
        /// Obtient les référentiels 1.
        /// </summary>
        public BulkObservableCollection<Ref1> Ref1s { get; private set; }

        /// <summary>
        /// Obtient les référentiels 2.
        /// </summary>
        public BulkObservableCollection<Ref2> Ref2s { get; private set; }

        /// <summary>
        /// Obtient les référentiels 3.
        /// </summary>
        public BulkObservableCollection<Ref3> Ref3s { get; private set; }

        /// <summary>
        /// Obtient les référentiels 4.
        /// </summary>
        public BulkObservableCollection<Ref4> Ref4s { get; private set; }

        /// <summary>
        /// Obtient les référentiels 5.
        /// </summary>
        public BulkObservableCollection<Ref5> Ref5s { get; private set; }

        /// <summary>
        /// Obtient les référentiels 6.
        /// </summary>
        public BulkObservableCollection<Ref6> Ref6s { get; private set; }

        /// <summary>
        /// Obtient les référentiels 7.
        /// </summary>
        public BulkObservableCollection<Ref7> Ref7s { get; private set; }

        /// <summary>
        /// Obtient les libellés des champs libres texte.
        /// </summary>
        public CustomFieldsLabels CustomFieldsLabels
        {
            get { return _customFieldsLabels; }
            protected set
            {
                if (_customFieldsLabels != value)
                {
                    _customFieldsLabels = value;
                    OnPropertyChanged("CustomFieldsLabels");
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si la sélection peut être changée.
        /// </summary>
        public virtual bool CanChange
        {
            get { return _canChange; }
            protected set
            {
                if (_canChange != value)
                {
                    _canChange = value;
                    OnPropertyChanged("CanChange");
                    OnCanChangeChanged();
                }
            }
        }

        /// <summary>
        /// Obtient la visibilité de l'indicateur d'attente de la grille.
        /// </summary>
        public Visibility GridWaitVisibility
        {
            get { return _gridWaitVisibility; }
            private set
            {
                if (_gridWaitVisibility != value)
                {
                    _gridWaitVisibility = value;
                    OnPropertyChanged("GridWaitVisibility");
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le scénario courant est en lecture seule.
        /// </summary>
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            protected set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                    OnPropertyChanged(nameof(IsReadOnly));
                    OnPropertyChanged(nameof(IsNotReadOnly));
                    OnIsReadOnlyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le scénario courant n'est pas en lecture seule.
        /// </summary>
        public bool IsNotReadOnly
        {
            get { return !IsReadOnly; }
        }

        /// <summary>
        /// Obtient les vues disponibles.
        /// </summary>
        public virtual GanttGridViewContainer[] Views
        {
            get { return _views; }
        }

        #endregion

        #region Méthodes protégées

        /// <summary>
        /// Crée les instances de <see cref="GanttGridViewContainer"/> et initialise les collections de référentiels.
        /// </summary>
        /// <returns>Les conteneurs créés.</returns>
        protected virtual IList<GanttGridViewContainer> CreateViewContainersAndInitializeReferentialCollections()
        {
            if (DesignMode.IsInDesignMode)
                return new List<GanttGridViewContainer> { new GanttGridViewContainer((int)GanttGridView.WBS, "WBS") };

            var referentialsUseService = IoC.Resolve<IReferentialsUseService>();

            var views = new List<GanttGridViewContainer>()
            {
                new GanttGridViewContainer((int)GanttGridView.WBS, LocalizationManagerExt.GetSafeDesignerString("View_AnalyzeGridCommon_ViewByWBS")),
            };

            // Toujours actif
            views.Add(new GanttGridViewContainer((int)GanttGridView.Resource, LocalizationManagerExt.GetSafeDesignerString("View_AnalyzeGridCommon_ViewByResource")));

            // Toujours actif
            this.Categories = new BulkObservableCollection<ActionCategory>(true);
            views.Add(new GanttGridViewContainer((int)GanttGridView.Category, ReferentialsUse.Category));
            this.Skills = new BulkObservableCollection<Skill>(true);
            views.Add(new GanttGridViewContainer((int)GanttGridView.Skill, ReferentialsUse.Skill));

            if (referentialsUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref1))
            {
                this.Ref1s = new BulkObservableCollection<Ref1>(true);
                views.Add(new GanttGridViewContainer((int)GanttGridView.Ref1, ReferentialsUse.Ref1));
                AddReferentialSortDescription(this.Ref1s);
            }

            if (referentialsUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref2))
            {
                this.Ref2s = new BulkObservableCollection<Ref2>(true);
                views.Add(new GanttGridViewContainer((int)GanttGridView.Ref2, ReferentialsUse.Ref2));
                AddReferentialSortDescription(this.Ref2s);
            }

            if (referentialsUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref3))
            {
                this.Ref3s = new BulkObservableCollection<Ref3>(true);
                views.Add(new GanttGridViewContainer((int)GanttGridView.Ref3, ReferentialsUse.Ref3));
                AddReferentialSortDescription(this.Ref3s);
            }

            if (referentialsUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref4))
            {
                this.Ref4s = new BulkObservableCollection<Ref4>(true);
                views.Add(new GanttGridViewContainer((int)GanttGridView.Ref4, ReferentialsUse.Ref4));
                AddReferentialSortDescription(this.Ref4s);
            }

            if (referentialsUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref5))
            {
                this.Ref5s = new BulkObservableCollection<Ref5>(true);
                views.Add(new GanttGridViewContainer((int)GanttGridView.Ref5, ReferentialsUse.Ref5));
                AddReferentialSortDescription(this.Ref5s);
            }

            if (referentialsUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref6))
            {
                this.Ref6s = new BulkObservableCollection<Ref6>(true);
                views.Add(new GanttGridViewContainer((int)GanttGridView.Ref6, ReferentialsUse.Ref6));
                AddReferentialSortDescription(this.Ref6s);
            }

            if (referentialsUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref7))
            {
                this.Ref7s = new BulkObservableCollection<Ref7>(true);
                views.Add(new GanttGridViewContainer((int)GanttGridView.Ref7, ReferentialsUse.Ref7));
                AddReferentialSortDescription(this.Ref7s);
            }

            return views;
        }

        /// <summary>
        /// Change la vue.
        /// </summary>
        /// <param name="view">La vue.</param>
        protected virtual void ChangeView(GanttGridView view)
        {
            if (DesignMode.IsInDesignMode)
            {
                ActionsManager.ChangeView(view, this.CurrentActionItem);
                return;
            }

            GridWaitVisibility = Visibility.Visible;

            Dispatcher.CurrentDispatcher.Invoke((Action)(() =>
            {
                ActionsManager.ChangeView(view, this.CurrentActionItem);
                OnPropertyChanged("View");
                if (base.EventBus != null)
                    base.EventBus.Publish(new GridViewChangedEvent(this, view));
            }));

            GridWaitVisibility = Visibility.Collapsed;

            ViewContainer = this.Views.First(v => v.View == (int)view);
            NavigationService.Preferences.GanttGridView = view;
        }

        /// <summary>
        /// Appelé lorsque l'action courante a changé
        /// </summary>
        /// <param name="previousValue">La valeur précédante.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
        protected virtual void OnCurrentActionChanged(TActionItem previousValue, TActionItem newValue)
        {
            if (base.ViewModelState == ViewModelStateEnum.ShuttingDown)
                return;

            if (NavigationService != null)
                NavigationService.Preferences.ActionId =
                    newValue != null && newValue.Action.ActionId != default(int) ?
                    newValue.Action.ActionId :
                    (int?)null;

            // Rafraichir IsSelected en respectant un ordre particulier
            Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() =>
            {
                _blockSelectionReentrancy = true;
                if (newValue != null)
                {
                    newValue.IsSelected = true;
                    foreach (var item in this.ActionGridItems)
                    {
                        if (item != newValue)
                            item.IsSelected = false;
                    }
                }
                else
                {
                    foreach (var item in this.ActionGridItems)
                        item.IsSelected = false;
                }
                _blockSelectionReentrancy = false;
            }), DispatcherPriority.Background);
        }

        /// <summary>
        /// Appelé lorsque l'élément courant de la grille a changé.
        /// </summary>
        /// <param name="previousValue">La valeur précédante.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
        protected virtual void OnCurrentGridItemChanged(TComponentItem previousValue, TComponentItem newValue)
        {
            this.CurrentActionItem = newValue as TActionItem;
        }

        /// <summary>
        /// Appelé lorsque la hiérarchie d'une ou plusieurs actions a changé.
        /// </summary>
        protected virtual void OnActionHierarchyChanged()
        {

        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="CanChange"/> a changé.
        /// </summary>
        protected virtual void OnCanChangeChanged()
        {
        }

        /// <summary>
        /// Appelé lorsque l'état de l'entité courante (spécifiée via RegisterToStateChanged) a changé.
        /// </summary>
        /// <param name="newState">le nouvel état.</param>
        protected override void OnEntityStateChanged(ObjectState newState)
        {
            CanChange &= newState == ObjectState.Unchanged;
            if (CurrentActionItem is ActionGanttItem actionGanttItem)
            {
                BrushesHelper.GetBrush(actionGanttItem.Action.Category?.Color, true, out Brush fillBrush, out Brush strokeBrush);
                actionGanttItem.FillBrush = fillBrush;
                actionGanttItem.StrokeBrush = strokeBrush;
                actionGanttItem.OrangeHeaderVisibility = ActionsTimingsMoveManagement.IsActionExternal(actionGanttItem.Action) ? Visibility.Visible : Visibility.Collapsed;
                ActionsManager.UpdateReductionDescription(actionGanttItem.Action, true);
            }
        }

        /// <summary>
        /// Reconstruit les éléments.
        /// </summary>
        /// <param name="previousSelection">La sélection précédente.</param>
        protected void RebuildItems(TActionItem previousSelection = null)
        {
            GridWaitVisibility = Visibility.Visible;

            Dispatcher.CurrentDispatcher.Invoke((Action)(() =>
            {
                this.ActionsManager.RebuildItems(previousSelection);
            }));

            GridWaitVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Restaure la sélection de l'action à partir des préférences de navigation.
        /// </summary>
        protected void RestoreActionSelection()
        {
            if (this.NavigationService != null && this.NavigationService.Preferences.ActionId.HasValue)
            {
                var firstMatch = this.ActionGridItems.FirstOrDefault(a => a.Action.ActionId == this.NavigationService.Preferences.ActionId);
                if (firstMatch != null)
                    this.CurrentGridItem = firstMatch;
            }
        }

        /// <summary>
        /// Enregistre les actions modifiées.
        /// </summary>
        /// <param name="refreshSelectionWhenDone"><c>true</c> pour rafraichir la sélection une fois la sauvegarde effectuée.</param>
        protected abstract Task<bool> SaveActions(bool refreshSelectionWhenDone);

        /// <summary>
        /// Affiche la liste des scénarios impactés à l'utilisateur.
        /// </summary>
        /// <param name="currentScenario">The current scenario.</param>
        /// <param name="allScenarios">All scenarios.</param>
        /// <param name="delete"><c>true</c> si l'action est une suppression.</param>
        /// <param name="actionsToDelete">Les actions à supprimer.</param>
        /// <param name="whenNoScenarioImpacted">Action à exécuter lorsqu'aucun scénario n'est impacté.</param>
        /// <param name="actionsWithUpdatedWBS">The actions with updated WBS.</param>
        protected async Task<bool> ShowImpactedScenarios(
            Scenario currentScenario, Scenario[] allScenarios,
            bool delete, KAction[] actionsToDelete,
            Action whenNoScenarioImpacted,
            KAction[] actionsWithUpdatedWBS = null)
        {
            Scenario[] scenarios = await ServiceBus.Get<IAnalyzeService>().PredictImpactedScenarios(currentScenario, allScenarios, actionsToDelete, actionsWithUpdatedWBS);

            bool ok = true;

            // Informer des scénarios impactés
            if (scenarios != null && scenarios.Any())
            {
                string messageKey = delete ? "VM_AnalyzeCommon_DeleteImpactedScenarios" : "VM_AnalyzeCommon_ImpactedScenarios";

                string scenariosJoint = string.Join(Environment.NewLine, scenarios.Select(s => s.Label));
                string message = string.Format(
                    LocalizationManager.GetString(messageKey), scenariosJoint);

                MessageDialogResult res = DialogFactory.GetDialogView<IMessageDialog>()
                    .Show(message, null, MessageDialogButton.YesNoCancel, MessageDialogImage.Warning);

                ok = res == MessageDialogResult.Yes;
            }
            else
                whenNoScenarioImpacted?.Invoke();

            return ok;
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="IsReadOnly"/> a changé.
        /// </summary>
        protected virtual void OnIsReadOnlyChanged()
        {
        }

        /// <summary>
        /// Détermine si la sélection peut être changée et s'il n'y pas de modifications en attente de sauvegarde.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la sélection peut être changée et s'il n'y pas de modifications en attente de sauvegarde; sinon, <c>false</c>.
        /// </returns>
        protected virtual bool CanChangeAndHasNoPendingChanges()
        {
            if (this.IsReadOnly)
                return true;

            var changedActions = this.ActionGridItems
                .Select(i => i.Action)
                .Where(a => a.IsNotMarkedAsUnchanged)
                .ToArray();

            if (changedActions.Any() || this.CurrentScenarioInternal.IsNotMarkedAsUnchanged)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Ajoute un tri sur le libellé à la collection spécifiée.
        /// </summary>
        /// <typeparam name="T">Le type des données</typeparam>
        /// <param name="collection">La collection.</param>
        protected void AddLabelSortDescription<T>(IList<T> collection)
        {
            CollectionViewSource.GetDefaultView(collection).SortDescriptions.Add(new System.ComponentModel.SortDescription("Label", System.ComponentModel.ListSortDirection.Ascending));
        }

        /// <summary>
        /// Ajoute un tri sur les référentiels à la collection spécifiée.
        /// </summary>
        /// <typeparam name="T">Le type des données</typeparam>
        /// <param name="collection">La collection.</param>
        protected void AddReferentialSortDescription<T>(IList<T> collection)
        {
            var collectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(collection);
            collectionView.CustomSort = new ReferentialsSort();
        }

        #endregion

        #region Méthodes privées

        private void OnUIUpdatedNotSynchronized(UIUpdatedNotSynchronizedEvent obj)
        {
            if (this.CanChange)
            {
                this.CanChange = false; // On force l'affichage des bouton cancel save
            }
        }

        #endregion

        #region Commandes

        private Command _moveUpCommand;
        /// <summary>
        /// Obtient la commande permettant de déplacer l'élément sélectionné vers le haut.
        /// </summary>
        public ICommand MoveUpCommand
        {
            get
            {
                if (_moveUpCommand == null)
                    _moveUpCommand = new Command(OnMoveUp, () => this.CurrentActionItem != null && OnCanMoveUp());
                return _moveUpCommand;
            }
        }

        /// <summary>
        /// Appelé afin de déterminer la commande MoveUp peut être exécutée.
        /// </summary>
        /// <returns><c>true</c> si la commande peut être executée.</returns>
        protected virtual bool OnCanMoveUp()
        {
            return true;
        }

        /// <summary>
        /// Appelé lorsque un déplacement vers le haut est demandé.
        /// </summary>
        protected abstract void OnMoveUp();

        private Command _moveDownCommand;
        /// <summary>
        /// Obtient la commande permettant de déplacer l'élément sélectionné vers le bas.
        /// </summary>
        public ICommand MoveDownCommand
        {
            get
            {
                if (_moveDownCommand == null)
                    _moveDownCommand = new Command(OnMoveDown, () => this.CurrentActionItem != null && OnCanMoveDown());
                return _moveDownCommand;
            }
        }

        /// <summary>
        /// Appelé afin de déterminer la commande MoveDown peut être exécutée.
        /// </summary>
        /// <returns><c>true</c> si la commande peut être executée.</returns>
        protected virtual bool OnCanMoveDown()
        {
            return true;
        }

        /// <summary>
        /// Appelé lorsque un déplacement vers le bas est demandé.
        /// </summary>
        protected abstract void OnMoveDown();

        #endregion

    }
}