// -----------------------------------------------------------------------
// <copyright file="CommandBinding.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace KProcess.Presentation.Windows.Data
{
    /// <summary>
    /// Binding spécifique aux commandes WPF
    /// </summary>
    /// <remarks>met par défaut le mode de binding à OneTime</remarks>
    public class CommandBinding : CustomBindingBase
    {
        #region Constructeurs

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public CommandBinding()
            : base()
        {
        }

        /// <summary>
        /// Constructeur indiquant le chemin de la source de données
        /// </summary>
        /// <param name="path">chemin de la source de données</param>
        public CommandBinding(string path)
            : base(path)
        {
        }

        #endregion

        #region Surcharges

        /// <summary>
        /// Initialise le binding
        /// </summary>
        protected override void Init()
        {
            this.Mode = BindingMode.OneTime;
        }

        #endregion
    }
}
