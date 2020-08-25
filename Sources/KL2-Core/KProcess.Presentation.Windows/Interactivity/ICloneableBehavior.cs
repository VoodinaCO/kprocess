using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Identifie un behavior gérant ses clones
    /// </summary>
    public interface ICloneableBehavior
    {

        /// <summary>
        /// Définit une valeur indiquant si le behavior est un clone.
        /// </summary>
        bool IsClone { set; }

    }
}
