using KProcess.KL2.API.App_Start;
using KProcess.KL2.API.Authentication;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Security;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API.Controller
{
    [SettingUserContextFilter]
    [RoutePrefix("Services/SharedDatabaseService")]
    public class SharedDatabaseServiceController : ApiController, ISharedDatabaseServiceController
    {
        private readonly ITraceManager _traceManager;
        private readonly ISharedDatabaseService _sharedDatabaseService;
        private readonly ISecurityContext _securityContext;

        /// <summary>
        /// ValidateServiceController ctors
        /// </summary>
        /// <param name="sharedDatabaseService"></param>
        /// <param name="securityContext"></param>
        public SharedDatabaseServiceController(ITraceManager traceManager, ISharedDatabaseService sharedDatabaseService, ISecurityContext securityContext)
        {
            _traceManager = traceManager;
            _sharedDatabaseService = sharedDatabaseService;
            _securityContext = securityContext;
        }

        [HttpPost]
        [Route("IsLocked")]
        [Authorize]
        public async Task<IHttpActionResult> IsLocked([DynamicBody]dynamic param)
        {
            try
            {
                string username = param.username;
                return Ok(await _sharedDatabaseService.IsLocked(username));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("ReleaseLock")]
        [Authorize]
        public async Task<IHttpActionResult> ReleaseLock([DynamicBody]dynamic param)
        {
            try
            {
                string username = param.username;
                await _sharedDatabaseService.ReleaseLock(username);
                return Ok();
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("UpdateLock")]
        [Authorize]
        public async Task<IHttpActionResult> UpdateLock([DynamicBody]dynamic param)
        {
            try
            {
                string username = param.username;
                await _sharedDatabaseService.UpdateLock(username);
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
