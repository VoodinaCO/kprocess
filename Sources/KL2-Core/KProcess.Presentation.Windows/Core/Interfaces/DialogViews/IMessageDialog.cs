// -----------------------------------------------------------------------
// <copyright file="IMessageDialog.cs" company="Tekigo">
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
    /// Définit l'interface des vues de dialogue de message
    /// </summary>
    [InheritedExport]
    public interface IMessageDialog : IDialogView
    {

        /// <summary>
        /// Affiche un message
        /// </summary>
        /// <param name="message">le message</param>
        /// <param name="caption">la légende du message</param>
        /// <param name="buttons">les boutons proposés à l'utilisateur</param>
        /// <param name="image">l'image à afficher</param>
        /// <returns>le résultat du choix de l'utilisateur</returns>
        MessageDialogResult Show(string message, string caption, MessageDialogButton buttons = MessageDialogButton.OK, MessageDialogImage image = MessageDialogImage.None);

    }
}
