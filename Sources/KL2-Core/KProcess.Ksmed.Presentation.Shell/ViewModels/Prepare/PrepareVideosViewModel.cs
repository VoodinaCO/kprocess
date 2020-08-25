using Kprocess.KL2.FileTransfer;
using KProcess.KL2.APIClient;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos.Prepare;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Helpers;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.Shell.Views.Wizard;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using KProcess.Presentation.Windows.Controls;
using MoreLinq;
using Murmur;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using LocalizationManager = KProcess.Globalization.LocalizationManager;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    [Microsoft.Practices.EnterpriseLibrary.Validation.Validators.HasSelfValidation]
    /// <summary>
    /// Représente le modèle de vue de l'écran de gestion des vidéos d'un projet.
    /// </summary>
    class PrepareVideosViewModel : FrameContentExtensibleViewModelBase<PrepareVideosViewModel, IPrepareVideosViewModel>, IPrepareVideosViewModel, IWizardViewModel
    {
        #region Champs privés

        DispatcherTimer _timerTranscoding;
        bool isComputing_Transcoding_Tick;

        int _processId;
        BulkObservableCollection<Resource> _resources;
        BindingList<Video> _videos;
        Video _currentVideo;
        ResourceType[] _resourceTypes;
        bool _canChange = true;

        /// <summary>
        /// Si définit, une navigation se produit à l'issue d'une sauvegarde
        /// </summary>
        IFrameNavigationToken _navigationToken;

        #endregion

        #region Surcharges

        WizardControl _wizard;
        public WizardControl Wizard
        {
            get => _wizard;
            set
            {
                if (_wizard != value)
                {
                    _wizard = value;
                    OnPropertyChanged();

                    if (!_wizard.HasItems)
                    {
                        _wizard.Items.Add(new AddVideo_CameraName());
                        _wizard.Items.Add(new AddVideo_ChoosingResource());
                        _wizard.Items.Add(new AddVideo_ChoosingResourceView());
                        _wizard.Items.Add(new AddVideo_ChoosingSync());
                    }
                }
            }
        }

        public WizardPage SelectedWizardPage =>
            Wizard?.Items?.CurrentItem as WizardPage;

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

        /// <summary>
        /// Initialise le VM en vue du chargement
        /// </summary>
        protected override void Initialize()
        {
            ResourceTypes = ResourcesHelper.CreateResourceTypes();
            FileTransferManager.OnFileAdded += (sender, file) =>
            {
                if (Path.GetExtension(file) == ".tmp" || Videos == null)
                    return;
                var hash = Path.GetFileNameWithoutExtension(file);
                Videos.Where(v => v.Hash == hash).ForEach(v => v.OnPropertyChanged(nameof(Video.IsSync)));
            };
        }

        async Task ReloadAllVideos()
        {
            VideoLoad data = await ServiceBus.Get<IPrepareService>().GetVideos(_processId);
            foreach (var video in data.ProcessVideos)
            {
                video.CanChangeSync = video.Sync ? (!video.IsUsed) : (video.OnServer == true || File.Exists(video.FilePath));
                if (!video.CanChangeSync && video.Sync)
                    video.CanChangeSyncTooltip = IoC.Resolve<ILocalizationManager>().GetString("View_PrepareVideos_VideoInUsed");
                else if (!video.CanChangeSync && !video.Sync)
                    video.CanChangeSyncTooltip = IoC.Resolve<ILocalizationManager>().GetStringFormat("View_PrepareVideos_VideoNotExists", video.FilePath);
                else
                    video.CanChangeSyncTooltip = null;
            }

            Resources = data.ProcessResources.ToBulkObservableCollection();
            if (Videos == null)
                Videos = new BindingList<Video>();
            else
            {
                foreach (var video in Videos)
                {
                    video.OnTransferFinished -= Video_OnTransferFinished;
                    video.OnTranscodingFinished -= Video_OnTranscodingFinished;
                }
                Videos.Clear();
            }
            if (data.ProcessVideos.Any())
            {
                data.ProcessVideos.ForEach(video =>
                {
                    video.OnTransferFinished += Video_OnTransferFinished;
                    video.OnTranscodingFinished += Video_OnTranscodingFinished;
                    Videos.Add(video);
                    video.StartTracking();
                });
            }
            RefreshTransferOperations();
        }

        void RefreshTransferOperations()
        {
            foreach (var video in Videos)
            {
                if (!string.IsNullOrEmpty(video.Filename) && FileTransferManager.TransferOperations.ContainsKey(video.Filename))
                {
                    var transfer = FileTransferManager.TransferOperations[video.Filename];
                    video.Transfer = transfer.IsFinished ? null : transfer;
                }
                else
                    video.Transfer = null;
            }

            RemoveCommand.Invalidate();
        }

        void Video_OnTransferFinished(object sender, JobType e)
        {
            if (sender is Video video)
            {
                var v = Videos?.SingleOrDefault(_ => _.VideoId == video.VideoId);
                if (v != null)
                {
                    if (e == JobType.Download)
                        v.OnPropertyChanged(nameof(Video.IsSync));
                    v.Transfer = null;
                }
                if (video.VideoId == CurrentVideo?.VideoId)
                {
                    OnPropertyChanged(nameof(CurrentVideo));
                    RemoveCommand.Invalidate();
                }
            }
        }

        async void Video_OnTranscodingFinished(object sender, EventArgs e)
        {
            if (sender is Video video)
            {
                var v = Videos?.SingleOrDefault(_ => _.VideoId == video.VideoId);
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
                    v.Process?.OnPropertyChanged(nameof(Procedure.IsSyncing));
                    OnPropertyChanged(nameof(CurrentVideo));
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

            _processId = ServiceBus.Get<IProjectManagerService>().CurrentProject.ProcessId;
            IsVideosPathReadOnly =
                !CanCurrentUserWrite &&
                !Security.SecurityContext.HasCurrentLicenseFeature(Security.Activation.ActivationFeatures.ReadOnly);

            ShowSpinner();

            try
            {
                Referential[] referentials = await ServiceBus.Get<IReferentialsService>().GetApplicationReferentials();
                string operatorLabel = referentials.Single(_ => _.ReferentialId == ProcessReferentialIdentifier.Operator).Label;
                string equipmentLabel = referentials.Single(_ => _.ReferentialId == ProcessReferentialIdentifier.Equipment).Label;

                var prepareService = ServiceBus.Get<IPrepareService>();
                Resource[] resources = await prepareService.GetAllResources(_processId);
                CurrentProject = prepareService.GetProjectSync(ServiceBus.Get<IProjectManagerService>().CurrentProject.ProjectId);
                DefaultResources = new ObservableCollection<DefaultResource>()
                {
                    new DefaultResource(string.Empty, null)
                };
                resources.OfType<Operator>().OrderBy(_ => _.Label).ForEach(resource =>
                {
                    var defaultResource = new DefaultResource(operatorLabel, resource);
                    DefaultResources.Add(defaultResource);
                });
                resources.OfType<Equipment>().OrderBy(_ => _.Label).ForEach(resource =>
                {
                    var defaultResource = new DefaultResource(equipmentLabel, resource);
                    DefaultResources.Add(defaultResource);
                });

                await ReloadAllVideos();
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
                // TODO : migrate SignalR to avoid timeout
                var progresses = await IoC.Resolve<IAPIHttpClient>().ServiceAsync<Dictionary<string, double>>(KL2_Server.File, null, "GetAllTranscodingProgress", null, "GET");
                if (Videos?.Any() == true)
                    foreach (Video video in Videos.Where(_ => _.OriginalHash != null))
                    {
                        var key = $"TRANSCODED_{video.OriginalHash}{video.Extension}";
                        if (progresses.Any())
                        {
                            if (progresses.ContainsKey(key))
                            {
                                video.TranscodingProgress = progresses[key];
                                continue;
                            }
                        }
                        video.TranscodingProgress = null;
                    }
                RemoveCommand.Invalidate();
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

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override Task OnInitializeDesigner()
        {
            Resources = DesignData.GenerateResources().ToBulkObservableCollection();
            Videos = new BindingList<Video>(DesignData.GenerateVideos().ToList());
            CurrentVideo = Videos[0];
            return Task.CompletedTask;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit les resources disponibles pour la sélection de la ressource par défaut.
        /// </summary>
        public BulkObservableCollection<Resource> Resources
        {
            get { return _resources; }
            private set
            {
                if (_resources != value)
                {
                    _resources = value;
                    OnPropertyChanged();
                }
            }
        }

        Project _currentProject;
        public Project CurrentProject
        {
            get => _currentProject;
            set
            {
                if (_currentProject == value)
                    return;
                _currentProject = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le projet courant est en lecture seule.
        /// </summary>
        public bool IsReadOnly =>
            CurrentProject?.IsReadOnly ?? true;

        /// <summary>
        /// Obtient les vidéos.
        /// </summary>
        public BindingList<Video> Videos
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
                    var previous = _currentVideo;
                    _currentVideo = value;
                    OnCurrentVideoChanged(previous, value);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SelectedDefaultResource));
                }
            }
        }

        public ResourceView[] ResourceViews =>
            Enums.ResourceViews.Select(_ => _.Value).ToArray();

        public Choice[] Choices =>
            Enums.Choices.Select(_ => _.Value).ToArray();

        /// <summary>
        /// Obtient les types de ressources.
        /// </summary>
        public ResourceType[] ResourceTypes
        {
            get { return _resourceTypes; }
            private set
            {
                if (_resourceTypes != value)
                {
                    _resourceTypes = value;
                    OnPropertyChanged("ResourceTypes");
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si la sélection peut être changée.
        /// </summary>
        public bool CanChange
        {
            get { return _canChange; }
            private set
            {
                if (_canChange != value)
                {
                    _canChange = value;
                    OnPropertyChanged("CanChange");
                }
            }
        }

        /// <summary>
        /// Obtient les noms des ressources disponibles pour les ressources par défaut.
        /// </summary>
        ObservableCollection<DefaultResource> _defaultResources;
        public ObservableCollection<DefaultResource> DefaultResources
        {
            get => _defaultResources;
            private set
            {
                if (_defaultResources != value)
                {
                    _defaultResources = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient la ressource par défaut sélectionnée.
        /// </summary>
        public DefaultResource SelectedDefaultResource
        {
            get => DefaultResources?.Single(_ => _.Resource?.ResourceId == CurrentVideo?.DefaultResourceId);
            set
            {
                if (CurrentVideo != null && CurrentVideo.DefaultResourceId != value?.Resource?.ResourceId)
                    CurrentVideo.DefaultResource = value?.Resource;

                if (Wizard == null)
                {
                    OnPropertyChanged();
                    return;
                }

                List<WizardPage> pages = Wizard.Items.OfType<WizardPage>().ToList();
                var cameraPage = pages.Single(_ => _ is AddVideo_CameraName);
                var choosingResourcePage = pages.Single(_ => _ is AddVideo_ChoosingResource);
                var choosingResourceViewPage = pages.Single(_ => _ is AddVideo_ChoosingResourceView);
                var choosingSyncPage = pages.Single(_ => _ is AddVideo_ChoosingSync);
                if (value?.Resource != null)
                {
                    cameraPage.NextPage = choosingResourcePage;
                    choosingResourcePage.NextPage = choosingResourceViewPage;
                    choosingResourceViewPage.NextPage = choosingSyncPage;
                    choosingSyncPage.NextPage = null;

                    cameraPage.PreviousPage = null;
                    choosingResourcePage.PreviousPage = cameraPage;
                    choosingResourceViewPage.PreviousPage = choosingResourcePage;
                    choosingSyncPage.PreviousPage = choosingResourceViewPage;
                }
                else
                {
                    cameraPage.NextPage = choosingResourcePage;
                    choosingResourcePage.NextPage = choosingSyncPage;
                    choosingResourceViewPage.NextPage = null;
                    choosingSyncPage.NextPage = null;

                    cameraPage.PreviousPage = null;
                    choosingResourcePage.PreviousPage = cameraPage;
                    choosingResourceViewPage.PreviousPage = null;
                    choosingSyncPage.PreviousPage = choosingResourcePage;
                }

                OnPropertyChanged();
            }
        }

        public ResourceView SelectedResourceView
        {
            get => ResourceViews.SingleOrDefault(_ => _.Id == CurrentVideo?.ResourceView);
            set
            {
                if (CurrentVideo != null)
                    CurrentVideo.ResourceView = value?.Id;
            }
        }

        Choice _addedVideoSyncVideo;
        public Choice AddedVideoSyncVideo
        {
            get => _addedVideoSyncVideo;
            set
            {
                _addedVideoSyncVideo = value;

                CurrentVideo.Sync = _addedVideoSyncVideo.Id == ChoiceEnum.Yes;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si les chemins des vidéos sont en lecture seule.
        /// </summary>
        public bool IsVideosPathReadOnly { get; private set; }

        bool _showAddVideoWizard;
        public bool ShowAddVideoWizard
        {
            get => _showAddVideoWizard;
            set
            {
                if (_showAddVideoWizard != value)
                {
                    _showAddVideoWizard = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Commandes

        Command _onNextWizardCommand;
        public ICommand OnNextWizardCommand
        {
            get
            {
                if (_onNextWizardCommand == null)
                    _onNextWizardCommand = new Command(async () =>
                    {
                        if (SelectedWizardPage.NextPage is IGotFocus iGotFocus)
                            await iGotFocus.GotFocus();
                        if (SelectedWizardPage is AddVideo_ChoosingSync)
                            CurrentVideo.SetNumSeqToNextAvailable(Videos);
                    });
                return _onNextWizardCommand;
            }
        }

        /// <summary>
        /// Obtient la commande permettant d'annuler l'ajout d'une vidéo
        /// </summary>
        Command _cancelWizardCommand;
        public ICommand CancelWizardCommand
        {
            get
            {
                if (_cancelWizardCommand == null)
                    _cancelWizardCommand = new Command(() =>
                    {
                        Wizard.SelectedWizardPage = null;
                        CurrentVideo = null;
                        ShowAddVideoWizard = false;
                    });
                return _cancelWizardCommand;
            }
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande AddCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnAddCommandCanExecute() =>
            CanChange && CanCurrentUserWrite && !IsReadOnly;

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande AddCommand
        /// </summary>
        protected override async void OnAddCommandExecute()
        {
            var newVideo = new Video()
            {
                ProcessId = _processId,
                ShootingDate = DateTime.Today // TODO : Get this from MediaInfo while it will move to use ffprobe
            };
            CurrentVideo = null;
            string filepath = Browse();

            if (!string.IsNullOrEmpty(filepath))
            {
                if (!FilesHelper.IsFileType(filepath, FileType.Video))
                {
                    base.OnError(LocalizationManager.GetStringFormat("Common_NotAFileOfFileType", LocalizationManager.GetString("Common_FileType_Video")));
                    return;
                }
                newVideo.FilePath = filepath;
                CurrentVideo = newVideo;
                UpdateVideoInformation();
                SelectedDefaultResource = DefaultResources.Single(_ => _.Resource == null);
                AddedVideoSyncVideo = Choices.Single(_ => _.Id == ChoiceEnum.No);
                ShowAddVideoWizard = true;
                if (SelectedWizardPage is IGotFocus iGotFocus)
                    await iGotFocus.GotFocus();
            }
        }

        Command _enterCommand;
        public ICommand EnterCommand
        {
            get
            {
                if (_enterCommand == null)
                {
                    _enterCommand = new Command(() =>
                    {
                        if (SelectedWizardPage.NextPage == null)
                            SendVideoCommand.Execute(null);
                        else
                            _wizard.MoveNext();
                    });
                }
                return _enterCommand;
            }
        }

        /// <summary>
        /// Obtient la commande permettant d'envoyer une vidéo
        /// </summary>
        Command _sendVideoCommand;
        public ICommand SendVideoCommand
        {
            get
            {
                if (_sendVideoCommand == null)
                    _sendVideoCommand = new Command(() =>
                    {
                        ShowAddVideoWizard = false;
                        ShowSpinner();
                        Task.Run(async () =>
                        {
                            try
                            {
                                var resultVideo = await SendVideo(CurrentVideo);

                                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                                {
                                    await ReloadAllVideos();
                                    CurrentVideo = Videos.Single(_ => _.VideoId == resultVideo.VideoId);
                                    HideSpinner();
                                });
                            }
                            catch (DirectoryNotFoundException e)
                            {
                                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                {
                                    CurrentVideo = null;
                                    CanChange = true;
                                    HideSpinner();
                                    DialogFactory.GetDialogView<IMessageDialog>().Show(
                                        e.Message,
                                        IoC.Resolve<ILocalizationManager>().GetString("Common_Error"),
                                        MessageDialogButton.OK, MessageDialogImage.Error);
                                });
                            }
                            catch (Exception e)
                            {
                                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                {
                                    CurrentVideo = null;
                                    CanChange = true;
                                    base.OnError(e);
                                });
                            }
                            finally
                            {
                                GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                {
                                    Wizard.SelectedWizardPage = null;
                                });
                            }
                        });
                    });
                return _sendVideoCommand;
            }
        }

        async Task<Video> SendVideo(Video video)
        {
            Video resultVideo = video;
            // On définit les infos de la vidéo si besoin
            if (string.IsNullOrEmpty(resultVideo.Hash))
            {
                HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
                using (var fileStream = File.OpenRead(resultVideo.FilePath))
                {
                    string originalHash = murmur128.ComputeHash(fileStream).ToHashString();
                    resultVideo.OriginalHash = originalHash;
                    resultVideo.Hash = originalHash;
                    resultVideo.Extension = Path.GetExtension(resultVideo.FilePath);
                }
                Video sameOriginal = await ServiceBus.Get<IPrepareService>().GetSameOriginalVideo(resultVideo.OriginalHash);
                if (sameOriginal != null)
                {
                    resultVideo.Hash = sameOriginal.Hash;
                    resultVideo.Extension = sameOriginal.Extension;
                    resultVideo.OnServer = sameOriginal.OnServer;
                }
                else
                    resultVideo.OnServer = await IoC.Resolve<IAPIHttpClient>().ServiceAsync<bool>(KL2_Server.File, null, $"Exists/{resultVideo.Hash}{resultVideo.Extension}", null, "GET");

                try
                {
                    resultVideo.SendVideo(IoC.Resolve<IAPIHttpClient>().Service<Dictionary<string, double>>(KL2_Server.File, null, "GetAllTranscodingProgress", null, "GET"));
                    resultVideo = await ServiceBus.Get<IPrepareService>().SaveVideo(resultVideo);
                    return resultVideo;
                }
                catch (DirectoryNotFoundException e)
                {
                    throw new DirectoryNotFoundException(IoC.Resolve<ILocalizationManager>().GetStringFormat("VM_PrepareVideos_VideoFileSourceNotReachable", resultVideo.FilePath), e);
                }
            }

            resultVideo.SendVideo(IoC.Resolve<IAPIHttpClient>().Service<Dictionary<string, double>>(KL2_Server.File, null, "GetAllTranscodingProgress", null, "GET"));
            return resultVideo;
        }

        /// <summary>
        /// Obtient la commande permettant de gérer la synchronisation d'une vidéo
        /// </summary>
        Command<Video> _syncChangeCommand;
        public ICommand SyncChangeCommand
        {
            get
            {
                if (_syncChangeCommand == null)
                    _syncChangeCommand = new Command<Video>(async video =>
                    {
                        if (video.OnServer != true && !video.Sync && !File.Exists(video.FilePath))
                            return;

                        Video resultVideo = video;
                        resultVideo.Sync = !resultVideo.Sync;
                        ShowSpinner();
                        try
                        {
                            resultVideo = await ServiceBus.Get<IPrepareService>().SaveVideo(resultVideo);
                            RefreshTransferOperations();
                            if (resultVideo.Sync) // On veut synchroniser
                            {
                                if (resultVideo.OnServer != true)
                                {
                                    if (FileTransferManager.TransferOperations.ContainsKey(resultVideo.Filename)) // Un transfert est en cours, on le résume
                                        FileTransferManager.TransferOperations[resultVideo.Filename].Resume();
                                    else if (File.Exists(resultVideo.FilePath))// Le fichier n'est pas sur le serveur, on doit l'envoyer
                                        resultVideo = await SendVideo(resultVideo);
                                }
                            }
                            else // On ne veut pas synchroniser
                            {
                                if (FileTransferManager.TransferOperations.ContainsKey(resultVideo.Filename)) // Un transfert est en cours, on l'annule
                                    FileTransferManager.TransferOperations[resultVideo.Filename].Cancel();
                                if (resultVideo.IsSync && await ServiceBus.Get<IPrepareService>().CanBeUnSync(resultVideo.Hash)) // Le fichier est présent en local, on doit le supprimer si il n'est pas utilisé ailleurs
                                    resultVideo.DeleteLocal(Videos);
                            }
                        }
                        catch (Exception e)
                        {
                            base.OnError(e);
                        }
                        await ReloadAllVideos();
                        CurrentVideo = Videos.Single(_ => _.VideoId == resultVideo.VideoId);
                        HideSpinner();
                        CanChange = true;
                    });
                return _syncChangeCommand;
            }
        }

        Command _browseCommand;
        /// <summary>
        /// Obtient la commande permettant de sélectionner la vidéo.
        /// </summary>
        public ICommand BrowseCommand
        {
            get
            {
                if (_browseCommand == null)
                    _browseCommand = new Command(() =>
                    {
                        var filepath = Browse();
                        // TODO : Detect if file path is correct (not on a network drive, because BITS has no rights on this)
                        if (!string.IsNullOrEmpty(filepath))
                        {
                            if (!FilesHelper.IsFileType(filepath, FileType.Video))
                                return;
                            CurrentVideo.FilePath = filepath;
                        }
                    }, () => CanCurrentUserWrite || !IsVideosPathReadOnly);
                return _browseCommand;
            }
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande RemoveCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnRemoveCommandCanExecute() =>
            CurrentVideo != null && CanChange && CanCurrentUserWrite && CurrentVideo.TranscodingProgress == null && !IsReadOnly;

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande RemoveCommand
        /// </summary>
        protected override async void OnRemoveCommandExecute()
        {
            if (CurrentVideo.IsMarkedAsAdded)
            {
                CurrentVideo.MarkAsUnchanged();
                Videos.Remove(CurrentVideo);
                CanChange = true;
            }
            else
                CanChange |= await DeleteVideo(CurrentVideo);
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande ValidateCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnValidateCommandCanExecute() =>
            !CanChange && CurrentVideo != null && (CanCurrentUserWrite || !IsVideosPathReadOnly);

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande CancelCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnCancelCommandCanExecute() =>
            !CanChange && CurrentVideo != null;

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande ValidateCommand
        /// </summary>
        protected override async void OnValidateCommandExecute()
        {
            // Vérfier si l'objet a changé
            if (CurrentVideo.IsNotMarkedAsUnchanged)
            {
                await DoValidate();
            }
        }

        async Task DoValidate()
        {
            // Vérfier si l'objet a changé
            if (CurrentVideo.IsNotMarkedAsUnchanged)
            {
                if (!ValidateCurrentVideo())
                    return;

                bool canSave = true;
                canSave = await SaveCurrentVideo();
                if (canSave)
                {
                    CanChange = true;
                    if (_navigationToken?.IsValid == true)
                        _navigationToken.Navigate();
                }
            }
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande CancelCommand
        /// </summary>
        protected override void OnCancelCommandExecute()
        {
            // Si la vidéo est nouvelle, simplement le supprimer
            if (CurrentVideo.IsMarkedAsAdded)
                RemoveCommand.Execute(null);
            else
                ReloadVideo(CurrentVideo);
            OnPropertyChanged(nameof(SelectedResourceView));
            OnPropertyChanged(nameof(SelectedDefaultResource));
            CanChange = true;
            HideValidationErrors();
        }

        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la navigation est acceptée.
        /// </returns>
        public override Task<bool> OnNavigatingAway(IFrameNavigationToken token)
        {
            if (CurrentVideo != null &&
                (!CanChange || CurrentVideo.IsNotMarkedAsUnchanged && !CurrentVideo.IsValid.GetValueOrDefault()))
            {
                MessageDialogResult answer = this.AskNavigatingAwayValidationConfirmation(() => _navigationToken = token);
                if (answer == MessageDialogResult.Yes)
                {
                    IoC.Resolve<IAPIHttpClient>().OnConnecting -= OnReconnecting;
                    Videos?.ForEach(video =>
                    {
                        if (video == null)
                            return;
                        video.OnTransferFinished -= Video_OnTransferFinished;
                        video.OnTranscodingFinished -= Video_OnTranscodingFinished;
                    });

                    _timerTranscoding?.Stop();
                    _timerTranscoding = null;
                }
                return Task.FromResult(answer == MessageDialogResult.No);
            }

            IoC.Resolve<IAPIHttpClient>().OnConnecting -= OnReconnecting;
            Videos?.ForEach(video =>
            {
                if (video == null)
                    return;
                video.OnTransferFinished -= Video_OnTransferFinished;
                video.OnTranscodingFinished -= Video_OnTranscodingFinished;
            });

            _timerTranscoding?.Stop();
            _timerTranscoding = null;
            return Task.FromResult(true);
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Appelé lorsque la vidéo courante a changé
        /// </summary>
        /// <param name="previousValue">La valeur précédante.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
        void OnCurrentVideoChanged(Video previousValue, Video newValue)
        {
            if (previousValue != null)
                UnregisterVideo(previousValue);

            if (newValue != null)
            {
                newValue.DefaultResourceChanged += OnVideoDefaultResourceChanged;
                if (EventBus != null)
                    EventBus.Publish(new MediaPlayerActionEvent(this, player =>
                    {
                        if (player is KMediaPlayer mediaPlayer)
                        {
                            mediaPlayer.CurrentPosition = 0;
                            mediaPlayer.MediaElement.SpeedRatio = 1;
                            mediaPlayer.HideVideoSelector();
                        }
                    }));
            }

            OnPropertyChanged(nameof(SelectedResourceView));

            RegisterToStateChanged(previousValue, newValue);
        }

        /// <summary>
        /// Appelé lorsqu'un élément est désabonné du changement d'état lors du Cleanup du VM.
        /// </summary>
        /// <param name="item">L'élément.</param>
        protected override void OnItemUnregisteredToStateChangedOnCleanup(IObjectWithChangeTracker item)
        {
            if (item is Video)
                UnregisterVideo((Video)item);
        }

        /// <summary>
        /// Se désabonne aux évènements de la vidéo spécifiée.
        /// </summary>
        /// <param name="video">La vidéo.</param>
        void UnregisterVideo(Video video)
        {
            video.DefaultResourceChanged -= OnVideoDefaultResourceChanged;
        }

        /// <summary>
        /// Appelé lorsque la resource de la vidéo a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.EventArgs&lt;Resource&gt;"/> contenant les données de l'évènement.</param>
        void OnVideoDefaultResourceChanged(object sender, PropertyChangedEventArgs<Resource> e)
        {
            if (sender is Video video)
            {
                if (e.OldValue == null)
                    video.ResourceView = ResourceViewEnum.Intern;
                else if (e.NewValue == null)
                    video.ResourceView = null;
                OnPropertyChanged(nameof(SelectedResourceView));
            }
        }

        /// <summary>
        /// Appelé lorsque l'état de l'entité courante (spécifiée via RegisterToStateChanged) a changé.
        /// </summary>
        /// <param name="newState">le nouvel état.</param>
        protected override void OnEntityStateChanged(ObjectState newState)
        {
            CanChange &= newState == ObjectState.Unchanged;
        }

        /// <summary>
        /// Met à jour les informations sur l'entité vidéo.
        /// </summary>
        void UpdateVideoInformation()
        {
            if (CurrentVideo != null)
            {
                EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.ActivateMediaPlayerService));
                EventBus.Publish(new MediaPlayerActionEvent(this, UpdateVideoInformationImpl));
            }
            else
            {
                CurrentVideo.Duration = 0;
                CurrentVideo.Format = null;
            }

        }

        /// <summary>
        /// Met à jour les informations sur l'entité vidéo.
        /// </summary>
        void UpdateVideoInformationImpl()
        {
            // Récupérer la durée et le format
            if (ServiceBus.IsAvailable<IMediaPlayerService>())
            {
                IMediaPlayerService service = ServiceBus.Get<IMediaPlayerService>();
                var mediaInfo = service.GetFormat();

                if (mediaInfo.VideoLength == default(TimeSpan))
                    CurrentVideo.Duration = service.GetDuration().GetValueOrDefault().Ticks;
                else
                    CurrentVideo.Duration = mediaInfo.VideoLength.Ticks;

                CurrentVideo.Format = string.Format(LocalizationManager.GetString("VM_PrepareVideos_VideoFormat"),
                        mediaInfo.AudioCodec ?? string.Empty,
                        mediaInfo.VideoCodec ?? string.Empty,
                        mediaInfo.FrameWidth,
                        mediaInfo.FrameHeight);
                if (CurrentVideo.Format.Length > 100)
                    CurrentVideo.Format = CurrentVideo.Format.Substring(0, 100);
            }

            EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.UnactivateMediaPlayerService));

            if (System.IO.File.Exists(CurrentVideo.FilePath))
            {
                // Utiliser le nom de la vidéo
                /*if (string.IsNullOrEmpty(CurrentVideo.Name))
                    CurrentVideo.Name = System.IO.Path.GetFileNameWithoutExtension(CurrentVideo.FilePath);*/

                if (CurrentVideo.ShootingDate.Date == DateTime.Today)
                {
                    CurrentVideo.ShootingDate = System.IO.File.GetCreationTime(CurrentVideo.FilePath);
                    if (CurrentVideo.ShootingDate.Year < 1950 || CurrentVideo.ShootingDate.Year > 2050)
                        CurrentVideo.ShootingDate = DateTime.Today;
                }

                // Récupérer la description et la date 
                /*var dic = new Dictionary<int, object>();
                try
                {
                    if (string.IsNullOrWhiteSpace(CurrentVideo.Description))
                    {
                        Shell32.Shell shell = new Shell32.Shell();
                        Shell32.Folder folder = shell.NameSpace(System.IO.Path.GetDirectoryName(CurrentVideo.FilePath));
                        Shell32.FolderItem folderItem = folder.ParseName(System.IO.Path.GetFileName(CurrentVideo.FilePath));
                        if (folderItem != null)
                        {
                            // Quelques index :
                            // 21 : Title
                            // 27 : Duration
                            // 24 : Comments

                            string title = folder.GetDetailsOf(folderItem, 21);
                            if (!string.IsNullOrEmpty(title))
                                CurrentVideo.Description = title;
                        }
                    }
                }
                catch (Exception)
                {
                }*/
            }

        }

        /// <summary>
        /// Valide les vidéos.
        /// </summary>
        /// <returns><c>true</c> si les vidéos sont toutes valides.</returns>
        private bool ValidateCurrentVideo()
        {
            EventBus.Publish(new MediaPlayerActionEvent(this, MediaPlayerAction.ActivateMediaPlayerService));
            UpdateVideoInformationImpl();
            CurrentVideo.Validate();
            Validate();
            RefreshValidationErrors(new ValidatableObject[] { CurrentVideo, this });

            if (!CurrentVideo.IsValid.GetValueOrDefault() || !IsValid.GetValueOrDefault())
            {
                // Active la validation auto
                CurrentVideo.EnableAutoValidation = true;

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
            if (this.CurrentVideo != null)
            {
                this.CurrentVideo.Validate();
                base.RefreshValidationErrors(this.CurrentVideo);
            }
        }

        /// <summary>
        /// Sauvegarde les données/
        /// </summary>
        /// <returns><c>true</c> si la sauvegarde s'est bien passée.</returns>
        private async Task<bool> SaveCurrentVideo()
        {
            if (CurrentVideo.IsMarkedAsUnchanged)
                return true;

            ShowSpinner();
            try
            {
                Video resultVideo = await ServiceBus.Get<IPrepareService>().SaveVideo(CurrentVideo);
                await ReloadAllVideos();
                CurrentVideo = Videos.Single(_ => _.VideoId == resultVideo.VideoId);
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
        /// Supprime la vidéo spécifiée.
        /// </summary>
        /// <param name="video">La vidéo.</param>
        async Task<bool> DeleteVideo(Video video)
        {
            if (!DialogFactory.GetDialogView<ICommonDialog>().ShowSureToDelete())
                return false;

            var vid = CurrentVideo;
            vid.MarkAsDeleted();

            ShowSpinner();
            try
            {
                if (FileTransferManager.TransferOperations.ContainsKey(CurrentVideo.Filename) && FileTransferManager.TransferOperations[CurrentVideo.Filename].State != TransferStatus.Transferred) // Un transfert est en cours, on l'annule
                    FileTransferManager.TransferOperations[CurrentVideo.Filename].Cancel();
                await ServiceBus.Get<IPrepareService>().SaveVideo(video);
                if (File.Exists(Path.Combine(Preferences.SyncDirectory, video.Filename)) && await ServiceBus.Get<IPrepareService>().CanBeUnSync(video.Hash))
                {
                    try
                    {
                        File.Delete(Path.Combine(Preferences.SyncDirectory, video.Filename));
                    }
                    catch (IOException)
                    {
                        var deleteTask = Task.Run(async () =>
                        {
                            await Task.Delay(TimeSpan.FromSeconds(5));
                            File.Delete(Path.Combine(Preferences.SyncDirectory, video.Filename));
                        });
                    }
                }
                await ReloadAllVideos();

                HideSpinner();
                return true;
            }
            catch (BLLException e)
            {
                if (e.ErrorCode == KnownErrorCodes.CannotDeleteVideoWithRelatedActions)
                {
                    DialogFactory.GetDialogView<IMessageDialog>().Show(
                        LocalizationManager.GetString("VM_PrepareVideos_CannotDeleteVideoWithRelatedActions"),
                        LocalizationManager.GetString("Common_Error"),
                        MessageDialogButton.OK, MessageDialogImage.Error);

                    CancelCommand.Execute(null);
                }
                HideSpinner();
            }
            catch(Exception e)
            {
                base.OnError(e);
            }
            return false;
        }

        /// <summary>
        /// Recharge la vidéo depuis la base de données.
        /// </summary>
        /// <param name="video">La vidéo.</param>
        void ReloadVideo(Video video)
        {
            HideValidationErrors();

            ObjectWithChangeTrackerExtensions.CancelChanges(
                Videos,
                Resources
                );

            foreach (var v in Videos)
                v.StartTracking();
        }

        /// <summary>
        /// Affiche l'interface permettant de sélectionner la vidéo.
        /// </summary>
        string Browse()
        {
            var file = DialogFactory.GetDialogView<IOpenFileDialog>()
                .Show(LocalizationManager.GetString("View_AddVideo_SelectAVideo"),
                filter: FileExtensionsDialogHelper.GetVideosFileDialogFilter());

            if (file != null && file.Any())
            {
                var fileName = file.First();
                return fileName;
            }
           return null;
        }

        #endregion
    }
}