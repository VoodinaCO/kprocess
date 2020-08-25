using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Linq;

namespace KProcess.Ksmed.Presentation.Shell
{
    /// <summary>
    /// Fournit des méthodes de tests de droits d'accès.
    /// </summary>
    internal static class AccessRules
    {


        /// <summary>
        /// Détermine si l'utilisateur courant peut lire le view model spécifié.
        /// </summary>
        /// <typeparam name="TViewModel">Le type du view model.</typeparam>
        /// <param name="roles">Les roles des utilisateurs dans le projet.</param>
        /// <param name="isProjectContext"><c>true</c> si le view model est lié au projet.</param>
        /// <returns>
        ///   <c>true</c> si l'utilisateur courant peut lire le view model spécifié; sinon, <c>false</c>.
        /// </returns>
        public static bool CanUserRead<TViewModel>(IDictionary<string, string[]> roles, bool isProjectContext)
        {
            // Si l'utilisateur est un administrateur global et que le produit est activé avec toutes les fonctionnalités, il a tout les droits
            if (Security.SecurityContext.HasCurrentUserRole(Security.KnownRoles.Administrator)
                && Security.SecurityContext.HasCurrentLicenseFeature(Security.Activation.ActivationFeatures.All))
                return true;

            // Si l'accès n'est pas lié au projet
            if (!isProjectContext)
                return Security.SecurityContext.CanUserRead<TViewModel>();
            if (roles == null)
                return false;
            return roles.Any(kvp => kvp.Key == Security.SecurityContext.CurrentUser.Username
                                    && kvp.Value.Any(roleCode => Security.SecurityContext.CanRoleRead<TViewModel>(roleCode)));
        }


        /// <summary>
        /// Détermine si l'utilisateur courant peut écrire sur le view model spécifié.
        /// </summary>
        /// <typeparam name="TViewModel">Le type du view model.</typeparam>
        /// <param name="roles">Les roles des utilisateurs dans le projet.</param>
        /// <param name="isProjectContext"><c>true</c> si le view model est lié au projet.</param>
        /// <returns>
        ///   <c>true</c> si l'utilisateur courant peut écrire sur le view model spécifié; sinon, <c>false</c>.
        /// </returns>
        public static bool CanUserWrite<TViewModel>(IDictionary<string, string[]> roles, bool isProjectContext)
        {
            // Si l'utilisateur est un administrateur global, il a tout les droits
            if (Security.SecurityContext.HasCurrentUserRole(Security.KnownRoles.Administrator)
                && Security.SecurityContext.HasCurrentLicenseFeature(Security.Activation.ActivationFeatures.All))
                return true;

            // Si l'accès n'est pas lié au projet
            if (!isProjectContext)
                return Security.SecurityContext.CanUserWrite<TViewModel>();
            if (roles == null)
                return false;
            return roles.Any(kvp => kvp.Key == Security.SecurityContext.CurrentUser.Username
                                    && kvp.Value.Any(roleCode => Security.SecurityContext.CanRoleWrite<TViewModel>(roleCode)));
        }

        /// <summary>
        /// Détermine si l'utilisateur courant a accès au process spécifié via le rôle spécifié.
        /// </summary>
        /// <param name="process">Le process.</param>
        /// <param name="roleCode">le role.</param>
        /// <returns>
        ///   <c>true</c> si l'utilisateur courant a accès au process spécifié via le rôle spécifié; sinon, <c>false</c>.
        /// </returns>
        public static bool HasUserAccessOnProcess(Procedure process, string roleCode)
        {
            return process.UserRoleProcesses
                .Any(urp => urp.User.Username == Security.SecurityContext.CurrentUser.Username);
        }


    }
}
