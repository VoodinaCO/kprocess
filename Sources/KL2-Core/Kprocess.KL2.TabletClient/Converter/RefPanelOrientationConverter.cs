using Kprocess.KL2.TabletClient.Models;
using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.TabletClient.Converter
{
    public class RefPanelOrientationConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is ICollection<PublishedReferentialAction> refs)
                {
                    if (refs.Any() == true && refs.All(_ => _.PublishedReferential.File != null && ExtensionsUtil.IsImageExtension(_.PublishedReferential.File.Extension)))
                        return Orientation.Horizontal;
                }
                return Orientation.Vertical;
            }
            catch
            {
                return Orientation.Vertical;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
