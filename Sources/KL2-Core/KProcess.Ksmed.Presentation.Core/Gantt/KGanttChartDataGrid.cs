using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using DlhSoft.Windows.Controls;
using KProcess.Globalization;
using KProcess.Presentation.Windows;
using Microsoft.Expression.Interactivity.Layout;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Représente un Gantt et une grille.
    /// </summary>
    [TemplatePart(Name = KGanttChartDataGrid.PART_CrossAirCanvas, Type = typeof(Canvas))]
    public class KGanttChartDataGrid : GanttChartDataGrid, IZoomableGanttTarget
    {

        private const string PART_CrossAirCanvas = "PART_CrossAirCanvas";

        private Canvas _crossAirCanvas;
        private Line _crossAirVerticalLine;
        private Line _crossAirHorizontalLine;

        /// <summary>
        /// Initialise la classe <see cref="KGanttChartDataGrid"/>.
        /// </summary>
        static KGanttChartDataGrid()
        {
            DataGridWidthProperty.OverrideMetadata(typeof(KGanttChartDataGrid),
                new PropertyMetadata(new GridLength(0.3, GridUnitType.Star)));
            ChartWidthProperty.OverrideMetadata(typeof(KGanttChartDataGrid),
                new PropertyMetadata(new GridLength(0.7, GridUnitType.Star)));

            TimelinePageStartProperty.OverrideMetadata(typeof(KGanttChartDataGrid), new PropertyMetadata(GanttDates.StartDate, OnTimelinePageStartChanged));
            TimelinePageFinishProperty.OverrideMetadata(typeof(KGanttChartDataGrid), new PropertyMetadata(GanttDates.StartDate, OnTimelinePageFinishChanged));

            DisplayedTimeProperty.OverrideMetadata(typeof(KGanttChartDataGrid), new PropertyMetadata(GanttDates.StartDate, OnDisplayedTimeChanged));

            IsAsyncPresentationEnabledProperty.OverrideMetadata(typeof(KGanttChartDataGrid), new PropertyMetadata(true, OnIsAsyncPresentationEnabledChanged));

            DependencyDeletionContextMenuItemHeaderProperty.OverrideMetadata(typeof(KGanttChartDataGrid), new PropertyMetadata(LocalizationManagerExt.GetSafeDesignerString("Core_Gantt_DeleteLink")));
        }

        private static void OnIsAsyncPresentationEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = d as KGanttChartDataGrid;
            if (grid != null)
            {
                grid.UpdateItemsSource();
            }
        }

        public override void OnApplyTemplate()
        {
            this.DataTreeGrid = base.GetTemplateChild("DataTreeGrid") as DlhSoft.Windows.Controls.DataTreeGrid;
            _crossAirCanvas = base.GetTemplateChild(PART_CrossAirCanvas) as Canvas;
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Obtient la grille interne.
        /// </summary>
        public new DataTreeGrid DataTreeGrid { get; private set; }

        public new KGanttChartView GanttChartView
        {
            get { return (KGanttChartView)base.GanttChartView; }
        }

        #region Correction gestion timings

        private static void OnTimelinePageStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KGanttChartDataGrid grid = d as KGanttChartDataGrid;
            if (grid != null)
            {
                grid.CoerceTimelinePage();
                grid.OnTimelinePageChanged();
            }
        }

        private static void OnTimelinePageFinishChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KGanttChartDataGrid grid = d as KGanttChartDataGrid;
            if (grid != null)
            {
                grid.CoerceTimelinePage();
                grid.OnTimelinePageChanged();
            }
        }

        private static void OnDisplayedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KGanttChartDataGrid grid = d as KGanttChartDataGrid;
            if (grid != null)
            {
                grid.UpdateChartDisplayedTime();
            }
        }

        #endregion

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
            DependencyProperty.Register("TaskGanttClickCommand", typeof(ICommand), typeof(KGanttChartDataGrid), new UIPropertyMetadata(null));

        #endregion

        #region Gestion timeline

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
            DependencyProperty.Register("CurrentTimelinePosition", typeof(DateTime), typeof(KGanttChartDataGrid),
            new FrameworkPropertyMetadata(DateTime.MinValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        #endregion

        #region Gestion décalage scrollbar

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
            DependencyProperty.Register("ScrollHorizontalOffset", typeof(double), typeof(KGanttChartDataGrid),
            new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

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
            DependencyProperty.Register("OriginalGainVisibility", typeof(Visibility), typeof(KGanttChartDataGrid),
            new UIPropertyMetadata(Visibility.Collapsed));

        #endregion

        #region Gestion de l'auto scale & Zoom

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le Gantt est prêt à être auto-scalé.
        /// </summary>
        public bool IsReadyForAutoScale { get; set; }

        public double ComputedIdealY { get; set; }

        public ZoomYInitialValues ZoomYInitialValues { get; set; }

        public event EventHandler ReadyForAutoScale;

        protected override void UpdateItemsSource()
        {
            base.UpdateItemsSource();
            TryAutoScale();
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var arr = base.ArrangeOverride(arrangeBounds);
            TryAutoScale();
            return arr;
        }

        /// <summary>
        /// Tente d'appliquer l'autoscale.
        /// </summary>
        private void TryAutoScale()
        {
            if (!IsReadyForAutoScale && this.ManagedItems != null && this.ManagedItems.Any() && this.GanttChartView != null &&
                this.DesiredSize != default(Size) && this.IsVisible)
            {
                IsReadyForAutoScale = true;
                if (ReadyForAutoScale != null)
                {
                    ReadyForAutoScale(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Gestion du cross air

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'affichage Cross Air est activé.
        /// </summary>
        public bool EnableCrossAir
        {
            get { return (bool)GetValue(EnableCrossAirProperty); }
            set { SetValue(EnableCrossAirProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="EnableCrossAir"/>.
        /// </summary>
        public static readonly DependencyProperty EnableCrossAirProperty =
            DependencyProperty.Register("EnableCrossAir", typeof(bool), typeof(KGanttChartDataGrid), new UIPropertyMetadata(true));

        /// <summary>
        /// Obtient ou définit le style appliqué aux lignes du cross air.
        /// </summary>
        public Style CrossAirLineStyle
        {
            get { return (Style)GetValue(CrossAirLineStyleProperty); }
            set { SetValue(CrossAirLineStyleProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="CrossAirLineStyle"/>.
        /// </summary>
        public static readonly DependencyProperty CrossAirLineStyleProperty =
            DependencyProperty.Register("CrossAirLineStyle", typeof(Style), typeof(KGanttChartDataGrid), new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit l'épaisseur des lines du cross air.
        /// </summary>
        public double CrossAirLineThickness
        {
            get { return (double)GetValue(CrossAirLineThicknessProperty); }
            set { SetValue(CrossAirLineThicknessProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="CrossAirLineThickness"/>.
        /// </summary>
        public static readonly DependencyProperty CrossAirLineThicknessProperty =
            DependencyProperty.Register("CrossAirLineThickness", typeof(double), typeof(KGanttChartDataGrid), new UIPropertyMetadata(1d));

        //private CrossAirAdorner _crossAirAdorner;

        /// <summary>
        /// Appelé lorsque la souris bouge au dessus du contrôle.
        /// </summary>
        /// <param name="e">les event args.</param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            if (this.EnableCrossAir)
            {
                UpdateCrossAir(e.GetPosition(this));
            }
        }

        /// <summary>
        /// Appelé lorsque la souris quitte le contrôle.
        /// </summary>
        /// <param name="e">les event args.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            HideCrossAir();
        }

        /// <summary>
        /// Met à jour la position du cross air.
        /// </summary>
        /// <param name="position">La position de la jonction des lignes.</param>
        private void UpdateCrossAir(Point position)
        {
            if (_crossAirCanvas != null)
            {
                if (_crossAirVerticalLine == null || _crossAirHorizontalLine == null)
                {
                    _crossAirVerticalLine = new Line()
                    {
                        Y2 = 1,
                        Width = this.CrossAirLineThickness,
                        StrokeThickness = this.CrossAirLineThickness,
                        Stretch = Stretch.Fill,
                        Style = this.CrossAirLineStyle,
                        IsHitTestVisible = false,
                    };

                    _crossAirHorizontalLine = new Line()
                    {
                        X2 = 1,
                        Height = this.CrossAirLineThickness,
                        StrokeThickness = this.CrossAirLineThickness,
                        Stretch = Stretch.Fill,
                        Style = this.CrossAirLineStyle,
                        IsHitTestVisible = false,
                    };

                    _crossAirCanvas.Children.Add(_crossAirVerticalLine);
                    _crossAirCanvas.Children.Add(_crossAirHorizontalLine);
                }

                _crossAirVerticalLine.Height = this.ActualHeight;
                _crossAirHorizontalLine.Width = this.ActualWidth;

                position = this.TranslatePoint(position, _crossAirCanvas);

                Canvas.SetLeft(_crossAirVerticalLine, position.X);
                Canvas.SetTop(_crossAirHorizontalLine, position.Y);
            }
        }

        /// <summary>
        /// Cache le cross air.
        /// </summary>
        private void HideCrossAir()
        {
            if (_crossAirCanvas != null)
            {
                _crossAirCanvas.Children.Clear();
                _crossAirVerticalLine = null;
                _crossAirHorizontalLine = null;
            }
        }

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
            DependencyProperty.Register("SpinnerVisibility", typeof(Visibility), typeof(KGanttChartDataGrid), new UIPropertyMetadata(Visibility.Collapsed));

        #endregion
    }
}
