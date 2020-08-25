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
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Core.Behaviors;
using KProcess.Ksmed.Presentation.Core.Controls;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    /// <summary>
    /// Représente la vue de l'écran de simulation dans la phase de construction.
    /// </summary>
    public partial class SimulateView : UserControl, IView
    {
        private MultiGanttZoomer _zoomer;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="AnalyzeSimulateView"/>.
        /// </summary>
        public SimulateView()
        {
            this.DifferenceReasonColumnVisibility = System.Windows.Visibility.Collapsed;

            InitializeComponent();

            _zoomer = (MultiGanttZoomer)this.Resources["Zoomer"];
            _zoomer.Gantts.Add(this.analyzeSimulateGrid1);
            _zoomer.Gantts.Add(this.analyzeSimulateGrid2);

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
                        .Subscribe<GanttAutoScaleEvent>(OnGanttAutoScale);

            };
        }

        /// <summary>
        /// Obtient ou définit la visibilité de la colonne "cause des écarts".
        /// </summary>
        public Visibility DifferenceReasonColumnVisibility { get; set; }

        private void OnGanttAutoScale(GanttAutoScaleEvent ev)
        {
            if (ev.Sender == this.DataContext)
                _zoomer.EnqueueAutoScale(true, false, true);
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

        private void analyzeSimulateGrid1_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = (KGanttChartDataGrid)sender;
            if (grid.GanttChartView != null)
                grid.GanttChartView.IsReadOnly = true;

            this.ColumnTopDifferenceReason.Visibility = this.DifferenceReasonColumnVisibility;
        }

        private void analyzeSimulateGrid2_Loaded(object sender, RoutedEventArgs e)
        {
            this.analyzeSimulateGrid2.GanttChartView.IsReadOnly = true;

            this.ColumnBottomDifferenceReason.Visibility = this.DifferenceReasonColumnVisibility;
        }

        private void CollapseAll_Click(object sender, RoutedEventArgs e)
        {
            this.analyzeSimulateGrid1.CollapseAll();
            this.analyzeSimulateGrid2.CollapseAll();
        }

        private void ExpandAll_Click(object sender, RoutedEventArgs e)
        {
            this.analyzeSimulateGrid1.ExpandAll();
            this.analyzeSimulateGrid2.ExpandAll();
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
            GanttChartDataGrid.SetDependencyLinesVisibility(this.analyzeSimulateGrid1, value ? Visibility.Visible : Visibility.Collapsed);
            GanttChartDataGrid.SetDependencyLinesVisibility(this.analyzeSimulateGrid2, value ? Visibility.Visible : Visibility.Collapsed);
            this.PredecessorsLinks.IsChecked = value;
        }
    }

}

