// -----------------------------------------------------------------------
// <copyright file="IWindowView.cs" company="Tekigo">
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
    /// Définit l'interface de base des vues étant leur propre conteneur
    /// </summary>
    public interface IWindowView : IView
    {
        /// <summary>
        /// Affiche la vue
        /// </summary>
        void Show();

        /// <summary>
        /// Active la vue
        /// </summary>
        /// <returns>true si la vue a été activée, false sinon</returns>
        bool Activate();

        /// <summary>
        /// Evénement déclenché lors de la fermeture de la vue
        /// </summary>
        event EventHandler Closed;
    }
}
