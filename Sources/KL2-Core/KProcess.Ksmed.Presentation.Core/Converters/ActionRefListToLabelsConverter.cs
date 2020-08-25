using KProcess.Ksmed.Models;
using System;
using System.Collections;
using System.Linq;
using System.Windows.Data;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    /// <summary>
    /// Convertie une liste d'actionRefs en un libellé
    /// </summary>
    public class ActionRefListToLabelsConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var enumeration = value as IEnumerable;
            if(enumeration != null)
            {
                var labels = enumeration.OfType<IReferentialActionLink>().Select(actionRef => actionRef.Referential.Label);
                return string.Join(", ", labels);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
