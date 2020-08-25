using Kprocess.PackIconKprocess;
using KProcess.Globalization;
using KProcess.KL2.APIClient;
using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.KL2.WebAdmin.Mapper;
using KProcess.KL2.WebAdmin.Models.Users;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Activation;
using Microsoft.AspNet.Identity;
using Syncfusion.EJ.Export;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace KProcess.KL2.WebAdmin.Controllers
{
    [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor)]
    [SettingUserContextFilter]
    public class UserController : LocalizedController
    {
        readonly ITraceManager _traceManager;
        IApplicationUsersService _applicationUsersService;
        ILocalizedStrings _localizedStrings;
        readonly IAPIHttpClient _apiHttpClient;

        //For populating Roles column in Excel Export
        List<UserViewModel> currentData;
        public int count;

        public UserController(IApplicationUsersService applicationUsersService, ITraceManager traceManager, ILocalizedStrings localizedStrings, IAPIHttpClient apiHttpClient,
            ILocalizationManager localizationManager)
            : base(localizationManager)
        {
            _applicationUsersService = applicationUsersService;
            _traceManager = traceManager;
            _localizedStrings = localizedStrings;
            _apiHttpClient = apiHttpClient;
        }

        /// <summary>
        /// Get index users page 
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index(bool partial = false)
        {
            if (partial)
                return PartialView(await GetData());
            return View(await GetData());
        }

        /// <summary>
        /// Get the roles
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetRoles()
        {
            var (Users, Roles, Languages, Teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var data = Roles.Where(r => r.RoleCode != KnownRoles.Contributor).Select(
                u => new { u.RoleCode, u.ShortLabel, Description = GetRolesDescription(u.RoleCode) }            
            ).ToList();

            var jsonSerializer = new JavaScriptSerializer();
            return Json(jsonSerializer.Serialize(data));
        }

        public static Dictionary<string, int> rolesOrder =
            new Dictionary<string, int>() {
              {KnownRoles.Administrator, 1},
              {KnownRoles.Analyst, 2},
              {KnownRoles.Documentalist, 3},
              {KnownRoles.Operator, 4},
              {KnownRoles.Technician, 5},
              {KnownRoles.Trainer, 6},
              {KnownRoles.Evaluator, 7},
              {KnownRoles.Supervisor, 8}
          };

        public async Task<ActionResult> GetRolesWithCodes(List<string> RoleCodes)
        {
            var (Users, Roles, Languages, Teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var data = Roles.Where(r => r.RoleCode != KnownRoles.Contributor).Select(
                u => new { u.RoleCode, u.ShortLabel, isChecked = RoleCodes != null ? RoleCodes.Contains(u.RoleCode) : false , Description = GetRolesDescription(u.RoleCode) }
            ).OrderBy(item => rolesOrder[item.RoleCode]).ToList();

            var jsonSerializer = new JavaScriptSerializer();
            return Json(jsonSerializer.Serialize(data));
        }

        /// <summary>
        /// Get description as title for tooltip
        /// </summary>
        /// <param name="roleCode"></param>
        /// <returns></returns>
        public string GetRolesDescription(string roleCode)
        {
            switch (roleCode)
            {
                case KnownRoles.Administrator:
                    return LocalizedStrings.GetString("AdminRoleDescription");
                case KnownRoles.Analyst:
                    return LocalizedStrings.GetString("AnalystRoleDescription");
                case KnownRoles.Operator:
                    return LocalizedStrings.GetString("OperatorRoleDescription");
                case KnownRoles.Supervisor:
                    return LocalizedStrings.GetString("SupervisorRoleDescription");
                case KnownRoles.Trainer:
                    return LocalizedStrings.GetString("TrainerRoleDescription");
                case KnownRoles.Documentalist:
                    return LocalizedStrings.GetString("DocumentalistRoleDescription");
                case KnownRoles.Technician:
                    return LocalizedStrings.GetString("TechnicianRoleDescription");
                case KnownRoles.Evaluator:
                    return LocalizedStrings.GetString("EvaluatorRoleDescription");
                default:
                    return "";
            }
        }

        public Task<ActionResult> GetTenured()
        {
            List<Tenured> listTenured = new List<Tenured>
            {
                new Tenured { Value = "", Text = "" },
                new Tenured { Value = "True", Text = LocalizedStrings.GetString("Tenant") },
                new Tenured { Value = "False", Text = LocalizedStrings.GetString("Interim") }
            };

            var jsonSerializer = new JavaScriptSerializer();
            return Task.FromResult<ActionResult>(Json(jsonSerializer.Serialize(listTenured)));
        }

        /// <summary>
        /// Get the languages
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> GetLanguages()
        {
            var (Users, Roles, Languages, Teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var data = Languages.Select(
                u => new { u.LanguageCode, u.Label, u.FlagName }
            ).ToList();

            var model = new List<LanguageViewModel>();

            foreach (var lang in data)
            {
                Enum.TryParse(lang.FlagName, out PackIconCountriesFlagsKind flag);
                
                var path = GetCountryFlagPath(flag);
                
                model.Add(new LanguageViewModel {
                    Label = lang.Label,
                    LanguageCode = lang.LanguageCode,
                    FlagName = lang.FlagName,
                    Flag = flag,
                    FlagPath = path
                });
            }

            var jsonSerializer = new JavaScriptSerializer();
            return Json(jsonSerializer.Serialize(model));
        }

        public string GetCountryFlagPath(PackIconCountriesFlagsKind flag)
        {
            string filePath = "";
            filePath = System.IO.Path.Combine("Files", "SVG_CountriesFlags", flag.ToString() + ".svg");
            return filePath;
        }

        [HttpGet]
        public async Task<ActionResult> GetUsernameLists()
        {
            var (Users, Roles, Languages, Teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var data = Users.Select(
                u => new { u.Username}
                ).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Action method that return a partial view with information about user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetUser(UserViewModel value)
        {
            var (Users, Roles, Languages, Teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var user = Users.FirstOrDefault(u => u.UserId == value.UserId);
            var rolesInfo = GetRolesForUser(user, Roles.Where(r => r.RoleCode != KnownRoles.Contributor).ToArray());
            value.Roles = rolesInfo.Item1;
            value.RolesId = rolesInfo.Item2;
            value.RoleCodes = rolesInfo.Item3;
            value.AdministratorCount = Users.Count(u => u.Roles.Any(r => r.RoleCode == KnownRoles.Administrator));

            AutocompleteProperties auto = new AutocompleteProperties
            {
                DataSource = Roles.Select(u => u.ShortLabel),
                FilterType = FilterOperatorType.Contains
            };
            AutocompleteFields fld = new AutocompleteFields();
            auto.Select = "onSelect";
            ViewData["auto"] = auto;

            return PartialView(value);
        }

        private (List<string>, List<int>, List<string>) GetRolesForUser(User user, Role[] allRoles)
        {
            if (user == null || user.Roles == null)
                return (new List<string>(), new List<int>(), new List<string>());

            var roles = new List<string>();
            var rolesId = new List<int>();
            var roleCodes = new List<string>();

            var roleList = allRoles.Select(r => new RoleViewModel
            {
                RoleCode = r.RoleCode,
                ShortLabel = r.ShortLabel
            }).ToList();

            // FIll roles label
            foreach (var roleCode in user.Roles)
            {
                var roleFound = roleList.First(u => u.RoleCode == roleCode.RoleCode);
                roles.Add(roleFound.ShortLabel);
                rolesId.Add(roleList.FindIndex(u => u.RoleCode == roleCode.RoleCode));
                roleCodes.Add(roleCode.RoleCode);
            }
            return (roles, rolesId, roleCodes);
        }

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> UpdateUser(CRUDModel<UserViewModel> user)
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

                if (user.Value.Roles == null || !user.Value.Roles.Any())
                    throw new Exception(LocalizedStrings.GetString("AskSelectRole"));

                var (Users, Roles, Languages, Teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
                var userEdit = Users.FirstOrDefault(u => u.UserId == user.Value.UserId);
                userEdit.StartTracking();

                userEdit.DefaultLanguageCode = user.Value.DefaultLanguageCode;
                userEdit.Tenured = user.Value.Tenured;
                userEdit.Username = user.Value.Username;
                userEdit.Firstname = user.Value.Firstname;
                userEdit.Name = user.Value.Name;
                userEdit.Email = user.Value.Email;
                userEdit.PhoneNumber = user.Value.PhoneNumber;
                if (user.Value.NewPassword != null)
                {
                    var hash = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(
                            Encoding.Default.GetBytes(user.Value.NewPassword));
                    userEdit.Password = hash;
                }
                // Reset the roles
                userEdit.Roles.Clear();
                foreach (var role in user.Value.Roles[0].Split(','))
                {
                    if (string.IsNullOrWhiteSpace(role))
                        continue;

                    userEdit.Roles.Add(Roles.First(u => u.RoleCode == role));
                }

                IEnumerable<User> editUser = new[] { userEdit };
                await _applicationUsersService.SaveUsers(editUser);

                var users = await GetData();
                var updateUser = users.UsersViewModel.First(u => u.Username == userEdit.Username);
                return Json(updateUser, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                 .SelectMany(v => v.Errors)
                 .Select(e => e.ErrorMessage));
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public async Task<ActionResult> UpdateUserStatus(List<int> userIds, bool status)
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;

            try
            {
                var license = await LicenseMapper.GetLicense();

                if (license.Status == WebLicenseStatus.NotFound)
                {
                    return Json(new { Success = false, message = LocalizedStrings.GetString( "Web_Controller_User_LicenseDoesntExist") }, JsonRequestBehavior.AllowGet);
                }
                else if (license.Status == WebLicenseStatus.MachineHashMismatch)
                {
                    return Json(new { Success = false, message = LocalizedStrings.GetString( "Web_Controller_User_LicenseMismatchesMachine") }, JsonRequestBehavior.AllowGet);
                }
                //Check user maximum limit
                if (status == true)
                {
                    if (license.UsersPool.Count >= license.NumberOfUsers)
                    {
                        return Json(new { Success = false, message = LocalizedStrings.GetString( "Web_Controller_User_OverageOfUsers") }, JsonRequestBehavior.AllowGet);
                    }
                }
                //Admin cannot deactivate his own Active status
                if (status == false && userIds.Any(u => u == int.Parse(System.Web.HttpContext.Current.User.Identity.GetUserId())))
                {
                    return Json(new { Success = false, message = LocalizedStrings.GetString( "Web_Controller_User_CantSelfDeactivate") }, JsonRequestBehavior.AllowGet);
                }

                if (status)
                {
                    foreach (var id in userIds)
                    {
                        license.UsersPool.Add(id);
                    }
                }   
                else
                {
                    await CancelAndDisableUserRelatedActivity(userIds);
                    foreach (var id in userIds)
                    {   
                        license.UsersPool.Remove(id);

                    }
                }

                await LicenseMapper.SetLicense(license);

                var users = await GetData();
                var updateUsers = users.UsersViewModel;

                return Json(new { Success = true, Users = updateUsers, activatedCount = license.UsersPool.Count() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                 .SelectMany(v => v.Errors)
                 .Select(e => e.ErrorMessage));
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public async Task<bool> CancelAndDisableUserRelatedActivity(List<int> userIds)
        {
            var (Users, Roles, Languages, Teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var toCheckUsers = Users.Where(u => userIds.Any(i => i == u.UserId));
            foreach (var user in toCheckUsers)
            {
                user.StartTracking();
                if (user.Audits.Count != 0)
                {
                    foreach (var audit in user.Audits)
                    {
                        if (audit.EndDate == null && audit.IsDeleted == false)
                        {
                            audit.IsDeleted = true;
                            audit.MarkAsModified();
                        }   
                    }
                }
                if (user.Teams.Count != 0)
                {
                    user.Teams.Clear();
                    user.MarkAsModified();
                }
            }
            await _applicationUsersService.SaveUsers(toCheckUsers);
            return true;
        }

        /// <summary>
        /// Insert user information
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<ActionResult> InsertUser(CRUDModel<UserViewModel> user)
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

                if (user.Value.Roles == null || !user.Value.Roles.Any() || user.Value.Roles.Any(n => n == ""))
                    throw new Exception(LocalizedStrings.GetString("AskSelectRole"));

                var (Users, Roles, Languages, Teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
                var newUser = new User
                {
                    Username = user.Value.Username,
                    DefaultLanguageCode = user.Value.DefaultLanguageCode,
                    Tenured = user.Value.Tenured,
                    Firstname = user.Value.Firstname,
                    Name = user.Value.Name,
                    Email = user.Value.Email,
                    PhoneNumber = user.Value.PhoneNumber,
                    CreationDate = DateTime.Now,
                    LastModificationDate = DateTime.Now
                };
                //Add password to user
                if (user.Value.Password != null)
                    newUser.Password = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(user.Value.Password));

                foreach (var role in user.Value.Roles[0].Split(','))
                {
                    if (string.IsNullOrWhiteSpace(role))
                        continue;
                    newUser.Roles.Add(Roles.First(u => u.RoleCode == role));
                }

                IEnumerable<User> addUser = new[] { newUser };
                await _applicationUsersService.SaveUsers(addUser);

                var users = await GetData();
                var createdUser = users.UsersViewModel.First(u => u.Username == newUser.Username);
                user.Value.UserId = createdUser.UserId;
                user.Key = user.Value.UserId;
                user.KeyColumn = "UserId";
                user.Value.Roles = createdUser.Roles;
                user.Value.Teams = createdUser.Teams;
                user.Value.TenuredDisplay = createdUser.Tenured.HasValue ? createdUser.Tenured.Value ? LocalizedStrings.GetString("Tenant") : LocalizedStrings.GetString("Interim") : "";
                return Json(user.Value, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                var message = string.Join(" | ", ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage));
                                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        /// <summary>
        /// Delete user information
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ActionResult> DeleteUser(int key)
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;

            try
            {
                var (Users, Roles, Languages, Teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
                var userDelete = Users.FirstOrDefault(u => u.UserId == key);
                if (userDelete != null)
                {
                    var license = await LicenseMapper.GetLicense();
                    if (license.UsersPool.Any(u => u == key))
                    {
                        //Delete user from License userpool
                        license.UsersPool.Remove(key);
                        await CancelAndDisableUserRelatedActivity(new List<int> { key });
                        await LicenseMapper.SetLicense(license);
                    }

                    userDelete.IsDeleted = true;
                    userDelete.MarkAsDeleted();

                    IEnumerable<User> editUser = new[] { userDelete };
                    await _applicationUsersService.SaveUsers(editUser);
                }
                var users = await GetData();
                return Json(users, JsonRequestBehavior.AllowGet);
            }
            catch (InvalidOperationException ex)
            {
                var message = LocalizedStrings.GetString("UnableUsedUserDelete");
                _traceManager.TraceError(ex, message);
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        /// <summary>
        /// Mapping users and roles to view model
        /// </summary>
        /// <returns></returns>
        private async Task<UserManageViewModel> GetData()
        {
            var license = await LicenseMapper.GetLicense();
            ViewBag.ActivatedUsersCount = license?.UsersPool?.Count ?? 0;
            ViewBag.ActivatedUsersMax = license?.NumberOfUsers ?? 0;

            var (Users, Roles, Languages, Teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            UserManageViewModel userManageModel = new UserManageViewModel();
            var users = Users.Select(u =>             
            {
                var rolesInfo = GetRolesForUser(u, Roles);
                return new UserViewModel
                {
                    UserId = u.UserId,
                    DefaultLanguageCode = u.DefaultLanguageCode,
                    Username = u.Username,
                    Firstname = u.Firstname,
                    Name = u.Name,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Roles = rolesInfo.Item1,
                    RolesId = rolesInfo.Item2,
                    Tenured = u.Tenured,
                    TenuredDisplay = u.Tenured.HasValue? u.Tenured.Value? LocalizedStrings.GetString("Tenant") : LocalizedStrings.GetString("Interim") : "",
                    Teams = u.Teams.Select(t => t.Name).ToList(),
                    IsActive = license?.UsersPool?.Contains(u.UserId) == true ? true : false
                };
            });
            var allRoles = Roles.ToList();
            var roleList = allRoles.Select(r => new RoleViewModel
            {
                RoleCode = r.RoleCode,
                ShortLabel = r.ShortLabel
            });
            var allLanguages = Languages.ToList();
            var languageList = allLanguages.Select(l => new LanguageViewModel
            {
                LanguageCode = l.LanguageCode,
                Label = l.Label
            });
            userManageModel.RolesViewModel = roleList.ToList();
            userManageModel.UsersViewModel = users.ToList();

            userManageModel.LanguagesViewModel = languageList.ToList();
            return userManageModel;
        }

        
        public async Task ExportToExcel(string GridModel)
        {

            ExcelExport exp = new ExcelExport();
            var data = await GetData();
            var users = data.UsersViewModel.ToList();
            var DataSource = users;
            currentData = users;
            var currentDate = DateTime.Today.ToShortDateString().Replace("/","-");

            GridProperties obj = (GridProperties)Syncfusion.JavaScript.Utils.DeserializeToModel(typeof(GridProperties), GridModel);
            //Clear if there are any filter columns
            //syncfusion bug in exporting while in filter mode
            obj.FilterSettings.FilteredColumns.Clear();

            obj.ServerExcelQueryCellInfo = QueryCellInfo;

            exp.Export(obj, DataSource, LocalizedStrings.GetString("User") + " " + currentDate + ".xlsx", ExcelVersion.Excel2010, false, false, "flat-saffron");

        }

        private void QueryCellInfo(object obj)
        {   
            IRange range = (IRange)obj;
            if (range.Column == 5)
            {
                range.Value = string.Join(",", currentData[count].Teams);
            }
            if (range.Column == 7)
            {
                range.Value = string.Join(",", currentData[count].Roles);
                count++;
            }
        }
    }
}