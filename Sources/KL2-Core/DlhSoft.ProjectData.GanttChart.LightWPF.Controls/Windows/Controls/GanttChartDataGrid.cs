namespace DlhSoft.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using DlhSoft.Windows.Data;
    using DlhSoft.Windows.Licensing;


    public partial class GanttChartDataGrid : DataGrid, IGanttChartView
    {
        public static readonly DependencyProperty AreUpdateTimelinePageButtonsVisibleProperty = DependencyProperty.Register("AreUpdateTimelinePageButtonsVisible", typeof(bool), typeof(GanttChartDataGrid), new PropertyMetadata(true));
        public static readonly DependencyProperty AssignmentsTemplateProperty = DependencyProperty.Register("AssignmentsTemplate", typeof(DataTemplate), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        private int asyncItemCount;
        private DispatcherTimer asyncTimer;
        public static readonly DependencyProperty BarHeightProperty = DependencyProperty.Register("BarHeight", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata((double)1.0 / (double)0.0));
        public static readonly DependencyProperty ChartWidthProperty = DependencyProperty.Register("ChartWidth", typeof(GridLength), typeof(GanttChartDataGrid), new PropertyMetadata(new GridLength(0.5, GridUnitType.Star)));
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(DlhSoft.Windows.Controls.DataGridColumnCollection), typeof(GanttChartDataGrid), new PropertyMetadata(null, new PropertyChangedCallback(GanttChartDataGrid.OnColumnsChanged)));
        public static readonly DependencyProperty CompletedBarHeightProperty = DependencyProperty.Register("CompletedBarHeight", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata((double)1.0 / (double)0.0));
        public static readonly DependencyProperty CurrentTimeLineStrokeProperty = DependencyProperty.Register("CurrentTimeLineStroke", typeof(Brush), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty DataGridWidthProperty = DependencyProperty.Register("DataGridWidth", typeof(GridLength), typeof(GanttChartDataGrid), new PropertyMetadata(new GridLength(0.5, GridUnitType.Star)));
        private Panel dataHeaderPanel;
        private Panel dataRowsPanel;
        private DlhSoft.Windows.Controls.DataTreeGrid dataTreeGrid;
        private ObservableCollection<DataGridColumn> dataTreeGridColumns;
        private ScrollViewer dataTreeGridScrollViewer;
        private ScrollBar dataTreeGridVerticalScrollBar;
        public static readonly DependencyProperty DependencyCreationValidatorProperty = DependencyProperty.Register("DependencyCreationValidator", typeof(DlhSoft.Windows.Controls.DependencyCreationValidator), typeof(GanttChartDataGrid), new PropertyMetadata(DlhSoft.Windows.Controls.GanttChartView.DefaultDependencyCreationValidator));
        public static readonly DependencyProperty DependencyDeletionContextMenuItemHeaderProperty = DependencyProperty.Register("DependencyDeletionContextMenuItemHeader", typeof(object), typeof(GanttChartDataGrid), new PropertyMetadata("Delete"));
        public static readonly DependencyProperty DependencyDeletionValidatorProperty = DependencyProperty.Register("DependencyDeletionValidator", typeof(DlhSoft.Windows.Controls.DependencyDeletionValidator), typeof(GanttChartDataGrid), new PropertyMetadata(DlhSoft.Windows.Controls.GanttChartView.DefaultDependencyDeletionValidator));
        public static readonly DependencyProperty DependencyLineStrokeProperty = DependencyProperty.Register("DependencyLineStroke", typeof(Brush), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty DependencyLineStrokeThicknessProperty = DependencyProperty.Register("DependencyLineStrokeThickness", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata(0.0));
        public static readonly DependencyProperty DependencyLineTemplateProperty = DependencyProperty.Register("DependencyLineTemplate", typeof(DataTemplate), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty DisplayedTimeProperty = DependencyProperty.Register("DisplayedTime", typeof(DateTime), typeof(GanttChartDataGrid), new PropertyMetadata(DateTime.Now.AddDays(-1.0), new PropertyChangedCallback(GanttChartDataGrid.OnDisplayedTimeChanged)));
        public static readonly DependencyProperty ExpanderTemplateProperty = DependencyProperty.Register("ExpanderTemplate", typeof(ControlTemplate), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        private DlhSoft.Windows.Controls.GanttChartView ganttChartView;
        private ScrollViewer ganttChartViewScrollViewer;
        private ScrollBar ganttChartViewVerticalScrollBar;
        public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(GanttChartDataGrid), new PropertyMetadata(null));

        // Modif Tekigo : La valeur par défaut est 1.0 / 0.0 = +Infini, ce qui fait planter lorsque la grille est dans un tabcontrol
        public static readonly DependencyProperty HeaderHeightProperty = DependencyProperty.Register("HeaderHeight", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata(double.NaN));

        public static readonly DependencyProperty HourWidthProperty = DependencyProperty.RegisterAttached("HourWidth", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata(2.5));
        public static readonly DependencyProperty IndentationUnitSizeProperty = DependencyProperty.Register("IndentationUnitSize", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata(16.0));
        private int internalUpdateItemsSourceCount;
        public static readonly DependencyProperty IsAsyncPresentationEnabledMinCountProperty = DependencyProperty.Register("IsAsyncPresentationEnabledMinCount", typeof(int), typeof(GanttChartDataGrid), new PropertyMetadata(0x100));
        public static readonly DependencyProperty IsAsyncPresentationEnabledPageSizeProperty = DependencyProperty.Register("IsAsyncPresentationEnabledPageSize", typeof(int), typeof(GanttChartDataGrid), new PropertyMetadata(0x10));
        public static readonly DependencyProperty IsAsyncPresentationEnabledProperty = DependencyProperty.Register("IsAsyncPresentationEnabled", typeof(bool), typeof(GanttChartDataGrid), new PropertyMetadata(true, new PropertyChangedCallback(GanttChartDataGrid.OnIsAsyncPresentationEnabledChanged)));
        private bool isAsyncTimerPaused;
        public static readonly DependencyProperty IsCurrentTimeLineVisibleProperty = DependencyProperty.Register("IsCurrentTimeLineVisible", typeof(bool), typeof(GanttChartDataGrid), new PropertyMetadata(true));
        public static readonly DependencyProperty IsDependencyToolTipVisibleProperty = DependencyProperty.Register("IsDependencyToolTipVisible", typeof(bool), typeof(GanttChartDataGrid), new PropertyMetadata(true));
        //private bool isDuringUpdateGanttChartViewVerticalScrollBarFromDataTree;
        public static readonly DependencyProperty IsNonworkingTimeHighlightedProperty = DependencyProperty.Register("IsNonworkingTimeHighlighted", typeof(bool), typeof(GanttChartDataGrid), new PropertyMetadata(true));
        public static readonly DependencyProperty IsSplitterEnabledProperty = DependencyProperty.Register("IsSplitterEnabled", typeof(bool), typeof(GanttChartDataGrid), new PropertyMetadata(true));
        public static readonly DependencyProperty IsTaskCompletedEffortVisibleProperty = DependencyProperty.Register("IsTaskCompletedEffortVisible", typeof(bool), typeof(GanttChartDataGrid), new PropertyMetadata(true));
        public static readonly DependencyProperty IsTaskToolTipVisibleProperty = DependencyProperty.Register("IsTaskToolTipVisible", typeof(bool), typeof(GanttChartDataGrid), new PropertyMetadata(true));
        private bool isTemplateApplied;
        public static readonly DependencyProperty IsVirtualizingProperty = DependencyProperty.Register("IsVirtualizing", typeof(bool), typeof(GanttChartDataGrid), new PropertyMetadata(true));
        public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register("ItemHeight", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata((double)1.0 / (double)0.0));
        private GanttChartItemCollection items;
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<GanttChartItem>), typeof(GanttChartDataGrid), new PropertyMetadata(null, new PropertyChangedCallback(GanttChartDataGrid.OnItemsChanged)));
        private DlhSoft.Windows.Controls.DataGridColumnCollection managedColumns;
        public static readonly DependencyProperty MaxChartWidthProperty = DependencyProperty.Register("MaxChartWidth", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata((double)1.0 / (double)0.0));
        public static readonly DependencyProperty MaxDataGridWidthProperty = DependencyProperty.Register("MaxDataGridWidth", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata((double)1.0 / (double)0.0));
        public static readonly DependencyProperty MilestoneBarFillProperty = DependencyProperty.Register("MilestoneBarFill", typeof(Brush), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty MilestoneBarStrokeProperty = DependencyProperty.Register("MilestoneBarStroke", typeof(Brush), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty MilestoneBarStrokeThicknessProperty = DependencyProperty.Register("MilestoneBarStrokeThickness", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata(0.0));
        public static readonly DependencyProperty MilestoneTaskTemplateProperty = DependencyProperty.Register("MilestoneTaskTemplate", typeof(DataTemplate), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty MinChartWidthProperty = DependencyProperty.Register("MinChartWidth", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata((double)1.0 / (double)0.0));
        public static readonly DependencyProperty MinDataGridWidthProperty = DependencyProperty.Register("MinDataGridWidth", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata((double)1.0 / (double)0.0));
        public static readonly DependencyProperty NonworkingIntervalsProperty = DependencyProperty.Register("NonworkingIntervals", typeof(ObservableCollection<TimeInterval>), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty NonworkingTimeBackgroundProperty = DependencyProperty.Register("NonworkingTimeBackground", typeof(Brush), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        private ObservableCollection<GanttChartItem> originalItems;
        public static readonly DependencyProperty PredecessorToolTipTemplateProperty = DependencyProperty.Register("PredecessorToolTipTemplate", typeof(DataTemplate), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty ScaleHeaderHeightProperty = DependencyProperty.Register("ScaleHeaderHeight", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata((double)1.0 / (double)0.0));
        public static readonly DependencyProperty ScalesProperty = DependencyProperty.Register("Scales", typeof(ScaleCollection), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty SplitterBackgroundProperty = DependencyProperty.Register("SplitterBackground", typeof(Brush), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty SplitterBorderBrushProperty = DependencyProperty.Register("SplitterBorderBrush", typeof(Brush), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty SplitterBorderThicknessProperty = DependencyProperty.Register("SplitterBorderThickness", typeof(Thickness), typeof(GanttChartDataGrid), new PropertyMetadata(new Thickness(0.0)));
        public static readonly DependencyProperty SplitterWidthProperty = DependencyProperty.Register("SplitterWidth", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata(4.0));
        public static readonly DependencyProperty StandardBarCornerRadiusProperty = DependencyProperty.Register("StandardBarCornerRadius", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata(0.0));
        public static readonly DependencyProperty StandardBarFillProperty = DependencyProperty.Register("StandardBarFill", typeof(Brush), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty StandardBarStrokeProperty = DependencyProperty.Register("StandardBarStroke", typeof(Brush), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty StandardBarStrokeThicknessProperty = DependencyProperty.Register("StandardBarStrokeThickness", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata(0.0));
        public static readonly DependencyProperty StandardCompletedBarCornerRadiusProperty = DependencyProperty.Register("StandardCompletedBarCornerRadius", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata(0.0));
        public static readonly DependencyProperty StandardCompletedBarFillProperty = DependencyProperty.Register("StandardCompletedBarFill", typeof(Brush), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty StandardCompletedBarStrokeProperty = DependencyProperty.Register("StandardCompletedBarStroke", typeof(Brush), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty StandardCompletedBarStrokeThicknessProperty = DependencyProperty.Register("StandardCompletedBarStrokeThickness", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata(0.0));
        public static readonly DependencyProperty StandardTaskTemplateProperty = DependencyProperty.Register("StandardTaskTemplate", typeof(DataTemplate), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty SummaryBarFillProperty = DependencyProperty.Register("SummaryBarFill", typeof(Brush), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty SummaryBarStrokeProperty = DependencyProperty.Register("SummaryBarStroke", typeof(Brush), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty SummaryBarStrokeThicknessProperty = DependencyProperty.Register("SummaryBarStrokeThickness", typeof(double), typeof(GanttChartDataGrid), new PropertyMetadata(0.0));
        public static readonly DependencyProperty SummaryTaskTemplateProperty = DependencyProperty.Register("SummaryTaskTemplate", typeof(DataTemplate), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty TimelinePageFinishProperty = DependencyProperty.Register("TimelinePageFinish", typeof(DateTime), typeof(GanttChartDataGrid), new PropertyMetadata(DateTime.Today.AddDays(1.0).AddMonths(3), new PropertyChangedCallback(GanttChartDataGrid.OnTimelinePageFinishChanged)));
        public static readonly DependencyProperty TimelinePageStartProperty = DependencyProperty.Register("TimelinePageStart", typeof(DateTime), typeof(GanttChartDataGrid), new PropertyMetadata(DateTime.Today.AddDays(-7.0), new PropertyChangedCallback(GanttChartDataGrid.OnTimelinePageStartChanged)));
        public static readonly DependencyProperty ToolTipTemplateProperty = DependencyProperty.Register("ToolTipTemplate", typeof(DataTemplate), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        //private DispatcherTimer updateGanttChartViewVerticalScrollBarFromDataTreeGridTimer;
        public static readonly DependencyProperty UpdateScaleIntervalProperty = DependencyProperty.Register("UpdateScaleInterval", typeof(TimeSpan), typeof(GanttChartDataGrid), new PropertyMetadata(TimeSpan.Zero));
        public static readonly DependencyProperty UpdateTimelinePageAmountProperty = DependencyProperty.Register("UpdateTimelinePageAmount", typeof(TimeSpan), typeof(GanttChartDataGrid), new PropertyMetadata(TimeSpan.Parse("7.00:00:00")));
        public static readonly DependencyProperty VisibleWeekFinishProperty = DependencyProperty.Register("VisibleWeekFinish", typeof(DayOfWeek), typeof(GanttChartDataGrid), new PropertyMetadata(DayOfWeek.Saturday, new PropertyChangedCallback(GanttChartDataGrid.OnVisibleWeekFinishChanged)));
        public static readonly DependencyProperty VisibleWeekStartProperty = DependencyProperty.Register("VisibleWeekStart", typeof(DayOfWeek), typeof(GanttChartDataGrid), new PropertyMetadata(DayOfWeek.Sunday, new PropertyChangedCallback(GanttChartDataGrid.OnVisibleWeekStartChanged)));
        public static readonly DependencyProperty WorkingTimeBackgroundProperty = DependencyProperty.Register("WorkingTimeBackground", typeof(Brush), typeof(GanttChartDataGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty WorkingWeekFinishProperty = DependencyProperty.Register("WorkingWeekFinish", typeof(DayOfWeek), typeof(GanttChartDataGrid), new PropertyMetadata(DayOfWeek.Friday, new PropertyChangedCallback(GanttChartDataGrid.OnWorkingWeekFinishChanged)));
        public static readonly DependencyProperty WorkingWeekStartProperty = DependencyProperty.Register("WorkingWeekStart", typeof(DayOfWeek), typeof(GanttChartDataGrid), new PropertyMetadata(DayOfWeek.Monday, new PropertyChangedCallback(GanttChartDataGrid.OnWorkingWeekStartChanged)));

        // Modifs Tekigo : valeurs par défaut de Finish Start
        public static readonly DependencyProperty VisibleDayFinishProperty = DependencyProperty.Register("VisibleDayFinish", typeof(TimeOfDay), typeof(GanttChartDataGrid), new PropertyMetadata(TimeOfDay.Parse("24:00:00"), new PropertyChangedCallback(GanttChartDataGrid.OnVisibleDayFinishChanged)));
        public static readonly DependencyProperty VisibleDayStartProperty = DependencyProperty.Register("VisibleDayStart", typeof(TimeOfDay), typeof(GanttChartDataGrid), new PropertyMetadata(TimeOfDay.Parse("00:00:00"), new PropertyChangedCallback(GanttChartDataGrid.OnVisibleDayStartChanged)));
        public static readonly DependencyProperty WorkingDayFinishProperty = DependencyProperty.Register("WorkingDayFinish", typeof(TimeOfDay), typeof(GanttChartDataGrid), new PropertyMetadata(TimeOfDay.Parse("24:00:00"), new PropertyChangedCallback(GanttChartDataGrid.OnWorkingDayFinishChanged)));
        public static readonly DependencyProperty WorkingDayStartProperty = DependencyProperty.Register("WorkingDayStart", typeof(TimeOfDay), typeof(GanttChartDataGrid), new PropertyMetadata(TimeOfDay.Parse("00:00:00"), new PropertyChangedCallback(GanttChartDataGrid.OnWorkingDayStartChanged)));


        public event ItemActivatedEventHandler ItemActivated;

        public event NotifyCollectionChangedEventHandler ItemCollectionChanged;

        public event PropertyChangedEventHandler ItemPropertyChanged;

        public event EventHandler TimelinePageChanged;

        public GanttChartDataGrid()
        {
            try
            {
                LicenseManager.Validate(typeof(GanttChartDataGrid));
            }
            catch (LicenseException)
            {
            }
            try
            {
                // Modif Tekigo
                GenericThemeResolver.Resolve(this);
            }
            catch
            {
            }
            base.AutoGenerateColumns = false;
            base.CanUserSortColumns = false;
            base.CanUserAddRows = false;
            base.CanUserDeleteRows = false;
            base.CanUserResizeRows = false;
            base.HeadersVisibility = DataGridHeadersVisibility.Column;
            base.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            if (this.Items == null)
            {
                this.Items = new ObservableCollection<GanttChartItem>();
            }
        }

        private void AddDataTreeGridColumns(int index, IList columns)
        {
            foreach (DataGridColumn column in columns)
            {
                this.dataTreeGridColumns.Insert(index++, column);
            }
        }

        private void AsyncTimer_Tick(object sender, EventArgs e)
        {
            if (this.asyncItemCount >= this.originalItems.Count)
            {
                this.asyncTimer.Stop();
                this.UpdateDataTreeGridVerticalScrollBarFromGanttChartView();
            }
            else
            {
                for (int i = 0; i < Math.Min(this.IsAsyncPresentationEnabledPageSize, this.originalItems.Count - this.asyncItemCount); i++)
                {
                    this.items.Add(this.originalItems[this.asyncItemCount++]);
                }
            }
        }

        public void AttachItem(GanttChartItem item)
        {
            if (this.dataTreeGrid != null)
            {
                this.DataTreeGrid.AttachItem(item);
            }
            if (this.ganttChartView != null)
            {
                this.ganttChartView.AttachItem(item);
            }
        }

        private void ClearDataGridPanels()
        {
            this.dataRowsPanel = null;
            this.dataHeaderPanel = null;
        }

        private void ClearDataTreeGridColumns()
        {
            this.dataTreeGridColumns.Clear();
        }

        public void ClearScheduleCacheValues()
        {
            if (this.ganttChartView != null)
            {
                this.ganttChartView.ClearScheduleCacheValues();
            }
        }

        protected void CoerceTimelinePage()
        {
            if (this.TimelinePageFinish <= this.TimelinePageStart)
            {
                this.TimelinePageFinish = this.TimelinePageStart.AddTicks(1L);
            }
        }

        private void CoerceVisibleDay()
        {
            if (this.VisibleDayFinish <= this.VisibleDayStart)
            {
                this.VisibleDayFinish = this.VisibleDayStart;
            }
        }

        private void CoerceVisibleWeek()
        {
            if (this.VisibleWeekStart < DayOfWeek.Sunday)
            {
                this.VisibleWeekStart = DayOfWeek.Sunday;
            }
            if (this.VisibleWeekStart > DayOfWeek.Saturday)
            {
                this.VisibleWeekStart = DayOfWeek.Saturday;
            }
            if (this.VisibleWeekFinish < DayOfWeek.Sunday)
            {
                this.VisibleWeekFinish = DayOfWeek.Sunday;
            }
            if (this.VisibleWeekFinish > DayOfWeek.Saturday)
            {
                this.VisibleWeekFinish = DayOfWeek.Saturday;
            }
            if (this.VisibleWeekFinish <= this.VisibleWeekStart)
            {
                this.VisibleWeekFinish = this.VisibleWeekStart;
            }
        }

        private void CoerceWorkingDay()
        {
            if (this.WorkingDayFinish <= this.WorkingDayStart)
            {
                this.WorkingDayFinish = this.WorkingDayStart;
            }
        }

        private void CoerceWorkingWeek()
        {
            if (this.WorkingWeekFinish <= this.WorkingWeekStart)
            {
                this.WorkingWeekFinish = this.WorkingWeekStart;
            }
        }

        public void CollapseAll()
        {
            if (this.ganttChartView != null)
            {
                this.ganttChartView.CollapseAll();
            }
        }

        private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.dataTreeGridColumns != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        this.AddDataTreeGridColumns(e.NewStartingIndex, e.NewItems);
                        return;

                    case NotifyCollectionChangedAction.Remove:
                        this.RemoveDataTreeGridColumns(e.OldStartingIndex, e.OldItems.Count);
                        return;

                    case NotifyCollectionChangedAction.Replace:
                    case NotifyCollectionChangedAction.Reset:
                        this.ClearDataTreeGridColumns();
                        this.AddDataTreeGridColumns(0, this.Columns);
                        return;

                    case NotifyCollectionChangedAction.Move:
                        return;
                }
            }
        }

        public void ContinueAsyncPresentation()
        {
            if (this.isAsyncTimerPaused)
            {
                this.isAsyncTimerPaused = false;
                if (this.asyncTimer != null)
                {
                    this.asyncTimer.Start();
                }
            }
        }

        private void DataTreeGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            this.OnBeginningEdit(e);
        }

        private void DataTreeGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            this.OnCellEditEnding(e);
        }

        private void DataTreeGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (this.dataTreeGrid != null)
            {
                // Bug : ArgumentOutOfRanceException levé après une mise à jour du .NET Framework.
                // Workaround : présentant dans la version 4.3.3 du produit
                try
                {
                    base.CurrentCell = this.dataTreeGrid.CurrentCell;
                }
                catch
                {
                }

            }
            else
            {
                this.OnCurrentCellChanged(e);
            }
        }

        private void DataTreeGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            this.OnPreparingCellForEdit(e);
        }

        private void DataTreeGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            this.OnRowEditEnding(e);
        }

        private void DataTreeGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.dataTreeGrid != null)
            {
                if (base.SelectionMode != DataGridSelectionMode.Extended)
                {
                    base.SelectedItem = this.dataTreeGrid.SelectedItem;
                }
                else
                {
                    foreach (object obj2 in e.RemovedItems)
                    {
                        base.SelectedItems.Remove(obj2);
                    }
                    foreach (object obj3 in e.AddedItems)
                    {
                        base.SelectedItems.Add(obj3);
                    }
                }
            }
            else
            {
                this.OnSelectionChanged(e);
            }
        }

        private void DataTreeGridScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged)
            {
                this.UpdateGanttChartViewVerticalScrollBarFromDataTreeGrid();
            }
        }

        private void DataTreeGridVerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.UpdateGanttChartViewVerticalScrollBarFromDataTreeGrid();
        }

        public void DecreaseTimelinePage()
        {
            DateTime timelinePageStart = this.TimelinePageStart;
            DateTime timelinePageFinish = this.TimelinePageFinish;
            timelinePageStart -= this.UpdateTimelinePageAmount;
            timelinePageFinish -= this.UpdateTimelinePageAmount;
            this.SetTimelinePage(timelinePageStart, timelinePageFinish);
        }

        public void ExpandAll()
        {
            if (this.ganttChartView != null)
            {
                this.ganttChartView.ExpandAll();
            }
        }

        public void Export(Delegate action)
        {
            this.PrepareExport();
            base.Dispatcher.BeginInvoke(action, new object[0]);
        }

        private void GanttChartView_ActualDisplayRowCountChanged(object sender, EventArgs e)
        {
            this.UpdateGanttChartViewVerticalScrollBarFromDataTreeGrid();
        }

        private void GanttChartView_DecreaseTimelinePageProvider(object sender, EventArgs e)
        {
            this.DecreaseTimelinePage();
        }

        private void GanttChartView_DisplayedTimeChanged(object sender, EventArgs e)
        {
            this.DisplayedTime = this.ganttChartView.DisplayedTime;
        }

        private void GanttChartView_IncreaseTimelinePageProvider(object sender, EventArgs e)
        {
            this.IncreaseTimelinePage();
        }

        private void GanttChartView_ItemActivated(object sender, ItemActivatedEventArgs e)
        {
            this.OnItemActivated(e.Item);
        }

        private void GanttChartViewVerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if (!this.isDuringUpdateGanttChartViewVerticalScrollBarFromDataTree)
            //{
                this.UpdateDataTreeGridVerticalScrollBarFromGanttChartView();
            //}
        }

        public IEnumerable<GanttChartItem> GetAllChildren(GanttChartItem item)
        {
            if (this.ganttChartView == null)
            {
                return null;
            }
            return this.ganttChartView.GetAllChildren(item);
        }

        public IEnumerable<GanttChartItem> GetAllChildren(int index)
        {
            if (this.ganttChartView == null)
            {
                return null;
            }
            return this.ganttChartView.GetAllChildren(index);
        }

        public IEnumerable<GanttChartItem> GetAllParents(GanttChartItem item)
        {
            if (this.ganttChartView == null)
            {
                return null;
            }
            return this.ganttChartView.GetAllParents(item);
        }

        public IEnumerable<GanttChartItem> GetAllParents(int index)
        {
            if (this.ganttChartView == null)
            {
                return null;
            }
            return this.ganttChartView.GetAllParents(index);
        }

        public IEnumerable<GanttChartItem> GetChildren(GanttChartItem item)
        {
            if (this.ganttChartView == null)
            {
                return null;
            }
            return this.ganttChartView.GetChildren(item);
        }

        public IEnumerable<GanttChartItem> GetChildren(int index)
        {
            if (this.ganttChartView == null)
            {
                return null;
            }
            return this.ganttChartView.GetChildren(index);
        }

        internal virtual double GetContentHeight()
        {
            if (this.ganttChartView == null)
            {
                return 0.0;
            }
            return (this.ItemHeight * this.ganttChartView.ActualDisplayRowCount);
        }

        internal DrawingVisual GetDataGridDrawingVisual(Size pageSize, int i, int j)
        {
            double headerHeight = this.HeaderHeight;
            double contentHeight = this.GetContentHeight();
            double dataGridWidth = this.GetDataGridWidth();
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                context.DrawRectangle(new SolidColorBrush(Colors.White), null, new Rect(0.0, 0.0, pageSize.Width, pageSize.Height));
                context.PushClip(new RectangleGeometry(new Rect(0.0, 0.0, pageSize.Width, headerHeight)));
                VisualBrush brush = new VisualBrush(this.GetDataHeaderPanel())
                {
                    Stretch = Stretch.None,
                    AlignmentX = AlignmentX.Left
                };
                context.DrawRectangle(brush, null, new Rect(-(j * pageSize.Width), 0.0, dataGridWidth, headerHeight));
                context.Pop();
                context.PushClip(new RectangleGeometry(new Rect(0.0, headerHeight, pageSize.Width, pageSize.Height - headerHeight)));
                VisualBrush brush2 = new VisualBrush(this.GetDataRowsPanel())
                {
                    Stretch = Stretch.None,
                    AlignmentX = AlignmentX.Left,
                    AlignmentY = AlignmentY.Top
                };
                context.DrawRectangle(brush2, null, new Rect(-(j * pageSize.Width), -(i * (pageSize.Height - headerHeight)) + headerHeight, dataGridWidth, contentHeight));
                context.Pop();
            }
            return visual;
        }

        private Panel GetDataGridPanel()
        {
            if (this.dataTreeGrid == null)
            {
                return null;
            }
            StackPanel panel = new StackPanel();
            Panel dataHeaderPanel = this.GetDataHeaderPanel();
            Panel dataRowsPanel = this.GetDataRowsPanel();
            double headerHeight = this.HeaderHeight;
            double contentHeight = this.GetContentHeight();
            dataHeaderPanel.Height = headerHeight;
            dataRowsPanel.Height = contentHeight;
            panel.Children.Add(dataHeaderPanel);
            panel.Children.Add(dataRowsPanel);
            Size availableSize = new Size(this.GetDataGridWidth(), headerHeight + contentHeight);
            panel.Measure(availableSize);
            panel.Arrange(new Rect(new Point(0.0, 0.0), availableSize));
            return panel;
        }

        internal double GetDataGridWidth()
        {
            if (this.dataTreeGrid == null)
            {
                return double.NaN;
            }
            double num = 0.0;
            foreach (DataGridColumn column in this.dataTreeGrid.Columns)
            {
                double num2 = Math.Max((double)0.0, (double)(column.ActualWidth - ((column is DataTreeGridColumn) ? ((double)12) : ((double)0))));
                num += num2;
            }
            return num;
        }

        private Panel GetDataHeaderPanel()
        {
            if (this.dataHeaderPanel != null)
            {
                return this.dataHeaderPanel;
            }
            if (this.dataTreeGrid == null)
            {
                return null;
            }
            Grid grid = new Grid();
            foreach (DataGridColumn column in this.dataTreeGrid.Columns)
            {
                double pixels = Math.Max((double)0.0, (double)(column.ActualWidth - ((column is DataTreeGridColumn) ? ((double)12) : ((double)0))));
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(pixels) });
                FrameworkElement element = new TextBlock
                {
                    Text = (column.Header != null) ? column.Header.ToString() : null,
                    TextAlignment = TextAlignment.Left,
                    TextWrapping = TextWrapping.Wrap,
                    TextTrimming = TextTrimming.WordEllipsis,
                    Padding = new Thickness(1.0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };
                FrameworkElement element2 = new Border
                {
                    Child = element,
                    BorderBrush = new SolidColorBrush(Colors.LightGray),
                    BorderThickness = new Thickness(0.0, 0.0, 1.0, 1.0),
                    Background = (this.ganttChartView != null) ? this.ganttChartView.HeaderBackground : null
                };
                Grid.SetColumn(element2, column.DisplayIndex);
                grid.Children.Add(element2);
            }
            Size availableSize = new Size(this.GetDataGridWidth(), this.HeaderHeight);
            grid.Measure(availableSize);
            grid.Arrange(new Rect(new Point(0.0, 0.0), availableSize));
            return grid;
        }

        internal virtual Panel GetDataRowsActualPanel()
        {
            if ((this.DataTreeGrid == null) || (this.ManagedItems == null))
            {
                return null;
            }
            Grid grid = new Grid();
            foreach (DataGridColumn column in this.DataTreeGrid.Columns)
            {
                double pixels = Math.Max((double)0.0, (double)(column.ActualWidth - ((column is DataTreeGridColumn) ? ((double)12) : ((double)0))));
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(pixels) });
            }
            int num2 = 0;
            foreach (DataTreeGridItem item in this.ManagedItems)
            {
                if (item.IsVisible)
                {
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(this.ItemHeight) });
                    foreach (DataGridColumn column2 in this.DataTreeGrid.Columns)
                    {
                        FrameworkElement element;
                        Math.Max((double)0.0, (double)(column2.ActualWidth - ((column2 is DataTreeGridColumn) ? ((double)12) : ((double)0))));
                        if (column2 is DataTreeGridColumn)
                        {
                            element = new TextBlock
                            {
                                Text = (item.Content != null) ? item.Content.ToString() : null,
                                TextAlignment = TextAlignment.Left,
                                TextWrapping = TextWrapping.Wrap,
                                TextTrimming = TextTrimming.WordEllipsis,
                                Padding = new Thickness(1.0 + (item.Indentation * this.DataTreeGrid.IndentationUnitSize), 1.0, 1.0, 1.0),
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                        }
                        else if (column2 is DataGridBoundColumn)
                        {
                            element = new TextBlock
                            {
                                DataContext = item,
                                TextAlignment = TextAlignment.Left,
                                TextWrapping = TextWrapping.Wrap,
                                TextTrimming = TextTrimming.WordEllipsis,
                                Padding = new Thickness(1.0),
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                            element.SetBinding(TextBlock.TextProperty, (column2 as DataGridBoundColumn).Binding);
                        }
                        else
                        {
                            FrameworkElement cellContent = column2.GetCellContent(item);
                            if (cellContent != null)
                            {
                                DrawingVisual visual = new DrawingVisual();
                                using (DrawingContext context = visual.RenderOpen())
                                {
                                    context.DrawRectangle(new SolidColorBrush(Colors.White), null, new Rect(0.0, 0.0, cellContent.ActualWidth, cellContent.ActualHeight));
                                    VisualBrush brush = new VisualBrush(cellContent)
                                    {
                                        Stretch = Stretch.None,
                                        AlignmentX = AlignmentX.Left,
                                        AlignmentY = AlignmentY.Top
                                    };
                                    context.DrawRectangle(brush, null, new Rect(0.0, 0.0, cellContent.ActualWidth, cellContent.ActualHeight));
                                }
                                RenderTargetBitmap bitmap = new RenderTargetBitmap((int)Math.Ceiling(cellContent.ActualWidth), (int)Math.Ceiling(cellContent.ActualHeight), 96.0, 96.0, PixelFormats.Default);
                                bitmap.Render(visual);
                                BitmapSource source = bitmap;
                                element = new Image
                                {
                                    Source = source
                                };
                            }
                            else
                            {
                                element = new TextBlock
                                {
                                    Text = "…",
                                    TextAlignment = TextAlignment.Left,
                                    TextWrapping = TextWrapping.Wrap,
                                    Padding = new Thickness(1.0),
                                    HorizontalAlignment = HorizontalAlignment.Left,
                                    VerticalAlignment = VerticalAlignment.Center
                                };
                            }
                        }
                        FrameworkElement element3 = new Border
                        {
                            Child = element,
                            BorderBrush = new SolidColorBrush(Colors.LightGray),
                            BorderThickness = new Thickness(0.0, 0.0, 1.0, 1.0)
                        };
                        Grid.SetColumn(element3, column2.DisplayIndex);
                        Grid.SetRow(element3, num2);
                        grid.Children.Add(element3);
                    }
                    num2++;
                }
            }
            Size availableSize = new Size(this.GetDataGridWidth(), this.GetContentHeight());
            grid.Measure(availableSize);
            grid.Arrange(new Rect(new Point(0.0, 0.0), availableSize));
            return grid;
        }

        private Panel GetDataRowsPanel()
        {
            if (this.dataRowsPanel != null)
            {
                return this.dataRowsPanel;
            }
            return this.GetDataRowsActualPanel();
        }

        public DateTime GetDateTime(double position)
        {
            if (this.ganttChartView == null)
            {
                return DateTime.MinValue;
            }
            return this.ganttChartView.GetDateTime(position);
        }

        public TimeSpan GetEffort(DateTime start, DateTime finish)
        {
            if (this.ganttChartView == null)
            {
                return TimeSpan.Zero;
            }
            return this.ganttChartView.GetEffort(start, finish);
        }

        public BitmapSource GetExportBitmapSource()
        {
            if ((this.dataTreeGrid == null) || (this.ganttChartView == null))
            {
                return null;
            }
            Size exportSize = this.GetExportSize();
            this.GetDataGridWidth();
            DrawingVisual exportDrawingVisual = this.GetExportDrawingVisual();
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)Math.Ceiling(exportSize.Width), (int)Math.Ceiling(exportSize.Height), 96.0, 96.0, PixelFormats.Default);
            bitmap.Render(exportDrawingVisual);
            return bitmap;
        }

        internal BitmapSource GetExportBitmapSource(Size pageSize, int i, int j)
        {
            if ((this.dataTreeGrid == null) || (this.ganttChartView == null))
            {
                return null;
            }
            int exportDataGridHorizontalLength = this.GetExportDataGridHorizontalLength(pageSize);
            if (j < exportDataGridHorizontalLength)
            {
                DrawingVisual visual = this.GetDataGridDrawingVisual(pageSize, i, j);
                RenderTargetBitmap bitmap = new RenderTargetBitmap((int)Math.Ceiling(pageSize.Width), (int)Math.Ceiling(pageSize.Height), 96.0, 96.0, PixelFormats.Default);
                bitmap.Render(visual);
                return bitmap;
            }
            return this.ganttChartView.GetExportBitmapSource(pageSize, i, j - exportDataGridHorizontalLength);
        }

        public BitmapSource[,] GetExportBitmapSources(Size pageSize)
        {
            this.PrepareDataGridPanels();
            int exportVerticalLength = this.GetExportVerticalLength(pageSize);
            int exportHorizontalLength = this.GetExportHorizontalLength(pageSize);
            BitmapSource[,] sourceArray = new BitmapSource[exportVerticalLength, exportHorizontalLength];
            for (int i = 0; i < exportVerticalLength; i++)
            {
                for (int j = 0; j < exportHorizontalLength; j++)
                {
                    sourceArray[i, j] = this.GetExportBitmapSource(pageSize, i, j);
                }
            }
            this.ClearDataGridPanels();
            return sourceArray;
        }

        private int GetExportDataGridHorizontalLength(Size pageSize)
        {
            return (int)Math.Ceiling((double)(this.GetDataGridWidth() / pageSize.Width));
        }

        internal DrawingVisual GetExportDrawingVisual()
        {
            if ((this.dataTreeGrid == null) || (this.ganttChartView == null))
            {
                return null;
            }
            Size exportSize = this.GetExportSize();
            double dataGridWidth = this.GetDataGridWidth();
            DrawingVisual visual = new DrawingVisual();
            Size pageSize = new Size(dataGridWidth, exportSize.Height);
            Size size3 = new Size(exportSize.Width - dataGridWidth, exportSize.Height);
            DrawingVisual visual2 = this.GetDataGridDrawingVisual(pageSize, 0, 0);
            DrawingVisual visual3 = this.ganttChartView.GetExportDrawingVisual(size3, 0, 0);
            visual2.Clip = new RectangleGeometry(new Rect(pageSize));
            visual2.Transform = new TranslateTransform(0.0, 0.0);
            visual.Children.Add(visual2);
            visual3.Clip = new RectangleGeometry(new Rect(size3));
            visual3.Transform = new TranslateTransform(dataGridWidth, 0.0);
            visual.Children.Add(visual3);
            using (DrawingContext context = visual.RenderOpen())
            {
                context.DrawRectangle(null, null, new Rect(0.0, 0.0, exportSize.Width, exportSize.Height));
                context.DrawRectangle(null, null, new Rect(0.0, 0.0, exportSize.Width, exportSize.Height));
            }
            return visual;
        }

        internal DrawingVisual GetExportDrawingVisual(Size pageSize, int i, int j)
        {
            if ((this.dataTreeGrid == null) || (this.ganttChartView == null))
            {
                return null;
            }
            int exportDataGridHorizontalLength = this.GetExportDataGridHorizontalLength(pageSize);
            if (j < exportDataGridHorizontalLength)
            {
                return this.GetDataGridDrawingVisual(pageSize, i, j);
            }
            return this.ganttChartView.GetExportDrawingVisual(pageSize, i, j - exportDataGridHorizontalLength);
        }

        private int GetExportHorizontalLength(Size pageSize)
        {
            if ((this.dataTreeGrid == null) || (this.ganttChartView == null))
            {
                return 0;
            }
            Size exportSize = this.ganttChartView.GetExportSize();
            return (this.GetExportDataGridHorizontalLength(pageSize) + ((int)Math.Ceiling((double)(exportSize.Width / pageSize.Width))));
        }

        public Size GetExportSize()
        {
            if ((this.dataTreeGrid == null) || (this.ganttChartView == null))
            {
                return Size.Empty;
            }
            Size exportSize = this.ganttChartView.GetExportSize();
            return new Size(this.GetDataGridWidth() + exportSize.Width, this.HeaderHeight + this.GetContentHeight());
        }

        private int GetExportVerticalLength(Size pageSize)
        {
            Size exportSize = this.GetExportSize();
            return (int)Math.Ceiling((double)(Math.Max((double)0.0, (double)(exportSize.Height - this.HeaderHeight)) / Math.Max((double)0.0, (double)(pageSize.Height - this.HeaderHeight))));
        }

        public DateTime GetFinish(DateTime start, TimeSpan effort)
        {
            if (this.ganttChartView == null)
            {
                return DateTime.MinValue;
            }
            return this.ganttChartView.GetFinish(start, effort);
        }

        public GanttChartItem GetItemAt(double position)
        {
            if (this.ganttChartView == null)
            {
                return null;
            }
            return this.ganttChartView.GetItemAt(position);
        }

        public int GetItemIndexAt(double position)
        {
            if (this.ganttChartView == null)
            {
                return -1;
            }
            return this.ganttChartView.GetItemIndexAt(position);
        }

        public double GetItemTop(GanttChartItem item)
        {
            return item.ComputedItemTop;
        }

        public double GetItemTop(int index)
        {
            if (this.ganttChartView == null)
            {
                return double.NaN;
            }
            return this.ganttChartView.GetItemTop(index);
        }

        public DateTime GetNextNonworkingTime(DateTime dateTime)
        {
            if (this.ganttChartView == null)
            {
                return DateTime.MinValue;
            }
            return this.ganttChartView.GetNextNonworkingTime(dateTime);
        }

        public DateTime GetNextVisibleNonworkingTime(DateTime dateTime)
        {
            if (this.ganttChartView == null)
            {
                return DateTime.MinValue;
            }
            return this.ganttChartView.GetNextVisibleNonworkingTime(dateTime);
        }

        public DateTime GetNextVisibleTime(DateTime dateTime)
        {
            if (this.ganttChartView == null)
            {
                return DateTime.MinValue;
            }
            return this.ganttChartView.GetNextVisibleTime(dateTime);
        }

        public DateTime GetNextVisibleWorkingTime(DateTime dateTime)
        {
            if (this.ganttChartView == null)
            {
                return DateTime.MinValue;
            }
            return this.ganttChartView.GetNextVisibleWorkingTime(dateTime);
        }

        public DateTime GetNextWorkingTime(DateTime dateTime)
        {
            if (this.ganttChartView == null)
            {
                return DateTime.MinValue;
            }
            return this.ganttChartView.GetNextWorkingTime(dateTime);
        }

        public GanttChartItem GetParent(GanttChartItem item)
        {
            if (this.ganttChartView == null)
            {
                return null;
            }
            return this.ganttChartView.GetParent(item);
        }

        public GanttChartItem GetParent(int index)
        {
            if (this.ganttChartView == null)
            {
                return null;
            }
            return this.ganttChartView.GetParent(index);
        }

        public double GetPosition(DateTime dateTime)
        {
            if (this.ganttChartView == null)
            {
                return double.NaN;
            }
            return this.ganttChartView.GetPosition(dateTime);
        }

        public IEnumerable<GanttChartItem> GetPredecessors(GanttChartItem item)
        {
            if (this.ganttChartView == null)
            {
                return null;
            }
            return this.ganttChartView.GetPredecessors(item);
        }

        public DateTime GetPreviousNonworkingTime(DateTime dateTime)
        {
            if (this.ganttChartView == null)
            {
                return DateTime.MaxValue;
            }
            return this.ganttChartView.GetPreviousNonworkingTime(dateTime);
        }

        public DateTime GetPreviousVisibleNonworkingTime(DateTime dateTime)
        {
            if (this.ganttChartView == null)
            {
                return DateTime.MaxValue;
            }
            return this.ganttChartView.GetPreviousVisibleNonworkingTime(dateTime);
        }

        public DateTime GetPreviousVisibleTime(DateTime dateTime)
        {
            if (this.ganttChartView == null)
            {
                return DateTime.MaxValue;
            }
            return this.ganttChartView.GetPreviousVisibleTime(dateTime);
        }

        public DateTime GetPreviousVisibleWorkingTime(DateTime dateTime)
        {
            if (this.ganttChartView == null)
            {
                return DateTime.MaxValue;
            }
            return this.ganttChartView.GetPreviousVisibleWorkingTime(dateTime);
        }

        public DateTime GetPreviousWorkingTime(DateTime dateTime)
        {
            if (this.ganttChartView == null)
            {
                return DateTime.MaxValue;
            }
            return this.ganttChartView.GetPreviousWorkingTime(dateTime);
        }

        public DateTime GetStart(TimeSpan effort, DateTime finish)
        {
            if (this.ganttChartView == null)
            {
                return DateTime.MaxValue;
            }
            return this.ganttChartView.GetStart(effort, finish);
        }

        public IEnumerable<PredecessorItem> GetSuccessorPredecessorItems(GanttChartItem item)
        {
            if (this.ganttChartView == null)
            {
                return null;
            }
            return this.ganttChartView.GetSuccessorPredecessorItems(item);
        }

        public IEnumerable<GanttChartItem> GetSuccessors(GanttChartItem item)
        {
            if (this.ganttChartView == null)
            {
                return null;
            }
            return this.ganttChartView.GetSuccessors(item);
        }

        public DateTime GetUpdateScaleTime(DateTime dateTime)
        {
            if (this.ganttChartView == null)
            {
                return DateTime.MinValue;
            }
            return this.ganttChartView.GetUpdateScaleTime(dateTime);
        }

        public IEnumerable<TimeInterval> GetWorkingTimeIntervals(DateTime start, DateTime finish)
        {
            if (this.ganttChartView == null)
            {
                return null;
            }
            return this.ganttChartView.GetWorkingTimeIntervals(start, finish);
        }

        public void IncreaseTimelinePage()
        {
            DateTime timelinePageStart = this.TimelinePageStart;
            DateTime timelinePageFinish = this.TimelinePageFinish;
            timelinePageStart += this.UpdateTimelinePageAmount;
            timelinePageFinish += this.UpdateTimelinePageAmount;
            this.SetTimelinePage(timelinePageStart, timelinePageFinish);
        }

        public int IndexOf(GanttChartItem item)
        {
            if (this.ganttChartView == null)
            {
                return -1;
            }
            return this.ganttChartView.IndexOf(item);
        }

        private void InitializeDataTreeGridSettings()
        {
            if (this.dataTreeGrid != null)
            {
                this.dataTreeGrid.HeadersVisibility = DataGridHeadersVisibility.Column;
                this.dataTreeGrid.CanUserAddRows = false;
                this.dataTreeGrid.CanUserDeleteRows = false;
                this.dataTreeGrid.CanUserResizeRows = false;
                BindingOperations.SetBinding(this.dataTreeGrid, DataGrid.IsReadOnlyProperty, new Binding("IsReadOnly") { Source = this });
                BindingOperations.SetBinding(this.dataTreeGrid, DataGrid.CurrentCellProperty, new Binding("CurrentCell") { Source = this });
                BindingOperations.SetBinding(this.dataTreeGrid, Selector.SelectedIndexProperty, new Binding("SelectedIndex") { Source = this });
                BindingOperations.SetBinding(this.dataTreeGrid, Selector.SelectedItemProperty, new Binding("SelectedItem") { Source = this });
                BindingOperations.SetBinding(this.dataTreeGrid, Selector.SelectedValueProperty, new Binding("SelectedValue") { Source = this });
            }
        }

        private void InitializeGanttChartViewSettings()
        {
            if (this.ganttChartView != null)
            {
                this.ganttChartView.IsScheduleCachingEnabled = this.IsScheduleCachingEnabled;
                BindingOperations.SetBinding(this.ganttChartView, DlhSoft.Windows.Controls.GanttChartView.IsReadOnlyProperty, new Binding("IsReadOnly") { Source = this });
            }
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnItemCollectionChanged(sender, e);
        }

        private void Items_ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnItemPropertyChanged(sender, e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (!this.isTemplateApplied)
            {
                LicenseValidator.Validate(this, typeof(GanttChartDataGrid), Assembly.GetCallingAssembly());
            }
            if (this.NonworkingIntervals == null)
            {
                this.NonworkingIntervals = new ObservableCollection<TimeInterval>();
            }
            if (this.Scales == null)
            {
                this.Scales = new ScaleCollection();
            }
            this.dataTreeGrid = base.GetTemplateChild("DataTreeGrid") as DlhSoft.Windows.Controls.DataTreeGrid;
            if (this.dataTreeGrid != null)
            {
                this.InitializeDataTreeGridSettings();
                this.dataTreeGrid.SelectionChanged += new SelectionChangedEventHandler(this.DataTreeGrid_SelectionChanged);
                this.dataTreeGrid.CurrentCellChanged += new EventHandler<EventArgs>(this.DataTreeGrid_CurrentCellChanged);
                this.dataTreeGrid.PreparingCellForEdit += new EventHandler<DataGridPreparingCellForEditEventArgs>(this.DataTreeGrid_PreparingCellForEdit);
                this.dataTreeGrid.BeginningEdit += new EventHandler<DataGridBeginningEditEventArgs>(this.DataTreeGrid_BeginningEdit);
                this.dataTreeGrid.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(this.DataTreeGrid_CellEditEnding);
                this.dataTreeGrid.RowEditEnding += new EventHandler<DataGridRowEditEndingEventArgs>(this.DataTreeGrid_RowEditEnding);
                this.dataTreeGrid.ApplyTemplate();
                FrameworkElement element = (VisualTreeHelper.GetChildrenCount(this.dataTreeGrid) == 1) ? (VisualTreeHelper.GetChild(this.dataTreeGrid, 0) as FrameworkElement) : null;
                if (element != null)
                {
                    this.dataTreeGridVerticalScrollBar = element.FindName("VerticalScrollbar") as ScrollBar;
                    if (this.dataTreeGridVerticalScrollBar == null)
                    {
                        this.dataTreeGridScrollViewer = element.FindName("DG_ScrollViewer") as ScrollViewer;
                        if (this.dataTreeGridScrollViewer != null)
                        {
                            this.dataTreeGridScrollViewer.CanContentScroll = false;
                            this.dataTreeGridScrollViewer.SizeChanged += new SizeChangedEventHandler(this.DataTreeGridScrollViewer_SizeChanged);
                            this.dataTreeGridScrollViewer.ApplyTemplate();
                            FrameworkElement element2 = (VisualTreeHelper.GetChildrenCount(this.dataTreeGridScrollViewer) == 1) ? (VisualTreeHelper.GetChild(this.dataTreeGridScrollViewer, 0) as FrameworkElement) : null;
                            if (element2 != null)
                            {
                                this.dataTreeGridVerticalScrollBar = element2.FindName("PART_VerticalScrollBar") as ScrollBar;
                            }
                        }
                    }
                    if (this.dataTreeGridVerticalScrollBar != null)
                    {
                        this.dataTreeGridVerticalScrollBar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.DataTreeGridVerticalScrollBar_ValueChanged);
                    }
                }
            }
            this.ganttChartView = base.GetTemplateChild("GanttChartView") as DlhSoft.Windows.Controls.GanttChartView;
            if (this.ganttChartView != null)
            {
                this.ganttChartView.ParentDataGrid = this;
                this.InitializeGanttChartViewSettings();
                this.ganttChartView.ActualDisplayRowCountChanged += new EventHandler(this.GanttChartView_ActualDisplayRowCountChanged);
                this.ganttChartView.DisplayedTimeChanged += new EventHandler(this.GanttChartView_DisplayedTimeChanged);
                this.ganttChartView.ApplyTemplate();
                FrameworkElement element3 = (VisualTreeHelper.GetChildrenCount(this.ganttChartView) == 1) ? (VisualTreeHelper.GetChild(this.ganttChartView, 0) as FrameworkElement) : null;
                if (element3 != null)
                {
                    this.ganttChartViewScrollViewer = element3.FindName("ScrollViewer") as ScrollViewer;
                    if (this.ganttChartViewScrollViewer != null)
                    {
                        this.ganttChartViewScrollViewer.ApplyTemplate();
                        FrameworkElement element4 = (VisualTreeHelper.GetChildrenCount(this.ganttChartViewScrollViewer) == 1) ? (VisualTreeHelper.GetChild(this.ganttChartViewScrollViewer, 0) as FrameworkElement) : null;
                        if (element4 != null)
                        {
                            this.ganttChartViewVerticalScrollBar = element4.FindName("VerticalScrollBar") as ScrollBar;
                            if (this.ganttChartViewVerticalScrollBar == null)
                            {
                                this.ganttChartViewVerticalScrollBar = element4.FindName("PART_VerticalScrollBar") as ScrollBar;
                            }
                            if (this.ganttChartViewVerticalScrollBar != null)
                            {
                                this.ganttChartViewVerticalScrollBar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.GanttChartViewVerticalScrollBar_ValueChanged);
                                this.ganttChartViewVerticalScrollBar.Scroll += new ScrollEventHandler(ganttChartViewVerticalScrollBar_Scroll);
                            }
                        }
                    }
                }
            }
            bool isTemplateApplied = this.isTemplateApplied;
            this.isTemplateApplied = true;
            this.UpdateChartDisplayedTime();
            if (!isTemplateApplied)
            {
                this.UpdateItemsSource();
                this.UpdateColumns();
                this.UpdateItemsSources();
            }
            if (this.ganttChartView != null)
            {
                this.ganttChartView.ItemActivated += new ItemActivatedEventHandler(this.GanttChartView_ItemActivated);
                this.ganttChartView.IncreaseTimelinePageProvider += new EventHandler(this.GanttChartView_IncreaseTimelinePageProvider);
                this.ganttChartView.DecreaseTimelinePageProvider += new EventHandler(this.GanttChartView_DecreaseTimelinePageProvider);
            }
        }

        void ganttChartViewVerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            //e.Handled = false;

        }

        private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartDataGrid grid = d as GanttChartDataGrid;
            if (grid != null)
            {
                grid.UpdateColumns();
            }
        }

        private static void OnDisplayedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartDataGrid grid = d as GanttChartDataGrid;
            if (grid != null)
            {
                grid.UpdateChartDisplayedTime();
            }
        }

        private static void OnIsAsyncPresentationEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartDataGrid grid = d as GanttChartDataGrid;
            if (grid != null)
            {
                grid.UpdateItemsSource();
            }
        }

        protected virtual void OnItemActivated(GanttChartItem item)
        {
            if (this.ItemActivated != null)
            {
                this.ItemActivated(this, new ItemActivatedEventArgs(item));
            }
        }

        protected virtual void OnItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.ItemCollectionChanged != null)
            {
                this.ItemCollectionChanged(sender, e);
            }
        }

        protected virtual void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.ItemPropertyChanged != null)
            {
                this.ItemPropertyChanged(sender, e);
            }
        }

        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartDataGrid grid = d as GanttChartDataGrid;
            if (grid != null)
            {
                grid.UpdateItemsSource();
            }
        }

        protected virtual void OnTimelinePageChanged()
        {
            if (this.TimelinePageChanged != null)
            {
                this.TimelinePageChanged(this, EventArgs.Empty);
            }
        }

        private static void OnTimelinePageFinishChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartDataGrid grid = d as GanttChartDataGrid;
            if (grid != null)
            {
                grid.CoerceTimelinePage();
                grid.OnTimelinePageChanged();
            }
        }

        private static void OnTimelinePageStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartDataGrid grid = d as GanttChartDataGrid;
            if (grid != null)
            {
                grid.CoerceTimelinePage();
                grid.OnTimelinePageChanged();
            }
        }

        private static void OnVisibleDayFinishChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartDataGrid grid = d as GanttChartDataGrid;
            if (grid != null)
            {
                grid.CoerceVisibleDay();
            }
        }

        private static void OnVisibleDayStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartDataGrid grid = d as GanttChartDataGrid;
            if (grid != null)
            {
                grid.CoerceVisibleDay();
            }
        }

        private static void OnVisibleWeekFinishChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartDataGrid grid = d as GanttChartDataGrid;
            if (grid != null)
            {
                grid.CoerceVisibleWeek();
            }
        }

        private static void OnVisibleWeekStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartDataGrid grid = d as GanttChartDataGrid;
            if (grid != null)
            {
                grid.CoerceVisibleWeek();
            }
        }

        private static void OnWorkingDayFinishChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartDataGrid grid = d as GanttChartDataGrid;
            if (grid != null)
            {
                grid.CoerceWorkingDay();
            }
        }

        private static void OnWorkingDayStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartDataGrid grid = d as GanttChartDataGrid;
            if (grid != null)
            {
                grid.CoerceWorkingDay();
            }
        }

        private static void OnWorkingWeekFinishChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartDataGrid grid = d as GanttChartDataGrid;
            if (grid != null)
            {
                grid.CoerceWorkingWeek();
            }
        }

        private static void OnWorkingWeekStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartDataGrid grid = d as GanttChartDataGrid;
            if (grid != null)
            {
                grid.CoerceWorkingWeek();
            }
        }

        private void OriginalItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (GanttChartItem item in e.NewItems)
                    {
                        this.AttachItem(item);
                    }
                    if (this.IsAsyncPresentationEnabled)
                    {
                        if (e.NewStartingIndex < this.asyncItemCount)
                        {
                            for (int j = 0; j < e.NewItems.Count; j++)
                            {
                                this.items.Insert(e.NewStartingIndex + j, e.NewItems[j] as GanttChartItem);
                            }
                            this.asyncItemCount += e.NewItems.Count;
                            return;
                        }
                        if (this.asyncTimer == null)
                        {
                            break;
                        }
                        this.asyncTimer.Start();
                        return;
                    }
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        this.items.Insert(e.NewStartingIndex + i, e.NewItems[i] as GanttChartItem);
                    }
                    return;

                case NotifyCollectionChangedAction.Remove:
                    {
                        if (this.IsAsyncPresentationEnabled)
                        {
                            if (e.OldStartingIndex < this.asyncItemCount)
                            {
                                int num4 = Math.Min(e.OldItems.Count, this.items.Count - e.OldStartingIndex);
                                int num5 = num4;
                                while (num5-- > 0)
                                {
                                    this.items.RemoveAt(e.OldStartingIndex + num5);
                                }
                                this.asyncItemCount -= num4;
                                return;
                            }
                            break;
                        }
                        int count = e.OldItems.Count;
                        while (count-- > 0)
                        {
                            this.items.RemoveAt(e.OldStartingIndex + count);
                        }
                        return;
                    }
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    this.UpdateItemsSource();
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                default:
                    return;
            }
        }

        public void PauseAsyncPresentation()
        {
            if ((this.asyncTimer != null) && this.asyncTimer.IsEnabled)
            {
                this.asyncTimer.Stop();
                this.UpdateDataTreeGridVerticalScrollBarFromGanttChartView();
                this.isAsyncTimerPaused = true;
            }
        }

        private void PrepareDataGridPanels()
        {
            this.dataHeaderPanel = this.GetDataHeaderPanel();
            this.dataRowsPanel = this.GetDataRowsPanel();
        }

        private void PrepareExport()
        {
            if (this.IsVirtualizing && (this.ManagedItems != null))
            {
                foreach (GanttChartItem item in this.ManagedItems)
                {
                    if (item.IsVisible && !item.IsVirtuallyVisible)
                    {
                        item.IsVirtuallyVisible = true;
                    }
                }
            }
        }

        public void Print()
        {
            this.Print(string.Format("{0} Document", base.GetType()));
        }

        public void Print(string documentName)
        {
            this.PrepareExport();
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
            {
                DocumentPaginator documentPaginator = new DocumentPaginator(this)
                {
                    PageSize = new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight)
                };
                dialog.PrintDocument(documentPaginator, documentName);
            }
        }

        private void RemoveDataTreeGridColumns(int index, int count)
        {
            int num = index + count;
            while (num-- > index)
            {
                this.dataTreeGridColumns.RemoveAt(num);
            }
        }

        public void ScrollTo(GanttChartItem item)
        {
            if (this.dataTreeGrid != null)
                this.dataTreeGrid.ScrollIntoView(item);
        }

        public void ScrollTo(DateTime dateTime)
        {
            this.DisplayedTime = dateTime;
        }

        public void ScrollTo(int index)
        {
            if (this.dataTreeGrid != null && this.ManagedItems != null)
                this.dataTreeGrid.ScrollIntoView(this.ManagedItems[index]);
        }

        public void SetTimelinePage(DateTime start, DateTime finish)
        {
            this.TimelinePageStart = start;
            this.TimelinePageFinish = finish;
        }

        internal protected void UpdateChartDisplayedTime()
        {
            if (this.ganttChartView != null)
            {
                this.ganttChartView.DisplayedTime = this.DisplayedTime;
            }
        }

        private void UpdateColumns()
        {
            if (this.managedColumns != null)
            {
                this.managedColumns.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.Columns_CollectionChanged);
            }
            this.managedColumns = null;
            if (this.dataTreeGridColumns != null)
            {
                this.ClearDataTreeGridColumns();
            }
            else
            {
                this.dataTreeGridColumns = null;
                if (this.dataTreeGrid == null)
                {
                    return;
                }
                this.dataTreeGrid.Columns.Clear();
                this.dataTreeGridColumns = this.dataTreeGrid.Columns;
            }
            this.managedColumns = this.Columns;
            if ((this.dataTreeGridColumns != null) && (this.managedColumns != null))
            {
                this.AddDataTreeGridColumns(0, this.managedColumns);
                this.managedColumns.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Columns_CollectionChanged);
            }
        }

        public void UpdateDataTreeGridVerticalScrollBarFromGanttChartView()
        {
            bool isAsynchronousPresentationRunning = (this.asyncTimer != null) && this.asyncTimer.IsEnabled;
            if (isAsynchronousPresentationRunning)
            {
                this.asyncTimer.Stop();
            }
            //base.Dispatcher.BeginInvoke((Action)(() =>
            //{
            if ((((this.ganttChartViewScrollViewer != null) && (this.dataTreeGrid != null)) && ((this.ganttChartViewScrollViewer.ScrollableHeight > 0.0) && (this.dataTreeGridScrollViewer != null))) && ((this.dataTreeGridVerticalScrollBar != null) && (this.dataTreeGrid != null)))
            {
                double verticalOffset = this.ganttChartViewScrollViewer.VerticalOffset;

                if (verticalOffset > this.dataTreeGridScrollViewer.ExtentHeight - this.dataTreeGridScrollViewer.ViewportHeight)
                    ScrollToVerticalOffset(this.dataTreeGridScrollViewer.ExtentHeight - this.dataTreeGridScrollViewer.ViewportHeight);
                else
                    ScrollToVerticalOffset(verticalOffset);
                //double offset = 0.0;
                //foreach (DataTreeGridItem item in this.dataTreeGrid.ItemsSource)
                //{
                //    if (item.IsVisible)
                //    {
                //        verticalOffset -= this.ItemHeight;
                //        if (verticalOffset < 0.0)
                //        {
                //            break;
                //        }
                //    }
                //    offset++;
                //}
                //double maximum = this.dataTreeGridVerticalScrollBar.Maximum;
                //if (offset > maximum)
                //{
                //    offset = maximum;
                //}
                //this.dataTreeGridScrollViewer.ScrollToVerticalOffset(offset);
            }
            if (isAsynchronousPresentationRunning && (this.asyncTimer != null))
            {
                this.asyncTimer.Start();
            }
            //}));
        }

        private void ScrollToVerticalOffset(double offset)
        {
            if (offset < 0)
                offset = 0;
            if (this.dataTreeGridScrollViewer.VerticalOffset == offset)
                UpdateGanttChartViewVerticalScrollBarFromDataTreeGrid();
            else
                this.dataTreeGridScrollViewer.ScrollToVerticalOffset(offset);

        }

        private void UpdateGanttChartViewVerticalScrollBarFromDataTreeGrid()
        {
            if (((this.dataTreeGridVerticalScrollBar != null) && (this.ganttChartView != null)) && ((this.ganttChartViewScrollViewer != null) ))
            {
                UpdateGanttChartViewVerticalScrollBarFromDataTreeGridTimer_Tick(this, EventArgs.Empty);
                //if (this.updateGanttChartViewVerticalScrollBarFromDataTreeGridTimer == null)
                //{
                //    this.updateGanttChartViewVerticalScrollBarFromDataTreeGridTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(0.1) };
                //    this.updateGanttChartViewVerticalScrollBarFromDataTreeGridTimer.Tick += new EventHandler(this.UpdateGanttChartViewVerticalScrollBarFromDataTreeGridTimer_Tick);
                //}
                //if (!this.updateGanttChartViewVerticalScrollBarFromDataTreeGridTimer.IsEnabled)
                //{
                //    this.updateGanttChartViewVerticalScrollBarFromDataTreeGridTimer.Start();
                //}
            }
        }

        public void UpdateGanttChartViewVerticalScrollBarFromDataTreeGridTimer_Tick(object sender, EventArgs e)
        {
            //ThreadStart method = null;
            //this.updateGanttChartViewVerticalScrollBarFromDataTreeGridTimer.Stop();
            if (((this.dataTreeGridVerticalScrollBar != null) && (this.ganttChartView != null)) && ((this.ganttChartViewScrollViewer != null) && (this.dataTreeGrid != null)))
            {
                this.ganttChartViewScrollViewer.ScrollToVerticalOffset(this.dataTreeGridScrollViewer.VerticalOffset);
                //int count = (int) this.dataTreeGridVerticalScrollBar.Value;
                //int num2 = (this.dataTreeGrid.ItemsSource as IEnumerable<DataTreeGridItem>).Take<DataTreeGridItem>(count).Count<DataTreeGridItem>(i => i.IsVisible);
                //int actualDisplayRowCount = this.ganttChartView.ActualDisplayRowCount;
                //if (actualDisplayRowCount > 0)
                //{
                //    double num4 = ((this.ganttChartViewScrollViewer != null) && !ScrollViewer.GetCanContentScroll(this.ganttChartViewScrollViewer)) ? Math.IEEERemainder(this.ganttChartViewScrollViewer.ViewportHeight, this.ItemHeight) : 0.0;
                //    while (num4 < 0.0)
                //    {
                //        num4 = this.ItemHeight + num4;
                //    }
                //    double d = ((this.ganttChartViewScrollViewer.ExtentHeight - num4) * num2) / ((double) actualDisplayRowCount);
                //    if (!double.IsNaN(d))
                //    {
                //this.isDuringUpdateGanttChartViewVerticalScrollBarFromDataTree = true;
                //this.ganttChartViewScrollViewer.ScrollToVerticalOffset(d);
                //        if (method == null)
                //        {
                //            method = delegate {
                //                this.isDuringUpdateGanttChartViewVerticalScrollBarFromDataTree = false;
                //            };
                //        }
                //        base.Dispatcher.BeginInvoke(method, new object[0]);
                //    }
                //}
            }
        }

        protected virtual void UpdateItemsSource()
        {
            if (this.isTemplateApplied)
            {
                if (this.items != null)
                {
                    this.items.ItemPropertyChanged -= new PropertyChangedEventHandler(this.Items_ItemPropertyChanged);
                    this.items.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.Items_CollectionChanged);
                }
                if (this.originalItems != null)
                {
                    this.originalItems.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OriginalItems_CollectionChanged);
                }
                if (this.asyncTimer != null)
                {
                    this.asyncTimer.Stop();
                    this.asyncTimer.Tick -= new EventHandler(this.AsyncTimer_Tick);
                    this.asyncTimer = null;
                }
                this.originalItems = this.Items;
                if (this.originalItems == null)
                {
                    base.ItemsSource = null;
                }
                else
                {
                    foreach (GanttChartItem item in this.originalItems)
                    {
                        this.AttachItem(item);
                    }
                    this.items = new GanttChartItemCollection();
                    if (!this.IsAsyncPresentationEnabled)
                    {
                        this.asyncItemCount = -1;
                        foreach (GanttChartItem item2 in this.originalItems)
                        {
                            this.items.Add(item2);
                        }
                    }
                    else
                    {
                        this.asyncItemCount = Math.Min(this.originalItems.Count, this.IsAsyncPresentationEnabledMinCount);
                        for (int i = 0; i < this.asyncItemCount; i++)
                        {
                            this.items.Add(this.originalItems[i]);
                        }
                        if (this.asyncTimer == null)
                        {
                            this.asyncTimer = new DispatcherTimer();
                            this.asyncTimer.Tick += new EventHandler(this.AsyncTimer_Tick);
                        }
                        this.asyncTimer.Start();
                    }
                    this.originalItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OriginalItems_CollectionChanged);
                    this.internalUpdateItemsSourceCount++;
                    base.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        if (--this.internalUpdateItemsSourceCount <= 0)
                        {
                            this.items.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Items_CollectionChanged);
                            this.items.ItemPropertyChanged += new PropertyChangedEventHandler(this.Items_ItemPropertyChanged);
                        }
                    }));
                    for (int i = 0; i < this.items.Count; i++)
                    {
                        this.items.UpdateVisibilityFromExpansion(i);
                    }
                    base.ItemsSource = this.items;
                }
            }
        }

        private void UpdateItemsSources()
        {
            if (this.dataTreeGrid != null)
            {
                this.dataTreeGrid.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("ItemsSource") { Source = this });
            }
            if (this.ganttChartView != null)
            {
                this.ganttChartView.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("ItemsSource") { Source = this });
            }
        }

        public bool AreUpdateTimelinePageButtonsVisible
        {
            get
            {
                return (bool)base.GetValue(AreUpdateTimelinePageButtonsVisibleProperty);
            }
            set
            {
                base.SetValue(AreUpdateTimelinePageButtonsVisibleProperty, value);
            }
        }

        public DataTemplate AssignmentsTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(AssignmentsTemplateProperty);
            }
            set
            {
                base.SetValue(AssignmentsTemplateProperty, value);
            }
        }

        public double BarHeight
        {
            get
            {
                return (double)base.GetValue(BarHeightProperty);
            }
            set
            {
                base.SetValue(BarHeightProperty, value);
            }
        }

        public FrameworkElement ChartContentElement
        {
            get
            {
                if (this.ganttChartView == null)
                {
                    return null;
                }
                return this.ganttChartView.ContentElement;
            }
        }

        public FrameworkElement ChartHeaderElement
        {
            get
            {
                if (this.ganttChartView == null)
                {
                    return null;
                }
                return this.ganttChartView.HeaderElement;
            }
        }

        public GridLength ChartWidth
        {
            get
            {
                return (GridLength)base.GetValue(ChartWidthProperty);
            }
            set
            {
                base.SetValue(ChartWidthProperty, value);
            }
        }

        public new DlhSoft.Windows.Controls.DataGridColumnCollection Columns
        {
            get
            {
                return (DlhSoft.Windows.Controls.DataGridColumnCollection)base.GetValue(ColumnsProperty);
            }
            set
            {
                base.SetValue(ColumnsProperty, value);
            }
        }

        public double CompletedBarHeight
        {
            get
            {
                return (double)base.GetValue(CompletedBarHeightProperty);
            }
            set
            {
                base.SetValue(CompletedBarHeightProperty, value);
            }
        }

        public Brush CurrentTimeLineStroke
        {
            get
            {
                return (Brush)base.GetValue(CurrentTimeLineStrokeProperty);
            }
            set
            {
                base.SetValue(CurrentTimeLineStrokeProperty, value);
            }
        }

        public GridLength DataGridWidth
        {
            get
            {
                return (GridLength)base.GetValue(DataGridWidthProperty);
            }
            set
            {
                base.SetValue(DataGridWidthProperty, value);
            }
        }

        public DlhSoft.Windows.Controls.DataTreeGrid DataTreeGrid
        {
            get
            {
                return this.dataTreeGrid;
            }
        }

        public static DlhSoft.Windows.Controls.DependencyCreationValidator DefaultDependencyCreationValidator
        {
            get
            {
                return DlhSoft.Windows.Controls.GanttChartView.DefaultDependencyCreationValidator;
            }
        }

        public static DlhSoft.Windows.Controls.DependencyDeletionValidator DefaultDependencyDeletionValidator
        {
            get
            {
                return DlhSoft.Windows.Controls.GanttChartView.DefaultDependencyDeletionValidator;
            }
        }

        public DlhSoft.Windows.Controls.DependencyCreationValidator DependencyCreationValidator
        {
            get
            {
                return (DlhSoft.Windows.Controls.DependencyCreationValidator)base.GetValue(DependencyCreationValidatorProperty);
            }
            set
            {
                base.SetValue(DependencyCreationValidatorProperty, value);
            }
        }

        public object DependencyDeletionContextMenuItemHeader
        {
            get
            {
                return base.GetValue(DependencyDeletionContextMenuItemHeaderProperty);
            }
            set
            {
                base.SetValue(DependencyDeletionContextMenuItemHeaderProperty, value);
            }
        }

        public DlhSoft.Windows.Controls.DependencyDeletionValidator DependencyDeletionValidator
        {
            get
            {
                return (DlhSoft.Windows.Controls.DependencyDeletionValidator)base.GetValue(DependencyDeletionValidatorProperty);
            }
            set
            {
                base.SetValue(DependencyDeletionValidatorProperty, value);
            }
        }

        public Brush DependencyLineStroke
        {
            get
            {
                return (Brush)base.GetValue(DependencyLineStrokeProperty);
            }
            set
            {
                base.SetValue(DependencyLineStrokeProperty, value);
            }
        }

        public double DependencyLineStrokeThickness
        {
            get
            {
                return (double)base.GetValue(DependencyLineStrokeThicknessProperty);
            }
            set
            {
                base.SetValue(DependencyLineStrokeThicknessProperty, value);
            }
        }

        public DataTemplate DependencyLineTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(DependencyLineTemplateProperty);
            }
            set
            {
                base.SetValue(DependencyLineTemplateProperty, value);
            }
        }

        public DateTime DisplayedTime
        {
            get
            {
                return (DateTime)base.GetValue(DisplayedTimeProperty);
            }
            set
            {
                base.SetValue(DisplayedTimeProperty, value);
            }
        }

        bool IGanttChartView.IsReadOnly
        {
            get
            {
                return base.IsReadOnly;
            }
            set
            {
                base.IsReadOnly = value;
            }
        }

        public ControlTemplate ExpanderTemplate
        {
            get
            {
                return (ControlTemplate)base.GetValue(ExpanderTemplateProperty);
            }
            set
            {
                base.SetValue(ExpanderTemplateProperty, value);
            }
        }

        public DlhSoft.Windows.Controls.GanttChartView GanttChartView
        {
            get
            {
                return this.ganttChartView;
            }
        }

        public Brush HeaderBackground
        {
            get
            {
                return (Brush)base.GetValue(HeaderBackgroundProperty);
            }
            set
            {
                base.SetValue(HeaderBackgroundProperty, value);
            }
        }

        public double HeaderHeight
        {
            get
            {
                return (double)base.GetValue(HeaderHeightProperty);
            }
            set
            {
                base.SetValue(HeaderHeightProperty, value);
            }
        }

        public double HourWidth
        {
            get
            {
                return (double)base.GetValue(HourWidthProperty);
            }
            set
            {
                base.SetValue(HourWidthProperty, value);
            }
        }

        public double IndentationUnitSize
        {
            get
            {
                return (double)base.GetValue(IndentationUnitSizeProperty);
            }
            set
            {
                base.SetValue(IndentationUnitSizeProperty, value);
            }
        }

        public bool IsAsyncPresentationEnabled
        {
            get
            {
                return (bool)base.GetValue(IsAsyncPresentationEnabledProperty);
            }
            set
            {
                base.SetValue(IsAsyncPresentationEnabledProperty, value);
            }
        }

        public int IsAsyncPresentationEnabledMinCount
        {
            get
            {
                return (int)base.GetValue(IsAsyncPresentationEnabledMinCountProperty);
            }
            set
            {
                base.SetValue(IsAsyncPresentationEnabledMinCountProperty, value);
            }
        }

        public int IsAsyncPresentationEnabledPageSize
        {
            get
            {
                return (int)base.GetValue(IsAsyncPresentationEnabledPageSizeProperty);
            }
            set
            {
                base.SetValue(IsAsyncPresentationEnabledPageSizeProperty, value);
            }
        }

        public bool IsCurrentTimeLineVisible
        {
            get
            {
                return (bool)base.GetValue(IsCurrentTimeLineVisibleProperty);
            }
            set
            {
                base.SetValue(IsCurrentTimeLineVisibleProperty, value);
            }
        }

        public bool IsDependencyToolTipVisible
        {
            get
            {
                return (bool)base.GetValue(IsDependencyToolTipVisibleProperty);
            }
            set
            {
                base.SetValue(IsDependencyToolTipVisibleProperty, value);
            }
        }

        public bool IsItemLoadCompleted
        {
            get
            {
                if (this.asyncItemCount >= 0)
                {
                    return (this.asyncItemCount >= this.originalItems.Count);
                }
                return true;
            }
        }

        public bool IsNonworkingTimeHighlighted
        {
            get
            {
                return (bool)base.GetValue(IsNonworkingTimeHighlightedProperty);
            }
            set
            {
                base.SetValue(IsNonworkingTimeHighlightedProperty, value);
            }
        }

        public bool IsScheduleCachingEnabled
        {
            get
            {
                if (this.ganttChartView == null)
                {
                    return false;
                }
                return this.ganttChartView.IsScheduleCachingEnabled;
            }
            set
            {
                if (this.ganttChartView != null)
                {
                    this.ganttChartView.IsScheduleCachingEnabled = value;
                }
            }
        }

        public bool IsSplitterEnabled
        {
            get
            {
                return (bool)base.GetValue(IsSplitterEnabledProperty);
            }
            set
            {
                base.SetValue(IsSplitterEnabledProperty, value);
            }
        }

        public bool IsTaskCompletedEffortVisible
        {
            get
            {
                return (bool)base.GetValue(IsTaskCompletedEffortVisibleProperty);
            }
            set
            {
                base.SetValue(IsTaskCompletedEffortVisibleProperty, value);
            }
        }

        public bool IsTaskToolTipVisible
        {
            get
            {
                return (bool)base.GetValue(IsTaskToolTipVisibleProperty);
            }
            set
            {
                base.SetValue(IsTaskToolTipVisibleProperty, value);
            }
        }

        public bool IsVirtualizing
        {
            get
            {
                return (bool)base.GetValue(IsVirtualizingProperty);
            }
            set
            {
                base.SetValue(IsVirtualizingProperty, value);
            }
        }

        public GanttChartItem this[int index]
        {
            get
            {
                if (this.ganttChartView == null)
                {
                    return null;
                }
                return this.ganttChartView[index];
            }
        }

        public double ItemHeight
        {
            get
            {
                return (double)base.GetValue(ItemHeightProperty);
            }
            set
            {
                base.SetValue(ItemHeightProperty, value);
            }
        }

        public new ObservableCollection<GanttChartItem> Items
        {
            get
            {
                return (ObservableCollection<GanttChartItem>)base.GetValue(ItemsProperty);
            }
            set
            {
                base.SetValue(ItemsProperty, value);
            }
        }

        public new ItemsPanelTemplate ItemsPanel
        {
            get
            {
                return (ItemsPanelTemplate)base.GetValue(ItemsControl.ItemsPanelProperty);
            }
            set
            {
                base.SetValue(ItemsControl.ItemsPanelProperty, value);
            }
        }

        public new DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(ItemsControl.ItemTemplateProperty);
            }
            set
            {
                base.SetValue(ItemsControl.ItemTemplateProperty, value);
            }
        }

        public GanttChartItemCollection ManagedItems
        {
            get
            {
                return this.items;
            }
        }

        public double MaxChartWidth
        {
            get
            {
                return (double)base.GetValue(MaxChartWidthProperty);
            }
            set
            {
                base.SetValue(MaxChartWidthProperty, value);
            }
        }

        public double MaxDataGridWidth
        {
            get
            {
                return (double)base.GetValue(MaxDataGridWidthProperty);
            }
            set
            {
                base.SetValue(MaxDataGridWidthProperty, value);
            }
        }

        public Brush MilestoneBarFill
        {
            get
            {
                return (Brush)base.GetValue(MilestoneBarFillProperty);
            }
            set
            {
                base.SetValue(MilestoneBarFillProperty, value);
            }
        }

        public Brush MilestoneBarStroke
        {
            get
            {
                return (Brush)base.GetValue(MilestoneBarStrokeProperty);
            }
            set
            {
                base.SetValue(MilestoneBarStrokeProperty, value);
            }
        }

        public double MilestoneBarStrokeThickness
        {
            get
            {
                return (double)base.GetValue(MilestoneBarStrokeThicknessProperty);
            }
            set
            {
                base.SetValue(MilestoneBarStrokeThicknessProperty, value);
            }
        }

        public DataTemplate MilestoneTaskTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(MilestoneTaskTemplateProperty);
            }
            set
            {
                base.SetValue(MilestoneTaskTemplateProperty, value);
            }
        }

        public double MinChartWidth
        {
            get
            {
                return (double)base.GetValue(MinChartWidthProperty);
            }
            set
            {
                base.SetValue(MinChartWidthProperty, value);
            }
        }

        public double MinDataGridWidth
        {
            get
            {
                return (double)base.GetValue(MinDataGridWidthProperty);
            }
            set
            {
                base.SetValue(MinDataGridWidthProperty, value);
            }
        }

        public ObservableCollection<TimeInterval> NonworkingIntervals
        {
            get
            {
                return (ObservableCollection<TimeInterval>)base.GetValue(NonworkingIntervalsProperty);
            }
            set
            {
                base.SetValue(NonworkingIntervalsProperty, value);
            }
        }

        public Brush NonworkingTimeBackground
        {
            get
            {
                return (Brush)base.GetValue(NonworkingTimeBackgroundProperty);
            }
            set
            {
                base.SetValue(NonworkingTimeBackgroundProperty, value);
            }
        }

        public DataTemplate PredecessorToolTipTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(PredecessorToolTipTemplateProperty);
            }
            set
            {
                base.SetValue(PredecessorToolTipTemplateProperty, value);
            }
        }

        public double ScaleHeaderHeight
        {
            get
            {
                return (double)base.GetValue(ScaleHeaderHeightProperty);
            }
            set
            {
                base.SetValue(ScaleHeaderHeightProperty, value);
            }
        }

        public ScaleCollection Scales
        {
            get
            {
                return (ScaleCollection)base.GetValue(ScalesProperty);
            }
            set
            {
                base.SetValue(ScalesProperty, value);
            }
        }

        public Brush SplitterBackground
        {
            get
            {
                return (Brush)base.GetValue(SplitterBackgroundProperty);
            }
            set
            {
                base.SetValue(SplitterBackgroundProperty, value);
            }
        }

        public Brush SplitterBorderBrush
        {
            get
            {
                return (Brush)base.GetValue(SplitterBorderBrushProperty);
            }
            set
            {
                base.SetValue(SplitterBorderBrushProperty, value);
            }
        }

        public Thickness SplitterBorderThickness
        {
            get
            {
                return (Thickness)base.GetValue(SplitterBorderThicknessProperty);
            }
            set
            {
                base.SetValue(SplitterBorderThicknessProperty, value);
            }
        }

        public double SplitterWidth
        {
            get
            {
                return (double)base.GetValue(SplitterWidthProperty);
            }
            set
            {
                base.SetValue(SplitterWidthProperty, value);
            }
        }

        public double StandardBarCornerRadius
        {
            get
            {
                return (double)base.GetValue(StandardBarCornerRadiusProperty);
            }
            set
            {
                base.SetValue(StandardBarCornerRadiusProperty, value);
            }
        }

        public Brush StandardBarFill
        {
            get
            {
                return (Brush)base.GetValue(StandardBarFillProperty);
            }
            set
            {
                base.SetValue(StandardBarFillProperty, value);
            }
        }

        public Brush StandardBarStroke
        {
            get
            {
                return (Brush)base.GetValue(StandardBarStrokeProperty);
            }
            set
            {
                base.SetValue(StandardBarStrokeProperty, value);
            }
        }

        public double StandardBarStrokeThickness
        {
            get
            {
                return (double)base.GetValue(StandardBarStrokeThicknessProperty);
            }
            set
            {
                base.SetValue(StandardBarStrokeThicknessProperty, value);
            }
        }

        public double StandardCompletedBarCornerRadius
        {
            get
            {
                return (double)base.GetValue(StandardCompletedBarCornerRadiusProperty);
            }
            set
            {
                base.SetValue(StandardCompletedBarCornerRadiusProperty, value);
            }
        }

        public Brush StandardCompletedBarFill
        {
            get
            {
                return (Brush)base.GetValue(StandardCompletedBarFillProperty);
            }
            set
            {
                base.SetValue(StandardCompletedBarFillProperty, value);
            }
        }

        public Brush StandardCompletedBarStroke
        {
            get
            {
                return (Brush)base.GetValue(StandardCompletedBarStrokeProperty);
            }
            set
            {
                base.SetValue(StandardCompletedBarStrokeProperty, value);
            }
        }

        public double StandardCompletedBarStrokeThickness
        {
            get
            {
                return (double)base.GetValue(StandardCompletedBarStrokeThicknessProperty);
            }
            set
            {
                base.SetValue(StandardCompletedBarStrokeThicknessProperty, value);
            }
        }

        public DataTemplate StandardTaskTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(StandardTaskTemplateProperty);
            }
            set
            {
                base.SetValue(StandardTaskTemplateProperty, value);
            }
        }

        public Brush SummaryBarFill
        {
            get
            {
                return (Brush)base.GetValue(SummaryBarFillProperty);
            }
            set
            {
                base.SetValue(SummaryBarFillProperty, value);
            }
        }

        public Brush SummaryBarStroke
        {
            get
            {
                return (Brush)base.GetValue(SummaryBarStrokeProperty);
            }
            set
            {
                base.SetValue(SummaryBarStrokeProperty, value);
            }
        }

        public double SummaryBarStrokeThickness
        {
            get
            {
                return (double)base.GetValue(SummaryBarStrokeThicknessProperty);
            }
            set
            {
                base.SetValue(SummaryBarStrokeThicknessProperty, value);
            }
        }

        public DataTemplate SummaryTaskTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(SummaryTaskTemplateProperty);
            }
            set
            {
                base.SetValue(SummaryTaskTemplateProperty, value);
            }
        }

        public DateTime TimelinePageFinish
        {
            get
            {
                return (DateTime)base.GetValue(TimelinePageFinishProperty);
            }
            set
            {
                base.SetValue(TimelinePageFinishProperty, value);
            }
        }

        public DateTime TimelinePageStart
        {
            get
            {
                return (DateTime)base.GetValue(TimelinePageStartProperty);
            }
            set
            {
                base.SetValue(TimelinePageStartProperty, value);
            }
        }

        public DataTemplate ToolTipTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(ToolTipTemplateProperty);
            }
            set
            {
                base.SetValue(ToolTipTemplateProperty, value);
            }
        }

        public TimeSpan UpdateScaleInterval
        {
            get
            {
                return (TimeSpan)base.GetValue(UpdateScaleIntervalProperty);
            }
            set
            {
                base.SetValue(UpdateScaleIntervalProperty, value);
            }
        }

        public TimeSpan UpdateTimelinePageAmount
        {
            get
            {
                return (TimeSpan)base.GetValue(UpdateTimelinePageAmountProperty);
            }
            set
            {
                base.SetValue(UpdateTimelinePageAmountProperty, value);
            }
        }

        public TimeOfDay VisibleDayFinish
        {
            get
            {
                return (TimeOfDay)base.GetValue(VisibleDayFinishProperty);
            }
            set
            {
                base.SetValue(VisibleDayFinishProperty, value);
            }
        }

        public TimeOfDay VisibleDayStart
        {
            get
            {
                return (TimeOfDay)base.GetValue(VisibleDayStartProperty);
            }
            set
            {
                base.SetValue(VisibleDayStartProperty, value);
            }
        }

        public DayOfWeek VisibleWeekFinish
        {
            get
            {
                return (DayOfWeek)base.GetValue(VisibleWeekFinishProperty);
            }
            set
            {
                base.SetValue(VisibleWeekFinishProperty, value);
            }
        }

        public DayOfWeek VisibleWeekStart
        {
            get
            {
                return (DayOfWeek)base.GetValue(VisibleWeekStartProperty);
            }
            set
            {
                base.SetValue(VisibleWeekStartProperty, value);
            }
        }

        public TimeOfDay WorkingDayFinish
        {
            get
            {
                return (TimeOfDay)base.GetValue(WorkingDayFinishProperty);
            }
            set
            {
                base.SetValue(WorkingDayFinishProperty, value);
            }
        }

        public TimeOfDay WorkingDayStart
        {
            get
            {
                return (TimeOfDay)base.GetValue(WorkingDayStartProperty);
            }
            set
            {
                base.SetValue(WorkingDayStartProperty, value);
            }
        }

        public Brush WorkingTimeBackground
        {
            get
            {
                return (Brush)base.GetValue(WorkingTimeBackgroundProperty);
            }
            set
            {
                base.SetValue(WorkingTimeBackgroundProperty, value);
            }
        }

        public DayOfWeek WorkingWeekFinish
        {
            get
            {
                return (DayOfWeek)base.GetValue(WorkingWeekFinishProperty);
            }
            set
            {
                base.SetValue(WorkingWeekFinishProperty, value);
            }
        }

        public DayOfWeek WorkingWeekStart
        {
            get
            {
                return (DayOfWeek)base.GetValue(WorkingWeekStartProperty);
            }
            set
            {
                base.SetValue(WorkingWeekStartProperty, value);
            }
        }

        public class DocumentPaginator : System.Windows.Documents.DocumentPaginator
        {
            private GanttChartDataGrid control;

            public DocumentPaginator(GanttChartDataGrid control)
            {
                this.control = control;
            }

            public override DocumentPage GetPage(int pageNumber)
            {
                if (pageNumber < 0)
                {
                    return DocumentPage.Missing;
                }
                double num = Math.IEEERemainder(((this.PageSize.Height - 2.0) - 32.0) - this.control.HeaderHeight, this.control.ItemHeight);
                if (num < 0.0)
                {
                    num += this.control.ItemHeight;
                }
                double num2 = this.PageSize.Height - num;
                Size pageSize = new Size(Math.Max((double)1.0, (double)((this.PageSize.Width - 2.0) - 32.0)), Math.Max((double)1.0, (double)((num2 - 2.0) - 32.0)));
                int exportVerticalLength = this.control.GetExportVerticalLength(pageSize);
                int exportHorizontalLength = this.control.GetExportHorizontalLength(pageSize);
                int i = pageNumber / exportHorizontalLength;
                int j = pageNumber % exportHorizontalLength;
                if ((i >= exportVerticalLength) || (j >= exportHorizontalLength))
                {
                    return DocumentPage.Missing;
                }
                BitmapSource source = this.control.GetExportBitmapSource(pageSize, i, j);
                FrameworkElement visual = new Border
                {
                    Child = new Image { Source = source, VerticalAlignment = VerticalAlignment.Top },
                    BorderBrush = new SolidColorBrush(Colors.DarkGray),
                    BorderThickness = new Thickness(1.0),
                    Margin = new Thickness(16.0)
                };
                visual.Measure(this.PageSize);
                visual.Arrange(new Rect(this.PageSize));
                return new DocumentPage(visual);
            }

            public GanttChartDataGrid Control
            {
                get
                {
                    return this.control;
                }
            }

            public override bool IsPageCountValid
            {
                get
                {
                    return true;
                }
            }

            public override int PageCount
            {
                get
                {
                    return (this.control.GetExportVerticalLength(this.PageSize) * this.control.GetExportHorizontalLength(this.PageSize));
                }
            }

            public override Size PageSize { get; set; }

            public override IDocumentPaginatorSource Source
            {
                get
                {
                    return null;
                }
            }
        }
    }
}

