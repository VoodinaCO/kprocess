using KProcess.Business;
using KProcess.KL2.APIClient;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Security;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Security.Business.API
{
    /// <summary>
    /// Représente un service d'authentification
    /// </summary>
    public class AuthenticationService : IBusinessService, IAuthenticationService
    {
        readonly IAPIHttpClient _apiHttpClient;
        readonly ILocalizationManager _localizationManager;

        public AuthenticationService(IAPIHttpClient apiHttpClient, ILocalizationManager localizationManager = null)
        {
            _apiHttpClient = apiHttpClient;
            _localizationManager = localizationManager;
        }

        /// <summary>
        /// Obtient l'utilisateur associé au login spécifié.
        /// </summary>
        /// <param name="username">Le login de l'utilisateur.</param>
        public virtual async Task<User> GetUser(string username, string language = null) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.username = username;
                param.language = _localizationManager.CurrentCulture.Name ?? IoC.Resolve<ILocalizationManager>().CurrentCulture.Name;
                return await _apiHttpClient.ServiceAsync<User>(KL2_Server.API, nameof(AuthenticationService), nameof(GetUser), param);
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
        public virtual Task<bool> IsUserValid(string username, byte[] password)
        {
            dynamic param = new ExpandoObject();
            param.username = username;
            param.password = password;
            return _apiHttpClient.ServiceAsync<bool>(KL2_Server.API, nameof(AuthenticationService), nameof(IsUserValid), param);
        }

        /// <summary>
        /// Sauvegarde l'utilisateur.
        /// </summary>
        /// <param name="user">L'utilisateur.</param>
        public virtual async Task SaveUser(User user, SecurityUser securityUser = null) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.user = user;
                await _apiHttpClient.ServiceAsync(KL2_Server.API, nameof(AuthenticationService), nameof(SaveUser), param);
            });
    }
}