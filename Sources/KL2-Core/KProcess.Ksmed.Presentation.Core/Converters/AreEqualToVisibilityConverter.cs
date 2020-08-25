using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    /// <summary>
    /// Convertit l'égalité entre la valeur et le paramètre en Visibility.
    /// Renvoie Visibility.Visible si les valeurs sont égales, Visibility.Collapsed sinon.
    /// </summary>
    public class AreEqualToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convertit l'égalité entre la valeur et le paramètre en Visibility.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>
        /// Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (DesignMode.IsInDesignMode)
                return Visibility.Visible;

            return Object.Equals(value, parameter) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Non supporté.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>
        /// Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
