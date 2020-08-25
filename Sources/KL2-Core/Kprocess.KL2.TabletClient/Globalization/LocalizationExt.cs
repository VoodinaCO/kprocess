using KProcess.KL2.Languages;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.TabletClient.Globalization
{
    [MarkupExtensionReturnType(typeof(object))]
    public class Localization : MarkupExtension
    {
        [ConstructorArgument("key")]
        public string Key { get; set; }

        public Localization() { }
        public Localization(string key)
        {
            Key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget pvt))
                return null;

            if (!(pvt.TargetObject is DependencyObject targetObject))
                return null;

            if (!(pvt.TargetProperty is DependencyProperty targetProperty))
                return null;

            var binding = new MultiBinding
            {
                Converter = new LocalizationConverter(),
                ConverterCulture = Locator.LocalizationManager.CurrentCulture,
                ConverterParameter = Key
            };
            binding.Bindings.Add(new Binding { Path = new PropertyPath("(0)", typeof(ILocalizationManager).GetProperty(nameof(ILocalizationManager.CurrentCulture))) });

            var expression = BindingOperations.SetBinding(targetObject, targetProperty, binding);

            return binding.ProvideValue(serviceProvider);
        }
    }

    public class LocalizationConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var temp = values.ToList();
            temp.RemoveAt(0);
            return Locator.LocalizationManager.GetStringFormat(parameter.ToString(), temp);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
