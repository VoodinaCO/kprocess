using Kprocess.KL2.Notification.Authentication;
using KProcess;
using KProcess.KL2.APIClient;
using KProcess.KL2.Business.Impl.Desktop;
using KProcess.KL2.Business.Impl.Shared.Migration;
using KProcess.KL2.Languages;
using KProcess.KL2.Languages.Provider;
using KProcess.KL2.Notification.Jobs;
using KProcess.KL2.Notification.Service;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Migration;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Business;
using KProcess.Supervision.Log4net;
using Microsoft.Practices.Unity;
using System.IO;
using System.Reflection;
using LocalizationManager = KProcess.KL2.Languages.LocalizationManager;

namespace Kprocess.KL2.Notification.App_Start
{
    public static class UnityResolver
    {
        public static IUnityContainer RegisterServices(IUnityContainer container)
        {
            // HierarchicalLifetimeManager scope is new instance per request
            container.RegisterType<ISecurityContext, IdentitySecurityContext>(new ContainerControlledLifetimeManager());

            container.RegisterType<ILanguageStorageProvider, SQLiteLanguageStorageProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<ILanguageStorageProvider, SQLiteLanguageStorageProvider>(new InjectionConstructor($"Data Source={Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources", "Localization.sqlite")};"));
            container.RegisterInstance<ILocalizedStrings>(new LocalizedStrings(container.Resolve<ILanguageStorageProvider>()), new ContainerControlledLifetimeManager());
            container.RegisterType<ILocalizationManager, LocalizationManager>(new HierarchicalLifetimeManager());

            container.RegisterType<IAnalyzeService, AnalyzeService>(new HierarchicalLifetimeManager());
            container.RegisterType<IApplicationUsersService, ApplicationUsersService>(new HierarchicalLifetimeManager());

            container.RegisterType<IAppResourceService, AppResourceService>(new HierarchicalLifetimeManager());
            container.RegisterType<IDataBaseService, DataBaseService>(new HierarchicalLifetimeManager());
            container.RegisterType<IImportExportService, ImportExportService>(new HierarchicalLifetimeManager());
            container.RegisterType<IReferentialsService, ReferentialsService>(new HierarchicalLifetimeManager());
            container.RegisterType<ISharedDatabaseService, SharedDatabaseService>(new HierarchicalLifetimeManager());
            container.RegisterType<ISystemInformationService, SystemInformationService>(new HierarchicalLifetimeManager());
            container.RegisterType<IUISettingsService, UISettingsService>(new HierarchicalLifetimeManager());
            container.RegisterType<IValidateService, ValidateService>(new HierarchicalLifetimeManager());

            container.RegisterType<IAuthenticationService, KProcess.Ksmed.Security.Business.Desktop.AuthenticationService>(new HierarchicalLifetimeManager());
            container.RegisterType<IPrepareService, PrepareService>(new HierarchicalLifetimeManager());
            container.RegisterType<INotificationService, NotificationService>(new HierarchicalLifetimeManager());
            container.RegisterType<IPublicationService, PublicationService>(new HierarchicalLifetimeManager());
            container.RegisterType<IScenarioCloneManager, MigratedScenarioCloneManager>(new HierarchicalLifetimeManager());
            container.RegisterType<ISharedScenarioActionsOperations, MigratedSharedScenarioActionsOperations>(new HierarchicalLifetimeManager());

            // Logging
            container.RegisterType<ITraceManager, Log4netTraceManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITraceWrapper, Log4netWrapper>(new ContainerControlledLifetimeManager());

            // API Client
            container.RegisterType<IAPIHttpClient, APIHttpClient>(new ContainerControlledLifetimeManager());

            // Jobs and service
            container.RegisterType<NotificationJobService>();
            container.RegisterType<SendEmailsJob>();
            container.RegisterType<CreateEmailJob>();
            container.RegisterType<CollectReportsJob>();

            return container;
        }
    }
}
