using Kprocess.KL2.FileServer.App_Start;
using Kprocess.KL2.FileServer.Authentication;
using KProcess;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using Murmur;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Kprocess.KL2.FileServer.Controller
{
    [Authorize]
    [RoutePrefix("Services/PublicationService")]
    [SettingUserContextFilter]
    public class PublicationServiceController : ApiController, IPublicationServiceController
    {
        readonly IPublicationService _publicationService;
        readonly ISecurityContext _securityContext;
        readonly ITraceManager _traceManager;

        /// <summary>
        /// PublicationServiceController ctors
        /// </summary>
        /// <param name="publicationService"></param>
        /// <param name="securityContext"></param>
        public PublicationServiceController(
            IPublicationService publicationService,
            ISecurityContext securityContext,
            ITraceManager traceManager
            )
        {
            _publicationService = publicationService;
            _securityContext = securityContext;
            _traceManager = traceManager;
        }

        [HttpPost]
        [Route("GetPublications")]
        [AllowAnonymous] // DEBUG
        public async Task<IHttpActionResult> GetPublications() =>
            Ok(await _publicationService.GetPublications());

        [HttpPost]
        [Route("Publish")]
        // [AllowAnonymous] // DEBUG
        public async Task<IHttpActionResult> Publish([DynamicBody]dynamic param)
        {
            try
            {
                _traceManager.TraceDebug("Demande de publication");
                Publication publication = param.publication;
                bool VideoExportIsEnabled = param.exportVideo;
                bool ExportOnlyKeyTasksVideos = param.exportOnlyKeyTasksVideos;

                return Ok(await _publicationService.Publish(publication, VideoExportIsEnabled, ExportOnlyKeyTasksVideos));
            }
            catch (Exception e)
            {
                _traceManager.TraceError(e, "Une erreur est survenue lors de la publication.");
                throw e;
            }
        }

        [HttpPost]
        [Route("PublishMulti")]
        // [AllowAnonymous] // DEBUG
        public async Task<IHttpActionResult> PublishMulti([DynamicBody]dynamic param)
        {
            try
            {
                _traceManager.TraceDebug("Demande de publication multiple");
                List<Publication> publications = param.publications;
                bool VideoExportIsEnabled = param.exportVideo;
                bool ExportOnlyKeyTasksVideos = param.exportOnlyKeyTasksVideos;

                return Ok(await _publicationService.PublishMulti(publications, VideoExportIsEnabled, ExportOnlyKeyTasksVideos));
            }
            catch (Exception e)
            {
                _traceManager.TraceError(e, "Une erreur est survenue lors de la publication multiple.");
                throw e;
            }
        }

        [HttpPost]
        [Route("FakingPublication")]
        // [AllowAnonymous] // DEBUG
        public async Task<IHttpActionResult> FakingPublication([DynamicBody]dynamic param)
        {
            try
            {
                return Ok(await _publicationService.FakingPublication());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("GetCurrentVersion")]
        public async Task<IHttpActionResult> GetCurrentVersion([DynamicBody]dynamic param)
        {
            int processId = (int)param.processId;
            PublishModeEnum publishMode = (PublishModeEnum)param.publishMode;
            return Ok(await _publicationService.GetCurrentVersion(processId, publishMode));
        }

        [HttpPost]
        [Route("GetFutureVersion")]
        public async Task<IHttpActionResult> GetFutureVersion([DynamicBody]dynamic param)
        {
            int processId = (int)param.processId;
            PublishModeEnum publishMode = (PublishModeEnum)param.publishMode;
            bool isMajor = param.isMajor;
            return Ok(await _publicationService.GetFutureVersion(processId, publishMode, isMajor));
        }

        [HttpPost]
        [Route("CancelPublication")]
        [AllowAnonymous] // DEBUG
        public async Task<IHttpActionResult> CancelPublication([DynamicBody]dynamic param)
        {
            int publicationHistoryId = (int)param.publicationHistoryId;
            return Ok(await _publicationService.CancelPublication(publicationHistoryId));
        }

        [HttpPost]
        [Route("GetProgress")]
        [AllowAnonymous] // DEBUG
        public IHttpActionResult GetProgress([FromBody]dynamic param)
        {
            int publicationHistoryId = (int)param.publicationHistoryId;
            return Ok(_publicationService.GetProgress(publicationHistoryId));
        }

        [HttpPost]
        [Route("GetProgresses")]
        [AllowAnonymous] // DEBUG
        public IHttpActionResult GetProgresses() =>
            Ok(_publicationService.GetProgresses());

        [HttpPost]
        [Route("GetProcessesPublicationStatus")]
        [AllowAnonymous] // DEBUG
        public async Task<IHttpActionResult> GetProcessesPublicationStatus([DynamicBody]dynamic param)
        {
            int[] processIds = param.processIds;
            return Ok(await _publicationService.GetProcessesPublicationStatus(processIds));
        }

        [HttpGet]
        [Route("GetFile/{hash}")]
        [AllowAnonymous] // DEBUG
        public Task<HttpResponseMessage> GetFile(string hash)
        {
            Uri PublishedFilesDirectoryUri = new Uri(_publicationService.PublishedFilesDirectory, UriKind.RelativeOrAbsolute);
            string fileServerPath = PublishedFilesDirectoryUri.IsAbsoluteUri ?
                PublishedFilesDirectoryUri.AbsolutePath :
                Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), PublishedFilesDirectoryUri.OriginalString);
            DirectoryInfo filesDirectory = new DirectoryInfo(fileServerPath);
            FileInfo[] files = filesDirectory.GetFiles($"{hash.ToUpper()}*", SearchOption.TopDirectoryOnly);
            if (files.Length == 1)
            {
                /*byte[] dataBytes = File.ReadAllBytes(files[0].FullName);
                var dataStream = new MemoryStream(dataBytes);
                return new FileResult(dataStream, Request, Path.GetFileName(files[0].FullName));*/
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new PushStreamContent(async (stream, context, transportContext) =>
                    {
                        try
                        {
                            using (var fileStream = File.OpenRead(files[0].FullName))
                            {
                                await fileStream.CopyToAsync(stream, StreamExtensions.BufferSize);
                            }
                        }
                        finally
                        {
                            stream.Close();
                        }
                    }, "application/octet-stream")
                };
                response.Headers.TransferEncodingChunked = true;
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = Path.GetFileName(files[0].FullName)
                };
                return Task.FromResult(response);
            }
            //return BadRequest($"File with hash {hash} not found.");
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent($"File with hash {hash} not found.")
            });
        }

        [HttpPost]
        [Route("GetFileSize")]
        [AllowAnonymous] // DEBUG
        public IHttpActionResult GetFileSize([DynamicBody]dynamic param)
        {
            string fileHash = param.fileHash;

            Uri PublishedFilesDirectoryUri = new Uri(_publicationService.PublishedFilesDirectory, UriKind.RelativeOrAbsolute);
            string fileServerPath = PublishedFilesDirectoryUri.IsAbsoluteUri ?
                PublishedFilesDirectoryUri.AbsolutePath :
                Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), PublishedFilesDirectoryUri.OriginalString);
            DirectoryInfo filesDirectory = new DirectoryInfo(fileServerPath);

            FileInfo[] files = filesDirectory.GetFiles($"{fileHash.ToUpper()}*", SearchOption.TopDirectoryOnly);

            if (files.Length == 1)
                return Ok(files[0].Length);
            return Ok((long)0);
        }

        [HttpPost]
        [Route("GetFilesSize")]
        [AllowAnonymous] // DEBUG
        public IHttpActionResult GetFilesSize([DynamicBody]dynamic param)
        {
            string[] fileHashes = param.fileHashes;

            Uri PublishedFilesDirectoryUri = new Uri(_publicationService.PublishedFilesDirectory, UriKind.RelativeOrAbsolute);
            string fileServerPath = PublishedFilesDirectoryUri.IsAbsoluteUri ?
                PublishedFilesDirectoryUri.AbsolutePath :
                Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), PublishedFilesDirectoryUri.OriginalString);
            DirectoryInfo filesDirectory = new DirectoryInfo(fileServerPath);

            long filesSize = fileHashes.Select(fileHash => filesDirectory.GetFiles($"{fileHash.ToUpper()}*", SearchOption.TopDirectoryOnly))
                .Where(_ => _.Length == 1)
                .Sum(_ => _[0].Length);

            return Ok(filesSize);
        }
                
        [HttpPost]
        [Route("GetPublicationHistories")]
        [AllowAnonymous] // DEBUG
        public async Task<IHttpActionResult> GetPublicationHistories([DynamicBody]dynamic param)
        {
            int pageNumber = (int)param.pageNumber;
            int pageSize = (int)param.pageSize;

            var publications = await _publicationService.GetPublicationHistories(pageNumber, pageSize);
            return Ok(publications);
        }

        [HttpPost]
        [Route("GetPublicationHistory")]
        [AllowAnonymous] // DEBUG
        public async Task<IHttpActionResult> GetPublicationHistory([DynamicBody]dynamic param)
        {
            int publicationHistoryId = (int)param.publicationHistoryId;

            return Ok(await _publicationService.GetPublicationHistory(publicationHistoryId));
        }

        /// <summary>
        /// Extract files from the form
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        void ExtractFilesForm<T>(ICollection<MultipartFileData> fileData, string parameter, Dictionary<T, (byte[] fileHash, string filePath)> model)
        {
            Console.WriteLine($"Extraction des fichiers {parameter}.");
            var files = new Dictionary<int, string>();
            string root = System.Reflection.Assembly.GetEntryAssembly().Location;
            string fileServerPath = Path.Combine(Path.GetDirectoryName(root), "App_Data", "Files");


            // Retrieve videos by filename
            foreach (MultipartFileData file in fileData.Where(u => u.Headers.ContentDisposition.Name.StartsWith($"\"{parameter}_")))
            {
                // Filename of video
                string filename = file.Headers.ContentDisposition.FileName.Replace("\"", "");

                // We use convention as name variable is te ID of the file: "Video_1" or "Resource_1"
                string hashAsString = file.Headers.ContentDisposition.Name.Replace("\"", "").Replace($"{parameter}_", "");
                byte[] hash = Convert.FromBase64String(hashAsString);


                HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
                using (var fileStream = File.OpenRead(file.LocalFileName))
                {
                    byte[] computedHash = murmur128.ComputeHash(fileStream);
                    if (!computedHash.SequenceEqual(hash))
                        throw new Exception($"File {file.Headers.ContentDisposition.FileName} corrupted");
                }

                if (!model.Any(u => u.Value.fileHash.SequenceEqual(hash)))
                    throw new Exception($"File {hash} for parameter {parameter} not found");
                

                var builder = new StringBuilder();
                foreach (Byte hashed in hash)
                    builder.AppendFormat("{0:x2}", hashed);

                string finalPath = Path.Combine(fileServerPath, builder.ToString());
                finalPath = $"{finalPath}{Path.GetExtension(filename)}";

                // Removing or moving from temporary folder
                if (File.Exists(finalPath))
                {
                    try
                    {
                        File.Delete(file.LocalFileName);
                    }
                    catch { }
                }
                else
                    File.Move(file.LocalFileName, finalPath);

                var oldEntry = model.Single(u => u.Value.fileHash.SequenceEqual(hash));
                var newEntry = (oldEntry.Value.fileHash, finalPath);
                int index = model.IndexOf(oldEntry);
                model[oldEntry.Key] = newEntry;
            }
        }

        public class FileResult : IHttpActionResult
        {
            readonly MemoryStream _data;
            readonly string _fileName;
            readonly HttpRequestMessage _requestMessage;

            public FileResult(MemoryStream data, HttpRequestMessage request, string filename)
            {
                _data = data;
                _fileName = filename;
                _requestMessage = request;
            }

            public Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
            {
                var responseMessage = _requestMessage.CreateResponse(HttpStatusCode.OK);
                responseMessage.Content = new StreamContent(_data);
                responseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = _fileName
                };
                responseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                return Task.FromResult(responseMessage);
            }
        }
    }
}
