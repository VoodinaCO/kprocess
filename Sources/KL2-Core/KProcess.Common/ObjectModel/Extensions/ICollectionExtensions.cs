//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : IEnumerableExtensions.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Regroupe les méthodes d'extension de la classe ICollection.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*   
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace KProcess
{
    /// <summary>
    /// Regroupe les méthodes d'extension de la classe ICollection.
    /// </summary>
    public static class ICollectionExtensions
    {
        #region Méthodes publiques

        /// <summary>
        /// Ajoute tous les éléments à la collection
        /// </summary>
        /// <typeparam name="T">type d'éléments</typeparam>
        /// <param name="value">collection à modifier</param>
        /// <param name="items">éléments à ajouter à la collection</param>
        public static void AddRange<T>(this ICollection<T> value, IEnumerable<T> items)
        {
            Assertion.NotNull(value, "value");
            if (items == null)
                return;

            foreach (var item in items)
                value.Add(item);
        }

        /// <summary>
        /// Ajoute tous les éléments à la collection uniquement s'ils n'y sont pas encore.
        /// </summary>
        /// <typeparam name="T">type d'éléments</typeparam>
        /// <param name="collection">collection à modifier.</param>
        /// <param name="items">éléments à ajouter à la collection.</param>
        public static void AddRangeNew<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            Assertion.NotNull(collection, "collection");
            Assertion.NotNull(items, "items");

            foreach (var item in items)
                collection.AddNew(item);
        }

        /// <summary>
        /// Ajoute l'élément à la collection uniquement s'il n'y est pas encore.
        /// </summary>
        /// <typeparam name="T">type d'éléments</typeparam>
        /// <param name="collection">collection à modifier.</param>
        /// <param name="item">élément à ajouter à la collection.</param>
        /// <returns><c>true</c> si l'ajout a eu lieu</returns>
        public static bool AddNew<T>(this ICollection<T> collection, T item)
        {
            Assertion.NotNull(collection, "collection");

            if (!collection.Contains(item))
            {
                collection.Add(item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Obtient la liste des éléments ajoutés à la collection <paramref name="pSource"/> pour obtenir la collection <paramref name="pCible"/>
        /// </summary>
        /// <typeparam name="TSource">Type des éléments de la collection <paramref name="pSource"/></typeparam>
        /// <typeparam name="TCible">Type des éléments de la collection <paramref name="pCible"/></typeparam>
        /// <param name="pSource">Collection source</param>
        /// <param name="pCible">Collection cible</param>
        /// <returns>Enumération des éléments présents dans <paramref name="pCible"/> qui ne sont pas présents dans <paramref name="pSource"/></returns>
        public static IEnumerable<TCible> GetAddedItems<TSource, TCible>(this ICollection<TSource> pSource, ICollection<TCible> pCible)
          where TCible : TSource
        {
            return GetAddedItems(pSource, pCible, null);
        }

        /// <summary>
        /// Obtient la liste des éléments ajoutés à la collection <paramref name="pSource"/> pour obtenir la collection <paramref name="pCible"/>
        /// </summary>
        /// <typeparam name="TSource">Type des éléments de la collection <paramref name="pSource"/></typeparam>
        /// <typeparam name="TCible">Type des éléments de la collection <paramref name="pCible"/></typeparam>
        /// <param name="pSource">Collection source</param>
        /// <param name="pCible">Collection cible</param>
        /// <param name="pAction">Action a réaliser sur chaque éléments ajouté</param>
        /// <returns>Enumération des éléments présents dans <paramref name="pCible"/> qui ne sont pas présents dans <paramref name="pSource"/></returns>
        public static IEnumerable<TCible> GetAddedItems<TSource, TCible>(this ICollection<TSource> pSource, ICollection<TCible> pCible, Action<TCible> pAction)
          where TCible : TSource
        {
            Assertion.NotNull(pSource, "pSource");
            Assertion.NotNull(pAction, "pAction");

            foreach (TCible item in pCible)
            {
                if (!pSource.Contains(item))
                {
                    pAction?.Invoke(item);
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Obtient la liste des éléments communs aux collections <paramref name="pSource"/> et <paramref name="pCible"/>
        /// </summary>
        /// <typeparam name="TSource">Type des éléments de la collection <paramref name="pSource"/></typeparam>
        /// <typeparam name="TCible">Type des éléments de la collection <paramref name="pCible"/></typeparam>
        /// <param name="pSource">Collection source</param>
        /// <param name="pCible">Collection cible</param>
        /// <returns>Enumération des éléments présents dans les collection <paramref name="pCible"/> et <paramref name="pSource"/></returns>
        public static IEnumerable<TCible> GetCommonItems<TSource, TCible>(this ICollection<TSource> pSource, ICollection<TCible> pCible)
          where TCible : TSource
        {
            return GetCommonItems(pSource, pCible, null);
        }

        /// <summary>
        /// Obtient la liste des éléments communs aux collections <paramref name="pSource"/> et <paramref name="pCible"/>
        /// </summary>
        /// <typeparam name="TSource">Type des éléments de la collection <paramref name="pSource"/></typeparam>
        /// <typeparam name="TCible">Type des éléments de la collection <paramref name="pCible"/></typeparam>
        /// <param name="pSource">Collection source</param>
        /// <param name="pCible">Collection cible</param>
        /// <param name="pAction">Action a réaliser sur chaque éléments commun</param>
        /// <returns>Enumération des éléments présents dans les collection <paramref name="pCible"/> et <paramref name="pSource"/></returns>
        public static IEnumerable<TCible> GetCommonItems<TSource, TCible>(this ICollection<TSource> pSource, ICollection<TCible> pCible, Action<TCible> pAction)
          where TCible : TSource
        {
            Assertion.NotNull(pSource, "pSource");
            Assertion.NotNull(pCible, "pCible");

            foreach (TCible item in pCible)
            {
                if (pSource.Contains(item))
                {
                    pAction?.Invoke(item);
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Obtient la liste des éléments retirés de la collection <paramref name="pSource"/> pour obtenir la collection <paramref name="pCible"/>
        /// </summary>
        /// <typeparam name="TSource">Type des éléments de la collection <paramref name="pSource"/></typeparam>
        /// <typeparam name="TCible">Type des éléments de la collection <paramref name="pCible"/></typeparam>
        /// <param name="pSource">Collection source</param>
        /// <param name="pCible">Collection cible</param>
        /// <returns>Enumération des éléments non présents dans <paramref name="pCible"/> qui sont présents dans <paramref name="pSource"/></returns>
        /// <remarks>Tout élément de <paramref name="pSource"/> doit être du type <typeparamref name="TCible"/></remarks>
        public static IEnumerable<TCible> GetRemovedItems<TSource, TCible>(this ICollection<TSource> pSource, ICollection<TCible> pCible)
          where TCible : TSource
        {
            return GetRemovedItems(pSource, pCible, null);
        }

        /// <summary>
        /// Obtient la liste des éléments retirés de la collection <paramref name="pSource"/> pour obtenir la collection <paramref name="pCible"/>
        /// </summary>
        /// <typeparam name="TSource">Type des éléments de la collection <paramref name="pSource"/></typeparam>
        /// <typeparam name="TCible">Type des éléments de la collection <paramref name="pCible"/></typeparam>
        /// <param name="pSource">Collection source</param>
        /// <param name="pCible">Collection cible</param>
        /// <param name="pAction">Action a réaliser sur chaque éléments retiré</param>
        /// <returns>Enumération des éléments non présents dans <paramref name="pCible"/> qui sont présents dans <paramref name="pSource"/></returns>
        /// <remarks>Tout élément de <paramref name="pSource"/> doit être du type <typeparamref name="TCible"/></remarks>
        public static IEnumerable<TCible> GetRemovedItems<TSource, TCible>(this ICollection<TSource> pSource, ICollection<TCible> pCible, Action<TCible> pAction)
          where TCible : TSource
        {
            Assertion.NotNull(pSource, "pSource");
            Assertion.NotNull(pCible, "pCible");

            foreach (TSource item in pSource)
            {
                TCible cible = (TCible)item;
                if (!pCible.Contains(cible))
                {
                    pAction?.Invoke(cible);
                    yield return cible;
                }
            }
        }

        /// <summary>
        /// Supprime tous les items de la collection
        /// </summary>
        /// <typeparam name="T">type d'éléments</typeparam>
        /// <param name="value">collection à modifier</param>
        /// <param name="items">éléments à supprimer de la collection</param>
        public static void RemoveRange<T>(this ICollection<T> value, IEnumerable<T> items)
        {
            Assertion.NotNull(value, "value");
            Assertion.NotNull(items, "items");

            foreach (var item in items)
                value.Remove(item);
        }

        /// <summary>
        /// Supprime les éléments répondant au prédicat
        /// </summary>
        /// <typeparam name="T">type d'éléments</typeparam>
        /// <param name="value">collection à modifier</param>
        /// <param name="predicate">prédicat de suppression</param>        
        public static void RemoveWhere<T>(this ICollection<T> value, Func<T, bool> predicate)
        {
            Assertion.NotNull(predicate, "predicate");

            value.RemoveRange(value.Where(predicate).ToList());
        }

        /// <summary>
        /// Supprime le premier élément répondant au prédicat
        /// </summary>
        /// <typeparam name="T">type d'éléments</typeparam>
        /// <param name="value">collection à modifier</param>
        /// <param name="predicate">prédicat de suppression</param>        
        public static void RemoveFirst<T>(this ICollection<T> value, Func<T, bool> predicate)
        {
            Assertion.NotNull(predicate, "predicate");

            try
            {
                T element = value.First(predicate);
                value.Remove(element);
            }
            catch { }
        }

        /// <summary>
        /// Synchronise les éléments de la collection afin que celle-ci contienne les mêmes éléments que l'énumeration fournie en argument.
        /// Cette syncrhonisation s'effectue d'abord par une suppression des éléments obsolètes puis par un ajout des nouveaux éléments.
        /// Les éléments existants à la fois dans la collection et dans l'énumeration restent inchangés.
        /// </summary>
        /// <typeparam name="T">type d'éléments</typeparam>
        /// <param name="collection">La collection à modifier.</param>
        /// <param name="items">L'énumération qui contient les éléments finaux.</param>
        /// <param name="onAdding">Le délégué appelé avant l'ajout d'un élément à la collection.</param>
        /// <param name="onAdded">Le délégué appelé après l'ajout d'un élément à la collection.</param>
        /// <param name="onRemoving">Le délégué appelé avant la suppression d'un élément à la collection.</param>
        /// <param name="onRemoved">Le délégué appelé après la suppression d'un élément à la collection.</param>
        public static void Sync<T>(this ICollection<T> collection, IEnumerable<T> items,
            Action<T> onAdding = null, Action<T> onAdded = null,
            Action<T> onRemoving = null, Action<T> onRemoved = null)
        {

            var itemsCopy = collection.ToList();
            foreach (T item in itemsCopy.Except(items))
            {
                onRemoving?.Invoke(item);

                collection.Remove(item);

                onRemoved?.Invoke(item);
            }

            foreach (T item in items.Except(itemsCopy))
            {
                onAdding?.Invoke(item);

                collection.Add(item);

                onAdded?.Invoke(item);
            }

        }

        #endregion
    }
}
