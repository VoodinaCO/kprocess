using KProcess.Ksmed.Business;
using System.ComponentModel.Composition;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Security
{
    /// <summary>
    /// Représente le mode d'authentification par base de données Ksmed.
    /// </summary>
    public class KSmedAuthenticationMode : IAuthenticationMode
    {
        public static string StaticName = "KSMED";

#pragma warning disable CS0649
        [Import]
        IServiceBus _serviceBus;
#pragma warning restore CS0649

        /// <summary>
        /// Obtient le nom unique de ce mode d'authentification.
        /// </summary>
        public string Name
        {
            get { return StaticName; }
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

            return _serviceBus.Get<Business.IAuthenticationService>().IsUserValid(username, passwordHash);

        }
    }
}
