using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace DlhSoft
{
    public static class LocalizationExt
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public static void RaiseStaticPropertyChanged([CallerMemberName] string propertyName = null) =>
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));

        public static string GetLocalizedValue(this CultureInfo culture, string key) =>
            Dictionary[culture.Name]?[key];

        private static CultureInfo _CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        public static CultureInfo CurrentCulture
        {
            get { return _CurrentCulture; }
            set
            {
                if (_CurrentCulture != value)
                {
                    _CurrentCulture = value;
                    RaiseStaticPropertyChanged();
                }
            }
        }

        public static Dictionary<string, Dictionary<string, string>> Dictionary = new Dictionary<string, Dictionary<string, string>>()
        {
            {
                CultureInfo.GetCultureInfo("en-US").Name, new Dictionary<string, string>()
                {
                    { "Core_Gantt_DeleteLink", "Delete the link" }
                }
            },
            {
                CultureInfo.GetCultureInfo("fr-FR").Name, new Dictionary<string, string>()
                {
                    { "Core_Gantt_DeleteLink", "Supprimer le lien" }
                }
            },
            {
                CultureInfo.GetCultureInfo("de-DE").Name, new Dictionary<string, string>()
                {
                    { "Core_Gantt_DeleteLink", "Link löschen" }
                }
            },
            {
                CultureInfo.GetCultureInfo("es-ES").Name, new Dictionary<string, string>()
                {
                    { "Core_Gantt_DeleteLink", "Quitar vínculo" }
                }
            },
            {
                CultureInfo.GetCultureInfo("pt-BR").Name, new Dictionary<string, string>()
                {
                    { "Core_Gantt_DeleteLink", "Excluir o link" }
                }
            },
            {
                CultureInfo.GetCultureInfo("pl-PL").Name, new Dictionary<string, string>()
                {
                    { "Core_Gantt_DeleteLink", "Usuń link" }
                }
            }
        };
    }

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
            var pvt = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (pvt == null)
                return null;

            var targetObject = pvt.TargetObject as DependencyObject;
            if (targetObject == null)
                return null;

            var targetProperty = pvt.TargetProperty as DependencyProperty;
            if (targetProperty == null)
                return null;

            var binding = new MultiBinding
            {
                Converter = new LocalizationConverter(),
                ConverterCulture = LocalizationExt.CurrentCulture,
                ConverterParameter = Key
            };
            binding.Bindings.Add(new Binding { Path = new PropertyPath("(0)", typeof(LocalizationExt).GetProperty("CurrentCulture")) });

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
            string result = LocalizationExt.Dictionary[LocalizationExt.CurrentCulture.Name][parameter.ToString()];
            try
            {
                return string.Format(result, temp.ToArray());
            }
            catch
            {
                return result;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
