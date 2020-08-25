using KProcess.KL2.JWT;
using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.KL2.WebAdmin.Mapper;
using KProcess.KL2.WebAdmin.Models.Audit;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using Syncfusion.EJ.Export;
using Syncfusion.JavaScript.Models;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KProcess.KL2.WebAdmin.Controllers
{
    [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Technician, KnownRoles.Documentalist)]
    [SettingUserContextFilter]
    public class AuditController : LocalizedController
    {
        readonly IApplicationUsersService _applicationUsersService;
        readonly IPrepareService _prepareService;
        readonly IReferentialsService _referentialsService;

        List<AuditViewModel> auditViewModel;
        List<AuditItemViewModel> auditItemViewModel;

        public int count;
        public string grid;

        public AuditController(
            IApplicationUsersService applicationUsersService,
            IPrepareService prepareService,
            IReferentialsService referentialsService,
            ILocalizationManager localizationManager
            )
            : base(localizationManager)
        {
            _applicationUsersService = applicationUsersService;
            _prepareService = prepareService;
            _referentialsService = referentialsService;
        }

        // List of audits
        [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Technician, KnownRoles.Documentalist)]
        public async Task<ActionResult> Index(int? userId, bool partial = false)
        {
            UserModel user = JwtTokenProvider.GetUserModel(Request.Cookies["token"].Value);
            if (!userId.HasValue && !user.Roles.Any(r => (new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Documentalist }.Contains(r))))
                return RedirectToAction("Index", new { userId = user.UserId, partial = partial });
            var audits = await _prepareService.GetAudits();
            if (userId.HasValue)
            {
                audits = audits.Where(a => a.AuditorId == userId.Value || a.Inspection.InspectionSteps.Any(s => s.InspectorId == userId.Value));
            }           

            var model = AuditMapper.ToAuditManageViewModel(audits);
            if (partial)
                return PartialView(model);
            return View(model);
        }


        public async Task<ActionResult> Detail(int? id, bool partial = false)
        {
            UserModel user = JwtTokenProvider.GetUserModel(Request.Cookies["token"].Value);
            var audits = await _prepareService.GetAudits(id);
            if (!audits.Any())
                return HttpNotFound();
            var audit = audits.First();
            if (!user.Roles.Any(r => (new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Documentalist }.Contains(r))) && audit.AuditorId != user.UserId && !audit.Inspection.InspectionSteps.Any(s => s.InspectorId == user.UserId))
                return RedirectToAction("Index", new { userId = user.UserId, partial = partial });
            var inspection = await _prepareService.GetInspection(audit.InspectionId);

            var inspectionSteps = InspectionController.GetInspectionStepAndChilds(inspection, inspection.Publication.PublishedActions.Distinct()
                    .OrderBy(a => a.WBSParts, new WBSPartsComparer()).ToList());
            var anomalies = await _prepareService.GetAnomalies(audit.InspectionId);
            var model = AuditMapper.ToAuditViewModel(audits.FirstOrDefault());
            model.Inspection = InspectionMapper.ToInspectionViewModel(inspection, inspectionSteps, anomalies.ToList());
            if (partial)
                return PartialView(model);
            return View(model);
        }

        [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Documentalist)]
        public async Task<ActionResult> AuditSummary(DateTime? month, bool partial = false)
        {
            var (users, roles, languages, teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            List<Team> allTeams = teams.OrderBy(t => t.Name).ToList();
            var audits = await _prepareService.GetAudits();
            if (month == null)
            {
                month = DateTime.Now;
            }
            audits = audits.Where(a => a.EndDate.Value.Month == month.Value.Month && a.EndDate.Value.Year == month.Value.Year);
            ViewBag.selectedMonth = month;
            List<AuditSummaryViewModel> model = new List<AuditSummaryViewModel>();
            if (audits.Any(a => !a.Auditor.Teams.Any()))
            {
                var total = audits.Count(a => !a.Auditor.Teams.Any());
                model.Add(new AuditSummaryViewModel
                {
                    TeamId = -1,
                    TeamName = LocalizedStrings.GetString("WithoutTeam"),
                    Total = total,
                    stringTotal = total.ToString(),
                    Color = total <= 1 ? "#FC1919" : "#07C900"
                });
            }
            foreach (var team in allTeams)
            {
                var total = audits.Count(a => a.Auditor.Teams.Any(t => t.Id == team.Id));
                model.Add(new AuditSummaryViewModel {
                    TeamId = team.Id,
                    TeamName = team.Name,
                    Total = total,
                    stringTotal = total.ToString(),
                    Color = total <= 1 ? "#FC1919" : "#07C900"
                });
            }
            if (partial)
                return PartialView(model);
            return View(model);
        }

        /// <summary>
        /// Verify ans creare an audit if possible
        /// </summary>
        /// <param name="InspectionId"></param>
        /// <param name="SurveyId"></param>
        /// <returns></returns>
        public async Task<JsonResult> VerifyAndCreateAudit(int InspectionId, int SurveyId)
        {
            try
            {
                UserModel user = JwtTokenProvider.GetUserModel(Request.Cookies["token"].Value);
                var result = await _prepareService.GetActiveAudit(user.UserId);
                if (result != null)
                    return Json(new { verified = false }, JsonRequestBehavior.AllowGet);

                var newAudit = new Audit
                {
                    SurveyId = SurveyId,
                    AuditorId = user.UserId,
                    InspectionId = InspectionId,
                    StartDate = DateTime.Now
                };
                var audit = await _prepareService.SaveAudit(newAudit);
                return Json(new { verified = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                return Json(new { verified = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            
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

            if (gridId == "Audits")
            {
                var audits = await _prepareService.GetAudits();
                if (userId.HasValue)
                {
                    audits = audits.Where(a => a.AuditorId == userId.Value || a.Inspection.InspectionSteps.Any(s => s.InspectorId == userId.Value));
                }
                var auditManage = AuditMapper.ToAuditManageViewModel(audits);
                //var dataSource = auditManage.Audits.ToList();
                //auditViewModel = dataSource;
                //var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");
                //obj.ServerExcelQueryCellInfo = QueryCellInfo;
                //exp.Export(obj, dataSource, "Audits " + currentDate + ".xlsx", ExcelVersion.Excel2013, false, false, "flat-saffron");

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    // Set the default application version as Excel 2016. 
                    excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2016;


                    //var group1 = auditManage.Audits.ToList();
                    var allAudits = auditManage.Audits.ToList();
                    int i = 0;
                    //Create a workbook with a worksheet. 
                    IWorkbook workbook = excelEngine.Excel.Workbooks.Create(audits.Select(a => a.Survey).Distinct().Count());

                    foreach (var survey in audits.Select(a => a.Survey).Distinct())
                    {
                        IWorksheet worksheet = workbook.Worksheets[i];
                        DataTable tbl = new DataTable();
                        //fill columns
                        tbl.Columns.Add(LocalizedStrings.GetString("AuditDate"), typeof(string));
                        tbl.Columns.Add(LocalizedStrings.GetString("Schedule"), typeof(string));
                        tbl.Columns.Add(LocalizedStrings.GetString("Team"), typeof(string));
                        tbl.Columns.Add(LocalizedStrings.GetString("Auditor"), typeof(string));
                        tbl.Columns.Add(LocalizedStrings.GetString("Auditee"), typeof(string));
                        tbl.Columns.Add(LocalizedStrings.GetString("AuditStatus"), typeof(string));
                        tbl.Columns.Add(LocalizedStrings.GetString("StandardAuditee"), typeof(string));
                        foreach (var surveyItem in survey.SurveyItems)
                        {
                            tbl.Columns.Add(surveyItem.Query, typeof(int));
                        }
                        tbl.Columns.Add("Score", typeof(int));
                        tbl.Columns.Add(LocalizedStrings.GetString("Comment"), typeof(string));
                        //fill rows
                        foreach (var audit in auditManage.Audits.Where(a => a.SurveyId == survey.Id))
                        {
                            DataRow dr = null;
                            dr = tbl.NewRow();
                            dr[LocalizedStrings.GetString("AuditDate")] = audit.EndDate.Value.ToShortDateString();
                            dr[LocalizedStrings.GetString("Schedule")] = audit.EndDate.Value.ToShortTimeString().Replace(":","H");
                            dr[LocalizedStrings.GetString("Team")] = string.Join(",", audit.AuditorTeams);
                            dr[LocalizedStrings.GetString("Auditor")] = audit.AuditorName;
                            dr[LocalizedStrings.GetString("Auditee")] = audit.AuditeeName;
                            dr[LocalizedStrings.GetString("AuditStatus")] = audits.Select(a => a.Auditor).FirstOrDefault(u => u.UserId == audit.AuditorId).Tenured.HasValue ? audits.Select(a => a.Auditor).FirstOrDefault(u => u.UserId == audit.AuditorId).Tenured.Value ? "Titulaire" : "Intérimaire" : "";
                            dr[LocalizedStrings.GetString("StandardAuditee")] = audit.ProcessName;
                            int scoreTemp = 0;
                            foreach (var surveyItem in survey.SurveyItems)
                            {
                                dr[surveyItem.Query] = audit.AuditItems.FirstOrDefault(item => item.Number == surveyItem.Number).IsOK.HasValue ? audit.AuditItems.FirstOrDefault(item => item.Number == surveyItem.Number).IsOK.Value ? 1 : 0 : -1;
                                if (audit.AuditItems.FirstOrDefault(item => item.Number == surveyItem.Number).IsOK.Value == true) scoreTemp++;
                            }
                            dr["Score"] = scoreTemp;
                            tbl.Rows.Add(dr);
                        }
                        worksheet.ImportDataTable(tbl,true,2,1);
                        worksheet.Name = survey.Name;
                        worksheet.UsedRange.WrapText = true;
                        worksheet.Columns[0].AutofitColumns();
                        worksheet.SetColumnWidth(1, 10);
                        IListObject table = worksheet.ListObjects.Create("tbl_" + survey.Name.Replace(" ","_"), worksheet.UsedRange);
                        table.BuiltInTableStyle = TableBuiltInStyles.TableStyleLight9;
                        foreach (var surveyItem in survey.SurveyItems)
                        {
                            //Apply conditional formats for IsOK questionnaire items
                            IConditionalFormats condition = worksheet.Columns[table.Columns.IndexOf(s => s.Name == surveyItem.Query)].ConditionalFormats;
                            IConditionalFormat condition1 = condition.AddCondition();
                            condition1.FormatType = ExcelCFType.CellValue;
                            condition1.Operator = ExcelComparisonOperator.Equal;
                            condition1.FirstFormula = "1";
                            condition1.BackColor = ExcelKnownColors.Green;
                            IConditionalFormat condition2 = condition.AddCondition();
                            condition2.FormatType = ExcelCFType.CellValue;
                            condition2.Operator = ExcelComparisonOperator.Equal;
                            condition2.FirstFormula = "0";
                            condition2.BackColor = ExcelKnownColors.Red;
                            worksheet.Columns[table.Columns.IndexOf(s => s.Name == surveyItem.Query)].RowHeight = 80;
                            worksheet.AutofitColumn(table.Columns.IndexOf(s => s.Name == surveyItem.Query));
                        }
                        i++;
                    }
                    //worksheet.ImportData(group1, 2, 1, true);
                    
                    var path = Server.MapPath("~/App_Data/" + LocalizedStrings.GetString("Audits") + ".xlsx");

                    workbook.SaveAs(path, HttpContext.ApplicationInstance.Response, ExcelDownloadType.PromptDialog, ExcelHttpContentType.Excel2010);
                    workbook.Close();
                    excelEngine.Dispose();
                }
            }
            if (gridId == "AuditItems")
            {
                var audits = await _prepareService.GetAudits(id);
                var audit = AuditMapper.ToAuditViewModel(audits.FirstOrDefault());
                var dataSource = audit.AuditItems.ToList();
                auditItemViewModel = dataSource;
                var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");
                obj.ServerExcelQueryCellInfo = QueryCellInfo;
                exp.Export(obj, dataSource, LocalizedStrings.GetString("Audit") + " " + process + " " + currentDate + ".xlsx", ExcelVersion.Excel2013, false, false, "flat-saffron");
            }
        }

        private void QueryCellInfo(object obj)
        {
            IRange range = (IRange)obj;
            
            if (grid == "Audits")
            {
                if (auditViewModel[count].IsOK == false)
                {
                    range.CellStyle.Color = Color.Red;
                    range.CellStyle.Font.Color = ExcelKnownColors.White;
                }
                if (range.Column == 3)
                {
                    //Auditor team column
                    range.Value = string.Join(",", auditViewModel[count].AuditorTeams);
                }
                if (range.Column == 7)
                {
                    //Last column
                    //increment the count to go to the next row data
                    count++;
                }
            }
            if (grid == "AuditItems")
            {
                //Result column
                if (range.Column == 2)
                {
                    if (auditItemViewModel[count].IsOK == true)
                    {
                        range.Value = "OK";
                        range.CellStyle.Color = Color.Green;
                        range.CellStyle.Font.Color = ExcelKnownColors.White;
                    }
                    else if(auditItemViewModel[count].IsOK == false)
                    {
                        range.Value = "NOK";
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