using Kprocess.KL2.FileTransfer;
using KProcess.Business;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos.Prepare;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Interfaces;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.API
{
    /// <summary>
    /// 
    /// </summary>
    public class PrepareService : IBusinessService, IPrepareService
    {
        readonly IAPIHttpClient _apiHttpClient;
        readonly FileTransferManager _fileTransferManager;

        public PrepareService(IAPIHttpClient apiHttpClient, FileTransferManager fileTransferManager)
        {
            _apiHttpClient = apiHttpClient;
            _fileTransferManager = fileTransferManager;
        }

        /// <summary>
        /// True si une publication existe pour le process.
        /// </summary>
        public async Task<bool> PublicationExistsForProcess(int processId)
        {
            dynamic param = new ExpandoObject();
            param.processId = processId;
            return await _apiHttpClient.ServiceAsync<bool>(KL2_Server.API, nameof(PrepareService), nameof(PublicationExistsForProcess), param);
        }

        /// <summary>
        /// True si une publication existe pour le process en sync.
        /// </summary>
        public bool PublicationExistsForProcessSync(int processId)
        {
            dynamic param = new ExpandoObject();
            param.processId = processId;
            return _apiHttpClient.Service<bool>(KL2_Server.API, nameof(PrepareService), nameof(PublicationExistsForProcessSync), param);
        }

        public Task<Publication> GetPublication(Guid publicationId)
        {
            dynamic param = new ExpandoObject();
            param.publicationId = publicationId;
            return _apiHttpClient.ServiceAsync<Publication>(KL2_Server.API, nameof(PrepareService), nameof(GetPublication), param);
        }

        public Task<Publication> GetTrainingPublication(Guid evaluationPublicationId)
        {
            dynamic param = new ExpandoObject();
            param.evaluationPublicationId = evaluationPublicationId;
            return _apiHttpClient.ServiceAsync<Publication>(KL2_Server.API, nameof(PrepareService), nameof(GetTrainingPublication), param);
        }

        public Task<Publication> GetEvaluationPublication(Guid trainingPublicationId)
        {
            dynamic param = new ExpandoObject();
            param.trainingPublicationId = trainingPublicationId;
            return _apiHttpClient.ServiceAsync<Publication>(KL2_Server.API, nameof(PrepareService), nameof(GetEvaluationPublication), param);
        }

        public Task<IEnumerable<Publication>> GetTrainingPublications(Guid[] evaluationPublicationIds)
        {
            dynamic param = new ExpandoObject();
            param.evaluationPublicationIds = evaluationPublicationIds;
            return _apiHttpClient.ServiceAsync<IEnumerable<Publication>>(KL2_Server.API, nameof(PrepareService), nameof(GetTrainingPublications), param);
        }

        public Task<IEnumerable<Publication>> GetEvaluationPublications(Guid[] trainingPublicationIds)
        {
            dynamic param = new ExpandoObject();
            param.trainingPublicationIds = trainingPublicationIds;
            return _apiHttpClient.ServiceAsync<IEnumerable<Publication>>(KL2_Server.API, nameof(PrepareService), nameof(GetEvaluationPublications), param);
        }

        public Task<Publication> GetLightPublication(Guid publicationId)
        {
            dynamic param = new ExpandoObject();
            param.publicationId = publicationId;
            return _apiHttpClient.ServiceAsync<Publication>(KL2_Server.API, nameof(PrepareService), nameof(GetLightPublication), param);
        }

        /// <summary>
        /// Obtient la dernière publication d'un process.
        /// </summary>
        public async Task<Publication> GetLastPublication(int processId)
        {
            dynamic param = new ExpandoObject();
            param.processId = processId;
            return await _apiHttpClient.ServiceAsync<Publication>(KL2_Server.API, nameof(PrepareService), nameof(GetLastPublication), param);
        }

        /// <summary>
        /// Obtient la dernière publication d'un process. with filter by publish mode
        /// </summary>
        public async Task<Publication> GetLastPublicationFiltered(int processId, int publishModeFilter)
        {
            dynamic param = new ExpandoObject();
            param.processId = processId;
            param.publishModeFilter = publishModeFilter;
            return await _apiHttpClient.ServiceAsync<Publication>(KL2_Server.API, nameof(PrepareService), nameof(GetLastPublicationFiltered), param);
        }

        /// <summary>
        /// Obtient la publication d'un process avec un audit ouvert pour l'utilisateur donné.
        /// </summary>
        public async Task<Publication> GetPublicationToAudit(int auditorId)
        {
            dynamic param = new ExpandoObject();
            param.auditorId = auditorId;
            return await _apiHttpClient.ServiceAsync<Publication>(KL2_Server.API, nameof(PrepareService), nameof(GetPublicationToAudit), param);
        }

        /// <summary>
        /// Obtient les dernières publications d'un process.
        /// </summary>
        public async Task<IEnumerable<Publication>> GetLastPublications(int publishModeFilter)
        {
            dynamic param = new ExpandoObject();
            param.publishModeFilter = publishModeFilter;
            return await _apiHttpClient.ServiceAsync<IEnumerable<Publication>>(KL2_Server.API, nameof(PrepareService), nameof(GetLastPublications), param);
        }

        public async Task<IEnumerable<Publication>> GetLastPublicationsForFilter(int publishModeFilter)
        {
            dynamic param = new ExpandoObject();
            param.publishModeFilter = publishModeFilter;
            return await _apiHttpClient.ServiceAsync<IEnumerable<Publication>>(KL2_Server.API, nameof(PrepareService), nameof(GetLastPublicationsForFilter), param);
        }

        /// <summary>
        /// Obtient les dernières publications d'un process ainsi que toutes les publications de la version courante
        /// </summary>
        public async Task<IEnumerable<Publication>> GetLastSameMajorPublications(int publishModeFilter)
        {
            dynamic param = new ExpandoObject();
            param.publishModeFilter = publishModeFilter;
            return await _apiHttpClient.ServiceAsync<IEnumerable<Publication>>(KL2_Server.API, nameof(PrepareService), nameof(GetLastSameMajorPublications), param);
        }

        public async Task<IEnumerable<Publication>> GetLastPublicationsPerMajor(int publishModeFilter)
        {
            dynamic param = new ExpandoObject();
            param.publishModeFilter = publishModeFilter;
            return await _apiHttpClient.ServiceAsync<IEnumerable<Publication>>(KL2_Server.API, nameof(PrepareService), nameof(GetLastSameMajorPublications), param);
        }

        /// <summary>
        /// Obtient les dernières publications d'un process. with IsSkill true
        /// </summary>
        public async Task<IEnumerable<Publication>> GetLastPublicationSkills()
        {
            dynamic param = new ExpandoObject();
            return await _apiHttpClient.ServiceAsync<IEnumerable<Publication>>(KL2_Server.API, nameof(PrepareService), nameof(GetLastPublicationSkills), param);
        }

        /// <summary>
        /// Get all audits or audits of inspections
        /// </summary>
        public async Task<IEnumerable<Audit>> GetAudits(int? auditId = null)
        {
            dynamic param = new ExpandoObject();
            param.auditId = auditId;
            return await _apiHttpClient.ServiceAsync<IEnumerable<Audit>>(KL2_Server.API, nameof(PrepareService), nameof(GetAudits), param);
        }

        /// <summary>
        /// Get all surveys or spesific survey
        /// </summary>
        public async Task<IEnumerable<Survey>> GetSurveys(int? surveyId = null)
        {
            dynamic param = new ExpandoObject();
            param.surveyId = surveyId;
            return await _apiHttpClient.ServiceAsync<IEnumerable<Survey>>(KL2_Server.API, nameof(PrepareService), nameof(GetSurveys), param);
        }

        /// <summary>
        /// Save surveys
        /// </summary>
        /// <param name="surveys">List of surveys to be saved</param>
        /// <returns>List of surveys that want to be saved</returns>
        public async Task<Survey[]> SaveSurveys(Survey[] surveys)
        {
            dynamic param = new ExpandoObject();
            param.surveys = surveys;
            return await _apiHttpClient.ServiceAsync<Survey[]>(KL2_Server.API, nameof(PrepareService), nameof(SaveSurveys), param);
        }

        /// <summary>
        /// Check if auditor already have active audit 
        /// </summary>
        public async Task<bool> CheckAuditorHaveActiveAudit(int? auditorId = null, Guid? publicationId = null)
        {
            dynamic param = new ExpandoObject();
            param.auditorId = auditorId;
            param.publicationId = publicationId;
            return await _apiHttpClient.ServiceAsync<bool>(KL2_Server.API, nameof(PrepareService), nameof(CheckAuditorHaveActiveAudit), param);
        }

        /// <summary>
        /// Get active audit 
        /// </summary>
        public async Task<Audit> GetActiveAudit(int? auditorId = null, Guid? publicationId = null)
        {
            dynamic param = new ExpandoObject();
            param.auditorId = auditorId;
            param.publicationId = publicationId;
            return await _apiHttpClient.ServiceAsync<Audit>(KL2_Server.API, nameof(PrepareService), nameof(GetActiveAudit), param);
        }
        
        /// <summary>
        /// Save audit
        /// </summary>
        /// <returns>Audit</returns>
        public async Task<Audit> SaveAudit(Audit audit)
        {
            dynamic param = new ExpandoObject();
            param.audit = audit;
            return await _apiHttpClient.ServiceAsync<Audit>(KL2_Server.API, nameof(PrepareService), nameof(SaveAudit), param);
        }

        /// <summary>
        /// Obtient un booléen qui indique si tous les process liés d'un scénario (si ils existent) ont été publiés.
        /// </summary>
        public async Task<(bool Result, Procedure[] NonPublishedProcesses)> AllLinkedProcessArePublished(int scenarioId)
        {
            dynamic param = new ExpandoObject();
            param.scenarioId = scenarioId;
            return await _apiHttpClient.ServiceAsync<(bool Result, Procedure[] NonPublishedProcesses)>(KL2_Server.API, nameof(PrepareService), nameof(AllLinkedProcessArePublished), param);
        }

        /// <summary>
        /// Obtient un booléen qui indique si tous les process liés d'un scénario (si ils existent) ont été publiés. (SYNC)
        /// </summary>
        public (bool Result, Procedure[] NonPublishedProcesses) AllLinkedProcessArePublishedSync(int scenarioId)
        {
            dynamic param = new ExpandoObject();
            param.scenarioId = scenarioId;
            return _apiHttpClient.Service<(bool Result, Procedure[] NonPublishedProcesses)>(KL2_Server.API, nameof(PrepareService), nameof(AllLinkedProcessArePublishedSync), param);
        }


        public Task<Publication> SetReadPublication(Guid publicationId, int UserId, DateTime? ReadingDate)
        {
            dynamic param = new ExpandoObject();
            param.publicationId = publicationId;
            param.UserId = UserId;
            param.ReadingDate = ReadingDate;
            return _apiHttpClient.ServiceAsync<Publication>(KL2_Server.API, nameof(PrepareService), nameof(SetReadPublication), param);
        }

        /// <summary>
        /// Obtient un projet
        /// </summary>
        public virtual async Task<Project> GetProject(int projectId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                return await _apiHttpClient.ServiceAsync<Project>(KL2_Server.API, nameof(PrepareService), nameof(GetProject), param);
            });

        /// <summary>
        /// Obtient un projet en sync
        /// </summary>
        public virtual Project GetProjectSync(int projectId)
        {
            dynamic param = new ExpandoObject();
            param.projectId = projectId;
            return _apiHttpClient.Service<Project>(KL2_Server.API, nameof(PrepareService), nameof(GetProjectSync), param);
        }

        /// <summary>
        /// Obtient l'arborescence des process ayant une publication
        /// </summary>
        public virtual async Task<INode[]> GetPublicationsTree(PublishModeEnum filter = PublishModeEnum.Formation | PublishModeEnum.Inspection | PublishModeEnum.Audit) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.filter = filter;
                return await _apiHttpClient.ServiceAsync<INode[]>(KL2_Server.API, nameof(PrepareService), nameof(GetPublicationsTree), param);
            });

        /// <summary>
        /// Get process information by id
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="includeAllInformations">Include all the informations (longer to execute)</param>
        /// <returns></returns>
        public async Task<Procedure> GetProcess(int processId, bool includeAllInformations = true)
        {
            dynamic param = new ExpandoObject();
            param.processId = processId;
            param.includeAllInformations = includeAllInformations;
            return await _apiHttpClient.ServiceAsync<Procedure>(KL2_Server.API, nameof(PrepareService), nameof(GetProcess), param);
        }

        public async Task<Procedure> GetProcessForPublishFormat(int processId)
        {
            dynamic param = new ExpandoObject();
            param.processId = processId;
            return await _apiHttpClient.ServiceAsync<Procedure>(KL2_Server.API, nameof(PrepareService), nameof(GetProcessForPublishFormat), param);
        }

        /// <summary>
        /// Retrieve action information for a specific scenario
        /// </summary>
        /// <param name="actionId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<KAction>> GetActionsByScenario(int scenarioId)
        {
            dynamic param = new ExpandoObject();
            param.scenarioId = scenarioId;
            return await _apiHttpClient.ServiceAsync<KAction>(KL2_Server.API, nameof(PrepareService), nameof(GetActionsByScenario), param);
        }

        public async Task<INode[]> GetProcessTreeWithScenario()
        {
            dynamic param = new ExpandoObject();
            return await _apiHttpClient.ServiceAsync<INode[]>(KL2_Server.API, nameof(PrepareService), nameof(GetProcessTreeWithScenario), param);
        }

        public async Task<string> GetProcessName(int processId)
        {
            dynamic param = new ExpandoObject();
            param.processId = processId;
            return await _apiHttpClient.ServiceAsync<string>(KL2_Server.API, nameof(PrepareService), nameof(GetProcessName), param);
        }

        public async Task<string> GetProjectName(int projectId)
        {
            dynamic param = new ExpandoObject();
            param.projectId = projectId;
            return await _apiHttpClient.ServiceAsync<string>(KL2_Server.API, nameof(PrepareService), nameof(GetProjectName), param);
        }

        public async Task<string> GetScenarioName(int scenarioId)
        {
            dynamic param = new ExpandoObject();
            param.scenarioId = scenarioId;
            return await _apiHttpClient.ServiceAsync<string>(KL2_Server.API, nameof(PrepareService), nameof(GetScenarioName), param);
        }

        /// <summary>
        /// Obtient les projets et les objectifs
        /// </summary>
        public virtual async Task<ProjectsData> GetProjects() =>
            await Task.Run(async () =>
            {
                return await _apiHttpClient.ServiceAsync<ProjectsData>(KL2_Server.API, nameof(PrepareService), nameof(GetProjects));
            });

        /// <summary>
        /// Sync all videos.
        /// </summary>
        public virtual async Task SyncVideos(params int[] processIds)
        {
            if (processIds?.Any() == true)
            {
                dynamic param = new ExpandoObject();
                param.processIds = processIds;
                var allVideos = await ListAllVideosToSync(processIds);
                if (allVideos.ContainsKey(VideoSyncTask.Sync) && allVideos[VideoSyncTask.Sync]?.Any() == true) // Download-Upload
                {
                    var videosToDownload = allVideos[VideoSyncTask.Sync]
                        .Where(_ => _.Sync && _.OnServer == true && !_.IsSync && !_fileTransferManager.DownloadOperations.ContainsKey(_.Filename))
                        .ToDictionary(key => key.Filename,
                            value => (remoteFilePath: $"{Preferences.FileServerUri}/GetFile/{value.Filename}", localFilePath: Path.Combine(Preferences.SyncDirectory, value.Filename)));
                    videosToDownload.ForEach(_ =>
                    {
                        var downloadOperation = _fileTransferManager.CreateDownload(_.Key, _.Value);
                        downloadOperation.Resume();
                    });

                    var videosToUpload = allVideos[VideoSyncTask.Sync]
                        .Where(_ => _.Sync && _.OnServer != true && _fileTransferManager.UploadOperations.ContainsKey(_.Filename));
                    videosToUpload.ForEach(videoToUpload =>
                    {
                        if (_fileTransferManager.UploadOperations[videoToUpload.Filename].State == TransferStatus.TransientError)
                        {
                            _fileTransferManager.UploadOperations[videoToUpload.Filename].Cancel();
                            var uploadOperation = _fileTransferManager.CreateUpload(videoToUpload.Filename, $"{Preferences.FileServerUri}/{videoToUpload.Filename}", videoToUpload.FilePath);
                            uploadOperation.Resume();
                        }
                    });
                }
                if (allVideos.ContainsKey(VideoSyncTask.NotSync) && allVideos[VideoSyncTask.NotSync]?.Any() == true) // Cancel download or Delete
                {
                    allVideos[VideoSyncTask.NotSync]
                        .Where(_ => _.IsSync)
                        .Select(_ => $"{_.Hash}{_.Extension}")
                        .ForEach(_ =>
                        {
                            if (_fileTransferManager.DownloadOperations.ContainsKey(_))
                            {
                                var downloadOperation = _fileTransferManager.DownloadOperations[_];
                                downloadOperation.Cancel();
                            }
                            else
                            {
                                try
                                {
                                    File.Delete(Path.Combine(Preferences.SyncDirectory, _));
                                }
                                catch
                                {
                                    TraceManager.TraceDebug($"La suppression du fichier \"{Path.Combine(Preferences.SyncDirectory, _)}\" a échoué.");
                                }
                            }
                        });
                }
            }
        }

        /// <summary>
        /// List all videos to sync.
        /// </summary>
        public virtual async Task<Dictionary<VideoSyncTask, List<Video>>> ListAllVideosToSync(params int[] processIds) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.processIds = processIds;
                return await _apiHttpClient.ServiceAsync<Dictionary<VideoSyncTask, List<Video>>>(KL2_Server.API, nameof(PrepareService), nameof(ListAllVideosToSync), param);
            });

        /// <summary>
        /// Get if a video can be unsynced.
        /// </summary>
        public virtual async Task<bool> CanBeUnSync(string videoHash) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.videoHash = videoHash;
                return await _apiHttpClient.ServiceAsync<bool>(KL2_Server.API, nameof(PrepareService), nameof(CanBeUnSync), param);
            });

        /// <summary>
        /// Obtient les dossiers
        /// </summary>
        public virtual async Task<ProjectDir[]> GetProjectDirs() =>
            await Task.Run(async () =>
            {
                return await _apiHttpClient.ServiceAsync<ProjectDir[]>(KL2_Server.API, nameof(PrepareService), nameof(GetProjectDirs));
            });

        /// <summary>
        /// Obtient les process.
        /// </summary>
        public async Task<Procedure[]> GetProcesses() =>
            await Task.Run(async () =>
            {
                return await _apiHttpClient.ServiceAsync<Procedure[]>(KL2_Server.API, nameof(PrepareService), nameof(GetProcesses));
            });

        public async Task<Procedure[]> GetPublishedProcessesForInspection()
        {
            return await _apiHttpClient.ServiceAsync<Procedure[]>(KL2_Server.API, nameof(PrepareService), nameof(GetPublishedProcessesForInspection));
        }

        /// <summary>
        /// Obtient si un process est lié à une tâche.
        /// </summary>
        public async Task<bool> ProcessIsLinkedToATask(int processId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.processId = processId;
                return await _apiHttpClient.ServiceAsync<bool>(KL2_Server.API, nameof(PrepareService), nameof(ProcessIsLinkedToATask), param);
            });

        /// <summary>
        /// Obtient les noms avec extension d'une liste de fichiers.
        /// </summary>
        public virtual async Task<string[]> GetFullName(IEnumerable<string> fileHashes) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.fileHashes = fileHashes;
                return await _apiHttpClient.ServiceAsync<string[]>(KL2_Server.API, nameof(PrepareService), nameof(GetFullName), param);
            });

        /// <summary>
        /// Sauvegarde le projet.
        /// </summary>
        /// <param name="project">Le projet.</param>
        public virtual async Task<Project> SaveProject(Project project) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.project = project;
                return await _apiHttpClient.ServiceAsync<Project>(KL2_Server.API, nameof(PrepareService), nameof(SaveProject), param);
            });

        /// <summary>
        /// Sauvegarde le dossier.
        /// </summary>
        /// <param name="folder">Le dossier.</param>
        public virtual async Task<ProjectDir> SaveFolder(ProjectDir folder) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.folder = folder;
                return await _apiHttpClient.ServiceAsync<ProjectDir>(KL2_Server.API, nameof(PrepareService), nameof(SaveFolder), param);
            });

        public virtual async Task<AppSetting[]> SaveAppSettings(AppSetting[] settings) =>
            await Task.Run(async () => {
                dynamic param = new ExpandoObject();
                param.setting = settings;
                return await _apiHttpClient.ServiceAsync<AppSetting[]>(KL2_Server.API, nameof(PrepareService), nameof(SaveAppSettings), param);
            });

        /// <summary>
        /// Sauvegarde le process.
        /// </summary>
        /// <param name="process">Le process.</param>
        public virtual async Task<Procedure> SaveProcess(Procedure process, bool notifyChanges = true) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.process = process;
                param.notifyChanges = notifyChanges;
                return await _apiHttpClient.ServiceAsync<Procedure>(KL2_Server.API, nameof(PrepareService), nameof(SaveProcess), param);
            });

        /// <summary>
        /// Obtient les raisons possibles d'une non qualification.
        /// </summary>
        public virtual async Task<List<QualificationReason>> GetQualificationReasons() =>
            await Task.Run(async () =>
            {
                return await _apiHttpClient.ServiceAsync<List<QualificationReason>>(KL2_Server.API, nameof(PrepareService), nameof(GetQualificationReasons));
            });

        /// <summary>
        /// Sauvegarde les raisons possibles d'une non qualification.
        /// </summary>
        /// <param name="reasons">Les raisons.</param>
        public async Task<List<QualificationReason>> SaveQualificationReasons(IEnumerable<QualificationReason> reasons) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.reasons = reasons;
                return await _apiHttpClient.ServiceAsync<List<QualificationReason>>(KL2_Server.API, nameof(PrepareService), nameof(SaveQualificationReasons), param);
            });

        /// <summary>
        /// Obtient les rôles des utilisateurs dans le process spécifié, tous les utilisateurs et tous les rôles disponibles.
        /// </summary>
        /// <param name="processId">L'identifiant du process.</param>
        public virtual async Task<(User[] Users, Role[] Roles)> GetMembers(int processId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.processId = processId;
                return await _apiHttpClient.ServiceAsync<(User[] Users, Role[] Roles)>(KL2_Server.API, nameof(PrepareService), nameof(GetMembers), param);
            });

        /// <summary>
        /// Sauvegarde le membre d'un process.
        /// </summary>
        /// <param name="processId">L'identifiant du process.</param>
        /// <param name="member">Le membre.</param>
        public virtual async Task<User> SaveMember(int processId, User member)
        {
            return await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.processId = processId;
                param.member = member;
                return await _apiHttpClient.ServiceAsync<User>(KL2_Server.API, nameof(PrepareService), nameof(SaveMember), param);
            });
        }


        /// <inheritdoc />
        public virtual async Task<Project> GetReferentials(int projectId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                return await _apiHttpClient.ServiceAsync<Project>(KL2_Server.API, nameof(PrepareService), nameof(GetReferentials), param);
            });

        /// <inheritdoc />
        public virtual async Task<Project> SaveReferentials(Project project) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.project = project;
                return await _apiHttpClient.ServiceAsync<Project>(KL2_Server.API, nameof(PrepareService), nameof(SaveReferentials), param);
            });

        /// <summary>
        /// Obtient toutes les ressources liées au process spécifié.
        /// </summary>
        /// <param name="processId">L'identifiant du process.</param>
        public virtual async Task<Resource[]> GetAllResources(int processId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.processId = processId;
                return await _apiHttpClient.ServiceAsync<Resource[]>(KL2_Server.API, nameof(PrepareService), nameof(GetAllResources), param);
            });

        /// <summary>
        /// Obtient une vidéo ayant la même vidéo d'origine si elle existe.
        /// </summary>
        /// <param name="originalHash">Le hash de la vidéo d'origine.</param>
        public virtual async Task<Video> GetSameOriginalVideo(string originalHash) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.originalHash = originalHash;
                return await _apiHttpClient.ServiceAsync<Video>(KL2_Server.API, nameof(PrepareService), nameof(GetSameOriginalVideo), param);
            });

        /// <summary>
        /// Obtient la vidéo.
        /// </summary>
        /// <param name="videoId">L'identifiant de la vidéo.</param>
        public virtual async Task<Video> GetVideo(int videoId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.videoId = videoId;
                return await _apiHttpClient.ServiceAsync<Video>(KL2_Server.API, nameof(PrepareService), nameof(GetVideo), param);
            });

        /// <summary>
        /// Obtient toutes les ressources liées au process spécifié.
        /// </summary>
        /// <param name="processId">L'identifiant du process.</param>
        public virtual async Task<AppSetting[]> GetAllAppSettings() =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                return await _apiHttpClient.ServiceAsync<AppSetting[]>(KL2_Server.API, nameof(PrepareService), nameof(GetAllAppSettings), param);
            });

        /// <summary>
        /// Obtient les vidéos et tous les éléments liés au chargement de Prepare - Videos, liés au process spécifié.
        /// </summary>
        /// <param name="processId">L'identifiant du process.</param>
        public virtual async Task<VideoLoad> GetVideos(int processId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.processId = processId;
                VideoLoad result = await _apiHttpClient.ServiceAsync<VideoLoad>(KL2_Server.API, nameof(PrepareService), nameof(GetVideos), param);
                if (result?.Sync == true && result?.ProcessVideos.Any() == true)
                {
                    Dictionary<string, (string remoteFilePath, string localFilePath)> videosToDownload = new Dictionary<string, (string remoteFilePath, string localFilePath)>();
                    foreach (Video video in result.ProcessVideos.Where(_ => _.Sync && _.OnServer == true && !_.IsSync && !_fileTransferManager.DownloadOperations.ContainsKey(_.Filename)))
                        if (!videosToDownload.ContainsKey(video.Filename))
                            videosToDownload.Add(video.Filename, (remoteFilePath: $"{Preferences.FileServerUri}/GetFile/{video.Filename}", localFilePath: Path.Combine(Preferences.SyncDirectory, video.Filename)));
                    videosToDownload.ForEach(_ =>
                    {
                        var downloadOperation = _fileTransferManager.CreateDownload(_.Key, _.Value);
                        downloadOperation.Resume();
                    });
                }
                return result;
            });

        /// <summary>
        /// Sauvegarde la vidéo d'un process.
        /// </summary>
        /// <param name="video">La vidéo.</param>
        public virtual async Task<Video> SaveVideo(Video video)
        {
            dynamic param = new ExpandoObject();
            param.video = video;
            return await _apiHttpClient.ServiceAsync<Video>(KL2_Server.API, nameof(PrepareService), nameof(SaveVideo), param);
        }

        /// <summary>
        /// Obtient les scénarios liés au projet spécifié.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<ScenariosData> GetScenarios(int projectId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                return await _apiHttpClient.ServiceAsync<ScenariosData>(KL2_Server.API, nameof(PrepareService), nameof(GetScenarios), param);
            });

        /// <summary>
        /// Obtient le scénario.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<Scenario> GetScenario(int scenarioId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.scenarioId = scenarioId;
                return await _apiHttpClient.ServiceAsync<ScenariosData>(KL2_Server.API, nameof(PrepareService), nameof(GetScenario), param);
            });

        /// <summary>
        /// Obtient le scénario pour publication
        /// </summary>
        /// <param name="scenarioId">L'identifiant du scenario.</param>
        public virtual async Task<Scenario> GetScenarioForPublish(int scenarioId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.scenarioId = scenarioId;
                return await _apiHttpClient.ServiceAsync<ScenariosData>(KL2_Server.API, nameof(PrepareService), nameof(GetScenarioForPublish), param);
            });


        /// <summary>
        /// Obtient les scénarios d'un projet.
        /// </summary>
        /// <param name="context">Le contexte EF.</param>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <returns>Les scénarios</returns>
        private async Task<ScenariosData> GetScenarios(KsmedEntities context, int projectId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.context = context;
                param.projectId = projectId;
                return await _apiHttpClient.ServiceAsync<ScenariosData>(KL2_Server.API, nameof(PrepareService), nameof(GetScenarios), param);
            });

        /// <summary>
        /// Crée un nouveau scénario initial.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<Scenario> CreateInitialScenario(int projectId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                return await _apiHttpClient.ServiceAsync<Scenario>(KL2_Server.API, nameof(PrepareService), nameof(CreateInitialScenario), param);
            });

        /// <summary>
        /// Crée un nouveau scénario.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="sourceScenario">Le scénario source.</param>
        /// <param name="keepVideoForUnchanged">Determine si les sequences initiales doivent être conservées.</param>
        public virtual async Task<Scenario> CreateScenario(int projectId, Scenario sourceScenario, bool keepVideoForUnchanged) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                // Rebuild scenario object as passing source scenario contains too much data                
                param.sourceScenario = sourceScenario;
                param.keepVideoForUnchanged = keepVideoForUnchanged;
                return await _apiHttpClient.ServiceAsync<Scenario>(KL2_Server.API, nameof(PrepareService), nameof(CreateScenario), param);
            });

        /// <summary>
        /// Deletes the scenario.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="scenario">le scénario a supprimer.</param>
        public virtual async Task<(bool Result, ScenarioCriticalPath[] Summary)> DeleteScenario(int projectId, Scenario scenario) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                param.scenario = scenario;
                return await _apiHttpClient.ServiceAsync<(bool Result, ScenarioCriticalPath[] Summary)>(KL2_Server.API, nameof(PrepareService), nameof(DeleteScenario), param);
            });

        /// <summary>
        /// Sauvegarde le scénario spécifié.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="scenario">Le scénario.</param>
        public virtual async Task<ScenariosData> SaveScenario(int projectId, Scenario scenario) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                param.scenario = scenario;
                return await _apiHttpClient.ServiceAsync<ScenariosData>(KL2_Server.API, nameof(PrepareService), nameof(SaveScenario), param);
            });

        /// <summary>
        /// Crée un nouveau projet à partir d'un scénario de validation.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="validatedScenario">Le scénario de validation.</param>
        public virtual async Task<int> CreateNewProjectFromValidatedScenario(int projectId, Scenario validatedScenario) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                param.validatedScenario = validatedScenario;
                return await _apiHttpClient.ServiceAsync<int>(KL2_Server.API, nameof(PrepareService), nameof(CreateNewProjectFromValidatedScenario), param);
            });

        /// <summary>
        /// Met à jour l'identifiant de publication web.
        /// </summary>
        /// <param name="scenarioId">L'identifiant du scénario.</param>
        /// <param name="publicationGuid">L'identifiant de publication.</param>
        public virtual async Task UpdateScenarioPublicationGuid(int scenarioId, Guid? publicationGuid) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.scenarioId = scenarioId;
                param.publicationGuid = publicationGuid;
                await _apiHttpClient.ServiceAsync(KL2_Server.API, nameof(PrepareService), nameof(UpdateScenarioPublicationGuid), param);
            });

        #region Publication

        /// <summary>
        /// Sauvegarde la publication
        /// </summary>
        /// <param name="publication">Publication à sauvegarder</param>
        /// <returns>La publication sauvegardée</returns>
        public async Task<Publication> SavePublication(Publication publication)
        {
            dynamic param = new ExpandoObject();
            param.publication = publication;
            return await _apiHttpClient.ServiceAsync<Publication>(KL2_Server.API, nameof(PrepareService), nameof(SavePublication), param);
        }


        /// <summary>
        /// Addd un fichier publié
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public async Task CreatePublishedFile(string hash, string extension)
        {
            dynamic param = new ExpandoObject();
            param.hash = hash;
            param.extension = extension;
            await _apiHttpClient.ServiceAsync<PublishedFile>(KL2_Server.API, nameof(PrepareService), nameof(CreatePublishedFile), param);
        }

        /// <summary>
        /// Recupere un fiicher publie via le guid
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public async Task<PublishedFile> GetPublishedFile(string hash)
        {
            dynamic param = new ExpandoObject();
            param.hash = hash;
            return await _apiHttpClient.ServiceAsync<PublishedFile>(KL2_Server.API, nameof(PrepareService), nameof(GetPublishedFile), param);
        }

        /// <summary>
        /// Recupere un fiicher publie via le guid
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public async Task<CutVideo> GetCutVideo(string hash)
        {
            dynamic param = new ExpandoObject();
            param.hash = hash;
            return await _apiHttpClient.ServiceAsync<CutVideo>(KL2_Server.API, nameof(PrepareService), nameof(GetCutVideo), param);
        }

            #endregion

        #region Formation

            /// <summary>
            /// Méthode permettant de récupérer les formations d'un utilisateur pour une publication
            /// </summary>
            /// <param name="publicationId">Identifiant de la publication</param>
            /// <param name="userId">Identifiant de l'utilisateur</param>
            /// <returns>La formation de l'utilisateur ou null s'il n'en existe pas avec les paramètres spécifié</returns>
            public async Task<Training> GetTraining(Guid publicationId, int userId)
        {
            dynamic param = new ExpandoObject();
            param.publicationId = publicationId;
            param.userId = userId;
            return await _apiHttpClient.ServiceAsync<Training>(KL2_Server.API, nameof(PrepareService), nameof(GetTraining), param);
        }

        public async Task<IEnumerable<Training>> GetTrainings()
        {
            dynamic param = new ExpandoObject();
            return await _apiHttpClient.ServiceAsync<IEnumerable<Training>>(KL2_Server.API, nameof(PrepareService), nameof(GetTrainings), param);
        }

        public async Task<IEnumerable<PublishedAction>> GetPublishedActions()
        {
            dynamic param = new ExpandoObject();
            return await _apiHttpClient.ServiceAsync<IEnumerable<PublishedAction>>(KL2_Server.API, nameof(PrepareService), nameof(GetPublishedActions), param);
        }
        /// <summary>
        /// Get a published actions for competency
        /// </summary>
        public async Task<PublishedAction> GetPublishedAction(int id)
        {
            dynamic param = new ExpandoObject();
            param.id = id;
            return await _apiHttpClient.ServiceAsync<Publication>(KL2_Server.API, nameof(PrepareService), nameof(GetPublishedAction), param);
        }

        /// <summary>
        /// Get an action
        /// </summary>
        public async Task<KAction> GetAction(int id)
        {
            dynamic param = new ExpandoObject();
            param.id = id;
            return await _apiHttpClient.ServiceAsync<Publication>(KL2_Server.API, nameof(PrepareService), nameof(GetAction), param);
        }

        /// <summary>
        /// Sauvegarde les formations
        /// </summary>
        /// <param name="trainings">Liste des formations à sauvegarder</param>
        /// <returns>La liste des formations sauvegardé</returns>
        public async Task<Training[]> SaveTrainings(Training[] trainings)
        {
            dynamic param = new ExpandoObject();
            param.trainings = trainings;
            return await _apiHttpClient.ServiceAsync<Training[]>(KL2_Server.API, nameof(PrepareService), nameof(SaveTrainings), param);
        }

        public async Task<Timeslot[]> SaveTimeslots(Timeslot[] timeslots)
        {
            dynamic param = new ExpandoObject();
            param.timeslots = timeslots;
            return await _apiHttpClient.ServiceAsync<Timeslot[]>(KL2_Server.API, nameof(PrepareService), nameof(SaveTimeslots), param);
        }

        /// <summary>
        /// Retrouve une qualification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Qualification> GetQualification(int id)
        {
            dynamic param = new ExpandoObject();
            param.id = id;
            return await _apiHttpClient.ServiceAsync<Qualification[]>(KL2_Server.API, nameof(PrepareService), nameof(GetQualification), param);
        }

        #endregion

        #region Inspection

        /// <summary>
        /// Méthode permettant de récupérer la dernière inspection d'une publication
        /// </summary>
        /// <param name="publicationId">Identifiant de la publication</param>
        /// <returns>La dernière inspection ou null s'il n'en existe pas avec les paramètres spécifié</returns>
        public async Task<Inspection> GetLastInspection(Guid publicationId)
        {
            dynamic param = new ExpandoObject();
            param.publicationId = publicationId;
            return await _apiHttpClient.ServiceAsync<Inspection>(KL2_Server.API, nameof(PrepareService), nameof(GetLastInspection), param);
        }

        public async Task<Inspection> GetInspection(int inspectionId)
        {
            dynamic param = new ExpandoObject();
            param.inspectionId = inspectionId;
            return await _apiHttpClient.ServiceAsync<Inspection>(KL2_Server.API, nameof(PrepareService), nameof(GetInspection), param);
        }

        /// <summary>
        /// Get all inspections
        /// </summary>
        public async Task<IEnumerable<Inspection>> GetInspections(Guid? publicationId = null)
        {
            dynamic param = new ExpandoObject();
            param.publicationId = publicationId;
            return await _apiHttpClient.ServiceAsync<IEnumerable<Inspection>>(KL2_Server.API, nameof(PrepareService), nameof(GetInspections), param);
        }

        /// <summary>
        /// Get all timeslots
        /// </summary>
        public async Task<IEnumerable<Timeslot>> GetTimeslots(int? timeslotId = null)
        {
            dynamic param = new ExpandoObject();
            param.timeslotId = timeslotId;
            return await _apiHttpClient.ServiceAsync<IEnumerable<Timeslot>>(KL2_Server.API, nameof(PrepareService), nameof(GetTimeslots), param);
        }

        /// <summary>
        /// Get all inspections schedule exclude inspections completed
        /// </summary>
        public async Task<IEnumerable<InspectionSchedule>> GetInspectionSchedules(int? InspectionScheduleId = null)
        {
            {
                dynamic param = new ExpandoObject();
                param.InspectionScheduleId = InspectionScheduleId;
                return await _apiHttpClient.ServiceAsync<IEnumerable<InspectionSchedule>>(KL2_Server.API, nameof(PrepareService), nameof(GetInspectionSchedules), param);
            }
        }

        /// <summary>
        /// Get all inspections schedule
        /// </summary>
        public async Task<IEnumerable<InspectionSchedule>> GetInspectionSchedulesNonFilter(int? InspectionScheduleId = null)
        {
            {
                dynamic param = new ExpandoObject();
                param.InspectionScheduleId = InspectionScheduleId;
                return await _apiHttpClient.ServiceAsync<IEnumerable<InspectionSchedule>>(KL2_Server.API, nameof(PrepareService), nameof(GetInspectionSchedulesNonFilter), param);
            }
        }

        /// <summary>
        /// Get all inspections schedule for timeslot
        /// </summary>
        public async Task<IEnumerable<InspectionSchedule>> GetInspectionsScheduleForTimeslot(int timeslotId)
        {
            {
                dynamic param = new ExpandoObject();
                param.timeslotId = timeslotId;
                return await _apiHttpClient.ServiceAsync<IEnumerable<InspectionSchedule>>(KL2_Server.API, nameof(PrepareService), nameof(GetInspectionsScheduleForTimeslot), param);
            }
        }

        /// <summary>
        /// Sauvegarde les inspections
        /// </summary>
        /// <param name="inspections">Liste des inspections à sauvegarder</param>
        /// <returns>La liste des inspections sauvegardées</returns>
        public async Task<Inspection[]> SaveInspections(Inspection[] inspections)
        {
            dynamic param = new ExpandoObject();
            param.inspections = inspections;
            return await _apiHttpClient.ServiceAsync<Inspection[]>(KL2_Server.API, nameof(PrepareService), nameof(SaveInspections), param);
        }

        /// <summary>
        /// Save inspection schedule
        /// </summary>
        /// <param name="schedule">Schedule of Inspection</param>
        /// <returns>Saved inspection schedule</returns>
        public async Task<InspectionSchedule> SaveInspectionSchedule(InspectionSchedule schedule)
        {
            dynamic param = new ExpandoObject();
            param.schedule = schedule;
            return await _apiHttpClient.ServiceAsync<Inspection[]>(KL2_Server.API, nameof(PrepareService), nameof(SaveInspectionSchedule), param);
        }

        public async Task<Anomaly[]> GetAnomalies(int inspectionId)
        {
            dynamic param = new ExpandoObject();
            param.inspectionId = inspectionId;
            return await _apiHttpClient.ServiceAsync<Anomaly[]>(KL2_Server.API, nameof(PrepareService), nameof(GetAnomalies), param);
        }

        public async Task<Anomaly> GetAnomaly(int AnomalyId)
        {
            dynamic param = new ExpandoObject();
            param.AnomalyId = AnomalyId;
            return await _apiHttpClient.ServiceAsync<Anomaly>(KL2_Server.API, nameof(PrepareService), nameof(GetAnomaly), param);
        }

        #endregion

        #region Documentation

        /// <summary>
        /// Retrieve the draft documentation for this scenario id
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public async Task<bool> HasDocumentationDraft(int scenarioId)
        {
            dynamic param = new ExpandoObject();
            param.scenarioId = scenarioId;
            return await _apiHttpClient.ServiceAsync<bool>(KL2_Server.API, nameof(PrepareService), nameof(HasDocumentationDraft), param);
        }

        /// <summary>
        /// Retrieve the draft documentation for this scenario id
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public bool HasDocumentationDraftSync(int scenarioId)
        {
            dynamic param = new ExpandoObject();
            param.scenarioId = scenarioId;
            return _apiHttpClient.Service<bool>(KL2_Server.API, nameof(PrepareService), nameof(HasDocumentationDraftSync), param);
        }

        /// <summary>
        /// Retrieve al the draft documentation for this project id
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public async Task<DocumentationDraft> GetLastDocumentationDraft(int processId, int scenarioId)
        {
            dynamic param = new ExpandoObject();
            param.processId = processId;
            param.scenarioId = scenarioId;
            return await _apiHttpClient.ServiceAsync<DocumentationDraft>(KL2_Server.API, nameof(PrepareService), nameof(GetLastDocumentationDraft), param);
        }


        /// <summary>
        /// Retrieve the draft documentation
        /// </summary>
        /// <param name="documentationDraftId"></param>
        /// <returns></returns>
        public async Task<DocumentationDraft> GetDocumentationDraft(int documentationDraftId)
        {
            dynamic param = new ExpandoObject();
            param.documentationDraftId = documentationDraftId;
            return await _apiHttpClient.ServiceAsync<DocumentationDraft>(KL2_Server.API, nameof(PrepareService), nameof(GetDocumentationDraft), param);
        }

        public async Task<IEnumerable<ProjectReferential>> GetUsedReferentials(int projectId)
        {
            dynamic param = new ExpandoObject();
            param.projectId = projectId;
            return await _apiHttpClient.ServiceAsync<IEnumerable<ProjectReferential>>(KL2_Server.API, nameof(PrepareService), nameof(GetUsedReferentials), param);
        }

        public async Task<long> GetProjectTimeScale(int scenarioId)
        {
            dynamic param = new ExpandoObject();
            param.scenarioId = scenarioId;
            return await _apiHttpClient.ServiceAsync<long>(KL2_Server.API, nameof(PrepareService), nameof(GetProjectTimeScale), param);
        }

        /// <summary>
        /// Save documentation draft
        /// </summary>
        /// <param name="documentationDraft"></param>  
        /// <returns></returns>
        public Task<DocumentationDraft> SaveDocumentationDraft(DocumentationDraft documentationDraft)
        {
            dynamic param = new ExpandoObject();
            param.documentationDraft = documentationDraft;
            return _apiHttpClient.ServiceAsync<DocumentationDraft>(KL2_Server.File, nameof(PrepareService), nameof(SaveDocumentationDraft), param);
        }

        /// <summary>
        /// Save documentation draft information for a specific project id and scenario id
        /// </summary>
        /// <param name="documentation"></param>
        /// <param name="projectId"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public async Task SaveDocumentationDraft(DocumentationDraft documentation, List<DocumentationActionDraft> actions, List<DocumentationActionDraftWBS> actionsWbs, int projectId, int scenarioId)
        {
            dynamic param = new ExpandoObject();
            param.documentation = documentation;
            param.projectId = projectId;
            param.scenarioId = scenarioId;
            param.actions = actions;
            param.actionsWbs = actionsWbs;
            await _apiHttpClient.Service<bool>(KL2_Server.File, nameof(PrepareService), nameof(SaveDocumentationDraft), param);
        }

        public async Task SaveDocumentationVideos(int documentationDraftId,
            bool activeVideoExport,
            bool slowMotion,
            double slowMotionDuration,
            bool waterMarking,
            string waterMarkingText,
            EVerticalAlign waterMarkingVAlign,
            EHorizontalAlign waterMarkingHAlign)
        {
            dynamic param = new ExpandoObject();
            param.documentationDraftId = documentationDraftId;
            param.activeVideoExport = activeVideoExport;
            param.slowMotion = slowMotion;
            param.slowMotionDuration = slowMotionDuration;
            param.waterMarking = waterMarking;
            param.waterMarkingText = waterMarkingText;
            param.waterMarkingVAlign = waterMarkingVAlign;
            param.waterMarkingHAlign = waterMarkingHAlign;
            await _apiHttpClient.Service<bool>(KL2_Server.File, nameof(PrepareService), nameof(SaveDocumentationVideos), param);
        }

        #endregion
    }
}