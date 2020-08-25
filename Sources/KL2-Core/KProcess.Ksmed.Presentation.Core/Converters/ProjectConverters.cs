using KProcess.Globalization;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class ProjectToIconConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Project project)
            {
                if (project == null || project.ScenariosDescriptions == null)
                    return new Uri("pack://siteoforigin:,,,/Resources/Images/project_empty.png");

                if (project.ScenariosDescriptions.Length == 0) // Aucun scénario
                    return new Uri("pack://siteoforigin:,,,/Resources/Images/project_empty.png");
                if (project.ScenariosDescriptions.Any(_ => _.NatureCode.Equals(KnownScenarioNatures.Realized) && _.StateCode.Equals(KnownScenarioStates.Validated))) // Un scénario de validation figé
                    return new Uri("pack://siteoforigin:,,,/Resources/Images/project_fixed_SV.png");
                if (project.ScenariosDescriptions.Any(_ => _.NatureCode.Equals(KnownScenarioNatures.Realized))) // Au moins un scénario de validation
                    return new Uri("pack://siteoforigin:,,,/Resources/Images/project_SV.png");
                if (project.ScenariosDescriptions.Any(_ => _.NatureCode.Equals(KnownScenarioNatures.Target) && _.StateCode.Equals(KnownScenarioStates.Validated))) // Un scénario cible figé
                    return new Uri("pack://siteoforigin:,,,/Resources/Images/project_fixed_SC.png");
                if (project.ScenariosDescriptions.Any(_ => _.NatureCode.Equals(KnownScenarioNatures.Target))) // Au moins un scénario cible
                    return new Uri("pack://siteoforigin:,,,/Resources/Images/project_SC.png");
                if (project.ScenariosDescriptions.Any(_ => _.NatureCode.Equals(KnownScenarioNatures.Initial) && _.StateCode.Equals(KnownScenarioStates.Validated))) // Un scénario initial figé
                    return new Uri("pack://siteoforigin:,,,/Resources/Images/project_fixed_SI.png");
                return new Uri("pack://siteoforigin:,,,/Resources/Images/project_empty.png"); // Un scénario initial
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class ProjectToTooltipConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Project project)
            {
                if (project == null || project.ScenariosDescriptions == null)
                    return LocalizationManager.GetString("View_PrepareProject_Empty");

                if (project.ScenariosDescriptions.Length == 0) // Aucun scénario
                    return LocalizationManager.GetString("View_PrepareProject_Empty");
                if (project.ScenariosDescriptions.Any(_ => _.NatureCode.Equals(KnownScenarioNatures.Realized) && _.StateCode.Equals(KnownScenarioStates.Validated))) // Un scénario de validation figé
                    return LocalizationManager.GetString("View_PrepareProject_FixedSV");
                if (project.ScenariosDescriptions.Any(_ => _.NatureCode.Equals(KnownScenarioNatures.Realized))) // Au moins un scénario de validation
                    return LocalizationManager.GetString("View_PrepareProject_OneSV");
                if (project.ScenariosDescriptions.Any(_ => _.NatureCode.Equals(KnownScenarioNatures.Target) && _.StateCode.Equals(KnownScenarioStates.Validated))) // Un scénario cible figé
                    return LocalizationManager.GetString("View_PrepareProject_FixedSC");
                if (project.ScenariosDescriptions.Any(_ => _.NatureCode.Equals(KnownScenarioNatures.Target))) // Au moins un scénario cible
                    return LocalizationManager.GetString("View_PrepareProject_OneSC");
                if (project.ScenariosDescriptions.Any(_ => _.NatureCode.Equals(KnownScenarioNatures.Initial) && _.StateCode.Equals(KnownScenarioStates.Validated))) // Un scénario initial figé
                    return LocalizationManager.GetString("View_PrepareProject_FixedSI");
                return LocalizationManager.GetString("View_PrepareProject_Empty"); // Un scénario initial
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
