using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Point = System.Windows.Point;

namespace Kprocess.TabTip
{
    static class AnimationHelper
    {
        static readonly Dictionary<FrameworkElement, Storyboard> MoveRootVisualStoryboards = new Dictionary<FrameworkElement, Storyboard>();

        static Point GetCurrentUIElementPoint(Visual element) => element.PointToScreen(new Point(0, 0)).ToPointInLogicalUnits(element);

        internal static event Action<Exception> ExceptionCatched;
        static Rectangle ToRectangleInLogicalUnits(this Rectangle rectangleToConvert, DependencyObject element)
        {
            const float logicalUnitDpi = 96.0f;
            // ReSharper disable once AssignNullToNotNullAttribute
            IntPtr windowHandle = new WindowInteropHelper(Window.GetWindow(element)).EnsureHandle();

            using (Graphics graphics = Graphics.FromHwnd(windowHandle))
                return Rectangle.FromLTRB(
                    (int)(rectangleToConvert.Left * logicalUnitDpi / graphics.DpiX),
                    (int)(rectangleToConvert.Top * logicalUnitDpi / graphics.DpiY),
                    (int)(rectangleToConvert.Right * logicalUnitDpi / graphics.DpiX),
                    (int)(rectangleToConvert.Bottom * logicalUnitDpi / graphics.DpiY));
        }

        static Point ToPointInLogicalUnits(this Point point, DependencyObject element)
        {
            const float logicalUnitDpi = 96.0f;

            // ReSharper disable once AssignNullToNotNullAttribute
            IntPtr windowHandle = new WindowInteropHelper(Window.GetWindow(element)).EnsureHandle();

            using (Graphics graphics = Graphics.FromHwnd(windowHandle))
                return new Point(point.X * logicalUnitDpi / graphics.DpiX, point.Y * logicalUnitDpi / graphics.DpiY);
        }

        // ReSharper disable once UnusedMember.Local
        static Point GetCurrentUIElementPointRelativeToRoot(UIElement element)
        {
            return element.TransformToAncestor(GetRootVisualForAnimation(element)).Transform(new Point(0, 0));
        }

        static Rectangle GetUIElementRect(UIElement element)
        {
            Rect rect = element.RenderTransform.TransformBounds(new Rect(GetCurrentUIElementPoint(element), element.RenderSize));

            return Rectangle.FromLTRB(
                (int)rect.Left,
                (int)rect.Top,
                (int)rect.Right,
                (int)rect.Bottom);
        }

        static Rectangle GetCurrentScreenBounds(DependencyObject element) =>
            new Screen(Window.GetWindow(element)).Bounds.ToRectangleInLogicalUnits(element);

        static Rectangle GetWorkAreaWithTabTipOpened(DependencyObject element)
        {
            Rectangle workAreaWithTabTipClosed = GetWorkAreaWithTabTipClosed(element);

            int tabTipRectangleTop = TabTip.GetWouldBeTabTipRectangle().ToRectangleInLogicalUnits(element).Top;

            int bottom = (tabTipRectangleTop == 0) ? workAreaWithTabTipClosed.Bottom / 2 : tabTipRectangleTop; // in case TabTip is not yet opened

            return Rectangle.FromLTRB(
                workAreaWithTabTipClosed.Left,
                workAreaWithTabTipClosed.Top,
                workAreaWithTabTipClosed.Right,
                bottom);
        }

        static Rectangle GetWorkAreaWithTabTipClosed(DependencyObject element)
        {
            Rectangle currentScreenBounds = GetCurrentScreenBounds(element);
            Taskbar taskbar = new Taskbar();
            Rectangle taskbarBounds = taskbar.Bounds.ToRectangleInLogicalUnits(element);

            switch (taskbar.Position)
            {
                case TaskbarPosition.Bottom:
                    return Rectangle.FromLTRB(
                        currentScreenBounds.Left,
                        currentScreenBounds.Top,
                        currentScreenBounds.Right,
                        taskbarBounds.Top);
                case TaskbarPosition.Top:
                    return Rectangle.FromLTRB(
                        currentScreenBounds.Left,
                        taskbarBounds.Bottom,
                        currentScreenBounds.Right,
                        currentScreenBounds.Bottom);
                default:
                    return currentScreenBounds;
            }
        }

        // ReSharper disable once UnusedMember.Local
        static bool IsUIElementInWorkAreaWithTabTipOpened(UIElement element)
        {
            return GetWorkAreaWithTabTipOpened(element).Contains(GetUIElementRect(element));
        }

        // ReSharper disable once UnusedMember.Local
        static bool IsUIElementInWorkArea(UIElement element, Rectangle workAreaRectangle)
        {
            return workAreaRectangle.Contains(GetUIElementRect(element));
        }

        static FrameworkElement GetRootVisualForAnimation(DependencyObject element)
        {
            Window rootWindow = Window.GetWindow(element);

            if (rootWindow?.WindowState != WindowState.Maximized)
                return rootWindow;
            return VisualTreeHelper.GetChild(rootWindow, 0) as FrameworkElement;
        }

        static double GetYOffsetToMoveUIElementInToWorkArea(Rectangle uiElementRectangle, Rectangle workAreaRectangle)
        {
            const double noOffset = 0;
            const int paddingTop = 30;
            const int paddingBottom = 10;

            if (uiElementRectangle.Top >= workAreaRectangle.Top &&
                uiElementRectangle.Bottom <= workAreaRectangle.Bottom)                             // UIElement is in work area
                return noOffset;

            if (uiElementRectangle.Top < workAreaRectangle.Top)                                    // Top of UIElement higher than work area
                return workAreaRectangle.Top - uiElementRectangle.Top + paddingTop;                // positive value to move down
            // Botom of UIElement lower than work area
            int offset = workAreaRectangle.Bottom - uiElementRectangle.Bottom - paddingBottom; // negative value to move up
            if (uiElementRectangle.Top > (workAreaRectangle.Top - offset))                     // will Top of UIElement be in work area if offset applied?
                return offset;                                                                 // negative value to move up
            return workAreaRectangle.Top - uiElementRectangle.Top + paddingTop;                // negative value to move up, but only to the point, where top of UIElement is just below top bound of work area
        }

        static Storyboard GetOrCreateMoveRootVisualStoryboard(FrameworkElement visualRoot)
        {
            if (MoveRootVisualStoryboards.ContainsKey(visualRoot))
                return MoveRootVisualStoryboards[visualRoot];
            return CreateMoveRootVisualStoryboard(visualRoot);
        }

        static Storyboard CreateMoveRootVisualStoryboard(FrameworkElement visualRoot)
        {
            Storyboard moveRootVisualStoryboard = new Storyboard
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.35))
            };

            DoubleAnimation moveAnimation = new DoubleAnimation
            {
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut },
                Duration = new Duration(TimeSpan.FromSeconds(0.35)),
                FillBehavior = (visualRoot is Window) ? FillBehavior.Stop : FillBehavior.HoldEnd
            };

            moveRootVisualStoryboard.Children.Add(moveAnimation);

            if (!(visualRoot is Window))
                visualRoot.RenderTransform = new TranslateTransform();

            Storyboard.SetTarget(moveAnimation, visualRoot);
            Storyboard.SetTargetProperty(
                moveAnimation,
                (visualRoot is Window) ? new PropertyPath("Top") : new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            MoveRootVisualStoryboards.Add(visualRoot, moveRootVisualStoryboard);
            SubscribeToWindowStateChangedToMoveRootVisual(visualRoot);

            return moveRootVisualStoryboard;
        }

        static void SubscribeToWindowStateChangedToMoveRootVisual(FrameworkElement visualRoot)
        {
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            if (visualRoot is Window)
            {
                Window window = (Window)visualRoot;

                window.StateChanged += (sender, args) =>
                {
                    if (window.WindowState == WindowState.Normal)
                        MoveRootVisualBy(
                            window,
                            GetYOffsetToMoveUIElementInToWorkArea(
                                GetWindowRectangle(window),
                                GetWorkAreaWithTabTipClosed(window)));
                };
            }
            else
            {
                Window window = Window.GetWindow(visualRoot);
                if (window != null)
                    window.StateChanged += (sender, args) =>
                    {
                        if (window.WindowState == WindowState.Normal)
                            MoveRootVisualTo(visualRoot, 0);
                    };
            }
        }

        static void MoveRootVisualBy(FrameworkElement rootVisual, double moveBy)
        {
            if (moveBy == 0)
                return;

            Storyboard moveRootVisualStoryboard = GetOrCreateMoveRootVisualStoryboard(rootVisual);

            if (moveRootVisualStoryboard.Children.First() is DoubleAnimation doubleAnimation)
            {
                if (rootVisual is Window window)
                {
                    doubleAnimation.From = window.Top;
                    doubleAnimation.To = window.Top + moveBy;
                }
                else
                {
                    doubleAnimation.From = doubleAnimation.To ?? 0;
                    doubleAnimation.To = (doubleAnimation.To ?? 0) + moveBy;
                }
            }

            moveRootVisualStoryboard.Begin();
        }

        static void MoveRootVisualTo(FrameworkElement rootVisual, double moveTo)
        {
            Storyboard moveRootVisualStoryboard = GetOrCreateMoveRootVisualStoryboard(rootVisual);

            if (moveRootVisualStoryboard.Children.First() is DoubleAnimation doubleAnimation)
            {
                if (rootVisual is Window window)
                {
                    doubleAnimation.From = window.Top;
                    doubleAnimation.To = moveTo;
                }
                else
                {
                    doubleAnimation.From = doubleAnimation.To ?? 0;
                    doubleAnimation.To = moveTo;
                }
            }

            moveRootVisualStoryboard.Begin();
        }

        internal static void GetUIElementInToWorkAreaWithTabTipOpened(UIElement element)
        {
            try
            {
                FrameworkElement rootVisualForAnimation = GetRootVisualForAnimation(element);
                Rectangle workAreaWithTabTipOpened = GetWorkAreaWithTabTipOpened(element);

                Rectangle uiElementRectangle;
                if (rootVisualForAnimation is Window window && workAreaWithTabTipOpened.Height >= window.Height)
                    uiElementRectangle = GetWindowRectangle(window);
                else
                    uiElementRectangle = GetUIElementRect(element);

                MoveRootVisualBy(
                    rootVisualForAnimation,
                    GetYOffsetToMoveUIElementInToWorkArea(
                        uiElementRectangle,
                        workAreaWithTabTipOpened));
            }
            catch (Exception ex)
            {
                ExceptionCatched?.Invoke(ex);
            }
        }

        static Rectangle GetWindowRectangle(Window window)
        {
            return Rectangle.FromLTRB(
                (int)window.Left,
                (int)window.Top,
                (int)(window.Left + window.Width),
                (int)(window.Top + window.Height));
        }

        internal static void GetEverythingInToWorkAreaWithTabTipClosed()
        {
            try
            {
                foreach (KeyValuePair<FrameworkElement, Storyboard> moveRootVisualStoryboard in MoveRootVisualStoryboards)
                {
                    // if window exist also check if it has not been closed
                    if (moveRootVisualStoryboard.Key is Window window && new WindowInteropHelper(window).Handle != IntPtr.Zero)
                        MoveRootVisualBy(
                            window,
                            GetYOffsetToMoveUIElementInToWorkArea(
                                GetWindowRectangle(window),
                                GetWorkAreaWithTabTipClosed(window)));
                    else
                        MoveRootVisualTo(moveRootVisualStoryboard.Key, 0);
                }
            }
            catch (Exception ex)
            {
                ExceptionCatched?.Invoke(ex);
            }
        }
    }
}
