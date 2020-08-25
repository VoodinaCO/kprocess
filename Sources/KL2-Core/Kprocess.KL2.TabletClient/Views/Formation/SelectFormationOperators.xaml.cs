using GalaSoft.MvvmLight.CommandWpf;
using Kprocess.KL2.TabletClient.Models;
using Kprocess.KL2.TabletClient.ViewModel;
using Syncfusion.UI.Xaml.Grid;

namespace Kprocess.KL2.TabletClient.Views
{
    /// <summary>
    /// Logique d'interaction pour PublishedActionDetailsDialog.xaml
    /// </summary>
    public partial class SelectFormationOperators
    {
        readonly GridRowSizingOptions gridRowResizingOptions = new GridRowSizingOptions();

        public double DefaultRowHeight { get; } = 60;

        #region Constructors

        public SelectFormationOperators()
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

        void DataGrid_ItemsSourceChanged(object sender, GridItemsSourceChangedEventArgs e)
        {
            if (DataContext is SelectFormationOperatorViewModel context)
                context.OperatorsDataGrid = DataGrid;
        }

        void DataGrid_CurrentCellActivating(object sender, CurrentCellActivatingEventArgs e)
        {
            if (DataContext is SelectFormationOperatorViewModel)
            {
                var rowIndex = e.CurrentRowColumnIndex.RowIndex;
                var recordIndex = DataGrid.ResolveToRecordIndex(rowIndex);
                if (recordIndex < 0)
                    return;
                e.Cancel |= DataGrid.View.Records.GetItemAt(recordIndex) is UIUser uiUser && uiUser.LastValidationTask.Contains(Locator.LocalizationManager.GetString("TrainingCompleted"));
            }
        }
    }
}
