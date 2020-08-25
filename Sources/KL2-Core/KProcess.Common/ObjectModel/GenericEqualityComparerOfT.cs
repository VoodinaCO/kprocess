using System;
using System.Collections.Generic;

namespace KProcess
{
    /// <summary>
    /// Un comparateur d'égalité générique.
    /// </summary>
    /// <typeparam name="T">Le type de l'objet à comparer.</typeparam>
    public class GenericEqualityComparer<T> : EqualityComparer<T>
    {
        /// <summary>
        /// Obtient ou définit la fonction qui détermine si deux objets sont égaux.
        /// </summary>
        public Func<T, T, bool> EqualityFunction { get; set; }

        /// <summary>
        /// Obtient ou définit la fonction qui obtient l'HashCode d'un objet.
        /// </summary>
        public Func<T, int> HashCodeFunction { get; set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="GenericEqualityComparer&lt;T&gt;"/>.
        /// </summary>
        /// <param name="equalityFunction">la fonction qui détermine si deux objets sont égaux.</param>
        /// <param name="hashCodeFunction">la fonction qui obtient l'HashCode d'un objet.</param>
        public GenericEqualityComparer(Func<T, T, bool> equalityFunction, Func<T, int> hashCodeFunction)
        {
            this.EqualityFunction = equalityFunction;
            this.HashCodeFunction = hashCodeFunction;
        }

        /// <inheritdoc />
        public override bool Equals(T x, T y)
        {
            return EqualityFunction(x, y);
        }

        /// <inheritdoc />
        public override int GetHashCode(T obj)
        {
            return HashCodeFunction(obj);
        }
    }
}
