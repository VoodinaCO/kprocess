using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.KL2.WebAdmin.Mapper;
using KProcess.KL2.WebAdmin.Models;
using KProcess.KL2.WebAdmin.Models.Inspection;
using KProcess.KL2.WebAdmin.Utils;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Syncfusion.EJ.Export;
using Syncfusion.JavaScript.Models;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace KProcess.KL2.WebAdmin.Controllers
{
    [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Technician)]
    [SettingUserContextFilter]
    public class InspectionController : LocalizedController
    {
        readonly ITraceManager _traceManager;
        readonly IApplicationUsersService _applicationUsersService;
        readonly IPrepareService _prepareService;
        readonly IReferentialsService _referentialsService;

        List<InspectionStepViewModel> inspectionStepViewModelData = null;
        List<InspectionViewModel> inspectionViewModelData = null;

        public int count;
        public string grid;

        public InspectionController(
            ITraceManager traceManager,
            IApplicationUsersService applicationUsersService,
            IPrepareService prepareService,
            IReferentialsService referentialsService,
            ILocalizationManager localizationManager
            )
            : base(localizationManager)
        {
            _traceManager = traceManager;
            _applicationUsersService = applicationUsersService;
            _prepareService = prepareService;
            _referentialsService = referentialsService;
        }

        public async Task<ActionResult> Index(string team, string userId, string publicationId, int? opeId, bool partial = false)
        {
            //check if user can create audit for inspection
            var currentUserId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var currentAudit = await _prepareService.GetActiveAudit(currentUserId);
            var inspections = (await _prepareService.GetLastPublications((int)PublishModeEnum.Inspection))
                .SelectMany(p => p.Inspections)
                .OrderByDescending(i => i.StartDate)
                .ToList();
            ViewBag.AllowCreateAudit = currentAudit == null;

            if (team == null && userId == null && publicationId == null)
            {
                var model = await InspectionMapper.ToInspectionManageViewModel(inspections);
                model.CurrentAuditInspectionId = currentAudit != null ? currentAudit.InspectionId : 0;
                model.IsCurrentAuditStarted = currentAudit != null && currentAudit.AuditItems != null ? currentAudit.AuditItems.Any() : false;
                model.Question = currentAudit != null ? currentAudit.Survey.Name : "";
                if (partial)
                    return PartialView(model);
                return View(model);
            }
            else
            {
                var (Users, Teams) = await _applicationUsersService.GetUsersTeams("0", "All", KnownRoles.Operator);
                var model = await InspectionMapper.ToInspectionManageViewModel(inspections, team, userId, publicationId);
                model.CurrentAuditInspectionId = currentAudit != null ? currentAudit.Id : 0;
                model.IsCurrentAuditStarted = currentAudit != null && currentAudit.AuditItems != null ? currentAudit.AuditItems.Any() : false;
                model.Question = currentAudit != null ? currentAudit.Survey.Name : "";
                var inspectionPublications = await _prepareService.GetLastPublicationsForFilter((int)PublishModeEnum.Inspection);
                var teams = Teams.Select(t => t.Id).ToList();
                var operators = Users.Select(u => u.UserId).ToList();
                var publications = inspectionPublications.Select(p => p.PublicationId.ToString()).ToList();
                model.selectedIndexTeam = (team == "0") ? 0 : teams.IndexOf(Convert.ToInt16(team)) + 1;
                model.selectedIndexOperator = (userId == "0") ? 0 : operators.IndexOf(Convert.ToInt16(userId)) + 1;
                model.selectedIndexPublication = (publicationId == "0") ? 0 : publications.IndexOf(publicationId) + 1;
                if (partial)
                    return PartialView(model);
                return View(model);
            }
        }

        public async Task<ActionResult> Detail(int id, bool partial = false)
        {
            ////check if user can create audit for inspection
            //var auditController = DependencyResolver.Current.GetService<AuditController>();
            //var allowCreateAudit = await auditController.allowUserCreateAudit();
            //ViewBag.AllowCreateAudit = allowCreateAudit;
            var inspection = await _prepareService.GetInspection(id);
            // Retrieve all the action of the publication
            var actionSorted = inspection.Publication.PublishedActions.Distinct()
                    .OrderBy(a => a.WBSParts, new WBSPartsComparer())
                    .ToList();

            var inspectionSteps =  GetInspectionStepAndChilds(inspection, actionSorted);
            var anomalies = await _prepareService.GetAnomalies(id);
            var model = InspectionMapper.ToInspectionViewModel(inspection, inspectionSteps, anomalies.ToList());
            if (partial)
                return PartialView(model);
            return View(model);
        }

        public static List<InspectionStep> GetInspectionStepAndChilds(Inspection inspection, List<PublishedAction> others)
        {
            var steps = new List<InspectionStep>();
            foreach (var step in inspection.InspectionSteps)
            {
                // Display the parent first
                int level = 0;
                foreach (var parent in WbsUtil.GetParents(step.PublishedAction, others))
                {
                    // If parent has been already added, we skip it
                    if (!steps.Any(u => u.PublishedAction.WBS == parent.WBS))
                    {
                        steps.Add(new InspectionStep
                        {
                            IsOk = step.IsOk,
                            Level = level,
                            PublishedAction = new PublishedAction
                            {
                                WBS = parent.WBS,
                                Label = parent.Label
                            },
                            IsParent = true
                        });
                    } 
                    level++;
                }

                step.Level = level;
                steps.Add(step);

                var linkedPublication = step.PublishedAction.LinkedPublication;
                if (linkedPublication == null)
                    continue;

                // Traitement des sous process
                foreach (var publishedAction in step.PublishedAction.LinkedPublication.PublishedActions)
                {
                    var linkedInspection = step.PublishedAction.LinkedPublication.Inspections
                         .Where(d => d.EndDate != null && !d.IsDeleted && d.PublicationId == step.PublishedAction.LinkedPublicationId)
                         .OrderByDescending(d => d.EndDate).FirstOrDefault();

                    if (linkedInspection == null)
                        continue;

                    foreach (var linkedStep in linkedInspection.InspectionSteps)
                    {
                        linkedStep.Level = 1;
                        if (!steps.Any(u => u.InspectionStepId == linkedStep.InspectionStepId))
                        {
                            linkedStep.PublishedAction.WBS = step.PublishedAction.WBS + "." + linkedStep.PublishedAction.WBS;
                            steps.Add(linkedStep);
                        }
                    }
                }
            }
            return steps;
        }

        public async Task<ActionResult> GetAnomalyDetail(int AnomalyId)
        {
            var anomaly = await _prepareService.GetAnomaly(AnomalyId);
            var model = new AnomalyViewModel
            {
                AnomalyId = anomaly.Id,
                Category = anomaly.Category,
                Date = anomaly.Date,
                Description = anomaly.Description,
                HasPhoto = anomaly.Photo != null ? true : false,
                Photo = anomaly.Photo == null ? "" : string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(anomaly.Photo)),
                Label = anomaly.Label,
                Line = anomaly.Line,
                Machine = anomaly.Machine,
                Priority = (int)anomaly.Priority.Value,
                Type = (int)anomaly.Type,
                TypeLabel = anomaly.Type.AnomalyTypeToString(),
                TypeColor = anomaly.Type == AnomalyType.Security ? "green" : anomaly.Type == AnomalyType.Maintenance ? "red" : anomaly.Type == AnomalyType.Operator ? "blue" : "gray"
            };
            model.PriorityLists = new List<Tuple<int, string>>
            {
                new Tuple<int, string>(1, "A"),
                new Tuple<int, string>(2, "B"),
                new Tuple<int, string>(3, "C")
            };

            model.KindItems = new List<IAnomalyKindItem>();
            model.KindItems = Anomalies.GetPossibleAnomalies(anomaly.Type);
            return PartialView("AnomalyDetail", model);
        }

        public async Task ExportToExcel(string GridModel, string gridId, int id, string process)
        {
            ExcelExport exp = new ExcelExport();
            GridProperties obj = (GridProperties)Syncfusion.JavaScript.Utils.DeserializeToModel(typeof(GridProperties), GridModel);
            //Clear if there are any filter columns
            //syncfusion bug in exporting while in filter mode
            obj.FilterSettings.FilteredColumns.Clear();
            grid = gridId;
            count = 0;
            if (gridId == "InspectionDetail")
            {
                var inspection = await _prepareService.GetInspection(id);
                // Retrieve all the action of the publication
                var actionSorted = inspection.Publication.PublishedActions.Distinct()
                        .OrderBy(a => a.WBSParts, new WBSPartsComparer())
                        .ToList();

                var inspectionSteps = GetInspectionStepAndChilds(inspection, actionSorted);
                var model = InspectionMapper.ToInspectionViewModel(inspection, inspectionSteps, new List<Anomaly>());
                var dataSource = model.Steps.ToList();
                inspectionStepViewModelData = dataSource;
                var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");
                obj.ServerExcelQueryCellInfo = QueryCellInfo;
                exp.Export(obj, dataSource, LocalizedStrings.GetString("Inspection") + " " + process + " " + currentDate + ".xlsx", ExcelVersion.Excel2013, false, false, "flat-saffron");
            }
            if (gridId == "InspectionManage")
            {
                var inspections = await _prepareService.GetInspections();
                var model = await InspectionMapper.ToInspectionManageViewModel(inspections);
                var dataSource = model.Inspections.ToList();
                inspectionViewModelData = dataSource;
                var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");
                obj.ServerExcelQueryCellInfo = QueryCellInfo;
                exp.Export(obj, dataSource, LocalizedStrings.GetString("Inspection") + " " + currentDate + ".xlsx", ExcelVersion.Excel2013, false, false, "flat-saffron");
            }
            if (gridId == "Anomaly")
            {
                var inspection = await _prepareService.GetInspection(id);
                var anomalies = await _prepareService.GetAnomalies(id);
                var model = InspectionMapper.ToInspectionViewModel(inspection, new List<InspectionStep>(), anomalies);
                var dataSource = model.Anomalies.ToList();
                var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");
                exp.Export(obj, dataSource, LocalizedStrings.GetString("Anomaly") + " " + process + " " + currentDate + ".xlsx", ExcelVersion.Excel2013, false, false, "flat-saffron");
            }
        }

        public async Task ExportToExcelInspectionDetail(string gridColumns, string gridHeaders, int id, string process)
        {
            try
            {
                List<Column> columnsInfos = (List<Column>)Syncfusion.JavaScript.Utils.DeserializeToModel(typeof(List<Column>), gridColumns);
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                var columnHeaders = JsonConvert.DeserializeObject<List<GridHeader>>(gridHeaders);

                var inspection = await _prepareService.GetInspection(id);
                // Retrieve all the action of the publication
                var actionSorted = inspection.Publication.PublishedActions.Distinct()
                        .OrderBy(a => a.WBSParts, new WBSPartsComparer())
                        .ToList();
                var inspectionSteps = GetInspectionStepAndChilds(inspection, actionSorted);
                var model = InspectionMapper.ToInspectionViewModel(inspection, inspectionSteps, new List<Anomaly>());
                var dataSource = model.Steps.ToList();

                var fileName = LocalizedStrings.GetString("Inspection") + " " + process + " " + ".xlsx";

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    //application.DefaultVersion = ExcelVersion.Excel2013;
                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    int rowCounter = 1;
                    int columnCounter = 1;
                    var headers = columnsInfos.Where(_ => _.Visible);
                    var shapes = new List<IPictureShape>();
                    var rangeLocations = new List<RangeLocation>();
                    var shapeCounter = 1;

                    // Adding headers
                    foreach (var header in headers)
                    {
                        worksheet.Range[rowCounter, columnCounter].VerticalAlignment = ExcelVAlign.VAlignCenter;
                        worksheet.Range[rowCounter, columnCounter].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        worksheet.Range[rowCounter, columnCounter++].Text = columnHeaders.FirstOrDefault(h => h.Field == header.Field).HeaderText;
                    }

                    foreach (var data in dataSource)
                    {
                        rowCounter++;
                        columnCounter = 1;
                        foreach (var header in headers)
                        {
                            if ( header.Field != "Photo" && (data.GetType().GetProperty(header.Field) == null || (data.GetType().GetProperty(header.Field).GetValue(data) == null && header.Field != "Description")))
                                continue;
                            var range = worksheet.Range[rowCounter, columnCounter++];
                            switch (header.Field)
                            {
                                case "ActionLabel":
                                    if (data.Level > 0)
                                        range.IndentLevel = 2;
                                    range.Value = data.GetType().GetProperty(header.Field).GetValue(data).ToString();
                                    break;
                                case "IsOK":
                                    if (data.IsOK == true)
                                    {
                                        range.Value = "OK";
                                        range.CellStyle.Color = Color.Green;
                                        range.CellStyle.Font.Color = ExcelKnownColors.White;
                                    }
                                    else
                                    {
                                        range.Value = "NOK";
                                        range.CellStyle.Color = Color.Red;
                                        range.CellStyle.Font.Color = ExcelKnownColors.White;
                                    }
                                    break;
                                case "Photo":
                                    if (data.HasThumbnail)
                                    {
                                        range.VerticalAlignment = ExcelVAlign.VAlignTop;
                                        var fileServer = ConfigurationManager.AppSettings["FileServerUri"];
                                        var filePath = data.ThumbnailHash + data.ThumbnailExt;
                                        try
                                        {
                                            range.Value = "";
                                            var downloadPath = Path.Combine(fileServer, "GetFile", filePath);
                                            downloadPath = downloadPath.Replace("\\", "/");

                                            var webClient = new WebClient();
                                            var imageBytes = webClient.DownloadData(downloadPath);
                                            var memoryStream = new MemoryStream(imageBytes);

                                            var imageFile = Image.FromStream(memoryStream);

                                            int maxPixelSize = 500;
                                            double maxWidthPoints = maxPixelSize.PixelsToWidthPoints();
                                            double maxHeightPoints = maxPixelSize.PixelsToHeightPoints();
                                            var widthScale = Convert.ToInt32(maxWidthPoints / imageFile.Width.PixelsToWidthPoints() * 100.0);
                                            var heightScale = Convert.ToInt32(maxHeightPoints / imageFile.Height.PixelsToHeightPoints() * 100.0);

                                            var imageScaleInPercent = Math.Min(widthScale, heightScale);

                                            range.RowHeight = (imageFile.Height >= imageFile.Width ? maxHeightPoints : (imageFile.Height * imageScaleInPercent / 100.0).PixelsToHeightPoints()) + 61;
                                            range.ColumnWidth = range.ColumnWidth >= maxWidthPoints + 22.86 ? range.ColumnWidth : maxWidthPoints + 22.86;
                                            var shape = range.Worksheet.Pictures.AddPicture(range.Row, range.Column, imageFile, imageScaleInPercent, imageScaleInPercent);
                                            shape.Name = "shape" + shapeCounter;
                                            shapes.Add(shape);
                                            rangeLocations.Add(new RangeLocation { ShapeName = shape.Name, Row = range.Row, Column = range.Column });
                                            shapeCounter++;
                                        }
                                        catch (Exception ex)
                                        {
                                            _traceManager.TraceError(ex, $"Error on {nameof(ExportToExcelInspectionDetail)}");
                                            range.Value = "";
                                        }
                                    }
                                    break;
                                default:
                                    range.Value = data.GetType().GetProperty(header.Field).GetValue(data) == null ? "" : data.GetType().GetProperty(header.Field).GetValue(data).ToString();
                                    break;
                            }
                        }
                    }

                    foreach (var shape in shapes)
                    {
                        //Rearrange cell height
                        var range = worksheet.Range[rangeLocations.FirstOrDefault(r => r.ShapeName == shape.Name).Row, rangeLocations.FirstOrDefault(r => r.ShapeName == shape.Name).Column];
                        range.RowHeight = shape.Height.PixelsToHeightPoints();
                    }

                    // Saving the workbook to disk in XLSX format
                    workbook.SaveAs(fileName,
                        ExcelSaveType.SaveAsXLS,
                        System.Web.HttpContext.Current.Response,
                        ExcelDownloadType.Open);
                }
            }
            catch (Exception e)
            {
                _traceManager.TraceError(e, $"Error on {nameof(ExportToExcelInspectionDetail)}");
            }
        }

        public async Task ExportToExcelAnomaly(string gridColumns, string gridHeaders, int id, string process)
        {
            try
            {
                List<Column> columnsInfos = (List<Column>)Syncfusion.JavaScript.Utils.DeserializeToModel(typeof(List<Column>), gridColumns);
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();
                var columnHeaders = JsonConvert.DeserializeObject<List<GridHeader>>(gridHeaders);

                var inspection = await _prepareService.GetInspection(id);
                var anomalies = await _prepareService.GetAnomalies(id);
                var model = InspectionMapper.ToInspectionViewModel(inspection, new List<InspectionStep>(), anomalies);
                var dataSource = model.Anomalies.ToList();

                var fileName = LocalizedStrings.GetString("Anomaly") + " " + process + " " + ".xlsx";

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    //application.DefaultVersion = ExcelVersion.Excel2013;
                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    int rowCounter = 1;
                    int columnCounter = 1;
                    var headers = columnsInfos.Where(_ => _.Visible);
                    var shapes = new List<IPictureShape>();
                    var rangeLocations = new List<RangeLocation>();
                    var shapeCounter = 1;

                    // Adding headers
                    foreach (var header in headers)
                    {
                        worksheet.Range[rowCounter, columnCounter].VerticalAlignment = ExcelVAlign.VAlignCenter;
                        worksheet.Range[rowCounter, columnCounter].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        worksheet.Range[rowCounter, columnCounter++].Text = columnHeaders.FirstOrDefault(h => h.Field == header.Field).HeaderText;
                    }

                    foreach (var data in dataSource)
                    {
                        rowCounter++;
                        columnCounter = 1;
                        foreach (var header in headers)
                        {
                            if (header.Field != "Photo" && (data.GetType().GetProperty(header.Field) == null || (data.GetType().GetProperty(header.Field).GetValue(data) == null && header.Field != "Description")))
                                continue;
                            var range = worksheet.Range[rowCounter, columnCounter++];
                            switch (header.Field)
                            {
                                case "Photo":
                                    if (data.HasPhoto)
                                    {
                                        range.VerticalAlignment = ExcelVAlign.VAlignTop;
                                        try
                                        {
                                            range.Value = "";
                                            var memoryStream = new MemoryStream(Convert.FromBase64String(data.RawPhoto));

                                            var imageFile = Image.FromStream(memoryStream);

                                            int maxPixelSize = 500;
                                            double maxWidthPoints = maxPixelSize.PixelsToWidthPoints();
                                            double maxHeightPoints = maxPixelSize.PixelsToHeightPoints();
                                            var widthScale = Convert.ToInt32(maxWidthPoints / imageFile.Width.PixelsToWidthPoints() * 100.0);
                                            var heightScale = Convert.ToInt32(maxHeightPoints / imageFile.Height.PixelsToHeightPoints() * 100.0);

                                            var imageScaleInPercent = Math.Min(widthScale, heightScale);
                                            range.RowHeight = (imageFile.Height >= imageFile.Width ? maxHeightPoints : (imageFile.Height * imageScaleInPercent / 100.0).PixelsToHeightPoints());
                                            range.ColumnWidth = maxWidthPoints;
                                            var shape = range.Worksheet.Pictures.AddPicture(range.Row, range.Column, imageFile, imageScaleInPercent, imageScaleInPercent);
                                            shape.Name = "shape" + shapeCounter;
                                            shapes.Add(shape);
                                            rangeLocations.Add(new RangeLocation { ShapeName = shape.Name, Row = range.Row, Column = range.Column });
                                            shapeCounter++;
                                        }
                                        catch (Exception ex)
                                        {
                                            _traceManager.TraceError(ex, $"Error on {nameof(ExportToExcelAnomaly)}");
                                            range.Value = "";
                                        }
                                    }
                                    break;
                                default:
                                    range.Value = data.GetType().GetProperty(header.Field).GetValue(data) == null ? "" : data.GetType().GetProperty(header.Field).GetValue(data).ToString();
                                    break;
                            }
                        }
                    }

                    foreach (var shape in shapes)
                    {
                        //Rearrange cell height
                        var range = worksheet.Range[rangeLocations.FirstOrDefault(r => r.ShapeName == shape.Name).Row, rangeLocations.FirstOrDefault(r => r.ShapeName == shape.Name).Column];
                        range.RowHeight = shape.Height.PixelsToHeightPoints();
                    }

                    // Saving the workbook to disk in XLSX format
                    workbook.SaveAs(fileName,
                        ExcelSaveType.SaveAsXLS,
                        System.Web.HttpContext.Current.Response,
                        ExcelDownloadType.Open);
                }
            }
            catch (Exception e)
            {
                _traceManager.TraceError(e, $"Error on {nameof(ExportToExcelAnomaly)}");
            }
        }

        private void QueryCellInfo(object obj)
        {
            IRange range = (IRange)obj;
            if (grid == "InspectionDetail")
            {
                //Action label column
                if (range.Column == 2)
                {
                    if (inspectionStepViewModelData[count].Level > 0)
                    {
                        range.IndentLevel = 2;
                    }
                }
                
                //Result column
                if (range.Column == 3)
                {
                    if (inspectionStepViewModelData[count].IsOK == true)
                    {
                        range.Value = "OK";
                        range.CellStyle.Color = Color.Green;
                        range.CellStyle.Font.Color = ExcelKnownColors.White;
                    }
                    else
                    {
                        range.Value = "NOK";
                        range.CellStyle.Color = Color.Red;
                        range.CellStyle.Font.Color = ExcelKnownColors.White;
                    }
                    //increment the count to go to the next row data
                    count++;
                }
            }
            if (grid == "InspectionManage")
            {
                if(inspectionViewModelData[count].IsOK == false)
                {
                    range.CellStyle.Color = Color.Red;
                    range.CellStyle.Font.Color = ExcelKnownColors.White;
                }
                if (range.Column == 3)
                {
                    //Operators
                    range.Value = string.Join(", ", inspectionViewModelData[count].Inspectors);
                }
                if (range.Column == 4)
                {
                    //Teams
                    range.Value = string.Join(", ", inspectionViewModelData[count].Teams);
                }
                if (range.Column == 6)
                {
                    //Last column : Anomaly
                    //increment the count to go to the next row data
                    count++;
                }
            }
        }

        /// <summary>
        /// Delete the current audit associated to this inspection
        /// </summary>
        /// <param name="inspectionId"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteInspectionCurrentAuditAsync()
        {
            try
            {
                var userId = Convert.ToInt32(System.Web.HttpContext.Current.User.Identity.GetUserId());
                var currentAudit = await _prepareService.GetActiveAudit(userId);

                if (currentAudit == null)
                    return Json(new { Success = false, Message = LocalizedStrings.GetString("Audit_Not_Exist_Anymore") });

                currentAudit.IsDeleted = true;
                currentAudit.MarkAsModified();
                //currentAudit.IsMarkedAsModified = true;
                await _prepareService.SaveAudit(currentAudit);
                return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// Get the Teams
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetTeams()
        {
            var (Users, Roles, Languages, Teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var data = Teams.Select(u => new { u.Name, u.Id }).OrderBy(u => u.Name).ToList();
            data.Insert(0, new { Name = LocalizedStrings.GetString("AllTeams"), Id = 0 });
            var jsonSerializer = new JavaScriptSerializer();
            return Json(jsonSerializer.Serialize(data));
        }

        /// <summary>
        /// Get the Operators
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetOperators()
        {
            var (Users, Roles, Languages, Teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var data = Users.Where(u => u.Roles.Where(r => r.RoleCode == KnownRoles.Operator).Any()).Select(u => new { Name = u.FullName, u.UserId }).OrderBy(u => u.Name).ToList();
            data.Insert(0, new { Name = LocalizedStrings.GetString("AllOperators"), UserId = 0 });
            var jsonSerializer = new JavaScriptSerializer();
            return Json(jsonSerializer.Serialize(data));
        }

        /// <summary>
        /// Get the Publications
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetPublications()
        {
            var inspectionPublications = await _prepareService.GetLastPublicationsForFilter((int)PublishModeEnum.Inspection);
            var data = inspectionPublications.Select(p => new { Label = $"{p.Process.Label} (v{p.Version})", PublicationId = p.PublicationId.ToString() })
                .OrderBy(p => p.Label)
                .ToList();
            data.Insert(0, new { Label = LocalizedStrings.GetString("All_Inspections_Label"), PublicationId = "0" });
            var jsonSerializer = new JavaScriptSerializer();
            return Json(jsonSerializer.Serialize(data));
        }

        public async Task<ActionResult> GetSurveys()
        {
            var surveys = await _prepareService.GetSurveys();
            var data = surveys.Where(s => s.SurveyItems.Any()).Select(s => new { Name = s.Name, Id = s.Id }).OrderBy(u => u.Name).ToList();
            //data.Insert(0, new { Name = "Sélectionnez un questionnaire", Id = 0 });
            var jsonSerializer = new JavaScriptSerializer();
            return Json(jsonSerializer.Serialize(data));
        }

        public class RangeLocation
        {
            public string ShapeName { get; set; }
            public int Row { get; set; }
            public int Column { get; set; }
        }
    }
}