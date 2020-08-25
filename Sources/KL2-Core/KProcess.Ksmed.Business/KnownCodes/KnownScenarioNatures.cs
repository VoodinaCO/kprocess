using System;
using System.Collections.Generic;

namespace KProcess.Ksmed.Business
{
    public static class KnownScenarioNatures
    {
        /// <summary>
        /// Initial
        /// </summary>
        public const string Initial = "INI001";
        /// <summary>
        /// Cible
        /// </summary>
        public const string Target = "CIB001";
        /// <summary>
        /// Réalisé
        /// </summary>
        public const string Realized = "REA001";

        /// <summary>
        /// Permet de trier les natures de scénarios.
        /// </summary>
        public class ScenarioNatureDefaultOrderComparer : IComparer<string>
        {

            static readonly string[] _order =
            {
                Initial,
                Target,
                Realized
            };

            public int Compare(string x, string y)
            {
                int indexX = Array.IndexOf(_order, x);
                if (indexX == -1)
                    indexX = _order.Length;
                int indexY = Array.IndexOf(_order, y);
                if (indexY == -1)
                    indexY = _order.Length;

                return indexX - indexY;
            }
        }
    }
}
