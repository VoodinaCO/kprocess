using Kprocess.KL2.FileTransfer;
using KProcess;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Models;
using Murmur;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kprocess.KL2.FileServer
{
    public class SimpleBITSRequestHandler : DelegatingHandler
    {
        //static readonly string ProtocolVersion = "HTTP/1.1";
        static readonly string BaseDir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "App_Data", "Files");
        static readonly Guid[] SupportedProtocols = { Guid.Parse("{7df0354d-249b-430f-820d-3d2a9bef4931}") };  // The only existing protocol version to date
        static readonly long FragmentSizeLimit = long.MaxValue;//81920; // 80KO
        static readonly string transcodeExt = ".mp4";

        public static readonly Dictionary<string, BITSUploadSession> Sessions = new Dictionary<string, BITSUploadSession>();
        public static readonly Dictionary<string, TaskProgressObservable<bool>> TranscodingSessions = new Dictionary<string, TaskProgressObservable<bool>>();

        readonly ITraceManager _traceManager;
        readonly IFileProvider _fileProvider;

        public SimpleBITSRequestHandler(IFileProvider fileProvider, ITraceManager traceManager)
        {
            _traceManager = traceManager;
            _fileProvider = fileProvider;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpMethod bitsPostMethod = new HttpMethod("BITS_POST");
            if (request.Method == bitsPostMethod)
                return BitsPost(request, cancellationToken);
            return base.SendAsync(request, cancellationToken);
        }

        public Task<HttpResponseMessage> BitsPost(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            string bits_packet_type = request.Headers.GetValues(Bits.K_BITS_PACKET_TYPE).First().ToLower();
            Exception exception = null;
            HttpStatusCode status_code = HttpStatusCode.OK;
            try
            {
                if (bits_packet_type == "ping")
                    return HandlePing(request, cancellationToken);
                if (bits_packet_type == "create-session")
                    return HandleCreateSession(request, cancellationToken);
                if (bits_packet_type == "cancel-session")
                    return HandleCancelSession(request, cancellationToken);
                if (bits_packet_type == "close-session")
                    return HandleCloseSession(request, cancellationToken);
                if (bits_packet_type == "fragment")
                    return HandleFragment(request, cancellationToken);
                throw new NotSupportedException($"{bits_packet_type}is an unknown BITS-Packet-Type");
            }
            catch (NotSupportedException e)
            {
                exception = e;
                // case an Unknown BITS-Packet-Type value was received by the server
                status_code = HttpStatusCode.BadRequest;
                response.Headers.Add(Bits.K_BITS_ERROR_CODE, Bits.BITSServerHResult.E_INVALIDARG.ToString());
            }
            catch (Bits.ServerInternalError e)
            {
                exception = e;
                status_code = HttpStatusCode.InternalServerError;
                response.Headers.Add(Bits.K_BITS_ERROR_CODE, Bits.BITSServerHResult.ERROR_CODE_GENERIC.ToString());
            }

            response.Headers.Add(Bits.K_BITS_ERROR_CONTEXT, Bits.BITSServerHResult.BG_ERROR_CONTEXT_REMOTE_FILE.ToString());
            _traceManager.TraceError($"Internal BITS Server Error. context:{response.Headers.GetValues(Bits.K_BITS_ERROR_CONTEXT).First()}, code:{response.Headers.GetValues(Bits.K_BITS_ERROR_CODE).First()}", exception);
            _traceManager.TraceError($"{exception.Message}\n{exception.StackTrace}");
            return Task.FromResult(SendResponse(response, status_code));
        }

        /// <summary>
        /// Sends server response w/ headers and status code
        /// </summary>
        /// <param name="response"></param>
        /// <param name="status_code"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        HttpResponseMessage SendResponse(HttpResponseMessage response, HttpStatusCode status_code = HttpStatusCode.OK, byte[] data = null)
        {
            response.StatusCode = status_code;
            response.Content = new ByteArrayContent(data ?? (new byte[0]));
            return response;
        }

        /// <summary>
        /// Releases server resources for a session termination caused by either:
        /// Close-Session or Cancel-Session
        /// </summary>
        async Task<HttpResponseMessage> ReleaseResources(HttpRequestMessage request, CancellationToken cancellationToken, bool deleteFile = false)
        {
            HttpStatusCode status_code = HttpStatusCode.OK;
            HttpResponseMessage response = request.CreateResponse();
            response.Headers.Add(Bits.K_BITS_PACKET_TYPE, Bits.V_ACK);
            //response.Headers.Add(Bits.K_CONTENT_LENGTH, "0");

            try
            {
                string session_id = request.Headers.GetValues(Bits.K_BITS_SESSION_ID)?.First();
                if (string.IsNullOrEmpty(session_id))
                    throw new NotSupportedException();
                _traceManager.TraceDebug($"Closing BITS-Session-Id: {session_id}");

                if (Sessions.ContainsKey(session_id))
                    await Sessions[session_id].Close(deleteFile);
                else
                {
                    FileInfo fileInfo = new FileInfo(Path.Combine(BITSUploadSession.VideoBufferDirectory, session_id));
                    if (fileInfo.Exists)
                        fileInfo.Delete();
                    else
                        await _fileProvider.Delete(session_id, DirectoryEnum.Uploaded);
                }
                if (Sessions.ContainsKey(session_id) && Sessions[session_id].Move)
                {
                    if (Sessions[session_id].IsTranscoding)
                    {
                        Progress<double> progress = new Progress<double>();
                        CancellationToken ct = CancellationToken.None;
                        var transcodeSession = new TaskProgressObservable<bool>
                        {
                            Task = TranscodeFileToFileProvider(Sessions[session_id], progress, ct),
                            Progress = progress,
                            CancellationToken = ct
                        };
                        TranscodingSessions.Add($"TRANSCODED_{Path.GetFileName(Sessions[session_id].FileName)}", transcodeSession);
                    }
                    else
                        await _fileProvider.Complete(Sessions[session_id].FileName);
                }
                if (Sessions.ContainsKey(session_id))
                    Sessions.Remove(session_id);

                status_code = HttpStatusCode.OK;
            }
            catch (NotSupportedException)
            {
                return SendResponse(response, HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                throw new Bits.ServerInternalError(e);
            }

            return SendResponse(response, status_code);
        }

        MediaInfo GetMediaInfo(FileInfo file)
        {
            MediaInfo result = null;
            string json = "";
            string exeFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ffme", "ffprobe.exe");
            using (var process = new Process())
            {
                process.StartInfo.FileName = exeFilePath;
                process.StartInfo.Arguments = $"-v quiet -print_format json -show_format -show_streams \"{file.FullName}\"";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;

                process.OutputDataReceived += (s, e) =>
                {
                    json += e.Data;
                };

                try
                {
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }
                catch (Exception e)
                {
                    throw new Exception("An issue has occured during video packaging. It seems that some components are missing.", e);
                }
            }
            JObject jsonRoot = JObject.Parse(json);
            if (jsonRoot.ContainsKey("streams"))
            {
                result = new MediaInfo();
                var streams = jsonRoot["streams"];
                var audio = streams.FirstOrDefault(_ => (string)_.SelectToken("codec_type") == "audio");
                var video = streams.FirstOrDefault(_ => (string)_.SelectToken("codec_type") == "video");
                if (audio != null)
                {
                    result.HasAudio = true;
                    if (int.TryParse((string)audio.SelectToken("bit_rate"), out int audioBitrate))
                        result.AudioBitrate = audioBitrate;
                    result.AudioCodec = (string)audio.SelectToken("codec_name");
                    if (!string.IsNullOrEmpty((string)audio.SelectToken("duration")))
                    {
                        if (double.TryParse((string)audio.SelectToken("duration"), NumberStyles.Any, CultureInfo.InvariantCulture, out double durationValue))
                            result.Duration = TimeSpan.FromSeconds(durationValue);
                        else
                            _traceManager.TraceWarning($"ERROR : Can't parse audio duration from '{(string)audio.SelectToken("duration")}'");
                    }
                }
                if (video != null)
                {
                    result.HasVideo = true;
                    if (int.TryParse((string)video.SelectToken("bit_rate"), out int videoBitrate))
                        result.VideoBitrate = videoBitrate;
                    result.VideoCodec = (string)video.SelectToken("codec_name");
                    if (!string.IsNullOrEmpty((string)video.SelectToken("duration")))
                    {
                        if (double.TryParse((string)video.SelectToken("duration"), NumberStyles.Any, CultureInfo.InvariantCulture, out double durationValue))
                            result.Duration = TimeSpan.FromSeconds(durationValue);
                        else
                            _traceManager.TraceWarning($"ERROR : Can't parse video duration from '{(string)video.SelectToken("duration")}'");
                    }
                    var rotate = (string)video["tags"]?.SelectToken("rotate");
                    double? NullableRotateValue = null;
                    if (!string.IsNullOrEmpty(rotate))
                    {
                        if (double.TryParse(rotate, out double rotateValue))
                            NullableRotateValue = rotateValue;
                        else
                            _traceManager.TraceWarning($"ERROR : Can't parse double from '{rotate}'");
                    }
                    bool invertWidthHeight = NullableRotateValue.HasValue && NullableRotateValue != 0 && NullableRotateValue.Value % 90 == 0;
                    result.Width = invertWidthHeight ? int.Parse((string)video.SelectToken("height")) : int.Parse((string)video.SelectToken("width"));
                    result.Height = invertWidthHeight ? int.Parse((string)video.SelectToken("width")) : int.Parse((string)video.SelectToken("height"));
                }
            }

            return result;
        }

        Task<bool> TranscodeFileToFileProvider(BITSUploadSession session, IProgress<double> progress, CancellationToken ct) =>
            Task.Run(async () =>
            {
                string exeFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ffme", "ffmpeg.exe");
                int maxAudioBitRate = 96000; // 96k
                int maxVideoBitRate = 1500000; // 1500k
                int maxResolution = 720;
                TimeSpan videoDuration = new TimeSpan(0);
                TimeSpan progressDuration = new TimeSpan(0);

                var fileInfo = new FileInfo(Path.Combine(BITSUploadSession.VideoBufferDirectory, Path.GetFileName(session.FileName)));
                var mediaInfo = GetMediaInfo(fileInfo);
                var processArgumentsBuilder = new StringBuilder($"-hide_banner -y -nostdin -i \"{fileInfo.FullName}\"");
                if (mediaInfo.HasAudio)
                    processArgumentsBuilder.Append($" -acodec mp3 -b:a {Math.Min(mediaInfo.AudioBitrate ?? maxAudioBitRate, maxAudioBitRate)}");
                if (mediaInfo.HasVideo)
                {
                    if (Math.Min(mediaInfo.Width, mediaInfo.Height) > maxResolution) // Needs downscaling
                        processArgumentsBuilder.Append($" -vf \"scale={(mediaInfo.Width > mediaInfo.Height ? $"-2:{maxResolution}" : $"{maxResolution}:-2")}\"");
                    else if (mediaInfo.Width % 2 != 0) // Needs rescaling (MP4 resolutions must be multiple of 2)
                    {
                        mediaInfo.Width++;
                        processArgumentsBuilder.Append($" -vf \"scale={mediaInfo.Width}:-2\"");
                    }
                    else if (mediaInfo.Height % 2 != 0) // Needs rescaling (MP4 resolutions must be multiple of 2)
                    {
                        mediaInfo.Height++;
                        processArgumentsBuilder.Append($" -vf \"scale=-2:{mediaInfo.Height}\"");
                    }
                    processArgumentsBuilder.Append($" -vcodec libx264 -preset fast -b:v {Math.Min(mediaInfo.VideoBitrate ?? maxVideoBitRate, maxVideoBitRate)}");
                }
                processArgumentsBuilder.Append($" -f mp4 -movflags faststart \"{Path.Combine(BITSUploadSession.VideoBufferDirectory, $"TRANSCODED_{Path.GetFileNameWithoutExtension(Path.GetFileName(session.FileName))}{transcodeExt}")}\"");

                using (var process = new Process())
                {
                    process.StartInfo.FileName = exeFilePath;
                    process.StartInfo.Arguments = processArgumentsBuilder.ToString();
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.RedirectStandardOutput = true;

                    process.OutputDataReceived += (s, e) => _traceManager.TraceDebug(e.Data);
                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (e.Data != null && e.Data.Trim().StartsWith("Duration:"))
                        {
                            string duration = e.Data.Trim().Split(',').First().Split(' ')[1].Trim();
                            TimeSpan.TryParse(duration, out videoDuration);
                        }
                        if (e.Data != null && e.Data.Trim().StartsWith("frame="))
                        {
                            string duration = e.Data.Trim().Split(' ').Single(_ => _.StartsWith("time=")).Split('=')[1];
                            TimeSpan.TryParse(duration, out progressDuration);
                            double progressValue = videoDuration.Ticks == 0 ? 0 : Math.Round(progressDuration.TotalMilliseconds * 100 / videoDuration.TotalMilliseconds);
                            progress?.Report(progressValue);
                        }
                        _traceManager.TraceDebug(e.Data);
                    };

                    try
                    {
                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        process.WaitForExit();
                        if (process.ExitCode != 0)
                           throw new Exception();
                    }
                    catch (Win32Exception e)
                    {
                        _traceManager.TraceError(e, "An issue has occured during video packaging. It seems that some components are missing.");
                        TranscodingSessions.Remove($"TRANSCODED_{Path.GetFileName(session.FileName)}");
                        return false;
                    }
                    catch (Exception e)
                    {
                        _traceManager.TraceError(e, "Une erreur non prévue s'est produite lors du split de la video");
                        TranscodingSessions.Remove($"TRANSCODED_{Path.GetFileName(session.FileName)}");
                        return false;
                    }
                    finally
                    {
                        try
                        {
                            File.Delete(Path.Combine(BITSUploadSession.VideoBufferDirectory, Path.GetFileName(session.FileName)));
                        }
                        catch (Exception ex)
                        {
                            _traceManager.TraceDebug(ex, $"Erreur lors de la suppression du fichier {Path.Combine(BITSUploadSession.VideoBufferDirectory, Path.GetFileName(session.FileName))}");
                        }
                    }
                }

                // Compute file hash
                string transcodeVideoHash;
                HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
                using (var fileStream = File.OpenRead(Path.Combine(BITSUploadSession.VideoBufferDirectory, $"TRANSCODED_{Path.GetFileNameWithoutExtension(session.FileName)}{transcodeExt}")))
                {
                    transcodeVideoHash = murmur128.ComputeHash(fileStream).ToHashString();
                }

                // Rename transcoded file
                File.Move(Path.Combine(BITSUploadSession.VideoBufferDirectory, $"TRANSCODED_{Path.GetFileNameWithoutExtension(session.FileName)}{transcodeExt}"), Path.Combine(BITSUploadSession.VideoBufferDirectory, $"{transcodeVideoHash}{transcodeExt}"));

                // Move transcoded file to FileProvider
                await _fileProvider.Upload(new FileInfo(Path.Combine(BITSUploadSession.VideoBufferDirectory, $"{transcodeVideoHash}{transcodeExt}")), $"{transcodeVideoHash}{transcodeExt}", DirectoryEnum.Uploaded);
                await _fileProvider.Complete($"{transcodeVideoHash}{transcodeExt}");
                File.Delete(Path.Combine(BITSUploadSession.VideoBufferDirectory, $"{transcodeVideoHash}{transcodeExt}"));

                // Update video infos in database
                using (var dbHelpers = new DbHelpers())
                {
                    await dbHelpers.ExecuteCommandFormatAsync<int>($"UPDATE [dbo].[Video] SET [Hash] = '{transcodeVideoHash}', [Extension] = '{transcodeExt}', [OnServer] = 1 WHERE [Hash] = '{Path.GetFileNameWithoutExtension(session.FileName)}';");
                }

                TranscodingSessions.Remove($"TRANSCODED_{Path.GetFileName(session.FileName)}");
                return true;
            });

        string GetCurrentSessionId(HttpRequestMessage request)
        {
            /*string cpuInfo = null;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (string.IsNullOrEmpty(cpuInfo))
                {
                    //Get only the first CPU's ID
                    cpuInfo = mo.Properties["processorID"].Value.ToString();
                    break;
                }
            }*/
            // TODO : On doit récupérer l'identité de l'appelant
            /*string clientId = string.Empty;
            if (request.Headers.Contains("MachineId"))
                clientId = request.Headers.GetValues("MachineId").First();
            return $"{clientId}{request.RequestUri.AbsolutePath}";*/
            return $"{{{Guid.NewGuid()}}}";
        }

        /// <summary>
        /// Handles Ping packet from client
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> HandlePing(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _traceManager.TraceDebug("PING RECEIVED");

            HttpResponseMessage response = request.CreateResponse();
            response.Headers.Add(Bits.K_BITS_PACKET_TYPE, Bits.V_ACK);
            response.Headers.Add(Bits.K_BITS_ERROR_CODE, "1");
            response.Headers.Add(Bits.K_BITS_ERROR_CONTEXT, "");
            //response.Headers.Add(Bits.K_CONTENT_LENGTH, "0");

            return Task.FromResult(SendResponse(response));
        }

        /// <summary>
        /// Handles Create-Session packet from client. Creates the UploadSession.
        /// The unique ID that identifies a session in this server is a hash of the client's address and requested path.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> HandleCreateSession(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _traceManager.TraceDebug("CREATE-SESSION RECEIVED");

            Exception exception = null;
            HttpStatusCode status_code = HttpStatusCode.OK;
            Guid client_supported_protocols = Guid.Empty;
            HttpResponseMessage response = request.CreateResponse();
            response.Headers.Add(Bits.K_BITS_PACKET_TYPE, Bits.V_ACK);
            //response.Headers.Add(Bits.K_CONTENT_LENGTH, "0");

            try
            {
                // check if server's protocol version is supported in client
                Guid.TryParse(request.Headers.GetValues(Bits.K_BITS_SUPPORTED_PROTOCOLS).First(), out client_supported_protocols);

                if (SupportedProtocols.Contains(client_supported_protocols)) // case mutual supported protocol is found
                {
                    response.Headers.Add(Bits.K_BITS_PROTOCOL, $"{{{client_supported_protocols}}}");

                    string session_id = GetCurrentSessionId(request);
                    _traceManager.TraceDebug($"Creating BITS-Session-Id:{session_id}");
                    if (Sessions.ContainsKey(session_id))
                    {
                        await Sessions[session_id].Close(true);
                        Sessions[session_id] = await BITSUploadSession.Create(_fileProvider, request.RequestUri.AbsolutePath, FragmentSizeLimit);
                    }
                    else
                    {
                        string existingSessionId = Sessions.Where(_ => _.Value.FileName == Path.GetFileName(request.RequestUri.AbsolutePath)).Select(_ => _.Key).FirstOrDefault();
                        while (!string.IsNullOrEmpty(existingSessionId) && Sessions.ContainsKey(existingSessionId))
                            await Task.Delay(TimeSpan.FromMilliseconds(10));
                        Sessions.Add(session_id, await BITSUploadSession.Create(_fileProvider, request.RequestUri.AbsolutePath, FragmentSizeLimit));
                    }

                    response.Headers.Add(Bits.K_BITS_SESSION_ID, session_id);
                    status_code = Sessions[session_id].GetLastStatusCode();
                    if (status_code == HttpStatusCode.Forbidden)
                        throw new Bits.UploadAccessDenied();
                }
                else // case no mutual supported protocol is found
                    throw new Bits.ClientProtocolNotSupported(client_supported_protocols);
            }
            catch (NotSupportedException)
            {
                return SendResponse(response, HttpStatusCode.BadRequest);
            }
            catch (Bits.ClientProtocolNotSupported e)
            {
                exception = e;
                response.Headers.Add(Bits.K_BITS_ERROR_CODE, Bits.BITSServerHResult.E_INVALIDARG.ToString());
                response.Headers.Add(Bits.K_BITS_ERROR_CONTEXT, Bits.BITSServerHResult.BG_ERROR_CONTEXT_REMOTE_FILE.ToString());
                _traceManager.TraceError($"ERROR creating new session - protocol mismatch (REQUEST_PROTOCOL). context:{response.Headers.GetValues(Bits.K_BITS_ERROR_CONTEXT).First()}, code:{response.Headers.GetValues(Bits.K_BITS_ERROR_CODE).First()}", e);
            }
            catch (Bits.UploadAccessDenied e)
            {
                exception = e;
                response.Headers.Add(Bits.K_BITS_ERROR_CODE, Bits.BITSServerHResult.E_ACCESSDENIED.ToString());
                response.Headers.Add(Bits.K_BITS_ERROR_CONTEXT, Bits.BITSServerHResult.BG_ERROR_CONTEXT_REMOTE_FILE.ToString());
                _traceManager.TraceError($"ERROR creating new session - Access Denied. context:{response.Headers.GetValues(Bits.K_BITS_ERROR_CONTEXT).First()}, code:{response.Headers.GetValues(Bits.K_BITS_ERROR_CODE).First()}", e);
            }
            catch (Exception e)
            {
                throw new Bits.ServerInternalError(e);
            }

            if (status_code == HttpStatusCode.OK || status_code == HttpStatusCode.Created)
                response.Headers.Add(Bits.K_ACCEPT_ENCODING, "identity");

            return SendResponse(response, status_code);
        }

        /// <summary>
        /// Cancel the upload session
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> HandleCancelSession(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _traceManager.TraceDebug("CANCEL-SESSION RECEIVED");
            return ReleaseResources(request, cancellationToken, true);
        }

        /// <summary>
        /// Close the upload session
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> HandleCloseSession(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _traceManager.TraceDebug("CLOSE-SESSION RECEIVED");
            return ReleaseResources(request, cancellationToken);
        }

        /// <summary>
        /// Handles a new Fragment packet from the client, adding it to the relevant upload session
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> HandleFragment(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _traceManager.TraceDebug("FRAGMENT RECEIVED");
            HttpStatusCode status_code = HttpStatusCode.OK;
            HttpResponseMessage response = request.CreateResponse();
            response.Headers.Add(Bits.K_BITS_PACKET_TYPE, Bits.V_ACK);
            //response.Headers.Add(Bits.K_CONTENT_LENGTH, "0");

            string session_id = null;
            long total_length = 0;
            long range_start = 0;
            long range_end = 0;
            try
            {
                // obtain client headers
                session_id = request.Headers.GetValues(Bits.K_BITS_SESSION_ID)?.First();
                if (string.IsNullOrEmpty(session_id) || !Sessions.ContainsKey(session_id))
                    throw new NotSupportedException("Invalid session_id");
                long content_length = request.Content.Headers.ContentLength.Value;
                string content_name = request.Headers.Contains(Bits.K_CONTENT_NAME) ? request.Headers.GetValues(Bits.K_CONTENT_NAME).First() : string.Empty;
                ICollection<string> content_encoding = request.Content.Headers.ContentEncoding;
                // set response headers's session id
                response.Headers.Add(Bits.K_BITS_SESSION_ID, session_id);
                // get fragment details
                total_length = request.Content.Headers.ContentRange.Length ?? 0;
                range_start = request.Content.Headers.ContentRange.From ?? 0;
                range_end = request.Content.Headers.ContentRange.To ?? 0;
                _traceManager.TraceDebug($"Fragment details : Total length = {total_length}, Range start = {range_start}, Range end = {range_end}");
            }
            catch (NotSupportedException e)
            {
                _traceManager.TraceError(e, "BITS NotSupportedException\n");
                return SendResponse(response, HttpStatusCode.BadRequest);
            }
            catch (IndexOutOfRangeException e)
            {
                _traceManager.TraceError(e, "BITS IndexOutOfRangeException\n");
                return SendResponse(response, HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                _traceManager.TraceError(e, "BITS General Exception :\n");
                response.Headers.Add("BITS-Received-Content-Range", $"{range_start}");
                status_code = HttpStatusCode.InternalServerError;
                return SendResponse(response, status_code);
            }

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                _traceManager.TraceDebug($"Begin reading data");
                byte[] data = await request.Content.ReadAsByteArrayAsync();
                _traceManager.TraceDebug($"End reading data");
                bool is_last_fragment = await Sessions[session_id].AddFragment(total_length, range_start, range_end, data);
                if (is_last_fragment)
                {
                    // On devra déplacer le fichier du dossier UploadedFiles vers le dossier PublishedFiles
                    Sessions[session_id].Move = true;
                }
                response.Headers.Add("BITS-Received-Content-Range", $"{range_end + 1}");
                status_code = HttpStatusCode.OK;
                return SendResponse(response, status_code);
            }
            catch (Bits.InvalidFragment e)
            {
                _traceManager.TraceError($"BITS InvalidFragment : LastRangeEnd = {e.LastRangeEnd}, NewRangeStart = {e.NewRangeStart}");
                response.Headers.Add("BITS-Received-Content-Range", $"{e.LastRangeEnd + 1}");
                response.Headers.Add(Bits.K_BITS_ERROR_CODE, Bits.BITSServerHResult.ZERO.ToString());
                response.Headers.Add(Bits.K_BITS_ERROR_CONTEXT, Bits.BITSServerHResult.BG_ERROR_CONTEXT_REMOTE_FILE.ToString());
                status_code = HttpStatusCode.RequestedRangeNotSatisfiable;
                return SendResponse(response, status_code);
            }
            catch (Bits.FragmentTooLarge e)
            {
                _traceManager.TraceError($"BITS FragmentTooLarge : FragmentSize = {e.FragmentSize}");
                response.Headers.Add("BITS-Received-Content-Range", $"{range_start}");
                response.Headers.Add(Bits.K_BITS_ERROR_CODE, Bits.BITSServerHResult.BG_E_TOO_LARGE.ToString());
                response.Headers.Add(Bits.K_BITS_ERROR_CONTEXT, Bits.BITSServerHResult.BG_ERROR_CONTEXT_REMOTE_FILE.ToString());
                status_code = HttpStatusCode.InternalServerError;
                return SendResponse(response, status_code);
            }
            catch (Exception e)
            {
                _traceManager.TraceError(e, "BITS General Exception :\n");
                response.Headers.Add("BITS-Received-Content-Range", $"{range_start}");
                status_code = HttpStatusCode.InternalServerError;
                return SendResponse(response, status_code);
            }
        }
    }
}
