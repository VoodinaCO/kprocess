using KProcess.KL2.API.App_Start;
using KProcess.KL2.API.Authentication;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API.Controller
{
    [SettingUserContextFilter]
    [RoutePrefix("Services/ValidateService")]
    public class ValidateServiceController : ApiController, IValidateServiceController
    {
        private readonly ITraceManager _traceManager;
        private readonly IValidateService _validateService;
        private readonly ISecurityContext _securityContext;

        /// <summary>
        /// ValidateServiceController ctors
        /// </summary>
        /// <param name="validateService"></param>
        /// <param name="securityContext"></param>
        public ValidateServiceController(ITraceManager traceManager, IValidateService validateService, ISecurityContext securityContext)
        {
            _traceManager = traceManager;
            _validateService = validateService;
            _securityContext = securityContext;
        }

        [HttpPost]
        [Route("GetAcquireData")]
        [Authorize]
        public async Task<IHttpActionResult> GetAcquireData([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                return Ok(await _validateService.GetAcquireData(projectId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("GetBuildData")]
        [Authorize]
        public async Task<IHttpActionResult> GetBuildData([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                return Ok(await _validateService.GetBuildData(projectId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("GetRestitutionData")]
        [Authorize]
        public async Task<IHttpActionResult> GetRestitutionData([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                return Ok(await _validateService.GetRestitutionData(projectId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("GetSimulateData")]
        [Authorize]
        public async Task<IHttpActionResult> GetSimulateData([DynamicBody]dynamic param)
        {
            try
            {
                int projectId = (int)param.projectId;
                return Ok(await _validateService.GetSimulateData(projectId));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("SaveAcquireData")]
        [Authorize]
        public async Task<IHttpActionResult> SaveAcquireData([DynamicBody]dynamic param)
        {
            try
            {
                Scenario[] allScenarios = param.allScenarios();
                Scenario updatedScenario = param.updatedScenario;
                await _validateService.SaveAcquireData(allScenarios, updatedScenario);
                return Ok((allScenarios, updatedScenario));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("SaveBuildScenario")]
        [Authorize]
        public async Task<IHttpActionResult> SaveBuildScenario([DynamicBody]dynamic param)
        {
            try
            {
                Scenario[] allScenarios = param.allScenarios;
                Scenario updatedScenario = param.updatedScenario;
                await _validateService.SaveBuildScenario(allScenarios, updatedScenario);
                return Ok((allScenarios, updatedScenario));
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("SaveSimulateData")]
        [Authorize]
        public async Task<IHttpActionResult> SaveSimulateData([DynamicBody]dynamic param)
        {
            try
            {
                Scenario updatedScenario = param.updatedScenario;
                await _validateService.SaveSimulateData(updatedScenario);
                return Ok(updatedScenario);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }
    }
}
