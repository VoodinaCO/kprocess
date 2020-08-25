// -----------------------------------------------------------------------
// <copyright file="InstantValidatingBinding.cs" company="Tekigo">
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
#if !SILVERLIGHT

    /// <summary>
    /// Binding gérant nativement la validation dés le changement de valeur de propriété
    /// </summary>
    /// <remarks>met par défaut ValidatesOnDataErrors à true et UpdateSourceTrigger à PropertyChanged</remarks>
    public class InstantValidatingBinding : ValidatingBinding
    {
        #region Constructeurs

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public InstantValidatingBinding()
            : base()
        {
        }

        /// <summary>
        /// Constructeur indiquant le chemin de la source de données
        /// </summary>
        /// <param name="path">chemin de la source de données</param>
        public InstantValidatingBinding(string path)
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
            base.Init();
            this.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }

        #endregion
    }

#endif
}
