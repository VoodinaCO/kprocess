using DlhSoft.Windows.Controls;
using KProcess.Globalization;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using KProcess.Presentation.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran de construction du scénario initial.
    /// </summary>
    abstract class BuildViewModelBase<TViewModel, TIViewModel> : GanttGridViewModelBase<TViewModel, TIViewModel, GanttChartItem, ActionGanttItem, ReferentialGanttItem>, IBuildViewModel
        where TViewModel : GanttGridViewModelBase<TViewModel, TIViewModel, GanttChartItem, ActionGanttItem, ReferentialGanttItem>, TIViewModel
        where TIViewModel : IBuildViewModel
    {

        #region Champs privés

        BulkObservableCollection<Scenario> _scenarios;
        ActionType[] _allActionTypes;
        long _currentTime;
        ActionPath[] _actionsPath;
        ActionPath[] _criticalPath;
        PlayingSource _playingSource = PlayingSource.None;
        long _totalDuration;
        ScenarioState[] _scenarioStates;
        ActionType[] _actionTypes;
        ActionPath _currentActionPath;
        bool _isChangingCriticalPathAction;
        bool _ignoreNextTimelineSyncFromCurrentAction;
        bool _allowTimingsDurationChange;
        Visibility _solutionsVisibility;
        Restitution.SolutionWrapper[] _solutionsWrappers;
        GanttGridViewContainer _solutionsView;
        IESFilter _selectedIESFilter;
        IESFilter[] _iESFilters;
        Visibility _IESFilterVisibility;
        bool _ignoreSelectedIESFilterChanges;
        bool _isMarkersLinkedModeEnabled;

        string _loadChartFilterAll;
        string _loadChartFilterEquipment;
        string _loadChartFilterOperator;

        /// <summary>
        /// Si définit, une navigation se produit à l'issue d'une sauvegarde
        /// </summary>
        IFrameNavigationToken _navigationToken;

        #endregion

        #region Surcharges

        /// <summary>
        /// Initialise le VM en vue du chargement
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            ActionItems = new BulkObservableCollection<GanttChartItem>();

            ActionsManager = new GanttActionsManager((BulkObservableCollection<GanttChartItem>)ActionItems,
                i => CurrentGridItem = i, null)
            {
                EnableRessourceLoad = true,
            };
            ActionsManager.CriticalPathChanged += OnActionsManagerCriticalPathChanged;
            ActionsManager.ReducedApprovedChanged += OnReducedApprovedChanged;
            _scenarios = new BulkObservableCollection<Scenario>(true);
            Operators = new BulkObservableCollection<Operator>(true);
            Equipments = new BulkObservableCollection<Equipment>(true);
            Solutions = new BulkObservableCollection<string>();

            ReducedActionCosts = KnownReducedActionValues.Costs;
            ReducedActionDifficulties = KnownReducedActionValues.Difficulties;

            _loadChartFilterAll = LocalizationManager.GetString("View_AnalyzeBuild_All");
            _loadChartFilterEquipment = LocalizationManager.GetString("View_AnalyzeAcquire_Equipment");
            _loadChartFilterOperator = LocalizationManager.GetString("View_AnalyzeAcquire_Operator");

            LoadChartFilters = new string[]
            {
                _loadChartFilterAll,
                _loadChartFilterEquipment,
                _loadChartFilterOperator,
            };
            SelectedLoadChartFilter = _loadChartFilterAll;
        }

        /// <inheritdoc />
        protected override IList<GanttGridViewContainer> CreateViewContainersAndInitializeReferentialCollections()
        {
            IList<GanttGridViewContainer> baseCollection = base.CreateViewContainersAndInitializeReferentialCollections();

            _solutionsView = new GanttGridViewContainer((int)AnalyzeBuildViewModelView.Solutions, LocalizationManager.GetString("View_AnalyzeBuild_ViewSolutions"));

            baseCollection.Add(_solutionsView);

            UpdateSolutionsVisibility();

            return baseCollection;
        }

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override async Task OnLoading()
        {
            await base.OnLoading();

            EventBus.Subscribe<ScenarioChangedEvent>(e =>
            {
                if (e.Scenario != null)
                    TryLoadScenario(e.Scenario.Id);
            });

            IProjectManagerService projectManager = ServiceBus.Get<IProjectManagerService>();
            ProjectInfo currentProject = projectManager.CurrentProject;
            await LoadData(currentProject.ProjectId);

            IsMarkersLinkedModeEnabled = !projectManager.IsUnlinkMarkerEnabledAndLocked;
        }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override async Task OnInitializeDesigner()
        {
            await base.OnInitializeDesigner();
            ChangeView(GanttGridView.WBS);

            CurrentScenario = DesignData.GenerateScenarioWithActions(
                DesignData.GenerateResources(), DesignData.GenerateActionCategories().Categories,
                DesignData.GenerateSkills(),
                DesignData.GenerateVideos());

            ActionsManager.RegisterInitialActions(CurrentScenario.Actions);

            CurrentGridItem = (ActionGanttItem)ActionItems.First();

            IESFilters = IESFilter.CreateDefault();
            SelectedIESFilter = IESFilters.First();

            CustomFieldsLabels = new CustomFieldsLabels
            {
                Text1 = new CustomFieldLabel(false, 1, "CustomTextLabel1"),
                Text2 = new CustomFieldLabel(false, 2, "CustomTextLabel2"),
                Text3 = new CustomFieldLabel(false, 3, "CustomTextLabel3"),
                Text4 = new CustomFieldLabel(false, 4, "CustomTextLabel4"),
                Numeric1 = new CustomFieldLabel(false, 1, "CustomNumericLabel1"),
                Numeric2 = new CustomFieldLabel(false, 2, "CustomNumericLabel2"),
                Numeric3 = new CustomFieldLabel(false, 3, "CustomNumericLabel3"),
                Numeric4 = new CustomFieldLabel(false, 4, "CustomNumericLabel4"),
            };
        }

        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la navigation est acceptée.
        /// </returns>
        public override Task<bool> OnNavigatingAway(IFrameNavigationToken token)
        {
            if (!CanChangeAndHasNoPendingChanges())
            {
                MessageDialogResult answer = this.AskNavigatingAwayValidationConfirmation(() => _navigationToken = token);
                return Task.FromResult(answer == MessageDialogResult.No);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Appelé pour libérer les ressources.
        /// </summary>
        protected override void OnCleanup()
        {
            ActionsManager.ReducedApprovedChanged -= OnReducedApprovedChanged;
            ActionsManager.CriticalPathChanged -= OnActionsManagerCriticalPathChanged;
            ClearSolutions();
            base.OnCleanup();
        }

        #endregion

        #region Propriétés

        public DateTime Today { get; private set; } = DateTime.Now.Date;

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les solutions sont à gérer.
        /// </summary>
        protected abstract bool EnableSolutions { get; }

        /// <summary>
        /// Obtient une valeur indiquant si seuls les champs de sélection I ou E sont accessibles.
        /// </summary>
        public virtual bool ShowPastInternalisationOrExternalisation { get { return false; } }

        /// <summary>
        /// Obtient le filtre des natures de scénarios à activer.
        /// </summary>
        public override string[] ScenarioNaturesFilter =>
            new string[] { KnownScenarioNatures.Initial, KnownScenarioNatures.Target };

        /// <summary>
        /// Obtient le gestionnaire d'actions pour Gantt.
        /// </summary>
        GanttActionsManager GanttActionsManager =>
            (GanttActionsManager)ActionsManager;

        /// <summary>
        /// Obtient ou définit le scénario actuel.
        /// </summary>
        public Scenario CurrentScenario
        {
            get { return CurrentScenarioInternal; }
            private set
            {
                if (CurrentScenarioInternal != value)
                {
                    Scenario old = CurrentScenarioInternal;
                    CurrentScenarioInternal = value;
                    OnPropertyChanged();
                    LoadScenario(old, value);
                }
            }
        }

        /// <summary>
        /// Obtient les opérateurs.
        /// </summary>
        public BulkObservableCollection<Operator> Operators { get; private set; }

        /// <summary>
        /// Obtient les équipements.
        /// </summary>
        public BulkObservableCollection<Equipment> Equipments { get; private set; }

        /// <summary>
        /// Obtient les états de scénario possibles.
        /// </summary>
        public ScenarioState[] ScenarioStates
        {
            get { return _scenarioStates; }
            private set
            {
                if (_scenarioStates != value)
                {
                    _scenarioStates = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Détermine si le mode lier/délier les marquers est disponible
        /// </summary>
        public bool IsMarkersLinkedModeEnabled
        {
            get { return _isMarkersLinkedModeEnabled; }
            private set
            {
                if (_isMarkersLinkedModeEnabled != value)
                {
                    _isMarkersLinkedModeEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        ActionValue[] _actionValues;
        /// <summary>
        /// Obtient les valorisations d'actions.
        /// </summary>
        public ActionValue[] ActionValues
        {
            get { return _actionValues; }
            private set
            {
                if (_actionValues != value)
                {
                    _actionValues = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les types d'actions.
        /// </summary>
        public ActionType[] ActionTypes
        {
            get { return _actionTypes; }
            private set
            {
                if (_actionTypes != value)
                {
                    _actionTypes = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit les solutions disponibles.
        /// </summary>
        public BulkObservableCollection<string> Solutions { get; set; }

        /// <summary>
        /// Obtient le validateur de création de liens.
        /// </summary>
        public DependencyCreationValidator DependencyCreationValidator =>
            ((GanttActionsManager)ActionsManager).DependencyCreationValidatorProvider;

        /// <summary>
        /// Obtient ou définit le temps actuel.
        /// </summary>
        public long CurrentTimelinePosition
        {
            get { return _currentTime; }
            set
            {
                if (_currentTime != value)
                {
                    _currentTime = value;
                    OnPropertyChanged();
                    OnCurrentTimelinePositionChanged();
                }
            }
        }

        /// <summary>
        /// Obtient la charge des ressources.
        /// </summary>
        public BulkObservableCollection<ReferentialLoad> ResourcesLoad =>
            ActionsManager.ReferentialsLoad;

        /// <summary>
        /// Obtient le chemin à jouer dans le lecteur.
        /// </summary>
        public ActionPath[] ActionsPath
        {
            get { return _actionsPath; }
            private set
            {
                if (_actionsPath != value)
                {
                    _actionsPath = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit l'élément courant dans la lecture du chemin critique.
        /// </summary>
        public ActionPath CurrentActionPath
        {
            get { return _currentActionPath; }
            set
            {
                if (_currentActionPath != value)
                {
                    ActionPath old = _currentActionPath;
                    _currentActionPath = value;
                    OnCurrentActionPathChanged(old, value);
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient la durée totale du process.
        /// </summary>
        public long TotalDuration
        {
            get { return _totalDuration; }
            private set
            {
                if (_totalDuration != value)
                {
                    _totalDuration = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les changements de durée sur les actions sont autorisés.
        /// </summary>
        public bool AllowTimingsDurationChange
        {
            get { return _allowTimingsDurationChange; }
            private set
            {
                if (_allowTimingsDurationChange != value)
                {
                    _allowTimingsDurationChange = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les niveaux de coûts des actions réduites.
        /// </summary>
        public short[] ReducedActionCosts { get; private set; }

        /// <summary>
        /// Obtient les niveaux de difficulté des actions réduites.
        /// </summary>
        public short[] ReducedActionDifficulties { get; private set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'action sélectionnée peut être réduite.
        /// </summary>
        public bool CanReduceCurrentActionItem
        {
            get
            {
                return !IsReadOnly && CurrentActionItem != null && CurrentScenario != null &&
                    (CurrentScenario.NatureCode == KnownScenarioNatures.Initial || CurrentScenario.NatureCode == KnownScenarioNatures.Target || CurrentScenario.NatureCode == KnownScenarioNatures.Realized);
            }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la réduction de l'action courante est visible.
        /// </summary>
        public Visibility ReduceCurrentActionItemVisibility =>
                CurrentActionItem != null && CurrentScenario != null &&
                    (CurrentScenario.NatureCode == KnownScenarioNatures.Initial || CurrentScenario.NatureCode == KnownScenarioNatures.Target || CurrentScenario.NatureCode == KnownScenarioNatures.Realized) ?
                    Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Obtient la visibilité des solutions.
        /// </summary>
        public Visibility SolutionsVisibility
        {
            get { return _solutionsVisibility; }
            private set
            {
                if (_solutionsVisibility != value)
                {
                    _solutionsVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les solutions.
        /// </summary>
        public Restitution.SolutionWrapper[] SolutionsWrappers
        {
            get { return _solutionsWrappers; }
            private set
            {
                if (_solutionsWrappers != value)
                {
                    _solutionsWrappers = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les filtres IES disponibles.
        /// </summary>
        public IESFilter[] IESFilters
        {
            get { return _iESFilters; }
            private set
            {
                if (_iESFilters != value)
                {
                    _iESFilters = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le filtre IES défini.
        /// </summary>
        public IESFilter SelectedIESFilter
        {
            get { return _selectedIESFilter; }
            set
            {
                if (_selectedIESFilter != value)
                {
                    _selectedIESFilter = value;
                    OnPropertyChanged();
                    OnSelectedIESFilterChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit la vue sélectionnée.
        /// </summary>
        public override GanttGridViewContainer ViewContainer
        {
            get { return ViewContainerInternal; }
            set
            {
                if (ViewContainerInternal != value)
                {
                    ViewContainerInternal = value;
                    OnPropertyChanged();
                    ChangeBuildView((AnalyzeBuildViewModelView)ViewContainerInternal.View);
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si la vidéo des référentiels peut être jouée.
        /// </summary>
        public bool CanPlayReferential =>
            View != GanttGridView.WBS;

        /// <summary>
        /// Obtient la visibilité du filtre IES.
        /// </summary>
        public Visibility IESFilterVisibility
        {
            get { return _IESFilterVisibility; }
            private set
            {
                if (_IESFilterVisibility != value)
                {
                    _IESFilterVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        public PlayingSource PlayingSource
        {
            get { return _playingSource; }
            private set
            {
                if (_playingSource != value)
                {
                    _playingSource = value;
                    OnPropertyChanged(nameof(PlayingSource));
                }
            }
        }

        /// <summary>
        /// Obtient les filtres sur le chart de charge.
        /// </summary>
        public string[] LoadChartFilters { get; private set; }

        string _selectedloadChartFilter;
        /// <summary>
        /// Obtient ou définit le filtre sur le chart de charge.
        /// </summary>
        public string SelectedLoadChartFilter
        {
            get { return _selectedloadChartFilter; }
            set
            {
                if (_selectedloadChartFilter != value)
                {
                    _selectedloadChartFilter = value;
                    OnPropertyChanged();
                    OnSelectedloadChartFilterChanged();
                }
            }
        }

        /// <summary>
        /// Obtient le libellé de l'onglet d'amélioration.
        /// </summary>
        public string ReductionLabel
        {
            get
            {
                if (ShowPastInternalisationOrExternalisation)
                    return LocalizationManagerExt.GetSafeDesignerString("View_AnalyzeBuild_Improvement_Past");
                return LocalizationManagerExt.GetSafeDesignerString("View_AnalyzeBuild_Improvement");
            }
        }

        #endregion

        #region Commandes

        protected bool ValidateCommandIsRunning { get; set; }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande ValidateCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnValidateCommandCanExecute() =>
            IsNotReadOnly && !ValidateCommandIsRunning;

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande CancelCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnCancelCommandCanExecute() =>
            !CanChange;

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande CancelCommand
        /// </summary>
        protected override void OnCancelCommandExecute()
        {
            // Fix for CustomNumericValues
            if (CurrentActionItem != null)
            {
                if (CurrentActionItem.Action.ChangeTracker.ModifiedValues.ContainsKey(nameof(KAction.CustomNumericValue)))
                {
                    CurrentActionItem.Action.CustomNumericValue = (double?)CurrentActionItem.Action.ChangeTracker.OriginalValues[nameof(KAction.CustomNumericValue)];
                    CurrentActionItem.Action.CustomNumericValueText = CurrentActionItem.Action.CustomNumericValue == null ? null : string.Format("{0:G29}", CurrentActionItem.Action.CustomNumericValue);
                }
                if (CurrentActionItem.Action.ChangeTracker.ModifiedValues.ContainsKey(nameof(KAction.CustomNumericValue2)))
                {
                    CurrentActionItem.Action.CustomNumericValue2 = (double?)CurrentActionItem.Action.ChangeTracker.OriginalValues[nameof(KAction.CustomNumericValue2)];
                    CurrentActionItem.Action.CustomNumericValue2Text = CurrentActionItem.Action.CustomNumericValue2 == null ? null : string.Format("{0:G29}", CurrentActionItem.Action.CustomNumericValue2);
                }
                if (CurrentActionItem.Action.ChangeTracker.ModifiedValues.ContainsKey(nameof(KAction.CustomNumericValue3)))
                {
                    CurrentActionItem.Action.CustomNumericValue3 = (double?)CurrentActionItem.Action.ChangeTracker.OriginalValues[nameof(KAction.CustomNumericValue3)];
                    CurrentActionItem.Action.CustomNumericValue3Text = CurrentActionItem.Action.CustomNumericValue3 == null ? null : string.Format("{0:G29}", CurrentActionItem.Action.CustomNumericValue3);
                }
                if (CurrentActionItem.Action.ChangeTracker.ModifiedValues.ContainsKey(nameof(KAction.CustomNumericValue4)))
                {
                    CurrentActionItem.Action.CustomNumericValue4 = (double?)CurrentActionItem.Action.ChangeTracker.OriginalValues[nameof(KAction.CustomNumericValue4)];
                    CurrentActionItem.Action.CustomNumericValue4Text = CurrentActionItem.Action.CustomNumericValue4 == null ? null : string.Format("{0:G29}", CurrentActionItem.Action.CustomNumericValue4);
                }
            }
            // Fix for CustomNumericValues

            HideValidationErrors();

            ActionsManager.UnregisterAllItems();

            ActionGanttItem previousSelection = CurrentActionItem;

            // Détecter les entités nouvelles et les supprimer
            KAction[] newActions = ActionGridItems.Select(i => i.Action).Distinct().Where(a => a.IsMarkedAsAdded).ToArray();
            foreach (KAction action in newActions)
                ActionsManager.DeleteAction(action);

            ObjectWithChangeTrackerExtensions.CancelChanges(
                Categories,
                Skills,
                new Scenario[] { CurrentScenario },
                CurrentScenario.Actions,
                CurrentScenario.Actions.Where(a => a.IsReduced).Select(a => a.Reduced),
                CurrentScenario.Solutions
                );

            if (EnableSolutions)
            {
                _scenarios.MapReducedResources();
            }

            RebuildItems(previousSelection);

            CanChange = true;
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande ValidateCommand
        /// </summary>
        protected override async void OnValidateCommandExecute()
        {
            ValidateCommandIsRunning = true;

            // Valider l'objet
            if (!ValidateActions())
            {
                ValidateCommandIsRunning = false;
                return;
            }

            if (await SaveActions(true))
            {
                CanChange = true;
                if (_navigationToken?.IsValid == true)
                    _navigationToken.Navigate();
            }
            RefreshSolutions();

            ValidateCommandIsRunning = false;
        }

        /// <summary>
        /// Appelé afin de déterminer la commande MoveUp peut être exécutée.
        /// </summary>
        /// <returns><c>true</c> si la commande peut être executée.</returns>
        protected override bool OnCanMoveUp() =>
            View == GanttGridView.WBS && !IsReadOnly;

        /// <summary>
        /// Appelé lorsque un déplacement vers le haut est demandé.
        /// </summary>
        protected override void OnMoveUp()
        {
            StopPlaying();
            ActionsManager.MoveUpAboveSibling(CurrentActionItem);
            OnActionHierarchyChanged();
        }

        /// <summary>
        /// Appelé afin de déterminer la commande MoveDown peut être exécutée.
        /// </summary>
        /// <returns><c>true</c> si la commande peut être executée.</returns>
        protected override bool OnCanMoveDown() =>
            View == GanttGridView.WBS && !IsReadOnly;

        /// <summary>
        /// Appelé lorsque un déplacement vers le bas est demandé.
        /// </summary>
        protected override void OnMoveDown()
        {
            StopPlaying();
            ActionsManager.MoveDownBelowSibling(CurrentActionItem);
            OnActionHierarchyChanged();
        }

        Command _convertToReducedCommand;
        /// <summary>
        /// Obtient la commande permettant de 
        /// </summary>
        public ICommand ConvertToReducedCommand
        {
            get
            {
                if (_convertToReducedCommand == null)
                    _convertToReducedCommand = new Command(() =>
                    {
                        if (CurrentActionItem != null && CurrentActionItem.Action.Reduced == null)
                        {
                            CurrentActionItem.Action.Reduced = new KActionReduced()
                            {
                                ReductionRatio = 0.0,
                                OriginalBuildDuration = CurrentActionItem.Action.Original != null ? CurrentActionItem.Action.Original.BuildDuration : CurrentActionItem.Action.BuildDuration,
                            };

                            // Copier le type de l'action depuis la catégorie ou alors mettre I par défaut
                            if (CurrentActionItem.Action.Category != null)
                                CurrentActionItem.Action.Reduced.ActionType = CurrentActionItem.Action.Category.Type;

                            if (CurrentActionItem.Action.Reduced.ActionType == null)
                                CurrentActionItem.Action.Reduced.ActionType =
                                    ActionTypes.First(at => at.ActionTypeCode == KnownActionCategoryTypes.I);

                            RegisterReduced(CurrentActionItem.Action.Reduced);
                        }
                    }, () => CanReduceCurrentActionItem);
                return _convertToReducedCommand;
            }
        }

        Command _pauseCommand;
        /// <summary>
        /// Obtient la commande permettant de mettre en pause l'action actuellement sélectionnée.
        /// </summary>
        public ICommand PauseCommand
        {
            get
            {
                if (_pauseCommand == null)
                    _pauseCommand = new Command(() =>
                    {
                        EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Pause));
                    });
                return _pauseCommand;
            }
        }

        Command _playCurrentAction;
        /// <summary>
        /// Obtient la commande permettant de jouer l'action actuellement sélectionnée.
        /// </summary>
        public ICommand PlayCurrentAction
        {
            get
            {
                if (_playCurrentAction == null)
                    _playCurrentAction = new Command(() =>
                    {
                        SetCurrentActionAsActionPath();

                        EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Play));
                    }, () => CurrentActionItem != null && !CurrentActionItem.Action.IsGroup);
                return _playCurrentAction;
            }
        }

        Command _resetAndPlayCurrentAction;
        /// <summary>
        /// Obtient la commande permettant de jouer l'action actuellement sélectionnée.
        /// </summary>
        public ICommand ResetAndPlayCurrentAction
        {
            get
            {
                if (_resetAndPlayCurrentAction == null)
                    _resetAndPlayCurrentAction = new Command(() =>
                    {
                        SetCurrentActionAsActionPath(forceReset: true);

                        EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Play));
                    }, () => CurrentActionItem != null);
                return _resetAndPlayCurrentAction;
            }
        }

        Command _playCriticalPath;
        /// <summary>
        /// Obtient la commande permettant de jouer le chemin critique.
        /// </summary>
        public ICommand PlayCriticalPath
        {
            get
            {
                if (_playCriticalPath == null)
                    _playCriticalPath = new Command(() =>
                    {
                        PlayingSource = PlayingSource.CriticalPath;

                        ActionsPath = _criticalPath;

                        EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Play));
                    }, () => CurrentActionItem != null);
                return _playCriticalPath;
            }
        }

        Command _playReferentialCommand;
        /// <summary>
        /// Obtient la commande permettant de lire les actions de la ressource sélectionnée.
        /// </summary>
        public ICommand PlayReferentialCommand
        {
            get
            {
                if (_playReferentialCommand == null)
                    _playReferentialCommand = new Command(() =>
                    {
                        PlayReferential();

                        EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Play));
                    }, () => CurrentActionItem != null);
                return _playReferentialCommand;
            }
        }

        Command<GanttChartItem> _selectGanttItemCommand;
        /// <summary>
        /// Obtient la commande permettant de sélectionner le GanttItem spécifié en argument.
        /// </summary>
        public ICommand SelectGanttItemCommand
        {
            get
            {
                if (_selectGanttItemCommand == null)
                    _selectGanttItemCommand = new Command<GanttChartItem>(param =>
                    {
                        _ignoreNextTimelineSyncFromCurrentAction = true;
                        CurrentGridItem = param;
                        _ignoreNextTimelineSyncFromCurrentAction = false;
                    });
                return _selectGanttItemCommand;
            }
        }

        #endregion

        #region Méthodes protégées

        /// <summary>
        /// Crée les filtres IES.
        /// </summary>
        /// <returns>Les filtres à utiliser.</returns>
        protected abstract IESFilter[] GetIESFilters();

        /// <summary>
        /// Charge les données.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        protected abstract Task LoadData(int projectId);

        /// <summary>
        /// Sauvegarde les actions.
        /// </summary>
        /// <param name="scenarios">Les scénarios à sauvegarder.</param>
        /// <param name="scenario">Le scénario àsauvegarder.</param>
        protected abstract Task<Scenario> SaveActionsServiceCall(Scenario[] scenarios, Scenario scenario);

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Charge les données.
        /// </summary>
        /// <param name="data">Les données.</param>
        protected void LoadDataInternal(Business.Dtos.BuildData data)
        {
            _scenarios.AddRange(data.Scenarios);

            _allActionTypes = data.ActionTypes;

            _scenarios.MapReducedResources();

            IProjectManagerService projectService = ServiceBus.Get<IProjectManagerService>();
            if (projectService.CurrentScenario == null)
                projectService.SelectScenario(_scenarios.First(s => ScenarioNaturesFilter.Contains(s.NatureCode)));
            else
                TryLoadScenario(projectService.CurrentScenario.Id);

            CustomFieldsLabels = new CustomFieldsLabels(data.CustomFieldsLabels);

            Categories.AddRange(data.Categories);
            AddLabelSortDescription(Categories);

            Skills.AddRange(data.Skills);
            AddLabelSortDescription(Skills);

            Equipments.AddRange(data.Resources.OfType<Equipment>());
            AddLabelSortDescription(Equipments);

            Operators.AddRange(data.Resources.OfType<Operator>());
            AddLabelSortDescription(Operators);

            RefreshSolutions();

            EventBus.Publish(new GanttAutoScaleEvent(this));

            HideSpinner();
        }

        /// <summary>
        /// Méthode appelée lors du rafraîchissement
        /// </summary>
        protected override async Task OnRefreshing()
        {
            await base.OnRefreshing();

            ActionsManager.Clear();

            Categories?.Clear();
            Skills?.Clear();
            Equipments?.Clear();
            Operators?.Clear();
            Ref1s?.Clear();
            Ref2s?.Clear();
            Ref4s?.Clear();
            Ref5s?.Clear();
            Ref6s?.Clear();
            Ref7s?.Clear();
            Ref3s?.Clear();

            int projectId = ServiceBus.Get<IProjectManagerService>().CurrentProject.ProjectId;
            await LoadData(projectId);
        }


        /// <summary>
        /// Crée la sélection des filtres IES.
        /// </summary>
        void CreateIESFilters()
        {
            IESFilter previousSelection = _selectedIESFilter;

            try
            {
                _ignoreSelectedIESFilterChanges = true;
                IESFilters = GetIESFilters();
            }
            finally
            {
                _ignoreSelectedIESFilterChanges = false;
            }

            if (previousSelection != null)
                _selectedIESFilter = IESFilters.FirstOrDefault(f => f.Value == previousSelection.Value);

            if (_selectedIESFilter == null)
                _selectedIESFilter = IESFilters.First();

            OnPropertyChanged(nameof(SelectedIESFilter));
            OnPropertyChanged(nameof(ShowPastInternalisationOrExternalisation));
            OnPropertyChanged(nameof(ReductionLabel));
        }

        /// <summary>
        /// Met à jour les types I/E/S dispos dans la réduction.
        /// </summary>
        void UpdateReductionActionTypes()
        {
            string eLabel, iLabel;
            ActionType i = _allActionTypes.First(at => at.ActionTypeCode == KnownActionCategoryTypes.I);
            ActionType e = _allActionTypes.First(at => at.ActionTypeCode == KnownActionCategoryTypes.E);
            ActionType s = _allActionTypes.First(at => at.ActionTypeCode == KnownActionCategoryTypes.S);

            if (ShowPastInternalisationOrExternalisation)
            {
                ActionTypes = _allActionTypes.Where(a => a.ActionTypeCode != KnownActionCategoryTypes.S).ToArray();

                // Passer le libellé "To keep Int" à "Internal"
                iLabel = LocalizationManager.GetString("View_AnalyzeBuild_ActionImproved_Internalized");
                // Passer le libellé "Externe" à "Externalisée"
                eLabel = LocalizationManager.GetString("View_AnalyzeBuild_ActionImproved_Externalized");
            }
            else
            {
                ActionTypes = _allActionTypes;
                eLabel = e.LongLabel;
                iLabel = i.LongLabel;
            }

            i.ContextualLabel = iLabel;
            e.ContextualLabel = eLabel;
            s.ContextualLabel = s.LongLabel;
        }

        /// <summary>
        /// Appelé lorsque la valeur de la propriété <see cref="GanttGridViewModelBase{TViewModel, TIViewModel, TComponentItem, TActionItem, TResourceItem}.CurrentGridItem"/> a changé.
        /// </summary>
        /// <param name="previousValue">The previous value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnCurrentActionChanged(ActionGanttItem previousValue, ActionGanttItem newValue)
        {
            base.OnCurrentActionChanged(previousValue, newValue);
            OnPropertyChanged(nameof(ReduceCurrentActionItemVisibility));
            OnPropertyChanged(nameof(CanReduceCurrentActionItem));

            if (newValue != null)
            {
                switch (PlayingSource)
                {
                    case PlayingSource.None:
                        //On affiche le snapshot
                        EventBus.Publish(new MediaPlayerActionEvent(this, (player) =>
                        {
                            if (player is KMiniPlayer mediaPlayer)
                                mediaPlayer.ShowThumbnailView(newValue?.Action);
                        }));
                        break;

                    case PlayingSource.CriticalPath:
                        if (ActionsPath == null || !ActionsPath.Any(cp => cp.Action == newValue.Action))
                            StopPlaying();
                        break;

                    case PlayingSource.Action:
                        StopPlaying();
                        //On affiche le snapshot
                        EventBus.Publish(new MediaPlayerActionEvent(this, (player) =>
                        {
                            if (player is KMiniPlayer mediaPlayer)
                                mediaPlayer.ShowThumbnailView(newValue?.Action);
                        }));
                        break;

                    case PlayingSource.Referential:
                        if (ActionsPath == null || !ActionsPath.Any(cp => cp.Action == newValue.Action))
                        {
                            StopPlaying();
                            //On affiche le snapshot
                            EventBus.Publish(new MediaPlayerActionEvent(this, (player) =>
                            {
                                if (player is KMiniPlayer mediaPlayer)
                                    mediaPlayer.ShowThumbnailView(newValue?.Action);
                            }));
                        }
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(PlayingSource));
                }

                if (!_ignoreNextTimelineSyncFromCurrentAction)
                    CurrentTimelinePosition = GanttDates.ToTicks(newValue.Start);
            }
            else
            {
                if (PlayingSource != PlayingSource.Referential && PlayingSource != PlayingSource.CriticalPath)
                    StopPlaying();
                //On affiche le snapshot
                EventBus.Publish(new MediaPlayerActionEvent(this, (player) =>
                {
                    if (player is KMiniPlayer mediaPlayer)
                        mediaPlayer.ShowThumbnailView(newValue?.Action);
                }));
            }
        }

        /// <summary>
        /// Appelé lorsque le chemin critique a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.EventArgs&lt;ViewModels.ActionCriticalPath[]&gt;"/> contenant les données de l'évènement.</param>
        void OnActionsManagerCriticalPathChanged(object sender, EventArgs<ActionPath[]> e)
        {
            _criticalPath = e.Data;

            StopPlaying();

            if (e.Data != null && e.Data.Any())
                TotalDuration = e.Data.Max(acp => acp.Finish) - e.Data.Min(acp => acp.Start);
            else if (ActionGridItems != null && ActionGridItems.Any())
                TotalDuration = GanttDates.ToTicks(ActionGridItems.Max(i => i.Finish) - ActionGridItems.Min(i => i.Start));
            else
                TotalDuration = 0;
        }

        /// <summary>
        /// Définit l'action actuel comme étant celle du chemin à visionner.
        /// </summary>
        void SetCurrentActionAsActionPath(bool forceReset = false)
        {
            PlayingSource = PlayingSource.Action;

            if (forceReset || ActionsPath == null || (ActionsPath.Any() && ActionsPath[0].Action != CurrentActionItem.Action))
            {
                ActionsPath = new ActionPath[]
                {
                    new ActionPath(CurrentActionItem.Action)
                    {
                        Start = CurrentActionItem.ManagedStart,
                        Finish = CurrentActionItem.ManagedFinish,
                    },
                };
            }
        }

        /// <summary>
        /// Joue la ressource de l'action en cours.
        /// </summary>
        void PlayReferential()
        {
            PlayingSource = PlayingSource.Referential;

            ActionsPath = ActionsManager.GetReferentialCriticalPath(CurrentActionItem);
        }

        /// <summary>
        /// Enregistre les actions modifiées.
        /// </summary>
        /// <param name="refreshSelectionWhenDone"><c>true</c> pour rafraichir la sélection une fois la sauvegarde effectuée.</param>
        protected override async Task<bool> SaveActions(bool refreshSelectionWhenDone)
        {
            ShowSpinner();

            try
            {
                if (await ShowImpactedScenarios(CurrentScenario, _scenarios.ToArray(), false, null, null))
                    return await SaveActionsWithoutPrompt(refreshSelectionWhenDone);
                HideSpinner();
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
            return false;
        }

        /// <summary>
        /// Enregistre les actions modifiées.
        /// </summary>
        /// <param name="refreshSelectionWhenDone"><c>true</c> pour rafraichir la sélection une fois la sauvegarde effectuée.</param>
        protected async Task<bool> SaveActionsWithoutPrompt(bool refreshSelectionWhenDone)
        {
            try
            {
                CurrentScenario = await SaveActionsServiceCall(_scenarios.ToArray(), CurrentScenario);

                ActionsManager = new GanttActionsManager((BulkObservableCollection<GanttChartItem>)ActionItems,
                    i => CurrentGridItem = i, null)
                {
                    EnableRessourceLoad = true,
                };
                ActionsManager.CriticalPathChanged += OnActionsManagerCriticalPathChanged;
                ActionsManager.ReducedApprovedChanged += OnReducedApprovedChanged;
                _scenarios = new BulkObservableCollection<Scenario>(true);
                Operators = new BulkObservableCollection<Operator>(true);
                Equipments = new BulkObservableCollection<Equipment>(true);
                Solutions = new BulkObservableCollection<string>();

                ReducedActionCosts = KnownReducedActionValues.Costs;
                ReducedActionDifficulties = KnownReducedActionValues.Difficulties;

                _loadChartFilterAll = LocalizationManager.GetString("View_AnalyzeBuild_All");
                _loadChartFilterEquipment = LocalizationManager.GetString("View_AnalyzeAcquire_Equipment");
                _loadChartFilterOperator = LocalizationManager.GetString("View_AnalyzeAcquire_Operator");

                LoadChartFilters = new string[]
                {
                _loadChartFilterAll,
                _loadChartFilterEquipment,
                _loadChartFilterOperator,
                };
                SelectedLoadChartFilter = _loadChartFilterAll;
                await Refresh();


                // Redémarrer le tracking
                CurrentScenario.StartTracking();
                foreach (KAction action in CurrentScenario.Actions)
                {
                    action.StartTracking();
                    if (action.IsReduced)
                        action.Reduced.StartTracking();
                }


                foreach (Solution solution in CurrentScenario.Solutions)
                    solution.StartTracking();

                if (refreshSelectionWhenDone)
                    RebuildItems(CurrentActionItem);
                else
                    RebuildItems(null);

                UpdateIsReadOnly();

                HideSpinner();

                return true;
            }
            catch (Exception e)
            {
                base.OnError(e);
                return false;
            }
        }

        /// <summary>
        /// Tente de charger le scénario spécifié.
        /// </summary>
        /// <param name="scenarioId">L'identifiant du scénario.</param>
        void TryLoadScenario(int scenarioId)
        {
            Scenario old = CurrentScenarioInternal;
            Scenario sc = _scenarios.FirstOrDefault(s => s.ScenarioId == scenarioId);
            if (sc == null)
            {
                sc = _scenarios.First();
                ServiceBus.Get<IProjectManagerService>().SelectScenario(_scenarios.First());
            }

            CurrentScenario = sc;
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="CurrentScenario"/> a changé.
        /// </summary>
        /// <param name="oldScenario">L'ancien scénario.</param>
        /// <param name="newScenario">Le nouveau scénario.</param>
        void LoadScenario(Scenario oldScenario, Scenario newScenario)
        {
            StopPlaying();
            ActionsManager.Clear();
            ActionItems.Clear();
            CreateIESFilters();
            UpdateEnableReducedPercentageRefresh();

            RegisterToStateChanged(oldScenario, newScenario);

            if (oldScenario != null)
            {
                oldScenario.StopTracking();
                foreach (KAction action in oldScenario.Actions)
                {
                    action.StopTracking();
                    UnregisterToStateChanged(action);

                    UnregisterReduced(action.Reduced);
                }

                foreach (Solution solution in oldScenario.Solutions)
                    solution.StopTracking();

            }

            if (newScenario != null)
            {
                newScenario.MapApprovedReduction();
                GanttActionsManager.RegisterInitialActions(newScenario.Actions, SelectedIESFilter.Value);

                newScenario.StartTracking();
                foreach (KAction action in newScenario.Actions)
                {
                    action.StartTracking();
                    RegisterToStateChanged(action);

                    RegisterReduced(action.Reduced);
                }

                foreach (Solution solution in newScenario.Solutions)
                    solution.StartTracking();

                RestoreActionSelection();

                if (NavigationService != null && NavigationService.Preferences.TimelinePosition.HasValue)
                {
                    _isChangingCriticalPathAction = true;
                    CurrentTimelinePosition = NavigationService.Preferences.TimelinePosition.Value;
                    _isChangingCriticalPathAction = false;
                }

                if (EnableSolutions)
                    foreach (Solution solution in newScenario.Solutions)
                        RegisterToStateChanged(solution);
                UpdateIESFilterVisibility();
            }

            UpdateIsReadOnly();
            UpdateReductionActionTypes();
            UpdateSolutionsVisibility();
            UpdateSolutions();
            OnPropertyChanged(nameof(CanReduceCurrentActionItem));
            OnPropertyChanged(nameof(ReduceCurrentActionItemVisibility));
        }

        /// <summary>
        /// S'abonne aux évènements de l'action réduite.
        /// </summary>
        /// <param name="reduced">L'action réduite.</param>
        void RegisterReduced(KActionReduced reduced)
        {
            if (reduced != null)
            {
                reduced.StartTracking();
                RegisterToStateChanged(reduced);
                if (EnableSolutions)
                    reduced.SolutionChanged += OnReducedSolutionChanged;
            }
        }

        /// <summary>
        /// Se désabonne aux évènements de l'action réduite.
        /// </summary>
        /// <param name="reduced">L'action réduite.</param>
        void UnregisterReduced(KActionReduced reduced)
        {
            if (reduced != null)
            {
                reduced.StopTracking();
                UnregisterToStateChanged(reduced);
                if (EnableSolutions)
                    reduced.SolutionChanged -= OnReducedSolutionChanged;
            }
        }

        /// <summary>
        /// Appelé lorsqu'un élément est désabonné du changement d'état lors du Cleanup du VM.
        /// </summary>
        /// <param name="item">L'élément.</param>
        protected override void OnItemUnregisteredToStateChangedOnCleanup(IObjectWithChangeTracker item)
        {
            if (item is KAction)
                UnregisterReduced(((KAction)item).Reduced);
        }

        /// <summary>
        /// Appelé lorsque la valeur de la propriété <see cref="Solution"/> d'une action réduite a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.EventArgs&lt;System.String&gt;"/> contenant les données de l'évènement.</param>
        void OnReducedSolutionChanged(object sender, PropertyChangedEventArgs<string> e)
        {
            var currentAction = ((KActionReduced)sender).Action;

            var matchingSolution = CurrentScenario.Solutions.FirstOrDefault(s => s.SolutionDescription == e.NewValue);
            if (matchingSolution != null)
                currentAction.Reduced.Approved = matchingSolution.Approved;

            RefreshSolutions();
        }

        /// <summary>
        /// Appelé lorsque la valeur de la propriété <see cref="KActionReduced.Approved"/> d'une action réduite a changé.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Les <see cref="Models.PropertyChangedEventArgs&lt;System.Boolean&gt;"/> contenant les données de l'évènement.</param>
        void OnReducedApprovedChanged(object sender, EventArgs<KActionReduced> e)
        {
            // Appliquer la valeur sur tous les champs réduits
            KAction currentAction = e.Data.Action;

            foreach (KAction action in CurrentScenario.Actions.Where(a => a != currentAction && a.IsReduced && a.Reduced.Solution == currentAction.Reduced.Solution))
                action.Reduced.Approved = e.Data.Approved;

            // Rechercher la solution correspondante et si elle existe, appliquer la modification
            Solution solution = CurrentScenario.Solutions.FirstOrDefault(s => s.SolutionDescription == currentAction.Reduced.Solution);
            if (solution != null)
                solution.Approved = e.Data.Approved;

            _scenarios.MapReducedResources();
        }

        /// <summary>
        /// Appelé lorsque la valeur de la propriété <see cref="GanttGridViewModelBase{TViewModel, TIViewModel, TComponentItem, TActionItem, TResourceItem}.CanChange"/> a changé.
        /// </summary>
        protected override void OnCanChangeChanged()
        {
            base.OnCanChangeChanged();

            IsScenarioPickerEnabled = CanChange;
        }

        /// <summary>
        /// Met à jour l'état lecture seule du scénario.
        /// </summary>
        void UpdateIsReadOnly()
        {
            IsReadOnly =
                (CurrentScenario != null &&
                    ServiceBus.Get<IProjectManagerService>().Scenarios.First(sc => sc.Id == CurrentScenarioInternal.ScenarioId).IsLocked) ||
                !CanCurrentUserWrite ||
                SelectedIESFilter.Value != IESFilterValue.IES;

            UpdateAllowTimingsChange();
        }

        /// <summary>
        /// Met à jour la propriété AllowTimingsChange.
        /// </summary>
        void UpdateAllowTimingsChange()
        {
            AllowTimingsDurationChange = !IsReadOnly &&
                CurrentScenario.NatureCode != KnownScenarioNatures.Initial &&
                CurrentScenario.NatureCode != KnownScenarioNatures.Realized;


            ActionsManager.AllowTimingsChange = !IsReadOnly;
            ActionsManager.AllowTimingsDurationChange = AllowTimingsDurationChange;
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="CurrentActionPath"/> a changé.
        /// </summary>
        /// <param name="old">L'ancienne valeur.</param>
        /// <param name="newValue">la nouvelle valeur.</param>
        void OnCurrentActionPathChanged(ActionPath old, ActionPath newValue)
        {
            if (!_isChangingCriticalPathAction && !_ignoreNextTimelineSyncFromCurrentAction)
            {
                if (newValue != null)
                {

                    _isChangingCriticalPathAction = true;
                    _ignoreNextTimelineSyncFromCurrentAction = true;
                    CurrentGridItem = ActionGridItems.First(i => i.Action == newValue.Action);
                    _ignoreNextTimelineSyncFromCurrentAction = false;
                    _isChangingCriticalPathAction = false;
                }
                // Inutile de déselectionner si this.ActionsPath == null
                else if (ActionsPath != null && ActionsPath.Length > 0)
                    CurrentGridItem = null;
            }
        }

        /// <summary>
        /// Valide les actions qui ont changé.
        /// </summary>
        /// <returns><c>true</c> si les actions sont valides.</returns>
        bool ValidateActions()
        {
            ActionsManager.FixPredecessorsSuccessorsTimings();

            CurrentScenario.Validate();

            KAction[] changedActions = ActionGridItems
                .Select(i => i.Action)
                .Where(a => a.IsNotMarkedAsUnchanged)
                .ToArray();

            foreach (KAction action in changedActions)
                action.Validate();

            List<ValidatableObject> allChanged = new List<ValidatableObject>(changedActions) { CurrentScenario };

            RefreshValidationErrors(allChanged);

            if (allChanged.Any(a => !a.IsValid.GetValueOrDefault()))
            {
                // Active la validation auto
                foreach (ValidatableObject item in allChanged)
                    item.EnableAutoValidation = true;

                return false;
            }

            return true;
        }

        /// <summary>
        /// Appelé lorsque la refraichissement des erreurs de validation est demandé.
        /// Dans une méthode dérivée, appeler RefreshValidationErrors.
        /// </summary>
        protected override void OnRefreshValidationErrorsRequested() =>
            ValidateActions();

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="CurrentTimelinePosition"/> a changé.
        /// </summary>
        void OnCurrentTimelinePositionChanged() =>
            NavigationService.Preferences.TimelinePosition = CurrentTimelinePosition;

        /// <summary>
        /// Rafraichit la liste des solutions.
        /// </summary>
        void RefreshSolutions()
        {
            if (!EnableSolutions)
                return;
            if (_scenarios != null && _scenarios.Any())
            {
                DispatcherHelper.SafeInvoke(() =>
                {
                    Solutions.ReplaceAll(
                        _scenarios
                            .SelectMany(s => s.Actions)
                            .Where(a => a.IsReduced)
                            .Select(a => a.Reduced.Solution)
                            .Distinct()
                        );
                    OnPropertyChanged(nameof(Solutions));
                });
            }
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="GanttGridViewModelBase{TViewModel, TIViewModel, TComponentItem, TActionItem, TResourceItem}.IsReadOnly"/> a changé.
        /// </summary>
        protected override void OnIsReadOnlyChanged()
        {
            OnPropertyChanged(nameof(CanReduceCurrentActionItem));
            OnPropertyChanged(nameof(ReduceCurrentActionItemVisibility));
        }

        /// <summary>
        /// Change la vue.
        /// </summary>
        /// <param name="view">La vue.</param>
        protected override void ChangeView(GanttGridView view)
        {
            base.ChangeView(view);
            OnPropertyChanged(nameof(CanPlayReferential));
            HideSolutions();
            IESFilterVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Cache les solutions.
        /// </summary>
        void HideSolutions()
        {
            SolutionsVisibility = Visibility.Collapsed;
            UpdateSolutions();
        }

        /// <summary>
        /// Affiche les solutions.
        /// </summary>
        void ShowSolutions()
        {
            if (!EnableSolutions)
                return;
            SolutionsVisibility = Visibility.Visible;
            UpdateSolutions();
        }

        /// <summary>
        /// Vide la liste des solutions.
        /// </summary>
        void ClearSolutions()
        {
            if (!EnableSolutions)
                return;
            // Se désabonner au évènements de modification
            if (SolutionsWrappers != null)
                foreach (Restitution.SolutionWrapper solution in SolutionsWrappers)
                {
                    UnregisterToStateChanged(solution.Solution);
                    solution.Solution.ApprovedChanged -= OnSolutionApprovedChanged;
                }
        }

        /// <summary>
        /// Met à jour la collection de solutions à afficher.
        /// </summary>
        void UpdateSolutions()
        {
            if (!EnableSolutions)
                return;
            ClearSolutions();

            if (CurrentScenario != null && SolutionsVisibility != Visibility.Collapsed)
            {
                List<Restitution.SolutionWrapper> solutions = new List<Restitution.SolutionWrapper>();

                // On ne doit pas afficher les solutions du scénario de validation
                if (CurrentScenario.NatureCode != KnownScenarioNatures.Realized)
                {
                    foreach (Solution solution in CurrentScenario.Solutions.OrderBy(s => s.SolutionDescription))
                    {
                        Restitution.SolutionWrapper w = CreateSolutionWrapper(CurrentScenario.Actions, solution);
                        w.IsNotReadOnly = true;
                        solutions.Add(w);

                        RegisterToStateChanged(solution);
                        solution.ApprovedChanged += OnSolutionApprovedChanged;
                    }
                }

                Scenario originalScenario = CurrentScenario.Original;
                while (originalScenario != null)
                {
                    // On ne doit pas afficher le scénario initial
                    if (originalScenario.NatureCode == KnownScenarioNatures.Initial)
                        break;

                    // Déterminer les actions qui sont concernées
                    IEnumerable<KAction> originalActions = originalScenario.Actions.Where(originalAction =>
                        CurrentScenario.Actions.Any(currentScenarioAction =>
                            ScenarioActionHierarchyHelper.IsAncestor(originalAction, currentScenarioAction)));

                    foreach (Solution solution in originalScenario.Solutions.OrderBy(s => s.SolutionDescription))
                    {
                        Restitution.SolutionWrapper wrapper = CreateSolutionWrapper(originalActions, solution);
                        // Ignorer les solutions qui n'apportent pas de gain. C'est un surplus d'infos inutile
                        if (wrapper.Saving != 0)
                            solutions.Add(wrapper);
                    }

                    originalScenario = originalScenario.Original;
                }

                // Définir l'index
                int i = 1;
                foreach (Restitution.SolutionWrapper wrapper in solutions)
                    wrapper.Index = i++;

                SolutionsWrappers = solutions.ToArray();
            }
            else
                SolutionsWrappers = null;
        }

        /// <summary>
        /// Appelé lorsque la valeur de la propriété <see cref="KActionReduced.Approved"/> a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="Models.PropertyChangedEventArgs&lt;System.Boolean&gt;"/> contenant les données de l'évènement.</param>
        void OnSolutionApprovedChanged(object sender, PropertyChangedEventArgs<bool> e)
        {
            // Rafraichir l'état approved sur toutes les actions
            Solution solution = (Solution)sender;
            foreach (KAction action in CurrentScenario.Actions.Where(a => a.IsReduced && a.Reduced.Solution == solution.SolutionDescription))
                action.Reduced.Approved = e.NewValue;

            if (SolutionsWrappers != null)
            {
                Restitution.SolutionWrapper wrapper = SolutionsWrappers.FirstOrDefault(w => w.Solution == solution);
                if (wrapper != null)
                    wrapper.RefreshSavings(CurrentScenario.Actions);
            }
        }

        /// <summary>
        /// Crée un conteneur pour la solution.
        /// </summary>
        /// <param name="actions">Les actions qui peuvent être liées à la solution.</param>
        /// <param name="solution">La solution.</param>
        /// <returns>Le conteneur</returns>
        Restitution.SolutionWrapper CreateSolutionWrapper(IEnumerable<KAction> actions, Solution solution)
        {
            Restitution.SolutionWrapper wrapper = new Restitution.SolutionWrapper(solution);
            wrapper.SetRelatedActions(actions);

            return wrapper;
        }

        /// <summary>
        /// Détermine si la sélection peut être changée et s'il n'y pas de modifications en attente de sauvegarde.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la sélection peut être changée et s'il n'y pas de modifications en attente de sauvegarde; sinon, <c>false</c>.
        /// </returns>
        protected override bool CanChangeAndHasNoPendingChanges() =>
            base.CanChangeAndHasNoPendingChanges() && !CurrentScenario.Solutions.Any(s => s.IsNotMarkedAsUnchanged);

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="SelectedIESFilter"/> a changé.
        /// </summary>
        void OnSelectedIESFilterChanged()
        {
            StopPlaying();
            if (!_ignoreSelectedIESFilterChanges && ActionsManager != null)
                LoadScenario(CurrentScenario, CurrentScenario);
        }

        /// <summary>
        /// Change la vue pour Constuire.
        /// </summary>
        /// <param name="view">La vue.</param>
        void ChangeBuildView(AnalyzeBuildViewModelView view)
        {
            StopPlaying();
            if (view == AnalyzeBuildViewModelView.Solutions)
                ShowSolutions();
            else
                ChangeView((GanttGridView)view);

            UpdateSolutionsVisibility();
            UpdateIESFilterVisibility();
        }

        /// <summary>
        /// Met à jour les vues
        /// </summary>
        void UpdateSolutionsVisibility()
        {
            _solutionsView.Visibility = EnableSolutions ? Visibility.Visible : Visibility.Collapsed;

            if (!EnableSolutions && ViewContainer == _solutionsView)
                ChangeBuildView(AnalyzeBuildViewModelView.WBS);
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="SelectedLoadChartFilter"/> a changé.
        /// </summary>
        void OnSelectedloadChartFilterChanged()
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(ResourcesLoad);
            using (cv.DeferRefresh())
            {
                if (SelectedLoadChartFilter != null && SelectedLoadChartFilter != _loadChartFilterAll)
                {
                    cv.Filter = (o) =>
                    {
                        Resource resource = ((ReferentialLoad)o).Resource;
                        if (resource == null)
                            return false;

                        if (SelectedLoadChartFilter == _loadChartFilterEquipment)
                            return resource is Equipment;
                        return resource is Operator;
                    };
                }
                else
                    cv.Filter = null;
            }
            if (EventBus != null)
                EventBus.Publish(new RefreshRequestedEvent(this));
        }

        /// <summary>
        /// Met à jour la visibilité du filtre IES.
        /// </summary>
        void UpdateIESFilterVisibility()
        {
            if (CurrentScenario != null)
            {
                bool hideIes = ViewContainer == _solutionsView;

                IESFilterVisibility = hideIes ? Visibility.Collapsed : Visibility.Visible;

                if (hideIes)
                    SelectedIESFilter = IESFilters.First(f => f.Value == IESFilterValue.IES);
            }
        }

        /// <summary>
        /// Arrête la lecture de ActionsPath.
        /// </summary>
        void StopPlaying()
        {
            PlayingSource = PlayingSource.None;
            ActionsPath = null;
        }

        /// <summary>
        /// Met à jour la propriété EnableReducedPercentageRefresh de l'ActionManager.
        /// </summary>
        void UpdateEnableReducedPercentageRefresh() =>
            ActionsManager.EnableReducedPercentageRefresh = CurrentScenarioInternal == null || CurrentScenarioInternal.NatureCode != KnownScenarioNatures.Initial && CurrentScenarioInternal.NatureCode != KnownScenarioNatures.Realized;

        #endregion
    }

    #region Types Liés

    /// <summary>
    /// Enumère les vues possibles
    /// </summary>
    public enum AnalyzeBuildViewModelView
    {
        /// <summary>
        /// Vue par WBS.
        /// </summary>
        WBS = 1,

        /// <summary>
        /// Vue par ressource.
        /// </summary>
        Resource = 2,

        /// <summary>
        /// Vue par catégorie.
        /// </summary>
        Category = 3,

        /// <summary>
        /// Vue référentiel 1.
        /// </summary>
        Ref1 = 4,

        /// <summary>
        /// Vue référentiel 2.
        /// </summary>
        Ref2 = 5,

        /// <summary>
        /// Vue référentiel 3.
        /// </summary>
        Ref3 = 6,

        /// <summary>
        /// Vue référentiel 4.
        /// </summary>
        Ref4 = 7,

        /// <summary>
        /// Vue référentiel 5.
        /// </summary>
        Ref5 = 8,

        /// <summary>
        /// Vue référentiel 6.
        /// </summary>
        Ref6 = 9,

        /// <summary>
        /// Vue référentiel 7.
        /// </summary>
        Ref7 = 10,

        /// <summary>
        /// Vue par solutions
        /// </summary>
        Solutions = 11,
    }

    /// <summary>
    /// La source de la lecture en cours.
    /// </summary>
    public enum PlayingSource
    {
        /// <summary>
        /// Pas de lecture en cours
        /// </summary>
        None,

        /// <summary>
        /// Le chemin critique
        /// </summary>
        CriticalPath,

        /// <summary>
        /// L'action sélectionnée
        /// </summary>
        Action,

        /// <summary>
        /// La ressource sélectionnée
        /// </summary>
        Referential
    }

    #endregion
}