// -----------------------------------------------------------------------
// <copyright file="VisualTreeHelperExtensions.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Propose un ensemble de méthodes permettant d'obtenir des informations sur l'arbre visuel,
    /// complétant celles de VisualTreeHelper du framework
    /// </summary>
    public static class VisualTreeHelperExtensions
    {
#if !SILVERLIGHT

        /// <summary>
        /// Tente de trouver le parent de child du type T
        /// </summary>
        /// <typeparam name="T">type du parent à retrouver</typeparam>
        /// <param name="child">enfant à investiguer</param>
        /// <returns>le premier parent du type demandé, null si aucun</returns>
        public static T TryFindParent<T>(this DependencyObject child)
          where T : DependencyObject
        {
            // Obtient le parent
            DependencyObject parentObject = GetParentObject(child);

            // Aucun parent n'a été trouvé
            if (parentObject == null)
                return null;

            // vérifie le type du parent trouvé
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
            {
                // Remonte d'un parent récursivement
                return TryFindParent<T>(parentObject);
            }
        }

        /// <summary>
        /// Méthode permettant de récupérer le parent d'un objet
        /// </summary>
        /// <remarks>supporte les contentElements</remarks>
        /// <param name="child">enfant à investiguer</param>
        /// <returns>le parent de l'enfant</returns>
        public static DependencyObject GetParentObject(this DependencyObject child)
        {
            if (child == null)
                return null;

            ContentElement contentElement = child as ContentElement;

            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null)
                    return parent;

                FrameworkContentElement fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            // Gestion classique de récupération du parent
            var p = VisualTreeHelper.GetParent(child);
            if (p == null)
                return LogicalTreeHelper.GetParent(child);
            else
                return p;
        }

        /// <summary>
        /// Retourne tous les enfants (visuels ou logique) d'item répondant à un prédicat
        /// </summary>
        /// <param name="item">objet parent à inspecter</param>        
        /// <returns>le premier enfant de type TChild</returns>
        public static TChild FindFirstChild<TChild>(this DependencyObject item)
            where TChild : DependencyObject
        {
            return item.FindChildren(d => d is TChild).FirstOrDefault() as TChild;
        }

        /// <summary>
        /// Retourne tous les enfants (visuels ou logique) d'item répondant à un prédicat
        /// </summary>
        /// <param name="item">objet parent à inspecter</param>
        /// <param name="predicate">prédicat à matcher</param>
        /// <returns>tous les enfants (visuels ou logique) d'item répondant à un prédicat</returns>
        public static DependencyObject FindFirstChild(this DependencyObject item, Predicate<DependencyObject> predicate)
        {
            return item.FindChildren(predicate).FirstOrDefault();
        }

        /// <summary>
        /// Retourne tous les enfants (visuels ou logique) d'item répondant à un prédicat
        /// </summary>
        /// <param name="item">objet parent à inspecter</param>
        /// <param name="predicate">prédicat à matcher</param>
        /// <returns>tous les enfants (visuels ou logique) d'item répondant à un prédicat</returns>
        public static TChild FindFirstChild<TChild>(this DependencyObject item, Predicate<TChild> predicate)
            where TChild : DependencyObject
        {
            return item.FindChildren(i => i is TChild child && predicate(child)).FirstOrDefault() as TChild;
        }

        /// <summary>
        /// Retourne tous les enfants (visuels ou logique) d'item répondant à un prédicat
        /// </summary>
        /// <param name="item">objet parent à inspecter</param>
        /// <param name="predicate">prédicat à matcher</param>
        /// <returns>tous les enfants (visuels ou logique) d'item répondant à un prédicat</returns>
        public static TChild FindLastChild<TChild>(this DependencyObject item, Predicate<TChild> predicate)
            where TChild : DependencyObject
        {
            return item.FindChildren(i => i is TChild child && predicate(child)).LastOrDefault() as TChild;
        }

        /// <summary>
        /// Retourne tous les enfants (visuels ou logique) d'item répondant à un prédicat
        /// </summary>
        /// <param name="item">objet parent à inspecter</param>
        /// <param name="predicate">prédicat à matcher</param>
        /// <returns>tous les enfants (visuels ou logique) d'item répondant à un prédicat</returns>
        public static IEnumerable<DependencyObject> FindChildren(this DependencyObject item, Predicate<DependencyObject> predicate)
        {
            if (item != null)
            {
                foreach (DependencyObject child in item.GetChildren())
                {
                    if (child != null && predicate(child))
                        yield return child;

                    foreach (var childOfChild in child.FindChildren(predicate))
                        yield return childOfChild;
                }
            }
        }

        /// <summary>
        /// Retourne les enfants (visuels ou logique) d'item
        /// </summary>
        /// <param name="item">objet à inspecter</param>
        /// <returns>les enfants (visuels ou logique) d'item</returns>
        public static IEnumerable<DependencyObject> GetChildren(this DependencyObject item)
        {
            if (item == null || !(item is Visual)) yield break;

            int count = VisualTreeHelper.GetChildrenCount(item);

            if (count == 0)
            {
                foreach (object obj in LogicalTreeHelper.GetChildren(item))
                {
                    var depObj = obj as DependencyObject;
                    if (depObj != null) yield return (DependencyObject)obj;
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    yield return VisualTreeHelper.GetChild(item, i);
                }
            }
        }

        /// <summary>
        /// Tente de trouver l'élément de même nature suivant dans l'arbre
        /// </summary>
        /// <typeparam name="T">type d'élément recherché</typeparam>
        /// <param name="item">item dont on cherche le suivant</param>
        /// <param name="predicate">prédicat pour filtrer la recherche</param>
        /// <returns>l'élément trouvé, null sinon</returns>
        public static T TryFindNext<T>(this T item, Predicate<T> predicate)
            where T : DependencyObject
        {
            // Tente de récupérer l'élément parmis ses enfants
            T result = (T)item.FindFirstChild(d => d != item && d is T);

            if (result == null)
            {
                // Parcourt les parents
                DependencyObject parent = GetParentObject(item);

                while (parent != null)
                {
                    // Cherche parmis les enfants
                    var children = parent.FindChildren(d => d is T && predicate((T)d));

                    if (children != null)
                    {
                        // Récupère l'élément et le suivant
                        var itemAndNextOne = children.SkipWhile(d => d != item).Take(2);
                        if (itemAndNextOne != null && itemAndNextOne.Count() == 2)
                            result = (T)itemAndNextOne.ElementAt(1);
                    }

                    if (result != null)
                        break;

                    parent = GetParentObject(parent);
                }
            }

            return result;
        }

        /// <summary>
        /// Tente de trouver l'élément de même nature précédant dans l'arbre
        /// </summary>
        /// <typeparam name="T">type d'élément recherché</typeparam>
        /// <param name="item">item dont on cherche le précédant</param>
        /// <param name="predicate">prédicat pour filtrer la recherche</param>
        /// <returns>l'élément trouvé, null sinon</returns>
        public static T TryFindPrevious<T>(this T item, Predicate<T> predicate)
            where T : DependencyObject
        {
            T result = null;

            // Parcourt les parents
            DependencyObject parent = GetParentObject(item);

            while (parent != null)
            {
                // Cherche parmis les enfants
                var children = parent.FindChildren(d => d is T && predicate((T)d));

                if (children != null)
                {
                    // Récupère les éléments précédants
                    var itemAndPreviousOne = children.TakeWhile(d => d != item);
                    if (itemAndPreviousOne != null)
                    {
                        var count = itemAndPreviousOne.Count();
                        if (count > 0)
                            result = (T)itemAndPreviousOne.ElementAt(count - 1);
                    }
                }

                if (result != null)
                    break;

                parent = GetParentObject(parent);
            }

            return result;
        }

        /// <summary>
        /// Retourne le dernier enfant d'item (au niveau de l'arbre visuel) pouvant avoir le focus
        /// </summary>
        /// <param name="item">objet à inspecter</param>
        /// <returns>le dernier enfant d'item pouvant avoir le focus, null si aucun</returns>
        public static UIElement GetLastFocusableChild(this DependencyObject item)
        {
            UIElement e = GetFirstFocusableChild(item);
            UIElement result = e;

            while (result != null)
            {
                e = result;
                result = GetFirstFocusableChild(result);
            }

            return e;
        }

        /// <summary>
        /// Retourne le premier enfant d'item pouvant avoir le focus
        /// </summary>
        /// <param name="item">objet à inspecter</param>
        /// <returns>le premier enfant d'item pouvant avoir le focus, null si aucun</returns>
        public static UIElement GetFirstFocusableChild(this DependencyObject item)
        {
            if (item != null)
            {
                foreach (var child in item.FindChildren(c => !FocusManager.GetIsFocusScope(c) && c is UIElement))
                {
                    UIElement e = (UIElement)child;
                    if (e.IsEnabled && e.Focusable && (!(e is FrameworkElement) || (((FrameworkElement)e).IsVisible)))
                        return e;
                }
            }

            return null;
        }

#endif
    }
}
