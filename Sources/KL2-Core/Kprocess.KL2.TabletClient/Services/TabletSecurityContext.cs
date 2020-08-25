using Kprocess.KL2.TabletClient;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Security;
using KProcess.Ksmed.Security.Business;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Security
{
    /// <summary>
    /// Représente le context de sécurité de l'application.
    /// </summary>
    public class TabletSecurityContext : ISecurityContext
    {
        static bool _isInitialized;

        /// <summary>
        /// Initialise le contexte.
        /// </summary>
        public TabletSecurityContext()
        {
            _isInitialized = true;
        }

        /// <summary>
        /// Obtient l'utilisateur courant.
        /// </summary>
        public SecurityUser CurrentUser
        {
            get => LogonManager.CurrentSecurityUser;
            set => LogonManager.CurrentSecurityUser = value;
        }

        /// <summary>
        /// Tente d'authentifier l'utilisateur.
        /// </summary>
        /// <param name="username">Le nom de l'utilisateur.</param>
        /// <param name="password">Le mot de passe.</param>
        /// <param name="domain">Le domaine.</param>
        public async Task<(User User, bool Result)> TryLogonUser(string username, string password, string domain)
        {
            if (!_isInitialized)
                throw new Exception("Contexte non initialisé");

            // Récupérer l'utilisateur en base
            //TraceManager.TraceDebug("SecurityContext.TryLogonUser : tentative de connexion : {0}", username);

            User user = await Locator.GetService<IAuthenticationService>().GetUser(username);

            if (user != null)
            {
                // TraceManager.TraceDebug("SecurityContext.TryLogonUser : utilisateur trouvé : {0}", user.Username);
                // Récupérer le mode d'authentification
                var authMode = new APIAuthenticationMode();

                // Authentifie l'utilisateur
                if (await authMode.TryLogonUser(username, password ?? string.Empty, domain))
                {
                    //   TraceManager.TraceDebug("SecurityContext.TryLogonUser : connexion réussie");
                    CurrentUser = new SecurityUser(user);
                    return (user, true);
                }
                return (user, false);
            }
            return (null, false);
        }

        /// <summary>
        /// Reconnecte l'utilisateur.
        /// </summary>
        /// <param name="user">L'utilisateur.</param>
        public bool ReconnectUser(User user)
        {
            if (!_isInitialized)
                throw new Exception("Contexte non initialisé");

            if (user != null)
            {
                // Authentifie l'utilisateur
                CurrentUser = new SecurityUser(user);
                return true;
            }
            return false;
        }

        /// <summary>
        /// S'authentifie automatique avec le compte admin par défaut.
        /// </summary>
        public async Task AutoLogAsDefaultAdmin()
        {
            User user = await Locator.GetService<IAuthenticationService>().GetUser(SecurityConstants.DefaultAdminUsername);
            if (user == null)
                throw new InvalidOperationException("Il n'y a pas d'admin par défaut en base");

            CurrentUser = new SecurityUser(user);
        }

        public void AutoLogAsForTests(User user) =>
            CurrentUser = new SecurityUser(user);

        /// <summary>
        /// Déconnecte l'utilisateur courant.
        /// </summary>
        public void DisconnectCurrentUser()
        {
            CurrentUser = null;
        }

        /// <summary>
        /// Obtient ou définit les informations de la licence actuellement chargée.
        /// </summary>
        public Activation.ProductLicense CurrentProductLicense { get; set; }

        /// <summary>
        /// Détermine si l'utilisateur actuellement connecté peut lire sur le type spécifié.
        /// </summary>
        /// <typeparam name="T">Le type</typeparam>
        /// <returns>
        ///   <c>true</c> si l'utilisateur actuellement connecté peut lire sur le type spécifié; sinon, <c>false</c>.
        /// </returns>
        public bool CanUserRead<T>()
        {
            return true;
        }

        /// <summary>
        /// Détermine si le role spécifié peut lire sur le type spécifié.
        /// </summary>
        /// <typeparam name="T">Le type</typeparam>
        /// <param name="role">Le role.</param>
        /// <returns>
        ///   <c>true</c> si le role spécifié peut lire sur le type spécifié; sinon, <c>false</c>.
        /// </returns>
        public bool CanRoleRead<T>(string role)
        {
            return true;
        }

        /// <summary>
        /// Détermine si l'utilisateur actuellement connecté peut écrire sur le type spécifié.
        /// </summary>
        /// <typeparam name="T">Le type</typeparam>
        /// <returns>
        ///   <c>true</c> si l'utilisateur actuellement connecté peut écrire sur le type spécifié; sinon, <c>false</c>.
        /// </returns>
        public bool CanUserWrite<T>()
        {
            return true;
        }

        /// <summary>
        /// Détermine si le role spécifié peut écrire sur le type spécifié.
        /// </summary>
        /// <typeparam name="T">Le type</typeparam>
        /// <param name="role">Le role.</param>
        /// <returns>
        ///   <c>true</c> si le role spécifié peut écrire sur le type spécifié; sinon, <c>false</c>.
        /// </returns>
        public bool CanRoleWrite<T>(string role)
        {
            return true;
        }

        /// <summary>
        /// Détermine si le l'utilisateur a accès au rôle spécifié.
        /// </summary>
        /// <param name="role">le rôle.</param>
        /// <returns>
        ///   <c>true</c> si le l'utilisateur a accès au rôle spécifié; sinon, <c>false</c>.
        /// </returns>
        public bool HasCurrentUserRole(string role) =>
            CurrentUser.HasRole(role);

        /// <summary>
        /// Détermine si la licence chargée possède la fonctionnalité spécifiée.
        /// </summary>
        /// <param name="feature">La fonctionnalité.</param>
        /// <returns>
        ///   <c>true</c> si la licence chargée possède la fonctionnalité spécifiée.; sinon, <c>false</c>.
        /// </returns>
        public bool HasCurrentLicenseFeature(Activation.ActivationFeatures feature) =>
            CurrentProductLicense != null && CurrentProductLicense.ProductFeatures == (short)feature;

        /// <summary>
        /// Enregistre les autorisations.
        /// </summary>
        public void RegisterAuthorizations(
            IDictionary<Type, string[]> rolesReadAuthorizations,
            IDictionary<Type, string[]> rolesWriteAuthorizations,
            IDictionary<Type, short[]> featuresReadAuthorizations,
            IDictionary<Type, short[]> featuresWriteAuthorizations,
            IDictionary<Type, Func<string, bool>> customReadAuthorizations,
            IDictionary<Type, Func<string, bool>> customWriteAuthorizations)
        {
        }

        /// <summary>
        /// Obtient une valeur indiquant si l'utilisateur est l'admin par défaut
        /// </summary>
        /// <param name="user">L'utilisateur</param>
        /// <returns><c>true</c> si l'utilisateur est l'admin par défaut</returns>
        public bool IsUserDefaultAdmin(User user)
        {
            return string.Compare(user.Username, SecurityConstants.DefaultAdminUsername, true) == 0 ||
                user.UserId == SecurityConstants.DefaultAdminUserId;
        }

        public bool CurrentAuthenticationModeNeedsDomain()
        {
            return false;
        }
    }
}