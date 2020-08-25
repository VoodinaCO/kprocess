using Syncfusion.UI.Xaml.Grid;

namespace Kprocess.KL2.TabletClient.Views
{
    /// <summary>
    /// Logique d'interaction pour PublishedActionDetailsDialog.xaml
    /// </summary>
    public partial class SelectQualificationOperators
    {
        GridRowSizingOptions gridRowResizingOptions = new GridRowSizingOptions();

        public double DefaultRowHeight { get; } = 60;

        #region Constructors

        public SelectQualificationOperators()
        {
            InitializeComponent();
            DataGrid.QueryRowHeight += DataGrid_QueryRowHeight;
        }

        #endregion

        void DataGrid_QueryRowHeight(object sender, QueryRowHeightEventArgs e)
        {
            if (DataGrid.GridColumnSizer.GetAutoRowHeight(e.RowIndex, gridRowResizingOptions, out double autoHeight))
            {
                if (autoHeight + 4 > DefaultRowHeight)
                {
                    e.Height = autoHeight + 4;
                    e.Handled = true;
                }
            }
        }
    }
}
