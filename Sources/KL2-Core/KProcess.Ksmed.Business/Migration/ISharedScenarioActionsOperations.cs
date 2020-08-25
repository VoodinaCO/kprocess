using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Business.Migration
{

    public enum GetDataScenarioNatures
    {
        InitialAndTarget,
        Realized,
        All
    }

    /// <summary>
    /// Représente la durée process d'une action.
    /// </summary>
    public class ActionDuration
    {
        public int ActionId { get; set; }
        public long BuildDuration { get; set; }
    }


    public interface ISharedScenarioActionsOperations
    {
        Task<AcquireData> GetAcquireData(KsmedEntities context, int projectId, GetDataScenarioNatures natureFilter);
        Task<Scenario> SaveAcquireData(KsmedEntities context, Scenario[] allScenarios, Scenario updatedScenario, bool recursive);
        Task<BuildData> GetBuildData(KsmedEntities context, int projectId, GetDataScenarioNatures natureFilter);
        Task SaveBuildScenario(KsmedEntities context, Scenario[] allScenarios, Scenario updatedScenario, bool recursive);
        Task<SimulateData> GetSimulateData(KsmedEntities context, int projectId, GetDataScenarioNatures natureFilter);
        Task<RestitutionData> GetRestitutionData(KsmedEntities context, int projectId);
        void ApplyNewReduced(KAction action, string actionTypeCode = null);
        void ApplyReduced(KAction originalAction, KAction newAction);
        void EnsureEmptySolutionExists(Scenario scenario);
        void UdpateSolutionsApprovedState(Scenario scenario);
        Task<IDictionary<ProcessReferentialIdentifier, bool>> GetReferentialsUse(KsmedEntities context, int projectId);
        Task<ActionDuration[]> GetActionsBuildDurations(KsmedEntities context, IEnumerable<int> ids);

    }
}
