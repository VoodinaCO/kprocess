using KProcess.Ksmed.Models;
using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Kprocess.KL2.TabletClient.Converter
{
    /// <summary>
    /// 
    /// </summary>
    public class QualificationRowStyleColorConverter : MarkupExtension, IValueConverter
    {
        static readonly SolidColorBrush FalseBrush = new SolidColorBrush(Color.FromArgb(0x90, 0x80, 0x80, 0x80));
        static readonly SolidColorBrush TrueBrush = Brushes.Transparent;

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
            if (value is PublishedAction publishedAction && !publishedAction.CanValidateQualificationStep)
                return FalseBrush;
            return TrueBrush;
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
