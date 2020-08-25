// -----------------------------------------------------------------------
// <copyright file="CompositeViewModelBase.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Définit l'implémentation de base des viewModels composites
    /// </summary>
    public abstract class CompositeViewModelBase : ViewModelBase
    {
        /// <summary>
        /// Obtient ou définit la factory des views et viewModels
        /// </summary>
        [Import]
        protected IUXFactory UXFactory { get; set; }
    }
}
