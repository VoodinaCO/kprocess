using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Kprocess.TabTip
{
    public static class TabTipAutomation
    {
        static TabTipAutomation()
        {
            if (EnvironmentEx.GetOSVersion() == OSVersion.Win7)
                return;

            TabTip.Closed += () => TabTipClosedSubject.OnNext(true);

            AutomateTabTipOpen(FocusSubject.AsObservable());
            AutomateTabTipClose(FocusSubject.AsObservable(), TabTipClosedSubject);

            AnimationHelper.ExceptionCatched += exception => ExceptionCatched?.Invoke(exception);
        }

        static readonly Subject<(UIElement uiElement, bool hasFocus)> FocusSubject = new Subject<(UIElement uiElement, bool hasFocus)>();
        static readonly Subject<bool> TabTipClosedSubject = new Subject<bool>();

        static readonly List<Type> BindedUIElements = new List<Type>();

        /// <summary>
        /// By default TabTip automation happens only when no keyboard is connected to device.
        /// Change IgnoreHardwareKeyboard if you want to automate
        /// TabTip even if keyboard is connected.
        /// </summary>
        public static HardwareKeyboardIgnoreOptions IgnoreHardwareKeyboard
        {
            get { return HardwareKeyboard.IgnoreOptions; }
            set { HardwareKeyboard.IgnoreOptions = value; }
        }

        /// <summary>
        /// Subscribe to this event if you want to know about exceptions (errors) in this library
        /// </summary>
        public static event Action<Exception> ExceptionCatched;

        /// <summary>
        /// Description of keyboards to ignore if there is only one instance of given keyboard.
        /// If you want to ignore some ghost keyboard, add it's description to this list
        /// </summary>
        public static List<string> ListOfHardwareKeyboardsToIgnoreIfSingleInstance => HardwareKeyboard.ListOfKeyboardsToIgnore;

        /// <summary>
        /// Description of keyboards to ignore.
        /// If you want to ignore some ghost keyboard, add it's description to this list
        /// </summary>
        public static List<string> ListOfKeyboardsToIgnore => HardwareKeyboard.ListOfKeyboardsToIgnore;

        static void AutomateTabTipClose(IObservable<(UIElement uiElement, bool hasFocus)> focusObservable, Subject<bool> tabTipClosedSubject)
        {
            focusObservable
                .ObserveOn(Scheduler.Default)
                .Where(_ => IgnoreHardwareKeyboard == HardwareKeyboardIgnoreOptions.IgnoreAll || !HardwareKeyboard.IsConnectedAsync().Result)
                .Throttle(TimeSpan.FromMilliseconds(100)) // Close only if no other UIElement got focus in 100 ms
                .Where(tuple => tuple.hasFocus == false)
                .Do(_ => TabTip.Close())
                .Subscribe(_ => tabTipClosedSubject.OnNext(true));

            tabTipClosedSubject
                .ObserveOnDispatcher()
                .Subscribe(_ => AnimationHelper.GetEverythingInToWorkAreaWithTabTipClosed());
        }

        static void AutomateTabTipOpen(IObservable<(UIElement uiElement, bool hasFocus)> focusObservable)
        {
            focusObservable
                .ObserveOn(Scheduler.Default)
                .Where(_ => IgnoreHardwareKeyboard == HardwareKeyboardIgnoreOptions.IgnoreAll || !HardwareKeyboard.IsConnectedAsync().Result)
                .Where(tuple => tuple.hasFocus == true)
                .Do(_ => TabTip.OpenUndockedAndStartPoolingForClosedEvent())
                .ObserveOnDispatcher()
                .Subscribe(tuple => AnimationHelper.GetUIElementInToWorkAreaWithTabTipOpened(tuple.uiElement));
        }

        /// <summary>
        /// Automate TabTip for given UIElement.
        /// Keyboard opens on GotFocusEvent or TouchDownEvent (if focused already) 
        /// and closes on LostFocusEvent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void BindTo<T>() where T : UIElement
        {
            if (EnvironmentEx.GetOSVersion() == OSVersion.Win7)
                return;

            if (BindedUIElements.Contains(typeof(T)))
                return;

            EventManager.RegisterClassHandler(
                typeof(T),
                UIElement.TouchDownEvent,
                new RoutedEventHandler(ManageGotFocus),
                true);

            EventManager.RegisterClassHandler(
                typeof(T),
                UIElement.GotFocusEvent,
                new RoutedEventHandler(ManageGotFocus),
                true);

            EventManager.RegisterClassHandler(
                typeof(T),
                UIElement.LostFocusEvent,
                new RoutedEventHandler((s, e) =>
                {
                    if (s is UIElement uiElement)
                    {
                        FocusSubject.OnNext((uiElement, false));
                    }
                }),
                true);

            BindedUIElements.Add(typeof(T));
        }

        static void ManageGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is UIElement uiElement)
            {
                if (uiElement is TextBoxBase textBoxBase)
                {
                    if (!textBoxBase.IsReadOnly)
                        FocusSubject.OnNext((uiElement, true));
                }
                else
                    FocusSubject.OnNext((uiElement, true));
            }
        }
    }
}
