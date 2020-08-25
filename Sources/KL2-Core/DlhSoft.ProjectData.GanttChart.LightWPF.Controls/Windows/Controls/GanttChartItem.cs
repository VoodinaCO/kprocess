namespace DlhSoft.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows;

    public class GanttChartItem : DataTreeGridItem, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ActualDisplayRowIndexProperty = DependencyProperty.Register("ActualDisplayRowIndex", typeof(int), typeof(GanttChartItem), new PropertyMetadata(0, new PropertyChangedCallback(OnActualDisplayRowIndexChanged)));
        public static readonly DependencyProperty AssignmentsContentProperty = DependencyProperty.Register("AssignmentsContent", typeof(object), typeof(GanttChartItem), new PropertyMetadata(null, new PropertyChangedCallback(OnAssignmentsContentChanged)));
        public static readonly DependencyProperty CompletedFinishProperty = DependencyProperty.Register("CompletedFinish", typeof(DateTime), typeof(GanttChartItem), new PropertyMetadata(DateTime.Today, new PropertyChangedCallback(OnCompletedFinishChanged)));
        public static readonly DependencyProperty ComputedBarHeightProperty = DependencyProperty.Register("ComputedBarHeight", typeof(double), typeof(GanttChartItem), new PropertyMetadata(0.0));
        public static readonly DependencyProperty ComputedBarLeftProperty = DependencyProperty.Register("ComputedBarLeft", typeof(double), typeof(GanttChartItem), new PropertyMetadata(0.0));
        public static readonly DependencyProperty ComputedBarWidthProperty = DependencyProperty.Register("ComputedBarWidth", typeof(double), typeof(GanttChartItem), new PropertyMetadata(0.0));
        public static readonly DependencyProperty ComputedCompletedBarWidthProperty = DependencyProperty.Register("ComputedCompletedBarWidth", typeof(double), typeof(GanttChartItem), new PropertyMetadata(0.0));
        public static readonly DependencyProperty ComputedItemTopProperty = DependencyProperty.Register("ComputedItemTop", typeof(double), typeof(GanttChartItem), new PropertyMetadata(0.0));
        public static readonly DependencyProperty ComputedTaskTemplateProperty = DependencyProperty.Register("ComputedTaskTemplate", typeof(DataTemplate), typeof(GanttChartItem), new PropertyMetadata(null));
        public static readonly DependencyProperty DisplayRowIndexProperty = DependencyProperty.Register("DisplayRowIndex", typeof(int?), typeof(GanttChartItem), new PropertyMetadata(null, new PropertyChangedCallback(OnDisplayRowIndexChanged)));
        public static readonly DependencyProperty ExpansionVisibilityProperty = DependencyProperty.Register("ExpansionVisibility", typeof(Visibility), typeof(GanttChartItem), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty FinishProperty = DependencyProperty.Register("Finish", typeof(DateTime), typeof(GanttChartItem), new PropertyMetadata(DateTime.Today, new PropertyChangedCallback(OnFinishChanged)));
        private IGanttChartView ganttChartView;
        public static readonly DependencyProperty IsCompletedProperty = DependencyProperty.Register("IsCompleted", typeof(bool), typeof(GanttChartItem), new PropertyMetadata(false, new PropertyChangedCallback(OnIsCompletedChanged)));
        private bool isDuringCoerceTiming;
        public static readonly DependencyProperty IsMilestoneProperty = DependencyProperty.Register("IsMilestone", typeof(bool), typeof(GanttChartItem), new PropertyMetadata(false, new PropertyChangedCallback(OnIsMilestoneChanged)));
        public static readonly DependencyProperty IsVirtuallyVisibleProperty = DependencyProperty.Register("IsVirtuallyVisible", typeof(bool), typeof(GanttChartItem), new PropertyMetadata(false, new PropertyChangedCallback(OnIsVirtuallyVisibleChanged)));
        private PredecessorItemCollection managedPredecessorItems;
        public static readonly DependencyProperty PredecessorsProperty = DependencyProperty.Register("Predecessors", typeof(PredecessorItemCollection), typeof(GanttChartItem), new PropertyMetadata(null, new PropertyChangedCallback(OnPredecessorsChanged)));
        public static readonly DependencyProperty StartProperty = DependencyProperty.Register("Start", typeof(DateTime), typeof(GanttChartItem), new PropertyMetadata(DateTime.Today, new PropertyChangedCallback(OnStartChanged)));
        public static readonly DependencyProperty VirtualVisibilityProperty = DependencyProperty.Register("VirtualVisibility", typeof(Visibility), typeof(GanttChartItem), new PropertyMetadata(Visibility.Collapsed));

        public event EventHandler ActualDisplayRowIndexChanged;

        public event EventHandler AssignmentsChanged;

        public event EventHandler DependenciesChanged;

        public event EventHandler DisplayRowIndexChanged;

        public event EventHandler PredecessorsChanged;

        public event EventHandler TimingChanged;

        public GanttChartItem()
        {
            this.Predecessors = new PredecessorItemCollection();
        }

        private void CoerceCompletion()
        {
            if (this.IsCompleted)
            {
                this.CompletedFinish = this.Finish;
            }
            else if (this.CompletedFinish == this.Finish)
            {
                this.CompletedFinish = this.Start;
            }
        }

        private void CoerceFinish()
        {
            if (((this.Finish > this.Start) && this.IsMilestone) && !base.HasChildren)
            {
                this.IsMilestone = false;
            }
        }

        private void CoerceTiming()
        {
            if ((!this.isDuringCoerceTiming && !base.HasChildren) && (this.GanttChartView != null))
            {
                this.isDuringCoerceTiming = true;
                this.Start = this.GanttChartView.GetNextWorkingTime(this.Start);
                if (!this.IsMilestone)
                {
                    this.Finish = this.GanttChartView.GetPreviousWorkingTime(this.Finish);
                    this.CompletedFinish = this.GanttChartView.GetPreviousWorkingTime(this.CompletedFinish);
                }
                this.isDuringCoerceTiming = false;
            }
            if (this.IsMilestone && !base.HasChildren)
            {
                this.Finish = this.Start;
            }
            if (this.Finish < this.Start)
            {
                this.Finish = this.Start;
            }
            if (this.CompletedFinish < this.Start)
            {
                this.CompletedFinish = this.Start;
            }
            if (this.CompletedFinish > this.Finish)
            {
                this.CompletedFinish = this.Finish;
            }
            if ((this.Finish > this.Start) && !base.HasChildren)
            {
                this.IsMilestone = false;
            }
        }

        private void ManagedPredecessorItems_Changed(object sender, EventArgs e)
        {
            this.OnPropertyChanged("Predecessors");
            this.OnDependenciesChanged();
        }

        private void ManagedPredecessorItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("Predecessors");
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.SetManagedPredecessorItems(e.NewItems);
                    return;

                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    this.SetManagedPredecessorItems(this.managedPredecessorItems);
                    break;

                default:
                    return;
            }
        }

        protected virtual void OnActualDisplayRowIndexChanged()
        {
            if (this.ActualDisplayRowIndexChanged != null)
            {
                this.ActualDisplayRowIndexChanged(this, EventArgs.Empty);
            }
        }

        private static void OnActualDisplayRowIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartItem item = d as GanttChartItem;
            if (item != null)
            {
                item.OnPropertyChanged("ActualDisplayRowIndex");
                item.OnActualDisplayRowIndexChanged();
                item.UpdateComputedItemTop();
                item.UpdateBar();
                item.UpdateDependencyLines();
            }
        }

        protected virtual void OnAssignmentsChanged()
        {
            if (this.AssignmentsChanged != null)
            {
                this.AssignmentsChanged(this, EventArgs.Empty);
            }
        }

        private static void OnAssignmentsContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartItem item = d as GanttChartItem;
            if (item != null)
            {
                item.OnPropertyChanged("AssignmentsContent");
                item.OnAssignmentsChanged();
            }
        }

        private static void OnCompletedFinishChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartItem item = d as GanttChartItem;
            if (item != null)
            {
                item.OnPropertyChanged("CompletedFinish");
                item.OnTimingChanged();
                item.UpdateIsCompleted();
            }
        }

        protected virtual void OnDependenciesChanged()
        {
            this.UpdateDependencyLines();
            if (this.DependenciesChanged != null)
            {
                this.DependenciesChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnDisplayRowIndexChanged()
        {
            if (this.DisplayRowIndexChanged != null)
            {
                this.DisplayRowIndexChanged(this, EventArgs.Empty);
            }
        }

        private static void OnDisplayRowIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartItem item = d as GanttChartItem;
            if (item != null)
            {
                item.OnPropertyChanged("DisplayRowIndex");
                item.OnDisplayRowIndexChanged();
            }
        }

        protected override void OnExpansionChanged()
        {
            base.OnExpansionChanged();
            this.ExpansionVisibility = base.IsExpanded ? Visibility.Collapsed : Visibility.Visible;
        }

        private static void OnFinishChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartItem item = d as GanttChartItem;
            if (item != null)
            {
                item.OnPropertyChanged("Finish");
                item.CoerceFinish();
                item.OnTimingChanged();
                item.UpdateDependencyLines();
                item.UpdateIsCompleted();
            }
        }

        private static void OnIsCompletedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartItem item = d as GanttChartItem;
            if (item != null)
            {
                item.OnPropertyChanged("IsCompleted");
                item.CoerceCompletion();
            }
        }

        private static void OnIsMilestoneChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartItem item = d as GanttChartItem;
            if (item != null)
            {
                item.OnPropertyChanged("IsMilestone");
                item.UpdateComputedTaskTemplate();
                item.OnTimingChanged();
                item.UpdateDependencyLines();
                item.UpdateIsCompleted();
            }
        }

        private static void OnIsVirtuallyVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartItem item = d as GanttChartItem;
            if (item != null)
            {
                item.VirtualVisibility = item.IsVirtuallyVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        protected virtual void OnPredecessorsChanged()
        {
            if (this.managedPredecessorItems != null)
            {
                this.managedPredecessorItems.Changed -= new EventHandler(this.ManagedPredecessorItems_Changed);
                this.managedPredecessorItems.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.ManagedPredecessorItems_CollectionChanged);
            }
            this.managedPredecessorItems = this.Predecessors;
            if (this.managedPredecessorItems != null)
            {
                this.SetManagedPredecessorItems(this.managedPredecessorItems);
                this.managedPredecessorItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.ManagedPredecessorItems_CollectionChanged);
                this.managedPredecessorItems.Changed += new EventHandler(this.ManagedPredecessorItems_Changed);
            }
            this.OnDependenciesChanged();
            if (this.PredecessorsChanged != null)
            {
                this.PredecessorsChanged(this, EventArgs.Empty);
            }
        }

        private static void OnPredecessorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartItem item = d as GanttChartItem;
            if (item != null)
            {
                item.OnPropertyChanged("Predecessors");
                item.OnPredecessorsChanged();
            }
        }

        private static void OnStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttChartItem item = d as GanttChartItem;
            if (item != null)
            {
                item.OnPropertyChanged("Start");
                item.OnTimingChanged();
                item.UpdateDependencyLines();
            }
        }

        protected virtual void OnTimingChanged()
        {
            this.CoerceTiming();
            this.UpdateBar();
            if (this.TimingChanged != null)
            {
                this.TimingChanged(this, EventArgs.Empty);
            }
        }

        private void SetManagedPredecessorItems(IList predecessorItems)
        {
            foreach (PredecessorItem item in predecessorItems)
            {
                item.DependentItem = this;
            }
        }

        internal void UpdateBar()
        {
            if (this.GanttChartView != null)
            {
                double barHeight = this.GanttChartView.BarHeight;
                double num2 = barHeight;
                double position = this.GanttChartView.GetPosition(this.Start);
                double num4 = this.GanttChartView.GetPosition(this.Finish) - position;
                double num5 = this.GanttChartView.GetPosition(this.CompletedFinish) - position;
                if (base.HasChildren || this.IsMilestone)
                {
                    position -= barHeight / 2.0;
                    num4 += barHeight;
                    if (base.HasChildren)
                    {
                        if (num4 < (2.0 * barHeight))
                        {
                            num4 = 2.0 * barHeight;
                        }
                        num2 += barHeight / 3.0;
                    }
                }
                if (num4 < 4.0)
                {
                    num4 = 4.0;
                }
                this.ComputedBarHeight = num2;
                this.ComputedBarLeft = position;
                this.ComputedBarWidth = num4;
                this.ComputedCompletedBarWidth = num5;
            }
        }

        internal void UpdateComputedItemTop()
        {
            if ((this.GanttChartView != null) && !double.IsNaN(this.GanttChartView.ItemHeight))
            {
                this.ComputedItemTop = this.ActualDisplayRowIndex * this.GanttChartView.ItemHeight;
            }
        }

        internal void UpdateComputedTaskTemplate()
        {
            if (this.GanttChartView != null)
            {
                this.ComputedTaskTemplate = base.HasChildren ? this.GanttChartView.SummaryTaskTemplate : (!this.IsMilestone ? this.GanttChartView.StandardTaskTemplate : this.GanttChartView.MilestoneTaskTemplate);
            }
        }

        internal void UpdateDependencyLines()
        {
            if ((this.GanttChartView != null) && (this.Predecessors != null))
            {
                foreach (PredecessorItem item in this.Predecessors)
                {
                    item.Update();
                }
            }
        }

        internal void UpdateIsCompleted()
        {
            if (this.Finish > this.Start)
            {
                this.IsCompleted = this.CompletedFinish == this.Finish;
            }
        }

        public int ActualDisplayRowIndex
        {
            get
            {
                return (int) base.GetValue(ActualDisplayRowIndexProperty);
            }
            set
            {
                base.SetValue(ActualDisplayRowIndexProperty, value);
            }
        }

        public object AssignmentsContent
        {
            get
            {
                return base.GetValue(AssignmentsContentProperty);
            }
            set
            {
                base.SetValue(AssignmentsContentProperty, value);
            }
        }

        public TimeSpan CompletedEffort
        {
            get
            {
                if (this.ganttChartView == null)
                {
                    return TimeSpan.Zero;
                }
                return this.ganttChartView.GetEffort(this.Start, this.CompletedFinish);
            }
            set
            {
                if (this.ganttChartView != null)
                {
                    this.CompletedFinish = this.ganttChartView.GetFinish(this.Start, value);
                }
            }
        }

        public DateTime CompletedFinish
        {
            get
            {
                return (DateTime) base.GetValue(CompletedFinishProperty);
            }
            set
            {
                base.SetValue(CompletedFinishProperty, value);
            }
        }

        public double ComputedBarHeight
        {
            get
            {
                return (double) base.GetValue(ComputedBarHeightProperty);
            }
            set
            {
                base.SetValue(ComputedBarHeightProperty, value);
            }
        }

        public double ComputedBarLeft
        {
            get
            {
                return (double) base.GetValue(ComputedBarLeftProperty);
            }
            set
            {
                base.SetValue(ComputedBarLeftProperty, value);
            }
        }

        public double ComputedBarWidth
        {
            get
            {
                return (double) base.GetValue(ComputedBarWidthProperty);
            }
            set
            {
                base.SetValue(ComputedBarWidthProperty, value);
            }
        }

        public double ComputedCompletedBarWidth
        {
            get
            {
                return (double) base.GetValue(ComputedCompletedBarWidthProperty);
            }
            set
            {
                base.SetValue(ComputedCompletedBarWidthProperty, value);
            }
        }

        public double ComputedItemTop
        {
            get
            {
                return (double) base.GetValue(ComputedItemTopProperty);
            }
            set
            {
                base.SetValue(ComputedItemTopProperty, value);
            }
        }

        public DataTemplate ComputedTaskTemplate
        {
            get
            {
                return (DataTemplate) base.GetValue(ComputedTaskTemplateProperty);
            }
            set
            {
                base.SetValue(ComputedTaskTemplateProperty, value);
            }
        }

        public int? DisplayRowIndex
        {
            get
            {
                return (int?) base.GetValue(DisplayRowIndexProperty);
            }
            set
            {
                base.SetValue(DisplayRowIndexProperty, value);
            }
        }

        public TimeSpan Effort
        {
            get
            {
                if (this.ganttChartView == null)
                {
                    return TimeSpan.Zero;
                }
                return this.ganttChartView.GetEffort(this.Start, this.Finish);
            }
            set
            {
                if (this.ganttChartView != null)
                {
                    this.Finish = this.ganttChartView.GetFinish(this.Start, value);
                }
            }
        }

        public Visibility ExpansionVisibility
        {
            get
            {
                return (Visibility) base.GetValue(ExpansionVisibilityProperty);
            }
            set
            {
                base.SetValue(ExpansionVisibilityProperty, value);
            }
        }

        public DateTime Finish
        {
            get
            {
                return (DateTime) base.GetValue(FinishProperty);
            }
            set
            {
                base.SetValue(FinishProperty, value);
            }
        }

        public IGanttChartView GanttChartView
        {
            get
            {
                return this.ganttChartView;
            }
            internal set
            {
                this.ganttChartView = value;
                this.CoerceTiming();
            }
        }

        public bool IsCompleted
        {
            get
            {
                return (bool) base.GetValue(IsCompletedProperty);
            }
            set
            {
                base.SetValue(IsCompletedProperty, value);
            }
        }

        public bool IsMilestone
        {
            get
            {
                return (bool) base.GetValue(IsMilestoneProperty);
            }
            set
            {
                base.SetValue(IsMilestoneProperty, value);
            }
        }

        public bool IsVirtuallyVisible
        {
            get
            {
                return (bool) base.GetValue(IsVirtuallyVisibleProperty);
            }
            set
            {
                base.SetValue(IsVirtuallyVisibleProperty, value);
            }
        }

        public PredecessorItemCollection Predecessors
        {
            get
            {
                return (PredecessorItemCollection) base.GetValue(PredecessorsProperty);
            }
            set
            {
                base.SetValue(PredecessorsProperty, value);
            }
        }

        public DateTime Start
        {
            get
            {
                return (DateTime) base.GetValue(StartProperty);
            }
            set
            {
                base.SetValue(StartProperty, value);
            }
        }

        public Visibility VirtualVisibility
        {
            get
            {
                return (Visibility) base.GetValue(VirtualVisibilityProperty);
            }
            set
            {
                base.SetValue(VirtualVisibilityProperty, value);
            }
        }

        #region Tekigo

        /// <summary>
        /// Obtient ou définit la visibilité du Thumb de création de dépendance.
        /// </summary>
        public Visibility DependencyCreationThumbVisibility
        {
            get { return (Visibility)GetValue(DependencyCreationThumbVisibilityProperty); }
            set { SetValue(DependencyCreationThumbVisibilityProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="DependencyCreationThumbVisibility"/>.
        /// </summary>
        public static readonly DependencyProperty DependencyCreationThumbVisibilityProperty =
            DependencyProperty.Register("DependencyCreationThumbVisibility", typeof(Visibility), typeof(GanttChartItem), new PropertyMetadata(Visibility.Visible));

        #endregion
    }
}

