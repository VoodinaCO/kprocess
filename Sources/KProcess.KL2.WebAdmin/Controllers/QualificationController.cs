using KProcess.KL2.JWT;
using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.KL2.WebAdmin.Mapper;
using KProcess.KL2.WebAdmin.Models.Qualification;
using KProcess.KL2.WebAdmin.Utils;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using Syncfusion.EJ.Export;
using Syncfusion.JavaScript.Models;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KProcess.KL2.WebAdmin.Controllers
{
    [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator, KnownRoles.Documentalist)]
    [SettingUserContextFilter]
    public class QualificationController : LocalizedController
    {
        readonly IApplicationUsersService _applicationUsersService;
        readonly IPrepareService _prepareService;
        readonly IReferentialsService _referentialsService;
        readonly ITraceManager _traceManager;

        List<QualificationViewModel> qualificationViewModelData;
        List<QualificationStepViewModel> qualificationStepViewModelData;

        public int count;
        public string grid;

        public QualificationController(
            IApplicationUsersService applicationUsersService,
            IPrepareService prepareService,
            IReferentialsService referentialsService,
            ITraceManager traceManager,
            ILocalizationManager localizationManager
            )
            : base(localizationManager)
        {
            _applicationUsersService = applicationUsersService;
            _prepareService = prepareService;
            _referentialsService = referentialsService;
            _traceManager = traceManager;
        }

        public async Task<ActionResult> Index(int? userId, bool partial = false)
        {
            UserModel user = JwtTokenProvider.GetUserModel(Request.Cookies["token"].Value);
            if (!userId.HasValue && !user.Roles.Any(r => (new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Documentalist }.Contains(r))))
                return RedirectToAction("Index", new { userId = user.UserId, partial = partial });
            _traceManager.TraceDebug("QualificationController starting index");
            var usersRolesLanguages = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var users = usersRolesLanguages.Users.ToList();
            var lastTrainingPublications = await _prepareService.GetLastSameMajorPublications((int)PublishModeEnum.Formation);
            var lastEvaluationPublications = await _prepareService.GetLastSameMajorPublications((int)PublishModeEnum.Evaluation);
            var qualificationManageViewModel = await QualificationMapper.ToQualificationManageViewModel(lastTrainingPublications, lastEvaluationPublications, users, userId);
            if (partial)
                return PartialView(qualificationManageViewModel);
            return View(qualificationManageViewModel);
        }

        public async Task<ActionResult> Detail(int id, bool partial = false)
        {
            UserModel user = JwtTokenProvider.GetUserModel(Request.Cookies["token"].Value);
            var usersRolesLanguages = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var qualification = await _prepareService.GetQualification(id);
            if (!user.Roles.Any(r => (new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Documentalist }.Contains(r))) && qualification.UserId != user.UserId)
                return RedirectToAction("Index", new { userId = user.UserId, partial = partial });
            // Retrieve all the action of the publication
            var actionSorted = qualification.Publication.PublishedActions.Distinct()
                    .OrderBy(a => a.WBSParts, new WBSPartsComparer())
                    .ToList();

            var qualificationSteps = GetQualificationStepAndChilds(qualification, actionSorted);
            var trainingPublications = await _prepareService.GetTrainingPublications(new[] { qualification.PublicationId });
            var qualificationManageViewModel = QualificationMapper.ToQualificationViewModel(
                qualification, qualificationSteps, usersRolesLanguages.Users.ToList(), trainingPublications);
            if (partial)
                return PartialView(qualificationManageViewModel);
            return View(qualificationManageViewModel);
        }


        /// <summary>
        /// One level is done
        /// </summary>
        /// <param name="qualification"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        private List<QualificationStep> GetQualificationStepAndChilds(
            Qualification qualification, List<PublishedAction> others)
        {
            var steps = new List<QualificationStep>();

            var qualificationSteps = qualification.QualificationSteps.OrderBy(a => a.PublishedAction.WBSParts, new WBSPartsComparer()).ToList();
            foreach (var step in qualificationSteps)
            {
                // Display the parent first
                int level = 0;
                foreach (var parent in WbsUtil.GetParents(step.PublishedAction, others))
                {
                    // If parent has been already added, we skip it
                    if (!steps.Any(u => u.PublishedAction.WBS == parent.WBS))
                    {
                        steps.Add(new QualificationStep
                        {
                            IsQualified = step.IsQualified,
                            Level = level,
                            PublishedAction = new PublishedAction
                            {
                                WBS = parent.WBS,
                                Label = parent.Label
                            },
                            IsParent = true
                        });
                    }
                    level = level + 1;
                }
                // Add the current one
                step.Level = level;
                steps.Add(step);

                // Check the linked publication
                var linkedPublication = step.PublishedAction.LinkedPublication;
                if (linkedPublication == null)
                    continue;

                // Traitement des sous process
                foreach (var publishedAction in step.PublishedAction.LinkedPublication.PublishedActions)
                {
                    var linkedQualification = step.PublishedAction.LinkedPublication.Qualifications
                        .Where(d => d.EndDate != null && !d.IsDeleted && d.PublicationId == step.PublishedAction.LinkedPublicationId)
                        .OrderByDescending(d => d.EndDate).FirstOrDefault();

                    if (linkedQualification == null)
                        continue;

                    foreach (var linkedStep in linkedQualification.QualificationSteps)
                    {
                        linkedStep.Level = 1;
                        if (!steps.Any(u => u.QualificationStepId == linkedStep.QualificationStepId))
                        {
                            linkedStep.PublishedAction.WBS = step.PublishedAction.WBS + "." + linkedStep.PublishedAction.WBS;
                            steps.Add(linkedStep);
                        }
                    }
                }
            }
            return steps;
        }

        public async Task ExportToExcel(string GridModel, string gridId, int id, string process, int? userId)
        {
            ExcelExport exp = new ExcelExport();
            GridProperties obj = (GridProperties)Syncfusion.JavaScript.Utils.DeserializeToModel(typeof(GridProperties), GridModel);
            //Clear if there are any filter columns
            //syncfusion bug in exporting while in filter mode
            obj.FilterSettings.FilteredColumns.Clear();
            grid = gridId;
            count = 0;
            var usersRolesLanguages = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var users = new List<User>();
            var userPools = await LicenseMapper.GetUserPools();
            users = usersRolesLanguages.Users.Where(u => userPools.Any(p => p == u.UserId)).ToList();

            if (gridId == "Qualification")
            {
                var lastTrainingPublications = await _prepareService.GetLastPublications((int)PublishModeEnum.Formation);
                var lastEvaluationPublications = await _prepareService.GetLastPublications((int)PublishModeEnum.Evaluation);
                var qualificationManageViewModel = await QualificationMapper.ToQualificationManageViewModel(lastTrainingPublications, lastEvaluationPublications, users, userId);
                var dataSource = qualificationManageViewModel.Qualifications.ToList();
                qualificationViewModelData = qualificationManageViewModel.Qualifications.ToList();
                var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");
                obj.Columns[10].Field = "Result";
                obj.ServerExcelQueryCellInfo = QueryCellInfo;
                exp.Export(obj, dataSource, LocalizedStrings.GetString("Qualification") + " " + currentDate + ".xlsx", ExcelVersion.Excel2013, false, false, "flat-saffron");
            }
            if (gridId == "QualificationDetail")
            {
                var qualification = await _prepareService.GetQualification(id);
                // Retrieve all the action of the publication
                var actionSorted = qualification.Publication.PublishedActions.Distinct()
                        .OrderBy(a => a.WBSParts, new WBSPartsComparer())
                        .ToList();

                var qualificationSteps =  GetQualificationStepAndChilds(qualification, actionSorted);
                var trainingPublications = await _prepareService.GetTrainingPublications(new[] { qualification.PublicationId });
                var qualificationManageViewModel = QualificationMapper.ToQualificationViewModel(qualification, qualificationSteps, users, trainingPublications);
                var dataSource = qualificationManageViewModel.Steps.ToList();
                qualificationStepViewModelData = qualificationManageViewModel.Steps.ToList();
                var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");
                obj.Columns[5].Field = HeadersHelper.IsQualified;
                obj.ServerExcelQueryCellInfo = QueryCellInfo;
                exp.Export(obj, dataSource, LocalizedStrings.GetString("QualificationDetail") + " " + process + " " +  currentDate + ".xlsx", ExcelVersion.Excel2013, false, false, "flat-saffron");
            }
        }

        private void QueryCellInfo(object obj)
        {
            IRange range = (IRange)obj;
            if (grid == "Qualification")
            {
                if (range.Column == 4)
                {
                    //column index for Teams in excel
                    range.Value = string.Join(",", qualificationViewModelData[count].Teams);
                }
                if (range.Column == 10)
                {
                    //column index for Result in excel
                    if (qualificationViewModelData[count].Result == true)
                    {
                        range.Value = qualificationViewModelData[count].PercentageResult + " %";
                        range.CellStyle.Color = Color.Green;
                        range.CellStyle.Font.Color = ExcelKnownColors.White;
                    }
                    else
                    {
                        range.Value = qualificationViewModelData[count].PercentageResult + " %";
                        range.CellStyle.Color = Color.Red;
                        range.CellStyle.Font.Color = ExcelKnownColors.White;
                    }
                    //increment the count to go to the next row data
                    count++;
                }
            }
            if (grid == "QualificationDetail")
            {
                //Action label column
                if (range.Column == 1)
                {
                    if (qualificationStepViewModelData[count].Level > 0)
                    {
                        range.IndentLevel = 2;
                    }
                }
                //IsQualified column
                if (range.Column == 5)
                {
                    if (qualificationStepViewModelData[count].IsQualified == true)
                    {
                        range.Value = qualificationStepViewModelData[count].Date;
                        range.CellStyle.Color = Color.Green;
                        range.CellStyle.Font.Color = ExcelKnownColors.White;
                    }
                    else
                    {
                        range.Value = "";
                        range.CellStyle.Color = Color.Red;
                        range.CellStyle.Font.Color = ExcelKnownColors.White;
                    }
                    //increment the count to go to the next row data
                    count++;
                }
            }
        }
    }
}