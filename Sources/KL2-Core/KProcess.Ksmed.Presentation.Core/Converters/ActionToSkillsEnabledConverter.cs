using KProcess.Ksmed.Models;
using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    /// <summary>
    /// Converter qui renvoie si l'élément de l'action doit être actif.
    /// </summary>
    public class ActionToSkillsEnabledConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Convertit un booléen en son inverse.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns>       
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;
            if (value is KAction action)
                return !(action.IsGroup || action.Scenario.Project.Process.IsSkill);
            return false;
        }

        /// <summary>
        /// Convertit un booléen en son inverse.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
