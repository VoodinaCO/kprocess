using KProcess.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Security;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Security.Business
{

    /// <summary>
    /// Definit le comprotement d'un service d'authentification.
    /// </summary>
    public interface IAuthenticationService : IBusinessService
    {

        /// <summary>
        /// Obtient l'utilisateur associé au login spécifié.
        /// </summary>
        /// <param name="username">Le login de l'utilisateur.</param>
        Task<User> GetUser(string username, string language = null);

        /// <summary>
        /// Obtient l'utilisateur associé au login spécifié.
        /// </summary>
        /// <param name="username">Le login de l'utilisateur.</param>
        User GetUserAsSync(string username, string language = null);

        /// <summary>
        /// Indique si le couple login / mot de passe est valide.
        /// </summary>
        /// <param name="username">Le login de l'utilisateur.</param>
        /// <param name="password">Le mot de passe.</param>
        /// <returns><c>true</c> si le couple est valide.</returns>
        Task<bool> IsUserValid(string username, byte[] password);
        
        /// <summary>
        /// Sauvegarde l'utilisateur.
        /// </summary>
        /// <param name="user">L'utilisateur.</param>
        Task SaveUser(User user, SecurityUser securityUser = null);

    }

}
