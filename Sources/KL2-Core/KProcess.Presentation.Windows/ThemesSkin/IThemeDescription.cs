
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows;

namespace KProcess.Presentation.Windows
{

    /// <summary>
    /// Représente la description d'un thème de l'application;
    /// </summary>
    [InheritedExport]
    public interface IThemeDescription
    {

        /// <summary>
        /// Obtient l'id du thème.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Obtient une valeur indiquant si le thème est celui par défaut.
        /// </summary>
        bool IsDefault { get; }

        /// <summary>
        /// Obtient le nom du thème.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Obtient les dictionnaires de ressources spécifique à ce thème.
        /// </summary>
        /// <returns>Les dictionnaires de ressources spécifique à ce thème.</returns>
        IEnumerable<ResourceDictionary> GetResourceDictionaries();

    }

}
