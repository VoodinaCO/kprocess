using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace KProcess.Ksmed.Server.Activation.KeygenApp
{
    public class RenewalDateToBrushConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values[0] != null && values[0] is DateTime)
            {
                var RenewalDate = (DateTime)values[0];
                if (RenewalDate < DateTime.Today)
                    return Brushes.Red;
                else if (RenewalDate.AddDays(-30) < DateTime.Today)
                    return Brushes.Orange;
                else
                    return Brushes.LightGreen;
            }

            return null;
            
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
