// -----------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using KProcess.Globalization;
using KProcess.Presentation.Windows.Localization;
using System;
using System.Collections.Generic;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Fournit des méthodes permettant d'utiliser le <see cref="LocalizationManager"/> en mode design.
    /// </summary>
    public static class LocalizationManagerExt
    {
        /// <summary>
        /// Retourne une resource en fonction de sa clé ou une chaîne vide si le fournisseur de ressources n'est pas configuré
        /// et que le contexte courant est en mode Design.
        /// </summary>
        /// <param name="key">clé de la resource</param>
        /// <param name="providerKey">clé du provider</param>
        /// <param name="defaultValue">valeur par défaut si la resource n'est pas trouvée</param>
        /// <returns>la valeur de la resource dans la culture courante</returns>
        public static object GetSafeDesignerValue(string key, string providerKey = null, string defaultValue = null)
        {
            if (DesignMode.IsInDesignMode)
                return GetShortKey(key);

            return LocalizationManager.GetString(key, providerKey, defaultValue);
        }

        /// <summary>
        /// Retourne une resource en fonction de sa clé ou une chaîne vide si le fournisseur de ressources n'est pas configuré
        /// et que le contexte courant est en mode Design.
        /// </summary>
        /// <param name="key">clé de la resource</param>
        /// <param name="providerKey">clé du provider</param>
        /// <param name="defaultValue">valeur par défaut si la resource n'est pas trouvée</param>
        /// <returns>la valeur de la resource dans la culture courante</returns>
        public static string GetSafeDesignerString(string key, string providerKey = null, string defaultValue = null)
        {
            if (DesignMode.IsInDesignMode)
                return GetShortKey(key);

            return LocalizationManager.GetString(key, providerKey, defaultValue);
        }

        /// <summary>
        /// Renvoie la clé allégée pour un affichage optimal en mode design.
        /// </summary>
        /// <param name="key">La clé de base.</param>
        /// <returns>La clé allégée.</returns>
        public static string GetShortKey(string key)
        {
            var index = key.LastIndexOf("_") + 1;
            if (index != 0 && key.Length >= index)
                return String.Format("*{0}*", key.Substring(index));

            return String.Format("*{0}*", key);
        }

        /// <summary>
        /// Holds the list of localization instances.
        /// </summary>
        /// <remarks>
        /// <see cref="WeakReference"/> cannot be used as a localization instance
        /// will be garbage collected on the next garbage collection
        /// as the localizaed object does not hold reference to it.
        /// </remarks>
        static List<LocalizeExtension> _localizations = new List<LocalizeExtension>();

        /// <summary>
        /// Holds the number of localizations added since the last purge of localizations.
        /// </summary>
        static int _localizationPurgeCount;

        /// <summary>
        /// Adds the specified localization instance to the list of manages localization instances.
        /// </summary>
        /// <param name="localization">The localization instance.</param>
        internal static void AddLocalization(LocalizeExtension localization)
        {
            if (localization == null)
                throw new ArgumentNullException("localization");

            if (_localizationPurgeCount > 50)
            {
                // It's really faster to fill a new list instead of removing elements
                // from the existing list when there are a lot of elements to remove.

                var localizatons = new List<LocalizeExtension>(_localizations.Count);

                foreach (var item in _localizations)
                {
                    if (item.IsAlive)
                        localizatons.Add(item);
                }

                _localizations = localizatons;
                _localizationPurgeCount = 0;
            }

            _localizations.Add(localization);
            _localizationPurgeCount++;
        }

        /// <summary>
        /// Updates the localizations.
        /// </summary>
        public static void UpdateLocalizations()
        {
            foreach (var item in _localizations)
            {
                item.UpdateTargetValue();
            }
        }
    }
}
