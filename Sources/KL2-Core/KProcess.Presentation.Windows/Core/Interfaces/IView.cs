// -----------------------------------------------------------------------
// <copyright file="IView.cs" company="Tekigo">
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
    /// Définit l'interface de base des vues
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Obtient ou définit le dataContext de la vue
        /// </summary>
        object DataContext { get; set; }

        /// <summary>
        /// Evénement déclenché lors du déchargement de la vue
        /// </summary>
        event RoutedEventHandler Unloaded;
    }
}
