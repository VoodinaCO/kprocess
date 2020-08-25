using KProcess.Presentation.Windows;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Kprocess.KL2.TabletClient.Behaviors
{
    public class ScrollableBehavior : Behavior<FrameworkElement>
    {
        const double deltaForClick = 10.0;

        ScrollViewer scrollViewer;
        Point scrollStartPoint;
        Point scrollStartOffset;
        bool _mouseIsCapturedByScrollable = false;
        bool _touchDeviceIsCapturedByScrollable = false;

        public static DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(nameof(IsEnabled), typeof(bool),
            typeof(ScrollableBehavior), new FrameworkPropertyMetadata(false, OnIsEnabledChanged));

        public delegate void ItemSelectedHandler(object sender, EventArgs e);
        public event ItemSelectedHandler ItemSelected;

        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uie)
            {
                var behColl = Interaction.GetBehaviors(uie);
                if (behColl != null)
                {
                    var existingBehavior = behColl.FirstOrDefault(b => b.GetType() == typeof(ScrollableBehavior)) as ScrollableBehavior;

                    if ((bool)e.NewValue == false && existingBehavior != null)
                        behColl.Remove(existingBehavior);
                    else if ((bool)e.NewValue == true && existingBehavior == null)
                        behColl.Add(new ScrollableBehavior());
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
            {
                AssociatedObject.Loaded += (sender, e) =>
                {
                    scrollViewer = VisualTreeHelperExtensions.FindFirstChild<ScrollViewer>(AssociatedObject);
                    if (scrollViewer != null)
                    {
                        AssociatedObject.PreviewMouseDown += Scrollable_PreviewMouseDown;
                        AssociatedObject.PreviewTouchDown += Scrollable_PreviewTouchDown;
                        AssociatedObject.PreviewMouseMove += Scrollable_PreviewMouseMove;
                        AssociatedObject.PreviewTouchMove += Scrollable_PreviewTouchMove;
                        AssociatedObject.PreviewMouseUp += Scrollable_PreviewMouseUp;
                        AssociatedObject.PreviewTouchUp += Scrollable_PreviewTouchUp;
                    }
                };
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null && scrollViewer != null)
            {
                AssociatedObject.PreviewMouseDown -= Scrollable_PreviewMouseDown;
                AssociatedObject.PreviewTouchDown -= Scrollable_PreviewTouchDown;
                AssociatedObject.PreviewMouseMove -= Scrollable_PreviewMouseMove;
                AssociatedObject.PreviewTouchMove -= Scrollable_PreviewTouchMove;
                AssociatedObject.PreviewMouseUp -= Scrollable_PreviewMouseUp;
                AssociatedObject.PreviewTouchUp -= Scrollable_PreviewTouchUp;
            }
        }

        void Scrollable_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (scrollViewer.IsMouseOver)
            {
                // Save starting point, used later when determining
                //how much to scroll.
                scrollStartPoint = e.GetPosition(AssociatedObject);
                scrollStartOffset.X = scrollViewer.HorizontalOffset;
                scrollStartOffset.Y = scrollViewer.VerticalOffset;

                // Update the cursor if can scroll or not.
                if (scrollViewer.ExtentWidth > scrollViewer.ViewportWidth && scrollViewer.ExtentHeight > scrollViewer.ViewportHeight)
                    AssociatedObject.Cursor = Cursors.ScrollAll;
                else if (scrollViewer.ExtentWidth > scrollViewer.ViewportWidth)
                    AssociatedObject.Cursor = Cursors.ScrollWE;
                else if (scrollViewer.ExtentHeight > scrollViewer.ViewportHeight)
                    AssociatedObject.Cursor = Cursors.ScrollNS;
                else
                    AssociatedObject.Cursor = Cursors.Arrow;

                _mouseIsCapturedByScrollable = true;
                e.Handled = true;
            }

            Type type = AssociatedObject.GetType().BaseType;
            var method = type.GetMethod("OnPreviewMouseDown", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(AssociatedObject, new object[] { e });
        }

        void Scrollable_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (scrollViewer.IsMouseOver)
            {
                // Save starting point, used later when determining
                //how much to scroll.
                scrollStartPoint = e.GetTouchPoint(AssociatedObject).Position;
                scrollStartOffset.X = scrollViewer.HorizontalOffset;
                scrollStartOffset.Y = scrollViewer.VerticalOffset;

                // Update the cursor if can scroll or not.
                if (scrollViewer.ExtentWidth > scrollViewer.ViewportWidth && scrollViewer.ExtentHeight > scrollViewer.ViewportHeight)
                    AssociatedObject.Cursor = Cursors.ScrollAll;
                else if (scrollViewer.ExtentWidth > scrollViewer.ViewportWidth)
                    AssociatedObject.Cursor = Cursors.ScrollWE;
                else if (scrollViewer.ExtentHeight > scrollViewer.ViewportHeight)
                    AssociatedObject.Cursor = Cursors.ScrollNS;
                else
                    AssociatedObject.Cursor = Cursors.Arrow;

                _touchDeviceIsCapturedByScrollable = true;
                e.Handled = true;
            }

            Type type = AssociatedObject.GetType().BaseType;
            var method = type.GetMethod("OnPreviewTouchDown", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(AssociatedObject, new object[] { e });
        }

        void Scrollable_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseIsCapturedByScrollable)
            {
                // Get the new scroll position.
                Point point = e.GetPosition(AssociatedObject);

                // Determine the new amount to scroll.
                Point delta = new Point((point.X > scrollStartPoint.X) ? -(point.X - scrollStartPoint.X) : (scrollStartPoint.X - point.X),
                    (point.Y > scrollStartPoint.Y) ? -(point.Y - scrollStartPoint.Y) : (scrollStartPoint.Y - point.Y));

                // Scroll to the new position.
                scrollViewer.ScrollToHorizontalOffset(scrollStartOffset.X + delta.X);
                scrollViewer.ScrollToVerticalOffset(scrollStartOffset.Y + delta.Y);
            }

            Type type = AssociatedObject.GetType().BaseType;
            var method = type.GetMethod("OnPreviewMouseMove", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(AssociatedObject, new object[] { e });
        }

        void Scrollable_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (_touchDeviceIsCapturedByScrollable)
            {
                // Get the new scroll position.
                Point point = e.GetTouchPoint(AssociatedObject).Position;

                // Determine the new amount to scroll.
                Point delta = new Point((point.X > scrollStartPoint.X) ? -(point.X - scrollStartPoint.X) : (scrollStartPoint.X - point.X),
                    (point.Y > scrollStartPoint.Y) ? -(point.Y - scrollStartPoint.Y) : (scrollStartPoint.Y - point.Y));

                // Scroll to the new position.
                scrollViewer.ScrollToHorizontalOffset(scrollStartOffset.X + delta.X);
                scrollViewer.ScrollToVerticalOffset(scrollStartOffset.Y + delta.Y);
            }

            Type type = AssociatedObject.GetType().BaseType;
            var method = type.GetMethod("OnPreviewTouchMove", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(AssociatedObject, new object[] { e });
        }

        void Scrollable_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.StylusDevice != null)
                e.Handled = true;
            else if (_mouseIsCapturedByScrollable)
            {
                AssociatedObject.Cursor = Cursors.Arrow;
                _mouseIsCapturedByScrollable = false;
                if (Math.Abs(scrollStartPoint.Y - Mouse.GetPosition(AssociatedObject).Y) > deltaForClick)
                    e.Handled = true;
                else
                    ItemSelected?.Invoke(this, e);
            }

            Type type = AssociatedObject.GetType().BaseType;
            var method = type.GetMethod("OnPreviewMouseUp", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(AssociatedObject, new object[] { e });
        }

        void Scrollable_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            if (_touchDeviceIsCapturedByScrollable)
            {
                AssociatedObject.Cursor = Cursors.Arrow;
                _touchDeviceIsCapturedByScrollable = false;
                if (Math.Abs(scrollStartPoint.Y - e.GetTouchPoint(AssociatedObject).Position.Y) > deltaForClick)
                    e.Handled = true;
                else
                    ItemSelected?.Invoke(this, e);
            }

            Type type = AssociatedObject.GetType().BaseType;
            var method = type.GetMethod("OnPreviewTouchUp", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(AssociatedObject, new object[] { e });
        }
    }
}
