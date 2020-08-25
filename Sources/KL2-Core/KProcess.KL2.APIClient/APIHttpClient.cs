//#define NOTDELETEFILES
using KProcess.Ksmed.Models;
using Murmur;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using static KProcess.KL2.APIClient.Delegates;

namespace KProcess.KL2.APIClient
{
    public class APIHttpClient : IAPIHttpClient
    {
        public const string ApiServerUriKey = "ApiServerUri";
        public const string FileServerUriKey = "FileServerUri";
        public const string BypassSSLValidationKey = "BypassSSLValidation";
        public const int PingTimeOut = 500;
        public const int RequestTimeOut = 120000;

        public event Connecting OnConnecting;
        public void RaiseOnConnecting(KL2_Server server) =>
            OnConnecting?.Invoke(server);

        public event Connecting OnDisconnecting;
        public delegate void Disconnecting(KL2_Server server);
        public void RaiseOnDisconnecting(KL2_Server server) =>
            OnDisconnecting?.Invoke(server);

        ITraceManager _traceManager;
        readonly bool BypassSSLValidation;

        public APIHttpClient()
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(BypassSSLValidationKey))
                bool.TryParse(ConfigurationManager.AppSettings[BypassSSLValidationKey], out BypassSSLValidation);

            ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback;
        }

        public static APIHttpClient GetExtendedInstance(ITraceManager traceManager, bool withConnectionChecker = false)
        {
            var instance = new APIHttpClient
            {
                _traceManager = traceManager
            };

            if (withConnectionChecker)
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains(ApiServerUriKey))
                    instance.ConnectionManager.Add(KL2_Server.API, new ConnectionChecker(instance, KL2_Server.API));
                if (ConfigurationManager.AppSettings.AllKeys.Contains(FileServerUriKey))
                    instance.ConnectionManager.Add(KL2_Server.File, new ConnectionChecker(instance, KL2_Server.File));
            }

            return instance;
        }

        public const string DownloadedFilesDirectory = @"DownloadedFiles\";

        readonly Dictionary<KL2_Server, Func<(string RootLocation, string ServiceLocation, string TokenLocation, string PingLocation)>> Servers = new Dictionary<KL2_Server, Func<(string RootLocation, string ServiceLocation, string TokenLocation, string PingLocation)>>
        {
            [KL2_Server.API] = () =>
            {
                var configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                var doc = XDocument.Load(configFile);
                var apiServerLocation = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApiServerUri").Attribute("value").Value;
                return (apiServerLocation,
                    $"{apiServerLocation}/Services/",
                    $"{apiServerLocation}/token",
                    $"{apiServerLocation}/Ping");
            },
            [KL2_Server.File] = () =>
            {
                var configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                var doc = XDocument.Load(configFile);
                var fileServerLocation = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileServerUri").Attribute("value").Value;
                return (fileServerLocation,
                    $"{fileServerLocation}/Services/",
                    $"{fileServerLocation}/token",
                    $"{fileServerLocation}/Ping");
            }
        };

        readonly Dictionary<KL2_Server, ConnectionChecker> ConnectionManager = new Dictionary<KL2_Server, ConnectionChecker>();

        readonly Dictionary<KL2_Server, int?> UserIds = new Dictionary<KL2_Server, int?>
        {
            [KL2_Server.API] = null,
            [KL2_Server.File] = null
        };

        /// <summary>
        /// A dictionary with Key = fileHash and Value = filePath
        /// </summary>
        public ObservableDictionary<string, string> DownloadedFiles { get; } = new ObservableDictionary<string, string>();
        public ConcurrentDictionary<string, Task> DownloadingFiles { get; } = new ConcurrentDictionary<string, Task>();

        /// <summary>
        /// Gets authentication token from the API
        /// <remarks>Gets from cach, if expired then re-authenticate with api and set to cache.</remarks>
        /// </summary>
        /// <returns>
        /// The token.
        /// </returns>
        public string Token { get; set; }

        /// <summary>
        /// Gets UserId
        /// </summary>
        /// <returns>
        /// The UserId.
        /// </returns>
        public int? UserId(KL2_Server server) =>
            UserIds[server];

        /// <summary>
        /// Sets UserId
        /// </summary>
        /// <returns>
        /// The UserId.
        /// </returns>
        public void SetUserId(KL2_Server server, int? id) =>
            UserIds[server] = id;

        T ToEnum<T>(string method) =>
            (T)Enum.Parse(typeof(T), method);

        DynRequest CreateRequest(KL2_Server server, string service, string function, dynamic param = null, Method method = Method.POST, bool multiPart = false)
        {
            RestClient client = new RestClient(string.IsNullOrEmpty(service) ? Servers[server]().RootLocation : Servers[server]().ServiceLocation)
            {
                Timeout = RequestTimeOut
            };
            RestRequest request = new RestRequest($"{(string.IsNullOrEmpty(service) ? string.Empty : $"{service}/")}{function}", method);
            request.AlwaysMultipartFormData |= (method == Method.POST && multiPart);
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            if (!string.IsNullOrEmpty(Token))
                request.AddHeader("Authorization", $"Bearer {Token}");

            if (method == Method.POST && param != null && !multiPart)
                request.AddParameter("application/json", JsonConvert.SerializeObject(param, settings), ParameterType.RequestBody);

            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null) // called from Web
            {
                var httpContext = HttpContext.Current;
                var remoteAddr = httpContext?.Request.ServerVariables["REMOTE_ADDR"];
                if (remoteAddr == "::1")
                    remoteAddr = "localhost";
                if (httpContext != null && remoteAddr != null)
                {
                    try
                    {
                        var hostEntry = Dns.GetHostEntry(remoteAddr);
                        request.AddHeader("RequestSource", $"Computer: {hostEntry.HostName}, IP: {remoteAddr}, App: Web, Browser: {httpContext.Request.Browser.Browser}");
                    }
                    catch (SocketException)
                    {
                        request.AddHeader("RequestSource", $"Computer: Unknown, IP: {remoteAddr}, App: Web, Browser: {httpContext.Request.Browser.Browser}");
                    }
                }
            }
            else // called from app
            {
                try
                {
                    var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                    request.AddHeader("RequestSource", $"Computer: {Environment.MachineName}, IP: {hostEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)}, App: {Assembly.GetEntryAssembly().GetName().Name}");
                }
                catch (SocketException)
                {
                    request.AddHeader("RequestSource", $"Computer: {Environment.MachineName}, IP: Unknown, App: {Assembly.GetEntryAssembly().GetName().Name}");
                }
            }

            return new DynRequest
            {
                Client = client,
                Request = request,
                Settings = settings
            };
        }

        Exception ComputeError(IRestResponse result, JsonSerializerSettings settings)
        {
            var error = JsonConvert.DeserializeObject<HttpError>(result.Content);
            if (result.StatusCode == HttpStatusCode.InternalServerError)
            {
                if (error.ContainsKey(nameof(Exception)))
                {
                    result.ErrorException = JsonConvert.DeserializeObject<Exception>((string)error["Exception"], settings);
                }
            }

            if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                result.ErrorException = JsonConvert.DeserializeObject<BLLFuncException>(error["Message"].ToString());
            }

            if (result.ErrorException == null)
                return new Exception((string)error?["ExceptionMessage"] ?? result.StatusDescription);
            return HandleResponeStatusCode(result);
        }

        Exception HandleResponeStatusCode(IRestResponse result)
        {
            var ex = result.ErrorException;
            var message = ex.Message;
            switch (result.ResponseStatus)
            {
                case ResponseStatus.Aborted:
                    return new OperationCanceledException(message, ex);
                case ResponseStatus.Error:
                    return new ServerNotReacheableException(Resources.LocalizationResources.Exception_ServerUnreachable, ex);
                case ResponseStatus.TimedOut:
                    return new TimeoutException(message, ex);
            }

            return ex;
        }

        public async Task<bool> Logon(string username, string password, string language = null)
        {
            RestClient client = new RestClient(Servers[KL2_Server.API]().RootLocation)
            {
                Timeout = RequestTimeOut
            };
            RestRequest request = new RestRequest("Token", Method.POST);
            request.AddHeader("username", username);
            request.AddHeader("password", password);
            if (!string.IsNullOrEmpty(language))
                request.AddHeader("language", language);

            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null) // called from Web
            {
                var httpContext = HttpContext.Current;
                var remoteAddr = httpContext?.Request.ServerVariables["REMOTE_ADDR"];
                if (remoteAddr == "::1")
                    remoteAddr = "localhost";
                if (httpContext != null && remoteAddr != null)
                {
                    try
                    {
                        var hostEntry = Dns.GetHostEntry(remoteAddr);
                        request.AddHeader("RequestSource", $"Computer: {hostEntry.HostName}, IP: {remoteAddr}, App: Web, Browser: {httpContext.Request.Browser.Browser}");
                    }
                    catch (SocketException)
                    {
                        request.AddHeader("RequestSource", $"Computer: Unknown, IP: {remoteAddr}, App: Web, Browser: {httpContext.Request.Browser.Browser}");
                    }
                }
            }
            else // called from app
            {
                try
                {
                    var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                    request.AddHeader("RequestSource", $"Computer: {Environment.MachineName}, IP: {hostEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)}, App: {Assembly.GetEntryAssembly().GetName().Name}");
                }
                catch (SocketException)
                {
                    request.AddHeader("RequestSource", $"Computer: {Environment.MachineName}, IP: Unknown, App: {Assembly.GetEntryAssembly().GetName().Name}");
                }
            }

            IRestResponse<bool> result = await client.ExecuteTaskAsync<bool>(request);

            if (result.StatusCode != HttpStatusCode.OK)
                return false;

            Token = (string)result.Headers.SingleOrDefault(_ => _.Name == "token")?.Value;

            return Token != null;
        }

        public async Task<bool> Relogon(string oldToken = null)
        {
            RestClient client = new RestClient(Servers[KL2_Server.API]().RootLocation)
            {
                Timeout = RequestTimeOut
            };
            RestRequest request = new RestRequest("RefreshToken", Method.POST);
            if (oldToken != null)
                request.AddHeader("token", oldToken);
            else
                request.AddHeader("token", Token);

            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null) // called from Web
            {
                var httpContext = HttpContext.Current;
                var remoteAddr = httpContext?.Request.ServerVariables["REMOTE_ADDR"];
                if (remoteAddr == "::1")
                    remoteAddr = "localhost";
                if (httpContext != null && remoteAddr != null)
                {
                    try
                    {
                        var hostEntry = Dns.GetHostEntry(remoteAddr);
                        request.AddHeader("RequestSource", $"Computer: {hostEntry.HostName}, IP: {remoteAddr}, App: Web, Browser: {httpContext.Request.Browser.Browser}");
                    }
                    catch (SocketException)
                    {
                        request.AddHeader("RequestSource", $"Computer: Unknown, IP: {remoteAddr}, App: Web, Browser: {httpContext.Request.Browser.Browser}");
                    }
                }
            }
            else // called from app
            {
                try
                {
                    var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                    request.AddHeader("RequestSource", $"Computer: {Environment.MachineName}, IP: {hostEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)}, App: {Assembly.GetEntryAssembly().GetName().Name}");
                }
                catch (SocketException)
                {
                    request.AddHeader("RequestSource", $"Computer: {Environment.MachineName}, IP: Unknown, App: {Assembly.GetEntryAssembly().GetName().Name}");
                }
            }

            IRestResponse<bool> result = await client.ExecuteTaskAsync<bool>(request);

            if (result.StatusCode != HttpStatusCode.OK)
                return false;

            Token = (string)result.Headers.SingleOrDefault(_ => _.Name == "token")?.Value;

            return Token != null;
        }

        public T Service<T>(KL2_Server server, string service, string function, dynamic param = null, string method = "POST")
        {
            DynRequest dynRequest = CreateRequest(server, service, function, param, ToEnum<Method>(method));

            IRestResponse result = dynRequest.Client.Execute(dynRequest.Request);

            if (result.StatusCode == HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<T>(result.Content, dynRequest.Settings);
            throw ComputeError(result, dynRequest.Settings);
        }

        public void Service(KL2_Server server, string service, string function, dynamic param = null, string method = "POST")
        {
            DynRequest dynRequest = CreateRequest(server, service, function, param, ToEnum<Method>(method));

            IRestResponse result = dynRequest.Client.Execute(dynRequest.Request);

            if (result.StatusCode == HttpStatusCode.OK)
                return;
            throw ComputeError(result, dynRequest.Settings);
        }

        public async Task<T> ServiceAsync<T>(KL2_Server server, string service, string function, dynamic param = null, string method = "POST")
        {
            DynRequest dynRequest = CreateRequest(server, service, function, param, ToEnum<Method>(method));

            IRestResponse<T> result = await dynRequest.Client.ExecuteTaskAsync<T>(dynRequest.Request);

            if (result.StatusCode == HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<T>(result.Content, dynRequest.Settings);
            throw ComputeError(result, dynRequest.Settings);
        }

        public async Task ServiceAsync(KL2_Server server, string service, string function, dynamic param = null, string method = "POST")
        {
            DynRequest dynRequest = CreateRequest(server, service, function, param, ToEnum<Method>(method));

            IRestResponse result = await dynRequest.Client.ExecuteTaskAsync(dynRequest.Request);

            if (result.StatusCode == HttpStatusCode.OK)
                return;
            throw ComputeError(result, dynRequest.Settings);
        }

        public async Task<T> ServiceFormDataAsync<T>(KL2_Server server, string service, string function, Dictionary<string, string> parameters, Dictionary<string, string> files, IProgress<double> progress = null)
        {
            DynRequest dynRequest = CreateRequest(server, service, function, null, Method.POST, true);

            foreach (var param in parameters)
                dynRequest.Request.AddParameter(param.Key, param.Value);

            // On récupère la taille des fichiers à envoyer
            if (files != null)
            {
                Dictionary<string, (string fileName, long fileLength)> filesWithLength = files.ToDictionary(k => k.Key, v => (v.Value, new FileInfo(v.Value).Length));
                long totalDataLength = filesWithLength.Sum(_ => _.Value.fileLength);
                long sentDataLength = 0;

                foreach (var file in filesWithLength)
                {
                    dynRequest.Request.Files.Add(new FileParameter
                    {
                        Name = file.Key,
                        Writer = (netStream) =>
                        {
                            using (var fileStream = File.OpenRead(file.Value.fileName))
                            {
                                byte[] buffer = new byte[StreamExtensions.BufferSize];
                                int read;
                                while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    netStream.Write(buffer, 0, read);
                                    sentDataLength += read;
                                    progress?.Report(Convert.ToInt32(sentDataLength * 100 / totalDataLength));
                                }
                            }
                        },
                        FileName = file.Value.fileName,
                        ContentLength = file.Value.fileLength
                    });
                }
            }

            IRestResponse<T> result = await dynRequest.Client.ExecuteTaskAsync<T>(dynRequest.Request);

            if (result.StatusCode == HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<T>(result.Content, dynRequest.Settings);
            throw ComputeError(result, dynRequest.Settings);
        }

        public Task<string[]> ListFiles(string remoteFolder = null)
        {
            return Task.Run(() =>
            {
                DirectoryInfo filesDirectory = new DirectoryInfo(string.IsNullOrEmpty(remoteFolder) ? DownloadedFilesDirectory : remoteFolder);
                return filesDirectory.GetFiles("*", SearchOption.TopDirectoryOnly).Select(_ => _.Name).ToArray();
            });
        }

        public Task<long> GetFileSize(string fileName)
        {
            dynamic param = new ExpandoObject();
            param.fileHash = fileName;
            return Service<long>(KL2_Server.File, "PublicationService", nameof(GetFileSize), param);
        }

        public Task<long> GetFilesSize(IEnumerable<string> fileNames)
        {
            dynamic param = new ExpandoObject();
            param.fileHashes = fileNames.ToArray();
            return Service<long>(KL2_Server.File, "PublicationService", nameof(GetFilesSize), param);
        }

        public Task GetFile(
            string fileName,
            string downloadFolder = null,
            IProgress<double> progress = null,
            CancellationTokenSource cancellationTokenSource = null) =>
            Task.Factory.StartNew(async () =>
            {
                string downloadedFilesDirectory = Path.Combine(downloadFolder ?? Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DownloadedFilesDirectory);
                Directory.CreateDirectory(downloadedFilesDirectory);
                DirectoryInfo filesDirectory = new DirectoryInfo(downloadedFilesDirectory);
                FileInfo[] fileInfo = filesDirectory.GetFiles(fileName.ToUpper(), SearchOption.TopDirectoryOnly);

                if (fileInfo.Length == 0)
                {
                    string downloadedFilePath = Path.Combine(downloadedFilesDirectory, fileName.ToUpper());
                    long fileSize = await GetFileSize(fileName);
                    try
                    {
                        while (!GetFileInternal(fileName, downloadedFilePath, 0, fileSize, progress, cancellationTokenSource))
                            _traceManager?.TraceDebug($"Le fichier {fileName} est corrompu, on réessaie le téléchargement.");
                        DownloadedFiles.Add(fileName.ToUpper(), downloadedFilePath);
                    }
                    catch (OperationCanceledException)
                    {
                        _traceManager?.TraceDebug($"Le téléchargement du fichier {fileName} a été annulé.");
                        try
                        {
                            if (File.Exists(downloadedFilePath))
                                File.Delete(downloadedFilePath);
                        }
                        catch
                        {
                            _traceManager?.TraceDebug($"La suppression du fichier {downloadedFilePath} a échoué.");
                        }
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _traceManager?.TraceError(ex, $"Une erreur s'est produite lors du téléchargement du fichier {fileName}.");
                        try
                        {
                            if (File.Exists(downloadedFilePath))
                                File.Delete(downloadedFilePath);
                        }
                        catch
                        {
                            _traceManager?.TraceDebug($"La suppression du fichier {downloadedFilePath} a échoué.");
                        }
                        throw;
                    }
                }
                else
                    _traceManager?.TraceDebug($"Le fichier {fileName} existe déjà, on ne le télécharge plus.");
            });

        public Task GetFiles(
            IEnumerable<string> fileNames,
            string downloadFolder = null,
            IProgress<double> progress = null,
            CancellationTokenSource cancellationTokenSource = null) =>
            Task.Factory.StartNew(async () =>
            {
                string downloadedFilesDirectory = Path.Combine(Path.GetDirectoryName(downloadFolder ?? Assembly.GetEntryAssembly().Location), DownloadedFilesDirectory);
                Directory.CreateDirectory(downloadedFilesDirectory);
                DirectoryInfo filesDirectory = new DirectoryInfo(downloadedFilesDirectory);
                (string fileHash, string downloadedFilePath)[] noExistingFiles = fileNames
                    .Where(_ => filesDirectory.GetFiles(_.ToUpper(), SearchOption.TopDirectoryOnly).Length == 0)
                    .Select(_ => (_.ToUpper(), Path.Combine(downloadedFilesDirectory, _.ToUpper())))
                    .ToArray();
                long filesSize = await GetFilesSize(noExistingFiles.Select(_ => _.fileHash).ToArray());
                long alreadyDownloadedFilesLength = 0;

                foreach ((string fileHash, string downloadedFilePath) in noExistingFiles)
                {
                    try
                    {
                        while (!GetFileInternal(fileHash, downloadedFilePath, alreadyDownloadedFilesLength, filesSize, progress, cancellationTokenSource))
                            _traceManager?.TraceDebug($"Le fichier {fileHash} est corrompu, on réessaie le téléchargement.");
                        DownloadedFiles.Add(fileHash, downloadedFilePath);
                        alreadyDownloadedFilesLength += (new FileInfo(downloadedFilePath)).Length;
                    }
                    catch (OperationCanceledException)
                    {
                        _traceManager?.TraceDebug($"Le téléchargement des fichiers a été annulé.");
                        foreach ((string fileHash, string downloadedFilePath) delFile in noExistingFiles)
                        {
                            try
                            {
                                if (File.Exists(downloadedFilePath))
                                    File.Delete(downloadedFilePath);
                            }
                            catch
                            {
                                _traceManager?.TraceDebug($"La suppression du fichier {downloadedFilePath} a échoué.");
                            }
                        }
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _traceManager?.TraceError(ex, $"Une erreur s'est produite lors du téléchargement des fichiers.");
                        foreach ((string fileHash, string downloadedFilePath) delFile in noExistingFiles)
                        {
                            try
                            {
                                if (File.Exists(downloadedFilePath))
                                    File.Delete(downloadedFilePath);
                            }
                            catch
                            {
                                _traceManager?.TraceDebug($"La suppression du fichier {downloadedFilePath} a échoué.");
                            }
                        }
                        throw;
                    }
                }
            });

        public Task PostFile(string fileName, string uploadFolder = null, IProgress<double> progress = null, CancellationTokenSource cancellationTokenSource = null)
        {
            return Task.CompletedTask;
        }

        public Task PostFiles(IEnumerable<string> fileNames, string uploadFolder = null, IProgress<double> progress = null, CancellationTokenSource cancellationTokenSource = null)
        {
            /*RestClient client = new RestClient(Servers[].ServiceLocation);
            RestRequest request = new RestRequest($"{service}/{function}", Method.POST)
            {
                AlwaysMultipartFormData = true
            };

            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

            if (Tokens[server] != null && !string.IsNullOrEmpty(Tokens[server].Access_Token))
                request.AddHeader("Authorization", $"Bearer {Tokens[server].Access_Token}");

            foreach (var param in parameters)
                request.AddParameter(param.Key, param.Value);

            // On récupère la taille des fichiers à envoyer
            if (files != null)
            {
                Dictionary<string, (string fileName, long fileLength)> filesWithLength = files.ToDictionary(k => k.Key, v => (v.Value, new FileInfo(v.Value).Length));
                long totalDataLength = filesWithLength.Sum(_ => _.Value.fileLength);
                long sentDataLength = 0;

                foreach (var file in filesWithLength)
                {
                    request.Files.Add(new FileParameter
                    {
                        Name = file.Key,
                        Writer = (netStream) =>
                        {
                            using (var fileStream = File.OpenRead(file.Value.fileName))
                            {
                                byte[] buffer = new byte[StreamExtensions.BufferSize];
                                int read;
                                while ((read = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    netStream.Write(buffer, 0, read);
                                    sentDataLength += read;
                                    progress?.Report(Convert.ToInt32(sentDataLength * 100 / totalDataLength));
                                }
                            }
                        },
                        FileName = file.Value.fileName,
                        ContentLength = file.Value.fileLength
                    });
                }
            }

            IRestResponse<T> result = await client.ExecuteTaskAsync<T>(request);

            if (result.StatusCode == HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<T>(result.Content, settings);
            if (result.StatusCode == HttpStatusCode.InternalServerError)
            {
                HttpError error = JsonConvert.DeserializeObject<HttpError>(result.Content);
                result.ErrorException = JsonConvert.DeserializeObject<Exception>((string)error["Exception"], settings);
            }

            throw result.ErrorException;*/
            return Task.CompletedTask;
        }

        bool GetFileInternal(string fileHash, string filePath, long alreadyDownloadedFilesLength, long totalFileLength, IProgress<double> progress = null, CancellationTokenSource cancellationTokenSource = null)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
            using (var fileStream = File.OpenWrite(filePath))
            {
                RestClient client = new RestClient($"{Servers[KL2_Server.File]().ServiceLocation}");
                RestRequest request = new RestRequest($"PublicationService/GetFile/{fileHash}", Method.GET);

                if (!string.IsNullOrEmpty(Token))
                    request.AddHeader("Authorization", $"Bearer {Token}");

                request.ResponseWriter = (netStream) =>
                {
                    byte[] buffer = new byte[StreamExtensions.BufferSize];
                    int read;
                    while ((read = netStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (cancellationTokenSource?.Token.IsCancellationRequested == true)
                            throw new OperationCanceledException();
                        fileStream.Write(buffer, 0, read);
                        //progress?.Report(Convert.ToInt32(totalFileLength * 100 / (alreadyDownloadedFilesLength + netStream.Length)));
                        progress?.Report(totalFileLength * 100 / (alreadyDownloadedFilesLength + netStream.Length));
                    }
                    fileStream.Close();
                };
                client.DownloadData(request, true);
            }
            // On vérifie le hash avant de l'ajouter au dictionnaire de fichiers
            HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
            using (var fileStream = File.OpenRead(filePath))
            {
                byte[] computedHash = murmur128.ComputeHash(fileStream);
                return fileHash == computedHash.ToHashString();
            }
        }

        public void RefreshDownloadedFiles(string downloadFolder = null)
        {
            string downloadedFilesDirectory = Path.Combine(downloadFolder ?? Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DownloadedFilesDirectory);
            Directory.CreateDirectory(downloadedFilesDirectory);
            DirectoryInfo filesDirectory = new DirectoryInfo(downloadedFilesDirectory);
            foreach (FileInfo fileInfo in filesDirectory.GetFiles())
                if (!DownloadingFiles.ContainsKey(fileInfo.Name))
                    DownloadedFiles.Add(fileInfo.Name, fileInfo.FullName);
        }

        public void CleanDownloadedFiles(string downloadFolder = null)
        {
            string downloadedFilesDirectory = Path.Combine(downloadFolder ?? Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DownloadedFilesDirectory);
#if NOTDELETEFILES
            // Pour des gains de temps, en debug, on n'efface pas les fichiers téléchargés
            RefreshDownloadedFiles(downloadFolder);
#else
            if (Directory.Exists(downloadedFilesDirectory))
            {
                try
                {
                    Directory.Delete(downloadedFilesDirectory, true);
                }
                catch
                {
                    _traceManager?.TraceDebug($"La suppression du dossier {downloadedFilesDirectory} a échoué.");
                }
            }
            Directory.CreateDirectory(downloadedFilesDirectory);
            DownloadedFiles.Clear();
#endif
        }

        public async Task<bool> CheckConnectionAsync(KL2_Server server)
        {
            RestClient client = new RestClient($"{Servers[server]().PingLocation}");
            RestRequest request = new RestRequest(Method.GET)
            {
                Timeout = PingTimeOut
            };
            IRestResponse result = await client.ExecuteTaskAsync(request);
            return result.StatusCode == HttpStatusCode.OK;
        }

        public bool CheckConnection(KL2_Server server)
        {
            RestClient client = new RestClient($"{Servers[server]().PingLocation}");
            RestRequest request = new RestRequest(Method.GET)
            {
                Timeout = PingTimeOut
            };
            IRestResponse result = client.Execute(request);
            return result.StatusCode == HttpStatusCode.OK;
        }

        public (string fileName, byte[] data) GetLog(KL2_Server server)
        {
            try
            {
                var fileName = string.Empty;
                byte[] uncompressedBytes;

                RestClient client = new RestClient($"{Servers[server]().RootLocation}");
                RestRequest request = new RestRequest("GetLog", Method.GET);
                if (!string.IsNullOrEmpty(Token))
                    request.AddHeader("Authorization", $"Bearer {Token}");
                var archiveData = client.DownloadData(request, true);

                using (var inStream = new MemoryStream(archiveData))
                {
                    using (var archive = new ZipArchive(inStream))
                    {
                        var logFile = archive.Entries.First();
                        fileName = logFile.Name;
                        using (var entryStream = logFile.Open())
                        {
                            using (var fileToUncompressStream = new MemoryStream())
                            {
                                entryStream.CopyTo(fileToUncompressStream);
                                uncompressedBytes = fileToUncompressStream.ToArray();
                            }
                        }
                    }
                }
                return (fileName, uncompressedBytes);
            }
            catch (Exception ex)
            {
                _traceManager.TraceError(ex, ex.Message);
                return (string.Empty, null);
            }
        }

        bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
            sslPolicyErrors == SslPolicyErrors.None || BypassSSLValidation;
    }

    public enum KL2_Server
    {
        API = 0,
        File
    }

    class DynRequest
    {
        public RestClient Client { get; set; }
        public RestRequest Request { get; set; }
        public JsonSerializerSettings Settings { get; set; }
    }

    class ConnectionChecker
    {
        static readonly ConcurrentBag<int> debugInstancesCount = new ConcurrentBag<int>();

        readonly APIHttpClient _client;
        readonly KL2_Server _server;
        //readonly Task Task;
        readonly BackgroundWorker BackgroundWorker;
        readonly Progress<bool> Progress;
        bool isBusy = false;

        public bool IsConnected { get; private set; }

        public ConnectionChecker(APIHttpClient client, KL2_Server server)
        {
            _client = client;
            _server = server;
            Progress = new Progress<bool>();
            Progress.ProgressChanged += (o, newValue) =>
            {
                if (isBusy)
                    return;

                isBusy = true;
                var oldValue = IsConnected;
                IsConnected = newValue;
                if (!oldValue && newValue)
                    _client.RaiseOnConnecting(_server);
                else if (oldValue && !newValue)
                    _client.RaiseOnDisconnecting(_server);
                isBusy = false;
            };

            //Task = WorkTask(Progress);
            BackgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            BackgroundWorker.ProgressChanged += (sender, e) => ((IProgress<bool>)Progress).Report((bool)e.UserState);
            BackgroundWorker.DoWork += (sender, e) =>
            {
                debugInstancesCount.Add(sender.GetHashCode());
                if (debugInstancesCount.Count() > 2)
                    Console.WriteLine("Connection checker bug is present");
                while (true)
                {
                    bool isOnline = client.CheckConnection(_server);
                    ((BackgroundWorker)sender).ReportProgress(0, isOnline);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            };
            BackgroundWorker.RunWorkerAsync();
        }

        Task WorkTask(IProgress<bool> progress) =>
            Task.Run(async () =>
            {
                while (true)
                {
                    progress.Report(await _client.CheckConnectionAsync(_server));
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            });
    }
}
