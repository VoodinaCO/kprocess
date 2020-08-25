using KProcess.KL2.JWT;
using KProcess.KL2.Languages;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace KProcess.KL2.WebAdmin.Authentication
{
    public class LocalizedController : Controller
    {
        ILocalizationManager _localizedStrings;
        public ILocalizationManager LocalizedStrings
        {
            get => _localizedStrings;
            private set
            {
                _localizedStrings = value;
                ViewBag.LocalizedStrings = _localizedStrings;
            }
        }

        public LocalizedController(ILocalizationManager localizationManager) :base()
        {
            LocalizedStrings = localizationManager;
            LocalizedStrings.CurrentCulture = HttpContext?.Request.Cookies.AllKeys.Contains("token") == true
                ? new CultureInfo(JwtTokenProvider.GetUserModelCurrentLanguage(HttpContext.Request.Cookies["token"].Value))
                : new CultureInfo("fr-FR");
        }
    }
}