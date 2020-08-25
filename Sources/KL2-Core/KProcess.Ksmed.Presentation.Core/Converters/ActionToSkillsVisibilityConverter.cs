using KProcess.Ksmed.Models;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    /// <summary>
    /// Converter qui renvoit une visibilité en fonction de l'action.
    /// </summary>
    public class ActionToSkillsVisibilityConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Convertit l'inverse d'un booléen en Visibility.
        /// </summary>
        /// <param name="value">La valeur produite par la source du binding.</param>
        /// <param name="targetType">Le type de la propriété cible du binding.</param>
        /// <param name="parameter">Le paramètre de conversion à utiliser.</param>
        /// <param name="culture">La culture à utiliser pour la conversion.</param>
        /// <returns>Une valeur convertie. Si la méthode retourne null, la valeur valide null est utilisée.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            if (value is KAction action)
            {
                if (action.IsGroup || action.Scenario.Project.Process.IsSkill)
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        /// <summary>
        /// Convertit une Visibility en booléen inversé.
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
