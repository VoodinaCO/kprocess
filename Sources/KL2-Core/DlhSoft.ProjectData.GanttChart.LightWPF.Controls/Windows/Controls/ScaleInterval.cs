namespace DlhSoft.Windows.Controls
{
    using DlhSoft.Windows.Data;
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;

    public class ScaleInterval : DependencyObject
    {
        public static readonly DependencyProperty ComputedLeftProperty = DependencyProperty.Register("ComputedLeft", typeof(double), typeof(ScaleInterval), new PropertyMetadata(0.0));
        public static readonly DependencyProperty ComputedWidthProperty = DependencyProperty.Register("ComputedWidth", typeof(double), typeof(ScaleInterval), new PropertyMetadata(0.0));
        public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.Register("HeaderContent", typeof(object), typeof(ScaleInterval), new PropertyMetadata(null));
        private DlhSoft.Windows.Controls.Scale scale;

        public ScaleInterval(DateTime start, DateTime finish)
        {
            this.TimeInterval = new DlhSoft.Windows.Data.TimeInterval(start, finish);
        }

        internal ScaleInterval(DateTime start, DateTime finish, double startPosition, double finishPosition) : this(start, finish)
        {
            this.ComputedLeft = startPosition;
            this.ComputedWidth = finishPosition - startPosition;
        }

        internal void UpdateBar()
        {
            if (this.GanttChartView != null)
            {
                this.ComputedLeft = this.GanttChartView.GetPosition(this.TimeInterval.Start);
                this.ComputedWidth = this.GanttChartView.GetPosition(this.TimeInterval.Finish) - this.ComputedLeft;
            }
        }

        public double ComputedLeft
        {
            get
            {
                return (double) base.GetValue(ComputedLeftProperty);
            }
            set
            {
                base.SetValue(ComputedLeftProperty, value);
            }
        }

        public double ComputedWidth
        {
            get
            {
                return (double) base.GetValue(ComputedWidthProperty);
            }
            set
            {
                base.SetValue(ComputedWidthProperty, value);
            }
        }

        public DlhSoft.Windows.Controls.GanttChartView GanttChartView
        {
            get
            {
                if (this.Scale == null)
                {
                    return null;
                }
                return this.Scale.GanttChartView;
            }
        }

        public object HeaderContent
        {
            get
            {
                return base.GetValue(HeaderContentProperty);
            }
            set
            {
                base.SetValue(HeaderContentProperty, value);
            }
        }

        public DlhSoft.Windows.Controls.Scale Scale
        {
            get
            {
                return this.scale;
            }
            internal set
            {
                this.scale = value;
                this.UpdateBar();
            }
        }

        public DlhSoft.Windows.Data.TimeInterval TimeInterval { get; private set; }
    }
}

