#region Imports
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
#endregion

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    /// <summary>
    /// Convertit un boolléen en visibilité
    /// <c>true</c> Visible
    /// <c>false</c> Collapsed
    /// </summary>
    public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
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
            {
                return Visibility.Collapsed;
            }

            return ((bool)value) ? Visibility.Visible : Visibility.Hidden;
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
