using System;
using System.Collections.Generic;
using System.Reflection;
using GalaSoft.MvvmLight.CommandWpf;
using Kprocess.KL2.TabletClient.ViewModel;
using KProcess.Ksmed.Models;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Kprocess.KL2.TabletClient.Views
{
    public partial class Inspection : UserControl
    {
        GridRowSizingOptions gridRowResizingOptions = new GridRowSizingOptions();

        public double DefaultRowHeight { get; } = 100;

        #region Constructors

        public Inspection()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Methods

        void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid.ExpandAllDetailsView();

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
            DataGrid.ExpandAllDetailsView();
            ScrollTo(sender, null);
        }

        void ScrollTo(object sender, RoutedEventArgs e)
        {
            if (DataGrid.DataContext is InspectionViewModel context)
                context.ScrollTo(null);
        }

        #endregion

        ICommand _expandOrCollapseCommand;
        public ICommand ExpandOrCollapseCommand
        {
            get
            {
                if (_expandOrCollapseCommand == null)
                    _expandOrCollapseCommand = new RelayCommand<PublishedAction>(pAction =>
                    {
                        int rowIndex = DataGrid.ResolveToRowIndex(pAction);
                        int recordIndex = DataGrid.ResolveToRecordIndex(rowIndex);
                        if (recordIndex < 0)
                            return;
                        if (DataGrid.View.Records[recordIndex].IsExpanded)
                            DataGrid.CollapseDetailsViewAt(recordIndex);
                        else
                            DataGrid.ExpandDetailsViewAt(recordIndex);
                    });
                return _expandOrCollapseCommand;
            }
        }
    }
}