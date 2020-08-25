using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit le comportement d'un service gérant les préférences utiliasteur.
    /// </summary>
    public interface ISettingsService : IPresentationService
    {
        /// <summary>
        /// Obtient ou définit une valeur indiquant si le son est assourdi.
        /// </summary>
        bool Mute { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'envoi de rapport est autorisé.
        /// </summary>
        bool SendReport { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant la dernière langue utilisée.
        /// </summary>
        string LastCulture { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant le dernier identifiant utilisateur utilisé.
        /// </summary>
        string LastUserName { get; set; }

        /// <summary>
        /// Obtient ou définit les extensions désactivées.
        /// </summary>
        List<Guid> DisabledExtensions { get; set; }

        /// <summary>
        /// Met à jour
        /// </summary>
        void Upgrade();

        /// <summary>
        /// Sauvegarde
        /// </summary>
        void Save();

        /// <summary>
        /// Recharge les données.
        /// </summary>
        void Reload();

        /// <summary>
        /// Charge des préférences au niveau de l'application pour une extension en particulier.
        /// </summary>
        /// <typeparam name="T">Le type de l'objet à stocker</typeparam>
        /// <param name="extensionId">L'identifiant unique de l'extension.</param>
        T LoadExtensionApplicationSettings<T>(Guid extensionId);

        /// <summary>
        /// Sauvegarde les préférences au niveau de l'applicaiton pour une extension en particulier.
        /// </summary>
        /// <typeparam name="T">Le type de l'objet à stocker</typeparam>
        /// <param name="extensionId">L'identifiant unique de l'extension.</param>
        /// <param name="settings">Les préférences à sauvegarder.</param>
        void SaveExtensionApplicationSettings<T>(Guid extensionId, T settings);
    }
}
