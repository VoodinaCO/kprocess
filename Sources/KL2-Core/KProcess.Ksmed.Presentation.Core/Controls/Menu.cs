using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Représente un menu de l'application.
    /// </summary>
    public class Menu : ItemsControl
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Menu"/>.
        /// </summary>
        public Menu()
        {
            base.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);
        }

        /// <summary>
        /// Gère l'évènement StatusChanged sur l'ItemContainerGenerator.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.EventArgs"/> contenant les données de l'évènement.</param>
        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (base.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                // Rafraîchir le SeparatorVisibility.
                var count = base.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    var container = base.ItemContainerGenerator.ContainerFromIndex(i) as MenuItem;
                    if (container != null && i == 0)
                        container.SeparatorVisibility = System.Windows.Visibility.Collapsed;
                }
            }
        }
        /// <summary>
        /// Crée un conteneur.
        /// </summary>
        /// <returns>Le conteneur.</returns>
        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            return new MenuItem();
        }

        /// <summary>
        /// Détermine si l'objet peut être utilisé en tant que conteneur.
        /// </summary>
        /// <param name="item">L'objet.</param>
        /// <returns>
        /// <c>true</c> si l'objet peut être utilisé en tant que conteneur.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MenuItem;
        }

    }
}
