using Syncfusion.UI.Xaml.Grid;
using System.Windows.Interactivity;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    public class QueryRowHeightBehaviour : Behavior<SfDataGrid>
    {
        GridRowSizingOptions gridRowResizingOptions = new GridRowSizingOptions();
        //List<string> excludeColumns = new List<string>();
        double Height = double.NaN;

        protected override void OnAttached()
        {
            //AssociatedObject.ItemsSourceChanged += AssociatedObject_ItemsSourceChanged;
            AssociatedObject.QueryRowHeight += AssociatedObject_QueryRowHeight;
        }

        /*void AssociatedObject_ItemsSourceChanged(object sender, GridItemsSourceChangedEventArgs e)
        {
            foreach (GridColumn column in AssociatedObject.Columns)
                if (!column.MappingName.Equals("Address") && !column.MappingName.Equals("CompanyName"))
                    excludeColumns.Add(column.MappingName);

            gridRowResizingOptions.ExcludeColumns = excludeColumns;
        }*/

        void AssociatedObject_QueryRowHeight(object sender, QueryRowHeightEventArgs e)
        {
            if (AssociatedObject.IsTableSummaryIndex(e.RowIndex))
            {
                e.Height = 40;
                e.Handled = true;
            }
            else if (AssociatedObject.GridColumnSizer.GetAutoRowHeight(e.RowIndex, gridRowResizingOptions, out Height))
            {
                if (Height > AssociatedObject.RowHeight)
                {
                    e.Height = Height;
                    e.Handled = true;
                }
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.QueryRowHeight -= AssociatedObject_QueryRowHeight;
            //AssociatedObject.ItemsSourceChanged -= AssociatedObject_ItemsSourceChanged;
        }
    }
}
