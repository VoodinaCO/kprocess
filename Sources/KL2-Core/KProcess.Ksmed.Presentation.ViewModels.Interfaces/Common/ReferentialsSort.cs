using KProcess.Ksmed.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Compare deux instances de réferentiels.
    /// </summary>
    /// <remarks>
    /// Détermine d'abord si les libellés sont des entiers.
    /// Les entiers sont plus petits que des chaînes.
    /// </remarks>
    public class ReferentialsSort : IComparer
    {
        #region IComparer Members

        /// <inheritdoc />
        public int Compare(object x, object y)
        {
            var ref1 = (IActionReferential)x;
            var ref2 = (IActionReferential)y;

            int ref1Int;
            bool isRef1Int = int.TryParse(ref1.Label, out  ref1Int);

            int ref2Int;
            bool isRef2Int = int.TryParse(ref2.Label, out  ref2Int);

            if (isRef1Int && !isRef2Int)
                return -1;
            else if (!isRef1Int && isRef2Int)
                return 1;
            else if (isRef1Int && isRef2Int)
                return ref1Int.CompareTo(ref2Int);
            else
                return Comparer<string>.Default.Compare(ref1.Label, ref2.Label);
        }

        #endregion
    }

}
