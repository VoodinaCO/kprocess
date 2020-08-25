namespace DlhSoft.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class GanttChartItemCollection : ObservableCollection<GanttChartItem>
    {
        private List<GanttChartItem> managedItems = new List<GanttChartItem>();
        private Dictionary<GanttChartItem, List<GanttChartItem>> managedSuccessorItems = new Dictionary<GanttChartItem, List<GanttChartItem>>();

        public event PropertyChangedEventHandler ItemPropertyChanged;

        private void AddManagedItems(int index, IList items)
        {
            int num = 0;
            int num2 = index;
            while (num2-- > 0)
            {
                GanttChartItem item = this.managedItems[num2];
                if (item.IsVisible)
                {
                    num = item.ActualDisplayRowIndex + 1;
                    break;
                }
            }
            foreach (GanttChartItem item2 in items)
            {
                if (!this.managedItems.Contains(item2))
                {
                    this.managedItems.Insert(index++, item2);
                }
                item2.HierarchyChanged += new EventHandler(this.Item_HierarchyChanged);
                item2.ExpansionChanged += new EventHandler(this.Item_ExpansionChanged);
                if (item2.IsVisible)
                {
                    if (item2.DisplayRowIndex.HasValue)
                    {
                        item2.ActualDisplayRowIndex = item2.DisplayRowIndex.Value;
                    }
                    else
                    {
                        item2.ActualDisplayRowIndex = num++;
                    }
                }
                item2.VisibilityChanged += new EventHandler(this.Item_VisibilityChanged);
                item2.DisplayRowIndexChanged += new EventHandler(this.Item_DisplayRowIndexChanged);
                item2.ActualDisplayRowIndexChanged += new EventHandler(this.Item_ActualDisplayRowIndexChanged);
                item2.HasChildrenChanged += new EventHandler(this.Item_HasChildrenChanged);
                item2.TimingChanged += new EventHandler(this.Item_TimingChanged);
                item2.DependenciesChanged += new EventHandler(this.Item_DependenciesChanged);
                item2.PropertyChanged += new PropertyChangedEventHandler(this.Item_PropertyChanged);
                this.AddManagedSuccessorItems(item2);
                item2.UpdateBar();
                item2.UpdateDependencyLines();
                if (this.managedSuccessorItems.ContainsKey(item2))
                {
                    foreach (GanttChartItem item3 in this.managedSuccessorItems[item2])
                    {
                        item3.UpdateDependencyLines();
                    }
                    continue;
                }
            }
            for (int i = index; i < base.Count; i++)
            {
                GanttChartItem item4 = base[i];
                if (item4.IsVisible)
                {
                    if (item4.DisplayRowIndex.HasValue)
                    {
                        item4.ActualDisplayRowIndex = item4.DisplayRowIndex.Value;
                    }
                    else
                    {
                        item4.ActualDisplayRowIndex = num++;
                    }
                }
            }
        }

        private void AddManagedSuccessorItems(GanttChartItem item)
        {
            if (item.Predecessors != null)
            {
                foreach (PredecessorItem item2 in item.Predecessors)
                {
                    GanttChartItem key = item2.Item;
                    if (key != null)
                    {
                        if (!this.managedSuccessorItems.ContainsKey(key))
                        {
                            this.managedSuccessorItems.Add(key, new List<GanttChartItem>());
                        }
                        this.managedSuccessorItems[key].Add(item);
                    }
                }
            }
        }

        private void ClearManagedItems()
        {
            this.ClearManagedSuccessorItems();
            foreach (GanttChartItem item in this.managedItems)
            {
                item.PropertyChanged -= new PropertyChangedEventHandler(this.Item_PropertyChanged);
                item.DependenciesChanged -= new EventHandler(this.Item_DependenciesChanged);
                item.TimingChanged -= new EventHandler(this.Item_TimingChanged);
                item.HasChildrenChanged -= new EventHandler(this.Item_HasChildrenChanged);
                item.ActualDisplayRowIndexChanged -= new EventHandler(this.Item_ActualDisplayRowIndexChanged);
                item.DisplayRowIndexChanged -= new EventHandler(this.Item_DisplayRowIndexChanged);
                item.VisibilityChanged -= new EventHandler(this.Item_VisibilityChanged);
                item.ExpansionChanged -= new EventHandler(this.Item_ExpansionChanged);
                item.HierarchyChanged -= new EventHandler(this.Item_HierarchyChanged);
            }
            this.managedItems.Clear();
        }

        private void ClearManagedSuccessorItems()
        {
            this.managedSuccessorItems.Clear();
        }

        private void CoerceParentTiming(GanttChartItem item)
        {
            if (item.GanttChartView == null || item.GanttChartView.DisableParentTimingCoercion)
                return;

            if (item.HasChildren)
            {
                IEnumerable<GanttChartItem> children = this.GetChildren(item);
                if (children.Any<GanttChartItem>())
                {
                    item.Start = (from childItem in children select childItem.Start).Min<DateTime>();
                    item.Finish = (from childItem in children select childItem.Finish).Max<DateTime>();
                }
                item.UpdateBar();
                item.UpdateDependencyLines();
            }
            GanttChartItem parent = this.GetParent(item);
            if (parent != null)
            {
                this.CoerceParentTiming(parent);
            }
        }

        public IEnumerable<GanttChartItem> GetAllChildren(GanttChartItem item)
        {
            return this.GetAllChildren(base.IndexOf(item));
        }

        public IEnumerable<GanttChartItem> GetAllChildren(int index)
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

        public IEnumerable<GanttChartItem> GetAllParents(GanttChartItem item)
        {
            return this.GetAllParents(base.IndexOf(item));
        }

        public IEnumerable<GanttChartItem> GetAllParents(int index)
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

        public IEnumerable<GanttChartItem> GetChildren(GanttChartItem item)
        {
            return this.GetChildren(base.IndexOf(item));
        }

        public IEnumerable<GanttChartItem> GetChildren(int index)
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

        private int GetDisplayedRowIndex(GanttChartItem item)
        {
            int index = base.IndexOf(item);
            int num3 = index;
            while (num3-- > 0)
            {
                GanttChartItem item2 = this.managedItems[num3];
                if (item2.IsVisible)
                {
                    return (item2.ActualDisplayRowIndex + 1);
                }
            }
            return 0;
        }

        public GanttChartItem GetParent(GanttChartItem item)
        {
            return this.GetParent(base.IndexOf(item));
        }

        public GanttChartItem GetParent(int index)
        {
            if ((index >= 0) && (index < base.Count))
            {
                GanttChartItem item = base[index];
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

        public IEnumerable<GanttChartItem> GetPredecessors(GanttChartItem item)
        {
            if (item.Predecessors != null)
                foreach (var i in item.Predecessors)
                    yield return i.Item;
        }

        public IEnumerable<PredecessorItem> GetSuccessorPredecessorItems(GanttChartItem item)
        {
            if (this.managedSuccessorItems.ContainsKey(item))
            {
                foreach (var successorItem in this.managedSuccessorItems[item])
                {
                    foreach (var predecessorItem in successorItem.Predecessors)
                    {
                        if (predecessorItem.Item != item)
                            continue;
                        yield return predecessorItem;
                    }
                }

            }
        }

        public IEnumerable<GanttChartItem> GetSuccessors(GanttChartItem item)
        {
            if (this.managedSuccessorItems.ContainsKey(item))
            {
                foreach (var successorItem in this.managedSuccessorItems[item])
                    yield return successorItem;
            }

        }

        private void Item_ActualDisplayRowIndexChanged(object sender, EventArgs e)
        {
            GanttChartItem key = sender as GanttChartItem;
            if ((key != null) && this.managedSuccessorItems.ContainsKey(key))
            {
                foreach (GanttChartItem item2 in this.managedSuccessorItems[key])
                {
                    item2.UpdateDependencyLines();
                }
            }
        }

        private void Item_DependenciesChanged(object sender, EventArgs e)
        {
            GanttChartItem item = sender as GanttChartItem;
            if (item != null)
            {
                this.UpdateManagedSuccessorItems(item);
            }
        }

        private void Item_DisplayRowIndexChanged(object sender, EventArgs e)
        {
            GanttChartItem item = sender as GanttChartItem;
            if (item != null)
            {
                if (item.DisplayRowIndex.HasValue)
                {
                    item.ActualDisplayRowIndex = item.DisplayRowIndex.Value;
                }
                else
                {
                    item.ActualDisplayRowIndex = this.GetDisplayedRowIndex(item);
                }
            }
        }

        private void Item_ExpansionChanged(object sender, EventArgs e)
        {
            GanttChartItem item = (GanttChartItem)sender;
            int index = base.IndexOf(item);
            this.UpdateVisibilityFromExpansion(index);
            this.UpdateDisplayRowIndexesFromExpansion(index);
        }

        private void Item_HasChildrenChanged(object sender, EventArgs e)
        {
            GanttChartItem item = sender as GanttChartItem;
            if (item != null)
            {
                if (item.HasChildren)
                {
                    this.CoerceParentTiming(item);
                }
                item.UpdateComputedTaskTemplate();
                item.UpdateBar();
                item.UpdateDependencyLines();
                if (this.managedSuccessorItems.ContainsKey(item))
                {
                    foreach (GanttChartItem item2 in this.managedSuccessorItems[item])
                    {
                        item2.UpdateDependencyLines();
                    }
                }
            }
        }

        private void Item_HierarchyChanged(object sender, EventArgs e)
        {
            GanttChartItem item = (GanttChartItem)sender;
            int index = base.IndexOf(item);
            this.UpdateHierarchy(index, 1);
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnItemPropertyChanged(sender, e);
        }

        private void Item_TimingChanged(object sender, EventArgs e)
        {
            GanttChartItem key = sender as GanttChartItem;
            if (key != null)
            {
                if (this.managedSuccessorItems.ContainsKey(key))
                {
                    foreach (GanttChartItem item2 in this.managedSuccessorItems[key])
                    {
                        item2.UpdateDependencyLines();
                    }
                }
                this.CoerceParentTiming(key);
            }
        }

        private void Item_VisibilityChanged(object sender, EventArgs e)
        {
            GanttChartItem item = (GanttChartItem)sender;
            base.IndexOf(item);
            item.UpdateDependencyLines();
            if (this.managedSuccessorItems.ContainsKey(item))
            {
                foreach (GanttChartItem item2 in this.managedSuccessorItems[item])
                {
                    item2.UpdateDependencyLines();
                }
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.AddManagedItems(e.NewStartingIndex, e.NewItems);
                    this.UpdateHierarchy(e.NewStartingIndex, e.NewItems.Count);
                    this.UpdateVisibility(e.NewStartingIndex, e.NewItems.Count);
                    return;

                case NotifyCollectionChangedAction.Remove:
                    this.RemoveManagedItems(e.OldStartingIndex, e.OldItems);
                    this.UpdateHierarchy(e.OldStartingIndex, 0);
                    return;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    this.ClearManagedItems();
                    this.AddManagedItems(0, this);
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

        private void RemoveManagedItems(int index, IList items)
        {
            int num = 0;
            int num2 = index;
            while (num2-- > 0)
            {
                GanttChartItem item = this.managedItems[num2];
                if (item.IsVisible)
                {
                    num = item.ActualDisplayRowIndex + 1;
                    break;
                }
            }
            foreach (GanttChartItem item2 in items)
            {
                if (this.managedSuccessorItems.ContainsKey(item2))
                {
                    foreach (GanttChartItem item3 in this.managedSuccessorItems[item2])
                    {
                        if (item3.Predecessors != null)
                        {
                            List<PredecessorItem> list = new List<PredecessorItem>();
                            foreach (PredecessorItem item4 in item3.Predecessors)
                            {
                                if (item4.Item == item2)
                                {
                                    list.Add(item4);
                                }
                            }
                            foreach (PredecessorItem item5 in list)
                            {
                                item3.Predecessors.Remove(item5);
                            }
                        }
                        item3.UpdateDependencyLines();
                    }
                }
                this.RemoveManagedSuccessorItems(item2);
                item2.PropertyChanged -= new PropertyChangedEventHandler(this.Item_PropertyChanged);
                item2.DependenciesChanged -= new EventHandler(this.Item_DependenciesChanged);
                item2.TimingChanged -= new EventHandler(this.Item_TimingChanged);
                item2.HasChildrenChanged -= new EventHandler(this.Item_HasChildrenChanged);
                item2.ActualDisplayRowIndexChanged -= new EventHandler(this.Item_ActualDisplayRowIndexChanged);
                item2.DisplayRowIndexChanged -= new EventHandler(this.Item_DisplayRowIndexChanged);
                item2.VisibilityChanged -= new EventHandler(this.Item_VisibilityChanged);
                item2.ExpansionChanged -= new EventHandler(this.Item_ExpansionChanged);
                item2.HierarchyChanged -= new EventHandler(this.Item_HierarchyChanged);
                this.managedItems.Remove(item2);
            }
            for (int i = index; i < base.Count; i++)
            {
                GanttChartItem item6 = base[i];
                if (item6.IsVisible)
                {
                    if (item6.DisplayRowIndex.HasValue)
                    {
                        item6.ActualDisplayRowIndex = item6.DisplayRowIndex.Value;
                    }
                    else
                    {
                        item6.ActualDisplayRowIndex = num++;
                    }
                }
            }
        }

        private void RemoveManagedSuccessorItems(GanttChartItem item)
        {
            if (item.Predecessors != null)
            {
                foreach (PredecessorItem item2 in item.Predecessors)
                {
                    GanttChartItem key = item2.Item;
                    if ((key != null) && this.managedSuccessorItems.ContainsKey(key))
                    {
                        this.managedSuccessorItems[key].Remove(item);
                    }
                }
            }
        }

        internal void UpdateDisplayRowIndexesFromExpansion(int index)
        {
            if ((index >= 0) && (index < base.Count))
            {
                GanttChartItem item = base[index];
                int num = item.ActualDisplayRowIndex + 1;
                while (++index < base.Count)
                {
                    item = base[index];
                    if (item.IsVisible)
                    {
                        if (item.DisplayRowIndex.HasValue)
                        {
                            item.ActualDisplayRowIndex = item.DisplayRowIndex.Value;
                        }
                        else
                        {
                            item.ActualDisplayRowIndex = num++;
                        }
                    }
                }
            }
        }

        internal void UpdateHierarchy(int index, int count)
        {
            if ((index >= 0) && (index <= base.Count))
            {
                GanttChartItem item = (index < base.Count) ? base[index] : null;
                int indentation = (item != null) ? item.Indentation : 0;
                if (index > 0)
                {
                    GanttChartItem item2 = base[index - 1];
                    bool flag = item2.Indentation < indentation;
                    if (item2.HasChildren != flag)
                    {
                        item2.HasChildren = flag;
                    }
                    this.CoerceParentTiming(item2);
                }
                for (int i = index; i < (index + count); i++)
                {
                    if (i < (base.Count - 1))
                    {
                        GanttChartItem item3 = base[i + 1];
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

        private void UpdateManagedSuccessorItems(GanttChartItem item)
        {
            this.RemoveManagedSuccessorItems(item);
            this.AddManagedSuccessorItems(item);
        }

        internal void UpdateVisibility(int index, int count)
        {
            if ((index >= 0) && (index < base.Count))
            {
                GanttChartItem item = base[index];
                int indentation = item.Indentation;
                int num2 = index;
                while (num2 >= 0)
                {
                    GanttChartItem item2 = base[num2];
                    if (item2.Indentation < indentation)
                    {
                        break;
                    }
                    num2--;
                }
                GanttChartItem item3 = (num2 >= 0) ? base[num2] : null;
                for (int i = index; i < (index + count); i++)
                {
                    GanttChartItem item4 = base[i];
                    item4.IsVisible = (item3 == null) || item3.IsExpanded;
                }
            }
        }

        internal void UpdateVisibilityFromExpansion(int index)
        {
            if ((index >= 0) && (index < base.Count))
            {
                GanttChartItem item = base[index];
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

