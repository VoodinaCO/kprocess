using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Contient les valeurs connues pour les actions réduites.
    /// </summary>
    public static class KnownReducedActionValues
    {

        /// <summary>
        /// Obtient les coûts connus.
        /// </summary>
        public static short[] Costs
        {
            get { return new short[] { 1, 2, 3 }; }
        }

        /// <summary>
        /// Obtient les difficultés connues.
        /// </summary>
        public static short[] Difficulties
        {
            get { return new short[] { 1, 2, 3 }; }
        }
    }
}
