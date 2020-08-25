using Kprocess.KL2.FileTransfer;
using KProcess.KL2.APIClient;
using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.KL2.WebAdmin.Mapper;
using KProcess.KL2.WebAdmin.Models.Action;
using KProcess.KL2.WebAdmin.Models.Documentation;
using KProcess.KL2.WebAdmin.Models.Procedure;
using KProcess.KL2.WebAdmin.Utils;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Helpers;
using KProcess.Ksmed.Models.Interfaces;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Business;
using MoreLinq;
using Newtonsoft.Json;
using SyncfusionHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using Publication = KProcess.Ksmed.Models.Publication;

namespace KProcess.KL2.WebAdmin.Controllers
{
    [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Documentalist)]
    [SettingUserContextFilter]
    public class DocumentationController : LocalizedController
    {
        readonly IPrepareService _prepareService;
        readonly IReferentialsService _referentialsService;
        readonly IPublicationService _publicationService;
        readonly IApplicationUsersService _applicationUsersService;
        readonly ITraceManager _traceManager;
        readonly ISecurityContext _securityContext;
        readonly IAuthenticationService _authenticationService;
        readonly IAPIHttpClient _apiHttpClient;

        public DocumentationController(IPrepareService prepareService,
            IReferentialsService referentialsService,
            IPublicationService publicationService,
            IApplicationUsersService applicationUsersService,
            ITraceManager traceManager,
            ISecurityContext securityContext, 
            IAuthenticationService authenticationService,
            IAPIHttpClient apiHttpClient,
            ILocalizationManager localizationManager
            )
            : base(localizationManager)
        {
            _prepareService = prepareService;
            _referentialsService = referentialsService;
            _publicationService = publicationService;
            _applicationUsersService = applicationUsersService;
            _traceManager = traceManager;
            _securityContext = securityContext;
            _authenticationService = authenticationService;
            _apiHttpClient = apiHttpClient;
        }

        // GET: Documentation
        public Task<ActionResult> Index(bool partial = false) =>
            Task.Run<ActionResult>(() =>
            {
                // TODO : Manage partial view
                if (partial)
                    return PartialView();
                return View();
            });

        public Task<ActionResult> DocumentationHeader(string processName, string projectName) =>
            Task.Run<ActionResult>(() =>
            {
                return PartialView(new HeaderModel { Process = processName, Project = projectName });
            });

        public async Task<ActionResult> DocumentationProcess()
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;

            INode[] publicationsTree = await _prepareService.GetProcessTreeWithScenario();
            
            var publicationInProgress = await _publicationService.GetProcessesPublicationStatus(publicationsTree.Flatten(n => n.Nodes)
                .Where(n => n is Procedure)
                .Cast<Procedure>()
                .Select(p => p.ProcessId)
                .ToArray());

            var nodes = new List<ProcedureNodeViewModel>
            {
                new ProcedureNodeViewModel
                {
                    Id = "-1",
                    Name = LocalizedStrings.GetString("All_Publications_Label"),
                    ParentId = null,
                    IsExpanded = true,
                    HasChild = publicationsTree != null && publicationsTree.Any(),
                    Sprite = "fa fa-folder"
                }
            };
            string ProcessLabel = "", ProjectName = "";
            foreach (var publicationTree in publicationsTree.Flatten(n => n.Nodes).OrderBy(n => n.Label))
            {
                if (publicationTree is ProjectDir)
                {
                    var projectDir = publicationTree as ProjectDir;
                    nodes.Add(new ProcedureNodeViewModel
                    {
                        Id = "d" + projectDir.Id,
                        Name = projectDir.Name,
                        ParentId = projectDir.ParentId != null ? "d" + projectDir.ParentId : "-1",
                        IsExpanded = true,
                        HasChild = projectDir.Nodes != null && projectDir.Nodes.Any(),
                        Sprite = "fa fa-folder"
                    });
                }
                else if (publicationTree is Procedure)
                {
                    var procedure = publicationTree as Procedure;
                    var status = publicationInProgress[procedure.ProcessId];
                    nodes.Add(new ProcedureNodeViewModel
                    {
                        Id = "p" + procedure.ProcessId,
                        Name = procedure.Label,
                        ParentId = procedure.ProjectDirId != null ? "d" + procedure.ProjectDirId : "-1",
                        HasChild = false,
                        Sprite = "fa fa-file-o",
                        Status = status,
                        StateAsLabel = status != null ? LocalizedStrings.GetString($"View_DocumentationHistory_State_{status.Value.ToString()}") : "",
                        StateAsString = status != null ? status.Value.ToString().ToLower() : "",
                        NodeProperty = status != null && (status.Value == PublicationStatus.Waiting || status.Value == PublicationStatus.InProgress)
                            ? new Dictionary<string, object>() { ["class"] = "disabledNode", ["d-ProcessId"] = procedure.ProcessId, ["d-NodeProjectId"] = procedure.NodeProjectId, ["d-NodeScenarioId"] = procedure.NodeScenarioId }
                            : new Dictionary<string, object>() { ["d-ProcessId"] = procedure.ProcessId, ["d-NodeProjectId"] = procedure.NodeProjectId, ["d-NodeScenarioId"] = procedure.NodeScenarioId }
                    });
                }
            }

            var model = new DocumentationManageViewModel
            {
                TreeNode = nodes,
                ProcessLabel = ProcessLabel,
                ProjectName = ProjectName,
                ProcessId = null
            };

            return PartialView(model);
        }

        public async Task<ActionResult> DocumentationReferentials(int processId, int projectId, int scenarioId)
        {
            ViewBag.ProcessName = await _prepareService.GetProcessName(processId);
            ViewBag.ProjectName = await _prepareService.GetProjectName(projectId);
            ViewBag.DocumentationReferentials = await GetDocumentationReferentials(processId, projectId, scenarioId);
            var modelReferentials = new ManageReferentialsViewModel
            {
                ProcessId = processId
            };
            return PartialView(modelReferentials);
        }

        public async Task<ActionResult> DocumentationFormat(int processId, int projectId, int scenarioId,
            bool? publishForTraining, bool? publishForEvaluation, bool? publishForInspection,
            int? documentationDraftId)
        {
            DocumentationDraft draft = null;
            if (documentationDraftId.HasValue)
                draft = await _prepareService.GetDocumentationDraft(documentationDraftId.Value);
            else
                draft = await _prepareService.GetLastDocumentationDraft(processId, scenarioId);

            var usedReferentials = await _prepareService.GetUsedReferentials(projectId);
            var headers = await DocumentationMapper.GetHeaders(processId, _referentialsService, false);

            var skills = (await _referentialsService.LoadSkills())
                .OrderBy(s => s.Label)
                .ToList();
            skills.Insert(0, new Skill { Id = -1, Label = LocalizedStrings.GetString("View_Documentation_NoSkills") });

            var analystActions = await _prepareService.GetActionsByScenario(scenarioId);
            var referentials = await GetReferentialsByScenario(processId);
            var projectTimeScale = await _prepareService.GetProjectTimeScale(scenarioId);
            foreach (var action in draft.DocumentationActionDraftWBS)
            {
                if (action.IsDocumentation)
                    action.DocumentationActionDraft.DurationString = DurationHelper.GetTimeScaleMaskValue(projectTimeScale, action.DocumentationActionDraft.Duration);
            }

            ViewBag.ProcessName = await _prepareService.GetProcessName(processId);
            ViewBag.ProjectName = await _prepareService.GetProjectName(projectId);
            ViewBag.DocumentationDraftId = draft.DocumentationDraftId;
            ViewBag.Draft = draft;
            ViewBag.Skills = skills;
            ViewBag.Referentials = referentials;
            ViewBag.ProjectTimeScale = projectTimeScale;
            ViewBag.AnalystActions = analystActions;

            var modelFormat = new FormatViewModel
            {
                ProcessId = processId,
                ProjectId = projectId,
                ScenarioId = scenarioId,
                PublishForList = new List<string>(),
                //Actions = new List<GenericActionViewModel>(),
                FormationActionHeaders = OrderActionHeaders(headers, draft.Formation_DispositionAsJson, PublishModeEnum.Formation),
                EvaluationActionHeaders = OrderActionHeaders(headers, draft.Evaluation_DispositionAsJson, PublishModeEnum.Evaluation),
                InspectionActionHeaders = OrderActionHeaders(headers, draft.Inspection_DispositionAsJson, PublishModeEnum.Inspection),
                TrainingFormatAsJson = draft.Formation_DispositionAsJson,
                EvaluationFormatAsJson = draft.Evaluation_DispositionAsJson,
                InspectionFormatAsJson = draft.Inspection_DispositionAsJson,
                IsDraftForPreviousScenario = draft.IsFromPrevious,
                PublishForTraining = draft.DocumentationActionDraftWBS.Any(_ => _.DocumentationPublishMode == (int)PublishModeEnum.Formation),
                PublishForEvaluation = draft.DocumentationActionDraftWBS.Any(_ => _.DocumentationPublishMode == (int)PublishModeEnum.Evaluation),
                PublishForInspection = draft.DocumentationActionDraftWBS.Any(_ => _.DocumentationPublishMode == (int)PublishModeEnum.Inspection),
            };

            if (modelFormat.IsDraftForPreviousScenario)
            {
                // Need to know if the changes are minor or major
                bool changesAreMinor = true;
                if (modelFormat.PublishForTraining)
                    changesAreMinor = draft.DocumentationActionDraftWBS
                        .Where(_ => _.DocumentationActionDraft == null && _.DocumentationPublishMode == (int)PublishModeEnum.Formation)
                        .All(_ => analystActions.Any(a => a.ActionId == _.ActionId));
                else
                    changesAreMinor = draft.DocumentationActionDraftWBS
                        .Where(_ => _.DocumentationActionDraft == null && _.DocumentationPublishMode == (int)PublishModeEnum.Inspection)
                        .All(_ => analystActions.Any(a => a.ActionId == _.ActionId));

                if (changesAreMinor)
                {
                    // We can keep actions unchanged
                    ViewBag.FormationActions = draft.DocumentationActionDraftWBS
                        .Where(_ => _.DocumentationPublishMode == (int)PublishModeEnum.Formation)
                        .OrderBy(a => a, new WBSComparer())
                        .ToList();
                    ViewBag.EvaluationActions = draft.DocumentationActionDraftWBS
                        .Where(_ => _.DocumentationPublishMode == (int)PublishModeEnum.Evaluation)
                        .OrderBy(a => a, new WBSComparer())
                        .ToList();
                    ViewBag.InspectionActions = draft.DocumentationActionDraftWBS
                        .Where(_ => _.DocumentationPublishMode == (int)PublishModeEnum.Inspection)
                        .OrderBy(a => a, new WBSComparer())
                        .ToList();
                }
                else
                {
                    // Get previous documentation actions
                    modelFormat.PreviousTrainingDocumentationActions = draft.DocumentationActionDraftWBS
                        .Where(_ => _.DocumentationActionDraft != null && _.DocumentationPublishMode == (int)PublishModeEnum.Formation)
                        .OrderBy(a => a, new WBSComparer())
                        .ToList();
                    modelFormat.PreviousEvaluationDocumentationActions = draft.DocumentationActionDraftWBS
                        .Where(_ => _.DocumentationActionDraft != null && _.DocumentationPublishMode == (int)PublishModeEnum.Evaluation)
                        .OrderBy(a => a, new WBSComparer())
                        .ToList();
                    modelFormat.PreviousInspectionDocumentationActions = draft.DocumentationActionDraftWBS
                        .Where(_ => _.DocumentationActionDraft != null && _.DocumentationPublishMode == (int)PublishModeEnum.Inspection)
                        .OrderBy(a => a, new WBSComparer())
                        .ToList();

                    // Get new analyst actions
                    List<DocumentationActionDraftWBS> allActionsDraftWBS = new List<DocumentationActionDraftWBS>();
                    if (modelFormat.PublishForTraining)
                    {
                        var trainingActionsDraftWBS = analystActions.Select(a => new DocumentationActionDraftWBS
                        {
                            DocumentationPublishMode = (int)PublishModeEnum.Formation,
                            ActionId = a.ActionId,
                            WBS = a.WBS,
                            TreeId = a.ActionId
                        }).ToList();
                        trainingActionsDraftWBS.ForEach(action =>
                        {
                            action.IsGroup = WBSHelper.HasChildren(action, trainingActionsDraftWBS);
                            action.ParentId = WBSHelper.GetParent(action, trainingActionsDraftWBS)?.TreeId;
                            action.IsExpanded = action.IsGroup;
                        });
                        allActionsDraftWBS.AddRange(trainingActionsDraftWBS);
                        // Affect TreeIds of previous documentation actions
                        var firstAvailableId = trainingActionsDraftWBS.Select(a => a.TreeId).Max() + 1;
                        modelFormat.PreviousTrainingDocumentationActions.ForEach(a =>
                        {
                            a.IsGroup = false;
                            a.IsExpanded = false;
                            a.ParentId = null;
                            a.TreeId = firstAvailableId++;
                        });
                    }
                    if (modelFormat.PublishForEvaluation)
                    {
                        var evaluationActionsDraftWBS = analystActions.Select(a => new DocumentationActionDraftWBS
                        {
                            DocumentationPublishMode = (int)PublishModeEnum.Evaluation,
                            ActionId = a.ActionId,
                            WBS = a.WBS,
                            TreeId = a.ActionId
                        }).ToList();
                        evaluationActionsDraftWBS.ForEach(action =>
                        {
                            action.IsGroup = WBSHelper.HasChildren(action, evaluationActionsDraftWBS);
                            action.ParentId = WBSHelper.GetParent(action, evaluationActionsDraftWBS)?.TreeId;
                            action.IsExpanded = action.IsGroup;
                        });
                        allActionsDraftWBS.AddRange(evaluationActionsDraftWBS);
                        // Affect TreeIds of previous documentation actions
                        var firstAvailableId = evaluationActionsDraftWBS.Select(a => a.TreeId).Max() + 1;
                        modelFormat.PreviousEvaluationDocumentationActions.ForEach(a =>
                        {
                            a.IsGroup = false;
                            a.IsExpanded = false;
                            a.ParentId = null;
                            a.TreeId = firstAvailableId++;
                        });
                    }
                    if (modelFormat.PublishForInspection)
                    {
                        var inspectionActionsDraftWBS = analystActions.Select(a => new DocumentationActionDraftWBS
                        {
                            DocumentationPublishMode = (int)PublishModeEnum.Inspection,
                            ActionId = a.ActionId,
                            WBS = a.WBS,
                            TreeId = a.ActionId
                        }).ToList();
                        inspectionActionsDraftWBS.ForEach(action =>
                        {
                            action.IsGroup = WBSHelper.HasChildren(action, inspectionActionsDraftWBS);
                            action.ParentId = WBSHelper.GetParent(action, inspectionActionsDraftWBS)?.TreeId;
                            action.IsExpanded = action.IsGroup;
                        });
                        allActionsDraftWBS.AddRange(inspectionActionsDraftWBS);
                        // Affect TreeIds of previous documentation actions
                        var firstAvailableId = inspectionActionsDraftWBS.Select(a => a.TreeId).Max() + 1;
                        modelFormat.PreviousInspectionDocumentationActions.ForEach(a =>
                        {
                            a.IsGroup = false;
                            a.IsExpanded = false;
                            a.ParentId = null;
                            a.TreeId = firstAvailableId++;
                        });
                    }
                    ViewBag.FormationActions = allActionsDraftWBS
                        .Where(_ => _.DocumentationActionDraft == null && _.DocumentationPublishMode == (int)PublishModeEnum.Formation)
                        .OrderBy(a => a, new WBSComparer())
                        .ToList();
                    ViewBag.EvaluationActions = allActionsDraftWBS
                        .Where(_ => _.DocumentationActionDraft == null && _.DocumentationPublishMode == (int)PublishModeEnum.Evaluation)
                        .OrderBy(a => a, new WBSComparer())
                        .ToList();
                    ViewBag.InspectionActions = allActionsDraftWBS
                        .Where(_ => _.DocumentationActionDraft == null && _.DocumentationPublishMode == (int)PublishModeEnum.Inspection)
                        .OrderBy(a => a, new WBSComparer())
                        .ToList();
                }
            }
            else
            {
                ViewBag.FormationActions = draft.DocumentationActionDraftWBS
                    .Where(_ => _.DocumentationPublishMode == (int)PublishModeEnum.Formation)
                    .OrderBy(a => a, new WBSComparer())
                    .ToList();
                ViewBag.EvaluationActions = draft.DocumentationActionDraftWBS
                    .Where(_ => _.DocumentationPublishMode == (int)PublishModeEnum.Evaluation)
                    .OrderBy(a => a, new WBSComparer())
                    .ToList();
                ViewBag.InspectionActions = draft.DocumentationActionDraftWBS
                    .Where(_ => _.DocumentationPublishMode == (int)PublishModeEnum.Inspection)
                    .OrderBy(a => a, new WBSComparer())
                    .ToList();
            }

            if (modelFormat.PublishForTraining)
                modelFormat.PublishForList.Add("Training");
            if (modelFormat.PublishForEvaluation)
                modelFormat.PublishForList.Add("Evaluation");
            if (modelFormat.PublishForInspection)
                modelFormat.PublishForList.Add("Inspection");

            return PartialView(modelFormat);
        }

        public async Task<ActionResult> DocumentationFormatActions(int processId, int projectId, int scenarioId,
            bool publishForTraining, bool publishForEvaluation, bool publishForInspection,
            int documentationDraftId)
        {
            var draft = await _prepareService.GetDocumentationDraft(documentationDraftId);

            ViewBag.ProcessName = await _prepareService.GetProcessName(processId);
            ViewBag.ProjectName = await _prepareService.GetProjectName(projectId);

            var documentationReferentials = (await GetDocumentationReferentials(processId, projectId, scenarioId))
                .Where(_ => _.ReferentialId != ProcessReferentialIdentifier.Operator
                    && _.ReferentialId != ProcessReferentialIdentifier.Equipment)
                .ToList();
            var model = new FormatActionsViewModel
            {
                ProcessId = processId,
                ScenarioId = scenarioId,
                TrainingReferentials = await GetAllReferentials(documentationReferentials, draft.Formation_ActionDisposition),
                EvaluationReferentials = await GetAllReferentials(documentationReferentials, draft.Evaluation_ActionDisposition),
                InspectionReferentials = await GetAllReferentials(documentationReferentials, draft.Inspection_ActionDisposition),
                PublishForTraining = publishForTraining,
                PublishForEvaluation = publishForEvaluation,
                PublishForInspection = publishForInspection
            };

            return PartialView(model);
        }

        public async Task<ActionResult> DocumentationVideos(int processId, int projectId,
            int documentationDraftId)
        {
            var draft = await _prepareService.GetDocumentationDraft(documentationDraftId);

            ViewBag.ProcessName = await _prepareService.GetProcessName(processId);
            ViewBag.ProjectName = await _prepareService.GetProjectName(projectId);

            return PartialView(new VideoViewModel
            {
                cbVideoExport = draft.ActiveVideoExport ?? false,
                cbSlowMotion = draft.SlowMotion ?? false,
                tbSlowDuration = draft.SlowMotionDuration.HasValue ? (int)draft.SlowMotionDuration.Value : 5,
                cbWatermarking = draft.WaterMarking ?? false,
                verticalAlign = draft.WaterMarkingVAlign.HasValue ? (int)draft.WaterMarkingVAlign.Value : (int)EVerticalAlign.Top,
                horizontalAlign = draft.WaterMarkingHAlign.HasValue ? (int)draft.WaterMarkingHAlign.Value : (int)EHorizontalAlign.Center,
                tbWatermarkText = draft.WaterMarkingText
            });
        }

        public async Task<ActionResult> DocumentationSummary(int processId, int projectId, int scenarioId,
            bool publishForTraining, bool publishForEvaluation, bool publishForInspection,
            int documentationDraftId)
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;

            var draft = await _prepareService.GetDocumentationDraft(documentationDraftId);

            ViewBag.ProcessName = await _prepareService.GetProcessName(processId);
            ViewBag.ProjectName = await _prepareService.GetProjectName(projectId);
            
            ViewBag.TrainingMinorVersion = await _publicationService.GetFutureVersion(processId, PublishModeEnum.Formation, false);
            ViewBag.EvaluationMinorVersion = await _publicationService.GetFutureVersion(processId, PublishModeEnum.Evaluation, false);
            ViewBag.InspectionMinorVersion = await _publicationService.GetFutureVersion(processId, PublishModeEnum.Inspection, false);
            ViewBag.TrainingMajorVersion = await _publicationService.GetFutureVersion(processId, PublishModeEnum.Formation, true);
            ViewBag.EvaluationMajorVersion = await _publicationService.GetFutureVersion(processId, PublishModeEnum.Evaluation, true);
            ViewBag.InspectionMajorVersion = await _publicationService.GetFutureVersion(processId, PublishModeEnum.Inspection, true);

            return PartialView(new SummaryViewModel
            {
                ProcessId = processId,
                ScenarioId = scenarioId,
                PublishForTraining = publishForTraining,
                PublishForInspection = publishForInspection,
                PublishForEvaluation = publishForEvaluation,
                TrainingReleaseNote = draft.Formation_ReleaseNote,
                InspectionReleaseNote = draft.Inspection_ReleaseNote,
                EvaluationReleaseNote = draft.Evaluation_ReleaseNote,
                TrainingVersioningIsMajor = ViewBag.TrainingMinorVersion == ViewBag.TrainingMajorVersion || draft.Formation_IsMajor,
                EvaluationVersioningIsMajor = ViewBag.EvaluationMinorVersion == ViewBag.EvaluationMajorVersion || draft.Evaluation_IsMajor,
                InspectionVersioningIsMajor = ViewBag.InspectionMinorVersion == ViewBag.InspectionMajorVersion || draft.Inspection_IsMajor
            });
        }

        /// <summary>
        /// Reorder action headers based on draft order savec in database
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="dispositionAsJson"></param>
        /// <returns></returns>
        private Dictionary<string, ActionHeader> OrderActionHeaders(Dictionary<string, string> headers, string dispositionAsJson, PublishModeEnum publishMode)
        {
            var evaluationMandatoryHeaders = new List<string> { HeadersHelper.IsQualified, HeadersHelper.IsNotQualified, HeadersHelper.QualificationStep_Comment };

            var sortedActionHeaders = new Dictionary<string, ActionHeader>();
            if (string.IsNullOrEmpty(dispositionAsJson) || dispositionAsJson == "{}")
            {
                sortedActionHeaders = headers.ToDictionary(kpv => kpv.Key, kpv => new ActionHeader
                {
                    Label = kpv.Value,
                    IsVisible = publishMode.ShouldBeVisible(kpv.Key),
                    Width = "0%"
                });
                if (publishMode == PublishModeEnum.Evaluation)
                {
                    sortedActionHeaders[HeadersHelper.IsQualified] = new ActionHeader
                    {
                        Label = LocalizedStrings.GetString("Common_OK"),
                        IsVisible = publishMode.ShouldBeVisible(HeadersHelper.IsQualified),
                        Width = "0%"
                    };
                    sortedActionHeaders[HeadersHelper.IsNotQualified] = new ActionHeader
                    {
                        Label = LocalizedStrings.GetString("Common_NOK"),
                        IsVisible = publishMode.ShouldBeVisible(HeadersHelper.IsNotQualified),
                        Width = "0%"
                    };
                    sortedActionHeaders[HeadersHelper.QualificationStep_Comment] = new ActionHeader
                    {
                        Label = LocalizedStrings.GetString("Comment"),
                        IsVisible = publishMode.ShouldBeVisible(HeadersHelper.QualificationStep_Comment),
                        Width = "0%"
                    };
                }
                var nbVisibleHeaders = sortedActionHeaders.Count(_ => _.Value.IsVisible);
                sortedActionHeaders.Where(_ => _.Value.IsVisible).ForEach(_ => _.Value.Width = $"{100.0 / nbVisibleHeaders}%");
                return sortedActionHeaders;
            }
            SfDataGridJsonDto disposition = JsonConvert.DeserializeObject<SfDataGridJsonDto>(dispositionAsJson);
            foreach (var column in disposition.Columns)
            {
                if (publishMode == PublishModeEnum.Evaluation && evaluationMandatoryHeaders.Contains(column.MappingName))
                {
                    if (column.MappingName == HeadersHelper.IsQualified)
                        sortedActionHeaders[column.MappingName] = new ActionHeader
                        {
                            Label = LocalizedStrings.GetString("Common_OK"),
                            IsVisible = publishMode.ShouldBeVisible(column.MappingName),
                            Width = column.WidthString
                        };
                    else if (column.MappingName == HeadersHelper.IsNotQualified)
                        sortedActionHeaders[column.MappingName] = new ActionHeader
                        {
                            Label = LocalizedStrings.GetString("Common_NOK"),
                            IsVisible = publishMode.ShouldBeVisible(column.MappingName),
                            Width = column.WidthString
                        };
                    else if (column.MappingName == HeadersHelper.QualificationStep_Comment)
                        sortedActionHeaders[column.MappingName] = new ActionHeader
                        {
                            Label = LocalizedStrings.GetString("Comment"),
                            IsVisible = publishMode.ShouldBeVisible(column.MappingName),
                            Width = column.WidthString
                        };
                    continue;
                }
                if (!headers.ContainsKey(column.MappingName))
                    continue;
                sortedActionHeaders[column.MappingName] = new ActionHeader
                {
                    Label = headers[column.MappingName],
                    IsVisible = column.IsVisible,
                    Width = column.WidthString
                };
            }
            foreach (var column in headers)
            {
                if (sortedActionHeaders.ContainsKey(column.Key))
                    continue;
                sortedActionHeaders[column.Key] = new ActionHeader
                {
                    Label = column.Value,
                    IsVisible = publishMode.ShouldBeVisible(column.Key),
                    Width = "0%"
                };
            }
            if (publishMode == PublishModeEnum.Evaluation)
            {
                // Add mandatory columns if not exist
                if (!sortedActionHeaders.ContainsKey(HeadersHelper.IsQualified))
                {
                    sortedActionHeaders[HeadersHelper.IsQualified] = new ActionHeader
                    {
                        Label = LocalizedStrings.GetString("Common_OK"),
                        IsVisible = publishMode.ShouldBeVisible(HeadersHelper.IsQualified),
                        Width = null
                    };
                }
                if (!sortedActionHeaders.ContainsKey(HeadersHelper.IsNotQualified))
                {
                    sortedActionHeaders[HeadersHelper.IsNotQualified] = new ActionHeader
                    {
                        Label = LocalizedStrings.GetString("Common_NOK"),
                        IsVisible = publishMode.ShouldBeVisible(HeadersHelper.IsNotQualified),
                        Width = null
                    };
                }
                if (!sortedActionHeaders.ContainsKey(HeadersHelper.QualificationStep_Comment))
                {
                    sortedActionHeaders[HeadersHelper.QualificationStep_Comment] = new ActionHeader
                    {
                        Label = LocalizedStrings.GetString("Comment"),
                        IsVisible = publishMode.ShouldBeVisible(HeadersHelper.QualificationStep_Comment),
                        Width = null
                    };
                }
                var addedHeaders = sortedActionHeaders.Where(_ => _.Value.Width == null);
            }
            return sortedActionHeaders;
        }

        /// <summary>
        /// Publication of a documentation (can be draft or publication)
        /// </summary>
        /// <param name="documentationDraftId"></param>
        /// <param name="processId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> Publish(int documentationDraftId)
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;

            var draft = await _prepareService.GetDocumentationDraft(documentationDraftId);

            var publishTraining = draft.DocumentationActionDraftWBS.Any(_ => _.DocumentationPublishMode == (int)PublishModeEnum.Formation);
            var publishEvaluation = draft.DocumentationActionDraftWBS.Any(_ => _.DocumentationPublishMode == (int)PublishModeEnum.Evaluation);
            var publishInspection = draft.DocumentationActionDraftWBS.Any(_ => _.DocumentationPublishMode == (int)PublishModeEnum.Inspection);

            var currentScenario = await _prepareService.GetScenarioForPublish(draft.ScenarioId);

            try
            {
                //Check if publish is inprogress for given process
                var processIds = new List<int>
                {
                    currentScenario.Project.ProcessId
                };
                var publicationInProgress = await _publicationService.GetProcessesPublicationStatus(processIds.ToArray());
                if (publicationInProgress[currentScenario.Project.ProcessId] != null)
                {
                    string errorMessage = LocalizedStrings.GetString("ErrorPublicationIsInProgress");
                    return errorMessage;
                }

                List<Publication> publications = new List<Publication>();
                if (publishTraining)
                    publications.Add(await GetFinalizedPublication(currentScenario, PublishModeEnum.Formation, draft));
                if (publishEvaluation)
                    publications.Add(await GetFinalizedPublication(currentScenario, PublishModeEnum.Evaluation, draft));
                if (publishInspection)
                    publications.Add(await GetFinalizedPublication(currentScenario, PublishModeEnum.Inspection, draft));

                currentScenario.Project.DeserializableOtherDisposition();
                if (currentScenario.Project.PublicationPreferences == null)
                    currentScenario.Project.PublicationPreferences = new PublicationPreferences();
                //Set other disposition
                currentScenario.Project.PublicationPreferences.VideoExportIsEnabled = draft.ActiveVideoExport.Value;
                currentScenario.Project.PublicationPreferences.SlowMotionIsEnabled = draft.SlowMotion.Value;
                currentScenario.Project.PublicationPreferences.DurationMini = draft.SlowMotionDuration.HasValue ? (double)draft.SlowMotionDuration.Value : 0;
                currentScenario.Project.PublicationPreferences.VideoMarkingIsEnabled = draft.WaterMarking.Value;
                currentScenario.Project.PublicationPreferences.OverlayTextVideo = draft.WaterMarkingText;
                currentScenario.Project.PublicationPreferences.HorizontalAlignement = draft.WaterMarkingHAlign ?? EHorizontalAlign.Center;
                currentScenario.Project.PublicationPreferences.VerticalAlignement = draft.WaterMarkingVAlign ?? EVerticalAlign.Top;
                currentScenario.Project.SerializableOtherDisposition();
                //Save project
                currentScenario.Project.MarkAsModified();
                await _prepareService.SaveProject(currentScenario.Project);

                var publicationWatermark = draft.WaterMarking.Value && !string.IsNullOrEmpty(draft.WaterMarkingText) ? JsonConvert.SerializeObject((draft.WaterMarkingText, true, true, draft.WaterMarkingHAlign.Value, draft.WaterMarkingVAlign.Value)) : null;
                foreach (var publication in publications)
                {
                    publication.Watermark = publicationWatermark;
                    publication.MinDurationVideo = Convert.ToInt64(draft.SlowMotionDuration ?? 0);
                }

                var user = await _authenticationService.GetUser(_securityContext.CurrentUser.Username);
                int publicationHistoryId = await _publicationService.PublishMulti(publications, draft.ActiveVideoExport.Value, false);
                return "success";
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                string errorMessage = LocalizedStrings.GetString("View_PublishVideos_PublishingError");
                return errorMessage;
            }
        }

        /// <summary>
        /// Build documentation draft predecessor
        /// </summary>
        /// <param name="draftActions"></param>
        /// <param name="allActions"></param>
        /*private void BuildDocumentationActionDraftPredecessors(List<DocumentationActionDraft> draftActions, List<GenericActionViewModel> allActions)
        {
            foreach (var action in draftActions)
            {
                var documentationAction = allActions.FirstOrDefault(u => u.WBS == action.WBS);
                if (documentationAction == null)
                    throw new Exception(LocalizedStrings.GetString("View_Documentation_TaskNotFound"));

                if (allActions.IndexOf(documentationAction) == 0)
                    continue;

                var previousDocumentationAction = allActions[allActions.IndexOf(documentationAction) - 1];
                // Previous task is a KAction task, we already know the id
                if (!previousDocumentationAction.IsDocumentation)
                {
                    action.PreviousActionId = previousDocumentationAction.ActionId;
                    continue;
                }
                // Previous task is DocumentationActionDraft, we dont yet the ID
                action.PreviousDocumentationActionDraft = draftActions.First(u => u.WBS == previousDocumentationAction.WBS);
            }
        }*/

        /// <summary>
        /// Build documentation draft predecessor
        /// </summary>
        /// <param name="draftActions"></param>
        /// <param name="allActions"></param>
        /*private List<DocumentationActionDraftWBS> BuildDocumentationActionDraftWBS(
            List<DocumentationActionDraft> draftActions, 
            List<GenericActionViewModel> allActions,
            PublishModeEnum publishMode)
        {
            var model = new List<DocumentationActionDraftWBS>();
            foreach (var action in allActions)
            {
                model.Add(new DocumentationActionDraftWBS
                {
                    ActionId = !action.IsDocumentation ? (int?)action.ActionId : null,
                    DocumentationActionDraft = action.IsDocumentation ? draftActions.First(u => u.WBS == action.WBS) : null,
                    WBS = action.WBS,
                    DocumentationPublishMode = (int)publishMode
                });
            }
            return model;
        }*/

        [ValidateInput(false)]
        async Task<Publication> GetFinalizedPublication(Scenario currentScenario, PublishModeEnum publishMode, DocumentationDraft draft)
        {
            var publication = new Publication(currentScenario);

            try
            {
                //Set Formation/Inspection disposition
                switch (publishMode)
                {
                    case PublishModeEnum.Formation:
                        publication.Formation_Disposition = Encoding.UTF8.GetBytes(ConvertToXMLGrid(draft.Formation_DispositionAsJson));
                        publication.Formation_ActionDisposition = draft.Formation_ActionDisposition;
                        publication.ReleaseNote = draft.Formation_ReleaseNote;
                        publication.IsMajor = draft.Formation_IsMajor;
                        currentScenario.Project.Formation_Disposition = publication.Formation_Disposition;
                        break;
                    case PublishModeEnum.Evaluation:
                        publication.Evaluation_Disposition = Encoding.UTF8.GetBytes(ConvertToXMLGrid(draft.Evaluation_DispositionAsJson));
                        publication.Evaluation_ActionDisposition = draft.Evaluation_ActionDisposition;
                        publication.ReleaseNote = draft.Evaluation_ReleaseNote;
                        publication.IsMajor = draft.Evaluation_IsMajor;
                        currentScenario.Project.Evaluation_Disposition = publication.Evaluation_Disposition;
                        break;
                    case PublishModeEnum.Inspection:
                        publication.Inspection_Disposition = Encoding.UTF8.GetBytes(ConvertToXMLGrid(draft.Inspection_DispositionAsJson));
                        publication.Inspection_ActionDisposition = draft.Inspection_ActionDisposition;
                        publication.ReleaseNote = draft.Inspection_ReleaseNote;
                        publication.IsMajor = draft.Inspection_IsMajor;
                        currentScenario.Project.Inspection_Disposition = publication.Inspection_Disposition;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(publishMode), publishMode, $"Impossible to publish with PublishMode : {publishMode}");
                }

                publication.PublishMode = publishMode;
                var wbsActions = draft.DocumentationActionDraftWBS
                    .Where(_ => _.DocumentationPublishMode == (int)publishMode)
                    .ToList();

                //CREATE PUBLISHED ACTIONS
                var actions = wbsActions
                    .Select(wbsAction =>
                    {
                        if (wbsAction.IsDocumentation)
                            return new PublishedAction(wbsAction.DocumentationActionDraft, wbsAction.WBS);
                        else
                            return new PublishedAction(currentScenario.Actions.Single(_ => _.ActionId == wbsAction.ActionId), wbsAction.WBS);
                    })
                    .OrderBy(_ => _, new WBSComparer())
                    .ToList();

                // Build predecessor and successor
                foreach (var action in actions)
                {
                    //action.Thumbnail = action.ThumbnailHash != null ? await _prepareService.GetPublishedFile(action.ThumbnailHash) : null;

                    var index = actions.IndexOf(action);
                    if (index == 0)
                        continue;
                    var predecessor = actions[index - 1];
                    WBSWebActionUtil<GenericActionViewModel>.AddPredecessor(actions, action, predecessor);
                }

                publication.PublishedActions.AddRange(actions);

                var headers = await DocumentationMapper.GetHeaders(currentScenario.Project.ProcessId, _referentialsService, true);

                var refsArray = await _referentialsService.GetApplicationReferentials();
                var localizations = refsArray.ToDictionary(k => $"{k.ReferentialId}", v => v.Label);
                localizations.Add(nameof(Project.CustomNumericLabel), headers.GetOrDefault(nameof(PublishedAction.CustomNumericValue)) ?? currentScenario.Project.CustomNumericLabel ?? nameof(PublishedAction.CustomNumericValue));
                localizations.Add(nameof(Project.CustomNumericLabel2), headers.GetOrDefault(nameof(PublishedAction.CustomNumericValue2)) ?? currentScenario.Project.CustomNumericLabel2 ?? nameof(PublishedAction.CustomNumericValue2));
                localizations.Add(nameof(Project.CustomNumericLabel3), headers.GetOrDefault(nameof(PublishedAction.CustomNumericValue3)) ?? currentScenario.Project.CustomNumericLabel3 ?? nameof(PublishedAction.CustomNumericValue3));
                localizations.Add(nameof(Project.CustomNumericLabel4), headers.GetOrDefault(nameof(PublishedAction.CustomNumericValue4)) ?? currentScenario.Project.CustomNumericLabel4 ?? nameof(PublishedAction.CustomNumericValue4));
                localizations.Add(nameof(Project.CustomTextLabel), headers.GetOrDefault(nameof(PublishedAction.CustomTextValue)) ?? currentScenario.Project.CustomTextLabel ?? nameof(PublishedAction.CustomTextValue));
                localizations.Add(nameof(Project.CustomTextLabel2), headers.GetOrDefault(nameof(PublishedAction.CustomTextValue2)) ?? currentScenario.Project.CustomTextLabel2 ?? nameof(PublishedAction.CustomTextValue2));
                localizations.Add(nameof(Project.CustomTextLabel3), headers.GetOrDefault(nameof(PublishedAction.CustomTextValue3)) ?? currentScenario.Project.CustomTextLabel3 ?? nameof(PublishedAction.CustomTextValue3));
                localizations.Add(nameof(Project.CustomTextLabel4), headers.GetOrDefault(nameof(PublishedAction.CustomTextValue4)) ?? currentScenario.Project.CustomTextLabel4 ?? nameof(PublishedAction.CustomTextValue4));
                publication.Localizations = new TrackableCollection<PublicationLocalization>(localizations.Select(_ => new PublicationLocalization { ResourceKey = _.Key, Value = _.Value }));

                var RefHasQuantity = currentScenario.Project.Referentials.ToDictionary(k => k.ReferentialId, v => v.HasQuantity);
                foreach (var pAction in publication.PublishedActions)
                {
                    if (WBSHelper.HasChildren(pAction.WBS, publication.PublishedActions.Select(_ => _.WBS)))
                        pAction.IsGroup = true;

                    // Documentation action does not have any action related
                    if (pAction.Action == null)
                        continue;

                    // On définie le LinkedPublication si existant
                    if (pAction.Action.LinkedProcessId != null)
                    {
                        var linkedPublication = await _prepareService.GetLastPublication(pAction.Action.LinkedProcessId.Value);
                        linkedPublication.StopTracking();
                        pAction.LinkedPublication = linkedPublication;
                    }

                    // On crée les prédécesseurs/successeurs
                    pAction.Predecessors = new TrackableCollection<PublishedAction>(publication.PublishedActions
                        .Where(_ => _.Action != null
                                    && pAction.Action.Predecessors
                                        .Select(p => p.ActionId)
                                        .Contains(_.Action.ActionId)));
                    pAction.Successors = new TrackableCollection<PublishedAction>(publication.PublishedActions
                        .Where(_ => _.Action != null 
                                    && pAction.Action.Successors
                                        .Select(p => p.ActionId)
                                        .Contains(_.Action.ActionId)));

                    // On construit les détails de la tâche
                    pAction.Refs = new TrackableCollection<RefsCollection>();
                    if (pAction.Action.Ref1.Any())
                        pAction.Refs.Add(new RefsCollection { Field = nameof(PublishedAction.Refs1), Label = localizations[nameof(Ref1)], Values = new TrackableCollection<PublishedReferentialAction>(pAction.Action.Ref1.Select(_ => new PublishedReferentialAction(_, RefHasQuantity))) });
                    if (pAction.Action.Ref2.Any())
                        pAction.Refs.Add(new RefsCollection { Field = nameof(PublishedAction.Refs2), Label = localizations[nameof(Ref2)], Values = new TrackableCollection<PublishedReferentialAction>(pAction.Action.Ref2.Select(_ => new PublishedReferentialAction(_, RefHasQuantity))) });
                    if (pAction.Action.Ref3.Any())
                        pAction.Refs.Add(new RefsCollection { Field = nameof(PublishedAction.Refs3), Label = localizations[nameof(Ref3)], Values = new TrackableCollection<PublishedReferentialAction>(pAction.Action.Ref3.Select(_ => new PublishedReferentialAction(_, RefHasQuantity))) });
                    if (pAction.Action.Ref4.Any())
                        pAction.Refs.Add(new RefsCollection { Field = nameof(PublishedAction.Refs4), Label = localizations[nameof(Ref4)], Values = new TrackableCollection<PublishedReferentialAction>(pAction.Action.Ref4.Select(_ => new PublishedReferentialAction(_, RefHasQuantity))) });
                    if (pAction.Action.Ref5.Any())
                        pAction.Refs.Add(new RefsCollection { Field = nameof(PublishedAction.Refs5), Label = localizations[nameof(Ref5)], Values = new TrackableCollection<PublishedReferentialAction>(pAction.Action.Ref5.Select(_ => new PublishedReferentialAction(_, RefHasQuantity))) });
                    if (pAction.Action.Ref6.Any())
                        pAction.Refs.Add(new RefsCollection { Field = nameof(PublishedAction.Refs6), Label = localizations[nameof(Ref6)], Values = new TrackableCollection<PublishedReferentialAction>(pAction.Action.Ref6.Select(_ => new PublishedReferentialAction(_, RefHasQuantity))) });
                    if (pAction.Action.Ref7.Any())
                        pAction.Refs.Add(new RefsCollection { Field = nameof(PublishedAction.Refs7), Label = localizations[nameof(Ref7)], Values = new TrackableCollection<PublishedReferentialAction>(pAction.Action.Ref7.Select(_ => new PublishedReferentialAction(_, RefHasQuantity))) });
                    pAction.CustomLabels = new TrackableCollection<CustomLabel>();
                    if (!string.IsNullOrEmpty(pAction.Action.CustomTextValue))
                        pAction.CustomLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomTextValue), Label = localizations[nameof(Project.CustomTextLabel)], Value = pAction.Action.CustomTextValue });
                    if (!string.IsNullOrEmpty(pAction.Action.CustomTextValue2))
                        pAction.CustomLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomTextValue2), Label = localizations[nameof(Project.CustomTextLabel2)], Value = pAction.Action.CustomTextValue2 });
                    if (!string.IsNullOrEmpty(pAction.Action.CustomTextValue3))
                        pAction.CustomLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomTextValue3), Label = localizations[nameof(Project.CustomTextLabel3)], Value = pAction.Action.CustomTextValue3 });
                    if (!string.IsNullOrEmpty(pAction.Action.CustomTextValue4))
                        pAction.CustomLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomTextValue4), Label = localizations[nameof(Project.CustomTextLabel4)], Value = pAction.Action.CustomTextValue4 });
                    if (pAction.Action.CustomNumericValue != null)
                        pAction.CustomLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomNumericValue), Label = localizations[nameof(Project.CustomNumericLabel)], Value = pAction.Action.CustomNumericValue.ToString() });
                    if (pAction.Action.CustomNumericValue2 != null)
                        pAction.CustomLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomNumericValue2), Label = localizations[nameof(Project.CustomNumericLabel2)], Value = pAction.Action.CustomNumericValue2.ToString() });
                    if (pAction.Action.CustomNumericValue3 != null)
                        pAction.CustomLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomNumericValue3), Label = localizations[nameof(Project.CustomNumericLabel3)], Value = pAction.Action.CustomNumericValue3.ToString() });
                    if (pAction.Action.CustomNumericValue4 != null)
                        pAction.CustomLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomNumericValue4), Label = localizations[nameof(Project.CustomNumericLabel4)], Value = pAction.Action.CustomNumericValue4.ToString() });
                }

                publication.PublishedActions.Where(_ => _.LinkedPublication != null).ForEach(_ =>
                {
                    _.StopTracking();
                    Guid guid = _.LinkedPublicationId.Value;
                    _.LinkedPublication = null;
                    _.LinkedPublicationId = guid;
                });

                return publication;
            }
            catch
            {
                throw;
            }
        }

        public string ConvertToXMLGrid(string json)
        {
            SfDataGridJsonDto sourceJsonDto = JsonConvert.DeserializeObject<SfDataGridJsonDto>(json);

            // Resize grid width based on tablet fixed width
            const double wpfFixedGridWidth = 1260.0 - 24; // Grid width - ExpanderColumnWidth
            sourceJsonDto.Columns.ForEach(u => u.Width = double.Parse(u.WidthString.Replace("%",""), CultureInfo.InvariantCulture) * wpfFixedGridWidth / 100);
            sourceJsonDto.Columns.ForEach(u => u.HeaderText = WebUtility.HtmlDecode(u.HeaderText));

            SfDataGridXmlDto destinationXmlDto = sourceJsonDto.ToXmlDto();
            var tempFile = Path.GetTempFileName();
            using (var fileStream = System.IO.File.OpenWrite(tempFile))
            using (var writer = XmlWriter.Create(fileStream, new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "\t",
                OmitXmlDeclaration = true
            }))
            {
                var doc = new XDocument(destinationXmlDto.Serialize());
                doc.WriteTo(writer);
            }
            var xmlContent_computed = System.IO.File.ReadAllText(tempFile, Encoding.UTF8).Replace(" />", "/>");
            return xmlContent_computed;
        }

        public string Query(string gridObj)
        {
            string xmlContent_computed;
            SfDataGridJsonDto sourceJsonDto = JsonConvert.DeserializeObject<SfDataGridJsonDto>(gridObj);
            SfDataGridXmlDto destinationXmlDto = sourceJsonDto.ToXmlDto();
            string tempFile = Path.GetTempFileName();
            using (var fileStream = System.IO.File.OpenWrite(tempFile))
            using (var writer = XmlWriter.Create(fileStream, new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "\t",
                OmitXmlDeclaration = true
            }))
            {
                var doc = new XDocument(destinationXmlDto.Serialize());
                doc.WriteTo(writer);
            }
            xmlContent_computed = System.IO.File.ReadAllText(tempFile, Encoding.UTF8).Replace(" />", "/>");
            return xmlContent_computed;
        }
        
        public async Task<ActionResult> History(bool partial = false)
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;

            ViewBag.StatesAsLabel = JsonConvert.SerializeObject(new StateAsLabel[]
            {
                new StateAsLabel { AsString = PublicationStatus.Waiting.ToString().ToLower(), AsLabel = LocalizedStrings.GetString($"View_DocumentationHistory_State_{PublicationStatus.Waiting}") },
                new StateAsLabel { AsString = PublicationStatus.InProgress.ToString().ToLower(), AsLabel = LocalizedStrings.GetString($"View_DocumentationHistory_State_{PublicationStatus.InProgress}") },
                new StateAsLabel { AsString = PublicationStatus.Completed.ToString().ToLower(), AsLabel = LocalizedStrings.GetString($"View_DocumentationHistory_State_{PublicationStatus.Completed}") },
                new StateAsLabel { AsString = PublicationStatus.InError.ToString().ToLower(), AsLabel = LocalizedStrings.GetString($"View_DocumentationHistory_State_{PublicationStatus.InError}") },
                new StateAsLabel { AsString = PublicationStatus.Cancelled.ToString().ToLower(), AsLabel = LocalizedStrings.GetString($"View_DocumentationHistory_State_{PublicationStatus.Cancelled}") }
            });
            var publications = await _publicationService.GetPublicationHistories();
            var model = publications.Select(u => DocumentationMapper.PublicationHistoryToDocumentation(u)).ToList();
            if (partial)
                return PartialView(model);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> UpdatePublicationHistory(int publicationHistoryId)
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;

            var publication = await _publicationService.GetPublicationHistory(publicationHistoryId);
            var result = DocumentationMapper.PublicationHistoryToDocumentation(publication);
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        /// <summary>
        /// Cancel on going publication
        /// </summary>
        /// <param name="publicationHistoryId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelPublication(int publicationHistoryId)
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;

            try
            {
                if (publicationHistoryId == 0)
                    throw new ArgumentNullException();
                var result = await _publicationService.CancelPublication(publicationHistoryId);
                
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Faking a publication
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FakingPublication()
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;

            try
            {
                var result = await _publicationService.FakingPublication();

                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        #region Grid actions

        /// <summary>
        /// Retrieve all the header for a specific process
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        private async Task<List<ReferentialField>> GetReferentialsByScenario(int processId)
        {
            var referentials = await _referentialsService.GetDocumentationReferentials(processId);
            var refs = referentials.Where(u => u.IsEnabled).OrderBy(r => r.Label).Select(u => new ReferentialField
            {
                ReferentialFieldId = u.ReferentialId,
                ReferentialFieldName = u.Label,
                HasMultipleSelection = u.HasMultipleSelection,
                HasQuantity = u.HasQuantity,
            }).ToList();
            foreach(var referential in refs)
                referential.ReferentialsFieldElements = await DocumentationMapper.GetReferentialValues(referential.ReferentialFieldId, referential.HasQuantity, processId);
            return refs;
        }

        /// <summary>
        /// Retrieve all the DocumentationReferential for a specific process
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public async Task<DocumentationReferential[]> GetDocumentationReferentials(int processId, int projectId, int scenarioId)
        {
            var projectReferentials = await _referentialsService.GetProjectReferentials(projectId);
            var documentationReferentials = await _referentialsService.GetDocumentationReferentials(processId);

            foreach (var documentationReferential in documentationReferentials)
            {
                var projectReferential = projectReferentials.SingleOrDefault(_ => _.ReferentialId == documentationReferential.ReferentialId);
                if (projectReferential != null && projectReferential.IsEnabled == true && ((byte)documentationReferential.ReferentialId >= (byte)ProcessReferentialIdentifier.CustomTextLabel
                        && (byte)documentationReferential.ReferentialId <= (byte)ProcessReferentialIdentifier.CustomNumericLabel4))
                {
                    documentationReferential.Label = projectReferential.Label;
                }
                else if (projectReferential != null && ((byte)documentationReferential.ReferentialId < (byte)ProcessReferentialIdentifier.CustomTextLabel
                        || (byte)documentationReferential.ReferentialId > (byte)ProcessReferentialIdentifier.CustomNumericLabel4))
                {
                    documentationReferential.Label = projectReferential.Label;
                }
                
            }

            foreach (var documentationReferential in documentationReferentials)
            {
                // Set properties that can't be update, because they were enabled by the analyst
                var projectReferential = projectReferentials.SingleOrDefault(_ => _.ReferentialId == documentationReferential.ReferentialId && _.IsEnabled);
                if (projectReferential != null)
                {
                    documentationReferential.IsEnabled = projectReferential.IsEnabled;
                    documentationReferential.HasMultipleSelection = projectReferential.HasMultipleSelection;
                    documentationReferential.HasQuantity = projectReferential.HasQuantity;
                    if ((byte)documentationReferential.ReferentialId < (byte)ProcessReferentialIdentifier.CustomTextLabel
                        || (byte)documentationReferential.ReferentialId > (byte)ProcessReferentialIdentifier.CustomNumericLabel4)
                        documentationReferential.Label = projectReferential.Label;
                }
            }
            // Add DocumentationReferentials that don't exist
            foreach (var projectReferential in projectReferentials)
            {
                // Set properties that can't be update, because they were enabled by the analyst
                var documentationReferential = documentationReferentials.SingleOrDefault(_ => _.ReferentialId == projectReferential.ReferentialId);
                if (documentationReferential == null)
                {
                    documentationReferential = new DocumentationReferential
                    {
                        ProcessId = processId,
                        ReferentialId = projectReferential.ReferentialId,
                        IsEnabled = projectReferential.IsEnabled,
                        HasMultipleSelection = projectReferential.HasMultipleSelection,
                        HasQuantity = projectReferential.HasQuantity,
                        Label = projectReferential.Label
                    };
                    documentationReferentials.Add(documentationReferential);
                }
            }
            // Save refreshed DocumentationReferentials
            documentationReferentials = await _referentialsService.SaveDocumentationReferentials(documentationReferentials.ToArray());
            
            foreach (var documentationReferential in documentationReferentials)
            {
                if ((byte)ProcessReferentialIdentifier.CustomTextLabel <= (byte)documentationReferential.ReferentialId
                    && (byte)documentationReferential.ReferentialId <= (byte)ProcessReferentialIdentifier.CustomNumericLabel4)
                {
                    // Enable edition of Custom Fields that are not used
                    documentationReferential.IsEditable = !await _referentialsService.CustomFieldIsUsed(scenarioId, documentationReferential.ReferentialId);
                }
                else
                {
                    // Disable edition of items that were enabled by the analyst
                    documentationReferential.IsEditable = !projectReferentials.Any(_ => _.ReferentialId == documentationReferential.ReferentialId
                        && _.IsEnabled);
                }
                documentationReferential.Category = documentationReferential.ReferentialId.GetReferentialCategory();
            }

            return documentationReferentials.ToArray();
        }

        /// <summary>
        /// Save the DocumentationReferential for a specific process
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> SaveDocumentationReferentials(DocumentationReferential[] documentationReferentials)
        {
            try
            {
                await _referentialsService.SaveDocumentationReferentials(documentationReferentials);
                return "success";
            }
            catch
            {
                string errorMessage = "Unable to save referentials settings";
                return errorMessage;
            }
        }

        /// <summary>
        /// Save the DocumentationDraft
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SaveDocumentationDraft(DocumentationDraft draft)
        {
            try
            {
                var saveDraft = await _prepareService.SaveDocumentationDraft(draft);
                var jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, PreserveReferencesHandling = PreserveReferencesHandling.All };
                return Content(JsonConvert.SerializeObject(saveDraft, jsonSettings), "application/json");
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content("Unable to save documentation draft");
            }
        }

        /// <summary>
        /// Update DocumentationDraft
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ActionResult> UpdateDispositionDocumentationDraft(UpdateDispositionDocumentationDraftModel model) =>
            Task.Run<ActionResult>(() =>
            {
                try
                {
                    switch (model.PublishMode)
                    {
                        case (int)PublishModeEnum.Formation:
                            model.Draft.Formation_DispositionAsJson = model.JsonDisposition;
                            break;
                        case (int)PublishModeEnum.Evaluation:
                            model.Draft.Evaluation_DispositionAsJson = model.JsonDisposition;
                            break;
                        case (int)PublishModeEnum.Inspection:
                            model.Draft.Inspection_DispositionAsJson = model.JsonDisposition;
                            break;
                    }
                    var jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, PreserveReferencesHandling = PreserveReferencesHandling.All };
                    return Content(JsonConvert.SerializeObject(model.Draft, jsonSettings), "application/json");
                }
                catch (Exception ex)
                {
                    _traceManager.TraceError(ex, ex.Message);
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Content("Unable to update documentation disposition");
                }
            });

        void ManageDraftUpdates(DocumentationDraft dbDraft, PublishModeEnum publishMode, List<DocumentationActionDraftWBS> actionsDraftWBS)
        {
            var filteredDbActionsDraftWBS = dbDraft.DocumentationActionDraftWBS
                    .Where(dbDraftWBS => dbDraftWBS.DocumentationPublishMode == (int)publishMode)
                    .ToList();
            if (actionsDraftWBS?.Any() != true) // Delete all actions
                filteredDbActionsDraftWBS.ForEach(dbDraftWBS => dbDraftWBS.Delete());
            else
            {
                // Delete DocumentationActionDraftWBS that don't exists anymore
                filteredDbActionsDraftWBS.ForEach(dbDraftWBS =>
                {
                    if (!actionsDraftWBS.Any(draftWBS => draftWBS.DocumentationActionDraftWBSId == dbDraftWBS.DocumentationActionDraftWBSId))
                        dbDraftWBS.Delete();
                });
                // Update or add DocumentationActionDraftWBS
                actionsDraftWBS.ForEach(draftWBS =>
                {
                    var dbDraftWBS = filteredDbActionsDraftWBS.SingleOrDefault(_ => _.DocumentationActionDraftWBSId == draftWBS.DocumentationActionDraftWBSId);
                    if (dbDraftWBS == null) // Add
                        dbDraft.DocumentationActionDraftWBS.Add(new DocumentationActionDraftWBS(draftWBS));
                    else // Update
                        dbDraftWBS.Update(draftWBS);
                });
            }
        }

        /// <summary>
        /// Save DocumentationDraft actions
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SaveDocumentationDraftActions(SaveDocumentationDraftActionsModel model)
        {
            try
            {
                var dbDraft = await _prepareService.GetDocumentationDraft(model.Draft.DocumentationDraftId);
                dbDraft.ChangeTracker.ChangeTrackingEnabled = true;

                // Training actions
                ManageDraftUpdates(dbDraft, PublishModeEnum.Formation, model.TrainingActions);
                // Evaluation actions
                ManageDraftUpdates(dbDraft, PublishModeEnum.Evaluation, model.EvaluationActions);
                // Inspection actions
                ManageDraftUpdates(dbDraft, PublishModeEnum.Inspection, model.InspectionActions);

                var saveDraft = await _prepareService.SaveDocumentationDraft(dbDraft);
                var jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, PreserveReferencesHandling = PreserveReferencesHandling.All };
                return Content(JsonConvert.SerializeObject(saveDraft, jsonSettings), "application/json");
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Content("Unable to update documentation actions");
            }
        }

        /// <summary>
        /// Update DocumentationDraft
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ActionResult> UpdateActionDispositionDocumentationDraft(UpdateActionDispositionDocumentationDraftModel model) =>
            Task.Run<ActionResult>(() =>
            {
                try
                {
                    switch (model.PublishMode)
                    {
                        case (int)PublishModeEnum.Formation:
                            model.Draft.Formation_ActionDisposition = model.ActionDisposition;
                            break;
                        case (int)PublishModeEnum.Evaluation:
                            model.Draft.Evaluation_ActionDisposition = model.ActionDisposition;
                            break;
                        case (int)PublishModeEnum.Inspection:
                            model.Draft.Inspection_ActionDisposition = model.ActionDisposition;
                            break;
                    }
                    var jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, PreserveReferencesHandling = PreserveReferencesHandling.All };
                    return Content(JsonConvert.SerializeObject(model.Draft, jsonSettings), "application/json");
                }
                catch (Exception ex)
                {
                    _traceManager.TraceError(ex, ex.Message);
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Content("Unable to update documentation action disposition");
                }
            });

        /// <summary>
        /// Update DocumentationDraft
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ActionResult> UpdateDocumentationVideos(UpdateVideoDocumentationDraftModel model) =>
            Task.Run<ActionResult>(() =>
            {
                try
                {
                    model.Draft.ActiveVideoExport = model.ActiveVideoExport;
                    model.Draft.SlowMotion = model.ActiveVideoExport ? model.SlowMotion : false;
                    model.Draft.SlowMotionDuration = model.Draft.SlowMotion == true ? Convert.ToDecimal(model.SlowMotionDuration) : (decimal?)null;
                    model.Draft.WaterMarking = model.ActiveVideoExport ? model.WaterMarking : false;
                    model.Draft.WaterMarkingText = model.Draft.WaterMarking == true ? model.WaterMarkingText : null;
                    model.Draft.WaterMarkingVAlign = model.Draft.WaterMarking == true ? (EVerticalAlign?)model.WaterMarkingVAlign : (EVerticalAlign?)null;
                    model.Draft.WaterMarkingHAlign = model.Draft.WaterMarking == true ? (EHorizontalAlign?)model.WaterMarkingHAlign : (EHorizontalAlign?)null;
                    var jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, PreserveReferencesHandling = PreserveReferencesHandling.All };
                    return Content(JsonConvert.SerializeObject(model.Draft, jsonSettings), "application/json");
                }
                catch (Exception ex)
                {
                    _traceManager.TraceError(ex, ex.Message);
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Content("Unable to update documentation videos settings");
                }
            });

        /// <summary>
        /// Update DocumentationDraft
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Task<ActionResult> UpdateDocumentationReleaseNotes(UpdateReleaseNotesDocumentationDraftModel model) =>
            Task.Run<ActionResult>(() =>
            {
                try
                {
                    switch (model.PublishMode)
                    {
                        case (int)PublishModeEnum.Formation:
                            model.Draft.Formation_IsMajor = model.IsMajor;
                            model.Draft.Formation_ReleaseNote = model.ReleaseNotes;
                            break;
                        case (int)PublishModeEnum.Evaluation:
                            model.Draft.Evaluation_IsMajor = model.IsMajor;
                            model.Draft.Evaluation_ReleaseNote = model.ReleaseNotes;
                            break;
                        case (int)PublishModeEnum.Inspection:
                            model.Draft.Inspection_IsMajor = model.IsMajor;
                            model.Draft.Inspection_ReleaseNote = model.ReleaseNotes;
                            break;
                    }
                    var jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, PreserveReferencesHandling = PreserveReferencesHandling.All };
                    return Content(JsonConvert.SerializeObject(model.Draft, jsonSettings), "application/json");
                }
                catch (Exception ex)
                {
                    _traceManager.TraceError(ex, ex.Message);
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Content("Unable to update documentation release notes");
                }
            });

        /// <summary>
        /// Retrieve all the header for a specific scenarios
        /// </summary>
        /// <param name="documentationReferentials"></param>
        /// <returns></returns>
        private async Task<List<FormatActionsElementViewModel>> GetAllReferentials(List<DocumentationReferential> documentationReferentials, string actionDisposition) =>
            await DocumentationMapper.GetRefsAsync(await _referentialsService.GetApplicationReferentials(), documentationReferentials, actionDisposition);

        /// <summary>
        /// Retrieve the HTML used to display popup when insert/edit documentation action
        /// Prerequesite is that the action id is already a documentation task (check done on UI side before sending the message)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="processId"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        [HttpPost]
        public Task<ActionResult> GetAction(GetActionModel param) =>
            Task.Run<ActionResult>(() =>
            {
                ViewBag.TimeScaleHeader = GetTimeScaleHeader(param.ProjectTimeScale);
                ViewBag.TimeScaleMask = GetTimeScaleMask(param.ProjectTimeScale);
                ViewBag.JsTimeScaleMask = GetJsTimeScaleMask(param.ProjectTimeScale);

                if (param.Verb == "add")
                {
                    var model = new GetActionModel
                    {
                        ProjectTimeScale = param.ProjectTimeScale,
                        ProcessId = param.ProcessId,
                        ScenarioId = param.ScenarioId,
                        Skills = param.Skills,
                        Referentials = param.Referentials,
                        PublishMode = param.PublishMode,
                        Verb = param.Verb,
                        DocumentationActionDraftWBS = new DocumentationActionDraftWBS
                        {
                            DocumentationPublishMode = (int)param.PublishMode,
                            IsGroup = false,
                            DocumentationActionDraft = new DocumentationActionDraft
                            {
                                IsKeyTask = false,
                                Duration = TimeSpan.FromSeconds(1).Ticks,
                                DurationString = DurationHelper.GetTimeScaleMaskValue(param.ProjectTimeScale, TimeSpan.FromSeconds(1).Ticks),
                                SkillId = -1
                            }
                        }
                    };
                    return PartialView(model);
                }

                if (param.DocumentationActionDraftWBS == null)
                    throw new ArgumentNullException(nameof(param.DocumentationActionDraftWBS));

                param.DocumentationActionDraftWBS.DocumentationActionDraft.DurationString = DurationHelper.GetTimeScaleMaskValue(param.ProjectTimeScale, param.DocumentationActionDraftWBS.DocumentationActionDraft.Duration);

                return PartialView(param);
            });

        public static long GetFileSize(string filename)
        {
            WebRequest webRequest = WebRequest.Create(WebConfigurationManager.AppSettings["FileServerUri"] + "/GetFileSize/" + filename);
            WebResponse webResp = webRequest.GetResponse();
            using (Stream stream = webResp.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                string responseString = reader.ReadToEnd().Replace("\"", "");
                return long.Parse(responseString);
            }
        }

        [HttpPost]
        public Task<ActionResult> GetFilenameFromTusId(string tusId) =>
            Task.Run<ActionResult>(async () =>
            {
                CloudFile newFile = await GetCloudFileFromTusId(tusId);
                if (newFile == null)
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Content("Unable to get file from TusId");
                }
                var serializer = new JavaScriptSerializer();
                return Content(serializer.Serialize(newFile), "application/json");
            });

        async Task<CloudFile> GetCloudFileFromTusId(string tusId)
        {
            try
            {
                CloudFile newFile = null;
                WebRequest webRequest = WebRequest.Create(WebConfigurationManager.AppSettings["FileServerUri"] + "/GetUploadNewName/" + tusId);
                WebResponse webResp = webRequest.GetResponse();
                using (Stream stream = webResp.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    var responseString = reader.ReadToEnd().Replace("\"", "");
                    var hash = Path.GetFileNameWithoutExtension(responseString);
                    var extension = Path.GetExtension(responseString);
                    newFile = await _referentialsService.GetCloudFile(hash);
                    if (newFile == null)
                        newFile = await _referentialsService.SaveCloudFile(new CloudFile(hash, extension));
                }

                return newFile;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        /// <summary>
        /// Retrieve next temporary id for documentation task. Used as temporary id only
        /// New id will be regenerated when saving in database
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        private int GetNextActionIdAvailable(List<GenericActionViewModel> actions, List<GenericActionViewModel> draftActions = null)
        {
            int maxActions = 0;
            int maxDraftActions = 0;

            if (actions.Any())
                maxActions = actions.Max(u => u.ActionId);
            if (draftActions != null && draftActions.Any())
                maxDraftActions = draftActions.Max(u => u.ActionId);

            var listMax = new List<int> { maxActions, maxDraftActions };

            return listMax.Max(u => u) + 1;
        }

        [HttpPost]
        public ActionResult DeleteConfirmation(int treeId, int publishMode)
        {
            ViewBag.TreeId = treeId;
            ViewBag.PublishMode = publishMode;
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> SaveDefault()
        {
            var uploadFile = System.Web.HttpContext.Current.Request.Files["uploadFile"];
            var (tusId, tusOperation) = TusFileTransferManager.Instance.Upload($"{WebConfigurationManager.AppSettings["FileServerUri"]}/files", uploadFile.FileName, uploadFile.InputStream);
            await tusOperation.WaitTransferFinished();
            var result = await GetFilenameFromTusId(tusId);
            return result;
        }

        public ActionResult RemoveDefault(string[] fileNames)
        {
            return Content("");
        }

        string GetTimeScaleHeader(long projectTimeScale)
        {
            if (projectTimeScale == TimeSpan.FromSeconds(1).Ticks)
                return "HH:MM:ss";
            else if (projectTimeScale == TimeSpan.FromSeconds(.1).Ticks)
                return "HH:MM:ss.s";
            else if (projectTimeScale == TimeSpan.FromSeconds(.01).Ticks)
                return "HH:MM:ss.ss";
            else if (projectTimeScale == TimeSpan.FromSeconds(.001).Ticks)
                return "HH:MM:ss.sss";
            return null;
        }

        string GetTimeScaleMask(long projectTimeScale)
        {
            if (projectTimeScale == TimeSpan.FromSeconds(1).Ticks)
                return "00:00:00";
            else if (projectTimeScale == TimeSpan.FromSeconds(.1).Ticks)
                return "00:00:00.0";
            else if (projectTimeScale == TimeSpan.FromSeconds(.01).Ticks)
                return "00:00:00.00";
            else if (projectTimeScale == TimeSpan.FromSeconds(.001).Ticks)
                return "00:00:00.000";
            return null;
        }

        string GetJsTimeScaleMask(long projectTimeScale)
        {
            if (projectTimeScale == TimeSpan.FromSeconds(1).Ticks)
                return "[0-9][0-9]:[0-9][0-9]:[0-9][0-9]";
            else if (projectTimeScale == TimeSpan.FromSeconds(.1).Ticks)
                return "[0-9][0-9]:[0-9][0-9]:[0-9][0-9].[0-9]";
            else if (projectTimeScale == TimeSpan.FromSeconds(.01).Ticks)
                return "[0-9][0-9]:[0-9][0-9]:[0-9][0-9].[0-9][0-9]";
            else if (projectTimeScale == TimeSpan.FromSeconds(.001).Ticks)
                return "[0-9][0-9]:[0-9][0-9]:[0-9][0-9].[0-9][0-9][0-9]";
            return null;
        }

        public class StateAsLabel
        {
            public string AsString { get; set; }
            public string AsLabel { get; set; }
        }
    }
}