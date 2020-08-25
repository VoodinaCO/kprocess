using KProcess.Globalization;
using KProcess.KL2.APIClient;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Helpers;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Ksmed.Security;
using KProcess.Presentation.Windows;
using log4net;
using log4net.Appender;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using LocalizationManager = KProcess.Globalization.LocalizationManager;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le VM de le fenêtre principale
    /// </summary>
    class MainWindowViewModel : CompositeViewModelBase, IMainWindowViewModel, Core.INotificationService, IDisconnectionService
    {
        #region Constantes

        /// <summary>
        /// Le dossier qui contient la documentation.
        /// </summary>
        const string DocumentFolder = @"Documentation\";

        /// <summary>
        /// La langue de la documentation par défault.
        /// </summary>
        const string DefaultDocumentationLanguage = "en-US";

        #endregion

        #region Champs privés

        IFrameContentViewModel _currentViewModel;
        NavigationManager _navigation;
        string _title;
        string _windowTitle;
        BulkObservableCollection<Core.Notification> _notifications;
        bool _hasMainWindowLoadedOnce;
        Visibility _scenariosPickerVisibility = Visibility.Collapsed;
        bool _initializeNavigation;
        string _currentUserName;
        string _defaultDocumentationFileName;
        string _lastMenu;

#pragma warning disable CS0649
        [System.ComponentModel.Composition.Import]
        System.ComponentModel.Composition.Hosting.CompositionContainer _compositionContainer;
#pragma warning restore CS0649

        #endregion

        #region Surcharges

        /// <summary>
        /// Initialise le VM en vue du chargement
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            _navigation = new NavigationManager(this);
            _notifications = new BulkObservableCollection<Core.Notification>();
        }

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override async Task OnLoading()
        {
            WindowState = WindowState.Normal;

            ServiceBus.Register<INavigationService>(_navigation);
            ServiceBus.Register<IProjectManagerService>(_navigation);
            ServiceBus.Register<Core.INotificationService>(this);
            ServiceBus.Register<IDisconnectionService>(this);

            EventBus.Subscribe<CurrentUserChangedEvent>(e =>
            {
                CurrentUserName = Security.SecurityContext.CurrentUser?.FullName;
            });

            EventBus.Subscribe<NavigationRequestedEvent>(async e =>
            {
                if (string.IsNullOrWhiteSpace(e.SubMenuCode))
                    await _navigation.TryNavigate(e.MenuCode);
                else
                    await _navigation.TryNavigate(e.MenuCode, e.SubMenuCode);
            });

            EventBus.Subscribe<CultureChangedEvent>(e =>
            {
                OnPropertyChanged(nameof(MaximizeLabel));
                OnPropertyChanged(nameof(MinimizeLabel));
                OnPropertyChanged(nameof(RestoreLabel));
                OnPropertyChanged(nameof(QuitLabel));
                OnPropertyChanged(nameof(AdministrationLabel));
                OnPropertyChanged(nameof(SettingsLabel));
                OnPropertyChanged(nameof(HelpLabel));
                OnPropertyChanged(nameof(AboutLabel));
                OnPropertyChanged(nameof(DisconnectLabel));
                OnPropertyChanged(nameof(ExtensionsLabel));

                if (!_initializeNavigation)
                    _navigation.RefreshMenuLabels();
            });

            TryShowAuthentication(true);
            IsRunningReadOnlyVersion = Security.SecurityContext.HasCurrentLicenseFeature(Security.Activation.ActivationFeatures.ReadOnly);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override async Task OnInitializeDesigner()
        {
            await base.OnInitializeDesigner();
            _navigation.InitializeDesigner();
            await _navigation.TryNavigate(KnownMenus.Prepare, KnownMenus.PrepareProject);
            _title = "Nom du process / Nom du projet";
            OnPropertyChanged(nameof(Title));
        }

        #endregion

        #region Propriétés

        private IEnumerable<ApplicationMenuItem> _menuItems;
        /// <summary>
        /// Obtient ou définit le menu items.
        /// </summary>
        public IEnumerable<ApplicationMenuItem> MenuItems
        {
            get { return _menuItems; }
            set
            {
                if (_menuItems != value)
                {
                    _menuItems = value;
                    OnPropertyChanged("MenuItems");
                }
            }
        }

        private ApplicationMenuItem _currentMenuItem;
        /// <summary>
        /// Obtient ou définit le menu actuel courant.
        /// </summary>
        public ApplicationMenuItem CurrentMenuItem
        {
            get { return _currentMenuItem; }
            set
            {
                if (_currentMenuItem != value)
                {
                    _currentMenuItem = value;
                    OnPropertyChanged("CurrentMenuItem");
                }
            }
        }

        private object _currentView;
        /// <summary>
        /// Obtient la vue courante.
        /// </summary>
        public object CurrentView
        {
            get { return _currentView; }
            private set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    OnPropertyChanged("CurrentView");
                }
            }
        }

        /// <summary>
        /// Obtient le titre appliqué en titre de la vue principale.
        /// </summary>
        public override string Title
        {
            get { return _title; }
        }

        /// <summary>
        /// Obtient le title de la fenêtre.
        /// </summary>
        public string WindowTitle
        {
            get { return _windowTitle; }
            private set
            {
                if (_windowTitle != value)
                {
                    _windowTitle = value;
                    OnPropertyChanged("WindowTitle");
                }
            }
        }

        /// <summary>
        /// Obtient les notifications.
        /// </summary>
        public BulkObservableCollection<Core.Notification> Notifications
        {
            get { return _notifications; }
            private set
            {
                if (_notifications != value)
                {
                    _notifications = value;
                    OnPropertyChanged("Notifications");
                }
            }
        }

        private Core.Notification _selectedNotification;
        public Core.Notification SelectedNotification
        {
            get { return _selectedNotification; }
            set
            {
                if (_selectedNotification != value)
                {
                    _selectedNotification = value;
                    OnPropertyChanged("SelectedNotification");
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le scénario courant.
        /// </summary>
        public ScenarioDescription CurrentScenario
        {
            get { return _navigation.CurrentScenario; }
            set
            {
                if (_navigation.CurrentScenario != value)
                    _navigation.CurrentScenario = value;

                OnPropertyChanged("CurrentScenario");
            }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le sélectionneur de scénarios est activé.
        /// </summary>
        public bool IsScenarioPickerEnabled
        {
            get { return _navigation.IsScenarioPickerEnabled; }
            private set
            {
                if (_navigation.IsScenarioPickerEnabled != value)
                    _navigation.IsScenarioPickerEnabled = value;

                OnPropertyChanged("IsScenarioPickerEnabled");
            }
        }

        /// <summary>
        /// Obtient ou définit la visibilité du sélectionneur de scénarios.
        /// </summary>
        public Visibility ScenariosPickerVisibility
        {
            get { return _scenariosPickerVisibility; }
            protected set
            {
                if (_scenariosPickerVisibility != value)
                {
                    _scenariosPickerVisibility = value;
                    OnPropertyChanged("ScenariosPickerVisibility");
                }
            }
        }

        /// <summary>
        /// Obtient les scenarii.
        /// </summary>
        public ReadOnlyObservableCollection<ScenarioDescription> Scenarios
        {
            get { return _navigation.Scenarios; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la licence actuelle est en lecture seule.
        /// </summary>
        public bool IsRunningReadOnlyVersion { get; private set; }

        public string MinimizeLabel => LocalizationManager.GetString("Style_Window_Minimize");
        public string MaximizeLabel => LocalizationManager.GetString("Style_Window_Maximize");
        public string RestoreLabel => LocalizationManager.GetString("Style_Window_Restore");
        public string QuitLabel => LocalizationManager.GetString("Style_Window_Quit");

        /// <summary>
        /// Obtient le libellé de l'administration.
        /// </summary>
        public string AdministrationLabel
        {
            get { return LocalizationManager.GetString("View_MainWindow_Administration"); }
        }

        /// <summary>
        /// Obtient le libellé des paramètres
        /// </summary>
        public string SettingsLabel
        {
            get { return LocalizationManager.GetString("View_MainWindow_Help"); }
        }

        /// <summary>
        /// Obtient le libellé de l'aide
        /// </summary>
        public string HelpLabel
        {
            get { return LocalizationManager.GetString("View_MainWindow_Help"); }
        }

        /// <summary>
        /// Obtient le libellé "A propos".
        /// </summary>
        public string AboutLabel
        {
            get { return LocalizationManager.GetString("View_MainWindow_About"); }
        }

        /// <summary>
        /// Obtient le libellé "Se déconnecter".
        /// </summary>
        public string DisconnectLabel
        {
            get { return LocalizationManager.GetString("View_MainWindow_Disconnect"); }
        }

        /// <summary>
        /// Obtient le libellé "Extensions".
        /// </summary>
        public string ExtensionsLabel
        {
            get { return LocalizationManager.GetString("Menu_Extensions"); }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Définit la vue courante
        /// </summary>
        /// <typeparam name="TViewModel">Le type de VM.</typeparam>
        /// <param name="initialization">The initialization delegate.</param>
        private void SetCurrentView<TViewModel>(Action<TViewModel> initialization = null)
            where TViewModel : IFrameContentViewModel
        {
            if (_currentViewModel != null)
                _currentViewModel.Shutdown();

            TViewModel vm;
            var view = base.UXFactory.GetView<TViewModel>(out vm);
            if (initialization != null)
                initialization(vm);

            vm.Load();

            _currentViewModel = vm;
            this.CurrentView = view;
        }

        /// <summary>
        /// Appelé lorsque le projet courant a changé.
        /// </summary>
        /// <param name="newProject">Le nouveau projet.</param>
        private void OnCurrentProjectChanged(ProjectInfo newProject)
        {
        }

        /// <summary>
        /// Appelé lorsque la publication courante a changé.
        /// </summary>
        /// <param name="newPublication">La nouvelle publication.</param>
        private void OnCurrentPublicationChanged(Publication newPublication)
        {
        }


        /// <summary>
        /// Définit le titre du VM.
        /// </summary>
        /// <param name="title">Le titre.</param>
        private void SetTitle(string title)
        {
            _title = title;

            if (string.IsNullOrWhiteSpace(title))
                this.WindowTitle = LocalizationManager.GetString("Common_ProductName");
            else
                this.WindowTitle = title;

            OnPropertyChanged("Title");
        }

        #endregion

        #region Commandes

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande CloseCommand
        /// </summary>
        protected override void OnCloseCommandExecute()
        {
            Application.Current.MainWindow.Close();
            //System.Windows.Application.Current.Shutdown();
        }

        public bool CheckCanClose()
        {
            var spinnerService = ServiceBus.Get<ISpinnerService>();
            if (spinnerService.Progress != null)
            {
                DialogFactory.GetDialogView<IMessageDialog>().Show(
                    LocalizationManager.GetString("VM_MainWindowViewModel_Message_PublicationInProgress"),
                    LocalizationManager.GetString("VM_MainWindowViewModel_Message_Title_PublicationInProgress"),
                    MessageDialogButton.OK,
                    MessageDialogImage.Error);
                return false;
            }
            var res = DialogFactory.GetDialogView<IMessageDialog>().Show(
                LocalizationManager.GetString("VM_MainWindowViewModel_Message_ConfirmExit"),
                LocalizationManager.GetString("VM_MainWindowViewModel_Message_Title_ConfirmExit"),
                MessageDialogButton.YesNoCancel,
                MessageDialogImage.Question);

            return res == MessageDialogResult.Yes;
        }

        private Command _shortcutSaveCommand;
        /// <summary>
        /// Obtient la commande permettant de sauvegarder via le raccourci clavier.
        /// </summary>
        public ICommand ShortcutSaveCommand
        {
            get
            {
                if (_shortcutSaveCommand == null)
                    _shortcutSaveCommand = new Command(() =>
                    {
                        if (_currentViewModel != null && _currentViewModel.ValidateCommand.CanExecute(null))
                            _currentViewModel.ValidateCommand.Execute(null);
                    });
                return _shortcutSaveCommand;
            }
        }

        private Command _shortcutAddCommand;
        /// <summary>
        /// Obtient la commande permettant d'ajouter un element via le raccourci clavier.
        /// </summary>
        public ICommand ShortcutAddCommand
        {
            get
            {
                if (_shortcutAddCommand == null)
                    _shortcutAddCommand = new Command(() =>
                    {
                        if (_currentViewModel != null && _currentViewModel.AddCommand.CanExecute(null))
                            _currentViewModel.AddCommand.Execute(null);
                    });
                return _shortcutAddCommand;
            }
        }

        private Command _shortcutDeleteCommand;
        /// <summary>
        /// Obtient la commande permettant de supprimer un element via le raccourci clavier.
        /// </summary>
        public ICommand ShortcutDeleteCommand
        {
            get
            {
                if (_shortcutDeleteCommand == null)
                    _shortcutDeleteCommand = new Command(() =>
                    {
                        if (_currentViewModel != null && _currentViewModel.RemoveCommand.CanExecute(null))
                            _currentViewModel.RemoveCommand.Execute(null);
                        else if (_currentViewModel != null && _currentViewModel.RemoveFolderCommand.CanExecute(null))
                            _currentViewModel.RemoveFolderCommand.Execute(null);
                        else if (_currentViewModel != null && _currentViewModel.RemoveProcessCommand.CanExecute(null))
                            _currentViewModel.RemoveProcessCommand.Execute(null);
                    });
                return _shortcutDeleteCommand;
            }
        }

        private Command _shortcutCancelCommand;
        /// <summary>
        /// Obtient la commande permettant d'annuler via le raccourci clavier.
        /// </summary>
        public ICommand ShortcutCancelCommand
        {
            get
            {
                if (_shortcutCancelCommand == null)
                    _shortcutCancelCommand = new Command(() =>
                    {
                        if (_currentViewModel != null && _currentViewModel.CancelCommand.CanExecute(null))
                            _currentViewModel.CancelCommand.Execute(null);
                    });
                return _shortcutCancelCommand;
            }
        }

        private Command _shortCutPlayerPlayPause;
        /// <summary>
        /// Obtient la commande permettant de faire un Play/Plause sur le player
        /// </summary>
        public ICommand ShortCutPlayerPlayPause
        {
            get
            {
                if (_shortCutPlayerPlayPause == null)
                    _shortCutPlayerPlayPause = new Command(() =>
                    {
                        if (_currentViewModel != null)
                            base.EventBus.Publish(new MediaPlayerActionEvent(_currentViewModel, MediaPlayerAction.PlayPause));
                    });
                return _shortCutPlayerPlayPause;
            }
        }

        private Command _shortCutPlayerStepBackward;
        /// <summary>
        /// Obtient la commande permettant de faire un pas en arrière sur le player
        /// </summary>
        public ICommand ShortCutPlayerStepBackward
        {
            get
            {
                if (_shortCutPlayerStepBackward == null)
                    _shortCutPlayerStepBackward = new Command(() =>
                    {
                        if (_currentViewModel != null)
                            base.EventBus.Publish(new MediaPlayerActionEvent(_currentViewModel, MediaPlayerAction.StepBackward));
                    });
                return _shortCutPlayerStepBackward;
            }
        }

        private Command _shortCutPlayerStepForward;
        /// <summary>
        /// Obtient la commande permettant de faire un pas en avant sur le player.
        /// </summary>
        public ICommand ShortCutPlayerStepForward
        {
            get
            {
                if (_shortCutPlayerStepForward == null)
                    _shortCutPlayerStepForward = new Command(() =>
                    {
                        if (_currentViewModel != null)
                            base.EventBus.Publish(new MediaPlayerActionEvent(_currentViewModel, MediaPlayerAction.StepForward));
                    });
                return _shortCutPlayerStepForward;
            }
        }

        private Command _shortCutPlayerToggleScreenMode;
        /// <summary>
        /// Obtient la commande permettant de basculer le mode de visionnage le player.
        /// </summary>
        public ICommand ShortCutPlayerToggleScreenMode
        {
            get
            {
                if (_shortCutPlayerToggleScreenMode == null)
                    _shortCutPlayerToggleScreenMode = new Command(() =>
                    {
                        if (_currentViewModel != null)
                            base.EventBus.Publish(new MediaPlayerActionEvent(_currentViewModel, MediaPlayerAction.ToggleScreenMode));
                    });
                return _shortCutPlayerToggleScreenMode;
            }
        }

        private Command _helpCommand;
        /// <summary>
        /// Obtient la commande permettant d'afficher la notice utilisateur.
        /// </summary>
        public ICommand HelpCommand
        {
            get
            {
                if (_helpCommand == null)
                    _helpCommand = new Command(async () =>
                    {
                        try
                        {
                            string currentLanguageFilePath = Path.Combine(PresentationConstants.AssemblyDirectory, DocumentFolder, LocalizationManager.GetString("HelpUserManualFileName"));

                            if (File.Exists(currentLanguageFilePath))
                                System.Diagnostics.Process.Start(currentLanguageFilePath);
                            else
                            {
                                if (_defaultDocumentationFileName == null)
                                    _defaultDocumentationFileName = (await ServiceBus.Get<IAppResourceService>().GetResource(DefaultDocumentationLanguage, "HelpUserManualFileName")).Value;

                                string defaultLanguageFilePath = Path.Combine(PresentationConstants.AssemblyDirectory, DocumentFolder, _defaultDocumentationFileName);

                                System.Diagnostics.Process.Start(defaultLanguageFilePath);
                            }
                        }

                        catch (Exception ex)
                        {
                            TraceManager.TraceError(ex, ex.Message);
                            IoC.Resolve<IDialogFactory>().GetDialogView<IErrorDialog>().Show(
                                LocalizationManager.GetString("VM_MainWindowViewModel_Message_ErrorLaunchingUserManual"),
                                LocalizationManager.GetString("Common_Error"), ex);
                        }
                    });
                return _helpCommand;
            }
        }

        private Command _syncCommand;
        /// <summary>
        /// Obtient la commande permettant de synchroniser avec la base de donéees.
        /// </summary>
        public ICommand SyncCommand
        {
            get
            {
                if (_syncCommand == null)
                    _syncCommand = new Command(() =>
                    {
                        //ServiceBus.Get<IPrepareService>().GetProjects(null, base.OnError);
                        if (_currentViewModel != null)
                            _currentViewModel.Load();
                    }, () =>
                    {
#if DEBUG
                        return true;
#else
                        return !ServiceBus.Get<IDataBaseService>().IsLocalDb();
#endif
                    });
                return _syncCommand;
            }
        }

        #endregion

        #region Gestion de l'état de la fenêtre

        private WindowState _windowState = WindowState.Minimized;
        /// <summary>
        /// Obtient ou définit l'état de la fenêtre.
        /// </summary>
        public WindowState WindowState
        {
            get { return _windowState; }
            set
            {
                if (_windowState != value)
                {
                    _windowState = value;
                    this.RestoreVisibility = value == System.Windows.WindowState.Maximized ? Visibility.Visible : Visibility.Collapsed;
                    this.MaximizeVisibility = value == System.Windows.WindowState.Maximized ? Visibility.Collapsed : Visibility.Visible;
                    OnPropertyChanged("WindowState");
                }
            }
        }

        private Visibility _restoreVisibility;
        /// <summary>
        /// Obtient la visibilité du bouton de restauration.
        /// </summary>
        public Visibility RestoreVisibility
        {
            get { return _restoreVisibility; }
            private set
            {
                if (_restoreVisibility != value)
                {
                    _restoreVisibility = value;
                    OnPropertyChanged("RestoreVisibility");
                }
            }
        }

        private Visibility _maximizeVisibility;
        /// <summary>
        /// Obtient la visibilité du bouton de maximisation.
        /// </summary>
        public Visibility MaximizeVisibility
        {
            get { return _maximizeVisibility; }
            private set
            {
                if (_maximizeVisibility != value)
                {
                    _maximizeVisibility = value;
                    OnPropertyChanged("MaximizeVisibility");
                }
            }
        }

        private Command _minimizeCommand;
        /// <summary>
        /// Obtient la commande permettant de minimiser la fenêtre.
        /// </summary>
        public ICommand MinimizeCommand
        {
            get
            {
                if (_minimizeCommand == null)
                    _minimizeCommand = new Command(() =>
                    {
                        this.WindowState = System.Windows.WindowState.Minimized;
                    });
                return _minimizeCommand;
            }
        }

        private Command _maximizeCommand;
        /// <summary>
        /// Obtient la commande permettant de maximiser la fenêtre.
        /// </summary>
        public ICommand MaximizeCommand
        {
            get
            {
                if (_maximizeCommand == null)
                    _maximizeCommand = new Command(() =>
                    {
                        this.WindowState = System.Windows.WindowState.Maximized;
                    });
                return _maximizeCommand;
            }
        }

        private Command _restoreCommand;
        /// <summary>
        /// Obtient la commande permettant de restaurer la fenêtre.
        /// </summary>
        public ICommand RestoreCommand
        {
            get
            {
                if (_restoreCommand == null)
                    _restoreCommand = new Command(() =>
                    {
                        this.WindowState = System.Windows.WindowState.Normal;
                    });
                return _restoreCommand;
            }
        }

        #endregion

        #region Gestion de la navigation et des menus

        /// <summary>
        /// Représente le gestionnaire de navigation.
        /// </summary>
        private class NavigationManager : INavigationService, IProjectManagerService, IPublicationManagerService
        {
            private MainWindowViewModel _vm;
            private List<ApplicationMenuItem> _projectMenuStrip;
            private List<ApplicationMenuItem> _adminMenuStrip;
            private List<ApplicationMenuItem> _extensionsMenuStrip;
            private Dictionary<string, ApplicationMenuItem> _menuItems;
            private Dictionary<string, ApplicationSubMenuItem> _subMenuItems;
            private Dictionary<Type, bool> _accessProjectContext;
            private bool _isUnlinkMarkerEnabledAndLocked = false;

            /// <summary>
            /// Initialise une nouvelle instance de la classe <see cref="NavigationManager"/>.
            /// </summary>
            /// <param name="vm">Le VM source.</param>
            public NavigationManager(MainWindowViewModel vm)
            {
                _vm = vm;
                _scenarios = new ObservableCollection<ScenarioDescription>();
                _accessProjectContext = new Dictionary<Type, bool>();
                this.Scenarios = new ReadOnlyObservableCollection<ScenarioDescription>(_scenarios);
                this.Preferences = new NavigationSharedPreferences();
                this.RestitutionState = new Dictionary<int, RestitutionState>();
            }

            /// <summary>
            /// Initialise le gestionnaire.
            /// </summary>
            public void Initialize()
            {
                _projectMenuStrip = new List<ApplicationMenuItem>();
                _adminMenuStrip = new List<ApplicationMenuItem>();
                _extensionsMenuStrip = new List<ApplicationMenuItem>();
                _menuItems = new Dictionary<string, ApplicationMenuItem>();
                _subMenuItems = new Dictionary<string, ApplicationSubMenuItem>();

                // Récupérer les menus de premier niveau
                var menus = _vm._compositionContainer.GetExports<IMenuDefinition>().Select(l => l.Value);

                foreach (var menu in menus)
                {
                    menu.Initialize();
                    var menuItem = new ApplicationMenuItem
                    {
                        LabelResourceKey = menu.TitleResourceKey,
                        Label = LocalizationManager.GetString(menu.TitleResourceKey),
                        Code = menu.Code,
                        IsEnabledDelegate = menu.IsEnabledDelegate,
                        Strip = menu.Strip,
                    };
                    _menuItems[menu.Code] = menuItem;

                    switch (menu.Strip)
                    {
                        case MenuStrip.Project:
                            _projectMenuStrip.Add(menuItem);
                            break;
                        case MenuStrip.Administration:
                            _adminMenuStrip.Add(menuItem);
                            break;
                        case MenuStrip.Settings:
                            throw new NotImplementedException();
                        case MenuStrip.Extensions:
                            _extensionsMenuStrip.Add(menuItem);
                            break;
                        default:
                            break;
                    }

                    menu.IsEnabledInvalidated += (s, e) =>
                    {
                        var sm = (IMenuDefinition)s;
                        var mi = _menuItems[sm.Code];
                        mi.InvalidateIsEnabled();
                    };
                }

                // Récupérer les menus de deuxième niveau
                var subMenus = _vm._compositionContainer.GetExports<ISubMenuDefinition>().Select(l => l.Value);
                var rolesReadAuthorizations = new Dictionary<Type, string[]>();
                var rolesWriteAuthorizations = new Dictionary<Type, string[]>();
                var featuresReadAuthorizations = new Dictionary<Type, short[]>();
                var featuresWriteAuthorizations = new Dictionary<Type, short[]>();
                var customReadAuthorizations = new Dictionary<Type, Func<string, bool>>();
                var customWriteAuthorizations = new Dictionary<Type, Func<string, bool>>();

                foreach (var subMenu in subMenus)
                {
                    subMenu.Initialize();
                    var parentMenu = _projectMenuStrip
                        .Concat(_adminMenuStrip)
                        .Concat(_extensionsMenuStrip)
                        .FirstOrDefault(m => string.Equals(m.Code, subMenu.ParentCode));

                    if (parentMenu == null)
                        throw new InvalidOperationException("Impossible de trouver le menu parent");

                    var subMenuItem = new ApplicationSubMenuItem()
                    {
                        Action = subMenu.Action,
                        Code = subMenu.Code,
                        LabelResourceKey = subMenu.TitleResourceKey,
                        Label = LocalizationManager.GetString(subMenu.TitleResourceKey),
                        IsEnabledDelegate = subMenu.IsEnabledDelegate,
                    };
                    _subMenuItems.Add(subMenu.Code, subMenuItem);

                    subMenu.TitleChanged += (s, e) =>
                    {
                        var sm = (ISubMenuDefinition)s;
                        var smi = this._subMenuItems[sm.Code];
                        smi.LabelResourceKey = sm.TitleResourceKey;
                        smi.Label = LocalizationManager.GetString(sm.TitleResourceKey);
                    };

                    subMenu.IsEnabledInvalidated += (s, e) =>
                    {
                        var sm = (ISubMenuDefinition)s;
                        var smi = this._subMenuItems[sm.Code];
                        smi.InvalidateIsEnabled();
                    };

                    if (parentMenu.SubItems == null)
                        parentMenu.SubItems = new List<ApplicationSubMenuItem>();

                    parentMenu.SubItems.Add(subMenuItem);

                    rolesReadAuthorizations[subMenu.ViewModelType] = subMenu.RolesCanRead;
                    rolesWriteAuthorizations[subMenu.ViewModelType] = subMenu.RolesCanWrite;
                    featuresReadAuthorizations[subMenu.ViewModelType] = subMenu.FeaturesCanRead;
                    featuresWriteAuthorizations[subMenu.ViewModelType] = subMenu.FeaturesCanWrite;
                    customReadAuthorizations[subMenu.ViewModelType] = subMenu.CustomCanRead;
                    customWriteAuthorizations[subMenu.ViewModelType] = subMenu.CustomCanWrite;
                    _accessProjectContext[subMenu.ViewModelType] = subMenu.IsSecurityProjectContext;
                }

                // Enregistrer les autorisations sur la navigation, les VM
                Security.SecurityContext.RegisterAuthorizations(
                    rolesReadAuthorizations,
                    rolesWriteAuthorizations,
                    featuresReadAuthorizations,
                    featuresWriteAuthorizations,
                    customReadAuthorizations,
                    customWriteAuthorizations);

                foreach (var menu in _adminMenuStrip.Concat(_extensionsMenuStrip))
                    menu.SeparatorVisibility = Visibility.Collapsed;
            }

            /// <summary>
            /// Initialise le gestionnaire en mode design.
            /// </summary>
            public void InitializeDesigner()
            {
                _projectMenuStrip = new List<ApplicationMenuItem>();
                _adminMenuStrip = new List<ApplicationMenuItem>();
                _menuItems = new Dictionary<string, ApplicationMenuItem>();
                _subMenuItems = new Dictionary<string, ApplicationSubMenuItem>();

                _projectMenuStrip.Add(new ApplicationMenuItem()
                {
                    Code = KnownMenus.Prepare,
                    Label = "Préparer",
                    SubItems = new List<ApplicationSubMenuItem>()
                    {
                        new ApplicationSubMenuItem()
                        {
                            Code = KnownMenus.PrepareProject,
                            Label = "Projet",
                            Action = sb => new Task<bool>(() => true),
                        },
                    },
                });

                _menuItems[KnownMenus.Prepare] = _projectMenuStrip[0];
                _subMenuItems[KnownMenus.PrepareProject] = _projectMenuStrip[0].SubItems[0];
            }

            /// <summary>
            /// Rafraichit les libellés des menus en fonction de la culture actuellement chargée.
            /// </summary>
            internal void RefreshMenuLabels()
            {
                foreach (var menu in _menuItems.Values)
                    menu.Label = LocalizationManager.GetString(menu.LabelResourceKey);

                foreach (var sm in _subMenuItems.Values)
                    sm.Label = LocalizationManager.GetString(sm.LabelResourceKey);
            }

            /// <summary>
            /// Remet ma navigation à l'état initial.
            /// </summary>
            public void Reset()
            {
                if (_menuItems != null)
                    foreach (var mi in _menuItems)
                        mi.Value.CurrentSubMenuItem = null;

                this.CurrentProject = null;
                this.CurrentScenario = null;

                if (_scenarios != null)
                    _scenarios.Clear();
            }

            #region INavigationService Members

            /// <summary>
            /// Obtient les préférences de navigation.
            /// </summary>
            public NavigationSharedPreferences Preferences { get; private set; }

            /// <summary>
            /// Tente d'afficher l'écran spécifié.
            /// </summary>
            /// <typeparam name="TViewModel">Le type du VM.</typeparam>
            /// <param name="initialization">L'action à exécuter une fois que le VM est créé.</param>
            /// <returns><c>true</c> si l'écran a été affiché.</returns>
            public async Task<bool> TryShow<TViewModel>(Action<TViewModel> initialization = null)
                where TViewModel : IFrameContentViewModel
            {
                if (DesignMode.IsInDesignMode)
                    return true;

                // Vérifier si l'utilisateur a le droit de lire le VM
                if (!AccessRules.CanUserRead<TViewModel>(CurrentProject?.Roles, _accessProjectContext[typeof(TViewModel)]))
                    return false;

                if (_vm._currentViewModel != null)
                {
                    var navigationToken = new FrameNavigationToken<TViewModel>();
                    if (!await _vm._currentViewModel.OnNavigatingAway(navigationToken))
                    {
                        navigationToken.Activate();
                        return false;
                    }
                    navigationToken.Dispose();

                    _vm._currentViewModel.Shutdown();
                }

                _vm.SetCurrentView<TViewModel>(vm =>
                {
                    vm.CanCurrentUserWrite = AccessRules.CanUserWrite<TViewModel>(CurrentProject?.Roles, _accessProjectContext[typeof(TViewModel)]);
                    initialization?.Invoke(vm);
                });

                if (_vm._currentViewModel.ShowScenarioPicker)
                    ShowScenariosPicker();
                else
                    HideScenariosPicker();


                if (Scenarios != null)
                    FilterScenarioNatures(_vm._currentViewModel.ScenarioNaturesFilter);

                return true;
            }

            /// <summary>
            /// Tente de naviguer vers le menu spécifié.
            /// </summary>
            /// <param name="menuCode">Le code du menu.</param>
            /// <returns><c>true</c> si l'opération a réussi.</returns>
            public async Task<bool> TryNavigate(string menuCode)
            {
                FrameNavigationTokenState.LastMenuCodeAttempt = menuCode;
                FrameNavigationTokenState.LastSubMenuCodeAttempt = null;

                var appMenuItem = _menuItems[menuCode];

                if (appMenuItem.IsEnabled)
                {
                    ApplicationSubMenuItem subItem = appMenuItem.CurrentSubMenuItem;

                    if (subItem == null && appMenuItem.SubItems != null)
                        subItem = appMenuItem.SubItems.FirstOrDefault();

                    if (subItem != null)
                    {
                        // Essayer de naviguer vers le sub menu actuel.
                        // Il est possible que le sub menu actuel soit inaccessible.
                        // Si c'est le cas, il faut réessayer avec tous les autres sub menus
                        NavigationResult result = await TryNavigateInternal(menuCode, subItem.Code);
                        if (result != NavigationResult.Success
                            && result != NavigationResult.AlreadyLoaded
                            && result != NavigationResult.CannotDisplay) // On suppose que dans le cas cannotDisplay, le menu était accessible et a préempté la navigation, sinon echo dans la confirmation de sauvegarde...
                        {
                            foreach (var sub in appMenuItem.SubItems.Where(s => s.Code != subItem.Code))
                            {
                                if (await TryNavigate(menuCode, sub.Code))
                                    break;
                            }
                        }
                        else
                            return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Tente de naviguer vers le menu spécifié.
            /// </summary>
            /// <param name="menuItemCode">Le code du menu.</param>
            /// <param name="subMenuCode">Le code du sous-menu.</param>
            /// <returns><c>true</c> si l'opération a réussi.</returns>
            public async Task<bool> TryNavigate(string menuCode, string subMenuCode)
            {
                FrameNavigationTokenState.LastMenuCodeAttempt = menuCode;
                FrameNavigationTokenState.LastSubMenuCodeAttempt = subMenuCode;
                return await TryNavigateInternal(menuCode, subMenuCode) == NavigationResult.Success;
            }

            private async Task<NavigationResult> TryNavigateInternal(string menuCode, string subMenuCode)
            {
                var appMenuItem = _menuItems[menuCode];
                var subMenuItem = _subMenuItems[subMenuCode];

                if (subMenuItem != null && subMenuItem.Action != null)
                {
                    if (_vm.CurrentMenuItem == null || (_vm.CurrentMenuItem.CurrentSubMenuItem != subMenuItem))
                    {
                        subMenuItem.InvalidateIsEnabled();
                        if (subMenuItem.IsEnabled)
                        {
                            bool canDisplay = await subMenuItem.Action(_vm.ServiceBus);

                            if (canDisplay)
                            {
                                LoadAccurateMenu(appMenuItem, subMenuItem);
                                SetSelection(appMenuItem, subMenuItem);
                                return NavigationResult.Success;
                            }
                            else
                                return NavigationResult.CannotDisplay;
                        }
                        else
                            return NavigationResult.Disabled;
                    }
                    else
                        return NavigationResult.AlreadyLoaded;
                }
                else
                    return NavigationResult.Invalid;
            }

            private enum NavigationResult
            {
                Success,
                AlreadyLoaded,
                Invalid,
                Disabled,
                CannotDisplay
            }

            /// <summary>
            /// Navigue vers le menu spécifié.
            /// </summary>
            /// <param name="menuItem">Le menu.</param>
            public async Task DoMenuNavigateCommand(object menuItem)
            {
                var appMenuItem = menuItem as ApplicationMenuItem;
                if (appMenuItem != null)
                {
                    await TryNavigate(appMenuItem.Code);
                    return;
                }
                var subMenuItem = menuItem as ApplicationSubMenuItem;
                {
                    await TryNavigate(_vm.CurrentMenuItem.Code, subMenuItem.Code);
                }
            }

            /// <summary>
            /// Navigue vers le menu et le sous menu spécifiés.
            /// </summary>
            /// <param name="mi">Le menu.</param>
            /// <param name="smi">Le sous menu.</param>
            private void LoadAccurateMenu(ApplicationMenuItem mi, ApplicationSubMenuItem smi)
            {
                if (_projectMenuStrip.Contains(mi) && _vm.MenuItems != _projectMenuStrip)
                {
                    SetCurrentProjectAsTitle();
                    _vm.MenuItems = _projectMenuStrip;
                }

                if (_adminMenuStrip.Contains(mi) && _vm.MenuItems != _adminMenuStrip)
                {
                    _vm.SetTitle(LocalizationManager.GetString("View_MainWindow_Administration"));
                    _vm.MenuItems = _adminMenuStrip;
                }

                if (_extensionsMenuStrip.Contains(mi) && _vm.MenuItems != _extensionsMenuStrip)
                {
                    _vm.SetTitle(LocalizationManager.GetString("Menu_Extensions"));
                    _vm.MenuItems = _extensionsMenuStrip;
                }

                if (DesignMode.IsInDesignMode)
                    _vm.BackButtonVisibility = Visibility.Visible;
                else
                    _vm.BackButtonVisibility = _projectMenuStrip.Contains(mi) ? Visibility.Collapsed : Visibility.Visible;
            }

            /// <summary>
            /// Définit la sélection du menu et du sous menu.
            /// </summary>
            /// <param name="mi">Le menu.</param>
            /// <param name="smi">Le sous menu.</param>
            private void SetSelection(ApplicationMenuItem mi, ApplicationSubMenuItem smi)
            {
                foreach (var menuItem in _vm.MenuItems)
                    menuItem.IsSelected = false;

                foreach (var subMenuItem in mi.SubItems)
                    subMenuItem.IsSelected = false;

                mi.IsSelected = true;
                smi.IsSelected = true;

                _vm.CurrentMenuItem = mi;
                mi.CurrentSubMenuItem = smi;
            }

            #endregion

            #region IProjectManagerService Members

            private ObservableCollection<ScenarioDescription> _scenarios;
            private ProjectInfo _currentProject;
            private ScenarioDescription _currentScenario;

            /// <summary>
            /// Obtient ou définit le projet courant.
            /// </summary>
            public ProjectInfo CurrentProject
            {
                get { return _currentProject; }
                private set
                {
                    if (_currentProject != value)
                    {
                        _currentProject = value;
                        OnCurrentProjectChanged(value);
                        _vm.OnCurrentProjectChanged(value);
                    }
                }
            }

            /// <summary>
            /// Définit le projet courant.
            /// </summary>
            /// <param name="p">Le projet.</param>
            public void SetCurrentProject(Project p)
            {
                if (p != null)
                {
                    this.CurrentProject = new ProjectInfo(p);
                    IoC.Resolve<IReferentialsUseService>().UpdateProjectReferentials(p.Referentials);
                }
                else
                {
                    this.CurrentProject = null;
                    IoC.Resolve<IReferentialsUseService>().UpdateProjectReferentials(null);
                }
            }

            /// <summary>
            /// SYnchronize le contexte relatif aux objectifs
            /// </summary>
            /// <param name="project"></param>
            public void SynchronizeProjectObjectivesInfo(Project project)
            {
                if (project != null)
                {
                    if (project.ObjectiveCode == KnownProjectObjectiveTypes.Audit)
                    {
                        _isUnlinkMarkerEnabledAndLocked = true;
                        return;
                    }
                }

                _isUnlinkMarkerEnabledAndLocked = false;
            }

            /// <summary>
            /// Appelé lorsque le projet courant a changé.
            /// </summary>
            /// <param name="newProject">Le nouveau projet.</param>
            private void OnCurrentProjectChanged(ProjectInfo newProject)
            {
                InvalidateMenus();

                // Vider la liste des scénarios au changement de projet
                _scenarios.Clear();

                SetCurrentProjectAsTitle();
            }

            /// <summary>
            /// Définit le titre du VM comme étant le nom du projet courant.
            /// </summary>
            private void SetCurrentProjectAsTitle()
            {
                if (CurrentProject != null)
                    _vm.SetTitle($"{CurrentProject.ProcessLabel} / {CurrentProject.Label}");
                else
                    _vm.SetTitle(null);
            }

            /// <summary>
            /// Obtient ou définit les scenarii.
            /// </summary>
            public ReadOnlyObservableCollection<ScenarioDescription> Scenarios { get; private set; }

            /// <summary>
            /// Obtient ou définit le scénario courant.
            /// </summary>
            public ScenarioDescription CurrentScenario
            {
                get { return _currentScenario; }
                set
                {
                    if (_currentScenario != value)
                    {
                        _currentScenario = value;
                        _vm.CurrentScenario = value;
                        OnCurrentScenarioChanged();
                    }
                }
            }

            private bool _isScenarioPickerEnabled;
            /// <summary>
            /// Obtient ou définit une valeur indiquant si le sélectionneur de scénarios est activé.
            /// </summary>
            public bool IsScenarioPickerEnabled
            {
                get { return _isScenarioPickerEnabled; }
                set
                {
                    if (_isScenarioPickerEnabled != value)
                    {
                        _isScenarioPickerEnabled = value;
                        _vm.IsScenarioPickerEnabled = value;
                    }
                }
            }

            /// <summary>
            /// Appelé lorsque la valeur de le propriété <see cref="CurrentScenario"/> a changé.
            /// </summary>
            private void OnCurrentScenarioChanged()
            {
                _vm.EventBus.Publish(new ScenarioChangedEvent(_vm, this.CurrentScenario));
            }

            private void UpdateAnalyzeBuildSimulateLabels()
            {
                // Met à jour les libellés
            }

            /// <summary>
            /// Ajoute les scénarios spécifiés dans le sélectionneur s'ils n'existent pas.
            /// </summary>
            /// <param name="scenarios">Les scénarios.</param>
            public void SyncScenarios(IEnumerable<Scenario> scenarios)
            {
                _scenarios.Clear();
                _scenarios.AddRange(scenarios.Select(sc => new ScenarioDescription(sc)));

                UdpateScenariosLockedState();

                _vm.EventBus.Publish(new ScenariosCollectionChangedEvent(_vm));
            }

            /// <summary>
            /// Ajoute les scénarios spécifiés dans le sélectionneur s'ils n'existent pas.
            /// </summary>
            /// <param name="scenarios">Les scénarios.</param>
            public void SyncScenarios(IEnumerable<ScenarioDescription> scenarios)
            {
                _scenarios.Clear();
                _scenarios.AddRange(scenarios);

                UdpateScenariosLockedState();

                _vm.EventBus.Publish(new ScenariosCollectionChangedEvent(_vm));
            }

            /// <summary>
            /// Supprime un scénario du sélectionneur.
            /// </summary>
            /// <param name="scenarioId">The scenario id.</param>
            public void RemoveScenario(int scenarioId)
            {
                _scenarios.RemoveFirst(sd => sd.Id == scenarioId);
            }

            /// <summary>
            /// Sélectionne un scénario dans le sélectionneur.
            /// </summary>
            /// <param name="scenario">Le scénario.</param>
            public void SelectScenario(Scenario scenario)
            {
                this.CurrentScenario = this.Scenarios.FirstOrDefault(s => s.Id == scenario.ScenarioId);
            }

            /// <summary>
            /// Cache le sélectionneur de scénarios.
            /// </summary>
            public void HideScenariosPicker()
            {
                _vm.ScenariosPickerVisibility = Visibility.Collapsed;
            }

            /// <summary>
            /// Affiche le sélectionneur de scénarios.
            /// </summary>
            public void ShowScenariosPicker()
            {
                _vm.ScenariosPickerVisibility = Visibility.Visible;
            }

            /// <summary>
            /// Met à jour l'état figé des scénarios.
            /// </summary>
            private void UdpateScenariosLockedState()
            {
                var hasValidationScenario = this.Scenarios.Any(sc => sc.NatureCode == KnownScenarioNatures.Realized);

                foreach (var scenario in this.Scenarios)
                {
                    scenario.IsLocked = scenario.StateCode == KnownScenarioStates.Validated ||
                        (scenario.NatureCode != KnownScenarioNatures.Realized && hasValidationScenario);
                }
            }

            /// <summary>
            /// Filtre les scenarios par nature. Désactive ceux qui ne passent pas le filtre.
            /// Si null ou tableau vide, désactive le filtre.
            /// </summary>
            /// <param name="natureCodes">Les codes des natures.</param>
            public void FilterScenarioNatures(string[] natureCodes)
            {
                if (natureCodes != null && natureCodes.Any())
                {
                    foreach (var scenario in this.Scenarios)
                        scenario.IsEnabled = natureCodes.Contains(scenario.NatureCode);
                }
                else
                    foreach (var scenario in this.Scenarios)
                        scenario.IsEnabled = true;
            }

            /// <summary>
            /// Obtient l'état de la synthèse.
            /// </summary>
            public IDictionary<int, RestitutionState> RestitutionState { get; private set; }

            #endregion

            #region IPublicationManagerService

            private Publication _currentPublication;

            /// <summary>
            /// Obtient ou définit la publication courante.
            /// </summary>
            public Publication CurrentPublication
            {
                get { return _currentPublication; }
                set
                {
                    if (_currentPublication != value)
                    {
                        var oldValue = _currentPublication;
                        _currentPublication = value;
                        OnCurrentPublicationChanged(oldValue, value);
                        _vm.OnCurrentPublicationChanged(value);
                    }
                }
            }

            /// <summary>
            /// Définit la publication courante.
            /// </summary>
            /// <param name="s">Le scénario.</param>
            public void SetCurrentPublication(Scenario s)
            {
                if (s != null)
                    CurrentPublication = new Publication(s);
                else
                    CurrentPublication = null;
            }

            /// <summary>
            /// Appelé lorsque la publication courante a changé.
            /// </summary>
            /// <param name="newPublication">La nouvelle publication.</param>
            private void OnCurrentPublicationChanged(Publication oldPublication, Publication newPublication)
            {
                if (oldPublication != null)
                    oldPublication.PropertyChanged -= RaiseInvalidateMenus;
                if (newPublication != null)
                    newPublication.PropertyChanged += RaiseInvalidateMenus;
                InvalidateMenus();
            }

            void RaiseInvalidateMenus(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(Publication.PublishMode))
                    InvalidateMenus();
            }

            #endregion

            public void InvalidateMenus()
            {
                // rafraichir IsEnabled
                foreach (var menu in _vm.MenuItems)
                {
                    menu.InvalidateIsEnabled();
                    if (menu.SubItems != null)
                    {
                        foreach (var submenu in menu.SubItems)
                            submenu.InvalidateIsEnabled();
                    }
                }
            }

            public bool IsUnlinkMarkerEnabledAndLocked
            {
                get
                {
                    return _isUnlinkMarkerEnabledAndLocked;
                }
            }
        }

        private Command<object> _menuNavigateCommand;
        /// <summary>
        /// Obtient la commande permettant de naviguer vers un menu.
        /// </summary>
        public ICommand MenuNavigateCommand
        {
            get
            {
                if (_menuNavigateCommand == null)
                    _menuNavigateCommand = new Command<object>(async obj => await _navigation.DoMenuNavigateCommand(obj));
                return _menuNavigateCommand;
            }
        }

        private Command _administrationCommand;
        /// <summary>
        /// Obtient la commande permettant de naviguer vers l'administration.
        /// </summary>
        public ICommand AdministrationCommand
        {
            get
            {
                if (_administrationCommand == null)
                    _administrationCommand = new Command(async () =>
                    {
                        SnapshotLastMenu();
                        await _navigation.TryNavigate(KnownMenus.AdminReferentials);
                        //await _navigation.TryNavigate(KnownMenus.AdminActivation);
                    });
                return _administrationCommand;
            }
        }

        private Command _extensionsCommand;
        /// <summary>
        /// Obtient la commande permettant de naviguer vers l'administration.
        /// </summary>
        public ICommand ExtensionsCommand
        {
            get
            {
                if (_extensionsCommand == null)
                    _extensionsCommand = new Command(async () =>
                    {
                        SnapshotLastMenu();
                        await _navigation.TryNavigate(KnownMenus.Extensions);
                    });
                return _extensionsCommand;
            }
        }

        private Command _backCommand;
        /// <summary>
        /// Obtient la commande permettant de revenir en arriere.
        /// </summary>
        public ICommand BackCommand
        {
            get
            {
                if (_backCommand == null)
                    _backCommand = new Command(async () =>
                    {
                        if (_lastMenu != null)
                            await _navigation.TryNavigate(_lastMenu);
                        else
                            await _navigation.TryNavigate(KnownMenus.Prepare);

                        _lastMenu = null;
                    });
                return _backCommand;
            }
        }

        private Visibility _backButtonVisibility;
        /// <summary>
        /// Obtient La visibilité du bouton de retour.
        /// </summary>
        public Visibility BackButtonVisibility
        {
            get { return _backButtonVisibility; }
            private set
            {
                if (_backButtonVisibility != value)
                {
                    _backButtonVisibility = value;
                    OnPropertyChanged("BackButtonVisibility");
                }
            }
        }

        private Command _disconnectCommand;
        /// <summary>
        /// Obtient la commande permettant de déconnecter l'utilisateur courant.
        /// </summary>
        public ICommand DisconnectCommand
        {
            get
            {
                if (_disconnectCommand == null)
                    _disconnectCommand = new Command(async () =>
                    {
                        await ServiceBus.Get<IDisconnectionService>().Disconnect();
                    }, () => !IsRunningReadOnlyVersion);
                return _disconnectCommand;
            }
        }

        private Command _aboutCommand;
        /// <summary>
        /// Obtient la commande permettant d'afficher la fenêtre "A propos".
        /// </summary>
        public ICommand AboutCommand
        {
            get
            {
                if (_aboutCommand == null)
                    _aboutCommand = new Command(() =>
                    {
                        IAboutViewModel vm;
                        IModalWindowView view = UXFactory.GetView<IAboutViewModel>(out vm) as IModalWindowView;
                        vm.Load();

                        view.ShowDialog();
                    });
                return _aboutCommand;
            }
        }

        /// <summary>
        /// Met de coté le menu en cours dans LastMenu.
        /// </summary>
        private void SnapshotLastMenu()
        {
            if (this.CurrentMenuItem != null)
            {
                if (this.CurrentMenuItem.Strip == MenuStrip.Project)
                    _lastMenu = this.CurrentMenuItem.Code;
            }
            else
                _lastMenu = null;
        }

        #endregion

        #region Gestion de la notification

        /// <summary>
        /// Affiche la notification spécifiée.
        /// </summary>
        /// <param name="notification">La notification.</param>
        public void Notify(Core.Notification notification)
        {
            if (!this.Notifications.Contains(notification))
                this.Notifications.Add(notification);
            notification.IsSelected = true;
        }

        /// <summary>
        /// Supprime la notification spécifiée.
        /// </summary>
        /// <param name="notification">La notification.</param>
        public void DeleteNotification(Core.Notification notification)
        {
            this.Notifications.Remove(notification);
        }

        #endregion

        #region Authentification & Déconnexion Members

        /// <summary>
        /// Obtient le nom de l'utilisateur courant.
        /// </summary>
        public string CurrentUserName
        {
            get { return _currentUserName; }
            private set
            {
                if (_currentUserName != value)
                {
                    _currentUserName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Tente de déconnecter l'utilisateur courant.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la déconnexion a réussi.
        /// </returns>
        public async Task<bool> Disconnect()
        {
            CurrentView = null;
            _currentViewModel = null;
            _navigation.Reset();

            await ServiceBus.Get<ISharedDatabasePresentationService>().ReleaseLock();
            Security.SecurityContext.DisconnectCurrentUser();
            EventBus.Publish(new CurrentUserChangedEvent(this));

            TryShowAuthentication(false);

            return true;
        }

        /// <summary>
        /// Appelé lorsque la vue associée a été chargée
        /// </summary>
        public async Task OnViewLoaded()
        {
            if (ViewModelState == ViewModelStateEnum.Inactive)
                return;

            if (!_hasMainWindowLoadedOnce)
            {
                _hasMainWindowLoadedOnce = true;

                // Test Sync path before showing Authentication
                await Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() =>
                {
                    while (!FilesHelper.TestSyncDirectoryRights())
                    {
                        var syncFolder = IoC.Resolve<IDialogFactory>().GetDialogView<IOpenFolderDialog>().Show(IoC.Resolve<ILocalizationManager>().GetString("Common_NoWritingRightsOnSyncPath"));
                        if (!string.IsNullOrEmpty(syncFolder))
                        {
                            // Set SyncPath in config file and reload it
                            FilesHelper.SetSyncFilesLocation(syncFolder);
                        }
                    }
                }), DispatcherPriority.Loaded);

                await Dispatcher.CurrentDispatcher.BeginInvoke((Action<bool>)ShowAuthentication, DispatcherPriority.Loaded, _initializeNavigation);
            }
        }

        /// <summary>
        /// Tente d'afficher la fenêtre d'authentification.
        /// </summary>
        /// <param name="initializeNavigation"><c>true</c> pour initialiser le menu.</param>
        private void TryShowAuthentication(bool initializeNavigation)
        {
            if (!_hasMainWindowLoadedOnce)
            {
                // Attendre que la vue soit chargée.
                // OnViewLoaded sera appelé
                _initializeNavigation = initializeNavigation;
            }
            else
                ShowAuthentication(initializeNavigation);
        }

        /// <summary>
        /// Affiche la fenêtre d'authentification.
        /// </summary>
        /// <param name="initializeNavigation"><c>true</c> pour initialiser le menu.</param>
        private void ShowAuthentication(bool initializeNavigation)
        {
            //ThrowException();
            var authenticationView = (IChildWindow)UXFactory.GetView(out IAuthenticationViewModel authenticationVm);

            // Afficher la fenêtre d'authentification une fois la fenêtre principale chargée
            authenticationVm.Load();
            ServiceBus.Get<IChildWindowService>().ShowDialog(
                ServiceBus.Get<IViewHandleService>().Resolve(this),
                authenticationView,
                onClosed: r => OnAuthClose(r, initializeNavigation));
        }

        /// <summary>
        /// Appelé lorsque la fenêtre d'authentification est fermée.
        /// </summary>
        /// <param name="result">Le résultat de la fenêtre.</param>
        /// <param name="initializeNavigation"><c>true</c> pour initialiser le menu.</param>
        private async void OnAuthClose(bool? result, bool initializeNavigation)
        {
            // Si l'utilisateur a annulé, on quitte l'appli
            if (KProcess.Ksmed.Security.SecurityContext.CurrentUser == null)
            {
                System.Windows.Application.Current.Shutdown();
                return;
            }

            base.EventBus.Publish(new CurrentUserChangedEvent(this));

            if (initializeNavigation)
                _navigation.Initialize();

            // Naviguer vers Prepare - Project
            await _navigation.TryNavigate(KnownMenus.Prepare, KnownMenus.PrepareProject);

            _initializeNavigation = false;
        }

        #endregion

        /// <summary>
        /// Débug : permet de lever volontairement une exception
        /// </summary>
        private void ThrowException()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() =>
            {
                throw new InvalidOperationException("Test exception");
            }));
        }

        private (string fileName, byte[] data) GetLogInternal()
        {
            var logFileLocation = LogManager.GetRepository().GetAppenders().OfType<FileAppender>().FirstOrDefault()?.File;

            var tmpFileName = Path.GetTempFileName();
            System.IO.File.Copy(logFileLocation, tmpFileName, true);
            var fileBytes = System.IO.File.ReadAllBytes(tmpFileName);
            System.IO.File.Delete(tmpFileName);

            return (Path.GetFileName(logFileLocation), fileBytes);
        }

        ICommand _getAllLogsCommand;
        public ICommand GetAllLogsCommand
        {
            get
            {
                if (_getAllLogsCommand == null)
                    _getAllLogsCommand = new Command(() =>
                    {
                        var version = Assembly.GetExecutingAssembly().FullName
                            .Split(',')
                            .Single(_ => _.Contains("Version="))
                            .Split('=')
                            .Last();
                        var dialog = new SaveFileDialog
                        {
                            CheckFileExists = false,
                            OverwritePrompt = true,
                            FileName = $"VA-log-v{version}.zip",
                            Filter = "Zip file|*.zip"
                        };

                        if (dialog.ShowDialog() ?? false)
                        {
                            var targetFileName = dialog.FileName;

                            var logFiles = new List<(string fileName, byte[] data)>
                            {
                                GetLogInternal() // VideoAnalyst
                            };
                            var apiClient = IoC.Resolve<IAPIHttpClient>();
                            var apiLog = apiClient.GetLog(KL2_Server.API);
                            if (!string.IsNullOrEmpty(apiLog.fileName) && apiLog.data != null)
                                logFiles.Add(apiLog);
                            var fileServerLog = apiClient.GetLog(KL2_Server.File);
                            if (!string.IsNullOrEmpty(fileServerLog.fileName) && fileServerLog.data != null)
                                logFiles.Add(fileServerLog);
                            //TODO : Get Logs from Filter

                            byte[] compressedBytes;

                            using (var outStream = new MemoryStream())
                            {
                                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                                {
                                    foreach (var (fileName, data) in logFiles)
                                    {
                                        var fileInArchive = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                                        using (var entryStream = fileInArchive.Open())
                                        using (var fileToCompressStream = new MemoryStream(data))
                                        {
                                            fileToCompressStream.CopyTo(entryStream);
                                        }
                                    }
                                }
                                compressedBytes = outStream.ToArray();
                            }

                            File.WriteAllBytes(targetFileName, compressedBytes);
                        }
                    }, () => SecurityContext.CurrentUser != null && SecurityContext.CurrentUser.User.Roles.Any(r => new[] { KnownRoles.Administrator }.Contains(r.RoleCode)));
                return _getAllLogsCommand;
            }
        }
    }
}