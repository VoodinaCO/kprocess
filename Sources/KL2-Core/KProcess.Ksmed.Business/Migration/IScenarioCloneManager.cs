using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Business.Migration
{
    public enum ActionCloneBehavior
    {
        InitialToTarget,
        TargetToTarget,
        TargetToRealized,
        Cascade,
        RealizedToNewInitial,
        TargetToNewInitial,
        InitialToNewInitial
    }

    public interface IScenarioCloneManager
    {
        Task<Scenario> CreateDerivatedScenario(KsmedEntities context, int projectId, int sourceScenarioId, string natureCode, bool save);
        Task<Scenario> CreateDerivatedScenario(KsmedEntities tempContext, Scenario sourceScenario, string natureCode, bool save);
        Task<string[]> EnsureCanShowScenarioInSummary(Scenario scenario, bool updateValue);
        KAction CloneAction(KAction action, ActionCloneBehavior cloneBehavior);
    }
}
