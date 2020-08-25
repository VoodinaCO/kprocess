using KProcess.KL2.APIClient;
using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.KL2.WebAdmin.Mapper;
using KProcess.KL2.WebAdmin.Models.License;
using KProcess.KL2.WebAdmin.Models.Users;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Activation;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KProcess.KL2.WebAdmin.Controllers
{
    [CustomAuthorize(KnownRoles.Administrator)]
    [SettingUserContextFilter]
    public class LicenseController : LocalizedController
    {
        readonly ITraceManager _traceManager;
        readonly IApplicationUsersService _applicationUsersService;
        readonly IAPIHttpClient _apiHttpClient;
        readonly ISystemInformationService _systemInformationService;

        public LicenseController(IApplicationUsersService applicationUsersService, ITraceManager traceManager, IAPIHttpClient apiHttpClient, ISystemInformationService systemInformationService,
            ILocalizationManager localizationManager)
            : base(localizationManager)
        {
            _applicationUsersService = applicationUsersService;
            _apiHttpClient = apiHttpClient;
            _systemInformationService = systemInformationService;
            _traceManager = traceManager;
        }

        // GET: License
        public async Task<ActionResult> Index(bool partial = false)
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;
            if (partial)
                return PartialView(await GetData());
            return View(await GetData());
        }

        public async Task<LicenseViewModel> GetData()
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;

            var model = new LicenseViewModel
            {
                //Prepare LicenseInfo
                LicenseInfo = new LicenseInfoViewModel
                {
                    ActivatedUsers = new List<UserViewModel>()
                }
            };

            //User information
            var userProvider = await LicenseMapper.GetUserInformation();
            if (userProvider != null)
            {
                model.LicenseInfo.Name = userProvider.Username;
                model.LicenseInfo.Company = userProvider.Company;
                model.LicenseInfo.Email = userProvider.Email;
            }

            var license = await LicenseMapper.GetLicense();

            if (license.Status != WebLicenseStatus.NotFound && license.Status != WebLicenseStatus.MachineHashMismatch)
            {
                model.LicenseFile = license;
                model.LicenseInfo.LicenseFile = license;
                model.LicenseInfo.LicenseMaxActivateUser = license.NumberOfUsers;
                model.LicenseInfo.LicenseTotalActivatedUsers = license.UsersPool.Count;
                model.LicenseInfo.ExpiredDate = license.ActivationDate.AddDays(license.TrialDays);

                model.LicenseInfo.Status = license.Status;
                model.LicenseInfo.StatusReason = license.StatusReason;

                //To display license information
                if ((model.LicenseInfo.Status != WebLicenseStatus.Licensed && DateTime.Now > model.LicenseInfo.ExpiredDate) || model.LicenseInfo.Status == WebLicenseStatus.Expired)
                {
                    model.LicenseInfo.LicenseInformation = LocalizedStrings.GetString("Web_Controller_License_InfoLicenseExpired");
                }
                else if (license.Status == WebLicenseStatus.OverageOfUsers)
                {
                    model.LicenseInfo.LicenseInformation = LocalizedStrings.GetString("Web_Controller_License_InfoOverageOfUsers");
                }
                else
                {
                    model.LicenseInfo.LicenseInformation = LocalizedStrings.GetString("Web_Controller_License_InfoLicenseValid");
                }

                if (model.LicenseInfo.LicenseTotalActivatedUsers != 0)
                {
                    //collect activated users
                    var usersRolesLanguages = await _applicationUsersService.GetUsersAndRolesAndLanguages();
                    var users = usersRolesLanguages.Users;
                    model.LicenseInfo.ActivatedUsers = license.UsersPool
                        .Where(userId => users.Any(_ => _.UserId == userId))
                        .Select(userId => users.Single(_ => _.UserId == userId))
                        .Select(user => new UserViewModel
                        {
                            UserId = user.UserId,
                            FullName = user.FullName
                        }).ToList();
                }
            }

            model.LicenseInfo.IDMachine = await LicenseMapper.GetMachineHash();
            model.LicenseInfo.Status = license.Status;
            model.LicenseInfo.StatusReason = LocalizedStrings.GetStringFormat(license.StatusReason, license.StatusReasonParams?.Select(_ => _.ToString()).ToArray());

            //Email to kprocess
            model.LicenseInfo.EmailTarget = ActivationConstants.KProcessEmail;

            return model;
        }

        public async Task<ActionResult> GetUsersActivated()
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;

            var license = await LicenseMapper.GetLicense();
            var (users, roles, languages, teams) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var model = new ActivatedUsersViewModel
            {
                ActivatedUsers = license.UsersPool
                    .Where(userId => users.Any(_ => _.UserId == userId))
                    .Select(userId => users.Single(_ => _.UserId == userId))
                    .Select(user => new UserViewModel
                    {
                        UserId = user.UserId,
                        FullName = user.FullName
                    }).ToList()
            };
            return PartialView(model);
        }

        public async Task<ActionResult> SaveUserInfo(string name, string company, string email)
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;

            await LicenseMapper.SetUserInformation(name, company, email);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<ActionResult> SaveDefault()
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;

            HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            var uploadedLicenseFile = System.Web.HttpContext.Current.Request.Files["uploadLicense"];

            var name = System.Web.HttpContext.Current.Request.Headers["name"];
            var company = System.Web.HttpContext.Current.Request.Headers["company"];
            var email = System.Web.HttpContext.Current.Request.Headers["email"];
            await LicenseMapper.SetUserInformation(name, company, email);

            ProductLicenseInfo licenseInfo = null;
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(uploadedLicenseFile.InputStream))
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ProductLicenseInfo));
                    licenseInfo = (ProductLicenseInfo)serializer.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                _traceManager.TraceError(e, e.Message);
                HttpContext.Response.StatusDescription = $"{HttpStatusCode.BadRequest}";
                return Content(LocalizedStrings.GetString("Web_Controller_License_CantCheckLicense"));
            }

            try
            {
                WebProductLicense license = await LicenseMapper.ActivateLicense(licenseInfo);

                if (license.Status == WebLicenseStatus.OverageOfUsers)
                {
                    HttpContext.Response.StatusDescription = $"{HttpStatusCode.ExpectationFailed}";
                    return Content(LocalizedStrings.GetStringFormat(license.StatusReason, license.StatusReasonParams?.Select(_ => _.ToString()).ToArray()));
                }
                // Comment this part if you want to be able to save a expired license
                if (license.Status != WebLicenseStatus.Licensed && license.Status != WebLicenseStatus.TrialVersion)
                {
                    HttpContext.Response.StatusDescription = $"{HttpStatusCode.Forbidden}";
                    return Content(LocalizedStrings.GetStringFormat(license.StatusReason, license.StatusReasonParams?.Select(_ => _.ToString()).ToArray()));
                }

                //First initialize of license, set UserPool to current Admin
                if (license.UsersPool.Count == 0)
                {
                    //Get current user id - Admin
                    var adminId = int.Parse(System.Web.HttpContext.Current.User.Identity.GetUserId());
                    license.UsersPool.Add(adminId);
                }
                await LicenseMapper.SetLicense(license);
                // TODO : Update license infos from the secured storage
            }
            catch (Exception e)
            {
                _traceManager.TraceError(e, e.Message);
                HttpContext.Response.StatusDescription = $"{HttpStatusCode.BadRequest}";
                return Content(LocalizedStrings.GetString("Web_Controller_License_LicenseInvalid"));
            }

            HttpContext.Response.StatusDescription = $"{HttpStatusCode.OK}";
            return Content($"{HttpStatusCode.OK}");
        }

        public async Task<ActionResult> DeleteLicense()
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;

            await LicenseMapper.SetLicense(null);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<ActionResult> GetTextTemplate(string name, string company, string email)
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;

            var sys = _systemInformationService.GetBasicInformation();
            var systemInfoStr = LocalizedStrings.GetStringFormat("Web_Controller_License_TextTemplate_SystemInfos",
                LocalizedStrings.GetString("Web_Controller_License_TextTemplate_MachineName"),
                sys.MachineName,
                LocalizedStrings.GetString("Web_Controller_License_TextTemplate_OperatingSystem"),
                string.Format("{0} {1} {2}", sys.OperatingSystem, sys.OperatingSystemArchitecture, sys.OperatingSystemVersion),
                LocalizedStrings.GetString("Web_Controller_License_TextTemplate_Language"),
                sys.OperatingSystemLanguage.ToString(),
                LocalizedStrings.GetString("Web_Controller_License_TextTemplate_SystemManufacturer"),
                sys.Manufacturer,
                LocalizedStrings.GetString("Web_Controller_License_TextTemplate_SystemModel"),
                sys.Model,
                LocalizedStrings.GetString("Web_Controller_License_TextTemplate_Processors"),
                sys.Processors != null ? string.Join("|", sys.Processors) : null,
                LocalizedStrings.GetString("Web_Controller_License_TextTemplate_Memory"),
                string.Format("{0} MB", (int)(sys.Memory / 1048576d)),
                LocalizedStrings.GetString("Web_Controller_License_TextTemplate_AvailableOSMemory"),
                string.Format("{0} MB", (int)(sys.OSVisibleMemory / 1024d)));
            if (sys.VideoControllers != null)
            {
                foreach (var vc in sys.VideoControllers)
                    if (!string.IsNullOrEmpty(vc.Resolution))
                        systemInfoStr += string.Format("\r\n{0}: {1}, {2}", LocalizedStrings.GetString("Web_Controller_License_TextTemplate_VideoController"), vc.Name, vc.Resolution);
            }

            var version = Assembly.GetExecutingAssembly().FullName
                .Split(',')
                .Single(_ => _.Contains("Version="))
                .Split('=')
                .Last();

            var productLicense = await LicenseMapper.GetLicense();

            var content = string.Format(LocalizedStrings.GetString("Web_Controller_License_TextTemplate"),
                name,
                company,
                email,
                await LicenseMapper.GetMachineHash(),
                //On n'affiche plus le statut TrialVersion, on Licensed à la place.
                string.Format("{0} - {1}", productLicense.Status == WebLicenseStatus.TrialVersion ? WebLicenseStatus.Licensed : productLicense.Status, LocalizedStrings.GetStringFormat(productLicense.StatusReason, productLicense.StatusReasonParams?.Select(_ => _.ToString()).ToArray())),
                version,
                systemInfoStr
                );

            return Content(content);
        }

        [HttpPost]
        public async Task<ActionResult> GetEmailTemplate(string name, string company, string email)
        {
            if (Request.Cookies.AllKeys.Contains("token"))
                _apiHttpClient.Token = Request.Cookies["token"].Value;

            var sys = _systemInformationService.GetBasicInformation();
            var systemInfoStr = LocalizedStrings.GetStringFormat("Web_Controller_License_TextTemplate_SystemInfos",
                LocalizedStrings.GetString("Web_Controller_License_TextTemplate_MachineName"),
                sys.MachineName,
                LocalizedStrings.GetString("Web_Controller_License_TextTemplate_OperatingSystem"),
                string.Format("{0} {1} {2}", sys.OperatingSystem, sys.OperatingSystemArchitecture, sys.OperatingSystemVersion),
                LocalizedStrings.GetString("Web_Controller_License_TextTemplate_Language"),
                sys.OperatingSystemLanguage.ToString(),
                LocalizedStrings.GetString("Web_Controller_License_TextTemplate_SystemManufacturer"),
                sys.Manufacturer,
                LocalizedStrings.GetString("Web_Controller_License_TextTemplate_SystemModel"),
                sys.Model,
                LocalizedStrings.GetString("Web_Controller_License_TextTemplate_Processors"),
                sys.Processors != null ? string.Join("|", sys.Processors) : null,
                LocalizedStrings.GetString("Web_Controller_License_TextTemplate_Memory"),
                string.Format("{0} MB", (int)(sys.Memory / 1048576d)),
                LocalizedStrings.GetString("Web_Controller_License_TextTemplate_AvailableOSMemory"),
                string.Format("{0} MB", (int)(sys.OSVisibleMemory / 1024d)));
            if (sys.VideoControllers != null)
            {
                foreach (var vc in sys.VideoControllers)
                    if (!string.IsNullOrEmpty(vc.Resolution))
                        systemInfoStr += string.Format("\r\n{0}: {1}, {2}", LocalizedStrings.GetString("Web_Controller_License_TextTemplate_VideoController"), vc.Name, vc.Resolution);
            }

            var version = Assembly.GetExecutingAssembly().FullName
                .Split(',')
                .Single(_ => _.Contains("Version="))
                .Split('=')
                .Last();

            var productLicense = await LicenseMapper.GetLicense();

            var content = string.Format(LocalizedStrings.GetString("Web_Controller_License_TextTemplate"),
                name,
                company,
                email,
                await LicenseMapper.GetMachineHash(),
                //On n'affiche plus le statut TrialVersion, on Licensed à la place.
                string.Format("{0} - {1}", productLicense.Status == WebLicenseStatus.TrialVersion ? WebLicenseStatus.Licensed : productLicense.Status, LocalizedStrings.GetStringFormat(productLicense.StatusReason, productLicense.StatusReasonParams?.Select(_ => _.ToString()).ToArray())),
                version,
                systemInfoStr
                );

            return Content(content);
        }
    }
}