using KProcess.Ksmed.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core.Converters
{
    public class PredecessorsConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            KAction item = (KAction)values[0];
            KAction dependentItem = (KAction)values[1];
            if (item.Original == null && dependentItem.Original == null) // On est dans un scénario initial
                return true;
            else if (item.Original == null || dependentItem.Original == null) // L'une des actions est nouvelle
                return false;
            else if (dependentItem.Original.Predecessors.Any(p => p.ActionId == item.Original.ActionId))
                return true;
            else
                return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class AllPredecessorsConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            KAction item = (KAction)value;
            if (item == null)
                return true;
            else if (item.Original == null) // L'item a été créé en premier
                return true;
            else if (item.Predecessors.Count != item.Original.Predecessors.Count)
                return false;
            else
                return item.Predecessors.All(pred => item.Original.Predecessors.Contains(pred.Original));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }

    public class OriginalPredecessorsToTooltipConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            KAction item = (KAction)value;
            if (item != null && item.Original != null && item.Original.Predecessors.Count != 0)
                return string.Join("\n", item.Original.Predecessors.OrderBy(_ => _.WBS).Select(_ => $"{_.WBS} - {_.Label}").ToArray());
            else
                return "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
