using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;
using KProcess.Ksmed.Presentation.Core;

namespace KProcess.Ksmed.Presentation.Shell
{
    /// <summary>
    /// Définit le comportement d'un service de gestion des extensions
    /// </summary>
    internal interface IExtensionsManager : IExtensionsService, IPresentationService
    {
        /// <summary>
        /// Détermine si l'extension spécifiée est activée.
        /// </summary>
        /// <param name="extensionId">L'identifiant de l'extension.</param>
        /// <returns>
        ///   <c>true</c> si l'extension est activée; sinon, <c>false</c>.
        /// </returns>
        bool IsExtensionEnabled(Guid extensionId);

        /// <summary>
        /// Active ou désactive une extension.
        /// </summary>
        /// <param name="extensionId">L'identifiant de l'extension.</param>
        /// <param name="isEnabled"><c>true</c> si l'extension est à activer.</param>
        void SetExtensionEnabled(Guid extensionId, bool isEnabled);
            
        /// <summary>
        /// Sauvegarde l'état désactivé des extensions.
        /// </summary>
        void SaveEnabledStates();

        /// <summary>
        /// Définit la validité et l'état activé des extensions.
        /// </summary>
        /// <param name="extensions">Les extensions.</param>
        void SetExtensions(IEnumerable<ExtensionDescription> extensions);

        /// <summary>
        /// Obtient les extensions de l'application.
        /// </summary>
        /// <returns>Les extensions.</returns>
        IEnumerable<ExtensionDescription> GetExtensions();

    }

}
