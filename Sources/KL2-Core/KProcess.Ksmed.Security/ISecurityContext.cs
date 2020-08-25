using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Security
{
    /// <summary>
    /// Représente le context de sécurité de l'application.
    /// </summary>
    public interface ISecurityContext
    {

        /// <summary>
        /// Obtient l'utilisateur courant.
        /// </summary>
        SecurityUser CurrentUser { get; set; }

        /// <summary>
        /// Tente d'authentifier l'utilisateur.
        /// </summary>
        /// <param name="username">Le nom de l'utilisateur.</param>
        /// <param name="password">Le mot de passe.</param>
        /// <param name="domain">Le domaine.</param>
        Task<(User User, bool Result)> TryLogonUser(string username, string password, string domain);

        /// <summary>
        /// Reconnecte l'utilisateur.
        /// </summary>
        /// <param name="user">L'utilisateur.</param>
        bool ReconnectUser(User user);

        /// <summary>
        /// S'authentifie automatique avec le compte admin par défaut.
        /// </summary>
        Task AutoLogAsDefaultAdmin();

        void AutoLogAsForTests(User user);

        /// <summary>
        /// Déconnecte l'utilisateur courant.
        /// </summary>
        void DisconnectCurrentUser();


        /// <summary>
        /// Obtient une valeur indiquant si le mode d'authentification courant nécessite un domaine pour fonctionner.
        /// </summary>
        /// <returns><c>true</c> si le mode d'authentification courant nécessite un domaine pour fonctionner.</returns>
        bool CurrentAuthenticationModeNeedsDomain();

        /// <summary>
        /// Obtient ou définit les informations de la licence actuellement chargée.
        /// </summary>
        Activation.ProductLicense CurrentProductLicense { get; set; }

        /// <summary>
        /// Détermine si l'utilisateur actuellement connecté peut lire sur le type spécifié.
        /// </summary>
        /// <typeparam name="T">Le type</typeparam>
        /// <returns>
        ///   <c>true</c> si l'utilisateur actuellement connecté peut lire sur le type spécifié; sinon, <c>false</c>.
        /// </returns>
        bool CanUserRead<T>();

        /// <summary>
        /// Détermine si le role spécifié peut lire sur le type spécifié.
        /// </summary>
        /// <typeparam name="T">Le type</typeparam>
        /// <param name="role">Le role.</param>
        /// <returns>
        ///   <c>true</c> si le role spécifié peut lire sur le type spécifié; sinon, <c>false</c>.
        /// </returns>
        bool CanRoleRead<T>(string role);

        /// <summary>
        /// Détermine si l'utilisateur actuellement connecté peut écrire sur le type spécifié.
        /// </summary>
        /// <typeparam name="T">Le type</typeparam>
        /// <returns>
        ///   <c>true</c> si l'utilisateur actuellement connecté peut écrire sur le type spécifié; sinon, <c>false</c>.
        /// </returns>
        bool CanUserWrite<T>();
        /// <summary>
        /// Détermine si le role spécifié peut écrire sur le type spécifié.
        /// </summary>
        /// <typeparam name="T">Le type</typeparam>
        /// <param name="role">Le role.</param>
        /// <returns>
        ///   <c>true</c> si le role spécifié peut écrire sur le type spécifié; sinon, <c>false</c>.
        /// </returns>
        bool CanRoleWrite<T>(string role);

        /// <summary>
        /// Détermine si le l'utilisateur a accès au rôle spécifié.
        /// </summary>
        /// <param name="role">le rôle.</param>
        /// <returns>
        ///   <c>true</c> si le l'utilisateur a accès au rôle spécifié; sinon, <c>false</c>.
        /// </returns>
        bool HasCurrentUserRole(string role);

        /// <summary>
        /// Détermine si la licence chargée possède la fonctionnalité spécifiée.
        /// </summary>
        /// <param name="feature">La fonctionnalité.</param>
        /// <returns>
        ///   <c>true</c> si la licence chargée possède la fonctionnalité spécifiée.; sinon, <c>false</c>.
        /// </returns>
        bool HasCurrentLicenseFeature(Activation.ActivationFeatures feature);

        /// <summary>
        /// Enregistre les autorisations.
        /// </summary>
        void RegisterAuthorizations(
            IDictionary<Type, string[]> rolesReadAuthorizations,
            IDictionary<Type, string[]> rolesWriteAuthorizations,
            IDictionary<Type, short[]> featuresReadAuthorizations,
            IDictionary<Type, short[]> featuresWriteAuthorizations,
            IDictionary<Type, Func<string, bool>> customReadAuthorizations,
            IDictionary<Type, Func<string, bool>> customWriteAuthorizations);

        /// <summary>
        /// Obtient une valeur indiquant si l'utilisateur est l'admin par défaut
        /// </summary>
        /// <param name="user">L'utilisateur</param>
        /// <returns><c>true</c> si l'utilisateur est l'admin par défaut</returns>
        bool IsUserDefaultAdmin(User user);

    }
}