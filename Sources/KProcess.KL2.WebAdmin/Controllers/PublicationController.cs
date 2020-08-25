using KProcess.KL2.JWT;
using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.KL2.WebAdmin.Mapper;
using KProcess.KL2.WebAdmin.Models.Action;
using KProcess.KL2.WebAdmin.Models.Procedure;
using KProcess.KL2.WebAdmin.Models.Publications;
using KProcess.KL2.WebAdmin.Models.Users;
using KProcess.KL2.WebAdmin.Utils;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Interfaces;
using KProcess.Ksmed.Security;
using Syncfusion.EJ.Export;
using Syncfusion.JavaScript.Models;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace KProcess.KL2.WebAdmin.Controllers
{
    [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator, KnownRoles.Documentalist)]
    [SettingUserContextFilter]
    public class PublicationController : LocalizedController
    {
        readonly ITraceManager _traceManager;
        readonly IPrepareService _prepareService;
        readonly IApplicationUsersService _applicationUsersService;
        readonly IReferentialsService _referentialsService;

        //For populating Publications column in Excel Export
        List<UserReadPublicationViewModel> PublicationReadData;
        List<OperatorViewModel> UptodateOperatorsData;
        List<UserCompetencyViewModel> UserCompetencyData;
        List<GenericActionViewModel> PublishedActionsData;

        public int count;
        public int columnCounter;
        public string grid;

        public const int height = 250;
        public const int width = 350;
        public const double heightPoint = height * 72 / 96;
        public const double widthPoint = width / 7;

        public PublicationController(ITraceManager traceManager,
            IPrepareService prepareService,
            IApplicationUsersService applicationUsersService,
            IReferentialsService referentialsService,
            ILocalizationManager localizationManager
            )
            : base(localizationManager)
        {
            _traceManager = traceManager;
            _prepareService = prepareService;
            _applicationUsersService = applicationUsersService;
            _referentialsService = referentialsService;
        }

        // GET: Read publication
        public async Task<ActionResult> Index(int? processId, int? PublishModeFilter, bool partial = false)
        {
            try
            {
                PublishModeEnum filter = PublishModeEnum.Formation;
                if (PublishModeFilter.HasValue)
                {
                    filter = (PublishModeEnum)PublishModeFilter.Value;
                }
                INode[] publicationsTree = await _prepareService.GetPublicationsTree(filter);
                var nodes = new List<ProcedureNodeViewModel> { new ProcedureNodeViewModel { Id = "-1", Name = LocalizedStrings.GetString("All_Publications_Label"), ParentId = null, IsExpanded = true, HasChild = publicationsTree != null && publicationsTree.Any(), Sprite = "fa fa-folder" } };

                foreach (var publicationTree in Flatten(publicationsTree.ToList()))
                {
                    //There could be same value of processId and projectDirId where treebox in view cannot define clearly define Id and its parentId

                    if (publicationTree is ProjectDir)
                    {
                        var projectDir = publicationTree as ProjectDir;
                        nodes.Add(new ProcedureNodeViewModel
                        {
                            //projectDirId are named as : ex. projectDirId = 1, Id = d1
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
                        nodes.Add(new ProcedureNodeViewModel
                        {
                            //projectDirId are named as : ex. ProcessId = 1, Id = p1
                            Id = "p" + procedure.ProcessId,
                            Name = procedure.Label,
                            ParentId = procedure.ProjectDirId != null ? "d" + procedure.ProjectDirId : "-1",
                            HasChild = false,
                            LinkAttribute = Url.Action("Index", new { PublishModeFilter = (int)filter, processId = procedure.ProcessId }),
                            LinkAttributeFunction = "javascript:LoadPartial('" + Url.Action("Index", new { PublishModeFilter = (int)filter, processId = procedure.ProcessId, partial = true }) + "', '#pageContainer')",
                            Sprite = filter == PublishModeEnum.Formation ? "fa fa-graduation-cap" : filter == PublishModeEnum.Inspection ? "fa fa-check" : "fa fa-file-o"
                        });
                    }
                }

                var model = new PublicationManageViewModel
                {
                    TreeNode = nodes,
                    Publication = processId.HasValue ? await GetPublication(processId.Value, filter) : null,
                    PublicationType = filter.ToString()
                };
                if (partial)
                    return PartialView(model);
                return View(model);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return null;
            }
        }

        IEnumerable<INode> Flatten(IEnumerable<INode> e)
        {
            return e.SelectMany(c => Flatten(c.Nodes)).Concat(new[] { e });
        }

        async Task<PublicationViewModel> GetPublication(int processId, PublishModeEnum publishModeFilter)
        {
            try
            {
                var publication = await _prepareService.GetLastPublicationFiltered(processId, (int)publishModeFilter);
                var localizations = publication.Localizations.ToDictionary(k => k.ResourceKey, v => v.Value);

                var model = new PublicationViewModel
                {
                    PublicationId = publication.PublicationId.ToString(),
                    ProcessId = publication.ProcessId,
                    Label = publication.Process.Label,
                    Actions = new List<GenericActionViewModel>(),
                    PublishModeEnum = (int)publishModeFilter,
                    Version = publication.Version,
                    IsMajor = publication.IsMajor,
                    ReferentialsUsed = new List<ActionValueViewModel>()
                };

                if ((publishModeFilter == PublishModeEnum.Formation && publication.Formation_Disposition != null)
                    || publishModeFilter == PublishModeEnum.Inspection && publication.Inspection_Disposition != null)
                {
                    var visibleColumns = PublicationSfDataGridXml.GetVisibleActionColumns(publication, publishModeFilter);
                    model.ActionsColumnCount = visibleColumns.Count();
                    model.ActionHeaders = ActionMapper.GetColumnHeader(visibleColumns, localizations);
                    model.ActionHeaderModels = ActionMapper.GetColumnHeaderWithValue(visibleColumns, localizations);
                    var refsUsed = new List<ActionValueViewModel>();

                    foreach (var action in publication.PublishedActions)
                    {
                        (List<RefsCollection> refs, List<CustomLabel> customLabel) = ActionMapper.BuildReferenceAndCustomLabel(action, localizations);
                        var values = new Dictionary<string, ActionColumnViewModel>();
                        foreach (var setting in visibleColumns)
                        {
                            values.Add(setting.MappingName,
                                ActionMapper.GetPublishedActionAttributes(
                                    action,
                                    ReflectionHelper.GetPropertyValue(action, setting.MappingName),
                                    setting.HeaderText,
                                    setting.MappingName,
                                    localizations,
                                    refs,
                                    customLabel));
                            var refCollection = ActionMapper.GetActionReferentials(refs, setting.MappingName, localizations);
                            if (refCollection.Count > 0)
                            {
                                refsUsed.AddRange(refCollection.Where(_ => refsUsed.All(r => r.ReferentialId != _.ReferentialId)));
                            }
                        }
                        model.Actions.Add(new GenericActionViewModel
                        {
                            ActionId = action.PublishedActionId,
                            ColumnValues = values,
                            PublishModeFilter = (int)publishModeFilter,
                            PublicationVersion = action.Publication.Version,
                            PublicationVersionIsMajor = action.Publication.IsMajor,
                            IsKeyTask = action.IsKeyTask,
                            IsGroup = WBSHelper.HasChildren(action, publication.PublishedActions)
                        });
                    }
                    model.ReferentialsUsed = refsUsed.OrderBy(r => r.MappingName).ToList();
                }
                return model;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<ActionResult> ReadInOrder(Guid? id, int? publishModeFilter)
        {
            var publication = await _prepareService.GetLightPublication(id.Value);
            if (publication == null)
                return HttpNotFound();

            //// TODO : Faire le calcul du chemin critique en incluant les tâches des sous-process
            //var tempCriticalPath = publication.PublishedActions.CriticalPath(p => p.Successors, p => p.BuildFinish - p.BuildStart);
            //var criticalPath = new List<PublishedAction>();
            //for (int i = tempCriticalPath.Count(); i > 0; i--)
            //    criticalPath.Add(tempCriticalPath.ElementAt(i - 1));

            //return RedirectToAction("Details", "Action", new { publishModeFilter = publishModeFilter.Value, computeReadNext = true, id = criticalPath.ElementAt(0).PublishedActionId });

            //Go through normal order path
            var startId = publication.PublishedActions.OrderBy(a => a.WBS.ToString()).First().PublishedActionId;
            return RedirectToAction("Details", "Action", new { PublishModeFilter = publishModeFilter.Value, computeReadNext = true, id = startId });
        }


        // GET: Read publication
        [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator)]
        public async Task<ActionResult> ReadPublication(int? userId, bool partial = false)
        {
            UserModel user = JwtTokenProvider.GetUserModel(Request.Cookies["token"].Value);
            if (!userId.HasValue && !user.Roles.Any(r => (new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer }.Contains(r))))
                return RedirectToAction("ReadPublication", new { userId = user.UserId, partial = partial });
            var (users, roles, languages, teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var lastPublications = await _prepareService.GetLastPublications((int)PublishModeEnum.Formation);
            var filteredUsers = new List<User>();
            if (userId.HasValue)
                filteredUsers = users.Where(u => u.UserId == userId.Value).ToList();
            else
                filteredUsers = (await LicenseMapper.FilterByActivatedUser(users, u => u.UserId)).ToList();
            var readPublicationViewModel = PublicationMapper.ToReadPublicationViewModel(lastPublications, filteredUsers);
            if (partial)
                return PartialView(readPublicationViewModel);
            return View(readPublicationViewModel);
        }

        // GET: Uptodate operator
        [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator, KnownRoles.Documentalist)]
        public async Task<ActionResult> UptodateOperator(int? userId, bool partial = false)
        {
            UserModel user = JwtTokenProvider.GetUserModel(Request.Cookies["token"].Value);
            if (!userId.HasValue && !user.Roles.Any(r => (new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Documentalist }.Contains(r))))
                return RedirectToAction("UptodateOperator", new { userId = user.UserId, partial = partial });
            var (users, roles, languages, teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var lastPublications = await _prepareService.GetLastPublications((int)PublishModeEnum.Formation);
            var trainings = await _prepareService.GetTrainings();
            var filteredUsers = new List<User>();
            if (userId.HasValue)
                filteredUsers = users.Where(u => u.UserId == userId.Value).ToList();
            else
                filteredUsers = (await LicenseMapper.FilterByActivatedUser(users, u => u.UserId)).ToList();
            var uptodateOperatorsViewModel = PublicationMapper.ToUptodateOperatorsViewModel(lastPublications, trainings, filteredUsers);
            if (partial)
                return PartialView(uptodateOperatorsViewModel);
            return View(uptodateOperatorsViewModel);
        }

        // GET: Publication qualification
        [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator, KnownRoles.Documentalist)]
        public async Task<ActionResult> PublicationQualification(int? userId, string team, string position, bool partial = false)
        {
            UserModel user = JwtTokenProvider.GetUserModel(Request.Cookies["token"].Value);
            if (!userId.HasValue && user.Roles.Any(r => !(new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Documentalist }.Contains(r))))
                return RedirectToAction("PublicationQualification", new { userId = user.UserId, team = team, position = position, partial = partial });
            var lastPublications = await _prepareService.GetLastSameMajorPublications((int)PublishModeEnum.Evaluation);
            if ((team == null && position == null) || (team == "0" && position == "All"))
            {
                var (users, roles, languages, teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
                if (userId.HasValue)
                    users = users.Where(u => u.UserId == userId.Value).ToArray();
                else
                    users = users.ToArray();
                if (partial)
                    return PartialView(PublicationMapper.ToPublicationQualificationsViewModel(lastPublications, (users, roles, languages, teams)));
                return View(PublicationMapper.ToPublicationQualificationsViewModel(lastPublications, (users, roles, languages, teams)));
            }
            else
            {
                var usersTeams = await _applicationUsersService.GetUsersTeams(team, position, null);
                var filteredUsers = new List<User>();
                if (userId.HasValue)
                    usersTeams.Users = usersTeams.Users.Where(u => u.UserId == userId.Value).ToArray();
                else
                    usersTeams.Users = usersTeams.Users.ToArray();
                if (partial)
                    return PartialView(PublicationMapper.ToPublicationQualificationsViewModel(lastPublications, usersTeams, team, position));
                return View(PublicationMapper.ToPublicationQualificationsViewModel(lastPublications, usersTeams, team, position));
            }
        }

        // GET: Operator qualification
        [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator, KnownRoles.Documentalist)]
        public async Task<ActionResult> OperatorQualification(int? userId, string team, string position, bool partial = false)
        {
            UserModel user = JwtTokenProvider.GetUserModel(Request.Cookies["token"].Value);
            if (!userId.HasValue && user.Roles.Any(r => !(new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Documentalist }.Contains(r))))
                return RedirectToAction("OperatorQualification", new { userId = user.UserId, team = team, position = position, partial = partial });
            var lastPublications = await _prepareService.GetLastSameMajorPublications((int)PublishModeEnum.Evaluation);
            var userPools = await LicenseMapper.GetUserPools();
            if (team == null && position == null)
            {
                var (users, roles, languages, teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
                users = users.Where(u => userPools.Any(p => p == u.UserId)).ToArray();
                if (userId.HasValue)
                    users = users.Where(u => u.UserId == userId.Value).ToArray();
                else
                    users = users.ToArray();
                if (partial)
                    return PartialView(PublicationMapper.ToOperatorQualificationsViewModel(lastPublications, (users, roles, languages, teams)));
                return View(PublicationMapper.ToOperatorQualificationsViewModel(lastPublications, (users, roles, languages, teams)));
            }
            else
            {
                var usersTeams = await _applicationUsersService.GetUsersTeams(team, position);
                var users = usersTeams.Users.Where(u => userPools.Any(p => p == u.UserId)).ToArray();
                if (userId.HasValue)
                    usersTeams.Users = users.Where(u => u.UserId == userId.Value).ToArray();
                else
                    usersTeams.Users = users.ToArray();
                if (partial)
                    return PartialView(PublicationMapper.ToOperatorQualificationsViewModel(lastPublications, usersTeams, team, position));
                return View(PublicationMapper.ToOperatorQualificationsViewModel(lastPublications, usersTeams, team, position));
            }
        }

        // GET: Competency
        [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator)]
        public async Task<ActionResult> Competency(string team, string position, int? userId, bool partial = false)
        {
            UserModel user = JwtTokenProvider.GetUserModel(Request.Cookies["token"].Value);
            if (!userId.HasValue && !user.Roles.Any(r => (new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer }.Contains(r))))
                return RedirectToAction("Competency", new { userId = user.UserId, team = team, position = position, partial = partial });
            if (team == null && position == null)
            {
                var (users, roles, languages, teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
                var filteredUsers = new List<User>();
                if (userId.HasValue)
                    filteredUsers = users.Where(u => u.UserId == userId.Value).ToList();
                else
                    filteredUsers = (await LicenseMapper.FilterByActivatedUser(users, u => u.UserId)).ToList();
                var publishedActions = await _prepareService.GetPublishedActions();
                var skills = await _referentialsService.LoadSkills(true);
                var lastPublicationSkills = await _prepareService.GetLastPublicationSkills();
                var competenciesViewModel = PublicationMapper.ToCompetenciesViewModel(skills,publishedActions, lastPublicationSkills, filteredUsers);
                if (partial)
                    return PartialView(competenciesViewModel);
                return View(competenciesViewModel);
            }
            else
            {
                var usersTeams = await _applicationUsersService.GetUsersTeams(team, position);
                if (userId.HasValue)
                    usersTeams.Users = usersTeams.Users.Where(u => u.UserId == userId.Value).ToArray();
                else
                    usersTeams.Users = (await LicenseMapper.FilterByActivatedUser(usersTeams.Users, u => u.UserId)).ToArray();
                var publishedActions = await _prepareService.GetPublishedActions();
                var skills = await _referentialsService.LoadSkills(true);
                var lastPublicationSkills = await _prepareService.GetLastPublicationSkills();
                var competenciesViewModel = PublicationMapper.ToCompetenciesViewModel(skills, publishedActions, lastPublicationSkills, usersTeams, team, position);
                if (partial)
                    return PartialView(competenciesViewModel);
                return View(competenciesViewModel);
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

        public Task<ActionResult> GetTenured()
        {
            List<Tenured> listTenured = new List<Tenured>
            {
                new Tenured { Value = "All", Text = LocalizedStrings.GetString("AllPositions") },
                new Tenured { Value = "True", Text = LocalizedStrings.GetString("Tenant") },
                new Tenured { Value = "False", Text = LocalizedStrings.GetString("Interim") }
            };

            var jsonSerializer = new JavaScriptSerializer();
            return Task.FromResult<ActionResult>(Json(jsonSerializer.Serialize(listTenured)));
        }

        public async Task ExportToExcel(string GridModel, string gridId, int? userId)
        {
            ExcelExport exp = new ExcelExport();
            GridProperties obj = (GridProperties)Syncfusion.JavaScript.Utils.DeserializeToModel(typeof(GridProperties), GridModel);
            //Clear if there are any filter columns
            //syncfusion bug in exporting while in filter mode
            obj.FilterSettings.FilteredColumns.Clear();
            grid = gridId;
            count = 0;
            if (gridId == "UsersReadPublications")
            {
                var (users, roles, languages, teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
                var filteredUsers = new List<User>();
                if (userId.HasValue)
                    filteredUsers = users.Where(u => u.UserId == userId.Value).ToList();
                else
                    filteredUsers = users.ToList();
                var lastPublications = await _prepareService.GetLastPublications((int)PublishModeEnum.Formation);
                var data = PublicationMapper.ToReadPublicationViewModel(lastPublications, filteredUsers);
                var publications = data.UserReadPublicationViewModel.ToList();
                columnCounter = data.PublicationCount;
                var dataSource = publications;

                PublicationReadData = publications.ToList();
                var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");
                if (columnCounter != 0)
                {
                    //For dynamic columns (in here : Publications), we need to redefine the Field to get all values
                    //While exporting values to excel (QueryCellInfo), assign the HasRead value to the correct Publications column

                    //columnCounter + 4 : because there is UserId, Fullname, Position and Teams column in the GridModel before Publications column
                    for (int i = 4; i < columnCounter + 4; i++)
                    {
                        obj.Columns[i].Field = "HasRead";
                    }
                }
                obj.ServerExcelQueryCellInfo = QueryCellInfo;
                exp.Export(obj, dataSource, LocalizedStrings.GetString("ReadPublication") + " " + currentDate + ".xlsx", ExcelVersion.Excel2013, false, false, "flat-saffron");
            }
            if (gridId == "UptodateOperators")
            {
                var (users, roles, languages, teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
                var filteredUsers = new List<User>();
                if (userId.HasValue)
                    filteredUsers = users.Where(u => u.UserId == userId.Value).ToList();
                else
                    filteredUsers = users.ToList();
                var lastPublications = await _prepareService.GetLastPublications((int)PublishModeEnum.Formation);
                var trainings = await _prepareService.GetTrainings();
                var data = PublicationMapper.ToUptodateOperatorsViewModel(lastPublications, trainings, filteredUsers);
                var publications = data.OperatorsViewModel.ToList();
                columnCounter = data.PublicationCount;
                var dataSource = publications;

                UptodateOperatorsData = publications.ToList();
                var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");
                if (columnCounter != 0)
                {
                    //For dynamic columns (in here : Publications), we need to redefine the Field to get all values
                    //While exporting values to excel (QueryCellInfo), assign the HasRead value to the correct Publications column

                    //columnCounter + 4 : because there is UserId, Fullname, Position and Teams column in the GridModel before Publications column
                    for (int i = 4; i < columnCounter + 4; i++)
                    {
                        obj.Columns[i].Field = "Uptodate";
                    }
                }
                obj.ServerExcelQueryCellInfo = QueryCellInfo;
                exp.Export(obj, dataSource, LocalizedStrings.GetString("UptodateOperator") + " " + currentDate + ".xlsx", ExcelVersion.Excel2013, false, false, "flat-saffron");
            }
            if (gridId == "PublicationQualification") {
                var (users, roles, languages, teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
                var lastPublications = await _prepareService.GetLastPublications((int)PublishModeEnum.Evaluation);
                var qualificationsViewModel = PublicationMapper.ToPublicationQualificationsViewModel(lastPublications, (users, roles, languages, teams));
                var dataSource = qualificationsViewModel.PublicationQualificationViewModel.ToList();
                var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");
                obj.ServerExcelQueryCellInfo = QueryCellInfo;
                exp.Export(obj, dataSource, LocalizedStrings.GetString("PublicationQualification") + " " + currentDate + ".xlsx", ExcelVersion.Excel2013, false, false, "flat-saffron");
            }
            if (gridId == "OperatorQualification")
            {
                var (users, roles, languages, teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
                var lastPublications = await _prepareService.GetLastPublications((int)PublishModeEnum.Evaluation);
                var qualificationsViewModel = PublicationMapper.ToOperatorQualificationsViewModel(lastPublications, (users, roles, languages, teams));
                var dataSource = qualificationsViewModel.OperatorQualificationViewModel.ToList();
                var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");
                obj.Columns[5].Field = "PercentageRate";
                obj.ServerExcelQueryCellInfo = QueryCellInfo;
                exp.Export(obj, dataSource, LocalizedStrings.GetString("OperatorQualification") + " " + currentDate + ".xlsx", ExcelVersion.Excel2013, false, false, "flat-saffron");
            }
            if (gridId == "Competency")
            {
                var (users, roles, languages, teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
                var filteredUsers = new List<User>();
                if (userId.HasValue)
                    filteredUsers = users.Where(u => u.UserId == userId.Value).ToList();
                else
                    filteredUsers = users.ToList();
                var publishedActions = await _prepareService.GetPublishedActions();
                var lastPublicationSkills = await _prepareService.GetLastPublicationSkills();
                var skills = await _referentialsService.LoadSkills(true);
                var data = PublicationMapper.ToCompetenciesViewModel(skills, publishedActions, lastPublicationSkills, filteredUsers);
                var userCompetency = data.UserCompetencyViewModel.ToList();
                columnCounter = data.ProcessCompetencyViewModel.Count() + data.TaskCompetencyViewModel.Count();
                var dataSource = userCompetency;

                UserCompetencyData = userCompetency.ToList();
                var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");
                if (columnCounter != 0)
                {
                    //For dynamic columns (in here : Competency), we need to redefine the Field to get all values
                    //While exporting values to excel (QueryCellInfo), assign the HasCompetency value to the correct Competency column

                    //columnCounter + 2 : because there is UserId, Fullname, Position and Teams column in the GridModel before Publications column
                    for (int i = 2; i < columnCounter + 2; i++)
                    {
                        obj.Columns[i].Field = "HasCompetency";
                    }
                }
                obj.ServerExcelQueryCellInfo = QueryCellInfo;
                exp.Export(obj, dataSource, LocalizedStrings.GetString("Competency") + " " + currentDate + ".xlsx", ExcelVersion.Excel2013, false, false, "flat-saffron");
            }
        }

        public async Task ExportToExcelPublishedActions(string gridColumns, string gridId, int Id, string process, int publishModeFilter)
        {
            List<Column> columnsInfos = (List<Column>)Syncfusion.JavaScript.Utils.DeserializeToModel(typeof(List<Column>), gridColumns);
            grid = gridId;

            if (gridId == "PublishedActions")
            {
                var data = await GetPublication(Id, (PublishModeEnum)publishModeFilter);

                var dataSource = data.Actions.ToList();
                columnCounter = data.ActionsColumnCount;
                var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    //application.DefaultVersion = ExcelVersion.Excel2013;
                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    int rowCounter = 1;
                    int columnCounter = 1;
                    var headers = columnsInfos.Where(_ => _.Visible);

                    // Adding headers
                    foreach (var header in headers)
                    {
                        worksheet.Range[rowCounter, columnCounter].VerticalAlignment = ExcelVAlign.VAlignCenter;
                        worksheet.Range[rowCounter, columnCounter].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                        worksheet.Range[rowCounter, columnCounter++].Text = data.ActionHeaders.GetOrDefault(header.Field);
                    }

                    // Adding datas
                    foreach (var pAction in data.Actions)
                    {
                        rowCounter++;
                        columnCounter = 1;
                        foreach (var header in headers)
                            ExportActionValueToExcel(worksheet.Range[rowCounter, columnCounter++], pAction.ColumnValues.GetOrDefault(header.Field), header.Field);
                    }

                    // Saving the workbook to disk in XLSX format
                    workbook.SaveAs($"{LocalizedStrings.GetString("ActionList")} {process} {currentDate}.xlsx",
                        ExcelSaveType.SaveAsXLS,
                        System.Web.HttpContext.Current.Response,
                        ExcelDownloadType.Open);
                }
            }
        }

        void ExportActionValueToExcel(IRange range, ActionColumnViewModel actionColumnVM, string field)
        {
            int maxPixelSize = 500;
            switch (field)
            {
                case "Refs1":
                case "Refs2":
                case "Refs3":
                case "Refs4":
                case "Refs5":
                case "Refs6":
                case "Refs7":
                default:
                    var valueCounter = 0;
                    foreach (var actionValueVM in actionColumnVM.Values)
                    {
                        if (actionValueVM == null)
                            continue;
                        switch (actionValueVM.Type)
                        {
                            case "Image":
                                range.VerticalAlignment = ExcelVAlign.VAlignTop;
                                var fileServer = ConfigurationManager.AppSettings["FileServerUri"];
                                var fileName = $"{actionValueVM.FileHash}{actionValueVM.FileExt}";
                                if (string.IsNullOrEmpty(fileName))
                                    break;
                                try
                                {
                                    range.Value = "";
                                    var downloadPath = Path.Combine(fileServer, "GetFile", fileName);
                                    downloadPath = downloadPath.Replace("\\", "/");

                                    var webClient = new WebClient();
                                    var imageBytes = webClient.DownloadData(downloadPath);
                                    var memoryStream = new MemoryStream(imageBytes);

                                    var imageFile = Image.FromStream(memoryStream);
                                    var PixelSize = maxPixelSize;

                                    if (field != "Thumbnail")
                                        PixelSize = maxPixelSize * 25 / 100;
                                    double maxWidthPoints = PixelSize.PixelsToWidthPoints();
                                    double maxHeightPoints = PixelSize.PixelsToHeightPoints();
                                    var widthScale = Convert.ToInt32(maxWidthPoints / imageFile.Width.PixelsToWidthPoints() * 100.0);
                                    var heightScale = Convert.ToInt32(maxHeightPoints / imageFile.Height.PixelsToHeightPoints() * 100.0);

                                    var imageScaleInPercent = Math.Min(widthScale, heightScale);
                                    var rowHeight = imageFile.Height >= imageFile.Width ? maxHeightPoints : (imageFile.Height * imageScaleInPercent / 100.0).PixelsToHeightPoints();
                                    if (range.RowHeight < rowHeight)
                                        range.RowHeight = rowHeight;
                                    if(range.ColumnWidth < maxWidthPoints)
                                        range.ColumnWidth = maxWidthPoints;

                                    if (field == "Thumbnail")
                                    {
                                        var shape = range.Worksheet.Pictures.AddPicture(range.Row, range.Column, range.Row + 1, range.Column + 1, imageFile);
                                        if (shape.Width.PixelsToWidthPoints() > range.ColumnWidth)
                                            shape.Width = range.ColumnWidth.WidthPointsToPixelsInt32();
                                        if (shape.Height.PixelsToHeightPoints() > range.RowHeight)
                                            shape.Height = range.RowHeight.HeightPointsToPixelsInt32();
                                    }
                                    else
                                    {
                                        var shape = range.Worksheet.Pictures.AddPicture(range.Row, range.Column, imageFile, imageScaleInPercent, imageScaleInPercent);
                                        shape.Left = shape.Left + 2;
                                        shape.Top = shape.Top + 2;
                                    }
                                    valueCounter++;
                                    
                                    //Arrange multiple refs pictures
                                    if (field != "Thumbnail" && actionColumnVM.Values.Count > 1 && valueCounter == actionColumnVM.Values.Count)
                                    {
                                        var totalCount = range.Worksheet.Pictures.Count;
                                        var rangeCollectivePictureWidth = range.Worksheet.Pictures[totalCount - valueCounter].Width;
                                        var rangeCollectivePictureHeight = range.Worksheet.Pictures[totalCount - valueCounter].Height;
                                        while (valueCounter != 1)
                                        {
                                            var rangePicture = range.Worksheet.Pictures[totalCount - 1];
                                            var rangePictureWidth = rangePicture.Width;
                                            var rangePictureHeight = rangePicture.Height;
                                            var tempColumnWidth = Convert.ToInt32(range.ColumnWidth);
                                            var tempRowHeight = Convert.ToInt32(range.RowHeight);
                                            //range.ColumnWidth = range.ColumnWidth + rangePictureWidth.PixelsToWidthPoints();
                                            //rangePicture.Left = rangePicture.Left + rangeCollectivePictureWidth;
                                            //rangeCollectivePictureWidth = rangeCollectivePictureWidth + rangePictureWidth;
                                            range.RowHeight = range.RowHeight + rangePictureHeight.PixelsToHeightPoints();
                                            rangePicture.Top = rangePicture.Top + rangeCollectivePictureHeight;
                                            rangeCollectivePictureHeight = rangeCollectivePictureHeight + rangePictureHeight;
                                            totalCount--;
                                            valueCounter--;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _traceManager.TraceWarning(ex, ex.Message);
                                    range.Value = "";
                                }
                                break;
                            case "File":
                                range.VerticalAlignment = ExcelVAlign.VAlignCenter;
                                range.Text = actionValueVM.Description;
                                break;
                            default:
                            case "Text":
                                range.VerticalAlignment = ExcelVAlign.VAlignCenter;
                                if (actionValueVM.Quantity != null)
                                {   
                                    range.Text = range.Text + actionValueVM.Quantity.Value + "x " + actionValueVM.Value;
                                }   
                                else
                                {
                                    range.Text = range.Text + actionValueVM.Value;
                                }
                                if (actionValueVM != actionColumnVM.Values.Last())
                                    range.Text = range.Text + " ";
                                break;
                        }
                    }
                    
                    break;
            }
        }

        private void QueryCellInfo(object obj)
        {
            IRange range = (IRange)obj;

            if (grid == "UsersReadPublications")
            {
                if (range.Column > 3)
                {
                    if (columnCounter != 0)
                    {
                        //range.Column - 4 : because exporting will only take visible column, there's Username column to be considered as well
                        if (PublicationReadData[count].HasReadPreviousVersion[range.Column - 4] == true)
                        {
                            range.Value = PublicationReadData[count].ReadDate[range.Column - 4];
                            range.CellStyle.Color = Color.Orange;
                            range.CellStyle.Font.Color = ExcelKnownColors.White;
                        }
                        else if (PublicationReadData[count].HasRead[range.Column - 4] == true)
                        {
                            range.Value = PublicationReadData[count].ReadDate[range.Column - 4];
                            range.CellStyle.Color = Color.Green;
                            range.CellStyle.Font.Color = ExcelKnownColors.White;
                        }
                        else
                        {
                            range.Value = "";
                            range.CellStyle.Color = Color.Red;
                            range.CellStyle.Font.Color = ExcelKnownColors.White;
                        }

                        //consider Teams column to check wether it is the last column or not
                        //increment the count to go to the next row data
                        if (range.Column == columnCounter + 3) count++;
                    }
                }
                //if (range.Column == 3)
                //{
                //    range.Value = string.Join(",", PublicationReadData[count].Teams);
                //}
            }
            if (grid == "UptodateOperators")
            {
                try
                {
                    if (range.Column > 3)
                    {
                        if (columnCounter != 0)
                        {
                            var uptodateVersion = UptodateOperatorsData[count].UptodatePreviousVersion;
                            var uptodate = UptodateOperatorsData[count].Uptodate;
                            //range.Column - 4 : because exporting will only take visible column, there's Username column to be considered as well
                            var rangeColumn = range.Column - 4;
                            if (uptodateVersion.Count >= rangeColumn + 1 && uptodateVersion[rangeColumn] == true)
                            {
                                range.Value = UptodateOperatorsData[count].UptodateDate[rangeColumn];
                                range.CellStyle.Color = Color.Orange;
                                range.CellStyle.Font.Color = ExcelKnownColors.White;
                            }
                            else if (uptodate.Count >= rangeColumn + 1 && uptodate[rangeColumn] == true)
                            {
                                range.Value = UptodateOperatorsData[count].UptodateDate[rangeColumn];
                                range.CellStyle.Color = Color.Green;
                                range.CellStyle.Font.Color = ExcelKnownColors.White;
                            }
                            else
                            {
                                range.Value = "";
                                range.CellStyle.Color = Color.Red;
                                range.CellStyle.Font.Color = ExcelKnownColors.White;
                            }

                            //consider Username column to check wether it is the last column or not
                            //increment the count to go to the next row data
                            if (range.Column == columnCounter + 3)
                                count++;
                        }
                    }
                    if (range.Column == 3)
                    {
                        range.Value = string.Join(",", UptodateOperatorsData[count].Teams);
                    }
                }
                catch (Exception ex)
                {
                    _traceManager.TraceError(ex, $"Error on {nameof(QueryCellInfo)}");
                }
            }
            if (grid == "OperatorQualification")
            {
                if (range.Column == 5)
                {
                    if (range.Value == "-1")
                        range.Value = "";
                    else
                        range.Value += " %";
                }
            }
            if (grid == "PublicationQualification")
            {
                if (range.Column == 6)
                {
                    if (range.Value == "-1")
                        range.Value = "";
                    else
                        range.Value += " %";
                }
            }
            if (grid == "Competency")
            {
                if (range.Column > 1)
                {
                    if (columnCounter != 0)
                    {
                        //range.Column - 2 : because exporting will only take visible column, there's Username column to be considered as well
                        if (UserCompetencyData[count].HasCompetencyPreviousVersion[range.Column - 2] == true)
                        {
                            range.Value = UserCompetencyData[count].HasCompetencyData[range.Column - 2];
                            range.CellStyle.Color = Color.Orange;
                            range.CellStyle.Font.Color = ExcelKnownColors.White;
                        }
                        else if (UserCompetencyData[count].HasCompetency[range.Column - 2] == true)
                        {
                            range.Value = UserCompetencyData[count].HasCompetencyData[range.Column - 2];
                            range.CellStyle.Color = Color.Green;
                            range.CellStyle.Font.Color = ExcelKnownColors.White;
                        }
                        else
                        {
                            range.Value = UserCompetencyData[count].HasCompetencyData[range.Column - 2];
                            range.CellStyle.Color = Color.Red;
                            range.CellStyle.Font.Color = ExcelKnownColors.White;
                        }

                        //consider Username column to check wether it is the last column or not
                        //increment the count to go to the next row data
                        if (range.Column == columnCounter + 1) count++;
                    }
                }
            }
            if (grid == "PublishedActions")
            {
                if (columnCounter != 0)
                {
                    var columnValue = PublishedActionsData[count].ColumnValues.ElementAt(range.Column - 1).Value.Values.FirstOrDefault();
                    if (columnValue != null)
                    {
                        //Set null if value type is not text (ex. Image)
                        if (columnValue.Type == "Text")
                        {
                            range.Value = PublishedActionsData[count].ColumnValues.ElementAt(range.Column - 1).Value.Values.Select(s => s.Value).FirstOrDefault();
                        }
                        else if (columnValue.Type == "Image")
                        {
                            var fileServer = ConfigurationManager.AppSettings["FileServerUri"];
                            try
                            {
                                range.Value = "";
                                var downloadPath = Path.Combine(fileServer, "GetFile", columnValue.FileHash + columnValue.FileExt);
                                downloadPath = downloadPath.Replace("\\", "/");

                                var webClient = new WebClient();
                                var imageBytes = webClient.DownloadData(downloadPath);
                                var memoryStream = new MemoryStream(imageBytes);

                                var imageFile = Image.FromStream(memoryStream);

                                var shape = range.Worksheet.Pictures.AddPicture(range.Row, range.Column, imageFile);

                                ////Resize picture
                                shape.Height = height;
                                shape.Width = width;
                            }
                            catch
                            {
                                range.Value = "";
                            }
                        }
                        else
                            range.Value = "";
                    }
                    else range.Value = "";

                    range.VerticalAlignment = ExcelVAlign.VAlignTop;

                    //increment the count to go to the next row data
                    if (range.Column == columnCounter) count++;
                }

            }
        }
    }
}