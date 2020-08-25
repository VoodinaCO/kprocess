using KProcess;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Kprocess.KL2.FileServer.Controller
{
    public class FilesServiceController : ApiController
    {
        readonly ISecurityContext _securityContext;
        readonly ITraceManager _traceManager;
        readonly IFileProvider _fileProvider;

        /// <summary>
        /// FilesServiceController ctors
        /// </summary>
        /// <param name="securityContext"></param>
        public FilesServiceController(
            ISecurityContext securityContext,
            ITraceManager traceManager,
            IFileProvider fileProvider
            )
        {
            _securityContext = securityContext;
            _traceManager = traceManager;
            _fileProvider = fileProvider;
        }

        [HttpHead]
        [Route("GetFile/{folder}/{fileName}")]
        [Route("GetFile/{fileName}")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> Head(string fileName, string folder = "Published")
        {
            try
            {
                DirectoryEnum directory = folder == "Uploaded" ? DirectoryEnum.Uploaded : DirectoryEnum.Published;
                if (!await _fileProvider.Exists(fileName, directory))
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                long fileLength = await _fileProvider.GetLengthAsync(fileName, directory);
                ContentInfo contentInfo = GetContentInfoFromRequest(Request, fileLength);
                if (contentInfo == null)
                    return new HttpResponseMessage(HttpStatusCode.RequestedRangeNotSatisfiable);

                var response = new HttpResponseMessage
                {
                    Content = new ByteArrayContent(new byte[0])
                };
                SetResponseHeaders(response, contentInfo, fileLength, fileName);
                return response;
            }
            catch (Exception e)
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString())
                };
                return errorResponse;
            }
        }

        ContentInfo GetContentInfoFromRequest(HttpRequestMessage request, long entityLength)
        {
            var result = new ContentInfo(false, 0, entityLength - 1);
            RangeHeaderValue rangeHeader = request.Headers.Range;
            if (rangeHeader != null && rangeHeader.Ranges.Count != 0)
            {
                //we support only one range
                if (rangeHeader.Ranges.Count > 1)
                {
                    //we probably return other status code here
                    return null;
                }
                RangeItemHeaderValue range = rangeHeader.Ranges.First();
                if (range.From.HasValue && range.From < 0
                    || range.To.HasValue && range.To > entityLength - 1)
                {
                    return null;
                }

                result.From = range.From ?? 0;
                result.To = range.To ?? entityLength - 1;
                result.IsPartial = true;
                result.Length = entityLength;
                if (range.From.HasValue && range.To.HasValue)
                    result.Length = range.To.Value - range.From.Value + 1;
                else if (range.From.HasValue)
                    result.Length = entityLength - range.From.Value + 1;
                else if (range.To.HasValue)
                    result.Length = range.To.Value + 1;
            }

            return result;
        }

        void SetResponseHeaders(HttpResponseMessage response, ContentInfo contentInfo, long fileLength, string fileName)
        {
            response.Headers.AcceptRanges.Add("bytes");
            response.StatusCode = contentInfo.IsPartial ? HttpStatusCode.PartialContent : HttpStatusCode.OK;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentLength = contentInfo.Length;
            if (contentInfo.IsPartial)
                response.Content.Headers.ContentRange = new ContentRangeHeaderValue(contentInfo.From, contentInfo.To, fileLength);
        }

        async Task<HttpResponseMessage> CreateRangeResponse(HttpRequestMessage request, string fileName, string mediaType)
        {
            HttpResponseMessage httpResponse = request.CreateResponse(HttpStatusCode.PartialContent);
            httpResponse.Content = new ByteRangeStreamContent(await _fileProvider.OpenRead(fileName), Request.Headers.Range, MediaTypeHeaderValue.Parse(mediaType), StreamExtensions.BufferSize);
            return httpResponse;
        }

        [HttpGet]
        [Route("GetFile/{folder}/{fileName}")]
        [Route("GetFile/{fileName}")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetFile(string fileName, string folder = "Published")
        {
            try
            {
                DirectoryEnum directory = folder == "Uploaded" ? DirectoryEnum.Uploaded : DirectoryEnum.Published;
                if (!await _fileProvider.Exists(fileName, directory))
                    return new HttpResponseMessage(HttpStatusCode.NotFound);

                long totalSize = await _fileProvider.GetLengthAsync(fileName, directory);
                if (Request.Headers.Range != null && _fileProvider.SupportsRange)
                    return await CreateRangeResponse(Request, fileName, "application/octet-stream");

                ContentInfo contentInfo = GetContentInfoFromRequest(Request, totalSize);
                if (contentInfo == null)
                    return new HttpResponseMessage(HttpStatusCode.RequestedRangeNotSatisfiable);
                Stream stream = new PartialFileStream(await _fileProvider.OpenRead(fileName, directory), contentInfo.From, contentInfo.To);
                HttpResponseMessage httpResponse = new HttpResponseMessage
                {
                    Content = new StreamContent(stream, StreamExtensions.BufferSize)
                };
                SetResponseHeaders(httpResponse, contentInfo, totalSize, fileName);
                return httpResponse;
            }
            catch (Exception e)
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString())
                };
                return errorResponse;
            }
        }

        [HttpGet]
        [Route("Stream/{folder}/{fileName}")]
        [Route("Stream/{fileName}")]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> Stream(string fileName, string folder = "Published")
        {
            try
            {
                DirectoryEnum directory = folder == "Uploaded" ? DirectoryEnum.Uploaded : DirectoryEnum.Published;
                if (!await _fileProvider.Exists(fileName, directory))
                    return new HttpResponseMessage(HttpStatusCode.NotFound);

                long totalSize = await _fileProvider.GetLengthAsync(fileName, directory);
                if (Request.Headers.Range != null)
                    return await CreateRangeResponse(Request, fileName, "application/octet-stream");

                ContentInfo contentInfo = GetContentInfoFromRequest(Request, totalSize);
                if (contentInfo == null)
                    return new HttpResponseMessage(HttpStatusCode.RequestedRangeNotSatisfiable);
                Stream stream = new PartialFileStream(await _fileProvider.OpenRead(fileName, directory), contentInfo.From, contentInfo.To);
                HttpResponseMessage httpResponse = new HttpResponseMessage
                {
                    Content = new StreamContent(stream, StreamExtensions.BufferSize)
                };
                SetResponseHeaders(httpResponse, contentInfo, totalSize, fileName);
                return httpResponse;
            }
            catch (Exception e)
            {
                var errorResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(e.ToString())
                };
                return errorResponse;
            }
        }

        [HttpGet]
        [Route("GetTranscodingProgress/{fileName}")]
        [AllowAnonymous]
        public IHttpActionResult GetTranscodingProgress(string fileName)
        {
            string key = fileName;
            if (SimpleBITSRequestHandler.TranscodingSessions.ContainsKey(key))
                return Ok((double?)SimpleBITSRequestHandler.TranscodingSessions[key].ProgressValue);
            return Ok((double?)null);
        }

        [HttpGet]
        [Route("GetAllTranscodingProgress")]
        [AllowAnonymous]
        public IHttpActionResult GetAllTranscodingProgress()
        {
            return Ok(SimpleBITSRequestHandler.TranscodingSessions.ToDictionary(key => key.Key, value => value.Value.ProgressValue));
        }

        [HttpPost]
        [Route("PostFile/{folder}/{fileName}")]
        [Route("PostFile/{fileName}")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> PostFile(string fileName, string folder = "Uploaded")
        {            
            using (var inStream = await Request.Content.ReadAsStreamAsync())
            {
                var outStream = _fileProvider.Create($"{(folder == "Published" ? _fileProvider.PublishedFilesDirectory : _fileProvider.UploadedFilesDirectory)}/{fileName}");
                try
                {
                    await inStream.CopyToAsync(outStream, StreamExtensions.BufferSize);
                }
                catch { }
                finally
                {
                    outStream.Close();
                }
            }
            return Ok();
        }

        [HttpGet]
        [Route("GetFileSize/{folder}/{fileName}")]
        [Route("GetFileSize/{fileName}")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetFileSize(string fileName, string folder = "Published")
        {
            if (folder == "Published")
                return Ok(await _fileProvider.GetLengthAsync(fileName, DirectoryEnum.Published));
            if (folder == "Uploaded")
                return Ok(await _fileProvider.GetLengthAsync(fileName, DirectoryEnum.Uploaded));
            return Ok(-1);
        }

        [HttpGet]
        [Route("Exists/{folder}/{fileName}")]
        [Route("Exists/{fileName}")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Exists(string fileName, string folder = "Published")
        {
            if (folder == "Published")
                return Ok(await _fileProvider.Exists(fileName, DirectoryEnum.Published));
            if (folder == "Uploaded")
                return Ok(await _fileProvider.Exists(fileName, DirectoryEnum.Uploaded));
            return Ok(false);
        }

        [HttpGet]
        [Route("Delete/{folder}/{fileName}")]
        [Route("Delete/{fileName}")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Delete(string fileName, string folder = "Published")
        {
            if (folder == "Published")
                return Ok(await _fileProvider.Delete(fileName, DirectoryEnum.Published));
            if (folder == "Uploaded")
                return Ok(await _fileProvider.Delete(fileName, DirectoryEnum.Uploaded));
            return NotFound();
        }

        [HttpPost]
        [Route("GetFilesSize/{folder}")]
        [Route("GetFilesSize")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetFilesSize([FromBody]IEnumerable<string> fileNames, string folder = "Published")
        {
            DirectoryEnum? dirEnum = null;
            if (folder == "Published")
                dirEnum = DirectoryEnum.Published;
            if (folder == "Uploaded")
                dirEnum = DirectoryEnum.Uploaded;
            if (dirEnum == null)
                return Ok(-1);
            long totalLength = 0;
            foreach (string fileName in fileNames)
            {
                long length = await _fileProvider.GetLengthAsync(fileName, dirEnum.Value);
                if (length != -1)
                    totalLength += length;
            }

            return Ok(totalLength);
        }

        [HttpGet]
        [Route("GetUploadNewName/{tusId}")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetUploadNewName(string tusId)
        {
            _traceManager.TraceDebug($"Start GetUploadNewName({tusId})");
            long timeoutMs = 5000;
            string tusFileName = $"{tusId}.name";

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!await _fileProvider.Exists(tusFileName, DirectoryEnum.Uploaded))
            {
                await Task.Delay(100);
                if (stopwatch.ElapsedMilliseconds > timeoutMs)
                {
                    _traceManager.TraceError("Delai d'attente dépassé");
                    return BadRequest(new TimeoutException("Delai d'attente dépassé").ToString());
                }
            }

            _traceManager.TraceDebug($"Reading {GetFilePath(_fileProvider.UploadedFilesDirectory, tusFileName)}");
            string finalFileName = _fileProvider.ReadAllText(GetFilePath(_fileProvider.UploadedFilesDirectory, tusFileName));
            _traceManager.TraceDebug($"Deleting {GetFilePath(_fileProvider.UploadedFilesDirectory, tusFileName)}");
            await _fileProvider.Delete(tusFileName, DirectoryEnum.Uploaded);

            return Ok(finalFileName);
        }

        string GetFilePath(string directory, string fileName) =>
            $"{directory.TrimEnd('/', '\\')}{(directory.Contains("/") ? "/" : "\\")}{fileName.TrimStart('/', '\\')}";
    }
}
