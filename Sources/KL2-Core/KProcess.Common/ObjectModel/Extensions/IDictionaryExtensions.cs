using System.Collections.Generic;

namespace KProcess
{
    /// <summary>
    /// Regroupe les méthodes d'extension de la classe IList.
    /// </summary>
    public static class IDictionaryExtensions
    {
        #region Méthodes publiques

        /// <summary>
        /// Supprime tous les éléments de la liste
        /// </summary>
        /// <typeparam name="T">type d'éléments</typeparam>
        /// <param name="value">liste à modifier</param>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
        {
            if (dict.TryGetValue(key, out TValue result))
                return result;
            return defaultValue;
        }

        #endregion
    }
}
