using KProcess.Business;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Definit le comportement d'un service de gestion des utilisateurs de l'application.
    /// </summary>
    public interface IApplicationUsersService : IBusinessService
    {

        /// <summary>
        /// Obtient les utilisateurs, les rôles et les langues disponibles.
        /// </summary>
        Task<(User[] Users, Role[] Roles, Language[] Languages, Team[] Teams)> GetUsersAndRolesAndLanguages();

        /// <summary>
        /// Get all users ids
        /// </summary>
        Task<int[]> GetAllUserIds();

        Task<(User[] Users, Team[] Teams)> GetUsersTeams(string team, string position, string role = null);

        /// <summary>
        /// Sauvegarde les utilisateurs spécifiés.
        /// </summary>
        /// <param name="users">Les utilisateurs.</param>
        Task<IEnumerable<User>> SaveUsers(IEnumerable<User> users);
        Task<IEnumerable<Team>> SaveTeams(IEnumerable<Team> teams);


    }
}