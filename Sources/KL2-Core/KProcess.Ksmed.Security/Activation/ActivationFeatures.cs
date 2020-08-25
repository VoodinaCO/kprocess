using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Security.Activation
{
    /// <summary>
    /// Les fonctionnalités de la license.
    /// </summary>
    public enum ActivationFeatures : short
    {

        /// <summary>
        /// Toutes les fonctionnalités activées.
        /// </summary>
        All = 1,

        /// <summary>
        /// Fonctionnalités lecture seule.
        /// </summary>
        ReadOnly = 2

    }
}
