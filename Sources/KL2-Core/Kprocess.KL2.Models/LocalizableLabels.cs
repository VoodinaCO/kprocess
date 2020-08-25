using KProcess.Globalization;
using KProcess.Ksmed.Models;
using System.Collections.Generic;

namespace Kprocess.KL2.Models
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

        public static void LoadShortLabel<TModel>(TModel model, ILocalizationManager localizationManager = null)
            where TModel : ILocalizedLabels
        {
            if (localizationManager != null)
                model.ShortLabel = localizationManager.GetString(model.ShortLabelResourceId);
        }

        public static void LoadLongLabel<TModel>(TModel model, ILocalizationManager localizationManager = null)
            where TModel : ILocalizedLabels
        {
            if (localizationManager != null)
                model.LongLabel = localizationManager.GetString(model.LongLabelResourceId);
        }

    }
}
