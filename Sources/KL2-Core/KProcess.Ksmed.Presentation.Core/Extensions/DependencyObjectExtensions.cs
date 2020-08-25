using System;
using System.Windows;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Regroupe l'ensemble des méthodes d'extensions relatives aux DependencyObjects
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Chercher un ancetre dans l'arbre visuel du type spécifié
        /// </summary>
        /// <typeparam name="T">Le type d'ancetre à recherché</typeparam>
        /// <param name="dependencyObject">Le noeud dans l'arbre visuel à partir duquel il faut chercher</param>
        /// <returns>Le premier noeud trouvé, ou null si aucun noeud parent ne correspond</returns>
        public static T FindAncestor<T>(this DependencyObject dependencyObject)
            where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);
 
            if (parent == null) return null;
 
            var parentT = parent as T;
            return parentT ?? FindAncestor<T>(parent);
        }

        /// <summary>
        /// Chercher un ancetre dans l'arbre visuel du type spécifié
        /// </summary>
        /// <typeparam name="T">Le type d'ancetre à recherché</typeparam>
        /// <param name="dependencyObject">Le noeud dans l'arbre visuel à partir duquel il faut chercher</param>
        /// <returns>Le premier noeud trouvé, ou null si aucun noeud parent ne correspond</returns>
        public static FrameworkElement FindAncestorByDataContext<T>(this DependencyObject dependencyObject)
            where T: class
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);

            if (parent == null) return null;

            if (parent is FrameworkElement parentWithDataContext)
            {
                var parentDataContextT = parentWithDataContext.DataContext as T;
                return dependencyObject as FrameworkElement ?? FindAncestorByDataContext<T>(parent);
            }

            return FindAncestorByDataContext<T>(parent);
        }

        /// <summary>
        /// Chercher un ancetre dans l'arbre visuel du type spécifié
        /// </summary>
        /// <param name="dataContextType">Le type d'ancetre à recherché</param>
        /// <param name="dependencyObject">Le noeud dans l'arbre visuel à partir duquel il faut chercher</param>
        /// <returns>Le premier noeud trouvé, ou null si aucun noeud parent ne correspond</returns>
        public static FrameworkElement FindAncestorByDataContext(this DependencyObject dependencyObject, Type dataContextType)
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);

            if (parent == null) return null;

            if (parent is FrameworkElement parentWithDataContext && parentWithDataContext.DataContext != null)
            {
                return dataContextType.IsAssignableFrom(parentWithDataContext.DataContext.GetType())
                    ? parentWithDataContext
                    : FindAncestorByDataContext(parent, dataContextType);
            }

            return FindAncestorByDataContext(parent, dataContextType);
        }
    }
}
