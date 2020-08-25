// -----------------------------------------------------------------------
// <copyright file="IDialogFactory.cs" company="Tekigo">
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
    /// Définit le comportement d'une fabrique de boite de dialogues.
    /// </summary>
    [InheritedExport]
    public interface IDialogFactory
    {
        /// <summary>
        /// Obtient une instance de la vue de dialogue demandée
        /// </summary>
        /// <typeparam name="TDialogView">type de vue de dialogue demandée</typeparam>
        /// <returns>l'instance de la vue de dialogue demandée</returns>
        TDialogView GetDialogView<TDialogView>() where TDialogView : IDialogView;
    }
}
