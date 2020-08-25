using Kprocess.KL2.TabletClient.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class UserIsQualifiedColorConverter : MarkupExtension, IValueConverter
    {
        static readonly SolidColorBrush IsQualifiedBrush = Brushes.Green;
        static readonly SolidColorBrush IsNotQualifiedBrush = Brushes.Orange;
        static readonly SolidColorBrush DefaultBrush = Brushes.Transparent;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UIUser uiUser)
            {
                if (uiUser.FinishQualification && uiUser.IsQualified)
                    return IsQualifiedBrush;
                if (uiUser.FinishQualification && !uiUser.IsQualified)
                    return IsNotQualifiedBrush;
                if (uiUser.LastQualificationStatus == UIUser.QualificationStatus.WaitingValidation && !uiUser.IsQualified)
                    return IsNotQualifiedBrush;
            }
            return DefaultBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
