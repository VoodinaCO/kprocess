using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Presentation.Windows.Converters
{
    /// <summary>
    /// Convertisseur qui multiple la valeur fournie (double) par le ratio fournit en paramètre (double).
    /// </summary>
    /// <remarks>La culture utilisée pour parser le paramètre est <see cref="CultureInfo.InvariantCulture"/></remarks>
    /// <example>
    /// Dans cet exemplate, le DataContext est un autre controle.
    /// <![CDATA[
    /// <Border Width="{Binding ActualWidth, Converter={StaticResource DoubleRatioConverter}, ConverterParameter=.5}" 
    ///         Height="{Binding ActualHeight, Converter={StaticResource DoubleRatioConverter}, ConverterParameter=.5}" />
    /// ]]>
    /// </example>
    public class DoubleRatioConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Convertit un la valeur et le ratio.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double val && Double.TryParse((string)parameter, NumberStyles.Any, CultureInfo.InvariantCulture, out double ratio))
                return val * ratio;
            return value;
        }

        /// <summary>
        /// N'est pas supporté.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
