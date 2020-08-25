using KProcess.Presentation.Windows;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit le comportement d'un service capable de déconnecter l'utilisateur courant.
    /// </summary>
    public interface IDisconnectionService : IPresentationService
    {

        /// <summary>
        /// Tente de déconnecter l'utilisateur courant.
        /// </summary>
        /// <returns><c>true</c> si la déconnexion a réussi.</returns>
        Task<bool> Disconnect();

    }
}
