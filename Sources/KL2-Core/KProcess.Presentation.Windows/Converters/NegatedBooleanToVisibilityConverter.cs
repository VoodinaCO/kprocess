using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Presentation.Windows.Converters
{
    /// <summary>
    /// Converter qui renvoit une visibilité en fonction de l'inverse d'un booléen.
    /// </summary>
    public class NegatedBooleanToVisibilityConverter : MarkupExtension, IValueConverter
    {
        private BooleanToVisibilityConverter _b;
        private NegatedBooleanConverter _n;

        /// <summary>
        /// Constructeur.
        /// </summary>
        public NegatedBooleanToVisibilityConverter()
        {
            _b = new BooleanToVisibilityConverter();
            _n = new NegatedBooleanConverter();
        }

        /// <summary>
        /// Convertit l'inverse d'un booléen en Visibility.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (DesignMode.IsInDesignMode)
                return Visibility.Visible;
            return _b.Convert(
                _n.Convert(value, targetType, parameter, culture),
                targetType, parameter, culture);
        }

        /// <summary>
        /// Convertit une Visibility en booléen inversé.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return _n.ConvertBack(
                _b.ConvertBack(value, targetType, parameter, culture),
                targetType, parameter, culture);
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class NegatedBooleansToVisibilityConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (DesignMode.IsInDesignMode)
                return Visibility.Visible;

            try { return (new NegatedBooleanToVisibilityConverter()).Convert(values.Cast<bool>().All(_ => _), targetType, parameter, culture); }
            catch { return Visibility.Collapsed; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class NegatedOrBooleansToVisibilityConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (DesignMode.IsInDesignMode)
                return Visibility.Visible;

            foreach (object value in values)
            {
                if (!(value is bool))
                    return Visibility.Collapsed;
            }
            try { return (new NegatedBooleanToVisibilityConverter()).Convert(values.Cast<bool>().Any(_ => _), targetType, parameter, culture); }
            catch { return Visibility.Collapsed; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
