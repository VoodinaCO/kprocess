using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using Kprocess.KL2.FileTransfer;
using Kprocess.KL2.TabletClient.Models;
using Kprocess.KL2.TabletClient.Services;
using Kprocess.KL2.TabletClient.ViewModel;
using KProcess;
using KProcess.Business;
using KProcess.KL2.APIClient;
using KProcess.KL2.Business.Impl.API;
using KProcess.KL2.Languages;
using KProcess.KL2.Languages.Provider;
using KProcess.KL2.SignalRClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Business;
using KProcess.Presentation.Windows.Service;
using KProcess.Supervision.Log4net;
using KProcess.Supervision.Trace;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using LocalizationManager = KProcess.KL2.Languages.LocalizationManager;

namespace Kprocess.KL2.TabletClient
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public static class Locator
    {
        /// <summary>
        /// Initializes a new instance of the Locator class.
        /// </summary>
        static Locator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}
            if (!SimpleIoc.Default.IsRegistered<ISecurityContext>())
                SimpleIoc.Default.Register<ISecurityContext, TabletSecurityContext>(true);
            SimpleIoc.Default.Register<ITraceWrapper, Log4netWrapper>(true);
            SimpleIoc.Default.Register<ITraceManager, Log4netTraceManager>(true);
            SimpleIoc.Default.Register<IDialogCoordinator, DialogCoordinator>();

            SimpleIoc.Default.Register<INavigationService, NavigationService>(true);

            SimpleIoc.Default.Register<IAPIHttpClient>(() => APIHttpClient.GetExtendedInstance(SimpleIoc.Default.GetInstance<ITraceManager>(), true), true);
            SimpleIoc.Default.Register<FFMEWebcam>(true);

            SimpleIoc.Default.Register<MainViewModel>();

            SimpleIoc.Default.Register<ILanguageStorageProvider>(() => new SQLiteLanguageStorageProvider("Data Source=Resources\\Localization.sqlite;"), true);
            SimpleIoc.Default.Register<ILocalizedStrings>(() => new LocalizedStrings(SimpleIoc.Default.GetInstance<ILanguageStorageProvider>()), true);
            SimpleIoc.Default.Register<ILocalizationManager, LocalizationManager>(true);

            SimpleIoc.Default.Register<IAuthenticationService, KProcess.Ksmed.Security.Business.API.AuthenticationService>();

            SimpleIoc.Default.Register<IPublicationService, PublicationService>();
            SimpleIoc.Default.Register<IPrepareService, PrepareService>();
            SimpleIoc.Default.Register<INotificationService, NotificationService>();
            SimpleIoc.Default.Register<IApplicationUsersService, ApplicationUsersService>();

            SimpleIoc.Default.Register<IEventSignalR, EventSignalR>(true);
            SimpleIoc.Default.Register<ISignalRFactory, SignalRFactory>(true);

            SimpleIoc.Default.GetInstance<ISignalRFactory>().Initialization(ServiceLocator.Current.GetInstance<IEventSignalR>());

            SimpleIoc.Default.Register<IAPIManager, APIManager>(true);

            SimpleIoc.Default.Register<FileTransferManager>(true);
            SimpleIoc.Default.Register<DownloadManager>(true);

            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
            PresentationTraceSources.DataBindingSource.Listeners.Add(new BindingErrorTraceListener(TraceManager));
        }

        public static MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public static ISignalRFactory SignalRFactory => ServiceLocator.Current.GetInstance<ISignalRFactory>();

        public static DownloadManager DownloadManager => ServiceLocator.Current.GetInstance<DownloadManager>();

        #region Properties

        /// <summary>
        /// Obtient le service de navigation
        /// </summary>
        public static INavigationService Navigation => ServiceLocator.Current.GetInstance<INavigationService>();

        /// <summary>
        /// Obtient le service de synchro API
        /// </summary>
        public static IAPIManager APIManager => ServiceLocator.Current.GetInstance<IAPIManager>();

        /// <summary>
        /// Obtient le service de traduction
        /// </summary>
        public static ILocalizationManager LocalizationManager => ServiceLocator.Current.GetInstance<ILocalizationManager>();

        /// <summary>
        /// Obtient le service de trace
        /// </summary>
        public static ITraceManager TraceManager => ServiceLocator.Current.GetInstance<ITraceManager>();

        public static FFMEWebcam FFMEWebcam => ServiceLocator.Current.GetInstance<FFMEWebcam>();

        #endregion


        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        public static T Resolve<T>() =>
            ServiceLocator.Current.GetInstance<T>();

        public static T GetInstance<T>() where T : ViewModelBase =>
            ServiceLocator.Current.GetInstance<T>();

        public static T GetService<T>() where T : IBusinessService =>
            (T)ServiceLocator.Current.GetService(typeof(T));

        public static async Task<TUserControl> GetView<TUserControl, TViewModel>()
            where TUserControl : UserControl, new()
            where TViewModel : ViewModelBase, new()
        {
            TUserControl result = null;
            await DispatcherHelper.RunAsync(() => result = new TUserControl { DataContext = new TViewModel() });
            return result;
        }

        public static async Task<TUserControl> GetView<TUserControl, TViewModel>(TViewModel viewModel)
            where TUserControl : UserControl, new()
            where TViewModel : ViewModelBase, new()
        {
            TUserControl result = null;
            await DispatcherHelper.RunAsync(() => result = new TUserControl { DataContext = viewModel });
            return result;
        }

        public static Flyout GetFlyout<TFlyout, TViewModel>()
            where TFlyout : Flyout, new()
            where TViewModel : ViewModelBase, new()
        {
            MainViewModel main = ServiceLocator.Current.GetInstance<MainViewModel>();
            Flyout flyout = null;
            if (main.Flyouts.Any(_ => _.Name == typeof(TFlyout).Name))
                flyout = main.Flyouts.Single(_ => _.Name == typeof(TFlyout).Name);
            if (flyout == null)
            {
                flyout = new TFlyout { Name = typeof(TFlyout).Name, DataContext = new TViewModel() };
                main.Flyouts.Add(flyout);
            }
            return flyout;
        }

        public static Flyout GetFlyout<TFlyout>(object context)
            where TFlyout : Flyout, new()
        {
            MainViewModel main = ServiceLocator.Current.GetInstance<MainViewModel>();
            Flyout flyout = null;
            if (main.Flyouts.Any(_ => _.Name == typeof(TFlyout).Name))
                flyout = main.Flyouts.Single(_ => _.Name == typeof(TFlyout).Name);
            if (flyout == null)
            {
                flyout = new TFlyout { Name = typeof(TFlyout).Name, DataContext = context };
                main.Flyouts.Add(flyout);
            }
            return flyout;
        }
    }
}