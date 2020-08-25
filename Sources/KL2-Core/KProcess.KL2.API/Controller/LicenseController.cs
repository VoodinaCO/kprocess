using KProcess.KL2.API.App_Start;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Security.Activation;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;

namespace KProcess.KL2.API.Controller
{
    [Authorize]
    [RoutePrefix("Services/LicenseService")]
    public class LicenseController : ApiController
    {
        private readonly ITraceManager _traceManager;
        private readonly IApplicationUsersService _applicationUsersService;

        /// <summary>
        /// UtilitiesController ctors
        /// </summary>
        public LicenseController(ITraceManager traceManager, IApplicationUsersService applicationUsersService)
        {
            _traceManager = traceManager;
            _applicationUsersService = applicationUsersService;
        }

        [HttpPost]
        [Route("GetMachineHash")]
        public IHttpActionResult GetMachineHash()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("Files/PublicKey.xml");
                ProductLicenseManager.Initialize(_traceManager, doc.OuterXml);
                var hash = ProductLicenseManager.Current.GetMachineHash();

                return Ok(hash);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("GetLicense")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetLicense()
        {
            try
            {
                var userIds = await _applicationUsersService.GetAllUserIds();
                if (userIds == null)
                    userIds = Array.Empty<int>();

                XmlDocument doc = new XmlDocument();
                doc.Load("Files/PublicKey.xml");
                ProductLicenseManager.Initialize(_traceManager, doc.OuterXml);
                var license = ProductLicenseManager.Current.LoadWebLicense(ActivationConstants.WebProductName);

                // Clean Userpool
                if (license != null && license.UsersPool != null && license.UsersPool.Any(u => !userIds.Contains(u)))
                {
                    license.UsersPool.RemoveWhere(u => !userIds.Contains(u));
                    ProductLicenseManager.Current.SaveWebLicense(ActivationConstants.WebProductName, license);
                }

                return Ok(license);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("SetLicense")]
        public IHttpActionResult SetLicense([DynamicBody]dynamic param)
        {
            try
            {
                WebProductLicense productLicense = (WebProductLicense)param.productLicense;

                XmlDocument doc = new XmlDocument();
                doc.Load("Files/PublicKey.xml");
                ProductLicenseManager.Initialize(_traceManager, doc.OuterXml);
                ProductLicenseManager.Current.SaveWebLicense(ActivationConstants.WebProductName, productLicense);

                return Ok();
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("ActivateLicense")]
        public IHttpActionResult ActivateLicense([DynamicBody]dynamic param)
        {
            try
            {
                ProductLicenseInfo productLicenseInfo = (ProductLicenseInfo)param.productLicenseInfo;

                XmlDocument doc = new XmlDocument();
                doc.Load("Files/PublicKey.xml");
                ProductLicenseManager.Initialize(_traceManager, doc.OuterXml);
                var license = ProductLicenseManager.Current.ActivateWebProduct(productLicenseInfo);

                return Ok(license);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("GetUserInformation")]
        public IHttpActionResult GetUserInformation()
        {
            try
            {
                var userProvider = new Ksmed.Security.Activation.Providers.UserInformationProvider(true);

                return Ok(userProvider);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("SetUserInformation")]
        public IHttpActionResult SetUserInformation([DynamicBody]dynamic param)
        {
            try
            {
                string name = (string)param.name;
                string company = (string)param.company;
                string email = (string)param.email;

                var userProvider = new Ksmed.Security.Activation.Providers.UserInformationProvider(true);
                if (userProvider.Username != name ||
                    userProvider.Company != company ||
                    userProvider.Email != email)
                {
                    userProvider.SetUserInformation(name, company, email);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }
    }
}
