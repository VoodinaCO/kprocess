using Kprocess.PackIconKprocess;
using KProcess.KL2.JWT;
using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.KL2.WebAdmin.Mapper;
using KProcess.KL2.WebAdmin.Models;
using KProcess.KL2.WebAdmin.Models.Users;
using KProcess.KL2.WebAdmin.Utils;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Activation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace KProcess.KL2.WebAdmin.Controllers
{
    [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Operator, KnownRoles.Technician, KnownRoles.Documentalist)]
    [SettingUserContextFilter]
    public class HomeController : LocalizedController
    {
        readonly ITraceManager _traceManager;
        ILocalizedStrings _localizedStrings;
        IApplicationUsersService _applicationUsersService;

        public HomeController(IApplicationUsersService applicationUsersService, ITraceManager traceManager, ILocalizedStrings localizedStrings,
            ILocalizationManager localizationManager)
            : base(localizationManager)
        {
            _applicationUsersService = applicationUsersService;
            _traceManager = traceManager;
            _localizedStrings = localizedStrings;
        }

        public ActionResult Index(bool partial = false)
        {
            if (partial)
                return PartialView();
            return View();
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

                model.Add(new LanguageViewModel
                {
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
            filePath = System.IO.Path.Combine("Files", "SVG_CountriesFlags", flag.ToString() + ".png");
            return filePath;
        }
        
        public PartialViewResult LanguageChoose()
        {
            var model = new LanguageChoose();
            //model.CurrentLanguageCode = JwtTokenProvider.GetUserModelCurrentLanguage(Request.Cookies["token"].Value);
            model.CurrentLanguageCode = JwtTokenProvider.GetUserModelCurrentLanguage(HttpContext.Request.Cookies["token"].Value);
            return PartialView(model);
        }

        public JsonResult ChangeCurrentLanguage(string languageCode)
        {
            var userModel = JwtTokenProvider.GetUserModel(HttpContext.Request.Cookies["token"].Value);
            userModel.CurrentLanguageCode = languageCode;
            Response.SetAuthorizationCookie(userModel);
            return Json(new { changed = true }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> CheckExpiredLicenseToNavigate(string targetAddress)
        {
            var redirect = await LicenseMapper.CheckLicenseStatus(WebLicenseStatus.OverageOfUsers, WebLicenseStatus.NotFound);
            if (targetAddress != null)
            {
                if (targetAddress.Contains("User") && await LicenseMapper.CheckLicenseStatus() != true)
                    redirect = false;
            }

            return Json(new { redirect = redirect }, JsonRequestBehavior.AllowGet);
        }
    }
}