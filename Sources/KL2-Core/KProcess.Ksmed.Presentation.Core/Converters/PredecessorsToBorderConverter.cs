using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class PredecessorsToBorderBrushConverter : MarkupExtension, IValueConverter
    {
        private static Brush SamePredecessors = Brushes.Transparent;
        private static Brush NotSamePredecessors = new SolidColorBrush(Color.FromArgb(68,0,0,0));

        private AllPredecessorsConverter converter = new AllPredecessorsConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (bool)converter.Convert(value, targetType, parameter, culture) ? SamePredecessors : NotSamePredecessors;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class PredecessorsToBorderThicknessConverter : MarkupExtension, IValueConverter
    {
        private static double SamePredecessors = 0;
        private static double NotSamePredecessors = 1;

        private AllPredecessorsConverter converter = new AllPredecessorsConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (bool)converter.Convert(value, targetType, parameter, culture) ? SamePredecessors : NotSamePredecessors;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
