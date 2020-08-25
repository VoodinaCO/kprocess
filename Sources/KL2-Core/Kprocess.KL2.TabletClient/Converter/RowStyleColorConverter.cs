#region Imports
using Kprocess.KL2.TabletClient.Models;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
#endregion

namespace Kprocess.KL2.TabletClient.Converter
{
    /// <summary>
    /// 
    /// </summary>
    public class RowStyleColorConverter : MarkupExtension, IValueConverter
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
            if (!(value as UIPublishedAction).CanValidate)
            {
                return " #90808080 ";
            }
            else
            {
                return " Transparent ";
            }
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
