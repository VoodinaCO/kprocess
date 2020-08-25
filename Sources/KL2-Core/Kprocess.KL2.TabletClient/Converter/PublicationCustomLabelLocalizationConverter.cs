using KProcess.Ksmed.Models;
using System;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class PublicationCustomLabelLocalizationConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                TrackableCollection<PublicationLocalization> localizations = (TrackableCollection<PublicationLocalization>)value;
                string refName = (string)parameter;
                var result = localizations.SingleOrDefault(_ => _.ResourceKey == refName)?.Value;
                return result ?? refName;
            }
            catch
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class LinkedPublicationCustomLabelLocalizationConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                TrackableCollection<PublicationLocalization> localizations = ((TrackableCollection<PublishedAction>)value)?.FirstOrDefault()?.Publication?.Localizations;
                string refName = (string)parameter;
                var result = localizations.SingleOrDefault(_ => _.ResourceKey == refName)?.Value;
                return result ?? refName;
            }
            catch
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
