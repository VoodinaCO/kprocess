using System.Collections.Generic;
using System.Data.Entity.Core.Objects;

namespace KProcess.Data
{
    /// <summary>
    /// Contient des méthodes d'extension pour les ObjectQuery.
    /// </summary>
    public static class ObjectQueryExt
    {

        /// <summary>
        /// Convertit un IEnumerable en ObjectQuery.
        /// Utile par exemple après un Where avec une lambda.
        /// </summary>
        /// <typeparam name="T">Le type de donnée</typeparam>
        /// <param name="query">La requête.</param>
        /// <returns>L'ObjectQuery</returns>
        public static ObjectQuery<T> AsObjectQuery<T>(this IEnumerable<T> query)
        {
            return (ObjectQuery<T>)query;
        }

    }
}
