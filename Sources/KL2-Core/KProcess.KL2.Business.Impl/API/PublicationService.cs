using KProcess.Business;
using KProcess.KL2.APIClient;
using KProcess.KL2.Business.Impl.Shared;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.API
{
    /// <summary>
    /// Représente un service de gestion des publications
    /// </summary>
    public class PublicationService : IBusinessService, IPublicationService
    {
        readonly IAPIHttpClient _apiHttpClient;
        readonly ITraceManager _traceManager;
        readonly ISecurityContext _securityContext;

        const string TemporaryFilesDirectory = @"TemporaryFiles\";

        public string PublishedFilesDirectory
        {
            get => ServiceConst.PublishedFilesDirectory;
            set => ServiceConst.PublishedFilesDirectory = value;
        }

        public PublicationService(IAPIHttpClient apiHttpClient, ISecurityContext securityContext, ITraceManager traceManager)
        {
            _apiHttpClient = apiHttpClient;
            _securityContext = securityContext;
            _traceManager = traceManager;
        }

        public Task<Publication[]> GetPublications() =>
            _apiHttpClient.ServiceAsync<Publication[]>(KL2_Server.File, nameof(PublicationService), nameof(GetPublications));

        public async Task<int> Publish(Publication publication,
            bool exportVideo,
            bool exportOnlyKeyTasksVideos)
        {
            _traceManager.TraceInfo("Publish using API");
            dynamic param = new ExpandoObject();
            param.publication = publication;
            param.exportVideo = exportVideo;
            param.exportOnlyKeyTasksVideos = exportOnlyKeyTasksVideos;

            int publicationHistoryId = await _apiHttpClient.ServiceAsync<Guid>(KL2_Server.File, nameof(PublicationService), nameof(Publish), param);

            _traceManager.TraceInfo($"Publish terminated using API with Id={publicationHistoryId}");
            return publicationHistoryId;
        }

        public async Task<int> PublishMulti(List<Publication> publications,
            bool exportVideo,
            bool exportOnlyKeyTasksVideos)
        {
            _traceManager.TraceInfo("PublishMulti using API");
            dynamic param = new ExpandoObject();
            param.publications = publications;
            param.exportVideo = exportVideo;
            param.exportOnlyKeyTasksVideos = exportOnlyKeyTasksVideos;

            int publicationHistoryId = await _apiHttpClient.ServiceAsync<int>(KL2_Server.File, nameof(PublicationService), nameof(PublishMulti), param);

            _traceManager.TraceInfo($"PublishMulti terminated using API with Id={publicationHistoryId}");
            return publicationHistoryId;
        }

        public Task<int> FakingPublication()
        {
            return _apiHttpClient.ServiceAsync<int>(KL2_Server.File, nameof(PublicationService), nameof(FakingPublication));
        }

        public async Task<string> GetCurrentVersion(int processId, PublishModeEnum publishMode)
        {
            dynamic param = new ExpandoObject();
            param.processId = processId;
            param.publishMode = (int)publishMode;
            return await _apiHttpClient.ServiceAsync<string>(KL2_Server.File, nameof(PublicationService), nameof(GetCurrentVersion), param);
        }

        public async Task<string> GetFutureVersion(int processId, PublishModeEnum publishMode, bool isMajor)
        {
            dynamic param = new ExpandoObject();
            param.processId = processId;
            param.publishMode = (int)publishMode;
            param.isMajor = isMajor;
            return await _apiHttpClient.ServiceAsync<string>(KL2_Server.File, nameof(PublicationService), nameof(GetFutureVersion), param);
        }

        public Task<bool> CancelPublication(int publicationHistoryId)
        {
            dynamic param = new ExpandoObject();
            param.publicationHistoryId = publicationHistoryId;
            return _apiHttpClient.ServiceAsync<bool>(KL2_Server.File, nameof(PublicationService), nameof(CancelPublication), param);
        }

        public PublishTaskProgressInfos GetProgress(int publicationHistoryId)
        {
            dynamic param = new ExpandoObject();
            param.publicationHistoryId = publicationHistoryId;
            return _apiHttpClient.Service<PublishTaskProgressInfos>(KL2_Server.File, nameof(PublicationService), nameof(GetProgress), param);
        }

        public Dictionary<int, PublishTaskProgressInfos> GetProgresses() =>
            _apiHttpClient.Service<Dictionary<int, PublishTaskProgressInfos>>(KL2_Server.File, nameof(PublicationService), nameof(GetProgresses));

        public Task<Dictionary<int, PublicationStatus?>> GetProcessesPublicationStatus(int[] processIds)
        {
            dynamic param = new ExpandoObject();
            param.processIds = processIds;
            return _apiHttpClient.ServiceAsync<Dictionary<int, PublicationStatus?>>(KL2_Server.File, nameof(PublicationService), nameof(GetProcessesPublicationStatus), param);
        }

        public async Task<CutVideo> GetCutVideo(string hash)
        {
            dynamic param = new ExpandoObject();
            param.hash = hash;
            return await _apiHttpClient.ServiceAsync<CutVideo>(KL2_Server.File, nameof(PublicationService), nameof(GetCutVideo), param);
        }

        public async Task<PublicationHistory[]> GetPublicationHistories(int pageNumber = 0, int pageSize = 10)
        {
            dynamic param = new ExpandoObject();
            param.pageNumber = pageNumber;
            param.pageSize = pageSize;
            return await _apiHttpClient.ServiceAsync<PublicationHistory[]>(KL2_Server.File, nameof(PublicationService), nameof(GetPublicationHistories), param);
        }

        public async Task<PublicationHistory> GetPublicationHistory(int publicationHistoryId)
        {
            dynamic param = new ExpandoObject();
            param.publicationHistoryId = publicationHistoryId;
            return await _apiHttpClient.ServiceAsync<PublicationHistory>(KL2_Server.File, nameof(PublicationService), nameof(GetPublicationHistory), param);
        }
    }
}