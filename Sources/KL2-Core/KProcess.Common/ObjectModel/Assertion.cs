//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : Assert.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Regroupe des méthodes d'assertion.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*   
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace KProcess
{
    /// <summary>
    /// Regroupe des méthodes d'assertion.
    /// </summary>
    public static class Assertion
    {
        #region Méthodes publiques

        /// <summary>
        /// Vérifie si value est null.
        /// </summary>
        /// <typeparam name="T">Type de l'élément à vérifier</typeparam>
        /// <param name="value">Instance à vérifier</param>
        /// <param name="parameterName">Nom du paramètres.</param>
        /// <exception cref="ArgumentNullException">Si value est null.</exception>
        [DebuggerStepThrough]
        public static void NotNull<T>(T value, string parameterName)
            where T : class
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Vérifie si la chaîne est null ou vide.
        /// </summary>
        /// <param name="value">Chaîne à vérifier.</param>
        /// <param name="parameterName">Nom du paramètre.</param>
        /// <exception cref="ArgumentException">Si value est null ou vide.</exception>
        [DebuggerStepThrough]
        public static void NotNullOrEmpty(string value, string parameterName)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("Value cannot be null or empty", parameterName);
        }

        /// <summary>
        /// Vérifie si la collection ou ses éléments sont null.
        /// </summary>
        /// <typeparam name="T">Type des éléments de la collection</typeparam>
        /// <param name="values">Collection à vérifier</param>
        /// <param name="parameterName">Nom du paramètre</param>
        /// <exception cref="ArgumentNullException">Si value ou un de ses éléments est null.</exception>
        [DebuggerStepThrough]
        public static void NotNullOrNullElements<T>(IEnumerable<T> values, string parameterName)
            where T : class
        {
            NotNull(values, parameterName);
            NotNullElements(values, parameterName);
        }

        /// <summary>
        /// Vérifie si les éléments de la collection sont null.
        /// </summary>
        /// <typeparam name="TKey">Type des clés.</typeparam>
        /// <typeparam name="TValue">Type des valeurs.</typeparam>
        /// <param name="values">Collection à vérifier.</param>
        /// <param name="parameterName">Nom du paramètre.</param>
        /// <exception cref="ArgumentNullException">Si value ou un de ses éléments est null.</exception>
        [DebuggerStepThrough]
        public static void NullOrNotNullElements<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> values, string parameterName)
            where TKey : class
            where TValue : class
        {
            if (values != null)
                NotNullElements(values, parameterName);
        }

        /// <summary>
        /// Vérifie si les éléments de la collection sont null.
        /// </summary>
        /// <typeparam name="T">Type des éléments de la collection</typeparam>
        /// <param name="values">Collection à vérifier</param>
        /// <param name="parameterName">Nom du paramètre.</param>
        [DebuggerStepThrough]
        public static void NullOrNotNullElements<T>(IEnumerable<T> values, string parameterName)
            where T : class
        {
            if (values != null)
                NotNullElements(values, parameterName);
        }

        /// <summary>
        /// Méthode permettant de déterminer si deux objets sont égaux
        /// </summary>
        /// <param name="pObj1">Objet à comparer</param>
        /// <param name="pObj2">Second objet à comparer</param>
        /// <returns>True si les deux objets à comparer sont égaux</returns>
        [DebuggerStepThrough]
        public static bool AreEquals(object pObj1, object pObj2)
        {
            bool obj1IsNull = object.ReferenceEquals(pObj1, null);
            bool obj2IsNull = object.ReferenceEquals(pObj2, null);

            // Deux objets nulls, OK
            if (obj1IsNull && obj2IsNull)
                return true;

            // Deux objets non nulls :
            if (!obj1IsNull && !obj2IsNull)
            {
                // Utiliser IComparable si possible, sinon utiliser Equals
                if (pObj1 is IComparable)
                    return ((IComparable)pObj1).CompareTo(pObj2) == 0;
                else if (pObj2 is IComparable)
                    return ((IComparable)pObj2).CompareTo(pObj1) == 0;
                else
                    return pObj1.Equals(pObj2);
            }

            return false;
        }

        #endregion

        #region Méthodes privées

        private static void NotNullElements<T>(IEnumerable<T> values, string parameterName)
            where T : class
        {
            if (values.Any(v => v == null))
                throw new ArgumentNullException(parameterName, "Value cannot contain null values");
        }

        private static void NotNullElements<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> values, string parameterName)
            where TKey : class
            where TValue : class
        {
            if (values.Any(kv => kv.Key == null || kv.Value == null))
                throw new ArgumentNullException(parameterName, "Value cannot contain null values");
        }

        #endregion
    }
}
