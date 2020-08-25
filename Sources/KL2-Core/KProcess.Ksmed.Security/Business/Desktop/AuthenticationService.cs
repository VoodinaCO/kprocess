using KProcess.Business;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Security;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Security.Business.Desktop
{
    /// <summary>
    /// Représente un service d'authentification
    /// </summary>
    public class AuthenticationService : IBusinessService, IAuthenticationService
    {
        private readonly ILocalizationManager _localizationManager;

        public AuthenticationService(ILocalizationManager localizationManager = null)
        {
            _localizationManager = localizationManager;
        }

        /// <summary>
        /// Obtient l'utilisateur associé au login spécifié.
        /// </summary>
        /// <param name="username">Le login de l'utilisateur.</param>
        public virtual async Task<User> GetUser(string username, string language = null) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_localizationManager))
                {
                    User user = await context.Users
                        .Include(nameof(User.DefaultLanguage))
                        .Include(nameof(User.Roles))
                        .Include(nameof(User.UserRoleProcesses))
                        .Include($"{nameof(User.UserRoleProcesses)}.{nameof(UserRoleProcess.Role)}")
                        .FirstOrDefaultAsync(u => !u.IsDeleted && u.Username == username);

                    return user;
                }
            });

        /// <summary>
        /// Obtient l'utilisateur associé au login spécifié.
        /// </summary>
        /// <param name="username">Le login de l'utilisateur.</param>
        public User GetUserAsSync(string username, string language = null)
        {
            using (var context = ContextFactory.GetNewContext(_localizationManager))
            {
                User user = context.Users
                    .Include(nameof(User.DefaultLanguage))
                    .Include(nameof(User.Roles))
                    .Include(nameof(User.UserRoleProcesses))
                    .Include($"{nameof(User.UserRoleProcesses)}.{nameof(UserRoleProcess.Role)}")
                    .FirstOrDefault(u => !u.IsDeleted && u.Username == username);

                return user;
            }
        }

        /// <summary>
        /// Indique si le couple login / mot de passe est valide.
        /// </summary>
        /// <param name="username">Le login de l'utilisateur.</param>
        /// <param name="password">Le mot de passe.</param>
        /// <returns><c>true</c> si le couple est valide.</returns>
        public virtual async Task<bool> IsUserValid(string username, byte[] password)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                return await context.Users.SingleOrDefaultAsync(u => !u.IsDeleted && u.Username == username && u.Password == password) != null;
            }
        }

        /// <summary>
        /// Sauvegarde l'utilisateur.
        /// </summary>
        /// <param name="user">L'utilisateur.</param>
        public virtual async Task SaveUser(User user, SecurityUser securityUser = null) =>
            await Task.Run(async () =>
            {
                if (securityUser == null) // Desktop call
                {
                    using (var context = ContextFactory.GetNewContext())
                    {
                        context.Users.ApplyChanges(user);
                        await context.SaveChangesAsync();
                    }
                }
                else // API Call
                {
                    using (var context = ContextFactory.GetNewContext(securityUser))
                    {
                        context.Users.ApplyChanges(user);
                        await context.SaveChangesAsync();
                    }
                }
            });

    }
}