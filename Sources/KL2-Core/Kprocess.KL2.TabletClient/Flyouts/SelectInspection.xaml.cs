using Kprocess.KL2.TabletClient.ViewModel;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Interfaces;
using KProcess.Presentation.Windows;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Linq;

namespace Kprocess.KL2.TabletClient.Flyouts
{
    /// <summary>
    /// Logique d'interaction pour SelectInspection.xaml
    /// </summary>
    public partial class SelectInspection
    {
        public SelectInspection()
        {
            InitializeComponent();
        }

        void PublicationsTreeView_Collapsing(object sender, ExpandingCollapsingEventArgs e)
        {
            if ((e.OriginalSource as TreeViewItemAdv)?.DataContext is ProjectDir folder && folder.Id == -1)
                e.Cancel = true;
        }

        async void PublicationsTreeView_ScrollableItemSelected(object sender, EventArgs e)
        {
            if (DataContext is InspectionChoiceViewModel vm)
            {
                var syncTreeViewItem = VisualTreeHelperExtensions.FindLastChild<TreeViewItemAdv>(PublicationsTreeView, i =>
                {
                    if (i is TreeViewItemAdv item)
                        return item.IsMouseOver;
                    return false;
                });
                if (syncTreeViewItem?.DataContext is ProjectDir folder && folder.Id != -1)
                {
                    if (folder.Id != -1)
                        folder.IsExpanded = !folder.IsExpanded;
                    vm.CurrentNode = null;
                }
                else if (syncTreeViewItem?.DataContext is Procedure process)
                {
                    if (process.ProcessId != (vm.InspectionPublication?.ProcessId ?? 0))
                    {
                        vm.CurrentNodeIsChanging = true;
                        await vm.OnCurrentNodeChanged(process);
                        PublicationsTreeView.SelectedTreeItem = vm.CurrentNode;
                    }
                }
                else
                {
                    vm.CurrentNode = null;
                }
            }
        }

        private void Flyout_IsOpenChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is InspectionChoiceViewModel vm)
            {
                if (vm.InspectionPublication != null)
                    PublicationsTreeView.SelectedTreeItem = TreeViewHelper.FindProcess(vm.Nodes.First(), vm.InspectionPublication.ProcessId);
                else
                    PublicationsTreeView.SelectedTreeItem = null;
            }
        }
    }
}
