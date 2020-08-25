using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Identique  <see cref="GridSplitter"/> mais permet de définir les index de ligne/colonne explicitement.
    /// </summary>
    [StyleTypedProperty(Property = "PreviewStyle", StyleTargetType = typeof(Control))]
    public class CustomGridSplitter : Thumb
    {
        // Fields
        private ResizeData _resizeData;
        public static readonly DependencyProperty DragIncrementProperty = DependencyProperty.Register("DragIncrement", typeof(double), typeof(CustomGridSplitter), new FrameworkPropertyMetadata(1.0), new ValidateValueCallback(CustomGridSplitter.IsValidDelta));
        public static readonly DependencyProperty KeyboardIncrementProperty = DependencyProperty.Register("KeyboardIncrement", typeof(double), typeof(CustomGridSplitter), new FrameworkPropertyMetadata(10.0), new ValidateValueCallback(CustomGridSplitter.IsValidDelta));
        public static readonly DependencyProperty PreviewStyleProperty = DependencyProperty.Register("PreviewStyle", typeof(Style), typeof(CustomGridSplitter), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ResizeDirectionProperty = DependencyProperty.Register("ResizeDirection", typeof(GridResizeDirection), typeof(CustomGridSplitter), new FrameworkPropertyMetadata(GridResizeDirection.Auto, new PropertyChangedCallback(CustomGridSplitter.UpdateCursor)), new ValidateValueCallback(CustomGridSplitter.IsValidResizeDirection));
        public static readonly DependencyProperty ShowsPreviewProperty = DependencyProperty.Register("ShowsPreview", typeof(bool), typeof(CustomGridSplitter), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Obtient ou définit l'index 1 de la definition.
        /// </summary>
        public int DefinitionIndex1
        {
            get { return (int)GetValue(DefinitionIndex1Property); }
            set { SetValue(DefinitionIndex1Property, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="DefinitionIndex1"/>.
        /// </summary>
        public static readonly DependencyProperty DefinitionIndex1Property =
            DependencyProperty.Register("DefinitionIndex1", typeof(int), typeof(CustomGridSplitter),
            new UIPropertyMetadata(0));

        /// <summary>
        /// Obtient ou définit l'index 2 de la définition.
        /// </summary>
        public int DefinitionIndex2
        {
            get { return (int)GetValue(DefinitionIndex2Property); }
            set { SetValue(DefinitionIndex2Property, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="DefinitionIndex2"/>.
        /// </summary>
        public static readonly DependencyProperty DefinitionIndex2Property =
            DependencyProperty.Register("DefinitionIndex2", typeof(int), typeof(CustomGridSplitter),
            new UIPropertyMetadata(0));


        // Methods
        static CustomGridSplitter()
        {
            EventManager.RegisterClassHandler(typeof(CustomGridSplitter), Thumb.DragStartedEvent, new DragStartedEventHandler(CustomGridSplitter.OnDragStarted));
            EventManager.RegisterClassHandler(typeof(CustomGridSplitter), Thumb.DragDeltaEvent, new DragDeltaEventHandler(CustomGridSplitter.OnDragDelta));
            EventManager.RegisterClassHandler(typeof(CustomGridSplitter), Thumb.DragCompletedEvent, new DragCompletedEventHandler(CustomGridSplitter.OnDragCompleted));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomGridSplitter), new FrameworkPropertyMetadata(typeof(CustomGridSplitter)));
            UIElement.FocusableProperty.OverrideMetadata(typeof(CustomGridSplitter), new FrameworkPropertyMetadata(true));
            FrameworkElement.HorizontalAlignmentProperty.OverrideMetadata(typeof(CustomGridSplitter), new FrameworkPropertyMetadata(HorizontalAlignment.Right));
            FrameworkElement.CursorProperty.OverrideMetadata(typeof(CustomGridSplitter), new FrameworkPropertyMetadata(null, new CoerceValueCallback(CustomGridSplitter.CoerceCursor)));
        }

        private void CancelResize()
        {
            DependencyObject parent = base.Parent;
            if (this._resizeData.ShowsPreview)
            {
                this.RemovePreviewAdorner();
            }
            else
            {
                SetDefinitionLength(this._resizeData.Definition1, this._resizeData.OriginalDefinition1Length);
                SetDefinitionLength(this._resizeData.Definition2, this._resizeData.OriginalDefinition2Length);
            }
            this._resizeData = null;
        }

        private static object CoerceCursor(DependencyObject o, object value)
        {
            CustomGridSplitter splitter = (CustomGridSplitter)o;
            switch (splitter.GetEffectiveResizeDirection())
            {
                case GridResizeDirection.Columns:
                    return Cursors.SizeWE;

                case GridResizeDirection.Rows:
                    return Cursors.SizeNS;
            }
            return value;
        }

        private double GetActualLength(DefinitionBase definition)
        {
            ColumnDefinition definition2 = definition as ColumnDefinition;
            if (definition2 != null)
            {
                return definition2.ActualWidth;
            }
            return ((RowDefinition)definition).ActualHeight;
        }

        private static double GetUserMinSize(DefinitionBase definition)
        {
            return (double)definition.GetValue(definition is ColumnDefinition ? ColumnDefinition.MinWidthProperty : RowDefinition.MinHeightProperty);
        }

        private static double GetUserMaxSize(DefinitionBase definition)
        {
            return (double)definition.GetValue(definition is ColumnDefinition ? ColumnDefinition.MaxWidthProperty : RowDefinition.MaxHeightProperty);
        }

        private static GridLength GetUserSize(DefinitionBase definition)
        {
            return (GridLength)definition.GetValue(definition is ColumnDefinition ? ColumnDefinition.WidthProperty : RowDefinition.HeightProperty);
        }

        private void GetDeltaConstraints(out double minDelta, out double maxDelta)
        {
            double actualLength = this.GetActualLength(this._resizeData.Definition1);
            double userMinSizeValueCache = GetUserMinSize(this._resizeData.Definition1);
            double userMaxSizeValueCache = GetUserMaxSize(this._resizeData.Definition1);
            double num4 = this.GetActualLength(this._resizeData.Definition2);
            double num5 = GetUserMinSize(this._resizeData.Definition2);
            double num6 = GetUserMaxSize(this._resizeData.Definition2);
            if (this._resizeData.SplitterIndex == this._resizeData.Definition1Index)
            {
                userMinSizeValueCache = Math.Max(userMinSizeValueCache, this._resizeData.SplitterLength);
            }
            else if (this._resizeData.SplitterIndex == this._resizeData.Definition2Index)
            {
                num5 = Math.Max(num5, this._resizeData.SplitterLength);
            }
            if (this._resizeData.SplitBehavior == SplitBehavior.Split)
            {
                minDelta = -Math.Min((double)(actualLength - userMinSizeValueCache), (double)(num6 - num4));
                maxDelta = Math.Min((double)(userMaxSizeValueCache - actualLength), (double)(num4 - num5));
            }
            else if (this._resizeData.SplitBehavior == SplitBehavior.Resize1)
            {
                minDelta = userMinSizeValueCache - actualLength;
                maxDelta = userMaxSizeValueCache - actualLength;
            }
            else
            {
                minDelta = num4 - num6;
                maxDelta = num4 - num5;
            }
        }

        private GridResizeDirection GetEffectiveResizeDirection()
        {
            GridResizeDirection resizeDirection = this.ResizeDirection;
            if (resizeDirection != GridResizeDirection.Auto)
            {
                return resizeDirection;
            }
            if (base.HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                return GridResizeDirection.Columns;
            }
            if ((base.VerticalAlignment == VerticalAlignment.Stretch) && (base.ActualWidth <= base.ActualHeight))
            {
                return GridResizeDirection.Columns;
            }
            return GridResizeDirection.Rows;
        }

        private static DefinitionBase GetGridDefinition(Grid grid, int index, GridResizeDirection direction)
        {
            if (direction != GridResizeDirection.Columns)
            {
                return grid.RowDefinitions[index];
            }
            return grid.ColumnDefinitions[index];
        }

        private void InitializeData(bool ShowsPreview)
        {
            Grid parent = base.Parent as Grid;
            if (parent != null)
            {
                this._resizeData = new ResizeData();
                this._resizeData.Grid = parent;
                this._resizeData.ShowsPreview = ShowsPreview;
                this._resizeData.ResizeDirection = this.GetEffectiveResizeDirection();
                this._resizeData.SplitterLength = Math.Min(base.ActualWidth, base.ActualHeight);
                if (!this.SetupDefinitionsToResize())
                {
                    this._resizeData = null;
                }
                else
                {
                    this.SetupPreview();
                }
            }
        }

        private static bool IsStar(DefinitionBase definition)
        {
            return GetUserSize(definition).IsStar;
        }

        private static bool IsValidDelta(object o)
        {
            double d = (double)o;
            return ((d > 0.0) && !double.IsPositiveInfinity(d));
        }

        private static bool IsValidResizeBehavior(object o)
        {
            GridResizeBehavior behavior = (GridResizeBehavior)o;
            if (((behavior != GridResizeBehavior.BasedOnAlignment) && (behavior != GridResizeBehavior.CurrentAndNext)) && (behavior != GridResizeBehavior.PreviousAndCurrent))
            {
                return (behavior == GridResizeBehavior.PreviousAndNext);
            }
            return true;
        }

        private static bool IsValidResizeDirection(object o)
        {
            GridResizeDirection direction = (GridResizeDirection)o;
            if ((direction != GridResizeDirection.Auto) && (direction != GridResizeDirection.Columns))
            {
                return (direction == GridResizeDirection.Rows);
            }
            return true;
        }

        internal bool KeyboardMoveSplitter(double horizontalChange, double verticalChange)
        {
            if (this._resizeData != null)
            {
                return false;
            }
            this.InitializeData(false);
            if (this._resizeData == null)
            {
                return false;
            }
            if (base.FlowDirection == FlowDirection.RightToLeft)
            {
                horizontalChange = -horizontalChange;
            }
            this.MoveSplitter(horizontalChange, verticalChange);
            this._resizeData = null;
            return true;
        }

        private void MoveSplitter(double horizontalChange, double verticalChange)
        {
            double num;
            if (this._resizeData.ResizeDirection == GridResizeDirection.Columns)
            {
                num = horizontalChange;
            }
            else
            {
                num = verticalChange;
            }
            DefinitionBase definition = this._resizeData.Definition1;
            DefinitionBase base3 = this._resizeData.Definition2;
            if ((definition != null) && (base3 != null))
            {
                double actualLength = this.GetActualLength(definition);
                double num3 = this.GetActualLength(base3);
                if ((this._resizeData.SplitBehavior == SplitBehavior.Split) && !AreClose(actualLength + num3, this._resizeData.OriginalDefinition1ActualLength + this._resizeData.OriginalDefinition2ActualLength))
                {
                    this.CancelResize();
                }
                else
                {
                    double num4;
                    double num5;
                    this.GetDeltaConstraints(out num4, out num5);
                    if (base.FlowDirection != this._resizeData.Grid.FlowDirection)
                    {
                        num = -num;
                    }
                    num = Math.Min(Math.Max(num, num4), num5);
                    double num6 = actualLength + num;
                    double num7 = (actualLength + num3) - num6;
                    this.SetLengths(num6, num7);
                }
            }
        }

        private void OnDragCompleted(DragCompletedEventArgs e)
        {
            if (this._resizeData != null)
            {
                if (this._resizeData.ShowsPreview)
                {
                    this.MoveSplitter(this._resizeData.Adorner.OffsetX, this._resizeData.Adorner.OffsetY);
                    this.RemovePreviewAdorner();
                }
                this._resizeData = null;
            }
        }

        private static void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            (sender as CustomGridSplitter).OnDragCompleted(e);
        }

        private void OnDragDelta(DragDeltaEventArgs e)
        {
            if (this._resizeData != null)
            {
                double horizontalChange = e.HorizontalChange;
                double verticalChange = e.VerticalChange;
                double dragIncrement = this.DragIncrement;
                horizontalChange = Math.Round((double)(horizontalChange / dragIncrement)) * dragIncrement;
                verticalChange = Math.Round((double)(verticalChange / dragIncrement)) * dragIncrement;
                if (this._resizeData.ShowsPreview)
                {
                    if (this._resizeData.ResizeDirection == GridResizeDirection.Columns)
                    {
                        this._resizeData.Adorner.OffsetX = Math.Min(Math.Max(horizontalChange, this._resizeData.MinChange), this._resizeData.MaxChange);
                    }
                    else
                    {
                        this._resizeData.Adorner.OffsetY = Math.Min(Math.Max(verticalChange, this._resizeData.MinChange), this._resizeData.MaxChange);
                    }
                }
                else
                {
                    this.MoveSplitter(horizontalChange, verticalChange);
                }
            }
        }

        private static void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            (sender as CustomGridSplitter).OnDragDelta(e);
        }

        private void OnDragStarted(DragStartedEventArgs e)
        {
            this.InitializeData(this.ShowsPreview);
        }

        private static void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            (sender as CustomGridSplitter).OnDragStarted(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    e.Handled = this.KeyboardMoveSplitter(-this.KeyboardIncrement, 0.0);
                    return;

                case Key.Up:
                    e.Handled = this.KeyboardMoveSplitter(0.0, -this.KeyboardIncrement);
                    return;

                case Key.Right:
                    e.Handled = this.KeyboardMoveSplitter(this.KeyboardIncrement, 0.0);
                    return;

                case Key.Down:
                    e.Handled = this.KeyboardMoveSplitter(0.0, this.KeyboardIncrement);
                    break;

                case Key.Escape:
                    if (this._resizeData == null)
                    {
                        break;
                    }
                    this.CancelResize();
                    e.Handled = true;
                    return;

                default:
                    return;
            }
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            if (this._resizeData != null)
            {
                this.CancelResize();
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            base.CoerceValue(FrameworkElement.CursorProperty);
        }

        private void RemovePreviewAdorner()
        {
            if (this._resizeData.Adorner != null)
            {
                (VisualTreeHelper.GetParent(this._resizeData.Adorner) as AdornerLayer).Remove(this._resizeData.Adorner);
            }
        }

        private static void SetDefinitionLength(DefinitionBase definition, GridLength length)
        {
            definition.SetValue((definition is ColumnDefinition) ? ColumnDefinition.WidthProperty : RowDefinition.HeightProperty, length);
        }

        private void SetLengths(double definition1Pixels, double definition2Pixels)
        {
            if (this._resizeData.SplitBehavior == SplitBehavior.Split)
            {
                IEnumerable enumerable = (this._resizeData.ResizeDirection == GridResizeDirection.Columns) ? ((IEnumerable)this._resizeData.Grid.ColumnDefinitions) : ((IEnumerable)this._resizeData.Grid.RowDefinitions);
                int num = 0;
                foreach (DefinitionBase base2 in enumerable)
                {
                    if (num == this._resizeData.Definition1Index)
                    {
                        SetDefinitionLength(base2, new GridLength(definition1Pixels, GridUnitType.Star));
                    }
                    else if (num == this._resizeData.Definition2Index)
                    {
                        SetDefinitionLength(base2, new GridLength(definition2Pixels, GridUnitType.Star));
                    }
                    else if (IsStar(base2))
                    {
                        SetDefinitionLength(base2, new GridLength(this.GetActualLength(base2), GridUnitType.Star));
                    }
                    num++;
                }
            }
            else if (this._resizeData.SplitBehavior == SplitBehavior.Resize1)
            {
                SetDefinitionLength(this._resizeData.Definition1, new GridLength(definition1Pixels));
            }
            else
            {
                SetDefinitionLength(this._resizeData.Definition2, new GridLength(definition2Pixels));
            }
        }

        private bool SetupDefinitionsToResize()
        {
            int num4 = (int)base.GetValue((this._resizeData.ResizeDirection == GridResizeDirection.Columns) ? Grid.ColumnSpanProperty : Grid.RowSpanProperty);
            if (num4 == 1)
            {
                int colRowIndex1 = this.DefinitionIndex1;
                int colRowIndex2 = this.DefinitionIndex2;
                int num = (int)base.GetValue((this._resizeData.ResizeDirection == GridResizeDirection.Columns) ? Grid.ColumnProperty : Grid.RowProperty);
                
                int num5 = (this._resizeData.ResizeDirection == GridResizeDirection.Columns) ? this._resizeData.Grid.ColumnDefinitions.Count : this._resizeData.Grid.RowDefinitions.Count;
                if ((colRowIndex1 >= 0) && (colRowIndex2 < num5))
                {
                    this._resizeData.SplitterIndex = num;
                    this._resizeData.Definition1Index = colRowIndex1;
                    this._resizeData.Definition1 = GetGridDefinition(this._resizeData.Grid, colRowIndex1, this._resizeData.ResizeDirection);
                    this._resizeData.OriginalDefinition1Length = GetUserSize(this._resizeData.Definition1);
                    this._resizeData.OriginalDefinition1ActualLength = this.GetActualLength(this._resizeData.Definition1);
                    this._resizeData.Definition2Index = colRowIndex2;
                    this._resizeData.Definition2 = GetGridDefinition(this._resizeData.Grid, colRowIndex2, this._resizeData.ResizeDirection);
                    this._resizeData.OriginalDefinition2Length = GetUserSize(this._resizeData.Definition2);
                    this._resizeData.OriginalDefinition2ActualLength = this.GetActualLength(this._resizeData.Definition2);
                    bool flag = IsStar(this._resizeData.Definition1);
                    bool flag2 = IsStar(this._resizeData.Definition2);
                    if (flag && flag2)
                    {
                        this._resizeData.SplitBehavior = SplitBehavior.Split;
                    }
                    else
                    {
                        this._resizeData.SplitBehavior = !flag ? SplitBehavior.Resize1 : SplitBehavior.Resize2;
                    }
                    return true;
                }
            }
            return false;
        }

        private void SetupPreview()
        {
            if (this._resizeData.ShowsPreview)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this._resizeData.Grid);
                if (adornerLayer != null)
                {
                    this._resizeData.Adorner = new PreviewAdorner(this, this.PreviewStyle);
                    adornerLayer.Add(this._resizeData.Adorner);
                    this.GetDeltaConstraints(out this._resizeData.MinChange, out this._resizeData.MaxChange);
                }
            }
        }

        private static void UpdateCursor(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            o.CoerceValue(FrameworkElement.CursorProperty);
        }

        // Properties
        public double DragIncrement
        {
            get
            {
                return (double)base.GetValue(DragIncrementProperty);
            }
            set
            {
                base.SetValue(DragIncrementProperty, value);
            }
        }

        public double KeyboardIncrement
        {
            get
            {
                return (double)base.GetValue(KeyboardIncrementProperty);
            }
            set
            {
                base.SetValue(KeyboardIncrementProperty, value);
            }
        }

        public Style PreviewStyle
        {
            get
            {
                return (Style)base.GetValue(PreviewStyleProperty);
            }
            set
            {
                base.SetValue(PreviewStyleProperty, value);
            }
        }

        public GridResizeDirection ResizeDirection
        {
            get
            {
                return (GridResizeDirection)base.GetValue(ResizeDirectionProperty);
            }
            set
            {
                base.SetValue(ResizeDirectionProperty, value);
            }
        }

        public bool ShowsPreview
        {
            get
            {
                return (bool)base.GetValue(ShowsPreviewProperty);
            }
            set
            {
                base.SetValue(ShowsPreviewProperty, value);
            }
        }

        internal static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }
            double num = value1 - value2;
            return ((num < 1.53E-06) && (num > -1.53E-06));
        }

        // Nested Types
        private sealed class PreviewAdorner : Adorner
        {
            // Fields
            private Decorator _decorator;
            private TranslateTransform Translation;

            // Methods
            public PreviewAdorner(CustomGridSplitter CustomGridSplitter, Style previewStyle)
                : base(CustomGridSplitter)
            {
                Control control = new Control();
                control.Style = previewStyle;
                control.IsEnabled = false;
                this.Translation = new TranslateTransform();
                this._decorator = new Decorator();
                this._decorator.Child = control;
                this._decorator.RenderTransform = this.Translation;
                base.AddVisualChild(this._decorator);
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                this._decorator.Arrange(new Rect(new Point(), finalSize));
                return finalSize;
            }

            protected override Visual GetVisualChild(int index)
            {
                if (index != 0)
                {
                    throw new ArgumentOutOfRangeException("index", index, "Visual_ArgumentOutOfRange");
                }
                return this._decorator;
            }

            // Properties
            public double OffsetX
            {
                get
                {
                    return this.Translation.X;
                }
                set
                {
                    this.Translation.X = value;
                }
            }

            public double OffsetY
            {
                get
                {
                    return this.Translation.Y;
                }
                set
                {
                    this.Translation.Y = value;
                }
            }

            protected override int VisualChildrenCount
            {
                get
                {
                    return 1;
                }
            }
        }

        private class ResizeData
        {
            // Fields
            public CustomGridSplitter.PreviewAdorner Adorner;
            public DefinitionBase Definition1;
            public int Definition1Index;
            public DefinitionBase Definition2;
            public int Definition2Index;
            public Grid Grid;
            public double MaxChange;
            public double MinChange;
            public double OriginalDefinition1ActualLength;
            public GridLength OriginalDefinition1Length;
            public double OriginalDefinition2ActualLength;
            public GridLength OriginalDefinition2Length;
            public GridResizeDirection ResizeDirection;
            public bool ShowsPreview;
            public CustomGridSplitter.SplitBehavior SplitBehavior;
            public int SplitterIndex;
            public double SplitterLength;
        }

        private enum SplitBehavior
        {
            Split,
            Resize1,
            Resize2
        }
    }



}
