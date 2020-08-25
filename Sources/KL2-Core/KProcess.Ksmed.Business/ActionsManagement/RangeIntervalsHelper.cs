using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Business.ActionsManagement
{
    /// <summary>
    /// Fournit des méthodes d'aide à la gestion d'intervalles.
    /// </summary>
    public static class RangeHelper
    {

        /// <summary>
        /// Représente une intervalle.
        /// </summary>
        /// <typeparam name="TData">Le type de la donnée.</typeparam>
        public class Range<TData>
        {
            /// <summary>
            /// Obtient ou définit le début.
            /// </summary>
            public TData Start { get; set; }

            /// <summary>
            /// Obtient ou définit la fin.
            /// </summary>
            public TData End { get; set; }
        }

        /// <summary>
        /// Effectue une union sur les intervalles fournies.
        /// </summary>
        /// <typeparam name="TData">Le type de la donnée comparée.</typeparam>
        /// <typeparam name="TModel">Le type de l'objet contenant la donnée.</typeparam>
        /// <param name="intervals">Les intervalles.</param>
        /// <param name="startGetter">Un délégué capable de récupérer la début de l'intervalle.</param>
        /// <param name="endGetter">Un délégué capable de récupérer la fin de l'intervalle.</param>
        /// <param name="comparer">Un comparateur entre les données.</param>
        /// <returns>Des intervalles.</returns>
        public static IEnumerable<Range<TData>> Union<TData, TModel>(this IEnumerable<TModel> intervals, Func<TModel, TData> startGetter, Func<TModel, TData> endGetter, IComparer<TData> comparer = null)
        {
            if (intervals == null || !intervals.Any())
                yield break;

            if (comparer == null)
                comparer = Comparer<TData>.Default;

            var orderdList = intervals.OrderBy(r => startGetter(r));
            var firstRange = orderdList.First();

            TData min = startGetter(firstRange);
            TData max = endGetter(firstRange);

            foreach (var current in orderdList.Skip(1))
            {
                if (comparer.Compare(endGetter(current), max) > 0 && comparer.Compare(startGetter(current), max) > 0)
                {
                    yield return Create(min, max);
                    min = startGetter(current);
                }
                max = comparer.Compare(max, endGetter(current)) > 0 ? max : endGetter(current);
            }
            yield return Create(min, max);
        }

        /// <summary>
        /// Crée une intervalle.
        /// </summary>
        /// <typeparam name="TData">Le type de la donnée.</typeparam>
        /// <param name="start">Le début.</param>
        /// <param name="end">La fin.</param>
        /// <returns>L'intervalle créée.</returns>
        private static Range<TData> Create<TData>(TData start, TData end)
        {
            return new Range<TData> { Start = start, End = end };
        }

    }
}
