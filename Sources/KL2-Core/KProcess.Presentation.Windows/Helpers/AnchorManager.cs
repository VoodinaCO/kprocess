// -----------------------------------------------------------------------
// <copyright file="AnchorManager.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace KProcess.Presentation.Windows
{
#if !SILVERLIGHT

    /// <summary>
    /// Gestionnaire d'ancre
    /// </summary>
    public static class AnchorManager
    {
        #region AnchorProperty

        /// <summary>
        /// Définit la dependency property correspondant à une ancre
        /// </summary>
        public static readonly DependencyProperty AnchorProperty = DependencyProperty.RegisterAttached("Anchor", typeof(object), typeof(AnchorManager), new PropertyMetadata(null));

        /// <summary>
        /// Obtient l'ancre ratachée à un dependency object
        /// </summary>
        /// <param name="d">dependency object pour lequel on cherche l'ancre ratachée</param>
        /// <returns>l'ancre ratachée au dependency object</returns>
        public static object GetAnchor(DependencyObject d)
        {
            return d.GetValue(AnchorProperty);
        }

        /// <summary>
        /// Définit l'ancre à ratacher à un dependency object
        /// </summary>
        /// <param name="d">dependency object auquel l'ancre est ratachée</param>
        /// <param name="o">ancre à ratacher</param>
        public static void SetAnchor(DependencyObject d, object o)
        {
            d.SetValue(AnchorProperty, o);
        }

        #endregion

        #region FocusedAnchorProperty

        /// <summary>
        /// Définit la dependency property correspondant à l'ancre sur laquelle doit être mis le focus
        /// </summary>
        public static readonly DependencyProperty FocusedAnchorProperty = DependencyProperty.RegisterAttached("FocusedAnchor", typeof(object), typeof(AnchorManager), new PropertyMetadata(null, OnFocusedAnchorChanged));

        /// <summary>
        /// Obtient l'ancre ayant le focus
        /// </summary>
        /// <param name="d">frameworkElement définissant le scope des ancres</param>
        /// <returns>ancre ayant le focus</returns>
        public static object GetFocusedAnchor(FrameworkElement d)
        {
            return d.GetValue(FocusedAnchorProperty);
        }

        /// <summary>
        /// Définit l'ancre ayant le focus
        /// </summary>
        /// <param name="d">frameworkElement définissant le scope des ancres</param>
        /// <param name="o">ancre</param>
        public static void SetFocusedAnchor(FrameworkElement d, object o)
        {
            // Si c'est la même ancre, on force le rappel, sans cela, l'élément risque de perdre le focus
            if (o.Equals(GetFocusedAnchor(d)))
                OnFocusedAnchorChanged(d, new DependencyPropertyChangedEventArgs(FocusedAnchorProperty, null, o));
            else
                d.SetValue(FocusedAnchorProperty, o);
        }

        private static void OnFocusedAnchorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                d.Dispatcher.BeginInvoke(new Action(() => SetFocus(d, e.NewValue)), System.Windows.Threading.DispatcherPriority.Render);
        }

        #endregion

        #region Méthodes privées

        private static void SetFocus(DependencyObject parent, object p)
        {
            DependencyObject element = null;
            var propertyName = p as string;

            // Cas où il s'agit d'une string d'un path complexe d'une grappe d'objet (ex: Model.Property1.SubProperty)
            if (propertyName != null && propertyName.Contains("."))
            {
                int i = 0;
                // Scinde la string pour retrouver tous les "sous path"
                string[] path = propertyName.Split('.');

                // Dans cette méthode anonyme, la variable i évolue dans l'itérateur plus bas
                Predicate<DependencyObject> predicate = d =>
                {
                    string anchor = GetAnchor(d) as string;

                    if (anchor == null)
                        return false;

                    return anchor == path[i];
                };

                // Tente de récupérer le premier élément correspondant à la première partie de la chaîne
                element = parent.FindFirstChild(predicate);

                // Parcourt tous les sous éléments
                for (i = 1; i < path.Length; i++)
                {
                    if (element == null)
                        break;

                    element = element.FindFirstChild(predicate);
                }
            }
            else
            {
                element = parent.FindFirstChild(d =>
                {
                    object anchor = GetAnchor(d);

                    if (anchor == null)
                        return false;

                    IComparable comparable = p as IComparable;
                    if (comparable != null)
                        return comparable.CompareTo(anchor) == 0;

                    return anchor == p;
                });
            }

            // Si l'élément correspondant a été trouvé
            if (element != null)
            {
                // Tente de le rendre visible à l'écran
                FrameworkElement frameworkElement = element as FrameworkElement;
                if (frameworkElement != null)
                    frameworkElement.BringIntoView();

                // Si c'est un textboxbase, le sélectionne
                System.Windows.Controls.Primitives.TextBoxBase textBoxBase = element as System.Windows.Controls.Primitives.TextBoxBase;
                if (textBoxBase != null)
                    textBoxBase.SelectAll();

                UIElement uiElement = element as UIElement;
                if (uiElement != null)
                {
                    // Tente de lui donner le focus
                    if (!uiElement.Focus())
                    {
                        // S'il n'est pas focusable, tente de le donner à son premier enfant qui l'est
                        uiElement = uiElement.GetFirstFocusableChild();
                        if (uiElement != null)
                            uiElement.Focus();
                    }
                }
            }
        }

        #endregion
    }
#endif
}