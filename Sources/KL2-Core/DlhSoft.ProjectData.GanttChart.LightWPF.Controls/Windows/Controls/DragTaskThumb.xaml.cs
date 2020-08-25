using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DlhSoft.Windows.Controls
{
    /// <summary>
    /// Interaction logic for DragTaskThumb.xaml
    /// </summary>
    public partial class DragTaskThumb : UserControl
    {
        private double horizontalChange;
        private bool isDuringInternalScrollOperation;
        private double originalCompletedFinishPositionDifference;
        private double originalFinishPositionDifference;
        private double originalHorizontalPosition;
        private double originalStartPosition;
        public static readonly DependencyProperty RoleProperty = DependencyProperty.Register("Role", typeof(DragTaskThumbRole), typeof(DragTaskThumb), new PropertyMetadata(DragTaskThumbRole.UpdateStart, new PropertyChangedCallback(DragTaskThumb.OnRoleChanged)));

        public DragTaskThumb()
        {
            InitializeComponent();
        }

        private static void OnRoleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DragTaskThumb thumb = d as DragTaskThumb;
            if (thumb != null)
            {
                thumb.UpdateCursorFromRole();
            }
        }


        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            GanttChartView ganttChartView;
            ScrollViewer scrollViewer;
            GanttChartItem dataContext = this.Thumb.DataContext as GanttChartItem;
            if (dataContext != null)
            {
                ganttChartView = dataContext.GanttChartView as GanttChartView;
                if ((ganttChartView != null) && !dataContext.HasChildren)
                {
                    this.horizontalChange = e.HorizontalChange;
                    scrollViewer = ganttChartView.ScrollViewer;
                    if (!this.isDuringInternalScrollOperation)
                    {
                        double x = this.originalHorizontalPosition + this.horizontalChange;
                        double itemHeight = ganttChartView.ItemHeight;
                        Point p = new Point(x, 0.0);
                        this.isDuringInternalScrollOperation = true;
                        base.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            double previousHorizontalOffset = scrollViewer.HorizontalOffset;
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
                                }
                                catch
                                {
                                }
                            }
                            this.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                this.originalHorizontalPosition -= scrollViewer.HorizontalOffset - previousHorizontalOffset;
                                this.isDuringInternalScrollOperation = false;
                            }));
                        }));
                    }
                    switch (this.Role)
                    {
                        case DragTaskThumbRole.UpdateFinish:
                            if (!dataContext.IsMilestone)
                            {
                                double num3 = ganttChartView.GetPosition(dataContext.Finish) + e.HorizontalChange;
                                dataContext.Finish = ganttChartView.GetPreviousVisibleWorkingTime(ganttChartView.GetUpdateScaleTime(ganttChartView.GetDateTime(num3)));
                                return;
                            }
                            return;

                        case DragTaskThumbRole.UpdateCompletedFinish:
                            if (!dataContext.IsMilestone)
                            {
                                double num4 = ganttChartView.GetPosition(dataContext.CompletedFinish) + e.HorizontalChange;
                                dataContext.CompletedFinish = ganttChartView.GetPreviousVisibleWorkingTime(ganttChartView.GetUpdateScaleTime(ganttChartView.GetDateTime(num4)));
                                return;
                            }
                            return;
                    }
                    double position = ganttChartView.GetPosition(dataContext.Start) + e.HorizontalChange;
                    TimeSpan effort = dataContext.Effort;
                    TimeSpan completedEffort = dataContext.CompletedEffort;
                    dataContext.Start = ganttChartView.GetNextVisibleWorkingTime(ganttChartView.GetUpdateScaleTime(ganttChartView.GetDateTime(position)));
                    if (!dataContext.IsMilestone)
                    {
                        dataContext.Effort = effort;
                        dataContext.CompletedEffort = completedEffort;
                    }
                }
            }
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            GanttChartItem dataContext = base.DataContext as GanttChartItem;
            if (dataContext != null)
            {
                GanttChartView ganttChartView = dataContext.GanttChartView as GanttChartView;
                if (ganttChartView != null)
                {
                    this.originalHorizontalPosition = e.HorizontalOffset;
                    this.horizontalChange = 0.0;
                    this.originalStartPosition = ganttChartView.GetPosition(dataContext.Start);
                    this.originalFinishPositionDifference = ganttChartView.GetPosition(dataContext.Finish) - this.originalStartPosition;
                    this.originalCompletedFinishPositionDifference = ganttChartView.GetPosition(dataContext.CompletedFinish) - this.originalStartPosition;
                }
            }
        }

        private void UpdateCursorFromRole()
        {
            switch (this.Role)
            {
                case DragTaskThumbRole.UpdateFinish:
                case DragTaskThumbRole.UpdateCompletedFinish:
                    this.Thumb.Cursor = Cursors.SizeWE;
                    return;
            }
            this.Thumb.Cursor = Cursors.Hand;
        }

        public DragTaskThumbRole Role
        {
            get
            {
                return (DragTaskThumbRole)base.GetValue(RoleProperty);
            }
            set
            {
                base.SetValue(RoleProperty, value);
            }
        }

    }
}
