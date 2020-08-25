using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API
{
    /// <summary>
    /// Definit le comprotement d'un service de gestion de la partie fonctionnelle "Préparer".
    /// </summary>
    public interface IPrepareServiceController
    {
        Task<IHttpActionResult> SetReadPublication(dynamic param);

        Task<IHttpActionResult> PublicationExistsForProcess(dynamic param);

        IHttpActionResult PublicationExistsForProcessSync(dynamic param);

        Task<IHttpActionResult> GetPublication(dynamic param);

        Task<IHttpActionResult> GetLastPublication(dynamic param);

        Task<IHttpActionResult> CheckAuditorHaveActiveAudit(dynamic param);

        Task<IHttpActionResult> GetPublicationToAudit(dynamic param);

        Task<IHttpActionResult> AllLinkedProcessArePublished(dynamic param);

        IHttpActionResult AllLinkedProcessArePublishedSync(dynamic param);

        Task<IHttpActionResult> GetProject(dynamic param);

        IHttpActionResult GetProjectSync(dynamic param);

        Task<IHttpActionResult> GetPublicationsTree(dynamic param);

        Task<IHttpActionResult> GetProjects();

        Task<IHttpActionResult> GetProjectDirs();

        Task<IHttpActionResult> GetProcesses();

        Task<IHttpActionResult> ProcessIsLinkedToATask(dynamic param);

        Task<IHttpActionResult> GetFullName(dynamic param);

        Task<IHttpActionResult> SaveProject(dynamic param);

        Task<IHttpActionResult> SaveFolder(dynamic param);

        Task<IHttpActionResult> SaveProcess(dynamic param);

        Task<IHttpActionResult> GetQualificationReasons();

        Task<IHttpActionResult> SaveQualificationReasons(dynamic param);

        Task<IHttpActionResult> GetMembers(dynamic param);

        Task<IHttpActionResult> SaveMember(dynamic param);

        Task<IHttpActionResult> GetReferentials(dynamic param);

        Task<IHttpActionResult> SaveReferentials(dynamic param);

        Task<IHttpActionResult> GetAllResources(dynamic param);

        Task<IHttpActionResult> GetVideos(dynamic param);

        Task<IHttpActionResult> SaveVideo(dynamic param);

        Task<IHttpActionResult> GetScenarios(dynamic param);

        Task<IHttpActionResult> GetScenario(dynamic param);

        Task<IHttpActionResult> CreateInitialScenario(dynamic param);

        Task<IHttpActionResult> CreateScenario(dynamic param);

        Task<IHttpActionResult> DeleteScenario(dynamic param);

        Task<IHttpActionResult> SaveScenario(dynamic param);

        Task<IHttpActionResult> CreateNewProjectFromValidatedScenario(dynamic param);

        Task<IHttpActionResult> UpdateScenarioPublicationGuid(dynamic param);

        Task<IHttpActionResult> GetAnomalies(dynamic param);
    }

}
