using KProcess.Ksmed.Models;
using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class PercentConverter : MarkupExtension, IMultiValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string param = parameter as string;
            if (values != null && values.Length == 2 && values[0] is TrackableCollection<CustomLabel> labels && values[1] is bool videoMaximized)
            {
                if (videoMaximized)
                    return param == "Visible" ? 1187d : 0d;
                return param == "Visible" ? 712d : 475d;
            }
            return param == "Visible" ? 1187d : 0d;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
