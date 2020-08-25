// -----------------------------------------------------------------------
// <copyright file="Command.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Windows.Input;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Classe permettant de découpler les commandes
    /// </summary>
    public class Command : Command<object>
    {
        #region Constructeurs

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="execute">Méthode d'exécution de la commande</param>
        public Command(Action execute)
            : base(o => execute())
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="execute">Méthode d'exécution de la commande</param>
        /// <param name="canExecute">Méthode permettant de savoir si la commande peut être exécutée</param>
        public Command(Action execute, Func<bool> canExecute)
            : base(o => execute(), o => canExecute())
        {
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Exécute la commande
        /// </summary>
        public void Execute()
        {
            Execute(null);
        }

        #endregion
    }
}
