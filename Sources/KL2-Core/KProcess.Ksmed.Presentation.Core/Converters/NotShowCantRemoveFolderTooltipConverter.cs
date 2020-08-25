using KProcess.Ksmed.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class NotShowCantRemoveFolderTooltipConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProjectDir folder)
            {
                if (folder.Id == -1)
                    return true;
                return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
