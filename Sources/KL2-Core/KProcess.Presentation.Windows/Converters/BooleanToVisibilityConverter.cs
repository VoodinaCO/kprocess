// -----------------------------------------------------------------------
// <copyright file="BooleanToVisibilityConverter.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Presentation.Windows.Converters
{
    /// <summary>
    /// Converter qui renvoit une visibilité en fonction d'un booléen
    /// </summary>
    public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Convertit un booléen en Visibility.
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

            bool valueToTest = (bool)value;

            // Si la valeur vaut true, on renvoit Visible
            if (valueToTest)
                return Visibility.Visible;

            // Si elle est fausse, on renvoie soit Collapsed soit la valeur du paramètre
            Visibility visibilityWhenFalse = Visibility.Collapsed;
            string parameterValue = parameter as string;

            if (!String.IsNullOrEmpty(parameterValue))
                visibilityWhenFalse = (Visibility)Enum.Parse(typeof(Visibility), parameterValue, true);

            return visibilityWhenFalse;
        }

        /// <summary>
        /// Convertit une Visibility en booléen.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class BooleansToVisibilityConverter : MarkupExtension, IMultiValueConverter
    {
        static BooleanToVisibilityConverter _converter = new BooleanToVisibilityConverter();

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (DesignMode.IsInDesignMode)
                return Visibility.Visible;

            return _converter.Convert(values.All(_ => _ is bool boolValue && boolValue), targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
