using KProcess.Business;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.API
{
    /// <summary>
    /// Représente un service de gestion des utilisateurs de l'application.
    /// </summary>
    public class ApplicationUsersService : IBusinessService, IApplicationUsersService
    {
        readonly IAPIHttpClient _apiHttpClient;

        public ApplicationUsersService(IAPIHttpClient apiHttpClient)
        {
            _apiHttpClient = apiHttpClient;
        }

        /// <summary>
        /// Obtient les utilisateurs, les rôles et les langues disponibles.
        /// </summary>
        public virtual async Task<(User[] Users, Role[] Roles, Language[] Languages, Team[] Teams)> GetUsersAndRolesAndLanguages() =>
            await Task.Run(async () =>
            {
                return await _apiHttpClient.ServiceAsync<(User[] Users, Role[] Roles, Language[] Languages, Team[] Teams)>(KL2_Server.API, nameof(ApplicationUsersService), nameof(GetUsersAndRolesAndLanguages));
            });

        public virtual async Task<(User[] Users, Team[] Teams)> GetUsersTeams(string team, string position, string role = null) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.team = team;
                param.position = position;
                param.roles = role;
                return await _apiHttpClient.ServiceAsync<(User[] Users, Team[] Teams)>(KL2_Server.API, nameof(ApplicationUsersService), nameof(GetUsersTeams), param);
            });

        /// <summary>
        /// Get all users ids
        /// </summary>
        public virtual async Task<int[]> GetAllUserIds() =>
            await Task.Run(async () =>
            {
                return await _apiHttpClient.ServiceAsync<int[]>(KL2_Server.API, nameof(ApplicationUsersService), nameof(GetAllUserIds));
            });

        /// <summary>
        /// Sauvegarde les utilisateurs spécifiés.
        /// </summary>
        /// <param name="users">Les utilisateurs.</param>
        public virtual async Task<IEnumerable<User>> SaveUsers(IEnumerable<User> users)
        {
            dynamic param = new ExpandoObject();
            param.users = users;
            return await _apiHttpClient.ServiceAsync<IEnumerable<User>>(KL2_Server.API, nameof(ApplicationUsersService), nameof(SaveUsers), param);
        }

        public virtual async Task<IEnumerable<Team>> SaveTeams(IEnumerable<Team> teams)
        {
            dynamic param = new ExpandoObject();
            param.users = teams;
            return await _apiHttpClient.ServiceAsync<IEnumerable<User>>(KL2_Server.API, nameof(ApplicationUsersService), nameof(SaveTeams), param);
        }

    }
}