using KProcess.KL2.API.Authentication;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Security;
using System;
using System.Data.Entity;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API.Controller
{
    [Authorize]
    [SettingUserContextFilter]
    [RoutePrefix("Services/CloudFilesService")]
    public class CloudFilesController : ApiController
    {
        private readonly ITraceManager _traceManager;
        readonly ISecurityContext _securityContext;

        /// <summary>
        /// CloudFilesController ctors
        /// </summary>
        /// <param name="securityContext"></param>
        public CloudFilesController(ITraceManager traceManager, ISecurityContext securityContext)
        {
            _traceManager = traceManager;
            _securityContext = securityContext;
        }

        /// <summary>
        /// Obtient si un fichier est présent en base.
        /// </summary>
        /// <param name="fileName"></param>
        [HttpGet]
        [Route("Exists/{fileName}")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Exists(string fileName)
        {
            try
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                {
                    string name = Path.GetFileNameWithoutExtension(fileName);
                    if (string.IsNullOrEmpty(name?.Trim()))
                        name = null;
                    string extension = Path.GetExtension(fileName);
                    if (string.IsNullOrEmpty(extension?.Trim()))
                        extension = null;
                    return Ok(await context.CloudFiles.AnyAsync(_ => _.Hash == name && _.Extension == extension));
                }
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Obtient si un fichier est présent en base.
        /// </summary>
        /// <param name="fileName"></param>
        [HttpGet]
        [Route("Get/{fileName}")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Get(string fileName)
        {
            try
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                {
                    string name = Path.GetFileNameWithoutExtension(fileName);
                    if (string.IsNullOrEmpty(name?.Trim()))
                        name = null;
                    string extension = Path.GetExtension(fileName);
                    if (string.IsNullOrEmpty(extension?.Trim()))
                        extension = null;
                    return Ok(await context.CloudFiles.SingleOrDefaultAsync(_ => _.Hash == name && _.Extension == extension));
                }
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }
    }
}
