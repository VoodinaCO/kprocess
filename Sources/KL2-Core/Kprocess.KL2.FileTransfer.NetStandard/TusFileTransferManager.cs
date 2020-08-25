using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using TusDotNetClient;

namespace Kprocess.KL2.FileTransfer
{
    public class TusFileTransferManager
    {
        static readonly FileSystemWatcher _watcher;

        public event EventHandler<string> OnFileAdded;
        public event EventHandler<string> OnFileDeleted;

        public delegate void DownloadingEvent(long bytesTransferred, long bytesTotal);
        public delegate void UploadingEvent(long bytesTransferred, long bytesTotal);

#pragma warning disable CS0067
        public event DownloadingEvent Downloading;
        public event UploadingEvent Uploading;
#pragma warning restore CS0067

        public IWebProxy Proxy { get; set; }

        public ObservableDictionary<string, TusOperation> DownloadOperations { get; } = new ObservableDictionary<string, TusOperation>();
        public ObservableDictionary<string, TusOperation> UploadOperations { get; } = new ObservableDictionary<string, TusOperation>();

        static TusFileTransferManager _instance;
        public static TusFileTransferManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TusFileTransferManager();
                return _instance;
            }
        }

        static TusFileTransferManager()
        {
            var syncDir = Preferences.SyncDirectory;
            if (syncDir == null) // SyncDirectory not supported (ie: Web)
                return;
            _watcher = new FileSystemWatcher(Preferences.SyncDirectory);
            _watcher.Created += (o, e) =>
            {
                Instance.OnFileAdded?.Invoke(Instance, e.FullPath);
            };
            _watcher.Renamed += (o, e) =>
            {
                Instance.OnFileAdded?.Invoke(Instance, e.FullPath);
            };
            _watcher.Deleted += (o, e) =>
            {
                Instance.OnFileDeleted?.Invoke(Instance, e.FullPath);
            };
            _watcher.EnableRaisingEvents = true;
        }

        public TusFileTransferManager()
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(Preferences.FileServerUriKey))
                Preferences.FileServerUri = ConfigurationManager.AppSettings[Preferences.FileServerUriKey];
        }

        string Create(string url, FileInfo file, params (string key, string value)[] metadata)
        {
            var metadataDictionary = metadata.ToDictionary(md => md.key, md => md.value);
            if (!metadataDictionary.ContainsKey(TusMetaData.FileName))
                metadataDictionary[TusMetaData.FileName] = file.Name;

            return Create(url, file.Length, metadataDictionary);
        }
        string Create(string url, long uploadLength, Dictionary<string, string> metadata)
        {
            var requestUri = new Uri(url);
            var client = new TusHttpClient { Proxy = Proxy };

            var request = new TusHttpRequest(url, RequestMethod.Post);
            /*foreach (var kvp in AdditionalHeaders)
                request.AddHeader(kvp.Key, kvp.Value);*/
            request.AddHeader(TusHeaderNames.UploadLength, uploadLength.ToString());
            request.AddHeader(TusHeaderNames.ContentLength, "0");

            request.AddHeader(TusHeaderNames.UploadMetadata, string.Join(",", metadata
                .Select(md =>
                    $"{md.Key.Replace(" ", "").Replace(",", "")} {Convert.ToBase64String(Encoding.UTF8.GetBytes(md.Value))}")));

            var response = client.PerformRequest(request);

            if (response.StatusCode != HttpStatusCode.Created)
                throw new Exception($"CreateFileInServer failed. {response.ResponseString}");

            if (!response.Headers.ContainsKey(TusHeaderNames.Location))
                throw new Exception("Location Header Missing");

            if (!Uri.TryCreate(response.Headers[TusHeaderNames.Location], UriKind.RelativeOrAbsolute, out var locationUri))
                throw new Exception("Invalid Location Header");

            if (!locationUri.IsAbsoluteUri)
                locationUri = new Uri(requestUri, locationUri);

            return locationUri.ToString();
        }

        string Create(string url, string fileName, Stream fileStream, params (string key, string value)[] metadata)
        {
            var metadataDictionary = metadata.ToDictionary(md => md.key, md => md.value);
            if (!metadataDictionary.ContainsKey(TusMetaData.FileName))
                metadataDictionary[TusMetaData.FileName] = fileName;

            return Create(url, fileStream.Length, metadataDictionary);
        }

        public TusOperation Upload(string url, FileInfo file, bool computeHash = false)
        {
            Progress<(long bytesTranferred, long bytesTotal)> uploadProgress = new Progress<(long bytesTranferred, long bytesTotal)>();
            string fileUrl = computeHash ? Create(url, file, (TusMetaData.ComputeHash, "1")) : Create(url, file);
            (string id, TusOperation operation) = TusOperation.CreateUpload(fileUrl, file, uploadProgress);
            UploadOperations.AddWithNotification(id, operation);
            return operation;
        }

        public (string id, TusOperation operation) Upload(string url, string fileName, Stream fileStream)
        {
            Progress<(long bytesTranferred, long bytesTotal)> uploadProgress = new Progress<(long bytesTranferred, long bytesTotal)>();
            string fileUrl = Create(url, fileName, fileStream, (TusMetaData.ComputeHash, "1"));
            (string id, TusOperation operation) = TusOperation.CreateUpload(fileUrl, fileStream, uploadProgress);
            UploadOperations.AddWithNotification(id, operation);
            return (id, operation);
        }
    }
}
