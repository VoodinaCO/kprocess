using KProcess;
using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Kprocess.KL2.TabletClient.Converter
{
    /// <summary>
    /// Indente le WBS.
    /// </summary>
    public class IndentWBSConverter : MarkupExtension, IValueConverter
    {
        static IndentWBSConverter _instance;
        public static IndentWBSConverter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new IndentWBSConverter();
                return _instance;
            }
        }

        public string Convert(string wbs) =>
            (string)Convert(wbs, typeof(string), null, null);

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string wbs)
            {
                string result = wbs;
                int indent = wbs.CountOccurences(".");
                while (indent-- > 0)
                    result = result.Insert(0, "  ");
                return result;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) =>
            (value as string)?.TrimStart();

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}
