using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.KL2.SetupUI
{
    public class LocalizationConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var temp = values.ToList();
            temp.RemoveAt(0);
            string result = LocalizationExt.Dictionary[LocalizationExt.CurrentLanguage.ToCultureInfoString()][parameter.ToString()];
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

    public class CurrentLanguageConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) =>
            (Languages)values?[0] == (Languages)values?[1];

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[2];
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class InstallModeConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (InstallationMode)value == (InstallationMode)parameter;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            (bool)value ? (InstallationMode)parameter : ((InstallationMode)parameter == InstallationMode.Standard ? InstallationMode.Advanced : InstallationMode.Standard);

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class DatabaseInstallModeConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (DatabaseInstallationMode)value == (DatabaseInstallationMode)parameter;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            (bool)value ? (DatabaseInstallationMode)parameter : ((DatabaseInstallationMode)parameter == DatabaseInstallationMode.Local ? DatabaseInstallationMode.Remote : DatabaseInstallationMode.Local);

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class StringToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            string.IsNullOrEmpty((string)value) ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class TooltipOnlyOnDisableConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (bool)values[1] ? null : values[0];
            }
            catch { return Binding.DoNothing; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class AndBooleanConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return values.Cast<bool>().All(_ => _);
            }
            catch { return false; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class OrBooleanConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return values.Cast<bool>().Any(_ => _);
            }
            catch { return false; }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class NotBooleanConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            !(bool)value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            !(bool)value;

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class IntEnumEqualsConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            (int)value == (int)parameter;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class LaunchActionToButtonContentConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var converter = new LocalizationConverter();
            if ((int)values[0] == (int)LaunchAction.UpdateReplace)
                return converter.Convert(new object[] { values[1] }, targetType, "UpdateButton", culture);
            else if ((int)values[0] == (int)LaunchAction.Repair)
                return converter.Convert(new object[] { values[1] }, targetType, "RepairButton", culture);
            else if ((int)values[0] == (int)LaunchAction.Uninstall)
                return converter.Convert(new object[] { values[1] }, targetType, "UninstallButton", culture);
            else
                return "!ERROR!";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class LaunchActionToTitleConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var converter = new LocalizationConverter();
            if ((int)values[0] == (int)LaunchAction.UpdateReplace)
                return converter.Convert(new object[]{ values[1], values[2] }, targetType, "UpdateTitle", culture);
            else if ((int)values[0] == (int)LaunchAction.Repair)
                return converter.Convert(new object[] { values[1], values[2] }, targetType, "RepairTitle", culture);
            else if ((int)values[0] == (int)LaunchAction.Uninstall)
                return converter.Convert(new object[] { values[1], values[2] }, targetType, "UninstallTitle", culture);
            else
                return "!ERROR!";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class IsNullConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value == null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
