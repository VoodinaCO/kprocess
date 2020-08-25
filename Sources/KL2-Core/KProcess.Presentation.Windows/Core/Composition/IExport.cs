// -----------------------------------------------------------------------
// <copyright file="IExport.cs" company="Tekigo">
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
    /// Identifie un export MEF gérant la durée de vie d'un élément.
    /// </summary>
    public interface IExport
    {
        /// <summary>
        /// Obtient le type de cycle de vie de la vue
        /// </summary>
        LifetimeManagement LifetimeManagement { get; }
    }
}
