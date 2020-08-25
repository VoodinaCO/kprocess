using KProcess.Business;
using KProcess.KL2.Business.Impl.Shared;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Business.Migration;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Desktop
{
    /// <summary>
    /// Représente un service de gestion de la partie fonctionnelle "Analyser".
    /// </summary>
    public class AnalyzeService : IBusinessService, IAnalyzeService
    {
        private readonly ITraceManager _traceManager;
        private readonly ISecurityContext _securityContext;
        private readonly ILocalizationManager _localizationManager;
        private readonly ISharedScenarioActionsOperations _sharedScenarioActionsOperations;

        public AnalyzeService(
            ISecurityContext securityContext,
            ILocalizationManager localizationManager,
            ITraceManager traceManager,
            ISharedScenarioActionsOperations sharedScenarioActionsOperations)
        {
            _securityContext = securityContext;
            _localizationManager = localizationManager;
            _traceManager = traceManager;
            _sharedScenarioActionsOperations = sharedScenarioActionsOperations;
        }




        /// <summary>
        /// Prédit les scénarios qui seront impactés par les modifications en attente de sauvegarde.
        /// </summary>
        /// <param name="sourceModifiedScenario">Le scenério source modifié.</param>
        /// <param name="allScenarios">Tous les scénarios.</param>
        /// <param name="actionsToDelete">Les actions à supprimer.</param>
        public virtual async Task<Scenario[]> PredictImpactedScenarios(Scenario sourceModifiedScenario, Scenario[] allScenarios, KAction[] actionsToDelete, KAction[] actionsWithUpdatedWBS) =>
            await Task.Run(() =>
            {
                return ActionsRecursiveUpdate.PredictImpactedScenarios(sourceModifiedScenario, allScenarios, actionsToDelete, actionsWithUpdatedWBS);
            });

        /// <summary>
        /// Obtient les données pour l'écran Acquérir.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<AcquireData> GetAcquireData(int projectId, bool getSyncedVideos = true) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await _sharedScenarioActionsOperations.GetAcquireData(context, projectId, GetDataScenarioNatures.InitialAndTarget);
                }
            });

        /// <summary>
        /// Sauvegarde les actions spécifiées.
        /// </summary>
        /// <param name="allScenarios">Tous les scénarios liés.</param>
        /// <param name="updatedScenario">Le scénario qui a été mis à jour.</param>
        public virtual async Task<Scenario> SaveAcquireData(Scenario[] allScenarios, Scenario updatedScenario)
        {
            return await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await _sharedScenarioActionsOperations.SaveAcquireData(context, allScenarios, updatedScenario, true);
                }
            });
        }


        /// <summary>
        /// Obtient les données pour l'écran Construire.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<BuildData> GetBuildData(int projectId, bool getSyncedVideos = true) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await _sharedScenarioActionsOperations.GetBuildData(context, projectId, GetDataScenarioNatures.InitialAndTarget);
                }
            });

        /// <summary>
        /// Sauvegarde le scénario spécifié.
        /// </summary>
        /// <param name="allScenarios">Tous les scénarios liés.</param>
        /// <param name="updatedScenario">Le scénario qui a été mis à jour.</param>
        public virtual async Task<Scenario> SaveBuildScenario(Scenario[] allScenarios, Scenario updatedScenario)
        {
            return await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    await _sharedScenarioActionsOperations.SaveBuildScenario(context, allScenarios, updatedScenario, true);
                }
                return updatedScenario;
            });
        }
     

        /// <summary>
        /// Obtient les données pour l'écran Simuler.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<SimulateData> GetSimulateData(int projectId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await _sharedScenarioActionsOperations.GetSimulateData(context, projectId, GetDataScenarioNatures.InitialAndTarget);
                }
            });



        /// <summary>
        /// Obtient les données pour l'écran Restituer.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<RestitutionData> GetRestitutionData(int projectId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await _sharedScenarioActionsOperations.GetRestitutionData(context, projectId);
                }
            });

        /// <summary>
        /// Obtient toutes les données du projet spécifié.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<RestitutionData> GetFullProjectDetails(int projectId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    IDictionary<ProcessReferentialIdentifier, bool> referentialsUsed = await _sharedScenarioActionsOperations.GetReferentialsUse(context, projectId);
                    Referentials referentials = await Queries.LoadAllReferentialsOfProject(context, projectId, referentialsUsed);

                    //await context.Videos.Where(v => v.ProjectId == projectId).ToArrayAsync();
                    await context.ScenarioNatures.ToArrayAsync();
                    await context.ScenarioStates.ToArrayAsync();
                    await context.ActionTypes.ToArrayAsync();
                    await context.ActionValues.ToArrayAsync();

                    Project project = await context.Projects
                        .Include(nameof(Project.Process))
                        .Include($"{nameof(Project.Process)}.{nameof(Procedure.Videos)}")
                        .Include($"{nameof(Project.Process)}.{nameof(Procedure.UserRoleProcesses)}")
                        .Include($"{nameof(Project.Process)}.{nameof(Procedure.UserRoleProcesses)}.{nameof(UserRoleProcess.User)}")
                        .Include($"{nameof(Project.Process)}.{nameof(Procedure.UserRoleProcesses)}.{nameof(UserRoleProcess.User)}.{nameof(User.DefaultLanguage)}")
                        .Include(nameof(Project.Scenarios))
                        .Include($"{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}")
                        .Include(nameof(Project.Objective))
                        .FirstAsync(s => s.ProjectId == projectId);

                    project.ScenariosCriticalPath = PrepareService.GetSummary(project, true);

                    // Scénarios
                    foreach (Scenario scenario in project.Scenarios.Where(s => s.OriginalScenarioId.HasValue))
                    {
                        // Remapper l'original
                        scenario.Original = project.Scenarios.Single(s => s.ScenarioId == scenario.OriginalScenarioId);

                        ScenarioCriticalPath matchingCriticalItem = project.ScenariosCriticalPath.FirstOrDefault(i => i.Id == scenario.ScenarioId);
                        if (matchingCriticalItem != null)
                            matchingCriticalItem.OriginalLabel = scenario.Original.Label;
                    }

                    ProjectReferential[] projectReferentials = await context.ProjectReferentials.Where(pr => pr.ProjectId == projectId).ToArrayAsync();

                    User user = await context.Users.FirstAsync(u => u.UserId == project.CreatedByUserId);

                    ModificationsUsers modificationsUsers = new ModificationsUsers
                    {
                        CreatedByFullName = (await context.Users.FirstAsync(u => u.UserId == project.ModifiedByUserId)).FullName,
                        LastModifiedByFullName = (await context.Users.FirstAsync(u => u.UserId == project.ModifiedByUserId)).FullName
                    };

                    Scenario[] scenarios = await context.Scenarios
                        .Where(s => s.ProjectId == projectId)
                        .ToArrayAsync();

                    await Queries.LoadScenariosDetails(context, scenarios, referentialsUsed);

                    ILookup<int, KAction> actionsToLoad = scenarios
                        .SelectMany(a => a.Actions)
                        .Where(a => a.IsReduced && a.OriginalActionId.HasValue)
                        .ToLookup(a => a.OriginalActionId.Value, a => a);

                    if (actionsToLoad.Any())
                        foreach (var duration in await _sharedScenarioActionsOperations.GetActionsBuildDurations(context, actionsToLoad.Select(g => g.Key)))
                            foreach (KAction action in actionsToLoad[duration.ActionId])
                                action.Reduced.Saving = duration.BuildDuration - action.BuildDuration;

                    ScenarioActionHierarchyHelper.MapScenariosActionsOriginals(scenarios);

                    return new RestitutionData()
                    {
                        Project = project,
                        ProjectCreatedByUser = user,
                        Scenarios = scenarios,
                        ActionCategories = referentials.Categories,
                        ModificationsUsers = modificationsUsers,
                        ReferentialsUse = projectReferentials,
                    };
                }
            });

        /// <summary>
        /// Met à jour l'affichage dans la synthèse pour le scénario spécifié.
        /// </summary>
        /// <param name="scenarioId">L'identifiant du scénario.</param>
        /// <param name="isShownInSummary"><c>true</c> pour l'afficher dans le scénario.</param>
        public virtual Task<Scenario> UpdateScenarioIsShownInSummary(int scenarioId, bool isShownInSummary) =>
            Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    var scenario = await context.Scenarios.FirstAsync(sc => sc.ScenarioId == scenarioId);
                    scenario.IsShownInSummary = isShownInSummary;
                    await context.SaveChangesAsync();
                    return scenario;
                }
            });

    }
}