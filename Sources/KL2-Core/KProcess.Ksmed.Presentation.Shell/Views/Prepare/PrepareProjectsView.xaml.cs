using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using Syncfusion.Windows.Tools.Controls;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    /// <summary>
    /// Interaction logic for ProjectView.xaml
    /// </summary>
    [ViewExport(typeof(IPrepareProjectsViewModel))]
    public partial class PrepareProjectsView : UserControl, IView
    {
        public PrepareProjectsView()
        {
            InitializeComponent();

            IoC.Resolve<IEventBus>().Subscribe<RefreshRequestedEvent>(e => UpdateSummaryDataGrid());
        }

        void OnListBoxItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItemAdv treeviewItem)
            {
                if (treeviewItem.ParentItemsControl.DataContext is IPrepareProjectsViewModel context && context?.CurrentProject != null)
                    context.OpenProjectCommand?.Execute(null);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Met à jour les données de la grille de synthèse
        /// </summary>
        void UpdateSummaryDataGrid()
        {
            SummaryDatagrid.ItemsSource = null;
            SummaryDatagrid.Columns.Clear();

            if (DataContext is IPrepareProjectsViewModel vm && vm?.CurrentProject?.ScenariosCriticalPath?.Any() == true)
            {
                SummaryBuilder.BuildScenarios(vm.CurrentProject.ScenariosCriticalPath, SummaryDatagrid);
                SummaryDatagrid.Visibility = Visibility.Visible;
            }
            else
                SummaryDatagrid.Visibility = Visibility.Hidden;
        }

        void ProjectsTreeview_Collapsing(object sender, ExpandingCollapsingEventArgs e)
        {
            if ((e.OriginalSource as TreeViewItemAdv)?.DataContext is ProjectDir folder && folder.Id == -1)
                e.Cancel = true;
        }
    }
}
