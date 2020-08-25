using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class ScenarioStateToVisibilityConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is Scenario scenario && values[1] is IEnumerable<Scenario> scenarios)
            {
                string stateCode = scenario.StateCode;
                string natureCode = scenario.NatureCode;
                bool hasValidationScenario = scenarios.Any(sc => sc.NatureCode == KnownScenarioNatures.Realized);
                return stateCode == KnownScenarioStates.Validated ||
                            (natureCode != KnownScenarioNatures.Realized && hasValidationScenario) ? Visibility.Visible : Visibility.Collapsed;
            }
            if (values[0] is ScenarioDescription scenarioDescription && values[1] is IEnumerable<ScenarioDescription> scenariosDescriptions)
            {
                string stateCode = scenarioDescription.StateCode;
                string natureCode = scenarioDescription.NatureCode;
                bool hasValidationScenario = scenariosDescriptions.Any(sc => sc.NatureCode == KnownScenarioNatures.Realized);
                return stateCode == KnownScenarioStates.Validated ||
                            (natureCode != KnownScenarioNatures.Realized && hasValidationScenario) ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
