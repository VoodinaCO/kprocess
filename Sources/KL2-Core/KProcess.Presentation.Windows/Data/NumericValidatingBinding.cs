using System;
using System.ComponentModel;
using System.Windows.Data;

namespace KProcess.Presentation.Windows.Data
{
#if !SILVERLIGHT

    /// <summary>
    /// Binding gérant nativement la validation dés le changement de valeur de propriété
    /// </summary>
    /// <remarks>met par défaut ValidatesOnDataErrors à true et UpdateSourceTrigger à PropertyChanged</remarks>
    public class NumericValidatingBinding : InstantValidatingBinding
    {
        #region Classe interne

        private class StringToNumericConverter : IValueConverter
        {

            /// <summary>
            /// Convertit une valeur.
            /// </summary>
            /// <param name="value">Valeur produite par la source de liaison.</param>
            /// <param name="targetType">Type de la propriété de cible de liaison.</param>
            /// <param name="parameter">Paramètre de convertisseur à utiliser.</param>
            /// <param name="culture">Culture à utiliser dans le convertisseur.</param>
            /// <returns>
            /// Une valeur convertie.Si la méthode retourne null, la valeur Null valide est utilisée.
            /// </returns>
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return value;
            }

            /// <summary>
            /// Convertit une valeur.
            /// </summary>
            /// <param name="value">Valeur produite par la cible de liaison.</param>
            /// <param name="targetType">Type dans lequel convertir.</param>
            /// <param name="parameter">Paramètre de convertisseur à utiliser.</param>
            /// <param name="culture">Culture à utiliser dans le convertisseur.</param>
            /// <returns>
            /// Une valeur convertie.Si la méthode retourne null, la valeur Null valide est utilisée.
            /// </returns>
            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                try
                {
                    return TypeDescriptor.GetConverter(targetType).ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, value);
                }
                catch
                {
                    return null;
                }
            }
        }
        #endregion

        /// <summary>
        /// Convertisseur spécifique à ce binding
        /// </summary>
        private static StringToNumericConverter converter = new StringToNumericConverter();

        #region Constructeurs

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public NumericValidatingBinding()
          : base()
        {
        }

        /// <summary>
        /// Constructeur indiquant le chemin de la source de données
        /// </summary>
        /// <param name="path">chemin de la source de données</param>
        public NumericValidatingBinding(string path)
          : base(path)
        {
        }

        #endregion

        #region Surcharges

        /// <summary>
        /// Initialise le binding
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.Converter = converter;
        }

        #endregion

    }

#endif



}