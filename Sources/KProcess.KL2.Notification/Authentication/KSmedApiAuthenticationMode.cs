using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Business;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kprocess.KL2.Notification.Authentication
{
    /// <summary>
    /// Représente le mode d'authentification par base de données Ksmed.
    /// </summary>
    public class KSmedAPIAuthenticationMode : IAuthenticationMode
    {
        readonly IAuthenticationService _authenticationService;

        public KSmedAPIAuthenticationMode(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Obtient le nom unique de ce mode d'authentification.
        /// </summary>
        public string Name
        {
            get { return "KSMED"; }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le mode d'authentification requiert un nom de domaine.
        /// </summary>
        public bool NeedsDomain
        {
            get { return false; }
        }

        /// <summary>
        /// Tente d'authentifier un utilisateur.
        /// </summary>
        /// <param name="username">Nom de l'utilisateur.</param>
        /// <param name="password">Le mot de passe.</param>
        /// <param name="domain">Le domaine.</param>
        /// <returns>
        ///   <c>true</c> si l'authentification a réussi.
        /// </returns>
        public Task<bool> TryLogonUser(string username, string password, string domain)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();

            byte[] passwordHash = sha.ComputeHash(Encoding.Default.GetBytes(password));

            return _authenticationService.IsUserValid(username, passwordHash);
        }
    }
}
