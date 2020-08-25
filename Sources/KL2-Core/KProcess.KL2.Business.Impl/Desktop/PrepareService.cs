using KProcess.Business;
using KProcess.Data;
using KProcess.KL2.APIClient;
using KProcess.KL2.Business.Impl.Helpers;
using KProcess.KL2.Business.Impl.Shared;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Business.Dtos.Prepare;
using KProcess.Ksmed.Business.Migration;
using KProcess.Ksmed.Business.Referentials;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Interfaces;
using KProcess.Ksmed.Security;
using MoreLinq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Desktop
{
    /// <summary>
    /// 
    /// </summary>
    public class PrepareService : IBusinessService, IPrepareService
    {
        readonly ITraceManager _traceManager;
        readonly ISecurityContext _securityContext;
        readonly ILocalizationManager _localizationManager;
        readonly IScenarioCloneManager _scenarioCloneManager;
        readonly INotificationService _notificationService;
        readonly IAPIHttpClient _apiHttpClient;

        public PrepareService(
            ISecurityContext securityContext,
            ILocalizationManager localizationManager,
            ITraceManager traceManager,
            IScenarioCloneManager scenarioCloneManager,
            INotificationService notificationService,
            IAPIHttpClient apiHttpClient)
        {
            _securityContext = securityContext;
            _localizationManager = localizationManager;
            _traceManager = traceManager;
            _scenarioCloneManager = scenarioCloneManager;
            _notificationService = notificationService;
            _apiHttpClient = apiHttpClient;
        }

        /// <summary>
        /// Le nombre de ProjectReferentials que doit avoir un projet.
        /// </summary>
        private const int DefaultReferentialsCount = 11;

        private class ProjectDesc
        {
            public Project Project { get; set; }
            public IList<ProjectReferential> Referentials { get; set; }
            public IEnumerable<ScenarioDesc> Scenarios { get; set; }
        }

        private class ScenarioDesc
        {
            public ScenarioDesc() { }

            public ScenarioDesc(Scenario scenario)
            {
                Id = scenario.ScenarioId;
                Label = scenario.Label;
                OriginalId = scenario.Original?.ScenarioId;
                NatureCode = scenario.NatureCode;
                StateCode = scenario.StateCode;
                IsShownInSummary = scenario.IsShownInSummary;
                CriticalPathIDuration = scenario.CriticalPathIDuration;

                Actions = scenario.Actions.Select(a => new KActionDesc(a)).ToList();

            }

            public int Id { get; set; }
            public string Label { get; set; }
            public int? OriginalId { get; set; }
            public string NatureCode { get; set; }
            public string StateCode { get; set; }
            public bool IsShownInSummary { get; set; }
            public long CriticalPathIDuration { get; set; }
            public long CriticalPathIEDuration { get; set; }
            public long BNVATotal { get; set; }
            public long NVATotal { get; set; }
            public long VATotal { get; set; }
            public long EmptyValueTotal { get; set; }
            public long TotalDurationsSum { get; set; }

            public IList<KActionDesc> Actions { get; set; }
        }

        private class KActionDesc
        {
            public KActionDesc() { }
            public KActionDesc(KAction action)
            {
                WBS = action.WBS;
                ScenarioId = action.ScenarioId;
                BuildStart = action.BuildStart;
                BuildFinish = action.BuildFinish;
                Category = action.Category;
                ResourceId = action.ResourceId;
                Resource = action.Resource;
            }

            public string WBS { get; set; }
            public int ScenarioId { get; set; }
            public long BuildStart { get; set; }
            public long BuildFinish { get; set; }
            public ActionCategory Category { get; set; }

            public int? ResourceId { get; set; }
            public Resource Resource { get; set; }
        }

        /// <summary>
        /// True si une publication existe pour le process.
        /// </summary>
        public async Task<bool> PublicationExistsForProcess(int processId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                return await context.Publications.AnyAsync(_ => _.ProcessId == processId);
            }
        }

        /// <summary>
        /// True si une publication existe pour le process en sync.
        /// </summary>
        public bool PublicationExistsForProcessSync(int processId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                return context.Publications.Any(_ => _.ProcessId == processId);
            }
        }

        public async Task<Publication> GetPublication(Guid publicationId) =>
            await Task.Run(async () =>
            {
                try
                {
                    using (var context = ContextFactory.GetNewContext())
                    {
                        Publication result = await context.Publications
                            .Include(nameof(Publication.Localizations))
                            .Include(nameof(Publication.Project))
                            .Include($"{nameof(Publication.Project)}.{nameof(Project.Process)}")
                            .Include($"{nameof(Publication.Project)}.{nameof(Project.Process)}.{nameof(Procedure.InspectionSchedules)}")
                            .Include($"{nameof(Publication.Project)}.{nameof(Project.Process)}.{nameof(Procedure.InspectionSchedules)}.{nameof(InspectionSchedule.Timeslot)}")
                            .Include(nameof(Publication.Readers))
                            .Include(nameof(Publication.Trainings))
                            .Include(nameof(Publication.Qualifications))
                            .Include($"{nameof(Publication.Qualifications)}.{nameof(PublishedAction.QualificationSteps)}")
                            .Include(nameof(Publication.Inspections))
                            .Include($"{nameof(Publication.Inspections)}.{nameof(Inspection.Anomalies)}")
                            .Include($"{nameof(Publication.Inspections)}.{nameof(Inspection.Audits)}")
                            .Include($"{nameof(Publication.Qualifications)}.{nameof(PublishedAction.QualificationSteps)}.{nameof(QualificationStep.User)}")

                            .SingleOrDefaultAsync(_ => _.PublicationId == publicationId);
                        if (result != null)
                        {
                            result.PublishedActions = new TrackableCollection<PublishedAction>(await context.PublishedActions.Where(_ => _.PublicationId == result.PublicationId)
                                .Include(nameof(PublishedAction.PublishedActionCategory))
                                .Include(nameof(PublishedAction.PublishedResource))
                                .Include(nameof(PublishedAction.Predecessors))
                                .Include(nameof(PublishedAction.Successors))
                                .Include(nameof(PublishedAction.PublishedReferentialActions))
                                .Include($"{nameof(PublishedAction.PublishedReferentialActions)}.{nameof(PublishedReferentialAction.PublishedReferential)}")
                                .Include($"{nameof(PublishedAction.PublishedReferentialActions)}.{nameof(PublishedReferentialAction.PublishedReferential)}.{nameof(PublishedReferential.File)}")
                                .Include(nameof(PublishedAction.ValidationTrainings))
                                .Include($"{nameof(PublishedAction.ValidationTrainings)}.{nameof(ValidationTraining.User)}")
                                .Include(nameof(PublishedAction.QualificationSteps))
                                .Include($"{nameof(PublishedAction.QualificationSteps)}.{nameof(QualificationStep.User)}")
                                .Include(nameof(PublishedAction.LinkedPublication))
                                .Include($"{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.Localizations)}")
                                .Include($"{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.Project)}")
                                .Include($"{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.Project)}.{nameof(Project.Process)}")
                                .Include($"{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.Project)}.{nameof(Project.Process)}.{nameof(Procedure.InspectionSchedules)}")
                                .Include($"{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.Project)}.{nameof(Project.Process)}.{nameof(Procedure.InspectionSchedules)}.{nameof(InspectionSchedule.Timeslot)}")
                                .Include($"{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.Trainings)}")
                                .Include($"{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.Qualifications)}")
                                .Include($"{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.Qualifications)}.{nameof(Qualification.QualificationSteps)}")
                                .Include($"{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.Qualifications)}.{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.User)}")
                                .Include($"{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.PublishedActions)}")
                                .Include($"{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.PublishedActions)}.{nameof(PublishedAction.ValidationTrainings)}")
                                .Include($"{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.PublishedActions)}.{nameof(PublishedAction.ValidationTrainings)}.{nameof(ValidationTraining.User)}")
                                .Include($"{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.PublishedActions)}.{nameof(PublishedAction.QualificationSteps)}")
                                .Include($"{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.PublishedActions)}.{nameof(PublishedAction.QualificationSteps)}.{nameof(ValidationTraining.User)}")
                                .OrderBy(_ => _.WBS)
                                .ToArrayAsync());
                        }
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    _traceManager.TraceError(ex, $"ERREUR lors de l'appel à la fonction {nameof(GetPublication)}({publicationId})");
                    return null;
                }
            });

        public async Task<Publication> GetTrainingPublication(Guid evaluationPublicationId) =>
            await Task.Run(async () =>
            {
                try
                {
                    Guid trainingPublicationId = Guid.Empty;
                    using (var context = ContextFactory.GetNewContext())
                    {
                        var publishedDate = await context.Publications
                            .Where(ep => ep.PublicationId == evaluationPublicationId)
                            .Select(ep => ep.PublishedDate)
                            .SingleAsync();
                        trainingPublicationId = (await context.Publications
                            .Where(tp => tp.PublishMode == PublishModeEnum.Formation)
                            .Select(tp => new { tp.PublicationId, tp.PublishedDate })
                            .ToListAsync())
                            .Single(tpKvp => tpKvp.PublishedDate == publishedDate)
                            .PublicationId;
                    }
                    return await GetPublication(trainingPublicationId);
                }
                catch (Exception ex)
                {
                    _traceManager.TraceError(ex, $"ERREUR lors de l'appel à la fonction {nameof(GetTrainingPublication)}({evaluationPublicationId})");
                    return null;
                }
            });

        public async Task<Publication> GetEvaluationPublication(Guid trainingPublicationId) =>
            await Task.Run(async () =>
            {
                try
                {
                    Guid evaluationPublicationId = Guid.Empty;
                    using (var context = ContextFactory.GetNewContext())
                    {
                        var publishedDate = await context.Publications
                            .Where(tp => tp.PublicationId == trainingPublicationId)
                            .Select(tp => tp.PublishedDate)
                            .SingleAsync();
                        evaluationPublicationId = (await context.Publications
                            .Where(ep => ep.PublishMode == PublishModeEnum.Evaluation)
                            .Select(ep => new { ep.PublicationId, ep.PublishedDate })
                            .ToListAsync())
                            .Single(epKvp => epKvp.PublishedDate == publishedDate)
                            .PublicationId;
                    }
                    return await GetPublication(evaluationPublicationId);
                }
                catch (Exception ex)
                {
                    _traceManager.TraceError(ex, $"ERREUR lors de l'appel à la fonction {nameof(GetEvaluationPublication)}({trainingPublicationId})");
                    return null;
                }
            });

        public async Task<IEnumerable<Publication>> GetTrainingPublications(Guid[] evaluationPublicationIds) =>
            await Task.Run(async () =>
            {
                try
                {
                    List<Publication> result = new List<Publication>();
                    foreach (var evaluationPublicationId in evaluationPublicationIds)
                        result.Add(await GetTrainingPublication(evaluationPublicationId));
                    return result;
                }
                catch (Exception ex)
                {
                    _traceManager.TraceError(ex, $"ERREUR lors de l'appel à la fonction {nameof(GetTrainingPublications)}({string.Join(", ", evaluationPublicationIds)})");
                    return null;
                }
            });

        public async Task<IEnumerable<Publication>> GetEvaluationPublications(Guid[] trainingPublicationIds) =>
            await Task.Run(async () =>
            {
                try
                {
                    List<Publication> result = new List<Publication>();
                    foreach (var trainingPublicationId in trainingPublicationIds)
                        result.Add(await GetEvaluationPublication(trainingPublicationId));
                    return result;
                }
                catch (Exception ex)
                {
                    _traceManager.TraceError(ex, $"ERREUR lors de l'appel à la fonction {nameof(GetEvaluationPublications)}({string.Join(", ", trainingPublicationIds)})");
                    return null;
                }
            });

        public async Task<Publication> GetLightPublication(Guid publicationId) =>
         await Task.Run(async () =>
         {
             try
             {
                 using (var context = ContextFactory.GetNewContext())
                 {
                     Publication result = await context.Publications
                         .Include(nameof(Publication.Localizations))
                         .Include(nameof(Publication.Project))
                         .Include($"{nameof(Publication.Project)}.{nameof(Project.Process)}")
                         .SingleOrDefaultAsync(_ => _.PublicationId == publicationId);
                     if (result != null)
                     {
                         result.PublishedActions = new TrackableCollection<PublishedAction>(await context.PublishedActions.Where(_ => _.PublicationId == result.PublicationId)
                                .Include(nameof(PublishedAction.PublishedActionCategory))
                                .Include(nameof(PublishedAction.PublishedResource))
                                .Include(nameof(PublishedAction.Predecessors))
                                .Include(nameof(PublishedAction.Successors))
                                .Include(nameof(PublishedAction.PublishedReferentialActions))
                                .Include($"{nameof(PublishedAction.PublishedReferentialActions)}.{nameof(PublishedReferentialAction.PublishedReferential)}")
                                .Include($"{nameof(PublishedAction.PublishedReferentialActions)}.{nameof(PublishedReferentialAction.PublishedReferential)}.{nameof(PublishedReferential.File)}")
                                            .OrderBy(_ => _.WBS)
                                .ToArrayAsync());
                     }
                     return result;
                 }
             }
             catch (Exception ex)
             {
                 _traceManager.TraceError(ex, $"ERREUR lors de l'appel à la fonction {nameof(GetLightPublication)}({publicationId})");
                 return null;
             }
         });

        /// <summary>
        /// Obtient la dernière publication d'un process.
        /// </summary>
        public Task<Publication> GetLastPublication(int processId) =>
            GetLastPublicationWithFunc(_ => _.ProcessId == processId);

        /// <summary>
        /// Obtient la dernière publication d'un process. with filter by publish mode
        /// </summary>
        public Task<Publication> GetLastPublicationFiltered(int processId, int publishModeFilter)
        {
            PublishModeEnum filter = (PublishModeEnum)publishModeFilter;
            return GetLastPublicationWithFunc(_ => _.ProcessId == processId && _.PublishMode == filter);
        }

        async Task<Publication> GetLastPublicationWithFunc(Expression<Func<Publication, bool>> predicate)
        {
            try
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    Guid? lastPublicationId = (await context.Publications
                        .Where(predicate)
                        .Select(_ => new { _.PublicationId, _.PublishedDate })
                        .OrderByDescending(_ => _.PublishedDate)
                        .FirstOrDefaultAsync())?.PublicationId;
                    if (!lastPublicationId.HasValue)
                        return null;
                    Publication publication = await context.Publications
                        .Include(nameof(Publication.Localizations))
                        .Include(nameof(Publication.Project))
                        .Include($"{nameof(Publication.Project)}.{nameof(Project.Objective)}")
                        .Include($"{nameof(Publication.Project)}.{nameof(Project.Process)}")
                        .Include($"{nameof(Publication.Project)}.{nameof(Project.Process)}.{nameof(Procedure.InspectionSchedules)}")
                        .Include($"{nameof(Publication.Project)}.{nameof(Project.Process)}.{nameof(Procedure.InspectionSchedules)}.{nameof(InspectionSchedule.Timeslot)}")
                        .Include(nameof(Publication.Readers))
                        .Include(nameof(Publication.Trainings))
                        .Include(nameof(Publication.Qualifications))
                        .Include(nameof(Publication.Inspections))
                        .Include(nameof(Publication.Process))
                        .SingleAsync(_ => _.PublicationId == lastPublicationId.Value);
                    foreach (var _ in publication.Trainings)
                    {
                        _.ValidationTrainings = new TrackableCollection<ValidationTraining>(await context.ValidationTrainings
                            .Include($"{nameof(ValidationTraining.User)}")
                            .Include($"{nameof(ValidationTraining.User)}.{nameof(User.Roles)}")
                            .Where(t => t.TrainingId == _.TrainingId)
                            .ToListAsync());
                    }
                    foreach (var _ in publication.Qualifications)
                    {
                        _.QualificationSteps = new TrackableCollection<QualificationStep>(await context.QualificationSteps
                            .Include($"{nameof(QualificationStep.User)}")
                            .Include($"{nameof(QualificationStep.User)}.{nameof(User.Roles)}")
                            .Where(q => q.QualificationId == _.QualificationId)
                            .ToListAsync());
                    }
                    foreach (var _ in publication.Inspections)
                    {
                        _.InspectionSteps = new TrackableCollection<InspectionStep>(await context.InspectionSteps
                            .Include($"{nameof(InspectionStep.Inspector)}")
                            .Include($"{nameof(InspectionStep.Inspector)}.{nameof(User.Roles)}")
                            .Include($"{nameof(InspectionStep.Anomaly)}")
                            .Include($"{nameof(InspectionStep.Anomaly)}.{nameof(Anomaly.Inspector)}")
                            .Include($"{nameof(InspectionStep.Anomaly)}.{nameof(Anomaly.Inspector)}.{nameof(User.Roles)}")
                            .Where(i => i.InspectionId == _.InspectionId)
                            .ToListAsync());
                        _.Anomalies = new TrackableCollection<Anomaly>(await context.Anomalies
                            .Include($"{nameof(Anomaly.Inspector)}")
                            .Include($"{nameof(Anomaly.Inspector)}.{nameof(User.Roles)}")
                            .Where(i => i.InspectionId == _.InspectionId)
                            .ToListAsync());
                        _.Audits = new TrackableCollection<Audit>(await context.Audits
                            .Include($"{nameof(Audit.Auditor)}")
                            .Include($"{nameof(Audit.Auditor)}.{nameof(User.Roles)}")
                            .Include($"{nameof(Audit.Survey)}")
                            .Include($"{nameof(Audit.Survey)}.{nameof(Survey.SurveyItems)}")
                            .Include($"{nameof(Audit.AuditItems)}")
                            .Where(i => i.InspectionId == _.InspectionId)
                            .ToListAsync());
                    }
                    publication.PublishedActions = new TrackableCollection<PublishedAction>((await context.PublishedActions
                        .Include(nameof(PublishedAction.Thumbnail))
                        .Include(nameof(PublishedAction.CutVideo))
                        .Include(nameof(PublishedAction.PublishedActionCategory))
                        .Include(nameof(PublishedAction.PublishedResource))
                        .Include($"{nameof(PublishedAction.PublishedResource)}.{nameof(PublishedResource.File)}")
                        .Include(nameof(PublishedAction.Skill))
                        .Include(nameof(PublishedAction.Predecessors))
                        .Include(nameof(PublishedAction.Successors))
                        .Include(nameof(PublishedAction.PublishedReferentialActions))
                        .Include(nameof(PublishedAction.ValidationTrainings))
                        .Include(nameof(PublishedAction.QualificationSteps))
                        .Include(nameof(PublishedAction.LinkedPublication))
                        .Where(_ => _.PublicationId == publication.PublicationId)
                        .ToArrayAsync())
                        .OrderBy(_ => _, new WBSComparer()));
                    foreach (var _ in publication.PublishedActions)
                    {
                        foreach (var p in _.PublishedReferentialActions)
                        {
                            p.PublishedReferential = await context.PublishedReferentials
                                .Include($"{nameof(PublishedReferential.File)}")
                                .SingleAsync(pr => pr.PublishedReferentialId == p.PublishedReferentialId);
                        }
                        foreach (var v in _.ValidationTrainings)
                        {
                            v.User = await context.Users
                                .Include($"{nameof(User.Roles)}")
                                .SingleAsync(u => u.UserId == v.TrainerId);
                        }
                        foreach (var q in _.QualificationSteps)
                        {
                            if (q.QualificationReasonId != null)
                                q.QualificationReason = await context.QualificationReasons
                                    .SingleAsync(qr => qr.Id == q.QualificationReasonId);
                            q.User = await context.Users
                                .Include($"{nameof(User.Roles)}")
                                .SingleAsync(u => u.UserId == q.QualifierId);
                        }

                        if (_.LinkedPublication != null)
                        {
                            _.LinkedPublication.Project = await context.Projects
                                .Include($"{nameof(Project.Objective)}")
                                .Include($"{nameof(Project.Process)}")
                                .Include($"{nameof(Project.Process)}.{nameof(Procedure.InspectionSchedules)}")
                                .Include($"{nameof(Project.Process)}.{nameof(Procedure.InspectionSchedules)}.{nameof(InspectionSchedule.Timeslot)}")
                                .SingleAsync(p => p.ProjectId == _.LinkedPublication.ProjectId);
                            _.LinkedPublication.Localizations = new TrackableCollection<PublicationLocalization>(await context.PublicationLocalizations
                                    .Where(l => l.PublicationId == _.LinkedPublicationId)
                                    .ToListAsync());
                            _.LinkedPublication.Readers = new TrackableCollection<UserReadPublication>(await context.UserReadPublications
                                .Where(ur => ur.PublicationId == _.LinkedPublicationId)
                                .ToListAsync());
                            _.LinkedPublication.Trainings = new TrackableCollection<Training>(await context.Trainings
                                .Where(t => t.PublicationId == _.LinkedPublicationId)
                                .ToListAsync());
                            _.LinkedPublication.Qualifications = new TrackableCollection<Qualification>(await context.Qualifications
                                .Include($"{nameof(Qualification.QualificationSteps)}")
                                .Include($"{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.QualificationReason)}")
                                .Include($"{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.User)}")
                                .Include($"{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.User)}.{nameof(User.Roles)}")
                                .Where(q => q.PublicationId == _.LinkedPublicationId)
                                .ToListAsync());
                            _.LinkedPublication.PublishedActions = new TrackableCollection<PublishedAction>((await context.PublishedActions
                                .Include($"{nameof(PublishedAction.Thumbnail)}")
                                .Include($"{nameof(PublishedAction.CutVideo)}")
                                .Include($"{nameof(PublishedAction.PublishedActionCategory)}")
                                .Include($"{nameof(PublishedAction.PublishedResource)}")
                                .Include($"{nameof(PublishedAction.PublishedResource)}.{nameof(PublishedResource.File)}")
                                .Include($"{nameof(PublishedAction.PublishedReferentialActions)}")
                                .Include($"{nameof(PublishedAction.PublishedReferentialActions)}.{nameof(PublishedReferentialAction.PublishedReferential)}")
                                .Include($"{nameof(PublishedAction.PublishedReferentialActions)}.{nameof(PublishedReferentialAction.PublishedReferential)}.{nameof(PublishedReferential.File)}")
                                .Include($"{nameof(PublishedAction.ValidationTrainings)}")
                                .Include($"{nameof(PublishedAction.ValidationTrainings)}.{nameof(ValidationTraining.User)}")
                                .Include($"{nameof(PublishedAction.ValidationTrainings)}.{nameof(ValidationTraining.User)}.{nameof(User.Roles)}")
                                .Include($"{nameof(PublishedAction.QualificationSteps)}")
                                .Include($"{nameof(PublishedAction.QualificationSteps)}.{nameof(ValidationTraining.User)}")
                                .Include($"{nameof(PublishedAction.QualificationSteps)}.{nameof(ValidationTraining.User)}.{nameof(User.Roles)}")
                                .Where(p => p.PublicationId == _.LinkedPublicationId)
                                .ToListAsync())
                                .OrderBy(p => p, new WBSComparer()));
                            _.LinkedPublication.Inspections = new TrackableCollection<Inspection>(await context.Inspections
                                .Include($"{nameof(Inspection.InspectionSteps)}")
                                .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}")
                                .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}.{nameof(User.Roles)}")
                                .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Anomaly)}")
                                .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Anomaly)}.{nameof(Anomaly.Inspector)}")
                                .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Anomaly)}.{nameof(Anomaly.Inspector)}.{nameof(User.Roles)}")
                                .Include($"{nameof(Inspection.Anomalies)}")
                                .Include($"{nameof(Inspection.Anomalies)}.{nameof(Anomaly.Inspector)}")
                                .Include($"{nameof(Inspection.Anomalies)}.{nameof(Anomaly.Inspector)}.{nameof(User.Roles)}")
                                .Include($"{nameof(Inspection.Audits)}")
                                .Include($"{nameof(Inspection.Audits)}.{nameof(Audit.Auditor)}")
                                .Include($"{nameof(Inspection.Audits)}.{nameof(Audit.Auditor)}.{nameof(User.Roles)}")
                                .Include($"{nameof(Inspection.Audits)}.{nameof(Audit.Survey)}")
                                .Include($"{nameof(Inspection.Audits)}.{nameof(Audit.Survey)}.{nameof(Survey.SurveyItems)}")
                                .Include($"{nameof(Inspection.Audits)}.{nameof(Audit.AuditItems)}")
                                .Where(i => i.PublicationId == _.LinkedPublicationId)
                                .ToListAsync());
                        }
                    }
                    return publication;
                }
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, $"ERREUR lors de l'appel à la fonction {nameof(GetLastPublication)}");
            }

            return null;
        }

        /// <summary>
        /// Obtient la publication d'un process avec un audit ouvert pour l'utilisateur donné.
        /// </summary>
        public async Task<Publication> GetPublicationToAudit(int auditorId)
        {
            int processId = 0;
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                List<Audit> openedAudits = await context.Audits
                    .Where(_ => !_.IsDeleted && _.AuditorId == auditorId && _.EndDate == null)
                    .ToListAsync();
                Audit lastOpenedAudit = openedAudits.MaxBy(_ => _.StartDate).FirstOrDefault();
                if (lastOpenedAudit == null)
                    return null;
                processId = (await context.Inspections
                        .Include(nameof(Inspection.Publication))
                        .SingleAsync(i => i.InspectionId == lastOpenedAudit.InspectionId))
                    .Publication.ProcessId;
            }

            return await GetLastPublicationFiltered(processId, (int)PublishModeEnum.Inspection);
        }

        /// <summary>
        /// Obtient les dernières publications d'un process.
        /// </summary>
        public async Task<IEnumerable<Publication>> GetLastPublications(int publishModeFilter)
        {
            var lastPublications = new List<Publication>();
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                Publication[] publications = await context.Publications
                    .Include(nameof(Publication.Localizations))
                    .Include(nameof(Publication.Project))
                    .Include($"{nameof(Publication.Project)}.{nameof(Project.Process)}")
                    .Include(nameof(Publication.Readers))
                    .Include(nameof(Publication.Trainings))
                    .Include($"{nameof(Publication.Trainings)}.{nameof(Training.ValidationTrainings)}")
                    .Include(nameof(Publication.Qualifications))
                    .Include(nameof(Publication.Inspections))
                    .Include($"{nameof(Publication.Inspections)}.{nameof(Inspection.InspectionSteps)}")
                    .Include($"{nameof(Publication.Inspections)}.{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}")
                    .Include($"{nameof(Publication.Inspections)}.{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}.{nameof(User.Teams)}")
                    .Include($"{nameof(Publication.Qualifications)}.{nameof(Qualification.User)}")
                    .Include($"{nameof(Publication.Qualifications)}.{nameof(Qualification.User)}.{nameof(User.Teams)}")
                    .Include($"{nameof(Publication.Qualifications)}.{nameof(Qualification.QualificationSteps)}")
                    .Include($"{nameof(Publication.Qualifications)}.{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.PublishedAction)}")
                    .Include($"{nameof(Publication.Qualifications)}.{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.User)}")
                    .Include(nameof(Publication.Process))
                    .Include($"{nameof(Publication.Process)}.{nameof(Procedure.ProjectDir)}")
                    .Include($"{nameof(Publication.Process)}.{nameof(Procedure.ProjectDir)}.{nameof(ProjectDir.Parent)}")
                    .OrderBy(parameters => parameters.Process.ProjectDirId)
                    .ToArrayAsync();

                ProjectDir[] projectDirs = await context.ProjectDirs.ToArrayAsync();

                var processIds = publications.Select(u => u.ProcessId).Distinct();

                foreach (var processId in processIds)
                {
                    if (publications.Length == 0)
                        continue;

                    Publication lastPublication = publications
                        .Where(_ => _.ProcessId == processId
                                    && (int)_.PublishMode == publishModeFilter)
                        .MaxBy(_ => _.PublishedDate)
                        .FirstOrDefault();
                    if (lastPublication == null)
                        continue;
                    lastPublication.Process.ProjectDir = projectDirs.FirstOrDefault(d => d.Id == lastPublication.Process.ProjectDirId);
                    var currentMajorVersion  = new Version(lastPublication.Version).Major;
                    if (currentMajorVersion > 1)
                    {
                        lastPublication.LastMajorReadDates = publications.Where(p => p.ProcessId == lastPublication.ProcessId && Version.Parse(p.Version).Major < currentMajorVersion)
                            .SelectMany(p => p.Readers).Where(r => r.ReadDate != null).GroupBy(r => r.UserId)
                            .ToDictionary(_ => _.Key, _ => _.MaxBy(r => r.ReadDate).First().ReadDate.Value);
                        lastPublication.LastMajorAncestorReads = publications.Where(p => p.ProcessId == lastPublication.ProcessId && Version.Parse(p.Version).Major < currentMajorVersion)
                            .SelectMany(p => p.Readers).Where(q => q.ReadDate != null && q.PreviousVersionPublication == null).ToList();
                        lastPublication.LastMajorTrainedDates = publications.Where(p => p.ProcessId == lastPublication.ProcessId && Version.Parse(p.Version).Major < currentMajorVersion)
                            .SelectMany(p => p.Trainings).Where(t => t.EndDate != null && !t.IsDeleted).GroupBy(t => t.UserId)
                            .ToDictionary(_ => _.Key, _ => _.MaxBy(t => t.EndDate).First().EndDate.Value);
                        lastPublication.LastMajorAncestorTrainings = publications.Where(p => p.ProcessId == lastPublication.ProcessId && Version.Parse(p.Version).Major < currentMajorVersion)
                            .SelectMany(p => p.Trainings).Where(q => q.EndDate != null && !q.IsDeleted && q.PreviousVersionTraining == null).ToList();
                        lastPublication.LastMajorQualifiedDates = publications.Where(p => p.ProcessId == lastPublication.ProcessId && Version.Parse(p.Version).Major < currentMajorVersion)
                            .SelectMany(p => p.Qualifications).Where(q => q.EndDate != null && q.IsQualified == true && !q.IsDeleted).GroupBy(q => q.UserId)
                            .ToDictionary(_ => _.Key, _ => _.MaxBy(q => q.EndDate).First().EndDate.Value);
                        lastPublication.LastMajorAncestorQualifications = publications.Where(p => p.ProcessId == lastPublication.ProcessId && Version.Parse(p.Version).Major < currentMajorVersion)
                            .SelectMany(p => p.Qualifications).Where(q => q.EndDate != null && q.IsQualified == true && !q.IsDeleted && q.PreviousVersionQualification == null).ToList();
                    }
                    lastPublication.PublishedActions = new TrackableCollection<PublishedAction>(await context.PublishedActions.Where(_ => _.PublicationId == lastPublication.PublicationId)
                                .Include(nameof(PublishedAction.PublishedActionCategory))
                                .Include(nameof(PublishedAction.PublishedResource))
                                .Include(nameof(PublishedAction.Predecessors))
                                .Include(nameof(PublishedAction.Successors))
                                .Include(nameof(PublishedAction.PublishedReferentialActions))
                                .Include($"{nameof(PublishedAction.PublishedReferentialActions)}.{nameof(PublishedReferentialAction.PublishedReferential)}")
                                .Include($"{nameof(PublishedAction.PublishedReferentialActions)}.{nameof(PublishedReferentialAction.PublishedReferential)}.{nameof(PublishedReferential.File)}")
                                .Include(nameof(PublishedAction.ValidationTrainings))
                                .Include($"{nameof(PublishedAction.ValidationTrainings)}.{nameof(ValidationTraining.User)}")
                                .OrderBy(_ => _.WBS)
                                .ToArrayAsync());
                    lastPublications.Add(lastPublication);
                }
                return lastPublications;
            }
        }

        public async Task<IEnumerable<Publication>> GetLastPublicationsForFilter(int publishModeFilter)
        {
            var lastPublications = new List<Publication>();
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                Publication[] publications = await context.Publications
                    .Include(nameof(Publication.Localizations))
                    .Include(nameof(Publication.Process))
                    .OrderBy(parameters => parameters.Process.Label)
                    .ToArrayAsync();

                var processIds = publications.Select(u => u.ProcessId).Distinct();

                foreach (var processId in processIds)
                {
                    if (publications.Length == 0)
                        continue;

                    Publication lastPublication = publications
                        .Where(_ => _.ProcessId == processId
                                    && (int)_.PublishMode == publishModeFilter)
                        .MaxBy(_ => _.PublishedDate)
                        .FirstOrDefault();
                    if (lastPublication == null)
                        continue;
                    lastPublications.Add(lastPublication);
                }
                return lastPublications;
            }
        }

        /// <summary>
        /// Obtient les dernières publications d'un process ainsi que toutes les publications de la version courante
        /// </summary>
        public async Task<IEnumerable<Publication>> GetLastSameMajorPublications(int publishModeFilter)
        {
            var lastPublications = new List<Publication>();
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                Publication[] publications = await context.Publications
                    .Include(nameof(Publication.Localizations))
                    .Include(nameof(Publication.Project))
                    .Include($"{nameof(Publication.Project)}.{nameof(Project.Process)}")
                    .Include(nameof(Publication.Readers))
                    .Include(nameof(Publication.Trainings))
                    .Include($"{nameof(Publication.Trainings)}.{nameof(Training.ValidationTrainings)}")
                    .Include(nameof(Publication.Qualifications))
                    .Include(nameof(Publication.Inspections))
                    .Include($"{nameof(Publication.Inspections)}.{nameof(Inspection.InspectionSteps)}")
                    .Include($"{nameof(Publication.Qualifications)}.{nameof(Qualification.User)}")
                    .Include($"{nameof(Publication.Qualifications)}.{nameof(Qualification.User)}.{nameof(User.Teams)}")
                    .Include($"{nameof(Publication.Qualifications)}.{nameof(Qualification.QualificationSteps)}")
                    .Include($"{nameof(Publication.Qualifications)}.{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.PublishedAction)}")
                    .Include($"{nameof(Publication.Qualifications)}.{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.User)}")
                    .Include(nameof(Publication.Process))
                    .Include($"{nameof(Publication.Process)}.{nameof(Procedure.ProjectDir)}")
                    .Include($"{nameof(Publication.Process)}.{nameof(Procedure.ProjectDir)}.{nameof(ProjectDir.Parent)}")
                    .OrderBy(parameters => parameters.Process.ProjectDirId)
                    .ToArrayAsync();

                ProjectDir[] projectDirs = await context.ProjectDirs.ToArrayAsync();

                var processIds = publications.Select(u => u.ProcessId).Distinct();

                foreach (var processId in processIds)
                {
                    if (publications.Length == 0)
                        continue;

                    Publication lastPublication = publications
                        .Where(_ => _.ProcessId == processId
                                    && (int)_.PublishMode == publishModeFilter)
                        .MaxBy(_ => _.PublishedDate)
                        .FirstOrDefault();
                    if (lastPublication == null)
                        continue;

                    var versionNumber = new Version(lastPublication.Version);
                    var lastPublicationsIncludeMinorVersions = publications
                        .Where(_ => _.ProcessId == processId && (int)_.PublishMode == publishModeFilter && new Version(_.Version).Major == versionNumber.Major);

                    lastPublicationsIncludeMinorVersions.ForEach(u => u.Process.ProjectDir = projectDirs.FirstOrDefault(d => d.Id == lastPublication.Process.ProjectDirId));
                    lastPublications.AddRange(lastPublicationsIncludeMinorVersions);
                }
                return lastPublications;
            }
        }

        /// <summary>
        /// Get last publication per major
        /// </summary>
        public async Task<IEnumerable<Publication>> GetLastPublicationsPerMajor(int publishModeFilter)
        {
            var lastPublications = new List<Publication>();
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                Publication[] publications = await context.Publications
                    .Include(nameof(Publication.Localizations))
                    .Include(nameof(Publication.Project))
                    .Include($"{nameof(Publication.Project)}.{nameof(Project.Process)}")
                    .Include(nameof(Publication.Readers))
                    .Include(nameof(Publication.Trainings))
                    .Include($"{nameof(Publication.Trainings)}.{nameof(Training.ValidationTrainings)}")
                    .Include(nameof(Publication.Qualifications))
                    .Include(nameof(Publication.Inspections))
                    .Include($"{nameof(Publication.Inspections)}.{nameof(Inspection.InspectionSteps)}")
                    .Include($"{nameof(Publication.Qualifications)}.{nameof(Qualification.User)}")
                    .Include($"{nameof(Publication.Qualifications)}.{nameof(Qualification.User)}.{nameof(User.Teams)}")
                    .Include($"{nameof(Publication.Qualifications)}.{nameof(Qualification.QualificationSteps)}")
                    .Include($"{nameof(Publication.Qualifications)}.{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.PublishedAction)}")
                    .Include($"{nameof(Publication.Qualifications)}.{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.User)}")
                    .Include(nameof(Publication.Process))
                    .Include($"{nameof(Publication.Process)}.{nameof(Procedure.ProjectDir)}")
                    .Include($"{nameof(Publication.Process)}.{nameof(Procedure.ProjectDir)}.{nameof(ProjectDir.Parent)}")
                    .OrderBy(parameters => parameters.Process.ProjectDirId)
                    .ToArrayAsync();

                ProjectDir[] projectDirs = await context.ProjectDirs.ToArrayAsync();

                var processIds = publications.Select(u => u.ProcessId).Distinct();

                foreach (var processId in processIds)
                {
                    if (publications.Length == 0)
                        continue;

                    var lastPublicationsPerMajor = publications
                        .Where(_ => _.ProcessId == processId
                                    && (int)_.PublishMode == publishModeFilter).GroupBy(p => new Version(p.Version).Major);
                    foreach (var lastPublicationsMajor in lastPublicationsPerMajor)
                    {
                        Publication lastPublication = lastPublicationsMajor
                            .MaxBy(_ => _.PublishedDate)
                            .FirstOrDefault();
                        if (lastPublication == null)
                            continue;

                        var versionNumber = new Version(lastPublication.Version);
                        var lastPublicationsIncludeMinorVersions = publications
                            .Where(_ => _.ProcessId == processId && (int)_.PublishMode == publishModeFilter && new Version(_.Version).Major == versionNumber.Major);

                        lastPublicationsIncludeMinorVersions.ForEach(u => u.Process.ProjectDir = projectDirs.FirstOrDefault(d => d.Id == lastPublication.Process.ProjectDirId));
                        lastPublications.AddRange(lastPublicationsIncludeMinorVersions);
                    }

                    
                }
                return lastPublications;
            }
        }


        /// <summary>
        /// Obtient les dernières publications d'un process.
        /// </summary>
        public async Task<IEnumerable<Publication>> GetLastPublicationSkills()
        {
            var lastPublications = new List<Publication>();
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                Publication[] publications = await context.Publications
                    .Include(nameof(Publication.Qualifications))
                    .Include(nameof(Publication.Process))
                    .ToArrayAsync();

                var processIds = publications.Select(u => u.ProcessId).Distinct();

                foreach (var processId in processIds)
                {
                    if (publications.Length == 0)
                        continue;

                    Publication lastPublication = publications
                        .Where(_ => _.ProcessId == processId
                                    && _.PublishMode == PublishModeEnum.Evaluation)
                        .MaxBy(_ => _.PublishedDate)
                        .FirstOrDefault();

                    //check if lastPublication is skill or not
                    if (lastPublication?.IsSkill != true)
                        continue;

                    var currentMajorVersion = new Version(lastPublication.Version).Major;
                    if (currentMajorVersion > 1)
                    {
                        lastPublication.LastMajorQualifiedDates = publications.Where(p => p.ProcessId == lastPublication.ProcessId && Version.Parse(p.Version).Major < currentMajorVersion)
                            .SelectMany(p => p.Qualifications).Where(q => q.EndDate != null && q.IsQualified == true && !q.IsDeleted).GroupBy(q => q.UserId)
                            .ToDictionary(_ => _.Key, _ => _.MaxBy(q => q.EndDate).First().EndDate.Value);
                        lastPublication.LastMajorAncestorQualifications = publications.Where(p => p.ProcessId == lastPublication.ProcessId && Version.Parse(p.Version).Major < currentMajorVersion)
                            .SelectMany(p => p.Qualifications).Where(q => q.EndDate != null && q.IsQualified == true && !q.IsDeleted && q.PreviousVersionQualification == null).ToList();
                    }
                    lastPublications.Add(lastPublication);
                }
                return lastPublications;
            }
        }

        /// <summary>
        /// Get all audits or specific audit
        /// </summary>
        public async Task<IEnumerable<Audit>> GetAudits(int? auditId = null)
        {
            IEnumerable<Audit> result = null;
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                if (auditId == null)
                {
                    var inspectionPublications = await context.Publications
                        .Include(nameof(Publication.Process))
                        .Include(nameof(Publication.Inspections))
                        .Include($"{nameof(Publication.Inspections)}.{nameof(Inspection.InspectionSteps)}")
                        .Include($"{nameof(Publication.Inspections)}.{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}")
                        .Include($"{nameof(Publication.Inspections)}.{nameof(Inspection.Anomalies)}")
                        .Include($"{nameof(Publication.Inspections)}.{nameof(Inspection.Audits)}")
                        .Include($"{nameof(Publication.Inspections)}.{nameof(Inspection.Audits)}.{nameof(Audit.AuditItems)}")
                        .Include($"{nameof(Publication.Inspections)}.{nameof(Inspection.Audits)}.{nameof(Audit.Survey)}")
                        .Include($"{nameof(Publication.Inspections)}.{nameof(Inspection.Audits)}.{nameof(Audit.Survey)}.{nameof(Survey.SurveyItems)}")
                        .Include($"{nameof(Publication.Inspections)}.{nameof(Inspection.Audits)}.{nameof(Audit.Auditor)}")
                        .Include($"{nameof(Publication.Inspections)}.{nameof(Inspection.Audits)}.{nameof(Audit.Auditor)}.{nameof(User.Teams)}")
                        .Where(p => p.PublishMode == PublishModeEnum.Inspection)
                        .ToArrayAsync();
                    var lastInspectionPublications = inspectionPublications
                        .GroupBy(p => p.Process)
                        .Select(_ => _.MaxBy(p => p.PublishedDate).First());
                    result = lastInspectionPublications
                        .SelectMany(p => p.Inspections)
                        .SelectMany(i => i.Audits)
                        .Where(a => a.EndDate != null)
                        .OrderByDescending(a => a.StartDate);
                }
                else
                    result = await context.Audits
                        .Include(nameof(Audit.AuditItems))
                        .Include(nameof(Audit.Survey))
                        .Include(nameof(Audit.Inspection))
                        .Include($"{nameof(Audit.Survey)}.{nameof(Survey.SurveyItems)}")
                        .Include($"{nameof(Audit.Inspection)}.{nameof(Inspection.Anomalies)}")
                        .Include($"{nameof(Audit.Inspection)}.{nameof(Inspection.InspectionSteps)}")
                        .Include($"{nameof(Audit.Inspection)}.{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}")
                        .Include($"{nameof(Audit.Inspection)}.{nameof(Inspection.Publication)}")
                        .Include($"{nameof(Audit.Inspection)}.{nameof(Inspection.Publication)}.{nameof(Publication.Process)}")
                        .Include(nameof(Audit.Auditor))
                        .Include($"{nameof(Audit.Auditor)}.{nameof(User.Teams)}")
                        .Where(_ => _.Id == auditId && _.EndDate != null)
                    .ToArrayAsync();
                return result;
            }
        }

        /// <summary>
        /// Get all surveys or spesific survey
        /// </summary>
        public async Task<IEnumerable<Survey>> GetSurveys(int? surveyId = null)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                if (surveyId == null)
                    return await context.Surveys
                        .Include(nameof(Survey.SurveyItems))
                        .Include(nameof(Survey.Audits))
                        .ToArrayAsync();
                return await context.Surveys
                        .Include(nameof(Survey.SurveyItems))
                        .Include(nameof(Survey.Audits))
                        .Where(_ => _.Id == surveyId)
                        .ToArrayAsync();
            }
        }

        /// <summary>
        /// Save surveys
        /// </summary>
        /// <param name="surveys">List of surveys to be saved</param>
        /// <returns>List of surveys that want to be saved</returns>
        public async Task<Survey[]> SaveSurveys(Survey[] surveys)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                List<Survey> surveysWithInvalidUserName = new List<Survey>();
                foreach (Survey survey in surveys)
                {
                    if (survey.IsMarkedAsModified || survey.IsMarkedAsAdded)
                    {
                        // Vérifier que la vidéo peut être supprimée
                        using (var tempContext = ContextFactory.GetNewContext())
                        {
                            bool surveyNameAlreadyExists = await tempContext.Surveys
                                .Where(u => u.Id != survey.Id && u.Name == survey.Name)
                                .AnyAsync();

                            if (surveyNameAlreadyExists)
                                surveysWithInvalidUserName.Add(survey);
                        }
                    }

                    if (survey.IsMarkedAsDeleted)
                    {
                        foreach (SurveyItem item in context.SurveyItems.Where(s => s.SurveyId == survey.Id))
                        {
                            context.SurveyItems.Attach(item);
                            context.SurveyItems.DeleteObject(item);
                            context.SurveyItems.ApplyChanges(item);
                        }

                        //Apply delete to Survey
                        context.Surveys.Attach(survey);
                        context.Surveys.DeleteObject(survey);
                    }
                }
                if (surveysWithInvalidUserName.Any())
                {
                    var ex = new BLLFuncException("Common_CannotUseSameName")
                    {
                        ErrorCode = KnownErrorCodes.CannotUseSameUserName
                    };
                    ex.Data.Add(KnownErrorCodes.CannotUseSameUserName_UsersKey, surveysWithInvalidUserName.ToArray());
                    throw ex;
                }

                foreach (var survey in surveys)
                {
                    context.Surveys.ApplyChanges(survey);
                }

                await context.SaveChangesAsync();
                return surveys;
            }
        }

        /// <summary>
        /// Check if auditor already have active audit 
        /// </summary>
        public async Task<bool> CheckAuditorHaveActiveAudit(int? auditorId = null, Guid? publicationId = null)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                if (publicationId == null || publicationId == Guid.Empty)
                    return await context.Audits.AnyAsync(_ => _.AuditorId == auditorId && _.EndDate == null && !_.IsDeleted);
                return await context.Audits
                    .Include(nameof(Audit.Inspection))
                    .AnyAsync(a => a.AuditorId == auditorId && a.EndDate == null && !a.IsDeleted && a.Inspection.PublicationId == publicationId);
            }
        }

        /// <summary>
        /// Get active audit 
        /// </summary>
        public async Task<Audit> GetActiveAudit(int? auditorId = null, Guid? publicationId = null)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                if (publicationId == null || publicationId == Guid.Empty)
                    return await context.Audits
                        .Include(nameof(Audit.Inspection))
                        .Include(nameof(Audit.AuditItems))
                        .Include(nameof(Audit.Survey))
                    .FirstOrDefaultAsync(_ => _.AuditorId == auditorId && _.EndDate == null && !_.IsDeleted);
                return await context.Audits
                    .Include(nameof(Audit.Inspection))
                    .Include(nameof(Audit.AuditItems))
                    .Include(nameof(Audit.Survey))
                    .FirstOrDefaultAsync(a => a.AuditorId == auditorId && a.EndDate == null && !a.IsDeleted && a.Inspection.PublicationId == publicationId);
            }
        }

        /// <summary>
        /// Save audit
        /// </summary>
        /// <returns>Audit</returns>
        public async Task<Audit> SaveAudit(Audit audit)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                Audit sameAudit = new Audit();
                if (audit.IsMarkedAsModified || audit.IsMarkedAsAdded)
                {
                    using (var tempContext = ContextFactory.GetNewContext())
                    {
                        bool auditAlreadyExist = await tempContext.Audits
                            .Where(u => u.SurveyId != audit.SurveyId
                                && u.AuditorId == audit.AuditorId
                                && u.InspectionId == audit.InspectionId
                                && u.EndDate == null
                                && !u.IsDeleted)
                            .AnyAsync();

                        if (auditAlreadyExist)
                            sameAudit = audit;
                    }
                }
                if (audit.IsMarkedAsDeleted)
                {
                    //Apply delete to Audit
                    context.Audits.Attach(audit);
                    context.Audits.DeleteObject(audit);
                }

                if (sameAudit == audit)
                {
                    var ex = new BLLFuncException("Audit déjà créé. Créer l'audit a échoué")
                    {
                        ErrorCode = KnownErrorCodes.CannotUseSameUserName
                    };
                    ex.Data.Add(KnownErrorCodes.CannotUseSameUserName_UsersKey, sameAudit);
                    throw ex;
                }

                context.Audits.ApplyChanges(audit);

                await context.SaveChangesAsync();
                return audit;
            }
        }

        /// <summary>
        /// Obtient un booléen qui indique si tous les process liés d'un scénario (si ils existent) ont été publiés.
        /// </summary>
        public async Task<(bool Result, Procedure[] NonPublishedProcesses)> AllLinkedProcessArePublished(int scenarioId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                Scenario scenario = await context.Scenarios
                    .Include(nameof(Scenario.Actions))
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.LinkedProcess)}")
                    .Include(nameof(Scenario.Project))
                    .Include($"{nameof(Scenario.Project)}.{nameof(Project.Process)}")
                    .SingleAsync(_ => _.ScenarioId == scenarioId);

                // Si le scénario fait parti d'un process/compétence on renvoie true car les process liés sont désactivés.
                if (scenario.Project.Process.IsSkill)
                    return (true, null);

                KAction[] actionsWithLinkedProcess = scenario.Actions
                    .Where(_ => _.LinkedProcessId != null)
                    .OrderBy(_ => _.WBS)
                    .ToArray();
                bool result = true;
                List<Procedure> nonPublishedProcesses = new List<Procedure>();
                foreach (KAction actionWithLinkedProcess in actionsWithLinkedProcess)
                {
                    result = result && await context.Publications.AnyAsync(p => p.ProcessId == actionWithLinkedProcess.LinkedProcessId.Value);
                    if (!result)
                        nonPublishedProcesses.Add(actionWithLinkedProcess.LinkedProcess);
                }
                if (nonPublishedProcesses.Any())
                    return (false, nonPublishedProcesses.ToArray());
                return (true, null);
            }
        }

        /// <summary>
        /// Obtient un booléen qui indique si tous les process liés d'un scénario (si ils existent) ont été publiés. (SYNC)
        /// </summary>
        public (bool Result, Procedure[] NonPublishedProcesses) AllLinkedProcessArePublishedSync(int scenarioId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                Scenario scenario = context.Scenarios
                    .Include(nameof(Scenario.Actions))
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.LinkedProcess)}")
                    .Include(nameof(Scenario.Project))
                    .Include($"{nameof(Scenario.Project)}.{nameof(Project.Process)}")
                    .Single(_ => _.ScenarioId == scenarioId);

                // Si le scénario fait parti d'un process/compétence on renvoie true car les process liés sont désactivés.
                if (scenario.Project.Process.IsSkill)
                    return (true, null);

                KAction[] actionsWithLinkedProcess = scenario.Actions
                    .Where(_ => _.LinkedProcessId != null)
                    .OrderBy(_ => _.WBS)
                    .ToArray();
                bool result = true;
                List<Procedure> nonPublishedProcesses = new List<Procedure>();
                foreach (KAction actionWithLinkedProcess in actionsWithLinkedProcess)
                {
                    result = result && context.Publications.Any(p => p.ProcessId == actionWithLinkedProcess.LinkedProcessId.Value);
                    if (!result)
                        nonPublishedProcesses.Add(actionWithLinkedProcess.LinkedProcess);
                }
                if (nonPublishedProcesses.Any())
                    return (false, nonPublishedProcesses.ToArray());
                return (true, null);
            }
        }

        public async Task<Publication> SetReadPublication(Guid publicationId, int UserId, DateTime? ReadingDate = null)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var userReadPublication = await context.UserReadPublications
                    .SingleOrDefaultAsync(_ => _.PublicationId == publicationId && _.UserId == UserId);
                if (userReadPublication != null)
                {
                    userReadPublication.ReadDate = ReadingDate;
                }
                else
                {
                    context.UserReadPublications.AddObject(new UserReadPublication
                    {
                        PublicationId = publicationId,
                        UserId = UserId,
                        ReadDate = ReadingDate
                    });
                }
                await context.SaveChangesAsync();

                return await context.Publications
                    .Include(nameof(Publication.Readers))
                    .SingleOrDefaultAsync(_ => _.PublicationId == publicationId);
            }
        }

        async Task LoadNode(INode node, KsmedEntities context, int userId, Expression<Func<Procedure, bool>> processFilter)
        {
            if (node is Project project)
            {
                project.Objective = await context.Objectives
                    .Where(_ => _.ObjectiveCode == project.ObjectiveCode)
                    .SingleOrDefaultAsync();
                project.Referentials = new TrackableCollection<ProjectReferential>(await context.ProjectReferentials
                    .Where(_ => _.ProjectId == project.ProjectId)
                    .ToArrayAsync());
                project.Scenarios = new TrackableCollection<Scenario>(await context.Scenarios
                    .Where(_ => _.ProjectId == project.ProjectId && !_.IsDeleted)
                    .ToArrayAsync());
                ProjectDesc projectDesc = new ProjectDesc
                {
                    Project = project,
                    Referentials = project.Referentials,
                    Scenarios = project.Scenarios
                        .Where(scenario => !scenario.IsDeleted)
                        .Select(scenario => new ScenarioDesc
                        {
                            Id = scenario.ScenarioId,
                            Label = scenario.Label,
                            OriginalId = scenario.OriginalScenarioId,
                            NatureCode = scenario.NatureCode,
                            StateCode = scenario.StateCode,
                            IsShownInSummary = scenario.IsShownInSummary,
                            CriticalPathIDuration = scenario.CriticalPathIDuration,
                        })
                        .ToArray()
                };
                project.ScenariosDescriptions = projectDesc.Scenarios
                    .Select(sc => new ScenarioDescription(sc.Id, sc.Label, sc.NatureCode, sc.StateCode))
                    .ToArray();
                if (project.Scenarios.Any(_ => !_.IsDeleted))
                {
                    int lastScenarioId = project.Scenarios
                        .Where(_ => !_.IsDeleted)
                        .MaxBy(s => s.CreationDate)
                        .First()
                        .ScenarioId;
                    project.Process.LastScenarioHasLinkedProcess = await context.KActions
                        .Where(_ => _.ScenarioId == lastScenarioId)
                        .AnyAsync(_ => _.LinkedProcessId != null);
                }
            }
            else if (node is Procedure process)
            {
                process.Projects = new TrackableCollection<Project>(await context.Projects
                    .Where(_ => _.ProcessId == process.ProcessId && !_.IsDeleted)
                    .ToArrayAsync());
                process.UserRoleProcesses = new TrackableCollection<UserRoleProcess>(await context.UserRoleProcesses
                    .Include(nameof(UserRoleProcess.User))
                    .Where(_ => _.ProcessId == process.ProcessId)
                    .ToArrayAsync());
                process.VideoSyncs = new TrackableCollection<VideoSync>(await context.VideoSyncs
                    .Where(_ => _.UserId == userId
                                && _.ProcessId == process.ProcessId)
                    .ToArrayAsync());
                process.Videos = new TrackableCollection<Video>(await context.Videos
                    .Where(_ => _.ProcessId == process.ProcessId)
                    .ToArrayAsync());
                process.LastScenarioHasLinkedProcess = false;
                foreach (Project _ in process.Projects)
                    await LoadNode(_, context, userId, processFilter);
            }
            else if (node is ProjectDir folder)
            {
                folder.Processes = new TrackableCollection<Procedure>(await context.Procedures
                    .Where(_ => _.ProjectDirId == folder.Id)
                    .Where(processFilter)
                    .ToArrayAsync());
                folder.Childs = new TrackableCollection<ProjectDir>(await context.ProjectDirs
                    .Where(_ => _.ParentId == folder.Id && !_.IsDeleted)
                    .ToArrayAsync());
                foreach (Procedure _ in folder.Processes)
                    await LoadNode(_, context, userId, processFilter);
                foreach (ProjectDir _ in folder.Childs)
                    await LoadNode(_, context, userId, processFilter);
            }
        }

        async Task LoadPublicationNode(INode node, KsmedEntities context, PublishModeEnum filter)
        {
            if (node is ProjectDir folder)
            {
                // Returns all the undeleted processes that have at least a publication of the type filter
                folder.Processes = new TrackableCollection<Procedure>(await context.Procedures
                        .Include(nameof(Procedure.Publications))
                        .Where(_ => _.ProjectDirId == folder.Id
                                    && !_.IsDeleted
                                    && _.Publications.Any(p => (p.PublishMode & filter) == filter))
                        .ToArrayAsync());

                folder.Childs = new TrackableCollection<ProjectDir>(await context.ProjectDirs
                    .Where(_ => _.ParentId == folder.Id
                                && !_.IsDeleted)
                    .ToArrayAsync());
                foreach (ProjectDir _ in folder.Childs)
                    await LoadPublicationNode(_, context, filter);
            }
        }

        bool HaveAnyProcess(ProjectDir folder)
        {
            bool result = folder.Processes.Any();
            if (result)
                return result; // Le dossier courant possède au moins un process
            foreach (ProjectDir subFolder in folder.Childs)
            {
                result = HaveAnyProcess(subFolder);
                if (result)
                    return result; // Le dossier enfant courant possède au moins un process
            }
            return result; // Le dossier courant ne possède aucun process
        }

        void RemoveEmptyFolder(TrackableCollection<ProjectDir> folders)
        {
            ProjectDir[] tempArray = folders.ToArray();
            foreach (ProjectDir folder in tempArray)
            {
                if (folder.Childs.Any())
                    RemoveEmptyFolder(folder.Childs);
                if (folder.Processes.Any())
                    continue;
                if (!folder.Childs.Any())
                    folders.Remove(folder);
            }
        }

        void RemoveDeletedScenarios(ICollection<INode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node is Project project)
                    project.Scenarios.RemoveWhere(s => s.IsDeleted);
                else if (node is Procedure process)
                    RemoveDeletedScenarios(process.Projects.Cast<INode>().ToArray());
                else if (node is ProjectDir dir)
                    RemoveDeletedScenarios(dir.Processes.Cast<INode>()
                        .Concat(dir.Childs.Cast<INode>()).ToArray());
            }
        }

        void RemoveEmptyProjects(ICollection<INode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node is Procedure process)
                    process.Projects.RemoveWhere(p => !p.Scenarios.Any());
                else if (node is ProjectDir dir)
                    RemoveEmptyProjects(dir.Processes.Cast<INode>()
                        .Concat(dir.Childs.Cast<INode>()).ToArray());
            }
        }

        void RemoveEmptyProcesses(ProjectDir dir)
        {
            dir.Processes.RemoveWhere(p => !p.Projects.Any());
            foreach (var subDir in dir.Childs)
                RemoveEmptyProcesses(subDir);
        }

        void RemoveEmptyDirs(ICollection<INode> nodes)
        {
            nodes.RemoveWhere(n => n is ProjectDir dir
                                   && !HaveAnyProcess(dir));
            foreach (var node in nodes.Where(n => n is ProjectDir))
                RemoveEmptyDirs((node as ProjectDir).Childs.Cast<INode>().ToArray());
        }

        void RemovePublications(ICollection<INode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node is Procedure process)
                    process.Publications.Clear();
                else if (node is ProjectDir dir)
                    RemovePublications(dir.Processes.Cast<INode>()
                        .Concat(dir.Childs.Cast<INode>())
                        .ToArray());
            }
        }

        void KeepOnlyLastProject(ICollection<INode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node is Procedure process)
                    process.Projects = new TrackableCollection<Project>(new[] { process.Projects.MaxBy(p => p.StartDate).First() });
                else if (node is ProjectDir dir)
                    KeepOnlyLastProject(dir.Processes.Cast<INode>()
                        .Concat(dir.Childs.Cast<INode>())
                        .ToArray());
            }
        }

        /// <summary>
        /// Obtient un projet
        /// </summary>
        public virtual async Task<Project> GetProject(int projectId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await context.Projects
                        .Include(nameof(Project.Process))
                        .Include($"{nameof(Project.Process)}.{nameof(Procedure.UserRoleProcesses)}")
                        .SingleAsync(p => p.ProjectId == projectId);
                }
            });

        /// <summary>
        /// Obtient un projet en sync
        /// </summary>
        public virtual Project GetProjectSync(int projectId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                return context.Projects
                    .Include(nameof(Project.Process))
                    .Include($"{nameof(Project.Process)}.{nameof(Procedure.Projects)}")
                    .Single(p => p.ProjectId == projectId);
            }
        }

        /// <summary>
        /// Obtient l'arborescence des process ayant une publication
        /// </summary>
        public virtual async Task<INode[]> GetPublicationsTree(PublishModeEnum filter = PublishModeEnum.Formation | PublishModeEnum.Inspection | PublishModeEnum.Audit) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    // Returns all the undeleted processes that have at least a publication of the type filter
                    List<Procedure> processes = await context.Procedures
                        .Include(nameof(Procedure.Publications))
                        .Where(_ => _.ProjectDirId == null
                                    && !_.IsDeleted
                                    && _.Publications.Any(p => (p.PublishMode & filter) == filter))
                         .ToListAsync();

                    List<ProjectDir> dirs = await context.ProjectDirs
                        .Where(_ => _.ParentId == null
                                    && !_.IsDeleted)
                        .ToListAsync();

                    dirs = dirs.OrderBy(d => d.Label).ToList();

                    foreach (ProjectDir _ in dirs)
                        await LoadPublicationNode(_, context, filter);

                    ProjectDir[] tempArray = dirs.ToArray();
                    foreach (ProjectDir folder in tempArray)
                    {
                        if (!HaveAnyProcess(folder))
                            dirs.Remove(folder);
                    }
                    tempArray = dirs.ToArray();
                    foreach (ProjectDir folder in tempArray)
                    {
                        if (folder.Childs.Any())
                            RemoveEmptyFolder(folder.Childs);
                        if (folder.Processes.Any())
                            continue;
                        if (!folder.Childs.Any())
                            dirs.Remove(folder);
                    }

                    List<INode> processesTree = dirs.Cast<INode>()
                         .Union(processes.Cast<INode>())
                         .ToList();
                    //RemoveEmptyDirs(processesTree);
                    RemovePublications(processesTree);

                    // Uncomment to test treeview scrolling on tablet
                    /*int counter = 10000;
                    for (int i = 0; i < 20; i++)
                        processesTree.Add(new ProjectDir { Id = counter, Name = $"Dossier {counter++}" });*/

                    return processesTree.ToArray();
                }
            });

        public async Task<INode[]> GetProcessTreeWithScenario()
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                // Get only fixed validation scenarios
                var processesInfos = await context.Scenarios
                    .Include(nameof(Scenario.Project))
                    .Include($"{nameof(Scenario.Project)}.{nameof(Project.Process)}")
                    .Where(s => !s.IsDeleted
                                && s.NatureCode == KnownScenarioNatures.Realized
                                && s.StateCode == KnownScenarioStates.Validated
                                && !s.Project.IsDeleted
                                && !s.Project.Process.IsDeleted)
                    .Select(s => new { s.Project.Process, s.Project.ProjectId, s.ScenarioId, s.CreationDate })
                    .AsNoTracking()
                    .ToListAsync();
                foreach(var pi in processesInfos)
                {
                    pi.Process.NodeProjectId = pi.ProjectId;
                    pi.Process.NodeScenarioId = pi.ScenarioId;
                }
                var processes = processesInfos.GroupBy(pi => pi.Process.ProcessId)
                    .Select(gpi => gpi.OrderByDescending(_ => _.CreationDate).First().Process)
                    .ToList();
                // Dirs list
                var projectDirs = await context.ProjectDirs
                    .OrderBy(d => d.Name)
                    .AsNoTracking()
                    .ToListAsync();
                // Load dirs
                var rootDirs = new List<ProjectDir>();
                foreach (var process in processes.Where(p => p.ProjectDirId != null))
                {
                    if (process.ProjectDir == null && process.ProjectDirId != null)
                        process.ProjectDir = projectDirs.Single(d => d.Id == process.ProjectDirId.Value);
                    var currentDir = process.ProjectDir;
                    while (currentDir.ParentId != null)
                    {
                        if (currentDir.Parent != null)
                            currentDir = currentDir.Parent;
                        else
                        {
                            currentDir.Parent = projectDirs.Single(d => d.Id == currentDir.ParentId.Value);
                            currentDir = currentDir.Parent;
                        }
                    }
                    if (!rootDirs.Contains(currentDir))
                        rootDirs.Add(currentDir);
                }
                // Concat to tree
                List<INode> processesTree = rootDirs.Cast<INode>()
                    .Concat(processes.Where(p => p.ProjectDirId == null).Cast<INode>())
                    .ToList();

                return processesTree.ToArray();
            }
        }

        public async Task<string> GetProcessName(int processId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                return await context.Procedures
                    .Where(_ => _.ProcessId == processId)
                    .Select(_ => _.Label)
                    .SingleOrDefaultAsync();
            }
        }

        public async Task<string> GetProjectName(int projectId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                return await context.Projects
                    .Where(_ => _.ProjectId == projectId)
                    .Select(_ => _.Label)
                    .SingleOrDefaultAsync();
            }
        }

        public async Task<string> GetScenarioName(int scenarioId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                return await context.Scenarios
                    .Where(_ => _.ScenarioId == scenarioId)
                    .Select(_ => _.Label)
                    .SingleOrDefaultAsync();
            }
        }

        public async Task<int?> GetLastFixedValidationScenarioId(int processId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var scenarios = await context.Procedures
                    .Where(proc => proc.ProcessId == processId)
                    .SelectMany(proc => proc.Projects)
                    .Where(proc => !proc.IsDeleted)
                    .SelectMany(proj => proj.Scenarios)
                    .Where(s => !s.IsDeleted
                                && s.StateCode == KnownScenarioStates.Validated
                                && s.NatureCode == KnownScenarioNatures.Realized)
                    .ToListAsync();
                return scenarios.MaxBy(s => s.CreationDate)
                    .SingleOrDefault()
                    ?.ScenarioId;
            }
        }

        /// <summary>
        /// Get process information by id
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="includeAllInformations">Include all the informations (longer to execute)</param>
        /// <returns></returns>
        public async Task<Procedure> GetProcess(int processId, bool includeAllInformations = true)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var process = context.Procedures
                        .Include(nameof(Procedure.UserRoleProcesses))
                        .Include(nameof(Procedure.Projects))
                        .Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}");

                if (includeAllInformations)
                    process = process
                        .Include($"{nameof(Procedure.Projects)}.{nameof(Project.Referentials)}")
                        .Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}")
                        .Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Thumbnail)}")
                        .Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Resource)}")
                        .Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Ref1)}.{nameof(Ref1Action.Ref1)}")
                        .Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Ref2)}.{nameof(Ref2Action.Ref2)}")
                        .Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Ref3)}.{nameof(Ref3Action.Ref3)}")
                        .Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Ref4)}.{nameof(Ref4Action.Ref4)}")
                        .Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Ref5)}.{nameof(Ref5Action.Ref5)}")
                        .Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Ref6)}.{nameof(Ref6Action.Ref6)}")
                        .Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Ref7)}.{nameof(Ref7Action.Ref7)}");

                return await process.FirstOrDefaultAsync(_ => _.ProcessId == processId);
            }
        }

        public async Task<Procedure> GetProcessForPublishFormat(int processId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                Procedure process = await context.Procedures
                        .Include(nameof(Procedure.Projects))
                        .Include($"{nameof(Procedure.Projects)}.{nameof(Project.Referentials)}")
                        .Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}")
                        //.Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}")
                        //.Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Thumbnail)}")
                        //.Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Ref1)}.{nameof(Ref1Action.Ref1)}.{nameof(Ref1.CloudFile)}")
                        //.Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Ref2)}.{nameof(Ref2Action.Ref2)}.{nameof(Ref2.CloudFile)}")
                        //.Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Ref3)}.{nameof(Ref3Action.Ref3)}.{nameof(Ref3.CloudFile)}")
                        //.Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Ref4)}.{nameof(Ref4Action.Ref4)}.{nameof(Ref4.CloudFile)}")
                        //.Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Ref5)}.{nameof(Ref5Action.Ref5)}.{nameof(Ref5.CloudFile)}")
                        //.Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Ref6)}.{nameof(Ref6Action.Ref6)}.{nameof(Ref6.CloudFile)}")
                        //.Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}.{nameof(Scenario.Actions)}.{nameof(KAction.Ref7)}.{nameof(Ref7Action.Ref7)}.{nameof(Ref7.CloudFile)}")
                        .SingleAsync(_ => _.ProcessId == processId);
                return process;
            }
        }

        /// <summary>
        /// Retrieve action information for a specific scenario
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<KAction>> GetActionsByScenario(int scenarioId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var projectTimeScale = await GetProjectTimeScale(scenarioId);
                var actions = await context.KActions
                    .AsNoTracking()
                    //.Include($"{nameof(KAction.Scenario)}.{nameof(Scenario.Project)}")
                    .Include($"{nameof(KAction.Thumbnail)}")
                    .Include($"{nameof(KAction.Ref1)}.{nameof(Ref1Action.Ref1)}.{nameof(Ref1.CloudFile)}")
                    .Include($"{nameof(KAction.Ref2)}.{nameof(Ref2Action.Ref2)}.{nameof(Ref2.CloudFile)}")
                    .Include($"{nameof(KAction.Ref3)}.{nameof(Ref3Action.Ref3)}.{nameof(Ref3.CloudFile)}")
                    .Include($"{nameof(KAction.Ref4)}.{nameof(Ref4Action.Ref4)}.{nameof(Ref4.CloudFile)}")
                    .Include($"{nameof(KAction.Ref5)}.{nameof(Ref5Action.Ref5)}.{nameof(Ref5.CloudFile)}")
                    .Include($"{nameof(KAction.Ref6)}.{nameof(Ref6Action.Ref6)}.{nameof(Ref6.CloudFile)}")
                    .Include($"{nameof(KAction.Ref7)}.{nameof(Ref7Action.Ref7)}.{nameof(Ref7.CloudFile)}")
                    .Include($"{nameof(KAction.Resource)}.{nameof(Resource.CloudFile)}")
                    .Include($"{nameof(KAction.Category)}.{nameof(ActionCategory.CloudFile)}")
                    .Where(_ => _.ScenarioId == scenarioId)
                    .ToListAsync();
                foreach (var action in actions)
                {
                    action.IsGroup = WBSHelper.HasChildren(action, actions);
                    action.DurationString = GetTimeScaleMaskValue(projectTimeScale, action.Duration);
                }
                return actions;
            }
        }

        static string GetTimeScaleMaskValue(long projectTimeScale, long ticks)
        {
            var timeSpan = TimeSpan.FromTicks(ticks);
            var timeSpanHours = Math.Floor(timeSpan.TotalHours);
            var timeSpanMinutes = timeSpan.Minutes;
            var timeSpanSeconds = timeSpan.Seconds;
            var timeSpanMilliseconds = timeSpan.Milliseconds;

            if (projectTimeScale == TimeSpan.FromSeconds(1).Ticks)
                return $"{timeSpanHours.ToString().PadLeft(2, '0')}{timeSpanMinutes.ToString().PadLeft(2, '0')}{timeSpanSeconds.ToString().PadLeft(2, '0')}";
            else if (projectTimeScale == TimeSpan.FromSeconds(.1).Ticks)
                return $"{timeSpanHours.ToString().PadLeft(2, '0')}{timeSpanMinutes.ToString().PadLeft(2, '0')}{timeSpanSeconds.ToString().PadLeft(2, '0')}{timeSpanMilliseconds.ToString().PadLeft(3, '0').Substring(0, 1)}";
            else if (projectTimeScale == TimeSpan.FromSeconds(.01).Ticks)
                return $"{timeSpanHours.ToString().PadLeft(2, '0')}{timeSpanMinutes.ToString().PadLeft(2, '0')}{timeSpanSeconds.ToString().PadLeft(2, '0')}{timeSpanMilliseconds.ToString().PadLeft(3, '0').Substring(0, 2)}";
            else if (projectTimeScale == TimeSpan.FromSeconds(.001).Ticks)
                return $"{timeSpanHours.ToString().PadLeft(2, '0')}{timeSpanMinutes.ToString().PadLeft(2, '0')}{timeSpanSeconds.ToString().PadLeft(2, '0')}{timeSpanMilliseconds.ToString().PadLeft(3, '0')}";
            return null;
        }

        /// <summary>
        /// Obtient les projets et les objectifs
        /// </summary>
        public virtual async Task<ProjectsData> GetProjects() =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    string userName = _securityContext.CurrentUser.Username;
                    User user = await context.Users
                        .Include(nameof(User.Roles))
                        .Include(nameof(User.UserRoleProcesses))
                        .SingleAsync(u => !u.IsDeleted && u.Username == userName);

                    int userId = user.UserId;

                    // Si l'utilisateur est un admin, on affiche tout
                    Expression<Func<Procedure, bool>> processFilter = p => !p.IsDeleted
                                                                         && p.UserRoleProcesses.Any(urp => urp.UserId == userId);
                    if (user.Roles.Any(r => string.Equals(r.RoleCode, KnownRoles.Administrator)))
                        processFilter = p => !p.IsDeleted;

                    var processes = await context.Procedures
                        .Where(_ => _.ProjectDirId == null)
                        .Where(processFilter)
                        .ToArrayAsync();
                    var dirs = await context.ProjectDirs
                        .Where(_ => _.ParentId == null && !_.IsDeleted)
                        .ToArrayAsync();
                    foreach (Procedure _ in processes)
                        await LoadNode(_, context, userId, processFilter);
                    foreach (ProjectDir _ in dirs)
                        await LoadNode(_, context, userId, processFilter);

                    var projectsTree = new ProjectDir
                    {
                        Processes = new TrackableCollection<Procedure>(processes),
                        Childs = new TrackableCollection<ProjectDir>(dirs)
                    };
                    //RemoveEmptyProcesses(projectsTree);
                    //RemoveEmptyFolder(projectsTree.Childs);

                    var projectsWithScenarios = projectsTree.Nodes.Flatten(node => node.Nodes)
                        .OfType<Project>()
                        .OrderBy(_ => _.Label)
                        .Select(project =>
                        {
                            project.Scenarios = new TrackableCollection<Scenario>(context.Scenarios
                                .Where(_ => _.ProjectId == project.ProjectId && !_.IsDeleted)
                                .ToArray());
                            return new ProjectDesc
                            {
                                Project = project,
                                Referentials = project.Referentials,
                                Scenarios = project.Scenarios
                                    .Where(scenario => !scenario.IsDeleted)
                                    .Select(scenario => new ScenarioDesc
                                    {
                                        Id = scenario.ScenarioId,
                                        Label = scenario.Label,
                                        OriginalId = scenario.OriginalScenarioId,
                                        NatureCode = scenario.NatureCode,
                                        StateCode = scenario.StateCode,
                                        IsShownInSummary = scenario.IsShownInSummary,
                                        CriticalPathIDuration = scenario.CriticalPathIDuration,
                                    })
                                    .ToArray()
                            };
                        })
                        .ToArray();

                    int[] scenarioIds = projectsWithScenarios
                        .SelectMany(p => p.Scenarios)
                        .Select(s => s.Id)
                        .ToArray();

                    // Ne pas tracker les actions ni les catégories chargées
                    KActionDesc[] actions = (await context.KActions
                            .Select(a => new KActionDesc
                            {
                                WBS = a.WBS,
                                ScenarioId = a.ScenarioId,
                                BuildStart = a.BuildStart,
                                BuildFinish = a.BuildFinish,
                                Category = a.Category,
                            })
                            .Where(a => scenarioIds.Contains(a.ScenarioId))
                            .AsObjectQuery()
                            .ExecuteAsync(MergeOption.NoTracking))
                            .ToArray();

                    // Calcul des valorisations
                    Dictionary<Project, ScenarioCriticalPath[]> summary = new Dictionary<Project, ScenarioCriticalPath[]>();

                    foreach (ProjectDesc projectWithScenarios in projectsWithScenarios)
                    {
                        projectWithScenarios.Project.ScenariosDescriptions = projectWithScenarios.Scenarios
                            .Select(sc => new ScenarioDescription(sc.Id, sc.Label, sc.NatureCode, sc.StateCode))
                            .ToArray();

                        foreach (ScenarioDesc scenario in projectWithScenarios.Scenarios)
                            scenario.Actions = actions
                                .Where(a => a.ScenarioId == scenario.Id)
                                .ToList();

                        ScenarioCriticalPath[] projectSummary = GetSummary(projectWithScenarios.Scenarios, false);
                        summary[projectWithScenarios.Project] = projectSummary;

                    }

                    Project[] projects = projectsWithScenarios
                        .Select(p => p.Project)
                        .Distinct()
                        .ToArray();

                    Objective[] obj = await context.Objectives.ToArrayAsync();

                    return new ProjectsData
                    {
                        Projects = projects,
                        ProjectsTree = projectsTree.Nodes.ToArray(),
                        Objectives = obj,
                        Summary = summary.ToList()
                    };
                }
            });

        /// <summary>
        /// Sync all videos.
        /// </summary>
        public virtual Task SyncVideos(params int[] processIds)
        {
            // Not used on desktop only or server side
            return Task.CompletedTask;
        }

        /// <summary>
        /// List all videos to sync.
        /// </summary>
        public virtual async Task<Dictionary<VideoSyncTask, List<Video>>> ListAllVideosToSync(params int[] processIds) =>
            await Task.Run(async () =>
            {
                Dictionary<VideoSyncTask, List<Video>> result = new Dictionary<VideoSyncTask, List<Video>>()
                {
                    [VideoSyncTask.Sync] = new List<Video>(),
                    [VideoSyncTask.NotSync] = new List<Video>()
                };
                if (processIds?.Any() == true)
                {
                    using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                    {
                        var allVideos = await context.Videos
                            .Include(nameof(Video.Process))
                            .Include($"{nameof(Video.Process)}.{nameof(Procedure.VideoSyncs)}")
                            .Where(_ => processIds.Contains(_.ProcessId))
                            .ToArrayAsync();
                        result[VideoSyncTask.Sync] = allVideos
                            .Where(_ => _.Process.VideoSyncs.Any(vs => vs.UserId == _securityContext.CurrentUser.User.UserId && vs.SyncValue))
                            .DistinctBy(_ => $"{_.Hash}{_.Extension}")
                            .ToList();
                        result[VideoSyncTask.NotSync] = allVideos
                            .Where(_ => !_.Process.VideoSyncs.Any(vs => vs.UserId == _securityContext.CurrentUser.User.UserId && vs.SyncValue)
                                && !result[VideoSyncTask.Sync].Any(v => $"{v.Hash}{v.Extension}" == $"{_.Hash}{_.Extension}"))
                            .DistinctBy(_ => $"{_.Hash}{_.Extension}")
                            .ToList();
                    }
                }
                return result;
            });

        /// <summary>
        /// Get if a video can be unsynced.
        /// </summary>
        public virtual async Task<bool> CanBeUnSync(string videoHash) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    User user = await context.Users.Include(nameof(User.Roles)).SingleAsync(_ => _.UserId == _securityContext.CurrentUser.User.UserId);

                    Video[] videosToSync = await context.Procedures
                        .Include(nameof(Procedure.SyncVideo))
                        .Include(nameof(Procedure.Videos))
                        .Where(_ => _.VideoSyncs.Any(vs => vs.UserId == user.UserId && vs.SyncValue))
                        .SelectMany(_ => _.Videos)
                        .Where(_ => _.Sync)
                        .ToArrayAsync();

                    return !videosToSync.Any(_ => _.Hash == videoHash);
                }
            });

        /// <summary>
        /// Obtient les dossiers
        /// </summary>
        public virtual async Task<ProjectDir[]> GetProjectDirs() =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await context.ProjectDirs.ToArrayAsync();
                }
            });

        /// <summary>
        /// Obtient les process.
        /// </summary>
        public async Task<Procedure[]> GetProcesses() =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await context.Procedures.OrderBy(_ => _.Label).ToArrayAsync();
                }
            });

        public async Task<Procedure[]> GetPublishedProcessesForInspection()
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var lastPublications = new List<Publication>();
                Publication[] publications = await context.Publications
                        .Include(nameof(Publication.Process))
                        .OrderBy(parameters => parameters.Process.ProjectDirId)
                        .ToArrayAsync();

                var processIds = publications.Select(u => u.ProcessId).Distinct();

                foreach (var processId in processIds)
                {
                    if (publications.Length == 0)
                        continue;

                    Publication lastPublication = publications
                        .Where(_ => _.ProcessId == processId
                                    && (int)_.PublishMode == (int)PublishModeEnum.Inspection)
                        .MaxBy(_ => _.PublishedDate)
                        .FirstOrDefault();
                    if (lastPublication == null)
                        continue;
                    lastPublications.Add(lastPublication);
                }
                return lastPublications.Select(p => p.Process)
                    .Distinct()
                    .OrderBy(p => p.Label)
                    .ToArray();
            }
        }

        /// <summary>
        /// Obtient si un process est lié à une tâche.
        /// </summary>
        public async Task<bool> ProcessIsLinkedToATask(int processId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await context.KActions.AnyAsync(_ => _.LinkedProcessId == processId);
                }
            });

        /// <summary>
        /// Obtient les noms avec extension d'une liste de fichiers.
        /// </summary>
        public virtual async Task<string[]> GetFullName(IEnumerable<string> fileHashes) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    string[] publishedFiles = await context.PublishedFiles.Where(_ => fileHashes.Any(f => f == _.Hash)).Select(_ => _.Hash + _.Extension).ToArrayAsync();
                    string[] cutVideos = await context.CutVideos.Where(_ => fileHashes.Any(f => f == _.Hash)).Select(_ => _.Hash + _.Extension).ToArrayAsync();
                    return publishedFiles.Concat(cutVideos).ToArray();
                }
            });

        /// <summary>
        /// Sauvegarde le projet.
        /// </summary>
        /// <param name="project">Le projet.</param>
        public virtual async Task<Project> SaveProject(Project project) =>
            await Task.Run(async () =>
            {
                bool projectIsAdded = project.IsMarkedAsAdded;
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    if (project.IsDeleted)
                    {
                        context.Projects.ApplyChanges(project);
                        await context.SaveChangesAsync();
                        return null;
                    }
                    if (projectIsAdded)
                    {
                        // Ajouter les paramètres de référentiels par défaut
                        ProjectReferential[] referentials = CreateDefaultProjectReferentials();
                        project.Referentials.AddRange(referentials);

                        // Ajouter le champ libre texte par défaut
                        project.CustomTextLabel = _localizationManager.GetString("Business_PrepareService_DefaultCustomFieldLabel1");
                    }

                    context.Projects.ApplyChanges(project);
                    // Correction bug : Si on vient de supprimer un projet, ce projet reste dans le store à l'état supprimé et produit une erreur car non existant dans la base
                    IObjectWithChangeTracker[] deleteEntities = context.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted).Select(_ => _.Entity).Cast<IObjectWithChangeTracker>().ToArray();
                    if (project.IsMarkedAsAdded)
                        await context.RefreshAsync(RefreshMode.StoreWins, deleteEntities);
                    // Correction bug
                    await context.SaveChangesAsync();
                    project.MarkAsUnchanged();
                    project.StartTracking();

                    /*if (projectIsAdded) // Si on ne fait pas ça, il y a une erreur sur la clé primaire de User
                    {
                        // Si le projet est nouveau, ajouter l'utilisateur courant en tant en tant que membre du projet
                        project.UserRoleProjects.Add(new UserRoleProject()
                        {
                            UserId = (await context.Users.SingleAsync(u => !u.IsDeleted && u.Username == _securityContext.CurrentUser.Username)).UserId,
                            RoleCode = KnownRoles.Analyst,
                        });
                        context.Projects.ApplyChanges(project);
                        await context.SaveChangesAsync();
                        project.MarkAsUnchanged();
                        project.StartTracking();
                    }*/
                    return project;
                }
            });

        async Task RecursivelyDeleteFolder(KsmedEntities context, int folderId)
        {
            ProjectDir folder = await context.ProjectDirs
                .Include(nameof(ProjectDir.Childs))
                .Include(nameof(ProjectDir.Processes))
                .SingleOrDefaultAsync(f => f.Id == folderId);
            ProjectDir[] childs = folder.Childs.ToArray();
            Procedure[] processes = folder.Processes.ToArray();
            foreach (ProjectDir subFolder in childs)
                await RecursivelyDeleteFolder(context, subFolder.Id);
            foreach (Procedure process in processes)
                await RecursivelyDeleteProcess(context, process.ProcessId);
            folder.IsDeleted = true;
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Sauvegarde le dossier.
        /// </summary>
        /// <param name="folder">Le dossier.</param>
        public virtual async Task<ProjectDir> SaveFolder(ProjectDir folder) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    if (folder.IsDeleted)
                    {
                        await RecursivelyDeleteFolder(context, folder.Id);
                        return null;
                    }
                    context.ProjectDirs.ApplyChanges(folder);
                    await context.SaveChangesAsync();
                    folder.MarkAsUnchanged();
                    folder.StartTracking();
                    return folder;
                }
            });

        public virtual async Task<AppSetting[]> SaveAppSettings(AppSetting[] settings) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    foreach (var setting in settings)
                    {
                        context.AppSettings.ApplyChanges(setting);
                        await context.SaveChangesAsync();
                    }
                    return settings;
                }
            });

        async Task RecursivelyDeleteProcess(KsmedEntities context, int processId)
        {
            Procedure process = await context.Procedures
                .Include(nameof(Procedure.Projects))
                .Include($"{nameof(Procedure.Projects)}.{nameof(Project.Scenarios)}")
                .Include(nameof(Procedure.Equipments))
                .Include(nameof(Procedure.Operators))
                .Include(nameof(Procedure.ActionCategories))
                .Include(nameof(Procedure.Refs1))
                .Include(nameof(Procedure.Refs2))
                .Include(nameof(Procedure.Refs3))
                .Include(nameof(Procedure.Refs4))
                .Include(nameof(Procedure.Refs5))
                .Include(nameof(Procedure.Refs6))
                .Include(nameof(Procedure.Refs7))
                .SingleOrDefaultAsync(p => p.ProcessId == processId);
            foreach (Project project in process.Projects)
            {
                foreach (Scenario scenario in project.Scenarios)
                    scenario.IsDeleted = true;
                project.IsDeleted = true;
            }
            foreach (var r in process.Equipments)
                r.IsDeleted = true;
            foreach (var r in process.Operators)
                r.IsDeleted = true;
            foreach (var r in process.ActionCategories)
                r.IsDeleted = true;
            foreach (var r in process.Refs1)
                r.IsDeleted = true;
            foreach (var r in process.Refs2)
                r.IsDeleted = true;
            foreach (var r in process.Refs3)
                r.IsDeleted = true;
            foreach (var r in process.Refs4)
                r.IsDeleted = true;
            foreach (var r in process.Refs5)
                r.IsDeleted = true;
            foreach (var r in process.Refs6)
                r.IsDeleted = true;
            foreach (var r in process.Refs7)
                r.IsDeleted = true;
            process.IsDeleted = true;
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Sauvegarde le process.
        /// </summary>
        /// <param name="process">Le process.</param>
        public virtual async Task<Procedure> SaveProcess(Procedure process, bool notifyChanges = true) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    if (process.IsDeleted)
                    {
                        await RecursivelyDeleteProcess(context, process.ProcessId);
                        return null;
                    }
                    if (process.IsMarkedAsAdded)
                    {
                        process.OwnerId = _securityContext.CurrentUser.User.UserId;
                        process.UserRoleProcesses.Add(new UserRoleProcess
                        {
                            UserId = process.OwnerId,
                            RoleCode = KnownRoles.Analyst
                        });
                    }
                    context.Procedures.ApplyChanges(process);
                    await context.SaveChangesAsync();
                    foreach (var urp in process.UserRoleProcesses)
                        urp.AcceptChanges();
                    process.AcceptChanges();
                    process.StartTracking();
                    return process;
                }
            });

        /// <summary>
        /// Obtient les raisons possibles d'une non qualification.
        /// </summary>
        public virtual async Task<List<QualificationReason>> GetQualificationReasons() =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await context.QualificationReasons.OrderBy(_ => _.Number).ToListAsync();
                }
            });

        /// <summary>
        /// Sauvegarde les raisons possibles d'une non qualification.
        /// </summary>
        /// <param name="reasons">Les raisons.</param>
        public async Task<List<QualificationReason>> SaveQualificationReasons(IEnumerable<QualificationReason> reasons) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    if (reasons != null)
                    {
                        foreach (var reason in reasons)
                        {
                            context.QualificationReasons.ApplyChanges(reason);
                        }

                        await context.SaveChangesAsync();
                    }

                    return reasons.ToList();
                }
            });

        /// <summary>
        /// Obtient les rôles des utilisateurs dans le process spécifié, tous les utilisateurs et tous les rôles disponibles.
        /// </summary>
        /// <param name="processId">L'identifiant du process.</param>
        public virtual async Task<(User[] Users, Role[] Roles)> GetMembers(int processId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    User[] users = await context.Users
                        .Include(nameof(User.Roles))
                        .OrderBy(u => u.Name).ThenBy(u => u.Firstname)
                        .ToArrayAsync();
                    users = users.OrderBy(u => u.FullName).ToArray();

                    Role[] roles = await context.Roles
                        .Where(r => r.RoleCode != KnownRoles.Administrator && r.RoleCode != KnownRoles.Exporter)
                        .ToArrayAsync();

                    UserRoleProcess[] urps = await context.UserRoleProcesses
                        .Where(p => p.ProcessId == processId)
                        .ToArrayAsync();

                    foreach (User user in users)
                        user.RoleCodes
                            .AddRange(urps.Where(urp => urp.UserId == user.UserId)
                                          .Select(urp => urp.RoleCode));

                    return (users, roles);
                }
            });

        /// <summary>
        /// Sauvegarde le membre d'un process.
        /// </summary>
        /// <param name="processId">L'identifiant du process.</param>
        /// <param name="member">Le membre.</param>
        public virtual async Task<User> SaveMember(int processId, User member)
        {
            return await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    // Charger les rôles existants
                    UserRoleProcess[] urps = await context.UserRoleProcesses
                            .Include(nameof(UserRoleProcess.Role))
                            .Include(nameof(UserRoleProcess.User))
                            .Where(p => p.ProcessId == processId)
                            .ToArrayAsync();

                    IEnumerable<UserRoleProcess> dbRoles = urps.Where(urp => urp.UserId == member.UserId);
                    IEnumerable<string> dbRolesCodes = dbRoles.Select(urp => urp.RoleCode);

                    IEnumerable<string> newRoles = member.RoleCodes.Except(dbRolesCodes);
                    foreach (string role in newRoles)
                    {
                        context.UserRoleProcesses.AddObject(
                            new UserRoleProcess()
                            {
                                ProcessId = processId,
                                UserId = member.UserId,
                                RoleCode = role,
                            });
                    }

                    IEnumerable<string> deletedRoles = dbRolesCodes.Except(member.RoleCodes);
                    foreach (string role in deletedRoles)
                    {
                        context.UserRoleProcesses.DeleteObject(
                            context.UserRoleProcesses.Single(urp =>
                                urp.ProcessId == processId &&
                                urp.UserId == member.UserId &&
                                urp.RoleCode == role));
                    }

                    await context.SaveChangesAsync();

                    member.MarkAsUnchanged();
                    return member;
                }
            });
        }


        /// <inheritdoc />
        public virtual async Task<Project> GetReferentials(int projectId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    Project project = await context.Projects
                        .Include(nameof(Project.Objective))
                        .Include(nameof(Project.Referentials))
                        .Where(pr => pr.ProjectId == projectId)
                        .FirstAsync();

                    project.Referentials = new TrackableCollection<ProjectReferential>(project.Referentials.OrderBy(pr => pr.ReferentialId));
                    var skill = project.Referentials.Single(x => x.ReferentialId == ProcessReferentialIdentifier.Skill);
                    project.Referentials.Remove(skill);
                    project.Referentials.Insert(3, skill);

                    if (project.Referentials.Count != DefaultReferentialsCount)
                        throw new InvalidOperationException("Les référentiels sont invalides.");

                    return project;
                }
            });

        /// <inheritdoc />
        public virtual async Task<Project> SaveReferentials(Project project) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    if (project.CustomTextLabel?.Length == 0)
                        project.CustomTextLabel = null;
                    if (project.CustomTextLabel2?.Length == 0)
                        project.CustomTextLabel2 = null;
                    if (project.CustomTextLabel3?.Length == 0)
                        project.CustomTextLabel3 = null;
                    if (project.CustomTextLabel4?.Length == 0)
                        project.CustomTextLabel4 = null;

                    if (project.CustomNumericLabel?.Length == 0)
                        project.CustomNumericLabel = null;
                    if (project.CustomNumericLabel2?.Length == 0)
                        project.CustomNumericLabel2 = null;
                    if (project.CustomNumericLabel3?.Length == 0)
                        project.CustomNumericLabel3 = null;
                    if (project.CustomNumericLabel4?.Length == 0)
                        project.CustomNumericLabel4 = null;

                    context.Projects.ApplyChanges(project);

                    foreach (ProjectReferential referential in project.Referentials)
                        context.ProjectReferentials.ApplyChanges(referential);

                    await context.SaveChangesAsync();

                    project.MarkAsUnchanged();
                    project.StartTracking();

                    foreach (ProjectReferential referential in project.Referentials)
                    {
                        referential.MarkAsUnchanged();
                        referential.StartTracking();
                    }

                    return project;
                }
            });

        /// <summary>
        /// Crée les référentiels des projets par défaut.
        /// </summary>
        /// <returns>Les référentiels.</returns>
        internal static ProjectReferential[] CreateDefaultProjectReferentials()
        {
            return new ProjectReferential[]
            {
                new ProjectReferential
                {
                    ReferentialId = ProcessReferentialIdentifier.Operator,
                    IsEnabled = true,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = true,
                },
                new ProjectReferential
                {
                    ReferentialId = ProcessReferentialIdentifier.Equipment,
                    IsEnabled = true,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = true,
                },
                new ProjectReferential
                {
                    ReferentialId = ProcessReferentialIdentifier.Category,
                    IsEnabled = true,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
                new ProjectReferential
                {
                    ReferentialId = ProcessReferentialIdentifier.Skill,
                    IsEnabled = true,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
                new ProjectReferential
                {
                    ReferentialId = ProcessReferentialIdentifier.Ref1,
                    IsEnabled = false,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
                new ProjectReferential
                {
                    ReferentialId = ProcessReferentialIdentifier.Ref2,
                    IsEnabled = false,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
                new ProjectReferential
                {
                    ReferentialId = ProcessReferentialIdentifier.Ref3,
                    IsEnabled = false,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
                new ProjectReferential
                {
                    ReferentialId = ProcessReferentialIdentifier.Ref4,
                    IsEnabled = false,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
                new ProjectReferential
                {
                    ReferentialId = ProcessReferentialIdentifier.Ref5,
                    IsEnabled = false,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
                new ProjectReferential
                {
                    ReferentialId = ProcessReferentialIdentifier.Ref6,
                    IsEnabled = false,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
                new ProjectReferential
                {
                    ReferentialId = ProcessReferentialIdentifier.Ref7,
                    IsEnabled = false,
                    HasMultipleSelection = false,
                    HasQuantity = false,
                    KeepsSelection = false,
                },
            };
        }

        /// <summary>
        /// Obtient toutes les ressources liées au process spécifié.
        /// </summary>
        /// <param name="processId">L'identifiant du process.</param>
        public virtual async Task<Resource[]> GetAllResources(int processId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await Queries.FilterResources(context, processId).ToArrayAsync();
                }
            });

        /// <summary>
        /// Obtient une vidéo ayant la même vidéo d'origine si elle existe.
        /// </summary>
        /// <param name="originalHash">Le hash de la vidéo d'origine.</param>
        public virtual async Task<Video> GetSameOriginalVideo(string originalHash) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await context.Videos.FirstOrDefaultAsync(_ => _.OriginalHash == originalHash);
                }
            });

        /// <summary>
        /// Obtient la vidéo.
        /// </summary>
        /// <param name="videoId">L'identifiant de la vidéo.</param>
        public virtual async Task<Video> GetVideo(int videoId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await context.Videos.SingleOrDefaultAsync(_ => _.VideoId == videoId);
                }
            });

        /// <summary>
        /// Obtient toutes les préférences d'application.
        /// </summary>
        public virtual async Task<AppSetting[]> GetAllAppSettings() =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await context.AppSettings.ToArrayAsync();
                }
            });

        /// <summary>
        /// Obtient les vidéos et tous les éléments liés au chargement de Prepare - Videos, liés au process spécifié.
        /// </summary>
        /// <param name="processId">L'identifiant du process.</param>
        public virtual async Task<VideoLoad> GetVideos(int processId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    Procedure process = await context.Procedures.Include(nameof(Procedure.VideoSyncs)).SingleAsync(_ => _.ProcessId == processId);
                    VideoSync userSync = process.VideoSyncs.SingleOrDefault(_ => _.UserId == _securityContext.CurrentUser.User.UserId);
                    Resource[] processResources = await Queries.FilterResources(context, processId).ToArrayAsync();
                    Video[] videos = await context.Videos
                        .Where(v => v.ProcessId == processId)
                        .OrderBy(v => v.CameraName).OrderBy(v => v.DefaultResourceId).OrderBy(v => v.ResourceView).OrderBy(v => v.ShootingDate)
                        .ToArrayAsync();
                    foreach (var video in videos)
                        video.IsUsed = await context.KActions.AnyAsync(a => a.VideoId == video.VideoId);
                    foreach (Video video in videos.Where(_ => _.OnServer != true))
                    {
                        if (!string.IsNullOrEmpty(video.Filename) && await _apiHttpClient.ServiceAsync<bool>(KL2_Server.File, null, $"Exists/{video.Filename}", null, "GET"))
                        {
                            video.OnServer = true;
                            await context.SaveChangesAsync();
                        }
                    }

                    VideoLoad data = new VideoLoad()
                    {
                        ProcessResources = processResources,
                        ProcessVideos = videos,
                        Sync = userSync?.SyncValue ?? false
                    };

                    return data;
                }
            });

        /// <summary>
        /// Sauvegarde la vidéo d'un process.
        /// </summary>
        /// <param name="video">La vidéo.</param>
        public virtual async Task<Video> SaveVideo(Video video)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                Video result = video;
                if (result.IsMarkedAsDeleted)
                {
                    // Vérifier que la vidéo peut être supprimée
                    Video videoInBase = await context.Videos.SingleOrDefaultAsync(v => v.VideoId == result.VideoId);
                    bool hasRelatedAction = await context.KActions
                        .Where(a => a.VideoId == result.VideoId)
                        .AnyAsync();
                    if (hasRelatedAction)
                    {
                        BLLFuncException ex = new BLLFuncException("Impossible de supprimer la vidéo car elle a des actions associées.")
                        {
                            ErrorCode = KnownErrorCodes.CannotDeleteVideoWithRelatedActions
                        };
                        throw ex;
                    }
                    // C'est OK pour la suppression
                    context.Videos.DeleteObject(videoInBase);
                    await context.SaveChangesAsync();

                    // Si le fichier video n'est plus utilisé nul part, le supprimer
                    if (!await context.Videos.AnyAsync(_ => _.Hash == result.Hash))
                        await _apiHttpClient.ServiceAsync(KL2_Server.File, null, $"Delete/{result.Filename}", null, "GET");

                    return null;
                }

                if (await context.Videos.AnyAsync(_ => _.Hash == result.Hash && _.OnServer == true))
                    result.OnServer = true;

                VideoSync videoSync = result.Process?.VideoSyncs.SingleOrDefault(_ => _.UserId == _securityContext.CurrentUser.User.UserId);
                if (videoSync == null)
                    videoSync = await context.VideoSyncs.SingleOrDefaultAsync(_ => _.UserId == _securityContext.CurrentUser.User.UserId && _.ProcessId == result.ProcessId);
                if (videoSync == null)
                {
                    videoSync = new VideoSync
                    {
                        UserId = _securityContext.CurrentUser.User.UserId,
                        ProcessId = result.ProcessId,
                        SyncValue = result.Sync
                    };
                    context.VideoSyncs.AddObject(videoSync);
                }

                if (result.IsMarkedAsAdded)
                {
                    if (result.DefaultResourceId != null)
                        result.DefaultResource = await context.Resources.SingleAsync(_ => _.ResourceId == result.DefaultResourceId);
                    context.Videos.AddObject(result);

                    await context.SaveChangesAsync();
                }
                else
                {
                    Video videoInBase = await context.Videos.SingleAsync(v => v.VideoId == result.VideoId);
                    Resource resourceInBase = null;
                    foreach (var modifiedValue in result.ChangeTracker.ModifiedValues)
                    {
                        if (modifiedValue.Key == nameof(Video.DefaultResource))
                        {
                            if (result.DefaultResource == null)
                                videoInBase.DefaultResource = null;
                            else
                                resourceInBase = await context.Resources.SingleAsync(_ =>
                                    _.ResourceId == result.DefaultResource.ResourceId);
                        }
                        else
                        {
                            videoInBase.SetPropertyValue(modifiedValue.Key, modifiedValue.Value);
                        }
                    }
                    resourceInBase?.Videos.Add(videoInBase);

                    await context.SaveChangesAsync();

                    result = videoInBase;
                }

                return result;
            }
        }

        /// <summary>
        /// Obtient les scénarios liés au projet spécifié.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<ScenariosData> GetScenarios(int projectId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await GetScenarios(context, projectId);
                }
            });

        /// <summary>
        /// Obtient le scénario
        /// </summary>
        /// <param name="scenarioId">L'identifiant du scenario.</param>
        public virtual async Task<Scenario> GetScenario(int scenarioId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    Scenario scenario = await context.Scenarios
                            .Include(nameof(Scenario.Project))
                            .Include($"{nameof(Scenario.Project)}.{nameof(Project.Referentials)}")
                            .SingleAsync(_ => _.ScenarioId == scenarioId);
                    return scenario;
                }
            });

        /// <summary>
        /// Obtient le scénario pour publication
        /// </summary>
        /// <param name="scenarioId">L'identifiant du scenario.</param>
        public virtual async Task<Scenario> GetScenarioForPublish(int scenarioId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    Scenario scenario = await context.Scenarios
                            .Include(nameof(Scenario.Project))
                            .Include($"{nameof(Scenario.Project)}.{nameof(Project.Process)}")
                            .Include($"{nameof(Scenario.Project)}.{nameof(Project.Referentials)}")
                            .Include(nameof(Scenario.Actions))
                            .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Ref1)}")
                            .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Ref1)}.{nameof(Ref1Action.Ref1)}")
                            .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Ref2)}")
                            .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Ref2)}.{nameof(Ref2Action.Ref2)}")
                            .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Ref3)}")
                            .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Ref3)}.{nameof(Ref3Action.Ref3)}")
                            .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Ref4)}")
                            .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Ref4)}.{nameof(Ref4Action.Ref4)}")
                            .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Ref5)}")
                            .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Ref5)}.{nameof(Ref5Action.Ref5)}")
                            .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Ref6)}")
                            .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Ref6)}.{nameof(Ref6Action.Ref6)}")
                            .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Ref7)}")
                            .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Ref7)}.{nameof(Ref7Action.Ref7)}")
                            .SingleAsync(_ => _.ScenarioId == scenarioId);
                    return scenario;
                }
            });

        /// <summary>
        /// Obtient les scénarios d'un projet.
        /// </summary>
        /// <param name="context">Le contexte EF.</param>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <returns>Les scénarios</returns>
        private async Task<ScenariosData> GetScenarios(KsmedEntities context, int projectId)
        {
            Scenario[] scenarios = await context.Scenarios
                .Include(nameof(Scenario.Actions))
                .Include(nameof(Scenario.Project))
                .Include($"{nameof(Scenario.Project)}.{nameof(Project.Process)}")
                .Where(s => s.ProjectId == projectId && !s.IsDeleted)
                .ToArrayAsync();
            Dictionary<int, Video> projectVideos = await context.Projects
                .Include(nameof(Project.Process))
                .Include($"{nameof(Project.Process)}.{nameof(Procedure.Videos)}")
                .Where(p => p.ProjectId == projectId)
                .SelectMany(p => p.Process.Videos)
                .ToDictionaryAsync(v => v.VideoId);

            // Charger les référentiels du projet
            ActionCategory[] categories = await Queries.FilterReferentials(context.ActionCategories, (await context.Projects.SingleAsync(p => p.ProjectId == projectId)).ProcessId, ProcessReferentialIdentifier.Category).ToArrayAsync();
            Resource[] resources = await Queries.FilterResources(context, (await context.Projects.SingleAsync(p => p.ProjectId == projectId)).ProcessId).ToArrayAsync();

            foreach (Scenario scenario in scenarios)
            {
                int[] usedVideoIds = await context.KActions
                    .Where(a => a.ScenarioId == scenario.ScenarioId && a.VideoId.HasValue)
                    .Select(a => a.VideoId.Value)
                    .Distinct()
                    .ToArrayAsync();

                scenario.UsedVideos = usedVideoIds.Select(id => projectVideos[id]).ToArray();
            }

            ScenarioActionHierarchyHelper.MapScenariosActionsOriginals(scenarios);

            ScenarioState[] states = await context.ScenarioStates.ToArrayAsync();
            ScenarioNature[] natures = await context.ScenarioNatures.ToArrayAsync();
            return new ScenariosData
            {
                Scenarios = scenarios,
                ScenarioStates = states,
                ScenarioNatures = natures,
                Summary = GetSummary(scenarios.Select(scenario => new ScenarioDesc(scenario)), true),
            };

        }

        /// <summary>
        /// Obtient la synthèse à partir d'un projet
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        internal static ScenarioCriticalPath[] GetSummary(Project project, bool includeResources)
        {
            return GetSummary(project.Scenarios.Select(scenario => new ScenarioDesc(scenario)), includeResources);
        }

        /// <summary>
        /// Obtient la synthèse à partir des scénarios
        /// </summary>
        /// <param name="scenarios">les scénarios.</param>
        /// <returns>La synthèse</returns>
        private static ScenarioCriticalPath[] GetSummary(IEnumerable<ScenarioDesc> scenarios, bool includeResources)
        {
            // Mettre en cache les scénarios pour les comparaisons futures
            scenarios = scenarios.ToArray();

            // Calculer les valeurs du chemin critique des scénarios
            List<ScenarioCriticalPath> scenariosCP = new List<ScenarioCriticalPath>();

            string[] naturesOrder =
                {
                    KnownScenarioNatures.Initial,
                    KnownScenarioNatures.Target,
                    KnownScenarioNatures.Realized,
                };

            ScenarioDesc rootScenario = scenarios.FirstOrDefault();

            // Récupérer ce qui correspond le plus au scénario racine, avec lequel les comparaisons seront faites
            foreach (string nature in naturesOrder)
            {
                ScenarioDesc first = scenarios.FirstOrDefault(s =>
                    (s.IsShownInSummary || s.StateCode == KnownScenarioStates.Validated)
                    && s.NatureCode == nature);
                if (first != null)
                {
                    rootScenario = first;
                    break;
                }
            }


            ResourceCriticalPath operatorsRootTotal = null;
            ResourceCriticalPath equipmentsRootTotal = null;
            Dictionary<Resource, ResourceCriticalPath> rootResourcesCriticalPath = new Dictionary<Resource, ResourceCriticalPath>();

            foreach (ScenarioDesc scenario in scenarios)
            {
                if (scenario.IsShownInSummary || scenario.StateCode == KnownScenarioStates.Validated)
                {
                    // Créer la partie scénario
                    bool isRootScenario = scenario == rootScenario;

                    ScenarioCriticalPath cp = new ScenarioCriticalPath()
                    {
                        Id = scenario.Id,
                        Label = scenario.Label,
                        IsLocked = scenario.StateCode == KnownScenarioStates.Validated,
                        CriticalPathDuration = scenario.CriticalPathIDuration
                    };

                    // Calculer les pourcentages des valorisations
                    IEnumerable<KActionDesc> actionsTarget = scenario.Actions.Where(a => !WBSHelper.HasChildren(a.WBS, scenario.Actions.Select(ac => ac.WBS)));
                    double totalDuration = actionsTarget.Any() ? actionsTarget.Sum(a => a.BuildFinish - a.BuildStart) : 0;

                    cp.Values = new Dictionary<string, double>
                    {
                        [KnownActionCategoryValues.VA] = CalcValuePercentage(actionsTarget, KnownActionCategoryValues.VA, totalDuration),
                        [KnownActionCategoryValues.NVA] = CalcValuePercentage(actionsTarget, KnownActionCategoryValues.NVA, totalDuration),
                        [KnownActionCategoryValues.BNVA] = CalcValuePercentage(actionsTarget, KnownActionCategoryValues.BNVA, totalDuration),
                        [ScenarioCriticalPath.ValueNoneKey] = CalcValuePercentage(actionsTarget, ScenarioCriticalPath.ValueNoneKey, totalDuration)
                    };

                    cp.CriticalPathDurationIE = scenario.Actions.Any() ?
                                            scenario.Actions.Max(a => a.BuildFinish) -
                                            scenario.Actions.Min(a => a.BuildStart)
                                            : 0;


                    if (scenario.OriginalId.HasValue)
                    {
                        ScenarioDesc original = scenarios.First(s => s.Id == scenario.OriginalId);
                        cp.OriginalLabel = original.Label;
                    }

                    if (rootScenario != scenario)
                    {
                        if (rootScenario.CriticalPathIDuration != 0)
                            cp.EarningPercent = 100 * (rootScenario.CriticalPathIDuration - scenario.CriticalPathIDuration) /
                                rootScenario.CriticalPathIDuration;

                        long rootDurationIE = rootScenario.Actions.Any() ?
                                                rootScenario.Actions.Max(a => a.BuildFinish) -
                                                rootScenario.Actions.Min(a => a.BuildStart)
                                                : 0;

                        if (rootDurationIE != 0)
                            cp.EarningPercentIE = 100 * (rootDurationIE - cp.CriticalPathDurationIE) /
                                rootDurationIE;
                    }

                    scenariosCP.Add(cp);

                    if (includeResources)
                    {
                        // Analyser les ressources
                        ResourceCriticalPathResult result = CalcResourceCP(scenario, rootScenario, operatorsRootTotal, equipmentsRootTotal, rootResourcesCriticalPath);

                        if (isRootScenario)
                        {
                            // Si c'est le root, définir les valeurs pour comparaison
                            foreach (KeyValuePair<Equipment, ResourceCriticalPath> kvp in result.EquipmentsCriticalPath)
                                rootResourcesCriticalPath.Add(kvp.Key, kvp.Value);

                            foreach (KeyValuePair<Operator, ResourceCriticalPath> kvp in result.OperatorsCriticalPath)
                                rootResourcesCriticalPath.Add(kvp.Key, kvp.Value);

                            equipmentsRootTotal = result.EquipmentsTotal;
                            operatorsRootTotal = result.OperatorsTotal;
                        }

                        cp.Operators = result.OperatorsCriticalPath.Values.ToArray();
                        cp.OperatorsTotal = result.OperatorsTotal;
                        cp.Equipments = result.EquipmentsCriticalPath.Values.ToArray();
                        cp.EquipmentsTotal = result.EquipmentsTotal;
                    }
                }
            }

            return scenariosCP.ToArray();
        }

        /// <summary>
        /// Calcule le pourcentage de la valorisation sur les actions.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        /// <param name="valueCode">Le code de la valorisation.</param>
        /// <param name="total">Le total du temps des actions.</param>
        /// <returns>Le pourcentage.</returns>
        static double CalcValuePercentage(IEnumerable<KActionDesc> actions, string valueCode, double total)
        {
            if (total == 0)
                return 0;

            double val;
            if (valueCode == ScenarioCriticalPath.ValueNoneKey)
                val = actions.Any(a => a.Category == null || a.Category.ActionValueCode == null) ? actions.Where(a => a.Category == null || a.Category.ActionValueCode == null).Sum(a => a.BuildFinish - a.BuildStart) : 0;
            else
                val = actions.Any(a => a.Category != null && a.Category.ActionValueCode == valueCode) ? actions.Where(a => a.Category != null && a.Category.ActionValueCode == valueCode).Sum(a => a.BuildFinish - a.BuildStart) : 0;

            return val / total * 100d;
        }

        /// <summary>
        /// Calcule les valeurs du chemin critique des ressources du scénario spécifié.
        /// </summary>
        /// <param name="scenario">Le scénario.</param>
        /// <param name="rootScenario">Le scénario racine avec lequel faire les comparaisons.</param>
        /// <param name="rootOperatorsTotal">Les valeurs totales des opérateurs du scénario racine.</param>
        /// <param name="rootEquipmentsTotal">Les valeurs totales des équipements du scénario racine.</param>
        /// <param name="rootResourceCriticalPath">Les valeurs des ressources du scénario racine.</param>
        /// <returns>Les résultats des calculs</returns>
        static ResourceCriticalPathResult CalcResourceCP(ScenarioDesc scenario,
            ScenarioDesc rootScenario,
            ResourceCriticalPath rootOperatorsTotal,
            ResourceCriticalPath rootEquipmentsTotal,
            Dictionary<Resource, ResourceCriticalPath> rootResourceCriticalPath)
        {
            // Exclure les groupes
            Resource[] distinctResources = scenario.Actions
                .Where(a => !WBSHelper.HasChildren(a.WBS, scenario.Actions.Select(ac => ac.WBS)) && a.Resource != null)
                .Select(a => a.Resource)
                .Distinct(new GenericEqualityComparer<Resource>((r1, r2) => r1.ResourceId == r2.ResourceId, r => r.ResourceId.GetHashCode()))
                .ToArray();

            Dictionary<Resource, ResourceCriticalPath> resourcesCP = new Dictionary<Resource, ResourceCriticalPath>();

            if (distinctResources.Any())
            {
                long totalTime = (scenario.Actions.Count > 0 ? scenario.Actions.Max(a => a.BuildFinish) : 0) -
                    (scenario.Actions.Count > 0 ? scenario.Actions.Min(a => a.BuildStart) : 0);

                foreach (Resource resource in distinctResources)
                {
                    // Exclure les groupes
                    IEnumerable<KActionDesc> actionsTarget = scenario.Actions.Where(a => a.ResourceId == resource.ResourceId && !WBSHelper.HasChildren(a.WBS, scenario.Actions.Select(ac => ac.WBS)));
                    IEnumerable<RangeHelper.Range<long>> ranges = actionsTarget.Union(a => a.BuildStart, a => a.BuildFinish);

                    // Calculer la durée, la charge, la surcharge
                    long loadRaw = ranges.Select(r => r.End - r.Start).Sum();
                    long resourceDuration = actionsTarget.Sum(a => a.BuildFinish - a.BuildStart);
                    long overloadRaw = resourceDuration - loadRaw;

                    ResourceCriticalPath rcp = new ResourceCriticalPath
                    {
                        Label = resource.Label,
                        Duration = resourceDuration,
                    };

                    // Calculer les pourcentages des valorisations
                    rcp.Values = new Dictionary<string, double>
                    {
                        [KnownActionCategoryValues.VA] = CalcValuePercentage(actionsTarget, KnownActionCategoryValues.VA, resourceDuration),
                        [KnownActionCategoryValues.NVA] = CalcValuePercentage(actionsTarget, KnownActionCategoryValues.NVA, resourceDuration),
                        [KnownActionCategoryValues.BNVA] = CalcValuePercentage(actionsTarget, KnownActionCategoryValues.BNVA, resourceDuration),
                        [ScenarioCriticalPath.ValueNoneKey] = CalcValuePercentage(actionsTarget, ScenarioCriticalPath.ValueNoneKey, resourceDuration)
                    };

                    if (rootResourceCriticalPath.ContainsKey(resource))
                        rcp.EarningPercent = 100 * (rootResourceCriticalPath[resource].Duration - rcp.Duration) / rootResourceCriticalPath[resource].Duration;

                    if (totalTime == 0)
                    {
                        rcp.LoadPercent = 0;
                        rcp.OverloadPercent = 0;
                    }
                    else
                    {
                        rcp.LoadPercent = (double)loadRaw * 100 / (double)totalTime;
                        rcp.OverloadPercent = (double)overloadRaw * 100 / (double)totalTime;
                    }

                    resourcesCP.Add(resource, rcp);
                }
            }

            ResourceCriticalPathResult result = new ResourceCriticalPathResult()
            {
                EquipmentsCriticalPath = resourcesCP.Where(kvp => kvp.Key is Equipment).ToDictionary(kvp => (Equipment)kvp.Key, kvp => kvp.Value),
                OperatorsCriticalPath = resourcesCP.Where(kvp => kvp.Key is Operator).ToDictionary(kvp => (Operator)kvp.Key, kvp => kvp.Value),
            };

            // Ajouter les totaux
            result.OperatorsTotal = new ResourceCriticalPath
            {
                Duration = result.OperatorsCriticalPath.Any() ? result.OperatorsCriticalPath.Values.Sum(r => r.Duration) : 0,
            };

            if (scenario != rootScenario && rootOperatorsTotal.Duration != 0)
                result.OperatorsTotal.EarningPercent = 100 * (rootOperatorsTotal.Duration - result.OperatorsTotal.Duration) / rootOperatorsTotal.Duration;


            result.EquipmentsTotal = new ResourceCriticalPath
            {
                Duration = result.EquipmentsCriticalPath.Any() ? result.EquipmentsCriticalPath.Values.Sum(r => r.Duration) : 0,
            };

            if (scenario != rootScenario && rootEquipmentsTotal.Duration != 0)
                result.EquipmentsTotal.EarningPercent = 100 * (rootEquipmentsTotal.Duration - result.EquipmentsTotal.Duration) / rootEquipmentsTotal.Duration;

            return result;
        }

        /// <summary>
        /// Représente le résultat des calculs.
        /// </summary>
        class ResourceCriticalPathResult
        {
            public Dictionary<Equipment, ResourceCriticalPath> EquipmentsCriticalPath { get; set; }
            public Dictionary<Operator, ResourceCriticalPath> OperatorsCriticalPath { get; set; }
            public ResourceCriticalPath OperatorsTotal { get; set; }
            public ResourceCriticalPath EquipmentsTotal { get; set; }
        }

        /// <summary>
        /// Crée un nouveau scénario initial.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<Scenario> CreateInitialScenario(int projectId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    Scenario newScenario = new Scenario()
                    {
                        Label = _localizationManager.GetString("Business_AnalyzeService_InitialScenarioLabel"),
                        ProjectId = projectId,
                        StateCode = KnownScenarioStates.Draft,
                        NatureCode = KnownScenarioNatures.Initial,
                        IsShownInSummary = true,
                    };
                    context.Scenarios.AddObject(newScenario);
                    await context.SaveChangesAsync();
                    return newScenario;
                }
            });

        /// <summary>
        /// Crée un nouveau scénario.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="sourceScenario">Le scénario source.</param>
        /// <param name="keepVideoForUnchanged">Determine si les sequences initiales doivent être conservées.</param>
        public virtual async Task<Scenario> CreateScenario(int projectId, Scenario sourceScenario, bool keepVideoForUnchanged) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    string targetNatureCode;
                    bool clearVideos = false;

                    switch (sourceScenario.NatureCode)
                    {
                        case KnownScenarioNatures.Initial:
                            targetNatureCode = KnownScenarioNatures.Target;
                            Scenario scenarioToValidate = await context.Scenarios.FirstAsync(s => s.ScenarioId == sourceScenario.ScenarioId);
                            if (scenarioToValidate.StateCode != KnownScenarioStates.Validated)
                            {
                                scenarioToValidate.StateCode = KnownScenarioStates.Validated;
                                scenarioToValidate.MarkAsModified();
                                //await context.SaveChangesAsync();
                            }

                            break;
                        case KnownScenarioNatures.Target:
                            if (sourceScenario.StateCode == KnownScenarioStates.Validated)
                            {
                                clearVideos = true;
                                targetNatureCode = KnownScenarioNatures.Realized;
                            }
                            else
                                targetNatureCode = KnownScenarioNatures.Target;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(sourceScenario), "Le NatureCode du scénario est invalide");
                    }

                    Scenario scenario = await _scenarioCloneManager.CreateDerivatedScenario(context, projectId, sourceScenario.ScenarioId, targetNatureCode, save: true);

                    if (clearVideos)
                    {
                        foreach (var action in scenario.Actions)
                        {
                            bool willKeepVideo = action.Original != null
                                && keepVideoForUnchanged
                                && action.BuildDuration == action.Original.Duration; // Algo -> Bug 4451

                            if (willKeepVideo)
                            {
                                action.Start = action.Original.Start;
                                action.Finish = action.Original.Finish;
                                action.Duration = action.Original.Duration;
                                action.Thumbnail = action.Original.Thumbnail;
                            }
                            else
                            {
                                action.Video = null;
                            }
                        }
                    }
                    return scenario;
                }
            });

        /// <summary>
        /// Deletes the scenario.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="scenario">le scénario a supprimer.</param>
        public virtual async Task<(bool Result, ScenarioCriticalPath[] Summary)> DeleteScenario(int projectId, Scenario scenario) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    var scenarioToDelete = await context.Scenarios.SingleAsync(_ => _.ScenarioId == scenario.ScenarioId);

                    bool hasLinkedScenario = await context.Scenarios.AnyAsync(s => s.OriginalScenarioId == scenarioToDelete.ScenarioId && !s.IsDeleted);

                    if (hasLinkedScenario)
                        return (false, null);

                    if (scenarioToDelete.NatureCode == KnownScenarioNatures.Target)
                    {
                        var originalInitialScenario = await context.Scenarios.FirstOrDefaultAsync(s => s.ScenarioId == scenarioToDelete.OriginalScenarioId && !s.IsDeleted);
                        int currentProjectId = scenarioToDelete.ProjectId;
                        bool stillHaveDerivatedScenarios = await context.Scenarios
                            .AnyAsync(s =>
                                s.ProjectId == currentProjectId
                                && s.ScenarioId != scenarioToDelete.ScenarioId
                                && s.NatureCode != KnownScenarioNatures.Initial
                                && !s.IsDeleted);
                        if (originalInitialScenario != null
                            && !stillHaveDerivatedScenarios
                            && originalInitialScenario.NatureCode == KnownScenarioNatures.Initial
                            && originalInitialScenario.StateCode == KnownScenarioStates.Validated)
                        {
                            originalInitialScenario.StateCode = KnownScenarioStates.Draft;
                        }
                    }
                    else if (scenarioToDelete.NatureCode == KnownScenarioNatures.Realized)
                    {
                        var actions = await context.Scenarios
                            .Where(s => s.ProjectId == projectId && s.ScenarioId != scenarioToDelete.ScenarioId)
                            .SelectMany(s => s.Actions).Where(a => a.DifferenceReason != null).ToArrayAsync();
                        // Lors de la suppression d'un scénario de validation, supprimer les causes des écarts sur les scénarios cible
                        foreach (KAction action in actions)
                            action.DifferenceReason = null;
                    }

                    scenarioToDelete.ProjectId = projectId;
                    scenarioToDelete.IsDeleted = true;

                    await context.SaveChangesAsync();

                    Scenario[] scenarios = (await context.Scenarios
                        .Include(nameof(Scenario.Actions))
                        .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Resource)}")
                        .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Category)}")
                        .Where($"it.{nameof(Scenario.ProjectId)} = @pid", new ObjectParameter("pid", projectId))
                        .ExecuteAsync(MergeOption.NoTracking))
                        .ToArray();

                    return (true, GetSummary(scenarios.Where(s => !s.IsDeleted).Select(s => new ScenarioDesc(s)), true));
                }
            });

        /// <summary>
        /// Sauvegarde le scénario spécifié.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="scenario">Le scénario.</param>
        public virtual async Task<ScenariosData> SaveScenario(int projectId, Scenario scenario) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    if (scenario.StateCode == KnownScenarioStates.Validated)
                    {
                        // Si le scénario est marqué comme validé, il faut vérifier qu'il n'y aucun autre scénario de même nature qui soit validé
                        using (var tempContext = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                        {
                            Scenario firstValidatedScenario = await tempContext.Scenarios
                                .FirstOrDefaultAsync(s =>
                                    s.StateCode == KnownScenarioStates.Validated &&
                                    s.NatureCode == scenario.NatureCode &&
                                    s.ProjectId == scenario.ProjectId &&
                                    !s.IsDeleted &&
                                    s.ScenarioId != scenario.ScenarioId);

                            if (firstValidatedScenario != null)
                            {
                                BLLFuncException ex = new BLLFuncException("Impossible de valider plus d'un scénario de même nature dans un projet")
                                {
                                    ErrorCode = KnownErrorCodes.CannotValidateMoreThanOneScenarioOfSameNature
                                };
                                ex.Data[KnownErrorCodes.CannotValidateMoreThanOneScenarioOfSameNature_ScenarioNameKey] = firstValidatedScenario.Label;
                                throw ex;
                            }
                        }
                    }
                    else if (scenario.ChangeTracker.OriginalValues.ContainsKey("StateCode") &&
                        ((string)scenario.ChangeTracker.OriginalValues["StateCode"]) == KnownScenarioStates.Validated &&
                        scenario.NatureCode != KnownScenarioNatures.Realized)
                    {
                        // Si : 
                        // - le scénario a été invalidé
                        // - c'était un scénario cible ou initial
                        // - il y a un scénario de validation 
                        // on lève une erreur
                        using (var tempContext = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                        {
                            Scenario firstRealizedValidatedcenario = await tempContext.Scenarios
                                .FirstOrDefaultAsync(s =>
                                    s.NatureCode == KnownScenarioNatures.Realized &&
                                    s.ProjectId == scenario.ProjectId && !s.IsDeleted);

                            if (firstRealizedValidatedcenario != null)
                            {
                                BLLFuncException ex = new BLLFuncException("Impossible d'invalider un scénario cible s'il existe un scénario de validation figé")
                                {
                                    ErrorCode = KnownErrorCodes.CannotInvalidateAScenarioWhenHavingRealizedScenario
                                };
                                throw ex;
                            }
                        }
                    }

                    scenario.ProjectId = projectId;

                    context.Scenarios.ApplyChanges(scenario);
                    await context.SaveChangesAsync();

                    scenario.MarkAsUnchanged();
                    scenario.StartTracking();

                    Scenario[] scenarios = (await context.Scenarios
                        .Include(nameof(Scenario.Actions))
                        .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Resource)}")
                        .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Category)}")
                        .Where(s => s.ProjectId == projectId)
                        .AsObjectQuery()
                        .ExecuteAsync(MergeOption.NoTracking))
                        .ToArray();

                    ScenarioActionHierarchyHelper.MapScenariosActionsOriginals(scenarios);

                    using (var tempContext = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                    {
                        return await GetScenarios(tempContext, projectId);
                    }
                }
            });

        /// <summary>
        /// Crée un nouveau projet à partir d'un scénario de validation.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="validatedScenario">Le scénario de validation.</param>
        public virtual async Task<int> CreateNewProjectFromValidatedScenario(int projectId, Scenario validatedScenario) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    //if (validatedScenario.NatureCode != KnownScenarioNatures.Realized)
                    //    throw new ArgumentOutOfRangeException("validatedScenario", "Le scénario doit être un scénario de validation.");

                    Project originalProject = await context.Projects
                        .Include(nameof(Project.Process))
                        .Include($"{nameof(Project.Process)}.{nameof(Procedure.UserRoleProcesses)}")
                        .Include($"{nameof(Project.Process)}.{nameof(Procedure.Videos)}")
                        .Include($"{nameof(Project.Process)}.{nameof(Procedure.Videos)}.{nameof(Video.DefaultResource)}")
                        .Include(nameof(Project.Objective))
                        .Include(nameof(Project.Scenarios))
                        .Include(nameof(Project.Referentials))
                        .FirstAsync(p => p.ProjectId == projectId);


                    Scenario newScenario = await _scenarioCloneManager.CreateDerivatedScenario(context, projectId, validatedScenario.ScenarioId, KnownScenarioNatures.Initial, false);

                    Project newProject = new Project()
                    {
                        Workshop = originalProject.Workshop,
                        Process = originalProject.Process,
                        Objective = originalProject.Objective,
                        OtherObjectiveLabel = originalProject.OtherObjectiveLabel,
                        Description = string.Format(_localizationManager.GetString("Business_PrepareService_NewProjetDescriptionFromRealizedProject"), originalProject.Label),
                        CustomTextLabel = originalProject.CustomTextLabel,
                        CustomTextLabel2 = originalProject.CustomTextLabel2,
                        CustomTextLabel3 = originalProject.CustomTextLabel3,
                        CustomTextLabel4 = originalProject.CustomTextLabel4,
                        CustomNumericLabel = originalProject.CustomNumericLabel,
                        CustomNumericLabel2 = originalProject.CustomNumericLabel2,
                        CustomNumericLabel3 = originalProject.CustomNumericLabel3,
                        CustomNumericLabel4 = originalProject.CustomNumericLabel4,
                        TimeScale = originalProject.TimeScale,
                        StartDate = DateTime.Now
                    };

                    switch (validatedScenario.NatureCode)
                    {
                        case KnownScenarioNatures.Initial:
                            newProject.Label = string.Format(_localizationManager.GetString("Business_PrepareService_NewProjetLabelFromInitialProject"), originalProject.Label, validatedScenario.Label);
                            newProject.Description = string.Format(_localizationManager.GetString("Business_PrepareService_NewProjetDescriptionFromInitialProject"), originalProject.Label, validatedScenario.Label);
                            break;

                        case KnownScenarioNatures.Target:
                            newProject.Label = string.Format(_localizationManager.GetString("Business_PrepareService_NewProjetLabelFromTargetProject"), originalProject.Label, validatedScenario.Label);
                            newProject.Description = string.Format(_localizationManager.GetString("Business_PrepareService_NewProjetDescriptionFromTargetProject"), originalProject.Label, validatedScenario.Label);
                            break;

                        case KnownScenarioNatures.Realized:
                            newProject.Label = string.Format(_localizationManager.GetString("Business_PrepareService_NewProjetLabelFromRealizedProject"), originalProject.Label);
                            newProject.Description = string.Format(_localizationManager.GetString("Business_PrepareService_NewProjetDescriptionFromRealizedProject"), originalProject.Label);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(Scenario.NatureCode));

                    }

                    // Copier les utilisations des référentiels
                    foreach (ProjectReferential projref in originalProject.Referentials)
                    {
                        newProject.Referentials.Add(new ProjectReferential
                        {
                            ReferentialId = projref.ReferentialId,
                            IsEnabled = projref.IsEnabled,
                            HasMultipleSelection = projref.HasMultipleSelection,
                            KeepsSelection = projref.KeepsSelection,
                            HasQuantity = projref.HasQuantity
                        });
                    }

                    // Copier et remapper les vidéos utilisée dans le scénario
                    /*string[] excludedVideoClonePropertyNames =
                    {
                        nameof(Video.VideoId),
                        nameof(Video.ProcessId),
                        nameof(Video.CreatedByUserId),
                        nameof(Video.CreationDate),
                        nameof(Video.ModifiedByUserId),
                        nameof(Video.LastModificationDate),
                        nameof(Video.Process),
                        nameof(Video.Nature),
                        nameof(Video.Actions)
                    };

                    List<Video> newVideos = new List<Video>();

                    foreach (Video originalVideo in originalProject.Process.Videos)
                    {
                        KAction[] linkedActions = newScenario.Actions.Where(a => a.VideoId == originalVideo.VideoId).ToArray();

                        if (linkedActions.Length > 0)
                        {
                            Video newVideo = new Video();
                            IDictionary<string, object> originalValues = originalVideo.GetCurrentValues();

                            foreach (KeyValuePair<string, object> kvp in originalValues)
                            {
                                if (!excludedVideoClonePropertyNames.Contains(kvp.Key))
                                    newVideo.SetPropertyValue(kvp.Key, kvp.Value);
                            }

                            foreach (KAction action in linkedActions)
                                action.Video = newVideo;

                            newVideos.Add(newVideo);
                            newProject.Process.Videos.Add(newVideo);
                        }
                    }*/

                    newProject.Scenarios.Add(newScenario);

                    // Copier et remapper les référentiels projet
                    IActionReferentialProcess[] refProject = ReferentialsHelper.GetAllReferentialsProject(newScenario.Actions)
                            .Union(ReferentialsHelper.GetAllReferentialsProject(newProject)).ToArray();

                    // Not needed anymore, because referentials are global to process
                    /*foreach (IActionReferentialProcess referential in refProject)
                    {
                        IActionReferentialProcess newRef = ReferentialsFactory.CopyToNewProject(referential);
                        ReferentialsHelper.UpdateReferentialReferences(newScenario.Actions, referential, newRef);

                        // Remapper également DefaultResourceId des vidéos
                        //if (referential is Resource)
                        //{
                        //    foreach (Video video in newVideos.Where(v => v.DefaultResourceId == referential.Id))
                        //        video.DefaultResource = (Resource)newRef;
                        //}
                    }*/

                    context.Projects.ApplyChanges(newProject);
                    await context.SaveChangesAsync();
                    return newProject.ProjectId;
                }
            });

        /// <summary>
        /// Met à jour l'identifiant de publication web.
        /// </summary>
        /// <param name="scenarioId">L'identifiant du scénario.</param>
        /// <param name="publicationGuid">L'identifiant de publication.</param>
        public virtual async Task UpdateScenarioPublicationGuid(int scenarioId, Guid? publicationGuid) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    Scenario scenario = await context.Scenarios.FirstAsync(s => s.ScenarioId == scenarioId);
                    scenario.WebPublicationGuid = publicationGuid;
                    await context.SaveChangesAsync();
                }
            });

        #region Publication

        /// <summary>
        /// Sauvegarde la publication
        /// </summary>
        /// <param name="publication">Publication à sauvegarder</param>
        /// <returns>La publication sauvegardée</returns>
        public async Task<Publication> SavePublication(Publication publication)
        {
            if (publication == null)
                return null;
            try
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    // Trainings are closing
                    var closingTrainings = publication.Trainings.Where(t => (t.ChangeTracker.ModifiedValues.ContainsKey(nameof(Training.EndDate)) && t.EndDate != null)
                                                                     || (t.IsMarkedAsAdded == true && t.EndDate != null)).ToList();
                    // Evaluations are closing
                    var closingEvaluations = publication.Qualifications.Where(q => (q.ChangeTracker.ModifiedValues.ContainsKey(nameof(Qualification.EndDate)) && q.EndDate != null)
                                                                            || (q.IsMarkedAsAdded == true && q.EndDate != null)).ToList();
                    // Anomalies from inpections are closing
                    var closingInspectionsAnomalies = publication.Inspections.Where(i => (i.ChangeTracker.ModifiedValues.ContainsKey(nameof(Inspection.EndDate)) && i.EndDate != null)
                                                                                         || (i.IsMarkedAsAdded == true && i.EndDate != null)).SelectMany(i => i.Anomalies).ToList();
                    // Audits are closing
                    var closingAudits = publication.Inspections.SelectMany(i => i.Audits.Where(a => (a.ChangeTracker.ModifiedValues.ContainsKey(nameof(Audit.EndDate)) && a.EndDate != null)
                                                                                             || (a.IsMarkedAsAdded == true && a.EndDate != null))).ToList();
                    // Anomalies from audits are closing
                    var closingAuditsAnomalies = publication.Inspections.SelectMany(i => i.Anomalies)
                        .Where(a => a.IsNotMarkedAsUnchanged).ToList();

                    // Concat all anomalies
                    var allAnomalies = closingInspectionsAnomalies.Concat(closingAuditsAnomalies).Distinct();

                    try
                    {
                        context.Publications.ApplyChanges(publication);
                        await context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException dbConcurrencyEx)
                    {
                        if (dbConcurrencyEx.Entries.Any(_ => _.Entity is Inspection))
                        {
                            //Prevent bug of concurrent inspection by multiple users
                            //Create as new inspections
                            publication.Inspections.OrderByDescending(i => i.EndDate).FirstOrDefault().ChangeTracker.State = ObjectState.Added;
                            foreach (var step in publication.Inspections.OrderByDescending(i => i.EndDate).FirstOrDefault().InspectionSteps)
                            {
                                step.ChangeTracker.State = ObjectState.Added;
                            }
                            context.Publications.ApplyChanges(publication);
                            await context.SaveChangesAsync();
                        }
                    }

                    if (closingTrainings.Any())
                    {
                        try
                        {
                            await _notificationService.SendTrainingsReport(closingTrainings.Select(t => t.TrainingId).ToList());
                        }
                        catch (Exception e)
                        {
                            _traceManager.TraceError(e, $"Can't send report for trainings {string.Join(", ", closingTrainings.Select(t => t.TrainingId).ToList())}");
                        }
                    }
                    foreach (var reportEvaluation in closingEvaluations)
                    {
                        try
                        {
                            await _notificationService.SendQualificationReport(reportEvaluation.QualificationId);
                        }
                        catch (Exception e)
                        {
                            _traceManager.TraceError(e, $"Can't send report for qualification {reportEvaluation.QualificationId}");
                        }
                    }
                    if (allAnomalies.Any())
                    {
                        try
                        {
                            await _notificationService.SendAnomaliesReport(allAnomalies.Select(a => a.Id).ToList());
                        }
                        catch (Exception e)
                        {
                            _traceManager.TraceError(e, $"Can't send report for anomalies {string.Join(", ", allAnomalies.Select(a => a.Id).ToList())}");
                        }
                    }
                    foreach (var reportAudit in closingAudits)
                    {
                        try
                        {
                            await _notificationService.SendAuditReport(reportAudit.Id);
                        }
                        catch (Exception e)
                        {
                            _traceManager.TraceError(e, $"Can't send report for audit {reportAudit.Id}");
                        }
                    }

                    return await GetLastPublicationFiltered(publication.ProcessId, (int)publication.PublishMode);
                }
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, $"ERREUR lors de l'appel à la fonction {nameof(SavePublication)}");
                return null;
            }
        }

        /// <summary>
        /// Récupère un fichier publié via le guid
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public async Task<PublishedFile> GetPublishedFile(string hash)
        {
            try
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await context.PublishedFiles.FirstOrDefaultAsync(u => u.Hash == hash);
                }
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, $"ERREUR lors de l'appel à la fonction {nameof(GetPublishedFile)}({hash})");
                return null;
            }
        }

        /// <summary>
        /// Addd un fichier publié
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public async Task CreatePublishedFile(string hash, string extension)
        {
            try
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    if (context.PublishedFiles.Any(u => u.Hash == hash))
                        throw new Exception($"{hash} published file already existing");
                    context.PublishedFiles.AddObject(new PublishedFile { Hash = hash, Extension = extension});
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, $"ERREUR lors de l'appel à la fonction {nameof(GetPublishedFile)}({hash})");
            }
        }

        /// <summary>
        /// Récupère un fichier publié via le guid
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public async Task<CutVideo> GetCutVideo(string hash)
        {
            try
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await context.CutVideos.FirstOrDefaultAsync(u => u.Hash == hash);
                }
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, $"ERREUR lors de l'appel à la fonction {nameof(GetCutVideo)}({hash})");
                return null;
            }
        }


        #endregion

        #region Formation

        /// <summary>
        /// Méthode permettant de récupérer les formations d'un utilisateur pour une publication
        /// </summary>
        /// <param name="publicationId">Identifiant de la publication</param>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>La formation de l'utilisateur ou null s'il n'en existe pas avec les paramètres spécifié</returns>
        public async Task<Training> GetTraining(Guid publicationId, int userId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var trainings = await context.Trainings
                    .Include(nameof(Training.ValidationTrainings))
                    .Include(nameof(Training.User))
                    .Where(_ => !_.IsDeleted
                                && _.PublicationId == publicationId
                                && _.UserId == userId)
                    .ToArrayAsync();

                if (!trainings.Any())
                    return null;

                return trainings
                    .MaxBy(_ => _.StartDate)
                    .First();
            }
        }

        /// <summary>
        /// Get all trainings
        /// </summary>
        public async Task<IEnumerable<Training>> GetTrainings()
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var trainings = await context.Trainings
                    .Include(nameof(Training.Publication))
                    .Include($"{nameof(Training.Publication)}.{nameof(Publication.Process)}")
                    .Include($"{nameof(Training.Publication)}.{nameof(Publication.Qualifications)}")
                    .Include(nameof(Training.ValidationTrainings))
                    .Include(nameof(Training.User))
                    .Where(_ => _.EndDate != null)
                    .ToArrayAsync();

                if (!trainings.Any())
                    return null;

                return trainings;
            }
        }

        /// <summary>
        /// Get all published actions for competency
        /// </summary>
        public async Task<IEnumerable<PublishedAction>> GetPublishedActions()
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                return await context.PublishedActions
                    .GroupBy(_ => new { _.PublicationId })
                    .Select(p => p.FirstOrDefault())
                    .Include(nameof(PublishedAction.Publication))
                    .Include($"{nameof(PublishedAction.Publication)}.{nameof(Publication.Qualifications)}")
                    .Include($"{nameof(PublishedAction.Publication)}.{nameof(Publication.Process)}")
                    .Include(nameof(PublishedAction.Skill))
                    .Where(_ => _.SkillId != null)
                    .ToArrayAsync();
            }
        }

        /// <summary>
        /// Get an action
        /// </summary>
        public async Task<KAction> GetAction(int id)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var action = await context.KActions
                .FirstOrDefaultAsync(_ => _.ActionId == id);
                return action;
            }
        }

        /// <summary>
        /// Get a published actions for competency
        /// </summary>
        public async Task<PublishedAction> GetPublishedAction(int id)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var publishedAction = await context.PublishedActions
                    .Include(nameof(PublishedAction.Publication))
                    .Include(nameof(PublishedAction.Thumbnail))
                    .Include($"{nameof(PublishedAction.Publication)}.{nameof(Publication.PublishedActions)}")
                    .Include($"{nameof(PublishedAction.Publication)}.{nameof(Publication.Qualifications)}")
                    .Include($"{nameof(PublishedAction.Publication)}.{nameof(Publication.Process)}")
                    .Include($"{nameof(PublishedAction.Publication)}.{nameof(Publication.Localizations)}")
                    .Include(nameof(PublishedAction.PublishedReferentialActions))
                    .Include($"{nameof(PublishedAction.PublishedReferentialActions)}.{nameof(PublishedReferentialAction.PublishedReferential)}")
                    .Include($"{nameof(PublishedAction.PublishedReferentialActions)}.{nameof(PublishedReferentialAction.PublishedReferential)}.{nameof(PublishedReferential.File)}")
                    .Include(nameof(PublishedAction.Skill))
                    .Include(nameof(PublishedAction.CutVideo))
                    .Include(nameof(PublishedAction.Successors))
                    .Include(nameof(PublishedAction.PublishedResource))
                    .Include($"{nameof(PublishedAction.PublishedResource)}.{nameof(PublishedResource.File)}")
                    .Include(nameof(PublishedAction.PublishedActionCategory))
                    .Include($"{nameof(PublishedAction.PublishedActionCategory)}.{nameof(PublishedActionCategory.File)}")
                    .FirstOrDefaultAsync(_ => _.PublishedActionId == id);

                return publishedAction;
            }
        }

        /// <summary>
        /// Retrouve une qualification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Qualification> GetQualification(int id)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var qualification = await context.Qualifications
                    .Include(nameof(Qualification.User))
                    .Include(nameof(Qualification.QualificationSteps))
                    .Include($"{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.PublishedAction)}")
                    .Include($"{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.PublishedAction)}.{nameof(PublishedAction.LinkedPublication)}")
                    .Include($"{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.PublishedAction)}.{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.PublishedActions)}")
                    .Include($"{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.PublishedAction)}.{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.Qualifications)}")
                    .Include($"{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.User)}")
                    .Include($"{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.PublishedAction)}.{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.Qualifications)}.{nameof(Qualification.QualificationSteps)}")
                    .Include(nameof(Qualification.Publication))
                    .Include($"{nameof(Qualification.Publication)}.{nameof(Publication.Process)}")
                    .Include($"{nameof(Qualification.Publication)}.{nameof(Publication.Trainings)}")
                    .Include($"{nameof(Qualification.Publication)}.{nameof(Publication.Trainings)}.{nameof(Training.ValidationTrainings)}")
                    .Include($"{nameof(Qualification.Publication)}.{nameof(Publication.PublishedActions)}")

                    .FirstOrDefaultAsync(_ => _.QualificationId == id);

                return qualification;
            }
        }


        /// <summary>
        /// Sauvegarde les formations
        /// </summary>
        /// <param name="trainings">Liste des formations à sauvegarder</param>
        /// <returns>La liste des formations sauvegardé</returns>
        public async Task<Training[]> SaveTrainings(Training[] trainings)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                if (trainings != null)
                {
                    foreach (var training in trainings)
                    {
                        context.Trainings.ApplyChanges(training);
                    }

                    await context.SaveChangesAsync();
                }

                return trainings;
            }
        }

        /// <summary>
        /// Save timeslots
        /// </summary>
        /// <param name="timeslots">List of timeslots that want to be saved</param>
        /// <returns>List of timeslots saved</returns>
        public async Task<Timeslot[]> SaveTimeslots(Timeslot[] timeslots)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                if (timeslots != null)
                {
                    foreach (var training in timeslots)
                    {
                        context.Timeslots.ApplyChanges(training);
                    }

                    await context.SaveChangesAsync();
                }

                return timeslots;
            }
        }

        #endregion

        #region Inspection

        /// <summary>
        /// Méthode permettant de récupérer la dernière inspection d'une publication
        /// </summary>
        /// <param name="publicationId">Identifiant de la publication</param>
        /// <returns>La dernière inspection ou null s'il n'en existe pas avec les paramètres spécifié</returns>
        public async Task<Inspection> GetLastInspection(Guid publicationId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var inspections = await context.Inspections
                    .Include(nameof(Inspection.InspectionSteps))
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}")
                    .Where(_ => !_.IsDeleted
                                && _.PublicationId == publicationId)
                    .ToArrayAsync();

                if (inspections.Any())
                    return inspections
                        .MaxBy(_ => _.StartDate)
                        .First();
                return null;
            }
        }

        /// <summary>
        /// Get inspection
        /// </summary>
        public async Task<Inspection> GetInspection(int inspectionId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                return await context.Inspections
                    .Include(nameof(Inspection.InspectionSteps))
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}")
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Anomaly)}")
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}.{nameof(User.Teams)}")
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.PublishedAction)}")
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.PublishedAction)}.{nameof(PublishedAction.LinkedPublication)}")
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.PublishedAction)}.{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.PublishedActions)}")
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.PublishedAction)}.{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.PublishedActions)}.{nameof(PublishedAction.Thumbnail)}")
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.PublishedAction)}.{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.Inspections)}")
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.PublishedAction)}.{nameof(PublishedAction.LinkedPublication)}.{nameof(Publication.Inspections)}.{nameof(Inspection.InspectionSteps)}")
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.PublishedAction)}.{nameof(PublishedAction.Thumbnail)}")
                    .Include(nameof(Inspection.Publication))
                    .Include(nameof(Inspection.Anomalies))
                    .Include($"{nameof(Inspection.Publication)}.{ nameof(Publication.Process)}")
                    .Include($"{nameof(Inspection.Publication)}.{ nameof(Publication.PublishedActions)}")
                    .FirstOrDefaultAsync(_ => _.InspectionId == inspectionId);
            }
        }

        /// <summary>
        /// Get all inspections
        /// </summary>
        public async Task<IEnumerable<Inspection>> GetInspections(Guid? publicationId = null)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                if (publicationId == null)
                    return await context.Inspections
                        .Include(nameof(Inspection.InspectionSteps))
                        .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}")
                        .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}.{nameof(User.Teams)}")
                        .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.PublishedAction)}")
                        .Include(nameof(Inspection.Publication))
                        .Include(nameof(Inspection.Anomalies))
                        .Include($"{nameof(Inspection.Publication)}.{ nameof(Publication.Process)}")
                        .Where(_ => !_.IsDeleted)
                        .ToArrayAsync();
                return await context.Inspections
                    .Include(nameof(Inspection.InspectionSteps))
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}")
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}.{nameof(User.Teams)}")
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.PublishedAction)}")
                    .Include(nameof(Inspection.Publication))
                    .Include($"{nameof(Inspection.Publication)}.{ nameof(Publication.Process)}")
                    .Where(_ => _.PublicationId == publicationId)
                    .ToArrayAsync();
            }
        }

        /// <summary>
        /// Get all timeslots
        /// </summary>
        public async Task<IEnumerable<Timeslot>> GetTimeslots(int? timeslotId = null)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                if (timeslotId == null)
                    return await context.Timeslots
                        .Where(_ => _.IsDeleted != true)
                        .OrderBy(_ => _.DisplayOrder)
                        .ToArrayAsync();
                return await context.Timeslots
                        .Where(_ => _.IsDeleted != true && _.TimeslotId == timeslotId)
                        .OrderBy(_ => _.DisplayOrder)
                        .ToArrayAsync();

            }
        }

        /// <summary>
        /// Get all inspections schedule for current TimeSlot
        /// </summary>
        public async Task<IEnumerable<InspectionSchedule>> GetInspectionSchedules(int? InspectionScheduleId = null)
        {
            try
            {
                var result = new ConcurrentBag<InspectionSchedule>();
                var now = DateTime.Now;
                var currentTime = now.TimeOfDay;
                var currentDate = now.Date;
                InspectionSchedule[] inspectionSchedules = null;

                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    inspectionSchedules = await context.InspectionSchedules
                        .Include(nameof(InspectionSchedule.Timeslot))
                        .Include(nameof(InspectionSchedule.Procedure))
                        .Where(_ => (InspectionScheduleId != null)
                            ? !_.Timeslot.IsDeleted && _.InspectionScheduleId == InspectionScheduleId
                            : !_.Timeslot.IsDeleted)
                        .ToArrayAsync();
                }

                inspectionSchedules.AsParallel().ForEach(inspectionSchedule =>
                {
                    var excludeDates = inspectionSchedule.RecurrenceException?.Split(',').Select(_ => DateTime.ParseExact(_.Split('T')[0], "yyyyMMdd", CultureInfo.InvariantCulture)).ToList();
                    var inspectionScheduleStartDateTime = new DateTime(inspectionSchedule.StartDate.Year,
                        inspectionSchedule.StartDate.Month,
                        inspectionSchedule.StartDate.Day,
                        inspectionSchedule.Timeslot.StartTime.Hours,
                        inspectionSchedule.Timeslot.StartTime.Minutes,
                        inspectionSchedule.Timeslot.StartTime.Seconds);
                    var recurrences = RecurrenceHelper.GetRecurrenceDateTimeCollection(inspectionSchedule.RecurrenceRule, inspectionScheduleStartDateTime, currentDate.AddDays(-1), currentDate.AddDays(2), excludeDates);
                    var timeSlots = recurrences.GetRecurrenceTimeSlotCollection(inspectionSchedule.Timeslot);
                    var currentTimeSlot = timeSlots.Contains(now);
                    if (currentTimeSlot != null)
                    {
                        using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                        {
                            var existentInspections = context.Inspections
                                .Include(nameof(Inspection.Publication))
                                .Where(_ => _.Publication.ProcessId == inspectionSchedule.ProcessId
                                                      && _.IsScheduled == true
                                                      && currentTimeSlot.StartDateTime <= _.StartDate
                                                      && _.StartDate < currentTimeSlot.EndDateTime)
                                .ToList();
                            inspectionSchedule.IsClosed = existentInspections.Any(_ => _.EndDate != null);
                            result.Add(inspectionSchedule);
                        }
                    }
                });

                return result;
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return new List<InspectionSchedule>();
            }
        }


        /// <summary>
        /// Get all inspections schedule
        /// </summary>
        public async Task<IEnumerable<InspectionSchedule>> GetInspectionSchedulesNonFilter(int? InspectionScheduleId = null)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                if (InspectionScheduleId == null)
                    return await context.InspectionSchedules
                        .Include(nameof(InspectionSchedule.Procedure))
                        .Include(nameof(InspectionSchedule.Timeslot))
                        .Where(_ => _.Timeslot.IsDeleted != true)
                        .ToArrayAsync();
                return await context.InspectionSchedules
                    .Include(nameof(InspectionSchedule.Procedure))
                    .Include(nameof(InspectionSchedule.Timeslot))
                    .Where(_ => _.InspectionScheduleId == InspectionScheduleId && _.Timeslot.IsDeleted != true)
                    .ToArrayAsync();
            }
        }

        /// <summary>
        /// Get all inspections schedule for timeslot
        /// </summary>
        public async Task<IEnumerable<InspectionSchedule>> GetInspectionsScheduleForTimeslot(int timeslotId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                return await context.InspectionSchedules
                    .Where(_ => _.TimeslotId == timeslotId)
                    .ToArrayAsync();
            }
        }

        /// <summary>
        /// Sauvegarde les inspections
        /// </summary>
        /// <param name="inspections">Liste des inspections à sauvegarder</param>
        /// <returns>La liste des inspections sauvegardées</returns>
        public async Task<Inspection[]> SaveInspections(Inspection[] inspections)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                if (inspections != null)
                {
                    foreach (var inspection in inspections)
                    {
                        context.Inspections.ApplyChanges(inspection);
                    }

                    await context.SaveChangesAsync();
                }

                return inspections;
            }
        }

        /// <summary>
        /// Save inspection schedule
        /// </summary>
        /// <param name="schedule">Schedule of Inspection</param>
        /// <returns>Saved inspection schedule</returns>
        public async Task<InspectionSchedule> SaveInspectionSchedule(InspectionSchedule schedule)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                if (schedule != null)
                {
                    context.InspectionSchedules.ApplyChanges(schedule);

                    await context.SaveChangesAsync();
                }
                return schedule;
            }
        }

        public async Task<Anomaly[]> GetAnomalies(int inspectionId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var inspection = await context.Inspections
                    .Include(nameof(Inspection.Anomalies))
                    .Include($"{nameof(Inspection.Anomalies)}.{nameof(Anomaly.Inspector)}")
                    .Include($"{nameof(Inspection.Anomalies)}.{nameof(Anomaly.InspectionSteps)}")
                    .Include($"{nameof(Inspection.Anomalies)}.{nameof(Anomaly.InspectionSteps)}.{nameof(InspectionStep.PublishedAction)}")

                    .SingleOrDefaultAsync(_ => _.InspectionId == inspectionId);
                return inspection?.Anomalies.ToArray();
            }
        }

        public async Task<Anomaly> GetAnomaly(int AnomalyId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var anomaly = await context.Anomalies
                    .SingleOrDefaultAsync(_ => _.Id == AnomalyId);
                return anomaly;
            }
        }

        #endregion

        #region Documentation

        /// <summary>
        /// Retrieve the draft documentation for this scenario id
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public async Task<bool> HasDocumentationDraft(int scenarioId)
        {
            return await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                {
                    return await context.DocumentationDrafts
                        .AnyAsync(u => u.ScenarioId == scenarioId);
                }
            });
        }

        /// <summary>
        /// Retrieve the draft documentation for this scenario id
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public bool HasDocumentationDraftSync(int scenarioId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
            {
                return context.DocumentationDrafts
                    .Any(u => u.ScenarioId == scenarioId);
            }
        }

        /// <summary>
        /// Retrieve for the scenarioId or the most recent draft documentation for the process
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public async Task<DocumentationDraft> GetLastDocumentationDraft(int processId, int scenarioId)
        {
            int draftId = default;
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
            {
                var scenarioCreationDate = await context.Scenarios
                    .AsNoTracking()
                    .Where(_ => _.ScenarioId == scenarioId)
                    .Select(_ => _.CreationDate)
                    .SingleAsync();
                try
                {
                    var processScenarios = await context.Scenarios
                        .Include(nameof(Scenario.Project))
                        .Include(nameof(Scenario.DocumentationDrafts))
                        .AsNoTracking()
                        .Where(_ => !_.IsDeleted
                            && !_.Project.IsDeleted
                            && _.Project.ProcessId == processId
                            && _.DocumentationDrafts.Any())
                        .ToListAsync();
                    var lastScenarioWithDraft = processScenarios.Where(_ => _.CreationDate <= scenarioCreationDate)
                        .MaxBy(_ => _.CreationDate)
                        .FirstOrDefault();
                    draftId = lastScenarioWithDraft?.DocumentationDrafts.First().DocumentationDraftId ?? 0;
                }
                catch(Exception ex)
                {
                    _traceManager.TraceError(ex, $"Error on {nameof(GetLastDocumentationDraft)}({nameof(processId)}:{processId}, {nameof(scenarioId)}:{scenarioId})");
                }
            }

            if (draftId == default) // No draft, have to create a new one
            {
                // TODO : Check if IsMarkAsAdded is correctly set
                var documentation = new DocumentationDraft
                {
                    ScenarioId = scenarioId,
                    ActiveVideoExport = false,
                    SlowMotion = false,
                    SlowMotionDuration = 5,
                    WaterMarking = false,
                    WaterMarkingVAlign = EVerticalAlign.Top,
                    WaterMarkingHAlign = EHorizontalAlign.Center,
                    Formation_IsMajor = false,
                    Evaluation_IsMajor = false,
                    Inspection_IsMajor = false,
                    Formation_DispositionAsJson = "{}",
                    Evaluation_DispositionAsJson = "{}",
                    Inspection_DispositionAsJson = "{}"
                };
                List<KAction> allActions = new List<KAction>();
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                {
                    allActions = await context.KActions
                        .AsNoTracking()
                        .Where(a => a.ScenarioId == scenarioId)
                        .ToListAsync();
                }
                var trainingActionsDraftWBS = allActions.Select(a => new DocumentationActionDraftWBS
                {
                    DocumentationPublishMode = (int)PublishModeEnum.Formation,
                    ActionId = a.ActionId,
                    WBS = a.WBS,
                    TreeId = a.ActionId
                });
                var evaluationActionsDraftWBS = allActions.Select(a => new DocumentationActionDraftWBS
                {
                    DocumentationPublishMode = (int)PublishModeEnum.Evaluation,
                    ActionId = a.ActionId,
                    WBS = a.WBS,
                    TreeId = a.ActionId
                });
                var inspectionActionsDraftWBS = allActions.Select(a => new DocumentationActionDraftWBS
                {
                    DocumentationPublishMode = (int)PublishModeEnum.Inspection,
                    ActionId = a.ActionId,
                    WBS = a.WBS,
                    TreeId = a.ActionId
                });
                documentation.DocumentationActionDraftWBS.AddRange(trainingActionsDraftWBS);
                documentation.DocumentationActionDraftWBS.AddRange(evaluationActionsDraftWBS);
                documentation.DocumentationActionDraftWBS.AddRange(inspectionActionsDraftWBS);

                foreach (var action in documentation.DocumentationActionDraftWBS)
                {
                    action.IsGroup = WBSHelper.HasChildren(action, documentation.DocumentationActionDraftWBS.Where(_ => _.DocumentationPublishMode == action.DocumentationPublishMode));
                    action.ParentId = WBSHelper.GetParent(action, documentation.DocumentationActionDraftWBS.Where(_ => _.DocumentationPublishMode == action.DocumentationPublishMode))?.TreeId;
                    if (action.IsGroup)
                        action.IsExpanded = true;
                }
                return documentation;
            }
            else
            {
                var documentation = await GetDocumentationDraft(draftId);
                if (documentation.ScenarioId != scenarioId)
                {
                    var newDocumentation = new DocumentationDraft
                    {
                        ScenarioId = scenarioId,
                        ActiveVideoExport = documentation.ActiveVideoExport,
                        SlowMotion = documentation.SlowMotion,
                        SlowMotionDuration = documentation.SlowMotionDuration,
                        WaterMarking = documentation.WaterMarking,
                        WaterMarkingVAlign = documentation.WaterMarkingVAlign,
                        WaterMarkingHAlign = documentation.WaterMarkingHAlign,
                        Formation_IsMajor = documentation.Formation_IsMajor,
                        Evaluation_IsMajor = documentation.Evaluation_IsMajor,
                        Inspection_IsMajor = documentation.Inspection_IsMajor,
                        Formation_DispositionAsJson = documentation.Formation_DispositionAsJson,
                        Evaluation_DispositionAsJson = documentation.Evaluation_DispositionAsJson,
                        Inspection_DispositionAsJson = documentation.Inspection_DispositionAsJson,
                        IsFromPrevious = true
                    };
                    documentation.DocumentationActionDraftWBS.ForEach(a =>
                    {
                        newDocumentation.DocumentationActionDraftWBS.Add(new DocumentationActionDraftWBS(a));
                    });
                    return newDocumentation;
                }
                return documentation;
            }
        }

        /// <summary>
        /// Retrieve the draft documentation
        /// </summary>
        /// <param name="documentationDraftId"></param>
        /// <returns></returns>
        public async Task<DocumentationDraft> GetDocumentationDraft(int documentationDraftId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
            {
                var documentation = await context.DocumentationDrafts
                    .Include(nameof(DocumentationDraft.DocumentationActionDraftWBS))
                    .Include($"{nameof(DocumentationDraft.DocumentationActionDraftWBS)}.{nameof(DocumentationActionDraftWBS.DocumentationActionDraft)}")
                    .Include($"{nameof(DocumentationDraft.DocumentationActionDraftWBS)}.{nameof(DocumentationActionDraftWBS.DocumentationActionDraft)}.{nameof(DocumentationActionDraft.Thumbnail)}")
                    .Include($"{nameof(DocumentationDraft.DocumentationActionDraftWBS)}.{nameof(DocumentationActionDraftWBS.DocumentationActionDraft)}.{nameof(DocumentationActionDraft.ReferentialDocumentations)}")
                    .SingleOrDefaultAsync(u => u.DocumentationDraftId == documentationDraftId);

                foreach(var action in documentation.DocumentationActionDraftWBS)
                {
                    action.TreeId = action.DocumentationActionDraftWBSId;
                    action.IsGroup = WBSHelper.HasChildren(action, documentation.DocumentationActionDraftWBS.Where(_ => _.DocumentationPublishMode == action.DocumentationPublishMode));
                    action.ParentId = WBSHelper.GetParent(action, documentation.DocumentationActionDraftWBS.Where(_ => _.DocumentationPublishMode == action.DocumentationPublishMode))?.DocumentationActionDraftWBSId;
                    if (action.IsGroup)
                        action.IsExpanded = true;
                }

                return documentation;
            }
        }

        public async Task<IEnumerable<ProjectReferential>> GetUsedReferentials(int projectId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
            {
                var usedReferentials = await context.ProjectReferentials
                    .Where(_ => _.ProjectId == projectId
                        && _.IsEnabled)
                    .ToListAsync();

                return usedReferentials;
            }
        }

        public async Task<long> GetProjectTimeScale(int scenarioId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
            {
                var timescale = await context.Scenarios
                    .Include(nameof(Scenario.Project))
                    .Where(_ => _.ScenarioId == scenarioId)
                    .Select(_ => _.Project.TimeScale)
                    .SingleOrDefaultAsync();

                return timescale;
            }
        }

        /// <summary>
        /// Save documentation draft
        /// </summary>
        /// <param name="documentationDraft"></param>  
        /// <returns></returns>
        public async Task<DocumentationDraft> SaveDocumentationDraft(DocumentationDraft documentationDraft)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                if (documentationDraft.IsMarkedAsAdded)
                {
                    context.DocumentationDrafts.AddObject(documentationDraft);
                }
                else
                {
                    context.ApplyChanges(nameof(context.DocumentationDrafts), documentationDraft);
                }
                await context.SaveChangesAsync();

                return documentationDraft;
            }
        }

        /// <summary>
        /// Save documentation draft information for a specific project id and scenario id
        /// </summary>
        /// <param name="documentation"></param>    
        /// <param name="projectId"></param>
        /// <param name="actions"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public async Task SaveDocumentationDraft(DocumentationDraft documentation, List<DocumentationActionDraft> actions, List<DocumentationActionDraftWBS> actionsWbs, int projectId, int scenarioId)
        {
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    DocumentationDraft draft = null;
                    if (documentation.IsMarkedAsAdded)
                    {
                        draft = documentation;
                    }
                    else if (documentation.IsMarkedAsModified) // Clean all the draft actions before saving
                    {
                        draft = await context.DocumentationDrafts
                            .Include(nameof(DocumentationDraft.DocumentationActionDraftWBS))
                            .Include($"{nameof(DocumentationDraft.DocumentationActionDraftWBS)}.{nameof(DocumentationActionDraftWBS.DocumentationActionDraft)}")
                            .Include($"{nameof(DocumentationDraft.DocumentationActionDraftWBS)}.{nameof(DocumentationActionDraftWBS.DocumentationActionDraft)}.{nameof(DocumentationActionDraft.ReferentialDocumentations)}")
                            .SingleAsync(u => u.DocumentationDraftId == documentation.DocumentationDraftId);

                        draft.ScenarioId = documentation.ScenarioId;
                        draft.Evaluation_DispositionAsJson = documentation.Evaluation_DispositionAsJson;
                        draft.Formation_DispositionAsJson = documentation.Formation_DispositionAsJson;
                        draft.Inspection_DispositionAsJson = documentation.Inspection_DispositionAsJson;
                        draft.ActiveVideoExport = documentation.ActiveVideoExport;
                        draft.SlowMotion = documentation.SlowMotion;
                        draft.SlowMotionDuration = documentation.SlowMotionDuration;
                        draft.WaterMarking = documentation.WaterMarking;
                        draft.WaterMarkingHAlign = documentation.WaterMarkingHAlign;
                        draft.WaterMarkingVAlign = documentation.WaterMarkingVAlign;
                        draft.WaterMarkingText = documentation.WaterMarkingText;
                        draft.Formation_ReleaseNote = documentation.Formation_ReleaseNote;
                        draft.Inspection_ReleaseNote = documentation.Inspection_ReleaseNote;
                        draft.Evaluation_ReleaseNote = documentation.Evaluation_ReleaseNote;
                        draft.Formation_IsMajor = documentation.Formation_IsMajor;
                        draft.Inspection_IsMajor = documentation.Inspection_IsMajor;
                        draft.Evaluation_IsMajor = documentation.Evaluation_IsMajor;
                        draft.Formation_ActionDisposition = documentation.Formation_ActionDisposition;
                        draft.Inspection_ActionDisposition = documentation.Inspection_ActionDisposition;
                        draft.Evaluation_ActionDisposition = documentation.Evaluation_ActionDisposition;

                        foreach (var item in draft.DocumentationActionDraftWBS)
                        {
                            if (item.DocumentationActionDraft != null)
                            {
                                foreach (var referential in item.DocumentationActionDraft.ReferentialDocumentations)
                                    referential.MarkAsDeleted();
                                item.DocumentationActionDraft.MarkAsDeleted();
                            }
                            item.MarkAsDeleted();
                        }
                    }

                    foreach (var item in actionsWbs)
                    {
                        draft.DocumentationActionDraftWBS.Add(item);
                    }

                    context.DocumentationDrafts.ApplyChanges(draft);
                    await context.SaveChangesAsync();
                }
            });
        }

        public async Task SaveDocumentationVideos(int documentationDraftId,
            bool activeVideoExport,
            bool slowMotion,
            double slowMotionDuration,
            bool waterMarking,
            string waterMarkingText,
            EVerticalAlign waterMarkingVAlign,
            EHorizontalAlign waterMarkingHAlign)
        {
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    var draft = await context.DocumentationDrafts.SingleAsync(u => u.DocumentationDraftId == documentationDraftId);

                    draft.ActiveVideoExport = activeVideoExport;
                    draft.SlowMotion = activeVideoExport ? slowMotion : false;
                    draft.SlowMotionDuration = draft.SlowMotion == true ? Convert.ToDecimal(slowMotionDuration) : (decimal?)null;
                    draft.WaterMarking = activeVideoExport ? waterMarking : false;
                    draft.WaterMarkingText = draft.WaterMarking == true ? waterMarkingText : null;
                    draft.WaterMarkingVAlign = draft.WaterMarking == true ? waterMarkingVAlign : (EVerticalAlign?)null;
                    draft.WaterMarkingHAlign = draft.WaterMarking == true ? waterMarkingHAlign : (EHorizontalAlign?)null;

                    await context.SaveChangesAsync();
                }
            });
        }

        #endregion

    }
}