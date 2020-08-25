namespace DlhSoft.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;

    public class ItemActivatedEventArgs : EventArgs
    {
        public ItemActivatedEventArgs(GanttChartItem item)
        {
            this.Item = item;
        }

        public GanttChartItem Item { get; private set; }
    }
}

