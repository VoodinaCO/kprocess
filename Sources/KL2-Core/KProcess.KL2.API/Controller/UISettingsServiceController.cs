using KProcess.KL2.API.App_Start;
using KProcess.KL2.API.Authentication;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API.Controller
{
    [Authorize]
    [SettingUserContextFilter]
    [RoutePrefix("Services/UISettingsService")]
    public class UISettingsServiceController : ApiController, IUISettingsServiceController
    {
        private readonly ITraceManager _traceManager;
        private readonly IUISettingsService _uiSettingsService;
        private readonly ISecurityContext _securityContext;

        /// <summary>
        /// UISettingsServiceController ctors
        /// </summary>
        /// <param name="analyzeService"></param>
        /// <param name="securityContext"></param>
        public UISettingsServiceController(ITraceManager traceManager, IUISettingsService uiSettingsService, ISecurityContext securityContext)
        {
            _traceManager = traceManager;
            _uiSettingsService = uiSettingsService;
            _securityContext = securityContext;
        }

        /// <summary>
        /// Sauve les columns infos
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("SaveColumnsInfo")]
        public async Task<IHttpActionResult> SaveColumnsInfoAsync(dynamic param)
        {
            try
            {
                string uiPartCode = param.uiPartCode;
                IDictionary<string, double> columnInfo = param.columnInfo;
                await _uiSettingsService.SaveColumnsInfo(uiPartCode, columnInfo);
                return Ok();
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient les données pour l'écran Acquérir.
        /// </summary>
        /// <param name="param"></param>
        [HttpPost]
        [Route("GetColumnsInfo")]
        public async Task<IHttpActionResult> GetColumnsInfoAsync(dynamic param)
        {
            try
            {
                string uiPartCode = param.uiPartCode;
                var result = await _uiSettingsService.GetColumnsInfo(uiPartCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }
        
    }
}
