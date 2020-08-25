using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels;
using KProcess.Presentation.Windows;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    /// <summary>
    /// Représente la vue de l'écran de gestion des membres du projet.
    /// </summary>
    [ViewExport(typeof(KProcess.Ksmed.Presentation.ViewModels.Interfaces.IPrepareMembersViewModel))]
    public partial class PrepareMembersView : UserControl, IView
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="PrepareMembersView"/>.
        /// </summary>
        public PrepareMembersView()
        {
            InitializeComponent();

            Loaded += PrepareMembersView_Loaded;
        }

        private void PrepareMembersView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var canWriteComparer = new UserCanWriteComparer();
            BindingOperations.SetBinding(canWriteComparer, UserCanWriteComparer.CurrentProcessProperty, new Binding("CurrentProcess") { Source = DataContext });
            BindingOperations.SetBinding(canWriteComparer, UserCanWriteComparer.SfDataGridProperty, new Binding() { Source = UsersGrid });
            var canWriteSortComparer = new SortComparer
            {
                Comparer = canWriteComparer,
                PropertyName = "CanWrite"
            };

            var canReadComparer = new UserCanReadComparer();
            BindingOperations.SetBinding(canReadComparer, UserCanReadComparer.CurrentProcessProperty, new Binding("CurrentProcess") { Source = DataContext });
            BindingOperations.SetBinding(canReadComparer, UserCanReadComparer.SfDataGridProperty, new Binding() { Source = UsersGrid });
            var canReadSortComparer = new SortComparer
            {
                Comparer = canReadComparer,
                PropertyName = "CanRead"
            };

            UsersGrid.SortComparers.AddRange(new []
            {
                canWriteSortComparer,
                canReadSortComparer
            });
        }

        private void UsersGrid_CurrentCellActivated(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellActivatedEventArgs e)
        {
            if (e.ActivationTrigger != ActivationTrigger.Mouse)
            {
                return;
            }

            var grid = sender as SfDataGrid;
            var user = grid.CurrentItem;
            var context = grid.DataContext as PrepareMembersViewModel;

            ICommand command = null;

            if (e.CurrentRowColumnIndex.ColumnIndex == 1)
            {
                command = context.CheckCanWriteCommand;
            }
            else if (e.CurrentRowColumnIndex.ColumnIndex == 2)
            {
                command = context.CheckCanReadCommand;
            }

            if (command != null && command.CanExecute(user))
            {
                command.Execute(user);
            }
        }
    }

}

