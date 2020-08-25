// -----------------------------------------------------------------------
// <copyright file="CustomBindingBase.cs" company="Tekigo">
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
    /// Binding de base des bindings personnalisés
    /// </summary>
    public abstract class CustomBindingBase : Binding
    {
        #region Constructeurs

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public CustomBindingBase()
            : base()
        {
            Init();
        }

        /// <summary>
        /// Constructeur indiquant le chemin de la source de données
        /// </summary>
        /// <param name="path">chemin de la source de données</param>
        public CustomBindingBase(string path)
            : base(path)
        {
            Init();
        }

        #endregion

        #region Méthodes protégées

        /// <summary>
        /// Initialise le binding
        /// </summary>
        protected virtual void Init()
        {
        }

        #endregion
    }
}
