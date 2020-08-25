// -----------------------------------------------------------------------
// <copyright file="IErrorDialog.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Définit l'interface des vues de dialogue d'erreur
    /// </summary>
    [InheritedExport]
    public interface IErrorDialog : IDialogView
    {

        /// <summary>
        /// Affiche un message d'erreur
        /// </summary>
        /// <param name="message">le message</param>
        /// <param name="caption">la légende du message</param>
        /// <param name="errorMessage">Le message de l'erreur.</param>
        void Show(string message, string caption = null, string errorMessage = null);

        /// <summary>
        /// Affiche un message d'erreur
        /// </summary>
        /// <param name="message">le message</param>
        /// <param name="caption">la légende du message</param>
        /// <param name="exception">L'exception associée.</param>
        void Show(string message, string caption, Exception exception, bool forceClose = false);

    }
}
