using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Activation;
using KProcess.Presentation.Windows;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.Shell.Menus
{

    /// <summary>
    /// Représente la classe de base d'un menu.
    /// </summary>
    abstract class NavigationMenu : IMenuDefinition
    {

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="NavigationMenu"/>.
        /// </summary>
        /// <param name="code">le code.</param>
        /// <param name="menuStrip">la barre de menus.</param>
        /// <param name="titleKey">la clé du titre.</param>
        protected NavigationMenu(string code, MenuStrip menuStrip, string titleKey)
        {
            Code = code;
            Strip = menuStrip;
            TitleResourceKey = titleKey;
        }

        /// <summary>
        /// Initialise le menu.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Obtient le bus d'évènements.
        /// </summary>
        [Import]
        protected IEventBus EventBus { get; private set; }

        /// <summary>
        /// Obtient le bus de services.
        /// </summary>
        [Import]
        protected IServiceBus ServiceBus { get; private set; }

        /// <summary>
        /// Obtient le code identifiant le menu.
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Obtient la barre de menu à laquelle appartient ce menu;
        /// </summary>
        public MenuStrip Strip { get; private set; }

        /// <summary>
        /// Obtient le titre du menu.
        /// </summary>
        public string TitleResourceKey { get; private set; }

        /// <summary>
        /// Survient lorsque le titre a changé.
        /// </summary>
        public event EventHandler TitleChanged;

        /// <summary>
        /// Appelé  afin de lever l'évènement TitleChanged.
        /// </summary>
        protected virtual void OnTitleChanged() =>
            TitleChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Survient lorsque la propriété IsEnabled doit être rafraîchie.
        /// </summary>
        public event EventHandler IsEnabledInvalidated;

        /// <summary>
        /// Appelé afin de lever l'évènement IsEnabledInvalidated.
        /// </summary>
        protected virtual void OnIsEnabledInvalidated() =>
            IsEnabledInvalidated?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public virtual Func<bool> IsEnabledDelegate { get { return null; } }

        /// <summary>
        /// Obtient un délégué qui revient true si le projet courant n'est pas null.
        /// </summary>
        internal static Func<bool> IsEnabledWhenProjectSelected(IServiceBus serviceBus)
        {
            return () => serviceBus.Get<IProjectManagerService>().CurrentProject != null;
        }

        /// <summary>
        /// Obtient un délégué qui revient true si le projet courant n'est pas null et qu'il n'est pas clotûré.
        /// </summary>
        internal static Func<bool> IsEnabledWhenProjectSelectedAndOpened(IServiceBus serviceBus)
        {
            return () =>
            {
                int? projectId = serviceBus.Get<IProjectManagerService>().CurrentProject?.ProjectId;
                if (projectId == null)
                    return false;
                IPrepareService prepareService = serviceBus.Get<IPrepareService>();
                Project currentProject = prepareService.GetProjectSync(projectId.Value);
                if (currentProject == null)
                    return false;
                return true;
            };
        }

        /// <summary>
        /// Obtient un délégué qui revient true si le projet courant n'est pas null.
        /// </summary>
        internal static Func<bool> IsEnabledWhenProjectHasAtLeastOneScenario(IServiceBus serviceBus)
        {
            return () => serviceBus.Get<IProjectManagerService>().Scenarios.Any();
        }

        /// <summary>
        /// Obtient un délégué qui revient true si le projet comporte au moins un scénario figé.
        /// </summary>
        internal static Func<bool> IsEnabledWhenProjectHasAtLeastOneFixedScenario(IServiceBus serviceBus)
        {
            return () => serviceBus.Get<IProjectManagerService>().Scenarios.Any(x => x.StateCode == KnownScenarioStates.Validated);
        }

        /// <summary>
        /// Obtient un délégué qui revient true si une publication existe.
        /// </summary>
        /*internal static Func<bool> IsEnabledWhenPublicationExists(IServiceBus serviceBus)
        {
            return () =>
            {
                int? projectId = serviceBus.Get<IProjectManagerService>().CurrentProject?.ProjectId;
                if (projectId == null)
                    return false;
                IPrepareService prepareService = serviceBus.Get<IPrepareService>();
                Project currentProject = prepareService.GetProjectSync(projectId.Value);
                if (currentProject == null)
                    return false;
                return prepareService.PublicationExistsForProcessSync(currentProject.ProcessId);
            };
        }*/

        /// <summary>
        /// Obtient un délégué qui revient true si une publication est en cours.
        /// </summary>
        /*internal static Func<bool> IsEnabledWhenCurrentPublicationExists(IServiceBus serviceBus)
        {
            return () =>
            {
                if (serviceBus.Get<INavigationService>() is IPublicationManagerService publicationManager)
                    return publicationManager.CurrentPublication != null;
                return false;
            };
        }*/

        /// <summary>
        /// Obtient un délégué qui revient true si une publication est en cours et que tous les process liés (si existants) sont publiés.
        /// </summary>
        /*internal static Func<bool> IsEnabledWhenCurrentPublicationExistsAndAllLinkedProcessArePublished(IServiceBus serviceBus)
        {
            return () =>
            {
                if (serviceBus.Get<INavigationService>() is IPublicationManagerService publicationManager)
                    return publicationManager.CurrentPublication != null && publicationManager.CurrentPublication.PublishMode != 0 && serviceBus.Get<IPrepareService>().AllLinkedProcessArePublishedSync(publicationManager.CurrentPublication.ScenarioId).Result;
                return false;
            };
        }*/

        /// <summary>
        /// Obtient un délégué qui revient true si une publication est en cours et que la mise en page a été définie.
        /// </summary>
        internal static Func<bool> IsEnabledWhenCurrentPublicationExistsAndFormatIsSet(IServiceBus serviceBus)
        {
            return () =>
            {
                if (serviceBus.Get<INavigationService>() is IPublicationManagerService publicationManager)
                {
                    if (publicationManager.CurrentPublication == null)
                        return false;
                    if ((publicationManager.CurrentPublication.PublishMode & PublishModeEnum.Formation) == PublishModeEnum.Formation && publicationManager.CurrentPublication.Formation_Disposition == null)
                        return false;
                    if ((publicationManager.CurrentPublication.PublishMode & PublishModeEnum.Inspection) == PublishModeEnum.Inspection && publicationManager.CurrentPublication.Inspection_Disposition == null)
                        return false;
                    if ((publicationManager.CurrentPublication.PublishMode & PublishModeEnum.Audit) == PublishModeEnum.Audit && publicationManager.CurrentPublication.Audit_Disposition == null)
                        return false;
                    return true;
                }
                return false;
            };
        }

        /// <summary>
        /// Obtient un délégué qui renvoie true si le projet courant possède au moins un scénario cible.
        /// </summary>
        internal static Func<bool> IsEnabledWhenProjectHasAtLeastOneTargetScenario(IServiceBus serviceBus)
        {
            return () => serviceBus.Get<IProjectManagerService>().Scenarios.Any(s => s.NatureCode == KnownScenarioNatures.Target);
        }

        /// <summary>
        /// Obtient un délégué qui renvoie true si le projet courant possède au moins un scénario de validation.
        /// </summary>
        internal static Func<bool> IsEnabledWhenProjectHasAtLeastOneValidationScenario(IServiceBus serviceBus)
        {
            return () => serviceBus.Get<IProjectManagerService>().Scenarios.Any(s => s.NatureCode == KnownScenarioNatures.Realized);
        }

    }

    /// <summary>
    /// Représente la classe de base d'un sous-menu dont le but est d'afficher un ViewModel/View.
    /// </summary>
    /// <typeparam name="TViewModel">Le type de view model à afficher.</typeparam>
    abstract class NavigationSubMenu<TViewModel> : ISubMenuDefinition
        where TViewModel : IFrameContentViewModel
    {

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="NavigationSubMenu&lt;TViewModel&gt;"/>.
        /// </summary>
        /// <param name="parentCode">Le code du menu parent.</param>
        /// <param name="code">Le code du sous-menu.</param>
        /// <param name="titleKey">La clé du titre.</param>
        protected NavigationSubMenu(string parentCode, string code, string titleKey)
        {
            ParentCode = parentCode;
            Code = code;
            TitleResourceKey = titleKey;
        }

        /// <summary>
        /// Initialise le menu.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Obtient le bus de services.
        /// </summary>
        [Import]
        protected IServiceBus ServiceBus { get; private set; }

        /// <summary>
        /// Obtient le bus d'évènements.
        /// </summary>
        [Import]
        protected IEventBus EventBus { get; private set; }

        /// <summary>
        /// Obtient le code identifiant le menu parent.
        /// </summary>
        public string ParentCode { get; private set; }

        /// <summary>
        /// Obtient le code identifiant le menu.
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Obtient le titre du menu.
        /// </summary>
        public string TitleResourceKey { get; protected set; }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public virtual Func<bool> IsEnabledDelegate { get { return null; } }

        /// <summary>
        /// Obtient le délégué appelé lorsque le menu est cliqué.
        /// </summary>
        public virtual Func<IServiceBus, Task<bool>> Action
        {
            get { return sb => sb.Get<INavigationService>().TryShow<TViewModel>(); }
        }

        /// <summary>
        /// Obtient le type du ViewModel.
        /// </summary>
        public Type ViewModelType =>
            typeof(TViewModel);

        /// <summary>
        /// Survient lorsque le titre a changé.
        /// </summary>
        public event EventHandler TitleChanged;

        /// <summary>
        /// Appelé  afin de lever l'évènement TitleChanged.
        /// </summary>
        protected virtual void OnTitleChanged() =>
            TitleChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Survient lorsque la propriété IsEnabled doit être rafraîchie.
        /// </summary>
        public event EventHandler IsEnabledInvalidated;

        /// <summary>
        /// Appelé afin de lever l'évènement IsEnabledInvalidated.
        /// </summary>
        protected virtual void OnIsEnabledInvalidated() =>
            IsEnabledInvalidated?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public abstract bool IsSecurityProjectContext { get; }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public abstract string[] RolesCanRead { get; }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public abstract string[] RolesCanWrite { get; }

        /// <summary>
        /// Obtient la liste des fonctionnalités permettant d'accéder à cet écran.
        /// </summary>
        public virtual short[] FeaturesCanRead =>
            new short[] { (short)ActivationFeatures.All, (short)ActivationFeatures.ReadOnly };

        /// <summary>
        /// Obtient la liste des fonctionnalités permettant d'écrire sur l'écran.
        /// Par défaut, seul "AllFeatures" y a accès.
        /// </summary>
        public virtual short[] FeaturesCanWrite =>
            new short[] { (short)ActivationFeatures.All };

        /// <summary>
        /// Obtient un délégué permettant de déterminer si l'utilisateur en cours peut lire.
        /// </summary>
        public virtual Func<string, bool> CustomCanRead => null;

        /// <summary>
        /// Obtient un délégué permettant de déterminer si l'utilisateur en cours peut écrire.
        /// </summary>
        public virtual Func<string, bool> CustomCanWrite => null;
    }

    #region Project

    class Prepare : NavigationMenu
    {
        public Prepare()
            : base(KnownMenus.Prepare, MenuStrip.Project, "Common_Prepare")
        {
        }
    }

    class PrepareProjects : NavigationSubMenu<IPrepareProjectsViewModel>
    {
        public PrepareProjects()
            : base(
                KnownMenus.Prepare, KnownMenus.PrepareProject,
                "Common_Prepare_Project"
                )
        {
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst, KnownRoles.Contributor, KnownRoles.Technician }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return false; }
        }
    }

    class PrepareMembers : NavigationSubMenu<IPrepareMembersViewModel>
    {
        public PrepareMembers()
            : base(
                KnownMenus.Prepare, KnownMenus.PrepareMembers,
                "Common_Prepare_Members"
                )
        {
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate
        {
            get { return NavigationMenu.IsEnabledWhenProjectSelectedAndOpened(base.ServiceBus); }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst, KnownRoles.Contributor }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return true; }
        }
    }

    class PrepareReferentials : NavigationSubMenu<IPrepareReferentialsViewModel>
    {
        public PrepareReferentials()
            : base(
                KnownMenus.Prepare, KnownMenus.PrepareReferentials,
                "Common_Prepare_Referentials"
                )
        {
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate
        {
            get { return NavigationMenu.IsEnabledWhenProjectSelectedAndOpened(base.ServiceBus); }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst, KnownRoles.Contributor }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return true; }
        }
    }

    class PrepareVideos : NavigationSubMenu<IPrepareVideosViewModel>
    {
        public PrepareVideos()
            : base(
                KnownMenus.Prepare, KnownMenus.PrepareVideos,
                "Common_Prepare_Videos"
                )
        {
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate
        {
            get { return NavigationMenu.IsEnabledWhenProjectSelectedAndOpened(base.ServiceBus); }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst, KnownRoles.Contributor }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return true; }
        }
    }

    class PrepareScenarios : NavigationSubMenu<IPrepareScenariosViewModel>
    {
        public PrepareScenarios()
            : base(
                KnownMenus.Prepare, KnownMenus.PrepareScenarios,
                "Common_Prepare_Scenarios"
                )
        {
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate
        {
            get { return NavigationMenu.IsEnabledWhenProjectSelectedAndOpened(base.ServiceBus); }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst, KnownRoles.Contributor }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return true; }
        }
    }

    class Analyze : NavigationMenu
    {
        public Analyze()
            : base(KnownMenus.Analyze, MenuStrip.Project, "Common_Analyze")
        {
        }

        public override void Initialize()
        {
            base.EventBus.Subscribe<ScenariosCollectionChangedEvent>(e =>
            {
                base.OnIsEnabledInvalidated();
            });
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate
        {
            get { return NavigationMenu.IsEnabledWhenProjectHasAtLeastOneScenario(base.ServiceBus); }
        }
    }

    class AnalyzeAcquire : NavigationSubMenu<IAnalyzeAcquireViewModel>
    {
        public AnalyzeAcquire()
            : base(
                KnownMenus.Analyze, KnownMenus.AnalyzeAcquire,
                "Common_Analyze_Acquire"
                )
        {
        }

        public override void Initialize()
        {
            base.EventBus.Subscribe<ScenariosCollectionChangedEvent>(e =>
            {
                base.OnIsEnabledInvalidated();
            });
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate
        {
            get { return NavigationMenu.IsEnabledWhenProjectHasAtLeastOneScenario(base.ServiceBus); }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst, KnownRoles.Contributor }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return true; }
        }
    }

    class AnalyzeBuild : NavigationSubMenu<IAnalyzeBuildViewModel>
    {
        public AnalyzeBuild()
            : base(
                KnownMenus.Analyze, KnownMenus.AnalyzeBuild,
                "Common_Analyze_Build"
                )
        {
        }

        public override void Initialize()
        {
            base.EventBus.Subscribe<ScenarioChangedEvent>(e =>
            {
                if (e.Scenario == null || e.Scenario.NatureCode == KProcess.Ksmed.Business.KnownScenarioNatures.Initial)
                    this.TitleResourceKey = "Common_Analyze_Build";
                else
                    this.TitleResourceKey = "Common_Analyze_Simulate";

                base.OnTitleChanged();
            });

            base.EventBus.Subscribe<ScenariosCollectionChangedEvent>(e =>
            {
                base.OnIsEnabledInvalidated();
            });
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate
        {
            get { return NavigationMenu.IsEnabledWhenProjectHasAtLeastOneScenario(base.ServiceBus); }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst, KnownRoles.Contributor }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return true; }
        }
    }

    class AnalyzeSimulate : NavigationSubMenu<IAnalyzeSimulateViewModel>
    {
        public AnalyzeSimulate()
            : base(
                KnownMenus.Analyze, KnownMenus.AnalyzeSimulate,
                "Common_Analyze_Compare"
                )
        {
        }

        public override void Initialize()
        {
            base.EventBus.Subscribe<ScenariosCollectionChangedEvent>(e =>
            {
                base.OnIsEnabledInvalidated();
            });
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate
        {
            get { return NavigationMenu.IsEnabledWhenProjectHasAtLeastOneTargetScenario(base.ServiceBus); }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst, KnownRoles.Contributor }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return true; }
        }
    }

    class AnalyzeRestore : NavigationSubMenu<IAnalyzeRestitutionViewModel>
    {
        public AnalyzeRestore()
            : base(
                KnownMenus.Analyze, KnownMenus.AnalyzeRestore,
                "Common_Analyze_Restore"
                )
        {
        }

        public override void Initialize()
        {
            base.EventBus.Subscribe<ScenariosCollectionChangedEvent>(e =>
            {
                base.OnIsEnabledInvalidated();
            });
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate
        {
            get { return NavigationMenu.IsEnabledWhenProjectHasAtLeastOneScenario(base.ServiceBus); }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst, KnownRoles.Contributor }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return true; }
        }
    }

    //class Track : NavigationMenu
    //{
    //    public Track()
    //        : base(KnownMenus.Track, MenuStrip.Project, "Common_Track")
    //    {
    //    }
    //}

    class Validate : NavigationMenu
    {
        public Validate()
            : base(KnownMenus.Validate, MenuStrip.Project, "Common_Validate")
        {
        }

        public override void Initialize()
        {
            base.EventBus.Subscribe<ScenariosCollectionChangedEvent>(e =>
            {
                base.OnIsEnabledInvalidated();
            });
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate
        {
            get { return IsEnabledWhenProjectHasAtLeastOneValidationScenario(base.ServiceBus); }
        }

        /// <summary>
        /// Détermine si un technicien peut lire le scénario validé.
        /// </summary>
        public static Func<string, bool> CanTechnicianReadWhenValidationScenarioIsLocked
        {
            get
            {
                return new Func<string, bool>(role =>
                {
                    Assertion.NotNull(role, "role"); // Ne peut être null car toujours lié au projet.
                    if (role == KnownRoles.Technician)
                        return IoC.Resolve<IServiceBus>().Get<IProjectManagerService>().Scenarios.Any(s => s.NatureCode == KnownScenarioNatures.Realized && s.IsLocked);
                    else
                        return true;
                });
            }
        }
    }


    class ValidateAcquire : NavigationSubMenu<IValidateAcquireViewModel>
    {
        public ValidateAcquire()
            : base(
                KnownMenus.Validate, KnownMenus.ValidateAcquire,
                "Common_Validate_Acquire"
                )
        {
        }

        public override void Initialize()
        {
            base.EventBus.Subscribe<ScenariosCollectionChangedEvent>(e =>
            {
                base.OnIsEnabledInvalidated();
            });
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate
        {
            get { return NavigationMenu.IsEnabledWhenProjectHasAtLeastOneValidationScenario(base.ServiceBus); }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst, KnownRoles.Contributor, KnownRoles.Technician }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return true; }
        }

        /// <summary>
        /// Obtient un délégué permettant de déterminer si l'utilisateur en cours peut lire.
        /// </summary>
        public override Func<string, bool> CustomCanRead
        {
            get { return Validate.CanTechnicianReadWhenValidationScenarioIsLocked; }
        }
    }

    class ValidateBuild : NavigationSubMenu<IValidateBuildViewModel>
    {
        public ValidateBuild()
            : base(
                KnownMenus.Validate, KnownMenus.ValidateBuild,
                "Common_Validate_Build"
                )
        {
        }

        public override void Initialize()
        {
            base.EventBus.Subscribe<ScenarioChangedEvent>(e =>
            {
                if (e.Scenario == null || e.Scenario.NatureCode == KProcess.Ksmed.Business.KnownScenarioNatures.Initial)
                    this.TitleResourceKey = "Common_Validate_Build";
                else
                    this.TitleResourceKey = "Common_Validate_Simulate";

                base.OnTitleChanged();
            });

            base.EventBus.Subscribe<ScenariosCollectionChangedEvent>(e =>
            {
                base.OnIsEnabledInvalidated();
            });
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate
        {
            get { return NavigationMenu.IsEnabledWhenProjectHasAtLeastOneValidationScenario(base.ServiceBus); }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst, KnownRoles.Contributor, KnownRoles.Technician }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return true; }
        }

        /// <summary>
        /// Obtient un délégué permettant de déterminer si l'utilisateur en cours peut lire.
        /// </summary>
        public override Func<string, bool> CustomCanRead
        {
            get { return Validate.CanTechnicianReadWhenValidationScenarioIsLocked; }
        }
    }

    class ValidateSimulate : NavigationSubMenu<IValidateSimulateViewModel>
    {
        public ValidateSimulate()
            : base(
                KnownMenus.Validate, KnownMenus.ValidateSimulate,
                "Common_Validate_Compare"
                )
        {
        }

        public override void Initialize()
        {
            base.EventBus.Subscribe<ScenariosCollectionChangedEvent>(e =>
            {
                base.OnIsEnabledInvalidated();
            });
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate
        {
            get { return NavigationMenu.IsEnabledWhenProjectHasAtLeastOneValidationScenario(base.ServiceBus); }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst, KnownRoles.Contributor, KnownRoles.Technician }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return true; }
        }

        /// <summary>
        /// Obtient un délégué permettant de déterminer si l'utilisateur en cours peut lire.
        /// </summary>
        public override Func<string, bool> CustomCanRead
        {
            get { return Validate.CanTechnicianReadWhenValidationScenarioIsLocked; }
        }
    }

    class ValidateRestore : NavigationSubMenu<IValidateRestitutionViewModel>
    {
        public ValidateRestore()
            : base(
                KnownMenus.Validate, KnownMenus.ValidateRestore,
                "Common_Validate_Restore"
                )
        {
        }

        public override void Initialize()
        {
            base.EventBus.Subscribe<ScenariosCollectionChangedEvent>(e =>
            {
                base.OnIsEnabledInvalidated();
            });
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate
        {
            get { return NavigationMenu.IsEnabledWhenProjectHasAtLeastOneValidationScenario(base.ServiceBus); }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst, KnownRoles.Contributor, KnownRoles.Technician }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return true; }
        }

        /// <summary>
        /// Obtient un délégué permettant de déterminer si l'utilisateur en cours peut lire.
        /// </summary>
        public override Func<string, bool> CustomCanRead
        {
            get { return Validate.CanTechnicianReadWhenValidationScenarioIsLocked; }
        }
    }

    //class Capitalize : NavigationMenu
    //{
    //    public Capitalize()
    //        : base(KnownMenus.Capitalize, MenuStrip.Project, "Common_Capitalize")
    //    {
    //    }

    //    /// <summary>
    //    /// Obtient le délégué déterminant si ce menu est activé.
    //    /// </summary>
    //    public override Func<bool> IsEnabledDelegate
    //    {
    //        get { return () => false; }
    //    }

    //    // A intégrer dans le sous menu

    //    ///// <summary>
    //    ///// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
    //    ///// </summary>
    //    //public override string[] RolesCanRead
    //    //{
    //    //    get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst, KnownRoles.Contributor, KnownRoles.Technician }; }
    //    //}

    //    ///// <summary>
    //    ///// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
    //    ///// </summary>
    //    //public override string[] RolesCanWrite
    //    //{
    //    //    get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
    //    //}

    //    ///// <summary>
    //    ///// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
    //    ///// </summary>
    //    //public override bool IsSecurityProjectContext
    //    //{
    //    //    get { return true; }
    //    //}
    //}

    /*class Publish : NavigationMenu
    {
        public Publish()
            : base(KnownMenus.Publish, MenuStrip.Project, "Common_Publish")
        {
        }

        public override void Initialize()
        {
            EventBus.Subscribe<ScenariosCollectionChangedEvent>(e =>
            {
                base.OnIsEnabledInvalidated();
            });
            EventBus.Subscribe<CurrentUserChangedEvent>(e =>
            {
                base.OnIsEnabledInvalidated();
            });
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate =>
            IsEnabledWhenProjectHasAtLeastOneScenario(ServiceBus);
    }*/

    /*class PublishSummary : NavigationSubMenu<IPublishSummaryViewModel>
    {
        public PublishSummary()
            : base(KnownMenus.Publish, KnownMenus.PublishSummary, "Common_Publish_Summary")
        {
        }

        public override void Initialize()
        {
            EventBus.Subscribe<ScenariosCollectionChangedEvent>(e =>
            {
                base.OnIsEnabledInvalidated();
            });
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate =>
            NavigationMenu.IsEnabledWhenPublicationExists(ServiceBus);

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead =>
            new string[] { KnownRoles.Administrator, KnownRoles.Analyst };

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite =>
            new string[] { KnownRoles.Administrator, KnownRoles.Analyst };

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext => true;
    }

    class PublishScenario : NavigationSubMenu<IPublishScenarioViewModel>
    {
        public PublishScenario()
            : base(KnownMenus.Publish, KnownMenus.PublishScenario, "Common_Publish_Scenario")
        {
        }

        public override void Initialize()
        {
            EventBus.Subscribe<ScenariosCollectionChangedEvent>(e =>
            {
                base.OnIsEnabledInvalidated();
            });
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate =>
            NavigationMenu.IsEnabledWhenProjectHasAtLeastOneScenario(ServiceBus);

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead =>
            new string[] { KnownRoles.Administrator, KnownRoles.Analyst };

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite =>
            new string[] { KnownRoles.Administrator, KnownRoles.Analyst };

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext => true;
    }

    class PublishFormat : NavigationSubMenu<IPublishFormatViewModel>
    {
        public PublishFormat()
            : base(
                KnownMenus.Publish, KnownMenus.PublishFormat,
                "Common_Publish_Format"
                )
        {
        }

        public override void Initialize()
        {
            base.EventBus.Subscribe<ScenariosCollectionChangedEvent>(e =>
            {
                base.OnIsEnabledInvalidated();
            });
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate
        {
            get { return NavigationMenu.IsEnabledWhenCurrentPublicationExistsAndAllLinkedProcessArePublished(ServiceBus); }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return true; }
        }
    }

    class PublishVideos : NavigationSubMenu<IPublishVideosViewModel>
    {
        public PublishVideos()
            : base(
                KnownMenus.Publish, KnownMenus.PublishVideos,
                "Common_Publish_Videos"
                )
        {
        }

        public override void Initialize()
        {
            base.EventBus.Subscribe<ScenariosCollectionChangedEvent>(e =>
            {
                base.OnIsEnabledInvalidated();
            });
        }

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        public override Func<bool> IsEnabledDelegate
        {
            get { return NavigationMenu.IsEnabledWhenCurrentPublicationExistsAndFormatIsSet(ServiceBus); }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return true; }
        }
    }*/

    #endregion

    #region Admin

    class AdminReferentials : NavigationMenu
    {
        public AdminReferentials()
            : base(KnownMenus.AdminReferentials, MenuStrip.Administration, "Menu_Admin_Referentials")
        {
        }
    }

    class AdminReferentialsReferentials : NavigationSubMenu<IAdminReferentialsViewModel>
    {
        public AdminReferentialsReferentials()
            : base(
                KnownMenus.AdminReferentials, KnownMenus.AdminReferentialsReferentials,
                "Menu_Admin_Referentials"
                )
        {
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst, KnownRoles.Contributor }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator, KnownRoles.Analyst }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return false; }
        }
    }

    /*class AdminUsers : NavigationMenu
    {
        public AdminUsers()
            : base(KnownMenus.AdminUsers, MenuStrip.Administration, "Menu_Admin_Users")
        {
        }
    }*/

    /*class AdminUsersUsers : NavigationSubMenu<IApplicationUsersViewModel>
    {
        public AdminUsersUsers()
            : base(
                KnownMenus.AdminUsers, KnownMenus.AdminUsersUsers,
                "Menu_Admin_Users_Users"
                )
        {
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return false; }
        }

        /// <summary>
        /// Obtient la liste des fonctionnalités permettant d'accéder à cet écran.
        /// </summary>
        public override short[] FeaturesCanRead
        {
            get { return new short[] { (short)ActivationFeatures.All }; }
        }

        /// <summary>
        /// Obtient la liste des fonctionnalités permettant d'écrire sur l'écran.
        /// Par défaut, seul "AllFeatures" y a accès.
        /// </summary>
        public override short[] FeaturesCanWrite
        {
            get { return new short[] { (short)ActivationFeatures.All }; }
        }
    }*/

    /*class AdminBackupRestore : NavigationMenu
    {
        public AdminBackupRestore()
            : base(KnownMenus.AdminBackupRestore, MenuStrip.Administration, "Menu_Admin_BackupRestore")
        {
        }
    }*/

    /*class AdminBackupRestoreBackupRestore : NavigationSubMenu<IBackupRestoreViewModel>
    {
        public AdminBackupRestoreBackupRestore()
            : base(
                KnownMenus.AdminBackupRestore, KnownMenus.AdminBackupRestoreBackupRestore,
                "Menu_Admin_BackupRestore_BackupRestore"
                )
        {
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return false; }
        }
    }*/

#if !NO_ACTIVATION
    /*class AdminActivation : NavigationMenu
    {
        public AdminActivation()
            : base(KnownMenus.AdminActivation, MenuStrip.Administration, "Menu_Admin_Activation")
        {
        }
    }*/

    /*class AdminActivationActivation : NavigationSubMenu<IActivationFrameViewModel>
    {
        public AdminActivationActivation()
            : base(
                KnownMenus.AdminActivation, KnownMenus.AdminActivationActivation,
                "Menu_Admin_Activation_Activation"
                )
        {
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return false; }
        }
    }*/
#endif

    #endregion

    #region Ext

    class Extensions : NavigationMenu
    {
        public Extensions()
            : base(KnownMenus.Extensions, MenuStrip.Extensions, "Menu_Extensions")
        {
        }
    }

    class ExtensionsConfiguration : NavigationSubMenu<IExtensionsConfigurationViewModel>
    {
        public ExtensionsConfiguration()
            : base(
                KnownMenus.Extensions, KnownMenus.ExtensionsConfiguration,
                "Menu_Extensions_Configuration"
                )
        {
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent lire l'écran associé.
        /// </summary>
        public override string[] RolesCanRead
        {
            get { return new string[] { KnownRoles.Administrator }; }
        }

        /// <summary>
        /// Obtient ou définit la liste des rôles qui peuvent écrire sur l'écran associé.
        /// </summary>
        public override string[] RolesCanWrite
        {
            get { return new string[] { KnownRoles.Administrator }; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les conditions d'accès au sous menu sont liées au projet.
        /// </summary>
        public override bool IsSecurityProjectContext
        {
            get { return false; }
        }
    }

    #endregion

}
