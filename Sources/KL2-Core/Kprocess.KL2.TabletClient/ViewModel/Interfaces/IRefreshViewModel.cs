using System.Threading.Tasks;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    /// <summary>
    /// Interface marqueuse permettant d'indiquer au view model de se rafraichir si besoin
    /// </summary>
    public interface IRefreshViewModel
    {
        /// <summary>
        /// Méthode permettant de refraichir les données du view model
        /// </summary>
        Task Refresh();
    }
}
