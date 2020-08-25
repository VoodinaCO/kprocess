namespace DlhSoft.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Threading;


    public class DataTreeGrid : DataGrid
    {
        private int asyncItemCount;
        private DispatcherTimer asyncTimer;
        public static readonly DependencyProperty ExpanderTemplateProperty = DependencyProperty.Register("ExpanderTemplate", typeof(ControlTemplate), typeof(DataTreeGrid), new PropertyMetadata(null));
        public static readonly DependencyProperty IndentationUnitSizeProperty = DependencyProperty.Register("IndentationUnitSize", typeof(double), typeof(DataTreeGrid), new PropertyMetadata(16.0, new PropertyChangedCallback(DataTreeGrid.OnIndentationUnitSizeChanged)));
        private int internalUpdateItemsSourceCount;
        public static readonly DependencyProperty IsAsyncPresentationEnabledMinCountProperty = DependencyProperty.Register("IsAsyncPresentationEnabledMinCount", typeof(int), typeof(DataTreeGrid), new PropertyMetadata(0x100));
        public static readonly DependencyProperty IsAsyncPresentationEnabledPageSizeProperty = DependencyProperty.Register("IsAsyncPresentationEnabledPageSize", typeof(int), typeof(DataTreeGrid), new PropertyMetadata(0x10));
        public static readonly DependencyProperty IsAsyncPresentationEnabledProperty = DependencyProperty.Register("IsAsyncPresentationEnabled", typeof(bool), typeof(DataTreeGrid), new PropertyMetadata(true, new PropertyChangedCallback(DataTreeGrid.OnIsAsyncPresentationEnabledChanged)));
        private bool isAsyncTimerPaused;
        private bool isTemplateApplied;
        private DataTreeGridItemCollection items;
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<DataTreeGridItem>), typeof(DataTreeGrid), new PropertyMetadata(null, new PropertyChangedCallback(DataTreeGrid.OnItemsChanged)));
        private ObservableCollection<DataTreeGridItem> originalItems;

        public event NotifyCollectionChangedEventHandler ItemCollectionChanged;

        public event PropertyChangedEventHandler ItemPropertyChanged;

        public DataTreeGrid()
        {
            try
            {
                LicenseManager.Validate(typeof(DataTreeGrid));
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
            base.CanUserDeleteRows = false;
            this.Items = new ObservableCollection<DataTreeGridItem>();
        }

        private void AsyncTimer_Tick(object sender, EventArgs e)
        {
            if (this.asyncItemCount >= this.originalItems.Count)
            {
                this.asyncTimer.Stop();
            }
            else
            {
                for (int i = 0; i < Math.Min(this.IsAsyncPresentationEnabledPageSize, this.originalItems.Count - this.asyncItemCount); i++)
                {
                    this.items.Add(this.originalItems[this.asyncItemCount++]);
                }
            }
        }

        public void AttachItem(DataTreeGridItem item)
        {
            item.DataTreeGrid = this;
            item.UpdateIndentationWidth();
        }

        public void CollapseAll()
        {
            if (this.items != null)
            {
                foreach (DataTreeGridItem item in this.items)
                {
                    if (item.HasChildren)
                    {
                        item.IsExpanded = false;
                    }
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

        public void ExpandAll()
        {
            if (this.items != null)
            {
                foreach (DataTreeGridItem item in this.items)
                {
                    if (item.HasChildren)
                    {
                        item.IsExpanded = true;
                    }
                }
            }
        }

        public IEnumerable<DataTreeGridItem> GetAllChildren(DataTreeGridItem item)
        {
            if (this.items == null)
            {
                return null;
            }
            return this.items.GetAllChildren(item);
        }

        public IEnumerable<DataTreeGridItem> GetAllChildren(int index)
        {
            if (this.items == null)
            {
                return null;
            }
            return this.items.GetAllChildren(index);
        }

        public IEnumerable<DataTreeGridItem> GetAllParents(DataTreeGridItem item)
        {
            if (this.items == null)
            {
                return null;
            }
            return this.items.GetAllParents(item);
        }

        public IEnumerable<DataTreeGridItem> GetAllParents(int index)
        {
            if (this.items == null)
            {
                return null;
            }
            return this.items.GetAllParents(index);
        }

        public IEnumerable<DataTreeGridItem> GetChildren(DataTreeGridItem item)
        {
            if (this.items == null)
            {
                return null;
            }
            return this.items.GetChildren(item);
        }

        public IEnumerable<DataTreeGridItem> GetChildren(int index)
        {
            if (this.items == null)
            {
                return null;
            }
            return this.items.GetChildren(index);
        }

        public DataTreeGridItem GetParent(DataTreeGridItem item)
        {
            if (this.items == null)
            {
                return null;
            }
            return this.items.GetParent(item);
        }

        public DataTreeGridItem GetParent(int index)
        {
            if (this.items == null)
            {
                return null;
            }
            return this.items.GetParent(index);
        }

        public int IndexOf(DataTreeGridItem item)
        {
            if (this.items == null)
            {
                return -1;
            }
            return this.items.IndexOf(item);
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
                //b.a(this, typeof(DataTreeGrid), Assembly.GetCallingAssembly());
            }
            bool isTemplateApplied = this.isTemplateApplied;
            this.isTemplateApplied = true;
            if (!isTemplateApplied)
            {
                this.UpdateItemsSource();
            }
            this.UpdateIndentationUnitSizes();
        }

        // Modif Tekigo
        /// <summary>
        /// Empeche le changement de ligne selectionnée à la validation
        /// </summary>
        public bool PreventRowAutoChangeOnValidation { get; set; }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.PreventRowAutoChangeOnValidation)
            {
                var selectedIndex = this.SelectedIndex;
                base.OnKeyDown(e);
                if (e.Key == Key.Enter || e.Key == Key.Return)
                {
                    this.SelectedIndex = selectedIndex;
                }
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        private static void OnIndentationUnitSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataTreeGrid grid = d as DataTreeGrid;
            if (grid != null)
            {
                grid.UpdateIndentationUnitSizes();
            }
        }

        private static void OnIsAsyncPresentationEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataTreeGrid grid = d as DataTreeGrid;
            if (grid != null)
            {
                grid.UpdateItemsSource();
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
            DataTreeGrid grid = d as DataTreeGrid;
            if (grid != null)
            {
                grid.UpdateItemsSource();
            }
        }

        protected override void OnLoadingRow(DataGridRowEventArgs e)
        {
            base.OnLoadingRow(e);
            e.Row.SetBinding(UIElement.VisibilityProperty, new Binding("Visibility"));
        }

        protected override void OnUnloadingRow(DataGridRowEventArgs e)
        {
            e.Row.ClearValue(UIElement.VisibilityProperty);
            base.OnUnloadingRow(e);
        }

        private void OriginalItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (DataTreeGridItem item in e.NewItems)
                    {
                        this.AttachItem(item);
                    }
                    if (this.IsAsyncPresentationEnabled)
                    {
                        if (e.NewStartingIndex < this.asyncItemCount)
                        {
                            for (int j = 0; j < e.NewItems.Count; j++)
                            {
                                this.items.Insert(e.NewStartingIndex + j, e.NewItems[j] as DataTreeGridItem);
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
                        this.items.Insert(e.NewStartingIndex + i, e.NewItems[i] as DataTreeGridItem);
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
                this.isAsyncTimerPaused = true;
            }
        }

        internal void UpdateIndentationUnitSizes()
        {
            if (this.isTemplateApplied && (this.items != null))
            {
                foreach (DataTreeGridItem item in this.items)
                {
                    item.UpdateIndentationWidth();
                }
            }
        }

        private void UpdateItemsSource()
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
                    foreach (DataTreeGridItem item in this.originalItems)
                    {
                        this.AttachItem(item);
                    }
                    this.items = new DataTreeGridItemCollection();
                    if (!this.IsAsyncPresentationEnabled)
                    {
                        this.asyncItemCount = -1;
                        foreach (DataTreeGridItem item2 in this.originalItems)
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

        public DataTreeGridItem this[int index]
        {
            get
            {
                if (this.items == null)
                {
                    return null;
                }
                return this.items[index];
            }
        }

        public new ObservableCollection<DataTreeGridItem> Items
        {
            get
            {
                return (ObservableCollection<DataTreeGridItem>)base.GetValue(ItemsProperty);
            }
            set
            {
                base.SetValue(ItemsProperty, value);
            }
        }
    }
}

