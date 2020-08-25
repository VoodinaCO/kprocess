using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using DlhSoft.Windows.Controls;
using KProcess.Globalization;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    public class KGanttChartView : GanttChartView
    {
        private const string PART_TimelineThumb = "PART_TimelineThumb";
        private const string PART_TimelineLine = "PART_TimelineLine";
        private const string PART_TimelineTime = "PART_TimelineTime";

        private Thumb _timelineThumb;
        private Shape _timelineLine;
        private ContentControl _timelineTime;
        private ScrollViewer _scrollViewer;

        private bool _coercingScroll = false;

        static KGanttChartView()
        {
            TimelinePageStartProperty.OverrideMetadata(typeof(KGanttChartView), new PropertyMetadata(GanttDates.StartDate, OnTimelinePageStartChanged));
            TimelinePageFinishProperty.OverrideMetadata(typeof(KGanttChartView), new PropertyMetadata(GanttDates.StartDate, OnTimelinePageFinishChanged));

            DisplayedTimeProperty.OverrideMetadata(typeof(KGanttChartView), new PropertyMetadata(GanttDates.StartDate, OnDisplayedTimeChanged));

            IsAsyncPresentationEnabledProperty.OverrideMetadata(typeof(KGanttChartView), new PropertyMetadata(true, OnIsAsyncPresentationEnabledChanged));

            BarHeightProperty.OverrideMetadata(typeof(KGanttChartView), new PropertyMetadata(double.NaN, OnBarHeightChanged));

            DependencyDeletionContextMenuItemHeaderProperty.OverrideMetadata(typeof(KGanttChartView), new PropertyMetadata(LocalizationManagerExt.GetSafeDesignerString("Core_Gantt_DeleteLink")));
        }

        private static void OnIsAsyncPresentationEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as KGanttChartView;
            if (grid != null)
            {
                grid.UpdateItemsSource();
            }
        }

        private static void OnTimelinePageFinishChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KGanttChartView view = d as KGanttChartView;
            if (view != null)
            {
                view.ClearScheduleCacheValues();
                view.CoerceTimelinePage();
                view.UpdateComputedWidth();
                view.UpdateScales();
                view.UpdateBars();
                view.UpdateDisplayedTimeFromScroll();
                view.OnTimelinePageChanged();
            }
        }

        private static void OnTimelinePageStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KGanttChartView view = d as KGanttChartView;
            if (view != null)
            {
                view.ClearScheduleCacheValues();
                view.CoerceTimelinePage();
                view.UpdateComputedWidth();
                view.UpdateScales();
                view.UpdateBars();
                view.UpdateDisplayedTimeFromScroll();
                view.OnTimelinePageChanged();
            }
        }

        private static void OnBarHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KGanttChartView view = d as KGanttChartView;
            if (view != null)
            {
                view.UpdateBars();
            }
        }

        private static void OnDisplayedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KGanttChartView view = d as KGanttChartView;
            if (view != null)
            {
                view.OnDisplayedTimeChanged();
                view.UpdateScrollFromDisplayedTime();
            }
        }

        /// <summary>
        /// Applique le control template du contrôle.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _timelineThumb = base.HeaderElement.FindName(PART_TimelineThumb) as Thumb;

            if (_timelineThumb != null)
            {
                _timelineThumb.DragDelta += new DragDeltaEventHandler(TimelineThumb_DragDelta);
                _timelineThumb.MouseEnter += new MouseEventHandler(TimelineThumb_MouseEnter);
            }

            _timelineLine = base.GetTemplateChild(PART_TimelineLine) as Shape;
            _timelineTime = base.HeaderElement.FindName(PART_TimelineTime) as ContentControl;

            _scrollViewer = base.GetTemplateChild("ScrollViewer") as System.Windows.Controls.ScrollViewer;
            _scrollViewer.ScrollChanged += new ScrollChangedEventHandler(ScrollViewer_ScrollChanged);


            var scrollViewerRoot = (VisualTreeHelper.GetChildrenCount(_scrollViewer) == 1) ? (VisualTreeHelper.GetChild(_scrollViewer, 0) as FrameworkElement) : null;

            this.ContentElement.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(ContentElement_MouseLeftButtonDown);

            UpdateTimelinePosition();
        }

        public KGanttChartDataGrid ParentKDataGrid
        {
            get { return (KGanttChartDataGrid)base.ParentDataGrid; }
        }

        #region Clic sur gantt pour sélection

        /// <summary>
        /// Obtient ou définit la commandé executée lorsque l'utilisateur clique sur une tâche dans le Gantt.
        /// </summary>
        public ICommand TaskGanttClickCommand
        {
            get { return (ICommand)GetValue(TaskGanttClickCommandProperty); }
            set { SetValue(TaskGanttClickCommandProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="TaskGanttClickCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TaskGanttClickCommandProperty =
            DependencyProperty.Register("TaskGanttClickCommand", typeof(ICommand), typeof(KGanttChartView), new UIPropertyMetadata(null));

        /// <summary>
        /// Invoqué lors de l'évènement <see cref="E:System.Windows.UIElement.PreviewMouseLeftButtonDown"/>.
        /// </summary>
        /// <param name="e">Les event args.</param>
        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            var source = e.OriginalSource as UIElement;
            if (source != null)
            {
                var tp = source.TryFindParent<TaskPresenter>();
                if (tp != null)
                {
                    var dataGrid = this.TryFindParent<GanttChartDataGrid>();
                    if (dataGrid != null)
                    {
                        var item = (GanttChartItem)tp.DataContext;
                        if (TaskGanttClickCommand != null && TaskGanttClickCommand.CanExecute(item))
                            TaskGanttClickCommand.Execute(item);
                    }
                }
            }
        }

        #endregion

        #region Gestion de la timeline

        /// <summary>
        /// Obtient ou définit le pinceau de la timeline.
        /// </summary>
        public Brush TimelineFill
        {
            get { return (Brush)GetValue(TimelineFillProperty); }
            set { SetValue(TimelineFillProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="TimelineFill"/>.
        /// </summary>
        public static readonly DependencyProperty TimelineFillProperty =
            DependencyProperty.Register("TimelineFill", typeof(Brush), typeof(KGanttChartView), new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit la position courant du curseur de la timeline.
        /// </summary>
        public DateTime CurrentTimelinePosition
        {
            get { return (DateTime)GetValue(CurrentTimelinePositionProperty); }
            set { SetValue(CurrentTimelinePositionProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="CurrentTimelinePosition"/>.
        /// </summary>
        public static readonly DependencyProperty CurrentTimelinePositionProperty =
            DependencyProperty.Register("CurrentTimelinePosition", typeof(DateTime), typeof(KGanttChartView),
            new FrameworkPropertyMetadata(GanttDates.StartDate, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCurrentTimeLinePositionChanged, CoerceCurrentTimeLinePosition));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="CurrentTimeLinePosition"/> a changé.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnCurrentTimeLinePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chart = (KGanttChartView)d;
            chart.UpdateTimelinePosition();
        }

        /// <summary>
        /// Force la valeur de la date de time line.
        /// </summary>
        /// <param name="d">La source.</param>
        /// <param name="baseValue">La valeur de base.</param>
        /// <returns>La valeur forcée</returns>
        private static object CoerceCurrentTimeLinePosition(DependencyObject d, object baseValue)
        {
            var val = (DateTime)baseValue;
            return val < GanttDates.StartDate ? GanttDates.StartDate : val;
        }

        /// <summary>
        /// Gère l'évènement DragDelta du contrôle TimelineThumb.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> contenant les données de l'évènement.</param>
        private void TimelineThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var date = base.GetDateTime(Canvas.GetLeft(_timelineThumb) + _timelineThumb.ActualWidth / 2 + e.HorizontalChange);
            this.CurrentTimelinePosition = date;
        }

        /// <summary>
        /// Gère l'évènement MouseLeftButtonDown du contrôle ContentElement.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.Input.MouseButtonEventArgs"/> contenant les données de l'évènement.</param>
        private void ContentElement_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this.ContentElement).X;
            this.CurrentTimelinePosition = GetDateTime(position);
        }

        /// <summary>
        /// Met à jour la position de la timeline.
        /// </summary>
        private void UpdateTimelinePosition()
        {
            if (DesignMode.IsInDesignMode)
                return;

            var timeService = IoC.Resolve<IServiceBus>().Get<ITimeTicksFormatService>();

            var position = base.GetPosition(this.CurrentTimelinePosition);
            if (position < 0)
                position = 0;

            if (_timelineThumb != null && _timelineLine != null)
            {
                var nextLeft = position - _timelineThumb.ActualWidth / 2.0;
                if (Canvas.GetLeft(_timelineThumb) != nextLeft)
                {
                    Canvas.SetLeft(_timelineThumb, nextLeft);
                    Canvas.SetLeft(_timelineLine, position);

                    if (_timelineTime != null)
                    {
                        _timelineTime.Content = timeService.TicksToString(GanttDates.ToTicks(this.CurrentTimelinePosition));

                        ShowTimelinePopup();
                    }
                }
            }
        }

        /// <summary>
        /// Affiche le popup de la timeline
        /// </summary>
        private void ShowTimelinePopup()
        {
            _timelineTime.Visibility = System.Windows.Visibility.Visible;

            var thumbRight = Canvas.GetLeft(_timelineThumb) + _timelineThumb.ActualWidth;

            var position = Canvas.GetLeft(_timelineThumb) + _timelineThumb.ActualWidth + 5;
            if (position + _timelineTime.ActualWidth > _scrollViewer.ViewportWidth)
                position = Canvas.GetLeft(_timelineThumb) - _timelineTime.ActualWidth - 5;


            Canvas.SetLeft(_timelineTime, position);
        }

        /// <summary>
        /// Handles the MouseEnter event of the TimelineThumb control.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.Input.MouseEventArgs"/> contenant les données de l'évènement.</param>
        private void TimelineThumb_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_timelineTime != null)
                ShowTimelinePopup();
        }

        /// <summary>
        /// Met à jour la le temps disponible pour le défilement.
        /// </summary>
        protected override void UpdateDisplayedTimeFromScroll()
        {
            base.UpdateDisplayedTimeFromScroll();
            UpdateTimelinePosition();
        }

        #endregion

        #region Gestion décalage scrollbar

        #region Horizontal

        /// <summary>
        /// Obtient ou définit le scroll horizontal
        /// </summary>
        public double ScrollHorizontalOffset
        {
            get { return (double)GetValue(ScrollHorizontalOffsetProperty); }
            set { SetValue(ScrollHorizontalOffsetProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="ScrollHorizontalOffset"/>.
        /// </summary>
        public static readonly DependencyProperty ScrollHorizontalOffsetProperty =
            DependencyProperty.Register("ScrollHorizontalOffset", typeof(double), typeof(KGanttChartView),
            new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnScrollHorizontalOffsetChanged, OnCoerceScrollHorizontalOffset));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="ScrollHorizontalOffset"/> a changé.
        /// </summary>
        /// <param name="d">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnScrollHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (KGanttChartView)d;
            var val = (double)e.NewValue;
            if (!source._coercingScroll)
                source._scrollViewer.ScrollToHorizontalOffset(val);
        }

        private static object OnCoerceScrollHorizontalOffset(DependencyObject d, object baseValue)
        {
            var val = (double)baseValue;
            return val <= 0 ? 0 : val;
        }

        private double? _nextHorizontalOffsetScroll;
        public void DeferScrollToHorizontalOffset(double value)
        {
            if (_scrollViewer.IsArrangeValid)
            {
                _nextHorizontalOffsetScroll = null;
                _scrollViewer.ScrollToHorizontalOffset(value);
            }
            else
            {
                _nextHorizontalOffsetScroll = value;
                _scrollViewer.LayoutUpdated += new EventHandler(_scrollViewer_LayoutUpdated);
            }
        }

        #endregion

        #region Vertical

        /// <summary>
        /// Obtient ou définit le scroll Vertical
        /// </summary>
        public double ScrollVerticalOffset
        {
            get { return (double)GetValue(ScrollVerticalOffsetProperty); }
            set { SetValue(ScrollVerticalOffsetProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="ScrollVerticalOffset"/>.
        /// </summary>
        public static readonly DependencyProperty ScrollVerticalOffsetProperty =
            DependencyProperty.Register("ScrollVerticalOffset", typeof(double), typeof(KGanttChartView),
            new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnScrollVerticalOffsetChanged, OnCoerceScrollVerticalOffset));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="ScrollVerticalOffset"/> a changé.
        /// </summary>
        /// <param name="d">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnScrollVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (KGanttChartView)d;
            var val = (double)e.NewValue;
            if (!source._coercingScroll)
                source._scrollViewer.ScrollToVerticalOffset(val);
        }

        private static object OnCoerceScrollVerticalOffset(DependencyObject d, object baseValue)
        {
            var val = (double)baseValue;
            return val <= 0 ? 0 : val;
        }

        private double? _nextVerticalOffsetScroll;
        public void DeferScrollToVerticalOffset(double value)
        {
            if (_scrollViewer.IsArrangeValid)
            {
                _nextHorizontalOffsetScroll = null;
                _scrollViewer.ScrollToVerticalOffset(value);
            }
            else
            {
                _nextVerticalOffsetScroll = value;
                _scrollViewer.LayoutUpdated += new EventHandler(_scrollViewer_LayoutUpdated);
            }
        }

        #endregion

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            _coercingScroll = true;
            this.ScrollHorizontalOffset = e.HorizontalOffset;
            this.ScrollVerticalOffset = e.VerticalOffset;
            _coercingScroll = false;
        }

        void _scrollViewer_LayoutUpdated(object sender, EventArgs e)
        {
            _scrollViewer.LayoutUpdated -= new EventHandler(_scrollViewer_LayoutUpdated);

            if (_nextHorizontalOffsetScroll.HasValue)
            {
                _scrollViewer.ScrollToHorizontalOffset(_nextHorizontalOffsetScroll.Value);
                _nextHorizontalOffsetScroll = null;
            }

            if (_nextVerticalOffsetScroll.HasValue)
            {
                _scrollViewer.ScrollToVerticalOffset(_nextVerticalOffsetScroll.Value);
                _nextVerticalOffsetScroll = null;
            }
        }

        #endregion

        #region Gestion du gain

        /// <summary>
        /// Obtient ou définit le OriginalGainVisibility.
        /// </summary>
        public Visibility OriginalGainVisibility
        {
            get { return (Visibility)GetValue(OriginalGainVisibilityProperty); }
            set { SetValue(OriginalGainVisibilityProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="OriginalGainVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OriginalGainVisibilityProperty =
            DependencyProperty.Register("OriginalGainVisibility", typeof(Visibility), typeof(KGanttChartView),
            new UIPropertyMetadata(Visibility.Collapsed));

        #endregion

        #region Gestion du Spinner

        /// <summary>
        /// Obtient ou définit la visibilité du Spinner.
        /// </summary>
        public Visibility SpinnerVisibility
        {
            get { return (Visibility)GetValue(SpinnerVisibilityProperty); }
            set { SetValue(SpinnerVisibilityProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="SpinnerVisibility"/>.
        /// </summary>
        public static readonly DependencyProperty SpinnerVisibilityProperty =
            DependencyProperty.Register("SpinnerVisibility", typeof(Visibility), typeof(KGanttChartView), new UIPropertyMetadata(Visibility.Collapsed));

        #endregion
    }
}
