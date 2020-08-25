using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API
{
    /// <summary>
    /// Definit le comportement d'un service d'import/export.
    /// </summary>
    public interface IImportExportServiceController
    {
        Task<IHttpActionResult> ExportProject(dynamic param);

        Task<IHttpActionResult> PredictMergedReferentialsProject(dynamic param);

        Task<IHttpActionResult> ImportProject(dynamic param);

        Task<IHttpActionResult> ExportVideoDecomposition(dynamic param);

        Task<IHttpActionResult> PredictMergedReferentialsVideoDecomposition(dynamic param);

        Task<IHttpActionResult> ImportVideoDecomposition(dynamic param);
    }
}