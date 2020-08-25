using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Kprocess.PackIconKprocess.Converter
{
    public class KindToPathConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IDictionary<PackIconKprocessKind, string> dict = parameter as IDictionary<PackIconKprocessKind, string>;
            if (value is PackIconKprocessKind kind)
                return dict[kind];
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class CountryToImageSourceConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PackIconCountriesFlagsKind country)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith($"{country}.xaml"));

                DrawingGroup imageDrawings = null;
                using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
                {
                    imageDrawings = (DrawingGroup)XamlReader.Load(resourceStream);
                }
                DrawingImage drawingImageSource = new DrawingImage(imageDrawings);
                drawingImageSource.Freeze();
                return drawingImageSource;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
