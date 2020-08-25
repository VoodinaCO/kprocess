using System.Windows;

namespace Kprocess.KL2.TabletClient.AttachBehaviors
{
    public class TreeviewExpandBehavior : DependencyObject
    {
        public const string IsExpandedAllPropertyName = "IsExpandedAll";
        public const string IsCollapsedAllPropertyName = "IsCollapsedAll";

        public static readonly DependencyProperty IsExpandedAllProperty = DependencyProperty.RegisterAttached(
            IsExpandedAllPropertyName, typeof(bool), typeof(TreeviewExpandBehavior), new PropertyMetadata(IsExpandedChangedCallback));

        static void IsExpandedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isExpand = (bool)e.NewValue;
            if (isExpand)
                UpdateTreeViewItem(d, true);
        }

        public static void SetIsExpandedAll(DependencyObject element, bool value) =>
            element.SetValue(IsExpandedAllProperty, value);

        public static bool GetIsExpandedAll(DependencyObject element) =>
            (bool)element.GetValue(IsExpandedAllProperty);

        public static readonly DependencyProperty IsCollapsedAllProperty = DependencyProperty.RegisterAttached(
            IsCollapsedAllPropertyName, typeof(bool), typeof(TreeviewExpandBehavior), new PropertyMetadata(IsCollapsedChangedCallback));

        static void IsCollapsedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var isCollapsed = (bool)e.NewValue;
            if (!isCollapsed)
                UpdateTreeViewItem(d, false);
        }

        public static void SetIsCollapsedAll(DependencyObject element, bool value) =>
            element.SetValue(IsCollapsedAllProperty, value);

        public static bool GetIsCollapsedAll(DependencyObject element) =>
            (bool)element.GetValue(IsCollapsedAllProperty);

        static void UpdateTreeViewItem(DependencyObject d, bool isExpand)
        {
            if (d is Syncfusion.Windows.Tools.Controls.TreeViewAdv treeView)
            {
                foreach (dynamic item in treeView.Items)
                {
                    if (item.Childs.Count == 0)
                        continue;

                    ExpandAllNodes(item, isExpand);
                    item.IsExpanded = isExpand;
                }

                if (!isExpand)
                {
                    //Keep Select root item for collasping 
                    dynamic root = treeView.Items[0];
                    root.IsSelected = true;

                    treeView.ClearSelection();
                }
            }
        }

        static void ExpandAllNodes(dynamic rootItem, bool isExpand)
        {
            foreach (dynamic item in rootItem.Nodes)
            {
                if (item.Nodes.Count == 0)
                    continue;

                ExpandAllNodes(item, isExpand);
                item.IsExpanded = isExpand;
            }
        }
    }
}
