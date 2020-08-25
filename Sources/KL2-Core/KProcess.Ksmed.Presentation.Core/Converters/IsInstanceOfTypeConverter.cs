using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    /// <summary>
    /// Renvoie <c>true</c> si l'instance est du type spécifié dans le paramètre.
    /// </summary>
    public class IsInstanceOfTypeConverter : IValueConverter
    {
        /// <summary>
        /// Renvoie <c>true</c> si l'instance est du type spécifié dans le paramètre.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>
        /// Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var paramType = parameter as Type;
            if (paramType == null || value == null)
                return false;

            var valueType = value.GetType();

            return valueType == paramType || paramType.IsAssignableFrom(valueType);
        }

        /// <summary>
        /// Non supporté.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>
        /// Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
