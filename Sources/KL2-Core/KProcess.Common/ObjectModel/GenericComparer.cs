using System;
using System.Collections.Generic;
using System.Linq;

namespace KProcess.Common
{
    /// <summary>
    /// Fournit des méthodes génériques et réutilisables de comparaisons.
    /// </summary>
    public static class GenericComparer
    {

        /// <summary>
        /// Compare deux instances, d'abord par leur nullité et ensuite par une de leur propriété.
        /// </summary>
        /// <typeparam name="TInstance">Le type de l'instance.</typeparam>
        /// <typeparam name="TData">Le type de la donnée dans l'instance.</typeparam>
        /// <param name="instance1">La première instance.</param>
        /// <param name="instance2">La deuxième instance.</param>
        /// <param name="propertyGetter">Le délégué capable de récupérer la propriété sur une instance.</param>
        /// <returns>Le résultat de la comparaison</returns>
        public static int CompareTo<TInstance, TData>(this TInstance instance1, TInstance instance2, Func<TInstance, TData> propertyGetter)
            where TInstance : class
            where TData : IComparable
        {

            if (instance1 == null && instance2 != null)
                return -1;

            if (instance1 != null && instance2 == null)
                return 1;

            if (instance1 == null && instance2 == null)
                return 0;

            return propertyGetter(instance1).CompareTo(propertyGetter(instance2));
        }

        /// <summary>
        /// Compare deux séquence, en vérifiant d'abord leur nullité.
        /// </summary>
        /// <typeparam name="TData">Le type de la donnée dans l'instance.</typeparam>
        /// <param name="first">La première énumération.</param>
        /// <param name="second">La deuxième énumération.</param>
        /// <returns>
        /// Le résultat de la comparaison
        /// </returns>
        public static bool SequenceEqualWithNull<TData>(this IEnumerable<TData> first, IEnumerable<TData> second)
        {

            if (first == null && second != null)
                return false;

            if (first != null && second == null)
                return false;

            return Enumerable.SequenceEqual(first, second);
        }
    }
}
