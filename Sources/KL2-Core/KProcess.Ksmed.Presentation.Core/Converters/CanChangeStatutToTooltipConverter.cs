using KProcess.KL2.APIClient;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Presentation.Windows.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class CanChangeStatutToTooltipConverter : MarkupExtension, IMultiValueConverter
    {
        readonly IAPIHttpClient _apiHttpClient;
        readonly ILocalizationManager _localizationManager;
        readonly IPrepareService _prepareService;

        public CanChangeStatutToTooltipConverter()
        {
            _apiHttpClient = IoC.Resolve<IAPIHttpClient>();
            _localizationManager = IoC.Resolve<ILocalizationManager>();
            _prepareService = IoC.Resolve<IServiceBus>().Get<IPrepareService>();
        }

        public CanChangeStatutToTooltipConverter(IAPIHttpClient apiHttpClient, ILocalizationManager localizationManager, IPrepareService prepareService)
        {
            _apiHttpClient = apiHttpClient;
            _localizationManager = localizationManager;
            _prepareService = prepareService;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 2
                && values[0] is Scenario currentScenario
                && values[1] is IEnumerable<Scenario> scenarios)
            {
                if (currentScenario?.NatureCode == KnownScenarioNatures.Realized)
                {
                    if (_apiHttpClient != null && !string.IsNullOrEmpty(_apiHttpClient.Token) && _prepareService.HasDocumentationDraftSync(currentScenario.ScenarioId))
                        return _localizationManager.GetString("View_PrepareScenarios_CantDeleteScenario");
                    return null;
                }
                return scenarios?.Any(s => s.NatureCode == KnownScenarioNatures.Realized) == true ?
                    _localizationManager.GetString("View_PrepareScenarios_CantChangeStatut") : null;
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class CanDeleteScenarioToTooltipConverter : MarkupExtension, IMultiValueConverter
    {
        readonly IAPIHttpClient _apiHttpClient;
        readonly ILocalizationManager _localizationManager;
        readonly IPrepareService _prepareService;
        readonly CanChangeStatutToTooltipConverter _converter;

        public CanDeleteScenarioToTooltipConverter()
        {
            _apiHttpClient = IoC.Resolve<IAPIHttpClient>();
            _localizationManager = IoC.Resolve<ILocalizationManager>();
            _prepareService = IoC.Resolve<IServiceBus>().Get<IPrepareService>();
            _converter = new CanChangeStatutToTooltipConverter(_apiHttpClient, _localizationManager, _prepareService);
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 3
                && values[0] is bool isEnabled
                && values[1] is Scenario currentScenario
                && values[2] is IEnumerable<Scenario>
                && !isEnabled
                && currentScenario.NatureCode == KnownScenarioNatures.Realized)
                return _converter.Convert(new [] { values[1], values[2] }, targetType, parameter, culture);
            return string.Format(LocalizeExtension.ShortcutStringFormat,
                _localizationManager.GetValue("View_PrepareScenarios_Remove_Tooltip"),
                ShortcutsManager.Manager[Shortcut.Delete]); ;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
