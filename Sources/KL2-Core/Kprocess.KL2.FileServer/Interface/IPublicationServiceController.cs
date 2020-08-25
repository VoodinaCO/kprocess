using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Kprocess.KL2.FileServer
{
    /// <summary>
    /// Definit le comportement d'un controller de service de Publication.
    /// </summary>
    public interface IPublicationServiceController
    {
        Task<IHttpActionResult> GetPublications();

        Task<IHttpActionResult> Publish(dynamic param);

        Task<IHttpActionResult> CancelPublication(dynamic param);

        IHttpActionResult GetProgresses();

        IHttpActionResult GetProgress(dynamic param);

        Task<HttpResponseMessage> GetFile(string hash);

        IHttpActionResult GetFileSize(dynamic param);

        IHttpActionResult GetFilesSize(dynamic param);

        Task<IHttpActionResult> GetPublicationHistories(dynamic param);
    }
}
