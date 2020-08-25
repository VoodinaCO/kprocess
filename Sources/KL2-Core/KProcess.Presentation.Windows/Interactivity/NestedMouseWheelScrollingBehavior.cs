using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Permet à un ScrollViewer de laisser ses ScrollViewers parents de scroller si celui-ci ne peut plus le faire.
    /// </summary>
    [System.ComponentModel.Description("Permet à un ScrollViewer de laisser ses ScrollViewers parents de scroller si celui-ci ne peut plus le faire.")]
    public class NestedMouseWheelScrollingBehavior : Behavior<ScrollViewer>
    {
        /// <summary>
        /// Obtient une valeur indiquant si une instance du Behavior doit être attachée.
        /// </summary>
        /// <param name="obj">La cible.</param>
        /// <returns><c>true</c> si une instance du Behavior doit être attachée.</returns>
        public static bool GetAttach(DependencyObject obj)
        {
            return (bool)obj.GetValue(AttachProperty);
        }

        /// <summary>
        /// Définit une valeur indiquant si une instance du Behavior doit être attachée.
        /// </summary>
        /// <param name="obj">La cible.</param>
        /// <param name="value"><c>true</c> si une instance du Behavior doit être attachée.</param>
        public static void SetAttach(DependencyObject obj, bool value)
        {
            obj.SetValue(AttachProperty, value);
        }

        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Attach"/>.
        /// </summary>
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(NestedMouseWheelScrollingBehavior),
            new PropertyMetadata(false, OnAttachChanged));

        /// <summary>
        /// Appelé lorsque la valeur de la propriété de dépendance <see cref="Attach"/> a changé.
        /// </summary>
        /// <param name="d">L'instance où la propriété a changé.</param>
        /// <param name="e">Les arguments de l'évènement <see cref="System.Windows.DependencyPropertyChangedEventArgs"/>.</param>
        private static void OnAttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                Interaction.GetBehaviors(d).Add(new NestedMouseWheelScrollingBehavior());
            else
                return;
        }


        /// <summary>
        /// Appelé une fois que le comportement est attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            base.AssociatedObject.PreviewMouseWheel += new MouseWheelEventHandler(OnPreviewMouseWheel);
        }

        /// <summary>
        /// Appelé lorsque le comportement est détaché de son AssociatedObject, mais avant qu'il ne se soit produit réellement.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            base.AssociatedObject.PreviewMouseWheel -= new MouseWheelEventHandler(OnPreviewMouseWheel);
        }

        private static List<MouseWheelEventArgs> _reentrantList = new List<MouseWheelEventArgs>();
        /// <summary>
        /// Appelé lorsque la molette de la souris a été roulée.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.Input.MouseWheelEventArgs"/> contenant les données de l'évènement.</param>
        private void OnPreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollControl = sender as ScrollViewer;
            if (!e.Handled && sender != null && !_reentrantList.Contains(e))
            {
                // Quand l'évènement arrive, le re-lever si l'élément associé afin que le scroll viewer associé le traite
                var previewEventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = UIElement.PreviewMouseWheelEvent,
                    Source = sender
                };
                var originalSource = e.OriginalSource as UIElement;
                if (originalSource != null)
                {
                    _reentrantList.Add(previewEventArg);
                    originalSource.RaiseEvent(previewEventArg);
                    _reentrantList.Remove(previewEventArg);

                    // Si le ScrollViewer ne s'en est pas chargé, on peut le lever pour les ancêtres
                    if (!previewEventArg.Handled && ((e.Delta > 0 && scrollControl.VerticalOffset == 0)
                        || (e.Delta <= 0 && scrollControl.VerticalOffset >= scrollControl.ExtentHeight - scrollControl.ViewportHeight)))
                    {
                        e.Handled = true;
                        var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                        eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                        eventArg.Source = sender;
                        var parent = (UIElement)((FrameworkElement)sender).Parent;
                        if (parent != null)
                            parent.RaiseEvent(eventArg);
                    }
                }
            }
        }


    }
}
