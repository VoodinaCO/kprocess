using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Definit le comprotement d'un service de gestion des ressources de l'application.
    /// </summary>
    public interface IAppResourceServiceController
    {
        Task<IHttpActionResult> GetLanguages();

        IHttpActionResult GetAllResources();

        IHttpActionResult GetResources(dynamic param);

        Task<IHttpActionResult> GetResource(dynamic param);
    }
}