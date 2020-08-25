// -----------------------------------------------------------------------
// <copyright file="NegatedBooleanConverter.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Presentation.Windows.Converters
{
    /// <summary>
    /// Converter qui renvoie l'inverse du booleén récupéré
    /// </summary>
    public class NegatedBooleanConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Convertit un booléen en son inverse.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns>       
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(System.Convert.ToBoolean(value));
        }

        /// <summary>
        /// Convertit un booléen en son inverse.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(System.Convert.ToBoolean(value));
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
