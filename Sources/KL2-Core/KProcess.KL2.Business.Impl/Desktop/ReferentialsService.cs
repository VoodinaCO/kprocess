using KProcess.Business;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Referentials;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Desktop
{
    /// <summary>
    /// Représente un service de gestion des référentiels d'actions.
    /// </summary>
    public class ReferentialsService : IBusinessService, IReferentialsService
    {

        readonly ITraceManager _traceManager;
        readonly ISecurityContext _securityContext;
        readonly ILocalizationManager _localizationManager;

        public ReferentialsService(
            ISecurityContext securityContext,
            ILocalizationManager localizationManager,
            ITraceManager traceManager)
        {
            _securityContext = securityContext;
            _localizationManager = localizationManager;
            _traceManager = traceManager;
        }

        public string GetLabel(ProcessReferentialIdentifier refe)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                return context.Referentials.SingleOrDefault(_ => _.ReferentialId == refe)?.Label;
            }
        }

        public string GetLabelPlural(ProcessReferentialIdentifier refe)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                return _localizationManager.GetStringFormat("ViewModel_Restitution_View_All", context.Referentials.SingleOrDefault(_ => _.ReferentialId == refe)?.Label);
            }
        }

        /// <summary>
        /// Obtient la configuration des réferentiels.
        /// </summary>
        /// <returns>Les référentiels.</returns>
        public async Task<Referential[]> GetApplicationReferentials()
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                return await context.Referentials.ToArrayAsync();
            }
        }

        public async Task<bool> ReferentialUsed(ProcessReferentialIdentifier processReferentialId, int referentialId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                switch (processReferentialId)
                {
                    case ProcessReferentialIdentifier.Operator:
                    case ProcessReferentialIdentifier.Equipment:
                        return await context.DocumentationActionDrafts.AnyAsync(_ => _.ResourceId == referentialId)
                            || await context.KActions.Include(nameof(KAction.Scenario)).AnyAsync(_ => _.ResourceId == referentialId && !_.Scenario.IsDeleted);
                    case ProcessReferentialIdentifier.Category:
                        return await context.DocumentationActionDrafts.AnyAsync(_ => _.ActionCategoryId == referentialId)
                            || await context.KActions.Include(nameof(KAction.Scenario)).AnyAsync(_ => _.CategoryId == referentialId && !_.Scenario.IsDeleted);
                    case ProcessReferentialIdentifier.Skill:
                        return await context.DocumentationActionDrafts.AnyAsync(_ => _.SkillId == referentialId)
                            || await context.KActions.Include(nameof(KAction.Scenario)).AnyAsync(_ => _.SkillId == referentialId && !_.Scenario.IsDeleted);
                    case ProcessReferentialIdentifier.Ref1:
                        return await context.ReferentialDocumentationActionDrafts.AnyAsync(_ => _.RefNumber == ((int)processReferentialId - 3) && _.ReferentialId == referentialId)
                            || await context.Ref1Actions.Include(nameof(Ref1Action.Action)).Include(nameof(KAction.Scenario)).AnyAsync(_ => _.RefId == referentialId && !_.Action.Scenario.IsDeleted);
                    case ProcessReferentialIdentifier.Ref2:
                        return await context.ReferentialDocumentationActionDrafts.AnyAsync(_ => _.RefNumber == ((int)processReferentialId - 3) && _.ReferentialId == referentialId)
                            || await context.Ref2Actions.Include(nameof(Ref2Action.Action)).Include(nameof(KAction.Scenario)).AnyAsync(_ => _.RefId == referentialId && !_.Action.Scenario.IsDeleted);
                    case ProcessReferentialIdentifier.Ref3:
                        return await context.ReferentialDocumentationActionDrafts.AnyAsync(_ => _.RefNumber == ((int)processReferentialId - 3) && _.ReferentialId == referentialId)
                            || await context.Ref3Actions.Include(nameof(Ref3Action.Action)).Include(nameof(KAction.Scenario)).AnyAsync(_ => _.RefId == referentialId && !_.Action.Scenario.IsDeleted);
                    case ProcessReferentialIdentifier.Ref4:
                        return await context.ReferentialDocumentationActionDrafts.AnyAsync(_ => _.RefNumber == ((int)processReferentialId - 3) && _.ReferentialId == referentialId)
                            || await context.Ref4Actions.Include(nameof(Ref4Action.Action)).Include(nameof(KAction.Scenario)).AnyAsync(_ => _.RefId == referentialId && !_.Action.Scenario.IsDeleted);
                    case ProcessReferentialIdentifier.Ref5:
                        return await context.ReferentialDocumentationActionDrafts.AnyAsync(_ => _.RefNumber == ((int)processReferentialId - 3) && _.ReferentialId == referentialId)
                            || await context.Ref5Actions.Include(nameof(Ref5Action.Action)).Include(nameof(KAction.Scenario)).AnyAsync(_ => _.RefId == referentialId && !_.Action.Scenario.IsDeleted);
                    case ProcessReferentialIdentifier.Ref6:
                        return await context.ReferentialDocumentationActionDrafts.AnyAsync(_ => _.RefNumber == ((int)processReferentialId - 3) && _.ReferentialId == referentialId)
                            || await context.Ref6Actions.Include(nameof(Ref6Action.Action)).Include(nameof(KAction.Scenario)).AnyAsync(_ => _.RefId == referentialId && !_.Action.Scenario.IsDeleted);
                    case ProcessReferentialIdentifier.Ref7:
                        return await context.ReferentialDocumentationActionDrafts.AnyAsync(_ => _.RefNumber == ((int)processReferentialId - 3) && _.ReferentialId == referentialId)
                            || await context.Ref7Actions.Include(nameof(Ref7Action.Action)).Include(nameof(KAction.Scenario)).AnyAsync(_ => _.RefId == referentialId && !_.Action.Scenario.IsDeleted);
                }

                // Not used
                return false;
            }
        }

        /// <inheritdoc />
        public async Task UpdateReferentialLabel(ProcessReferentialIdentifier refId, string label) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    Referential[] allOtherReferentials = await context.Referentials.Where(r => r.ReferentialId != refId).ToArrayAsync();

                    if (allOtherReferentials.Any(r => string.Equals(label, r.Label, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        BLLFuncException ex = new BLLFuncException("Le nom du référentiel est déjà utilisé.")
                        {
                            ErrorCode = KnownErrorCodes.ReferentialNameAlreadyUsed
                        };
                        throw ex;
                    }

                    Referential refe = new Referential
                    {
                        ReferentialId = refId,
                        Label = label,
                    };
                    refe.MarkAsModified();

                    context.Referentials.ApplyChanges(refe);
                    await context.SaveChangesAsync();
                }
            });

        #region Catégories

        /// <summary>
        /// Obtient dans l'ordre :
        /// Les catégories d'actions de tous les projets
        /// Tous les types d'actions 
        /// Toutes les valorisations d'actions.
        /// </summary>
        public async Task<(ActionCategory[] Categories, ActionValue[] ActionValues, ActionType[] ActionTypes, Procedure[] Processes)> LoadCategories() =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    ActionCategory[] categories = await context.ActionCategories
                        .Include(nameof(ActionCategory.Type))
                        .Include(nameof(ActionCategory.Value))
                        .Include(nameof(ActionCategory.CloudFile))
                        .Include($"{nameof(ActionCategory.Actions)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                        .Include(nameof(ActionCategory.DocumentationActionDrafts))
                        .Where(ac => !ac.IsDeleted)
                        .ToArrayAsync();

                    foreach (ActionCategory cat in categories)
                    {
                        cat.StartTracking();
                        cat.HasRelatedActions = await context.KActions
                            .Where(a => a.CategoryId == cat.ActionCategoryId)
                            .AnyAsync();
                    }

                    ActionType[] types = await context.ActionTypes
                        .ToArrayAsync();

                    ActionValue[] values = await context.ActionValues
                        .ToArrayAsync();

                    return (categories, values, types, await GetProcesses(context));
                }
            });

        /// <summary>
        /// Sauvegarde les catégories.
        /// </summary>
        /// <param name="categories">Les catégories.</param>
        public async Task<IEnumerable<ActionCategory>> SaveCategories(IEnumerable<ActionCategory> categories) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    foreach (ActionCategory category in categories)
                        context.ActionCategories.ApplyChanges(category);

                    await context.SaveChangesAsync();

                    foreach (ActionCategory category in categories)
                        category.MarkAsUnchanged();

                    return categories;
                }
            });

        #endregion

        #region Compétences

        /// <summary>
        /// Obtient les compétences d'actions de tous les projets
        /// </summary>
        public async Task<Skill[]> LoadSkills(bool allInfos = false) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    Skill[] skills = null;
                    if (allInfos)
                        skills = await context.Skills
                            .Include(nameof(Skill.CloudFile))
                            .Include(nameof(Skill.PublishedActions))
                            .Include($"{nameof(Skill.PublishedActions)}.{nameof(PublishedAction.Publication)}")
                            .Include($"{nameof(Skill.PublishedActions)}.{nameof(PublishedAction.Publication)}.{nameof(Publication.Process)}")
                            .Include($"{nameof(Skill.PublishedActions)}.{nameof(PublishedAction.Publication)}.{nameof(Publication.Qualifications)}")
                            .Include($"{nameof(Skill.PublishedActions)}.{nameof(PublishedAction.LinkedPublication)}")
                            .Include($"{nameof(Skill.PublishedActions)}.{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.Qualifications)}")
                            .Include($"{nameof(Skill.Actions)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                            .Include(nameof(Skill.DocumentationActionDrafts))
                            .Where(s => !s.IsDeleted)
                            .ToArrayAsync();
                    else
                        skills = await context.Skills
                            .Include(nameof(Skill.CloudFile))
                            .Where(s => !s.IsDeleted)
                            .ToArrayAsync();


                    foreach (Skill skill in skills)
                    {
                        skill.StartTracking();
                        skill.HasRelatedActions = await context.PublishedActions
                            .Where(a => a.SkillId == skill.SkillId)
                            .AnyAsync();
                    }

                    return skills;
                }
            });

        /// <summary>
        /// Sauvegarde les compétences.
        /// </summary>
        /// <param name="skills">Les compétences.</param>
        public async Task<IEnumerable<Skill>> SaveSkills(IEnumerable<Skill> skills) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    foreach (Skill skill in skills)
                        context.Skills.ApplyChanges(skill);

                    await context.SaveChangesAsync();

                    foreach (Skill skill in skills)
                        skill.MarkAsUnchanged();

                    return skills;
                }
            });

        #endregion

        #region Référentiels multiples


        /// <summary>
        /// Obtient les référentiels standards et projets du type spécifié.
        /// </summary>
        /// <param name="refId">L'identifiant du référentiel.</param>
        public async Task<(IMultipleActionReferential[] Referentials, Procedure[] Processes)> GetReferentials(ProcessReferentialIdentifier refId, int? processId = null) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    IMultipleActionReferential[] results;

                    int[] refIdsUsed;
                    switch (refId)
                    {
                        case ProcessReferentialIdentifier.Operator:
                        case ProcessReferentialIdentifier.Equipment:
                        case ProcessReferentialIdentifier.Category:
                        case ProcessReferentialIdentifier.Skill:
                            throw new InvalidOperationException();
                        case ProcessReferentialIdentifier.Ref1:
                            results = await context.Refs1
                                .Include(nameof(Ref1.CloudFile))
                                .Include(nameof(Ref1.Process))
                                .Include(nameof(Ref1.Ref1Actions))
                                .Include($"{nameof(Ref1.Ref1Actions)}.{nameof(Ref1Action.Action)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .ToArrayAsync();
                            refIdsUsed = await context.Ref1Actions
                                .Select(al => al.RefId)
                                .Distinct()
                                .ToArrayAsync();
                            break;
                        case ProcessReferentialIdentifier.Ref2:
                            results = await context.Refs2
                                .Include(nameof(Ref2.CloudFile))
                                .Include(nameof(Ref2.Process))
                                .Include(nameof(Ref2.Ref2Actions))
                                .Include($"{nameof(Ref2.Ref2Actions)}.{nameof(Ref2Action.Action)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .ToArrayAsync();
                            refIdsUsed = await context.Ref2Actions
                                .Select(al => al.RefId)
                                .Distinct()
                                .ToArrayAsync();
                            break;
                        case ProcessReferentialIdentifier.Ref3:
                            results = await context.Refs3
                                .Include(nameof(Ref3.CloudFile))
                                .Include(nameof(Ref3.Process))
                                .Include(nameof(Ref3.Ref3Actions))
                                .Include($"{nameof(Ref3.Ref3Actions)}.{nameof(Ref3Action.Action)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .ToArrayAsync();
                            refIdsUsed = await context.Ref3Actions
                                .Select(al => al.RefId)
                                .Distinct()
                                .ToArrayAsync();
                            break;
                        case ProcessReferentialIdentifier.Ref4:
                            results = await context.Refs4
                                .Include(nameof(Ref4.CloudFile))
                                .Include(nameof(Ref4.Process))
                                .Include(nameof(Ref4.Ref4Actions))
                                .Include($"{nameof(Ref4.Ref4Actions)}.{nameof(Ref4Action.Action)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .ToArrayAsync();
                            refIdsUsed = await context.Ref4Actions
                                .Select(al => al.RefId)
                                .Distinct()
                                .ToArrayAsync();
                            break;
                        case ProcessReferentialIdentifier.Ref5:
                            results = await context.Refs5
                                .Include(nameof(Ref5.CloudFile))
                                .Include(nameof(Ref5.Process))
                                .Include(nameof(Ref5.Ref5Actions))
                                .Include($"{nameof(Ref5.Ref5Actions)}.{nameof(Ref5Action.Action)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .ToArrayAsync();
                            refIdsUsed = await context.Ref5Actions
                                .Select(al => al.RefId)
                                .Distinct()
                                .ToArrayAsync();
                            break;
                        case ProcessReferentialIdentifier.Ref6:
                            results = await context.Refs6
                                .Include(nameof(Ref6.CloudFile))
                                .Include(nameof(Ref6.Process))
                                .Include(nameof(Ref6.Ref6Actions))
                                .Include($"{nameof(Ref6.Ref6Actions)}.{nameof(Ref6Action.Action)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .ToArrayAsync();
                            refIdsUsed = await context.Ref6Actions
                                .Select(al => al.RefId)
                                .Distinct()
                                .ToArrayAsync();
                            break;
                        case ProcessReferentialIdentifier.Ref7:
                            results = await context.Refs7
                                .Include(nameof(Ref7.CloudFile))
                                .Include(nameof(Ref7.Process))
                                .Include(nameof(Ref7.Ref7Actions))
                                .Include($"{nameof(Ref7.Ref7Actions)}.{nameof(Ref7Action.Action)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .ToArrayAsync();
                            refIdsUsed = await context.Ref7Actions
                                .Select(al => al.RefId)
                                .Distinct()
                                .ToArrayAsync();
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    foreach (IMultipleActionReferential refe in results)
                    {
                        refe.StartTracking();
                        refe.HasRelatedActions = Array.IndexOf(refIdsUsed, refe.Id) != -1;
                    }

                    return (results, await GetProcesses(context));
                }
            });

        /// <summary>
        /// Obtient les référentiels standards et projets du type spécifié.
        /// </summary>
        /// <param name="refId">L'identifiant du référentiel.</param>
        public async Task<(IActionReferential[] Referentials, Procedure[] Processes)> GetAllReferentials(ProcessReferentialIdentifier refId, int? processId = null) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    IActionReferential[] results;

                    int[] refIdsUsed;
                    bool isRef = false;
                    switch (refId)
                    {
                        case ProcessReferentialIdentifier.Operator:
                        case ProcessReferentialIdentifier.Equipment:
                            var resultOperator = await context.Resources.OfType<Operator>()
                                .Include(nameof(Operator.CloudFile))
                                .Include(nameof(Operator.Process))
                                .Include(nameof(Operator.Actions))
                                .Include($"{nameof(Operator.Actions)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .OrderBy(r => r.Label)
                                .Cast<Resource>()
                                .ToListAsync();
                            var resultEquipment = await context.Resources.OfType<Equipment>()
                                .Include(nameof(Equipment.CloudFile))
                                .Include(nameof(Equipment.Process))
                                .Include(nameof(Equipment.Actions))
                                .Include($"{nameof(Equipment.Actions)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .OrderBy(r => r.Label)
                                .Cast<Resource>()
                                .ToListAsync();
                            resultOperator.AddRange(resultEquipment);
                            results = resultOperator.ToArray();
                            refIdsUsed = null;
                            break;
                        case ProcessReferentialIdentifier.Category:
                            results = await context.ActionCategories
                                .Include(nameof(ActionCategory.CloudFile))
                                .Include(nameof(ActionCategory.Process))
                                .Include(nameof(ActionCategory.Actions))
                                .Include($"{nameof(ActionCategory.Actions)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .OrderBy(r => r.Label)
                                .ToArrayAsync();
                            refIdsUsed = null;
                            break;
                        case ProcessReferentialIdentifier.Skill:
                            results = await context.Skills
                                .Include(nameof(Skill.CloudFile))
                                //.Include(nameof(Skill.Process))
                                .Include(nameof(Skill.Actions))
                                .Include($"{nameof(Skill.Actions)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted /*&& (processId == null || (r.ProcessId == processId || r.ProcessId == null))*/)
                                .OrderBy(r => r.Label)
                                .ToArrayAsync();
                            refIdsUsed = null;
                            break;
                        case ProcessReferentialIdentifier.Ref1:
                            isRef = true;
                            results = await context.Refs1
                                .Include(nameof(Ref1.CloudFile))
                                .Include(nameof(Ref1.Process))
                                .Include(nameof(Ref1.Ref1Actions))
                                .Include($"{nameof(Ref1.Ref1Actions)}.{nameof(Ref1Action.Action)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .OrderBy(r => r.Label)
                                .ToArrayAsync();
                            refIdsUsed = await context.Ref1Actions
                                .Select(al => al.RefId)
                                .Distinct()
                                .ToArrayAsync();
                            break;
                        case ProcessReferentialIdentifier.Ref2:
                            isRef = true;
                            results = await context.Refs2
                                .Include(nameof(Ref2.CloudFile))
                                .Include(nameof(Ref2.Process))
                                .Include(nameof(Ref2.Ref2Actions))
                                .Include($"{nameof(Ref2.Ref2Actions)}.{nameof(Ref2Action.Action)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .OrderBy(r => r.Label)
                                .ToArrayAsync();
                            refIdsUsed = await context.Ref2Actions
                                .Select(al => al.RefId)
                                .Distinct()
                                .ToArrayAsync();
                            break;
                        case ProcessReferentialIdentifier.Ref3:
                            isRef = true;
                            results = await context.Refs3
                                .Include(nameof(Ref3.CloudFile))
                                .Include(nameof(Ref3.Process))
                                .Include(nameof(Ref3.Ref3Actions))
                                .Include($"{nameof(Ref3.Ref3Actions)}.{nameof(Ref3Action.Action)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .OrderBy(r => r.Label)
                                .ToArrayAsync();
                            refIdsUsed = await context.Ref3Actions
                                .Select(al => al.RefId)
                                .Distinct()
                                .ToArrayAsync();
                            break;
                        case ProcessReferentialIdentifier.Ref4:
                            isRef = true;
                            results = await context.Refs4
                                .Include(nameof(Ref4.CloudFile))
                                .Include(nameof(Ref4.Process))
                                .Include(nameof(Ref4.Ref4Actions))
                                .Include($"{nameof(Ref4.Ref4Actions)}.{nameof(Ref4Action.Action)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .OrderBy(r => r.Label)
                                .ToArrayAsync();
                            refIdsUsed = await context.Ref4Actions
                                .Select(al => al.RefId)
                                .Distinct()
                                .ToArrayAsync();
                            break;
                        case ProcessReferentialIdentifier.Ref5:
                            isRef = true;
                            results = await context.Refs5
                                .Include(nameof(Ref5.CloudFile))
                                .Include(nameof(Ref5.Process))
                                .Include(nameof(Ref5.Ref5Actions))
                                .Include($"{nameof(Ref5.Ref5Actions)}.{nameof(Ref5Action.Action)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .OrderBy(r => r.Label)
                                .ToArrayAsync();
                            refIdsUsed = await context.Ref5Actions
                                .Select(al => al.RefId)
                                .Distinct()
                                .ToArrayAsync();
                            break;
                        case ProcessReferentialIdentifier.Ref6:
                            isRef = true;
                            results = await context.Refs6
                                .Include(nameof(Ref6.CloudFile))
                                .Include(nameof(Ref6.Process))
                                .Include(nameof(Ref6.Ref6Actions))
                                .Include($"{nameof(Ref6.Ref6Actions)}.{nameof(Ref6Action.Action)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .OrderBy(r => r.Label)
                                .ToArrayAsync();
                            refIdsUsed = await context.Ref6Actions
                                .Select(al => al.RefId)
                                .Distinct()
                                .ToArrayAsync();
                            break;
                        case ProcessReferentialIdentifier.Ref7:
                            isRef = true;
                            results = await context.Refs7
                                .Include(nameof(Ref7.CloudFile))
                                .Include(nameof(Ref7.Process))
                                .Include(nameof(Ref7.Ref7Actions))
                                .Include($"{nameof(Ref7.Ref7Actions)}.{nameof(Ref7Action.Action)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                                .Where(r => !r.IsDeleted && (processId == null || (r.ProcessId == processId || r.ProcessId == null)))
                                .OrderBy(r => r.Label)
                                .ToArrayAsync();
                            refIdsUsed = await context.Ref7Actions
                                .Select(al => al.RefId)
                                .Distinct()
                                .ToArrayAsync();
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (isRef == true)
                    {
                        foreach (IMultipleActionReferential refe in results)
                        {
                            refe.StartTracking();
                            refe.HasRelatedActions = Array.IndexOf(refIdsUsed, refe.Id) != -1;
                        }
                    }


                    return (results, await GetProcesses(context));
                }
            });

        /// <summary>
        /// Sauvegarde les référentiels.
        /// </summary>
        /// <typeparam name="TReferential">Le type du référentiel.</typeparam>
        /// <param name="referentials">Les référentiels.</param>
        public async Task<IEnumerable<TReferential>> SaveReferentials<TReferential>(IEnumerable<TReferential> referentials) where TReferential : class, IActionReferential =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    foreach (TReferential refe in referentials)
                        context.CreateObjectSet<TReferential>().ApplyChanges(refe);

                    await context.SaveChangesAsync();

                    foreach (TReferential refe in referentials)
                        refe.MarkAsUnchanged();
                    return referentials;
                }
            });

        #endregion

        #region Ressources

        /// <summary>
        /// Obtient dans l'ordre :
        /// Les équipements de tous les projets.
        /// </summary>
        public async Task<(Equipment[] Equipments, Procedure[] Processes)> LoadEquipments() =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    Equipment[] equipements = await context.Resources.OfType<Equipment>()
                        .Include(nameof(Equipment.CloudFile))
                        .Include(nameof(Equipment.DocumentationActionDrafts))
                        .Include($"{nameof(Equipment.Actions)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                        .Where(e => !e.IsDeleted)
                        .ToArrayAsync();

                    foreach (Equipment equipment in equipements)
                    {
                        equipment.StartTracking();
                        equipment.HasRelatedActions = await context.KActions
                            .Where(a => a.ResourceId == equipment.ResourceId)
                            .AnyAsync()
                            || await context.Videos
                                .Where(v => v.DefaultResourceId == equipment.ResourceId)
                                .AnyAsync();
                    }

                    return (equipements, await GetProcesses(context));
                }
            });

        /// <summary>
        /// Obtient dans l'ordre :
        /// Les opérateurs de tous les projets.
        /// </summary>
        public async Task<(Operator[] Operators, Procedure[] Processes)> LoadOperators() =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    Operator[] operators = await context.Resources.OfType<Operator>()
                        .Include(nameof(Operator.CloudFile))
                        .Include(nameof(Operator.DocumentationActionDrafts))
                        .Include($"{nameof(Operator.Actions)}.{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                        .Where(o => !o.IsDeleted)
                        .ToArrayAsync();

                    foreach (Operator op in operators)
                    {
                        op.StartTracking();
                        op.HasRelatedActions = await context.KActions
                            .Where(a => a.ResourceId == op.ResourceId)
                            .AnyAsync() 
                            || await context.Videos
                                .Where(v => v.DefaultResourceId == op.ResourceId)
                                .AnyAsync();
                    }

                    return (operators, await GetProcesses(context));
                }
            });

        /// <summary>
        /// Sauvegarde les ressources.
        /// </summary>
        /// <param name="resources">Les ressources.</param>
        public async Task<IEnumerable<Resource>> SaveResources(IEnumerable<Resource> resources) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    foreach (Resource res in resources)
                        context.Resources.ApplyChanges(res);

                    await context.SaveChangesAsync();

                    foreach (Resource res in resources)
                        res.MarkAsUnchanged();

                    return resources;
                }
            });

        #endregion

        /// <summary>
        /// Obtient les processes.
        /// </summary>
        /// <param name="context">Le contexte EF.</param>
        /// <returns>Les processes</returns>
        async Task<Procedure[]> GetProcesses(KsmedEntities context)
        {
            return await context.Procedures
                .Include(nameof(Procedure.Projects))
                .Include(nameof(Procedure.UserRoleProcesses))
                .Include($"{nameof(Procedure.UserRoleProcesses)}.{nameof(UserRoleProcess.User)}")
                .Where(p => !p.IsDeleted)
                .AsNoTracking()
                .ToArrayAsync();

            // Important : permet de mettre en cache tous les utilisateurs afin qu'ils soient définis en tant que navigation Property dans UserRoleProcesses
            /*User[] users = await context.Users
                .Where(u => !u.IsDeleted)
                .ToArrayAsync();

            foreach (Procedure p in processes)
            {
                p.StopTracking();
                p.MarkAsUnchanged();
            }

            return processes;*/
        }

        /// <summary>
        /// Fusionne des référentiels
        /// </summary>
        /// <param name="master">Le référentiel maître.</param>
        /// <param name="slaves">Les référentiels esclaves.</param>
        public async Task MergeReferentials(IActionReferential master, IActionReferential[] slaves) =>
            await Task.Run(async () =>
            {
                var slavesIds = slaves.Select(s => s.Id).ToArray();
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    if (master is IMultipleActionReferential)
                    {
                        // Si le référentiel est multiple, utiliser les groupes intermédiaires

                        if (master is Ref1)
                        {
                            await UpdateActionMultipleRefId<Ref1Action>(context, KsmedEntities.Ref1ActionsEntitySetName, 1, nameof(Ref1Action.RefId), master, slaves);
                            foreach (var referential in slaves)
                                context.Refs1.DeleteObject(await context.Refs1.SingleAsync(_ => _.RefId == referential.Id));
                        }
                        else if (master is Ref2)
                        {
                            await UpdateActionMultipleRefId<Ref2Action>(context, KsmedEntities.Ref2ActionsEntitySetName, 2, nameof(Ref2Action.RefId), master, slaves);
                            foreach (var referential in slaves)
                                context.Refs2.DeleteObject(await context.Refs2.SingleAsync(_ => _.RefId == referential.Id));
                        }
                        else if (master is Ref3)
                        {
                            await UpdateActionMultipleRefId<Ref3Action>(context, KsmedEntities.Ref3ActionsEntitySetName, 3, nameof(Ref3Action.RefId), master, slaves);
                            foreach (var referential in slaves)
                                context.Refs3.DeleteObject(await context.Refs3.SingleAsync(_ => _.RefId == referential.Id));
                        }
                        else if (master is Ref4)
                        {
                            await UpdateActionMultipleRefId<Ref4Action>(context, KsmedEntities.Ref4ActionsEntitySetName, 4, nameof(Ref4Action.RefId), master, slaves);
                            foreach (var referential in slaves)
                                context.Refs4.DeleteObject(await context.Refs4.SingleAsync(_ => _.RefId == referential.Id));
                        }
                        else if (master is Ref5)
                        {
                            await UpdateActionMultipleRefId<Ref5Action>(context, KsmedEntities.Ref5ActionsEntitySetName, 5, nameof(Ref5Action.RefId), master, slaves);
                            foreach (var referential in slaves)
                                context.Refs5.DeleteObject(await context.Refs5.SingleAsync(_ => _.RefId == referential.Id));
                        }
                        else if (master is Ref6)
                        {
                            await UpdateActionMultipleRefId<Ref6Action>(context, KsmedEntities.Ref6ActionsEntitySetName, 6, nameof(Ref6Action.RefId), master, slaves);
                            foreach (var referential in slaves)
                                context.Refs6.DeleteObject(await context.Refs6.SingleAsync(_ => _.RefId == referential.Id));
                        }
                        else if (master is Ref7)
                        {
                            await UpdateActionMultipleRefId<Ref7Action>(context, KsmedEntities.Ref7ActionsEntitySetName, 7, nameof(Ref7Action.RefId), master, slaves);
                            foreach (var referential in slaves)
                                context.Refs7.DeleteObject(await context.Refs7.SingleAsync(_ => _.RefId == referential.Id));
                        }
                        else
                            throw new ArgumentOutOfRangeException(nameof(master));
                    }
                    else
                    {
                        if (master is ActionCategory)
                        {
                            foreach (var kaction in await context.KActions.Where(_ => _.CategoryId.HasValue && slavesIds.Contains(_.CategoryId.Value)).ToListAsync())
                                kaction.CategoryId = master.Id;
                            foreach (var docAction in await context.DocumentationActionDrafts.Where(_ => _.ActionCategoryId.HasValue && slavesIds.Contains(_.ActionCategoryId.Value)).ToListAsync())
                                docAction.ActionCategoryId = master.Id;
                            foreach (var referential in slaves)
                                context.ActionCategories.DeleteObject(await context.ActionCategories.SingleAsync(_ => _.ActionCategoryId == referential.Id));
                        }
                        else if (master is Resource)
                        {
                            foreach (var kaction in await context.KActions.Where(_ => _.ResourceId.HasValue && slavesIds.Contains(_.ResourceId.Value)).ToListAsync())
                                kaction.ResourceId = master.Id;
                            foreach (var docAction in await context.DocumentationActionDrafts.Where(_ => _.ResourceId.HasValue && slavesIds.Contains(_.ResourceId.Value)).ToListAsync())
                                docAction.ResourceId = master.Id;
                            foreach (Video video in await context.Videos.Where(v => v.DefaultResourceId.HasValue && slavesIds.Contains(v.DefaultResourceId.Value)).ToListAsync())
                                video.DefaultResourceId = master.Id;
                            foreach (var referential in slaves)
                                context.Resources.DeleteObject(await context.Resources.SingleAsync(_ => _.ResourceId == referential.Id));
                        }
                        else
                            throw new ArgumentOutOfRangeException(nameof(master));
                    }

                    await context.SaveChangesAsync();
                }
            });

        /// <summary>
        /// Met à jour les FK des référentiels à valeurs multiples sur la table de liens Référentiels-Actions.
        /// </summary>
        /// <typeparam name="TReferentialAction">Le type de lien référentiels - actions.</typeparam>
        /// <param name="context">Le contexte EF.</param>
        /// <param name="actionsRefSetName">Le nom du set des liens reférentiels - actions.</param>
        /// <param name="propertyName">Le nom de la propriété de la FK.</param>
        /// <param name="master">Le référentiel maître.</param>
        /// <param name="slaves">Les référentiels esclaves.</param>
        async Task UpdateActionMultipleRefId<TReferentialAction>(KsmedEntities context, string actionsRefSetName, int refNumber, string propertyName, IActionReferential master, IActionReferential[] slaves)
            where TReferentialAction : class, IReferentialActionLink, IObjectWithChangeTracker, new()
        {
            var slavesIds = slaves.Select(s => s.Id).ToArray();
            StringBuilder slavesQuery = new StringBuilder();
            slavesQuery.Append($"SELECT value a FROM {actionsRefSetName} AS a WHERE a.{propertyName} IN {{ ");
            slavesQuery.Append(string.Join(", ", slavesIds));
            slavesQuery.Append(" }");

            string masterQuery = $"SELECT value a FROM {actionsRefSetName} AS a WHERE a.{propertyName} = {master.Id}";

            // Initialise l'OSpace
            context.CreateQuery<TReferentialAction>(actionsRefSetName);

            ObjectSet<TReferentialAction> referentialActionsSet = context.CreateObjectSet<TReferentialAction>();

            List<TReferentialAction> slavesReferentialActions = await new ObjectQuery<TReferentialAction>(slavesQuery.ToString(), context).ToListAsync();
            List<ReferentialDocumentationActionDraft> slavesReferentialDocumentationActions = await context.ReferentialDocumentationActionDrafts.Where(_ => _.RefNumber == refNumber && slavesIds.Contains(_.ReferentialId)).ToListAsync();
            List<TReferentialAction> masterReferentialActions = await new ObjectQuery<TReferentialAction>(masterQuery, context).ToListAsync();
            List<ReferentialDocumentationActionDraft> masterReferentialDocumentationActions = await context.ReferentialDocumentationActionDrafts.Where(_ => _.RefNumber == refNumber && _.ReferentialId == master.Id).ToListAsync();
            List<int> actionsReferencingMaster = masterReferentialActions.Select(ra => ra.ActionId).Distinct().ToList();
            List<int> documentationActionsReferencingMaster = masterReferentialDocumentationActions.Select(ra => ra.DocumentationActionDraftId).Distinct().ToList();

            List<TReferentialAction> newItems = new List<TReferentialAction>();
            List<ReferentialDocumentationActionDraft> newDocumentationItems = new List<ReferentialDocumentationActionDraft>();

            foreach (TReferentialAction referentialAction in slavesReferentialActions)
            {
                // Créer les nouveaux liens

                // En mode référentiels multiples, les référentiels mergés peuvent se trouver sur la même action. Dans ce cas, il ne faut pas ajouter le nouveau lien puisqu'il existe déjà.
                // Ex. : Action1 a Ref1 et Ref2. On merge Ref2 dans Ref1.
                if (!actionsReferencingMaster.Contains(referentialAction.ActionId))
                {
                    TReferentialAction newRef = ReferentialsFactory.CloneReferentialActionsLink(referentialAction, false, true);
                    newRef.ReferentialId = master.Id;
                    newItems.Add(newRef);
                }

                // Supprimer les anciens liens
                //context.DeleteObject(referentialAction);
            }
            foreach (ReferentialDocumentationActionDraft referentialDocumentationAction in slavesReferentialDocumentationActions)
            {
                // Créer les nouveaux liens

                // En mode référentiels multiples, les référentiels mergés peuvent se trouver sur la même action. Dans ce cas, il ne faut pas ajouter le nouveau lien puisqu'il existe déjà.
                // Ex. : Action1 a Ref1 et Ref2. On merge Ref2 dans Ref1.
                if (!documentationActionsReferencingMaster.Contains(referentialDocumentationAction.DocumentationActionDraftId))
                {
                    ReferentialDocumentationActionDraft newDocumentationRef = ReferentialsFactory.CloneReferentialDocumentationActionsLink(referentialDocumentationAction, true);
                    newDocumentationRef.ReferentialId = master.Id;
                    newDocumentationItems.Add(newDocumentationRef);
                }

                // Supprimer les anciens liens
                context.DeleteObject(referentialDocumentationAction);
            }

            foreach (TReferentialAction i in newItems)
                referentialActionsSet.AddObject(i);
            foreach (ReferentialDocumentationActionDraft i in newDocumentationItems)
                context.ReferentialDocumentationActionDrafts.AddObject(i);
        }

        /// <summary>
        /// Obtient un CloudFile
        /// <param name="hash">Hash généré comme identifiant</param>
        /// </summary>
        public async Task<CloudFile> GetCloudFile(string hash) =>
            await Task.Run(() =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    CloudFile cloudfile = context.CloudFiles.SingleOrDefault(c => c.Hash == hash);
                    if (cloudfile != null)
                        cloudfile.StartTracking();
                    return cloudfile;
                }
            });

        /// <summary>
        /// Sauvegarde un CloudFile
        /// </summary>
        public async Task<CloudFile> SaveCloudFile(CloudFile file) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    CloudFile cloudfile = context.CloudFiles.SingleOrDefault(c => c.Hash == file.Hash);
                    if (cloudfile == null)
                    {
                        context.CloudFiles.AddObject(new CloudFile(file.Hash, file.Extension));
                        await context.SaveChangesAsync();
                        cloudfile = context.CloudFiles.SingleOrDefault(c => c.Hash == file.Hash);
                    }
                    return cloudfile;
                }
            });

        /// <summary>
        /// Obtient les référentiels du projet spécifié.
        /// </summary>
        public async Task<List<ProjectReferential>> GetProjectReferentials(int projectId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                {
                    var project = await context.Projects
                        .SingleOrDefaultAsync(_ => _.ProjectId == projectId);
                    if (project == null)
                        return new List<ProjectReferential>();
                    var projectRefs = await context.ProjectReferentials
                        .Where(pr => pr.ProjectId == projectId)
                        .ToListAsync();
                    var refLocalization = await context.Referentials.ToDictionaryAsync(_ => _.ReferentialId, _ => _.Label);
                    projectRefs.ForEach(_ => _.Label = refLocalization[_.ReferentialId]);
                    projectRefs.AddRange(new[]
                    {
                        new ProjectReferential
                        {
                            ProjectId = projectId,
                            ReferentialId = ProcessReferentialIdentifier.CustomTextLabel,
                            IsEnabled = !string.IsNullOrEmpty(project.CustomTextLabel),
                            HasMultipleSelection = false,
                            HasQuantity = false,
                            Label = !string.IsNullOrEmpty(project.CustomTextLabel) ?project.CustomTextLabel : nameof(ProcessReferentialIdentifier.CustomTextLabel)
                        },
                        new ProjectReferential
                        {
                            ProjectId = projectId,
                            ReferentialId = ProcessReferentialIdentifier.CustomTextLabel2,
                            IsEnabled = !string.IsNullOrEmpty(project.CustomTextLabel2),
                            HasMultipleSelection = false,
                            HasQuantity = false,
                            Label = !string.IsNullOrEmpty(project.CustomTextLabel2) ? project.CustomTextLabel2 : nameof(ProcessReferentialIdentifier.CustomTextLabel2)
                        },
                        new ProjectReferential
                        {
                            ProjectId = projectId,
                            ReferentialId = ProcessReferentialIdentifier.CustomTextLabel3,
                            IsEnabled = !string.IsNullOrEmpty(project.CustomTextLabel3),
                            HasMultipleSelection = false,
                            HasQuantity = false,
                            Label = !string.IsNullOrEmpty(project.CustomTextLabel3) ? project.CustomTextLabel3 : nameof(ProcessReferentialIdentifier.CustomTextLabel3)
                        },
                        new ProjectReferential
                        {
                            ProjectId = projectId,
                            ReferentialId = ProcessReferentialIdentifier.CustomTextLabel4,
                            IsEnabled = !string.IsNullOrEmpty(project.CustomTextLabel4),
                            HasMultipleSelection = false,
                            HasQuantity = false,
                            Label = !string.IsNullOrEmpty(project.CustomTextLabel4) ? project.CustomTextLabel4 : nameof(ProcessReferentialIdentifier.CustomTextLabel4)
                        },
                        new ProjectReferential
                        {
                            ProjectId = projectId,
                            ReferentialId = ProcessReferentialIdentifier.CustomNumericLabel,
                            IsEnabled = !string.IsNullOrEmpty(project.CustomNumericLabel),
                            HasMultipleSelection = false,
                            HasQuantity = false,
                            Label = !string.IsNullOrEmpty(project.CustomNumericLabel) ? project.CustomNumericLabel : nameof(ProcessReferentialIdentifier.CustomNumericLabel)
                        },
                        new ProjectReferential
                        {
                            ProjectId = projectId,
                            ReferentialId = ProcessReferentialIdentifier.CustomNumericLabel2,
                            IsEnabled = !string.IsNullOrEmpty(project.CustomNumericLabel2),
                            HasMultipleSelection = false,
                            HasQuantity = false,
                            Label = !string.IsNullOrEmpty(project.CustomNumericLabel2) ? project.CustomNumericLabel2 : nameof(ProcessReferentialIdentifier.CustomNumericLabel2)
                        },
                        new ProjectReferential
                        {
                            ProjectId = projectId,
                            ReferentialId = ProcessReferentialIdentifier.CustomNumericLabel3,
                            IsEnabled = !string.IsNullOrEmpty(project.CustomNumericLabel3),
                            HasMultipleSelection = false,
                            HasQuantity = false,
                            Label = !string.IsNullOrEmpty(project.CustomNumericLabel3) ? project.CustomNumericLabel3 : nameof(ProcessReferentialIdentifier.CustomNumericLabel3)
                        },
                        new ProjectReferential
                        {
                            ProjectId = projectId,
                            ReferentialId = ProcessReferentialIdentifier.CustomNumericLabel4,
                            IsEnabled = !string.IsNullOrEmpty(project.CustomNumericLabel4),
                            HasMultipleSelection = false,
                            HasQuantity = false,
                            Label = !string.IsNullOrEmpty(project.CustomNumericLabel4) ? project.CustomNumericLabel4 : nameof(ProcessReferentialIdentifier.CustomNumericLabel4)
                        }
                    });
                    return projectRefs;
                }
            });

        /// <summary>
        /// Obtient si le Custom Field est utilisé dans le scénario spécifié.
        /// </summary>
        public async Task<bool> CustomFieldIsUsed(int scenarioId, ProcessReferentialIdentifier customFieldId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                {
                    switch (customFieldId)
                    {
                        case ProcessReferentialIdentifier.CustomTextLabel:
                            return await context.KActions
                                .Where(a => a.ScenarioId == scenarioId)
                                .AnyAsync(a => !string.IsNullOrEmpty(a.CustomTextValue));
                        case ProcessReferentialIdentifier.CustomTextLabel2:
                            return await context.KActions
                                .Where(a => a.ScenarioId == scenarioId)
                                .AnyAsync(a => !string.IsNullOrEmpty(a.CustomTextValue2));
                        case ProcessReferentialIdentifier.CustomTextLabel3:
                            return await context.KActions
                                .Where(a => a.ScenarioId == scenarioId)
                                .AnyAsync(a => !string.IsNullOrEmpty(a.CustomTextValue3));
                        case ProcessReferentialIdentifier.CustomTextLabel4:
                            return await context.KActions
                                .Where(a => a.ScenarioId == scenarioId)
                                .AnyAsync(a => !string.IsNullOrEmpty(a.CustomTextValue4));
                        case ProcessReferentialIdentifier.CustomNumericLabel:
                            return await context.KActions
                                .Where(a => a.ScenarioId == scenarioId)
                                .AnyAsync(a => a.CustomNumericValue != null);
                        case ProcessReferentialIdentifier.CustomNumericLabel2:
                            return await context.KActions
                                .Where(a => a.ScenarioId == scenarioId)
                                .AnyAsync(a => a.CustomNumericValue2 != null);
                        case ProcessReferentialIdentifier.CustomNumericLabel3:
                            return await context.KActions
                                .Where(a => a.ScenarioId == scenarioId)
                                .AnyAsync(a => a.CustomNumericValue3 != null);
                        case ProcessReferentialIdentifier.CustomNumericLabel4:
                            return await context.KActions
                                .Where(a => a.ScenarioId == scenarioId)
                                .AnyAsync(a => a.CustomNumericValue4 != null);
                        default:
                            return true;
                    }
                }
            });

        /// <summary>
        /// Obtient les référentiels de documentation du process spécifié.
        /// </summary>
        public async Task<List<DocumentationReferential>> GetDocumentationReferentials(int processId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                {
                    var docRefs = await context.DocumentationReferentials
                        .Where(dr => dr.ProcessId == processId)
                        .ToListAsync();
                    var docRefsLocalization = await context.DocumentationDraftLocalizations
                        .Where(_ => _.ProcessId == processId)
                        .ToDictionaryAsync(_ => _.ReferentialId, _ => _.Value);
                    docRefs.ForEach(_ => _.Label = docRefsLocalization.ContainsKey(_.ReferentialId) ? docRefsLocalization[_.ReferentialId] : _.ReferentialId.ToString());
                    return docRefs;
                }
            });

        /// <summary>
        /// Sauvegarde les référentiels de documentation.
        /// </summary>
        public async Task<List<DocumentationReferential>> SaveDocumentationReferentials(DocumentationReferential[] documentationReferentials) =>
            await Task.Run(async () =>
            {
                int processId = documentationReferentials.First().ProcessId;
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                {
                    foreach (var documentationReferential in documentationReferentials)
                    {
                        var currentDocumentationReferential = await context.DocumentationReferentials.SingleOrDefaultAsync(_ => _.ProcessId == documentationReferential.ProcessId
                            && _.ReferentialId == documentationReferential.ReferentialId);
                        var currentDocumentationReferentialLocalization = await context.DocumentationDraftLocalizations.SingleOrDefaultAsync(_ => _.ProcessId == documentationReferential.ProcessId
                            && _.ReferentialId == documentationReferential.ReferentialId);
                        if (currentDocumentationReferential == null)
                        {
                            // Add
                            context.DocumentationReferentials.AddObject(documentationReferential);
                        }
                        else
                        {
                            // Update
                            currentDocumentationReferential.IsEnabled = documentationReferential.IsEnabled;
                            currentDocumentationReferential.HasMultipleSelection = documentationReferential.HasMultipleSelection;
                            currentDocumentationReferential.HasQuantity = documentationReferential.HasQuantity;
                            currentDocumentationReferential.Label = documentationReferential.Label;
                        }
                        if (currentDocumentationReferentialLocalization == null)
                        {
                            context.DocumentationDraftLocalizations.AddObject(new DocumentationDraftLocalization
                            {
                                ProcessId = processId,
                                ReferentialId = documentationReferential.ReferentialId,
                                Value = documentationReferential.Label
                            });
                        }
                        else
                        {
                            currentDocumentationReferentialLocalization.Value = documentationReferential.Label;
                        }
                    }
                    await context.SaveChangesAsync();
                    var docRefs = await context.DocumentationReferentials
                        .Where(dr => dr.ProcessId == processId)
                        .ToListAsync();
                    var docRefsLocalizations = await context.DocumentationDraftLocalizations
                        .Where(_ => _.ProcessId == processId)
                        .ToDictionaryAsync(_ => _.ReferentialId, _ => _.Value);
                    docRefs.ForEach(_ => _.Label = docRefsLocalizations[_.ReferentialId]);
                    return docRefs;
                }
            });
    }
}