// -----------------------------------------------------------------------
// <copyright file="CloseViewRequestedEvent.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Evénement propagé sur le bus d'événements pour indiquer qu'un ViewModel doit être fermé
    /// </summary>
    public class CloseViewRequestedEvent : EventBase
    {
        #region Propriétés

        /// <summary>
        /// Obtient le viewModel à fermer
        /// </summary>
        public ViewModelBase ViewModel { get; private set; }

        #endregion

        #region Constructeurs

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="sender">élément à l'origine de l'événement</param>
        /// <param name="viewModel">viewModel à fermer</param>
        public CloseViewRequestedEvent(object sender, ViewModelBase viewModel)
            : base(sender)
        {
            ViewModel = viewModel;
        }

        /// <summary>
        /// Constructeur utilisé lorsque c'est le viewModel lui-même qui demande sa fermeture
        /// </summary>
        /// <param name="viewModel">viewModel à fermer</param>
        public CloseViewRequestedEvent(ViewModelBase viewModel)
            : this(viewModel, viewModel)
        {
        }

        #endregion
    }
}
