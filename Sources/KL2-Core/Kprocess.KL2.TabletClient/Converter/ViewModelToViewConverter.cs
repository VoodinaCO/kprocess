using Kprocess.KL2.TabletClient.ViewModel;
using Kprocess.KL2.TabletClient.Views;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class ViewModelToViewConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (value.GetType() == typeof(ActivityChoiceViewModel))
                return new ActivityChoice { DataContext = value };

            /*if (value.GetType() == typeof(SummaryViewModel))
                return new Summary { DataContext = value };*/

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
