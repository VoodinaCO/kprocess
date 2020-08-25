using KProcess.Ksmed.Models;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    /// <summary>
    /// Visibilité du bouton selon la vue référentiel
    /// </summary>
    public class CurrentReferentielViewToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ProcessReferentialIdentifier refId && refId == ProcessReferentialIdentifier.Skill)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) =>
            throw new NotImplementedException();

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
