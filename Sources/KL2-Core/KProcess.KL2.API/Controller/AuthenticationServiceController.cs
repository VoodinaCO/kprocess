using KProcess.KL2.API.App_Start;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Security;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Business;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API.Controller
{
    [AllowAnonymous]
    [RoutePrefix("Services/AuthenticationService")]
    public class AuthenticationServiceController : ApiController, IAuthenticationServiceController
    {
        private readonly ITraceManager _traceManager;
        private readonly IAuthenticationService _authenticationService;
        private readonly ISecurityContext _securityContext;
        private readonly ILocalizationManager _localizationManager;

        /// <summary>
        /// AuthenticationServiceController ctors
        /// </summary>
        /// <param name="authenticationService"></param>
        /// <param name="securityContext"></param>
        public AuthenticationServiceController(ITraceManager traceManager, IAuthenticationService authenticationService, ISecurityContext securityContext, ILocalizationManager localizationManager)
        {
            _traceManager = traceManager;
            _authenticationService = authenticationService;
            _securityContext = securityContext;
            _localizationManager = localizationManager;
        }

        /// <summary>
        /// Obtient l'utilisateur associé au login spécifié.
        /// </summary>
        /// <param name="param">Le login de l'utilisateur (username)</param>
        [HttpPost]
        [Route("GetUser")]
        public async Task<IHttpActionResult> GetUser([DynamicBody]dynamic param)
        {
            try
            {
                string username = param.username;
                string languageCode = param.language;
                return Ok(await _authenticationService.GetUser(username, languageCode));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Indique si le couple login / mot de passe est valide.
        /// </summary>
        /// <param name="param">Le login de l'utilisateur (username) et son mot de passe (password).</param>
        /// <returns><c>true</c> si le couple est valide.</returns>
        [HttpPost]
        [Route("IsUserValid")]
        public async Task<IHttpActionResult> IsUserValid([DynamicBody]dynamic param)
        {
            try
            {
                string username = param.username;
                byte[] password = param.password;
                return Ok(await _authenticationService.IsUserValid(username, password));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Sauvegarde l'utilisateur.
        /// </summary>
        /// <param name="param">L'utilisateur (user).</param>
        [HttpPost]
        [Route("SaveUser")]
        [Authorize]
        public async Task<IHttpActionResult> SaveUser([DynamicBody]dynamic param)
        {
            try
            {
                User user = param.user;
                User contextUser = await _authenticationService.GetUser(User.Identity.Name);
                SecurityUser securityUser = new SecurityUser(contextUser);
                await _authenticationService.SaveUser(user, securityUser);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }
    }
}
