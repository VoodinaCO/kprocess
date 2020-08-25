//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : BulkObservableCollection.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : ObservableCollection fournissant des traitements par lot.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*   
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace KProcess
{
    /// <summary>
    /// ObservableCollection fournissant des traitements par lot.
    /// </summary>
    /// <typeparam name="T">type d'objets gérés par la collection</typeparam>
    public class BulkObservableCollection<T> : ObservableCollection<T>
    {
        #region Constructeurs

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="disableRangeNotifications">Indique si la collection doit désactiver les notifications avec des éléments multiples.</param>
        public BulkObservableCollection(bool disableRangeNotifications = false)
            : base()
        {
            this.DisableRangeNotifications = disableRangeNotifications;
        }

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="collection">Eléménts initialisant la collection</param>
        /// <param name="disableRangeNotifications">Indique si la collection doit désactiver les notifications avec des éléments multiples.</param>
        public BulkObservableCollection(IEnumerable<T> collection, bool disableRangeNotifications = false)
            : base(collection)
        {
            this.DisableRangeNotifications = disableRangeNotifications;
        }

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="list">Eléménts initialisant la collection</param>
        /// <param name="disableRangeNotifications">Indique si la collection doit désactiver les notifications avec des éléments multiples.</param>
        public BulkObservableCollection(List<T> list, bool disableRangeNotifications = false)
            : base(list)
        {
            this.DisableRangeNotifications = disableRangeNotifications;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la collection doit désactiver les notifications avec des éléments multiples. 
        /// Si la valeur est <c>true</c>, la collection enverra des multiples notifications avec un unique élément.
        /// </summary>
        /// <remarks>Activer cette option est utile lorsque la collection est susceptible d'être bindée à des contrôles WPF.</remarks>
        public bool DisableRangeNotifications { get; set; }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Ajoute plusieurs éléments à la collection.
        /// </summary>
        /// <param name="items">Eléments à ajouter.</param>
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            int index = base.Count;

            foreach (var item in items)
                base.Items.Add(item);

            if (DisableRangeNotifications)
            {
                foreach (var item in items)
                {
                    base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
                    index++;
                }
            }
            else
                base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items), index));
        }

        /// <summary>
        /// Supprime plusieurs éléments
        /// </summary>
        /// <param name="items">Eléments à supprimer.</param>
        public void RemoveRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            foreach (var item in items)
            {
                var index = base.Items.IndexOf(item);
                if (base.Items.Remove(item) && DisableRangeNotifications)
                    base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            }

            if (!DisableRangeNotifications)
                base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<T>(items), 0));
        }

        /// <summary>
        /// Supprime les éléments répondant à un prédicat
        /// </summary>
        /// <param name="predicate">Prédicat des éléments à supprimer.</param>
        public void RemoveWhere(Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            RemoveRange(base.Items.Where(predicate).ToArray());
        }

        /// <summary>
        /// Remplace l'objet actuellement dans la collection par un nouvel objet.
        /// </summary>
        /// <param name="oldItem">L'ancien élément.</param>
        /// <param name="newItem">Le nouvel élément.</param>
        /// <returns>A booléen indiquant si le remplacement a bien eu lieu.</returns>
        public bool Replace(T oldItem, T newItem)
        {
            var oldIndex = base.Items.IndexOf(oldItem);
            if (oldIndex >= 0)
            {
                base.Items.RemoveAt(oldIndex);
                base.Items.Insert(oldIndex, newItem);
                base.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(
                    System.Collections.Specialized.NotifyCollectionChangedAction.Replace, newItem, oldItem, oldIndex));
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Remplace tous les éléments par ceux fournis.
        /// </summary>
        /// <param name="items">Eléments remplaçants.</param>
        public void ReplaceAll(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            base.Items.Clear();

            foreach (var item in items)
                base.Items.Add(item);

            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Trie la collection.
        /// </summary>
        public void Sort()
        {
            this.Sort(0, Count, null);
        }

        /// <summary>
        /// Trie la collection avec le comparateur spécifié.
        /// </summary>
        /// <param name="comparer">Le comparateur.</param>
        public void Sort(IComparer<T> comparer)
        {
            this.Sort(0, Count, comparer);
        }

        /// <summary>
        /// Trie une partie de la collection avec le comparateur spécifié.
        /// </summary>
        /// <param name="index">L'index de départ.</param>
        /// <param name="count">Le nombre délémens à trier.</param>
        /// <param name="comparer">Le comparateur.</param>
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            (Items as List<T>).Sort(index, count, comparer);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion
    }
}
