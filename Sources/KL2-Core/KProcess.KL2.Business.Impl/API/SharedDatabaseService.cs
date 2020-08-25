using KProcess.Business;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Business;
using System.Dynamic;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.API
{
    /// <summary>
    /// Service de gestion de verrous en base de données partagée.
    /// </summary>
    public class SharedDatabaseService : IBusinessService, ISharedDatabaseService
    {
        readonly IAPIHttpClient _apiHttpClient;

        public SharedDatabaseService(IAPIHttpClient apiHttpClient)
        {
            _apiHttpClient = apiHttpClient;
        }

        /// <summary>
        /// Détermine si la base de données est vérouillée pour l'utilisateur spécifié.
        /// </summary>
        /// <param name="username">le nom d'utilisateur</param>
        public async Task<bool> IsLocked(string username) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.username = username;
                return await _apiHttpClient.ServiceAsync<bool>(KL2_Server.API, nameof(SharedDatabaseService), nameof(IsLocked), param);
            });

        /// <summary>
        /// Met à jour le verrou pour l'utilisateur spécifié.
        /// </summary>
        /// <param name="username">le nom d'utilisateur</param>
        public async Task UpdateLock(string username) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.username = username;
                return await _apiHttpClient.ServiceAsync<bool>(KL2_Server.API, nameof(SharedDatabaseService), nameof(UpdateLock), param);
            });

        /// <summary>
        /// Libère le verrou pour l'utilisateur spécifié.
        /// </summary>
        /// <param name="username">le nom d'utilisateur</param>
        public Task ReleaseLock(string username)
        {
            dynamic param = new ExpandoObject();
            param.username = username;
            return _apiHttpClient.ServiceAsync(KL2_Server.API, nameof(SharedDatabaseService), nameof(ReleaseLock), param);
        }
    }
}