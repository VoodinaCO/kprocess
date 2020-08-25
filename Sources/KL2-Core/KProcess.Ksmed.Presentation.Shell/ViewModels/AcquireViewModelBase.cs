using DlhSoft.Windows.Controls;
using Kprocess.KL2.FileTransfer;
using KProcess.Common;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Business.Dtos.Prepare;
using KProcess.Ksmed.Business.Referentials;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Helpers;
using KProcess.Ksmed.Models.Interfaces;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using KProcess.Presentation.Windows.Controls;
using KProcess.Presentation.Windows.Controls.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Image = NetVips.Image;
using LocalizationManager = KProcess.Globalization.LocalizationManager;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran d'acquisition du scénario principal.
    /// </summary>
    public abstract class AcquireViewModelBase<TViewModel, TIViewModel> : GanttGridViewModelBase<TViewModel, TIViewModel, DataTreeGridItem, ActionGridItem, ReferentialGridItem>, IAcquireViewModel
        where TViewModel : GanttGridViewModelBase<TViewModel, TIViewModel, DataTreeGridItem, ActionGridItem, ReferentialGridItem>, TIViewModel
        where TIViewModel : IAcquireViewModel
    {

        #region Constantes

        const int DefaultThumbnailMaxSize = 300;
        enum Cursors { Start, Finish }

        #endregion

        #region Champs privés

        BulkObservableCollection<INode> _allProcesses;
        INode _currentNode;
        ProjectDir _currentTreeViewFolder;
        Procedure _currentTreeViewProcess;
        Scenario[] _scenarios;
        Video[] _videos;
        Video _currentVideo;
        long _currentVideoPosition;
        bool _autoPause = true;
        ActionGridItem[] _currentMarkers;
        ActionGridItem _selectedMarker;
        bool _ignoreMarkerSelection;
        bool _areMarkersLinked = true;
        bool _isMarkersLinkedModeEnabled = true;
        TimelineNarrow _subTaskCreationTimelineNarrow;
        TimelineNarrow _taskPlayTimelineNarrow;
        bool _hasjustSeeked;
        int? _thumbnailMaxSize;
        string _validationActionLabelLong;
        string _validationActionLabelShort;
        bool _hasThumbnail;
        bool _annotations;
        bool _isCanceling;
        bool _timelinesEndReached;
        bool _startAndFinishAreSame;
        bool _keepVideo = true;
        KAction _afterItem;
        bool _isSameIndent = true;
        Cursors _lastCursor = Cursors.Start;
        bool _currentVideoPositionIsChanging;
        bool _isDuplicating;
        bool _currentProcessIsLinkedToATask;
        bool _justCreatingNewActionWithMarkersUnlinked;

        bool isAdding;
        bool isAddingAsChild;
        bool isDuplicating;
        bool isMoving;
        bool isGrouping;

        /// <summary>
        /// Si définit, une navigation se produit à l'issue d'une sauvegarde
        /// </summary>
        IFrameNavigationToken _navigationToken;

        FileTransferManager _fileTransferManager;
        public FileTransferManager FileTransferManager
        {
            get
            {
                if (_fileTransferManager == null)
                    _fileTransferManager = IoC.Resolve<FileTransferManager>();
                return _fileTransferManager;
            }
        }

        #endregion

        #region Surcharges

        /// <summary>
        /// Initialise le VM en vue du chargement
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            ActionItems = new BulkObservableCollection<DataTreeGridItem>();

            ActionsManager = new GridActionsManager((BulkObservableCollection<DataTreeGridItem>)ActionItems,
                i => CurrentGridItem = i,
                videoId => Videos.FirstOrDefault(v => v.VideoId == videoId))
            {
                AreMarkersLinked = AreMarkersLinked,
                IsCriticalPathEnabled = false,
            };
            ActionsManager.ActionVideoTimingChanged += OnActionVideoTimingChanged;

            Operators = new BulkObservableCollection<Operator>(true);
            Equipments = new BulkObservableCollection<Equipment>(true);
            ActionsLabels = new BulkObservableCollection<string>(true);

            _validationActionLabelLong = LocalizationManager.GetString("VM_Acquire_ValidationActionLabel");
            _validationActionLabelShort = LocalizationManager.GetString("VM_Acquire_ValidationActionLabel_WBSOnly");
        }

        void RefreshSync(string fileName)
        {
            if (Videos?.Any() == true)
                Videos.SingleOrDefault(_ => _.Filename == fileName)?.OnPropertyChanged(nameof(Video.IsSync));
        }

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override async Task OnLoading()
        {
            FileTransferManager.OnFileAdded += (o, e) => RefreshSync(Path.GetFileName(e));
            FileTransferManager.OnFileDeleted += (o, e) => RefreshSync(Path.GetFileName(e));

            await base.OnLoading();
            EventBus.Subscribe<ScenarioChangedEvent>(e =>
            {
                if (e.Scenario != null)
                    TryLoadScenario(e.Scenario.Id);
            });

            IProjectManagerService projectManager = ServiceBus.Get<IProjectManagerService>();
            ProjectInfo currentProject = projectManager.CurrentProject;
            await LoadData(currentProject.ProjectId);

            if (projectManager.IsUnlinkMarkerEnabledAndLocked)
            {
                AreMarkersLinked = false;
                IsMarkersLinkedModeEnabled = false;
            }
            else
                IsMarkersLinkedModeEnabled = true;

            OnPropertyChanged(nameof(IsManualInput));
        }

        /// <summary>
        /// Charge les données.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        protected abstract Task LoadData(int projectId);

        /// <summary>
        /// Charge les données en interne.
        /// </summary>
        /// <param name="data">les données.</param>
        protected async Task LoadDataInternal(AcquireData data)
        {
            var prepareService = ServiceBus.Get<IPrepareService>();
            ProjectsData projectsData = await prepareService.GetProjects();
            var root = new ProjectDir { Id = -1, Name = IoC.Resolve<ILocalizationManager>().GetString("View_PrepareProject_AllProjects"), ParentId = null, IsExpanded = true };
            root.MarkAsUnchanged();
            root.StopTracking();
            IProjectManagerService projectService = ServiceBus.Get<IProjectManagerService>();
            int currentScenarioId = projectService.CurrentScenario?.Id ?? data.Scenarios.First().ScenarioId;
            int? currentProcessId = data.Scenarios.SingleOrDefault(_ => _.ScenarioId == currentScenarioId)?.Project.ProcessId;
            if (currentProcessId != null)
                _currentProcessIsLinkedToATask = await prepareService.ProcessIsLinkedToATask(currentProcessId.Value);
            bool keepPredicate(Procedure p)
            {
                if (p.ProcessId == currentProcessId)
                    return false; // On ne garde pas le process du scénario en cours
                if (p.LastScenarioHasLinkedProcess)
                    return false; // On ne garde pas les process ayant déjà des process liés (limite d'imbrication à 1)
                if (p.Projects.Any())
                {
                    var lastProject = p.Projects.MaxWithValue(_ => _.CreationDate);
                    if (lastProject.Scenarios.Any())
                        return true;
                    return false; // On ne garde pas les process dont le dernier projet n'a pas de scénarios
                }
                return false; // On ne garde pas les process sans projets, donc sans scénarios
            }
            TrackableCollection<ProjectDir> folders = new TrackableCollection<ProjectDir>(projectsData.ProjectsTree.OfType<ProjectDir>());
            foreach (ProjectDir folder in folders)
                TreeViewHelper.KeepOnly(folder, keepPredicate);
            TrackableCollection<Procedure> processes = new TrackableCollection<Procedure>(projectsData.ProjectsTree.OfType<Procedure>());
            processes.RemoveWhere(p => !keepPredicate(p));
            root.Childs = folders;
            root.Processes = processes;
            AllProcesses = new BulkObservableCollection<INode> { root };

            IReferentialsUseService referentialsUseService = IoC.Resolve<IReferentialsUseService>();

            CustomFieldsLabels = new CustomFieldsLabels(data.CustomFieldsLabels);

            // Toujours actif
            Categories.AddRange(data.Categories);
            AddLabelSortDescription(Categories);
            ObjectWithChangeTrackerExtensions.StartTracking(Categories);
            Skills.AddRange(data.Skills);
            AddLabelSortDescription(Skills);
            ObjectWithChangeTrackerExtensions.StartTracking(Skills);

            if (referentialsUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref1))
            {
                Ref1s.AddRange(data.Ref1s);
                ObjectWithChangeTrackerExtensions.StartTracking(Ref1s);
                Ref1s.CollectionChanged += RefsOnCollectionChanged;
            }
            if (referentialsUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref2))
            {
                Ref2s.AddRange(data.Ref2s);
                ObjectWithChangeTrackerExtensions.StartTracking(Ref2s);
                Ref2s.CollectionChanged += RefsOnCollectionChanged;
            }
            if (referentialsUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref3))
            {
                Ref3s.AddRange(data.Ref3s);
                ObjectWithChangeTrackerExtensions.StartTracking(Ref3s);
                Ref3s.CollectionChanged += RefsOnCollectionChanged;
            }
            if (referentialsUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref4))
            {
                Ref4s.AddRange(data.Ref4s);
                ObjectWithChangeTrackerExtensions.StartTracking(Ref4s);
                Ref4s.CollectionChanged += RefsOnCollectionChanged;
            }
            if (referentialsUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref5))
            {
                Ref5s.AddRange(data.Ref5s);
                ObjectWithChangeTrackerExtensions.StartTracking(Ref5s);
                Ref5s.CollectionChanged += RefsOnCollectionChanged;
            }
            if (referentialsUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref6))
            {
                Ref6s.AddRange(data.Ref6s);
                ObjectWithChangeTrackerExtensions.StartTracking(Ref6s);
                Ref6s.CollectionChanged += RefsOnCollectionChanged;
            }
            if (referentialsUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref7))
            {
                Ref7s.AddRange(data.Ref7s);
                ObjectWithChangeTrackerExtensions.StartTracking(Ref7s);
                Ref7s.CollectionChanged += RefsOnCollectionChanged;
            }

            // Toujours actif
            Operators.AddRange(data.Resources.OfType<Operator>());
            AddLabelSortDescription(Operators);
            ObjectWithChangeTrackerExtensions.StartTracking(Operators);

            // Toujours actif
            Equipments.AddRange(data.Resources.OfType<Equipment>());
            AddLabelSortDescription(Equipments);
            ObjectWithChangeTrackerExtensions.StartTracking(Equipments);

            RegisterAllMultipleReferentialsChanged();

            Videos = data.Videos;
            ObjectWithChangeTrackerExtensions.StartTracking(Videos);


            _scenarios = data.Scenarios;

            if (projectService.CurrentScenario == null)
                projectService.SelectScenario(_scenarios.First());
            else
                TryLoadScenario(projectService.CurrentScenario.Id);

            HideSpinner();
        }

        void RefsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                CanChange = false;
            else if (e.Action == NotifyCollectionChangedAction.Remove && CurrentActionItem.Action.IsMarkedAsUnchanged)
                CanChange = true;
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
            Videos = null;

            int projectId = ServiceBus.Get<IProjectManagerService>().CurrentProject.ProjectId;
            await LoadData(projectId);
        }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override async Task OnInitializeDesigner()
        {
            await base.OnInitializeDesigner();
            ChangeView(GanttGridView.WBS);

            Videos = DesignData.GenerateVideos().ToArray();

            CurrentScenarioInternal = DesignData.GenerateScenarioWithActions(
                DesignData.GenerateResources(),
                DesignData.GenerateActionCategories().Categories,
                DesignData.GenerateSkills(),
                Videos);

            ActionsManager.RegisterInitialActions(CurrentScenarioInternal.Actions);

            CurrentGridItem = (ActionGridItem)ActionItems.First();

            Ref2s.AddRange(DesignData.GenerateRef2s());
            Ref4s.AddRange(DesignData.GenerateRef4s());
            Ref3s.AddRange(DesignData.GenerateRef3s());
            IEnumerable<Resource> resources = DesignData.GenerateResources();
            Operators.AddRange(resources.OfType<Operator>());
            Equipments.AddRange(resources.OfType<Equipment>());
            Ref1s.AddRange(DesignData.GenerateRef1s());

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

            IsMarkersLinkedModeEnabled = true;
        }

        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la navigation est acceptée.
        /// </returns>
        public override Task<bool> OnNavigatingAway(IFrameNavigationToken token)
        {
            if (ServiceBus.IsAvailable<IMediaPlayerService>())
                EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.UnactivateMediaPlayerService));
            if (!base.CanChangeAndHasNoPendingChanges())
            {
                MessageDialogResult answer = this.AskNavigatingAwayValidationConfirmation(() => _navigationToken = token);
                if ((answer == MessageDialogResult.No || answer == MessageDialogResult.Yes) && _annotations)
                {
                    EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.HideAnnotations));
                    _annotations = false;
                }
                return Task.FromResult(answer == MessageDialogResult.No);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="GanttGridViewModelBase{TViewModel, TIViewModel, TComponentItem, TActionItem, TResourceItem}.IsReadOnly"/> a changé.
        /// </summary>
        protected override void OnIsReadOnlyChanged()
        {
            base.OnIsReadOnlyChanged();
            OnPropertyChanged(nameof(AreTimingsReadOnly));
        }

        /// <summary>
        /// Change la vue.
        /// </summary>
        /// <param name="view">La vue.</param>
        protected override void ChangeView(GanttGridView view)
        {
            base.ChangeView(view);
            OnPropertyChanged(nameof(AreTimingsVisible));
        }

        /// <summary>
        /// Appelé pour nettoyer les ressources managées et non managées.
        /// </summary>
        protected override void OnCleanup()
        {
            base.OnCleanup();
            ActionsManager.ActionVideoTimingChanged -= OnActionVideoTimingChanged;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient la taille maximale des vignettes.
        /// </summary>
        protected int ThumbnailMaxSize
        {
            get
            {
                if (!_thumbnailMaxSize.HasValue)
                {
                    int size;

                    string val = System.Configuration.ConfigurationManager.AppSettings["ThumbnailMaxSize"];
                    if (val != null)
                    {
                        if (!int.TryParse(val, out size))
                            size = DefaultThumbnailMaxSize;
                    }
                    else
                        size = DefaultThumbnailMaxSize;

                    _thumbnailMaxSize = size;
                }

                return _thumbnailMaxSize.Value;
            }
        }

        /// <summary>
        /// Obtient les vidéos.
        /// </summary>
        public Video[] Videos
        {
            get { return _videos; }
            private set
            {
                if (_videos != value)
                {
                    _videos = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit la vidéo courante.
        /// </summary>
        public Video CurrentVideo
        {
            get { return _currentVideo; }
            set
            {
                if (_currentVideo != value)
                {
                    _currentVideo = value;
                    OnPropertyChanged();
                    OnCurrentVideoChanged();
                }
            }
        }

        /// <summary>
        /// Obtient la position dans la vidéo courante.
        /// </summary>
        public long CurrentVideoPosition
        {
            get { return _currentVideoPosition; }
            set
            {
                if (_currentVideoPosition != value)
                {
                    long oldValue = _currentVideoPosition;
                    _currentVideoPosition = value;
                    OnPropertyChanged();
                    if (!_currentVideoPositionIsChanging)
                    {
                        OnCurrentVideoPositionChanged(oldValue, value);
                        _currentVideoPositionIsChanging = false;
                    }
                }
            }
        }

        /// <summary>
        /// Obtient les marqueurs à afficher.
        /// </summary>
        public ActionGridItem[] CurrentMarkers
        {
            get { return _currentMarkers; }
            private set
            {
                if (_currentMarkers != value && !GenericComparer.SequenceEqualWithNull(_currentMarkers, value))
                {
                    _currentMarkers = value;
                    OnPropertyChanged();
                    OnCurrentMarkersChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le marqueur sélectionné.
        /// </summary>
        public ActionGridItem SelectedMarker
        {
            get { return _selectedMarker; }
            set
            {
                if (_selectedMarker != value && !_ignoreMarkerSelection)
                {
                    _selectedMarker = value;
                    OnPropertyChanged();
                    OnSelectedMarkerChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit tous les process.
        /// </summary>
        public BulkObservableCollection<INode> AllProcesses
        {
            get { return _allProcesses; }
            set
            {
                if (_allProcesses != value)
                {
                    _allProcesses = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le noeud.
        /// </summary>
        public INode CurrentNode
        {
            get { return _currentNode; }
            set
            {
                if (_currentNode != value)
                {
                    if (_currentNode != null)
                        _currentNode.IsSelected = false;

                    _currentNode = value;

                    if (_currentNode == null)
                    {
                        CurrentTreeViewFolder = null;
                        CurrentTreeViewProcess = null;
                    }
                    else if (_currentNode is ProjectDir projectDir)
                    {
                        CurrentTreeViewFolder = projectDir?.Id == -1 ? null : projectDir;
                        CurrentTreeViewProcess = null;
                    }
                    else if (_currentNode is Procedure process)
                    {
                        CurrentTreeViewFolder = null;
                        CurrentTreeViewProcess = process;
                    }

                    if (_currentNode != null)
                        _currentNode.IsSelected = true;

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le répertoire.
        /// </summary>
        public ProjectDir CurrentTreeViewFolder
        {
            get { return _currentTreeViewFolder; }
            set
            {
                if (_currentTreeViewFolder != value)
                {
                    ProjectDir previous = _currentTreeViewFolder;
                    _currentTreeViewFolder = value;
                    OnPropertyChanged();
                    OnCurrentTreeViewFolderChanged(previous, _currentTreeViewFolder);
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le process.
        /// </summary>
        public Procedure CurrentTreeViewProcess
        {
            get { return _currentTreeViewProcess; }
            set
            {
                if (_currentTreeViewProcess != value)
                {
                    Procedure previous = _currentTreeViewProcess;
                    _currentTreeViewProcess = value;
                    if (CurrentActionItem.Action.IsMarkedAsAdded && value != null)
                        CurrentActionItem.Action.Label = value.Label;
                    OnPropertyChanged();
                    OnCurrentTreeViewProcessChanged(previous, _currentTreeViewProcess);
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
        /// Obtient les libellés d'actions déjà utilisés.
        /// </summary>
        public BulkObservableCollection<string> ActionsLabels { get; private set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant la saisie est à la volée.
        /// </summary>
        public bool AutoPause
        {
            get { return _autoPause; }
            set
            {
                if (_autoPause != value)
                {
                    _autoPause = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si la saisie est manuelle (sans vidéo associée).
        /// </summary>
        public bool IsManualInput =>
            CurrentVideo == null && (CurrentActionItem != null && !CurrentActionItem.IsGroup.GetValueOrDefault(false));

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la vue associée est chargée.
        /// </summary>
        public bool IsViewLoaded { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les markers doivent être liés lors de leurs déplacements.
        /// </summary>
        public bool AreMarkersLinked
        {
            get { return _areMarkersLinked; }
            set
            {
                if (_areMarkersLinked != value)
                {
                    _areMarkersLinked = value;
                    OnPropertyChanged();
                    ActionsManager.AreMarkersLinked = value;
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

        /// <summary>
        /// Obtient une valeur indiquant si la vidéo d'une action peut être changée à tout moment.
        /// </summary>
        public virtual bool CanChangeActionVideo =>
            !IsReadOnlyForCurrentUser && (CurrentGridItem == null || (CurrentActionItem != null && !CurrentActionItem.IsGroup.GetValueOrDefault()));

        /// <summary>
        /// Obtient une valeur indiquant si les timings (début/durée/fin) sont en lecture seule.
        /// </summary>
        public bool AreTimingsReadOnly =>
            IsReadOnly || (CurrentActionItem != null && CurrentActionItem.IsGroup.GetValueOrDefault(true));

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les timings (début/durée/fin) sont affichés.
        /// </summary>
        public bool AreTimingsVisible
        {
            get
            {
                if (View != GanttGridView.WBS && CurrentActionItem != null && CurrentActionItem.IsGroup.GetValueOrDefault(false))
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'action courante a une vignette.
        /// </summary>
        public bool HasThumbnail
        {
            get { return _hasThumbnail; }
            private set
            {
                if (_hasThumbnail != value)
                {
                    _hasThumbnail = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commandes

        Command _unselectItemCommand;
        /// <summary>
        /// Obtient la commande permettant de faire un Play sur le player
        /// </summary>
        public ICommand UnselectItemCommand
        {
            get
            {
                if (_unselectItemCommand == null)
                    _unselectItemCommand = new Command(() =>
                    {
                        CurrentGridItem = null;
                    },
                    () =>
                    {
                        if (isMoving)
                            return false;
                        if (IsReadOnly)
                            return false;
                        if (_annotations)
                            return false;
                        if (CurrentActionItem != null && CurrentActionItem.Action.IsMarkedAsAdded)
                            return false;
                        if (!ServiceBus.IsAvailable<IMediaPlayerService>())
                            EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.ActivateMediaPlayerService));
                        IMediaPlayerService service = ServiceBus.Get<IMediaPlayerService>();
                        return !service.IsPlaying();
                    });
                return _unselectItemCommand;
            }
        }

        Command<MediaPlayerAction> _mediaPlayerControlCommand;
        /// <summary>
        /// Obtient la commande permettant de faire un Play sur le player
        /// </summary>
        public ICommand MediaPlayerControlCommand
        {
            get
            {
                if (_mediaPlayerControlCommand == null)
                    _mediaPlayerControlCommand = new Command<MediaPlayerAction>(action =>
                    {
                        // On réinitialise les statuts des curseurs de lecture de l'ancienne action avant la lecture de la nouvelle
                        _timelinesEndReached = false;
                        if (CurrentActionItem != null)
                            CurrentActionItem.Action.CanModifyFinish = false;

                        EventBus.Publish(new MediaPlayerActionEvent(this, action));
                    },
                    action => CurrentActionItem != null && !CurrentActionItem.Action.IsGroup);
                return _mediaPlayerControlCommand;
            }
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande AddCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnAddCommandCanExecute() =>
            CanChange && !IsReadOnly;

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande AddCommand
        /// </summary>
        protected override void OnAddCommandExecute()
        {
            isAdding = true;

            // Si l'action courante est nulle, on doit sélectionner la dernière action de rang 1 avec la même vidéo si elle existe
            ActionGridItem currentAction = CurrentActionItem;
            if (CurrentActionItem == null)
            {
                IEnumerable<ActionGridItem> rootActions = ActionGridItems.Where(a => a.Action.WBSParts.Length == 1 && a.Action.VideoId == CurrentVideo?.VideoId);
                if (rootActions.Any())
                    currentAction = rootActions.MaxWithValue(a => a.Action.WBS);
            }
            AddAction(currentAction);
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande RemoveCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnRemoveCommandCanExecute() =>
            CurrentActionItem != null && CanChange && !IsReadOnly;

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande RemoveCommand
        /// </summary>
        protected override async void OnRemoveCommandExecute() =>
            await DeleteAction();

        protected override bool OnDuplicateTaskCommandCanExecute()
        {
            if (isAdding)
                return false;
            if (isAddingAsChild)
                return false;
            if (isDuplicating)
                return false;
            if (isMoving)
                return false;
            if (isGrouping)
                return false;
            if (IsReadOnly)
                return false;
            if (_annotations)
                return false;
            if (ViewContainer.Label != "ID")
                return false;
            //WBSHelper.IndentationFromWBS();
            if (SelectedItems.OfType<ActionGridItem>().Count() > 1)
            {
                int? lastWBSIndent = null;
                foreach (ActionGridItem item in SelectedItems.OfType<ActionGridItem>())
                {
                    if (lastWBSIndent != null && lastWBSIndent != WBSHelper.IndentationFromWBS(item.Action.WBS))
                    {
                        _isSameIndent = false;
                        break;
                    }
                    lastWBSIndent = WBSHelper.IndentationFromWBS(item.Action.WBS);
                }
            }
            else
                _isSameIndent = true;


            return CurrentGridItem != null && _isSameIndent;
        }

        protected override void OnDuplicateTaskCommandExecute()
        {
            isDuplicating = true;
            _isDuplicating = true;
            DuplicateTask();
            Application.Current.MainWindow.Focus();
        }

        protected  bool ValidateCommandIsRunning { get; set; }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande ValidateCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnValidateCommandCanExecute() =>
            !CanChange && !IsReadOnly && !ValidateCommandIsRunning;

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande CancelCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnCancelCommandCanExecute() =>
            !CanChange && CurrentGridItem != null;

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande CancelCommand
        /// </summary>
        protected override void OnCancelCommandExecute()
        {
            _isCanceling = true;

            // Correction pour la compétence
            if (CurrentActionItem != null && CurrentActionItem.Action.ChangeTracker.ModifiedValues.ContainsKey(nameof(KAction.LinkedProcessId)))
            {
                object originalLinkedProcessId = CurrentActionItem.Action.ChangeTracker.OriginalValues[nameof(KAction.LinkedProcessId)];
                Procedure processInTree = originalLinkedProcessId == null ? null : TreeViewHelper.FindProcess(AllProcesses[0], (int)originalLinkedProcessId );
                CurrentNode = processInTree;
            }
            // Correction pour la compétence

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

            _isDuplicating = false;
            if (CurrentActionItem != null)
                CurrentActionItem.Action.CanModifyFinish = false;

            if (ServiceBus.IsAvailable<IMediaPlayerService>())
                EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.UnactivateMediaPlayerService));
            if (_annotations)
            {
                EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.HideAnnotations));
                _annotations = false;
            }
            HideValidationErrors();
            DeleteCurrentCreatingReferential();
            EndCurrentCreatingReferential();
            UnregisterAllMultipleReferentialsChanged();

            ActionsManager.UnregisterAllItems();

            ActionGridItem previousSelection = CurrentActionItem;
            long? previousVideoPosition = null;
            KAction parent = null;

            if (previousSelection?.Action.IsMarkedAsAdded == true)
            {
                if (previousSelection.CreationPredecessor == null)
                    previousVideoPosition = previousSelection.Action.Start;
                parent = WBSHelper.GetParent(previousSelection.Action, CurrentScenarioInternal.Actions);

                previousSelection = (ActionGridItem)previousSelection.CreationPredecessor;
            }

            // Détecter les entités nouvelles et les supprimer
            KAction[] newActions = ActionGridItems.Select(i => i.Action).Distinct().Where(a => a.IsMarkedAsAdded).ToArray();
            foreach (KAction action in newActions)
            {
                ActionsManager.DeleteAction(action);
                CurrentScenarioInternal.Actions.Remove(action);
            }

            Categories?.RemoveWhere(e => e.IsMarkedAsAdded);
            Skills?.RemoveWhere(e => e.IsMarkedAsAdded);
            Equipments?.RemoveWhere(e => e.IsMarkedAsAdded);
            Operators?.RemoveWhere(e => e.IsMarkedAsAdded);
            Ref1s?.RemoveWhere(e => e.IsMarkedAsAdded);
            Ref2s?.RemoveWhere(e => e.IsMarkedAsAdded);
            Ref3s?.RemoveWhere(e => e.IsMarkedAsAdded);
            Ref4s?.RemoveWhere(e => e.IsMarkedAsAdded);
            Ref5s?.RemoveWhere(e => e.IsMarkedAsAdded);
            Ref6s?.RemoveWhere(e => e.IsMarkedAsAdded);
            Ref7s?.RemoveWhere(e => e.IsMarkedAsAdded);
            Videos.RemoveWhere(e => e.IsMarkedAsAdded);

            // Désactiver le tracking volontairement sur tous les ActionLinks qui ont été supprimés
            // En effet, ils ne sont actuellement liés à aucune tâche, et l'annulation entraînera une "réinsertion" de ceux-ci.
            // Cette réinsertion sera par défaut sur des actions links avec le tracking activé, ce qui aura pour effet de tracker tous les changements effectués par l'annulation.
            // En désactivant le tracking, le CancelChanges() effectué dans CancelChangesActionReferentialsAndFixupLinks permettra de restaurer correctement l'état initial.

            string[] properties =
            {
                nameof(KAction.Ref1),
                nameof(KAction.Ref2),
                nameof(KAction.Ref3),
                nameof(KAction.Ref4),
                nameof(KAction.Ref5),
                nameof(KAction.Ref6),
                nameof(KAction.Ref7)
            };
            foreach (IObjectWithChangeTracker action in CurrentScenarioInternal.Actions.Cast<IObjectWithChangeTracker>())
            {
                foreach (string prop in properties)
                {
                    if (action.ChangeTracker.ObjectsRemovedFromCollectionProperties.TryGetValue(prop, out ObjectList collection))
                    {
                        foreach (IObjectWithChangeTracker element in collection)
                            element.StopTracking();
                    }
                }
            }

            ObjectWithChangeTrackerExtensions.CancelChanges(
                Categories ?? Enumerable.Empty<ActionCategory>(),
                Skills ?? Enumerable.Empty<Skill>(),
                Operators ?? Enumerable.Empty<Operator>(),
                Equipments ?? Enumerable.Empty<Equipment>(),
                Ref1s ?? Enumerable.Empty<Ref1>(),
                Ref2s ?? Enumerable.Empty<Ref2>(),
                Ref3s ?? Enumerable.Empty<Ref3>(),
                Ref4s ?? Enumerable.Empty<Ref4>(),
                Ref5s ?? Enumerable.Empty<Ref5>(),
                Ref6s ?? Enumerable.Empty<Ref6>(),
                Ref7s ?? Enumerable.Empty<Ref7>(),
                Videos,
                CurrentScenarioInternal.Actions
                );

            if (previousSelection != null)
                CancelChangesActionReferentialsAndFixupLinks(previousSelection.Action);

            if (parent != null)
                CancelChangesActionReferentialsAndFixupLinks(parent);

            RebuildItems(previousSelection);

            if (previousVideoPosition.HasValue && previousSelection != null)
                CurrentVideoPosition = previousVideoPosition.Value;

            UpdateActionsLabels();
            RegisterAllMultipleReferentialsChanged();

            CanChange = true;
            EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Pause));
            _timelinesEndReached = false;

            // TODO : Replace the CancelChanges globally
            // Get all 0..1 relationships of the entity, set the relation property to null and mark it as unchanged
            if (isDuplicating)
            {
                var internalChanges = CurrentScenarioInternal.GetChanges().Where(_ => _.IsMarkedAsAdded).ToArray();
                foreach (var change in internalChanges)
                {
                    if (change is KAction kAction)
                    {
                        kAction.Scenario = null;
                        kAction.Resource = null;
                        kAction.Category = null;
                        kAction.Original = null;
                        kAction.Video = null;
                        kAction.Creator = null;
                        kAction.LastModifier = null;
                        kAction.Thumbnail = null;
                        kAction.LinkedProcess = null;
                        kAction.Skill = null;
                    }
                }
            }
            var changes = CurrentScenarioInternal.GetChanges();
            foreach (var change in changes)
                change.MarkAsUnchanged();

            isAdding = false;
            isAddingAsChild = false;
            isDuplicating = false;
            isMoving = false;
            isGrouping = false;

            _isCanceling = false;
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande ValidateCommand
        /// </summary>
        protected async override void OnValidateCommandExecute()
        {
            ValidateCommandIsRunning = true;

            if (CurrentActionItem != null)
                CurrentActionItem.Action.CanModifyFinish = false;
            EventBus.Publish(new UIBindingSynchronizationRequested(this));
            EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Pause));
            EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.ActivateMediaPlayerService));

            //Save thumbnail if needs
            if (_annotations)
            {
                await UpdateThumbnail(CurrentActionItem.Action, CurrentVideoPosition, true);
                //EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.UnactivateMediaPlayerService));
                EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.HideAnnotations));
                _annotations = false;
            }

            ValidateCurrentCreatingReferential();
            // Valider l'objet
            if (!ValidateActions())
            {
                ValidateCommandIsRunning = false;

                isAdding = false;
                isAddingAsChild = false;
                isDuplicating = false;
                isMoving = false;
                isGrouping = false;

                return;
            }

            ActionGridItem currentAction = CurrentActionItem;
            bool wasNew = currentAction?.Action.IsMarkedAsAdded ?? false;
            bool isGroup = currentAction?.IsGroup.GetValueOrDefault() ?? false;

            if (wasNew)
            {
                // Définir les Build timings
                if (CurrentActionItem.CreationPredecessor != null)
                {
                    // Créer un lien Prédecesseur/Sucesseur
                    if (CurrentActionItem.Action.Start == CurrentActionItem.CreationPredecessor.Action.Finish ||
                        CurrentActionItem.Action.Video == null && CurrentActionItem.CreationPredecessor.Action.Video == null)
                    {
                        ActionsManager.AddPredecessor(CurrentActionItem, (ActionGridItem)CurrentActionItem.CreationPredecessor);
                        CurrentActionItem.Action.BuildStart = CurrentActionItem.CreationPredecessor.Action.BuildFinish;
                    }
                    else
                        CurrentActionItem.Action.BuildStart = 0;
                }
            }

            // Pour les scénarios Initiaux et de Validation, faire correspondre les timings Vidéo et les timings Process via le jugement d'allure
            // OU dans le cas où la tâche n'a pas d'origine (créée dans un scenario cible)

            foreach (KAction action in CurrentScenarioInternal.Actions.Where(a => a.IsMarkedAsModified))
            {
                if (CurrentScenarioInternal.NatureCode == KnownScenarioNatures.Initial ||
                    CurrentScenarioInternal.NatureCode == KnownScenarioNatures.Realized ||
                    action.Original == null)
                {
                    ActionsTimingsMoveManagement.GetOrignalModifiedVideoDurations(action, out long originalDuration, out long modifiedDuration);

                    if (originalDuration != modifiedDuration)
                    {
                        double paceRating = action.Resource?.PaceRating ?? 1d;
                        action.BuildDuration = Convert.ToInt64(modifiedDuration * paceRating);

                        if (action.Original == null)
                        {
                            action.Reduced.StartTracking();
                            action.Reduced.OriginalBuildDuration = action.BuildDuration;
                            ActionsTimingsMoveManagement.UpdateTimingsFromReducedReduction(action);
                        }
                    }
                }
            }

            if (CurrentActionItem != null)
                CurrentActionItem.CreationPredecessor = null;

            bool skipActionAutoCreation = _isDuplicating;
            // Si la nouvelle action est positionnée à la fin du timeline narrow, la sous décompo de la tâche est finie et on n'en crèe pas de nouveau
            if (_subTaskCreationTimelineNarrow != null && currentAction.Action.Finish >= _subTaskCreationTimelineNarrow.Finish)
            {
                _subTaskCreationTimelineNarrow = null;
                _timelinesEndReached = false;
                skipActionAutoCreation = true;
            }
            skipActionAutoCreation |= (_currentVideo != null && _currentVideoPosition >= _currentVideo.Duration);

            // Ne pas créer une nouvelle tâche si on sauvegarde un groupe
            skipActionAutoCreation |= isGroup;

            // Supprimer les référentiels des groupes
            foreach (ActionGridItem item in ActionGridItems)
            {
                if (item.IsGroup.GetValueOrDefault())
                    ClearAllMultipleReferentials(item.Action);
            }

            // Mettre à jour les vignettes
            if (!_isDuplicating)
            {
                foreach (KAction action in CurrentScenarioInternal.Actions)
                {
                    if (action.IsMarkedAsAdded || action.IsMarkedAsModified)
                    {
                        if (action.Video == null)
                            action.ThumbnailPosition = null;
                    }
                }
            }

            if (_isDuplicating)
                _isDuplicating = false;

            async void onFinished()
            {
                if (await SaveActions(!wasNew || skipActionAutoCreation))
                {
                    currentAction = CurrentActionItem;
                    CanChange = true;
                    // seules les actions nouvelles entraineront un changement
                    if (wasNew)
                    {
                        // Si les marqueurs ne sont pas liés, reprendre la lecture de la vidéo et sélectionnant la tâche précédemment ajoutée
                        if (!AreMarkersLinked)
                        {
                            CurrentGridItem = ActionGridItems.SingleOrDefault(_ => _.Action.ActionId == currentAction.Action.ActionId);
                            if (CurrentVideo != null)
                            {
                                EventBus.Publish(new MediaPlayerActionEvent(this, () =>
                                {
                                    _currentVideoPosition = currentAction.Action.Finish;
                                    OnPropertyChanged(nameof(CurrentVideoPosition));
                                }));
                                _justCreatingNewActionWithMarkersUnlinked = true;
                                EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Play) { SendWhenPlayerDispatcherReady = true });
                            }
                        }
                        // Sinon, sauf contre indication, démarrer l'ajout d'une nouvelle tâche
                        else if (!skipActionAutoCreation)
                            AddAction(currentAction);
                    }

                    if (_navigationToken?.IsValid == true)
                    {
                        _navigationToken.Navigate();
                        _navigationToken = null;
                    }

                    ValidateCommandIsRunning = false;

                    isAdding = false;
                    isAddingAsChild = false;
                    isDuplicating = false;
                    isMoving = false;
                    isGrouping = false;
                }
            }

            onFinished();

            EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.UnactivateMediaPlayerService));
        }

        Command<object> _addReferentialCommand;
        /// <summary>
        /// Obtient la commande permettant d'ajouter un nouveau référentiel.
        /// </summary>
        public ICommand AddReferentialCommand
        {
            get
            {
                if (_addReferentialCommand == null)
                    _addReferentialCommand = new Command<object>(source =>
                    {
                        if (source == Categories)
                            CreateNewCategory();
                        else if (source == Skills)
                            CreateNewSkill();
                        else if (source == Operators)
                            CreateNewOperator();
                        else if (source == Equipments)
                            CreateNewEquipment();
                        else if (source == Ref1s)
                            CreateNewReferential<Ref1, Ref1>(Ref1s, a => a.Ref1);
                        else if (source == Ref2s)
                            CreateNewReferential<Ref2, Ref2>(Ref2s, a => a.Ref2);
                        else if (source == Ref3s)
                            CreateNewReferential<Ref3, Ref3>(Ref3s, a => a.Ref3);
                        else if (source == Ref4s)
                            CreateNewReferential<Ref4, Ref4>(Ref4s, a => a.Ref4);
                        else if (source == Ref5s)
                            CreateNewReferential<Ref5, Ref5>(Ref5s, a => a.Ref5);
                        else if (source == Ref6s)
                            CreateNewReferential<Ref6, Ref6>(Ref6s, a => a.Ref6);
                        else if (source == Ref7s)
                            CreateNewReferential<Ref7, Ref7>(Ref7s, a => a.Ref7);
                    }, source => CurrentActionItem != null);
                return _addReferentialCommand;
            }
        }

        Command _unlinkVideoCommand;
        /// <summary>
        /// Obtient la commande permettant de délier l'action courante de la vidéo.
        /// </summary>
        public ICommand UnlinkVideoCommand
        {
            get
            {
                if (_unlinkVideoCommand == null)
                    _unlinkVideoCommand = new Command(() =>
                    {
                        CurrentVideo = null;
                    },
                    () => CurrentVideo != null && !IsReadOnly);
                return _unlinkVideoCommand;
            }
        }

        Command<DataTreeGrid> _collapseAllCommand;
        /// <summary>
        /// Obtient la commande permettant de réduire le DataTreeGrid.
        /// </summary>
        public ICommand CollapseAllCommand
        {
            get
            {
                if (_collapseAllCommand == null)
                    _collapseAllCommand = new Command<DataTreeGrid>((dataTreeGrid) =>
                    {
                        dataTreeGrid.CollapseAll();
                    },
                    (dataTreeGrid) => !_annotations && AreMarkersLinked && !isAdding && !isAddingAsChild && !isDuplicating && !isMoving && !isGrouping);
                return _collapseAllCommand;
            }
        }

        Command<DataTreeGrid> _expandAllCommand;
        /// <summary>
        /// Obtient la commande permettant d'étendre le DataTreeGrid.
        /// </summary>
        public ICommand ExpandAllCommand
        {
            get
            {
                if (_expandAllCommand == null)
                    _expandAllCommand = new Command<DataTreeGrid>((dataTreeGrid) =>
                    {
                        dataTreeGrid.ExpandAll();
                    },
                    (dataTreeGrid) => !_annotations && AreMarkersLinked && !isAdding && !isAddingAsChild && !isDuplicating && !isMoving && !isGrouping);
                return _expandAllCommand;
            }
        }

        Command _groupCommand;
        /// <summary>
        /// Obtient la commande permettant de grouper les éléments.
        /// </summary>
        public ICommand GroupCommand
        {
            get
            {
                if (_groupCommand == null)
                    _groupCommand = new Command(() =>
                    {
                        isGrouping = true;
                        IEnumerable<ActionGridItem> selection = SelectedItems.OfType<ActionGridItem>();
                        if (ActionsManager.CanGroup(selection))
                        {
                            // Annuler le narrow sur la tâche en cours de lecture
                            _taskPlayTimelineNarrow = null;
                            EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Pause));

                            ActionGridItem newAction = ActionsManager.Group(selection);
                            newAction.Action.Scenario = CurrentScenarioInternal;
                            newAction.Action.Resource = selection.First().Action.Resource;

                            RegisterToStateChanged(newAction.Action);

                            // Focus le champ par défaut
                            EventBus.Publish(new FocusDefaultFieldWhenCreatingEvent(this));
                        }
                    },
                    () => !_annotations && !IsReadOnly && SelectedItems.Count > 0 && !_isDuplicating && AreMarkersLinked && !isAdding && !isAddingAsChild && !isMoving && !isGrouping);
                return _groupCommand;
            }
        }

        Command _ungroupCommand;
        /// <summary>
        /// Obtient la commande permettant de dégrouper un groupe.
        /// </summary>
        public ICommand UngroupCommand
        {
            get
            {
                if (_ungroupCommand == null)
                    _ungroupCommand = new Command(async () =>
                    {
                        isGrouping = true;
                        try
                        {
                            // Annuler le narrow sur la tâche en cours de lecture
                            _taskPlayTimelineNarrow = null;
                            EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Pause));

                            await Ungroup(CurrentActionItem);
                            OnActionHierarchyChanged();
                        }
                        catch(Exception ex)
                        {
                            OnError(ex);
                        }
                        finally
                        {
                            isGrouping = false;
                        }
                    },
                    () => !_annotations && CurrentActionItem != null && CurrentActionItem.IsGroup.GetValueOrDefault() && View == GanttGridView.WBS && !IsReadOnly && !_isDuplicating && !isGrouping && AreMarkersLinked);
                return _ungroupCommand;
            }
        }

        /// <summary>
        /// Appelé afin de déterminer la commande MoveDown peut être exécutée.
        /// </summary>
        /// <returns><c>true</c> si la commande peut être executée.</returns>
        protected override bool OnCanMoveDown() =>
            !_annotations && !IsReadOnly && View == GanttGridView.WBS && !isAdding && !isAddingAsChild && !isDuplicating && !isGrouping;

        /// <summary>
        /// Appelé lorsque un déplacement vers le haut est demandé.
        /// </summary>
        protected override void OnMoveUp()
        {
            isMoving = true;
            ActionsManager.MoveUpAboveSibling(CurrentActionItem);
            OnActionHierarchyChanged();
        }

        /// <summary>
        /// Appelé afin de déterminer la commande MoveUp peut être exécutée.
        /// </summary>
        /// <returns><c>true</c> si la commande peut être executée.</returns>
        protected override bool OnCanMoveUp() =>
            !_annotations && !IsReadOnly && View == GanttGridView.WBS && !isAdding && !isAddingAsChild && !isDuplicating && !isGrouping;

        /// <summary>
        /// Appelé lorsque un déplacement vers le bas est demandé.
        /// </summary>
        protected override void OnMoveDown()
        {
            isMoving = true;
            ActionsManager.MoveDownBelowSibling(CurrentActionItem);
            OnActionHierarchyChanged();
        }

        Command _addAsChildCommand;
        /// <summary>
        /// Obtient la commande permettant d'ajouter une action en tant qu'enfant de l'élément sélectionné.
        /// </summary>
        public ICommand AddAsChildCommand
        {
            get
            {
                if (_addAsChildCommand == null)
                    _addAsChildCommand = new Command(() =>
                    {
                        isAddingAsChild = true;
                        AddActionAsChild(CurrentActionItem);
                    },
                    () => View == GanttGridView.WBS && CurrentActionItem != null && CurrentActionItem.Action.LinkedProcessId == null && !IsReadOnly && CanChange && AreMarkersLinked);
                return _addAsChildCommand;
            }
        }

        Command _addAsProcessCommand;
        /// <summary>
        /// Obtient la commande permettant d'ajouter une action en tant que process.
        /// </summary>
        public ICommand AddAsProcessCommand
        {
            get
            {
                if (_addAsProcessCommand == null)
                    _addAsProcessCommand = new Command(() =>
                    {
                        AddActionAsProcess(CurrentActionItem);
                    },
                    () => CanChange && !IsReadOnly && !_currentProcessIsLinkedToATask);
                return _addAsProcessCommand;
            }
        }

        Command _onPlayCommand;
        /// <summary>
        /// Obtient la commande exécutée lorsque la lecture commence sur le player.
        /// </summary>
        public ICommand OnPlayCommand
        {
            get
            {
                if (_onPlayCommand == null)
                    _onPlayCommand = new Command(() =>
                    {
                        _taskPlayTimelineNarrow = CreateTimelineNarrowForCurrentAction();
                        if (CurrentActionItem != null
                            && !CurrentActionItem.Action.CanModifyFinish
                            && CurrentVideoPosition == CurrentActionItem.Action.Finish
                            && !_justCreatingNewActionWithMarkersUnlinked)
                            CurrentActionItem.Action.CanModifyFinish = true;
                    });
                return _onPlayCommand;
            }
        }

        Command<object> _doubleClickCommand;
        /// <summary>
        /// Obtient la commande exécutée lorsqu'on double clique sur une action.
        /// </summary>
        public ICommand DoubleClickCommand
        {
            get
            {
                if (_doubleClickCommand == null)
                    _doubleClickCommand = new Command<object>((obj) =>
                    {
                        if (CurrentActionItem == null)
                            return;
                        CurrentActionItem.Action.CanModifyFinish = false;
                        EventBus.Publish(new MediaPlayerActionEvent(this, async (player) =>
                        {
                            if (player is KMediaPlayer mediaplayer)
                            {
                                while (mediaplayer.CurrentPosition != CurrentActionItem.Action.Start)
                                {
                                    await Task.Delay(10);
                                    mediaplayer.CurrentPosition = CurrentActionItem.Action.Start; // Reforce position
                                }
                                mediaplayer.Play();
                            }
                        }));
                    });
                return _doubleClickCommand;
            }
        }

        Command _onSeekCommand;
        /// <summary>
        /// Obtient la commande exécutée lorsque la position dans la vidéo a été déplacée.
        /// </summary>
        public ICommand OnSeekCommand
        {
            get
            {
                if (_onSeekCommand == null)
                    _onSeekCommand = new Command(() =>
                    {
                        _hasjustSeeked |= _taskPlayTimelineNarrow != null;
                    });
                return _onSeekCommand;
            }
        }

        Command _thumbnailCommand;
        /// <summary>
        /// Obtient la commande permettant de définir la vignette.
        /// </summary>
        public ICommand ThumbnailCommand
        {
            get
            {
                if (_thumbnailCommand == null)
                    _thumbnailCommand = new Command(async () =>
                    {
                        EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.ActivateMediaPlayerService));
                        if (CurrentActionItem.Action.Thumbnail != null)
                        {
                            CurrentActionItem.Action.Thumbnail = null;
                            CurrentActionItem.Action.IsThumbnailSpecific = true;
                            CurrentActionItem.Action.ThumbnailPosition = null;
                            UpdateHasThumbnail();
                            CommandManager.InvalidateRequerySuggested();
                        }
                        else if (CurrentVideo == null)
                        {
                            SetExternalThumbnail();
                            UpdateHasThumbnail();
                        }
                        else
                        {
                            if (CurrentVideoPosition < CurrentActionItem.Action.Start || CurrentVideoPosition > CurrentActionItem.Action.Finish)
                            {
                                DialogFactory.GetDialogView<IMessageDialog>().Show(
                                    LocalizationManager.GetString("VM_Acquire_Message_CannotSetThumbnailOutOfBounds"),
                                    LocalizationManager.GetString("Common_Error"),
                                    MessageDialogButton.OK,
                                    MessageDialogImage.Exclamation);
                                return;
                            }

                            ShowSpinner();
                            EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.ShowAnnotations));
                            _annotations = true;
                            await UpdateThumbnail(CurrentActionItem.Action, CurrentVideoPosition, true);
                            HideSpinner();
                            UpdateHasThumbnail();
                            CanChange = false;
                        }
                    }, () => CurrentActionItem != null && !IsReadOnly && !CurrentActionItem.IsGroup.GetValueOrDefault());
                return _thumbnailCommand;
            }
        }

        Command _thumbnailExternalCommand;
        /// <summary>
        /// Obtient la commande permettant de définir la vignette à partir d'un fichier externe.
        /// </summary>
        public ICommand ThumbnailExternalCommand
        {
            get
            {
                if (_thumbnailExternalCommand == null)
                    _thumbnailExternalCommand = new Command(() =>
                    {
                        EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.HideAnnotations));
                        if (CurrentActionItem.Action.Thumbnail != null)
                        {
                            _annotations = false;
                            CurrentActionItem.Action.Thumbnail = null;
                            CurrentActionItem.Action.IsThumbnailSpecific = true;
                            CurrentActionItem.Action.ThumbnailPosition = null;
                            UpdateHasThumbnail();
                            CommandManager.InvalidateRequerySuggested();
                        }
                        else
                        {
                            EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Pause));
                            SetExternalThumbnail();
                            UpdateHasThumbnail();
                        }
                        EventBus.Publish(new MediaPlayerActionEvent(this, (player) =>
                        {
                            if (player is KMediaPlayer mediaPlayer)
                                mediaPlayer.ShowThumbnailView(CurrentActionItem.Action);
                        }));
                    }, () => CurrentActionItem != null && !IsReadOnly );
                return _thumbnailExternalCommand;
            }
        }

        Command _screenModeCommand;
        /// <summary>
        /// Obtient la commande permettant de permutter le mode d'affichage de la vidéo.
        /// </summary>
        public ICommand ScreenModeCommand
        {
            get
            {
                if (_screenModeCommand == null)
                    _screenModeCommand = new Command(() =>
                    {
                        EventBus.Publish(new PlayerScreenModeChangeRequestedEvent(this));
                    });
                return _screenModeCommand;
            }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Tente de charger le scénario spécifié.
        /// </summary>
        /// <param name="scenarioId">L'identifiant du scénario.</param>
        void TryLoadScenario(int scenarioId)
        {
            Scenario old = CurrentScenarioInternal;
            CurrentScenarioInternal = _scenarios.FirstOrDefault(s => s.ScenarioId == scenarioId);
            if (CurrentScenarioInternal == null)
            {
                CurrentScenarioInternal = _scenarios.First();
                ServiceBus.Get<IProjectManagerService>().SelectScenario(_scenarios.First());
            }

            LoadCurrentScenario(old, CurrentScenarioInternal);
        }

        /// <summary>
        /// Charge les données d'un scénario.
        /// </summary>
        /// <param name="oldScenario">L'ancien scénario.</param>
        /// <param name="newScenario">Le nouveau scénario.</param>
        void LoadCurrentScenario(Scenario oldScenario, Scenario newScenario)
        {
            if (oldScenario == newScenario)
                return;

            ActionsManager.Clear();
            ActionItems.Clear();

            if (oldScenario != null)
            {
                oldScenario.StopTracking();
                foreach (KAction action in oldScenario.Actions)
                {
                    action.StopTracking();
                    UnregisterToStateChanged(action);
                }
            }

            if (newScenario != null)
            {
                ActionsManager.RegisterInitialActions(newScenario.Actions);

                newScenario.StartTracking();
                foreach (KAction action in newScenario.Actions)
                {
                    action.StartTracking();
                    RegisterToStateChanged(action);
                }

                RestoreActionSelection();
            }

            UpdateItemsEnabledState();
            UpdateIsReadOnly();
            UpdateActionsLabels();
            UpdateHasThumbnail();
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="CurrentVideoPosition"/> a changé.
        /// </summary>
        void OnCurrentVideoPositionChanged(long oldValue, long newValue)
        {
            if (_isCanceling) // On est en cours d'annulation
                return;

            _currentVideoPositionIsChanging = true;

            Slider slider = ((DependencyObject)Mouse.DirectlyOver).TryFindParent<Slider>();
            if (CurrentActionItem != null
                && Mouse.LeftButton == MouseButtonState.Pressed
                && slider?.Name == "PART_TimeLine")
            //Il s'agit d'un saut du curseur de lecture
            {
                CurrentActionItem.Action.CanModifyFinish = false;
                return;
            }
            if (CurrentActionItem != null
                            && Mouse.LeftButton == MouseButtonState.Pressed
                            && slider?.Name == "LowerSlider")
            {
                EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Pause));
                CurrentActionItem.Action.CanModifyFinish = false;
                return;
            }
            if (CurrentActionItem != null
                            && Mouse.LeftButton == MouseButtonState.Pressed
                            && slider?.Name == "UpperSlider")
            {
                _timelinesEndReached = false;
                if (_taskPlayTimelineNarrow != null && newValue > _taskPlayTimelineNarrow.Finish || _subTaskCreationTimelineNarrow != null) // Permet le déplacement manuel
                {
                    CurrentActionItem.Action.Finish = CurrentVideoPosition;
                    _timelinesEndReached = _subTaskCreationTimelineNarrow != null && CurrentVideoPosition >= _subTaskCreationTimelineNarrow.Finish;
                    return;
                }
            }

            if (CurrentActionItem != null &&
                ((Mouse.LeftButton == MouseButtonState.Pressed && Mouse.DirectlyOver.GetType() == typeof(RepeatButton)
                && (((RepeatButton)Mouse.DirectlyOver).Name == "PART_Next" || ((RepeatButton)Mouse.DirectlyOver).Name == "PART_Previous")) ||
                (Keyboard.IsKeyDown(Key.F2) || Keyboard.IsKeyDown(Key.F4))))
            //Il s'agit d'un appui sur un bouton
            {
                EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Pause));
                CurrentActionItem.Action.CanModifyFinish = false;
                _timelinesEndReached = false;
                if (Math.Min(oldValue, newValue) <= CurrentActionItem.Action.Start && CurrentActionItem.Action.Start <= Math.Max(oldValue, newValue)
                    && Math.Min(oldValue, newValue) <= CurrentActionItem.Action.Finish && CurrentActionItem.Action.Finish <= Math.Max(oldValue, newValue))
                {
                    if (oldValue < newValue) // On avance
                    {
                        if (_startAndFinishAreSame && _lastCursor == Cursors.Finish)
                            CurrentActionItem.Action.Finish = newValue;
                        else
                            CurrentActionItem.Action.Start = newValue;
                    }
                    else if (oldValue > newValue) // On recule
                    {
                        if (_startAndFinishAreSame && _lastCursor == Cursors.Start)
                            CurrentActionItem.Action.Start = newValue;
                        else
                            CurrentActionItem.Action.Finish = newValue;
                    }
                    _startAndFinishAreSame = true;
                    return;
                }
                if (Math.Min(oldValue, newValue) <= CurrentActionItem.Action.Start && CurrentActionItem.Action.Start <= Math.Max(oldValue, newValue))
                {
                    CurrentActionItem.Action.Start = newValue;
                    _lastCursor = Cursors.Start;
                    _startAndFinishAreSame = false;
                    return;
                }
                if (Math.Min(oldValue, newValue) <= CurrentActionItem.Action.Finish && CurrentActionItem.Action.Finish <= Math.Max(oldValue, newValue))
                {
                    CurrentActionItem.Action.Finish = newValue;
                    _lastCursor = Cursors.Finish;
                    _startAndFinishAreSame = false;
                    return;
                }
            }

            if (CurrentActionItem != null && CurrentActionItem.Action.Video != null && CurrentVideo != null
                && !CurrentActionItem.Action.CanModifyFinish
                && newValue >= CurrentActionItem.Action.Finish
                && oldValue <= CurrentActionItem.Action.Finish
                && (Mouse.LeftButton == MouseButtonState.Released || slider == null))
            //Mettre en pause et rentrer en mode édition si on arrive au curseur de fin de la tâche
            {
                if (_justCreatingNewActionWithMarkersUnlinked)
                {
                    _justCreatingNewActionWithMarkersUnlinked = false;
                }
                else
                {
                    EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Pause));
                    CurrentActionItem.Action.CanModifyFinish = true;
                    CurrentVideoPosition = CurrentActionItem.Action.Finish;
                    CommandManager.InvalidateRequerySuggested();
                    return;
                }
            }
            if (CurrentActionItem != null && CurrentActionItem.Action.Video != null && CurrentVideo != null
                            && CurrentActionItem.Action.CanModifyFinish && _areMarkersLinked)
            //On doit arrêter le lecteur si il arrive sur le marqueur de fin du successeur
            //(si le marqueur de début et de fin du successeur sont les mêmes)
            {
                ActionGridItem marker0 = _currentMarkers.DefaultIfEmpty(null).SingleOrDefault(_ => (_.Action.Finish - _.Action.Start) <= 0 && _.Action.Start > CurrentActionItem.Action.Start);
                if (marker0 != null && !marker0.Action.IsMarkedAsAdded)
                {
                    EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Pause));
                    CurrentVideoPosition = marker0.Action.Start;
                    return;
                }
            }

            if (CurrentActionItem != null && CurrentActionItem.Action.Video != null && CurrentVideo != null
                && _subTaskCreationTimelineNarrow != null)
            //On arrete la lecture lorsqu'une sous tâche arrive à la fin de sa tâche parente.
            {
                if (CurrentVideoPosition >= _subTaskCreationTimelineNarrow.Finish && !_timelinesEndReached)
                {
                    long finish = _subTaskCreationTimelineNarrow.Finish;
                    EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Pause));
                    CurrentVideoPosition = finish;
                    CurrentActionItem.Action.Finish = finish;
                    _timelinesEndReached = true;
                    return;
                }
            }
            else if (CurrentActionItem != null && CurrentActionItem.Action.Video != null && CurrentVideo != null
                && _taskPlayTimelineNarrow != null)
            //On arrete la lecture lorsqu'une sous tâche arrive à la fin de sa timeline
            {
                if (CurrentVideoPosition >= _taskPlayTimelineNarrow.Finish && !_timelinesEndReached && CurrentActionItem.Action.Finish <= _taskPlayTimelineNarrow.Finish)
                {
                    long finish = _taskPlayTimelineNarrow.Finish;
                    EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Pause));
                    CurrentVideoPosition = finish;
                    CurrentActionItem.Action.Finish = finish;
                    _timelinesEndReached = true;
                    return;
                }
            }

            // Déplacer le curseur de fin de la tâche si on est en mode édition
            if (CurrentActionItem != null && CurrentActionItem.Action.Video != null && CurrentVideo != null
                && CurrentActionItem.Action.CanModifyFinish && newValue > oldValue)
            {
                CurrentActionItem.Action.Finish = CurrentVideoPosition;
                //Rafraichit l'état des boutons valider et annuler
                //A remplacer si des problèmes de performance apparaient
                CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="GanttGridViewModelBase{TViewModel, TIViewModel, TComponentItem, TActionItem, TResourceItem}.CanChange"/> a changé.
        /// </summary>
        protected override void OnCanChangeChanged()
        {
            base.OnCanChangeChanged();

            IsScenarioPickerEnabled = CanChange;

            // Rafraichir IsEnabled
            UpdateItemsEnabledState();
        }

        /// <summary>
        /// Obtient le temps de début pour une action donc la vidéo en cours a changé.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <returns>Le début.</returns>
        protected virtual long? GetActionNewVideoStart(ActionGridItem action) => null;

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="CurrentVideo"/> a changé.
        /// </summary>
        void OnCurrentVideoChanged()
        {
            // Annuler le narrow sur la tâche en cours de lecture
            _taskPlayTimelineNarrow = null;

            if (CurrentActionItem != null && IsViewLoaded)
            {
                bool hasChanged = CurrentActionItem.Action.Video != CurrentVideo;
                bool hasNoVideo = CurrentActionItem.Action.Video == null;

                // En lecture seule, déselectionner l'élément courant.
                // Cela évitera les problèmes de marqueurs désynchronisés
                if (hasChanged && IsReadOnly)
                    CurrentGridItem = null;
                else
                {
                    CurrentActionItem.Action.Video = CurrentVideo;
                    if (hasChanged && CurrentVideo == null && !CurrentActionItem.IsGroup.GetValueOrDefault())
                    {
                        long currentDuration = CurrentActionItem.Action.Duration;
                        CurrentActionItem.Action.Start = 0;
                        CurrentActionItem.Action.Finish = currentDuration;
                    }

                    // Pour une action nouvelle et une vidéo POV, assigner la ressource par défaut
                    if (CurrentActionItem.Action.IsMarkedAsAdded && CurrentVideo != null && CurrentVideo.ResourceView != null)
                        CurrentActionItem.Action.Resource = CurrentVideo.DefaultResource;

                    if (hasChanged)
                    {
                        if (!CanChangeActionVideo || CurrentActionItem.Action.IsMarkedAsAdded)
                        {
                            // Si on a assigné une nouvelle vidéo à l'action et que ce n'est pas prévu ou que l'action est nouvelle, remettre les timings à zéro
                            bool areLinked = AreMarkersLinked;
                            AreMarkersLinked = false;

                            CurrentActionItem.Action.Start = 0;
                            CurrentActionItem.Action.Finish = 0;

                            AreMarkersLinked = areLinked;
                        }
                        else
                        {
                            long maxPosition = (long?)CurrentVideo?.Duration ?? 0;

                            // Sinon, les timings seront conservés
                            long? start = GetActionNewVideoStart(CurrentActionItem);
                            if (start.HasValue)
                            {
                                // Conserver la durée
                                long duration = CurrentActionItem.Action.Duration;

                                bool areLinked = AreMarkersLinked;
                                AreMarkersLinked = false;
                                ActionsManager.NotVerifMarkers = true;

                                if (start.Value > maxPosition)
                                {
                                    CurrentActionItem.Action.Start = maxPosition;
                                    CurrentActionItem.Action.Duration = 0;
                                }
                                else
                                {
                                    CurrentActionItem.Action.Start = start.Value;
                                    CurrentActionItem.Action.Duration = duration;
                                }

                                AreMarkersLinked = areLinked;
                                ActionsManager.NotVerifMarkers = false;
                            }
                            else
                            {
                                if (CurrentActionItem.Action.Start > maxPosition)
                                {
                                    CurrentActionItem.Action.Start = maxPosition;
                                    CurrentActionItem.Action.Duration = 0;
                                    start = maxPosition;
                                }
                                else
                                    start = Math.Min(CurrentActionItem.Action.BuildStart, maxPosition);
                            }

                            // Positionner le curseur
                            EventBus.Publish(new MediaPlayerActionEvent(this, () => CurrentVideoPosition = start.Value));
                        }
                    }
                }
            }

            UpdateCurrentMarkers();
            OnPropertyChanged(nameof(IsManualInput));
        }

        /// <summary>
        /// Appelé lorsque la hiérarchie d'une ou plusieurs actions a changé.
        /// </summary>
        protected override void OnActionHierarchyChanged() =>
            UpdateCurrentMarkers();

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="SelectedMarker"/> a changé.
        /// </summary>
        void OnSelectedMarkerChanged()
        {
            if (SelectedMarker != null && SelectedMarker != CurrentActionItem && CanChange)
                CurrentGridItem = SelectedMarker;
        }

        /// <summary>
        /// Met à jour la liste des actions pour les marqueurs.
        /// </summary>
        void UpdateCurrentMarkers()
        {
            if (!_ignoreMarkerSelection)
            {
                _ignoreMarkerSelection = true;

                if (CurrentVideo != null)
                {
                    Func<KAction, bool> predicate;
                    if (CurrentActionItem == null)
                        predicate = a => WBSHelper.IndentationFromWBS(a.WBS) == 0;
                    else
                        predicate = a => WBSHelper.AreSiblings(CurrentActionItem.Action.WBS, a.WBS);

                    IEnumerable<ActionGridItem> markers = ActionGridItems.Where(i =>
                        !i.IsGroup.GetValueOrDefault() &&
                        i.Action.Video == CurrentVideo &&
                        predicate(i.Action));

                    // Filtrer aussi par ressource
                    if (View != GanttGridView.WBS && CurrentActionItem != null)
                        markers = markers.Where(i => i.ParentReferentialItem == CurrentActionItem.ParentReferentialItem);

                    CurrentMarkers = markers.ToArray();
                }
                else if (CurrentVideo != null)
                {
                    CurrentMarkers = null;

                    int indentation = View == GanttGridView.WBS ? 0 : 1;

                    IEnumerable<ActionGridItem> markers = ActionGridItems.Where(i =>
                        !i.IsGroup.GetValueOrDefault() &&
                        i.Action.Video == CurrentVideo &&
                        i.Indentation == indentation);

                    CurrentMarkers = markers.ToArray();
                }
                else
                    CurrentMarkers = null;

                _ignoreMarkerSelection = false;
            }

            if (CurrentActionItem != null && CurrentMarkers != null && CurrentMarkers.Contains(CurrentActionItem))
                SelectedMarker = CurrentActionItem;
            else
                SelectedMarker = null;
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="CurrentMarkers"/> a changé.
        /// </summary>
        void OnCurrentMarkersChanged()
        {
            if (IsInDesignMode)
                return;

            bool hasTimelineNarrow = _subTaskCreationTimelineNarrow != null;

            if (CurrentMarkers != null && CurrentMarkers.Length > 0)
            {
                if (WBSHelper.IndentationFromWBS(CurrentMarkers.First().Action.WBS) == 0)
                {
                    // Au plus niveau
                    if (CurrentVideo == null)
                        EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.ResetTimeline));
                    else
                        EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.ShowTimeline)
                        {
                            ShowTimelineStart = 0,
                            ShowTimelineEnd = (long)CurrentVideo.Duration,
                            SendWhenPlayerDispatcherReady = true,
                        });
                }
                else if (CurrentMarkers.Length == 1 && CurrentMarkers.First().Action.IsMarkedAsAdded && !hasTimelineNarrow)
                {
                    // S'il n'y a qu'un seul marqueur et qu'il est nouveau
                    EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.ResetTimeline));
                }
                else if (CurrentMarkers.Any(m => m.Action.ChangeTracker.ModifiedValues.ContainsKey(nameof(KAction.VideoId))))
                {
                    // Si on a changé la vidéo
                    EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.ResetTimeline));
                }
                else
                {
                    long start;
                    long end;
                    if (hasTimelineNarrow)
                    {
                        start = _subTaskCreationTimelineNarrow.Start;
                        end = _subTaskCreationTimelineNarrow.Finish;
                    }
                    else
                    {
                        start = CurrentMarkers.Select(m => m.Action.Start).Min();
                        end = CurrentMarkers.Select(m => m.Action.Finish).Max();
                    }

                    // Envoyer en async parce qu'il faut laisser la vidéo se charger au préalable
                    EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.ShowTimeline)
                    {
                        ShowTimelineStart = start,
                        ShowTimelineEnd = end,
                        SendWhenPlayerDispatcherReady = true,
                    });
                }
            }
            else
                EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.ResetTimeline));
        }

        /// <summary>
        /// Met à jour l'état activé/désactivé sur tous les éléments.
        /// </summary>
        void UpdateItemsEnabledState()
        {
            foreach (IGridItem item in ActionItems)
                item.IsEnabled = CanChange || item == CurrentGridItem;

            foreach (ActionGridItem item in ActionGridItems)
                item.AreMarkersEnabled = item.IsEnabled && !item.IsGroup.GetValueOrDefault() && !IsReadOnly;
        }

        /// <summary>
        /// Crée une nouvelle action.
        /// </summary>
        /// <param name="previousSelection">La sélection précédente.</param>
        protected virtual void AddAction(ActionGridItem previousSelection)
        {
            // Annuler le narrow sur la tâche en cours de lecture
            _taskPlayTimelineNarrow = null;

            KAction newAction = new KAction() { CanModifyFinish = true };
            ActionGridItem predecessor = InitializeNewAction(newAction, previousSelection, AreMarkersLinked);

            ActionGridItem item;
            if (previousSelection == null)
                item = ActionsManager.AddAction(newAction);
            else
                item = ActionsManager.AddAction(newAction, previousSelection);

            item.CreationPredecessor = predecessor;

            CurrentGridItem = item;

            RegisterToStateChanged(item.Action);

            // Focus le champ par défaut
            EventBus.Publish(new FocusDefaultFieldWhenCreatingEvent(this));

            CanChange = false;

            // Jouer la vidéo lorsque l'on crée une nouvelle action
            if (CurrentVideo != null)
                EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Play) { SendWhenPlayerDispatcherReady = true });
        }

        /// <summary>
        /// Ajoute une nouvelle action en tant que premier enfant de l'action sélectionnée.
        /// </summary>
        /// <param name="previousSelection">l'action actuellement sélectionnée.</param>
        void AddActionAsChild(ActionGridItem previousSelection)
        {
            KAction newAction = new KAction()
            {
                Scenario = CurrentScenarioInternal,
                CanModifyFinish = true
            };

            // Copier les informations
            if (!IsManualInput)
                newAction.Video = CurrentVideo;

            ActionGridItem predecessor = null;


            // Utiliser les même timings que l'élément s'il n'est pas encore un groupe
            if (!previousSelection.Action.IsGroup)
            {
                newAction.Start = previousSelection.Action.Start;
                // Prendre les mêmes prédécesseurs
                newAction.Predecessors.AddRange(previousSelection.Action.Predecessors);

                // Prendre les mêmes successeurs
                newAction.Successors.AddRange(previousSelection.Action.Successors);

                _subTaskCreationTimelineNarrow = new TimelineNarrow()
                {
                    ActionId = previousSelection.Action.ActionId,
                    Start = previousSelection.Action.Start,
                    Finish = previousSelection.Action.Finish
                };

                // Supprimer la vignette sur le groupe
                previousSelection.Action.Thumbnail = null;
                previousSelection.Action.IsThumbnailSpecific = false;
                previousSelection.Action.ThumbnailPosition = null;
            }
            else
            {
                KAction lastChild = WBSHelper.GetLastChild(previousSelection.Action, ActionsManager.GetActionsSortedByWBS());
                newAction.Start = lastChild.Finish;

                predecessor = ActionGridItems.FirstOrDefault(i => i.Action == lastChild);
            }
            newAction.Duration = 0;

            // Copier les référentiels
            newAction.Category = previousSelection.Action.Category;
            newAction.Resource = previousSelection.Action.Resource;
            foreach (Ref1Action al in previousSelection.Action.Ref1)
                newAction.Ref1.Add(ReferentialsFactory.CloneReferentialActionsLink(al, true, false));
            foreach (Ref2Action al in previousSelection.Action.Ref2)
                newAction.Ref2.Add(ReferentialsFactory.CloneReferentialActionsLink(al, true, false));
            foreach (Ref3Action al in previousSelection.Action.Ref3)
                newAction.Ref3.Add(ReferentialsFactory.CloneReferentialActionsLink(al, true, false));
            foreach (Ref4Action al in previousSelection.Action.Ref4)
                newAction.Ref4.Add(ReferentialsFactory.CloneReferentialActionsLink(al, true, false));
            foreach (Ref5Action al in previousSelection.Action.Ref5)
                newAction.Ref5.Add(ReferentialsFactory.CloneReferentialActionsLink(al, true, false));
            foreach (Ref6Action al in previousSelection.Action.Ref6)
                newAction.Ref6.Add(ReferentialsFactory.CloneReferentialActionsLink(al, true, false));
            foreach (Ref7Action al in previousSelection.Action.Ref7)
                newAction.Ref7.Add(ReferentialsFactory.CloneReferentialActionsLink(al, true, false));

            // Supprimer les référentiels de l'ancienne tâche
            ClearAllMultipleReferentials(previousSelection.Action);

            ActionGridItem item = ActionsManager.AddActionAsChild(newAction, previousSelection, null);

            if (predecessor != null)
                item.CreationPredecessor = predecessor;

            CurrentGridItem = item;

            RegisterToStateChanged(item.Action);

            // Focus le champ par défaut
            EventBus.Publish(new FocusDefaultFieldWhenCreatingEvent(this));

            CanChange = false;

            // Jouer la vidéo lorsque l'on crée une nouvelle action
            if (CurrentVideo != null)
                EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Play));
        }

        /// <summary>
        /// Crée une nouvelle action lié à un process.
        /// </summary>
        /// <param name="previousSelection">La sélection précédente.</param>
        void AddActionAsProcess(ActionGridItem previousSelection)
        {
            // Annuler le narrow sur la tâche en cours de lecture
            _taskPlayTimelineNarrow = null;

            KAction newAction = new KAction();
            ActionGridItem predecessor = InitializeNewAction(newAction, previousSelection, AreMarkersLinked);

            ActionGridItem item;
            if (previousSelection == null)
                item = ActionsManager.AddAction(newAction);
            else
                item = ActionsManager.AddAction(newAction, previousSelection);

            item.CreationPredecessor = predecessor;

            CurrentGridItem = item;

            RegisterToStateChanged(item.Action);

            // Focus le champ par défaut
            EventBus.Publish(new FocusDefaultFieldWhenCreatingEvent(this));

            CanChange = false;

            item.Action.ChangeTracker.ChangeTrackingEnabled = false;

            item.Action.IsLinkToProcess = true;
            item.Action.Category = null;
            item.Action.Resource = null;
            item.Action.Video = null;
            item.Action.Ref1 = null;
            item.Action.Ref2 = null;
            item.Action.Ref3 = null;
            item.Action.Ref4 = null;
            item.Action.Ref5 = null;
            item.Action.Ref6 = null;
            item.Action.Ref7 = null;
            item.Action.CustomNumericValue = null;
            item.Action.CustomNumericValue2 = null;
            item.Action.CustomNumericValue3 = null;
            item.Action.CustomNumericValue4 = null;
            item.Action.CustomTextValue = null;
            item.Action.CustomTextValue2 = null;
            item.Action.CustomTextValue3 = null;
            item.Action.CustomTextValue4 = null;

            item.Action.ChangeTracker.ChangeTrackingEnabled = true;

            if (CurrentTreeViewProcess != null)
            {
                item.Action.LinkedProcessId = CurrentTreeViewProcess.ProcessId;
                item.Action.Label = CurrentTreeViewProcess.Label;
                item.Action.Duration = CurrentTreeViewProcess.Projects.MaxWithValue(p => p.CreationDate).Scenarios.MaxWithValue(s => s.CreationDate).CriticalPathIDuration;
            }
        }

        /// <summary>
        /// Initialise une nouvelle action.
        /// </summary>
        /// <param name="newAction">La nouvelle action.</param>
        /// <param name="previousSelection">la précédente sélection.</param>
        /// <param name="usePredecessorTimings"><c>true</c> pour utiliser le timing de fin du prédécesseur.</param>
        /// <returns>
        /// Le prédécesseur éventuel, ou null
        /// </returns>
        protected virtual ActionGridItem InitializeNewAction(KAction newAction, ActionGridItem previousSelection, bool usePredecessorTimings)
        {
            ReferentialGridItem currentReferentialSelection = CurrentGridItem as ReferentialGridItem;

            Video video;
            long position;
            ActionGridItem predecessor;

            bool keepResource = KeepsSelection(ProcessReferentialIdentifier.Equipment);

            KAction referentialsSelection;

            // Si la sélection en cours est un groupe, utiliser :
            // La vidéo de la dernière action
            // La position de cette tâche dans la vidéo, si elle a une vidéo
            if (previousSelection != null && previousSelection.Action.IsGroup && CurrentVideo != null)
            {
                KAction lastAction =
                    WBSHelper
                        .GetChildren(previousSelection.Action, CurrentScenarioInternal.Actions)
                        .OrderByWBS()
                        .LastOrDefault();
                referentialsSelection = lastAction;

                video = lastAction?.Video ?? null;

                if (lastAction != null)
                    predecessor = ActionGridItems.FirstOrDefault(gi => gi.Action == lastAction);
                else
                    predecessor = null;

                if (usePredecessorTimings)
                    position = lastAction != null ? lastAction.Finish : 0;
                else
                    position = CurrentVideoPosition;
            }
            else
            {
                video = CurrentVideo;
                predecessor = previousSelection;
                referentialsSelection = previousSelection?.Action ?? null;

                if (!IsManualInput)
                {
                    if (previousSelection != null && usePredecessorTimings)
                        position = previousSelection.Action.Finish;
                    else
                        position = CurrentVideoPosition;
                }
                else
                    position = 0;
            }

            if (referentialsSelection != null)
            {
                CloneActionLinks(ProcessReferentialIdentifier.Ref1, referentialsSelection.Ref1, newAction.Ref1);
                CloneActionLinks(ProcessReferentialIdentifier.Ref2, referentialsSelection.Ref2, newAction.Ref2);
                CloneActionLinks(ProcessReferentialIdentifier.Ref3, referentialsSelection.Ref3, newAction.Ref3);
                CloneActionLinks(ProcessReferentialIdentifier.Ref4, referentialsSelection.Ref4, newAction.Ref4);
                CloneActionLinks(ProcessReferentialIdentifier.Ref5, referentialsSelection.Ref5, newAction.Ref5);
                CloneActionLinks(ProcessReferentialIdentifier.Ref6, referentialsSelection.Ref6, newAction.Ref6);
                CloneActionLinks(ProcessReferentialIdentifier.Ref7, referentialsSelection.Ref7, newAction.Ref7);

                if (keepResource)
                    newAction.Resource = referentialsSelection?.Resource ?? null;
            }

            newAction.Scenario = CurrentScenarioInternal;

            newAction.Start = position;
            newAction.Finish = newAction.Start;
            newAction.Video = video;

            if (referentialsSelection == null && newAction.Video != null && newAction.Video.ResourceView != null)
                newAction.Resource = newAction.Video.DefaultResource;

            return predecessor;
        }

        /// <summary>
        /// Détermine si la sélection doit être conservée pour le référentiel spécifié.
        /// </summary>
        /// <param name="id">L'identifiant du référentiel.</param>
        /// <returns><c>true</c> si la sélection doit être conservée.</returns>
        bool KeepsSelection(ProcessReferentialIdentifier id)
        {
            IReferentialsUseService refUseService = IoC.Resolve<IReferentialsUseService>();
            if (id == ProcessReferentialIdentifier.Operator || id == ProcessReferentialIdentifier.Equipment)
            {
                return (refUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Operator) || refUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Equipment)) &&
                    (refUseService.Referentials[ProcessReferentialIdentifier.Operator].KeepsSelection || refUseService.Referentials[ProcessReferentialIdentifier.Equipment].KeepsSelection);
            }
            return refUseService.IsReferentialEnabled(id) && refUseService.Referentials[id].KeepsSelection;
        }

        /// <summary>
        /// Clone les liens Référentiel - Actions depuis une collection vers une autre.
        /// </summary>
        /// <typeparam name="TActionLink"></typeparam>
        /// <param name="refId"></param>
        /// <param name="oldCollection"></param>
        /// <param name="newCollection"></param>
        void CloneActionLinks<TActionLink>(ProcessReferentialIdentifier refId, IList<TActionLink> oldCollection, IList<TActionLink> newCollection)
            where TActionLink : IReferentialActionLink, new()
        {
            if (KeepsSelection(refId))
            {
                foreach (TActionLink actionLink in oldCollection)
                {
                    TActionLink newAl = ReferentialsFactory.CloneReferentialActionsLink(actionLink, true, false);
                    newCollection.Add(newAl);
                }
            }
        }

        /// <summary>
        /// Valide les actions qui ont changé.
        /// </summary>
        /// <returns><c>true</c> si les actions sont valides.</returns>
        bool ValidateActions()
        {
            ActionsManager.FixPredecessorsSuccessorsTimings();

            KAction[] changedActions = ActionGridItems
                .Select(i => i.Action)
                .Where(a => a.IsNotMarkedAsUnchanged)
                .ToArray();

            foreach (KAction action in changedActions)
                action.Validate();

            // Equivalent de RefreshValidationErrors mais en indiquant l'action
            StringBuilder sb = new StringBuilder();

            foreach (KAction action in changedActions.Where(m => !m.IsValid.GetValueOrDefault()))
            {
                string actionLabel;
                if (!string.IsNullOrEmpty(action.Label))
                    actionLabel = string.Format(_validationActionLabelLong, action.Label, action.WBS);
                else
                    actionLabel = string.Format(_validationActionLabelShort, action.WBS);

                sb.Append(actionLabel);
                sb.AppendLine(" :");

                foreach (string error in action.Errors.Select(e => e.Message).Distinct())
                {
                    sb.Append("  ");
                    sb.AppendLine(error);
                }
            }

            if (sb.Length > 0)
            {
                ShowValidationErrors(string.Join("\n", sb.ToString()));

                // Active la validation auto
                foreach (KAction action in changedActions)
                    action.EnableAutoValidation = true;

                return false;
            }
            HideValidationErrors();

            return true;
        }

        /// <summary>
        /// Appelé lorsque la refraichissement des erreurs de validation est demandé.
        /// Dans une méthode dérivée, appeler RefreshValidationErrors.
        /// </summary>
        protected override void OnRefreshValidationErrorsRequested() =>
            ValidateActions();


        WBSTreeVirtualizer tree;

        /// <summary>
        /// Duplique les taches
        /// </summary>
        void DuplicateTask()
        {
            _taskPlayTimelineNarrow = null;
            EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Pause));

            tree = ActionsManager.GetActionsSortedByWBS().VirtualizeTree();

            MessageDialogResult dialog = DialogFactory.GetDialogView<IMessageDialog>().Show(
                              LocalizationManager.GetString("VM_Acquire_Message_KeepVideos"),//"Souhaitez-vous conserver les même vidéos",
                              LocalizationManager.GetString("VM_Acquire_Message_KeepVideosTitle"),// "Duplication des vidéos",
                              MessageDialogButton.YesNo,
                              MessageDialogImage.Question);

            _keepVideo = dialog == MessageDialogResult.Yes;

            //Liste de toutes les actions descendentes des actions sélectionnées (comprenant les actions sélectionnées elles-même)
            List<KAction> TotalDescendentSelectedList = new List<KAction>();
            foreach (ActionGridItem selectedItem in SelectedItems.OfType<ActionGridItem>())
            {
                TotalDescendentSelectedList.AddNew(selectedItem.Action);
                TotalDescendentSelectedList.AddRangeNew(WBSHelper.GetDescendants(selectedItem.Action, CurrentScenarioInternal.Actions));
            }

            //Liste de correspondance entre les différentes actions dupliquées
            List<(KAction oldAction, KAction newAction)> duplicatesCorrespondanceList = new List<(KAction oldAction, KAction newAction)>();

            for (int i = 0; i < SelectedItems.OfType<ActionGridItem>().Count(); i++)
            {
                ActionGridItem item = SelectedItems.OfType<ActionGridItem>().ElementAt(i);
                WBSTreeVirtualizer.Node node = (WBSTreeVirtualizer.Node)tree.GetNode(item.Action).Clone();
                tree.AddNode(node);
                KAction parent = WBSHelper.GetParent(node.Action, CurrentScenarioInternal.Actions);
                ActionGridItem parentItem = ActionGridItems.FirstOrDefault(t => t.Action == parent);
                //_afterItem = parent;

                if (parent == null)
                    _afterItem = CurrentScenarioInternal.Actions.FirstOrDefault(a => a.WBS == item.Action.WBS);
                else
                    GetLastChildRecursively(item.Action);


                Duplicate(node, parentItem, duplicatesCorrespondanceList);
            }

            CopyLinksBetweenTasks(TotalDescendentSelectedList, duplicatesCorrespondanceList);

            _afterItem = null;

            CurrentGridItem = ActionGridItems.Where(t => t.Action.IsMarkedAsAdded).MinWithValue(t => t.Action.WBS);

            CanChange = false;

        }
        /// <summary>
        /// Copie les liens entre les tâches dupliquées (predecesors et succesors) en se basant sur les liens de tâches originales
        /// </summary>
        /// <param name="TotalDescendentSelectedList">List de toutes les actions descendentes des actions sélectionnées (comprenant les actions sélectionnées elles-même)</param>
        /// <param name="duplicatesCorrespondanceList">Liste de correspondance entre les différentes actions dupliquées</param>
        public void CopyLinksBetweenTasks(List<KAction> TotalDescendentSelectedList, List<(KAction oldAction, KAction newAction)> duplicatesCorrespondanceList)
        {

            //Copie des predecesseurs et successeurs
            for (int i = 0; i < duplicatesCorrespondanceList.Count; i++)
            {
                foreach (KAction predecessor in duplicatesCorrespondanceList[i].oldAction.Predecessors)
                {
                    if (TotalDescendentSelectedList.Contains(predecessor))
                    {
                        (KAction oldAction, KAction newAction)? tuple = duplicatesCorrespondanceList.SingleOrDefault(a => a.oldAction.WBS == predecessor.WBS);
                        if (tuple.HasValue)
                            duplicatesCorrespondanceList[i].newAction.Predecessors.Add(tuple.Value.newAction);
                    }
                }

                foreach (KAction successor in duplicatesCorrespondanceList[i].oldAction.Successors)
                {
                    if (TotalDescendentSelectedList.Contains(successor))
                    {
                        (KAction oldAction, KAction newAction)? tuple = duplicatesCorrespondanceList.SingleOrDefault(a => a.oldAction.WBS == successor.WBS);
                        if (tuple.HasValue)
                            duplicatesCorrespondanceList[i].newAction.Successors.Add(tuple.Value.newAction);
                    }

                }
            }

            //Copie des prédecesseurs et des successeurs managés
            for (int i = 0; i < duplicatesCorrespondanceList.Count; i++)
            {
                foreach (KAction predecessor in duplicatesCorrespondanceList[i].oldAction.PredecessorsManaged)
                {
                    if (TotalDescendentSelectedList.Contains(predecessor))
                    {
                        (KAction oldAction, KAction newAction)? tuple = duplicatesCorrespondanceList.SingleOrDefault(a => a.oldAction.WBS == predecessor.WBS);
                        if (tuple.HasValue)
                            duplicatesCorrespondanceList[i].newAction.PredecessorsManaged.Add(tuple.Value.newAction);
                    }
                }

                foreach (KAction successor in duplicatesCorrespondanceList[i].oldAction.SuccessorsManaged)
                {
                    if (TotalDescendentSelectedList.Contains(successor))
                    {
                        (KAction oldAction, KAction newAction)? tuple = duplicatesCorrespondanceList.SingleOrDefault(a => a.oldAction.WBS == successor.WBS);
                        if (tuple.HasValue)
                            duplicatesCorrespondanceList[i].newAction.SuccessorsManaged.Add(tuple.Value.newAction);
                    }

                }
            }
        }

        public void GetLastChildRecursively(KAction action)
        {
            _afterItem = CurrentScenarioInternal.Actions.FirstOrDefault(a => a.WBS == action.WBS);
            KAction child = null;

            if (_afterItem != null)
                child = WBSHelper.GetLastChild(_afterItem, CurrentScenarioInternal.Actions.OrderByWBS());

            if (child != null)
                GetLastChildRecursively(child);
        }

        public void Duplicate(WBSTreeVirtualizer.Node node, ActionGridItem parentItem, List<(KAction oldAction, KAction newAction)> correspondanceList)
        {
            KAction action = node.Action;

            KAction newAction = new KAction()
            {
                ScenarioId = action.ScenarioId,
                ResourceId = action.ResourceId,
                OriginalActionId = action.OriginalActionId,
                Original = action.Original,
                VideoId = _keepVideo ? action.VideoId : null,
                Label = action.Label,
                Start = action.Start,
                Finish = action.Finish,//action.IsReduced ? action.Reduced.Action.Finish : action.Finish,
                BuildStart = action.BuildStart,
                BuildFinish = action.BuildFinish,
                IsRandom = action.IsRandom,
                CreatedByUserId = action.CreatedByUserId,
                ModifiedByUserId = action.ModifiedByUserId,
                LastModificationDate = action.LastModificationDate,
                CustomNumericValue = action.CustomNumericValue,
                CustomNumericValue2 = action.CustomNumericValue2,
                CustomNumericValue3 = action.CustomNumericValue3,
                CustomNumericValue4 = action.CustomNumericValue4,
                CustomTextValue = action.CustomTextValue,
                CustomTextValue2 = action.CustomTextValue2,
                CustomTextValue3 = action.CustomTextValue3,
                CustomTextValue4 = action.CustomTextValue4,
                DifferenceReason = action.DifferenceReason,
                Thumbnail = action.Thumbnail,
                IsThumbnailSpecific = action.IsThumbnailSpecific,
                ThumbnailPosition = action.ThumbnailPosition,
                Video = _keepVideo ? action.Video : null,
                Scenario = action.Scenario,
                Resource = action.Resource,
                Operator = action.Operator,
                Category = action.Category,
                CategoryId = action.CategoryId,
                Equipment = action.Equipment,
                AmeliorationDescription = action.AmeliorationDescription,
                IsGroup = action.IsGroup,
                BuildDuration = action.BuildDuration,
                Duration = action.Duration
            };

            correspondanceList.Add((action, newAction));

            foreach (Ref1Action al in action.Ref1)
                newAction.Ref1.Add(ReferentialsFactory.CloneReferentialActionsLink(al, true, false));
            foreach (Ref2Action al in action.Ref2)
                newAction.Ref2.Add(ReferentialsFactory.CloneReferentialActionsLink(al, true, false));
            foreach (Ref3Action al in action.Ref3)
                newAction.Ref3.Add(ReferentialsFactory.CloneReferentialActionsLink(al, true, false));
            foreach (Ref4Action al in action.Ref4)
                newAction.Ref4.Add(ReferentialsFactory.CloneReferentialActionsLink(al, true, false));
            foreach (Ref5Action al in action.Ref5)
                newAction.Ref5.Add(ReferentialsFactory.CloneReferentialActionsLink(al, true, false));
            foreach (Ref6Action al in action.Ref6)
                newAction.Ref6.Add(ReferentialsFactory.CloneReferentialActionsLink(al, true, false));
            foreach (Ref7Action al in action.Ref7)
                newAction.Ref7.Add(ReferentialsFactory.CloneReferentialActionsLink(al, true, false));

            newAction.Resource = action.Resource;

            //Reduced
            if (action.IsReduced)
            {
                newAction.Reduced = new KActionReduced()
                {
                    Action = newAction,
                    ActionTypeCode = action.Reduced.ActionTypeCode,
                    Solution = action.Reduced.Solution,
                    ReductionRatio = action.Reduced.ReductionRatio,
                    OriginalBuildDuration = action.Reduced.OriginalBuildDuration
                };
                ActionsTimingsMoveManagement.UpdateTimingsFromReducedReduction(newAction);
                newAction.Reduced.MarkAsAdded();
            }

            if (parentItem == null)
            {
                //convert _afterItem into TComponentItem 
                ActionGridItem afterItem = ActionGridItems.FirstOrDefault(t => t.Action == _afterItem);
                parentItem = ActionsManager.AddAction(newAction, afterItem);
            }
            else
                parentItem = ActionsManager.AddActionAsChild(newAction, parentItem, _afterItem);
            _afterItem = newAction;

            newAction.MarkAsAdded();

            foreach (WBSTreeVirtualizer.Node childNode in node.Children)
                Duplicate(childNode, parentItem, correspondanceList);
            RegisterToStateChanged(parentItem.Action);
        }

        /// <summary>
        /// Supprime l'action spécifiée.
        /// </summary>
        async Task DeleteAction()
        {
            // Annuler le narrow sur la tâche en cours de lecture
            _taskPlayTimelineNarrow = null;
            EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Pause));

            for (int i = 0; i < SelectedItems.OfType<ActionGridItem>().Count(); i++)
            {
                ActionGridItem item = SelectedItems.OfType<ActionGridItem>().ElementAt(i);

                List<KAction> actionsToDelete = new List<KAction>
                {
                    item.Action
                };

                // Supprimer tous les enfants du groupe
                if (item.IsGroup.GetValueOrDefault())
                    actionsToDelete.AddRange(WBSHelper.GetDescendants(item.Action, CurrentScenarioInternal.Actions).OrderByWBS());

                bool okToDelete = true;

                try
                {
                    bool ok = await ShowImpactedScenarios(CurrentScenarioInternal, _scenarios, true, actionsToDelete.ToArray(),
                        () =>
                        {
                            okToDelete = DialogFactory.GetDialogView<ICommonDialog>().ShowSureToDelete();
                        });

                    if (ok && okToDelete)
                    {
                        foreach (KAction action in actionsToDelete)
                        {
                            action.MarkAsDeleted();
                            ActionsManager.DeleteAction(action);
                        }

                        await SaveActionsWithoutPrompt(true);
                        CanChange = true;
                    }
                }
                catch (Exception e)
                {
                    base.OnError(e);
                }
            }
        }

        /// <summary>
        /// Dégroupe l'action spécifiée.
        /// </summary>
        /// <param name="item">L'élément lié à l'action.</param>
        async Task Ungroup(ActionGridItem item)
        {
            bool okToUngroup = true;

            try
            {
                bool ok = await ShowImpactedScenarios(CurrentScenarioInternal, _scenarios, true, new KAction[] { item.Action },
                    () =>
                    {
                        MessageDialogResult res = DialogFactory.GetDialogView<IMessageDialog>().Show(
                            LocalizationManager.GetString("VM_Acquire_Message_SureToUngroup"),
                            LocalizationManager.GetString("Common_Confirm"),
                            MessageDialogButton.YesNoCancel,
                            MessageDialogImage.Question);

                        okToUngroup = res == MessageDialogResult.Yes;
                    }, ActionsManager.PredictUngroupModifiedAction(item));
                if (ok && okToUngroup)
                {
                    item.Action.MarkAsDeleted();
                    ActionsManager.Ungroup(item);

                    CanChange |= await SaveActions(true);
                }
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
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
                bool ok = await ShowImpactedScenarios(CurrentScenarioInternal, _scenarios, false, null, null);

                if (ok)
                {
                    await SaveActionsWithoutPrompt(refreshSelectionWhenDone);
                    return true;
                }
                HideSpinner();
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
            return false;
        }

        /// <summary>
        /// Sauvegarde les actions.
        /// </summary>
        /// <param name="scenarios">Les scénarios à sauvegarder.</param>
        /// <param name="scenario">Le scénario àsauvegarder.</param>
        protected abstract Task<Scenario> SaveActionsServiceCall(Scenario[] scenarios, Scenario scenario);

        /// <summary>
        /// Sauvegarde les actions sans aucun message.
        /// </summary>
        /// <param name="refreshSelectionWhenDone"><c>true</c> pour rafraichir la sélection une fois la sauvegarde effectuée.</param>
        protected async Task SaveActionsWithoutPrompt(bool refreshSelectionWhenDone)
        {
            // Recupere le WBS courant pour pouvoir reloader l'action courante plus tard
            var currentWbs = CurrentActionItem?.Action.WBS;

            // Sauvegarder
            CurrentScenarioInternal = await SaveActionsServiceCall(_scenarios, CurrentScenarioInternal);

            await Refresh();
            if (!string.IsNullOrEmpty(currentWbs))
                CurrentActionItem = ActionGridItems.Single(u => u.Action.WBS == currentWbs);

            foreach (ActionGridItem item in ActionGridItems.Where(i => !i.Action.IsMarkedAsDeleted))
                item.Action.StartTracking();

            if (refreshSelectionWhenDone)
                RebuildItems(CurrentActionItem);
            else
            {
                RebuildItems(null);
                UpdateCurrentMarkers();
            }

            UpdateActionsLabels();
            HideSpinner();
        }

        /// <summary>
        /// Appelé lorsque l'action courante a changé
        /// </summary>
        /// <param name="previousValue">La valeur précédante.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
        protected override void OnCurrentActionChanged(ActionGridItem previousValue, ActionGridItem newValue)
        {
            if (previousValue != null)
                previousValue.Action.CanModifyFinish = false;

            base.OnCurrentActionChanged(previousValue, newValue);

            if (newValue?.Action.IsLinkToProcess == true)
            {
                Procedure processInTree = TreeViewHelper.FindProcess(AllProcesses[0], newValue.Action.LinkedProcessId.Value);
                CurrentNode = processInTree;
                if (processInTree.ProjectDir != null)
                    TreeViewHelper.ExpandFolder(processInTree.ProjectDir);
            }

            if (IsInDesignMode)
                return;

            _lastCursor = Cursors.Start;

            // Annuler le narrow sur la tâche en cours de lecture
            _taskPlayTimelineNarrow = null;

            // Synchroniser la vidéo et la position dans celle-ci
            if (newValue != null && newValue.Action.Video != null && Videos.Contains(newValue.Action.Video))
            {
                /*EventBus.Publish(new MediaPlayerActionEvent(this, (player) =>
                {
                    if (player is KMediaPlayer mediaPlayer)
                        mediaPlayer.SetVideoVisibility(false);
                }));*/
                CurrentVideo = newValue.Action.Video;

                EventBus.Publish(new MediaPlayerActionEvent(this, () =>
                {
                    _currentVideoPosition = newValue.Action.Start;
                    OnPropertyChanged(nameof(CurrentVideoPosition));
                    _taskPlayTimelineNarrow = CreateTimelineNarrowForCurrentAction();
                }));
                EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Pause));
            }
            else if (newValue != null && newValue.Action.Video == null)
            {
                CurrentVideo = null;
                EventBus.Publish(new MediaPlayerActionEvent(this, (player) =>
                {
                    if (player is KMediaPlayer mediaPlayer)
                        mediaPlayer.ShowThumbnailView(null);
                }));
                EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.Stop));
            }

            // Mettre à jour la timeline narrow si nécessaire
            if (_subTaskCreationTimelineNarrow != null)
            {
                if (newValue != null)
                {
                    KAction parent = WBSHelper.GetParent(newValue.Action, base.CurrentScenarioInternal.Actions);
                    if (parent == null || (parent != null && parent.ActionId != _subTaskCreationTimelineNarrow.ActionId))
                        _subTaskCreationTimelineNarrow = null;
                }
            }

            UpdateCurrentMarkers();
            UpdateItemsEnabledState();
            UpdateMultiReferentials();
            UpdateHasThumbnail();

            //On affiche le snapshot
            if (newValue?.Action.Thumbnail != null)
                EventBus.Publish(new MediaPlayerActionEvent(this, (player) =>
                {
                    if (player is KMediaPlayer mediaPlayer)
                        mediaPlayer.ShowThumbnailView(newValue?.Action);
                }));

            OnPropertyChanged(nameof(IsManualInput));
            OnPropertyChanged(nameof(AreTimingsReadOnly));
            OnPropertyChanged(nameof(AreTimingsVisible));
        }

        /// <summary>
        /// Met à jour l'état lecture seule du scénario.
        /// </summary>
        void UpdateIsReadOnly()
        {
            IsReadOnly =
                (CurrentScenarioInternal != null &&
                    ServiceBus.Get<IProjectManagerService>().Scenarios.First(sc => sc.Id == CurrentScenarioInternal.ScenarioId).IsLocked) ||
                !CanCurrentUserWrite;
            UpdateItemsEnabledState();
        }

        /// <summary>
        /// Met à jour la liste des libellés d'actions.
        /// </summary>
        void UpdateActionsLabels()
        {
            IEnumerable<string> labels = CurrentScenarioInternal.Actions.Select(a => a.Label).Where(l => !string.IsNullOrWhiteSpace(l));

            ActionsLabels.Sync(labels);
            ActionsLabels.Sort();
        }

        /// <summary>
        /// Appelé lorsque les timings vidéo d'une action ont changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.EventArgs&lt;KAction&gt;"/> contenant les données de l'évènement.</param>
        void OnActionVideoTimingChanged(object sender, EventArgs<KAction> e)
        {
            // S'assurer que la plage de lecture de l'action en cours soit annulée
            //_taskPlayTimelineNarrow = null;
        }

        /// <summary>
        /// Définit la vignette de la tâche courante à partir d'un fichier externe.
        /// </summary>
        void SetExternalThumbnail()
        {
            // Pas de vidéo : proposer d'en charger une
            string[] file = DialogFactory.GetDialogView<IOpenFileDialog>().Show(
                LocalizationManager.GetString("VM_Acquire_ThumbnailCaption"),
                filter: FileExtensionsDialogHelper.GetImagesFileDialogFilter());

            if (file != null && file.Any())
            {
                string fileName = file.First();

                // Une fois le fichier spécifié, vérifier qu'il puisse bien être décodé et que le taille soit correcte.
                try
                {
                    if (!FilesHelper.IsFileType(fileName, FileType.Picture))
                    {
                        base.OnError(LocalizationManager.GetStringFormat("Common_NotAFileOfFileType", LocalizationManager.GetString("Common_FileType_Picture")));
                        return;
                    }

                    var thumbnailsLocation = FilesHelper.GetSyncFilesLocation();
                    var thumbnailPath = Path.Combine(thumbnailsLocation, $"{Guid.NewGuid()}_{Path.GetExtension(fileName)}");
                    int maxSize = 720;
                    var size = (new Uri(fileName)).GetThumbnailSize();
                    if (Math.Min(size.Width, size.Height) <= maxSize)
                        File.Copy(fileName, thumbnailPath, true);
                    else
                    {
                        int thumbnailWidth = size.Width > size.Height ? (int)size.Width * maxSize / (int)size.Height : maxSize;
                        int thumbnailHeight = size.Width > size.Height ? maxSize : (int)size.Height * maxSize / (int)size.Width;
                        Image.Thumbnail(fileName, thumbnailWidth, thumbnailHeight)
                            .WriteToFile(thumbnailPath);
                    }

                    CurrentActionItem.Action.Thumbnail = new CloudFile(File.ReadAllBytes(thumbnailPath), Path.GetExtension(thumbnailPath));
                    CurrentActionItem.Action.IsThumbnailSpecific = true;
                    CurrentActionItem.Action.ThumbnailPosition = null;
                }
                catch (Exception e)
                {
                    this.TraceDebug(e, "Erreur lors de l'ouverture de la vignette");

                    DialogFactory.GetDialogView<IMessageDialog>().Show(
                        LocalizationManager.GetString("VM_Acquire_Message_InvalidThumbnailFile"),
                        LocalizationManager.GetString("Common_Error"),
                        MessageDialogButton.OK,
                        MessageDialogImage.Exclamation);
                    return;
                }
            }
        }

        /// <summary>
        /// Met à jour la vignette pour une action.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="position">La position dans la vidéo.</param>
        /// <param name="isSpecific"><c>true</c> si la vignette a été désignée spécifiquement.</param>
        async Task UpdateThumbnail(KAction action, long position, bool isSpecific)
        {
            if (action.Video == null)
                throw new InvalidOperationException("L'action doit avoir une vidéo");

            if (ServiceBus.IsAvailable<IMediaPlayerService>())
            {
                IMediaPlayerService service = ServiceBus.Get<IMediaPlayerService>();
                using (MemoryStream ms = new MemoryStream())
                {
                    service.GetThumbnailWithAnnotations(ms);
                    if (ms.Length == 0)
                    {
                        BitmapSource bitmap = await SnapshotGrabber.GetSnapshotAsync(action.Video.FilePath, position);
                        if (bitmap != null)
                        {
                            JpegBitmapEncoder encoder = new JpegBitmapEncoder
                            {
                                QualityLevel = 90
                            };

                            encoder.Frames.Add(BitmapFrame.Create(bitmap));
                            encoder.Save(ms);
                        }
                        else
                        {
                            action.Thumbnail = null;
                            return;
                        }
                    }
                    else
                        service.ResetAnnotations();
                    action.Thumbnail = new CloudFile(ms.ToArray(), ".jpg");
                }
                action.IsThumbnailSpecific = isSpecific;
                action.ThumbnailPosition = position;
            }
        }

        /// <summary>
        /// Met à jour l'état ajout/suppr de la vignette.
        /// </summary>
        void UpdateHasThumbnail() =>
            HasThumbnail = CurrentActionItem?.Thumbnail != null;

        /// <summary>
        /// Appelé lorsque le dossier courant a changé.
        /// </summary>
        /// <param name="previousProjectDir">La ressource précédente.</param>
        /// <param name="newProjectDir">La nouvelle ressourc.</param>
        void OnCurrentTreeViewFolderChanged(ProjectDir previousProjectDir, ProjectDir newProjectDir)
        {
            CurrentActionItem.Action.LinkedProcess = null;
        }

        /// <summary>
        /// Appelé lorsque le process courant a changé.
        /// </summary>
        /// <param name="previousProcess">La ressource précédente.</param>
        /// <param name="newProcess">La nouvelle ressourc.</param>
        void OnCurrentTreeViewProcessChanged(Procedure previousProcess, Procedure newProcess)
        {
            if (newProcess?.ProcessId != CurrentActionItem.Action.LinkedProcessId)
            {
                CurrentActionItem.Action.LinkedProcessId = newProcess?.ProcessId;
                CurrentActionItem.Action.Video = null;
                if (newProcess != null)
                    CurrentActionItem.Action.Duration = newProcess.Projects.MaxWithValue(p => p.CreationDate).Scenarios.MaxWithValue(s => s.CreationDate).CriticalPathIDuration;
            }
        }

        #endregion

        #region Gestion référentiels

        bool _ignoreReferentialsChange;

        IActionReferential _currentCreatingReferential;
        IList _currentCreatingReferentialCollection;

        /// <summary>
        /// Crée une nouvelle catégorie.
        /// </summary>
        void CreateNewCategory()
        {
            ActionCategory cat = new ActionCategory()
            {
                ProcessId = CurrentScenarioInternal.Project.ProcessId,
                ActionValueCode = KnownActionCategoryValues.VA,
                IsEditable = true,
                Color = GetReferentialRandomColor(a => a.Category != null ? EnumerableExt.Concat(a.Category) : Enumerable.Empty<ActionCategory>()),
            };

            SubscribeDeleteEmptyLabels(cat, Categories);

            Categories.Add(cat);
            CurrentActionItem.Action.Category = cat;
        }

        /// <summary>
        /// Crée une nouvelle compétence.
        /// </summary>
        void CreateNewSkill()
        {
            Skill skill = new Skill()
            {
                IsEditable = true,
                Color = GetReferentialRandomColor(a => a.Skill != null ? EnumerableExt.Concat(a.Skill) : Enumerable.Empty<Skill>()),
            };

            SubscribeDeleteEmptyLabels(skill, Skills);

            Skills.Add(skill);
            CurrentActionItem.Action.Skill = skill;
        }

        /// <summary>
        /// Crée un nouvel opérateur.
        /// </summary>
        void CreateNewOperator()
        {
            Operator op = new Operator()
            {
                ProcessId = CurrentScenarioInternal.Project.ProcessId,
                IsEditable = true,
                Color = GetReferentialRandomColor(a => a.Operator != null ? EnumerableExt.Concat(a.Operator) : Enumerable.Empty<Operator>()),
            };

            SubscribeDeleteEmptyLabels(op, Operators);

            Operators.Add(op);
            CurrentActionItem.Action.Operator = op;
        }

        /// <summary>
        /// Crée un nouvel équipement.
        /// </summary>
        void CreateNewEquipment()
        {
            Equipment equipment = new Equipment()
            {
                ProcessId = CurrentScenarioInternal.Project.ProcessId,
                IsEditable = true,
                Color = GetReferentialRandomColor(a => a.Equipment != null ? EnumerableExt.Concat(a.Equipment) : Enumerable.Empty<Equipment>()),
            };

            SubscribeDeleteEmptyLabels(equipment, Equipments);

            Equipments.Add(equipment);
            CurrentActionItem.Action.Equipment = equipment;
        }



        /// <summary>
        /// Crée un nouveau référentiel.
        /// </summary>
        /// <typeparam name="TReferential">Le type de référentiel.</typeparam>
        /// <typeparam name="TReferentialProject">Le type de référentiel projet.</typeparam>
        /// <param name="collection">La collection des référentiels.</param>
        /// <param name="actionsLinkGetter">Un délégué récupérant les liens entre une action et les référentiels.</param>
        void CreateNewReferential<TReferential, TReferentialProject>(BulkObservableCollection<TReferential> collection, Func<KAction, IEnumerable<IReferentialActionLink>> actionsLinkGetter)
            where TReferential : IMultipleActionReferential
            where TReferentialProject : IMultipleActionReferential, IActionReferentialProcess, TReferential, new()
        {
            TReferentialProject refe = new TReferentialProject()
            {
                ProcessId = CurrentScenarioInternal.Project.ProcessId,
                IsEditable = true,
                Color = GetReferentialRandomColor(a => actionsLinkGetter(a).Select(al => al.Referential)),
                Quantity = 1,
            };

            SubscribeDeleteEmptyLabels(refe, collection);

            collection.Add(refe);
        }

        /// <summary>
        /// Obtient une couleur aléatoire pour un référentiel.
        /// </summary>
        /// <param name="referentialsGetter">L'accesseur au référentiel.</param>
        /// <returns>Une couleur aléatoire.</returns>
        string GetReferentialRandomColor(Func<KAction, IEnumerable<IActionReferential>> referentialsGetter)
        {
            return ColorsHelper.GetRandomColor(ColorsHelper.StandardColorsExcludedGreenYellowOrangeRed,
                CurrentScenarioInternal.Actions.SelectMany(action => referentialsGetter(action))
                .Where(a => !string.IsNullOrEmpty(a.Color))
                .Select(a => a.Color)
                .Distinct()
                ).ToString();
        }

        /// <summary>
        /// Souscris aux évènements nécessaires pour supprimer automatiquement un nouveau référentiel qui aurait un libellé vide.
        /// </summary>
        /// <typeparam name="TReferential">Le type du référentiel.</typeparam>
        /// <param name="referential">Le référentiel.</param>
        /// <param name="collection">La collection de référentiels.</param>
        void SubscribeDeleteEmptyLabels<TActionReferential>(IActionReferential referential,
            BulkObservableCollection<TActionReferential> collection)
            where TActionReferential : IActionReferential
        {
            if (_currentCreatingReferential != null && _currentCreatingReferentialCollection != null)
                _currentCreatingReferential.IsEditable = false;

            _currentCreatingReferential = referential;
            _currentCreatingReferentialCollection = collection;
            referential.IsEditableChanged += OnIsEditableChanged;
        }

        /// <summary>
        /// Appelé lorsque la valeur de la propriété IsEditable a changé sur le référentiel en cours de création.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.EventArgs"/> contenant les données de l'évènement.</param>
        void OnIsEditableChanged(object sender, EventArgs e) =>
            ValidateCurrentCreatingReferential();

        /// <summary>
        /// Valide la saisie actuelle de création de référentiel.
        /// </summary>
        void ValidateCurrentCreatingReferential()
        {
            if (_currentCreatingReferentialCollection != null && _currentCreatingReferential != null)
            {
                if (string.IsNullOrWhiteSpace(_currentCreatingReferential.Label))
                    DeleteCurrentCreatingReferential();
                else
                {
                    _currentCreatingReferential.IsEditableChanged -= OnIsEditableChanged;

                    // Ajouter le lien référentiel - action
                    if (_currentCreatingReferential is IMultipleActionReferential)
                    {
                        bool hasMultipleSelection = IoC.Resolve<IReferentialsUseService>().Referentials[_currentCreatingReferential.ProcessReferentialId].HasMultipleSelection;

                        if (!hasMultipleSelection)
                        {
                            // Si l'élément n'autorise pas la sélection multiple, supprimer les autres
                            ReferentialsFactory.ClearReferentialActionLinks(_currentCreatingReferential.ProcessReferentialId, CurrentActionItem.Action);
                        }

                        ReferentialsFactory.CreateReferentialActionLink((IMultipleActionReferential)_currentCreatingReferential, CurrentActionItem.Action, _currentCreatingReferential.Quantity);
                        RegisterReferentialChanged((IMultipleActionReferential)_currentCreatingReferential);
                        UpdateMultiReferentials();
                        CanChange = false;
                    }
                }
                _currentCreatingReferential.IsEditable = false;

                CollectionViewSource.GetDefaultView(_currentCreatingReferentialCollection).Refresh();

                EndCurrentCreatingReferential();
            }
        }

        /// <summary>
        /// Termine la session de saisie de référentiel.
        /// </summary>
        void EndCurrentCreatingReferential()
        {
            _currentCreatingReferentialCollection = null;
            _currentCreatingReferential = null;
        }

        /// <summary>
        /// Annule et supprime le référentiel en cours de création.
        /// </summary>
        void DeleteCurrentCreatingReferential()
        {
            if (_currentCreatingReferentialCollection != null && _currentCreatingReferential != null)
            {
                _currentCreatingReferential.IsEditableChanged -= OnIsEditableChanged;
                _currentCreatingReferentialCollection.Remove(_currentCreatingReferential);
            }
        }

        /// <summary>
        /// Met à jour les propriétés UI des référentiels.
        /// </summary>
        void UpdateMultiReferentials()
        {
            _ignoreReferentialsChange = true;

            //  Tout désélectionner
            foreach (IMultipleActionReferential refe in GetAllMultipleReferentials())
            {
                refe.IsSelected = false;
                refe.Quantity = 1;
            }

            if (CurrentActionItem != null)
            {
                // Sélectionner ce qui est actif
                KAction action = CurrentActionItem.Action;
                foreach (IReferentialActionLink la in EnumerableExt.Concat<IReferentialActionLink>(
                    action.Ref1,
                    action.Ref2,
                    action.Ref3,
                    action.Ref4,
                    action.Ref5,
                    action.Ref6,
                    action.Ref7
                ))
                {
                    la.Referential.IsSelected = true;
                    la.Referential.Quantity = la.Quantity;
                }
            }

            _ignoreReferentialsChange = false;
        }

        /// <summary>
        /// S'abonne aux changements sur tous les référentiels de l'action en cours.
        /// </summary>
        void RegisterAllMultipleReferentialsChanged()
        {
            foreach (IMultipleActionReferential refe in GetAllMultipleReferentials())
                RegisterReferentialChanged(refe);
        }

        /// <summary>
        /// Se désabonne aux changements sur tous les référentiels de l'action en cours.
        /// </summary>
        void UnregisterAllMultipleReferentialsChanged()
        {
            foreach (IMultipleActionReferential refe in GetAllMultipleReferentials())
                UnregisterReferentiaChanged(refe);
        }

        /// <summary>
        /// S'abonne aux changements sur le référentiel spécifié.
        /// </summary>
        /// <param name="refe">Le référential</param>
        void RegisterReferentialChanged(IMultipleActionReferential refe)
        {
            refe.IsSelectedChanged += OnReferentialIsSelectedChanged;
            refe.QuantityChanged += OnReferentialQuantityChanged;
        }

        /// <summary>
        /// Se désabonne aux changements sur le référentiel spécifié.
        /// </summary>
        /// <param name="refe">Le référential</param>
        void UnregisterReferentiaChanged(IMultipleActionReferential refe)
        {
            refe.IsSelectedChanged -= OnReferentialIsSelectedChanged;
            refe.QuantityChanged -= OnReferentialQuantityChanged;
        }

        /// <summary>
        /// Appelé lorsque la sélection a changé sur un référentiel.
        /// </summary>
        void OnReferentialIsSelectedChanged(object sender, EventArgs e)
        {
            if (!_ignoreReferentialsChange && CurrentActionItem.Action != null)
            {
                IMultipleActionReferential refe = (IMultipleActionReferential)sender;

                if (refe.IsSelected)
                {
                    bool hasMultipleSelection = IoC.Resolve<IReferentialsUseService>().Referentials[refe.ProcessReferentialId].HasMultipleSelection;
                    if (!hasMultipleSelection)
                    {
                        // Supprimer l'autre lien
                        ReferentialsFactory.ClearReferentialActionLinks(refe.ProcessReferentialId, CurrentActionItem.Action, true);
                    }

                    ReferentialsFactory.CreateReferentialActionLink(refe, CurrentActionItem.Action, 1);
                    refe.Quantity = 1;

                }
                else
                {
                    ReferentialsFactory.DeleteReferentialActionLink(refe, CurrentActionItem.Action);
                }

                UpdateMultiReferentials();

                CanChange = false;
            }
        }

        /// <summary>
        /// Appelé lorsque la quantité a changé sur un référentiel.
        /// </summary>
        void OnReferentialQuantityChanged(object sender, EventArgs e)
        {
            if (!_ignoreReferentialsChange && CurrentActionItem.Action != null)
            {
                IMultipleActionReferential refe = (IMultipleActionReferential)sender;

                IReferentialActionLink link = ReferentialsFactory.GetReferentialActionLink(refe, CurrentActionItem.Action);
                ((IObjectWithChangeTracker)link).StartTracking();
                link.Quantity = refe.Quantity;

                CanChange = false;
            }
        }

        /// <summary>
        /// Obtient tous les référentiels multiples.
        /// </summary>
        /// <returns>Les référentiels multiples.</returns>
        IEnumerable<IMultipleActionReferential> GetAllMultipleReferentials() =>
            EnumerableExt.Concat<IMultipleActionReferential>(Ref1s, Ref2s, Ref3s, Ref4s, Ref5s, Ref6s, Ref7s);

        /// <summary>
        /// Annule les changements sur les liens référentiels multiples et corrige les valeurs des propriétés de navigation dans les liens Actions - Référentiels.
        /// </summary>
        /// <param name="action">L'action où corriger les liens.</param>
        void CancelChangesActionReferentialsAndFixupLinks(KAction action)
        {
            foreach (Ref1Action la in action.Ref1)
            {
                if (la.Referential == null)
                    la.Referential = Ref1s.First(c => c.RefId == la.ReferentialId);
                ObjectWithChangeTrackerExtensions.CancelChanges(la);
            }

            foreach (Ref2Action la in action.Ref2)
            {
                if (la.Referential == null)
                    la.Referential = Ref2s.First(c => c.RefId == la.ReferentialId);
                ObjectWithChangeTrackerExtensions.CancelChanges(la);
            }

            foreach (Ref3Action la in action.Ref3)
            {
                if (la.Referential == null)
                    la.Referential = Ref3s.First(c => c.RefId == la.ReferentialId);
                ObjectWithChangeTrackerExtensions.CancelChanges(la);
            }

            foreach (Ref4Action la in action.Ref4)
            {
                if (la.Referential == null)
                    la.Referential = Ref4s.First(c => c.RefId == la.ReferentialId);
                ObjectWithChangeTrackerExtensions.CancelChanges(la);
            }

            foreach (Ref5Action la in action.Ref5)
            {
                if (la.Referential == null)
                    la.Referential = Ref5s.First(c => c.RefId == la.ReferentialId);
                ObjectWithChangeTrackerExtensions.CancelChanges(la);
            }

            foreach (Ref6Action la in action.Ref6)
            {
                if (la.Referential == null)
                    la.Referential = Ref6s.First(c => c.RefId == la.ReferentialId);
                ObjectWithChangeTrackerExtensions.CancelChanges(la);
            }

            foreach (Ref7Action la in action.Ref7)
            {
                if (la.Referential == null)
                    la.Referential = Ref7s.First(c => c.RefId == la.ReferentialId);
                ObjectWithChangeTrackerExtensions.CancelChanges(la);
            }
        }

        /// <summary>
        /// Supprime tous les référentiels additionnels d'une action.
        /// </summary>
        /// <param name="action">L'action</param>
        void ClearAllMultipleReferentials(KAction action)
        {
            ReferentialsFactory.ClearReferentialActionLinks(ProcessReferentialIdentifier.Ref1, action);
            ReferentialsFactory.ClearReferentialActionLinks(ProcessReferentialIdentifier.Ref2, action);
            ReferentialsFactory.ClearReferentialActionLinks(ProcessReferentialIdentifier.Ref3, action);
            ReferentialsFactory.ClearReferentialActionLinks(ProcessReferentialIdentifier.Ref4, action);
            ReferentialsFactory.ClearReferentialActionLinks(ProcessReferentialIdentifier.Ref5, action);
            ReferentialsFactory.ClearReferentialActionLinks(ProcessReferentialIdentifier.Ref6, action);
            ReferentialsFactory.ClearReferentialActionLinks(ProcessReferentialIdentifier.Ref7, action);
        }

        /// <summary>
        /// Crée des bornes de lecture pour l'action courante.
        /// </summary>
        /// <returns>Les bornes.</returns>
        TimelineNarrow CreateTimelineNarrowForCurrentAction()
        {
            if (CurrentActionItem != null && CurrentVideo != null &&
                CurrentVideoPosition >= CurrentActionItem.Action.Start)
            {
                long finish = CurrentActionItem.Action.Finish;
                if (CurrentActionItem.Action.ChangeTracker.ModifiedValues.ContainsKey("Finish"))
                    finish = (long)CurrentActionItem.Action.ChangeTracker.OriginalValues["Finish"];
                if (CurrentVideoPosition < finish)
                    return new TimelineNarrow()
                    {
                        ActionId = CurrentActionItem.Action.ActionId,
                        Start = CurrentActionItem.Action.Start,
                        Finish = finish//this.CurrentActionItem.Action.Finish,
                    };
            }
            return null;
        }

        #endregion

        #region Types imbriqués

        /// <summary>
        /// Représente une plage sur la timeline.
        /// </summary>
        class TimelineNarrow
        {
            public int ActionId { get; set; }
            public long Start { get; set; }
            public long Finish { get; set; }
        }

        #endregion

    }
}