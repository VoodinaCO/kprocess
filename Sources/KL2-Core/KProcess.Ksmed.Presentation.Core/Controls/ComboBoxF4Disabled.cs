using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Représente une ComboBox qui n'ouvre pas le popup lors de l'appui sur le bouton F4.
    /// </summary>
    public class ComboBoxF4Disabled : ComboBox
    {
        /// <summary>
        /// Initialise la classe <see cref="ComboBoxF4Disabled"/>.
        /// </summary>
        static ComboBoxF4Disabled()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBoxF4Disabled), new FrameworkPropertyMetadata(typeof(ComboBox)));
        }

        /// <summary>
        /// Invoked when a <see cref="E:System.Windows.Input.Keyboard.PreviewKeyDown"/> attached routed event occurs.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.F4)
                base.OnPreviewKeyDown(e);
        }

        /// <summary>
        /// Invoked when a <see cref="E:System.Windows.Input.Keyboard.KeyDown"/> attached routed event occurs.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.F4)
                base.OnKeyDown(e);
        }

    }
}
