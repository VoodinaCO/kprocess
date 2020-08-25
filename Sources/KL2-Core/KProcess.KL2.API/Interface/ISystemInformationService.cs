using System.Threading.Tasks;
using System.Web.Http;

namespace KProcess.KL2.API
{
    /// <summary>
    /// Décrit le comportement d'un service fournissant des informatiosn sur le système.
    /// </summary>
    public interface ISystemInformationServiceController
    {
        Task<IHttpActionResult> GetBasicInformation();
    }
}
