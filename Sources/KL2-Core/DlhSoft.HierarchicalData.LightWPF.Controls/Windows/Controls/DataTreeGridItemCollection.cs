namespace DlhSoft.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class DataTreeGridItemCollection : ObservableCollection<DataTreeGridItem>
    {
        private List<DataTreeGridItem> managedItems = new List<DataTreeGridItem>();

        public event PropertyChangedEventHandler ItemPropertyChanged;

        private void AddManagedItems(IList items)
        {
            foreach (DataTreeGridItem item in items)
            {
                if (!this.managedItems.Contains(item))
                {
                    this.managedItems.Add(item);
                }
                item.HierarchyChanged += new EventHandler(this.Item_HierarchyChanged);
                item.ExpansionChanged += new EventHandler(this.Item_ExpansionChanged);
                item.PropertyChanged += new PropertyChangedEventHandler(this.Item_PropertyChanged);
            }
        }

        private void ClearManagedItems()
        {
            foreach (DataTreeGridItem item in this.managedItems)
            {
                item.PropertyChanged -= new PropertyChangedEventHandler(this.Item_PropertyChanged);
                item.HierarchyChanged -= new EventHandler(this.Item_HierarchyChanged);
                item.ExpansionChanged -= new EventHandler(this.Item_ExpansionChanged);
            }
            this.managedItems.Clear();
        }

        public IEnumerable<DataTreeGridItem> GetAllChildren(DataTreeGridItem item)
        {
            return this.GetAllChildren(base.IndexOf(item));
        }

        public IEnumerable<DataTreeGridItem> GetAllChildren(int index)
        {
            if ((index >= 0) && (index < this.Count))
            {
                var item = this[index];
                var indentation = item.Indentation;
                if (indentation >= 0)
                {
                    for (int i = index + 1; i < this.Count; i++)
                    {
                        item = this[i];
                        if (item.Indentation > indentation)
                            yield return item;
                        else
                            break;
                    }
                }
            }
        }

        public IEnumerable<DataTreeGridItem> GetAllParents(DataTreeGridItem item)
        {
            return this.GetAllParents(base.IndexOf(item));
        }

        public IEnumerable<DataTreeGridItem> GetAllParents(int index)
        {
            if (index >= 0 && index < this.Count)
            {
                var item = this[index];
                var indentation = item.Indentation;
                if (indentation > 0)
                {
                    while (index-- > 0)
                    {
                        item = this[index];

                        if (item.Indentation < indentation)
                            yield return item;

                        indentation = item.Indentation;
                        if (item.Indentation <= 0)
                            break;
                    }
                }
            }
        }

        public IEnumerable<DataTreeGridItem> GetChildren(DataTreeGridItem item)
        {
            return this.GetChildren(base.IndexOf(item));
        }

        public IEnumerable<DataTreeGridItem> GetChildren(int index)
        {
            if (index >= 0 && index < this.Count)
            {
                var item = this[index];
                var indentation = item.Indentation;
                if (indentation >= 0)
                {
                    while (++index < this.Count)
                    {
                        item = this[index];
                        if (item.Indentation > indentation)
                        {
                            if (item.Indentation == indentation + 1)
                                yield return item;
                        }
                        else
                            break;
                    }
                }
            }
        }

        public DataTreeGridItem GetParent(DataTreeGridItem item)
        {
            return this.GetParent(base.IndexOf(item));
        }

        public DataTreeGridItem GetParent(int index)
        {
            if ((index >= 0) && (index < base.Count))
            {
                DataTreeGridItem item = base[index];
                int indentation = item.Indentation;
                if (indentation > 0)
                {
                    while (index-- > 0)
                    {
                        item = base[index];
                        if (item.Indentation < indentation)
                        {
                            return item;
                        }
                        if (item.Indentation <= 0)
                        {
                            break;
                        }
                    }
                    return null;
                }
            }
            return null;
        }

        private void Item_ExpansionChanged(object sender, EventArgs e)
        {
            DataTreeGridItem item = (DataTreeGridItem)sender;
            int index = base.IndexOf(item);
            this.UpdateVisibilityFromExpansion(index);
        }

        private void Item_HierarchyChanged(object sender, EventArgs e)
        {
            DataTreeGridItem item = (DataTreeGridItem)sender;
            int index = base.IndexOf(item);
            this.UpdateHierarchy(index, 1);
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnItemPropertyChanged(sender, e);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.AddManagedItems(e.NewItems);
                    this.UpdateHierarchy(e.NewStartingIndex, e.NewItems.Count);
                    this.UpdateVisibility(e.NewStartingIndex, e.NewItems.Count);
                    return;

                case NotifyCollectionChangedAction.Remove:
                    this.RemoveManagedItems(e.OldItems);
                    this.UpdateHierarchy(e.OldStartingIndex, 0);
                    return;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    this.ClearManagedItems();
                    this.AddManagedItems(this);
                    this.UpdateHierarchy(0, base.Count);
                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                default:
                    return;
            }
        }

        protected virtual void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.ItemPropertyChanged != null)
            {
                this.ItemPropertyChanged(sender, e);
            }
        }

        private void RemoveManagedItems(IList items)
        {
            foreach (DataTreeGridItem item in items)
            {
                item.PropertyChanged -= new PropertyChangedEventHandler(this.Item_PropertyChanged);
                item.ExpansionChanged -= new EventHandler(this.Item_ExpansionChanged);
                item.HierarchyChanged -= new EventHandler(this.Item_HierarchyChanged);
                this.managedItems.Remove(item);
            }
        }

        internal void UpdateHierarchy(int index, int count)
        {
            if ((index >= 0) && (index <= base.Count))
            {
                DataTreeGridItem item = (index < base.Count) ? base[index] : null;
                int indentation = (item != null) ? item.Indentation : 0;
                if (index > 0)
                {
                    DataTreeGridItem item2 = base[index - 1];
                    bool flag = item2.Indentation < indentation;
                    if (item2.HasChildren != flag)
                    {
                        item2.HasChildren = flag;
                    }
                }
                for (int i = index; i < (index + count); i++)
                {
                    if (i < (base.Count - 1))
                    {
                        DataTreeGridItem item3 = base[i + 1];
                        bool flag2 = indentation < item3.Indentation;
                        if (item.HasChildren != flag2)
                        {
                            item.HasChildren = flag2;
                        }
                        item = item3;
                        indentation = item.Indentation;
                    }
                    else if (item != null)
                    {
                        item.HasChildren = false;
                        return;
                    }
                }
            }
        }

        internal void UpdateVisibility(int index, int count)
        {
            if ((index >= 0) && (index < base.Count))
            {
                DataTreeGridItem item = base[index];
                int indentation = item.Indentation;
                int num2 = index;
                while (num2 >= 0)
                {
                    DataTreeGridItem item2 = base[num2];
                    if (item2.Indentation < indentation)
                    {
                        break;
                    }
                    num2--;
                }
                DataTreeGridItem item3 = (num2 >= 0) ? base[num2] : null;
                for (int i = index; i < (index + count); i++)
                {
                    DataTreeGridItem item4 = base[i];
                    item4.IsVisible = (item3 == null) || item3.IsExpanded;
                }
            }
        }

        internal void UpdateVisibilityFromExpansion(int index)
        {
            if ((index >= 0) && (index < base.Count))
            {
                DataTreeGridItem item = base[index];
                int indentation = item.Indentation;
                if (!item.IsExpanded)
                {
                    while (++index < base.Count)
                    {
                        item = base[index];
                        if (item.Indentation <= indentation)
                        {
                            return;
                        }
                        item.IsVisible = false;
                    }
                }
                else
                {
                    int num2 = indentation + (item.IsVisible ? 1 : 0);
                    while (++index < base.Count)
                    {
                        item = base[index];
                        int num3 = item.Indentation;
                        if (num3 <= indentation)
                        {
                            return;
                        }
                        bool flag = num3 <= num2;
                        item.IsVisible = flag;
                        if (flag)
                        {
                            num2 = num3 + (item.IsExpanded ? 1 : 0);
                        }
                    }
                }
            }
        }
    }
}

