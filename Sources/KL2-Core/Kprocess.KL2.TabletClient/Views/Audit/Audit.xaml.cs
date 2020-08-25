using Kprocess.KL2.TabletClient.ViewModel;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System.Windows;
using System.Windows.Controls;

namespace Kprocess.KL2.TabletClient.Views
{
    public partial class Audit : UserControl
    {
        GridRowSizingOptions gridRowResizingOptions = new GridRowSizingOptions();

        public double DefaultRowHeight { get; } = 100;

        #region Constructors

        public Audit()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Methods

        void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var visualContainer = DataGrid.GetVisualContainer();
            int count = visualContainer.RowCount;
            for (int i = 1; i < count; i += 2)
            {
                if (DataGrid.GridColumnSizer.GetAutoRowHeight(i, gridRowResizingOptions, out double autoHeight))
                {
                    if (autoHeight + 4 > DefaultRowHeight)
                        visualContainer.RowHeights[i] = autoHeight + 4;
                }
            }
            visualContainer.InvalidateMeasure();

            visualContainer.Loaded += ScrollTo;
        }

        /// <summary>
        /// Méthode appellé lorsque la source de la grille change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DataGrid_ItemsSourceChanged(object sender, GridItemsSourceChangedEventArgs e)
        {
            ScrollTo(sender, null);
        }

        void ScrollTo(object sender, RoutedEventArgs e)
        {
            if (DataGrid.DataContext is AuditViewModel context)
                context.ScrollTo(null);
        }

        #endregion
    }
}