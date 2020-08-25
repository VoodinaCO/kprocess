using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit le comportement d'un service permettant de contrôler les extensions.
    /// </summary>
    public interface IExtensionsService : IPresentationService
    {

        /// <summary>
        /// Détermine si l'extension spécifiée est activée et si la version requise est valide.
        /// </summary>
        /// <param name="extensionId">L'identifiant de l'extension.</param>
        /// <returns>
        ///   <c>true</c> si l'extension est activée; sinon, <c>false</c>.
        /// </returns>
        bool IsExtensionEnabledAndVersionValid(Guid extensionId);

    }
}
