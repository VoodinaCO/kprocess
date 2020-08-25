using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Business.ActionsManagement
{

    /// <summary>
    /// Le comportement attendu de l'opération de désindentation.
    /// </summary>
    public enum UnindentationBehavior
    {
        /// <summary>
        /// Met l'élément désindenté au dessus de son parent actuel.
        /// </summary>
        PutAbove,

        /// <summary>
        /// Met l'élément désindenté à la suite de son parent actuel et de ses enfants.
        /// </summary>
        PutBelow,

        /// <summary>
        /// Met l'élément désindenté à la suite de son parent actuel mais avant tous les successeurs de l'action spécifiée.
        /// </summary>
        PutInline
    }

}
