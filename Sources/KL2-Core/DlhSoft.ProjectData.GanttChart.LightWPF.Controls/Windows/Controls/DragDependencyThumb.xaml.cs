using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace DlhSoft.Windows.Controls
{
    /// <summary>
    /// Interaction logic for DragDependencyThumb.xaml
    /// </summary>
    public partial class DragDependencyThumb : UserControl
    {
        private GanttChartItem dragOverItem;
        private double horizontalChange;
        private double originalHorizontalPosition;
        private double originalVerticalPosition;
        private double verticalChange;

        public DragDependencyThumb()
        {
            InitializeComponent();
        }


        private void Root_MouseEnter(object sender, MouseEventArgs e)
        {
            this.HoveringIndicator.Visibility = Visibility.Visible;
        }

        private void Root_MouseLeave(object sender, MouseEventArgs e)
        {
            this.HoveringIndicator.Visibility = Visibility.Collapsed;
        }


        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            GanttChartItem dataContext = this.Thumb.DataContext as GanttChartItem;
            if (dataContext != null)
            {
                this.AcceptedLine.Visibility = Visibility.Collapsed;
                this.Line.Visibility = Visibility.Collapsed;
                this.Indicator.Visibility = Visibility.Collapsed;
                if (this.dragOverItem != null)
                {
                    this.dragOverItem.Predecessors.Add(new PredecessorItem { Item = dataContext });
                    if (!this.dragOverItem.HasChildren && (this.dragOverItem.Start < dataContext.Finish))
                    {
                        TimeSpan effort = this.dragOverItem.Effort;
                        TimeSpan completedEffort = this.dragOverItem.CompletedEffort;
                        this.dragOverItem.Start = dataContext.Finish;
                        if (!this.dragOverItem.IsMilestone)
                        {
                            this.dragOverItem.Effort = effort;
                            this.dragOverItem.CompletedEffort = completedEffort;
                        }
                    }
                }
            }
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            GanttChartView ganttChartView;
            ScrollViewer scrollViewer;
            double itemHeight;
            Point p;
            GanttChartItem dataContext = this.Thumb.DataContext as GanttChartItem;
            if (dataContext != null)
            {
                ganttChartView = dataContext.GanttChartView as GanttChartView;
                if ((ganttChartView != null) && (ganttChartView.ContentElement != null))
                {
                    this.dragOverItem = null;
                    this.AcceptedLine.Visibility = Visibility.Collapsed;
                    this.Line.Visibility = Visibility.Visible;
                    this.horizontalChange = e.HorizontalChange;
                    this.verticalChange = e.VerticalChange;
                    scrollViewer = ganttChartView.ScrollViewer;
                    double x = this.originalHorizontalPosition + this.horizontalChange;
                    double y = this.originalVerticalPosition + this.verticalChange;
                    itemHeight = ganttChartView.ItemHeight;
                    p = new Point(x, y);
                    base.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        ScrollContentPresenter visual = ganttChartView.ScrollContentPresenter;
                        if ((scrollViewer != null) && (visual != null))
                        {
                            try
                            {
                                p = this.TransformToVisual(visual).Transform(p);
                                if (p.X < itemHeight)
                                {
                                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - itemHeight);
                                }
                                if (p.X > (scrollViewer.ViewportWidth - itemHeight))
                                {
                                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + itemHeight);
                                }
                                if (p.Y < itemHeight)
                                {
                                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - itemHeight);
                                }
                                if (p.Y > (scrollViewer.ViewportHeight - itemHeight))
                                {
                                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + itemHeight);
                                }
                            }
                            catch
                            {
                            }
                        }
                    }));
                    this.Line.X2 = x;
                    this.Line.Y2 = y;
                    this.AcceptedLine.X2 = this.Line.X2;
                    this.AcceptedLine.Y2 = this.Line.Y2;
                    double num3 = dataContext.ActualDisplayRowIndex * itemHeight;
                    double position = y + num3;
                    int itemIndexAt = ganttChartView.GetItemIndexAt(position);
                    GanttChartItemCollection managedItems = ganttChartView.ManagedItems;
                    if (((managedItems != null) && (itemIndexAt >= 0)) && (itemIndexAt < managedItems.Count))
                    {
                        this.dragOverItem = managedItems[itemIndexAt];
                        itemIndexAt = this.dragOverItem.ActualDisplayRowIndex;
                        x += dataContext.ComputedBarLeft + dataContext.ComputedBarWidth;
                        double computedBarLeft = this.dragOverItem.ComputedBarLeft;
                        double num7 = computedBarLeft + this.dragOverItem.ComputedBarWidth;
                        if (((x < computedBarLeft) || (x > num7)) || ((this.dragOverItem.Predecessors == null) || ((ganttChartView.DependencyCreationValidator != null) && !ganttChartView.DependencyCreationValidator(this.dragOverItem, dataContext))))
                        {
                            this.dragOverItem = null;
                        }
                        else
                        {
                            if (this.verticalChange >= 0.0)
                            {
                                y = (((itemIndexAt * itemHeight) - num3) + ((itemHeight - this.dragOverItem.ComputedBarHeight) / 2.0)) + (this.dragOverItem.HasChildren ? (this.dragOverItem.ComputedBarHeight * 0.2) : 0.0);
                            }
                            else
                            {
                                y = (((itemIndexAt * itemHeight) - num3) + (itemHeight / 2.0)) - (this.dragOverItem.HasChildren ? (this.dragOverItem.ComputedBarHeight * 0.2) : 0.0);
                            }
                            this.Line.Y2 = y;
                            this.AcceptedLine.Y2 = y;
                            this.AcceptedLine.Visibility = Visibility.Visible;
                            this.Line.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            GanttChartItem dataContext = base.DataContext as GanttChartItem;
            if ((dataContext != null) && (dataContext.GanttChartView is GanttChartView))
            {
                this.originalHorizontalPosition = e.HorizontalOffset;
                this.originalVerticalPosition = e.VerticalOffset;
                this.horizontalChange = 0.0;
                this.verticalChange = 0.0;
                this.Line.X1 = this.IndicatorRoot.ActualWidth / 2.0;
                this.Line.Y1 = this.Root.ActualHeight / 2.0;
                this.Line.X2 = this.originalHorizontalPosition;
                this.Line.Y2 = this.originalVerticalPosition;
                this.AcceptedLine.Visibility = Visibility.Collapsed;
                this.Line.Visibility = Visibility.Visible;
                this.Indicator.Visibility = Visibility.Visible;
                this.HoveringIndicator.Visibility = Visibility.Collapsed;
                this.AcceptedLine.X1 = this.Line.X1;
                this.AcceptedLine.Y1 = this.Line.Y1;
                this.AcceptedLine.X2 = this.Line.X2;
                this.AcceptedLine.Y2 = this.Line.Y2;
            }
        }
    }
}
