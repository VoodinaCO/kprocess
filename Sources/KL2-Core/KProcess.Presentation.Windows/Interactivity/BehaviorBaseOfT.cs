using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interactivity;

namespace KProcess.Presentation.Windows
{

    /// <summary>
    /// Représente un Behavior qui gère l'évènement OnUnloaded car <see cref="Behavior.OnDetaching"/> n'est pas toujours appelé.
    /// </summary>
    /// <typeparam name="T">Le type associé.</typeparam>
    public abstract class BehaviorBase<T> : Behavior<T>
        where T : FrameworkElement
    {

        private bool _isClean = true;

        /// <summary>
        /// Appelé une fois que le comportement est attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Unloaded += OnAssociatedObjectUnloaded;
            _isClean = false;
        }

        /// <summary>
        /// Appelé lorsque le comportement est détaché de son AssociatedObject, mais avant qu'il ne se soit produit réellement.
        /// </summary>
        protected override void OnDetaching()
        {
            CleanUp();

            base.OnDetaching();
        }

        /// <summary>
        /// Appelé lorsque l'objet associé est déchargé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.EventArgs"/> contenant les données de l'évènement.</param>
        private void OnAssociatedObjectUnloaded(object sender, EventArgs e)
        {
            CleanUp();
        }

        /// <summary>
        /// Se désabonne.
        /// </summary>
        private void CleanUp()
        {
            if (_isClean)
                return;

            _isClean = true;

            if (AssociatedObject != null)
            {
                AssociatedObject.Unloaded -= OnAssociatedObjectUnloaded;
            }

            OnDetaching();
        }
    }
}
