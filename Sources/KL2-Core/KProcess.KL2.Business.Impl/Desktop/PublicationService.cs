using KProcess.Business;
using KProcess.KL2.APIClient;
using KProcess.KL2.Business.Impl.Shared;
using KProcess.KL2.Languages;
using KProcess.KL2.SignalRClient;
using KProcess.KL2.SignalRClient.Context;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Data.Extensions;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using MoreLinq;
using Murmur;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Desktop
{
    /// <summary>
    /// Représente un service de publication de scénario.
    /// </summary>
    public class PublicationService : IBusinessService, IPublicationService
    {
        readonly ITraceManager _traceManager;
        readonly ISecurityContext _securityContext;
        readonly ILocalizationManager _localizationManager;
        readonly IFileProvider _fileProvider;
        readonly IKLPublicationRepository _publicationRepository;
        readonly string transcodeExt = ".mp4";
        readonly int maxAudioBitRate = 96000; // 96k
        readonly int maxVideoBitRate = 1500000; // 1500k

        readonly string VideoBufferDirectory = @"VideosBuffer";
        const int _MARKING_PADDING = 10;

        static readonly ConcurrentDictionary<int, PublishTaskInfos> _runningPublications = new ConcurrentDictionary<int, PublishTaskInfos>();

        public string PublishedFilesDirectory
        {
            get => ServiceConst.PublishedFilesDirectory;
            set
            {
                ServiceConst.PublishedFilesDirectory = value;
            }
        }

        public PublicationService(ISecurityContext securityContext, ILocalizationManager localizationManager, ITraceManager traceManager, IFileProvider fileProvider, IKLPublicationRepository publicationRepository)
        {
            _securityContext = securityContext;
            _localizationManager = localizationManager;
            _traceManager = traceManager;
            _fileProvider = fileProvider;
            _publicationRepository = publicationRepository;

            //VideoBufferDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), VideoBufferDirectory);
            VideoBufferDirectory = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), VideoBufferDirectory);
            Directory.CreateDirectory(VideoBufferDirectory);
        }

        public async Task<CutVideo> GetCutVideo(string hash)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
            {
                return await context.CutVideos.FirstOrDefaultAsync(u => u.Hash == hash);
            }
        }


        public async Task<Publication[]> GetPublications() =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                {
                    var publications = await context.Publications
                        .Include(nameof(Publication.Project))
                        .Include(nameof(Publication.Readers))
                        .Include($"{nameof(Publication.Project)}.{nameof(Project.Process)}")
                        .ToArrayAsync();
                    foreach (var publication in publications)
                    {
                        publication.PublishedActions = new TrackableCollection<PublishedAction>(await context.PublishedActions.Where(_ => _.PublicationId == publication.PublicationId)
                            .Include(nameof(PublishedAction.PublishedActionCategory))
                            .Include(nameof(PublishedAction.PublishedResource))
                            .Include(nameof(PublishedAction.PublishedReferentialActions))
                            .Include($"{nameof(PublishedAction.PublishedReferentialActions)}.{nameof(PublishedReferentialAction.PublishedReferential)}")
                            .OrderBy(_ => _.WBS)
                            .ToArrayAsync());
                    }
                    return publications;
                }
            });

        // Revoir les paramètres pour l'export des tâches principales uniquement
        public async Task<int> Publish(Publication publication,
            bool exportVideo,
            bool exportOnlyKeyTasksVideos)
        {
            // Create publication history
            var publicationHistory = new PublicationHistory
            {
                ProcessId = publication.ProcessId,
                State = PublicationStatus.Waiting,
                PublisherId = _securityContext.CurrentUser.User.UserId
            };
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
            {
                context.PublicationHistories.AddObject(publicationHistory);
                await context.SaveChangesAsync();
            }

            _traceManager.TraceDebug($"VideoBufferFolder directory : {VideoBufferDirectory}");

            Guid publicationGuid = Guid.NewGuid();
            (IProgress<string> Step, IProgress<double?> Progress, IProgress<bool> CanCancel, IProgress<PublicationStatus> Status) publicationTaskProgress =
                (new Progress<string>(), new Progress<double?>(), new Progress<bool>(), new Progress<PublicationStatus>());
            ((Progress<string>)publicationTaskProgress.Step).ProgressChanged += (sender, e) => { _runningPublications[publicationHistory.PublicationHistoryId].ProgressInfo.Step = e; };
            ((Progress<double?>)publicationTaskProgress.Progress).ProgressChanged += (sender, e) => { _runningPublications[publicationHistory.PublicationHistoryId].ProgressInfo.Progress = e; };
            ((Progress<bool>)publicationTaskProgress.CanCancel).ProgressChanged += (sender, e) => { _runningPublications[publicationHistory.PublicationHistoryId].ProgressInfo.CanCancel = e; };
            ((Progress<PublicationStatus>)publicationTaskProgress.Status).ProgressChanged += (sender, e) => { _runningPublications[publicationHistory.PublicationHistoryId].ProgressInfo.Status = e; };
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            void RemoveTaskFromCollection()
            {
                _runningPublications.TryRemove(publicationHistory.PublicationHistoryId, out PublishTaskInfos deleted);
            }
            void SetError(string errorMessage)
            {
                var progressInfo = _runningPublications[publicationHistory.PublicationHistoryId].ProgressInfo;
                progressInfo.ErrorMessage = errorMessage;
                progressInfo.Status = PublicationStatus.InError;
            }
            Scenario publishedScenario; 
            User publisher;
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
            {
                publishedScenario = await context.Scenarios
                    .Include(nameof(Scenario.Project))
                    .Include($"{nameof(Scenario.Project)}.{nameof(Project.Process)}")
                    .Include($"{nameof(Scenario.Project)}.{nameof(Project.Referentials)}")
                    .Include(nameof(Scenario.Actions))
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Thumbnail)}")
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Resource)}")
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Resource)}.{nameof(Resource.CloudFile)}")
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Category)}")
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Category)}.{nameof(ActionCategory.CloudFile)}")
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Video)}")
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Video)}.{nameof(Video.DefaultResource)}")
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Video)}.{nameof(Video.DefaultResource)}.{nameof(Resource.CloudFile)}")
                    .SingleAsync(_ => _.ScenarioId == publication.ScenarioId);
                void LoadRef(Scenario scenario, ProcessReferentialIdentifier referentialId)
                {
                    if (scenario.Project.Referentials.Single(_ => _.ReferentialId == referentialId).IsEnabled)
                    {
                        foreach (var action in scenario.Actions)
                        {
                            context.LoadProperty(action, referentialId.ToString());
                            if (action.Refs(referentialId)?.Any() == true)
                                foreach (var refAction in action.Refs(referentialId))
                                {
                                    context.LoadProperty(refAction, referentialId.ToString());
                                    if (refAction.Referential.Hash != null)
                                        context.LoadProperty(refAction.Referential, nameof(CloudFile));
                                }
                        }
                    }
                };
                LoadRef(publishedScenario, ProcessReferentialIdentifier.Ref1);
                LoadRef(publishedScenario, ProcessReferentialIdentifier.Ref2);
                LoadRef(publishedScenario, ProcessReferentialIdentifier.Ref3);
                LoadRef(publishedScenario, ProcessReferentialIdentifier.Ref4);
                LoadRef(publishedScenario, ProcessReferentialIdentifier.Ref5);
                LoadRef(publishedScenario, ProcessReferentialIdentifier.Ref6);
                LoadRef(publishedScenario, ProcessReferentialIdentifier.Ref7);
                publisher = await context.Users.SingleAsync(_ => _.Username == _securityContext.CurrentUser.Username);
            }
            Task publicationTask = new Task(async (dummy) =>
            {
                publicationTaskProgress.CanCancel.Report(true);
                   _traceManager.TraceDebug($"Test d'existence d'autre publication en cours...");
                // On vérifie qu'il n'y aie pas de publication en cours (Annulable)
                while (_runningPublications.Keys.IndexOf(publicationHistory.PublicationHistoryId) != 0)
                {
                    publicationTaskProgress.Step.Report("Une autre publication est en cours, veuillez patienter...");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    if (tokenSource.IsCancellationRequested)
                    {
                        RemoveTaskFromCollection();
                        return;
                    }
                }
                _traceManager.TraceDebug($"Start new publication : {publicationGuid}");
                (string OverlayTextVideo, bool AddProjectNameToWatermark, bool WBSMarkingIsEnabled, EHorizontalAlign HorizontalAlignement, EVerticalAlign VerticalAlignement) =
                    !exportVideo || string.IsNullOrEmpty(publication.Watermark) ?
                    (null, false, false, EHorizontalAlign.Center, EVerticalAlign.Top) :
                    JsonConvert.DeserializeObject<(string OverlayTextVideo, bool AddProjectNameToWatermark, bool WBSMarkingIsEnabled, EHorizontalAlign HorizontalAlignement, EVerticalAlign VerticalAlignement)>(publication.Watermark);
                Task<KeyValuePair<int, (string fileHash, string fileName, string originalFileHash)>>[] cutVideoTasks = new Task<KeyValuePair<int, (string fileHash, string filePath, string originalFileHash)>>[0];
                // On crée les tâches de découpage vidéo qui sont nécessaires (annulable)
                if (exportVideo)
                {
                    publicationTaskProgress.CanCancel.Report(true);
                    _traceManager.TraceDebug($"Création des tâches de découpe vidéo...");
                    cutVideoTasks = publishedScenario.Actions
                        .Where(_ => _.VideoId != null && (!exportOnlyKeyTasksVideos || _.IsKeyTaskManaged))
                        .Select(_ => CutVideo(_,
                            GetFinalWatermark(OverlayTextVideo,
                                AddProjectNameToWatermark ? publishedScenario.Project.Process.Label : null,
                                WBSMarkingIsEnabled ? _.WBS : null),
                            HorizontalAlignement,
                            VerticalAlignement,
                            publication.MinDurationVideo,
                            tokenSource.Token))
                        .ToArray();

                    try
                    {
                        int cutTasksCount = cutVideoTasks.Count();
                        int cutTasksCounter = 0;
                        foreach (var cutTask in cutVideoTasks)
                        {
                            publicationTaskProgress.Step.Report("Extraction des séquences depuis les vidéos brutes");
                            publicationTaskProgress.Progress.Report(Math.Round(cutTasksCounter * 100d / cutTasksCount));
                            cutTask.Start();
                            await Task.WhenAll(cutTask);
                            cutTasksCounter++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _traceManager.TraceInfo($"Exception when executing cutvideotasks with msg={ex.Message}");
                    }
                    if (cutVideoTasks.Any(_ => _.IsFaulted)) // Il y a une erreur sur un fichier (il faudrait pouvoir renvoyer la liste des fichiers en erreur à l'utilisateur)
                    {
                        // Nettoyage des vidéos découpées qui sont dans le buffer
                        ClearVideoBuffer(publishedScenario);
                        RemoveTaskFromCollection();
                        return;
                    }
                    if (cutVideoTasks.Any(_ => _.IsCanceled)) // On a annulé la publication
                    {
                        // Nettoyage des vidéos découpées qui sont dans le buffer
                        ClearVideoBuffer(publishedScenario);
                        RemoveTaskFromCollection();
                        return;
                    }
                }
                // On crée les tâches de création des thumbnails (non annulable)
                publicationTaskProgress.CanCancel.Report(false);
                publicationTaskProgress.Progress.Report(null);
                publicationTaskProgress.Step.Report("Création des vignettes...");
                _traceManager.TraceDebug($"Création des thumbnails...");
                Task<KeyValuePair<int, (string fileHash, DirectoryEnum directory, string fileName, string originalFileHash)>>[] thumbnailsToCreate = publishedScenario.Actions.Where(_ => _.Thumbnail != null)
                    .Select(_ => CreateThumbnail(_,
                        GetFinalWatermark(OverlayTextVideo,
                            AddProjectNameToWatermark ? publishedScenario.Project.Process.Label : null,
                            WBSMarkingIsEnabled ? _.WBS : null),
                        HorizontalAlignement,
                        VerticalAlignement))
                    .ToArray();
                try
                {
                    int thumbnailTasksCount = thumbnailsToCreate.Length;
                    int thumbnailTasksCounter = 0;
                    if (thumbnailsToCreate.Any())
                    {
                        publicationTaskProgress.Step.Report("Extraction des vignettes");
                        publicationTaskProgress.Progress.Report(Math.Round(thumbnailTasksCounter * 100d / thumbnailTasksCount));
                    }
                    thumbnailsToCreate.AsParallel().ForAll(thumbnailTask =>
                    {
                        thumbnailTask.Start();
                        thumbnailTask.Wait();
                        publicationTaskProgress.Progress.Report(Math.Round(++thumbnailTasksCounter * 100d / thumbnailTasksCount));
                    });
                }
                catch (Exception ex)
                {
                    _traceManager.TraceInfo($"Exception when executing CreateThumbnail with msg={ex.Message}");
                }
                if (thumbnailsToCreate.Any(_ => _.IsFaulted)) // Il y a une erreur sur un fichier (il faudrait pouvoir renvoyer la liste des fichiers en erreur à l'utilisateur)
                {
                    // Clean cut videos folder
                    ClearVideoBuffer(publishedScenario);
                    var failedFiles = thumbnailsToCreate.Where(task => task.IsFaulted && task.Exception.InnerException is ThumbnailCreationException)
                        .Select(task => ((ThumbnailCreationException)task.Exception.InnerException).Name).ToList();
                    var secContext = _securityContext;
                    var errorMessage = _localizationManager.GetStringFormat("View_PublishVideos_PublishThumbnailsError",
                        string.Join("\n", failedFiles.ToArray()));
                    SetError(errorMessage);
                    return;
                }
                // On crée les tâches de copie des fichiers qui sont nécessaires (non annulable)
                // Ne pas effacer quand on est en Desktop
                publicationTaskProgress.Step.Report("Stockage des séquences vidéo dans l'espace de partage...");
                _traceManager.TraceDebug("Création des tâches de copie de fichiers...");
                // Moving thumbnails
                IEnumerable<(string oldName, string newName)> filesToMove = thumbnailsToCreate
                    .Where(_ => _.Result.Value.directory == DirectoryEnum.Uploaded)
                    .Select(_ => (oldName: _.Result.Value.fileName, newName: $"{_.Result.Value.fileHash}{Path.GetExtension(_.Result.Value.fileName)}"));
                foreach ((string oldName, string newName) in filesToMove)
                    await _fileProvider.Complete(oldName, newName);
                // Moving cutted videos
                Progress<double> copyProgress = new Progress<double>();
                copyProgress.ProgressChanged += (o, e) => publicationTaskProgress.Progress.Report(Math.Round(e, 2));
                filesToMove = cutVideoTasks.Select(_ => (oldName : _.Result.Value.fileName, newName : $"{_.Result.Value.fileHash}{Path.GetExtension(_.Result.Value.fileName)}"));
                await _fileProvider.Upload(filesToMove.Select(_ => (new FileInfo(Path.Combine(VideoBufferDirectory, _.oldName)), _.newName)).ToArray(), DirectoryEnum.Published, copyProgress);
                ClearVideoBuffer(publishedScenario);
                // On modifie la publication (non annulable)
                publicationTaskProgress.Step.Report("Enregistrement de la publication...");
                publication.PublicationId = publicationGuid;
                publication.PublishedByUserId = publisher.UserId;
                publication.PublishedDate = DateTime.Now;
                try
                {
                    // Permet de stocker les éléments qui vont être ajoutés à la base (pour éviter les doublons)
                    List<PublishedActionCategory> createdCategories = new List<PublishedActionCategory>();
                    List<PublishedResource> createdResources = new List<PublishedResource>();
                    List<PublishedReferential> createdReferentials = new List<PublishedReferential>();
                    List<CutVideo> createdCutVideos = new List<CutVideo>();
                    List<PublishedFile> createdFiles = new List<PublishedFile>();

                    using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                    {
                        List<CutVideo> dbVideos = await context.CutVideos.ToListAsync();
                        List<PublishedFile> dbFiles = await context.PublishedFiles.ToListAsync();
                        // On ajoute tous les fichiers générés à la base
                        var resources = publishedScenario.Actions
                            .Where(_ => _.Resource != null && _.Resource.CloudFile != null)
                            .Select(_ => _.Resource)
                            .Distinct()
                            .ToDictionary(_ => _.ResourceId, _ => _.CloudFile);
                        var categories = publishedScenario.Actions
                            .Where(_ => _.Category != null && _.Resource.CloudFile != null)
                            .Select(_ => _.Category)
                            .Distinct()
                            .ToDictionary(_ => _.ActionCategoryId, _ => _.CloudFile);
                        var videoResources = publishedScenario.Actions
                            .Where(_ => _.Video != null && _.Video.DefaultResource != null && _.Video.DefaultResource.CloudFile != null)
                            .Select(_ => _.Video.DefaultResource)
                            .Distinct()
                            .ToDictionary(_ => _.ResourceId, _ => _.CloudFile);
                        var cloudFiles = thumbnailsToCreate.Select(_ => (_.Result.Value.fileHash, _.Result.Value.fileName))
                            .Union(resources.Select(_ => _.Value).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(categories.Select(_ => _.Value).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(videoResources.Select(_ => _.Value).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publishedScenario.Actions.Where(_ => _.Ref1?.Any() == true).SelectMany(_ => _.Ref1).Where(_ => _.Ref1.CloudFile != null).Select(_ => _.Ref1.CloudFile).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publishedScenario.Actions.Where(_ => _.Ref2?.Any() == true).SelectMany(_ => _.Ref2).Where(_ => _.Ref2.CloudFile != null).Select(_ => _.Ref2.CloudFile).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publishedScenario.Actions.Where(_ => _.Ref3?.Any() == true).SelectMany(_ => _.Ref3).Where(_ => _.Ref3.CloudFile != null).Select(_ => _.Ref3.CloudFile).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publishedScenario.Actions.Where(_ => _.Ref4?.Any() == true).SelectMany(_ => _.Ref4).Where(_ => _.Ref4.CloudFile != null).Select(_ => _.Ref4.CloudFile).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publishedScenario.Actions.Where(_ => _.Ref5?.Any() == true).SelectMany(_ => _.Ref5).Where(_ => _.Ref5.CloudFile != null).Select(_ => _.Ref5.CloudFile).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publishedScenario.Actions.Where(_ => _.Ref6?.Any() == true).SelectMany(_ => _.Ref6).Where(_ => _.Ref6.CloudFile != null).Select(_ => _.Ref6.CloudFile).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publishedScenario.Actions.Where(_ => _.Ref7?.Any() == true).SelectMany(_ => _.Ref7).Where(_ => _.Ref7.CloudFile != null).Select(_ => _.Ref7.CloudFile).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")));
                        cloudFiles.ForEach(_ =>
                            {
                                if (!dbFiles.Any(f => f.Hash == _.fileHash) && !createdFiles.Any(f => f.Hash == _.fileHash))
                                {
                                    PublishedFile file = new PublishedFile { Hash = _.fileHash, Extension = Path.GetExtension(_.fileName) };
                                    context.PublishedFiles.AddObject(file);
                                    createdFiles.Add(file);
                                }
                            });

                        // On définit les propriétés de navigation de chaque PublishedAction (Resource, Category, CutVideo, Thumbnail, Referentials)
                        foreach (var pAction in publication.PublishedActions)
                        {
                            if (pAction.Action == null)
                                continue;

                            if (pAction.Action.Thumbnail != null)
                                pAction.ThumbnailHash = thumbnailsToCreate.Select(_ => _.Result).Single(_ => _.Key == pAction.Action.ActionId).Value.fileHash;
                            if (pAction.Action.Resource != null)
                            {
                                Resource refLocal = pAction.Action.Resource;
                                PublishedResource refInBase = null;
                                string fileHash = pAction.Action.Resource.Hash;
                                // Test dans la base
                                refInBase = await context.PublishedResources.SingleOrDefaultAsync(_ =>
                                    _.PaceRating == refLocal.PaceRating
                                    && _.Label == refLocal.Label
                                    && (refLocal.Description == null ? _.Description == null : _.Description == refLocal.Description)
                                    && _.FileHash == fileHash);
                                // Test dans les éléments à ajouter
                                if (refInBase == null)
                                    refInBase = createdResources.SingleOrDefault(_ =>
                                        _.PaceRating == refLocal.PaceRating
                                        && _.Label == refLocal.Label
                                        && _.Description == refLocal.Description
                                        && _.FileHash == fileHash);
                                if (refInBase == null) // On doit créer une nouvelle PublishedResource
                                {
                                    refInBase = new PublishedResource(refLocal);
                                    context.PublishedResources.AddObject(refInBase);
                                    createdResources.Add(refInBase);
                                }
                                pAction.PublishedResource = refInBase;
                            }
                            if (pAction.Action.Category != null)
                            {
                                ActionCategory catLocal = pAction.Action.Category;
                                PublishedActionCategory catInBase = null;
                                string fileHash = pAction.Action.Category.Hash;
                                // Test dans la base
                                catInBase = await context.PublishedActionCategories.SingleOrDefaultAsync(_ =>
                                    _.Label == catLocal.Label
                                    && (catLocal.Description == null ? _.Description == null : _.Description == catLocal.Description)
                                    && (catLocal.ActionTypeCode == null ? _.ActionTypeCode == null : _.ActionTypeCode == catLocal.ActionTypeCode)
                                    && (catLocal.ActionValueCode == null ? _.ActionTypeCode == null : _.ActionValueCode == catLocal.ActionValueCode)
                                    && _.FileHash == fileHash);
                                // Test dans les éléments à ajouter
                                if (catInBase == null)
                                    catInBase = createdCategories.SingleOrDefault(_ =>
                                        _.Label == catLocal.Label
                                        && _.Description == catLocal.Description
                                        && _.ActionTypeCode == catLocal.ActionTypeCode
                                        && _.ActionValueCode == catLocal.ActionValueCode
                                        && _.FileHash == fileHash);
                                if (catInBase == null) // On doit créer une nouvelle PublishedActionCategory
                                {
                                    catInBase = new PublishedActionCategory(catLocal);
                                    context.PublishedActionCategories.AddObject(catInBase);
                                    createdCategories.Add(catInBase);
                                }
                                pAction.PublishedActionCategory = catInBase;
                            }
                            // TODO : Add DefaultPublishedResource property to CutVideo
                            /*if (pAction.Action.Video?.DefaultResource != null)
                            {
                                Resource refLocal = pAction.Action.Video.DefaultResource;
                                PublishedResource refInBase = null;
                                string fileHash = pAction.Action.Video.DefaultResource.Hash;
                                // Test dans la base
                                refInBase = await context.PublishedResources.SingleOrDefaultAsync(_ =>
                                    _.PaceRating == refLocal.PaceRating
                                    && _.Label == refLocal.Label
                                    && (refLocal.Description == null ? _.Description == null : _.Description == refLocal.Description)
                                    && _.FileHash == fileHash);
                                // Test dans les éléments à ajouter
                                if (refInBase == null)
                                    refInBase = createdResources.SingleOrDefault(_ =>
                                        _.PaceRating == refLocal.PaceRating
                                        && _.Label == refLocal.Label
                                        && _.Description == refLocal.Description
                                        && _.FileHash == fileHash);
                                if (refInBase == null) // On doit créer une nouvelle PublishedResource
                                {
                                    refInBase = new PublishedResource(refLocal);
                                    context.PublishedResources.AddObject(refInBase);
                                    createdResources.Add(refInBase);
                                }
                                pAction.CutVideo.DefaultPublishedResource = refInBase;
                            }*/
                            if (pAction.Action.VideoId != null && cutVideoTasks?.Select(_ => _.Result).Any(_ => _.Key == pAction.Action.ActionId) == true)
                            {
                                string fileHash = cutVideoTasks.Select(_ => _.Result).Single(_ => _.Key == pAction.Action.ActionId).Value.fileHash;
                                string originalFileHash = cutVideoTasks.Select(_ => _.Result).Single(_ => _.Key == pAction.Action.ActionId).Value.originalFileHash;
                                string fileName = cutVideoTasks.Select(_ => _.Result).Single(_ => _.Key == pAction.Action.ActionId).Value.fileName;
                                CutVideo videoInBase = await context.CutVideos.SingleOrDefaultAsync(_ =>
                                    _.HashOriginalVideo == originalFileHash
                                    && _.Start == pAction.Action.Start
                                    && _.End == pAction.Action.Finish
                                    && (publication.Watermark == null ? _.Watermark == null : _.Watermark == publication.Watermark)
                                    && _.MinDuration == publication.MinDurationVideo);
                                if (videoInBase == null) // La CutVideo n'existe pas encore dans la base de données
                                {
                                    if (createdCutVideos.Any(_ => _.Hash == fileHash)) // La vidéos est déjà dans la liste des vidéos à ajouter
                                    {
                                        videoInBase = createdCutVideos.Single(_ => _.Hash == fileHash);
                                    }
                                    else
                                    {
                                        videoInBase = new CutVideo(pAction.Action)
                                        {
                                            Hash = fileHash,
                                            HashOriginalVideo = originalFileHash,
                                            Watermark = publication.Watermark,
                                            MinDuration = publication.MinDurationVideo,
                                            Extension = Path.GetExtension(fileName)
                                        };
                                        context.CutVideos.AddObject(videoInBase);
                                        createdCutVideos.Add(videoInBase);
                                    }
                                }
                                pAction.CutVideo = videoInBase;
                            }
                            // Gestion des référentiels
                            Dictionary<ProcessReferentialIdentifier, bool> refHasQuantity = await context.ProjectReferentials
                                .Where(_ => _.ProjectId == publication.ProjectId)
                                .ToDictionaryAsync(k => k.ReferentialId, v => v.HasQuantity);
                            await AddReferentialActions(context, pAction == null ? new TrackableCollection<Ref1Action>(pAction.DocumentationRefs1) : pAction.Action.Ref1, createdReferentials, pAction, refHasQuantity);
                            await AddReferentialActions(context, pAction == null ? new TrackableCollection<Ref2Action>(pAction.DocumentationRefs2) : pAction.Action.Ref2, createdReferentials, pAction, refHasQuantity);
                            await AddReferentialActions(context, pAction == null ? new TrackableCollection<Ref3Action>(pAction.DocumentationRefs3) : pAction.Action.Ref3, createdReferentials, pAction, refHasQuantity);
                            await AddReferentialActions(context, pAction == null ? new TrackableCollection<Ref4Action>(pAction.DocumentationRefs4) : pAction.Action.Ref4, createdReferentials, pAction, refHasQuantity);
                            await AddReferentialActions(context, pAction == null ? new TrackableCollection<Ref5Action>(pAction.DocumentationRefs5) : pAction.Action.Ref5, createdReferentials, pAction, refHasQuantity);
                            await AddReferentialActions(context, pAction == null ? new TrackableCollection<Ref6Action>(pAction.DocumentationRefs6) : pAction.Action.Ref6, createdReferentials, pAction, refHasQuantity);
                            await AddReferentialActions(context, pAction == null ? new TrackableCollection<Ref7Action>(pAction.DocumentationRefs7) : pAction.Action.Ref7, createdReferentials, pAction, refHasQuantity);
                        }
                        // On sauvegarde dans la base
                        context.Publications.ApplyChanges(publication);
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _traceManager.TraceError(ex, ex.Message);
                }
                // On supprime la tâche de publication de la liste
                // Nettoyage des vidéos découpées qui sont dans le buffer
                ClearVideoBuffer(publishedScenario);
                RemoveTaskFromCollection();
            }, publicationTaskProgress, tokenSource.Token);
            _runningPublications.TryAdd(publicationHistory.PublicationHistoryId, new PublishTaskInfos(publicationTask, tokenSource,
                publication.PublishMode == PublishModeEnum.Formation ? publication.Version.ToString() : null,
                publication.PublishMode == PublishModeEnum.Evaluation ? publication.Version.ToString() : null,
                publication.PublishMode == PublishModeEnum.Inspection ? publication.Version.ToString() : null));
            publicationTask.Start();
            return publicationHistory.PublicationHistoryId;
        }

        // Revoir les paramètres pour l'export des tâches principales uniquement
        public async Task<int> PublishMulti(List<Publication> publications,
            bool exportVideo,
            bool exportOnlyKeyTasksVideos)
        {
            foreach (var publication in publications)
                publication.Version = await GetFutureVersion(publication.ProcessId, publication.PublishMode, publication.IsMajor);
            // Create publication history
            var publicationHistory = new PublicationHistory
            {
                ProcessId = publications.First().ProcessId,
                State = PublicationStatus.Waiting,
                PublisherId = _securityContext.CurrentUser.User.UserId
            };
            publicationHistory.TrainingPublicationVersion = publications.FirstOrDefault(_ => _.PublishMode == PublishModeEnum.Formation)?.Version?.ToString();
            publicationHistory.EvaluationPublicationVersion = publications.FirstOrDefault(_ => _.PublishMode == PublishModeEnum.Evaluation)?.Version?.ToString();
            publicationHistory.InspectionPublicationVersion = publications.FirstOrDefault(_ => _.PublishMode == PublishModeEnum.Inspection)?.Version?.ToString();
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
            {
                context.PublicationHistories.AddObject(publicationHistory);
                await context.SaveChangesAsync();
                await _publicationRepository.RefreshPublicationProgress(new PublicationProgressEventArgs(publicationHistory.PublicationHistoryId,
                    PublicationStatus.Waiting,
                    "Creating publication task",
                    null,
                    true));
            }

            _traceManager.TraceDebug($"VideoBufferFolder directory : {VideoBufferDirectory}");

            async void OnStepProgressChanged(object sender, string e)
            {
                var progressInfo = _runningPublications.GetOrDefault(publicationHistory.PublicationHistoryId)?.ProgressInfo;
                if (progressInfo != null)
                {
                    progressInfo.Step = e;
                    await _publicationRepository.SendToSignalR(publicationHistory.PublicationHistoryId, progressInfo);
                }
            }
            async void OnProgressProgressChanged(object sender, double? e)
            {
                var progressInfo = _runningPublications.GetOrDefault(publicationHistory.PublicationHistoryId)?.ProgressInfo;
                if (progressInfo != null)
                {
                    progressInfo.Progress = e;
                    await _publicationRepository.SendToSignalR(publicationHistory.PublicationHistoryId, progressInfo);
                }
            }
            async void OnCanCancelProgressChanged(object sender, bool e)
            {
                var progressInfo = _runningPublications.GetOrDefault(publicationHistory.PublicationHistoryId)?.ProgressInfo;
                if (progressInfo != null)
                {
                    progressInfo.CanCancel = e;
                    await _publicationRepository.SendToSignalR(publicationHistory.PublicationHistoryId, progressInfo);
                }
            }
            async void OnStatusProgressChanged(object sender, PublicationStatus e)
            {
                var progressInfo = _runningPublications.GetOrDefault(publicationHistory.PublicationHistoryId)?.ProgressInfo;
                if (progressInfo != null)
                {
                    progressInfo.Status = e;
                    switch (e)
                    {
                        case PublicationStatus.Waiting:
                            progressInfo.Step = null;
                            progressInfo.Progress = null;
                            progressInfo.CanCancel = true;
                            break;
                        case PublicationStatus.InProgress:
                            break;
                        case PublicationStatus.Cancelled:
                        case PublicationStatus.InError:
                        case PublicationStatus.Completed:
                            progressInfo.Step = null;
                            progressInfo.Progress = null;
                            progressInfo.CanCancel = false;
                            break;
                    }
                    using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                    {
                        var currentPublication = await context.PublicationHistories.SingleAsync(p => p.PublicationHistoryId == publicationHistory.PublicationHistoryId);
                        currentPublication.State = e;
                        await context.SaveChangesAsync();
                    }
                    await _publicationRepository.SendToSignalR(publicationHistory.PublicationHistoryId, progressInfo);
                }
            }

            (IProgress<string> Step, IProgress<double?> Progress, IProgress<bool> CanCancel, IProgress<PublicationStatus> Status) publicationTaskProgress =
                (new Progress<string>(), new Progress<double?>(), new Progress<bool>(), new Progress<PublicationStatus>());
            ((Progress<string>)publicationTaskProgress.Step).ProgressChanged += OnStepProgressChanged;
            ((Progress<double?>)publicationTaskProgress.Progress).ProgressChanged += OnProgressProgressChanged;
            ((Progress<bool>)publicationTaskProgress.CanCancel).ProgressChanged += OnCanCancelProgressChanged;
            ((Progress<PublicationStatus>)publicationTaskProgress.Status).ProgressChanged += OnStatusProgressChanged;
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            void RemoveTaskFromCollection()
            {
                _runningPublications.TryRemove(publicationHistory.PublicationHistoryId, out PublishTaskInfos deleted);
            }
            async Task SetError(string errorMessage)
            {
                ((Progress<string>)publicationTaskProgress.Step).ProgressChanged -= OnStepProgressChanged;
                ((Progress<double?>)publicationTaskProgress.Progress).ProgressChanged -= OnProgressProgressChanged;
                ((Progress<bool>)publicationTaskProgress.CanCancel).ProgressChanged -= OnCanCancelProgressChanged;
                ((Progress<PublicationStatus>)publicationTaskProgress.Status).ProgressChanged -= OnStatusProgressChanged;
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                {
                    var currentPublication = await context.PublicationHistories.SingleAsync(p => p.PublicationHistoryId == publicationHistory.PublicationHistoryId);
                    currentPublication.ErrorMessage = errorMessage;
                    currentPublication.State = PublicationStatus.InError;
                    await context.SaveChangesAsync();
                }
                var progressInfo = _runningPublications.GetOrDefault(publicationHistory.PublicationHistoryId)?.ProgressInfo;
                if (progressInfo != null)
                {
                    progressInfo.ErrorMessage = errorMessage;
                    progressInfo.Status = PublicationStatus.InError;
                    await _publicationRepository.SendToSignalR(publicationHistory.PublicationHistoryId, progressInfo);
                }
            }
            async Task CancelPublication()
            {
                ((Progress<string>)publicationTaskProgress.Step).ProgressChanged -= OnStepProgressChanged;
                ((Progress<double?>)publicationTaskProgress.Progress).ProgressChanged -= OnProgressProgressChanged;
                ((Progress<bool>)publicationTaskProgress.CanCancel).ProgressChanged -= OnCanCancelProgressChanged;
                ((Progress<PublicationStatus>)publicationTaskProgress.Status).ProgressChanged -= OnStatusProgressChanged;
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                {
                    var currentPublication = await context.PublicationHistories.SingleAsync(p => p.PublicationHistoryId == publicationHistory.PublicationHistoryId);
                    currentPublication.State = PublicationStatus.Cancelled;
                    await context.SaveChangesAsync();
                }
                var progressInfo = _runningPublications.GetOrDefault(publicationHistory.PublicationHistoryId)?.ProgressInfo;
                if (progressInfo != null)
                {
                    progressInfo.Status = PublicationStatus.Cancelled;
                    progressInfo.Step = null;
                    progressInfo.Progress = null;
                    progressInfo.CanCancel = false;
                    await _publicationRepository.SendToSignalR(publicationHistory.PublicationHistoryId, progressInfo);
                }
            }
            async Task CompletePublication()
            {
                ((Progress<string>)publicationTaskProgress.Step).ProgressChanged -= OnStepProgressChanged;
                ((Progress<double?>)publicationTaskProgress.Progress).ProgressChanged -= OnProgressProgressChanged;
                ((Progress<bool>)publicationTaskProgress.CanCancel).ProgressChanged -= OnCanCancelProgressChanged;
                ((Progress<PublicationStatus>)publicationTaskProgress.Status).ProgressChanged -= OnStatusProgressChanged;
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                {
                    var currentPublication = await context.PublicationHistories.SingleAsync(p => p.PublicationHistoryId == publicationHistory.PublicationHistoryId);
                    currentPublication.State = PublicationStatus.Completed;
                    currentPublication.TrainingDocumentationId = publications.SingleOrDefault(p => p.PublishMode == PublishModeEnum.Formation)?.PublicationId;
                    currentPublication.EvaluationDocumentationId = publications.SingleOrDefault(p => p.PublishMode == PublishModeEnum.Evaluation)?.PublicationId;
                    currentPublication.InspectionDocumentationId = publications.SingleOrDefault(p => p.PublishMode == PublishModeEnum.Inspection)?.PublicationId;
                    await context.SaveChangesAsync();
                }
                var progressInfo = _runningPublications.GetOrDefault(publicationHistory.PublicationHistoryId)?.ProgressInfo;
                if (progressInfo != null)
                {
                    progressInfo.Status = PublicationStatus.Completed;
                    progressInfo.Step = null;
                    progressInfo.Progress = null;
                    progressInfo.CanCancel = false;
                    await _publicationRepository.SendToSignalR(publicationHistory.PublicationHistoryId, progressInfo);
                }
            }
            Scenario publishedScenario;
            User publisher;
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
            {
                var currentScenarioId = publications.First().ScenarioId;
                publishedScenario = await context.Scenarios
                    .Include(nameof(Scenario.Project))
                    .Include($"{nameof(Scenario.Project)}.{nameof(Project.Process)}")
                    .Include($"{nameof(Scenario.Project)}.{nameof(Project.Referentials)}")
                    .Include(nameof(Scenario.Actions))
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Thumbnail)}")
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Resource)}")
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Resource)}.{nameof(Resource.CloudFile)}")
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Category)}")
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Category)}.{nameof(ActionCategory.CloudFile)}")
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Video)}")
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Video)}.{nameof(Video.DefaultResource)}")
                    .Include($"{nameof(Scenario.Actions)}.{nameof(KAction.Video)}.{nameof(Video.DefaultResource)}.{nameof(Resource.CloudFile)}")
                    .SingleAsync(_ => _.ScenarioId == currentScenarioId);
                void LoadRef(Scenario scenario, ProcessReferentialIdentifier referentialId)
                {
                    if (scenario.Project.Referentials.Single(_ => _.ReferentialId == referentialId).IsEnabled)
                    {
                        foreach (var action in scenario.Actions)
                        {
                            context.LoadProperty(action, referentialId.ToString());
                            if (action.Refs(referentialId)?.Any() == true)
                                foreach (var refAction in action.Refs(referentialId))
                                {
                                    context.LoadProperty(refAction, referentialId.ToString());
                                    if (refAction.Referential.Hash != null)
                                        context.LoadProperty(refAction.Referential, nameof(CloudFile));
                                }
                        }
                    }
                }
                LoadRef(publishedScenario, ProcessReferentialIdentifier.Ref1);
                LoadRef(publishedScenario, ProcessReferentialIdentifier.Ref2);
                LoadRef(publishedScenario, ProcessReferentialIdentifier.Ref3);
                LoadRef(publishedScenario, ProcessReferentialIdentifier.Ref4);
                LoadRef(publishedScenario, ProcessReferentialIdentifier.Ref5);
                LoadRef(publishedScenario, ProcessReferentialIdentifier.Ref6);
                LoadRef(publishedScenario, ProcessReferentialIdentifier.Ref7);
                publisher = await context.Users.SingleAsync(_ => _.Username == _securityContext.CurrentUser.Username);
            }
            Task publicationTask = new Task(async (dummy) =>
            {
                publicationTaskProgress.CanCancel.Report(true);
                _traceManager.TraceDebug($"Test d'existence d'autre publication en cours...");
                // On vérifie qu'il n'y aie pas de publication en cours (Annulable)
                while (_runningPublications.Keys.IndexOf(publicationHistory.PublicationHistoryId) != 0)
                {
                    publicationTaskProgress.Step.Report("Une autre publication est en cours, veuillez patienter...");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    if (tokenSource.IsCancellationRequested)
                    {
                        await CancelPublication();
                        RemoveTaskFromCollection();
                        return;
                    }
                }
                _traceManager.TraceDebug($"Start new publications with Publication History Id: {publicationHistory.PublicationHistoryId}");
                (string OverlayTextVideo, bool AddProjectNameToWatermark, bool WBSMarkingIsEnabled, EHorizontalAlign HorizontalAlignement, EVerticalAlign VerticalAlignement) =
                    !exportVideo || string.IsNullOrEmpty(publications.First().Watermark) ?
                    (null, false, false, EHorizontalAlign.Center, EVerticalAlign.Top) :
                    JsonConvert.DeserializeObject<(string OverlayTextVideo, bool AddProjectNameToWatermark, bool WBSMarkingIsEnabled, EHorizontalAlign HorizontalAlignement, EVerticalAlign VerticalAlignement)>(publications.First().Watermark);
                Task<KeyValuePair<int, (string fileHash, string fileName, string originalFileHash)>>[] cutVideoTasks = new Task<KeyValuePair<int, (string fileHash, string filePath, string originalFileHash)>>[0];
                // On crée les tâches de découpage vidéo qui sont nécessaires (annulable)
                if (exportVideo)
                {
                    publicationTaskProgress.Status.Report(PublicationStatus.InProgress);
                    publicationTaskProgress.CanCancel.Report(true);
                    _traceManager.TraceDebug($"Création des tâches de découpe vidéo...");
                    cutVideoTasks = publishedScenario.Actions
                        .Where(_ => _.VideoId != null && (!exportOnlyKeyTasksVideos || _.IsKeyTaskManaged))
                        .Select(_ => CutVideo(_,
                            GetFinalWatermark(OverlayTextVideo,
                                /*AddProjectNameToWatermark ? publishedScenario.Project.Process.Label : */null,
                                /*WBSMarkingIsEnabled ? _.WBS : */null),
                            HorizontalAlignement,
                            VerticalAlignement,
                            publications.First().MinDurationVideo,
                            tokenSource.Token))
                        .ToArray();

                    try
                    {
                        int cutTasksCount = cutVideoTasks.Count();
                        int cutTasksCounter = 0;
                        foreach (var cutTask in cutVideoTasks)
                        {
                            publicationTaskProgress.Step.Report("Extraction des séquences depuis les vidéos brutes");
                            publicationTaskProgress.Progress.Report(Math.Round(cutTasksCounter * 100d / cutTasksCount));
                            cutTask.Start();
                            await Task.WhenAll(cutTask);
                            cutTasksCounter++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _traceManager.TraceInfo($"Exception when executing cutvideotasks with msg={ex.Message}");
                    }
                    if (cutVideoTasks.Any(_ => _.IsFaulted)) // Il y a une erreur sur un fichier (il faudrait pouvoir renvoyer la liste des fichiers en erreur à l'utilisateur)
                    {
                        // Nettoyage des vidéos découpées qui sont dans le buffer
                        ClearVideoBuffer(publishedScenario);
                        await SetError("Error on videos cutting task");
                        RemoveTaskFromCollection();
                        return;
                    }
                    if (cutVideoTasks.Any(_ => _.IsCanceled)) // On a annulé la publication
                    {
                        // Nettoyage des vidéos découpées qui sont dans le buffer
                        ClearVideoBuffer(publishedScenario);
                        await CancelPublication();
                        RemoveTaskFromCollection();
                        return;
                    }
                }
                // On crée les tâches de création des thumbnails (non annulable)
                publicationTaskProgress.Status.Report(PublicationStatus.InProgress);
                publicationTaskProgress.CanCancel.Report(false);
                publicationTaskProgress.Progress.Report(null);
                publicationTaskProgress.Step.Report("Création des vignettes...");
                _traceManager.TraceDebug($"Création des thumbnails...");
                // TODO: Les WBS ne correspondent plus. Il faudrait utiliser ceux des PublishedAction et ajouter un publishMode pour le dictionnaire
                List<Task<KeyValuePair<int, (string fileHash, DirectoryEnum directory, string fileName, string originalFileHash)>>> thumbnailsToCreate = publishedScenario.Actions
                    .Where(_ => _.Thumbnail != null)
                    .Select(_ => CreateThumbnail(_,
                        GetFinalWatermark(OverlayTextVideo,
                            /*AddProjectNameToWatermark ? publishedScenario.Project.Process.Label : */null,
                            /*WBSMarkingIsEnabled ? _.WBS : */null),
                        HorizontalAlignement,
                        VerticalAlignement))
                    .ToList();
                thumbnailsToCreate.AddRange(publications.SelectMany(p => p.PublishedActions)
                    .Where(_ => _.DocumentationAction != null && _.DocumentationAction.Thumbnail != null)
                    .Select(_ => CreateThumbnail(_.DocumentationAction,
                        GetFinalWatermark(OverlayTextVideo),
                        HorizontalAlignement,
                        VerticalAlignement))
                    .ToArray());
                try
                {
                    int thumbnailTasksCount = thumbnailsToCreate.Count;
                    int thumbnailTasksCounter = 0;
                    if (thumbnailsToCreate.Any())
                    {
                        publicationTaskProgress.Step.Report("Extraction des vignettes");
                        publicationTaskProgress.Progress.Report(Math.Round(thumbnailTasksCounter * 100d / thumbnailTasksCount));
                    }
                    thumbnailsToCreate.AsParallel().ForAll(thumbnailTask =>
                    {
                        thumbnailTask.Start();
                        thumbnailTask.Wait();
                        publicationTaskProgress.Progress.Report(Math.Round(++thumbnailTasksCounter * 100d / thumbnailTasksCount));
                    });
                }
                catch (Exception ex)
                {
                    _traceManager.TraceInfo($"Exception when executing CreateThumbnail with msg={ex.Message}");
                }
                if (thumbnailsToCreate.Any(_ => _.IsFaulted)) // Il y a une erreur sur un fichier (il faudrait pouvoir renvoyer la liste des fichiers en erreur à l'utilisateur)
                {
                    // Clean cut videos folder
                    ClearVideoBuffer(publishedScenario);
                    var failedFiles = thumbnailsToCreate.Where(task => task.IsFaulted && task.Exception.InnerException is ThumbnailCreationException)
                        .Select(task => ((ThumbnailCreationException)task.Exception.InnerException).Name).ToList();
                    var secContext = _securityContext;
                    var errorMessage = _localizationManager.GetStringFormat("View_PublishVideos_PublishThumbnailsError",
                        string.Join("\n", failedFiles.ToArray()));
                    await SetError(errorMessage);
                    RemoveTaskFromCollection();
                    return;
                }
                // On crée les tâches de copie des fichiers qui sont nécessaires (non annulable)
                // Ne pas effacer quand on est en Desktop
                publicationTaskProgress.Step.Report("Stockage des séquences vidéo dans l'espace de partage...");
                _traceManager.TraceDebug("Création des tâches de copie de fichiers...");
                // Moving thumbnails
                IEnumerable<(string oldName, string newName)> filesToMove = thumbnailsToCreate
                    .Where(_ => _.Result.Value.directory == DirectoryEnum.Uploaded)
                    .Select(_ => (oldName: _.Result.Value.fileName, newName: $"{_.Result.Value.fileHash}{Path.GetExtension(_.Result.Value.fileName)}"));
                foreach ((string oldName, string newName) in filesToMove)
                    await _fileProvider.Complete(oldName, newName);
                // Moving cutted videos
                Progress<double> copyProgress = new Progress<double>();
                copyProgress.ProgressChanged += (o, e) => publicationTaskProgress.Progress.Report(Math.Round(e, 2));
                filesToMove = cutVideoTasks.Select(_ => (oldName: _.Result.Value.fileName, newName: $"{_.Result.Value.fileHash}{Path.GetExtension(_.Result.Value.fileName)}"));
                await _fileProvider.Upload(filesToMove.Select(_ => (new FileInfo(Path.Combine(VideoBufferDirectory, _.oldName)), _.newName)).ToArray(), DirectoryEnum.Published, copyProgress);
                ClearVideoBuffer(publishedScenario);
                // On modifie les publications (non annulable)
                publicationTaskProgress.Step.Report("Enregistrement des publications...");
                var publicationPublishedDate = DateTime.Now;
                foreach (var publication in publications)
                {
                    publication.PublicationId = Guid.NewGuid();
                    publication.PublishedByUserId = publisher.UserId;
                    publication.PublishedDate = publicationPublishedDate;                    
                }
                try
                {
                    // Permet de stocker les éléments qui vont être ajoutés à la base (pour éviter les doublons)
                    List<PublishedActionCategory> createdCategories = new List<PublishedActionCategory>();
                    List<PublishedResource> createdResources = new List<PublishedResource>();
                    List<PublishedReferential> createdReferentials = new List<PublishedReferential>();
                    List<CutVideo> createdCutVideos = new List<CutVideo>();
                    List<PublishedFile> createdFiles = new List<PublishedFile>();

                    using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                    {
                        await LoadDocumentationResources(context, publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null));
                        await LoadDocumentationCategories(context, publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null));
                        await LoadDocumentationReferentials(context, publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null).SelectMany(p => p.DocumentationRefs1).Cast<IReferentialActionLink>());
                        await LoadDocumentationReferentials(context, publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null).SelectMany(p => p.DocumentationRefs2).Cast<IReferentialActionLink>());
                        await LoadDocumentationReferentials(context, publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null).SelectMany(p => p.DocumentationRefs3).Cast<IReferentialActionLink>());
                        await LoadDocumentationReferentials(context, publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null).SelectMany(p => p.DocumentationRefs4).Cast<IReferentialActionLink>());
                        await LoadDocumentationReferentials(context, publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null).SelectMany(p => p.DocumentationRefs5).Cast<IReferentialActionLink>());
                        await LoadDocumentationReferentials(context, publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null).SelectMany(p => p.DocumentationRefs6).Cast<IReferentialActionLink>());
                        await LoadDocumentationReferentials(context, publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null).SelectMany(p => p.DocumentationRefs7).Cast<IReferentialActionLink>());

                        List<CutVideo> dbVideos = await context.CutVideos.ToListAsync();
                        List<PublishedFile> dbFiles = await context.PublishedFiles.ToListAsync();
                        // On ajoute tous les fichiers générés à la base
                        var resources = publishedScenario.Actions
                            .Where(_ => _.Resource != null && _.Resource.CloudFile != null)
                            .Select(_ => _.Resource)
                            .Distinct()
                            .ToDictionary(_ => _.ResourceId, _ => _.CloudFile);
                        var categories = publishedScenario.Actions
                            .Where(_ => _.Category != null && _.Category.CloudFile != null)
                            .Select(_ => _.Category)
                            .Distinct()
                            .ToDictionary(_ => _.ActionCategoryId, _ => _.CloudFile);
                        var videoResources = publishedScenario.Actions
                            .Where(_ => _.Video != null && _.Video.DefaultResource != null && _.Video.DefaultResource.CloudFile != null)
                            .Select(_ => _.Video.DefaultResource)
                            .Distinct()
                            .ToDictionary(_ => _.ResourceId, _ => _.CloudFile);
                        var cloudFiles = thumbnailsToCreate.Select(_ => (_.Result.Value.fileHash, _.Result.Value.fileName))
                            .Union(publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null).Select(p => p.DocumentationAction).Where(_ => _.Thumbnail != null).Select(_ => (fileHash: _.Thumbnail.Hash, fileName: $"{_.Thumbnail.Hash}{_.Thumbnail.Extension}")))
                            .Union(resources.Select(_ => _.Value).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(categories.Select(_ => _.Value).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(videoResources.Select(_ => _.Value).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publishedScenario.Actions.Where(_ => _.Ref1?.Any() == true).SelectMany(_ => _.Ref1).Where(_ => _.Ref1.CloudFile != null).Select(_ => _.Ref1.CloudFile).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publishedScenario.Actions.Where(_ => _.Ref2?.Any() == true).SelectMany(_ => _.Ref2).Where(_ => _.Ref2.CloudFile != null).Select(_ => _.Ref2.CloudFile).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publishedScenario.Actions.Where(_ => _.Ref3?.Any() == true).SelectMany(_ => _.Ref3).Where(_ => _.Ref3.CloudFile != null).Select(_ => _.Ref3.CloudFile).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publishedScenario.Actions.Where(_ => _.Ref4?.Any() == true).SelectMany(_ => _.Ref4).Where(_ => _.Ref4.CloudFile != null).Select(_ => _.Ref4.CloudFile).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publishedScenario.Actions.Where(_ => _.Ref5?.Any() == true).SelectMany(_ => _.Ref5).Where(_ => _.Ref5.CloudFile != null).Select(_ => _.Ref5.CloudFile).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publishedScenario.Actions.Where(_ => _.Ref6?.Any() == true).SelectMany(_ => _.Ref6).Where(_ => _.Ref6.CloudFile != null).Select(_ => _.Ref6.CloudFile).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publishedScenario.Actions.Where(_ => _.Ref7?.Any() == true).SelectMany(_ => _.Ref7).Where(_ => _.Ref7.CloudFile != null).Select(_ => _.Ref7.CloudFile).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null).SelectMany(p => p.DocumentationRefs1.Where(_ => _.Referential.CloudFile != null).Select(_ => _.Referential.CloudFile)).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null).SelectMany(p => p.DocumentationRefs2.Where(_ => _.Referential.CloudFile != null).Select(_ => _.Referential.CloudFile)).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null).SelectMany(p => p.DocumentationRefs3.Where(_ => _.Referential.CloudFile != null).Select(_ => _.Referential.CloudFile)).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null).SelectMany(p => p.DocumentationRefs4.Where(_ => _.Referential.CloudFile != null).Select(_ => _.Referential.CloudFile)).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null).SelectMany(p => p.DocumentationRefs5.Where(_ => _.Referential.CloudFile != null).Select(_ => _.Referential.CloudFile)).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null).SelectMany(p => p.DocumentationRefs6.Where(_ => _.Referential.CloudFile != null).Select(_ => _.Referential.CloudFile)).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")))
                            .Union(publications.SelectMany(_ => _.PublishedActions).Where(p => p.Action == null).SelectMany(p => p.DocumentationRefs7.Where(_ => _.Referential.CloudFile != null).Select(_ => _.Referential.CloudFile)).Select(_ => (fileHash: _.Hash, fileName: $"{_.Hash}{_.Extension}")));
                        cloudFiles.ForEach(_ =>
                        {
                            if (!dbFiles.Any(f => f.Hash == _.fileHash) && !createdFiles.Any(f => f.Hash == _.fileHash))
                            {
                                PublishedFile file = new PublishedFile { Hash = _.fileHash, Extension = Path.GetExtension(_.fileName) };
                                context.PublishedFiles.AddObject(file);
                                createdFiles.Add(file);
                            }
                        });

                        foreach (var publication in publications)
                        {
                            // On définit les propriétés de navigation de chaque PublishedAction (Resource, Category, CutVideo, Thumbnail, Referentials)
                            foreach (var pAction in publication.PublishedActions)
                            {
                                // Update Action to have access to all tasks infos
                                if (pAction.Action != null)
                                    pAction.Action = publishedScenario.Actions.Single(_ => _.ActionId == pAction.Action.ActionId);

                                // Manage Thumbnail
                                // TODO: Manage Documentation task thumbnails
                                if (pAction.Action?.Thumbnail != null)
                                {
                                    var results = thumbnailsToCreate.Select(_ => _.Result);
                                    if (results.Any(_ => _.Key == pAction.Action.ActionId))
                                    {
                                        var thumbnailResults = results.Single(_ => _.Key == pAction.Action.ActionId);
                                        pAction.ThumbnailHash = thumbnailResults.Value.fileHash;
                                    }
                                }
                                else if (pAction.DocumentationAction?.Thumbnail != null)
                                {
                                    var results = thumbnailsToCreate.Select(_ => _.Result);
                                    if (results.Any(_ => _.Key == -pAction.DocumentationAction.DocumentationActionDraftId))
                                    {
                                        var thumbnailResults = results.Single(_ => _.Key == -pAction.DocumentationAction.DocumentationActionDraftId);
                                        pAction.ThumbnailHash = thumbnailResults.Value.fileHash;
                                    }
                                }

                                // Manage Resource
                                if (pAction.Action?.Resource != null || pAction.DocumentationAction?.DocumentationResource != null)
                                {
                                    Resource refLocal = pAction.Action?.Resource != null ? pAction.Action.Resource : pAction.DocumentationAction.DocumentationResource;
                                    PublishedResource refInBase = null;
                                    string fileHash = pAction.Action?.Resource != null ? pAction.Action.Resource.Hash : pAction.DocumentationAction.DocumentationResource.Hash;
                                    // Test dans la base
                                    refInBase = await context.PublishedResources.SingleOrDefaultAsync(_ =>
                                        _.PaceRating == refLocal.PaceRating
                                        && _.Label == refLocal.Label
                                        && (refLocal.Description == null ? _.Description == null : _.Description == refLocal.Description)
                                        && _.FileHash == fileHash);
                                    // Test dans les éléments à ajouter
                                    if (refInBase == null)
                                        refInBase = createdResources.SingleOrDefault(_ =>
                                            _.PaceRating == refLocal.PaceRating
                                            && _.Label == refLocal.Label
                                            && _.Description == refLocal.Description
                                            && _.FileHash == fileHash);
                                    if (refInBase == null) // On doit créer une nouvelle PublishedResource
                                    {
                                        refInBase = new PublishedResource(refLocal);
                                        context.PublishedResources.AddObject(refInBase);
                                        createdResources.Add(refInBase);
                                    }
                                    pAction.PublishedResource = refInBase;
                                }

                                // Manage Category
                                if (pAction.Action?.Category != null || pAction.DocumentationAction?.DocumentationCategory != null)
                                {
                                    ActionCategory catLocal = pAction.Action?.Category != null ? pAction.Action.Category : pAction.DocumentationAction.DocumentationCategory;
                                    PublishedActionCategory catInBase = null;
                                    string fileHash = pAction.Action?.Category != null ? pAction.Action.Category.Hash : pAction.DocumentationAction.DocumentationCategory.Hash;
                                    // Test dans la base
                                    catInBase = await context.PublishedActionCategories.SingleOrDefaultAsync(_ =>
                                        _.Label == catLocal.Label
                                        && (catLocal.Description == null ? _.Description == null : _.Description == catLocal.Description)
                                        && (catLocal.ActionTypeCode == null ? _.ActionTypeCode == null : _.ActionTypeCode == catLocal.ActionTypeCode)
                                        && (catLocal.ActionValueCode == null ? _.ActionTypeCode == null : _.ActionValueCode == catLocal.ActionValueCode)
                                        && _.FileHash == fileHash);
                                    // Test dans les éléments à ajouter
                                    if (catInBase == null)
                                        catInBase = createdCategories.SingleOrDefault(_ =>
                                            _.Label == catLocal.Label
                                            && _.Description == catLocal.Description
                                            && _.ActionTypeCode == catLocal.ActionTypeCode
                                            && _.ActionValueCode == catLocal.ActionValueCode
                                            && _.FileHash == fileHash);
                                    if (catInBase == null) // On doit créer une nouvelle PublishedActionCategory
                                    {
                                        catInBase = new PublishedActionCategory(catLocal);
                                        context.PublishedActionCategories.AddObject(catInBase);
                                        createdCategories.Add(catInBase);
                                    }
                                    pAction.PublishedActionCategory = catInBase;
                                }

                                // TODO : Add DefaultPublishedResource property to CutVideo
                                /*if (pAction.Action.Video?.DefaultResource != null)
                                {
                                    Resource refLocal = pAction.Action.Video.DefaultResource;
                                    PublishedResource refInBase = null;
                                    string fileHash = pAction.Action.Video.DefaultResource.Hash;
                                    // Test dans la base
                                    refInBase = await context.PublishedResources.SingleOrDefaultAsync(_ =>
                                        _.PaceRating == refLocal.PaceRating
                                        && _.Label == refLocal.Label
                                        && (refLocal.Description == null ? _.Description == null : _.Description == refLocal.Description)
                                        && _.FileHash == fileHash);
                                    // Test dans les éléments à ajouter
                                    if (refInBase == null)
                                        refInBase = createdResources.SingleOrDefault(_ =>
                                            _.PaceRating == refLocal.PaceRating
                                            && _.Label == refLocal.Label
                                            && _.Description == refLocal.Description
                                            && _.FileHash == fileHash);
                                    if (refInBase == null) // On doit créer une nouvelle PublishedResource
                                    {
                                        refInBase = new PublishedResource(refLocal);
                                        context.PublishedResources.AddObject(refInBase);
                                        createdResources.Add(refInBase);
                                    }
                                    pAction.CutVideo.DefaultPublishedResource = refInBase;
                                }*/

                                // Manage Video
                                if (pAction.Action?.VideoId != null && cutVideoTasks?.Select(_ => _.Result).Any(_ => _.Key == pAction.Action.ActionId) == true)
                                {
                                    string fileHash = cutVideoTasks.Select(_ => _.Result).Single(_ => _.Key == pAction.Action.ActionId).Value.fileHash;
                                    string originalFileHash = cutVideoTasks.Select(_ => _.Result).Single(_ => _.Key == pAction.Action.ActionId).Value.originalFileHash;
                                    string fileName = cutVideoTasks.Select(_ => _.Result).Single(_ => _.Key == pAction.Action.ActionId).Value.fileName;
                                    CutVideo videoInBase = await context.CutVideos.SingleOrDefaultAsync(_ => _.Hash == fileHash ||
                                        (_.HashOriginalVideo == originalFileHash
                                        && _.Start == pAction.Action.Start
                                        && _.End == pAction.Action.Finish
                                        && (publication.Watermark == null ? _.Watermark == null : _.Watermark == publication.Watermark)
                                        && _.MinDuration == publication.MinDurationVideo));
                                    if (videoInBase == null) // La CutVideo n'existe pas encore dans la base de données
                                    {
                                        if (createdCutVideos.Any(_ => _.Hash == fileHash)) // La vidéos est déjà dans la liste des vidéos à ajouter
                                        {
                                            videoInBase = createdCutVideos.Single(_ => _.Hash == fileHash);
                                        }
                                        else
                                        {
                                            videoInBase = new CutVideo(pAction.Action)
                                            {
                                                Hash = fileHash,
                                                HashOriginalVideo = originalFileHash,
                                                Watermark = publication.Watermark,
                                                MinDuration = publication.MinDurationVideo,
                                                Extension = Path.GetExtension(fileName)
                                            };
                                            context.CutVideos.AddObject(videoInBase);
                                            createdCutVideos.Add(videoInBase);
                                        }
                                    }
                                    pAction.CutVideo = videoInBase;
                                }

                                // Manage referentials
                                Dictionary<ProcessReferentialIdentifier, bool> refHasQuantity = await context.DocumentationReferentials
                                    .Where(_ => _.ProcessId == publication.ProcessId)
                                    .ToDictionaryAsync(k => k.ReferentialId, v => v.HasQuantity);

                                await AddReferentialActions(context, pAction.Action?.Ref1 ?? new TrackableCollection<Ref1Action>(pAction.DocumentationRefs1), createdReferentials, pAction, refHasQuantity);
                                await AddReferentialActions(context, pAction.Action?.Ref2 ?? new TrackableCollection<Ref2Action>(pAction.DocumentationRefs2), createdReferentials, pAction, refHasQuantity);
                                await AddReferentialActions(context, pAction.Action?.Ref3 ?? new TrackableCollection<Ref3Action>(pAction.DocumentationRefs3), createdReferentials, pAction, refHasQuantity);
                                await AddReferentialActions(context, pAction.Action?.Ref4 ?? new TrackableCollection<Ref4Action>(pAction.DocumentationRefs4), createdReferentials, pAction, refHasQuantity);
                                await AddReferentialActions(context, pAction.Action?.Ref5 ?? new TrackableCollection<Ref5Action>(pAction.DocumentationRefs5), createdReferentials, pAction, refHasQuantity);
                                await AddReferentialActions(context, pAction.Action?.Ref6 ?? new TrackableCollection<Ref6Action>(pAction.DocumentationRefs6), createdReferentials, pAction, refHasQuantity);
                                await AddReferentialActions(context, pAction.Action?.Ref7 ?? new TrackableCollection<Ref7Action>(pAction.DocumentationRefs7), createdReferentials, pAction, refHasQuantity);
                            }

                            // On duplique les éléments issus de la publication précédente si mineure
                            IEnumerable<Training> previousVersionTrainings = null;
                            IEnumerable<Qualification> previousVersionEvaluations = null;
                            if (!publication.IsMajor && publication.PublishMode != PublishModeEnum.Inspection)
                            {
                                var previousVersion = GetMinorPreviousVersion(publication.Version);
                                var previousVersionPublicationId = await context.Publications
                                    .Where(p => p.ProcessId == publication.ProcessId
                                                && p.PublishMode == publication.PublishMode
                                                && p.Version == previousVersion)
                                    .Select(p => p.PublicationId)
                                    .SingleAsync();
                                if (publication.PublishMode == PublishModeEnum.Formation)
                                {
                                    var versionMajor = $"{publication.Version.Split('.').First()}.";
                                    // Users reads publication
                                    var lastUserReadPublications = await context.Publications
                                        .Include(nameof(Publication.Readers))
                                        .Where(p => p.ProcessId == publication.ProcessId
                                            && p.Version.StartsWith(versionMajor))
                                        .SelectMany(p => p.Readers)
                                        .Where(r => r.ReadDate != null)
                                        .GroupBy(urp => urp.UserId)
                                        .ToArrayAsync();
                                    publication.Readers.AddRange(lastUserReadPublications.Select(gurp => new UserReadPublication
                                    {
                                        UserId = gurp.Key,
                                        ReadDate = null,
                                        PreviousVersionPublicationId = gurp.MaxBy(_ => _.ReadDate).First().PublicationId
                                    }));
                                    // Trainings
                                    var previousVersionTrainingsNotGrouped = await context.Trainings
                                        .Include(nameof(Training.ValidationTrainings))
                                        .Where(t => t.PublicationId == previousVersionPublicationId
                                                    && !t.IsDeleted
                                                    && t.EndDate != null)
                                        .ToArrayAsync();
                                    previousVersionTrainings = previousVersionTrainingsNotGrouped
                                        .GroupBy(t => t.UserId)
                                        .ToDictionary(t => t.Key, t => t.ToArray())
                                        .Select(kvp => kvp.Value.MaxBy(t => t.StartDate).First());
                                    publication.Trainings.AddRange(previousVersionTrainings.Select(t => new Training
                                    {
                                        StartDate = publication.PublishedDate,
                                        EndDate = publication.PublishedDate,
                                        UserId = t.UserId,
                                        PreviousVersionTrainingId = t.TrainingId,
                                        ValidationTrainings = new TrackableCollection<ValidationTraining>(t.ValidationTrainings
                                            .Where(vt => !vt.IsDeleted)
                                            .GroupBy(vt => vt.PublishedActionId)
                                            .Select(kvp => kvp.MaxBy(vt => vt.StartDate).First())
                                            .Select(vt => new ValidationTraining
                                            {
                                                StartDate = publication.PublishedDate,
                                                EndDate = publication.PublishedDate,
                                                PublishedActionId = vt.PublishedActionId,
                                                TrainerId = vt.TrainerId
                                            }))
                                    }));
                                }
                                if (publication.PublishMode == PublishModeEnum.Evaluation)
                                {
                                    // Evaluations
                                    var previousVersionEvaluationsNotGrouped = await context.Qualifications
                                        .Include(nameof(Qualification.QualificationSteps))
                                        .Where(q => q.PublicationId == previousVersionPublicationId
                                                    && !q.IsDeleted
                                                    && q.EndDate != null
                                                    && q.IsQualified == true)
                                        .ToArrayAsync();
                                    previousVersionEvaluations = previousVersionEvaluationsNotGrouped
                                        .GroupBy(q => q.UserId)
                                        .ToDictionary(q => q.Key, q => q.ToArray())
                                        .Select(kvp => kvp.Value.MaxBy(q => q.StartDate).First());
                                    publication.Qualifications.AddRange(previousVersionEvaluations.Select(q => new Qualification
                                    {
                                        StartDate = publication.PublishedDate,
                                        EndDate = publication.PublishedDate,
                                        UserId = q.UserId,
                                        Result = q.Result,
                                        IsQualified = q.IsQualified,
                                        Comment = q.Comment,
                                        PreviousVersionQualificationId = q.QualificationId,
                                        QualificationSteps = new TrackableCollection<QualificationStep>(q.QualificationSteps
                                            .Where(vt => !vt.IsDeleted)
                                            .GroupBy(vt => vt.PublishedActionId)
                                            .Select(kvp => kvp.MaxBy(qs => qs.Date).First())
                                            .Select(qs => new QualificationStep
                                            {
                                                Date = publication.PublishedDate,
                                                Comment = qs.Comment,
                                                QualificationReasonId = qs.QualificationReasonId,
                                                PublishedActionId = qs.PublishedActionId,
                                                QualifierId = qs.QualifierId,
                                                IsQualified = qs.IsQualified
                                            }))
                                    }));
                                }
                            }

                            // On sauvegarde dans la base
                            context.Publications.ApplyChanges(publication);
                            await context.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _traceManager.TraceError(ex, ex.Message);
                    // On supprime la tâche de publication de la liste
                    // Nettoyage des vidéos découpées qui sont dans le buffer
                    ClearVideoBuffer(publishedScenario);
                    await SetError(ex.Message);
                    RemoveTaskFromCollection();
                    return;
                }
                // On supprime la tâche de publication de la liste
                // Nettoyage des vidéos découpées qui sont dans le buffer
                ClearVideoBuffer(publishedScenario);
                await CompletePublication();
                RemoveTaskFromCollection();
            }, publicationTaskProgress, tokenSource.Token);
            _runningPublications.TryAdd(publicationHistory.PublicationHistoryId, new PublishTaskInfos(publicationTask, tokenSource,
                publications.FirstOrDefault(_ => _.PublishMode == PublishModeEnum.Formation)?.Version.ToString(),
                publications.FirstOrDefault(_ => _.PublishMode == PublishModeEnum.Evaluation)?.Version.ToString(),
                publications.FirstOrDefault(_ => _.PublishMode == PublishModeEnum.Inspection)?.Version.ToString()));
            publicationTask.Start();
            return publicationHistory.PublicationHistoryId;
        }

        public async Task<int> FakingPublication()
        {
            PublicationHistory publicationHistory = null;
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
            {
                // Create publication history
                publicationHistory = new PublicationHistory
                {
                    ProcessId = (await context.Procedures.FirstAsync()).ProcessId,
                    State = PublicationStatus.Waiting,
                    PublisherId = _securityContext.CurrentUser.User.UserId
                };
                context.PublicationHistories.AddObject(publicationHistory);
                await context.SaveChangesAsync();
                await _publicationRepository.RefreshPublicationProgress(new PublicationProgressEventArgs(publicationHistory.PublicationHistoryId,
                    PublicationStatus.Waiting,
                    "Creating publication task",
                    0.0,
                    false));
            }

            async void OnStepProgressChanged(object sender, string e)
            {
                var progressInfo = _runningPublications.GetOrDefault(publicationHistory.PublicationHistoryId)?.ProgressInfo;
                if (progressInfo != null)
                {
                    progressInfo.Step = e;
                    await _publicationRepository.SendToSignalR(publicationHistory.PublicationHistoryId, progressInfo);
                }
            }
            async void OnProgressProgressChanged(object sender, double? e)
            {
                var progressInfo = _runningPublications.GetOrDefault(publicationHistory.PublicationHistoryId)?.ProgressInfo;
                if (progressInfo != null)
                {
                    progressInfo.Progress = e;
                    await _publicationRepository.SendToSignalR(publicationHistory.PublicationHistoryId, progressInfo);
                }
            }
            async void OnCanCancelProgressChanged(object sender, bool e)
            {
                var progressInfo = _runningPublications.GetOrDefault(publicationHistory.PublicationHistoryId)?.ProgressInfo;
                if (progressInfo != null)
                {
                    progressInfo.CanCancel = e;
                    await _publicationRepository.SendToSignalR(publicationHistory.PublicationHistoryId, progressInfo);
                }
            }
            async void OnStatusProgressChanged(object sender, PublicationStatus e)
            {
                var progressInfo = _runningPublications.GetOrDefault(publicationHistory.PublicationHistoryId)?.ProgressInfo;
                if (progressInfo != null)
                {
                    progressInfo.Status = e;
                    switch (e)
                    {
                        case PublicationStatus.Waiting:
                            progressInfo.Step = null;
                            progressInfo.Progress = null;
                            progressInfo.CanCancel = true;
                            break;
                        case PublicationStatus.InProgress:
                            break;
                        case PublicationStatus.Cancelled:
                        case PublicationStatus.InError:
                        case PublicationStatus.Completed:
                            progressInfo.Step = null;
                            progressInfo.Progress = null;
                            progressInfo.CanCancel = false;
                            break;
                    }
                    using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                    {
                        var currentPublication = await context.PublicationHistories.SingleAsync(p => p.PublicationHistoryId == publicationHistory.PublicationHistoryId);
                        currentPublication.State = e;
                        await context.SaveChangesAsync();
                    }
                    await _publicationRepository.SendToSignalR(publicationHistory.PublicationHistoryId, progressInfo);
                }
            }

            (IProgress<string> Step, IProgress<double?> Progress, IProgress<bool> CanCancel, IProgress<PublicationStatus> Status) publicationTaskProgress =
                (new Progress<string>(), new Progress<double?>(), new Progress<bool>(), new Progress<PublicationStatus>());
            ((Progress<string>)publicationTaskProgress.Step).ProgressChanged += OnStepProgressChanged;
            ((Progress<double?>)publicationTaskProgress.Progress).ProgressChanged += OnProgressProgressChanged;
            ((Progress<bool>)publicationTaskProgress.CanCancel).ProgressChanged += OnCanCancelProgressChanged;
            ((Progress<PublicationStatus>)publicationTaskProgress.Status).ProgressChanged += OnStatusProgressChanged;
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            void RemoveTaskFromCollection()
            {
                _runningPublications.TryRemove(publicationHistory.PublicationHistoryId, out PublishTaskInfos deleted);
            }
            async Task CancelPublication()
            {
                ((Progress<string>)publicationTaskProgress.Step).ProgressChanged -= OnStepProgressChanged;
                ((Progress<double?>)publicationTaskProgress.Progress).ProgressChanged -= OnProgressProgressChanged;
                ((Progress<bool>)publicationTaskProgress.CanCancel).ProgressChanged -= OnCanCancelProgressChanged;
                ((Progress<PublicationStatus>)publicationTaskProgress.Status).ProgressChanged -= OnStatusProgressChanged;
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                {
                    var currentPublication = await context.PublicationHistories.SingleAsync(p => p.PublicationHistoryId == publicationHistory.PublicationHistoryId);
                    currentPublication.State = PublicationStatus.Cancelled;
                    await context.SaveChangesAsync();
                }
                var progressInfo = _runningPublications.GetOrDefault(publicationHistory.PublicationHistoryId)?.ProgressInfo;
                if (progressInfo != null)
                {
                    progressInfo.Status = PublicationStatus.Cancelled;
                    progressInfo.Step = null;
                    progressInfo.Progress = null;
                    progressInfo.CanCancel = false;
                    await _publicationRepository.SendToSignalR(publicationHistory.PublicationHistoryId, progressInfo);
                }
            }
            async Task CompletePublication()
            {
                ((Progress<string>)publicationTaskProgress.Step).ProgressChanged -= OnStepProgressChanged;
                ((Progress<double?>)publicationTaskProgress.Progress).ProgressChanged -= OnProgressProgressChanged;
                ((Progress<bool>)publicationTaskProgress.CanCancel).ProgressChanged -= OnCanCancelProgressChanged;
                ((Progress<PublicationStatus>)publicationTaskProgress.Status).ProgressChanged -= OnStatusProgressChanged;
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                {
                    var currentPublication = await context.PublicationHistories.SingleAsync(p => p.PublicationHistoryId == publicationHistory.PublicationHistoryId);
                    currentPublication.State = PublicationStatus.Completed;
                    await context.SaveChangesAsync();
                }
                var progressInfo = _runningPublications.GetOrDefault(publicationHistory.PublicationHistoryId)?.ProgressInfo;
                if (progressInfo != null)
                {
                    progressInfo.Status = PublicationStatus.Completed;
                    progressInfo.Step = null;
                    progressInfo.Progress = null;
                    progressInfo.CanCancel = false;
                    await _publicationRepository.SendToSignalR(publicationHistory.PublicationHistoryId, progressInfo);
                }
            }

            Task publicationTask = new Task(async (dummy) =>
            {
                publicationTaskProgress.CanCancel.Report(true);
                _traceManager.TraceDebug($"Test d'existence d'autre publication en cours...");
                // On vérifie qu'il n'y aie pas de publication en cours (Annulable)
                while (_runningPublications.Keys.IndexOf(publicationHistory.PublicationHistoryId) != 0)
                {
                    publicationTaskProgress.Step.Report("Une autre publication est en cours, veuillez patienter...");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    if (tokenSource.IsCancellationRequested)
                    {
                        await CancelPublication();
                        RemoveTaskFromCollection();
                        return;
                    }
                }
                _traceManager.TraceDebug($"Start new publications with Publication History Id: {publicationHistory.PublicationHistoryId}");
                publicationTaskProgress.Status.Report(PublicationStatus.InProgress);
                publicationTaskProgress.Step.Report("Faking a publication... :-)");
                double fakeProgress = 0;
                while (fakeProgress < 100)
                {
                    if (tokenSource.IsCancellationRequested)
                    {
                        await CancelPublication();
                        RemoveTaskFromCollection();
                        return;
                    }
                    await Task.Delay(250);
                    publicationTaskProgress.Progress.Report(++fakeProgress);
                    Console.WriteLine($"{fakeProgress}%");
                }
                await CompletePublication();
                RemoveTaskFromCollection();
            }, publicationTaskProgress, tokenSource.Token);

            _runningPublications.TryAdd(publicationHistory.PublicationHistoryId, new PublishTaskInfos(publicationTask, tokenSource, "1.0", "1.0", "1.0"));
            publicationTask.Start();
            return publicationHistory.PublicationHistoryId;
        }

        string GetMinorPreviousVersion(string currentVersionString)
        {
            var currentVersion = new Version(currentVersionString);
            return new Version(currentVersion.Major, currentVersion.Minor - 1).ToString(2);
        }

        public async Task<string> GetCurrentVersion(int processId, PublishModeEnum publishMode)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
            {
                var publicationVersions = await context.Publications
                    .Where(p => p.ProcessId == processId
                                && p.PublishMode == publishMode)
                    .Select(p => p.Version)
                    .ToArrayAsync();

                if (!publicationVersions.Any())
                    return null;

                return publicationVersions.Select(v => new Version(v))
                    .MaxBy(v => v)
                    .First()
                    .ToString(2);
            }
        }

        public async Task<string> GetFutureVersion(int processId, PublishModeEnum publishMode, bool isMajor)
        {
            var currentVersionString = await GetCurrentVersion(processId, publishMode);
            if (currentVersionString == null)
                return new Version(1, 0).ToString(2);

            var currentVersion = new Version(currentVersionString);

            if (isMajor)
                return new Version(currentVersion.Major + 1, 0).ToString(2);
            return new Version(currentVersion.Major, currentVersion.Minor + 1).ToString(2);
        }

        async Task LoadDocumentationReferentials(KsmedEntities context, IEnumerable<IReferentialActionLink> documentationReferentialActions)
        {
            foreach (var refAction in documentationReferentialActions)
            {
                IMultipleActionReferential refLocal = null;
                switch (refAction)
                {
                    case Ref1Action ref1Action:
                        refLocal = await context.Refs1.Include(nameof(IMultipleActionReferential.CloudFile)).AsNoTracking().FirstAsync(_ => _.RefId == refAction.ReferentialId);
                        refAction.Referential = refLocal;
                        break;
                    case Ref2Action ref2Action:
                        refLocal = await context.Refs2.Include(nameof(IMultipleActionReferential.CloudFile)).AsNoTracking().FirstAsync(_ => _.RefId == refAction.ReferentialId);
                        refAction.Referential = refLocal;
                        break;
                    case Ref3Action ref3Action:
                        refLocal = await context.Refs3.Include(nameof(IMultipleActionReferential.CloudFile)).AsNoTracking().FirstAsync(_ => _.RefId == refAction.ReferentialId);
                        refAction.Referential = refLocal;
                        break;
                    case Ref4Action ref4Action:
                        refLocal = await context.Refs4.Include(nameof(IMultipleActionReferential.CloudFile)).AsNoTracking().FirstAsync(_ => _.RefId == refAction.ReferentialId);
                        refAction.Referential = refLocal;
                        break;
                    case Ref5Action ref5Action:
                        refLocal = await context.Refs5.Include(nameof(IMultipleActionReferential.CloudFile)).AsNoTracking().FirstAsync(_ => _.RefId == refAction.ReferentialId);
                        refAction.Referential = refLocal;
                        break;
                    case Ref6Action ref6Action:
                        refLocal = await context.Refs6.Include(nameof(IMultipleActionReferential.CloudFile)).AsNoTracking().FirstAsync(_ => _.RefId == refAction.ReferentialId);
                        refAction.Referential = refLocal;
                        break;
                    case Ref7Action ref7Action:
                        refLocal = await context.Refs7.Include(nameof(IMultipleActionReferential.CloudFile)).AsNoTracking().FirstAsync(_ => _.RefId == refAction.ReferentialId);
                        refAction.Referential = refLocal;
                        break;
                }
            }

        }

        async Task LoadDocumentationResources(KsmedEntities context, IEnumerable<PublishedAction> documentationActions)
        {
            foreach (var docAction in documentationActions.Where(_ => _.DocumentationAction.ResourceId != null))
            {
                Resource resLocal = await context.Resources.AsNoTracking().FirstAsync(_ => _.ResourceId == docAction.DocumentationAction.ResourceId);
                docAction.DocumentationAction.DocumentationResource = resLocal;
            }

        }

        async Task LoadDocumentationCategories(KsmedEntities context, IEnumerable<PublishedAction> documentationActions)
        {
            foreach (var docAction in documentationActions.Where(_ => _.DocumentationAction.ActionCategoryId != null))
            {
                ActionCategory catLocal = await context.ActionCategories.AsNoTracking().FirstAsync(_ => _.ActionCategoryId == docAction.DocumentationAction.ActionCategoryId);
                docAction.DocumentationAction.DocumentationCategory = catLocal;
            }

        }

        async Task AddReferentialActions<RefActionType>(KsmedEntities context,
            TrackableCollection<RefActionType> refs,
            List<PublishedReferential> createdRefs,
            PublishedAction pAction,
            Dictionary<ProcessReferentialIdentifier, bool> refHasQuantity)
            where RefActionType : IReferentialActionLink
        {
            foreach (RefActionType refAction in refs)
            {
                IMultipleActionReferential refLocal = refAction.Referential;
                PublishedReferential refInBase = null;
                string fileHash = refLocal.Hash;
                // Test dans la base
                refInBase = await context.PublishedReferentials.SingleOrDefaultAsync(_ =>
                    _.Label == refLocal.Label
                    && (refLocal.Description == null ? _.Description == null : _.Description == refLocal.Description)
                    && _.FileHash == fileHash);
                // Test dans les éléments à ajouter
                if (refInBase == null)
                    refInBase = createdRefs.SingleOrDefault(_ =>
                        _.Label == refLocal.Label
                        && _.Description == refLocal.Description
                        && _.FileHash == fileHash);
                if (refInBase == null) // On doit créer un nouveau PublishedReferential
                {
                    refInBase = new PublishedReferential(refLocal);
                    context.PublishedReferentials.AddObject(refInBase);
                    createdRefs.Add(refInBase);
                }
                PublishedReferentialAction publishedRefAction = new PublishedReferentialAction(refAction, refHasQuantity)
                {
                    PublishedAction = pAction,
                    PublishedReferential = refInBase
                };
                context.PublishedReferentialActions.AddObject(publishedRefAction);
            }
        }

        public async Task<PublicationHistory[]> GetPublicationHistories(int pageNumber = 0, int pageSize = 10)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
            {
                var publications = context.PublicationHistories
                    .Include(nameof(PublicationHistory.PublishedProcess))
                    .Include(nameof(PublicationHistory.TrainingPublication))
                    .Include(nameof(PublicationHistory.InspectionPublication))
                    .Include(nameof(PublicationHistory.EvaluationPublication))
                    .Include(nameof(PublicationHistory.Publisher))
                    .OrderByDescending(u => u.Timestamp);

                // Get versions
                foreach(var publication in publications)
                {
                    if (publication.TrainingDocumentationId == null
                        && publication.EvaluationDocumentationId == null
                        && publication.InspectionDocumentationId == null)
                    {
                        if (_runningPublications.ContainsKey(publication.PublicationHistoryId))
                        {
                            publication.TrainingPublicationVersion = _runningPublications[publication.PublicationHistoryId].TrainingPublicationVersion;
                            publication.EvaluationPublicationVersion = _runningPublications[publication.PublicationHistoryId].EvaluationPublicationVersion;
                            publication.InspectionPublicationVersion = _runningPublications[publication.PublicationHistoryId].InspectionPublicationVersion;
                        }
                    }
                    else
                    {
                        publication.TrainingPublicationVersion = publication.TrainingPublication?.Version.ToString();
                        publication.EvaluationPublicationVersion = publication.EvaluationPublication?.Version.ToString();
                        publication.InspectionPublicationVersion = publication.InspectionPublication?.Version.ToString();
                    }
                }

                if (pageNumber == 0)
                    return await publications.ToArrayAsync();
                
                // Pagination is used to avoid to retrieve too much data
                return await publications.ToPagedQuery(pageNumber, pageSize).ToArrayAsync();
            }
        }

        public async Task<PublicationHistory> GetPublicationHistory(int publicationHistoryId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
            {
                var result = await context.PublicationHistories
                    .Include(nameof(PublicationHistory.PublishedProcess))
                    .Include(nameof(PublicationHistory.TrainingPublication))
                    .Include(nameof(PublicationHistory.InspectionPublication))
                    .Include(nameof(PublicationHistory.EvaluationPublication))
                    .Include(nameof(PublicationHistory.Publisher))
                    .SingleOrDefaultAsync(u => u.PublicationHistoryId == publicationHistoryId);
                // Get versions
                if (result.TrainingDocumentationId == null
                    && result.EvaluationDocumentationId == null
                    && result.InspectionDocumentationId == null)
                {
                    if (_runningPublications.ContainsKey(publicationHistoryId))
                    {
                        result.TrainingPublicationVersion = _runningPublications[publicationHistoryId].TrainingPublicationVersion;
                        result.EvaluationPublicationVersion = _runningPublications[publicationHistoryId].EvaluationPublicationVersion;
                        result.InspectionPublicationVersion = _runningPublications[publicationHistoryId].InspectionPublicationVersion;
                    }
                }
                else
                {
                    result.TrainingPublicationVersion = result.TrainingPublication?.Version.ToString();
                    result.EvaluationPublicationVersion = result.EvaluationPublication?.Version.ToString();
                    result.InspectionPublicationVersion = result.InspectionPublication?.Version.ToString();
                }
                return result;
            }
        }

        public async Task<bool> CancelPublication(int publicationHistoryId)
        {
            if (_runningPublications.ContainsKey(publicationHistoryId))
            {
                _runningPublications[publicationHistoryId].Cts.Cancel();
                return _runningPublications.TryRemove(publicationHistoryId, out PublishTaskInfos deleted);
            }
            else
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
                {
                    var currentPublication = await context.PublicationHistories.SingleAsync(p => p.PublicationHistoryId == publicationHistoryId);
                    currentPublication.State = PublicationStatus.Cancelled;
                    await context.SaveChangesAsync();
                }
                var progressInfo = _runningPublications.GetOrDefault(publicationHistoryId)?.ProgressInfo;
                if (progressInfo != null)
                {
                    progressInfo.Status = PublicationStatus.Cancelled;
                    progressInfo.Step = null;
                    progressInfo.Progress = null;
                    progressInfo.CanCancel = false;
                    await _publicationRepository.SendToSignalR(publicationHistoryId, progressInfo);
                }
            }
            return false;
        }

        public PublishTaskProgressInfos GetProgress(int publicationHistoryId)
        {
            if (!_runningPublications.ContainsKey(publicationHistoryId))
                return null;
            var errorMessage = _runningPublications[publicationHistoryId].ProgressInfo.ErrorMessage;
            if (errorMessage != null)
            {
                _runningPublications.TryRemove(publicationHistoryId, out PublishTaskInfos deleted);
                throw new OperationCanceledException(errorMessage);
            }
            return new PublishTaskProgressInfos
            (
                _runningPublications[publicationHistoryId].ProgressInfo.Status,
                _runningPublications[publicationHistoryId].ProgressInfo.Step,
                _runningPublications[publicationHistoryId].ProgressInfo.Progress,
                _runningPublications[publicationHistoryId].ProgressInfo.CanCancel,
                _runningPublications[publicationHistoryId].ProgressInfo.ErrorMessage
            );
        }

        public Dictionary<int, PublishTaskProgressInfos> GetProgresses() =>
            _runningPublications.ToDictionary(x => x.Key, x => new PublishTaskProgressInfos(
                x.Value.ProgressInfo.Status,
                x.Value.ProgressInfo.Step,
                x.Value.ProgressInfo.Progress,
                x.Value.ProgressInfo.CanCancel,
                x.Value.ProgressInfo.ErrorMessage));

        public async Task<Dictionary<int, PublicationStatus?>> GetProcessesPublicationStatus(int[] processIds)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser))
            {
                var inProgressOrWaiting = await context.PublicationHistories
                    .Where(ph => processIds.Any(p => p == ph.ProcessId)
                        && (ph.State == PublicationStatus.InProgress
                        || ph.State == PublicationStatus.Waiting))
                    .ToDictionaryAsync(ph => ph.ProcessId, ph => (PublicationStatus?)ph.State);
                foreach (var processId in processIds)
                    if (!inProgressOrWaiting.ContainsKey(processId))
                        inProgressOrWaiting.Add(processId, null);
                return inProgressOrWaiting;
            }
        }

        Task<bool> VerifHash(byte[] fileHash, string filePath) =>
            Task.Factory.StartNew(() =>
            {
                HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
                using (var fileStream = File.OpenRead(filePath))
                {
                    return murmur128.ComputeHash(fileStream).ToHashString() == fileHash.ToHashString();
                }
            });

        string GetFinalWatermark(string baseWatermark = null, string projectLabel = null, string wbs = null)
        {
            string resultWatermark = null;
            if (!string.IsNullOrEmpty(baseWatermark))
            {
                resultWatermark = baseWatermark;
                if (!string.IsNullOrEmpty(projectLabel))
                    resultWatermark += $" - {projectLabel}";
                if (!string.IsNullOrEmpty(wbs))
                    resultWatermark += $" - {wbs}";
            }
            return resultWatermark;
        }

        string GetCutVideoFileName(KAction action)
        {
            TimeSpan from = TimeSpan.FromTicks(action.Start);
            TimeSpan duration = TimeSpan.FromTicks(action.Duration);

            string cutVideoFileName = $"{action.Video.Hash}_{from.Ticks}_{duration.Ticks}{transcodeExt}";
            return cutVideoFileName;
        }

        Task<KeyValuePair<int, (string fileHash, string fileName, string originalFileHash)>> CutVideo(
            KAction action,
            string overlayTextMarking,
            EHorizontalAlign markingHorizontalAlign,
            EVerticalAlign markingVerticalAlign,
            double durationMini,
            CancellationToken cancellationToken) =>
            new Task<KeyValuePair<int, (string fileHash, string fileName, string originalFileHash)>>(() =>
            {
                TimeSpan from = TimeSpan.FromTicks(action.Start);
                TimeSpan duration = TimeSpan.FromTicks(action.Duration);
                string originalFileName = $"{action.Video.Hash}{action.Video.Extension}";
                if (!_fileProvider.Exists(originalFileName, DirectoryEnum.Published).GetAwaiter().GetResult())
                    throw new IOException($"File '{originalFileName}' doesn't exist on server.");
                string cutVideoFileName = GetCutVideoFileName(action);

                _traceManager.TraceDebug($"Saving video \"{Path.Combine(VideoBufferDirectory, cutVideoFileName)}\"");

                string exeFilePath = Path.Combine(
                    Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "ffme", "ffmpeg.exe");
                string fontFilePath = Path.Combine(
                    Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "App_Data", "arial.ttf");

                string textFilePath = Path.GetTempFileName();
                if (!string.IsNullOrWhiteSpace(overlayTextMarking))
                {
                    using (var stream = File.OpenWrite(textFilePath))
                    using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                    {
                        writer.Write(overlayTextMarking.Replace(@"\", @"\\"));
                    }
                }

                string markingAlignment = string.Empty;

                switch (markingHorizontalAlign)
                {
                    case EHorizontalAlign.Center:
                        markingAlignment = "x=(w-tw)/2";
                        break;
                    case EHorizontalAlign.Left:
                        markingAlignment = $"x={_MARKING_PADDING}";
                        break;
                    case EHorizontalAlign.Right:
                        markingAlignment = $"x=w-tw-{_MARKING_PADDING}";
                        break;
                }

                switch (markingVerticalAlign)
                {
                    case EVerticalAlign.Bottom:
                        markingAlignment += $":y=h-th-{_MARKING_PADDING}";
                        break;
                    case EVerticalAlign.Center:
                        markingAlignment += ":y=(h-th)/2";
                        break;
                    case EVerticalAlign.Top:
                        markingAlignment += $":y={_MARKING_PADDING}";
                        break;
                }

                double speed = 1;
                bool slowMotion = false;
                if (durationMini > 0 && duration.TotalSeconds < durationMini)
                {
                    slowMotion = true;
                    speed = duration.TotalMilliseconds / (durationMini * 1000);
                }
                List<string> filters = new List<string>();
                if (slowMotion)
                    filters.Add($"setpts=(1/{speed.ToString().Replace(',', '.')})*PTS");
                if (!string.IsNullOrWhiteSpace(overlayTextMarking))
                {
                    string formattedFontFilePath = fontFilePath.Replace(@"\", @"\\").Replace(":", @"\:");
                    string formattedtextFilePath = textFilePath.Replace(@"\", @"\\").Replace(":", @"\:");
                    filters.Add($"drawtext=fontfile='{formattedFontFilePath}':textfile='{formattedtextFilePath}':fontcolor=white:fontsize=24:box=1:boxcolor=black@0.2:boxborderw=5:{markingAlignment}");
                }

                var mediaInfo = GetMediaInfo($"{ConfigurationManager.AppSettings["ApplicationUrl"].Replace("*", "localhost")}/Stream/{originalFileName}");
                var processArgumentsBuilder = new StringBuilder("-hide_banner -y -nostdin");
                processArgumentsBuilder.Append($" -ss {from.ToString(@"hh\:mm\:ss")}.{from.Milliseconds}");
                processArgumentsBuilder.Append($" -t {duration.ToString(@"hh\:mm\:ss")}.{Math.Max(duration.Milliseconds, 200)}");
                processArgumentsBuilder.Append($" -i \"{ConfigurationManager.AppSettings["ApplicationUrl"].Replace("*", "localhost")}/Stream/{originalFileName}\"");
                if (slowMotion || !mediaInfo.HasAudio)
                    processArgumentsBuilder.Append(" -an");
                else
                    processArgumentsBuilder.Append($" -acodec mp3 -b:a {Math.Min(mediaInfo.AudioBitrate ?? maxAudioBitRate, maxAudioBitRate)}");
                if (filters.Count > 0)
                    processArgumentsBuilder.Append($" -vf \"{string.Join(",", filters)}\"");
                processArgumentsBuilder.Append($" -vcodec libx264 -preset fast -b:v {Math.Min(mediaInfo.VideoBitrate ?? maxVideoBitRate, maxVideoBitRate)} -f mp4 -movflags faststart \"{Path.Combine(VideoBufferDirectory, cutVideoFileName)}\""); // Bitrate : 1500k

                TimeSpan videoDuration = new TimeSpan(0);
                TimeSpan progressDuration = new TimeSpan(0);
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
                        if (cancellationToken.IsCancellationRequested == true)
                        {
                            process.Close();
                            throw new OperationCanceledException();
                        }
                        if (e.Data != null && e.Data.Trim().StartsWith("Duration:"))
                        {
                            string videoDurationString = e.Data.Trim().Split(',').First().Split(' ')[1].Trim();
                            TimeSpan.TryParse(videoDurationString, out videoDuration);
                        }
                        if (e.Data != null && e.Data.Trim().StartsWith("frame="))
                        {
                            string videoDurationString = e.Data.Trim().Split(' ').Single(_ => _.StartsWith("time=")).Split('=')[1];
                            TimeSpan.TryParse(videoDurationString, out progressDuration);
                            double progressValue = videoDuration.Ticks == 0 ? 0 : Math.Round(progressDuration.TotalMilliseconds * 100 / videoDuration.TotalMilliseconds);
                            //progress?.Report(progressValue);
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
                        _traceManager.TraceError(e, "Le splitter de video n'a pas été trouvé");
                        throw new Exception("An issue has occured during video packaging. It seems that some components are missing.", e);
                    }
                    catch (OperationCanceledException e)
                    {
                        _traceManager.TraceError(e, "L'export a été annulé lors du split des vidéos");
                        throw;
                    }
                    catch (Exception e)
                    {
                        _traceManager.TraceError(e, "Une erreur non prévue s'est produite lors du split de la video");
                        throw;
                    }
                    finally
                    {
                        try
                        {
                            if (File.Exists(textFilePath))
                                File.Delete(textFilePath);
                        }
                        catch (Exception ex)
                        {
                            _traceManager.TraceDebug(ex, $"Erreur lors de la suppression du fichier {textFilePath}");
                        }
                    }
                }

                // On calcule le hash du fichier
                string cutVideoHash;
                HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
                using (var fileStream = File.Open(Path.Combine(VideoBufferDirectory, cutVideoFileName), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    cutVideoHash = murmur128.ComputeHash(fileStream).ToHashString();
                }

                return new KeyValuePair<int, (string fileHash, string fileName, string originalFileHash)>(action.ActionId, (cutVideoHash, cutVideoFileName, action.Video.Hash));
            }, cancellationToken);

        MediaInfo GetMediaInfo(string filePath)
        {
            MediaInfo result = null;
            string json = "";
            string exeFilePath = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "ffme", "ffprobe.exe");
            using (var process = new Process())
            {
                process.StartInfo.FileName = exeFilePath;
                process.StartInfo.Arguments = $"-v quiet -print_format json -show_format -show_streams \"{filePath}\"";
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

        Task<KeyValuePair<int, (string fileHash, DirectoryEnum directory, string fileName, string originalFileHash)>> CreateThumbnail(KAction action, string overlayTextMarking, EHorizontalAlign markingHorizontalAlign, EVerticalAlign markingVerticalAlign) =>
            new Task<KeyValuePair<int, (string fileHash, DirectoryEnum directory, string fileName, string originalFileHash)>>(() =>
            {
                string originalFileHash = action.ThumbnailHash;
                string originalFileName = $"{action.Thumbnail.Hash}{action.Thumbnail.Extension}";

                // Uncomment next line to test
                //throw new ThumbnailCreationException(originalFileName);
                
                string exeFilePath = Path.Combine(
                    Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "ffme", "ffmpeg.exe");
                string fontFilePath = Path.Combine(
                    Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "App_Data", "arial.ttf");
                string tempPath = Path.Combine(
                    Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "App_Data", "Temp");
                Directory.CreateDirectory(tempPath);

                string textFilePath = Path.GetTempFileName();
                if (!string.IsNullOrWhiteSpace(overlayTextMarking))
                {
                    using (var stream = File.OpenWrite(textFilePath))
                    using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                    {
                        writer.Write(overlayTextMarking.Replace(@"\", @"\\"));
                    }
                }

                string markingAlignment = string.Empty;

                switch (markingHorizontalAlign)
                {
                    case EHorizontalAlign.Center:
                        markingAlignment = "x=(w-tw)/2";
                        break;
                    case EHorizontalAlign.Left:
                        markingAlignment = $"x={_MARKING_PADDING}";
                        break;
                    case EHorizontalAlign.Right:
                        markingAlignment = $"x=w-tw-{_MARKING_PADDING}";
                        break;
                }

                switch (markingVerticalAlign)
                {
                    case EVerticalAlign.Bottom:
                        markingAlignment += $":y=h-th-{_MARKING_PADDING}";
                        break;
                    case EVerticalAlign.Center:
                        markingAlignment += ":y=(h-th)/2";
                        break;
                    case EVerticalAlign.Top:
                        markingAlignment += $":y={_MARKING_PADDING}";
                        break;
                }

                var processArgumentsBuilder = new StringBuilder("-hide_banner -y -nostdin");
                processArgumentsBuilder.Append($" -i \"{ConfigurationManager.AppSettings["ApplicationUrl"].Replace("*", "localhost")}/GetFile/{originalFileName}\"");
                if (!string.IsNullOrWhiteSpace(overlayTextMarking))
                {
                    string formattedFontFilePath = fontFilePath.Replace(@"\", @"\\").Replace(":", @"\:");
                    string formattedtextFilePath = textFilePath.Replace(@"\", @"\\").Replace(":", @"\:");
                    processArgumentsBuilder.Append($" -vf \"drawtext=fontfile='{formattedFontFilePath}':textfile='{formattedtextFilePath}':fontcolor=white:fontsize=24:box=1:boxcolor=black@0.2:boxborderw=5:{markingAlignment}\"");
                }
                else
                {
                    return new KeyValuePair<int, (string fileHash, DirectoryEnum directory, string filePath, string originalFileHash)>(action.ActionId, (originalFileHash, DirectoryEnum.Published, originalFileName, originalFileHash));
                }
                string watermarkedFileHash;
                string watermarkedFileName = $"{Guid.NewGuid()}.jpg";
                string watermarkedFilePath = $"{ConfigurationManager.AppSettings["ApplicationUrl"].Replace("*", "localhost")}/PostFile/{watermarkedFileName}";
                processArgumentsBuilder.Append($" \"{Path.Combine(tempPath, watermarkedFileName)}\"");

                using (var process = new Process())
                {
                    process.StartInfo.FileName = exeFilePath;
                    process.StartInfo.Arguments = processArgumentsBuilder.ToString();
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.RedirectStandardOutput = true;

                    var syncTrace = $"{exeFilePath} {processArgumentsBuilder}\n";

                    process.OutputDataReceived += (s, e) => syncTrace += $"{e.Data}\n";
                    process.ErrorDataReceived += (s, e) => syncTrace += $"{e.Data}\n";

                    try
                    {
                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        process.WaitForExit();
                        if (process.ExitCode != 0)
                            throw new IOException("Le processus FFMPEG a reporté des erreurs.");

                        _traceManager.TraceInfo(syncTrace);
                    }
                    catch (Exception e)
                    {
                        _traceManager.TraceError(syncTrace);
                        _traceManager.TraceError(e, "Une erreur non prévue s'est produite lors du watermarking du thumbnail");
                        throw new ThumbnailCreationException(originalFileName);
                    }
                    finally
                    {
                        try
                        {
                            File.Delete(textFilePath);
                        }
                        catch (Exception ex)
                        {
                            _traceManager.TraceDebug(ex, $"Erreur lors de la suppression du fichier {textFilePath}");
                        }
                    }
                }

                // On calcule le hash du fichier
                HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
                using (var fileStream = File.OpenRead(Path.Combine(tempPath, watermarkedFileName)))
                {
                    watermarkedFileHash = murmur128.ComputeHash(fileStream).ToHashString();
                }

                // On le déplace sur le serveur
                using (var fileStream = File.OpenRead(Path.Combine(tempPath, watermarkedFileName)))
                using (var networkStream = _fileProvider.OpenWrite(watermarkedFileName, DirectoryEnum.Uploaded).GetAwaiter().GetResult())
                {
                    fileStream.CopyTo(networkStream, StreamExtensions.BufferSize);
                }
                File.Delete(Path.Combine(tempPath, watermarkedFileName));

                return new KeyValuePair<int, (string fileHash, DirectoryEnum directory, string filePath, string originalFileHash)>(action.ActionId, (watermarkedFileHash, DirectoryEnum.Uploaded, watermarkedFileName, originalFileHash));
            });

        // For documentation task
        Task<KeyValuePair<int, (string fileHash, DirectoryEnum directory, string fileName, string originalFileHash)>> CreateThumbnail(DocumentationActionDraft action, string overlayTextMarking, EHorizontalAlign markingHorizontalAlign, EVerticalAlign markingVerticalAlign) =>
            new Task<KeyValuePair<int, (string fileHash, DirectoryEnum directory, string fileName, string originalFileHash)>>(() =>
            {
                string originalFileHash = action.ThumbnailHash;
                string originalFileName = $"{action.Thumbnail.Hash}{action.Thumbnail.Extension}";

                // Uncomment next line to test
                //throw new ThumbnailCreationException(originalFileName);

                string exeFilePath = Path.Combine(
                    Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "ffme", "ffmpeg.exe");
                string fontFilePath = Path.Combine(
                    Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "App_Data", "arial.ttf");
                string tempPath = Path.Combine(
                    Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "App_Data", "Temp");
                Directory.CreateDirectory(tempPath);

                string textFilePath = Path.GetTempFileName();
                if (!string.IsNullOrWhiteSpace(overlayTextMarking))
                {
                    using (var stream = File.OpenWrite(textFilePath))
                    using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                    {
                        writer.Write(overlayTextMarking.Replace(@"\", @"\\"));
                    }
                }

                string markingAlignment = string.Empty;

                switch (markingHorizontalAlign)
                {
                    case EHorizontalAlign.Center:
                        markingAlignment = "x=(w-tw)/2";
                        break;
                    case EHorizontalAlign.Left:
                        markingAlignment = $"x={_MARKING_PADDING}";
                        break;
                    case EHorizontalAlign.Right:
                        markingAlignment = $"x=w-tw-{_MARKING_PADDING}";
                        break;
                }

                switch (markingVerticalAlign)
                {
                    case EVerticalAlign.Bottom:
                        markingAlignment += $":y=h-th-{_MARKING_PADDING}";
                        break;
                    case EVerticalAlign.Center:
                        markingAlignment += ":y=(h-th)/2";
                        break;
                    case EVerticalAlign.Top:
                        markingAlignment += $":y={_MARKING_PADDING}";
                        break;
                }

                var processArgumentsBuilder = new StringBuilder("-hide_banner -y -nostdin");
                processArgumentsBuilder.Append($" -i \"{ConfigurationManager.AppSettings["ApplicationUrl"].Replace("*", "localhost")}/GetFile/{originalFileName}\"");
                if (!string.IsNullOrWhiteSpace(overlayTextMarking))
                {
                    string formattedFontFilePath = fontFilePath.Replace(@"\", @"\\").Replace(":", @"\:");
                    string formattedtextFilePath = textFilePath.Replace(@"\", @"\\").Replace(":", @"\:");
                    processArgumentsBuilder.Append($" -vf \"drawtext=fontfile='{formattedFontFilePath}':textfile='{formattedtextFilePath}':fontcolor=white:fontsize=24:box=1:boxcolor=black@0.2:boxborderw=5:{markingAlignment}\"");
                }
                else
                {
                    return new KeyValuePair<int, (string fileHash, DirectoryEnum directory, string filePath, string originalFileHash)>(-action.DocumentationActionDraftId, (originalFileHash, DirectoryEnum.Published, originalFileName, originalFileHash));
                }
                string watermarkedFileHash;
                string watermarkedFileName = $"{Guid.NewGuid()}.jpg";
                string watermarkedFilePath = $"{ConfigurationManager.AppSettings["ApplicationUrl"].Replace("*", "localhost")}/PostFile/{watermarkedFileName}";
                processArgumentsBuilder.Append($" \"{Path.Combine(tempPath, watermarkedFileName)}\"");

                using (var process = new Process())
                {
                    process.StartInfo.FileName = exeFilePath;
                    process.StartInfo.Arguments = processArgumentsBuilder.ToString();
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.RedirectStandardOutput = true;

                    var syncTrace = $"{exeFilePath} {processArgumentsBuilder}\n";

                    process.OutputDataReceived += (s, e) => syncTrace += $"{e.Data}\n";
                    process.ErrorDataReceived += (s, e) => syncTrace += $"{e.Data}\n";

                    try
                    {
                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        process.WaitForExit();
                        if (process.ExitCode != 0)
                            throw new IOException("Le processus FFMPEG a reporté des erreurs.");

                        _traceManager.TraceInfo(syncTrace);
                    }
                    catch (Exception e)
                    {
                        _traceManager.TraceError(syncTrace);
                        _traceManager.TraceError(e, "Une erreur non prévue s'est produite lors du watermarking du thumbnail");
                        throw new ThumbnailCreationException(originalFileName);
                    }
                    finally
                    {
                        try
                        {
                            File.Delete(textFilePath);
                        }
                        catch (Exception ex)
                        {
                            _traceManager.TraceDebug(ex, $"Erreur lors de la suppression du fichier {textFilePath}");
                        }
                    }
                }

                // On calcule le hash du fichier
                HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
                using (var fileStream = File.OpenRead(Path.Combine(tempPath, watermarkedFileName)))
                {
                    watermarkedFileHash = murmur128.ComputeHash(fileStream).ToHashString();
                }

                // On le déplace sur le serveur
                using (var fileStream = File.OpenRead(Path.Combine(tempPath, watermarkedFileName)))
                using (var networkStream = _fileProvider.OpenWrite(watermarkedFileName, DirectoryEnum.Uploaded).GetAwaiter().GetResult())
                {
                    fileStream.CopyTo(networkStream, StreamExtensions.BufferSize);
                }
                File.Delete(Path.Combine(tempPath, watermarkedFileName));

                return new KeyValuePair<int, (string fileHash, DirectoryEnum directory, string filePath, string originalFileHash)>(-action.DocumentationActionDraftId, (watermarkedFileHash, DirectoryEnum.Uploaded, watermarkedFileName, originalFileHash));
            });

        /// <summary>
        /// Efface toutes les videos qui ont pu être crées lors du decoupage
        /// </summary>
        /// <param name="publishedScenario"></param>
        void ClearVideoBuffer(Scenario publishedScenario)
        {
            publishedScenario.Actions
                .Where(action => action.VideoId != null)
                .Select(action => Path.Combine(VideoBufferDirectory, GetCutVideoFileName(action)))
                .AsParallel()
                .ForAll(file =>
                {
                    try
                    {
                        if (File.Exists(file))
                            File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        _traceManager.TraceDebug(ex, $"Erreur lors de la suppression du fichier {file}");
                    }
                });
        }

        /// <summary>
        /// Check if application running is API or DESKTOP
        /// </summary>
        /// <returns></returns>
        bool IsDesktopMode()
        {
            var assembly = Assembly.GetEntryAssembly().Location;
            string name = Path.GetFileName(assembly);
            return name == "KL².exe";
        }


        public class MediaInfo
        {
            public bool HasVideo { get; set; }
            public bool HasAudio { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int? VideoBitrate { get; set; }
            public int? AudioBitrate { get; set; }
            public string VideoCodec { get; set; }
            public string AudioCodec { get; set; }
            public TimeSpan Duration { get; set; }
        }

        public class ThumbnailCreationException : Exception
        {
            public string Name { get; }

            public ThumbnailCreationException(string name)
            {
                Name = name;
            }
        }
    }

    public static class PublicationRepositoryExt
    {
        public static Task SendToSignalR(this IKLPublicationRepository publicationRepository, int publicationHistoryId, PublishTaskProgressInfos progressInfo) =>
            publicationRepository.RefreshPublicationProgress(new PublicationProgressEventArgs(publicationHistoryId,
                progressInfo.Status,
                progressInfo.Step,
                progressInfo.Progress,
                progressInfo.CanCancel));
    }
}