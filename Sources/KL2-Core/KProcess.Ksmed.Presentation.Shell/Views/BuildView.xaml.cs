using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Core.Controls;
using KProcess.Ksmed.Presentation.ViewModels;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using KProcess.Common;
using System.Threading;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    /// <summary>
    /// Représente la vue de l'écran de construction du scénario initial.
    /// </summary>
    public partial class BuildView : UserControl, IView
    {
        private GanttGridViewContainer _previousView;
        private MultiGanttZoomer _zoomer;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="AnalyzeBuildView"/>.
        /// </summary>
        public BuildView()
        {
            InitializeComponent();

            _zoomer = (MultiGanttZoomer)this.Resources["Zoomer"];
            _zoomer.Gantts.Add(this.analyzeBuildGrid);

            var serviceBus = IoC.Resolve<IServiceBus>();
            var projectService = serviceBus.Get<IProjectManagerService>();
            var navigationService = serviceBus.Get<INavigationService>();

            if (navigationService.Preferences != null && projectService.CurrentProject != null && navigationService.Preferences.GanttDependencyLinesVisibilities.ContainsKey(projectService.CurrentProject.ProjectId))
                SetPredecessorsLinksVisibility(navigationService.Preferences.GanttDependencyLinesVisibilities[projectService.CurrentProject.ProjectId]);
            else
                SetPredecessorsLinksVisibility(true);

            this.DataContextChanged += (s, e) =>
            {
                IoC.Resolve<IEventBus>().Unsubscribe(this);
                if (this.DataContext != null)
                    IoC.Resolve<IEventBus>()
                        .Subscribe<GanttAutoScaleEvent>(OnGanttAutoScale)
                        .Subscribe<GridViewChangedEvent>(ev =>
                        {
                            if (ev.Sender == base.DataContext)
                            {
                                UdpateColumnsVisibility(ev.View);
                                SetupUnlinkMarkersMode();
                            }
                        })
                        .Subscribe<RefreshRequestedEvent>(ev =>
                        {
                            series1.Refresh();
                            series2.Refresh();
                        });
            };

            this.Loaded += (s, e) =>
            {
                var vm = (IBuildViewModel)base.DataContext;
                this.SetupUnlinkMarkersMode();
                vm.PropertyChanged += OnVmPropertyChanged;
            };

            this.Unloaded += (s, e) =>
            {
                var vm = (IBuildViewModel)base.DataContext;
                vm.PropertyChanged -= OnVmPropertyChanged; 
                IoC.Resolve<IEventBus>().Unsubscribe(this);
            };

           // OnViewSelectionChanged(null, null);
        }

        private void OnVmPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ExtractFrom<IBuildViewModel>.MemberName(vm => vm.IsMarkersLinkedModeEnabled))
            {
                this.SetupUnlinkMarkersMode();
            }
        }

        /// <summary>
        /// Met à jour les colonnes relatives au mode de liaison des marqueurs
        /// </summary>
        private void SetupUnlinkMarkersMode()
        {
            var vm = (IBuildViewModel)base.DataContext;
            if (!vm.IsMarkersLinkedModeEnabled)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.ColumnPredecessors.Visibility = System.Windows.Visibility.Collapsed;
                }));
            }
        }

        private void OnGanttAutoScale(GanttAutoScaleEvent ev)
        {
            if (ev.Sender == this.DataContext)
                _zoomer.EnqueueAutoScale(true, false, true);
        }

        /// <summary>
        /// Met à jour la visibilité des colonnes en fonction de la vue.
        /// </summary>
        /// <param name="view">La vue.</param>
        private void UdpateColumnsVisibility(GanttGridView view)
        {
            switch (view)
            {
                case GanttGridView.WBS:
                    GridBehavior.TryShow(ColumnVideo, view);
                    GridBehavior.TryShow(ColumnTask, view);
                    GridBehavior.TryShow(ColumnDuration, view);
                    GridBehavior.TryShow(ColumnCategory, view);
                    GridBehavior.TryShow(ColumnWBS, view);
                    GridBehavior.TryShow(ColumnResource, view);
                    GridBehavior.TryShow(ColumnPredecessors, view);
                    break;

                default:
                    GridBehavior.TryShow(ColumnVideo, view);
                    GridBehavior.TryShow(ColumnTask, view);
                    GridBehavior.TryShow(ColumnDuration, view);
                    GridBehavior.TryShow(ColumnCategory, view);
                    GridBehavior.TryHide(ColumnWBS, view);
                    GridBehavior.TryHide(ColumnResource, view);
                    GridBehavior.TryHide(ColumnPredecessors, view);
                    break;
            }

            SetupUnlinkMarkersMode();
        }

        private void AutoZoomHorizontal_Click(object sender, RoutedEventArgs e)
        {
            _zoomer.EnqueueAutoScale(true, false, true);
        }

        private void AutoZoomVertical_Click(object sender, RoutedEventArgs e)
        {
            _zoomer.EnqueueAutoScale(false, true, false);
        }

        private void AutoZoomVerticalRevert_Click(object sender, RoutedEventArgs e)
        {
            _zoomer.ZoomY = 1d;
        }

        private void AutoZoomBoth_Click(object sender, RoutedEventArgs e)
        {
            _zoomer.EnqueueAutoScale(true, true, true);
        }
        

        private void CollapseAll_Click(object sender, RoutedEventArgs e)
        {
            this.analyzeBuildGrid.CollapseAll();
        }

        private void ExpandAll_Click(object sender, RoutedEventArgs e)
        {
            this.analyzeBuildGrid.ExpandAll();
        }

        private void PredecessorsLinks_CheckedChanged(object sender, RoutedEventArgs e)
        {
            var isChecked = this.PredecessorsLinks.IsChecked.GetValueOrDefault();
            SetPredecessorsLinksVisibility(isChecked);

            var serviceBus = IoC.Resolve<IServiceBus>();
            var projectService = serviceBus.Get<IProjectManagerService>();
            var navigationService = serviceBus.Get<INavigationService>();

            if (navigationService.Preferences != null && projectService.CurrentProject != null)
                navigationService.Preferences.GanttDependencyLinesVisibilities[projectService.CurrentProject.ProjectId] = isChecked;
        }

        private void SetPredecessorsLinksVisibility(bool value)
        {
            GanttChartDataGrid.SetDependencyLinesVisibility(this.analyzeBuildGrid, value ? Visibility.Visible : Visibility.Collapsed);
            this.PredecessorsLinks.IsChecked = value;
        }

        private void OperatorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CloseDropDownButtonOnSelectionChanged(this.OperatorsListBox);
        }

        private void EquipmentsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CloseDropDownButtonOnSelectionChanged(this.EquipmentsListBox);
        }

        private void ReductionOperatorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CloseDropDownButtonOnSelectionChanged(this.ReductionOperatorsListBox);
        }

        private void ReductionEquipmentsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CloseDropDownButtonOnSelectionChanged(this.ReductionEquipmentsListBox);
        }

        private void CategoriesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CloseDropDownButtonOnSelectionChanged(this.CategoriesListBox);
        }

        private void SkillsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CloseDropDownButtonOnSelectionChanged(this.SkillsListBox);
        }


        private void CloseDropDownButtonOnSelectionChanged(ReferentialPicker e)
        {
            if (!e.IsLoaded)
                return;

            // Ne pas fermer si on est en mode saisie
            for (int i = 0; i < e.Items.Count; i++)
            {
                var item = (ReferentialPickerItem)e.ItemContainerGenerator.ContainerFromIndex(i);
                if (item != null && item.IsEditable)
                    return;
            }

            var ddb = VisualTreeHelperExtensions.TryFindParent<DropDownButton>(e);
            if (ddb != null)
            {
                ddb.IsOpen = false;
            }
        }

        private void OnViewSelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_previousView!=null)
            {
            Dispatcher.BeginInvoke(new ThreadStart(delegate ()
                {
                    this.analyzeBuildGrid.CollapseAll();
                }), DispatcherPriority.ApplicationIdle, null);

               
            }

            _previousView = ((ComboBox)e.Source).SelectedItem as GanttGridViewContainer;
        }
    }

}

