using KProcess.Business;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.API
{
    /// <summary>
    /// Représente un service de gestion des ressources de l'application.
    /// </summary>
    public class AppResourceService : IBusinessService, IAppResourceService
    {
        readonly IAPIHttpClient _apiHttpClient;

        public AppResourceService(IAPIHttpClient apiHttpClient)
        {
            _apiHttpClient = apiHttpClient;
        }

        /// <summary>
        /// Obtient les langues disponibles pour l'application.
        /// </summary>
        public virtual async Task<Language[]> GetLanguages() =>
            await Task.Run(async () =>
            {
                return await _apiHttpClient.ServiceAsync<Language[]>(KL2_Server.API, nameof(AppResourceService), nameof(GetLanguages));
            });

        public Language[] GetLanguagesAsSync()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Obtient toutes les ressources.
        /// </summary>
        /// <returns>Les ressources.</returns>
        public virtual List<KeyValuePair<string, AppResourceValue[]>> GetAllResources()
        {
            return _apiHttpClient.Service<List<KeyValuePair<string, AppResourceValue[]>>>(KL2_Server.API, nameof(AppResourceService), nameof(GetAllResources));
        }

        /// <summary>
        /// Obtient toutes les ressources de la langue spécifiée.
        /// </summary>
        /// <param name="languageCode">Le code de la langue.</param>
        /// <returns>Les ressources.</returns>
        public virtual AppResourceValue[] GetResources(string languageCode)
        {
            dynamic param = new ExpandoObject();
            param.languageCode = languageCode;
            return _apiHttpClient.Service<AppResourceValue[]>(KL2_Server.API, nameof(AppResourceService), nameof(GetResources), param);
        }

        /// <summary>
        /// Obtient la ressource de la clé spécifiée dans la langue spécifiée.
        /// </summary>
        /// <param name="languageCode">Le code de la langue.</param>
        /// <param name="key">la clé.</param>
        /// <returns>
        /// Les ressources.
        /// </returns>
        public virtual async Task<AppResourceValue> GetResource(string languageCode, string key) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.languageCode = languageCode;
                param.key = key;
                return await _apiHttpClient.ServiceAsync<AppResourceValue>(KL2_Server.API, nameof(AppResourceService), nameof(GetResource), param);
            });
    }
}