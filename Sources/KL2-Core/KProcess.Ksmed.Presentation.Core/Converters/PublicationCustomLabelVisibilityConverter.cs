using KProcess.Ksmed.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class PublicationCustomLabelVisibilityConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                Publication publication = (Publication)value;
                string refName = (string)parameter;
                bool result = false;
                switch (refName)
                {
                    case nameof(Project.CustomNumericLabel):
                        result = publication?.PublishedActions.Any(p => p.Action.CustomNumericValue != null) ?? false;
                        break;
                    case nameof(Project.CustomNumericLabel2):
                        result = publication?.PublishedActions.Any(p => p.Action.CustomNumericValue2 != null) ?? false;
                        break;
                    case nameof(Project.CustomNumericLabel3):
                        result = publication?.PublishedActions.Any(p => p.Action.CustomNumericValue3 != null) ?? false;
                        break;
                    case nameof(Project.CustomNumericLabel4):
                        result = publication?.PublishedActions.Any(p => p.Action.CustomNumericValue4 != null) ?? false;
                        break;
                    case nameof(Project.CustomTextLabel):
                        result = publication?.PublishedActions.Any(p => !string.IsNullOrEmpty(p.Action.CustomTextValue)) ?? false;
                        break;
                    case nameof(Project.CustomTextLabel2):
                        result = publication?.PublishedActions.Any(p => !string.IsNullOrEmpty(p.Action.CustomTextValue2)) ?? false;
                        break;
                    case nameof(Project.CustomTextLabel3):
                        result = publication?.PublishedActions.Any(p => !string.IsNullOrEmpty(p.Action.CustomTextValue3)) ?? false;
                        break;
                    case nameof(Project.CustomTextLabel4):
                        result = publication?.PublishedActions.Any(p => !string.IsNullOrEmpty(p.Action.CustomTextValue4)) ?? false;
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
