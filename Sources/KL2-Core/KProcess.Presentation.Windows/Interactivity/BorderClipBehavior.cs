using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace KProcess.Presentation.Windows
{

    /// <summary>
    /// Réduit (Clip) le contenu du Border associée en fonction de CornerRadius.
    /// </summary>
    [System.ComponentModel.Description("Réduit (Clip) le contenu du Border associée en fonction de CornerRadius")]
    public class BorderClipBehavior : Behavior<Border>
    {
        private RectangleGeometry _clipRect = new RectangleGeometry();
        private object _oldClip;
        private object _oldChild;

        /// <summary>
        /// Appelé après que le Behavior ait été attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            base.AssociatedObject.LayoutUpdated += new EventHandler(OnAssociatedObjectSizeChanged);
        }

        /// <summary>
        /// Gère l'évènement LayoutUpdated de l'AssociatedObject.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les données de l'évènement.</param>
        private void OnAssociatedObjectSizeChanged(object sender, EventArgs e)
        {
            if (base.AssociatedObject.CornerRadius != null)
            {

                // Applique le Clip au nouveau contenu.
                if (base.AssociatedObject.Child != null)
                {
                    _clipRect.RadiusX = _clipRect.RadiusY = Math.Max(0.0, base.AssociatedObject.CornerRadius.TopLeft - (base.AssociatedObject.BorderThickness.Left * 0.5));
                    _clipRect.Rect = new Rect(base.AssociatedObject.Child.RenderSize);
                    base.AssociatedObject.Child.Clip = _clipRect;
                }

                // Retire le Clip de l'ancien contenu.
                if (base.AssociatedObject.Child != _oldChild)
                {

                    if (_oldChild != null)
                    {
                        base.AssociatedObject.Child.SetValue(UIElement.ClipProperty, _oldClip);
                    }

                    if (base.AssociatedObject.Child != null)
                    {
                        _oldClip = base.AssociatedObject.Child.ReadLocalValue(UIElement.ClipProperty);
                    }
                    else
                    {
                        _oldClip = null;
                    }

                    _oldChild = base.AssociatedObject.Child;
                }
            }
        }

        /// <summary>
        /// Appelé lorsque le Behavior est en train d'être détaché de l'AssociatedObject.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            base.AssociatedObject.LayoutUpdated -= new EventHandler(OnAssociatedObjectSizeChanged);
        }

    }
}
