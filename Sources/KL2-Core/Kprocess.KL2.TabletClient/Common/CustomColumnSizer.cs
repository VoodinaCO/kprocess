using Syncfusion.UI.Xaml.Grid;
using System;
using System.Windows;
using System.Windows.Media;

namespace Kprocess.KL2.TabletClient.Common
{
    // TODO : Gérer directement le DataTemplate
    public class CustomColumnSizer : GridColumnSizer
    {
        public CustomColumnSizer(SfDataGrid grid)
               : base(grid)
        {
        }

        protected override Size MeasureTemplate(Size rect, object record, GridColumn column, GridQueryBounds bounds)
        {
            if (column is GridTextColumn)
            {
                string[] properties = column.MappingName.Split('.');
                object data = record;
                foreach (string property in properties)
                    data = data?.GetType().GetProperty(property).GetValue(data);
                string datatext = Convert.ToString(data);

                return MeasureText(datatext, column);
            }
            return rect;
        }

        protected override FormattedText GetFormattedText(GridColumn column, object record, string datatext) =>
            GetFormattedTextInternal(column, datatext);

        FormattedText GetFormattedTextInternal(GridColumn column, string datatext)
        {
            FormattedText formattedtext;
            formattedtext = new FormattedText(datatext, System.Globalization.CultureInfo.CurrentCulture, DataGrid.FlowDirection, new Typeface(FontFamily, DataGrid.FontStyle, DataGrid.FontWeight, DataGrid.FontStretch), DataGrid.FontSize, Brushes.Black);
            return formattedtext;
        }

        public Size MeasureText(string datatext, GridColumn column, bool ignoreWidth = false)
        {
            if (string.IsNullOrEmpty(datatext))
                return new Size(0, 0);

            FormattedText formattedtext = GetFormattedTextInternal(column, datatext);
            formattedtext.Trimming = TextTrimming.None;

            if (!ignoreWidth && !double.IsNaN(column.Width))
                formattedtext.MaxTextWidth = column.Width - column.Padding.Left - column.Padding.Right;
            else if (!ignoreWidth && !double.IsNaN(column.ActualWidth))
                formattedtext.MaxTextWidth = column.ActualWidth - column.Padding.Left - column.Padding.Right;
            formattedtext.MaxTextHeight = double.MaxValue;

            if (formattedtext.MaxTextWidth > (Margin.Left + Margin.Right))
                formattedtext.MaxTextWidth -= (Margin.Left + Margin.Right);

            return new Size(formattedtext.Width, formattedtext.Height);
        }
    }
}
