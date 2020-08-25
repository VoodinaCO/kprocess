using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Business.Dtos.Prepare;
using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Business.Tests.Helper
{
    public static class AsyncServiceOperationExtensions
    {
        #region IPrepareService

        public static ProjectsData GetProjects(this IPrepareService service)
        {
            var operation = new AsyncServiceOperation<ProjectsData>();

            service.GetProjects(operation.OnDone, operation.OnError);

            return operation.WaitCompletion();
        }

        public static void SaveProject(this IPrepareService service, Project project)
        {
            var operation = new AsyncServiceOperation();

            service.SaveProject(project, operation.OnDone, operation.OnError);

            operation.WaitCompletion();
        }

        public static Scenario CreateInitialScenario(this IPrepareService service, int projectId)
        {
            var operation = new AsyncServiceOperation<Scenario>();

            service.CreateInitialScenario(projectId, operation.OnDone, operation.OnError);

            return operation.WaitCompletion();
        }

        public static Scenario CreateScenario(this IPrepareService service, int projectId, Scenario sourceScenario)
        {
            var operation = new AsyncServiceOperation<Scenario>();

            service.CreateScenario(projectId, sourceScenario, true, operation.OnDone, operation.OnError);

            return operation.WaitCompletion();
        }

        public static ScenariosData SaveScenario(this IPrepareService service, int projectId, Scenario scenario)
        {
            var operation = new AsyncServiceOperation<ScenariosData>();

            service.SaveScenario(projectId, scenario, operation.OnDone, operation.OnError);

            return operation.WaitCompletion();
        }

        #endregion

        #region AnalyzeService

        public static void SaveAcquireData(this IAnalyzeService service, Scenario[] allScenarios, Scenario updatedScenario)
        {
            var operation = new AsyncServiceOperation();

            service.SaveAcquireData(allScenarios, updatedScenario, operation.OnDone, operation.OnError);

            operation.WaitCompletion();
        }

        public static AcquireData GetAcquireData(this IAnalyzeService service, int projectId)
        {
            var operation = new AsyncServiceOperation<AcquireData>();

            service.GetAcquireData(projectId, operation.OnDone, operation.OnError);

            return operation.WaitCompletion();
        }

        public static RestitutionData GetFullProjectDetails(this IAnalyzeService service, int projectId)
        {
            var operation = new AsyncServiceOperation<RestitutionData>();

            service.GetFullProjectDetails(projectId, operation.OnDone, operation.OnError);

            return operation.WaitCompletion();
        }

        #endregion

        #region ReferentialsService

        public static void MergeReferentials(this IReferentialsService service, IActionReferential master, IActionReferential[] slaves)
        {
            var operation = new AsyncServiceOperation();

            service.MergeReferentials(master, slaves, operation.OnDone, operation.OnError);

            operation.WaitCompletion();
        }

        public static void SaveReferentials<TReferential>(this IReferentialsService service, IEnumerable<TReferential> referentials)
            where TReferential : class, IObjectWithChangeTracker
        {
            var operation = new AsyncServiceOperation();

            service.SaveReferentials(referentials, operation.OnDone, operation.OnError);

            operation.WaitCompletion();
        }

        #endregion

    }
}
