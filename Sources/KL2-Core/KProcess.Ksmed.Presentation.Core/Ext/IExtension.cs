using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente une extension à l'application.
    /// </summary>
    [InheritedExport]
    public interface IExtension
    {

        /// <summary>
        /// Obtient le Guid unique de l'extension.
        /// </summary>
        Guid ExtensionId { get; }

        /// <summary>
        /// Obtient le libellé de l'extension.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Obtient une valeur indiquant si l'extension est configurable.
        /// </summary>
        bool HasConfiguration { get; }

        /// <summary>
        /// Obtient la version minimale de l'application requise au chargement.
        /// </summary>
        Version MinimumApplicationVersion { get; }

        /// <summary>
        /// Obtient le couple VM/View de configuration.
        /// </summary>
        /// <param name="extensionConfigurationView">La vue.</param>
        /// <returns>Le ViewModel.</returns>
        IExtensionConfigurationViewModel GetExtensionConfiguration(out IView extensionConfigurationView);

    }
}
