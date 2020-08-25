using KProcess.Ksmed.Models;
using System;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class PublicationRefsLocalizationConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is TrackableCollection<PublicationLocalization> localizations && parameter is ProcessReferentialIdentifier refID)
                return localizations.SingleOrDefault(_ => _.ResourceKey == refID.ToString())?.Value;
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class LinkedPublicationRefsLocalizationConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is TrackableCollection<PublishedAction> pActions
                && pActions.FirstOrDefault()?.Publication?.Localizations is TrackableCollection<PublicationLocalization> localizations
                && parameter is ProcessReferentialIdentifier refID)
                return localizations.SingleOrDefault(_ => _.ResourceKey == refID.ToString())?.Value;
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
