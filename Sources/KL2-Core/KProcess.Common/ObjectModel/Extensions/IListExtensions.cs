//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : IListExtensions.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Regroupe les méthodes d'extension de la classe IList.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*   
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace KProcess
{
    /// <summary>
    /// Regroupe les méthodes d'extension de la classe IList.
    /// </summary>
    public static class IListExtensions
    {
        #region Méthodes publiques

        /// <summary>
        /// Supprime tous les éléments de la liste
        /// </summary>
        /// <typeparam name="T">type d'éléments</typeparam>
        /// <param name="value">liste à modifier</param>
        public static void RemoveAll<T>(this IList<T> value)
        {
            Assertion.NotNull(value, "value");

            while (value.Count > 0)
                value.RemoveAt(0);
        }

        /// <summary>
        /// Retourne l'index du premier élément répondant au prédicat
        /// </summary>
        /// <typeparam name="T">type d'éléments</typeparam>
        /// <param name="value">liste à rechercher</param>
        /// <param name="predicate">prédicat de recherche</param>
        /// <returns>l'index du premier élément répondant au prédicat, -1 si aucun</returns>
        public static int IndexOf<T>(this IList<T> value, Func<T, bool> predicate)
        {
            Assertion.NotNull(value, "predicate");
            
            for (int i = 0; i < value.Count; i++)
            {
                if (predicate(value[i]))
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Remplace ou ajoute un élément de la liste répondant à un prédicat par un autre
        /// </summary>
        /// <typeparam name="T">type d'éléments</typeparam>
        /// <param name="value">liste à modifier</param>
        /// <param name="predicate">prédicat de recherche</param>
        /// <param name="newItem">élément de remplacement</param>
        public static void ReplaceWith<T>(this IList<T> value, Func<T, bool> predicate, T newItem)
        {
            Assertion.NotNull(value, "predicate");

            int index = value.IndexOf(predicate);

            if (index == -1)
                value.Add(newItem);
            else
            {
                value.RemoveAt(index);
                value.Insert(index, newItem);
            }
        }

        #endregion
    }
}
