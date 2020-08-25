// -----------------------------------------------------------------------
// <copyright file="IModuleController.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Définit l'interface de base d'un contrôleur de module
    /// </summary>
    [InheritedExport]
    public interface IModuleController : IController
    {
        /// <summary>
        /// Obtient le nom du module
        /// </summary>
        string Name { get; }
    }
}
