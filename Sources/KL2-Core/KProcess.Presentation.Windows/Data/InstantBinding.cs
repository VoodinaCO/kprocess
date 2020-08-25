using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace KProcess.Presentation.Windows.Data
{
#if !SILVERLIGHT
    /// <summary>
    /// Binding fonctionnant dès que la propriété change.
    /// </summary>
    public class InstantBinding : CustomBindingBase
    {
        #region Constructeurs

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public InstantBinding()
            : base()
        {
        }

        /// <summary>
        /// Constructeur indiquant le chemin de la source de données
        /// </summary>
        /// <param name="path">chemin de la source de données</param>
        public InstantBinding(string path)
            : base(path)
        {
        }

        #endregion

        /// <summary>
        /// Initialise le binding
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }
    }
#endif
}
