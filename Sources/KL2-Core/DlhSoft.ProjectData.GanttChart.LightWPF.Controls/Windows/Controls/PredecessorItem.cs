namespace DlhSoft.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media;

    public class PredecessorItem : DependencyObject
    {
        public static readonly DependencyProperty ComputedDependencyLinePointsProperty = DependencyProperty.Register("ComputedDependencyLinePoints", typeof(PointCollection), typeof(PredecessorItem), new PropertyMetadata(null));
        public static readonly DependencyProperty DependencyTypeProperty = DependencyProperty.Register("DependencyType", typeof(DlhSoft.Windows.Controls.DependencyType), typeof(PredecessorItem), new PropertyMetadata(DlhSoft.Windows.Controls.DependencyType.FinishStart, new PropertyChangedCallback(PredecessorItem.OnDependencyTypeChanged)));
        public static readonly DependencyProperty ItemProperty = DependencyProperty.Register("Item", typeof(GanttChartItem), typeof(PredecessorItem), new PropertyMetadata(null, new PropertyChangedCallback(PredecessorItem.OnItemChanged)));

        public event EventHandler Changed;

        protected virtual void OnChanged()
        {
            if (this.Changed != null)
            {
                this.Changed(this, EventArgs.Empty);
            }
        }

        private static void OnDependencyTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PredecessorItem item = d as PredecessorItem;
            if (item != null)
            {
                item.OnChanged();
            }
        }

        private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PredecessorItem item = d as PredecessorItem;
            if (item != null)
            {
                item.OnChanged();
            }
        }

        private void UpdateImpl()
        {
            PointCollection points;
            if (((this.Item == null) || (this.DependentItem == null)) || (this.GanttChartView == null))
            {
                return;
            }
            if ((this.Item.IsVisible && this.DependentItem.IsVisible) && (((this.DependencyType == DlhSoft.Windows.Controls.DependencyType.FinishStart) || (this.DependencyType == DlhSoft.Windows.Controls.DependencyType.StartStart)) || ((this.DependencyType == DlhSoft.Windows.Controls.DependencyType.FinishFinish) || (this.DependencyType == DlhSoft.Windows.Controls.DependencyType.StartFinish))))
            {
                points = new PointCollection();
                double num = this.Item.ComputedBarLeft - this.DependentItem.ComputedBarLeft;
                double y = (this.Item.ActualDisplayRowIndex - this.DependentItem.ActualDisplayRowIndex) * this.GanttChartView.ItemHeight;
                double num3 = this.GanttChartView.BarHeight / 2.0;
                double num4 = (y <= 0.0) ? (this.GanttChartView.ItemHeight / 2.0) : (-this.GanttChartView.ItemHeight / 2.0);
                switch (this.DependencyType)
                {
                    case DlhSoft.Windows.Controls.DependencyType.FinishStart:
                        {
                            double x = num + this.Item.ComputedBarWidth;
                            points.Add(new Point(x, y));
                            double num6 = !this.DependentItem.IsMilestone ? Math.Min(this.DependentItem.ComputedBarWidth / 3.0, num3) : (this.DependentItem.ComputedBarWidth / 2.0);
                            if (num6 < (x + num3))
                            {
                                x += num3;
                                points.Add(new Point(x, y));
                                y += num4;
                                points.Add(new Point(x, y));
                                points.Add(new Point(num6, y));
                            }
                            points.Add(new Point(num6, y));
                            points.Add(new Point(num6, (y <= 0.0) ? ((-this.DependentItem.ComputedBarHeight / 2.0) + (this.DependentItem.HasChildren ? (this.DependentItem.ComputedBarHeight * 0.2) : 0.0)) : ((this.DependentItem.ComputedBarHeight / 2.0) - (this.DependentItem.HasChildren ? (this.DependentItem.ComputedBarHeight * 0.0) : 0.0))));
                            goto Label_0509;
                        }
                    case DlhSoft.Windows.Controls.DependencyType.StartStart:
                        {
                            double num9 = num;
                            points.Add(new Point(num9, y));
                            double num10 = -num3;
                            if (num10 > (num9 - num3))
                            {
                                num9 -= num3;
                                points.Add(new Point(num9, y));
                                y += num4;
                                points.Add(new Point(num9, y));
                                points.Add(new Point(num10, y));
                            }
                            points.Add(new Point(num10, y));
                            points.Add(new Point(num10, 0.0));
                            points.Add(new Point(0.0, 0.0));
                            goto Label_0509;
                        }
                    case DlhSoft.Windows.Controls.DependencyType.FinishFinish:
                        {
                            double num7 = num + this.Item.ComputedBarWidth;
                            points.Add(new Point(num7, y));
                            double num8 = this.Item.ComputedBarWidth + num3;
                            if (num8 < (num7 + num3))
                            {
                                num7 += num3;
                                points.Add(new Point(num7, y));
                                y += num4;
                                points.Add(new Point(num7, y));
                                points.Add(new Point(num8, y));
                            }
                            points.Add(new Point(num8, y));
                            points.Add(new Point(num8, 0.0));
                            points.Add(new Point(this.DependentItem.ComputedBarWidth, 0.0));
                            goto Label_0509;
                        }
                    case DlhSoft.Windows.Controls.DependencyType.StartFinish:
                        {
                            double num11 = num;
                            points.Add(new Point(num11, y));
                            double num12 = !this.DependentItem.IsMilestone ? Math.Max((double)((this.DependentItem.ComputedBarWidth / 3.0) * 2.0), (double)(this.DependentItem.ComputedBarWidth - num3)) : (this.DependentItem.ComputedBarWidth / 2.0);
                            if (num12 > (num11 - num3))
                            {
                                num11 -= num3;
                                points.Add(new Point(num11, y));
                                y += num4;
                                points.Add(new Point(num11, y));
                                points.Add(new Point(num12, y));
                            }
                            points.Add(new Point(num12, y));
                            points.Add(new Point(num12, (y <= 0.0) ? ((-this.DependentItem.ComputedBarHeight / 2.0) + (this.DependentItem.HasChildren ? (this.DependentItem.ComputedBarHeight * 0.2) : 0.0)) : ((this.DependentItem.ComputedBarHeight / 2.0) - (this.DependentItem.HasChildren ? (this.DependentItem.ComputedBarHeight * 0.0) : 0.0))));
                            goto Label_0509;
                        }
                }
            }
            else
            {
                points = null;
            }
        Label_0509:
            this.ComputedDependencyLinePoints = points;
            _nextUpdate = null;
        }

        // Modifs Tekigo : Amélioration des perfs : 
        // Delay de la mise à jour dans le dispatcher
        // Cela permet de faire trop de calcul lorsque les entités sont très souvent modifiées.
        private System.Windows.Threading.DispatcherOperation _nextUpdate;
        internal void Update()
        {
            if (_nextUpdate == null)
                _nextUpdate = Dispatcher.BeginInvoke((Action)UpdateImpl, System.Windows.Threading.DispatcherPriority.Loaded);
        }

        public PointCollection ComputedDependencyLinePoints
        {
            get
            {
                return (PointCollection)base.GetValue(ComputedDependencyLinePointsProperty);
            }
            set
            {
                base.SetValue(ComputedDependencyLinePointsProperty, value);
            }
        }

        public DlhSoft.Windows.Controls.DependencyType DependencyType
        {
            get
            {
                return (DlhSoft.Windows.Controls.DependencyType)base.GetValue(DependencyTypeProperty);
            }
            set
            {
                base.SetValue(DependencyTypeProperty, value);
            }
        }

        public GanttChartItem DependentItem { get; internal set; }

        public DlhSoft.Windows.Controls.GanttChartView GanttChartView
        {
            get
            {
                if (this.DependentItem == null)
                {
                    return null;
                }
                return (this.DependentItem.GanttChartView as DlhSoft.Windows.Controls.GanttChartView);
            }
        }

        public GanttChartItem Item
        {
            get
            {
                return (GanttChartItem)base.GetValue(ItemProperty);
            }
            set
            {
                base.SetValue(ItemProperty, value);
            }
        }
    }
}

