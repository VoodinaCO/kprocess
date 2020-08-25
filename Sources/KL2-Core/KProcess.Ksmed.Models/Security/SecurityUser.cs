using KProcess.Globalization;
using System;
using System.Linq;

namespace KProcess.Ksmed.Models.Security
{
    /// <summary>
    /// Représente un utilisateur connecté à l'application.
    /// </summary>
    public class SecurityUser
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="SecurityUser"/>.
        /// </summary>
        /// <param name="userModel">le modèle métier associé.</param>
        public SecurityUser(User userModel)
        {
            User = userModel;
        }

        public User User { get; private set; }

        /// <summary>
        /// Met à jour les noms.
        /// </summary>
        /// <param name="userModel">L'utilisateur.</param>
        public void UpdateNames(User userModel)
        {
            if (userModel.Username != Username)
                throw new ArgumentException("L'utilisateur ne correspond pas à celui actuellement connecté", nameof(userModel));

            User = userModel;
        }

        /// <summary>
        /// Obtient le login de l'utilisateur.
        /// </summary>
        public string Username =>
            User.Username;

        /// <summary>
        /// Obtient le prénom.
        /// </summary>
        public string Firstname =>
            User.Firstname;

        /// <summary>
        /// Obtient le nom.
        /// </summary>
        public string Lastname =>
            User.Name;

        /// <summary>
        /// Détermine si le l'utilisateur a accès au rôle spécifié.
        /// </summary>
        /// <param name="role">le rôle.</param>
        /// <returns>
        ///   <c>true</c> si le l'utilisateur a accès au rôle spécifié; sinon, <c>false</c>.
        /// </returns>
        public bool HasRole(string role)
        {
            bool result = false;
            if (User.RoleCodes?.Any() == true)
                result = User.RoleCodes.Any(_ => _ == role);
            if (!result && User.Roles?.Any() == true)
                result = User.Roles.Any(_ => _.RoleCode == role);
            return result;
        }

        /// <summary>
        /// Obtient le nom complet de l'utilisateur
        /// </summary>
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(Firstname) && string.IsNullOrEmpty(Lastname))
                    return Username;

                return $"{Firstname} {Lastname}".Trim();
            }
        }

    }
}
