using System.Threading.Tasks;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    /// <summary>
    /// Interface marqueuse permettant d'indiquer au view model d'attendre un résultat
    /// </summary>
    public interface IWaitResultViewModel<T>
    {
        /// <summary>
        /// Méthode permettant de traiter le résultat
        /// </summary>
        Task ComputeResult(T result);
    }
}
