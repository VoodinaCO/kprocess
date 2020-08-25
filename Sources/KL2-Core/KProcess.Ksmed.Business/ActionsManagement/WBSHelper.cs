using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Business.ActionsManagement
{
    /// <summary>
    /// Fournit des méthodes d'aide à la gestion des WBS.
    /// </summary>
    public static class WBSHelper
    {
        const char _separator = '.';
        const int _first = 1;
        static readonly WBSComparer _wbsComparer = new WBSComparer();

        /// <summary>
        /// Obtient le niveau d'indentation pour le premier element dans un ensemble
        /// </summary>
        /// <returns></returns>
        public static int GetFirstIndentation() =>
            _first;

        /// <summary>
        /// Obtient les parties du WBS.
        /// </summary>
        /// <param name="wbs">Le WBS.</param>
        /// <returns>Les parties</returns>
        public static int[] GetParts(string wbs) =>
            wbs.Split(_separator).Select(str => int.Parse(str)).ToArray();

        /// <summary>
        /// Obtient la valeur du WBS qui serait celui voisin à droite de celui spécifié
        /// </summary>
        /// <param name="wbs">le WBS.</param>
        /// <returns>Le WBS voisin</returns>
        public static string GetNextSiblingWBS(string wbs)
        {
            var parts = GetParts(wbs);
            parts[parts.Count() - 1]++;
            return LevelsToWBS(parts);
        }

        /// <summary>
        /// Obtient l'indentation à partir du WBS.
        /// </summary>
        /// <param name="wbs">le WBS.</param>
        /// <returns>L'indentation.</returns>
        public static int IndentationFromWBS(string wbs)
        {
            if (string.IsNullOrEmpty(wbs))
                return 0;
            return GetParts(wbs).Count() - 1;
        }

        /// <summary>
        /// Convertit des niveaux d'indentation en WBS.
        /// </summary>
        /// <param name="levels">Les niveaux.</param>
        /// <returns>Le WBS</returns>
        public static string LevelsToWBS(IEnumerable<int> levels) =>
            string.Join(_separator.ToString(), levels);

        /// <summary>
        /// Obtient le numéro du WBS au niveau spécifié.
        /// </summary>
        /// <param name="wbs">Le WBS.</param>
        /// <param name="level">Le niveau.</param>
        /// <returns></returns>
        public static int GetNumberAtLevel(string wbs, int level) =>
            GetParts(wbs)[level];

        /// <summary>
        /// Détermine si l'action spécifiée contient des enfants.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="others">Les autres actions.</param>
        /// <returns>
        ///   <c>true</c> si le WBS spécifié contient des enfants; sinon, <c>false</c>.
        /// </returns>
        public static bool HasChildren(KAction action, IEnumerable<KAction> others) =>
            HasChildren(action.WBS, others.Select(a => a.WBS));

        public static bool HasChildren(DocumentationActionDraftWBS action, IEnumerable<DocumentationActionDraftWBS> others) =>
            HasChildren(action.WBS, others.Select(a => a.WBS));

        public static bool HasChildren(PublishedAction action, IEnumerable<PublishedAction> others) =>
            HasChildren(action.WBS, others.Select(a => a.WBS));

        /// <summary>
        /// Détermine si le WBS spécifiée contient des enfants.
        /// </summary>
        /// <param name="actionWbs">Le wbs.</param>
        /// <param name="othersWbs">Les autres wbs.</param>
        /// <returns>
        ///   <c>true</c> si le WBS spécifié contient des enfants; sinon, <c>false</c>.
        /// </returns>
        public static bool HasChildren(string actionWbs, IEnumerable<string> othersWbs)
        {
            var actionParts = GetParts(actionWbs);
            return othersWbs.Any(w => !string.Equals(w, actionWbs) && StartsWith(GetParts(w), actionParts));
        }

        /// <summary>
        /// Obtient les enfants et tous les enfants des enfants (...) de l'action spécifiée.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="others">The others.</param>
        /// <returns>Les enfants.</returns>
        public static IEnumerable<KAction> GetDescendants(KAction action, IEnumerable<KAction> others) =>
            others.Where(a => !string.Equals(a.WBS, action.WBS)
                              && StartsWith(a.WBSParts, action.WBSParts));

        /// <summary>
        /// Obtient les enfants immédiats (indentation + 1) de l'action spécifiée.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="others">The others.</param>
        /// <returns>Les enfants.</returns>
        public static IEnumerable<KAction> GetChildren(KAction action, IEnumerable<KAction> others) =>
            others.Where(a => !string.Equals(a.WBS, action.WBS)
                              && StartsWith(a.WBSParts, action.WBSParts)
                              && IndentationFromWBS(action.WBS) + 1 == IndentationFromWBS(a.WBS));

        /// <summary>
        /// Obtient les parents et grand parents de l'enfant spécifié.
        /// </summary>
        /// <param name="child">L'action enfant.</param>
        /// <param name="others">Les autres actions.</param>
        /// <returns>
        /// Les parents.
        /// </returns>
        public static KAction[] GetParents(KAction child, IEnumerable<KAction> others) =>
            others.Where(action => !string.Equals(child.WBS, action.WBS) && StartsWith(child.WBSParts, action.WBSParts))
                .OrderBy(action => action.WBSParts, _wbsComparer)
                .ToArray();

        /// <summary>
        /// Obtient le parent de l'enfant spécifié.
        /// </summary>
        /// <param name="child">L'action enfant.</param>
        /// <param name="others">Les autres actions.</param>
        /// <returns>
        /// Le parent.
        /// </returns>
        public static KAction GetParent(KAction child, IEnumerable<KAction> others) =>
            others.SingleOrDefault(action => string.Equals(Unindent(child.WBS), action.WBS));

        public static DocumentationActionDraftWBS GetParent(DocumentationActionDraftWBS child, IEnumerable<DocumentationActionDraftWBS> others) =>
            others.SingleOrDefault(action => string.Equals(Unindent(child.WBS), action.WBS));

        /// <summary>
        /// Obtient le premier WBS disponible et suivant celui spécifié.
        /// </summary>
        /// <param name="wbs">Le WBS ou <c>null</c>.</param>
        /// <param name="otherWBS">Les autres WBS.</param>
        /// <returns>
        /// Le WBS suivant
        /// </returns>
        public static string GetNextAvailableWBS(string wbs, IEnumerable<string> otherWBS)
        {
            if (string.IsNullOrEmpty(wbs))
                return "1";

            var indentation = IndentationFromWBS(wbs);
            var lastInSameLevel = otherWBS.Where(w => IndentationFromWBS(w) == indentation).Max();

            var parts = GetParts(lastInSameLevel);
            parts[parts.Length - 1] = parts[parts.Length - 1] + 1;
            return LevelsToWBS(parts);
        }

        /// <summary>
        /// Obtient un nouveau WBS à la fin de ceux fournis en argument.
        /// </summary>
        /// <param name="wbs">Les WBS.</param>
        /// <returns>Un nouveau WBS.</returns>
        public static string GetNewWBSAtEnd(IEnumerable<string> wbs)
        {
            var level1 = wbs.Where(w => IndentationFromWBS(w) == 0).OrderBy(w => w);
            return level1.Any() ? GetNextAvailableWBS(level1.Last(), wbs) : "1";
        }

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

        /// <summary>
        /// Diminue l'indentation du WBS spécifié à partir du niveau spécifié.
        /// </summary>
        /// <param name="wbs">Le wbs.</param>
        /// <param name="level">Le niveau.</param>
        /// <returns>Le wbs désindenté.</returns>
        public static string Unindent(string wbs, int level)
        {
            var parts = GetParts(wbs);

            var partsTaken = new int[parts.Length - 1];
            for (int i = 0; i < parts.Length; i++)
            {
                if (i < level)
                    partsTaken[i] = parts[i];
                else if (i > level)
                    partsTaken[i - 1] = parts[i];
            }

            return LevelsToWBS(partsTaken);
        }

        /// <summary>
        /// Augmente l'indentation du WBS spécifié à la fin.
        /// </summary>
        /// <param name="wbs">Le wbs.</param>
        /// <returns>Le wbs désindenté.</returns>
        public static string Indent(string wbs)
        {
            var parts = GetParts(wbs);
            var newParts = new int[parts.Length + 1];
            parts.CopyTo(newParts, 0);
            newParts[newParts.Length - 1] = 1;
            return LevelsToWBS(newParts);
        }

        /// <summary>
        /// Obtient un WBS représentant un déplacement vers le haut du WBS spécifié au niveau spécifié.
        /// </summary>
        /// <param name="wbs">Le WBS.</param>
        /// <param name="level">Le niveau.</param>
        /// <returns>Le wbs déplacé.</returns>
        public static string MoveUp(string wbs, int level)
        {
            var parts = GetParts(wbs);
            parts[level] = parts[level] - 1;
            return LevelsToWBS(parts);
        }

        /// <summary>
        /// Obtient un WBS représentant un déplacement vers le bas du WBS spécifié au niveau spécifié.
        /// </summary>
        /// <param name="wbs">Le WBS.</param>
        /// <param name="level">Le niveau.</param>
        /// <returns>Le wbs déplacé.</returns>
        public static string MoveDown(string wbs, int level)
        {
            var parts = GetParts(wbs);
            parts[level] = parts[level] + 1;
            return LevelsToWBS(parts);
        }

        /// <summary>
        /// Définit une valeur spécifique pour un niveau.
        /// </summary>
        /// <param name="wbs">Le WBS.</param>
        /// <param name="level">Le niveau.</param>
        /// <param name="value">La valeur.</param>
        /// <returns>Le WBS modifié.</returns>
        public static string SetNumberAtLevel(string wbs, int level, int value)
        {
            var parts = GetParts(wbs);
            parts[level] = value;
            return LevelsToWBS(parts);
        }

        /// <summary>
        /// Copie la valeur d'un WBS à un autre au niveau spécifié.
        /// </summary>
        /// <param name="sourceWbs">Le WBS source.</param>
        /// <param name="targetWbs">Le WBS cible.</param>
        /// <param name="level">Le niveau.</param>
        /// <returns>Le WBS modifié.</returns>
        public static string CopyNumberAtLevel(string sourceWbs, string targetWbs, int level) =>
            SetNumberAtLevel(targetWbs, level, GetNumberAtLevel(sourceWbs, level));

        /// <summary>
        /// Calcule le décalage vertical entre deux actions.
        /// </summary>
        /// <param name="firstAction">La première action.</param>
        /// <param name="secondAction">La seconde action.</param>
        /// <param name="allActions">Toutes les actions.</param>
        /// <returns>Le décalage</returns>
        public static int GetVerticalOffset(KAction firstAction, KAction secondAction, IEnumerable<KAction> allActions)
        {
            var actions = allActions.OrderBy(a => a.WBSParts, _wbsComparer).ToList();
            return actions.IndexOf(secondAction) - actions.IndexOf(firstAction);
        }

        /// <summary>
        /// Détermine si les deux WBS spécifiés peuvent se succéder.
        /// </summary>
        /// <param name="wbs1">Le premier WBS.</param>
        /// <param name="wbs2">Le second WBS.</param>
        /// <returns>
        ///   <c>true</c> si les deux WBS spécifiés peuvent se succéder; sinon, <c>false</c>.
        /// </returns>
        public static bool CanBeSuccessive(string wbs1, string wbs2)
        {
            var parts1 = GetParts(wbs1);
            var parts2 = GetParts(wbs2);

            int identicalParts;

            if (parts2.Length > parts1.Length)
            {
                // Ex. : 
                // 1
                // 1.1

                // Est d'une profondeur supérieur à +1
                if (parts2.Length > parts1.Length + 1)
                    return false;

                // Ne démarre par à 1
                if (parts2[parts2.Length - 1] != 1)
                    return false;

                identicalParts = parts1.Length;
            }
            else if (parts1.Length > parts2.Length)
            {
                // Ex. : 
                // 1.1.2
                // 1.2
                identicalParts = parts2.Length - 1;

                // N'a pas la dernière part qui suit la précédente
                if (parts2[parts2.Length - 1] != parts1[parts2.Length - 1] + 1)
                    return false;
            }
            else
            {
                identicalParts = parts2.Length - 1;

                // N'a pas la dernière part qui est identique à la précédente
                if (parts2[parts1.Length - 1] != parts1[parts1.Length - 1] + 1)
                    return false;
            }

            for (int i = 0; i < identicalParts; i++)
                if (parts1[i] != parts2[i])
                    return false;

            return true;

        }

        /// <summary>
        /// Obtient le dernier frère qui précéde l'action spécifiée.
        /// </summary>
        /// <param name="actionWbs">Le WBS de l'action.</param>
        /// <param name="allActions">Toutes les actions.</param>
        /// <returns>
        /// Le dernier frère qui précéde l'action spécifiée.
        /// </returns>
        public static KAction GetLastPrecedingSibling(string actionWbs, IEnumerable<KAction> allActions)
        {
            var actions = allActions.OrderBy(a => a.WBSParts, _wbsComparer).ToList();
            var actionIndentation = IndentationFromWBS(actionWbs);

            var parts = GetParts(actionWbs);

            var lastPartUp = parts.Last() - 1;
            if (lastPartUp == 0)
                return null;

            int[] targetParts = new int[parts.Length];
            Array.ConstrainedCopy(parts, 0, targetParts, 0, targetParts.Length - 1);
            targetParts[targetParts.Length - 1] = lastPartUp;
            string targetWbs = LevelsToWBS(targetParts);

            return allActions.FirstOrDefault(a => a.WBS == targetWbs);
        }

        /// <summary>
        /// Obtient le dernier enfant de l'action spécifiée, au niveau n + 1.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="allActions">Toutes les actions. Les actions doivent être triées.</param>
        /// <returns>Le dernier enfant de l'action spécifiée, ou null si l'action n'a pas d'enfant</returns>
        public static KAction GetLastChild(KAction action, IEnumerable<KAction> allActions)
        {
            var children = GetDescendants(action, allActions);
            return children.LastOrDefault(c => IndentationFromWBS(c.WBS) == IndentationFromWBS(action.WBS) + 1);
        }

        

       //public static KAction GetNextActionByWBS(string WBS, IEnumerable<KAction> allActions)
       // {

       //     var split = WBS.Split('.');
       //     var count = WBS.Split('.').Count();

       //     var lastDigitWBS = Int32.Parse(split.ElementAt(count - 1));
       //     var newWBS = WBS.Replace(split.ElementAt(count - 1), lastDigitWBS.ToString());

       //     return allActions.Where(a => a.WBS == newWBS).FirstOrDefault();
       // }

        /// <summary>
        /// Obtient les frères qui suivent l'action spécifiée ainsi que leurs descendants.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="allActions">Toutes les actions.</param>
        /// <returns>Les frères qui suivent ainsi que leurs descendants.</returns>
        public static IEnumerable<KAction> GetSucessiveSliblingsAndDescendents(KAction action, IEnumerable<KAction> allActions) =>
            GetSucessiveSliblingsAndDescendents(action.WBS, allActions);

        /// <summary>
        /// Obtient les frères qui suivent l'action spécifiée ainsi que leurs descendants.
        /// </summary>
        /// <param name="actionWBS">Le WBS de l'action.</param>
        /// <param name="allActions">Toutes les actions.</param>
        /// <returns>Les frères qui suivent ainsi que leurs descendants.</returns>
        public static IEnumerable<KAction> GetSucessiveSliblingsAndDescendents(string actionWBS, IEnumerable<KAction> allActions)
        {
            // trier
            var actions = allActions.OrderBy(a => a.WBSParts, _wbsComparer).ToList();
            var actionIndentation = IndentationFromWBS(actionWBS);

            var parts = GetParts(actionWBS);

            foreach (var a in actions)
            {
                var aIndentation = IndentationFromWBS(a.WBS);

                if (aIndentation < actionIndentation)
                    continue;

                var aParts = GetParts(a.WBS);

                bool match = true;
                // Vérifier que toute la première partie correspond
                for (int i = 0; i < parts.Length - 1; i++)
                    match &= parts[i] == aParts[i];

                if (!match)
                    continue;

                // Vérfier que la dernière partie commune est bien après
                if (parts[parts.Length - 1] >= aParts[parts.Length - 1])
                    continue;

                yield return a;
            }

        }

        /// <summary>
        /// Détermine si les deux WBS spécifiés sont frères.
        /// </summary>
        /// <param name="wbs1">Le WBS1.</param>
        /// <param name="wbs2">Le WBS2.</param>
        /// <returns>
        /// <c>true</c> si les deux WBS sont frères.
        /// </returns>
        public static bool AreSiblings(string wbs1, string wbs2)
        {
            var parts1 = GetParts(wbs1);
            var parts2 = GetParts(wbs2);

            if (parts1.Length != parts2.Length)
                return false;

            for (int i = 0; i < parts1.Length - 1; i++)
            {
                var part1 = parts1[i];
                var part2 = parts2[i];

                if (part1 != part2)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Obtient toutes les actions qui sont au niveau d'indentation spécifié.
        /// </summary>
        /// <param name="indentationLevel">Le niveau d'indentation.</param>
        /// <param name="allActions">Toutes les actions.</param>
        /// <returns>Les actions qui sont au niveau d'indentation spécifié.</returns>
        public static IEnumerable<KAction> GetActionsAtIndentationLevel(int indentationLevel, IEnumerable<KAction> allActions) =>
            allActions.Where(action => IndentationFromWBS(action.WBS) == indentationLevel);

        /// <summary>
        /// Obtient toutes les actions qui sont au niveau d'indentation spécifié et ont les valeurs spécifiées pour les indentations précédentes.
        /// </summary>
        /// <param name="indentationLevel">Le niveau d'indentation.</param>
        /// <param name="firstLevelsValue">Les valeurs pour les premiers niveaux d'indentation.</param>
        /// <param name="allActions">Toutes les actions.</param>
        /// <returns>
        /// Les actions qui sont au niveau d'indentation spécifié.
        /// </returns>
        public static IEnumerable<KAction> GetActionsAtIndentationLevel(int indentationLevel, int[] firstLevelsValue, IEnumerable<KAction> allActions)
        {
            Assertion.AreEquals(firstLevelsValue.Length, indentationLevel);

            return allActions.Where(action =>
            {
                var isValid = true;
                isValid &= IndentationFromWBS(action.WBS) == indentationLevel;

                if (isValid)
                    for (int i = 0; i < firstLevelsValue.Length; i++)
                    {
                        isValid &= GetNumberAtLevel(action.WBS, i) == firstLevelsValue[i];
                    }

                return isValid;
            });
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

        /// <summary>
        /// Détermine si les deux WBS spécifiés ont les mêmes valeurs exceptées pour le dernier niveau d'indentation.
        /// </summary>
        /// <param name="wbs1">Le premier WBS.</param>
        /// <param name="wbs2">Le deuxième WBS.</param>
        public static bool StartsWithSameExceptLast(string wbs1, string wbs2)
        {
            Assertion.NotNull(wbs1, "wbs1");
            Assertion.NotNull(wbs2, "wbs2");

            var parts1 = GetParts(wbs1);
            var parts2 = GetParts(wbs2);

            if (parts1.Length != parts2.Length)
                return false;

            return StartsWith(parts1.Take(parts1.Length - 1).ToArray(), parts2.Take(parts2.Length - 1).ToArray());
        }

        /// <summary>
        /// Compare les WBS spécifiés.
        /// </summary>
        /// <param name="wbs1">le premier WBS.</param>
        /// <param name="wbs2">le second WBS.</param>
        /// <returns>Le résultat</returns>
        public static int Compare(string wbs1, string wbs2) =>
            WBSComparer.CompareInternal(GetParts(wbs1), GetParts(wbs2));

        /// <summary>
        /// Tries les actions spécifiées par leur WBS.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        /// <returns>Les actions triées</returns>
        public static IEnumerable<KAction> OrderByWBS(this IEnumerable<KAction> actions) =>
            actions.OrderBy(a => a.WBSParts, _wbsComparer);

        /// <summary>
        /// Compare des WBS.
        /// </summary>
        public class WBSComparer : IComparer<int[]>
        {
            /// <summary>
            /// Compare deux WBS.
            /// </summary>
            /// <param name="x">Le premier.</param>
            /// <param name="y">Le second.</param>
            /// <returns>Le résultat de la comparaison.</returns>
            public int Compare(int[] x, int[] y) =>
                CompareInternal(x, y);

            /// <summary>
            /// Compare deux WBS.
            /// </summary>
            /// <param name="x">Le premier.</param>
            /// <param name="y">Le second.</param>
            /// <returns>Le résultat de la comparaison.</returns>
            internal static int CompareInternal(int[] x, int[] y)
            {
                if (x == null && y != null)
                    return -1;
                if (x != null && y == null)
                    return 1;
                if (x == y)
                    return 0;

                var minLength = Math.Min(x.Length, y.Length);

                for (int i = 0; i < minLength; i++)
                {
                    if (x[i] > y[i])
                        return 1;
                    if (x[i] < y[i])
                        return -1;
                }

                // toute la première partie est égale
                if (x.Length != y.Length)
                    return x.Length > y.Length ? 1 : -1;

                return 0;
            }
        }

    }
}
