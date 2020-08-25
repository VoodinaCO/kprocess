// -----------------------------------------------------------------------
// <copyright file="ImportAsPerCallAttribute.cs" company="Tekigo">
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
    /// Spécifie qu'une propriété, un champ ou un paramètre requiert que l'export associé renvoie une nouvelle instance à chaque import.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ImportAsPerCallAttribute : ImportAttribute
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ImportAsPerCallAttribute"/>.
        /// </summary>
        public ImportAsPerCallAttribute()
        {
            this.RequiredCreationPolicy = CreationPolicy.NonShared;
        }
    }
}
