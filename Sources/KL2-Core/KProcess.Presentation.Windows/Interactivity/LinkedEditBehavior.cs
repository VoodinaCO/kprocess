// -----------------------------------------------------------------------
// <copyright file="LinkedEditBehavior.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace KProcess.Presentation.Windows
{
#if !SILVERLIGHT
    /// <summary>
    /// Permet de naviguer entre les différents champs d'édition lorsque la touche entrée est pressée
    /// </summary>
    [System.ComponentModel.Description("Permet de naviguer entre les différents champs d'édition lorsque la touche entrée est pressée")]
    public class LinkedEditBehavior : Behavior<UIElement>
    {
        #region Surcharges

        /// <summary>
        /// Appelé après que le Behavior ait été attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            // Abonnements
#if SILVERLIGHT
            if (AssociatedObject is System.Windows.Controls.TextBox)
#else
            if (AssociatedObject is TextBoxBase)
#endif
                AssociatedObject.GotFocus += new System.Windows.RoutedEventHandler(AssociatedObject_GotFocus);

            AssociatedObject.KeyDown += new System.Windows.Input.KeyEventHandler(AssociatedObject_KeyDown);
        }

        /// <summary>
        /// Appelé lorsque le Behavior est en train d'être détaché de l'AssociatedObject.
        /// </summary>
        protected override void OnDetaching()
        {
            // Désabonnements
#if SILVERLIGHT
            if (AssociatedObject is System.Windows.Controls.TextBox)
#else
            if (AssociatedObject is TextBoxBase)
#endif
                AssociatedObject.KeyDown -= new System.Windows.Input.KeyEventHandler(AssociatedObject_KeyDown);

            AssociatedObject.GotFocus -= new System.Windows.RoutedEventHandler(AssociatedObject_GotFocus);

            base.OnDetaching();
        }

        #endregion

        #region Gestion des événements

        /// <summary>
        /// Traite l'évènement KeyDown de l'élément associé.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.Input.KeyEventArgs"/> contenant les données de l'évènement.</param>
        void AssociatedObject_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Détecte la touche entrée (ou return)
#if SILVERLIGHT
            if (e.Key == System.Windows.Input.Key.Enter)
#else
            if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Return)
#endif
            {
                UIElement element = null;

                // Suivant que shift est enfoncé, cela influe sur le sens de navigation
                if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
                    // On cherche l'élément précédant le courant
                    element = VisualTreeHelperExtensions.TryFindPrevious<UIElement>(AssociatedObject, tb => Interaction.GetBehaviors(tb).Any(b => b is LinkedEditBehavior));
                else
                    // On cherche l'élément suivant le courant
                    element = VisualTreeHelperExtensions.TryFindNext<UIElement>(AssociatedObject, tb => Interaction.GetBehaviors(tb).Any(b => b is LinkedEditBehavior));

                // Si un élément est trouvé, on tente de lui donner le focus
                if (!element.Focus())
                {
                    // S'il n'est pas focusable, tente de le donner à son premier enfant qui l'est
                    element = element.GetFirstFocusableChild();
                    if (element != null)
                        element.Focus();
                }
            }
        }

        /// <summary>
        /// Traite l'évènement GotFocus de l'élément associé.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        void AssociatedObject_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBoxBase textBox = (TextBoxBase)AssociatedObject;

            // Sélectionne tout le textbox lorsqu'il a le focus
            Dispatcher.BeginInvoke
                            (
                                DispatcherPriority.ContextIdle,
                                new Action
                                (
                                    () =>
                                    {
                                        textBox.SelectAll();
                                        textBox.ReleaseMouseCapture();
                                    }
                                )
                            );

        }

        #endregion
    }
#endif
}
