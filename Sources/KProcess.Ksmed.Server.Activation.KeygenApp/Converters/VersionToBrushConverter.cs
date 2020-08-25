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
    public class VersionToBrushConverter : MarkupExtension, IMultiValueConverter
    {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values!=null && values[0] != null && values[1] != null)
            {
                Version version = new Version();
                if(!Version.TryParse(values[0].ToString(), out version))
                {
                    var tab = values[0].ToString().Split(':');
                   

                    version = new Version(tab[0]);
                }


                var currentVersion = new Version(values[1].ToString());
                if (version >= currentVersion)
                    return Brushes.LightGreen;
            }


            return Brushes.Red;
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
