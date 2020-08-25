// -----------------------------------------------------------------------
// <copyright file="ValidatingBinding.cs" company="Tekigo">
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
    /// Binding gérant nativement la validation
    /// </summary>
    /// <remarks>met par défaut ValidatesOnDataErrors à true</remarks>
    public class ValidatingBinding : CustomBindingBase
    {
        #region Constructeurs

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public ValidatingBinding()
            : base()
        {
        }

        /// <summary>
        /// Constructeur indiquant le chemin de la source de données
        /// </summary>
        /// <param name="path">chemin de la source de données</param>
        public ValidatingBinding(string path)
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
            this.ValidatesOnDataErrors = true;
        }

        #endregion
    }
}
