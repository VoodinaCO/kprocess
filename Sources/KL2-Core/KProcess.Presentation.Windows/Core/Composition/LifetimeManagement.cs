// -----------------------------------------------------------------------
// <copyright file="LifetimeManagement.cs" company="Tekigo">
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
    /// Le mode de gestion de durée de vie d'un export MEF.
    /// </summary>
    public enum LifetimeManagement
    {
        /// <summary>
        /// A chaque import, une nouvelle instance est créée.
        /// </summary>
        PerCall
    }
}
