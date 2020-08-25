using KProcess.Presentation.Windows;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Gère la notification concernant le fait que le textbox associé a été modifié sans que le DataContext associé ait été synchronisé
    /// </summary>
    public class HandleNotSynchronizedTextBoxInputBehavior : Behavior<TextBox>
    {
        private bool _isFirstTextChangeNotified;

        public object ResetObject
        {
            get { return (object)GetValue(ResetObjectProperty); }
            set { SetValue(ResetObjectProperty, value); }
        }

        public static readonly DependencyProperty ResetObjectProperty = DependencyProperty.Register("ResetObject", typeof(object), 
            typeof(HandleNotSynchronizedTextBoxInputBehavior), new PropertyMetadata(null, OnResetObjectPropertyChanged));

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.GotFocus += AssociatedObjectGotFocus;
            this.AssociatedObject.LostFocus += AssociatedObjectLostFocus;
        }

        private static void OnResetObjectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = (HandleNotSynchronizedTextBoxInputBehavior)d;
            if (e.OldValue != e.NewValue && e.NewValue != null)
            {
                sender.Dispatcher.BeginInvoke(new Action(sender.ResetTracking));
            }
        }

        private void AssociatedObjectLostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            ResetTracking();

            IoC.Resolve<IEventBus>().Unsubscribe<UIBindingSynchronizationRequested>(OnBindingRefreshRequested);
            this.AssociatedObject.TextChanged -= AssociatedObjectTextChanged;
        }

        private void AssociatedObjectGotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            ResetTracking();

            this.AssociatedObject.TextChanged -= AssociatedObjectTextChanged;
            this.AssociatedObject.TextChanged += AssociatedObjectTextChanged;
            IoC.Resolve<IEventBus>().Subscribe<UIBindingSynchronizationRequested>(OnBindingRefreshRequested);
        }

        private void OnBindingRefreshRequested(UIBindingSynchronizationRequested obj)
        {
            try
            {
                var bindingExpression = BindingOperations.GetBindingExpression(this.AssociatedObject, TextBox.TextProperty);
                bindingExpression.UpdateSource();
            }
            catch (InvalidOperationException) { }
        }

        private void ResetTracking()
        {
            _isFirstTextChangeNotified = false;
        }

        private void AssociatedObjectTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Trace.WriteLine(_isFirstTextChangeNotified);

            if (!_isFirstTextChangeNotified)
            {
                IoC.Resolve<IEventBus>().Publish(new UIUpdatedNotSynchronizedEvent(this, this.AssociatedObject));
                _isFirstTextChangeNotified = true;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.LostFocus -= AssociatedObjectLostFocus;
            this.AssociatedObject.GotFocus -= AssociatedObjectGotFocus;
            this.AssociatedObject.TextChanged -= AssociatedObjectTextChanged;
            IoC.Resolve<IEventBus>().Unsubscribe<UIBindingSynchronizationRequested>(OnBindingRefreshRequested);
        }
    }
}
