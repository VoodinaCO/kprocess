using KProcess.Business;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Definit le comprotement d'un service de gestion des ressources de l'application.
    /// </summary>
    public interface IAppResourceService : IBusinessService
    {
        /// <summary>
        /// Obtient les langues disponibles pour l'application.
        /// </summary>
        Task<Language[]> GetLanguages();

        /// <summary>
        /// Retrieve the language as sync (used by web)
        /// </summary>
        /// <returns></returns>
        Language[] GetLanguagesAsSync();

        /// <summary>
        /// Obtient toutes les ressources.
        /// </summary>
        /// <returns>Les ressources.</returns>
        List<KeyValuePair<string, AppResourceValue[]>> GetAllResources();

        /// <summary>
        /// Obtient toutes les ressources de la langue spécifiée.
        /// </summary>
        /// <param name="languageCode">Le code de la langue.</param>
        /// <returns>Les ressources.</returns>
        AppResourceValue[] GetResources(string languageCode);

        /// <summary>
        /// Obtient la ressource de la clé spécifiée dans la langue spécifiée.
        /// </summary>
        /// <param name="languageCode">Le code de la langue.</param>
        /// <param name="key">la clé.</param>
        /// <returns>
        /// Les ressources.
        /// </returns>
        Task<AppResourceValue> GetResource(string languageCode, string key);
    }
}