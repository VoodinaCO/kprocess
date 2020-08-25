using KProcess.Ksmed.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class PublicationRefsVisibilityConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                Publication publication = (Publication)value;
                ProcessReferentialIdentifier refID = (ProcessReferentialIdentifier)parameter;
                bool result = false;
                switch (refID)
                {
                    case ProcessReferentialIdentifier.Ref1:
                        result = publication?.PublishedActions.Any(p => p.Action.Ref1.Any()) ?? false;
                        break;
                    case ProcessReferentialIdentifier.Ref2:
                        result = publication?.PublishedActions.Any(p => p.Action.Ref2.Any()) ?? false;
                        break;
                    case ProcessReferentialIdentifier.Ref3:
                        result = publication?.PublishedActions.Any(p => p.Action.Ref3.Any()) ?? false;
                        break;
                    case ProcessReferentialIdentifier.Ref4:
                        result = publication?.PublishedActions.Any(p => p.Action.Ref4.Any()) ?? false;
                        break;
                    case ProcessReferentialIdentifier.Ref5:
                        result = publication?.PublishedActions.Any(p => p.Action.Ref5.Any()) ?? false;
                        break;
                    case ProcessReferentialIdentifier.Ref6:
                        result = publication?.PublishedActions.Any(p => p.Action.Ref6.Any()) ?? false;
                        break;
                    case ProcessReferentialIdentifier.Ref7:
                        result = publication?.PublishedActions.Any(p => p.Action.Ref7.Any()) ?? false;
                        break;
                }
                return result ? Visibility.Visible : Visibility.Collapsed;
            }
            catch
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
