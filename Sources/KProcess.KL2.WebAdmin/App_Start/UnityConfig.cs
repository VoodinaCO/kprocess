using KProcess.KL2.APIClient;
using KProcess.KL2.Business.Impl.Desktop;
using KProcess.KL2.Business.Impl.Shared.Migration;
using KProcess.KL2.Languages;
using KProcess.KL2.Languages.Provider;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Controllers;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Migration;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Activation;
using KProcess.Ksmed.Security.Activation.Providers;
using KProcess.Ksmed.Security.Business;
using KProcess.Ksmed.Security.Business.API;
using KProcess.Supervision.Log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Practices.Unity;
using System;
using LocalizationManager = KProcess.KL2.Languages.LocalizationManager;

namespace KProcess.KL2.WebAdmin
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container

        private static readonly Lazy<IUnityContainer> _container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => _container.Value;

        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();

            container.RegisterType<ILanguageStorageProvider, SQLiteLanguageStorageProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<ILanguageStorageProvider, SQLiteLanguageStorageProvider>(new InjectionConstructor("Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + "Localization.sqlite;"));
            container.RegisterInstance<ILocalizedStrings>(new LocalizedStrings(container.Resolve<ILanguageStorageProvider>()), new ContainerControlledLifetimeManager());
            container.RegisterType<ILocalizationManager, LocalizationManager>(new HierarchicalLifetimeManager());

            container.RegisterType<IApplicationUsersService, ApplicationUsersService>(new HierarchicalLifetimeManager());
            //container.RegisterType<IUserStore<KL2IdentityUser>, UserStore>(new InjectionConstructor());
            //container.RegisterType<IRoleStore<KL2IdentityRole>, RoleStore>();
            
            container.RegisterType<UserManager<KL2IdentityUser>>(new HierarchicalLifetimeManager());
            container.RegisterType<IUserStore<KL2IdentityUser>, UserStore>(new HierarchicalLifetimeManager());
            container.RegisterType<ISecurityContext, IdentitySecurityContext>(new HierarchicalLifetimeManager());
            container.RegisterType<IAnalyzeService, AnalyzeService>(new HierarchicalLifetimeManager());
            container.RegisterType<IApplicationUsersService, ApplicationUsersService>(new HierarchicalLifetimeManager());
            container.RegisterType<IPublicationService, KProcess.KL2.Business.Impl.API.PublicationService>(new HierarchicalLifetimeManager());
            container.RegisterType<INotificationService, NotificationService>(new HierarchicalLifetimeManager());
            container.RegisterType<IPrepareService, PrepareService>(new HierarchicalLifetimeManager());
            container.RegisterType<IScenarioCloneManager, MigratedScenarioCloneManager>(new HierarchicalLifetimeManager());
            container.RegisterType<ISharedScenarioActionsOperations, MigratedSharedScenarioActionsOperations>(new HierarchicalLifetimeManager());

            container.RegisterType<IAppResourceService, AppResourceService>(new HierarchicalLifetimeManager());
            container.RegisterType<IDataBaseService, DataBaseService>(new HierarchicalLifetimeManager());
            container.RegisterType<IImportExportService, ImportExportService>(new HierarchicalLifetimeManager());
            container.RegisterType<IReferentialsService, ReferentialsService>(new HierarchicalLifetimeManager());
            container.RegisterType<ISharedDatabaseService, SharedDatabaseService>(new HierarchicalLifetimeManager());
            container.RegisterType<ISystemInformationService, SystemInformationService>(new HierarchicalLifetimeManager());
            container.RegisterType<IUISettingsService, UISettingsService>(new HierarchicalLifetimeManager());
            container.RegisterType<IValidateService, ValidateService>(new HierarchicalLifetimeManager());
            container.RegisterType<IMachineIdentifierProvider, MachineIdentifierProvider>(new HierarchicalLifetimeManager());
            container.RegisterType<IUserInformationProvider, UserInformationProvider>(new HierarchicalLifetimeManager());
            container.RegisterType<ISystemInformationService, SystemInformationService>(new HierarchicalLifetimeManager());

            container.RegisterType<IAuthenticationService, AuthenticationService>(new HierarchicalLifetimeManager());
            container.RegisterType<IFileProvider, LocalFileProvider>(new HierarchicalLifetimeManager());
            //container.RegisterType<IFileProvider, FtpFileProvider>(new HierarchicalLifetimeManager());
            //container.RegisterType<IFileProvider, SFtpFileProvider>(new HierarchicalLifetimeManager());

            // Logging
            container.RegisterType<ITraceManager, Log4netTraceManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITraceWrapper, Log4netWrapper>(new ContainerControlledLifetimeManager());

            // API Client
            container.RegisterType<IAPIHttpClient, APIHttpClient>(new HierarchicalLifetimeManager());

            //container.RegisterType<AccountController>(new InjectionConstructor());
            container.RegisterType<AccountController>(
               new InjectionConstructor(
                container.Resolve<ILanguageStorageProvider>(),
                container.Resolve<ILocalizedStrings>(),
                container.Resolve<IAPIHttpClient>(),
                container.Resolve<IApplicationUsersService>(),
                container.Resolve<ILocalizationManager>()));
        }
    }
}