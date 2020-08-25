using Kprocess.KL2.FileTransfer;
using KProcess.KL2.APIClient;
using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.KL2.WebAdmin.Mapper;
using KProcess.KL2.WebAdmin.Models.Referentials;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using Syncfusion.EJ.Export;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace KProcess.KL2.WebAdmin.Controllers
{
    [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Documentalist)]
    [SettingUserContextFilter]
    public class ReferentialsController : LocalizedController
    {
        readonly IApplicationUsersService _applicationUsersService;
        readonly IPrepareService _prepareService;
        readonly IReferentialsService _referentialsService;

        public ReferentialsController(
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

        // GET: Referentials
        public async Task<ActionResult> Index(int? refId, bool partial = false)
        {
            if (refId == null)
            {
                return RedirectToAction("Index", new { refId = 1 , partial = partial});
            }
            var serviceReferentials = await _referentialsService.GetApplicationReferentials();
            var referentialsOC = serviceReferentials.OrderBy(r => r.ReferentialId).ToObservableCollection();
            referentialsOC.Move(referentialsOC.IndexOf(referentialsOC.FirstOrDefault(r => r.ReferentialId == ProcessReferentialIdentifier.Skill)), referentialsOC.IndexOf(referentialsOC.FirstOrDefault(r => r.ReferentialId == ProcessReferentialIdentifier.Ref1)));
            var referentials = referentialsOC.ToList();
            var model = ReferentialsMapper.ToReferentialsViewModel(referentials);
            
            var identifier = (ProcessReferentialIdentifier)refId;
            model.Refs = await GetData(refId.Value);
            model.RefIdentifier = identifier;
            model.RefLabel = model.Referentials.FirstOrDefault(r => (int)r.refId == refId)?.Label ?? string.Empty;
            if (partial)
                return PartialView(model);
            return View(model);
        }

        public async Task<ActionResult> Resources()
        {
            var serviceReferentials = await _referentialsService.GetApplicationReferentials();
            var referentialsOC = serviceReferentials.OrderBy(r => r.ReferentialId).ToObservableCollection();
            referentialsOC.Move(referentialsOC.IndexOf(referentialsOC.FirstOrDefault(r => r.ReferentialId == ProcessReferentialIdentifier.Skill)), referentialsOC.IndexOf(referentialsOC.FirstOrDefault(r => r.ReferentialId == ProcessReferentialIdentifier.Ref1)));
            var referentials = referentialsOC.ToList();
            var model = ReferentialsMapper.ToReferentialsViewModel(referentials);
            return PartialView(model);
        }

        public async Task<ActionResult> Referentials(int? refId)
        {
            var serviceReferentials = await _referentialsService.GetApplicationReferentials();
            var referentialsOC = serviceReferentials.OrderBy(r => r.ReferentialId).ToObservableCollection();
            referentialsOC.Move(referentialsOC.IndexOf(referentialsOC.FirstOrDefault(r => r.ReferentialId == ProcessReferentialIdentifier.Skill)), referentialsOC.IndexOf(referentialsOC.FirstOrDefault(r => r.ReferentialId == ProcessReferentialIdentifier.Ref1)));
            var referentials = referentialsOC.ToList();
            var model = ReferentialsMapper.ToReferentialsViewModel(referentials);

            var identifier = (ProcessReferentialIdentifier)refId;
            model.Refs = await GetData(refId.Value);
            model.RefIdentifier = identifier;
            model.RefLabel = model.Referentials.FirstOrDefault(r => (int)r.refId == refId)?.Label ?? string.Empty;
            return PartialView(model);
        }

        public async Task<List<RefResourceViewModel>> GetData(int refId)
        {
            var identifier = (ProcessReferentialIdentifier)refId;
            List<RefResourceViewModel> refs = new List<RefResourceViewModel>();
            List<ProcedureViewModel> projects = new List<ProcedureViewModel>();
            List<ActionTypeViewModel> actionTypes = new List<ActionTypeViewModel>();
            List<ActionValueViewModel> actionValues = new List<ActionValueViewModel>();
            switch (identifier)
            {
                case ProcessReferentialIdentifier.Operator:
                    var opeResult = await _referentialsService.LoadOperators();
                    (refs, projects) = ReferentialsMapper.ToRefResourceViewModel(refId, operators: opeResult.Operators.ToList(), procedures: opeResult.Processes.ToList());
                    break;
                case ProcessReferentialIdentifier.Equipment:
                    var equResult = await _referentialsService.LoadEquipments();
                    (refs, projects) = ReferentialsMapper.ToRefResourceViewModel(refId, equipments: equResult.Equipments.ToList(), procedures: equResult.Processes.ToList());
                    break;
                case ProcessReferentialIdentifier.Category:
                    var catResult = await _referentialsService.LoadCategories();
                    actionTypes.Add(new ActionTypeViewModel { Code = "-1", Label = LocalizedStrings.GetString("View_AppActionCategories_ActionType_None") });
                    actionTypes.AddRange(catResult.ActionTypes.Select(
                        t => new ActionTypeViewModel { Code = t.ActionTypeCode, Label = t.LongLabel }
                        ).ToList());
                    actionValues = catResult.ActionValues.Select(
                        v => new ActionValueViewModel { Code = v.ActionValueCode, Label = v.ShortLabel }
                        ).ToList();
                    (refs, projects) = ReferentialsMapper.ToRefResourceViewModel(refId, categories: catResult.Categories.ToList(), procedures: catResult.Processes.ToList(), actionTypes: actionTypes, actionValues: actionValues);
                    break;
                case ProcessReferentialIdentifier.Skill:
                    var skillResult = await _referentialsService.LoadSkills();
                    (refs, projects) = ReferentialsMapper.ToRefResourceViewModel(refId, skills: skillResult.ToList());
                    break;
                case ProcessReferentialIdentifier.Ref1:
                case ProcessReferentialIdentifier.Ref2:
                case ProcessReferentialIdentifier.Ref3:
                case ProcessReferentialIdentifier.Ref4:
                case ProcessReferentialIdentifier.Ref5:
                case ProcessReferentialIdentifier.Ref6:
                case ProcessReferentialIdentifier.Ref7:
                    var (Referentials, Processes) = await _referentialsService.GetReferentials(identifier);
                    (refs, projects) = ReferentialsMapper.ToRefResourceViewModel(refId, referentials: Referentials.ToList(), procedures: Processes.ToList());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return refs;
        }

        public async Task<IEnumerable<IActionReferential>> GetDataFromService(ProcessReferentialIdentifier identifier)
        {
            IEnumerable<IActionReferential> refs = null;
            switch (identifier)
            {
                case ProcessReferentialIdentifier.Operator:
                    var (Operators, _) = await _referentialsService.LoadOperators();
                    refs = Operators;
                    break;
                case ProcessReferentialIdentifier.Equipment:
                    var (Equipments, _) = await _referentialsService.LoadEquipments();
                    refs = Equipments;
                    break;
                case ProcessReferentialIdentifier.Category:
                    var (Categories, _, _, _) = await _referentialsService.LoadCategories();
                    refs = Categories;
                    break;
                case ProcessReferentialIdentifier.Skill:
                    await _referentialsService.LoadSkills();
                    break;
                case ProcessReferentialIdentifier.Ref1:
                case ProcessReferentialIdentifier.Ref2:
                case ProcessReferentialIdentifier.Ref3:
                case ProcessReferentialIdentifier.Ref4:
                case ProcessReferentialIdentifier.Ref5:
                case ProcessReferentialIdentifier.Ref6:
                case ProcessReferentialIdentifier.Ref7:
                    var (Referentials, _) = await _referentialsService.GetReferentials(identifier);
                    refs = Referentials;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return refs;
        }

        public async Task ExportToExcel(string GridModel, int RefIdentifier)
        {
            ExcelExport exp = new ExcelExport();
            GridProperties obj = (GridProperties)Syncfusion.JavaScript.Utils.DeserializeToModel(typeof(GridProperties), GridModel);
            //Clear if there are any filter columns
            //syncfusion bug in exporting while in filter mode
            obj.FilterSettings.FilteredColumns.Clear();

            //change grouping to ProcessLabel
            obj.GroupSettings.GroupedColumns.Clear();
            obj.GroupSettings.GroupedColumns.Add("ProcessLabel");

            var serviceReferentials = await _referentialsService.GetApplicationReferentials();
            var identifier = (ProcessReferentialIdentifier)RefIdentifier;
            var identifierLabel = serviceReferentials.FirstOrDefault(r => (int)r.ReferentialId == RefIdentifier).Label;
            var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");
            var model = await GetData(RefIdentifier);

            obj.ServerExcelQueryCellInfo = QueryCellInfo;
            exp.Export(obj, model, "Référentiels " + identifierLabel + " " + currentDate + ".xlsx", ExcelVersion.Excel2013, false, false, "flat-saffron");
        }

        private void QueryCellInfo(object obj)
        {
            IRange range = (IRange)obj;
            if (range.Column == 2 && range.Value != null)
            {
                range.CellStyle.Color = ColorTranslator.FromHtml(range.Value);
                range.Value = "";
            }
        }

        public async Task<ActionResult> GetReferential(int Id, int refId, bool linkToProcess = false)
        {
            var identifier = (ProcessReferentialIdentifier)refId;
            List<RefResourceViewModel> refs = new List<RefResourceViewModel>();
            List<ProcedureViewModel> projects = new List<ProcedureViewModel>();
            List<ActionTypeViewModel> actionTypes = new List<ActionTypeViewModel>();
            List<ActionValueViewModel> actionValues = new List<ActionValueViewModel>();

            switch (identifier)
            {
                case ProcessReferentialIdentifier.Operator:
                    var opeResult = await _referentialsService.LoadOperators();
                    (refs, projects) = ReferentialsMapper.ToRefResourceViewModel(refId, operators: opeResult.Operators.ToList(), procedures: opeResult.Processes.ToList());
                    break;
                case ProcessReferentialIdentifier.Equipment:
                    var equResult = await _referentialsService.LoadEquipments();
                    (refs, projects) = ReferentialsMapper.ToRefResourceViewModel(refId, equipments: equResult.Equipments.ToList(), procedures: equResult.Processes.ToList());
                    break;
                case ProcessReferentialIdentifier.Category:
                    var catResult = await _referentialsService.LoadCategories();
                    actionTypes.Add(new ActionTypeViewModel { Code = "-1", Label = LocalizedStrings.GetString("View_AppActionCategories_ActionType_None") });
                    actionTypes.AddRange(catResult.ActionTypes.Select(
                        t => new ActionTypeViewModel { Code = t.ActionTypeCode, Label = t.LongLabel }
                        ).ToList());
                    actionValues = catResult.ActionValues.Select(
                        v => new ActionValueViewModel { Code = v.ActionValueCode, Label = v.ShortLabel }
                        ).ToList();
                    ViewBag.ActionType = actionTypes;
                    ViewBag.ActionValue = actionValues;
                    (refs, projects) = ReferentialsMapper.ToRefResourceViewModel(refId, categories: catResult.Categories.ToList(), procedures: catResult.Processes.ToList(), actionTypes: actionTypes, actionValues: actionValues);
                    break;
                case ProcessReferentialIdentifier.Skill:
                    var skillResult = await _referentialsService.LoadSkills();
                    (refs, projects) = ReferentialsMapper.ToRefResourceViewModel(refId, skills: skillResult.ToList());
                    break;
                case ProcessReferentialIdentifier.Ref1:
                case ProcessReferentialIdentifier.Ref2:
                case ProcessReferentialIdentifier.Ref3:
                case ProcessReferentialIdentifier.Ref4:
                case ProcessReferentialIdentifier.Ref5:
                case ProcessReferentialIdentifier.Ref6:
                case ProcessReferentialIdentifier.Ref7:
                    var (Referentials, Processes) = await _referentialsService.GetReferentials(identifier);
                    (refs, projects) = ReferentialsMapper.ToRefResourceViewModel(refId, referentials: Referentials.ToList(), procedures: Processes.ToList());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var refResourceViewModel = Id != 0 ? refs.FirstOrDefault(r => r.itemId == Id) : new RefResourceViewModel();

            refResourceViewModel.RefIdentifier = identifier;

            if (identifier == ProcessReferentialIdentifier.Category)
            {
                ViewBag.TypeIndex = actionTypes.FindIndex(t => t.Code == refResourceViewModel.ActionTypeCode);
                ViewBag.ValueIndex = actionValues.FindIndex(v => v.Code == refResourceViewModel.ActionValueCode);
            }

            if (linkToProcess == true)
                refResourceViewModel.ProcedureViewModels = projects;
            ViewBag.LinkToProcess = linkToProcess;
            return PartialView(refResourceViewModel);
        }

        public async Task<ActionResult> InsertReferential(CRUDModel<RefResourceViewModel> referential)
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
                if (referential.Value.IsProcessLink == true && referential.Value.ProcessId == null)
                {
                    throw new Exception(LocalizedStrings.GetString("AskSelectProcess"));
                }
                var identifier = referential.Value.RefIdentifier;
                referential.Value.intRefIdentifier = (int)identifier;
                CloudFile cloudFile = null;
                if (referential.Value.Hash != null)
                    cloudFile = await _referentialsService.GetCloudFile(referential.Value.Hash);

                async Task<JsonResult> AddRef<T>() where T : IActionReferential, new()
                {
                    var addedReferential = new T
                    {
                        Label = referential.Value.Label,
                        Description = referential.Value.Description,
                        Color = referential.Value.Color != null ? $"#FF{referential.Value.Color.Remove(referential.Value.Color.Length - 2).Substring(1).ToUpper()}".ToUpper() : null,
                        Hash = cloudFile?.Hash
                    };
                    if (addedReferential is IActionReferentialProcess addedReferentialProcess)
                        addedReferentialProcess.ProcessId = referential.Value.ProcessId;
                    if (addedReferential is IResource addedResource)
                        addedResource.PaceRating = referential.Value.PaceRating;
                    if (addedReferential is IActionCategory addedActionCategory)
                    {
                        var (Categories, ActionValues, ActionTypes, Processes) = await _referentialsService.LoadCategories();
                        addedActionCategory.ActionTypeCode = ActionTypes.SingleOrDefault(_ => _.ActionTypeCode == referential.Value.ActionTypeCode)?.ActionTypeCode;
                        addedActionCategory.ActionValueCode = referential.Value.ActionValueCode;
                    }

                    if (cloudFile?.IsMarkedAsAdded == true)
                        addedReferential.CloudFile = cloudFile;
                    var addReferentials = new List<T>(new[] { addedReferential });
                    if (typeof(T) == typeof(Ksmed.Models.Operator))
                    {
                        var resource = addReferentials.First() as Resource;
                        resource.PaceRating = 1;
                        await _referentialsService.SaveResources(addReferentials.Cast<Resource>());
                    }
                    else if (typeof(T) == typeof(Ksmed.Models.Equipment))
                    {
                        var resource = addReferentials.First() as Resource;
                        resource.PaceRating = 1;
                        await _referentialsService.SaveResources(addReferentials.Cast<Resource>());
                    }
                    else if (typeof(T) == typeof(ActionCategory))
                        await _referentialsService.SaveCategories(addReferentials.Cast<ActionCategory>());
                    else if (typeof(T) == typeof(Ref1))
                        await _referentialsService.SaveReferentials(addReferentials.Cast<Ref1>());
                    else if (typeof(T) == typeof(Ref2))
                        await _referentialsService.SaveReferentials(addReferentials.Cast<Ref2>());
                    else if (typeof(T) == typeof(Ref3))
                        await _referentialsService.SaveReferentials(addReferentials.Cast<Ref3>());
                    else if (typeof(T) == typeof(Ref4))
                        await _referentialsService.SaveReferentials(addReferentials.Cast<Ref4>());
                    else if (typeof(T) == typeof(Ref5))
                        await _referentialsService.SaveReferentials(addReferentials.Cast<Ref5>());
                    else if (typeof(T) == typeof(Ref6))
                        await _referentialsService.SaveReferentials(addReferentials.Cast<Ref6>());
                    else if (typeof(T) == typeof(Ref7))
                        await _referentialsService.SaveReferentials(addReferentials.Cast<Ref7>());
                    else if (typeof(T) == typeof(Skill))
                        await _referentialsService.SaveSkills(addReferentials.Cast<Skill>());

                    referential.Value.itemId = addReferentials.FirstOrDefault().Id;
                    referential.Key = referential.Value.itemId;
                    referential.KeyColumn = "Id";

                    if (typeof(T) != typeof(Skill))
                    {
                        //add process label Standard if no linked process
                        referential.Value.ProcessLabel = referential.Value.ProcessId.HasValue ? referential.Value.ProcessLabel : "Standard";
                        referential.Value.ProcessLabelSort = referential.Value.ProcessId.HasValue ? referential.Value.ProcessLabel : string.Empty;
                    }

                    var refs = await GetData((int)identifier);
                    return Json(refs.FirstOrDefault(r => r.itemId == referential.Value.itemId), JsonRequestBehavior.AllowGet);
                }

                switch (identifier)
                {
                    case ProcessReferentialIdentifier.Operator:
                        return await AddRef<Ksmed.Models.Operator>();
                    case ProcessReferentialIdentifier.Equipment:
                        return await AddRef<Ksmed.Models.Equipment>();
                    case ProcessReferentialIdentifier.Category:
                        if (referential.Value.ActionTypeCode == "-1")
                            referential.Value.ActionTypeCode = null;
                        //validation rules
                        if (referential.Value.ActionValueCode == null)
                            throw new Exception("Veuillez sélectionner un valorisation");
                        return await AddRef<ActionCategory>();
                    case ProcessReferentialIdentifier.Skill:
                        return await AddRef<Skill>();
                    case ProcessReferentialIdentifier.Ref1:
                        return await AddRef<Ref1>();
                    case ProcessReferentialIdentifier.Ref2:
                        return await AddRef<Ref2>();
                    case ProcessReferentialIdentifier.Ref3:
                        return await AddRef<Ref3>();
                    case ProcessReferentialIdentifier.Ref4:
                        return await AddRef<Ref4>();
                    case ProcessReferentialIdentifier.Ref5:
                        return await AddRef<Ref5>();
                    case ProcessReferentialIdentifier.Ref6:
                        return await AddRef<Ref6>();
                    case ProcessReferentialIdentifier.Ref7:
                        return await AddRef<Ref7>();
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
            catch (Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public async Task<ActionResult> UpdateReferential(CRUDModel<RefResourceViewModel> referential)
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
                var identifier = referential.Value.RefIdentifier;
                int intRefIdentifier = (int)identifier, itemId = referential.Value.itemId;
                bool clearFile = referential.Value.Uri == null;
                referential.Value.intRefIdentifier = (int)identifier;
                CloudFile cloudFile = null;
                if (referential.Value.Hash != null)
                    cloudFile = await _referentialsService.GetCloudFile(referential.Value.Hash);

                async Task UpdateRef<T>(IActionReferential updatedReferential) where T : IActionReferential
                {
                    if (updatedReferential is IActionReferentialProcess updatedReferentialProcess)
                        updatedReferentialProcess.ProcessId = referential.Value.ProcessId;
                    if (updatedReferential is IResource updatedResource)
                        updatedResource.PaceRating = referential.Value.PaceRating;
                    if (updatedReferential is IActionCategory updatedActionCategory)
                    {
                        var (Categories, ActionValues, ActionTypes, Processes) = await _referentialsService.LoadCategories();
                        updatedActionCategory.ActionTypeCode = ActionTypes.SingleOrDefault(_ => _.ActionTypeCode == referential.Value.ActionTypeCode)?.ActionTypeCode;
                        updatedActionCategory.ActionValueCode = referential.Value.ActionValueCode;
                    }
                    updatedReferential.StartTracking();
                    updatedReferential.Label = referential.Value.Label;
                    updatedReferential.Description = referential.Value.Description;
                    updatedReferential.Color = referential.Value.Color != null ? $"#FF{referential.Value.Color.Remove(referential.Value.Color.Length - 2).Substring(1).ToUpper()}".ToUpper() : null;
                    if (clearFile || cloudFile?.IsMarkedAsAdded == true || cloudFile?.IsMarkedAsUnchanged == true)
                        updatedReferential.Hash = cloudFile?.Hash;
                    
                    if (cloudFile?.IsMarkedAsAdded == true)
                        updatedReferential.CloudFile = cloudFile;

                    var addReferentials = new List<IActionReferential>(new[] { updatedReferential });
                    if (typeof(T) == typeof(Resource))
                    {
                        var resource = addReferentials.First() as Resource;
                        resource.PaceRating = 1;
                        await _referentialsService.SaveResources(addReferentials.Cast<Resource>());
                    }
                    else if (typeof(T) == typeof(ActionCategory))
                        await _referentialsService.SaveCategories(addReferentials.Cast<ActionCategory>());
                    else if (typeof(T) == typeof(Ref1))
                        await _referentialsService.SaveReferentials(addReferentials.Cast<Ref1>());
                    else if (typeof(T) == typeof(Ref2))
                        await _referentialsService.SaveReferentials(addReferentials.Cast<Ref2>());
                    else if (typeof(T) == typeof(Ref3))
                        await _referentialsService.SaveReferentials(addReferentials.Cast<Ref3>());
                    else if (typeof(T) == typeof(Ref4))
                        await _referentialsService.SaveReferentials(addReferentials.Cast<Ref4>());
                    else if (typeof(T) == typeof(Ref5))
                        await _referentialsService.SaveReferentials(addReferentials.Cast<Ref5>());
                    else if (typeof(T) == typeof(Ref6))
                        await _referentialsService.SaveReferentials(addReferentials.Cast<Ref6>());
                    else if (typeof(T) == typeof(Ref7))
                        await _referentialsService.SaveReferentials(addReferentials.Cast<Ref7>());
                    else if (typeof(T) == typeof(Skill))
                        await _referentialsService.SaveSkills(addReferentials.Cast<Skill>());

                    if (typeof(T) != typeof(Skill))
                    {
                        //add process label Standard if no linked process
                        referential.Value.ProcessLabel = referential.Value.ProcessId.HasValue ? referential.Value.ProcessLabel : "Standard";
                        referential.Value.ProcessLabelSort = referential.Value.ProcessId.HasValue ? referential.Value.ProcessLabel : string.Empty;
                    }
                }

                switch (identifier)
                {
                    case ProcessReferentialIdentifier.Operator:
                        var (Operators, Processes) = await _referentialsService.LoadOperators();
                        var editOpe = Operators.FirstOrDefault(o => o.Id == referential.Value.itemId);
                        await UpdateRef<Resource>(editOpe);
                        break;
                    case ProcessReferentialIdentifier.Equipment:
                        var equResult = await _referentialsService.LoadEquipments();
                        var editEqu = equResult.Equipments.FirstOrDefault(o => o.Id == referential.Value.itemId);
                        await UpdateRef<Resource>(editEqu);
                        break;
                    case ProcessReferentialIdentifier.Category:
                        //validation rules
                        if (referential.Value.ActionTypeCode == "-1")
                            referential.Value.ActionTypeCode = null;
                        if (referential.Value.ActionValueCode == null)
                            throw new Exception("Veuillez sélectionner un valorisation");
                        var catResult = await _referentialsService.LoadCategories();
                        var editCat = catResult.Categories.FirstOrDefault(o => o.Id == referential.Value.itemId);
                        await UpdateRef<ActionCategory>(editCat);
                        break;
                    case ProcessReferentialIdentifier.Skill:
                        var skillResult = await _referentialsService.LoadSkills();
                        var editSkill = skillResult.FirstOrDefault(o => o.Id == referential.Value.itemId);
                        await UpdateRef<Skill>(editSkill);
                        break;
                    case ProcessReferentialIdentifier.Ref1:
                        var refResult1 = await _referentialsService.GetReferentials(identifier);
                        var editRef1 = refResult1.Referentials.OfType<Ref1>().FirstOrDefault(r => r.Id == referential.Value.itemId);
                        await UpdateRef<Ref1>(editRef1);
                        break;
                    case ProcessReferentialIdentifier.Ref2:
                        var refResult2 = await _referentialsService.GetReferentials(identifier);
                        var editRef2 = refResult2.Referentials.OfType<Ref2>().FirstOrDefault(r => r.Id == referential.Value.itemId);
                        await UpdateRef<Ref2>(editRef2);
                        break;
                    case ProcessReferentialIdentifier.Ref3:
                        var refResult3 = await _referentialsService.GetReferentials(identifier);
                        var editRef3 = refResult3.Referentials.OfType<Ref3>().FirstOrDefault(r => r.Id == referential.Value.itemId);
                        await UpdateRef<Ref3>(editRef3);
                        break;
                    case ProcessReferentialIdentifier.Ref4:
                        var refResult4 = await _referentialsService.GetReferentials(identifier);
                        var editRef4 = refResult4.Referentials.OfType<Ref4>().FirstOrDefault(r => r.Id == referential.Value.itemId);
                        await UpdateRef<Ref4>(editRef4);
                        break;
                    case ProcessReferentialIdentifier.Ref5:
                        var refResult5 = await _referentialsService.GetReferentials(identifier);
                        var editRef5 = refResult5.Referentials.OfType<Ref5>().FirstOrDefault(r => r.Id == referential.Value.itemId);
                        await UpdateRef<Ref5>(editRef5);
                        break;
                    case ProcessReferentialIdentifier.Ref6:
                        var refResult6 = await _referentialsService.GetReferentials(identifier);
                        var editRef6 = refResult6.Referentials.OfType<Ref6>().FirstOrDefault(r => r.Id == referential.Value.itemId);
                        await UpdateRef<Ref6>(editRef6);
                        break;
                    case ProcessReferentialIdentifier.Ref7:
                        var refResult7 = await _referentialsService.GetReferentials(identifier);
                        var editRef7 = refResult7.Referentials.OfType<Ref7>().FirstOrDefault(r => r.Id == referential.Value.itemId);
                        await UpdateRef<Ref7>(editRef7);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (cloudFile?.Extension != null || clearFile)
                {
                    //DeleteFile(intRefIdentifier, itemId, extension);
                }
                ModelState.Clear();
                var refs = await GetData((int)identifier);
                return Json(refs.FirstOrDefault(r => r.itemId == referential.Value.itemId), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
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

        //public async Task<ActionResult> DeleteReferential(int key)
        //{
        //    try
        //    {
        //        var identifier = Convert.ToInt16(Request.Headers.GetValues("refIdentifier")[0]);
        //        var finish = await DeleteOperation(key, identifier);
        //        var serviceReferentials = await _referentialsService.GetApplicationReferentials();
        //        var referentials = serviceReferentials.OrderBy(r => r.Label).ToList();
        //        var model = ReferentialsMapper.ToReferentialsViewModel(referentials);
        //        return Json(model, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
        //    }
        //}

        public async Task<ActionResult> CRUDReferential(CRUDModel<RefResourceViewModel> item, string action)
        {
            try
            {
                var identifier = (ProcessReferentialIdentifier)Convert.ToInt32(item.Params["refIdentifier"]);
                var ItemId = Convert.ToInt32(item.Params["ItemId"]);
                if (await _referentialsService.ReferentialUsed(identifier, ItemId))
                {
                    var message = LocalizedStrings.GetString("VM_AppActionsReferentialsViewModelBase_Message_CannotDeleteAttachedActions");
                    throw new Exception(message);
                }
                await DeleteOperation(ItemId, identifier);
                var serviceReferentials = await _referentialsService.GetApplicationReferentials();
                var referentials = serviceReferentials.OrderBy(r => r.Label).ToList();
                var model = ReferentialsMapper.ToReferentialsViewModel(referentials);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
            //return Json(item.Value, JsonRequestBehavior.AllowGet);
        }

        public async Task DeleteOperation(int key, ProcessReferentialIdentifier identifier)
        {
            async Task<string> DelRef<T>(IActionReferential deletedReferential) where T : IActionReferential
            {
                deletedReferential.MarkAsDeleted();
                var extensionResult = deletedReferential.CloudFile?.Extension;
                var delReferentials = new List<IActionReferential>(new[] { deletedReferential });
                if (typeof(T) == typeof(Resource))
                    await _referentialsService.SaveResources(delReferentials.Cast<Resource>());
                else if (typeof(T) == typeof(ActionCategory))
                    await _referentialsService.SaveCategories(delReferentials.Cast<ActionCategory>());
                else if (typeof(T) == typeof(Ref1))
                    await _referentialsService.SaveReferentials(delReferentials.Cast<Ref1>());
                else if (typeof(T) == typeof(Ref2))
                    await _referentialsService.SaveReferentials(delReferentials.Cast<Ref2>());
                else if (typeof(T) == typeof(Ref3))
                    await _referentialsService.SaveReferentials(delReferentials.Cast<Ref3>());
                else if (typeof(T) == typeof(Ref4))
                    await _referentialsService.SaveReferentials(delReferentials.Cast<Ref4>());
                else if (typeof(T) == typeof(Ref5))
                    await _referentialsService.SaveReferentials(delReferentials.Cast<Ref5>());
                else if (typeof(T) == typeof(Ref6))
                    await _referentialsService.SaveReferentials(delReferentials.Cast<Ref6>());
                else if (typeof(T) == typeof(Ref7))
                    await _referentialsService.SaveReferentials(delReferentials.Cast<Ref7>());
                else if (typeof(T) == typeof(Skill))
                    await _referentialsService.SaveSkills(delReferentials.Cast<Skill>());
                return extensionResult;
            }

            int itemId = key;
            string extension;
            switch (identifier)
            {
                case ProcessReferentialIdentifier.Operator:
                    var (Operators, Processes) = await _referentialsService.LoadOperators();
                    var editOpe = Operators.FirstOrDefault(o => o.Id == key);
                    extension = await DelRef<Resource>(editOpe);
                    break;

                case ProcessReferentialIdentifier.Equipment:
                    var equResult = await _referentialsService.LoadEquipments();
                    var editEqu = equResult.Equipments.FirstOrDefault(o => o.Id == key);
                    extension = await DelRef<Resource>(editEqu);
                    break;

                case ProcessReferentialIdentifier.Category:
                    var catResult = await _referentialsService.LoadCategories();
                    var editCat = catResult.Categories.FirstOrDefault(o => o.Id == key);
                    extension = await DelRef<ActionCategory>(editCat);
                    break;

                case ProcessReferentialIdentifier.Skill:
                    var skillResult = await _referentialsService.LoadSkills();
                    var editSkill = skillResult.FirstOrDefault(o => o.Id == key);
                    extension = await DelRef<Skill>(editSkill);
                    break;

                case ProcessReferentialIdentifier.Ref1:
                    var refResult1 = await _referentialsService.GetReferentials(identifier);
                    var editRef1 = refResult1.Referentials.OfType<Ref1>().FirstOrDefault(r => r.Id == key);
                    extension = await DelRef<Ref1>(editRef1);
                    break;

                case ProcessReferentialIdentifier.Ref2:
                    var refResult2 = await _referentialsService.GetReferentials(identifier);
                    var editRef2 = refResult2.Referentials.OfType<Ref2>().FirstOrDefault(r => r.Id == key);
                    extension = await DelRef<Ref2>(editRef2);
                    break;

                case ProcessReferentialIdentifier.Ref3:
                    var refResult3 = await _referentialsService.GetReferentials(identifier);
                    var editRef3 = refResult3.Referentials.OfType<Ref3>().FirstOrDefault(r => r.Id == key);
                    extension = await DelRef<Ref3>(editRef3);
                    break;

                case ProcessReferentialIdentifier.Ref4:
                    var refResult4 = await _referentialsService.GetReferentials(identifier);
                    var editRef4 = refResult4.Referentials.OfType<Ref4>().FirstOrDefault(r => r.Id == key);
                    extension = await DelRef<Ref4>(editRef4);
                    break;

                case ProcessReferentialIdentifier.Ref5:
                    var refResult5 = await _referentialsService.GetReferentials(identifier);
                    var editRef5 = refResult5.Referentials.OfType<Ref5>().FirstOrDefault(r => r.Id == key);
                    extension = await DelRef<Ref5>(editRef5);
                    break;

                case ProcessReferentialIdentifier.Ref6:
                    var refResult6 = await _referentialsService.GetReferentials(identifier);
                    var editRef6 = refResult6.Referentials.OfType<Ref6>().FirstOrDefault(r => r.Id == key);
                    extension = await DelRef<Ref6>(editRef6);
                    break;

                case ProcessReferentialIdentifier.Ref7:
                    var refResult7 = await _referentialsService.GetReferentials(identifier);
                    var editRef7 = refResult7.Referentials.OfType<Ref7>().FirstOrDefault(r => r.Id == key);
                    extension = await DelRef<Ref7>(editRef7);
                    break;
                default:
                    extension = null;
                    break;
            }
            if (extension != null)
            {
                //DeleteFile(intRefIdentifier, itemId, extension);
            }
        }

        public void DeleteFile(int intRefIdentifier, int itemId, string extension)
        {
            var filePath = ReferentialsMapper.GetServerFilePath(intRefIdentifier, itemId, extension);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        public string GetServerFilePath(int intRefIdentifier, int itemId, string extension)
        {
            string targetFolder = Server.MapPath("~/Files");
            string fileName = intRefIdentifier + "." + itemId + "." + extension;
            string filePath = Path.Combine(targetFolder, fileName);
            return filePath;
        }

        public JsonResult GetHashAndExtension()
        {
            try
            {
                var file = Request.Files["FileUpload"];
                byte[] data;
                string extension = Path.GetExtension(file.FileName);
                using (Stream inputStream = file.InputStream)
                {
                    if (!(inputStream is MemoryStream memoryStream))
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream, StreamExtensions.BufferSize);
                    }
                    data = memoryStream.ToArray();
                }
                var cloudFile = ConvertToCloudFile(data, extension);
                return Json(new { success = true, hash = cloudFile.Hash, extension = cloudFile.Extension }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public CloudFile ConvertToCloudFile(byte[] data, string extension)
        {
            var cloudFile = new CloudFile(data, extension);
            return cloudFile;
        }

        public async Task<JsonResult> ChangeRefLabel(int refId, string label)
        {
            try
            {
                var referential = (ProcessReferentialIdentifier)refId;
                await _referentialsService.UpdateReferentialLabel(referential, label);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> GetReferentialListForMerge(int Id, int refId)
        {
            try
            {
                var refs = await GetData(refId);
                var processId = refs.FirstOrDefault(r => r.itemId == Id).ProcessId;
                refs.RemoveWhere(r => r.itemId == Id);
                var filteredRefs = processId.HasValue ? refs.Where(r => r.ProcessId == processId).OrderBy(r => r.ProcessLabelSort).ThenBy(r => r.Label).ToList() : refs.OrderBy(r => r.ProcessLabelSort).ThenBy(r => r.Label).ToList();
                return Json(new { success = true, refs = filteredRefs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        
        public async Task<JsonResult> MergeReferentials(int masterId, List<int> mergeIds, int refIdentifier)
        {
            try
            {
                var refs = await GetDataFromService((ProcessReferentialIdentifier)refIdentifier);

                var master = refs.FirstOrDefault(r => r.Id == masterId);
                List<IActionReferential> slaves = new List<IActionReferential>();
                foreach (var id in mergeIds)
                {
                    slaves.Add(refs.FirstOrDefault(r => r.Id == id));
                }
                await _referentialsService.MergeReferentials(master, slaves.ToArray());
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> GetReferentialsValues(int id)
        {
            try
            {
                var refs = await GetDataFromService((ProcessReferentialIdentifier)id);
                return Json( refs.ToList().Select(u => new { u.Label }), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public FileResult Download(string filePath, string name)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string fileName = name;
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
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
    }
}