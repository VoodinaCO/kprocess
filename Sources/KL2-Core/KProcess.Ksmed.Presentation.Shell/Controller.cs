using Kprocess.KL2.FileTransfer;
using KProcess.Globalization;
using KProcess.KL2.APIClient;
using KProcess.KL2.Business.Impl.Shared.Migration;
using KProcess.KL2.Languages;
using KProcess.KL2.Languages.Provider;
using KProcess.KL2.SignalRClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Migration;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Business;
using KProcess.Presentation.Windows;
using KProcess.Presentation.Windows.Service;
using KProcess.Supervision.Trace;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using LocalizationManager = KProcess.Globalization.LocalizationManager;

namespace KProcess.Ksmed.Presentation.Shell
{
    /// <summary>
    /// Contrôleur de l'application.
    /// </summary>
    public class Controller : MEFControllerBase
    {
        #region Constantes

        const string EncoderPresetsFileName = "Ksmed2 Encoder.xml";
        const string EncoderPresetsLocalFolder = "Encoder Presets";
        const Environment.SpecialFolder EncoderPresetsDeploymentMainFolder = Environment.SpecialFolder.MyDocuments;
        const string EncoderPresetsDeploymentSubFolder = @"Expression\Expression Encoder\JobPresets";

        #endregion

        #region Démarrage

        bool _shutdown;
        SplashScreen _splashScreen;
        bool _serverNonReachable;

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du module
        /// </summary>
        protected override async Task OnLoaded()
        {
            _splashScreen = new SplashScreen("Resources/kl2_VideoAnalyst-splash.png");
            _splashScreen.Show(false, true);

            Assembly.LoadFile(Path.Combine(PresentationConstants.AssemblyDirectory, "DlhSoft.HierarchicalData.LightWPF.Controls.dll"));
            Assembly.LoadFile(Path.Combine(PresentationConstants.AssemblyDirectory, "DlhSoft.ProjectData.GanttChart.LightWPF.Controls.dll"));

            Container.SatisfyImportsOnce(Application.Current);

            ServiceBus.Register((IThemeManagerService)Application.Current);

            //SignalR
            InitializeSignalR();

            InitializeSecurity();
            InitializeEncoderPresets();
            InitializeSettings();
            if (!InitializeDirectShowFilters())
            {
                _shutdown = true;
                return;
            }

            LoadServices();

            try
            {
                InitializeDataBaseVersion();
            }
            catch (ApplicationException e)
            {
                MessageBox.Show(e.Message, "KL² has failed to launch", MessageBoxButton.OK, MessageBoxImage.Error);
                _shutdown = true;
                return;
            }

            try
            {
                InitializeExtensions();
                if (!InitializeLocalization())
                {
                    _shutdown = true;
                    return;
                }

                await InitializeReferentials();
            }
            catch (ServerNotReacheableException)
            {
                _serverNonReachable = true;
                throw;
            }
            finally
            {
                await base.OnLoaded(); // appelle les méthodes de chargement de tous les modules
            }
        }

        /// <summary>
        /// Démarre le contrôleur
        /// </summary>
        protected override async Task OnStart()
        {
            await base.OnStart();

            if (_shutdown)
                Application.Current.Shutdown();
            else
            {
                _splashScreen.Close(TimeSpan.FromMilliseconds(200));

                await ShowMainWindow();
            }
        }

        #endregion

        #region Fermeture

        /// <summary>
        /// Arrête le contrôleur
        /// </summary>
        protected override async Task OnStop()
        {
            SaveSettings();

            SignalRFactory.Dispose();

            if (!_shutdown)
                await ServiceBus.Get<ISharedDatabasePresentationService>().ReleaseLock();

            await base.OnStop();
            if (Application.Current != null)
                Application.Current.Shutdown();
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Initialize la version de la base de données
        /// </summary>
        void InitializeDataBaseVersion()
        {
            var databaseService = ServiceBus.Get<IDataBaseService>();

            var applicationVersion = Assembly.GetEntryAssembly().GetName().Version;
            var databaseVersion = Version.Parse("4.0.0.0"); //databaseService.GetVersion().GetResultSynchronously();

            if (databaseVersion == null)
            {
                throw new ApplicationException(@"Unable to launch KL².
The version of the database has not been identified.

This may occur when using a 2.8 or higher version of KL² whith a lower (<2.8) database version.");

            }
            if (applicationVersion < databaseVersion)
            {
                throw new ApplicationException(@"Unable to launch KL².
The version of the database is higher than KL².

You may considere upgrading the application.");
            }
            /*if (databaseVersion < applicationVersion)
            {
                try
                {
                    this.TraceDebug("Suppression des fichiers existants dans le dans le repertoire de backup");
                    foreach (var file in Directory.GetFiles(FilesHelper.GetBackupDir()))
                    {
                        try
                        {
                            this.TraceDebug($"  - {file}");
                            File.Delete(file);
                        }
                        catch (Exception e)
                        {
                            this.TraceError(e, file);
                        }
                    }

                    if (databaseService.IsLocalDb())
                    {
                        this.TraceDebug("Réalisation d'un backup de la base de données avant migration...");
                        string backupFilePath = databaseService.Backup().GetResultSynchronously();
                        this.TraceDebug($"Un backup a été créé avant upgrade de l'application: {backupFilePath}");
                    }
                    databaseService.Upgrade(applicationVersion);
                    this.TraceDebug("La mise à jour semble s'être effectué correctement. L'application continue à démarer");
                }
                catch (Exception e)
                {
                    this.TraceError(e, "Une erreur s'est produite lors de la mise à jour de la base de données");
                    throw new ApplicationException(@"Unable to update database version.
Please, contact your administrator.", e);
                }
            }
            else
            {
                this.TraceDebug("La base de données de KL² est à jour");
            }*/
        }

        /// <summary>
        /// Initialise les extensions.
        /// </summary>
        void InitializeExtensions()
        {
            // Charger les extensions
            var extensionsManager = ServiceBus.Get<IExtensionsManager>();

            var extensions = Container.GetExports<IExtension>().Select(e => e.Value);

            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            var descriptions = extensions.Select(e =>
                new ExtensionDescription(e, currentVersion >= e.MinimumApplicationVersion, extensionsManager.IsExtensionEnabled(e.ExtensionId)));

            extensionsManager.SetExtensions(descriptions);
        }

        void InitializeSignalR()
        {
            IoC.RegisterType<IEventSignalR, EventSignalR>(true);
            IoC.RegisterType<ISignalRFactory, SignalRFactory>(true);

            SignalRFactory.Initialization(IoC.Resolve<IEventSignalR>());

            //TODO : Service connect or disconnect 
            SignalRFactory.OnConnected += (sender, args) =>
            {
                TraceManager.TraceDebug("API connected.");
            };
            SignalRFactory.OnDisconnected += (sender, args) =>
            {
                TraceManager.TraceDebug("API disconnected.");
            };
        }

        /// <summary>
        /// Initialise la sécurité de l'application.
        /// </summary>
        void InitializeSecurity()
        {
            var authModes =
                Container.GetExports<IAuthenticationMode>()
                .Select(l => l.Value)
                .ToArray();

            SecurityContext.Initialize(ServiceBus, authModes);
        }

        /// <summary>
        /// Initialise les référentiels.
        /// </summary>
        async Task InitializeReferentials()
        {
            var referentials = await ServiceBus.Get<IReferentialsService>().GetApplicationReferentials();
            //IoC.Resolve<IReferentialsUseService>().UpdateReferentials(referentials);
        }

        /// <summary>
        /// Charge les services dans le service bus de l'application.
        /// </summary>
        private void LoadServices()
        {
            // Services UI
            var s = new ChildWindowService();
            ServiceBus.Register<IChildWindowService>(s);
            ServiceBus.Register<IViewHandleService>(s);
            ServiceBus.Register<ISpinnerService>(new SpinnerService());
            ServiceBus.Register<IEmailService>(new EmailService());
            ServiceBus.Register<IFreshdeskService>(new FreshdeskService());
            ServiceBus.Register<ITimeTicksFormatService>(new TimeTicksFormatService());
            ServiceBus.Register<IVideoColorService>(new VideoColorService(ServiceBus.Get<IVideoColorPersistanceService>()));

            var extManager = new ExtensionsManager();
            ServiceBus.Register<IExtensionsService>(extManager);
            ServiceBus.Register<IExtensionsManager>(extManager);

            var sharedDatabaseService = new Services.SharedDatabasePresentationService();
            ServiceBus.Register<ISharedDatabasePresentationService>(sharedDatabaseService);
            IoC.RegisterInstance<ISharedDatabaseSettingsService>(sharedDatabaseService);
            IoC.RegisterType<IReferentialsUseService, ReferentialsUseService>(true);

            IAuthenticationMode authMode = SecurityContext.GetAuthenticationMode();
            // Services Métier
            if (authMode?.Name == KSmedAuthenticationMode.StaticName)
            {
                ServiceBus.RegisterType<Business.INotificationService, KL2.Business.Impl.Desktop.NotificationService>();
                ServiceBus.RegisterType<IPrepareService, KL2.Business.Impl.Desktop.PrepareService>();
                ServiceBus.RegisterType<IAnalyzeService, KL2.Business.Impl.Desktop.AnalyzeService>();
                ServiceBus.RegisterType<IValidateService, KL2.Business.Impl.Desktop.ValidateService>();
                ServiceBus.RegisterType<IAppResourceService, KL2.Business.Impl.Desktop.AppResourceService>();
                ServiceBus.RegisterType<IAuthenticationService, Security.Business.Desktop.AuthenticationService>();
                ServiceBus.RegisterType<IDataBaseService, KL2.Business.Impl.Desktop.DataBaseService>();
                ServiceBus.RegisterType<IApplicationUsersService, KL2.Business.Impl.Desktop.ApplicationUsersService>();
                ServiceBus.RegisterType<IReferentialsService, KL2.Business.Impl.Desktop.ReferentialsService>();
                ServiceBus.RegisterType<IImportExportService, KL2.Business.Impl.Desktop.ImportExportService>();
                ServiceBus.RegisterType<ISharedDatabaseService, KL2.Business.Impl.Desktop.SharedDatabaseService>();
                ServiceBus.RegisterType<ISystemInformationService, KL2.Business.Impl.Desktop.SystemInformationService>();
                ServiceBus.RegisterType<IUISettingsService, KL2.Business.Impl.Desktop.UISettingsService>();
                ServiceBus.RegisterType<IPublicationService, KL2.Business.Impl.Desktop.PublicationService>();
            }
            else if (authMode?.Name == APIAuthenticationMode.StaticName)
            {
                // A remplacer au fur et à mesure de la migration vers l'API
                ServiceBus.RegisterType<Business.INotificationService, KL2.Business.Impl.API.NotificationService>();
                ServiceBus.RegisterType<IPrepareService, KL2.Business.Impl.API.PrepareService>();
                ServiceBus.RegisterType<IAnalyzeService, KL2.Business.Impl.API.AnalyzeService>();
                ServiceBus.RegisterType<IValidateService, KL2.Business.Impl.API.ValidateService>();
                ServiceBus.RegisterType<IAppResourceService, KL2.Business.Impl.API.AppResourceService>();
                ServiceBus.RegisterType<IAuthenticationService, Security.Business.API.AuthenticationService>();
                ServiceBus.RegisterType<IDataBaseService, KL2.Business.Impl.API.DataBaseService>();
                ServiceBus.RegisterType<IApplicationUsersService, KL2.Business.Impl.Desktop.ApplicationUsersService>();
                ServiceBus.RegisterType<IReferentialsService, KL2.Business.Impl.API.ReferentialsService>();
                ServiceBus.RegisterType<IImportExportService, KL2.Business.Impl.Desktop.ImportExportService>();
                ServiceBus.RegisterType<ISharedDatabaseService, KL2.Business.Impl.Desktop.SharedDatabaseService>();
                ServiceBus.RegisterType<ISystemInformationService, KL2.Business.Impl.Desktop.SystemInformationService>();
                ServiceBus.RegisterType<IUISettingsService, KL2.Business.Impl.API.UISettingsService>();
                ServiceBus.RegisterType<IPublicationService, KL2.Business.Impl.API.PublicationService>();
            }

            // Compatibility between Web API and WPF applications
            // HierarchicalLifetimeManager scope is new instance per request
            var authenticationMode = Container.GetExports<IAuthenticationMode>().Select(l => l.Value).ToArray();
            IoC.RegisterInstance<IEnumerable<IAuthenticationMode>>(authenticationMode);
            IoC.RegisterType<ISecurityContext, DesktopSecurityContext>(true);
            IoC.RegisterInstance<ILanguageStorageProvider>(new SQLiteLanguageStorageProvider("Data Source=Resources\\Localization.sqlite;"));
            IoC.RegisterInstance<ILocalizedStrings>(new LocalizedStrings(IoC.Resolve<ILanguageStorageProvider>()));
            IoC.RegisterType<ILocalizationManager, KL2.Languages.LocalizationManager>(true);
            IoC.RegisterType<IScenarioCloneManager, MigratedScenarioCloneManager>();
            IoC.RegisterType<ISharedScenarioActionsOperations, MigratedSharedScenarioActionsOperations>();

            // Logging
            IoC.RegisterType<ITraceManager, Log4netTraceManager>(true);
            //IoC.RegisterType<ITraceWrapper, Log4netTraceWrapper>(true);
            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
            PresentationTraceSources.DataBindingSource.Listeners.Add(new BindingErrorTraceListener(IoC.Resolve<ITraceManager>()));

            // API Client
            IoC.RegisterInstance<IAPIHttpClient>(APIHttpClient.GetExtendedInstance(IoC.Resolve<ITraceManager>(), true));
            // FileTransferManager
            IoC.RegisterInstance(new FileTransferManager());
        }

        /// <summary>
        /// Initialise les filtres DirectShow.
        /// </summary>
        /// <returns><c>true</c> si l'initialisation a réussi</returns>
        private bool InitializeDirectShowFilters()
        {
            var service = IoC.Resolve<IServiceBus>().Get<KProcess.Presentation.Windows.Controls.IDecoderInfoService>();
            return service.InitializeFiltersConfiguration();
        }

        /// <summary>
        /// Requiert l'authentification de l'utilisateur.
        /// </summary>
        async Task ShowMainWindow()
        {
            IModalWindowView view = UXFactory.GetView(out IMainWindowViewModel vm) as IModalWindowView;

            if (!_serverNonReachable)
                await vm.Load();

            InitializeSpinner(view);

            view?.ShowDialog();

            await Stop();
        }

        /// <summary>
        /// Initialise le spinner.
        /// </summary>
        /// <param name="view">La vue où sera affiché le spinner.</param>
        void InitializeSpinner(IView view)
        {
            var window = (Window)view;
            void onLoaded(object s, RoutedEventArgs e)
            {
                // Initialiser le spinner une première fois quand la fenêtre sera chargée
                ServiceBus.Get<ISpinnerService>().Initialize(window,
                    Application.Current.TryFindResource("spinnerDataTemplate") as DataTemplate,
                    "Spinner_DefaultWaitMessage");
                window.Loaded -= onLoaded;
            }

            window.Loaded += onLoaded;

            // Rafrichir le style du spinner lors d'un changement de thème
            EventBus.Subscribe<ThemeChangedEvent>(e =>
            {
                ServiceBus.Get<ISpinnerService>().Initialize(window,
                    Application.Current.TryFindResource("spinnerDataTemplate") as DataTemplate,
                    "Spinner_DefaultWaitMessage");
            });
        }

        /// <summary>
        /// Déploie ou met à jour les presets encoder si nécessaire.
        /// </summary>
        void InitializeEncoderPresets()
        {
            try
            {
                var deploymentBaseFolder = Environment.GetFolderPath(EncoderPresetsDeploymentMainFolder);
                var deploymentFullFolderLocation = Path.Combine(deploymentBaseFolder, EncoderPresetsDeploymentSubFolder);
                var deploymentLocation = Path.Combine(deploymentFullFolderLocation, EncoderPresetsFileName);

                var localFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var localLocation = Path.Combine(localFolder ?? string.Empty, EncoderPresetsLocalFolder, EncoderPresetsFileName);

                if (File.Exists(localLocation))
                {
                    bool shouldDeploy;

                    if (!Directory.Exists(deploymentFullFolderLocation))
                    {
                        Directory.CreateDirectory(deploymentFullFolderLocation);
                        shouldDeploy = true;
                    }
                    else
                    {
                        var localFileModificationDate = new FileInfo(localLocation).LastWriteTimeUtc;
                        var deployedFileModificationDate = new FileInfo(deploymentLocation).LastWriteTimeUtc;

                        shouldDeploy = localFileModificationDate > deployedFileModificationDate;
                    }

                    if (shouldDeploy)
                    {
                        File.Copy(localLocation, deploymentLocation, true);
                        this.TraceDebug("Fichier Encoder Presets déployé");
                    }
                }
            }
            catch (Exception e)
            {
                this.TraceWarning(e, "Erreur lors du déploiement du fichier de presets Encoder.");
            }
        }

        /// <summary>
        /// Initialise les settings.
        /// </summary>
        void InitializeSettings()
        {
            LocalSettings.Initialize();
            LocalSettings.Instance.Upgrade();
        }

        /// <summary>
        /// Sauvegarde les settings.
        /// </summary>
        private void SaveSettings()
        {
            LocalSettings.Instance.Save();
        }

        #endregion

        #region Composition

        /// <summary>
        /// Obtient le catalogue global MEF de l'application.
        /// </summary>
        /// <returns>Le catalogue global MEF.</returns>
        protected override ComposablePartCatalog GetCatalog()
        {
            return new AggregateCatalog(
                new AssemblyCatalog(Assembly.GetExecutingAssembly()),
                new DirectoryCatalog(".", "KProcess.Presentation.Windows.dll"),
                new DirectoryCatalog(".", "KProcess.Ksmed.Presentation.Core.dll"),
                new DirectoryCatalog(".", "KProcess.Ksmed.Security.dll"),
                new StrongNameCatalog("Extensions", "KProcess.Ksmed.Ext.*dll", true, "c4e0e49468ad5230")
            );
        }

        #endregion

        #region Localisation

        IEnumerable<CultureInfo> _supportedCultures;
        /// <summary>
        /// Obtient les cultures supportées.
        /// </summary>
        IEnumerable<CultureInfo> SupportedCultures
        {
            get
            {
                if (_supportedCultures != null)
                    return _supportedCultures;
                var codes = ConfigurationManager.AppSettings["Cultures"].Split(';');
                _supportedCultures = codes.Select(CultureInfo.GetCultureInfo).ToArray();
                return _supportedCultures;
            }
        }

        /// <summary>
        /// Initialise les ressources de localisation.
        /// </summary>
        /// <returns><c>true</c> si l'initialisation s'est correctement produite.</returns>
        bool InitializeLocalization()
        {
            foreach (var culture in SupportedCultures)
            {
                LocalizationManager.SupportedCultures.Add(culture);
                IoC.Resolve<ILocalizationManager>().SupportedCultures.Add(culture);
            }

            // La première langue de la liste est la langue par défaut sur un poste dont la langue n'est pas supportée
            if (!LocalizationManager.SupportedCultures.Contains(LocalizationManager.CurrentCulture))
                LocalizationManager.CurrentCulture = SupportedCultures.First();
            if (!IoC.Resolve<ILocalizationManager>().SupportedCultures.Contains(LocalizationManager.CurrentCulture))
                IoC.Resolve<ILocalizationManager>().CurrentCulture = SupportedCultures.First();
            AnnotationsLib.LocalizationExt.CurrentCulture = AnnotationsLib.LocalizationExt.Dictionary.ContainsKey(LocalizationManager.CurrentCulture.Name) ?
                LocalizationManager.CurrentCulture :
                CultureInfo.GetCultureInfo(AnnotationsLib.LocalizationExt.Dictionary.Keys.First());
            DlhSoft.LocalizationExt.CurrentCulture = DlhSoft.LocalizationExt.Dictionary.ContainsKey(LocalizationManager.CurrentCulture.Name) ?
                LocalizationManager.CurrentCulture :
                CultureInfo.GetCultureInfo(DlhSoft.LocalizationExt.Dictionary.Keys.First());

            return true;
        }

        /// <summary>
        /// Comparateur de fournisseurs de localisation.
        /// </summary>
        class LocalizationProviderComparer : IEqualityComparer<ILocalizedResourceProvider>
        {
            public bool Equals(ILocalizedResourceProvider x, ILocalizedResourceProvider y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false;
                return x.UniqueID.Equals(y.UniqueID);
            }

            public int GetHashCode(ILocalizedResourceProvider obj)
            {
                return obj.UniqueID.GetHashCode();
            }
        }

        #endregion
    }
}