using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace KProcess
{
    /// <summary>
    /// Regroupe les méthodes d'extension de la classe IEnumerable.
    /// </summary>
    public static class EnumerableExt
    {
        #region Méthodes publiques

        /// <summary>
        /// Met à plat un arbre
        /// </summary>
        /// <typeparam name="T">type d'objet</typeparam>
        /// <param name="value">enumerable à mettre à plat</param>
        /// <param name="childrenSelector">méthode récupérant les enfants</param>
        /// <returns>la liste mise à plat</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> value, Func<T, IEnumerable<T>> childrenSelector)
        {
            var result = value;

            if (value != null)
            {
                foreach (T element in value)
                {
                    var children = childrenSelector(element).Flatten(childrenSelector);

                    if (children != null)
                        result = result.Concat(children);
                }
            }

            return result;
        }

        /// <summary>
        /// Convertit les éléments
        /// </summary>
        /// <typeparam name="TInput">type d'objet</typeparam>
        /// <typeparam name="TOutput">type d'objet en sortie</typeparam>
        /// <param name="value">enumerable à convertir</param>
        /// <param name="action"></param>
        /// <returns>enumerable des éléments convertis</returns>
        public static IEnumerable<TOutput> Convert<TInput, TOutput>(this IEnumerable<TInput> value, Func<TInput, TOutput> action)
        {
            Assertion.NotNull(value, "value");
            Assertion.NotNull(action, "action");

            foreach (TInput item in value)
                yield return action(item);
        }

        /// <summary>
        /// Détermine si une énumération est vide
        /// </summary>
        /// <param name="pValue">Enumeration à tester</param>
        /// <returns>True si l'énumération est null ou vide, False si elle contient au moins un élément</returns>
        public static bool IsEmpty(this IEnumerable pValue)
        {
            if (object.ReferenceEquals(pValue, null))
                return true;

            return !pValue.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// Retourne les élément de la liste séparés par une virgule
        /// </summary>
        /// <param name="list">liste à inspecter</param>
        /// <returns>liste séparés par une virgule</returns>
        public static string ToStringList(this IEnumerable list)
        {
            if (list.IsEmpty())
                return String.Empty;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (var item in list)
                sb.AppendFormat("{0}, ", item);

            sb.Remove(sb.Length - 2, 2);
            return sb.ToString();
        }

        /// <summary>
        /// Crée une BulkObservableCollection à partir d'un IEnumerable.
        /// </summary>
        /// <typeparam name="T">Le type générique.</typeparam>
        /// <param name="enumeration">L'enumeration.</param>
        /// <returns>La BulkObservableCollection.</returns>
        public static BulkObservableCollection<T> ToBulkObservableCollection<T>(this IEnumerable<T> enumeration)
        {
            return new BulkObservableCollection<T>(enumeration);
        }

        /// <summary>
        /// Crée une ObservableCollection à partir d'un IEnumerable.
        /// </summary>
        /// <typeparam name="T">Le type générique.</typeparam>
        /// <param name="enumeration">L'enumeration.</param>
        /// <returns>L'ObservableCollection.</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumeration)
        {
            return new ObservableCollection<T>(enumeration);
        }

        /// <summary>
        /// Obtient l'objet source dont la valeur récupérée par le <paramref name="valueSelector"/> est la minimale de l'énumération.
        /// </summary>
        /// <typeparam name="TSource">Le type de l'objet source.</typeparam>
        /// <typeparam name="TValue">Le type de l'objet valeur.</typeparam>
        /// <param name="source">L'énumeration source.</param>
        /// <param name="valueSelector">Le délégué capable de sélectionner la valeur.</param>
        /// <returns>L'objet source dont la valeur spécifiée est la minimale.</returns>
        public static TSource MinWithValue<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, TValue> valueSelector)
            where TValue : IComparable<TValue>
        {

            TValue maxValue = default(TValue);
            TSource maxSource = default(TSource);

            bool hasGoneOnce = false;

            foreach (var currentSource in source)
            {
                var value = valueSelector(currentSource);
                if (hasGoneOnce)
                {
                    if (value.CompareTo(maxValue) < 0)
                    {
                        maxValue = value;
                        maxSource = currentSource;
                    }
                }
                else
                {
                    maxValue = value;
                    maxSource = currentSource;
                    hasGoneOnce = true;
                }
            }

            if (!hasGoneOnce)
            {
                throw new ArgumentException("La source ne doit pas être vide.");
            }

            return maxSource;
        }

        /// <summary>
        /// Obtient l'objet source dont la valeur récupérée par le <paramref name="valueSelector"/> est la maximale de l'énumération.
        /// </summary>
        /// <typeparam name="TSource">Le type de l'objet source.</typeparam>
        /// <typeparam name="TValue">Le type de l'objet valeur.</typeparam>
        /// <param name="source">L'énumeration source.</param>
        /// <param name="valueSelector">Le délégué capable de sélectionner la valeur.</param>
        /// <returns>L'objet source dont la valeur spécifiée est la maximale.</returns>
        public static TSource MaxWithValue<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, TValue> valueSelector)
            where TValue : IComparable<TValue>
        {

            TValue maxValue = default(TValue);
            TSource minSource = default(TSource);

            bool hasGoneOnce = false;

            foreach (var currentSource in source)
            {
                var value = valueSelector(currentSource);
                if (hasGoneOnce)
                {
                    if (value.CompareTo(maxValue) > 0)
                    {
                        maxValue = value;
                        minSource = currentSource;
                    }
                }
                else
                {
                    maxValue = value;
                    minSource = currentSource;
                    hasGoneOnce = true;
                }
            }

            if (!hasGoneOnce)
            {
                throw new ArgumentException("La source ne doit pas être vide.");
            }

            return minSource;
        }

        /// <summary>
        /// Retourne l'énumération spécifiée sauf les éléments contenu dans others.
        /// </summary>
        /// <typeparam name="TSource">Le type de la donnée.</typeparam>
        /// <param name="source">L'énumération source.</param>
        /// <param name="others">Les autres éléments à exclure.</param>
        /// <returns>
        /// Une énumération contenant l'énumération avec les éléments others exclus.
        /// </returns>
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> source, params TSource[] others)
        {
            foreach (var item in source)
            {
                if (!others.Contains(item))
                    yield return item;
            }
        }

        /// <summary>
        /// Retourne une énumération ne contenant que les éléments spécifiés.
        /// </summary>
        /// <typeparam name="T">type d'éléments</typeparam>
        /// <param name="values">Les valeurs.</param>
        /// <returns>Une énumération ne contenant que les éléments spécifiés</returns>
        public static IEnumerable<T> Concat<T>(params T[] values)
        {
            foreach (var val in values)
                yield return val;
        }

        /// <summary>
        /// Concatène l'énumération avec les éléments spécifiés.
        /// </summary>
        /// <typeparam name="TSource">Le type de la donnée.</typeparam>
        /// <param name="source">L'énumération source.</param>
        /// <param name="others">Les autres éléments.</param>
        /// <returns>
        /// Une énumération contenant l'énumération et les éléments spécifiés.
        /// </returns>
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> source, params TSource[] others)
        {
            foreach (var item in source)
                yield return item;

            foreach (var other in others)
                yield return other;
        }

        /// <summary>
        /// Concatène les éléments spécifiés avec à la suite l'énumération.
        /// </summary>
        /// <typeparam name="TSource">Le type de la donnée.</typeparam>
        /// <param name="source">L'énumération source.</param>
        /// <param name="others">Les autres éléments.</param>
        /// <returns>
        /// Une énumération contenant l'énumération et les éléments spécifiés.
        /// </returns>
        public static IEnumerable<TSource> ConcatBefore<TSource>(this IEnumerable<TSource> source, params TSource[] others)
        {
            foreach (var other in others)
                yield return other;

            foreach (var item in source)
                yield return item;
        }

        /// <summary>
        /// Concatène les énumérations spécifiées.
        /// </summary>
        /// <typeparam name="TSource">Le type de la donnée.</typeparam>
        /// <param name="source">L'énumération source.</param>
        /// <param name="others">Les autres énumérations.</param>
        /// <returns>Une énumération contenant toutes les énumérations dans l'ordre spécifié.</returns>
        public static IEnumerable<TSource> Concat<TSource>(this IEnumerable<TSource> source, params IEnumerable<TSource>[] others)
        {
            if (source != null)
                foreach (var item in source)
                    yield return item;

            foreach (var other in others)
                if (other != null)
                    foreach (var item in other)
                        yield return item;
        }

        /// <summary>
        /// Retourne l'index d'un élément dans une énumération, ou -1 s'il n'y est pas.
        /// </summary>
        /// <typeparam name="TSource">Le type de la donnée.</typeparam>
        /// <param name="source">L'énumération source.</param>
        /// <param name="value">L'élément à rechercher.</param>
        /// <returns>
        /// L'index de l'élément ou -1 s'il n'a pas été trouvé.
        /// </returns>
        public static int IndexOf<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            int index = 0;
            foreach (object item in source)
            {
                if (object.ReferenceEquals(value, item) || value.Equals(item))
                    return index;
                index++;
            }
            return -1;
        }

        #endregion
    }
}
