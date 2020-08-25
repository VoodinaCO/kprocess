using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.Ksmed.Security;
using System.Web.Mvc;

namespace KProcess.KL2.WebAdmin.Controllers
{
    [CustomAuthorize(KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Operator, KnownRoles.Technician, KnownRoles.Documentalist)]
    [SettingUserContextFilter]
    public class EmptyController : LocalizedController
    {
        public EmptyController(ILocalizationManager localizationManager)
            :base(localizationManager)
        { }

        public ActionResult Index(bool partial = false)
        {
            if (partial)
                return PartialView();
            return View();
        }
    }
}