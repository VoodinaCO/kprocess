using KProcess.Ksmed.Models;
using System;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    /// <summary>
    /// Converter qui renvoie si au moins une vidéo du process est en cours de téléchargement
    /// </summary>
    public class IsSyncingConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null && values.Any() && values[0] is Procedure process)
            {
                if (!process.SyncVideo)
                    return false;
                return process.IsSyncing;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
