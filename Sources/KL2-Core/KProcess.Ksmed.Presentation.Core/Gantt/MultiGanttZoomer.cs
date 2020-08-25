using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DlhSoft.Windows.Controls;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Représente un composant capable de contrôler le zoom de plusieurs Gantt en les gardant synchronisés.
    /// </summary>
    public class MultiGanttZoomer : DependencyObject, IZoommerGantt
    {

        /// <summary>
        /// Le décalage à droite qui est automatiquement inséré lors de l'auto scale, afin de pouvoir utiliser le crayon pour lier les éléments.
        /// </summary>
        private const int AutoScaleXMinRightOffset = 5;

        private bool _ignoreZoomChanges = false;
        private BulkObservableCollection<IZoomableGanttTarget> _gantts = new BulkObservableCollection<IZoomableGanttTarget>();

        /// <summary>
        /// Obtient ou définit le zoom.
        /// </summary>
        public double ZoomX
        {
            get { return (double)GetValue(ZoomXProperty); }
            set { SetValue(ZoomXProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="ZoomX"/>.
        /// </summary>
        public static readonly DependencyProperty ZoomXProperty =
            DependencyProperty.Register("ZoomX", typeof(double), typeof(MultiGanttZoomer),
            new UIPropertyMetadata(1.0, OnZoomXChanged, OnCoerceZoomX));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété de dépendance <see cref="ZoomX"/> a changé.
        /// </summary>
        /// <param name="d">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="e">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnZoomXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zoomer = (MultiGanttZoomer)d;
            if (!zoomer._ignoreZoomChanges)
                zoomer.AutoScale(true, false);
        }

        /// <summary>
        /// Appelé pour coercer le zoom.
        /// </summary>
        /// <param name="d">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="baseValue">La valeur d'origine.</param>
        /// <returns>La valeur coercée.</returns>
        private static object OnCoerceZoomX(DependencyObject d, object baseValue)
        {
            var val = (double)baseValue;
            return val <= 0.1 ? 0.1 : val;
        }


        /// <summary>
        /// Obtient ou définit le zoom.
        /// </summary>
        public double ZoomY
        {
            get { return (double)GetValue(ZoomYProperty); }
            set { SetValue(ZoomYProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="ZoomY"/>.
        /// </summary>
        public static readonly DependencyProperty ZoomYProperty =
            DependencyProperty.Register("ZoomY", typeof(double), typeof(MultiGanttZoomer),
            new UIPropertyMetadata(1.0, OnZoomYChanged, OnCoerceZoomY));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété de dépendance <see cref="ZoomY"/> a changé.
        /// </summary>
        /// <param name="d">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="e">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnZoomYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zoomer = (MultiGanttZoomer)d;
            if (!zoomer._ignoreZoomChanges)
                zoomer.AutoScale(false, false);
        }

        /// <summary>
        /// Appelé pour coercer le zoom.
        /// </summary>
        /// <param name="d">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="baseValue">La valeur d'origine.</param>
        /// <returns>La valeur coercée.</returns>
        private static object OnCoerceZoomY(DependencyObject d, object baseValue)
        {
            var val = (double)baseValue;
            val = Math.Max(.1, val);
            val = Math.Min(1, val);

            return val;
        }

        /// <summary>
        /// Obtient les différents Gantt qui vont être liés à ce zoomer.
        /// </summary>
        public BulkObservableCollection<IZoomableGanttTarget> Gantts
        {
            get { return _gantts; }
        }

        /// <summary>
        /// Met en attente un auto-scale qui sera exécuté dès que le gantt sera prêt.
        /// </summary>
        /// <param name="horizontal"><c>true</c> pour scaler horizontalement.</param>
        /// <param name="vertical"><c>true</c> pour scaler verticalement.</param>
        /// <param name="resetZooms"><c>true</c> pour  RAZ le zoom.</param>
        public void EnqueueAutoScale(bool horizontal, bool vertical, bool resetZooms)
        {
            if (!this.Gantts.Any())
                return;

            if (resetZooms)
            {
                _ignoreZoomChanges = true;
                if (horizontal)
                {
                    this.ZoomX = 1;
                }
                if (vertical)
                {
                    this.ZoomY = 1;
                }

                foreach (var chart in this.Gantts)
                {
                    KGanttChartView ganttChart = null;
                    var grid = chart as GanttChartDataGrid;
                    if (grid != null && grid.GanttChartView != null)
                        ganttChart = grid.GanttChartView as KGanttChartView;
                    else
                    {
                        var v = chart as GanttChartView;
                        if (v != null)
                            ganttChart = v as KGanttChartView;
                    }

                    if (ganttChart != null)
                    {
                        if (horizontal)
                            ganttChart.DeferScrollToHorizontalOffset(0);
                        if (vertical)
                            ganttChart.DeferScrollToVerticalOffset(0);
                    }
                }

                _ignoreZoomChanges = false;
            }

            if (this.Gantts.Any(g => g.IsReadyForAutoScale))
            {
                this.AutoScale(horizontal, vertical);
            }
            else if (this.Gantts.Any(g => !g.IsReadyForAutoScale))
            {
                this.AutoScaleEnqueued = () => this.AutoScale(horizontal, vertical);
                foreach (var gantt in this.Gantts.Where(g => !g.IsReadyForAutoScale))
                {
                    gantt.ReadyForAutoScale -= OnReadyForAutoScale;
                    gantt.ReadyForAutoScale += OnReadyForAutoScale;
                }
            }
        }

        private void OnReadyForAutoScale(object sender, EventArgs e)
        {
            ((IZoomableGanttTarget)sender).ReadyForAutoScale -= OnReadyForAutoScale;

            if (this.AutoScaleEnqueued != null)
                this.AutoScaleEnqueued();
        }

        /// <summary>
        /// Applique une échelle automatique affichant tous les éléments.
        /// </summary>
        /// <param name="updateHorizontal"><c>true</c> pour mettre à jour le zoom horizontal.</param>
        /// <param name="idealVertical"><c>true</c> pour calculer le zoom vertical permettant de voir tous les éléments.</param>
        public void AutoScale(bool updateHorizontal, bool idealVertical)
        {
            var ganttsReady = this.Gantts.Where(g => g.IsReadyForAutoScale);
            if (idealVertical)
            {
                double minIdealZoom = 1d;
                foreach (var gantt in ganttsReady)
                    minIdealZoom = Math.Min(minIdealZoom, PredictIdealZoomYToSeeAllItems(gantt));
                _ignoreZoomChanges = true;
                this.ZoomY = minIdealZoom;
                _ignoreZoomChanges = false;
            }
            AutoScale(updateHorizontal, this.ZoomX, this.ZoomY, ganttsReady);
        }

        private double PredictIdealZoomYToSeeAllItems(IZoomableGanttTarget zoomable)
        {
            var grid = zoomable as KGanttChartDataGrid;
            var gantt = grid.GanttChartView;

            // Détermination du zoom Y idéal
            var availableHeight = gantt.ScrollContentPresenter.ViewportHeight;
            var itemsCount = grid.Items.Where(i => i.IsVisible).Count();

            double relativeZoom;

            if (itemsCount != 0)
            {
                var idealItemHeight = availableHeight / itemsCount;
                relativeZoom = idealItemHeight / zoomable.ZoomYInitialValues.ItemHeight;
            }
            else
                relativeZoom = 1d;

            return Math.Min(1d, relativeZoom);
        }

        /// <summary>
        /// Applique un auto scale sur un ou plusieurs Gantts.
        /// </summary>
        /// <param name="updateTimelineRange"><c>true</c> pour mettre également à jour les ranges de la Timeline.</param>
        /// <param name="charts">Les différents Gantts.</param>
        private void AutoScale(bool updateTimelineRange, double zoomX, double zoomY, IEnumerable<IZoomableGanttTarget> charts)
        {
            var timeService = IoC.Resolve<IServiceBus>().Get<ITimeTicksFormatService>();
            var chartsChart = new Dictionary<IZoomableGanttTarget, GanttChartView>();

            if (!charts.Any(c => c.IsReadyForAutoScale))
                return;

            foreach (var chart in charts)
            {
                var grid = chart as GanttChartDataGrid;
                if (grid != null && grid.GanttChartView != null)
                    chartsChart[chart] = grid.GanttChartView;

                else
                {
                    var v = chart as GanttChartView;
                    if (v != null)
                        chartsChart[chart] = v;
                }
            }

            IEnumerable<GanttChartItem> tasks = null;

            foreach (var chart in charts)
            {
                if (tasks == null)
                    tasks = chartsChart[chart].ManagedItems;
                else
                    tasks = tasks.Concat(chartsChart[chart].ManagedItems);
            }

            if (tasks == null || !tasks.Any())
            {
                if (updateTimelineRange)
                {
                    foreach (var zoomable in charts)
                    {
                        var chart = chartsChart[zoomable];
                        chart.TimelinePageStart = GanttDates.StartDate;
                        chart.TimelinePageFinish = GanttDates.DefaultEndDate;
                        if (chart.ScrollContentPresenter != null)
                            chart.UpdateScaleInterval = TimeSpan.FromTicks(timeService.CurrentTimeScale);
                    }
                }
            }
            else
            {
                var start = GanttDates.StartDate;
                var end = tasks.Max(t => t.Finish);

                if (updateTimelineRange)
                {
                    foreach (var zoomable in charts)
                    {
                        var chart = chartsChart[zoomable];
                        chart.TimelinePageStart = GanttDates.StartDate;
                        double finish = (end - start).Ticks * 5;
                        chart.TimelinePageFinish = GanttDates.ToDateTime(Convert.ToInt64(finish));

                        if (chart.ScrollContentPresenter != null)
                            chart.UpdateScaleInterval = TimeSpan.FromTicks(timeService.CurrentTimeScale);
                    }

                    var total = end - start;

                    double minWidth = double.PositiveInfinity;
                    foreach (var zoomable in charts)
                    {
                        var chart = chartsChart[zoomable];
                        if (chart.ScrollContentPresenter != null)
                            minWidth = Math.Min(minWidth, GetVisibleWidth(chart));
                    }

                    foreach (var zoomable in charts)
                    {
                        var chart = chartsChart[zoomable];
                        if (minWidth / total.TotalHours > 0.0)
                            chart.HourWidth = minWidth * zoomX / total.TotalHours;
                    }

                    foreach (var zoomable in charts)
                    {
                        var chart = chartsChart[zoomable];
                        var zoomableScale = (ZoomableScale)chart.Scales[0];
                        zoomableScale.ChartWidth = minWidth;
                        zoomableScale.CalculateInitialIntervals(zoomX);
                    }
                }

                // Application du zoom Y
                foreach (var zoomable in charts)
                {
                    var chart = chartsChart[zoomable];

                    var grid = zoomable as KGanttChartDataGrid;
                    if (grid == null)
                    {
                        grid = ((KGanttChartView)chart).ParentKDataGrid;
                    }

                    if (zoomable.ZoomYInitialValues == null)
                    {
                        zoomable.ZoomYInitialValues = new ZoomYInitialValues
                        {
                            BarHeightRatio = grid.BarHeight / grid.ItemHeight,
                            ItemHeight = grid.ItemHeight,
                            FontSize = grid.FontSize,
                            DataGridRowsPresenter = grid.FindFirstChild<System.Windows.Controls.Primitives.DataGridRowsPresenter>(),
                        };
                    }

                    // Arrondir au pixel près car autrement le LayoutRounding risque de causer des décalages entre la grille et les barres
                    grid.ItemHeight = Math.Round(zoomable.ZoomYInitialValues.ItemHeight * zoomY, 0);
                    grid.BarHeight = zoomable.ZoomYInitialValues.BarHeightRatio * grid.ItemHeight;
                    double fontSize = zoomable.ZoomYInitialValues.FontSize * zoomY;
                    System.Windows.Documents.TextElement.SetFontSize(zoomable.ZoomYInitialValues.DataGridRowsPresenter, fontSize);
                    System.Windows.Documents.TextElement.SetFontSize(chart.ScrollContentPresenter, fontSize);
                }
            }

            SaveZoom(zoomX, zoomY);
        }

        /// <summary>
        /// Sauvegarde le zoom pour le projet.
        /// </summary>
        private static void SaveZoom(double zoomX, double zoomY)
        {
            if (!DesignMode.IsInDesignMode)
            {
                var serviceBus = IoC.Resolve<IServiceBus>();
                var projectService = serviceBus.Get<IProjectManagerService>();
                var navigationService = serviceBus.Get<INavigationService>();

                if (navigationService.Preferences != null && projectService.CurrentScenario != null)
                    navigationService.Preferences.GanttZooms[projectService.CurrentScenario.Id] = (zoomX, zoomY);
            }
        }

        /// <summary>
        /// Obtient la taille visible du Gantt.
        /// </summary>
        /// <param name="gantt">le Gantt.</param>
        /// <returns>La taille.</returns>
        private static double GetVisibleWidth(GanttChartView gantt)
        {
            return Math.Max(0.0, gantt.ScrollContentPresenter.ViewportWidth - AutoScaleXMinRightOffset);
        }

        /// <summary>
        /// Obtient ou définit un délégué exécuté lorsque l'auto scale aura lieu.
        /// </summary>
        public Action AutoScaleEnqueued { get; set; }
    }
}
