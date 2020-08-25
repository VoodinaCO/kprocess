using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Utils
{
    public class WbsUtil
    {

        /// <summary>
        /// Diminue l'indentation du WBS spécifié à la fin.
        /// </summary>
        /// <param name="wbs">Le wbs.</param>
        /// <returns>Le wbs désindenté.</returns>
        public static string Unindent(string wbs)
        {
            var parts = GetParts(wbs);
            return LevelsToWBS(parts.Take(parts.Length - 1));
        }

        private const char _separator = '.';


        /// <summary>
        /// Convertit des niveaux d'indentation en WBS.
        /// </summary>
        /// <param name="levels">Les niveaux.</param>
        /// <returns>Le WBS</returns>
        public static string LevelsToWBS(IEnumerable<int> levels)
        {
            return string.Join(_separator.ToString(), levels);
        }


        /// <summary>
        /// Obtient les parties du WBS.
        /// </summary>
        /// <param name="wbs">Le WBS.</param>
        /// <returns>Les parties</returns>
        public static int[] GetParts(string wbs)
        {
            return wbs.Split(_separator).Select(str => int.Parse(str)).ToArray();
        }

        /// <summary>
        /// Détermine si input commence par start.
        /// </summary>
        /// <param name="input">la chaine d'entrée.</param>
        /// <param name="start">Le début à tester.</param>
        public static bool StartsWith(int[] input, int[] start)
        {
            Assertion.NotNull(input, "input");
            Assertion.NotNull(start, "start");

            if (start.Length > input.Length)
                return false;

            for (int i = 0; i < start.Length; i++)
            {
                if (input[i] != start[i])
                    return false;
            }

            return true;
        }

        public static List<PublishedAction> GetParents(PublishedAction child, List<PublishedAction> others)
        {
            return others
                .Where(action => !string.Equals(child.WBS, action.WBS) && StartsWith(child.WBSParts, action.WBSParts))
                .OrderBy(action => action.WBSParts, new WBSPartsComparer()).ToList();
        }

    }
}