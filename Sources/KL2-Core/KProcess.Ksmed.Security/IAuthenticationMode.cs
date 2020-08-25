using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Security
{
    /// <summary>
    /// Définit le comportement d'un mode d'authentification.
    /// </summary>
    [InheritedExport]
    public interface IAuthenticationMode
    {

        /// <summary>
        /// Obtient le nom unique de ce mode d'authentification.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Obtient une valeur indiquant si le mode d'authentification requiert un nom de domaine.
        /// </summary>
        bool NeedsDomain { get; }

        /// <summary>
        /// Tente d'authentifier un utilisateur.
        /// </summary>
        /// <param name="username">Nom de l'utilisateur.</param>
        /// <param name="password">Le mot de passe.</param>
        /// <param name="domain">Le domaine.</param>
        /// <returns><c>true</c> si l'authentification a réussi.</returns>
        Task<bool> TryLogonUser(string username, string password, string domain);

    }
}
