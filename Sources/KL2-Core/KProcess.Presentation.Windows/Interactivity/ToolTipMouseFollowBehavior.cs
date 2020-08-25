using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Active le suivi automatique du pointeur de la souris pour le tooltip de l'élément associé.
    /// </summary>
    /// <remarks>Ajoutez un contrôle ToolTip dans la propriété ToolTip pour que cela fonctionne.</remarks>
    public class ToolTipMouseFollowBehavior : Behavior<FrameworkElement>
    {

        /// <summary>
        /// Gets or sets the VerticalOffset.
        /// </summary>
        public double VerticalOffset
        {
            get { return (double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="VerticalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register("VerticalOffset", typeof(double), typeof(ToolTipMouseFollowBehavior), new UIPropertyMetadata(0));

        /// <summary>
        /// Gets or sets the HorizontalOffset.
        /// </summary>
        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="HorizontalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(ToolTipMouseFollowBehavior), new UIPropertyMetadata(10));

        /// <summary>
        /// Appelé une fois que le comportement est attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            base.AssociatedObject.MouseMove += new System.Windows.Input.MouseEventHandler(AssociatedObject_MouseMove);
        }

        /// <summary>
        /// Appelé lorsque le comportement est détaché de son AssociatedObject, mais avant qu'il ne se soit produit réellement.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            base.AssociatedObject.MouseMove -= new System.Windows.Input.MouseEventHandler(AssociatedObject_MouseMove);
        }

        private void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var tooltip = base.AssociatedObject.ToolTip as ToolTip;
            if (tooltip != null && tooltip.IsOpen)
            {
                tooltip.Placement = System.Windows.Controls.Primitives.PlacementMode.Relative;
                tooltip.HorizontalOffset = e.GetPosition((IInputElement)sender).X + VerticalOffset;
                tooltip.VerticalOffset = e.GetPosition((IInputElement)sender).Y + HorizontalOffset;
            }
        }
    }
}
