using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace KProcess.Presentation.Windows.Localization
{

    /// <summary>
    /// Définit le comportement d'un objet fournissant les informations nécessaires à la localisation et dont la cible est un <see cref="DependencyObject"/>.
    /// </summary>
    public interface IDependencyLocalizeExtension : ILocalizeExtension
    {
        /// <summary>
        /// Obtient ou définit le value converter à utiliser
        /// </summary>
        IValueConverter Converter { get; set; }

        /// <summary>
        /// Obtient le convertisseur de type.
        /// </summary>
        TypeConverter TypeConverter { get; }
    }
}
