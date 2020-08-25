﻿using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Security
{
    /// <summary>
    /// Représente le context de sécurité de l'application.
    /// </summary>
    public class DesktopSecurityContext : ISecurityContext
    {
        private static IServiceBus _serviceBus;
        private static IEnumerable<IAuthenticationMode> _authenticationModes;
        private static bool _isInitialized;
        private static Dictionary<Type, string[]> _rolesReadAuthorizations;
        private static Dictionary<Type, string[]> _rolesWriteAuthorizations;
        private static Dictionary<Type, short[]> _featuresReadAuthorizations;
        private static Dictionary<Type, short[]> _featuresWriteAuthorizations;

        private static Dictionary<Type, Func<string, bool>> _customReadAuthorizations;
        private static Dictionary<Type, Func<string, bool>> _customWriteAuthorizations;

        /// <summary>
        /// Initialise le contexte.
        /// </summary>
        /// <param name="serviceBus">Le service bus de l'application.</param>
        /// <param name="authenticationModes">Les modes d'authentification disponibles.</param>
        public DesktopSecurityContext(
            IServiceBus serviceBus,
            IEnumerable<IAuthenticationMode> authenticationModes)
        {
            Assertion.NotNull(serviceBus, "serviceBus");
            Assertion.NotNull(authenticationModes, "authenticationModes");
            _serviceBus = serviceBus;
            _authenticationModes = authenticationModes;

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

            try
            {
                User user = await _serviceBus.Get<Business.IAuthenticationService>().GetUser(username);

                if (user != null)
                {
                    // TraceManager.TraceDebug("SecurityContext.TryLogonUser : utilisateur trouvé : {0}", user.Username);
                    // Récupérer le mode d'authentification
                    var authMode = GetAuthenticationMode();

                    // Authentifie l'utilisateur
                    if (await authMode.TryLogonUser(username, password ?? string.Empty, domain))
                    {
                        //   TraceManager.TraceDebug("SecurityContext.TryLogonUser : connexion réussie");
                        CurrentUser = new SecurityUser(user);
                        return (user, true);
                    }
                    return (null, false);
                }
                return (null, false);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Reconnecte l'utilisateur.
        /// </summary>
        /// <param name="user">L'utilisateur.</param>
        public bool ReconnectUser(User user)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// S'authentifie automatique avec le compte admin par défaut.
        /// </summary>
        public async Task AutoLogAsDefaultAdmin()
        {
            User user = await _serviceBus.Get<Business.IAuthenticationService>().GetUser(SecurityConstants.DefaultAdminUsername);
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
        /// Obtient le mode d'authentification en fonction de ce qui a été configuré.
        /// </summary>
        /// <returns></returns>
        private IAuthenticationMode GetAuthenticationMode()
        {
            var conf = ConfigurationManager.AppSettings["AuthenticationMode"];
            if (conf != null)
            {
                var authMode = _authenticationModes.FirstOrDefault(a => string.Equals(a.Name, conf));
                if (authMode != null)
                    return authMode;
            }

            return _authenticationModes.First();
        }

        /// <summary>
        /// Obtient une valeur indiquant si le mode d'authentification courant nécessite un domaine pour fonctionner.
        /// </summary>
        /// <returns><c>true</c> si le mode d'authentification courant nécessite un domaine pour fonctionner.</returns>
        public bool CurrentAuthenticationModeNeedsDomain()
        {
            return GetAuthenticationMode().NeedsDomain;
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
            if (_rolesReadAuthorizations == null)
                throw new InvalidOperationException("Les autorisations n'ont pas encore été enregistrées.");

            if (CurrentUser == null)
                throw new InvalidOperationException("Il n'y a pas d'utilisateur actuellement connecté.");

            bool canRoleRead = false;
            foreach (var role in _rolesReadAuthorizations[typeof(T)])
            {
                if (CurrentUser.HasRole(role))
                {
                    canRoleRead = true;
                    break;
                }
            }

            if (canRoleRead)
            {
                canRoleRead = false;
                if (CurrentProductLicense == null)
                    canRoleRead = true;
                else
                    canRoleRead |= _featuresReadAuthorizations[typeof(T)].Contains(CurrentProductLicense.ProductFeatures);
            }

            if (canRoleRead)
            {
                if (_customReadAuthorizations[typeof(T)] != null)
                    return _customReadAuthorizations[typeof(T)](null);
                return true;
            }

            return false;
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
            if (_rolesReadAuthorizations == null)
                throw new InvalidOperationException("Les autorisations n'ont pas encore été enregistrées.");

            var canRoleRead = _rolesReadAuthorizations[typeof(T)].Contains(role);

            if (canRoleRead)
            {
                canRoleRead = false;
                if (CurrentProductLicense == null)
                    canRoleRead = true;
                else
                    canRoleRead |= _featuresReadAuthorizations[typeof(T)].Contains(CurrentProductLicense.ProductFeatures);
            }

            if (canRoleRead)
            {
                if (_customReadAuthorizations[typeof(T)] != null)
                    return _customReadAuthorizations[typeof(T)](role);
                return true;
            }

            return false;
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
            if (_rolesWriteAuthorizations == null)
                throw new InvalidOperationException("Les autorisations n'ont pas encore été enregistrées.");

            if (CurrentUser == null)
                throw new InvalidOperationException("Il n'y a pas d'utilisateur actuellement connecté.");

            bool canRoleWrite = false;
            foreach (var role in _rolesWriteAuthorizations[typeof(T)])
            {
                if (CurrentUser.HasRole(role))
                {
                    canRoleWrite = true;
                    break;
                }
            }

            if (canRoleWrite)
            {
                canRoleWrite = false;
                if (CurrentProductLicense == null)
                    canRoleWrite = true;
                else
                    canRoleWrite |= _featuresWriteAuthorizations[typeof(T)].Contains(CurrentProductLicense.ProductFeatures);
            }

            if (canRoleWrite)
            {
                if (_customWriteAuthorizations[typeof(T)] != null)
                    return _customWriteAuthorizations[typeof(T)](null);
                return true;
            }

            return false;
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
            if (_rolesWriteAuthorizations == null)
                throw new InvalidOperationException("Les autorisations n'ont pas encore été enregistrées.");

            var canRoleWrite = _rolesWriteAuthorizations[typeof(T)].Contains(role);

            if (canRoleWrite)
            {
                canRoleWrite = false;
                if (CurrentProductLicense == null)
                    canRoleWrite = true;
                else
                    canRoleWrite |= _featuresWriteAuthorizations[typeof(T)].Contains(CurrentProductLicense.ProductFeatures);
            }

            if (canRoleWrite)
            {
                if (_customWriteAuthorizations[typeof(T)] != null)
                    return _customWriteAuthorizations[typeof(T)](role);
                return true;
            }

            return false;
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
            if (_rolesWriteAuthorizations != null ||
                _rolesReadAuthorizations != null ||
                _featuresReadAuthorizations != null ||
                _featuresWriteAuthorizations != null)
                throw new InvalidOperationException("Les authorisations ne peuvent être enregistrées qu'une fois");

            _rolesReadAuthorizations = rolesReadAuthorizations.ToDictionary(a => a.Key, a => a.Value);
            _rolesWriteAuthorizations = rolesWriteAuthorizations.ToDictionary(a => a.Key, a => a.Value);
            _featuresReadAuthorizations = featuresReadAuthorizations.ToDictionary(a => a.Key, a => a.Value);
            _featuresWriteAuthorizations = featuresWriteAuthorizations.ToDictionary(a => a.Key, a => a.Value);
            _customReadAuthorizations = customReadAuthorizations.ToDictionary(a => a.Key, a => a.Value);
            _customWriteAuthorizations = customWriteAuthorizations.ToDictionary(a => a.Key, a => a.Value);
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

    }
}