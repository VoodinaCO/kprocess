using KProcess.Presentation.Windows;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace Kprocess.KL2.TabletClient.Behaviors
{
    public class FrictionScrollableBehavior : Behavior<FrameworkElement>
    {
        const double deltaForClick = 10.0;

        ScrollViewer scrollViewer;
        Point scrollTarget;
        Point scrollStartPoint;
        Point scrollStartOffset;
        Point previousPoint;
        Vector velocity;
        bool _mouseIsCapturedByScrollable = false;
        bool _touchDeviceIsCapturedByScrollable = false;
        TouchDevice _touchDevice;
        readonly DispatcherTimer animationTimer = new DispatcherTimer();

        public static DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(nameof(IsEnabled), typeof(bool),
            typeof(FrictionScrollableBehavior), new FrameworkPropertyMetadata(false, OnIsEnabledChanged));

        public static DependencyProperty FrictionProperty =
            DependencyProperty.Register(nameof(Friction), typeof(double),
            typeof(FrictionScrollableBehavior), new PropertyMetadata(0.05));

        public delegate void ItemSelectedHandler(object sender, EventArgs e);
        public event ItemSelectedHandler ItemSelected;

        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public double Friction
        {
            get => (double)GetValue(FrictionProperty);
            set => SetValue(FrictionProperty, value);
        }

        static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uie)
            {
                var behColl = Interaction.GetBehaviors(uie);
                if (behColl != null)
                {
                    var existingBehavior = behColl.FirstOrDefault(b => b.GetType() == typeof(FrictionScrollableBehavior)) as FrictionScrollableBehavior;

                    if ((bool)e.NewValue == false && existingBehavior != null)
                        behColl.Remove(existingBehavior);
                    else if ((bool)e.NewValue == true && existingBehavior == null)
                        behColl.Add(new FrictionScrollableBehavior());
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

                        animationTimer.Interval = TimeSpan.FromMilliseconds(20);
                        animationTimer.Tick += new EventHandler(HandleFrictionTimerTick);
                    }
                };
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null && scrollViewer != null)
            {
                animationTimer.Stop();
                animationTimer.Tick -= HandleFrictionTimerTick;

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
                animationTimer.Start();
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
                animationTimer.Start();
                // Save starting point, used later when determining
                //how much to scroll.
                _touchDevice = e.TouchDevice;
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
                Point currentPoint = e.GetPosition(AssociatedObject);

                // Determine the new amount to scroll.
                Point delta = new Point(scrollStartPoint.X - currentPoint.X,
                    scrollStartPoint.Y - currentPoint.Y);

                scrollTarget.X = scrollStartOffset.X + delta.X;
                scrollTarget.Y = scrollStartOffset.Y + delta.Y;

                // Scroll to the new position.
                scrollViewer.ScrollToHorizontalOffset(scrollTarget.X);
                scrollViewer.ScrollToVerticalOffset(scrollTarget.Y);
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
                Point currentPoint = e.GetTouchPoint(AssociatedObject).Position;

                // Determine the new amount to scroll.
                Point delta = new Point(scrollStartPoint.X - currentPoint.X,
                    scrollStartPoint.Y - currentPoint.Y);

                scrollTarget.X = scrollStartOffset.X + delta.X;
                scrollTarget.Y = scrollStartOffset.Y + delta.Y;

                // Scroll to the new position.
                scrollViewer.ScrollToHorizontalOffset(scrollTarget.X);
                scrollViewer.ScrollToVerticalOffset(scrollTarget.Y);
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
                {
                    animationTimer.Stop();
                    ItemSelected?.Invoke(this, e);
                }
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
                {
                    animationTimer.Stop();
                    ItemSelected?.Invoke(this, e);
                }
            }

            Type type = AssociatedObject.GetType().BaseType;
            var method = type.GetMethod("OnPreviewTouchUp", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(AssociatedObject, new object[] { e });
        }

        void HandleFrictionTimerTick(object sender, EventArgs e)
        {
            if (_mouseIsCapturedByScrollable)
            {
                Point currentPoint = Mouse.GetPosition(AssociatedObject);
                velocity = previousPoint - currentPoint;
                previousPoint = currentPoint;
            }
            else if (_touchDeviceIsCapturedByScrollable)
            {
                Point currentPoint = _touchDevice.GetTouchPoint(AssociatedObject).Position;
                velocity = previousPoint - currentPoint;
                previousPoint = currentPoint;
            }
            else
            {
                if (velocity.Length > 1)
                {
                    scrollViewer.ScrollToHorizontalOffset(scrollTarget.X);
                    scrollViewer.ScrollToVerticalOffset(scrollTarget.Y);
                    scrollTarget.X += velocity.X;
                    scrollTarget.Y += velocity.Y;
                    velocity *= (1.0 - Math.Max(Math.Min(Friction, 1.0), 0.0));
                }
            }
        }
    }
}
