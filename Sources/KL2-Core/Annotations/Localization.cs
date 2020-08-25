using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace AnnotationsLib
{
    public static class LocalizationExt
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        public static void RaiseStaticPropertyChanged([CallerMemberName] string propertyName = null) =>
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));

        public static string GetLocalizedValue(this CultureInfo culture, string key) =>
            Dictionary[culture.Name]?[key];

        private static CultureInfo _CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
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
                    { "RectangleAnnotationLabel", "Rectangle" },
                    { "TextAnnotationLabel", "Text (Click and hold to set your text box)" },
                    { "ArrowAnnotationLabel", "Arrow" },
                    { "DoubleArrowAnnotationLabel", "Double arrow" },
                    { "LineAnnotationLabel", "Line" },
                    { "MagnifyingGlassAnnotationLabel", "Magnifying glass" },
                    { "ColorAnnotationLabel", "Color" },

                    { "ChangeBrushActionLabel", "Change brush" },
                    { "DeleteActionLabel", "Delete" },
                    { "EditTextActionLabel", "Edit" },
                    { "IncreaseThicknessActionLabel", "Increase thickness" },
                    { "DecreaseThicknessActionLabel", "Decrease thickness" },
                    { "IncreaseZoomActionLabel", "Increase zoom" },
                    { "DecreaseZoomActionLabel", "Decrease zoom" }
                }
            },
            {
                CultureInfo.GetCultureInfo("fr-FR").Name, new Dictionary<string, string>()
                {
                    { "RectangleAnnotationLabel", "Rectangle" },
                    { "TextAnnotationLabel", "Texte (Cliquez et maintenez cliqué pour définir votre zone de texte)" },
                    { "ArrowAnnotationLabel", "Flèche" },
                    { "DoubleArrowAnnotationLabel", "Double flèche" },
                    { "LineAnnotationLabel", "Ligne" },
                    { "MagnifyingGlassAnnotationLabel", "Loupe" },
                    { "ColorAnnotationLabel", "Couleur" },

                    { "ChangeBrushActionLabel", "Changer de pinceau" },
                    { "DeleteActionLabel", "Effacer" },
                    { "EditTextActionLabel", "Modifier" },
                    { "IncreaseThicknessActionLabel", "Augmenter l'épaisseur" },
                    { "DecreaseThicknessActionLabel", "Diminuer l'épaisseur" },
                    { "IncreaseZoomActionLabel", "Augmenter le zoom" },
                    { "DecreaseZoomActionLabel", "Diminuer le zoom" }
                }
            },
            {
                CultureInfo.GetCultureInfo("de-DE").Name, new Dictionary<string, string>()
                {
                    { "RectangleAnnotationLabel", "Rechteck" },
                    { "TextAnnotationLabel", "Text (Klicken und halten geklickt Ihr Textfeld zu setzen)" },
                    { "ArrowAnnotationLabel", "Pfeil" },
                    { "DoubleArrowAnnotationLabel", "Doppelpfeil" },
                    { "LineAnnotationLabel", "Linie" },
                    { "MagnifyingGlassAnnotationLabel", "Lupe" },
                    { "ColorAnnotationLabel", "Farbe" },

                    { "ChangeBrushActionLabel", "Pinsel wechseln" },
                    { "DeleteActionLabel", "Löschen" },
                    { "EditTextActionLabel", "Bearbeiten" },
                    { "IncreaseThicknessActionLabel", "Dicke erhöhen" },
                    { "DecreaseThicknessActionLabel", "Dicke verringern" },
                    { "IncreaseZoomActionLabel", "Zoom erhöhen" },
                    { "DecreaseZoomActionLabel", "Zoom verringern" }
                }
            },
            {
                CultureInfo.GetCultureInfo("es-ES").Name, new Dictionary<string, string>()
                {
                    { "RectangleAnnotationLabel", "Rectángulo" },
                    { "TextAnnotationLabel", "Texto (Haga clic y mantenga clic para establecer el cuadro de texto)" },
                    { "ArrowAnnotationLabel", "Flecha" },
                    { "DoubleArrowAnnotationLabel", "Doble flecha" },
                    { "LineAnnotationLabel", "Línea" },
                    { "MagnifyingGlassAnnotationLabel", "Lupa" },
                    { "ColorAnnotationLabel", "Color" },

                    { "ChangeBrushActionLabel", "Cambiar pincel" },
                    { "DeleteActionLabel", "Eliminar" },
                    { "EditTextActionLabel", "Editar" },
                    { "IncreaseThicknessActionLabel", "Aumentar el espesor" },
                    { "DecreaseThicknessActionLabel", "Disminuir el espesor" },
                    { "IncreaseZoomActionLabel", "Aumentar el zoom" },
                    { "DecreaseZoomActionLabel", "Disminuir el zoom" }
                }
            },
            {
                CultureInfo.GetCultureInfo("pt-BR").Name, new Dictionary<string, string>()
                {
                    { "RectangleAnnotationLabel", "Retângulo" },
                    { "TextAnnotationLabel", "Texto (Clique e mantenha clicado para definir a sua caixa de texto)" },
                    { "ArrowAnnotationLabel", "Seta" },
                    { "DoubleArrowAnnotationLabel", "Seta dupla" },
                    { "LineAnnotationLabel", "Linha" },
                    { "MagnifyingGlassAnnotationLabel", "Lupa" },
                    { "ColorAnnotationLabel", "Cor" },

                    { "ChangeBrushActionLabel", "Trocar escova" },
                    { "DeleteActionLabel", "Excluir" },
                    { "EditTextActionLabel", "Editar" },
                    { "IncreaseThicknessActionLabel", "Aumentar espessura" },
                    { "DecreaseThicknessActionLabel", "Diminuir espessura" },
                    { "IncreaseZoomActionLabel", "Aumentar zoom" },
                    { "DecreaseZoomActionLabel", "Diminuir zoom" }
                }
            },
            {
                CultureInfo.GetCultureInfo("pl-PL").Name, new Dictionary<string, string>()
                {
                    { "RectangleAnnotationLabel", "Prostokąt" },
                    { "TextAnnotationLabel", "Tekst (kliknij i przytrzymaj, aby ustawić pole tekstowe)" },
                    { "ArrowAnnotationLabel", "Strzałka" },
                    { "DoubleArrowAnnotationLabel", "Podwójna strzałka" },
                    { "LineAnnotationLabel", "Linia" },
                    { "MagnifyingGlassAnnotationLabel", "Szkło powiększające" },
                    { "ColorAnnotationLabel", "Kolor" },

                    { "ChangeBrushActionLabel", "Zmień pędzel" },
                    { "DeleteActionLabel", "Usunąć" },
                    { "EditTextActionLabel", "Edytować" },
                    { "IncreaseThicknessActionLabel", "Zwiększ grubość" },
                    { "DecreaseThicknessActionLabel", "Zmniejsz grubość" },
                    { "IncreaseZoomActionLabel", "Zwiększ powiększenie" },
                    { "DecreaseZoomActionLabel", "Zmniejsz powiększenie" }
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
