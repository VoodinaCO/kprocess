// -----------------------------------------------------------------------
// <copyright file="ModuleControllerBase.cs" company="Tekigo">
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
    /// Définit l'implémentation de base des contrôleurs de module
    /// </summary>
    public abstract class ModuleControllerBase : ControllerBase, IModuleController
    {
        /// <summary>
        /// Obtient le nom du module
        /// </summary>
        public string Name { get; protected set; }
    }
}
