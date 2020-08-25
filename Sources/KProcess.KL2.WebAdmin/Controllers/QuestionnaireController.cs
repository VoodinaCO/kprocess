using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.KL2.WebAdmin.Mapper;
using KProcess.KL2.WebAdmin.Models.Survey;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using Syncfusion.EJ.Export;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace KProcess.KL2.WebAdmin.Controllers
{
    [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor)]
    [SettingUserContextFilter]
    public class QuestionnaireController : LocalizedController
    {
        private IApplicationUsersService _applicationUsersService;
        private IPrepareService _prepareService;
        private IReferentialsService _referentialsService;

        public int count;
        public string grid;

        public QuestionnaireController(
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
        public async Task<ActionResult> Index(bool partial = false)
        {
            var surveys = await _prepareService.GetSurveys();
            var model = QuestionnaireMapper.ToSurveyManageViewModel(surveys);
            if (partial)
                return PartialView(model);
            return View(model);
        }
        public async Task<ActionResult> Detail(int id, bool partial = false)
        {
            var surveys = await _prepareService.GetSurveys(id);
            var survey = surveys.FirstOrDefault();
            var model = QuestionnaireMapper.ToSurveyViewModel(survey);
            if (partial)
                return PartialView(model);
            return View(model);
        }

        public async Task<ActionResult> InsertSurvey(CRUDModel<SurveyViewModel> survey)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);
                }

                var newSurvey = new Survey
                {
                    Name = survey.Value.Name
                };

                Survey[] addSurvey = new[] { newSurvey };
                await _prepareService.SaveSurveys(addSurvey);

                var newSurveys = await _prepareService.GetSurveys();
                var createdSurvey = newSurveys.Last(u => u.Name == survey.Value.Name);
                survey.Value.SurveyId = createdSurvey.Id;
                survey.Value.Name = survey.Value.Name;
                return Json(survey.Value, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                if (ex.Message == "Common_CannotUseSameName")
                {
                    message = LocalizedStrings.GetString("Common_CannotUseSameName");
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);
            }
        }

        public async Task<ActionResult> InsertSurveyItem(CRUDModel<SurveyItemViewModel> surveyItem)
        {
            try
            {
                int surveyId = int.Parse(Request.Headers.GetValues("SurveyId")[0]);
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);
                }

                var surveys = await _prepareService.GetSurveys(surveyId);
                var survey = surveys.FirstOrDefault();
                var newSurveyItem = new SurveyItem
                {
                    SurveyId = surveyId,
                    Number = surveyItem.Value.Number,
                    Query = surveyItem.Value.Query
                };
                survey.SurveyItems.Add(newSurveyItem);
                Survey[] addSurvey = new[] { survey };
                await _prepareService.SaveSurveys(addSurvey);

                return Json(surveyItem.Value, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public async Task<ActionResult> UpdateSurvey(CRUDModel<SurveyViewModel> survey)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);
                }

                var surveysEdit = await _prepareService.GetSurveys(survey.Value.SurveyId);
                var surveyEdit = surveysEdit.FirstOrDefault();
                surveyEdit.StartTracking();

                surveyEdit.Name = survey.Value.Name;
                surveyEdit.MarkAsModified();

                Survey[] editSurvey = new[] { surveyEdit };
                await _prepareService.SaveSurveys(editSurvey);
                survey.Value.SurveyId = editSurvey.FirstOrDefault().Id;
                return Json(survey.Value, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                 .SelectMany(v => v.Errors)
                 .Select(e => e.ErrorMessage));
                if (ex.Message == "Common_CannotUseSameName")
                {
                    message = LocalizedStrings.GetString("Common_CannotUseSameName");
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);
            }
        }

        public async Task<ActionResult> UpdateSurveyItem(CRUDModel<SurveyItemViewModel> surveyItem)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);
                }

                var surveys = await _prepareService.GetSurveys(surveyItem.Value.SurveyId);
                var survey = surveys.FirstOrDefault();
                survey.StartTracking();

                survey.SurveyItems.Where(i => i.Number == surveyItem.Value.Number).Select(i => { i.Query = surveyItem.Value.Query; return i; }).ToList();
                survey.SurveyItems.FirstOrDefault(i => i.Number == surveyItem.Value.Number).MarkAsModified();
                survey.MarkAsModified();


                Survey[] editSurvey = new[] { survey };
                await _prepareService.SaveSurveys(editSurvey);

                return Json(surveyItem.Value, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public async Task<ActionResult> DeleteSurvey(int key)
        {
            try
            {
                var survey = await _prepareService.GetSurveys(key);
                var surveyDelete = survey.FirstOrDefault();
                if (surveyDelete != null)
                {
                    surveyDelete.StartTracking();
                    surveyDelete.MarkAsDeleted();
                    Survey[] deleteSurvey = new[] { surveyDelete };
                    await _prepareService.SaveSurveys(deleteSurvey);
                }
                return Json(surveyDelete, JsonRequestBehavior.AllowGet);
            }
            catch(UpdateException)
            {
                var message = LocalizedStrings.GetString("UnableUsedAuditDelete")/*"Impossible de supprimer un audit utilisé"*/;
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        
        public async Task<ActionResult> DeleteSurveyItem(int key)
        {
            try
            {
                int surveyId = int.Parse(Request.Headers.GetValues("SurveyId")[0]);
                var survey = await _prepareService.GetSurveys(surveyId);
                var surveyDelete = survey.FirstOrDefault();
                if (surveyDelete != null)
                {
                    surveyDelete.StartTracking();
                    if (surveyDelete.SurveyItems.Any(i => i.Number == key))
                        surveyDelete.SurveyItems.FirstOrDefault(i => i.Number == key).MarkAsDeleted();
                    Survey[] deleteSurveyItem = new[] { surveyDelete };
                    await _prepareService.SaveSurveys(deleteSurveyItem);

                    //Reorder
                    List<SurveyItem> surveyItemsTemp = new List<SurveyItem>();
                    var countReorder = 1;
                    foreach (var item in surveyDelete.SurveyItems.ToList())
                    {
                        var temp = new SurveyItem {
                            SurveyId = item.SurveyId,
                            Number = countReorder,
                            Query = item.Query
                        };
                        surveyItemsTemp.Add(temp);
                        countReorder++;
                    }

                    //Delete all survey items
                    surveyDelete.StartTracking();
                    foreach (var tempItem in surveyDelete.SurveyItems.ToList())
                        surveyDelete.SurveyItems.ToList().FirstOrDefault(i => i.Number == tempItem.Number).MarkAsDeleted();

                    //Reenter from temporary
                    surveyDelete.StartTracking();
                    foreach (var item in surveyItemsTemp)
                    {
                        surveyDelete.SurveyItems.Add(item);
                    }
                    Survey[] editSurvey = new[] { surveyDelete };
                    await _prepareService.SaveSurveys(editSurvey);

                    return Json(surveyDelete.SurveyItems.Any(i => i.Number == key), JsonRequestBehavior.AllowGet);
                }
                return Json(survey, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public async Task ExportToExcel(string GridModel, string gridId, int id, string survey)
        {
            ExcelExport exp = new ExcelExport();
            GridProperties obj = (GridProperties)Syncfusion.JavaScript.Utils.DeserializeToModel(typeof(GridProperties), GridModel);
            //Clear if there are any filter columns
            //syncfusion bug in exporting while in filter mode
            obj.FilterSettings.FilteredColumns.Clear();
            grid = gridId;
            count = 0;

            if (gridId == "Surveys")
            {
                var surveys = await _prepareService.GetSurveys();
                var model = QuestionnaireMapper.ToSurveyManageViewModel(surveys);
                var dataSource = model.Surveys.ToList();
                var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");
                exp.Export(obj, dataSource, LocalizedStrings.GetString("Questionnaire") + " "/*"Questionnaire "*/ + currentDate + ".xlsx", ExcelVersion.Excel2013, false, false, "flat-saffron");
            }
            if (gridId == "SurveyItems")
            {
                var surveys = await _prepareService.GetSurveys(id);
                var data = surveys.FirstOrDefault();
                var model = QuestionnaireMapper.ToSurveyViewModel(data);
                var dataSource = model.SurveyItems.ToList();
                var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");
                exp.Export(obj, dataSource, LocalizedStrings.GetString("Questionnaire") + " "/*"Questionnaire "*/ + survey + " " + currentDate + ".xlsx", ExcelVersion.Excel2013, false, false, "flat-saffron");
            }
        }
        [HttpPost]
        public async Task<ActionResult> RowDropHandler(List<SurveyItemViewModel> changed, int dropIndex)
        {
            try
            {
                JavaScriptSerializer ser = new JavaScriptSerializer();
                var count = 0;
                if (changed != null)
                {
                    var surveys = await _prepareService.GetSurveys(changed.FirstOrDefault().SurveyId);
                    var survey = surveys.FirstOrDefault();
                    var surveyTemp = survey;
                    List<SurveyItem> surveyItemsTemp = survey.SurveyItems.ToList();


                    surveyTemp.StartTracking();
                    //Firstly delete all surveyItem to avoid bug as Number also a primary key
                    foreach (var tempItem in surveyTemp.SurveyItems.ToList())
                        surveyTemp.SurveyItems.ToList().FirstOrDefault(i => i.Number == tempItem.Number).MarkAsDeleted();
                    Survey[] deleteSurveyItem = new[] { surveyTemp };
                    await _prepareService.SaveSurveys(deleteSurveyItem);



                    foreach (var item in changed)
                    {
                        SurveyItem changeItem = new SurveyItem
                        {
                            SurveyId = item.SurveyId,
                            Number = dropIndex,
                            Query = item.Query
                        };

                        surveyItemsTemp.RemoveWhere(i => i.Number == item.Number);
                        surveyItemsTemp.Insert(dropIndex + count, changeItem);
                    }

                    survey.StartTracking();
                    var countReorder = 1;
                    foreach (var item in surveyItemsTemp)
                    {
                        survey.SurveyItems.Add(new SurveyItem
                        {
                            Number = countReorder,
                            Query = item.Query,
                            SurveyId = item.SurveyId
                        });
                        countReorder++;
                    }
                    Survey[] editSurvey = new[] { survey };
                    await _prepareService.SaveSurveys(editSurvey);
                }
                else throw new ArgumentException(LocalizedStrings.GetString("AskSelectLineChangePosition")/*"Veuillez sélectionner une ligne avant de changer de position"*/);

                return Json(changed, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }

        }
    }
}