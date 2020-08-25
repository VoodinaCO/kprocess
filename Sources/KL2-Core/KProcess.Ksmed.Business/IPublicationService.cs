using KProcess.Business;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Definit le comprotement d'un service de gestion de la partie fonctionnelle de publication des projects
    /// </summary>
    public interface IPublicationService : IBusinessService
    {
        string PublishedFilesDirectory { get; set; }

        Task<Publication[]> GetPublications();

        Task<int> Publish(Publication publication,
            bool exportVideo,
            bool exportOnlyKeyTasksVideos);

        Task<int> PublishMulti(List<Publication> publications,
            bool exportVideo,
            bool exportOnlyKeyTasksVideos);

        Task<int> FakingPublication();

        Task<bool> CancelPublication(int publicationHistoryId);

        Dictionary<int, PublishTaskProgressInfos> GetProgresses();

        Task<Dictionary<int, PublicationStatus?>> GetProcessesPublicationStatus(int[] processIds);

        PublishTaskProgressInfos GetProgress(int publicationHistoryId);

        Task<CutVideo> GetCutVideo(string hash);

        Task<string> GetCurrentVersion(int processId, PublishModeEnum publishMode);

        Task<string> GetFutureVersion(int processId, PublishModeEnum publishMode, bool isMajor);

        Task<PublicationHistory[]> GetPublicationHistories(int pageNumber = 0, int pageSize = 10);

        Task<PublicationHistory> GetPublicationHistory(int publicationHistoryId);
    }

    public class PublishTaskInfos
    {
        public CancellationTokenSource Cts { get; }

        public Task Task { get; }

        public PublishTaskProgressInfos ProgressInfo { get; }

        public string TrainingPublicationVersion { get; set; }

        public string EvaluationPublicationVersion { get; set; }

        public string InspectionPublicationVersion { get; set; }

        public PublishTaskInfos(Task task, CancellationTokenSource cts,
            string trainingPublicationVersion = null, string evaluationPublicationVersion = null, string inspectionPublicationVersion = null)
        {
            Task = task;
            Cts = cts;
            ProgressInfo = new PublishTaskProgressInfos();
            TrainingPublicationVersion = trainingPublicationVersion;
            EvaluationPublicationVersion = evaluationPublicationVersion;
            InspectionPublicationVersion = inspectionPublicationVersion;
        }
    }

    public class PublishTaskProgressInfos
    {
        public string Step { get; set; }

        public double? Progress { get; set; }

        public bool CanCancel { get; set; }

        public PublicationStatus Status { get; set; }

        public string ErrorMessage { get; set; }

        public PublishTaskProgressInfos() { }

        public PublishTaskProgressInfos(PublicationStatus status, string step, double? progress, bool canCancel, string errorMessage = null)
        {
            Status = status;
            Step = step;
            Progress = progress;
            CanCancel = canCancel;
            ErrorMessage = errorMessage;
        }
    }
}