using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Permet l'activation des <see cref="EnterKeyBinding"/>.
    /// </summary>
    public static class ControlEnterBindingUpdate
    {

        /// <summary>
        /// Active la gsetion des <see cref="EnterKeyBinding"/> pour les textboxes.
        /// </summary>
        public static void ActivateTextBox()
        {
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.KeyDownEvent, (KeyEventHandler)OnTextBoxKeyDown);
        }

        /// <summary>
        /// Appelé lorsqu'une touche a été appuyée sur une TextBox.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KeyEventArgs"/> contenant les données de l'évènement.</param>
        static void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            var tb = (TextBox)sender;

            if (!tb.AcceptsReturn && e.Key == Key.Enter)
            {
                var binding = GetBinding(tb, TextBox.TextProperty, BindingMode.TwoWay);
                if (binding != null)
                    binding.UpdateSource();
            }
        }

        /// <summary>
        /// Obtient le binding appliqué ou null si non trouvé ou invalide.
        /// </summary>
        /// <param name="source">La source.</param>
        /// <param name="dp">La propriété de dépendance.</param>
        /// <returns>Le binding</returns>
        static BindingExpression GetBinding(DependencyObject source, DependencyProperty dp, BindingMode defaultbindingMode)
        {
            var bindingExp = BindingOperations.GetBindingExpression(source, TextBox.TextProperty);
            if (bindingExp != null)
            {
                if (bindingExp.ParentBinding is EnterKeyBinding binding
                    && (binding.Mode == BindingMode.TwoWay
                        || binding.Mode == BindingMode.OneWayToSource
                        || (binding.Mode == BindingMode.Default
                            && defaultbindingMode == BindingMode.TwoWay
                            || defaultbindingMode == BindingMode.OneWayToSource
                        )
                    )
                )
                    return bindingExp;
            }

            return null;
        }

    }
}
