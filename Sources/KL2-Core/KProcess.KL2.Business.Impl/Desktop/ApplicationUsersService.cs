using KProcess.Business;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Desktop
{
    /// <summary>
    /// Représente un service de gestion des utilisateurs de l'application.
    /// </summary>
    public class ApplicationUsersService : IBusinessService, IApplicationUsersService
    {
        private readonly ITraceManager _traceManager;
        private readonly ISecurityContext _securityContext;
        private readonly ILocalizationManager _localizationManager;

        public ApplicationUsersService(ISecurityContext securityContext, ILocalizationManager localizationManager, ITraceManager traceManager)
        {
            _securityContext = securityContext;
            _localizationManager = localizationManager;
            _traceManager = traceManager;
        }

        /// <summary>
        /// Obtient les utilisateurs, les rôles et les langues disponibles.
        /// </summary>
        public virtual async Task<(User[] Users, Role[] Roles, Language[] Languages, Team[] Teams)> GetUsersAndRolesAndLanguages() =>
            await Task.Run(async () =>
            {
                var test = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "kl2suitewebadminconfig.exe");
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {                    
                    User[] users = await context.Users
                        .Include(nameof(User.Roles))
                        .Include(nameof(User.DefaultLanguage))
                        .Include(nameof(User.Teams))
                        .Include(nameof(User.Audits))
                        .Where(u => !u.IsDeleted)
                        .ToArrayAsync();
                    Role[] roles = await context.Roles.ToArrayAsync();

                    Language[] langues = await context.Languages.ToArrayAsync();

                    Team[] teams = await context.Teams.ToArrayAsync();
                    return (users, roles, langues, teams);
                }
            });

        /// <summary>
        /// Get all users ids
        /// </summary>
        public virtual async Task<int[]> GetAllUserIds() =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext())
                {
                    int[] users = await context.Users
                        .Where(u => !u.IsDeleted)
                        .Select(u => u.UserId)
                        .ToArrayAsync();
                    return users;
                }
            });

        public virtual async Task<(User[] Users, Team[] Teams)> GetUsersTeams(string team, string position, string role = null) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    bool boolPosition = position == "All" ? false : Convert.ToBoolean(position);
                    int teamId = team == "0" ? 0 : Convert.ToInt16(team);
                    User[] users = await context.Users
                        .Include("Roles")
                        .Include("Teams")
                        .Include("Qualifications")
                        .Include("Publication")
                        .Include("Skills")
                        .Where(u => !u.IsDeleted && 
                            (position == "All" ? true : u.Tenured == boolPosition) && (team == "0" ? true : u.Teams.Where(t => t.Id == teamId).Any())
                            && (role == null ? true : u.Roles.Where(r => r.RoleCode == role).Any()))
                        .ToArrayAsync();
                    Team[] teams = await context.Teams.ToArrayAsync();
                    return (users,teams);
                }
            });

        /// <summary>
        /// Sauvegarde les utilisateurs spécifiés.
        /// </summary>
        /// <param name="users">Les utilisateurs.</param>
        public virtual async Task<IEnumerable<User>> SaveUsers(IEnumerable<User> users)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                List<User> usersWithInvalidUserName = new List<User>();

                foreach (User user in users)
                {
                    if (user.IsMarkedAsModified || user.IsMarkedAsAdded)
                    {
                        // Vérifier que la vidéo peut être supprimée
                        using (var tempContext = ContextFactory.GetNewContext())
                        {
                            bool userNameAlreadyExists = await tempContext.Users
                                .Where(u => !u.IsDeleted && u.UserId != user.UserId && u.Username == user.Username)
                                .AnyAsync();

                            if (userNameAlreadyExists)
                                usersWithInvalidUserName.Add(user);
                        }
                    }

                    // Si l'utilisateur ne peut pas être supprimé physiquement, le marqué comme supprimé
                    if (user.IsMarkedAsDeleted)
                    {
                        bool userHasElement = false;
                        using (var tempContext = ContextFactory.GetNewContext())
                        {
                            userHasElement = await tempContext.Procedures.AnyAsync(e => e.OwnerId == user.UserId); // We should change the owner
                            if (!userHasElement)
                                userHasElement = await tempContext.Users.AnyAsync(e => e.CreatedByUserId == user.UserId
                                                                                       || e.ModifiedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Videos.AnyAsync(e => e.CreatedByUserId == user.UserId
                                                                                        || e.ModifiedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Projects.AnyAsync(e => e.CreatedByUserId == user.UserId
                                                                                          || e.ModifiedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Scenarios.AnyAsync(e => e.CreatedByUserId == user.UserId
                                                                                           || e.ModifiedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.KActions.AnyAsync(e => e.CreatedByUserId == user.UserId
                                                                                          || e.ModifiedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.AppResourceValues.AnyAsync(e => e.CreatedByUserId == user.UserId
                                                                                                   || e.ModifiedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Resources.AnyAsync(e => e.CreatedByUserId == user.UserId
                                                                                           || e.ModifiedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Refs1.AnyAsync(e => e.CreatedByUserId == user.UserId
                                                                                       || e.ModifiedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Refs1.AnyAsync(e => e.CreatedByUserId == user.UserId
                                                                                       || e.ModifiedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Refs2.AnyAsync(e => e.CreatedByUserId == user.UserId
                                                                                       || e.ModifiedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Refs3.AnyAsync(e => e.CreatedByUserId == user.UserId
                                                                                       || e.ModifiedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Refs4.AnyAsync(e => e.CreatedByUserId == user.UserId
                                                                                       || e.ModifiedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Refs5.AnyAsync(e => e.CreatedByUserId == user.UserId
                                                                                       || e.ModifiedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Refs6.AnyAsync(e => e.CreatedByUserId == user.UserId
                                                                                       || e.ModifiedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Refs7.AnyAsync(e => e.CreatedByUserId == user.UserId
                                                                                       || e.ModifiedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Skills.AnyAsync(e => e.CreatedByUserId == user.UserId
                                                                                        || e.ModifiedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.PublicationHistories.AnyAsync(e => e.PublisherId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Publications.AnyAsync(e => e.PublishedByUserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Trainings.AnyAsync(e => e.UserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.ValidationTrainings.AnyAsync(e => e.TrainerId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Qualifications.AnyAsync(e => e.UserId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.QualificationSteps.AnyAsync(e => e.QualifierId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.InspectionSteps.AnyAsync(e => e.InspectorId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Anomalies.AnyAsync(e => e.InspectorId == user.UserId);
                            if (!userHasElement)
                                userHasElement = await tempContext.Audits.AnyAsync(e => e.AuditorId == user.UserId);
                        }
                        if (userHasElement)
                        {
                            user.CancelChanges();
                            user.IsDeleted = true;
                            user.MarkAsModified();
                        }
                    }

                    if (user.IsDeleted)
                    {
                        foreach (var urp in await context.UserRoleProcesses.Where(u => u.UserId == user.UserId).ToArrayAsync())
                        {
                            context.DeleteObject(urp);
                            urp.MarkAsDeleted();
                        }
                    }
                }

                if (usersWithInvalidUserName.Any())
                {
                    var ex = new BLLFuncException("Impossible d'utiliser un nom d'utilisateur existant.")
                    {
                        ErrorCode = KnownErrorCodes.CannotUseSameUserName
                    };
                    ex.Data.Add(KnownErrorCodes.CannotUseSameUserName_UsersKey, usersWithInvalidUserName.ToArray());
                    throw ex;
                }

                foreach (var user in users)
                    context.Users.ApplyChanges(user);

                await context.SaveChangesAsync();
                return users;
            }
        }
        public virtual async Task<IEnumerable<Team>> SaveTeams(IEnumerable<Team> teams)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                List<Team> teamsWithInvalidUserName = new List<Team>();
                foreach (Team team in teams)
                {
                    if (team.IsMarkedAsModified || team.IsMarkedAsAdded)
                    {
                        // Vérifier que la vidéo peut être supprimée
                        using (var tempContext = ContextFactory.GetNewContext())
                        {
                            bool teamNameAlreadyExists = await tempContext.Teams
                                .Where(u => u.Id != team.Id && u.Name == team.Name)
                                .AnyAsync();

                            if (teamNameAlreadyExists)
                                teamsWithInvalidUserName.Add(team);
                        }
                    }

                    if (team.IsMarkedAsDeleted)
                    {
                        //Apply delete to UserTeam
                        context.Teams.Attach(team);
                        context.Teams.DeleteObject(team);
                        
                    }
                }
                if (teamsWithInvalidUserName.Any())
                {
                    var ex = new BLLFuncException("Common_CannotUseSameName")
                    {
                        ErrorCode = KnownErrorCodes.CannotUseSameUserName
                    };
                    ex.Data.Add(KnownErrorCodes.CannotUseSameUserName_UsersKey, teamsWithInvalidUserName.ToArray());
                    throw ex;
                }

                foreach (var team in teams)
                    context.Teams.ApplyChanges(team);

                await context.SaveChangesAsync();
                return teams;
            }
        }
    }
}