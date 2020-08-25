using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Permet l'activation du spell checking
    /// </summary>
    public static class ControlSpellChecking
    {
        /// <summary>
        /// Active le spell checking.
        /// </summary>
        public static void ActivateInputElement()
        {
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewTextInputEvent, (RoutedEventHandler)OnEvent);
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotFocusEvent, (RoutedEventHandler)OnEvent);
            CommandManager.RegisterClassCommandBinding(typeof(TextBox), new CommandBinding(ApplicationCommands.Paste, OnEvent));
        }

        static void OnEvent(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            SpellCheck.SetIsEnabled(tb, SpellCheckConfig.GetEnabled(tb));
        }

    }

    public static class SpellCheckConfig
    {
        public static void SetEnabled(UIElement element, bool value) =>
            element.SetValue(EnabledProperty, value);

        public static bool GetEnabled(UIElement element) =>
            (bool)element.GetValue(EnabledProperty);

        public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(SpellCheckConfig), new PropertyMetadata(true));
    }
}
