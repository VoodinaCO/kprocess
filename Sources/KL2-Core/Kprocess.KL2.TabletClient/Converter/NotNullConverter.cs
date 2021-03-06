﻿using KProcess.Ksmed.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.TabletClient.Converter
{
    /// <summary>
    /// Renvoie True si non Null
    /// </summary>
    public class NotNullConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Renvoie True si non Null
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>True si Null.</returns> 
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(value == null);
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

    public class HaveAnyConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RefsCollection refsCollection && refsCollection.Values.Any())
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
