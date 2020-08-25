using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.TabletClient.Converter
{
    /// <summary>
    /// Convertit la non nullité de la valeur en <see cref="Visibility"/>.
    /// Si la valeur n'est pas nulle, renvoie Visible; sinon, renvoie Collapsed.
    /// </summary>
    /// <remarks>Renvoie toujours visible en mode design.</remarks>
    public class NotNullToVisibilityConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Convertit la non nullité de la valeur en <see cref="Visibility"/>.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns> 
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility notVisible = Visibility.Collapsed;

            string param = parameter as string;
            if (string.Compare(param, "Hidden", true) == 0)
                notVisible = Visibility.Hidden;

            return value != null ? Visibility.Visible : notVisible;
        }

        /// <summary>
        /// Non supporté
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
