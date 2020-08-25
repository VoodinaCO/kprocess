// -----------------------------------------------------------------------
// <copyright file="INotifyCommandExecuted.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Permet de notifier que la commande a été exécutée
    /// </summary>
    public interface INotifyCommandExecuted : ICommand
    {
        /// <summary>
        /// Evénement déclenché lorsque la commande a été exécutée
        /// </summary>
        event EventHandler<EventArgs<object>> CommandExecuted;
    }
}
