using Kprocess.KL2.FileTransfer;
using KProcess.Ksmed.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kprocess.KL2.TabletClient.Services
{
    public class DownloadManager
    {
        readonly FileTransferManager _fileTransferManager;
        CancellationTokenSource cts;
        Task<TaskResult> _downloadTask;

        List<string> oldSyncFiles = new List<string>();
        ConcurrentBag<(string remoteFilePath, string localFilePath)> newSyncFiles = new ConcurrentBag<(string remoteFilePath, string localFilePath)>();

        public DownloadManager(FileTransferManager fileTransferManager)
        {
            _fileTransferManager = fileTransferManager;
        }

        public bool IsDownloading =>
            _downloadTask != null && !_downloadTask.IsCompleted;

        public async Task StopDownload()
        {
            if (_downloadTask == null)
                return;
            cts.Cancel();
            await Task.WhenAll(_downloadTask);
            _downloadTask = null;
            await FileTransferManager.DeleteSyncFiles(newSyncFiles.Select(newFile => newFile.localFilePath)
                .Where(newFile => !oldSyncFiles.Any(oldFile => oldFile.Equals(newFile))));
        }

        public async Task<TaskResult> StartDownload(Publication publication)
        {
            oldSyncFiles = FileTransferManager.ListSyncFiles();
            cts = new CancellationTokenSource();

            _downloadTask = DownloadFile(publication);
            TaskResult result = await _downloadTask;
            Locator.Main.LoadingText = null;

            if (result == TaskResult.Ok)
                await FileTransferManager.DeleteSyncFiles(oldSyncFiles.Where(oldFile => !newSyncFiles.Any(newFile => oldFile.Equals(newFile.localFilePath))));

            return result;
        }

        public async Task<TaskResult> StartDownload(IEnumerable<Publication> publications)
        {
            oldSyncFiles = FileTransferManager.ListSyncFiles();
            cts = new CancellationTokenSource();

            _downloadTask = DownloadFile(publications);
            TaskResult result = await _downloadTask;
            Locator.Main.LoadingText = null;

            if (result == TaskResult.Ok)
                await FileTransferManager.DeleteSyncFiles(oldSyncFiles.Where(oldFile => !newSyncFiles.Any(newFile => oldFile.Equals(newFile.localFilePath))));

            return result;
        }

        public static List<(string remoteFilePath, string localFilePath, string remoteExistsPath)> GetFilesList(Publication publication)
        {
            // Les fichiers liés à la publication
            var thumbnails = publication.PublishedActions.Where(_ => _.Thumbnail != null).Select(_ => $"{_.Thumbnail.Hash}{_.Thumbnail.Extension}").Distinct();
            var resources = publication.PublishedActions.Where(_ => _.PublishedResource?.File != null).Select(_ => $"{_.PublishedResource.File.Hash}{_.PublishedResource.File.Extension}").Distinct();
            var categories = publication.PublishedActions.Where(_ => _.PublishedActionCategory?.File != null).Select(_ => $"{_.PublishedActionCategory.File.Hash}{_.PublishedActionCategory.File.Extension}").Distinct();
            var referentials = publication.PublishedActions.SelectMany(_ => _.PublishedReferentialActions).Where(_ => _.PublishedReferential?.File != null).Select(_ => $"{_.PublishedReferential.File.Hash}{_.PublishedReferential.File.Extension}").Distinct();
            var videos = publication.PublishedActions.Where(_ => _.CutVideo != null).Select(_ => $"{_.CutVideo.Hash}{_.CutVideo.Extension}").Distinct();

            // Les fichiers liés aux publications liées
            var linkedThumbnails = publication.PublishedActions.Where(_ => _.LinkedPublication != null).SelectMany(_ => _.LinkedPublication.PublishedActions)
                .Where(_ => _.Thumbnail != null).Select(_ => $"{_.Thumbnail.Hash}{_.Thumbnail.Extension}").Distinct();
            var linkedResources = publication.PublishedActions.Where(_ => _.LinkedPublication != null).SelectMany(_ => _.LinkedPublication.PublishedActions)
                .Where(_ => _.PublishedResource?.File != null).Select(_ => $"{_.PublishedResource.File.Hash}{_.PublishedResource.File.Extension}").Distinct();
            var linkedCategories = publication.PublishedActions.Where(_ => _.LinkedPublication != null).SelectMany(_ => _.LinkedPublication.PublishedActions)
                .Where(_ => _.PublishedActionCategory?.File != null).Select(_ => $"{_.PublishedActionCategory.File.Hash}{_.PublishedActionCategory.File.Extension}").Distinct();
            var linkedReferentials = publication.PublishedActions.Where(_ => _.LinkedPublication != null).SelectMany(_ => _.LinkedPublication.PublishedActions)
                .SelectMany(_ => _.PublishedReferentialActions).Where(_ => _.PublishedReferential?.File != null).Select(_ => $"{_.PublishedReferential.File.Hash}{_.PublishedReferential.File.Extension}").Distinct();
            var linkedVideos = publication.PublishedActions.Where(_ => _.LinkedPublication != null).SelectMany(_ => _.LinkedPublication.PublishedActions)
                .Where(_ => _.CutVideo != null).Select(_ => $"{_.CutVideo.Hash}{_.CutVideo.Extension}").Distinct();

            string[] files = thumbnails.Union(linkedThumbnails)
                .Union(resources).Union(linkedResources)
                .Union(categories).Union(linkedCategories)
                .Union(referentials).Union(linkedReferentials)
                .Union(videos).Union(linkedVideos).Distinct().ToArray();

            return files
                .Select(_ => ($"{Preferences.FileServerUri}/GetFile/{_}", Path.Combine(Preferences.SyncDirectory, _), $"{Preferences.FileServerUri}/Exists/{_}"))
                .ToList();
        }

        public static List<(string remoteFilePath, string localFilePath, string remoteExistsPath)> GetFilesList(IEnumerable<Publication> publications)
        {
            return publications.SelectMany(p => GetFilesList(p)).Distinct().ToList();
        }

        async Task<TaskResult> DownloadFile(Publication publication)
        {
            Progress<double?> downloadProgress = new Progress<double?>(progress =>
            {
                if (progress.HasValue)
                    Locator.Main.LoadingText = $"{Math.Round(progress.Value)} %";
                else
                    Locator.Main.LoadingText = "Connexion au serveur de fichier...";
            });
            (downloadProgress as IProgress<double?>)?.Report(null);

            newSyncFiles = new ConcurrentBag<(string remoteFilePath, string localFilePath)>();
            GetFilesList(publication).AsParallel().ForAll(_ =>
            {
                WebRequest webRequest = WebRequest.Create(_.remoteExistsPath);
                WebResponse webResp = webRequest.GetResponse();
                string responseString = null;
                using (Stream stream = webResp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    responseString = reader.ReadToEnd().Replace("\"", "");
                    if (bool.Parse(responseString))
                        newSyncFiles.Add((_.remoteFilePath, _.localFilePath));
                }
            });
            (string remoteFilePath, string localFilePath)[] downloadList = newSyncFiles.Where(_ => !File.Exists(_.localFilePath))
                .Select(_ => (_.remoteFilePath, _.localFilePath))
                .ToArray();

            try
            {
                if (downloadList.Any())
                {
                    var downloadOperation = _fileTransferManager.CreateDownload($"Download files for publication '{publication.Label}'", downloadList);
                    downloadOperation.Progress.PropertyChanged += (o, e) =>
                    {
                        (downloadProgress as IProgress<double?>)?.Report(downloadOperation.Progress.PercentBytesTransferred);
                    };
                    downloadOperation.Resume();
                    return await downloadOperation.WaitTransferFinished(cts);
                }
                return TaskResult.Ok;
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Une erreur s'est produite lors du téléchargement.");
                return TaskResult.Nok;
            }
            finally
            {
                downloadProgress = null;
            }
        }

        async Task<TaskResult> DownloadFile(IEnumerable<Publication> publications)
        {
            Progress<double?> downloadProgress = new Progress<double?>(progress =>
            {
                if (progress.HasValue)
                    Locator.Main.LoadingText = $"{Math.Round(progress.Value)} %";
                else
                    Locator.Main.LoadingText = "Connexion au serveur de fichier...";
            });
            (downloadProgress as IProgress<double?>)?.Report(null);

            newSyncFiles = new ConcurrentBag<(string remoteFilePath, string localFilePath)>();
            GetFilesList(publications).AsParallel().ForAll(_ =>
            {
                WebRequest webRequest = WebRequest.Create(_.remoteExistsPath);
                WebResponse webResp = webRequest.GetResponse();
                string responseString = null;
                using (Stream stream = webResp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    responseString = reader.ReadToEnd().Replace("\"", "");
                    if (bool.Parse(responseString))
                        newSyncFiles.Add((_.remoteFilePath, _.localFilePath));
                }
            });
            (string remoteFilePath, string localFilePath)[] downloadList = newSyncFiles.Where(_ => !File.Exists(_.localFilePath))
                .Select(_ => (_.remoteFilePath, _.localFilePath))
                .ToArray();

            try
            {
                if (downloadList.Any())
                {
                    var downloadOperation = _fileTransferManager.CreateDownload($"Download files for publication '{publications.First().Label}'", downloadList);
                    downloadOperation.Progress.PropertyChanged += (o, e) =>
                    {
                        (downloadProgress as IProgress<double?>)?.Report(downloadOperation.Progress.PercentBytesTransferred);
                    };
                    downloadOperation.Resume();
                    return await downloadOperation.WaitTransferFinished(cts);
                }
                return TaskResult.Ok;
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Une erreur s'est produite lors du téléchargement.");
                return TaskResult.Nok;
            }
            finally
            {
                downloadProgress = null;
            }
        }
    }
}
