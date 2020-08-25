using KProcess.KL2.API.App_Start;
using KProcess.KL2.JWT;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security.Business;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Web.Http;
using log4net;
using log4net.Appender;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http.Headers;

namespace KProcess.KL2.API.Controller
{
    public class UtilitiesController : ApiController
    {
        private readonly ITraceManager _traceManager;
        readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// UtilitiesController ctors
        /// </summary>
        public UtilitiesController(ITraceManager traceManager, IAuthenticationService authenticationService)
        {
            _traceManager = traceManager;
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Utiliser pour vérifier la connectivité
        /// </summary>
        [HttpGet]
        [Route("Ping")]
        [AllowAnonymous]
        public IHttpActionResult Ping()
        {
            try
            {
                return Ok($"{ApiService.ServiceName} is running.");
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Token generation
        /// </summary>
        [HttpPost]
        [Route("Token")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> Token()
        {
            var username = Request.Headers.Contains("username") ? Request.Headers.GetValues("username").FirstOrDefault() : null;
            var password = Request.Headers.Contains("password") ? Request.Headers.GetValues("password").FirstOrDefault() : null;
            var language = Request.Headers.Contains("language") ? Request.Headers.GetValues("language").FirstOrDefault() : null;
            User user = await _authenticationService.GetUser(username);
            var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, false);
            if (user == null)
                return response;
            using (SHA1 sha = new SHA1CryptoServiceProvider())
            {
                byte[] passwordHash = sha.ComputeHash(Encoding.Default.GetBytes(password));
                if (await _authenticationService.IsUserValid(username, passwordHash))
                {
                    response = Request.CreateResponse(System.Net.HttpStatusCode.OK, false);
                    var requestSource = Request.Headers.Contains("RequestSource") ? Request.Headers.GetValues("RequestSource") : null;
                    var audience = JwtTokenProvider.DesktopAudience;
                    if (requestSource.Contains("App : KL²"))
                        audience = JwtTokenProvider.DesktopAudience;
                    else if (requestSource.Contains("App : Web"))
                        audience = JwtTokenProvider.WebAudience;
                    else
                        audience = JwtTokenProvider.TabletAudience;
                    user.CurrentLanguageCode = language ?? user.DefaultLanguageCode;
                    response.Headers.Add("token", JwtTokenProvider.GenerateAccessToken(audience, user.ToUserModel()));
                }
            }
            return response;
        }

        /// <summary>
        /// Token regeneration
        /// </summary>
        [HttpPost]
        [Route("RefreshToken")]
        [AllowAnonymous]
        public HttpResponseMessage RefreshToken()
        {
            var oldToken = Request.Headers.Contains("token") ? Request.Headers.GetValues("token").FirstOrDefault() : null;
            var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, false);
            if (string.IsNullOrEmpty(oldToken))
                return response;

            response = Request.CreateResponse(System.Net.HttpStatusCode.OK, false);
            var requestSource = Request.Headers.Contains("RequestSource") ? Request.Headers.GetValues("RequestSource") : null;
            var audience = JwtTokenProvider.DesktopAudience;
            if (requestSource.Contains("App : KL²"))
                audience = JwtTokenProvider.DesktopAudience;
            else if (requestSource.Contains("App : Web"))
                audience = JwtTokenProvider.WebAudience;
            else
                audience = JwtTokenProvider.TabletAudience;
            response.Headers.Add("token", JwtTokenProvider.RegenerateAccessToken(audience, oldToken));

            return response;
        }

        private (string fileName, byte[] data) GetLogInternal()
        {
            var logFileLocation = LogManager.GetRepository().GetAppenders().OfType<FileAppender>().FirstOrDefault()?.File;

            var tmpFileName = Path.GetTempFileName();
            System.IO.File.Copy(logFileLocation, tmpFileName, true);
            var fileBytes = System.IO.File.ReadAllBytes(tmpFileName);
            System.IO.File.Delete(tmpFileName);

            return (Path.GetFileName(logFileLocation), fileBytes);
        }

        /// <summary>
        /// Get log
        /// </summary>
        [HttpGet]
        [Route("GetLog")]
        public HttpResponseMessage GetLog()
        {
            try
            {
                using (var outStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                    {
                        var (fileName, fileBytes) = GetLogInternal();
                        var fileInArchive = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                        using (var entryStream = fileInArchive.Open())
                        {
                            using (var fileToCompressStream = new MemoryStream(fileBytes))
                            {
                                fileToCompressStream.CopyTo(entryStream);
                            }
                        }
                    }
                    var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(outStream.ToArray()) };
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "API_Log.zip" };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    return result;
                }
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
