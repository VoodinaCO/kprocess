using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Représente une barre de notification.
    /// </summary>
    public class NotificationBar : ListBox
    {

        /// <summary>
        /// Initialise la classe <see cref="NotificationBar"/>.
        /// </summary>
        static NotificationBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NotificationBar), new FrameworkPropertyMetadata(typeof(NotificationBar)));
        }

        /// <summary>
        /// Crée un conteneur.
        /// </summary>
        /// <returns>Le conteneur.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new NotificationBarItem();
        }

        /// <summary>
        /// Détermine si l'objet est un conteneur valide.
        /// </summary>
        /// <param name="item">L'objet à valider.</param>
        /// <returns><c>true</c> si l'objet est un conteneur valide.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is NotificationBarItem;
        }

    }
}
