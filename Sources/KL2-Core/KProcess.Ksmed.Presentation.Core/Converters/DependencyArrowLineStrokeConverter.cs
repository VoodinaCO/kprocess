using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    /// <summary>
    /// Effectue une division entre la valeur et la paramètre. La valeur est le dividende et le paramètre est le diviseur.
    /// </summary>
    public class DependencyArrowLineStrokeConverter : MarkupExtension, IMultiValueConverter
    {
        private static SolidColorBrush SameResource = Brushes.Blue;
        private static SolidColorBrush NotSameResource = Brushes.Red;

        /// <summary>
        /// Convertitune la valeur.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>
        /// Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? itemResourceId = (int?)values[0];
            int? dependentItemResourceId = (int?)values[1];
            return itemResourceId == dependentItemResourceId ? SameResource : NotSameResource;
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
