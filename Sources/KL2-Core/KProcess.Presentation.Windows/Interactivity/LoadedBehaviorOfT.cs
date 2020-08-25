using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interactivity;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Représente un behavior permettant d'exécuter une action une fois que l'objet attaché a été chargé.
    /// </summary>
    /// <typeparam name="T">Le type de l'objet associé.</typeparam>
    public abstract class LoadedBehavior<T> : BehaviorBase<T>
        where T : FrameworkElement
    {

        /// <summary>
        /// Appelé une fois que le comportement est attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (base.AssociatedObject.IsLoaded)
                OnLoaded();
            else
                base.AssociatedObject.Loaded += new RoutedEventHandler(AssociatedObject_Loaded);
        }

        /// <summary>
        /// Appelé lorsque le comportement est détaché de son AssociatedObject, mais avant qu'il ne se soit produit réellement.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            base.AssociatedObject.Loaded -= new RoutedEventHandler(AssociatedObject_Loaded);
        }

        /// <summary>
        /// Gère l'évènement Loaded de l'AssociatedObject.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            base.AssociatedObject.Loaded -= new RoutedEventHandler(AssociatedObject_Loaded);
            OnLoaded();
        }

        /// <summary>
        /// Appelé lorsque l'objet attaché a été chargé.
        /// </summary>
        protected virtual void OnLoaded()
        {
        }

    }
}
