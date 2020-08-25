using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using TusDotNetClient;

namespace Kprocess.KL2.FileTransfer
{
    public class TusOperation : NotifiableObject
    {
        public event EventHandler OnTransferFinished;

        public Dictionary<string, string> AdditionalHeaders { get; } = new Dictionary<string, string>();

        TusTransferProgress _progress;
        public TusTransferProgress Progress
        {
            get => _progress;
            private set
            {
                if (_progress != value)
                {
                    _progress = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Task Task { get; private set; }

        readonly IProgress<(long bytesTransferred, long bytesTotal)> TaskProgress;

        readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();

        string _error;
        public string Error
        {
            get => _error;
            set
            {
                if (_error != value)
                {
                    _error = value;
                    RaisePropertyChanged();
                }
            }
        }

        TransferStatus _state;
        public TransferStatus State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    RaisePropertyChanged();
                    CanResume = _state == TransferStatus.Suspended;
                    CanPause = _state == TransferStatus.Transferring;
                    CanCancel = _state != TransferStatus.Canceled;
                }
            }
        }

        bool _isFinished;
        public bool IsFinished
        {
            get => _isFinished;
            set
            {
                if (_isFinished != value)
                {
                    _isFinished = value;
                    if (_isFinished)
                    {
                        //_job.Complete(); // Post traitement sur le fichier

                        OnTransferFinished?.Invoke(this, null);
                    }
                    RaisePropertyChanged();
                }
            }
        }

        bool _canResume;
        public bool CanResume
        {
            get => _canResume;
            set
            {
                if (_canResume != value)
                {
                    _canResume = value;
                    RaisePropertyChanged();
                }
            }
        }

        bool _canPause;
        public bool CanPause
        {
            get => _canPause;
            set
            {
                if (_canPause != value)
                {
                    _canPause = value;
                    RaisePropertyChanged();
                }
            }
        }

        bool _canCancel;
        public bool CanCancel
        {
            get => _canCancel;
            set
            {
                if (_canCancel != value)
                {
                    _canCancel = value;
                    RaisePropertyChanged();
                }
            }
        }

        public TusOperation(IProgress<(long bytesTransferred, long bytesTotal)> progress = null, CancellationTokenSource cancelTokenSource = null)
        {
            TaskProgress = progress;
            _cancellationSource = cancelTokenSource;
            Progress = new TusTransferProgress();
            Progress.PropertyChanged += (sender, e) =>
            {
                RaisePropertyChanged(nameof(Progress));
            };
            ((Progress<(long bytesTransferred, long bytesTotal)>)TaskProgress).ProgressChanged += (o, e) =>
            {
                State = TransferStatus.Transferring;
                Progress.BytesTotal = e.bytesTotal;
                Progress.BytesTransferred = e.bytesTransferred;
                Progress.RaisePropertyChanged(nameof(TusTransferProgress.PercentBytesTransferred));
            };
            State = TransferStatus.Queued;
            Resume();
        }

        public static (string id, TusOperation operation) CreateUpload(string url, FileInfo file, IProgress<(long bytesTransferred, long bytesTotal)> progress = null)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            TusOperation result = new TusOperation(progress, cts);

            result.Task = Task.Factory.StartNew(() =>
            {
                using (var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    result.UploadInternal(url, fs);
                }
            });

            return (Path.GetFileName(url), result);
        }

        public static (string id, TusOperation operation) CreateUpload(string url, Stream fileStream, IProgress<(long bytesTransferred, long bytesTotal)> progress = null)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            TusOperation result = new TusOperation(progress, cts);

            result.Task = Task.Factory.StartNew(() =>
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                result.UploadInternal(url, fileStream);
            });

            return (Path.GetFileName(url), result);
        }

        public void Resume()
        {
            //_job.Resume(); // Il faut relancer l'upload
            Progress.stopWatch.Restart();
        }

        public void Pause()
        {
            //_job.Suspend(); // Il faut lancer une requete de pause
            Progress.Speed = 0;
        }

        public void Cancel()
        {
            _cancellationSource.Cancel();
            State = TransferStatus.Canceled;
        }

        public async Task<bool> WaitTransferFinished(CancellationTokenSource cts = null)
        {
            try
            {
                while (!IsFinished)
                {
                    if (cts?.IsCancellationRequested == true)
                    {
                        Cancel();
                        return false;
                    }
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        void UploadInternal(string url, Stream fs)
        {
            var offset = GetFileOffset(url);
            var client = new TusHttpClient();
            SHA1 sha = new SHA1Managed();

            int chunkSize = (int)Math.Ceiling(3 * 1024.0 * 1024.0); // 3 MB
            if (offset == fs.Length)
                OnUploadProgress(fs.Length, fs.Length);

            while (offset < fs.Length)
            {
                fs.Seek(offset, SeekOrigin.Begin);
                var buffer = new byte[chunkSize];
                var bytesRead = fs.Read(buffer, 0, buffer.Length);
                Array.Resize(ref buffer, bytesRead);
                var sha1Hash = sha.ComputeHash(buffer);
                var request = new TusHttpRequest(url, RequestMethod.Patch, buffer, _cancellationSource.Token);
                foreach (var kvp in AdditionalHeaders)
                    request.AddHeader(kvp.Key, kvp.Value);
                request.AddHeader(TusHeaderNames.UploadOffset, $"{offset}");
                request.AddHeader(TusHeaderNames.UploadChecksum, $"sha1 {Convert.ToBase64String(sha1Hash)}");
                request.AddHeader(TusHeaderNames.ContentType, "application/offset+octet-stream");

                request.UploadProgressed += (bytesTransferred, bytesTotal) =>
                {
                    if (Progress.stopWatch.ElapsedMilliseconds >= 200)
                        TaskProgress.Report((offset + bytesTransferred, fs.Length));
                };

                try
                {
                    var response = client.PerformRequest(request);
                    if (response.StatusCode != HttpStatusCode.NoContent)
                        throw new Exception($"WriteFileInServer failed. {response.ResponseString}");

                    offset = long.Parse(response.Headers[TusHeaderNames.UploadOffset]);

                    OnUploadProgress(offset, fs.Length);
                }
                catch (IOException ex)
                {
                    if (ex.InnerException is SocketException socketException)
                    {
                        if (socketException.SocketErrorCode == SocketError.ConnectionReset)
                        {
                            // retry by continuing the while loop but get new offset from server to prevent Conflict error
                            offset = GetFileOffset(url);
                        }
                        else
                            throw socketException;
                    }
                    else
                        throw;
                }
            }
            IsFinished = true;
        }

        public TusHttpResponse Download(string url)
        {
            var client = new TusHttpClient();
            var request = new TusHttpRequest(url, RequestMethod.Get, null, _cancellationSource.Token);
            foreach (var kvp in AdditionalHeaders)
                request.AddHeader(kvp.Key, kvp.Value);
            request.DownloadProgressed += OnDownloadProgress;
            var response = client.PerformRequest(request);
            return response;
        }

        public TusHttpResponse Head(string url)
        {
            var client = new TusHttpClient();
            var request = new TusHttpRequest(url, RequestMethod.Head);
            foreach (var kvp in AdditionalHeaders)
                request.AddHeader(kvp.Key, kvp.Value);
            try
            {
                return client.PerformRequest(request);
            }
            catch (TusException ex)
            {
                return new TusHttpResponse(ex.StatusCode);
            }
        }

        public TusServerInfo GetServerInfo(string url)
        {
            var client = new TusHttpClient();
            var request = new TusHttpRequest(url, RequestMethod.Options);
            foreach (var kvp in AdditionalHeaders)
                request.AddHeader(kvp.Key, kvp.Value);
            var response = client.PerformRequest(request);
            if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"{nameof(GetServerInfo)} failed. " + response.ResponseString);

            // Spec says NoContent but tusd gives OK because of browser bugs
            response.Headers.TryGetValue(TusHeaderNames.TusResumable, out var version);
            response.Headers.TryGetValue(TusHeaderNames.TusVersion, out var supportedVersion);
            response.Headers.TryGetValue(TusHeaderNames.TusExtension, out var extensions);
            response.Headers.TryGetValue(TusHeaderNames.TusMaxSize, out var maxSizeString);
            long.TryParse(maxSizeString, out var maxSize);
            return new TusServerInfo(version, supportedVersion, extensions, maxSize);
        }

        public bool Delete(string url)
        {
            var client = new TusHttpClient();
            var request = new TusHttpRequest(url, RequestMethod.Delete);
            foreach (var kvp in AdditionalHeaders)
                request.AddHeader(kvp.Key, kvp.Value);
            var response = client.PerformRequest(request);

            return response.StatusCode == HttpStatusCode.NoContent ||
                   response.StatusCode == HttpStatusCode.NotFound ||
                   response.StatusCode == HttpStatusCode.Gone;
        }

        public void OnUploadProgress(long bytesTransferred, long bytesTotal) =>
            TaskProgress?.Report((bytesTransferred, bytesTotal));

        public void OnDownloadProgress(long bytesTransferred, long bytesTotal) =>
            TaskProgress?.Report((bytesTransferred, bytesTotal));

        long GetFileOffset(string url)
        {
            var client = new TusHttpClient();
            var request = new TusHttpRequest(url, RequestMethod.Head);
            foreach (var kvp in AdditionalHeaders)
                request.AddHeader(kvp.Key, kvp.Value);
            var response = client.PerformRequest(request);

            if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.OK)
                throw new Exception($"{nameof(GetFileOffset)} failed. {response.ResponseString}");

            if (!response.Headers.ContainsKey(TusHeaderNames.UploadOffset))
                throw new Exception("Offset Header Missing");

            return long.Parse(response.Headers[TusHeaderNames.UploadOffset]);
        }
    }
}
