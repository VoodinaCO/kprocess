namespace DlhSoft.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Threading;

    public class PredecessorItemCollection : ObservableCollection<PredecessorItem>
    {
        private List<PredecessorItem> managedItems = new List<PredecessorItem>();

        public event EventHandler Changed;

        private void AddManagedItems(int index, IList items)
        {
            foreach (PredecessorItem item in items)
            {
                if (!this.managedItems.Contains(item))
                {
                    this.managedItems.Insert(index++, item);
                    item.Changed += new EventHandler(this.Item_Changed);
                }
            }
        }

        private void ClearManagedItems()
        {
            foreach (PredecessorItem item in this.managedItems)
            {
                item.Changed -= new EventHandler(this.Item_Changed);
            }
            this.managedItems.Clear();
        }

        private void Item_Changed(object sender, EventArgs e)
        {
            this.OnChanged();
        }

        protected virtual void OnChanged()
        {
            if (this.Changed != null)
            {
                this.Changed(this, EventArgs.Empty);
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.AddManagedItems(e.NewStartingIndex, e.NewItems);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    this.RemoveManagedItems(e.OldItems);
                    break;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    this.ClearManagedItems();
                    this.AddManagedItems(0, this);
                    break;
            }
            this.OnChanged();
        }

        private void RemoveManagedItems(IList items)
        {
            foreach (PredecessorItem item in items)
            {
                item.Changed -= new EventHandler(this.Item_Changed);
                this.managedItems.Remove(item);
            }
        }
    }
}

