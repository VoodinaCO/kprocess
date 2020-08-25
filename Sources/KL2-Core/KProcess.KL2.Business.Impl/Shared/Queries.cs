using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Shared
{
    /// <summary>
    /// Fournit des aides au requêtage EF et Linq.
    /// </summary>
    internal static class Queries
    {

        /// <summary>
        /// Filtre les référentiels pour un projet au travers d'une requête ESQL modifiée.
        /// </summary>
        /// <typeparam name="T">Le type du référentiel</typeparam>
        /// <param name="query">La requête.</param>
        /// <param name="processId">L'identifiant du process.</param>
        /// <param name="refId">L'identifiant du référentiel.</param>
        /// <returns>Les éléments filtrés</returns>
        internal static ObjectQuery<T> FilterReferentials<T>(ObjectQuery<T> query, int processId, ProcessReferentialIdentifier refId)
            where T : IActionReferential
        {
            string typeName;
            string where = @"it IS OF ({0}) AND it.IsDeleted = FALSE AND (it.ProcessId IS NULL OR it.ProcessId = @ProcessId)";

            switch (refId)
            {
                case ProcessReferentialIdentifier.Operator:
                case ProcessReferentialIdentifier.Equipment:
                    throw new InvalidOperationException("Utiliser FilterResources à la place");
                case ProcessReferentialIdentifier.Category:
                    typeName = ActionCategory.TypeFullName;
                    break;
                case ProcessReferentialIdentifier.Skill:
                    typeName = Skill.TypeFullName;
                    where = @"it IS OF ({0}) AND it.IsDeleted = FALSE";
                    break;
                case ProcessReferentialIdentifier.Ref1:
                    typeName = Ref1.TypeFullName;
                    break;
                case ProcessReferentialIdentifier.Ref2:
                    typeName = Ref2.TypeFullName;
                    break;
                case ProcessReferentialIdentifier.Ref3:
                    typeName = Ref3.TypeFullName;
                    break;
                case ProcessReferentialIdentifier.Ref4:
                    typeName = Ref4.TypeFullName;
                    break;
                case ProcessReferentialIdentifier.Ref5:
                    typeName = Ref5.TypeFullName;
                    break;
                case ProcessReferentialIdentifier.Ref6:
                    typeName = Ref6.TypeFullName;
                    break;
                case ProcessReferentialIdentifier.Ref7:
                    typeName = Ref7.TypeFullName;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return query.Where(string.Format(where, typeName), new ObjectParameter("ProcessId", processId));
        }

        /// <summary>
        /// Charge les référentiels du projet spécifié dans le contexte.
        /// </summary>
        /// <param name="context">Le contexte.</param>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="referentialsUse">Les référentiels utilisés, ou <c>null</c> pour les utiliser tous.</param>
        /// <returns>Les référentiels chargés.</returns>
        internal static async Task<Referentials> LoadAllReferentialsOfProject(KsmedEntities context, int projectId, IDictionary<ProcessReferentialIdentifier, bool> referentialsUse)
        {
            bool isNull = referentialsUse == null;
            int processId = (await context.Projects.SingleAsync(p => p.ProjectId == projectId)).ProcessId;

            Referentials results = new Referentials
            {
                // Toujours actif
                Categories = await FilterReferentials(context.ActionCategories, processId, ProcessReferentialIdentifier.Category).ToArrayAsync(),
                Skills = await FilterReferentials(context.Skills, processId, ProcessReferentialIdentifier.Skill).ToArrayAsync(),
                Resources = await FilterResources(context, processId).ToArrayAsync()
            };

            if (isNull || referentialsUse[ProcessReferentialIdentifier.Ref1])
                results.Ref1s = await FilterReferentials(context.Refs1, processId, ProcessReferentialIdentifier.Ref1).ToArrayAsync();

            if (isNull || referentialsUse[ProcessReferentialIdentifier.Ref2])
                results.Ref2s = await FilterReferentials(context.Refs2, processId, ProcessReferentialIdentifier.Ref2).ToArrayAsync();

            if (isNull || referentialsUse[ProcessReferentialIdentifier.Ref3])
                results.Ref3s = await FilterReferentials(context.Refs3, processId, ProcessReferentialIdentifier.Ref3).ToArrayAsync();

            if (isNull || referentialsUse[ProcessReferentialIdentifier.Ref4])
                results.Ref4s = await FilterReferentials(context.Refs4, processId, ProcessReferentialIdentifier.Ref4).ToArrayAsync();

            if (isNull || referentialsUse[ProcessReferentialIdentifier.Ref5])
                results.Ref5s = await FilterReferentials(context.Refs5, processId, ProcessReferentialIdentifier.Ref5).ToArrayAsync();

            if (isNull || referentialsUse[ProcessReferentialIdentifier.Ref6])
                results.Ref6s = await FilterReferentials(context.Refs6, processId, ProcessReferentialIdentifier.Ref6).ToArrayAsync();

            if (isNull || referentialsUse[ProcessReferentialIdentifier.Ref7])
                results.Ref7s = await FilterReferentials(context.Refs7, processId, ProcessReferentialIdentifier.Ref7).ToArrayAsync();

            return results;
        }

        /// <summary>
        /// Permet de charger les ressources utilisées dans le process.
        /// </summary>
        /// <param name="context">Le contexte.</param>
        /// <param name="processId">L'identifiant du process.</param>
        /// <returns>La requête</returns>
        internal static IQueryable<Resource> FilterResources(KsmedEntities context, int processId)
        {
            return context.Resources.OfType<Equipment>().Where(e => (e.ProcessId == null || e.ProcessId == processId) && !e.IsDeleted).Cast<Resource>().Union(
                context.Resources.OfType<Operator>().Where(o => (o.ProcessId == null || o.ProcessId == processId) && !o.IsDeleted).Cast<Resource>());
        }

        /// <summary>
        /// Charge les liens Référentiels - Actions des actions spécifiées.
        /// </summary>
        /// <param name="context">Le contexte.</param>
        /// <param name="actions">Les actions.</param>
        /// <param name="referentialsUse">Les référentiels utilisés, ou <c>null</c> pour les utiliser tous.</param>
        internal static async Task LoadActionsReferentials(KsmedEntities context, IEnumerable<KAction> actions, IDictionary<ProcessReferentialIdentifier, bool> referentialsUse)
        {
            int[] actionIds = actions.Select(a => a.ActionId).ToArray();

            bool isNull = referentialsUse == null;

            if (isNull || referentialsUse[ProcessReferentialIdentifier.Ref1])
                await context.Ref1Actions
                    .Include(nameof(Ref1Action.Ref1))
                    .Include($"{nameof(Ref1Action.Ref1)}.{nameof(Ref1.CloudFile)}")
                    .Where(r => actionIds.Contains(r.ActionId)).ToArrayAsync();

            if (isNull || referentialsUse[ProcessReferentialIdentifier.Ref2])
                await context.Ref2Actions
                    .Include(nameof(Ref2Action.Ref2))
                    .Include($"{nameof(Ref2Action.Ref2)}.{nameof(Ref2.CloudFile)}")
                    .Where(r => actionIds.Contains(r.ActionId)).ToArrayAsync();

            if (isNull || referentialsUse[ProcessReferentialIdentifier.Ref3])
                await context.Ref3Actions
                    .Include(nameof(Ref3Action.Ref3))
                    .Include($"{nameof(Ref3Action.Ref3)}.{nameof(Ref3.CloudFile)}")
                    .Where(r => actionIds.Contains(r.ActionId)).ToArrayAsync();

            if (isNull || referentialsUse[ProcessReferentialIdentifier.Ref4])
                await context.Ref4Actions
                    .Include(nameof(Ref4Action.Ref4))
                    .Include($"{nameof(Ref4Action.Ref4)}.{nameof(Ref4.CloudFile)}")
                    .Where(r => actionIds.Contains(r.ActionId)).ToArrayAsync();

            if (isNull || referentialsUse[ProcessReferentialIdentifier.Ref5])
                await context.Ref5Actions
                    .Include(nameof(Ref5Action.Ref5))
                    .Include($"{nameof(Ref5Action.Ref5)}.{nameof(Ref5.CloudFile)}")
                    .Where(r => actionIds.Contains(r.ActionId)).ToArrayAsync();

            if (isNull || referentialsUse[ProcessReferentialIdentifier.Ref6])
                await context.Ref6Actions
                    .Include(nameof(Ref6Action.Ref6))
                    .Include($"{nameof(Ref6Action.Ref6)}.{nameof(Ref6.CloudFile)}")
                    .Where(r => actionIds.Contains(r.ActionId)).ToArrayAsync();

            if (isNull || referentialsUse[ProcessReferentialIdentifier.Ref7])
                await context.Ref7Actions
                    .Include(nameof(Ref7Action.Ref7))
                    .Include($"{nameof(Ref7Action.Ref7)}.{nameof(Ref7.CloudFile)}")
                    .Where(r => actionIds.Contains(r.ActionId)).ToArrayAsync();
        }

        /// <summary>
        /// Charge les détails des scénarios :
        /// Actions
        /// Actions.Reduced
        /// Actions.Predecessors
        /// Actions.Solution
        /// Actions.Ref*
        /// </summary>
        /// <param name="context">Le contexte.</param>
        /// <param name="scenarios">Les scénarios</param>
        /// <param name="referentialsUse">Les référentiels utilisés, ou <c>null</c> pour les utiliser tous.</param>
        internal static async Task LoadScenariosDetails(KsmedEntities context, IEnumerable<Scenario> scenarios, IDictionary<ProcessReferentialIdentifier, bool> referentialsUse)
        {
            IEnumerable<int> scId = scenarios.Select(s => s.ScenarioId);

            KAction[] actions = await context.KActions
                .Include(nameof(KAction.Predecessors))
                .Include(nameof(KAction.Reduced))
                .Include(nameof(KAction.LinkedProcess))
                .Include(nameof(KAction.Thumbnail))
                .Where(a => scId.Contains(a.ScenarioId))
                .ToArrayAsync();

            Solution[] solutions = await context.Solutions.Where(s => scId.Contains(s.ScenarioId)).ToArrayAsync();

            await LoadActionsReferentials(context, actions, referentialsUse);
        }

        /// <summary>
        /// Charge toutes les propriétés de navigation des référentiels des actions spécifiées.
        /// </summary>
        /// <param name="context">Le contexte EF.</param>
        /// <param name="actions">les actions.</param>
        internal static void LoadAllReferentials(KsmedEntities context, IEnumerable<KAction> actions)
        {
            Dictionary<int, ActionCategory> categories = new Dictionary<int, ActionCategory>();
            Dictionary<int, Skill> skills = new Dictionary<int, Skill>();
            Dictionary<int, Resource> resources = new Dictionary<int, Resource>();
            Dictionary<int, Ref1> ref1 = new Dictionary<int, Ref1>();
            Dictionary<int, Ref2> ref2 = new Dictionary<int, Ref2>();
            Dictionary<int, Ref3> ref3 = new Dictionary<int, Ref3>();
            Dictionary<int, Ref4> ref4 = new Dictionary<int, Ref4>();
            Dictionary<int, Ref5> ref5 = new Dictionary<int, Ref5>();
            Dictionary<int, Ref6> ref6 = new Dictionary<int, Ref6>();
            Dictionary<int, Ref7> ref7 = new Dictionary<int, Ref7>();

            foreach (KAction action in actions)
            {
                // Catégorie
                if (action.CategoryId.HasValue)
                {
                    if (categories.ContainsKey(action.CategoryId.Value))
                        action.Category = categories[action.CategoryId.Value];
                    else
                    {
                        context.LoadProperty(action, a => a.Category);
                        categories[action.CategoryId.Value] = action.Category;
                    }
                }

                // Compétence
                if (action.SkillId.HasValue)
                {
                    if (skills.ContainsKey(action.SkillId.Value))
                        action.Skill = skills[action.SkillId.Value];
                    else
                    {
                        context.LoadProperty(action, a => a.Skill);
                        skills[action.SkillId.Value] = action.Skill;
                    }
                }

                // Ressource
                if (action.ResourceId != null)
                {
                    if (resources.ContainsKey(action.ResourceId.Value))
                        action.Resource = resources[action.ResourceId.Value];
                    else
                    {
                        context.LoadProperty(action, a => a.Resource);
                        resources[action.ResourceId.Value] = action.Resource;
                    }
                }

                // Ref1
                if (action.Ref1.Count > 0)
                {
                    foreach (Ref1Action actionLink in action.Ref1)
                    {
                        if (ref1.ContainsKey(actionLink.RefId))
                            actionLink.Ref1 = ref1[actionLink.RefId];
                        else
                        {
                            context.LoadProperty(actionLink, a => a.Ref1);
                            ref1[actionLink.RefId] = actionLink.Ref1;
                        }
                    }
                }

                // Ref2
                if (action.Ref2.Count > 0)
                {
                    foreach (Ref2Action actionLink in action.Ref2)
                    {
                        if (ref2.ContainsKey(actionLink.RefId))
                            actionLink.Ref2 = ref2[actionLink.RefId];
                        else
                        {
                            context.LoadProperty(actionLink, a => a.Ref2);
                            ref2[actionLink.RefId] = actionLink.Ref2;
                        }
                    }
                }

                // Ref3
                if (action.Ref3.Count > 0)
                {
                    foreach (Ref3Action actionLink in action.Ref3)
                    {
                        if (ref3.ContainsKey(actionLink.RefId))
                            actionLink.Ref3 = ref3[actionLink.RefId];
                        else
                        {
                            context.LoadProperty(actionLink, a => a.Ref3);
                            ref3[actionLink.RefId] = actionLink.Ref3;
                        }
                    }
                }

                // Ref4
                if (action.Ref4.Count > 0)
                {
                    foreach (Ref4Action actionLink in action.Ref4)
                    {
                        if (ref4.ContainsKey(actionLink.RefId))
                            actionLink.Ref4 = ref4[actionLink.RefId];
                        else
                        {
                            context.LoadProperty(actionLink, a => a.Ref4);
                            ref4[actionLink.RefId] = actionLink.Ref4;
                        }
                    }
                }

                // Ref5
                if (action.Ref5.Count > 0)
                {
                    foreach (Ref5Action actionLink in action.Ref5)
                    {
                        if (ref5.ContainsKey(actionLink.RefId))
                            actionLink.Ref5 = ref5[actionLink.RefId];
                        else
                        {
                            context.LoadProperty(actionLink, a => a.Ref5);
                            ref5[actionLink.RefId] = actionLink.Ref5;
                        }
                    }
                }

                // Ref6
                if (action.Ref6.Count > 0)
                {
                    foreach (Ref6Action actionLink in action.Ref6)
                    {
                        if (ref6.ContainsKey(actionLink.RefId))
                            actionLink.Ref6 = ref6[actionLink.RefId];
                        else
                        {
                            context.LoadProperty(actionLink, a => a.Ref6);
                            ref6[actionLink.RefId] = actionLink.Ref6;
                        }
                    }
                }

                // Ref7
                if (action.Ref7.Count > 0)
                {
                    foreach (Ref7Action actionLink in action.Ref7)
                    {
                        if (ref7.ContainsKey(actionLink.RefId))
                            actionLink.Ref7 = ref7[actionLink.RefId];
                        else
                        {
                            context.LoadProperty(actionLink, a => a.Ref7);
                            ref7[actionLink.RefId] = actionLink.Ref7;
                        }
                    }
                }


            }
        }

    }

    /// <summary>
    /// Représente les résultats de requêtage des référentiels
    /// </summary>
    public class Referentials
    {
        /// <summary>
        /// Obtient ou définit les catégories.
        /// </summary>
        public ActionCategory[] Categories { get; set; }

        /// <summary>
        /// Obtient ou définit les compétences.
        /// </summary>
        public Skill[] Skills { get; set; }

        /// <summary>
        /// Obtient ou définit les ressources.
        /// </summary>
        public Resource[] Resources { get; set; }

        /// <summary>
        /// Obtient ou définit les opérateurs.
        /// </summary>
        public Operator[] Operators { get; set; }

        /// <summary>
        /// Obtient ou définit les équipements.
        /// </summary>
        public Equipment[] Equipments { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels 1.
        /// </summary>
        public Ref1[] Ref1s { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels 2.
        /// </summary>
        public Ref2[] Ref2s { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels 3.
        /// </summary>
        public Ref3[] Ref3s { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels 4.
        /// </summary>
        public Ref4[] Ref4s { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels 5.
        /// </summary>
        public Ref5[] Ref5s { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels 6.
        /// </summary>
        public Ref6[] Ref6s { get; set; }

        /// <summary>
        /// Obtient ou définit les référentiels 7
        /// </summary>
        public Ref7[] Ref7s { get; set; }
    }

}
