using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Fournit le scroll par molette pour un IZoomableGantt.
    /// </summary>
    public class WheelZoomableBehavior : Behavior<FrameworkElement>
    {

        private System.Reactive.Subjects.Subject<ZoomAction> _zoomOperations;

        /// <summary>
        /// Gets or sets the ExternalZoomable.
        /// </summary>
        public IZoommerGantt ExternalZoomable
        {
            get { return (IZoommerGantt)GetValue(ExternalZoomableProperty); }
            set { SetValue(ExternalZoomableProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="ExternalZoomable"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExternalZoomableProperty =
            DependencyProperty.Register("ExternalZoomable", typeof(IZoommerGantt), typeof(WheelZoomableBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// Appelé une fois que le comportement est attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            base.AssociatedObject.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(AssociatedObject_PreviewMouseWheel);

            var savedZoom = GetSavedZoom();
            if (savedZoom.HasValue)
                ApplyZoom(savedZoom.Value.X, savedZoom.Value.Y);
        }

        /// <summary>
        /// Appelé lorsque le comportement est détaché de son AssociatedObject, mais avant qu'il ne se soit produit réellement.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            base.AssociatedObject.PreviewMouseWheel -= new System.Windows.Input.MouseWheelEventHandler(AssociatedObject_PreviewMouseWheel);
        }

        /// <summary>
        /// Execute les différentes actions de zoom mise en file.
        /// </summary>
        /// <param name="zoomable">Le gantt.</param>
        /// <param name="chart">Le chart.</param>
        /// <param name="actions">Les actions.</param>
        private void ExecuteZoomActions(IZoommerGantt zoomable, KGanttChartView chart, IEnumerable<ZoomAction> actions)
        {
            double zoomX = zoomable.ZoomX;
            double zoomY = zoomable.ZoomY;
            double verticalOffset = chart.ScrollVerticalOffset;
            double horizontalOffset = chart.ScrollHorizontalOffset;
            bool scrollHorizontal = false;
            bool scrollVertical = false;

            foreach (var action in actions)
            {
                switch (action.Action)
                {
                    case ZoomActionEnum.ZoomXIn:
                        {
                            double delta = zoomX * 1.2 - zoomX;
                            zoomX += delta;
                            horizontalOffset += action.MousePosition.X * delta;
                            scrollHorizontal = true;
                        }
                        break;

                    case ZoomActionEnum.ZoomXOut:
                        {
                            double delta = zoomX / 1.2 - zoomX;
                            zoomX += delta;
                            horizontalOffset += action.MousePosition.X * delta;
                            scrollHorizontal = true;
                        }
                        break;

                    case ZoomActionEnum.ZoomYIn:
                        {
                            double delta = zoomY * 1.2 - zoomY;
                            zoomY += delta;
                            verticalOffset += action.MousePosition.Y * delta;
                            scrollVertical = true;
                        }
                        break;

                    case ZoomActionEnum.ZoomYOut:
                        {
                            double delta = zoomY / 1.2 - zoomY;
                            zoomY += delta;
                            verticalOffset += action.MousePosition.Y * delta;
                            scrollVertical = true;
                        }
                        break;

                    case ZoomActionEnum.ScrollRight:
                        horizontalOffset += 48;
                        scrollHorizontal = true;
                        break;

                    case ZoomActionEnum.ScrollLeft:
                        horizontalOffset -= 48;
                        scrollHorizontal = true;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("action");
                }
            }

            ApplyZoom(zoomX, zoomY);
            if (scrollHorizontal)
                chart.DeferScrollToHorizontalOffset(horizontalOffset);
            if (scrollVertical)
                chart.DeferScrollToVerticalOffset(verticalOffset);

            SetBusy(false, chart);
        }

        /// <summary>
        /// S'assure que les zoom opérations existent.
        /// </summary>
        /// <param name="zoomable">The zoomable.</param>
        /// <param name="chart">The chart.</param>
        private void EnsureOperations(IZoommerGantt zoomable, KGanttChartView chart)
        {
            if (_zoomOperations == null)
            {
                _zoomOperations = new Subject<ZoomAction>();
                _zoomOperations
                    .BufferWithInactivity(TimeSpan.FromMilliseconds(200))
                    .Take(1)
                    .ObserveOnDispatcher()
                    .Subscribe(actions =>
                    {
                        ExecuteZoomActions(zoomable, chart, actions);
                        _zoomOperations.OnCompleted();
                        _zoomOperations.Dispose();
                        _zoomOperations = null;
                    });
            }
        }

        /// <summary>
        /// Gère l'évènement PreviewMouseWheel.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.Input.MouseWheelEventArgs"/> contenant les données de l'évènement.</param>
        private void AssociatedObject_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var zoomable = this.ExternalZoomable;

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) &&
                (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            {
                // Zoom horizontal
                // Vérifier que le scroll se fasse bien au dessus d'un chart
                var mousePosition = e.GetPosition(this.AssociatedObject);
                var chart = GetHitChart(mousePosition);
                if (chart != null)
                {
                    EnsureOperations(zoomable, chart);

                    mousePosition = e.GetPosition(chart);

                    if (e.Delta < 0)
                        _zoomOperations.OnNext(new ZoomAction { Action = ZoomActionEnum.ZoomXOut, MousePosition = mousePosition });
                    else
                        _zoomOperations.OnNext(new ZoomAction { Action = ZoomActionEnum.ZoomXIn, MousePosition = mousePosition });
                    e.Handled = true;

                    SetBusy(true, chart);
                }
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                // Shift = Scroll horizontal
                var chart = GetHitChart(e.GetPosition(this.AssociatedObject));
                if (chart != null)
                {
                    EnsureOperations(zoomable, chart);
                    bool toRight = e.Delta < 0;

                    double horizontalOffset = chart.ScrollHorizontalOffset;

                    if (toRight)
                        _zoomOperations.OnNext(new ZoomAction { Action = ZoomActionEnum.ScrollRight });
                    else
                        _zoomOperations.OnNext(new ZoomAction { Action = ZoomActionEnum.ScrollLeft });

                    SetBusy(true, chart);

                    _zoomOperations.OnCompleted();

                    e.Handled = true;
                }
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                // Ctrl + Shift = Zoom vertical
                var chart = GetHitChart(e.GetPosition(this.AssociatedObject));
                if (chart != null)
                {
                    EnsureOperations(zoomable, chart);

                    var mousePosition = e.GetPosition(chart);

                    if (e.Delta < 0)
                        _zoomOperations.OnNext(new ZoomAction { Action = ZoomActionEnum.ZoomYOut, MousePosition = mousePosition });
                    else
                        _zoomOperations.OnNext(new ZoomAction { Action = ZoomActionEnum.ZoomYIn, MousePosition = mousePosition });
                    e.Handled = true;

                    SetBusy(true, chart);
                }
            }
        }

        /// <summary>
        /// Définit l'état occupé d'un chart.
        /// </summary>
        /// <param name="isBusy"><c>true</c> si occupé.</param>
        /// <param name="chart">Le chart.</param>
        private void SetBusy(bool isBusy, KGanttChartView chart)
        {
            chart.SpinnerVisibility = isBusy ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Applique le zoom.
        /// </summary>
        /// <param name="zoomX">Le zoom X</param>
        /// <param name="zoomX">Le zoom Y</param>
        private void ApplyZoom(double zoomX, double zoomY)
        {
            var zoomable = this.ExternalZoomable;

            if (zoomable != null)
            {
                zoomable.ZoomX = zoomX;
                zoomable.ZoomY = zoomY;
            }
        }

        /// <summary>
        /// Tente d'obtenir le chart via un hit test.
        /// </summary>
        /// <param name="mousePosition">La position de la souris.</param>
        /// <returns>Le chart ou null si non trouvé.</returns>
        private KGanttChartView GetHitChart(Point mousePosition)
        {
            var baseAssociated = base.AssociatedObject as KGanttChartView;
            if (baseAssociated != null)
                return baseAssociated;

            var hitTest = this.AssociatedObject.InputHitTest(mousePosition) as DependencyObject;
            if (hitTest != null)
            {
                var chart = VisualTreeHelperExtensions.TryFindParent<KGanttChartView>(hitTest);
                return chart;
            }
            return null;
        }

        /// <summary>
        /// Obtient le zoom sauvegardé.
        /// </summary>
        /// <returns>Le zoom sauvegardé.</returns>
        private (double X, double Y)? GetSavedZoom()
        {
            if (!DesignMode.IsInDesignMode)
            {
                var serviceBus = IoC.Resolve<IServiceBus>();
                var projectService = serviceBus.Get<IProjectManagerService>();
                var navigationService = serviceBus.Get<INavigationService>();

                if (navigationService.Preferences != null && projectService.CurrentScenario != null)
                {
                    if (navigationService.Preferences.GanttZooms.ContainsKey(projectService.CurrentScenario.Id))
                        return navigationService.Preferences.GanttZooms[projectService.CurrentScenario.Id];
                }
            }

            return null;
        }

        /// <summary>
        /// Une action de zoom.
        /// </summary>
        private class ZoomAction
        {
            /// <summary>
            /// Obtient ou définit la position de la souris.
            /// </summary>
            public Point MousePosition { get; set; }

            /// <summary>
            /// Obtient ou définit l'action réalisée.
            /// </summary>
            public ZoomActionEnum Action { get; set; }
        }

        private enum ZoomActionEnum
        {
            ZoomXIn,
            ZoomXOut,
            ZoomYIn,
            ZoomYOut,
            ScrollRight,
            ScrollLeft
        }
    }
}
