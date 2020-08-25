using KProcess.KL2.WebAdmin.Models.Action;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;


namespace KProcess.KL2.WebAdmin.Utils
{

        public interface IIdentable
        {
            string WBS { get; set; }   
            int[] WBSParts { get;  }
        }

        /// <summary>
        /// Fournit des méthodes d'aide à la gestion des WBS.
        /// </summary>
        public static class WBSWebActionUtil<T> where T : IIdentable
        {
            private const char _separator = '.';
            private const int _first = 1;
            private static readonly WBSComparer _wbsComparer = new WBSComparer();

            /// <summary>
            /// Obtient le niveau d'indentation pour le premier element dans un ensemble
            /// </summary>
            /// <returns></returns>
            public static int GetFirstIndentation()
            {
                return _first;
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
                else
                    return GetParts(wbs).Count() - 1;
            }

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
            /// Obtient le numéro du WBS au niveau spécifié.
            /// </summary>
            /// <param name="wbs">Le WBS.</param>
            /// <param name="level">Le niveau.</param>
            /// <returns></returns>
            public static int GetNumberAtLevel(string wbs, int level)
            {
                return GetParts(wbs)[level];
            }

            /// <summary>
            /// Détermine si l'action spécifiée contient des enfants.
            /// </summary>
            /// <param name="action">L'action.</param>
            /// <param name="others">Les autres actions.</param>
            /// <returns>
            ///   <c>true</c> si le WBS spécifié contient des enfants; sinon, <c>false</c>.
            /// </returns>
            public static bool HasChildren(T action, IEnumerable<T> others)
            {
                return HasChildren(action.WBS, others.Select(a => a.WBS));
            }

            /// <summary>
            /// Détermine si le WBS spécifiée contient des enfants.
            /// </summary>
            /// <param name="action">Le wbs.</param>
            /// <param name="others">Les autres wbs.</param>
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
            public static IEnumerable<T> GetDescendants(T action, IEnumerable<T> others)
            {
                return others.Where(a => !string.Equals(a.WBS, action.WBS) && StartsWith(a.WBSParts, action.WBSParts));
            }

            /// <summary>
            /// Obtient les enfants immédiats (indentation + 1) de l'action spécifiée.
            /// </summary>
            /// <param name="action">L'action.</param>
            /// <param name="others">The others.</param>
            /// <returns>Les enfants.</returns>
            public static IEnumerable<T> GetChildren(T action, IEnumerable<T> others)
            {
                return others.Where(a => !string.Equals(a.WBS, action.WBS) && StartsWith(a.WBSParts, action.WBSParts) &&
                    IndentationFromWBS(action.WBS) + 1 == IndentationFromWBS(a.WBS));
            }

            /// <summary>
            /// Obtient les parents et grand parents de l'enfant spécifié.
            /// </summary>
            /// <param name="child">L'action enfant.</param>
            /// <param name="others">Les autres actions.</param>
            /// <returns>
            /// Les parents.
            /// </returns>
            public static T[] GetParents(T child, IEnumerable<T> others)
            {
                return others
                    .Where(action => !string.Equals(child.WBS, action.WBS) && StartsWith(child.WBSParts, action.WBSParts))
                    .OrderBy(action => action.WBSParts, _wbsComparer)
                    .ToArray();
            }

            /// <summary>
            /// Obtient le parent de l'enfant spécifié.
            /// </summary>
            /// <param name="child">L'action enfant.</param>
            /// <param name="others">Les autres actions.</param>
            /// <returns>
            /// Le parent.
            /// </returns>
            public static T GetParent(T child, IEnumerable<T> others)
            {
                return others
                    .SingleOrDefault(action => string.Equals(Unindent(child.WBS), action.WBS));
            }

            /// <summary>
            /// Obtient le premier WBS disponible et suivant celui spécifié.
            /// </summary>
            /// <param name="wbs">Le WBS ou <c>null</c>.</param>
            /// <param name="otherWBS">Les autres WBS.</param>
            /// <returns>
            /// Le WBS suivant
            /// </returns>
            public static string GetNextAvailaibleWBS(string wbs, IEnumerable<string> otherWBS)
            {
                if (string.IsNullOrEmpty(wbs))
                    return "1";
                else
                {
                    var indentation = IndentationFromWBS(wbs);
                    var lastInSameLevel = otherWBS.Where(w => IndentationFromWBS(w) == indentation).Max();

                    var parts = GetParts(lastInSameLevel);
                    parts[parts.Length - 1] = parts[parts.Length - 1] + 1;
                    return LevelsToWBS(parts);
                }
            }

            /// <summary>
            /// Obtient un nouveau WBS à la fin de ceux fournis en argument.
            /// </summary>
            /// <param name="wbs">Les WBS.</param>
            /// <returns>Un nouveau WBS.</returns>
            public static string GetNewWBSAtEnd(IEnumerable<string> wbs)
            {
                var level1 = wbs.Where(w => IndentationFromWBS(w) == 0).OrderBy(w => w);
                return level1.Any() ? GetNextAvailaibleWBS(level1.Last(), wbs) : "1";
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
            public static string CopyNumberAtLevel(string sourceWbs, string targetWbs, int level)
            {
                return SetNumberAtLevel(targetWbs, level, GetNumberAtLevel(sourceWbs, level));
            }

            /// <summary>
            /// Calcule le décalage vertical entre deux éléments.
            /// </summary>
            /// <param name="firstWBS">Le premier WBS.</param>
            /// <param name="secondWbs">Le second WBS.</param>
            /// <returns>Le décalage</returns>
            public static int GetVerticalOffset(T action1, T action2, IEnumerable<T> allActions)
            {
                var actions = allActions.OrderBy(a => a.WBSParts, _wbsComparer).ToList();

                return actions.IndexOf(action2) - actions.IndexOf(action1);
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
            public static T GetLastPrecedingSibling(string actionWbs, IEnumerable<T> allActions)
            {
                var actions = allActions.OrderBy(a => a.WBSParts, _wbsComparer).ToList();
                var actionIndentation = IndentationFromWBS(actionWbs);

                var parts = GetParts(actionWbs);

                var lastPartUp = parts.Last() - 1;
                if (lastPartUp == 0)
                    return default(T);

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
            public static T GetLastChild(T action, IEnumerable<T> allActions)
            {
                var children = GetDescendants(action, allActions);
                return children.Where(c => IndentationFromWBS(c.WBS) == IndentationFromWBS(action.WBS) + 1).LastOrDefault();
            }



            //public static T GetNextActionByWBS(string WBS, IEnumerable<T> allActions)
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
            public static IEnumerable<T> GetSucessiveSliblingsAndDescendents(T action, IEnumerable<T> allActions)
            {
                return GetSucessiveSliblingsAndDescendents(action.WBS, allActions);
            }

            /// <summary>
            /// Obtient les frères qui suivent l'action spécifiée ainsi que leurs descendants.
            /// </summary>
            /// <param name="action">L'action.</param>
            /// <param name="allActions">Toutes les actions.</param>
            /// <returns>Les frères qui suivent ainsi que leurs descendants.</returns>
            public static IEnumerable<T> GetSucessiveSliblingsAndDescendents(string actionWBS, IEnumerable<T> allActions)
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
            public static IEnumerable<T> GetActionsAtIndentationLevel(int indentationLevel, IEnumerable<T> allActions)
            {
                return allActions.Where(action => IndentationFromWBS(action.WBS) == indentationLevel);
            }

            /// <summary>
            /// Obtient toutes les actions qui sont au niveau d'indentation spécifié et ont les valeurs spécifiées pour les indentations précédentes.
            /// </summary>
            /// <param name="indentationLevel">Le niveau d'indentation.</param>
            /// <param name="firstLevelsValue">Les valeurs pour les premiers niveaux d'indentation.</param>
            /// <param name="allActions">Toutes les actions.</param>
            /// <returns>
            /// Les actions qui sont au niveau d'indentation spécifié.
            /// </returns>
            public static IEnumerable<T> GetActionsAtIndentationLevel(int indentationLevel, int[] firstLevelsValue, IEnumerable<T> allActions)
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
            public static int Compare(string wbs1, string wbs2)
            {
                return WBSComparer.CompareInternal(GetParts(wbs1), GetParts(wbs2));
            }

            /// <summary>
            /// Tries les actions spécifiées par leur WBS.
            /// </summary>
            /// <param name="actions">Les actions.</param>
            /// <returns>Les actions triées</returns>
            public static IEnumerable<T> OrderByWBS(IEnumerable<T> actions)
            {
                return actions.OrderBy(a => a.WBSParts, _wbsComparer);
            }

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
                public int Compare(int[] x, int[] y)
                {
                    return CompareInternal(x, y);
                }

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
                    else if (x != null && y == null)
                        return 1;
                    else if (x == y)
                        return 0;
                    else
                    {
                        var minLength = Math.Min(x.Length, y.Length);

                        for (int i = 0; i < minLength; i++)
                        {
                            if (x[i] > y[i])
                                return 1;
                            else if (x[i] < y[i])
                                return -1;
                        }

                        // toute la première partie est égale
                        if (x.Length != y.Length)
                            return
                                x.Length > y.Length ? 1 : -1;

                        return 0;
                    }
                }
            }

        /// <summary>
        /// Ajoute un prédécesseur à l'action.
        /// </summary>
        /// <param name="allActions">Toutes les actions.</param>
        /// <param name="action">L'action successeur.</param>
        /// <param name="predecessor">Le prédécesseur.</param>
        /// <returns><c>true</c> si l'ajout est possible.</returns>
        public static bool AddPredecessor(List<PublishedAction> allActions, PublishedAction action, PublishedAction predecessor)
        {
            if (!CheckCanAddPredecessor(allActions, action, predecessor))
                return false;

            action.Predecessors.Add(predecessor);
            return true;
        }

        /// <summary>
        /// Vérifie si un prédecesseur peut être ajouté.
        /// </summary>
        /// <param name="allActions">Toutes les actions.</param>
        /// <param name="action">L'action successeur.</param>
        /// <param name="predecessor">L'action prédécesseur.</param>
        /// <returns>
        ///   <c>true</c> si l'ajout est possible.
        /// </returns>
        public static bool CheckCanAddPredecessor(List<PublishedAction> allActions, PublishedAction action, PublishedAction predecessor)
        {
            Debug.Assert(allActions != null);
            Debug.Assert(action != null);
            Debug.Assert(predecessor != null);

            // impossible qu'un groupe soit prédécesseur ou successeur.
            if (predecessor == null || action == null || allActions == null || predecessor.IsGroup || action.IsGroup)
                return false;

            // Déterminer si le lien créerait des références cycliques.
            var predecessorLinks = new List<(PublishedAction Predecessor, PublishedAction Successor)>();

            // Créer les liens entre éléments
            foreach (var a in allActions)
            {
                Debug.Assert(a.Predecessors != null);
                if (a.Predecessors != null)
                {
                    foreach (var p in a.Predecessors)
                        predecessorLinks.Add((p, a));
                }
            }

            predecessorLinks.Add((predecessor, action));

            while (predecessorLinks.Any())
            {
                var leafs = predecessorLinks.Where(tuple =>
                    !predecessorLinks.Select(p => p.Predecessor).Contains(tuple.Successor) ||
                    !predecessorLinks.Select(p => p.Successor).Contains(tuple.Predecessor)
                    ).ToList();
                if (leafs.Any())
                    predecessorLinks.RemoveRange(leafs);
                else
                    return false;
            }

            return true;
        }




        /// <summary>
        /// Met à jour les WBS en vue d'une insertion d'une action.
        /// </summary>
        /// <param name="allActions">Toutes les actions du scénario.</param>
        /// <param name="newAction">La nouvelle action.</param>
        /// <param name="parent">le parent de la nouvelle action ou <c>null</c> si indentation 0.</param>
        /// <param name="targetWbsLevel">Le niveau WBS souhaité. -1 pour l'ajouter à la fin du niveau.</param>
        public static void InsertUpdateWBS(List<T> allActions, T newAction, T parent,
            int targetWbsLevel = -1)
        {
            int insertIndentation;
            int[] finalWBS;

            if (parent != null)
                insertIndentation = IndentationFromWBS(parent.WBS) + 1;
            else
                insertIndentation = 0;

            // Prendre au maximum le niveau max possible en fonction des actions existantes
            var levelsFirstValues = parent != null ? GetParts(parent.WBS) : new int[] { };

            var atSameLevel = GetActionsAtIndentationLevel(insertIndentation, levelsFirstValues, allActions);
            if (atSameLevel.Any())
            {
                var max = atSameLevel.Max(a => GetParts(a.WBS).Last()) + 1;

                if (targetWbsLevel == -1)
                    targetWbsLevel = max;
                else
                    targetWbsLevel = Math.Min(targetWbsLevel, max);
            }

            if (targetWbsLevel == -1)
                targetWbsLevel = 1;

            if (parent != null)
            {
                var parentParts = GetParts(parent.WBS);
                finalWBS = parentParts.Concat(new int[] { targetWbsLevel }).ToArray();
            }
            else
                finalWBS = new int[] { targetWbsLevel };

            var previousAction = GetLastPrecedingSibling(LevelsToWBS(finalWBS), allActions);
            if (previousAction != null)
            {
                // Décaler vers le bas tous les éléments à partir de l'index spécifié
                var successiveSiblings = GetSucessiveSliblingsAndDescendents(previousAction.WBS, allActions);
                foreach (var a in successiveSiblings)
                {
                    var wbs = MoveDown(a.WBS, insertIndentation);
                    a.WBS = wbs;
                }
            }

            var w = LevelsToWBS(finalWBS);
            newAction.WBS = w;
        }



        /// <summary>
        /// Met à jour les WBS en vue d'une suppression d'une action.
        /// </summary>
        /// <param name="allActions">Toutes les actions du scénario.</param>
        /// <param name="action">L'action à supprimer.</param>
        public static void DeleteUpdateWBS(List<T> allActions, T action)
        {
            //var actionIndex = Array.IndexOf(allActions, action);
            var actionIndex = allActions.IndexOf(action);

            var actionIndentation = IndentationFromWBS(action.WBS);

            var nextActionIndex = actionIndex + 1;
            //if (nextActionIndex < allActions.Length)
            if (nextActionIndex < allActions.Count)
            {
                var nextAction = allActions[nextActionIndex];
                if (nextAction != null)
                {
                    var nextItemIndentation = IndentationFromWBS(nextAction.WBS);

                    // Déterminer si à l'index + 1, l'élément pourra garder son indentation
                    // Les else if ont un ordre bien particulier à respecter
                    bool shouldDecreaseIndentation;
                    if (nextItemIndentation == 0)
                        shouldDecreaseIndentation = false;
                    else if (actionIndex - 1 < 0)
                        shouldDecreaseIndentation = true;
                    else if (nextItemIndentation <= actionIndentation)
                        shouldDecreaseIndentation = false;
                    else
                        shouldDecreaseIndentation = true;

                    // Désindenter l'action suivant l'action à supprimer
                    if (shouldDecreaseIndentation)
                        Unindent(allActions, nextAction, UnindentationBehavior.PutInline);
                }
            }

            var children = GetDescendants(action, allActions).ToArray();
            var successiveSiblings = GetSucessiveSliblingsAndDescendents(action, allActions).ToArray();

            // On détermine, pour les enfants actuels, à partir de combien il faudra reprendre la numérotation
            int startIndex = 0;
            var nextParent = GetLastPrecedingSibling(action.WBS, allActions);
            if (nextParent != null)
            {
                var lastChild = GetLastChild(nextParent, allActions);
                if (lastChild != null)
                    startIndex = GetNumberAtLevel(lastChild.WBS, actionIndentation + 1);
            }

            foreach (var child in children)
            {
                var wbs = MoveUp(child.WBS, actionIndentation);
                wbs = SetNumberAtLevel(wbs, actionIndentation + 1, GetNumberAtLevel(wbs, actionIndentation + 1) + startIndex);

                child.WBS = wbs;
            }

            foreach (var sibling in successiveSiblings)
            {
                var wbs = MoveUp(sibling.WBS, actionIndentation);
                sibling.WBS = wbs;
            }
        }

        /// <summary>
        /// Désindente l'action spécifié.
        /// Elle n'est pas censée avoir d'enfants.
        /// </summary>
        /// <param name="allActions">Les actions.</param>
        /// <param name="action">L'action.</param>
        /// <param name="behavior">The behavior.</param>
        public static void Unindent(List<T> allActions, T action, UnindentationBehavior behavior)
        {
            var actionIndentation = IndentationFromWBS(action.WBS);

            // toutes les actions qui sont au niveau de l'élément mais en dessous, doivent être remontées d'un cran

            T[] futureSiblings = null;

            switch (behavior)
            {
                case UnindentationBehavior.PutAbove:

                    var successiveSiblings = GetSucessiveSliblingsAndDescendents(action, allActions);
                    foreach (var a in successiveSiblings)
                        a.WBS = MoveUp(a.WBS, actionIndentation);

                    action.WBS = Unindent(action.WBS);
                    break;

                case UnindentationBehavior.PutBelow:

                    successiveSiblings = GetSucessiveSliblingsAndDescendents(action, allActions);
                    foreach (var a in successiveSiblings)
                        a.WBS = MoveUp(a.WBS, actionIndentation);

                    action.WBS = Unindent(action.WBS);
                    action.WBS = MoveDown(action.WBS, actionIndentation - 1);
                    break;

                case UnindentationBehavior.PutInline:

                    var children = GetDescendants(action, allActions).ToArray();

                    var successiveSiblingsCache = GetSucessiveSliblingsAndDescendents(action, allActions).ToArray();

                    var actionWBSExceptLastPart = GetParts(action.WBS);
                    actionWBSExceptLastPart = actionWBSExceptLastPart.Take(actionWBSExceptLastPart.Length - 2).ToArray();

                    futureSiblings = allActions.Where(a =>
                        IndentationFromWBS(a.WBS) >= actionIndentation - 1 &&
                        GetNumberAtLevel(a.WBS, actionIndentation - 1) > GetNumberAtLevel(action.WBS, actionIndentation - 1) &&
                        StartsWith(GetParts(a.WBS), actionWBSExceptLastPart))
                        .ToArray();

                    action.WBS = Unindent(action.WBS);
                    action.WBS = MoveDown(action.WBS, actionIndentation - 1);

                    // Pour tous les enfants, désindenter
                    // Ex: la désindentation de T2 entraîne la désindentation de T21 et T22

                    //  G1 1
                    //      T1 1.1
                    //      T2 1.2
                    //          T21 1.2.1
                    //          T22 1.2.2
                    //      T3 1.3

                    foreach (var child in children)
                    {
                        child.WBS = Unindent(child.WBS, IndentationFromWBS(action.WBS) + 1);
                        for (int i = 0; i < IndentationFromWBS(action.WBS) + 1; i++)
                            child.WBS = CopyNumberAtLevel(action.WBS, child.WBS, i);
                    }

                    int upDownDiff = -1;
                    if (successiveSiblingsCache.Any())
                    {
                        var firstSibling = successiveSiblingsCache.First();
                        var childrenIndentation = IndentationFromWBS(action.WBS) + 1;
                        if (children.Any())
                        {
                            var lastChildWithSameIndentation = children.LastOrDefault(c =>
                                IndentationFromWBS(c.WBS) == childrenIndentation);
                            var expectedNumber = GetNumberAtLevel(lastChildWithSameIndentation.WBS, childrenIndentation) + 1;
                            upDownDiff = expectedNumber - GetNumberAtLevel(firstSibling.WBS, childrenIndentation);
                        }
                        else
                            upDownDiff = 1 - GetNumberAtLevel(firstSibling.WBS, childrenIndentation);
                    }

                    // En suivant l'exemple précédent, il faut que T3 suive T21 et T22
                    foreach (var a in successiveSiblingsCache)
                    {
                        if (upDownDiff < 0)
                            for (int i = 0; i < Math.Abs(upDownDiff); i++)
                                a.WBS = MoveUp(a.WBS, actionIndentation);
                        else
                            for (int i = 0; i < Math.Abs(upDownDiff); i++)
                                a.WBS = MoveDown(a.WBS, actionIndentation);

                        a.WBS = MoveDown(a.WBS, actionIndentation - 1);
                    }

                    break;

                default:
                    break;
            }

            // L'action prennant un WBS, tous les WBS en dessous doivent être modifiés
            if (behavior == UnindentationBehavior.PutInline)
                IncreaseWBSFor(actionIndentation - 1, 1, futureSiblings);
            else
            {
                IncreaseWbsForAllDown(allActions, actionIndentation - 1, GetNumberAtLevel(action.WBS, actionIndentation - 1), 1);
                actionIndentation--;
                action.WBS = MoveUp(action.WBS, actionIndentation);
            }
        }

        /// <summary>
        /// Augmente le WBS pour tous les éléments spécifiés.
        /// </summary>
        /// <param name="level">Le niveau d'indentation.</param>
        /// <param name="increment">L'incrément.</param>
        /// <param name="targettedActions">Les actions cibles.</param>
        private static void IncreaseWBSFor(int level, int increment, IEnumerable<T> targettedActions)
        {
            foreach (var action in targettedActions)
            {
                if (increment < 0)
                {
                    for (int i = 0; i > increment; i--)
                        action.WBS = MoveUp(action.WBS, level);
                }
                else
                {
                    for (int i = 0; i < increment; i++)
                        action.WBS = MoveDown(action.WBS, level);
                }
            }
        }

        /// <summary>
        /// Augmente le WBS pour tous les éléments se trouvant en dessous de celui spécifié.
        /// </summary>
        /// <param name="level">Le niveau d'indentation.</param>
        /// <param name="start">Le début.</param>
        /// <param name="increment">L'incrément.</param>
        private static void IncreaseWbsForAllDown(List<T> allActions, int level, int start, int increment)
        {
            var targettedActions = allActions.Where(a =>
                IndentationFromWBS(a.WBS) >= level && GetNumberAtLevel(a.WBS, level) >= start);
            IncreaseWBSFor(level, increment, targettedActions);
        }

    }

}