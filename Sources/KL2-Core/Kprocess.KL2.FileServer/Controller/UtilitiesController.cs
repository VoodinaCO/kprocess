using Kprocess.KL2.FileServer.App_Start;
using KProcess;
using log4net;
using log4net.Appender;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Kprocess.KL2.FileServer.Controller
{
    public class UtilitiesController : ApiController
    {
        private readonly ITraceManager _traceManager;

        /// <summary>
        /// UtilitiesController ctors
        /// </summary>
        public UtilitiesController(ITraceManager traceManager)
        {
            _traceManager = traceManager;
        }

        /// <summary>
        /// Utiliser pour vérifier la connectivité
        /// </summary>
        [HttpGet]
        [Route("Ping")]
        [AllowAnonymous]
        public IHttpActionResult Ping()
        {
            return Ok($"{FileServerService.ServiceName} is running.");
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
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "FileServer_Log.zip" };
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
