using KProcess.KL2.API.Authentication;
using KProcess.KL2.API.Controller;
using KProcess.KL2.APIClient;
using KProcess.KL2.Business.Impl.Desktop;
using KProcess.KL2.Business.Impl.Shared.Migration;
using KProcess.KL2.Languages;
using KProcess.KL2.Languages.Provider;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Migration;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Business;
using KProcess.Ksmed.Security.Business.Desktop;
using KProcess.Supervision.Log4net;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using LocalizationManager = KProcess.KL2.Languages.LocalizationManager;

namespace KProcess.KL2.API.App_Start
{
    public class UnityResolver : IDependencyResolver, IServiceBus
    {
        protected IUnityContainer container;

        public UnityResolver(IUnityContainer container)
        {
            this.container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public void RegisterServices()
        {
            // HierarchicalLifetimeManager scope is new instance per request
            container.RegisterType<ISecurityContext, IdentitySecurityContext>(new HierarchicalLifetimeManager());

            container.RegisterType<ILanguageStorageProvider, SQLiteLanguageStorageProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<ILanguageStorageProvider, SQLiteLanguageStorageProvider>(new InjectionConstructor("Data Source=Resources\\Localization.sqlite;"));
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

            container.RegisterType<IAuthenticationService, AuthenticationService>(new HierarchicalLifetimeManager());
            container.RegisterType<INotificationService, NotificationService>(new HierarchicalLifetimeManager());
            container.RegisterType<IPrepareService, PrepareService>(new HierarchicalLifetimeManager());
            container.RegisterType<IPublicationService, PublicationService>(new HierarchicalLifetimeManager());
            container.RegisterType<IScenarioCloneManager, MigratedScenarioCloneManager>(new HierarchicalLifetimeManager());
            container.RegisterType<ISharedScenarioActionsOperations, MigratedSharedScenarioActionsOperations>(new HierarchicalLifetimeManager());

            //ApiClient
            container.RegisterType<IAPIHttpClient, APIHttpClient>(new HierarchicalLifetimeManager());

            // Logging
            container.RegisterType<ITraceManager, Log4netTraceManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITraceWrapper, Log4netWrapper>(new ContainerControlledLifetimeManager());
        }

        public void RegisterControllers()
        {
            container.RegisterType<LicenseController>(new HierarchicalLifetimeManager());
            container.RegisterType<CloudFilesController>(new HierarchicalLifetimeManager());
            container.RegisterType<AnalyzeServiceController>(new HierarchicalLifetimeManager());
            container.RegisterType<ApplicationUsersServiceController>(new HierarchicalLifetimeManager());
            container.RegisterType<AppResourceServiceController>(new HierarchicalLifetimeManager());
            container.RegisterType<AuthenticationServiceController>(new HierarchicalLifetimeManager());
            container.RegisterType<ImportExportServiceController>(new HierarchicalLifetimeManager());
            container.RegisterType<PrepareServiceController>(new HierarchicalLifetimeManager());
            container.RegisterType<ReferentialsServiceController>(new HierarchicalLifetimeManager());
            container.RegisterType<SharedDatabaseServiceController>(new HierarchicalLifetimeManager());
            container.RegisterType<UISettingsServiceController>(new HierarchicalLifetimeManager());
            container.RegisterType<UtilitiesController>(new HierarchicalLifetimeManager());
            container.RegisterType<ValidateServiceController>(new HierarchicalLifetimeManager());
        }

        public object GetService(Type serviceType)
        {
            try
            {
                if (container.IsRegistered(serviceType))
                    return container.Resolve(serviceType);
                return null;
            }
            catch (ResolutionFailedException) 
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public IDependencyScope BeginScope()
        {
            var child = container.CreateChildContainer();
            return new UnityResolver(child);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            container.Dispose();
        }

        #region IServiceBus implementation

        public TService Get<TService>() where TService : IService
        {
            return container.Resolve<TService>();
        }

        public bool IsAvailable<TService>()
        {
            return container.IsRegistered<TService>();
        }

        public IServiceBus Register<TService>(TService service) where TService : IService
        {
            container.RegisterInstance(service);
            return this;
        }

        public IServiceBus RegisterType<TIService, TService>()
            where TIService : IService
            where TService : TIService
        {
            container.RegisterType<TIService, TService>(new ContainerControlledLifetimeManager());
            return this;
        }

        public IServiceBus Unregister<TService>() where TService : IService
        {
            container.Teardown(container.Resolve<TService>());
            return this;
        }

        #endregion
    }
}
