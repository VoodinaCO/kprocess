// -----------------------------------------------------------------------
// <copyright file="EventBaseOfT.cs" company="Tekigo">
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
    /// Classe d'événement générique du bus d'événements
    /// </summary>
    public abstract class EventBase<TData> : EventBase
    {
        /// <summary>
        /// Obtient la donnée associée à l'événement
        /// </summary>
        public TData Data { get; private set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="sender">objet à l'origine de l'événement</param>
        /// <param name="data">données de l'événement</param>
        public EventBase(object sender, TData data)
            : base(sender)
        {
            Data = data;
        }
    }
}
