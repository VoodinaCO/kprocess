﻿using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.TabletClient.Converter
{
    /// <summary>
    /// Convertit l'inverse d'un booléen en visibilité
    /// <c>true</c> Collapsed
    /// <c>false</c> Visible
    /// </summary>
    public class InvertBoolConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(!(value is bool))
                return true;

            return !(bool)value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
