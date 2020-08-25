using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    /// <summary>
    /// Convertit un ratio en pourcentage.
    /// </summary>
    public class RatioToPercentConverter : IValueConverter
    {
        /// <summary>
        /// Obtient ou définit le nombre de décimales à utiliser.
        /// </summary>
        public int DisplayedDecimals { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="RatioToPercentConverter"/>.
        /// </summary>
        public RatioToPercentConverter()
        {
            this.DisplayedDecimals = 1;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var v = System.Convert.ToDouble(value);
            v = Math.Round(v * 100, DisplayedDecimals);
            return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var v = System.Convert.ToDouble(value);
                return v / 100;
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}
