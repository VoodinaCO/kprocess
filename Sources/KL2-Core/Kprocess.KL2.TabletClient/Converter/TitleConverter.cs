using KProcess.Ksmed.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class TitleConverter : MarkupExtension, IValueConverter
    {
        static readonly string DefaultTitle = string.Empty;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is Publication publication)
                return publication.Process?.Label;
            return DefaultTitle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
