using KProcess.KL2.Languages;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Linq;

namespace KProcess.Ksmed.Data
{
    /// <summary>
    /// Fournit des méthodes afin de localiser les libellés sur des modèles.
    /// </summary>
    public static class LocalizableLabels
    {

        /// <summary>
        /// Charge les libellés d'une collection de modèles.
        /// </summary>
        /// <typeparam name="TModel">Le type du modèle.</typeparam>
        /// <param name="models">Les modèles.</param>
        public static void LoadLabels<TModel>(IEnumerable<TModel> models, ILocalizationManager localizationManager = null)
            where TModel : ILocalizedLabels
        {
            foreach (TModel model in models)
                LoadLabel(model, localizationManager);
        }

        /// <summary>
        /// Charge les libellés d'un modèle
        /// </summary>
        /// <typeparam name="TModel">Le type du modèle.</typeparam>
        /// <param name="model">le modèle.</param>
        public static void LoadLabel<TModel>(TModel model, ILocalizationManager localizationManager = null)
            where TModel : ILocalizedLabels
        {
            LoadShortLabel(model, localizationManager);
            LoadLongLabel(model, localizationManager);
        }

        // TODO : Migrate to use ShortLabelResourceId as string key and not int key
        public static void LoadShortLabel<TModel>(TModel model, ILocalizationManager localizationManager = null)
            where TModel : ILocalizedLabels
        {
            if (localizationManager != null)
            {
                using (var context = ContextFactory.GetNewContext())
                {
                    var resource = context.AppResourceKeys.SingleOrDefault(_ => _.ResourceId == model.ShortLabelResourceId);
                    model.ShortLabel = localizationManager.GetString(resource.ResourceKey);
                }
            }
        }

        // TODO : Migrate to use ShortLabelResourceId as string key and not int key
        public static void LoadLongLabel<TModel>(TModel model, ILocalizationManager localizationManager = null)
            where TModel : ILocalizedLabels
        {
            if (localizationManager != null)
            {
                using (var context = ContextFactory.GetNewContext())
                {
                    var resource = context.AppResourceKeys.SingleOrDefault(_ => _.ResourceId == model.LongLabelResourceId);
                    model.LongLabel = localizationManager.GetString(resource.ResourceKey);
                }
            }
        }

    }
}
