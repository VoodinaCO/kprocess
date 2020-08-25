using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed
{
    /// <summary>
    /// Helper pour les services du service Bus permettant de créer des Mocks
    /// </summary>
    public static class ServicesHelper
    {

        public static IServiceBus RegisterMockServices()
        {
            var serviceBus = new ServiceBus();
            IoC.RegisterInstance<IServiceBus>(serviceBus);

            var ps = IProjectManagerService();
            ps.SetCurrentProject(new KProcess.Ksmed.Models.Project
            {
                ProjectId = 1,
                Label = "Project",
                TimeScale = TimeSpan.FromSeconds(1).Ticks,
            });

            ITimeTicksFormatService();
            IReferentialsUseService();
            IVideoColorService();

            return serviceBus;
        }

        private static ProjectManagerService IProjectManagerService()
        {
            var projectManager = new ProjectManagerService();
            IoC.Resolve<IServiceBus>().Register<IProjectManagerService>(projectManager);
            return projectManager;
        }

        private class ProjectManagerService : IProjectManagerService
        {
            public ProjectInfo CurrentProject { get; set; }

            public void SetCurrentProject(Models.Project p)
            {
                this.CurrentProject = new ProjectInfo(p);
            }

            public System.Collections.ObjectModel.ReadOnlyObservableCollection<Models.ScenarioDescription> Scenarios
            {
                get { throw new NotImplementedException(); }
            }

            public Models.ScenarioDescription CurrentScenario { get; set; }

            public void SyncScenarios(System.Collections.Generic.IEnumerable<Models.Scenario> scenarios)
            {
                throw new NotImplementedException();
            }

            public void SyncScenarios(System.Collections.Generic.IEnumerable<Models.ScenarioDescription> scenarios)
            {
                throw new NotImplementedException();
            }

            public void RemoveScenario(int scenarioId)
            {
                throw new NotImplementedException();
            }

            public void SelectScenario(Models.Scenario scenario)
            {
                this.CurrentScenario = new Models.ScenarioDescription(scenario);
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

        private static ITimeTicksFormatService ITimeTicksFormatService()
        {
            var service = new TimeTicksFormatService();
            IoC.Resolve<IServiceBus>().Register<ITimeTicksFormatService>(service);
            return service;
        }

        private static IReferentialsUseService IReferentialsUseService()
        {
            var service = new KProcess.Ksmed.Presentation.Shell.ReferentialsUseService();
            var refes = KProcess.Ksmed.Business.Impl.PrepareService.CreateDefaultProjectReferentials();
            service.UpdateProjectReferentials(refes);
            service.UpdateReferentials(new Models.Referential[]
            {
                new Models.Referential { ReferentialId = (int)Models.ProcessReferentialIdentifier.Operator, Label = "Operator" },
                new Models.Referential { ReferentialId = (int)Models.ProcessReferentialIdentifier.Equipment, Label = "Equipment" },
                new Models.Referential { ReferentialId = (int)Models.ProcessReferentialIdentifier.Category, Label = "Category" },
                new Models.Referential { ReferentialId = (int)Models.ProcessReferentialIdentifier.Ref1, Label = "Ref1" },
                new Models.Referential { ReferentialId = (int)Models.ProcessReferentialIdentifier.Ref2, Label = "Ref2" },
                new Models.Referential { ReferentialId = (int)Models.ProcessReferentialIdentifier.Ref3, Label = "Ref3" },
                new Models.Referential { ReferentialId = (int)Models.ProcessReferentialIdentifier.Ref4, Label = "Ref4" },
                new Models.Referential { ReferentialId = (int)Models.ProcessReferentialIdentifier.Ref5, Label = "Ref5" },
                new Models.Referential { ReferentialId = (int)Models.ProcessReferentialIdentifier.Ref6, Label = "Ref6" },
                new Models.Referential { ReferentialId = (int)Models.ProcessReferentialIdentifier.Ref7, Label = "Ref7" },
            });
            IoC.RegisterInstance<IReferentialsUseService>(service);
            return service;
        }

        private static IVideoColorService IVideoColorService()
        {
            var service = new VideoColorService();
            IoC.Resolve<IServiceBus>().Register<IVideoColorService>(service);
            return service;
        }

        private class VideoColorService : IVideoColorService
        {
            #region IVideoColorService Members

            public System.Windows.Media.Brush GetColor(Models.Video video)
            {
                return System.Windows.Media.Brushes.Black;
            }

            #endregion
        }


    }
}
