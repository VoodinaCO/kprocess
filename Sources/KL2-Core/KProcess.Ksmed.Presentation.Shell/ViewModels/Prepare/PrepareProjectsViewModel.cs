using Kprocess.KL2.FileTransfer;
using KProcess.KL2.APIClient;
using KProcess.KL2.Languages;
using KProcess.KL2.SignalRClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Business.Dtos.Export;
using KProcess.Ksmed.Business.Dtos.Prepare;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Interfaces;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Extensions;
using KProcess.Presentation.Windows;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using MoreLinq;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using LocalizationManager = KProcess.Globalization.LocalizationManager;
using Procedure = KProcess.Ksmed.Models.Procedure;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    [HasSelfValidation]
    /// <summary>
    /// Représente le VM de la gestion de projet.
    /// </summary>
    class PrepareProjectsViewModel : FrameContentExtensibleViewModelBase<PrepareProjectsViewModel, IPrepareProjectsViewModel>, IPrepareProjectsViewModel, ISignalRHandle<AnalyzeEventArgs>
    {
        public PrepareProjectsViewModel()
        {
            EventSignalR.Subscribe(this);
        }

        ~PrepareProjectsViewModel()
        {
            EventSignalR.Unsubscribe(this);
        }

        #region Champs privés

        DispatcherTimer _timerTranscoding;
        bool isComputing_Transcoding_Tick;

        BulkObservableCollection<INode> _allProjects;
        INode _currentNode;
        Project _currentProject;
        ProjectDir _currentFolder;
        Procedure _currentProcess;
        Objective[] _objectives;
        string _alternateObjective;
        bool _isAlternateObjectiveChecked;
        Visibility _projectsListVisibility;
        int? _currentGlobalProjectId;
        bool _canChangeProject = true;
        readonly object dragLock = new object();

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
            TimeScales = new TimeScaleContainer[]
            {
                new TimeScaleContainer { Label = LocalizationManager.GetString("VM_PrepareProject_TimeScale_Second"), Value = KnownTimeScales.Second },
                new TimeScaleContainer { Label = LocalizationManager.GetString("VM_PrepareProject_TimeScale_SecondTenth"), Value = KnownTimeScales.SecondTenth },
                new TimeScaleContainer { Label = LocalizationManager.GetString("VM_PrepareProject_TimeScale_SecondHundredth"), Value = KnownTimeScales.SecondHundredth },
                new TimeScaleContainer { Label = LocalizationManager.GetString("VM_PrepareProject_TimeScale_SecondThousandth"), Value = KnownTimeScales.SecondThousandth },
            };
        }

        void SetOldExpandedState(ProjectDir rootNode, List<ProjectDir> flattenProjectDirs, List<Procedure> flattenProcesses)
        {
            foreach (var projectDir in rootNode.Nodes.Flatten(_ => _.Nodes).OfType<ProjectDir>())
            {
                foreach (var oldProjectDir in flattenProjectDirs.Where(_ => _.Id == projectDir.Id))
                {
                    if (oldProjectDir.IsExpanded)
                        projectDir.IsExpanded = true;
                }
            }
            foreach (var process in rootNode.Nodes.Flatten(_ => _.Nodes).OfType<Procedure>())
            {
                foreach (var oldProcess in flattenProcesses.Where(_ => _.ProcessId == process.ProcessId))
                {
                    if (oldProcess.IsExpanded)
                        process.IsExpanded = true;
                }
            }
        }

        async Task ReloadAllProjects()
        {
            AllProcesses?.SelectMany(_ => _.Videos).ForEach(video =>
            {
                video.OnTransferFinished -= Video_OnTransferFinished;
                video.OnTranscodingFinished -= Video_OnTranscodingFinished;
            });

            IPrepareService prepareService = ServiceBus.Get<IPrepareService>();
            ProjectsData data = await prepareService.GetProjects();
            Objectives = data.Objectives;
            Dictionary<Project, ScenarioCriticalPath[]> summaryDict = data.Summary.ToDictionary(x => x.Key, x => x.Value);

            var root = new ProjectDir { Id = -1, Name = IoC.Resolve<ILocalizationManager>().GetString("View_PrepareProject_AllProjects"), ParentId = null, IsExpanded = true };
            root.MarkAsUnchanged();
            root.StopTracking();
            root.Childs = new TrackableCollection<ProjectDir>(data.ProjectsTree.OfType<ProjectDir>());
            root.Processes = new TrackableCollection<Procedure>(data.ProjectsTree.OfType<Procedure>());
            if (AllProjects != null)
                SetOldExpandedState(root, AllProjects.Flatten(_ => _.Nodes).OfType<ProjectDir>().ToList(), AllProjects.Flatten(_ => _.Nodes).OfType<Procedure>().ToList());
            root.StartMonitorNodesChanged();
            AllProjects = new BulkObservableCollection<INode> { root };
            AllProcesses?.SelectMany(_ => _.Videos).ForEach(video =>
            {
                video.OnTransferFinished += Video_OnTransferFinished;
                video.OnTranscodingFinished += Video_OnTranscodingFinished;
            });

            foreach (Project p in data.Projects)
            {
                p.ScenariosCriticalPath = summaryDict[p];
                Project projectInTree = TreeViewHelper.FindProject(AllProjects[0], p.ProjectId);
                if (projectInTree != null)
                    projectInTree.ScenariosCriticalPath = summaryDict[p];
            }

            // Start tracking
            foreach (INode node in AllProjects[0].Nodes)
                StartTracking(node);

            // Sync videos
            await prepareService.SyncVideos(data.Projects.Select(_ => _.ProcessId).Distinct().ToArray());
            RefreshTransferOperations();
            OnPropertyChanged(nameof(RefreshIsSyncing));
        }

        // Used to raise refreshing of all IsSyncing properties
        public bool RefreshIsSyncing { get; private set; }

        void RefreshTransferOperations()
        {
            var allProcesses = AllProcesses;
            foreach (var process in allProcesses)
            {
                bool needRefresh = false;
                foreach (var video in process.Videos)
                {
                    if (!string.IsNullOrEmpty(video.Filename) && FileTransferManager.TransferOperations.ContainsKey(video.Filename))
                    {
                        var transfer = FileTransferManager.TransferOperations[video.Filename];
                        if (transfer?.IsFinished == true)
                        {
                            needRefresh = true;
                            video.Transfer = null;
                        }
                        else
                        {
                            needRefresh |= video.Transfer == null;
                            video.Transfer = FileTransferManager.TransferOperations[video.Filename];
                        }
                    }
                    else
                    {
                        needRefresh |= video.Transfer != null;
                        video.Transfer = null;
                    }
                }
                if (needRefresh)
                    process.OnPropertyChanged(nameof(Procedure.IsSyncing));
            }
        }

        void Video_OnTransferFinished(object sender, JobType e)
        {
            if (sender is Video video)
            {
                var v = AllProcesses.SelectMany(_ => _.Videos)?.SingleOrDefault(_ => _.VideoId == video.VideoId);
                if (v != null)
                {
                    v.Transfer = null;
                    if (e == JobType.Download)
                    {
                        v.OnPropertyChanged(nameof(Video.IsSync));
                        v.Process.OnPropertyChanged(nameof(Procedure.IsSyncing));
                    }
                }
            }
        }

        async void Video_OnTranscodingFinished(object sender, EventArgs e)
        {
            if (sender is Video video)
            {
                var v = AllProcesses.SelectMany(_ => _.Videos)?.SingleOrDefault(_ => _.VideoId == video.VideoId);
                if (v != null)
                {
                    v.ChangeTracker.ChangeTrackingEnabled = false;
                    var refreshedVideo = await ServiceBus.Get<IPrepareService>().GetVideo(v.VideoId);
                    v.Hash = refreshedVideo.Hash;
                    v.Extension = refreshedVideo.Extension;
                    v.OnServer = refreshedVideo.OnServer;
                    v.ChangeTracker.ChangeTrackingEnabled = true;

                    if (v.Sync && !v.IsSync && !FileTransferManager.DownloadOperations.ContainsKey(v.Filename))
                    {
                        var downloadOperation = FileTransferManager.CreateDownload(video.Filename, (remoteFilePath: $"{Preferences.FileServerUri}/GetFile/{v.Filename}", localFilePath: Path.Combine(Preferences.SyncDirectory, v.Filename)));
                        downloadOperation.Resume();
                    }
                    v.OnPropertyChanged(nameof(Video.OnServer));
                    v.OnPropertyChanged(nameof(Video.IsSync));
                    v.Process.OnPropertyChanged(nameof(Procedure.IsSyncing));
                    RefreshTransferOperations();
                }
            }
        }

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override async Task OnLoading()
        {
            IoC.Resolve<IAPIHttpClient>().OnConnecting += OnReconnecting;
            _timerTranscoding = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _timerTranscoding.Tick += TimerTranscoding_Tick;
            _timerTranscoding.Start();

            ShowSpinner();

            FileTransferManager.OnFileAdded += (sender, e) =>
            {
                OnPropertyChanged(nameof(RefreshIsSyncing));
            };

            ProjectInfo currentProject = ServiceBus.Get<IProjectManagerService>().CurrentProject;
            _currentGlobalProjectId = currentProject != null ? (int?)currentProject.ProjectId : null;

            IsCurrentUserAdmin = SecurityContext.HasCurrentUserRole(KnownRoles.Administrator) &&
                SecurityContext.HasCurrentLicenseFeature(Security.Activation.ActivationFeatures.All);

            IsCurrentUserExporter = SecurityContext.HasCurrentUserRole(KnownRoles.Exporter) &&
                SecurityContext.HasCurrentLicenseFeature(Security.Activation.ActivationFeatures.All);

            IsRunningReadOnlyVersion = SecurityContext.HasCurrentLicenseFeature(Security.Activation.ActivationFeatures.ReadOnly);

            try
            {
                await ReloadAllProjects();

                if (_currentGlobalProjectId.HasValue)
                {
                    // On récupère le projet global
                    Project globalProject = TreeViewHelper.FindProject(AllProjects[0], _currentGlobalProjectId.Value);
                    // On étend tous les dossiers parents
                    if (globalProject != null)
                    {
                        ExpandProject(globalProject);

                        CurrentNode = globalProject;
                    }
                    else 
                    {
                        CurrentNode = null;
                        SetCurrentGlobalProject(null);
                    }
                }
                HideSpinner();
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
        }

        async void TimerTranscoding_Tick(object sender, EventArgs e)
        {
            if (isComputing_Transcoding_Tick)
                return;

            isComputing_Transcoding_Tick = true;

            try
            {
                // TODO : migrate SignalR to avoid timeouts
                var progresses = await IoC.Resolve<IAPIHttpClient>().ServiceAsync<Dictionary<string, double>>(KL2_Server.File, null, "GetAllTranscodingProgress", null, "GET");
                var allVideos = AllProcesses?.SelectMany(_ => _.Videos).Where(_ => _.OriginalHash != null) ?? new List<Video>();
                bool needRefresh = false;
                foreach (var video in allVideos)
                {
                    var key = $"TRANSCODED_{video.OriginalHash}{video.Extension}";
                    if (progresses.ContainsKey(key))
                    {
                        needRefresh |= video.TranscodingProgress == null;
                        video.TranscodingProgress = progresses[key];
                    }
                    else
                    {
                        needRefresh |= video.TranscodingProgress != null;
                        video.TranscodingProgress = null;
                    }
                }
                if (needRefresh)
                    OnPropertyChanged(nameof(RefreshIsSyncing));
            }
            catch (TimeoutException ex)
            {
                TraceManager.TraceError(ex, $"{nameof(TimeoutException)}\n");
            }
            catch (ServerNotReacheableException ex)
            {
                TraceManager.TraceError(ex, $"{nameof(ServerNotReacheableException)}\n");
            }
            catch (Exception ex)
            {
                TraceManager.TraceError(ex, $"{nameof(Exception)}\n");
            }
            finally
            {
                isComputing_Transcoding_Tick = false;
            }
        }

        void OnReconnecting(KL2_Server server)
        {
            if (server != KL2_Server.File)
                return;

            FileTransferManager.ResumeAllJobOnError();
            RefreshTransferOperations();
        }

        void StartTracking(INode root)
        {
            if (root is ProjectDir projectDir)
            {
                projectDir.StartMonitorNodesChanged();
                projectDir.StartTracking();
                projectDir.Childs.ForEach(StartTracking);
                projectDir.Processes.ForEach(StartTracking);
            }
            else if (root is Procedure process)
            {
                process.StartMonitorNodesChanged();
                process.StartTracking();
                process.Projects.ForEach(_ => _.StartTracking());
            }
            else if (root is Project project)
            {
                project.StartMonitorNodesChanged();
                project.StartTracking();
            }
        }

        INode GetParentFolderNode(INode node)
        {
            if (node == null)
                return null;
            if (node is Project project)
                return project.Process.ProjectDir;
            if (node is Procedure process)
                return process.ProjectDir;
            if (node is ProjectDir projectDir)
                return projectDir.Parent;
            return null;
        }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override Task OnInitializeDesigner()
        {
            Objectives = DesignData.GeneratesObjectives().ToArray();

            ProjectsListVisibility = Visibility.Visible;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        /// <returns><c>true</c> si la navigation est acceptée.</returns>
        public override Task<bool> OnNavigatingAway(IFrameNavigationToken token)
        {
            if (CurrentProject != null &&
                (!CanChangeProject || CurrentProject.IsNotMarkedAsUnchanged && !CurrentProject.IsValid.GetValueOrDefault()))
            {
                MessageDialogResult answer = this.AskNavigatingAwayValidationConfirmation(() => _navigationToken = token);
                if (answer == MessageDialogResult.Yes)
                {
                    IoC.Resolve<IAPIHttpClient>().OnConnecting -= OnReconnecting;
                    AllProcesses?.SelectMany(_ => _.Videos).ForEach(video =>
                    {
                        video.OnTransferFinished -= Video_OnTransferFinished;
                        video.OnTranscodingFinished -= Video_OnTranscodingFinished;
                    });
                    _timerTranscoding?.Stop();
                    _timerTranscoding = null;
                }
                return Task.FromResult(answer == MessageDialogResult.No);
            }

            IoC.Resolve<IAPIHttpClient>().OnConnecting -= OnReconnecting;
            AllProcesses?.SelectMany(_ => _.Videos).ForEach(video =>
            {
                video.OnTransferFinished -= Video_OnTransferFinished;
                video.OnTranscodingFinished -= Video_OnTranscodingFinished;
            });
            _timerTranscoding?.Stop();
            _timerTranscoding = null;
            return Task.FromResult(true);
        }

        /// <summary>
        /// Appelé afin de nettoyer les ressources.
        /// </summary>
        protected override void OnCleanup()
        {
            base.OnCleanup();

            if (CurrentProject != null)
                CurrentProject.PropertyChanged -= OnProjectPropertyChanged;
        }

        /// <summary>
        /// Lorsque surchargé dans une classe fille, cette méthode sert à exécuter une validation personnalisée.
        /// </summary>
        /// <returns>
        /// Une énumération des erreurs de validation, ou null s'il n'y en a pas.
        /// </returns>
        [SelfValidation]
        protected override IEnumerable<ValidationError> OnCustomValidate()
        {
            if (CurrentProject != null && CurrentProject.Errors != null)
            {
                IEnumerable<ValidationError> otherObjectiveErrors = CurrentProject.Errors.Where(e => e.Key == "OtherObjectiveLabel");
                foreach (ValidationError error in otherObjectiveErrors)
                    yield return new ValidationError(nameof(AlternateObjective), error.Message);
            }
        }

        #endregion

        #region Propriétés

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
                        CurrentFolder = null;
                        CurrentProcess = null;
                        CurrentProject = null;
                    }
                    else if (_currentNode is ProjectDir projectDir)
                    {
                        CurrentFolder = projectDir?.Id == -1 ? null : projectDir;
                        CurrentProcess = null;
                        CurrentProject = null;
                    }
                    else if (_currentNode is Procedure process)
                    {
                        CurrentFolder = null;
                        CurrentProcess = process;
                        CurrentProject = null;
                    }
                    else if (_currentNode is Project project)
                    {
                        CurrentFolder = null;
                        CurrentProcess = null;
                        CurrentProject = project;
                    }

                    if (_currentNode != null)
                        _currentNode.IsSelected = true;

                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le projet.
        /// </summary>
        public Project CurrentProject
        {
            get { return _currentProject; }
            set
            {
                if (_currentProject != value)
                {
                    Project previous = _currentProject;
                    _currentProject = value;
                    ServiceBus.Get<IProjectManagerService>().SynchronizeProjectObjectivesInfo(value);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanUpdateProject));
                    OnPropertyChanged(nameof(IsCurrentProjectOpened));
                    OnCurrentProjectChanged(previous, _currentProject);

                    /*if(_currentProject != null)
                        IsAbandonProject = _currentProject.IsAbandoned;*/
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le répertoire.
        /// </summary>
        public ProjectDir CurrentFolder
        {
            get { return _currentFolder; }
            set
            {
                if (_currentFolder != value)
                {
                    ProjectDir previous = _currentFolder;
                    _currentFolder = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanUpdateFolder));
                    RemoveFolderCommand.Invalidate();
                    OnCurrentFolderChanged(previous, _currentFolder);
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le process.
        /// </summary>
        public Procedure CurrentProcess
        {
            get => _currentProcess;
            set
            {
                if (_currentProcess == value)
                    return;
                Procedure previous = _currentProcess;
                _currentProcess = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanUpdateProcess));
                AddCommand.Invalidate();
                OnCurrentProcessChanged(previous, _currentProcess);
            }
        }

        /// <summary>
        /// Obtient ou définit les objectifs disponibles.
        /// </summary>
        public Objective[] Objectives
        {
            get { return _objectives; }
            set
            {
                if (_objectives != value)
                {
                    _objectives = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit l''objectif additionel.
        /// </summary>
        public string AlternateObjective
        {
            get { return _alternateObjective; }
            set
            {
                if (_alternateObjective != value)
                {
                    _alternateObjective = value;
                    OnPropertyChanged();

                    IsAlternateObjectiveChecked |= !string.IsNullOrEmpty(value);

                    UpdateAlternateObj();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'objectif additionel est coché.
        /// </summary>
        public bool IsAlternateObjectiveChecked
        {
            get { return _isAlternateObjectiveChecked; }
            set
            {
                if (_isAlternateObjectiveChecked != value)
                {
                    _isAlternateObjectiveChecked = value;
                    OnPropertyChanged();
                    UpdateAlternateObj();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit tous les projets.
        /// </summary>
        public BulkObservableCollection<INode> AllProjects
        {
            get { return _allProjects; }
            set
            {
                if (_allProjects != value)
                {
                    _allProjects = value;
                    OnPropertyChanged();
                }
            }
        }

        List<Procedure> AllProcesses =>
            AllProjects?.Flatten(_ => _.Nodes).OfType<Procedure>().ToList() ?? new List<Procedure>();

        /// <summary>
        /// Obtient ou définit la visibilité de la liste des projets.
        /// </summary>
        public Visibility ProjectsListVisibility
        {
            get { return _projectsListVisibility; }
            set
            {
                if (_projectsListVisibility != value)
                {
                    _projectsListVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit une propriété indiquant si l'utilisateur est authorisé à changer de projet.
        /// </summary>
        public bool CanChangeProject
        {
            get { return _canChangeProject; }
            private set
            {
                if (_canChangeProject != value)
                {
                    _canChangeProject = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le project courant est ouvert.
        /// </summary>
        public bool IsCurrentProjectOpened =>
            CurrentProject?.ProjectId == _currentGlobalProjectId;

        /// <summary>
        /// Obtient une valeur indiquant si l'utilisateur courant est un administrateur.
        /// </summary>
        public bool IsCurrentUserAdmin { get; private set; }

        /// <summary>
        /// Obtient une valeur indiquant si l'utilisateur courant est un exporteur.
        /// </summary>
        public bool IsCurrentUserExporter { get; private set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la licence actuelle est en lecture seule.
        /// </summary>
        public bool IsRunningReadOnlyVersion { get; private set; }

        /// <summary>
        /// Obtient une valeur indiquant si les projets peuvent être importés.
        /// </summary>
        public bool CanImportProject =>
            IsRunningReadOnlyVersion || IsCurrentUserAdmin || IsCurrentUserExporter;

        /// <summary>
        /// Obtient une valeur indiquant si les projets peuvent être exportés.
        /// </summary>
        public bool CanExportProject =>
            IsRunningReadOnlyVersion || IsCurrentUserAdmin || IsCurrentUserExporter;

        /// <summary>
        /// Obtient une valeur indiquant si les dossiers peuvent être créés.
        /// </summary>
        public bool CanAddFolder =>
            ProjectDirSecurity.CanAdd();

        /// <summary>
        /// Obtient une valeur indiquant si le dossier courant peut être modifié.
        /// </summary>
        public bool CanUpdateFolder =>
            ProjectDirSecurity.CanUpdate();

        /// <summary>
        /// Obtient une valeur indiquant si les processes peuvent être créés.
        /// </summary>
        public bool CanAddProcess =>
            ProcedureSecurity.CanAdd();

        /// <summary>
        /// Obtient une valeur indiquant si le process courant peut être modifié.
        /// </summary>
        public bool CanUpdateProcess =>
            CurrentProcess != null
            && SecurityContext.CurrentUser != null
            && (CurrentProcess.IsMarkedAsAdded
                || ProcedureSecurity.CanUpdate(CurrentProcess));

        /// <summary>
        /// Obtient une valeur indiquant si les projets peuvent être créés.
        /// </summary>
        public bool CanAddProject =>
            ProjectSecurity.CanAdd(CurrentProcess);

        /// <summary>
        /// Obtient une valeur indiquant si le projet courant peut être modifié.
        /// </summary>
        public bool CanUpdateProject =>
            ProjectSecurity.CanUpdate(CurrentProject);

        /// <summary>
        /// Obtient les échelles de temps possibles.
        /// </summary>
        public TimeScaleContainer[] TimeScales { get; private set; }

        /*private bool _isAbandonedProject;
        public bool IsAbandonProject
        {
            get { return _isAbandonedProject; }
            private set
            {
                if (_isAbandonedProject != value)
                {
                    _isAbandonedProject = value;
                    OnPropertyChanged();
                }
            }
        }*/

        #endregion

        #region Commandes

        INode DraggedNode;
        ProjectDir DropNode;

        Command<DragTreeViewItemAdvEventArgs> _dragStartCommand;
        /// <summary>
        /// Obtient la commande exécutée lors d'un drag.
        /// </summary>
        public ICommand DragStartCommand
        {
            get
            {
                if (_dragStartCommand == null)
                    _dragStartCommand = new Command<DragTreeViewItemAdvEventArgs>((e) =>
                    {
                        if (e.DraggingItems.First().DataContext is Project project)
                            e.AllowDragDrop = false;
                        else if (e.DraggingItems.First().DataContext is ProjectDir folder)
                        {
                            if (folder.Id == -1)
                                e.AllowDragDrop = false;
                            else
                                DraggedNode = folder;
                        }
                        else if (e.DraggingItems.First().DataContext is Procedure process)
                            DraggedNode = process;
                    });
                return _dragStartCommand;
            }
        }

        Command<DragTreeViewItemAdvEventArgs> _dragEndCommand;
        /// <summary>
        /// Obtient la commande exécutée lors d'un drop.
        /// </summary>
        public ICommand DragEndCommand
        {
            get
            {
                if (_dragEndCommand == null)
                    _dragEndCommand = new Command<DragTreeViewItemAdvEventArgs>((e) =>
                    {
                        ProjectDir TargetDropNode = null;
                        if ((e.TargetDropItem is TreeViewItemAdv _treeViewItemAdv && _treeViewItemAdv.DataContext is ProjectDir)
                            || (e.TargetDropItem is TreeViewAdv _treeViewAdv && _treeViewAdv.Name == "ProjectsTreeview"))
                        {
                            int? projectDirId = null;
                            if (e.TargetDropItem is TreeViewItemAdv treeViewItemAdv)
                                TargetDropNode = treeViewItemAdv.DataContext as ProjectDir;
                            else
                                TargetDropNode = AllProjects.First() as ProjectDir;
                            if (TargetDropNode.Id != -1)
                                projectDirId = TargetDropNode.Id;
                            if (DraggedNode is Procedure draggedProcess)
                            {
                                if (draggedProcess.ProjectDirId == projectDirId) // Le dossier parent est le même
                                {
                                    DropNode = null;
                                    e.Cancel = true;
                                }
                                else
                                {
                                    DropNode = TargetDropNode;
                                    e.Handled = true;
                                }
                            }
                            else if (DraggedNode is ProjectDir draggedFolder)
                            {
                                if (draggedFolder.ParentId == projectDirId) // Le dossier parent est le même
                                {
                                    DropNode = null;
                                    e.Cancel = true;
                                }
                                else
                                {
                                    DropNode = TargetDropNode;
                                    e.Handled = true;
                                }
                            }
                        }
                        else // Le même niveau que la racine (on empêche cela)
                        {
                            e.Cancel = true;
                            return;
                        }
                    });
                return _dragEndCommand;
            }
        }

        Command<DragTreeViewItemAdvEventArgs> _dragCompletedCommand;
        /// <summary>
        /// Obtient la commande exécutée lors d'un drop.
        /// </summary>
        public ICommand DragCompletedCommand
        {
            get
            {
                if (_dragCompletedCommand == null)
                    _dragCompletedCommand = new Command<DragTreeViewItemAdvEventArgs>(async (e) =>
                    {
                        if (DropNode != null)
                        {
                            try
                            {
                                if (DraggedNode is Procedure draggedProcess)
                                {
                                    draggedProcess.ProjectDir?.Processes.Remove(draggedProcess);
                                    DropNode.Processes.Add(draggedProcess);
                                    if (!await SaveProcess(draggedProcess, false))
                                        throw new Exception();
                                    CanChangeProject = true;
                                }
                                else if (DraggedNode is ProjectDir draggedFolder)
                                {
                                    draggedFolder.Parent?.Childs.Remove(draggedFolder);
                                    DropNode.Childs.Add(draggedFolder);
                                    if (!await SaveFolder(draggedFolder, false))
                                        throw new Exception();
                                    CanChangeProject = true;
                                }
                            }
                            catch
                            {
                                // On recharge l'arborescence
                                await ReloadAllProjects();
                                INode nodeInTree = null;
                                if (DraggedNode is Procedure draggedProcess)
                                {
                                    // On retrouve l'item
                                    nodeInTree = TreeViewHelper.FindProcess(AllProjects[0], draggedProcess.ProcessId);
                                    // On le sélectionne
                                    CurrentNode = nodeInTree;
                                    // On ouvre l'arbre
                                    /*if ((nodeInTree as Procedure)?.ProjectDir is ProjectDir aFolder)
                                        TreeViewHelper.ExpandFolder(aFolder);*/
                                }
                                else if (DraggedNode is ProjectDir draggedFolder)
                                {
                                    // On retrouve l'item
                                    nodeInTree = TreeViewHelper.FindFolder(AllProjects[0], draggedFolder.Id);
                                    // On le sélectionne
                                    CurrentNode = nodeInTree;
                                    // On ouvre l'arbre
                                    /*if (nodeInTree is ProjectDir aFolder)
                                        TreeViewHelper.ExpandFolder(aFolder);*/
                                }

                                HideSpinner();
                                CanChangeProject = true;
                            }
                        }
                    });
                return _dragCompletedCommand;
            }
        }

        Command _openCommand;
        /// <summary>
        /// Obtient la commande permettant d'ouvrir la liste des projets.
        /// </summary>
        public ICommand OpenCommand
        {
            get
            {
                if (_openCommand == null)
                    _openCommand = new Command(() =>
                    {
                        ProjectsListVisibility = ProjectsListVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
                    });
                return _openCommand;
            }
        }

        /// <summary>
        /// Obtient la commande permettant de gérer la synchronisation des vidéos d'un process
        /// </summary>
        Command _syncChangeCommand;
        public ICommand SyncChangeCommand
        {
            get
            {
                if (_syncChangeCommand == null)
                    _syncChangeCommand = new Command(async () =>
                    {
                        if (CurrentProcess == null)
                            return;

                        CanChangeProject = false;

                        VideoSync videoSync = null;
                        if (CurrentProcess.VideoSyncs.Any()) // VideoSync already exists, update it
                        {
                            videoSync = CurrentProcess.VideoSyncs.First();
                            videoSync.SyncValue = !CurrentProcess.SyncVideo;
                        }
                        else // VideoSync doesn't exist, create it
                        {
                            videoSync = new VideoSync
                            {
                                UserId = IoC.Resolve<ISecurityContext>().CurrentUser.User.UserId,
                                SyncValue = !CurrentProcess.SyncVideo
                            };
                            CurrentProcess.VideoSyncs.Add(videoSync);
                        }
                        await SaveProcess(CurrentProcess, true, false);

                        CanChangeProject = true;
                    }, () => CanChangeProject);
                return _syncChangeCommand;
            }
        }

        protected override bool OnAddCommandCanExecute() =>
            CanChangeProject
            && CanAddProject
            && CurrentProcess.Projects.All(p => p.Scenarios.Any(s => s.NatureCode == KnownScenarioNatures.Realized && s.StateCode == KnownScenarioStates.Validated));

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande AddCommand
        /// </summary>
        protected override async void OnAddCommandExecute()
        {
            ShowSpinner();

            // Si un projet existe déjà, on crée un nouveau projet depuis le scénario de validation du projet le plus récent
            if (CurrentProcess.Projects.Any())
            {
                try
                {
                    var lastProject = CurrentProcess.Projects.MaxWithValue(_ => _.RealEndDate.Value);
                    var validatedScenario = lastProject.Scenarios.Single(_ => _.NatureCode == KnownScenarioNatures.Realized);
                    int p = await ServiceBus.Get<IPrepareService>().CreateNewProjectFromValidatedScenario(lastProject.ProjectId, validatedScenario);

                    // On recharge l'arborescence
                    await ReloadAllProjects();
                    // On retrouve le projet créé
                    Project projectInTree = TreeViewHelper.FindProject(AllProjects[0], p);
                    // On le sélectionne
                    CurrentNode = projectInTree;
                    // On ouvre l'arbre
                    //ExpandProject(projectInTree);

                    await SignalRFactory.GetSignalR<IKL2AnalyzeHubConnect>()
                        .RefreshProcess(new AnalyzeEventArgs("Add child project"));

                    HideSpinner();

                    SetCurrentGlobalProject(CurrentProject);
                    CanChangeProject = true;
                    if (_navigationToken?.IsValid == true)
                        _navigationToken.Navigate();
                }
                catch (Exception e)
                {
                    base.OnError(e);
                }
            }
            else
            {
                try
                {
                    Project newProject = new Project
                    {
                        StartDate = DateTime.Now,
                        ObjectiveCode = Objectives.First().ObjectiveCode,
                        TimeScale = KnownTimeScales.SecondTenth
                    };
                    newProject.ChangeTracker.ChangeTrackingEnabled = false;
                    newProject.Objective = Objectives.First();
                    newProject.ChangeTracker.ChangeTrackingEnabled = true;
                    CurrentProcess.Projects.Add(newProject);
                    if (!CurrentProcess.IsExpanded)
                        CurrentProcess.IsExpanded = true;

                    // Create Initial scenario if needed
                    var newScenario = new Scenario()
                    {
                        Label = LocalizationManager.GetString("Business_AnalyzeService_InitialScenarioLabel"),
                        StateCode = KnownScenarioStates.Draft,
                        NatureCode = KnownScenarioNatures.Initial,
                        IsShownInSummary = true,
                    };
                    newProject.Scenarios.Add(newScenario);
                    newProject.ScenariosDescriptions = new[] { new ScenarioDescription(newScenario) };

                    CurrentNode = newProject;
                    CanChangeProject = false;

                    HideSpinner();
                }
                catch (Exception e)
                {
                    base.OnError(e);
                }
            }
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande RemoveCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnRemoveCommandCanExecute() =>
            CanChangeProject
            && ProjectSecurity.CanDelete(CurrentProject);

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande RemoveCommand
        /// </summary>
        protected override async void OnRemoveCommandExecute()
        {
            if (!DialogFactory.GetDialogView<ICommonDialog>().ShowSureToDelete())
                return;
            await DeleteProject(CurrentProject);
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande RemoveFolderCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnRemoveFolderCommandCanExecute() =>
            CanChangeProject
            && CurrentFolder != null
            && CurrentFolder.Id != -1
            && ProjectDirSecurity.CanDelete()
            && FolderCanBeDeleted(CurrentFolder);

        bool FolderCanBeDeleted(ProjectDir folder)
        {
            bool processes_ok = folder.Processes.All(_ => !(_.Projects.Any() || _.Projects.Any(p => p.Scenarios.Any())));
            bool subfolders_ok = folder.Childs.All(FolderCanBeDeleted);
            return processes_ok && subfolders_ok;
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande RemoveFolderCommand
        /// </summary>
        protected override async void OnRemoveFolderCommandExecute()
        {
            if (!DialogFactory.GetDialogView<ICommonDialog>().ShowSureToDelete())
                return;
            await DeleteFolder(CurrentFolder);
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande RemoveProcessCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnRemoveProcessCommandCanExecute() =>
            CanChangeProject
            && ProcedureSecurity.CanDelete(CurrentProcess)
            && ProcessCanBeDeleted(CurrentProcess);

        bool ProcessCanBeDeleted(Procedure process) =>
            process.Projects.All(_ => !_.Scenarios.Any());

        Task<bool> ProcessHasPublication(Procedure process) =>
            ServiceBus.Get<IPrepareService>().PublicationExistsForProcess(process.ProcessId);

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande RemoveProcessCommand
        /// </summary>
        protected override async void OnRemoveProcessCommandExecute()
        {
            if (await ProcessHasPublication(CurrentProcess))
            {
                if (DialogFactory.GetDialogView<IMessageDialog>().Show(
                        LocalizationManager.GetString("Common_Warning_DeleteProcessWithPublication"),
                        LocalizationManager.GetString("Common_Confirm"),
                        MessageDialogButton.YesNoCancel,
                        MessageDialogImage.Warning)
                    != MessageDialogResult.Yes)
                    return;
            }
            else if (!DialogFactory.GetDialogView<ICommonDialog>().ShowSureToDelete())
                return;
            await DeleteProcess(CurrentProcess);
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande ValidateCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnValidateCommandCanExecute() =>
            CurrentNode != null;

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande CancelCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnCancelCommandCanExecute() =>
            !CanChangeProject && CurrentNode != null;

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande ValidateCommand
        /// </summary>
        protected override async void OnValidateCommandExecute()
        {
            base.OnValidateCommandExecute();

            if (CurrentProject != null)
            {
                // Vérfier si l'objet a changé
                if (CurrentProject.IsNotMarkedAsUnchanged)
                {
                    // Valider l'objet
                    if (!ValidateProject())
                        return;

                    try
                    {
                        if (await SaveProject())
                        {
                            CurrentProject.ScenariosDescriptions = CurrentProject.Scenarios.Select(sc => new ScenarioDescription(sc)).OrderBy(sd => sd.Id).ToArray();
                            SetCurrentGlobalProject(CurrentProject);
                            CanChangeProject = true;
                            if (_navigationToken?.IsValid == true)
                                _navigationToken.Navigate();
                        }
                    }
                    catch (Exception e)
                    {
                        base.OnError(e);
                    }
                }
                else
                {
                    // Si le projet est différent de celui actuel dans l'application
                    if (!_currentGlobalProjectId.HasValue || _currentGlobalProjectId.Value != CurrentProject.ProjectId)
                    {
                        SetCurrentGlobalProject(CurrentProject);
                        CanChangeProject = true;
                    }
                }
            }
            else if (CurrentFolder != null)
            {
                // Vérfier si l'objet a changé
                if (CurrentFolder.IsNotMarkedAsUnchanged)
                {
                    // Valider l'objet
                    if (!ValidateFolder())
                        return;

                    if (await SaveFolder(CurrentFolder))
                    {
                        CanChangeProject = true;
                        if (_navigationToken?.IsValid == true)
                            _navigationToken.Navigate();
                    }
                }
                else
                {
                    // Si le projet est différent de celui actuel dans l'application
                    if (!_currentGlobalProjectId.HasValue || (CurrentProject != null && _currentGlobalProjectId.Value != CurrentProject.ProjectId))
                    {
                        SetCurrentGlobalProject(CurrentProject);
                        CanChangeProject = true;
                    }
                }
            }
            else if (CurrentProcess != null)
            {
                // Vérfier si l'objet a changé
                if (CurrentProcess.IsNotMarkedAsUnchanged)
                {
                    // Valider l'objet
                    if (!ValidateProcess())
                        return;

                    if (await SaveProcess(CurrentProcess))
                    {
                        CanChangeProject = true;
                        if (_navigationToken?.IsValid == true)
                            _navigationToken.Navigate();
                    }
                }
                else
                {
                    // Si le projet est différent de celui actuel dans l'application
                    if (!_currentGlobalProjectId.HasValue || _currentGlobalProjectId.Value != CurrentProject.ProjectId)
                    {
                        SetCurrentGlobalProject(CurrentProject);
                        CanChangeProject = true;
                    }
                }
            }
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande CancelCommand
        /// </summary>
        protected override async void OnCancelCommandExecute()
        {
            if (_currentProject != null)
            {
                // Si le projet est nouveau, simplement le supprimer
                if (_currentProject.IsMarkedAsAdded)
                    await DeleteProject(_currentProject);
                else
                    ReloadProject();
            }
            else if (CurrentFolder != null)
            {
                // Si le dossier est nouveau, simplement le supprimer
                if (_currentFolder.IsMarkedAsAdded)
                    await DeleteFolder(_currentFolder);
                else
                    ReloadFolder();
            }
            else if (_currentProcess != null)
            {
                // Si le process est nouveau, simplement le supprimer
                if (_currentProcess.IsMarkedAsAdded)
                    await DeleteProcess(_currentProcess);
                else
                    ReloadProcess();
            }
            CanChangeProject = true;
            HideValidationErrors();
            Validate();
        }

        Command _openProjectCommand;
        /// <summary>
        /// Obtient la commande permettant d'ouvrir le projet sélectionné (si celui-ci n'est pas modifié).
        /// </summary>
        public ICommand OpenProjectCommand
        {
            get
            {
                if (_openProjectCommand == null)
                    _openProjectCommand = new Command(() =>
                    {
                        ValidateCommand.Execute(null);
                    }, () => CurrentProject?.IsNotMarkedAsUnchanged == true);
                return _openProjectCommand;
            }
        }

        /*ICommand _changeAbandonedCommand;
        public ICommand ChangeAbandonedCommand
        {
            get
            {
                if (_changeAbandonedCommand == null)
                    _changeAbandonedCommand = new Command(() =>
                    {
                        string mess = CurrentProject.IsAbandoned ?
                            LocalizationManager.GetString("View_PrepareProject_Recover_Ask") :
                            LocalizationManager.GetString("View_PrepareProject_Abandone_Ask");

                        var result = DialogFactory.GetDialogView<IMessageDialog>().Show(string.Format(mess, CurrentProject.Label),
                            LocalizationManager.GetString("Common_Project"), MessageDialogButton.YesNo, MessageDialogImage.Question);
                        if (result == MessageDialogResult.Yes)
                        {
                            CurrentProject.IsAbandoned = !CurrentProject.IsAbandoned;
                            if (OnValidateCommandCanExecute())
                                OnValidateCommandExecute();
                        }

                    }, () => CurrentProject != null);
                return _changeAbandonedCommand;
            }
        }*/

        void ExpandProject(Project project)
        {
            project.Process.IsExpanded = true;
            ProjectDir parentFolder = project.Process.ProjectDir;
            while (parentFolder != null)
            {
                parentFolder.IsExpanded = true;
                parentFolder = parentFolder.Parent;
            }
        }

        void SetIsExpanded(INode treeviewItem, bool expand)
        {
            if (treeviewItem is ProjectDir projectDir)
            {
                projectDir.Childs.ForEach(_ => SetIsExpanded(_, expand));
                projectDir.Processes.ForEach(_ => SetIsExpanded(_, expand));
            }
            else if (treeviewItem is Procedure process)
                process.Projects.ForEach(_ => SetIsExpanded(_, expand));
            else if (treeviewItem is Project project)
                project.IsExpanded = expand;
            treeviewItem.IsExpanded = expand;
        }

        Command<TreeViewAdv> _collapseAllCommand;
        /// <summary>
        /// Obtient la commande permettant de réduire tous les dossiers.
        /// </summary>
        public ICommand CollapseAllCommand
        {
            get
            {
                if (_collapseAllCommand == null)
                    _collapseAllCommand = new Command<TreeViewAdv>((treeview) =>
                    {
                        foreach (INode treeviewItem in treeview.Items)
                            SetIsExpanded(treeviewItem, false);
                    },
                    (treeview) => CanChangeProject && CurrentProject == null);
                return _collapseAllCommand;
            }
        }

        Command<TreeViewAdv> _expandAllCommand;
        /// <summary>
        /// Obtient la commande permettant d'étendre tous les dossiers.
        /// </summary>
        public ICommand ExpandAllCommand
        {
            get
            {
                if (_expandAllCommand == null)
                    _expandAllCommand = new Command<TreeViewAdv>((treeview) =>
                    {
                        foreach (INode treeviewItem in treeview.Items)
                            SetIsExpanded(treeviewItem, true);
                    },
                    (treeview) => CanChangeProject && CurrentProject == null);
                return _expandAllCommand;
            }
        }

        Command _addProcessCommand;
        /// <summary>
        /// Obtient la commande permettant d'ajouter un nouveau process.
        /// </summary>
        public ICommand AddProcessCommand
        {
            get
            {
                if (_addProcessCommand == null)
                    _addProcessCommand = new Command(() =>
                    {
                        Procedure newProcess = new Procedure();
                        if (CurrentFolder == null)
                            (AllProjects.First() as ProjectDir).Processes.Add(newProcess);
                        else
                        {
                            CurrentFolder.Processes.Add(newProcess);
                            TreeViewHelper.ExpandFolder(CurrentFolder);
                        }

                        CurrentNode = newProcess;

                        CanChangeProject = false;
                    },
                    () => CurrentProcess == null
                          && CurrentProject == null
                          && (CurrentFolder == null || CurrentFolder.IsMarkedAsUnchanged)
                          && CanAddProcess);
                return _addProcessCommand;
            }
        }

        Command _addFolderCommand;
        /// <summary>
        /// Obtient la commande permettant de créer un dossier.
        /// </summary>
        public ICommand AddFolderCommand
        {
            get
            {
                if (_addFolderCommand == null)
                    _addFolderCommand = new Command(() =>
                    {
                        ProjectDir newFolder = new ProjectDir();
                        if (CurrentFolder == null)
                            (AllProjects.First() as ProjectDir).Childs.Add(newFolder);
                        else
                        {
                            CurrentFolder.Childs.Add(newFolder);
                            TreeViewHelper.ExpandFolder(CurrentFolder);
                        }

                        CurrentNode = newFolder;

                        CanChangeProject = false;
                    },
                    () => CurrentProcess == null 
                          && CurrentProject == null 
                          && (CurrentFolder == null || CurrentFolder.IsMarkedAsUnchanged)
                          && CanAddFolder);
                return _addFolderCommand;
            }
        }

        Command _exportProjectCommand;
        /// <summary>
        /// Obtient la commande permettant d'exporter un projet.
        /// </summary>
        public ICommand ExportProjectCommand
        {
            get
            {
                if (_exportProjectCommand == null)
                    _exportProjectCommand = new Command(async () =>
                    {

                        string fileName = DialogFactory.GetDialogView<IExportDialog>().ExportProject();

                        if (string.IsNullOrEmpty(fileName))
                            return;

                        try
                        {
                            ShowSpinner();

                            try
                            {
                                Stream s = await ServiceBus.Get<IImportExportService>().ExportProject(CurrentProject.ProjectId);

                                using (FileStream fs = File.Create(fileName))
                                {
                                    await s.CopyToAsync(fs, StreamExtensions.BufferSize);
                                }

                                HideSpinner();
                            }
                            catch (Exception e)
                            {
                                base.OnError(e);
                            }
                        }
                        catch (Exception e)
                        {
                            TraceManager.TraceError(e, e.Message);
                            DialogFactory.GetDialogView<IErrorDialog>().Show(
                                LocalizationManager.GetString("VM_PrepareProject_Message_ErrorDuringExport"),
                                LocalizationManager.GetString("Common_Error"), e);
                        }


                    }, () => CurrentProject != null && CanExportProject && CanChangeProject);
                return _exportProjectCommand;
            }
        }

        Command _importProjectCommand;
        /// <summary>
        /// Obtient la commande permettant d'importer un projet.
        /// </summary>
        public ICommand ImportProjectCommand
        {
            get
            {
                if (_importProjectCommand == null)
                    _importProjectCommand = new Command(async () =>
                    {

                        ImportWithVideoFolderResult result = DialogFactory.GetDialogView<IExportDialog>().ImportProject();

                        if (!result.Accepts)
                            return;

                        try
                        {
                            ShowSpinner();

                            byte[] fileContent = File.ReadAllBytes(result.Filename);

                            try
                            {
                                ProjectImport pi = await ServiceBus.Get<IImportExportService>().PredictMergedReferentialsProject(fileContent);

                                bool mergeReferentials;

                                if (pi.ProjectReferentialsMergeCandidates.Count > 0 || pi.StandardReferentialsMergeCandidates.Count > 0)
                                {
                                    string mergedReferentialsLabels = string.Join(Environment.NewLine,
                                        pi.ProjectReferentialsMergeCandidates.Keys.Select(r => r.Label)
                                            .Union(pi.StandardReferentialsMergeCandidates.Keys.Select(r => r.Label))
                                            .Distinct()
                                            .OrderBy(l => l));

                                    MessageDialogResult dialogResult = DialogFactory.GetDialogView<IMessageDialog>().Show(
                                        LocalizationManager.GetStringFormat("VM_PrepareProject_Message_ImportProjectMergeReferentials", mergedReferentialsLabels),
                                        LocalizationManager.GetString("VM_PrepareProject_Message_ImportProjectMergeReferentials_Title"),
                                        MessageDialogButton.YesNoCancel,
                                        MessageDialogImage.Question);

                                    switch (dialogResult)
                                    {
                                        case MessageDialogResult.Yes:
                                            mergeReferentials = true;
                                            break;
                                        case MessageDialogResult.No:
                                            mergeReferentials = false;
                                            break;
                                        default:
                                            HideSpinner();
                                            return;
                                    }
                                }
                                else
                                    mergeReferentials = false;

                                // On définit le process du projet importé
                                pi.ExportedProject.Project.Process = CurrentProcess;

                                try
                                {
                                    Project p = await ServiceBus.Get<IImportExportService>().ImportProject(pi, mergeReferentials, result.VideosFolder);

                                    HideSpinner();
                                    await Load();
                                }
                                catch (Exception e)
                                {
                                    base.OnError(e);
                                }
                            }
                            catch (Exception e)
                            {
                                base.OnError(e);
                            }
                        }
                        catch (Exception e)
                        {
                            TraceManager.TraceError(e, e.Message);
                            DialogFactory.GetDialogView<IErrorDialog>().Show(
                                LocalizationManager.GetString("VM_PrepareProject_Message_ErrorDuringimport"),
                                LocalizationManager.GetString("Common_Error"), e);
                        }

                    }, () => CanImportProject && CanChangeProject && CurrentProcess != null);
                return _importProjectCommand;
            }
        }

        Command _exportExcelCommand;
        /// <summary>
        /// Obtient la commande permettant d'exporter le projet sélectionné vers excel.
        /// </summary>
        public ICommand ExportExcelCommand
        {
            get
            {
                if (_exportExcelCommand == null)
                    _exportExcelCommand = new Command(async () =>
                    {


                        // Affiche la fenêtre de confirmation
                        ExportResult result = DialogFactory.GetDialogView<IExportDialog>().ShowExportToExcel(ExcelFormat.Xlsm);

                        // On sauvegarde
                        if (!result.Accepts)
                            return;

                        // on récupère les données
                        ShowSpinner();

                        try
                        {
                            RestitutionData data = await ServiceBus.Get<IAnalyzeService>().GetFullProjectDetails(CurrentProject.ProjectId);

                            await new Restitution.ExportProjectToExcel(data, result).Export();
                            HideSpinner();
                        }
                        catch (Exception e)
                        {
                            base.OnError(e);
                        }

                    }, () => CurrentProject != null && CanExportProject && CanChangeProject);
                return _exportExcelCommand;
            }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Définit le projet actuel de l'application.
        /// </summary>
        /// <param name="project">The project.</param>
        void SetCurrentGlobalProject(Project project)
        {
            IProjectManagerService service = ServiceBus.Get<IProjectManagerService>();
            service.SetCurrentProject(project);
            if (project != null && project.ScenariosDescriptions != null)
                service.SyncScenarios(project.ScenariosDescriptions);

            _currentGlobalProjectId = project != null ? project.ProjectId : (int?)null;
            OnPropertyChanged(nameof(IsCurrentProjectOpened));
        }

        /// <summary>
        /// Valide le projet.
        /// </summary>
        /// <returns><c>true</c> si le projet est valide.</returns>
        bool ValidateProject()
        {
            CurrentProject.Validate();
            Validate();
            RefreshValidationErrors(CurrentProject);

            if (!CurrentProject.IsValid.GetValueOrDefault())
            {
                // Active la validation auto
                CurrentProject.EnableAutoValidation = true;
                return false;
            }

            ServiceBus.Get<IProjectManagerService>().SynchronizeProjectObjectivesInfo(CurrentProject);
            return true;
        }

        /// <summary>
        /// Valide le dossier.
        /// </summary>
        /// <returns><c>true</c> si le dossier est valide.</returns>
        bool ValidateFolder()
        {
            CurrentFolder.Validate();
            Validate();
            RefreshValidationErrors(CurrentFolder);

            if (!CurrentFolder.IsValid.GetValueOrDefault())
            {
                // Active la validation auto
                CurrentFolder.EnableAutoValidation = true;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valide le process.
        /// </summary>
        /// <returns><c>true</c> si le process est valide.</returns>
        bool ValidateProcess()
        {
            CurrentProcess.Validate();
            Validate();
            RefreshValidationErrors(CurrentProcess);

            if (!CurrentProcess.IsValid.GetValueOrDefault())
            {
                // Active la validation auto
                CurrentProcess.EnableAutoValidation = true;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Appelé lorsque la refraichissement des erreurs de validation est demandé.
        /// Dans une méthode dérivée, appeler RefreshValidationErrors.
        /// </summary>
        protected override void OnRefreshValidationErrorsRequested()
        {
            if (CurrentProject != null)
            {
                CurrentProject.Validate();
                RefreshValidationErrors(CurrentProject);
            }
        }

        /// <summary>
        /// Sauvegarde le projet;
        /// </summary>
        async Task<bool> SaveProject()
        {
            if (CurrentProject.Objective != null)
                CurrentProject.OtherObjectiveLabel = null;
            ShowSpinner();
            try
            {
                // On fixe le projet pour la requête
                Project resultProject = await ServiceBus.Get<IPrepareService>().SaveProject(CurrentProject);

                await SignalRFactory.GetSignalR<IKL2AnalyzeHubConnect>()
                    .RefreshProcess(new AnalyzeEventArgs(nameof(SaveProject)));

                // On recharge l'arborescence
                await ReloadAllProjects();
                // On retrouve le projet créé
                Project projectInTree = TreeViewHelper.FindProject(AllProjects[0], resultProject.ProjectId);
                // On le sélectionne
                CurrentNode = projectInTree;
                // On ouvre l'arbre
                //ExpandProject(projectInTree);

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
        /// Sauvegarde le dossier;
        /// </summary>
        async Task<bool> SaveFolder(ProjectDir toAdd, bool manageError = true)
        {
            ShowSpinner();
            try
            {
                // Si on est à la racine, on met le parent à null
                if (toAdd.ParentId == -1)
                    toAdd.Parent = null;

                ProjectDir resultFolder = await ServiceBus.Get<IPrepareService>().SaveFolder(toAdd);

                await SignalRFactory.GetSignalR<IKL2AnalyzeHubConnect>()
                    .RefreshProcess(new AnalyzeEventArgs(nameof(SaveFolder)));

                // On recharge l'arborescence
                await ReloadAllProjects();
                // On retrouve le dossier créé
                ProjectDir folderInTree = TreeViewHelper.FindFolder(AllProjects[0], resultFolder.Id);
                // On le sélectionne
                CurrentNode = folderInTree;
                // On ouvre l'arbre
                //TreeViewHelper.ExpandFolder(folderInTree);

                HideSpinner();
                return true;
            }
            catch (Exception e)
            {
                if (manageError)
                    base.OnError(e);
                return false;
            }
        }

        /// <summary>
        /// Sauvegarde le process;
        /// </summary>
        async Task<bool> SaveProcess(Procedure toAdd, bool manageError = true, bool notifyChanges = true)
        {
            ShowSpinner();
            try
            {
                // Si on est à la racine, on met le parent à null
                if (toAdd.ProjectDirId == -1)
                    toAdd.ProjectDir = null;

                Procedure resultProcess = await ServiceBus.Get<IPrepareService>().SaveProcess(toAdd, notifyChanges);

                if (notifyChanges)
                    await SignalRFactory.GetSignalR<IKL2AnalyzeHubConnect>()
                        .RefreshProcess(new AnalyzeEventArgs(nameof(SaveProcess)));

                // On recharge l'arborescence
                await ReloadAllProjects();
                // On retrouve le process créé
                Procedure processInTree = TreeViewHelper.FindProcess(AllProjects[0], resultProcess.ProcessId);
                // On le sélectionne
                CurrentNode = processInTree;
                // On ouvre l'arbre
                if (processInTree.ProjectDir != null)
                    TreeViewHelper.ExpandFolder(processInTree.ProjectDir);

                HideSpinner();
                return true;
            }
            catch (Exception e)
            {
                if (manageError)
                    base.OnError(e);
                return false;
            }
        }

        /// <summary>
        /// Supprime le projet spécifié.
        /// </summary>
        /// <param name="project">le projet.</param>
        async Task DeleteProject(Project project)
        {
            Procedure parentProcess = project.Process;
            if (project.IsMarkedAsAdded) // Le projet est forcément vide
            {
                project.Scenarios.Clear();
                project.Process = null;
                project.MarkAsUnchanged();
                CurrentNode = parentProcess;
                return;
            }

            ShowSpinner();

            ProjectInfo currentGlobalProject = ServiceBus.Get<IProjectManagerService>().CurrentProject;
            if (currentGlobalProject != null && currentGlobalProject.ProjectId == project.ProjectId)
                SetCurrentGlobalProject(null);

            CurrentNode = parentProcess;
            project.IsDeleted = true;

            try
            {
                await ServiceBus.Get<IPrepareService>().SaveProject(project);

                await SignalRFactory.GetSignalR<IKL2AnalyzeHubConnect>()
                    .RefreshProcess(new AnalyzeEventArgs(nameof(DeleteProject)));

                // On recharge l'arborescence
                await ReloadAllProjects();
                // On retrouve le process du projet supprimé
                Procedure processInTree = TreeViewHelper.FindProcess(AllProjects[0], parentProcess.ProcessId);
                // On le sélectionne
                CurrentNode = processInTree;
                // On ouvre l'arbre
                /*if (processInTree != null)
                    TreeViewHelper.ExpandFolder(processInTree.ProjectDir);*/

                HideSpinner();
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
        }

        /// <summary>
        /// Supprime le dossier spécifié.
        /// </summary>
        /// <param name="folder">le dossier.</param>
        async Task<bool> DeleteFolder(ProjectDir folder)
        {
            ProjectDir parentFolder = null;
            if (folder.IsMarkedAsAdded) // Le dossier est forcément vide
            {
                parentFolder = folder.Parent;
                parentFolder.Childs.Remove(folder);
                CurrentNode = parentFolder;
                return true;
            }
            // Le dossier contient peut être des process, mais ils peuvent être supprimés
            ShowSpinner();

            // Si le projet courant se situe dans le dossier que l'on veut supprimer, on le désélectionne
            ProjectInfo currentGlobalProject = ServiceBus.Get<IProjectManagerService>().CurrentProject;
            if (currentGlobalProject != null && ProjectIsInFolder(folder, currentGlobalProject.ProjectId))
                SetCurrentGlobalProject(null);

            parentFolder = folder.Parent;
            CurrentNode = parentFolder;
            if (parentFolder == null)
                AllProjects.Remove(folder);
            else
                parentFolder.Childs.Remove(folder);

            folder.IsDeleted = true;

            try
            {
                await ServiceBus.Get<IPrepareService>().SaveFolder(folder);

                await SignalRFactory.GetSignalR<IKL2AnalyzeHubConnect>()
                    .RefreshProcess(new AnalyzeEventArgs(nameof(DeleteFolder)));

                // On recharge l'arborescence
                await ReloadAllProjects();
                // On retrouve le parent du dossier supprimé
                ProjectDir folderInTree = parentFolder == null ? null : TreeViewHelper.FindFolder(AllProjects[0], parentFolder.Id);
                // On le sélectionne
                CurrentNode = folderInTree;
                // On ouvre l'arbre
                /*if (folderInTree != null)
                    TreeViewHelper.ExpandFolder(folderInTree);*/

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
        /// Supprime le process spécifié.
        /// </summary>
        /// <param name="process">le process.</param>
        async Task<bool> DeleteProcess(Procedure process)
        {
            ProjectDir parentFolder = null;
            if (process.IsMarkedAsAdded) // Le process est forcément vide
            {
                parentFolder = process.ProjectDir;
                parentFolder.Processes.Remove(process);
                CurrentNode = parentFolder;
                return true;
            }
            // Le process contient peut être des projets, mais ils n'ont pas de scénarios et peuvent être supprimés
            ShowSpinner();

            ProjectInfo currentGlobalProject = ServiceBus.Get<IProjectManagerService>().CurrentProject;
            if (currentGlobalProject != null && process.Projects.Any(_ => _.ProjectId == currentGlobalProject.ProjectId))
                SetCurrentGlobalProject(null);

            parentFolder = process.ProjectDir;
            CurrentNode = parentFolder;
            if (parentFolder == null)
                AllProjects.Remove(process);
            else
                parentFolder.Processes.Remove(process);

            process.IsDeleted = true;

            try
            {
                await ServiceBus.Get<IPrepareService>().SaveProcess(process);

                await SignalRFactory.GetSignalR<IKL2AnalyzeHubConnect>()
                    .RefreshProcess(new AnalyzeEventArgs(nameof(DeleteProcess)));

                // On recharge l'arborescence
                await ReloadAllProjects();
                // On retrouve le parent du process supprimé
                ProjectDir folderInTree = parentFolder == null ? null : TreeViewHelper.FindFolder(AllProjects[0], parentFolder.Id);
                // On le sélectionne
                CurrentNode = folderInTree;
                // On ouvre l'arbre
                /*if (folderInTree != null)
                    TreeViewHelper.ExpandFolder(folderInTree);*/

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
        /// Appelé lorsque le projet courant a changé.
        /// </summary>
        /// <param name="previousProject">La ressource précédente.</param>
        /// <param name="newProject">La nouvelle ressourc.</param>
        void OnCurrentProjectChanged(Project previousProject, Project newProject)
        {
            SyncObjectives(newProject);
            RegisterToStateChanged(previousProject, newProject);
            RegisterToObjectiveChanged(previousProject, newProject);

            if (EventBus != null)
                EventBus.Publish(new RefreshRequestedEvent(this));
        }

        /// <summary>
        /// Appelé lorsque le dossier courant a changé.
        /// </summary>
        /// <param name="previousProjectDir">La ressource précédente.</param>
        /// <param name="newProjectDir">La nouvelle ressourc.</param>
        void OnCurrentFolderChanged(ProjectDir previousProjectDir, ProjectDir newProjectDir)
        {
            RegisterToStateChanged(previousProjectDir, newProjectDir);

            if (EventBus != null)
                EventBus.Publish(new RefreshRequestedEvent(this));
        }

        /// <summary>
        /// Appelé lorsque le process courant a changé.
        /// </summary>
        /// <param name="previousProcess">La ressource précédente.</param>
        /// <param name="newProcess">La nouvelle ressourc.</param>
        void OnCurrentProcessChanged(Procedure previousProcess, Procedure newProcess)
        {
            RegisterToStateChanged(previousProcess, newProcess);

            if (EventBus != null)
                EventBus.Publish(new RefreshRequestedEvent(this));
        }

        /// <summary>
        /// S'enregistre et gère le changement d'objectif.
        /// </summary>
        /// <param name="previousProject">La ressource précédente.</param>
        /// <param name="newProject">La nouvelle ressourc.</param>
        void RegisterToObjectiveChanged(Project previousProject, Project newProject)
        {
            if (previousProject != null)
                previousProject.PropertyChanged -= OnProjectPropertyChanged;

            if (newProject != null)
                newProject.PropertyChanged += OnProjectPropertyChanged;
        }

        void OnProjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(Project.Objective)))
                IsAlternateObjectiveChecked &= CurrentProject.Objective == null;
        }

        /// <summary>
        /// Appelé lorsque l'état de l'entité courante (spécifiée via RegisterToStateChanged) a changé.
        /// </summary>
        /// <param name="newState">le nouvel état.</param>
        protected override void OnEntityStateChanged(ObjectState newState) =>
            CanChangeProject &= newState == ObjectState.Unchanged;

        /// <summary>
        /// Synchronise l'état des objectifs.
        /// </summary>
        /// <param name="project">Le projet.</param>
        void SyncObjectives(Project project)
        {
            if (project != null)
            {
                _isAlternateObjectiveChecked = !string.IsNullOrEmpty(project.OtherObjectiveLabel);
                OnPropertyChanged(nameof(IsAlternateObjectiveChecked));

                _alternateObjective = project.OtherObjectiveLabel;
                OnPropertyChanged(nameof(AlternateObjective));
            }
        }

        /// <summary>
        /// Met à jour l'objectif additionel dans le projet.
        /// </summary>
        void UpdateAlternateObj()
        {
            if (IsAlternateObjectiveChecked)
            {
                CurrentProject.Objective = null;
                CurrentProject.OtherObjectiveLabel = AlternateObjective;
            }
        }

        bool ProjectIsInFolder(ProjectDir folder, int projectId)
        {
            Project searchFolder = TreeViewHelper.FindProject(folder, projectId);
            if (searchFolder != null)
                return true;
            return false;
        }

        /// <summary>
        /// Recharge le projet depuis la base de données.
        /// </summary>
        void ReloadProject()
        {
            ShowSpinner();
            CurrentProject.CancelChanges();
            HideSpinner();
        }

        /// <summary>
        /// Recharge le dossier depuis la base de données.
        /// </summary>
        void ReloadFolder()
        {
            ShowSpinner();
            CurrentFolder.CancelChanges();
            HideSpinner();
        }

        /// <summary>
        /// Recharge le process depuis la base de données.
        /// </summary>
        void ReloadProcess()
        {
            ShowSpinner();
            CurrentProcess.CancelChanges();
            HideSpinner();
        }

        public Task SignalRHandler(AnalyzeEventArgs args)
        {
            var mess = "{0} have update from server!";

            switch (args.TargetAnalyze)
            {
                case TargetAnalyze.Project:
                    {
                        mess = string.Format(mess, "Project " + args.TargetProject?.Label);
                        break;
                    }
                case TargetAnalyze.ProjectDir:
                    {
                        mess = string.Format(mess, "Project dir " + args.TargetProjectDir?.Name);
                        break;
                    }
                case TargetAnalyze.Process:
                    {
                        mess = string.Format(mess, "Process " + args.TargetProcess?.Label);
                        break;
                    }
                default:
                    {
                        mess = string.Format(mess, "Your list");
                        break;
                    }
            }

            TraceManager.TraceInfo(mess);

            return ReloadAllProjects();
        }

        #endregion


    }
}