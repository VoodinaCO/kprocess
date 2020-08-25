using KProcess.Ksmed.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class LinkedPublicationColorConverter : MarkupExtension, IValueConverter
    {
        static readonly SolidColorBrush LinkedPublicationBrush = new SolidColorBrush(Color.FromArgb(System.Convert.ToByte(0x44), 0, 0, 0));
        static readonly SolidColorBrush NoLinkedPublicationBrush = Brushes.Transparent;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PublishedAction input && input.LinkedPublicationId != null)
                return LinkedPublicationBrush;
            return NoLinkedPublicationBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
