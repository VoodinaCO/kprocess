// -----------------------------------------------------------------------
// <copyright file="IExtendedCommand.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Propose une interface de commande étendue
    /// </summary>
    public interface IExtendedCommand : INotifyCommandExecuted, INotifyPropertyChanged
    {
        /// <summary>
        /// Indique si la commande est disponible
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Rafraîchit l'état d'exécution de la commande
        /// </summary>
        void Invalidate();
    }
}
