using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Contient les codes de types connus de catégories d'actions.
    /// </summary>
    public static class KnownActionCategoryTypes
    {
        /// <summary>
        /// Interne
        /// </summary>
        public const string I = "I00001";

        /// <summary>
        /// Externe
        /// </summary>
        public const string E = "E00001";

        /// <summary>
        /// Supprimé
        /// </summary>
        public const string S = "S00001";

        /// <summary>
        /// Permet de trier les types d'actions.
        /// </summary>
        public class ActionCategoryTypeDefaultOrderComparer : IComparer<string>
        {

            private static readonly string[] _order = new string[]
            {
                I,
                E,
                S,
            };

            public int Compare(string x, string y)
            {
                var indexX = Array.IndexOf(_order, x);
                if (indexX == -1)
                    indexX = _order.Length;
                var indexY = Array.IndexOf(_order, y);
                if (indexY == -1)
                    indexY = _order.Length;

                return indexX - indexY;
            }
        }
        
    }
}
