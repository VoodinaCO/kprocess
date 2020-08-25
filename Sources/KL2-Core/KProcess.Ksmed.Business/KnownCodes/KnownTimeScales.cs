using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Contient les échelles de temps connues.
    /// </summary>
    public static class KnownTimeScales
    {

        /// <summary>
        /// Seconde
        /// </summary>
        public static readonly long Second = TimeSpan.FromSeconds(1).Ticks;

        /// <summary>
        /// Dixième de seconde
        /// </summary>
        public static readonly long SecondTenth = TimeSpan.FromSeconds(.1).Ticks;

        /// <summary>
        /// Centième de seconde
        /// </summary>
        public static readonly long SecondHundredth = TimeSpan.FromSeconds(.01).Ticks;

        /// <summary>
        /// Millième de seconde
        /// </summary>
        public static readonly long SecondThousandth = TimeSpan.FromSeconds(.001).Ticks;

    }
}
