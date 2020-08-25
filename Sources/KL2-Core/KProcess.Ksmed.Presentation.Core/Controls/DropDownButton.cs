using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    [TemplatePart(Name = PART_DropDownButton, Type = typeof(ToggleButton))]
    [TemplatePart(Name = PART_ContentPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    public class DropDownButton : ContentControl
    {
        private const string PART_DropDownButton = "PART_DropDownButton";
        private const string PART_ContentPresenter = "PART_ContentPresenter";
        private const string PART_Popup = "PART_Popup";

        #region Membres privés

        private ContentPresenter _contentPresenter;
        private Popup _popup;

        #endregion

        #region Constructeurs

        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));
        }

        public DropDownButton()
        {
            Keyboard.AddKeyDownHandler(this, OnKeyDown);
            EventManager.RegisterClassHandler(typeof(DropDownButton), Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture));
            EventManager.RegisterClassHandler(typeof(DropDownButton), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButtonDown), true);
        }

        #endregion //Constructors

        #region Propriétés

        private System.Windows.Controls.Primitives.ButtonBase _button;
        protected System.Windows.Controls.Primitives.ButtonBase Button
        {
            get { return _button; }
            set
            {
                if (_button != null)
                    _button.Click -= DropDownButton_Click;

                _button = value;

                if (_button != null)
                    _button.Click += DropDownButton_Click;
            }
        }

        #region DropDownContent

        /// <summary>
        /// Obtient ou définit le contenu du DropDown.
        /// </summary>
        public object DropDownContent
        {
            get { return (object)GetValue(DropDownContentProperty); }
            set { SetValue(DropDownContentProperty, value); }
        }
        public static readonly DependencyProperty DropDownContentProperty = DependencyProperty.Register("DropDownContent", typeof(object), typeof(DropDownButton), new UIPropertyMetadata(null, OnDropDownContentChanged));

        private static void OnDropDownContentChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DropDownButton dropDownButton = o as DropDownButton;
            if (dropDownButton != null)
                dropDownButton.OnDropDownContentChanged((object)e.OldValue, (object)e.NewValue);
        }

        protected virtual void OnDropDownContentChanged(object oldValue, object newValue)
        {
        }

        #endregion

        #region IsOpen

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(DropDownButton), new UIPropertyMetadata(false, OnIsOpenChanged));

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le DropDown est ouvert.
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        private static void OnIsOpenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            DropDownButton dropDownButton = o as DropDownButton;
            if (dropDownButton != null)
                dropDownButton.OnIsOpenChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnIsOpenChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                _hasMouseEnteredPopup = false;
                Mouse.Capture(this, CaptureMode.SubTree);
                RaiseRoutedEvent(DropDownButton.OpenedEvent);
            }
            else
            {
                base.ReleaseMouseCapture();
                RaiseRoutedEvent(DropDownButton.ClosedEvent);
            }
        }

        #endregion

        #endregion

        #region Surcharges

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button = GetTemplateChild(PART_DropDownButton) as ToggleButton;

            _contentPresenter = GetTemplateChild(PART_ContentPresenter) as ContentPresenter;

            if (_popup != null)
                _popup.Opened -= Popup_Opened;

            _popup = GetTemplateChild(PART_Popup) as Popup;

            if (_popup != null)
                _popup.Opened += Popup_Opened;
        }

        #endregion

        #region Evènements

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DropDownButton));
        public event RoutedEventHandler Click
        {
            add
            {
                AddHandler(ClickEvent, value);
            }
            remove
            {
                RemoveHandler(ClickEvent, value);
            }
        }

        public static readonly RoutedEvent OpenedEvent = EventManager.RegisterRoutedEvent("Opened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DropDownButton));
        public event RoutedEventHandler Opened
        {
            add
            {
                AddHandler(OpenedEvent, value);
            }
            remove
            {
                RemoveHandler(OpenedEvent, value);
            }
        }

        public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DropDownButton));
        public event RoutedEventHandler Closed
        {
            add
            {
                AddHandler(ClosedEvent, value);
            }
            remove
            {
                RemoveHandler(ClosedEvent, value);
            }
        }

        #endregion //Events

        #region Event Handlers

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            bool hasModyingKey = (((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt) && ((e.SystemKey == Key.Down) || (e.SystemKey == Key.Up)));

            if (!IsOpen)
            {
                if (hasModyingKey)
                {
                    IsOpen = true;
                    // ContentPresenter items will get focus in Popup_Opened().
                    e.Handled = true;
                }
            }
            else
            {
                if (hasModyingKey)
                {
                    CloseDropDown(true);
                    e.Handled = true;
                }
                else if (e.Key == Key.Escape)
                {
                    CloseDropDown(true);
                    e.Handled = true;
                }
            }
        }

        private bool _hasMouseEnteredPopup = false;

        private bool _isMouseOverPopup = false;

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (_hasMouseEnteredPopup && !_isMouseOverPopup && this.IsOpen)
            {
                this.CloseDropDown(true);
                e.Handled = true;
            }
            base.OnMouseLeftButtonUp(e);
        }

        private static void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            var dd = (DropDownButton)sender;

            if (Mouse.Captured != dd)
            {
                if (e.OriginalSource == dd)
                {
                    if (Mouse.Captured == null)
                    {
                        dd.CloseDropDown(false);
                        return;
                    }
                }
                else
                {
                    dd.CloseDropDown(false);
                }
            }

            e.Handled = true;
        }

        private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            var ddb = (DropDownButton)sender;
            if (!ddb.IsOpen && !ddb.IsKeyboardFocusWithin)
            {
                ddb.Focus();
            }
            e.Handled = true;
            if (Mouse.Captured == ddb && e.OriginalSource == ddb)
            {
                ddb.CloseDropDown(false);
            }
        }

        private void DropDownButton_Click(object sender, RoutedEventArgs e)
        {
            OnClick();
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            // Set the focus on the content of the ContentPresenter.
            if (_contentPresenter != null)
            {
                _contentPresenter.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
        }

        #endregion

        #region Méthodes

        private void CloseDropDown(bool isFocusOnButton)
        {
            if (IsOpen)
                IsOpen = false;

            if (isFocusOnButton)
                Button.Focus();
        }

        protected virtual void OnClick()
        {
            RaiseRoutedEvent(DropDownButton.ClickEvent);
        }

        private void RaiseRoutedEvent(RoutedEvent routedEvent)
        {
            RoutedEventArgs args = new RoutedEventArgs(routedEvent, this);
            RaiseEvent(args);
        }

        #endregion
    }
}
