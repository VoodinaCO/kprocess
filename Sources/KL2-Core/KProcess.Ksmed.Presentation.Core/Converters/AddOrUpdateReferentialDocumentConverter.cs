using KProcess.Globalization;
using KProcess.Ksmed.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class AddOrUpdateReferentialDocumentConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IActionReferential refAction)
                return refAction.CloudFile == null ? LocalizationManager.GetString("View_AdminReferentials_AddDocument") : LocalizationManager.GetString("View_AdminReferentials_UpdateDocument");
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
