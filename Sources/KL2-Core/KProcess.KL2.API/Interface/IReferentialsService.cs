using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API
{
    /// <summary>
    /// Definit le comportement d'un service de gestion des référentiels d'actions.
    /// </summary>
    public interface IReferentialsServiceController
    {
        Task<IHttpActionResult> GetApplicationReferentials();

        Task<IHttpActionResult> UpdateReferentialLabel(dynamic param);

        Task<IHttpActionResult> LoadCategories();

        Task<IHttpActionResult> SaveCategories(dynamic param);

        Task<IHttpActionResult> LoadSkills(dynamic param);

        Task<IHttpActionResult> SaveSkills(dynamic param);

        Task<IHttpActionResult> GetReferentials(dynamic param);

        Task<IHttpActionResult> SaveReferentials(dynamic param);

        Task<IHttpActionResult> LoadEquipments();

        Task<IHttpActionResult> LoadOperators();

        Task<IHttpActionResult> SaveResources(dynamic param);

        Task<IHttpActionResult> MergeReferentials(dynamic param);
    }
}