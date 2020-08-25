// -----------------------------------------------------------------------
// <copyright file="InheritedExportAsAttribute.cs" company="Tekigo">
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
    /// Spécifie qu'un type fournit un export particulier et que les sous types de cette classe fourniront également cet export.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple=false, Inherited=true)]
    public class InheritedExportAsAttribute : System.ComponentModel.Composition.InheritedExportAttribute
    {
        /// <summary>
        /// Obtient le mode de gestion de la durée de vie.
        /// </summary>
        public LifetimeManagement LifetimeManagement { get; private set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="InheritedExportAsAttribute"/>.
        /// </summary>
        /// <param name="mode">le mode de gestion de la durée de vie.</param>
        public InheritedExportAsAttribute(LifetimeManagement mode)
            : base()
        {
            LifetimeManagement = mode;
        }
    }

    /// <summary>
    /// Spécifie qu'un type fournit un export particulier et que les sous types de cette classe fourniront également cet export.
    /// A chaque import, une nouvelle instance de cet export est créée.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class InheritedExportAsPerCallAttribute : InheritedExportAsAttribute
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="InheritedExportAsPerCallAttribute"/>.
        /// </summary>
        public InheritedExportAsPerCallAttribute()
            : base(LifetimeManagement.PerCall)
        {
        }
    }
}
