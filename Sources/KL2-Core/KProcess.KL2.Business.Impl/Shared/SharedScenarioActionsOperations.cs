using KProcess.Data;
using KProcess.Globalization;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Business.Migration;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Shared
{
    /// <summary>
    /// Fournit des méthodes partagées pour les parties "Analyser" et "Valider" (éventuellement à venir "Suivre")
    /// </summary>
    internal static class SharedScenarioActionsOperations
    {

        /// <summary>
        /// Obtient les données pour l'écran Acquérir.
        /// </summary>
        /// <param name="context">Le contexte EF.</param>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="natureFilter">Le filtre sur les codes de nature de scénario.</param>
        /// <returns>
        /// Les données
        /// </returns>
        public static async Task<AcquireData> GetAcquireData(KsmedEntities context, int projectId, GetDataScenarioNatures natureFilter)
        {
            IDictionary<ProcessReferentialIdentifier, bool> referentialsUsed = await GetReferentialsUse(context, projectId);
            int processId = (await context.Projects.SingleAsync(p => p.ProjectId == projectId)).ProcessId;

            AcquireData data = new AcquireData
            {
                Categories = await Queries.FilterReferentials(context.ActionCategories, processId, ProcessReferentialIdentifier.Category).ToArrayAsync(),
                Skills = await Queries.FilterReferentials(context.Skills, processId, ProcessReferentialIdentifier.Skill).ToArrayAsync(),
                Resources = await Queries.FilterResources(context, processId).ToArrayAsync(),
                Videos = await context.Projects
                    .Include(nameof(Project.Process))
                    .Include($"{nameof(Project.Process)}.{nameof(Procedure.Videos)}")
                    .Where(p => p.ProjectId == projectId)
                    .SelectMany(p => p.Process.Videos)
                    .OrderBy(v => v.CameraName).OrderBy(v => v.DefaultResourceId).OrderBy(v => v.ResourceView).OrderBy(v => v.ShootingDate)
                    .ToArrayAsync()
            };

            if (referentialsUsed[ProcessReferentialIdentifier.Ref1])
                data.Ref1s = await Queries.FilterReferentials(context.Refs1, processId, ProcessReferentialIdentifier.Ref1).ToArrayAsync();

            if (referentialsUsed[ProcessReferentialIdentifier.Ref2])
                data.Ref2s = await Queries.FilterReferentials(context.Refs2, processId, ProcessReferentialIdentifier.Ref2).ToArrayAsync();

            if (referentialsUsed[ProcessReferentialIdentifier.Ref3])
                data.Ref3s = await Queries.FilterReferentials(context.Refs3, processId, ProcessReferentialIdentifier.Ref3).ToArrayAsync();

            if (referentialsUsed[ProcessReferentialIdentifier.Ref4])
                data.Ref4s = await Queries.FilterReferentials(context.Refs4, processId, ProcessReferentialIdentifier.Ref4).ToArrayAsync();

            if (referentialsUsed[ProcessReferentialIdentifier.Ref5])
                data.Ref5s = await Queries.FilterReferentials(context.Refs5, processId, ProcessReferentialIdentifier.Ref5).ToArrayAsync();

            if (referentialsUsed[ProcessReferentialIdentifier.Ref6])
                data.Ref6s = await Queries.FilterReferentials(context.Refs6, processId, ProcessReferentialIdentifier.Ref6).ToArrayAsync();

            if (referentialsUsed[ProcessReferentialIdentifier.Ref7])
                data.Ref7s = await Queries.FilterReferentials(context.Refs7, processId, ProcessReferentialIdentifier.Ref7).ToArrayAsync();

            Project project = await context.Projects.Include(nameof(Project.Process)).SingleAsync(p => p.ProjectId == projectId);
            data.CustomFieldsLabels = GetCustomFieldsLabels(project);

            Scenario[] scenarios = null;
            Scenario[] scenariosUsedForMapping = null;

            if (natureFilter == GetDataScenarioNatures.InitialAndTarget)
            {
                // Il y a tout ce qu'il faut pour faire le mapping dans les scenarii chargés
                scenariosUsedForMapping = await LoadScenarios(context, projectId, referentialsUsed, GetDataScenarioNatures.InitialAndTarget);
                scenarios = scenariosUsedForMapping;
            }
            else if (natureFilter == GetDataScenarioNatures.Realized)
            {
                // Il faut charger au moins les scenarii cibles également
                scenariosUsedForMapping = await LoadScenarios(context, projectId, referentialsUsed, GetDataScenarioNatures.All);
                scenarios = scenariosUsedForMapping.Where(scenario => scenario.NatureCode == KnownScenarioNatures.Realized).ToArray();
            }
            else
            {
                // Le cas all à priori, on considère de toute façon qu'il s'agit du cas par défaut
                scenariosUsedForMapping = await LoadScenarios(context, projectId, referentialsUsed, natureFilter);
                scenarios = scenariosUsedForMapping;
            }

            ScenarioActionHierarchyHelper.MapScenariosActionsOriginals(scenariosUsedForMapping);

            data.Scenarios = scenarios;

            return data;
        }

        /// <summary>
        /// Sauvegarde les actions spécifiées.
        /// </summary>
        /// <param name="context">Le contexte.</param>
        /// <param name="allScenarios">Tous les scénarios liés.</param>
        /// <param name="updatedScenario">Le scénario qui a été mis à jour.</param>
        /// <param name="recursive"><c>true</c> pour appliquer les changements récursivement sur les scénarios dérivés.</param>
        public static Task SaveAcquireData(KsmedEntities context, Scenario[] allScenarios, Scenario updatedScenario, bool recursive)
        {
            KAction[] actionsToDelete;
            IList<KAction> actionsWithOriginal;

            if (recursive)
                ActionsRecursiveUpdate.UpdateActions(context, updatedScenario, allScenarios, out actionsToDelete, out actionsWithOriginal);
            else
            {
                actionsWithOriginal = null;
                actionsToDelete = updatedScenario.Actions.Where(a => a.IsMarkedAsDeleted).ToArray();
                updatedScenario.CriticalPathIDuration = ActionsTimingsMoveManagement.GetInternalCriticalPathDuration(updatedScenario);
            }

            // Consolider les solutions vides
            EnsureEmptySolutionExists(updatedScenario);
            UdpateSolutionsApprovedState(updatedScenario);

            KAction[] allActions = allScenarios
                .SelectMany(s => s.Actions)
                .Where(a => a.IsNotMarkedAsUnchanged)
                .ToArray();

            foreach (KAction action in allActions)
            {
                if (!action.IsReduced)
                    ApplyNewReduced(action, KnownActionCategoryTypes.I);
                context.KActions.ApplyChanges(action);
                context.KActionsReduced.ApplyChanges(action.Reduced);
            }

            if (actionsWithOriginal != null)
                foreach (KAction action in actionsWithOriginal)
                    SetActionsOriginalReference(context, action);

            foreach (Scenario scenario in allScenarios.Where(s => s.IsNotMarkedAsUnchanged))
                context.Scenarios.ApplyChanges(scenario);

            context.Scenarios.ApplyChanges(updatedScenario);

            foreach (KAction action in actionsToDelete)
            {
                action.Predecessors.Clear();
                action.Successors.Clear();
                action.MarkAsDeleted();

                // Ne pas appeler ApplyChanges car les self tracking le gèrent mal (plantage lors de la sauvegarde)
                // Ajouter l'action au contexte si elle n'y est pas attachée
                if (!context.ObjectStateManager.TryGetObjectStateEntry(action, out ObjectStateEntry entry))
                {
                    context.AddObject(KsmedEntities.KActionsEntitySetName, action);
                    context.ObjectStateManager.ChangeObjectState(action, EntityState.Deleted);
                }
                else
                    context.KActions.DeleteObject(action);
            }

            // Vérifier que tout est correct
            if (recursive)
                ActionsTimingsMoveManagement.DebugCheckAllWBS(allScenarios.Where(s => s.IsNotMarkedAsUnchanged));

            return context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtient les données pour l'écran Construire.
        /// </summary>
        /// <param name="context">Le contexte EF.</param>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="natureFilter">Le filtre sur les codes de nature de scénario.</param>
        /// <returns>Les données</returns>
        public static async Task<BuildData> GetBuildData(KsmedEntities context, int projectId, GetDataScenarioNatures natureFilter)
        {
            IDictionary<ProcessReferentialIdentifier, bool> referentialsUsed = await GetReferentialsUse(context, projectId);

            Referentials referentials = await Queries.LoadAllReferentialsOfProject(context, projectId, referentialsUsed);

            BuildData data = new BuildData
            {
                Categories = referentials.Categories,
                Skills = referentials.Skills,
                Resources = referentials.Resources,
                Videos = await context.Projects
                    .Include(nameof(Project.Process))
                    .Include($"{nameof(Project.Process)}.{nameof(Procedure.Videos)}")
                    .Where(p => p.ProjectId == projectId)
                    .SelectMany(p => p.Process.Videos)
                    .ToArrayAsync(),
                ActionTypes = (await context.ActionTypes.ToArrayAsync()).OrderBy(a => a.ActionTypeCode, new KnownActionCategoryTypes.ActionCategoryTypeDefaultOrderComparer()).ToArray()
            };

            Project project = await context.Projects.SingleAsync(p => p.ProjectId == projectId);
            data.CustomFieldsLabels = GetCustomFieldsLabels(project);

            Scenario[] scenarios = await LoadScenarios(context, projectId, referentialsUsed, natureFilter);

            ScenarioActionHierarchyHelper.MapScenariosActionsOriginals(scenarios);

            data.Scenarios = scenarios;

            return data;
        }

        /// <summary>
        /// Sauvegarde le scénario spécifié.
        /// </summary>
        /// <param name="context">Le contexte EF.</param>
        /// <param name="allScenarios">Tous les scénarios liés.</param>
        /// <param name="recursive"><c>true</c> pour appliquer les changements récursivement sur les scénarios dérivés.</param>
        public static Task SaveBuildScenario(KsmedEntities context, Scenario[] allScenarios, Scenario updatedScenario, bool recursive)
        {
            // Consolider les solutions
            string[] distinctSolutionsLabels = updatedScenario.Actions
                .Where(a => a.IsReduced && !string.IsNullOrWhiteSpace(a.Reduced.Solution))
                .Select(a => a.Reduced.Solution)
                .Distinct()
                .ToArray();

            // Ajouter les nouvelles solutions
            foreach (string solutionLabel in distinctSolutionsLabels)
            {
                if (!updatedScenario.Solutions.Any(s => s.SolutionDescription == solutionLabel))
                {
                    // Créer une nouvelle solution
                    Solution solution = new Solution()
                    {
                        SolutionDescription = solutionLabel,
                    };
                    updatedScenario.Solutions.Add(solution);
                }
            }

            EnsureEmptySolutionExists(updatedScenario);

            // Supprimer les anciennes solutions
            Solution[] allSolutions = updatedScenario.Solutions.Where(s => !s.IsEmpty).ToArray();
            foreach (Solution sol in allSolutions)
            {
                if (!distinctSolutionsLabels.Contains(sol.SolutionDescription))
                {
                    sol.MarkAsDeleted();
                    updatedScenario.Solutions.Remove(sol);
                }
            }

            // Copier le temps original
            foreach (KActionReduced reduced in updatedScenario.Actions
                .Where(a => a.IsReduced)
                .Select(a => a.Reduced)
                .Where(r => r.OriginalBuildDuration == default(long)))
                reduced.OriginalBuildDuration = reduced.Action.BuildDuration;

            // Appliquer l'état Approved
            UdpateSolutionsApprovedState(updatedScenario);

            ActionsRecursiveUpdate.UpdateActions(context, updatedScenario, allScenarios, out KAction[] actionsToDelete, out IList<KAction> actionsWithOriginal);

            KAction[] allActions = allScenarios
                .SelectMany(s => s.Actions)
                .Where(a => a.IsNotMarkedAsUnchanged || (a.IsReduced && a.Reduced.IsNotMarkedAsUnchanged))
                .ToArray();

            foreach (KAction action in allActions)
            {
                context.KActions.ApplyChanges(action);
                if (action.IsReduced)
                    context.KActionsReduced.ApplyChanges(action.Reduced);
            }

            foreach (KAction action in actionsWithOriginal)
                SetActionsOriginalReference(context, action);


            foreach (Scenario scenario in allScenarios.Where(s => s.IsNotMarkedAsUnchanged))
                context.Scenarios.ApplyChanges(scenario);

            foreach (Solution solution in updatedScenario.Solutions)
                context.Solutions.ApplyChanges(solution);

            // Vérifier que tout est correct
            ActionsTimingsMoveManagement.DebugCheckAllWBS(allScenarios.Where(s => s.IsNotMarkedAsUnchanged));

            return context.SaveChangesAsync();
        }


        /// <summary>
        /// Obtient les données pour l'écran Simuler.
        /// </summary>
        /// <param name="context">le contexte EF.</param>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="natureFilter">Le filtre des codes de nature.</param>
        /// <returns>
        /// Les données
        /// </returns>
        public static async Task<SimulateData> GetSimulateData(KsmedEntities context, int projectId, GetDataScenarioNatures natureFilter)
        {
            IDictionary<ProcessReferentialIdentifier, bool> referentialsUsed = await GetReferentialsUse(context, projectId);
            await Queries.LoadAllReferentialsOfProject(context, projectId, referentialsUsed);

            await context.Projects
                    .Include(nameof(Project.Process))
                    .Include($"{nameof(Project.Process)}.{nameof(Procedure.Videos)}")
                    .Where(p => p.ProjectId == projectId)
                    .SelectMany(p => p.Process.Videos)
                    .ToArrayAsync();

            Scenario[] scenarios = await LoadScenarios(context, projectId, referentialsUsed, natureFilter);

            Project project = await context.Projects.SingleAsync(p => p.ProjectId == projectId);

            ScenarioActionHierarchyHelper.MapScenariosActionsOriginals(scenarios);

            SimulateData data = new SimulateData()
            {
                Scenarios = scenarios,
                ActionTypes = await context.ActionTypes.ToArrayAsync(),
                CustomFieldsLabels = GetCustomFieldsLabels(project),
            };

            return data;
        }


        /// <summary>
        /// Obtient les données pour l'écran Restituer.
        /// </summary>
        /// <param name="context">le contexte EF.</param>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <returns>
        /// Les données
        /// </returns>
        public static async Task<RestitutionData> GetRestitutionData(KsmedEntities context, int projectId)
        {
            IDictionary<ProcessReferentialIdentifier, bool> referentialsUsed = await GetReferentialsUse(context, projectId);
            Referentials referentials = await Queries.LoadAllReferentialsOfProject(context, projectId, referentialsUsed);
            ActionCategory[] categories = referentials.Categories;
            await context.Projects
                    .Include(nameof(Project.Process))
                    .Include($"{nameof(Project.Process)}.{nameof(Procedure.Videos)}")
                    .Where(p => p.ProjectId == projectId)
                    .SelectMany(p => p.Process.Videos)
                    .ToArrayAsync();

            Scenario[] scenarios = await context.Scenarios
                .Where(s => s.ProjectId == projectId)
                .ToArrayAsync();

            await Queries.LoadScenariosDetails(context, scenarios, referentialsUsed);

            ILookup<int, KAction> actionsToLoad = scenarios
                .SelectMany(a => a.Actions)
                .Where(a => a.IsReduced && a.OriginalActionId.HasValue)
                .ToLookup(a => a.OriginalActionId.Value, a => a);

            if (actionsToLoad.Any())
                foreach (ActionDuration duration in await GetActionsBuildDurations(context, actionsToLoad.Select(g => g.Key)))
                    foreach (KAction action in actionsToLoad[duration.ActionId])
                        action.Reduced.Saving = duration.BuildDuration - action.BuildDuration;

            ScenarioActionHierarchyHelper.MapScenariosActionsOriginals(scenarios);
            foreach (Scenario scenario in scenarios)
                UpdateIsGroup(scenario);

            RestitutionData data = new RestitutionData()
            {
                Scenarios = scenarios,
                ActionCategories = categories,
            };

            return data;
        }

        /// <summary>
        /// Charge les scénarios.
        /// </summary>
        /// <param name="context">Le contexte EF.</param>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="referentialsUsed">Les référentiels utilisés.</param>
        /// <param name="natureFilter">Le filtre sur les codes de nature de scénario.</param>
        /// <returns>Les scénarios.</returns>
        private static async Task<Scenario[]> LoadScenarios(KsmedEntities context, int projectId, IDictionary<ProcessReferentialIdentifier, bool> referentialsUsed, GetDataScenarioNatures natureFilter)
        {
            Scenario[] scenarios;

            ObjectQuery<Scenario> query = context.Scenarios.Include(nameof(Scenario.State));

            switch (natureFilter)
            {
                case GetDataScenarioNatures.InitialAndTarget:
                    scenarios = await query
                        .Where(s => s.ProjectId == projectId && (s.NatureCode == KnownScenarioNatures.Initial || s.NatureCode == KnownScenarioNatures.Target))
                        .ToArrayAsync();
                    break;

                case GetDataScenarioNatures.Realized:
                    scenarios = await query
                        .Where(s => s.ProjectId == projectId && s.NatureCode == KnownScenarioNatures.Realized)
                        .ToArrayAsync();
                    break;

                case GetDataScenarioNatures.All:
                    scenarios = await query
                        .Where(s => s.ProjectId == projectId)
                        .ToArrayAsync();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }


            await Queries.LoadScenariosDetails(context, scenarios, referentialsUsed);

            return scenarios;
        }

        /// <summary>
        /// Obtient la durée process des actions spécifiées. 
        /// </summary>
        /// <param name="context">Le contexte EF.</param>
        /// <param name="ids">Les identifiants.</param>
        /// <returns>Les durées</returns>
        internal static async Task<ActionDuration[]> GetActionsBuildDurations(KsmedEntities context, IEnumerable<int> ids)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT a.ActionId, a.BuildFinish, a.BuildStart FROM KActions AS a WHERE a.ActionId IN { ");

            foreach (int id in ids)
            {
                query.Append(id);
                query.Append(", ");
            }

            query.Remove(query.Length - 2, 2);

            query.Append(" }");

            // Initialise l'OSpace
            context.CreateQuery<KAction>(KsmedEntities.KActionsEntitySetName);

            return (await new ObjectQuery<DbDataRecord>(query.ToString(), context).ExecuteAsync(MergeOption.NoTracking))
                .Select(dbdr => new ActionDuration
                {
                    ActionId = dbdr.GetInt32(0),
                    BuildDuration = dbdr.GetInt64(1) - dbdr.GetInt64(2),
                }).ToArray();
        }

        /// <summary>
        /// Crée une nouvelle partie réduite et l'active sur l'action.
        /// </summary>
        /// <param name="originalAction">L'action d'origine.</param>
        /// <param name="newAction">L'action dérivée.</param>
        internal static void ApplyReduced(KAction originalAction, KAction newAction)
        {
            string actionTypeCode;

            if (originalAction.IsReduced && originalAction.Reduced.ActionTypeCode != null)
                actionTypeCode = originalAction.Reduced.ActionTypeCode;
            else
                actionTypeCode = null;

            ApplyNewReduced(newAction, actionTypeCode);
        }

        /// <summary>
        /// Crée une nouvelle partie réduite et l'active sur l'action.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="actionTypeCode">Le type souhaité ou <c>null</c> pour qu'il soit déterminé automatiquement.</param>
        internal static void ApplyNewReduced(KAction action, string actionTypeCode = null)
        {
            KActionReduced newReduced = new KActionReduced()
            {
                ReductionRatio = 0.0,
                OriginalBuildDuration = action.BuildDuration,
            };

            if (actionTypeCode != null)
                newReduced.ActionTypeCode = actionTypeCode;
            else if (action.IsReduced && action.Reduced.ActionTypeCode != null)
                newReduced.ActionTypeCode = action.Reduced.ActionTypeCode;
            else if (action.Category is ActionCategory actionCategory && actionCategory?.ActionTypeCode != null)
                newReduced.ActionTypeCode = actionCategory.ActionTypeCode;
            else
                newReduced.ActionTypeCode = KnownActionCategoryTypes.I;

            action.Reduced = newReduced;
            ActionsTimingsMoveManagement.ApplyReducedType(action, null);
        }

        /// <summary>
        /// S'assure qu'une solution vide existe sur le scénario cible, s'il est nécessaire d'en avoir une.
        /// Remappe également les actions avec les solutions vides.
        /// </summary>
        /// <param name="scenario">Le scénario.</param>
        internal static void EnsureEmptySolutionExists(Scenario scenario)
        {
            IEnumerable<KAction> emptySolutions = scenario.Actions
                .Where(s => s.IsReduced && string.IsNullOrWhiteSpace(s.Reduced.Solution));

            if (emptySolutions.Any())
            {
                Solution empty = scenario.Solutions.FirstOrDefault(s => s.IsEmpty);
                if (empty == null)
                {
                    // Créer une nouvelle solution
                    empty = new Solution()
                    {
                        SolutionDescription = LocalizationManager.GetString("Business_EmptySolution"),
                        IsEmpty = true,
                        Approved = true,
                    };
                    scenario.Solutions.Add(empty);
                }

                foreach (KAction action in emptySolutions)
                    action.Reduced.Solution = empty.SolutionDescription;
            }

        }

        /// <summary>
        /// Met à jour l'état "Approuvé" des solutions.
        /// </summary>
        /// <param name="scenario">Le scénario mis à jour.</param>
        internal static void UdpateSolutionsApprovedState(Scenario scenario)
        {
            // Appliquer l'état Approved
            foreach (Solution solution in scenario.Solutions)
            {
                KAction firstMatchingReduced = scenario.Actions.FirstOrDefault(a => a.IsReduced && a.Reduced.Solution == solution.SolutionDescription);

                if (firstMatchingReduced != null)
                {
                    if (solution.IsMarkedAsUnchanged)
                        solution.MarkAsModified();
                    solution.Approved = firstMatchingReduced.Reduced.Approved;
                }
            }
        }

        /// <summary>
        /// Met à jour IsGroup sur les actions du scénario.
        /// </summary>
        /// <param name="scenario">Le scénario</param>
        internal static void UpdateIsGroup(Scenario scenario)
        {
            // Mettre à jour IsGroup
            foreach (KAction action in scenario.Actions)
                action.IsGroup = WBSHelper.HasChildren(action, scenario.Actions);
        }

        /// <summary>
        /// Obtient les valeurs indiquant si les référentiels sont utilisés en effectuant une requête.
        /// </summary>
        /// <param name="context">Le contexte.</param>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <returns>les valeurs indiquant si les référentiels sont utilisés </returns>
        internal static async Task<IDictionary<ProcessReferentialIdentifier, bool>> GetReferentialsUse(KsmedEntities context, int projectId) =>
            GetReferentialsUse(await context.ProjectReferentials.Where(pr => pr.ProjectId == projectId).ToArrayAsync());

        /// <summary>
        /// Obtient les valeurs indiquant si les référentiels sont utilisés.
        /// </summary>
        /// <param name="prs">Les instances des <see cref="ProjectReferential"/>.</param>
        /// <returns>les valeurs indiquant si les référentiels sont utilisés </returns>
        internal static IDictionary<ProcessReferentialIdentifier, bool> GetReferentialsUse(IEnumerable<ProjectReferential> prs) =>
            prs.ToDictionary(pr => (ProcessReferentialIdentifier)pr.ReferentialId, pr => pr.IsEnabled);

        /// <summary>
        /// Définit la référence vers Orginal dans le contexte sur la tâche.
        /// </summary>
        /// <param name="context">Le contexte EF.</param>
        /// <param name="action">L'action.</param>
        private static void SetActionsOriginalReference(KsmedEntities context, KAction action)
        {
            if (action.Original != null && action.Original.ActionId == default(int))
            {
                // Puique la propriété Original n'est pas mappée et OriginalId vaut 0 car l'action est New, il faut passer par le relationship manager
                context.SetRelationShipReferenceValue(
                    action,
                    action.Original,
                    a => a.OriginalActionId);
            }
        }

        /// <summary>
        /// Obtient les libellés des champs personnalisés.
        /// </summary>
        /// <param name="project">Le projet</param>
        /// <returns>Les libellés.</returns>
        private static CustomFieldsLabels GetCustomFieldsLabels(Project project)
        {
            return new CustomFieldsLabels
            {
                Text1 = project.CustomTextLabel,
                Text2 = project.CustomTextLabel2,
                Text3 = project.CustomTextLabel3,
                Text4 = project.CustomTextLabel4,
                Numeric1 = project.CustomNumericLabel,
                Numeric2 = project.CustomNumericLabel2,
                Numeric3 = project.CustomNumericLabel3,
                Numeric4 = project.CustomNumericLabel4,
            };
        }

        /// <summary>
        /// Représente la durée process d'une action.
        /// </summary>
        internal class ActionDuration
        {
            public int ActionId { get; set; }
            public long BuildDuration { get; set; }
        }


        /// <summary>
        /// Permet de trier scénarios pour la vue restitution.
        /// </summary>
        public class ScenarioRestitutionComparer : IComparer<Scenario>
        {
            readonly KnownScenarioNatures.ScenarioNatureDefaultOrderComparer _natureComparer;

            /// <summary>
            /// Initialise une nouvelle instance de la classe <see cref="ScenarioRestitutionComparer"/>.
            /// </summary>
            public ScenarioRestitutionComparer()
            {
                _natureComparer = new KnownScenarioNatures.ScenarioNatureDefaultOrderComparer();
            }

            public int Compare(Scenario x, Scenario y)
            {
                int nature = _natureComparer.Compare(x.NatureCode, y.NatureCode);

                if (nature != 0)
                    return nature;

                // Comparer l'état
                if (x.StateCode == KnownScenarioStates.Draft && x.StateCode == KnownScenarioStates.Validated)
                    return -1;

                // Comparer l'id
                return x.ScenarioId.CompareTo(y.ScenarioId);
            }
        }


    }
}