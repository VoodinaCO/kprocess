using KProcess.KL2.API.App_Start;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Security;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API.Controller
{
    [AllowAnonymous]
    [RoutePrefix("Services/AppResourceService")]
    public class AppResourceServiceController : ApiController, IAppResourceServiceController
    {
        private readonly ITraceManager _traceManager;
        private readonly IAppResourceService _appResourceService;
        private readonly ISecurityContext _securityContext;

        /// <summary>
        /// AppResourceServiceController ctors
        /// </summary>
        /// <param name="appResourceService"></param>
        /// <param name="securityContext"></param>
        public AppResourceServiceController(ITraceManager traceManager, IAppResourceService appResourceService, ISecurityContext securityContext)
        {
            _traceManager = traceManager;
            _appResourceService = appResourceService;
            _securityContext = securityContext;
        }

        /// <summary>
        /// Obtient les langues disponibles pour l'application.
        /// </summary>
        [HttpPost]
        [Route("GetLanguages")]
        public async Task<IHttpActionResult> GetLanguages()
        {
            try
            {
                var result = await _appResourceService.GetLanguages();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient toutes les ressources.
        /// </summary>
        [HttpPost]
        [Route("GetAllResources")]
        public IHttpActionResult GetAllResources()
        {
            try
            {
                return Ok(_appResourceService.GetAllResources());
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient toutes les ressources de la langue spécifiée.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("GetResources")]
        public IHttpActionResult GetResources([DynamicBody]dynamic param)
        {
            try
            {
                string languageCode = param.languageCode;
                var result = _appResourceService.GetResources(languageCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient la ressource de la clé spécifiée dans la langue spécifiée.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("GetResource")]
        public async Task<IHttpActionResult> GetResource([DynamicBody]dynamic param)
        {
            try
            {
                string languageCode = param.languageCode;
                string key = param.key;

                await _appResourceService.GetResource(languageCode, key);
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
