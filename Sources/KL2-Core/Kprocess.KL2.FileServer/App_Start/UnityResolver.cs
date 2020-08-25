using Kprocess.KL2.FileServer.Authentication;
using Kprocess.KL2.FileServer.Controller;
using KProcess;
using KProcess.KL2.APIClient;
using KProcess.KL2.Business.Impl.API;
using KProcess.KL2.Business.Impl.Shared.Migration;
using KProcess.KL2.Languages;
using KProcess.KL2.Languages.Provider;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Migration;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Business;
using KProcess.Supervision.Log4net;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http.Dependencies;
using LocalizationManager = KProcess.KL2.Languages.LocalizationManager;

namespace Kprocess.KL2.FileServer.App_Start
{
    public class UnityResolver : IDependencyResolver, IServiceBus
    {
        const string FileProviderKey = "FileProvider";

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
            container.RegisterType<ISystemInformationService, KProcess.KL2.Business.Impl.Desktop.SystemInformationService>(new HierarchicalLifetimeManager());
            container.RegisterType<IUISettingsService, UISettingsService>(new HierarchicalLifetimeManager());
            container.RegisterType<IValidateService, ValidateService>(new HierarchicalLifetimeManager());

            container.RegisterType<IAuthenticationService, KProcess.Ksmed.Security.Business.Desktop.AuthenticationService>(new HierarchicalLifetimeManager());
            container.RegisterType<INotificationService, NotificationService>(new HierarchicalLifetimeManager());
            container.RegisterType<IPrepareService, PrepareService>(new HierarchicalLifetimeManager());
            container.RegisterType<IPublicationService, KProcess.KL2.Business.Impl.Desktop.PublicationService>(new HierarchicalLifetimeManager());
            container.RegisterType<IScenarioCloneManager, MigratedScenarioCloneManager>(new HierarchicalLifetimeManager());
            container.RegisterType<ISharedScenarioActionsOperations, MigratedSharedScenarioActionsOperations>(new HierarchicalLifetimeManager());

            // Logging
            container.RegisterType<ITraceManager, Log4netTraceManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITraceWrapper, Log4netWrapper>(new ContainerControlledLifetimeManager());

            // API Client
            container.RegisterType<IAPIHttpClient, APIHttpClient>(new ContainerControlledLifetimeManager());

            // FileProvider
            string fileProvider = ConfigurationManager.AppSettings.AllKeys.Contains(FileProviderKey) ?
                ConfigurationManager.AppSettings[FileProviderKey] :
                "Local";
            switch(fileProvider)
            {
                case "Ftp":
                    container.RegisterType<IFileProvider, FtpFileProvider>(new ContainerControlledLifetimeManager());
                    break;
                case "SFtp":
                    container.RegisterType<IFileProvider, SFtpFileProvider>(new ContainerControlledLifetimeManager());
                    break;
                case "Local":
                default:
                    container.RegisterType<IFileProvider, LocalFileProvider>(new ContainerControlledLifetimeManager());
                    break;
            }
        }

        public void RegisterControllers()
        {
            container.RegisterType<FilesServiceController>(new HierarchicalLifetimeManager());
            container.RegisterType<PublicationServiceController>(new HierarchicalLifetimeManager());
            container.RegisterType<UtilitiesController>(new HierarchicalLifetimeManager());
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

        public TService Resolve<TService>()
        {
            return container.Resolve<TService>();
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
            container.RegisterInstance<TService>(service);
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
