using KProcess.Ksmed.Business;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace KProcess.Ksmed.Presentation.Tests
{
    /// <summary>
    /// Contient des tests d'accès
    /// </summary>
    [TestClass]
    public class RolesAccessTests
    {

        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem("RolesAccessTestsData.xml")]
        public void NavigationCanReadWrite()
        {
            IMenuDefinition[] menus;
            ISubMenuDefinition[] subMenus;
            var navigationService = new StubNavigationService();
            IServiceBus serviceBus;

            InitializeSecurityContext(navigationService, out menus, out subMenus, out serviceBus);

            var projectManagerService = (StubProjectManagerService)serviceBus.Get<IProjectManagerService>();

            var root = XDocument.Load(Path.Combine(TestContext.DeploymentDirectory, "RolesAccessTestsData.xml")).Root;

            foreach (var testcase in root.Elements("TestCase"))
            {
                var menuCode = testcase.Attribute("Menu").Value;
                var subMenuCode = testcase.Attribute("SubMenu").Value;
                var userRoles = testcase.Element("User").Elements("Role").Select(r => r.Value).ToArray();
                var userProjectRoles = testcase.Element("Project").Elements("Role").Select(r => r.Value).ToArray();
                var expectedIsEnabled = bool.Parse(testcase.Element("IsEnabled").Value);
                var expectedCanRead = bool.Parse(testcase.Element("CanRead").Value);
                var expectedCanWrite = bool.Parse(testcase.Element("CanWrite").Value);
                var hasRealizedLockedScenario = bool.Parse(testcase.Element("Project").Element("HasRealizedLockedScenario").Value);

                var subMenuDefinition = subMenus.First(smd => smd.Code == subMenuCode && smd.ParentCode == menuCode);

                var modelUser = new KProcess.Ksmed.Models.User
                {
                    Username = "Test User",
                };
                modelUser.RoleCodes.AddRange(userRoles);

                Security.SecurityContext.CurrentUser = new Security.User(modelUser);

                navigationService.IsProjectSecurityContext = subMenuDefinition.IsSecurityProjectContext;
                navigationService.ProjectRoles = userProjectRoles;

                projectManagerService.HasRealizedLockedScenario = hasRealizedLockedScenario;

                bool hasTryShowBeenCalled = false;

                Assert.AreEqual(expectedIsEnabled, subMenuDefinition.IsEnabledDelegate());

                navigationService.TryShowCallback = () =>
                {
                    hasTryShowBeenCalled = true;

                    Assert.AreEqual(expectedCanRead, navigationService.CanRead, string.Format("Expected CanRead faux : \r\n{0}", testcase.ToString()));
                    Assert.AreEqual(expectedCanWrite, navigationService.CanWrite, string.Format("Expected CanWrite faux : \r\n{0}", testcase.ToString()));
                };

                subMenuDefinition.Action(serviceBus);

                Assert.IsTrue(hasTryShowBeenCalled);

            }
        }

        private void InitializeSecurityContext(StubNavigationService navigationService, out IMenuDefinition[] menus, out ISubMenuDefinition[] subMenus, out IServiceBus serviceBus)
        {
            // N'importer que les IMenuDefinition et ISubMenuDefinition

            var menuDeftype = typeof(IMenuDefinition);
            var subMenuDeftype = typeof(ISubMenuDefinition);

            var assembly = typeof(KProcess.Ksmed.Presentation.Shell.Controller).Assembly;

            // Rechercher les types qui les implémentent
            var menuTypes = assembly.GetTypes().Where(t => t.GetInterface(menuDeftype.FullName) != null || t.GetInterface(subMenuDeftype.FullName) != null).ToArray();


            // Composer MEF
            var container = new CompositionContainer(
                new AggregateCatalog(
                    new TypeCatalog(menuTypes),
                    new TypeCatalog(
                        typeof(IServiceBus), typeof(ServiceBus),
                        typeof(IEventBus), typeof(EventBus)
                        )));
            CompositionBatch batch = new CompositionBatch();
            batch.AddPart(this);
            batch.AddExportedValue<CompositionContainer>(container);
            //IoC.RegisterInstance<CompositionContainer>(_container);
            container.Compose(batch);

            menus = container.GetExports<IMenuDefinition>().Select(l => l.Value).ToArray();
            subMenus = container.GetExports<ISubMenuDefinition>().Select(l => l.Value).ToArray();

            foreach (var menu in menus)
            {
                menu.Initialize();
            }


            var rolesReadAuthorizations = new Dictionary<Type, string[]>();
            var rolesWriteAuthorizations = new Dictionary<Type, string[]>();
            var featuresReadAuthorizations = new Dictionary<Type, short[]>();
            var featuresWriteAuthorizations = new Dictionary<Type, short[]>();
            var customReadAuthorizations = new Dictionary<Type, Func<string, bool>>();
            var customWriteAuthorizations = new Dictionary<Type, Func<string, bool>>();
            var accessProjectContext = new Dictionary<Type, bool>();

            foreach (var subMenu in subMenus)
            {
                subMenu.Initialize();
                rolesReadAuthorizations[subMenu.ViewModelType] = subMenu.RolesCanRead;
                rolesWriteAuthorizations[subMenu.ViewModelType] = subMenu.RolesCanWrite;
                featuresReadAuthorizations[subMenu.ViewModelType] = subMenu.FeaturesCanRead;
                featuresWriteAuthorizations[subMenu.ViewModelType] = subMenu.FeaturesCanWrite;
                customReadAuthorizations[subMenu.ViewModelType] = subMenu.CustomCanRead;
                customWriteAuthorizations[subMenu.ViewModelType] = subMenu.CustomCanWrite;
                accessProjectContext[subMenu.ViewModelType] = subMenu.IsSecurityProjectContext;
            }

            Security.SecurityContext.RegisterAuthorizations(
                rolesReadAuthorizations,
                rolesWriteAuthorizations,
                featuresReadAuthorizations,
                featuresWriteAuthorizations,
                customReadAuthorizations,
                customWriteAuthorizations);

            serviceBus = container.GetExport<IServiceBus>().Value;
            serviceBus.Register<INavigationService>(navigationService);
            serviceBus.Register<IProjectManagerService>(new StubProjectManagerService());

        }

        private class StubNavigationService : INavigationService
        {
            public bool TryShow<TViewModel>(Action<TViewModel> initialization = null) where TViewModel : IFrameContentViewModel
            {

                Dictionary<string, string[]> projectRoles;
                if (this.ProjectRoles == null)
                    projectRoles = null;
                else
                {
                    projectRoles = new Dictionary<string, string[]>();
                    projectRoles[Security.SecurityContext.CurrentUser.Username] = this.ProjectRoles;
                }

                this.CanRead = KProcess.Ksmed.Presentation.Shell.AccessRules.CanUserRead<TViewModel>(projectRoles, IsProjectSecurityContext);
                this.CanWrite = KProcess.Ksmed.Presentation.Shell.AccessRules.CanUserWrite<TViewModel>(projectRoles, IsProjectSecurityContext);

                TryShowCallback();
                return true;
            }

            public bool IsProjectSecurityContext { get; set; }
            public string[] ProjectRoles { get; set; }

            public bool CanRead { get; set; }
            public bool CanWrite { get; set; }
            public Action TryShowCallback { get; set; }

            public bool TryNavigate(string menuCode)
            {
                throw new NotImplementedException();
            }

            public bool TryNavigate(string menuCode, string subMenuCode)
            {
                throw new NotImplementedException();
            }

            public NavigationSharedPreferences Preferences
            {
                get { throw new NotImplementedException(); }
            }
        }


        private class StubProjectManagerService : IProjectManagerService
        {
            public ProjectInfo CurrentProject
            {
                get { throw new NotImplementedException(); }
            }

            public void SetCurrentProject(Models.Project p)
            {
                throw new NotImplementedException();
            }

            public System.Collections.ObjectModel.ReadOnlyObservableCollection<Models.ScenarioDescription> Scenarios { get; private set; }

            public bool HasRealizedLockedScenario
            {
                set
                {
                    this.Scenarios = new System.Collections.ObjectModel.ReadOnlyObservableCollection<Models.ScenarioDescription>(
                        new System.Collections.ObjectModel.ObservableCollection<Models.ScenarioDescription>()
                        {
                            new Models.ScenarioDescription(new Models.Scenario
                            {
                                NatureCode = KnownScenarioNatures.Realized,
                            })
                            {
                                IsLocked = value,
                            },
                        });
                }
            }

            public Models.ScenarioDescription CurrentScenario
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public void SyncScenarios(IEnumerable<Models.Scenario> scenarios)
            {
                throw new NotImplementedException();
            }

            public void SyncScenarios(IEnumerable<Models.ScenarioDescription> scenarios)
            {
                throw new NotImplementedException();
            }

            public void RemoveScenario(int scenarioId)
            {
                throw new NotImplementedException();
            }

            public void SelectScenario(Models.Scenario scenario)
            {
                throw new NotImplementedException();
            }

            public void UpdateScenarioDescription(Models.Scenario scenario)
            {
                throw new NotImplementedException();
            }

            public void HideScenariosPicker()
            {
                throw new NotImplementedException();
            }

            public void ShowScenariosPicker()
            {
                throw new NotImplementedException();
            }

            public bool IsScenarioPickerEnabled
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public void FilterScenarioNatures(string[] natureCodes)
            {
                throw new NotImplementedException();
            }


            public IDictionary<int, RestitutionState> RestitutionState
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }


            public bool IsUnlinkMarkerEnabledAndLocked
            {
                get { throw new NotImplementedException(); }
            }


            public void SynchronizeProjectObjectivesInfo(Models.Project project)
            {
                throw new NotImplementedException();
            }
        }



    }
}
