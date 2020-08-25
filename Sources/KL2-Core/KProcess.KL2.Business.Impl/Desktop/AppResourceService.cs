using KProcess.Business;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Desktop
{
    /// <summary>
    /// Représente un service de gestion des ressources de l'application.
    /// </summary>
    public class AppResourceService : IBusinessService, IAppResourceService
    {

        /// <summary>
        /// Obtient les langues disponibles pour l'application.
        /// </summary>
        public virtual async Task<Language[]> GetLanguages() =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext())
                {
                    return await context.Languages.ToArrayAsync();
                }
            });


        /// <summary>
        /// Obtient les langues disponibles pour l'application.
        /// </summary>
        public virtual Language[] GetLanguagesAsSync()
        {
            using (var context = ContextFactory.GetNewContext())
            {
                return context.Languages.ToArray();
            }
        }

        /// <summary>
        /// Obtient toutes les ressources
        /// </summary>
        /// <returns>Les ressources.</returns>
        public virtual List<KeyValuePair<string, AppResourceValue[]>> GetAllResources()
        {
            using (var context = ContextFactory.GetNewContext())
            {
                var appResourceValues = context.AppResourceValues.Include("AppResourceKey").ToArray();
                return appResourceValues.GroupBy(r => r.LanguageCode).ToDictionary(x => x.Key, y => y.ToArray()).ToList();
            }
        }

        /// <summary>
        /// Obtient toutes les ressources de la langue spécifiée.
        /// </summary>
        /// <param name="languageCode">Le code de la langue.</param>
        /// <returns>Les ressources.</returns>
        public virtual AppResourceValue[] GetResources(string languageCode)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                return context.AppResourceValues.Include("AppResourceKey").Where(r => r.LanguageCode == languageCode).ToArray();
            }
        }

        /// <summary>
        /// Obtient la ressource de la clé spécifiée dans la langue spécifiée.
        /// </summary>
        /// <param name="languageCode">Le code de la langue.</param>
        /// <param name="key">la clé.</param>
        /// <returns>
        /// Les ressources.
        /// </returns>
        public virtual async Task<AppResourceValue> GetResource(string languageCode, string key)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                return await context.AppResourceValues.Include("AppResourceKey").Where(r => r.LanguageCode == languageCode && r.AppResourceKey.ResourceKey == key).FirstOrDefaultAsync();
            }
        }
    }
}