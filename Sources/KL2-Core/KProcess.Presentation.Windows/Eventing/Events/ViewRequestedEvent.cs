// -----------------------------------------------------------------------
// <copyright file="ViewRequestedEvent.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Evénement propagé sur le bus d'événements pour indiquer que l'affichage d'une vue est requis
    /// </summary>
    public class ViewRequestedEvent : EventBase
    {
        #region Propriétés

        /// <summary>
        /// Obtient la vue à afficher
        /// </summary>
        public IView View { get; private set; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="sender">élément à l'origine de l'événement</param>
        /// <param name="view">instance de la vue à afficher</param>
        public ViewRequestedEvent(object sender, IView view)
            : base(sender)
        {
            View = view;
        }

        #endregion
    }
}
