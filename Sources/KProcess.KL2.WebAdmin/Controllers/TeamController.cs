using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.KL2.WebAdmin.Models.Teams;
using KProcess.KL2.WebAdmin.Models.Users;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using Syncfusion.EJ.Export;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using KProcess.KL2.JWT;
using KProcess.KL2.WebAdmin.Mapper;

namespace KProcess.KL2.WebAdmin.Controllers
{
    [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor)]
    [SettingUserContextFilter]
    public class TeamController : LocalizedController
    {
        private IApplicationUsersService _applicationUsersService;

        //For populating Users column in Excel Export
        List<TeamViewModel> currentData = null;
        public int count = 0;

        public TeamController(IApplicationUsersService applicationUsersService,
            ILocalizationManager localizationManager)
            : base(localizationManager)
        {
            _applicationUsersService = applicationUsersService;
        }
        // GET: Team
        public async Task<ActionResult> Index(bool partial = false)
        {
            if (partial)
                return PartialView(await GetData());
            return View(await GetData());
        }

        public async Task<ActionResult> InsertTeam(CRUDModel<TeamViewModel> team)
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

                var usersRolesLanguages = await _applicationUsersService.GetUsersAndRolesAndLanguages();
                var newTeam = new Team
                {
                    Name = team.Value.TeamName
                };
                List<string> fullnames = new List<string>();
                if (team.Value.Fullname.Count != 0)
                {
                    foreach (var user in team.Value.Fullname[0].Split(','))
                    {
                        if (string.IsNullOrWhiteSpace(user))
                            continue;
                        var userFound = usersRolesLanguages.Users.First(u => u.UserId == Convert.ToInt32(user));
                        newTeam.Users.Add(userFound);
                        fullnames.Add(userFound.FullName);
                    }
                }
                

                IEnumerable<Team> addTeam = new[] { newTeam };
                await _applicationUsersService.SaveTeams(addTeam);

                var teams = await GetData();
                var createdTeam = teams.TeamViewModel.First(u => u.TeamName == newTeam.Name);
                team.Value.TeamId = createdTeam.TeamId;
                team.Value.Fullname = fullnames;
                return Json(team.Value, JsonRequestBehavior.AllowGet);
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

        public async Task<ActionResult> UpdateTeam(CRUDModel<TeamViewModel> team)
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

                var usersRolesLanguages = await _applicationUsersService.GetUsersAndRolesAndLanguages();
                var teamEdit = usersRolesLanguages.Teams.FirstOrDefault(t => t.Id == team.Value.TeamId);
                teamEdit.StartTracking();

                teamEdit.Name = team.Value.TeamName;

                //Update team users
                teamEdit.Users.Clear();
                if (team.Value.Fullname.Count != 0)
                {   
                    foreach (var user in team.Value.Fullname[0].Split(','))
                    {
                        if (string.IsNullOrWhiteSpace(user))
                            continue;
                        teamEdit.Users.Add(usersRolesLanguages.Users.First(u => u.UserId == Convert.ToInt32(user)));
                    }
                }
                teamEdit.MarkAsModified();
                


                IEnumerable<Team> editTeam = new[] { teamEdit };
                await _applicationUsersService.SaveTeams(editTeam);

                var teams = await GetData();
                var updateTeam = teams.TeamViewModel.First(u => u.TeamName == teamEdit.Name);
                return Json(updateTeam, JsonRequestBehavior.AllowGet);
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


        public async Task<ActionResult> DeleteTeam(int key)
        {
            try
            {
                var usersRolesLanguages = await _applicationUsersService.GetUsersAndRolesAndLanguages();
                var teamDelete = usersRolesLanguages.Teams.FirstOrDefault(u => u.Id == key);
                if (teamDelete != null)
                {
                    teamDelete.StartTracking();
                    teamDelete.MarkAsDeleted();
                    IEnumerable<Team> deleteTeam = new[] { teamDelete };
                    await _applicationUsersService.SaveTeams(deleteTeam);
                }
                var users = await GetData();
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }


        private async Task<TeamsViewModel> GetData()
        {
            var usersRolesLanguages = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            TeamsViewModel teamsViewModel = new TeamsViewModel();
            var teams = usersRolesLanguages.Teams.Select(t =>
            {
                var userinfo = GetUsersForTeam(t, usersRolesLanguages.Users);
                return new TeamViewModel
                {
                    TeamId = t.Id,
                    TeamName = t.Name,
                    UserId = userinfo.Item1,
                    Username = userinfo.Item2,
                    Fullname = userinfo.Item3
                };
            });
            var allUsername = usersRolesLanguages.Users.Select(u => u.Username).ToList();
            teamsViewModel.TeamViewModel = teams.ToList();
            teamsViewModel.AllUsers = allUsername;
            return teamsViewModel;
        }

        public async Task<ActionResult> GetUser()
        {
            var usersRolesLanguages = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var data = usersRolesLanguages.Users.Select(
                u => new { Fullname = u.FullName, u.Username, u.UserId }
            ).ToList();

            var jsonSerializer = new JavaScriptSerializer();
            return Json(jsonSerializer.Serialize(data));
        }

        public async Task<ActionResult> GetUserWithIds(List<int> ids)
        {
            var usersRolesLanguages = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var userPools = await LicenseMapper.GetUserPools();
            var data = usersRolesLanguages.Users.Where(u => userPools.Any(p => p == u.UserId)).Select(
                u => new { Fullname = u.FullName, u.Username, u.UserId, IsChecked = ids != null ? ids.Contains(u.UserId) : false }
            ).ToList();

            var jsonSerializer = new JavaScriptSerializer();
            return Json(jsonSerializer.Serialize(data));
        }

        public async Task<ActionResult> GetTeam(TeamViewModel value)
        {
            var usersRolesLanguages = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var team = usersRolesLanguages.Teams.FirstOrDefault(u => u.Id == value.TeamId);
            var teamInfo = GetUsersForTeam(team, usersRolesLanguages.Users);
            value.UserId = teamInfo.Item1;
            value.Username = teamInfo.Item2;
            value.Fullname = teamInfo.Item3;

            return PartialView(value);
        }

        private (List<int>, List<string>, List<string>) GetUsersForTeam(Team team, User[] allUsers)
        {
            if (team == null || team.Users == null)
                return (new List<int>(), new List<string>(), new List<string>());

            var UserId = new List<int>();
            var Username = new List<string>();
            var Fullname = new List<string>();
            

            var userList = allUsers.Select(u => new UserViewModel
            {
                UserId = u.UserId,
                Username = u.Username,
                Firstname = u.Firstname,
                Name = u.Name,
                FullName = u.FullName
            }).ToList();
            
            foreach (var user in team.Users.OrderBy(u => u.FullName))
            {
                var userFound = userList.First(u => u.UserId == user.UserId);
                Username.Add(userFound.Username);
                UserId.Add(userFound.UserId.Value);
                Fullname.Add(userFound.FullName);
            }
            return (UserId, Username, Fullname);
        }

        public async Task ExportToExcel(string GridModel)
        {

            ExcelExport exp = new ExcelExport();
            var data = await GetData();
            var teams = data.TeamViewModel.ToList();
            var DataSource = teams;
            currentData = teams;
            var currentDate = DateTime.Today.ToShortDateString().Replace("/", "-");

            GridProperties obj = (GridProperties)Syncfusion.JavaScript.Utils.DeserializeToModel(typeof(GridProperties), GridModel);
            obj.ServerExcelQueryCellInfo = QueryCellInfo;

            exp.Export(obj, DataSource, LocalizedStrings.GetString("Team") + " " + currentDate + ".xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");

        }

        private void QueryCellInfo(object obj)
        {
            IRange range = (IRange)obj;
            if (range.Column == 2)
            {
                range.Value = string.Join(",", currentData[count].Username);
                count++;
            }
        }
    }
}