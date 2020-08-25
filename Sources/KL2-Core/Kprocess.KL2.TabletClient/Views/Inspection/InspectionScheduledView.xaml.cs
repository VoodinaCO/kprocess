using Kprocess.KL2.TabletClient.ViewModel;
using KProcess.Ksmed.Models;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;

namespace Kprocess.KL2.TabletClient.Views
{
    /// <summary>
    /// Logique d'interaction pour InspectionScheduledView.xaml
    /// </summary>
    public partial class InspectionScheduledView
    {
        static double DefaultRowHeight { get; } = 100;

        readonly GridRowSizingOptions gridRowResizingOptions = new GridRowSizingOptions();

        public InspectionScheduledView()
        {
            InitializeComponent();
        }

        #region Event Methods

        /// <summary>
        /// Méthode permettant de définir la hauteur de
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DataGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
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
        }

        #endregion

        void DataGrid_OnCellTapped(object sender, GridCellTappedEventArgs e)
        {
            if (!(DataContext is InspectionScheduledViewModel context))
                return;

            if (!(e.Record is InspectionSchedule inspectionSchedule) || inspectionSchedule.IsClosed)
                return;

            context.SelectInspectionCommand.Execute(inspectionSchedule);
        }
    }
}
