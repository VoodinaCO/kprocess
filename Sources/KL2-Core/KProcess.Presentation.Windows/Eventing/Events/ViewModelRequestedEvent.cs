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
    /// Evénement propagé sur le bus d'événements pour indiquer que l'affichage d'un viewModel est requis
    /// </summary>
    public class ViewModelRequestedEvent : EventBase
    {
        #region Propriétés

        /// <summary>
        /// Obtient le viewModel à afficher
        /// </summary>
        public ViewModelBase ViewModel { get; private set; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="sender">élément à l'origine de l'événement</param>
        /// <param name="viewModel">instance du viewModel à afficher</param>
        public ViewModelRequestedEvent(object sender, ViewModelBase viewModel)
            : base(sender)
        {
            ViewModel = viewModel;
        }

        #endregion
    }
}
