using KProcess.Ksmed.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class GroupColorConverter : MarkupExtension, IValueConverter
    {
        static readonly SolidColorBrush GroupBrush = new SolidColorBrush(Color.FromArgb(System.Convert.ToByte(0x44), 0, 0, 0));
        static readonly SolidColorBrush NoGroupBrush = Brushes.Transparent;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PublishedAction input && parameter is SolidColorBrush oriBrush)
            {
                var brush = new SolidColorBrush(Color.FromArgb(System.Convert.ToByte(0x44), oriBrush.Color.R, oriBrush.Color.G, oriBrush.Color.B));
                if (input.IsGroup)
                    return GroupBrush;
            }
            return NoGroupBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
