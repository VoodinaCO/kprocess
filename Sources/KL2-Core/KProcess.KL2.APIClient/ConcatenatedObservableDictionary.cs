using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace KProcess.KL2.APIClient
{
    /// <summary>
	/// Provides a dictionary for use with data binding.
	/// </summary>
	/// <typeparam name="TKey">Specifies the type of the keys in this collection.</typeparam>
	/// <typeparam name="TValue">Specifies the type of the values in this collection.</typeparam>
    [DebuggerDisplay("Count={Count}")]
    public class ConcatenatedObservableDictionary<TKey, TValue> :
        ICollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>,
        INotifyCollectionChanged, INotifyPropertyChanged
    {
        readonly List<ObservableDictionary<TKey, TValue>> dictionaries;

        /// <summary>Event raised when the collection changes.</summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, args) => { };

        /// <summary>Event raised when a property on the collection changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

        /// <summary>
        /// Initializes an instance of the class using another dictionary as 
        /// the key/value store.
        /// </summary>
        public ConcatenatedObservableDictionary(params ObservableDictionary<TKey, TValue>[] dictionaries)
        {
            this.dictionaries = dictionaries.ToList();
            foreach (var dictionary in dictionaries)
            {
                dictionary.CollectionChanged += (o, e) => CollectionChanged?.Invoke(this, e);
                dictionary.PropertyChanged += (o, e) => PropertyChanged?.Invoke(this, e);
            }
        }

        /// <summary>
        /// Allows derived classes to raise custom property changed events.
        /// </summary>
        protected void RaisePropertyChanged(PropertyChangedEventArgs args) =>
            PropertyChanged(this, args);

        #region IDictionary<TKey,TValue> Members

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public void Add(TKey key, TValue value) =>
            throw new NotSupportedException();

        /// <summary>
        /// Clear dictionary
        /// </summary>
        public void Clear() =>
            dictionaries.ForEach(_ => _.Clear());

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, false.
        /// </returns>
        public bool ContainsKey(TKey key) =>
            dictionaries.Any(_ => _.ContainsKey(key));

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
        public ICollection<TKey> Keys =>
            (ICollection<TKey>)dictionaries.SelectMany(_ => _.Keys);

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </returns>
        public bool Remove(TKey key)
        {
            var dictionary = dictionaries.SingleOrDefault(_ => _.ContainsKey(key));
            if (dictionary == null)
                return false;
            return dictionary.RemoveWithNotification(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, false.
        /// </returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            var dictionary = dictionaries.SingleOrDefault(_ => _.ContainsKey(key));
            if (dictionary == null)
            {
                value = default(TValue);
                return false;
            }
            return dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.</returns>
        public ICollection<TValue> Values =>
            dictionaries.SelectMany(_ => _.Values).ToList();

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                var dictionary = dictionaries.SingleOrDefault(_ => _.ContainsKey(key));
                return dictionary == null ? default(TValue) : dictionary[key];
            }
            set
            {
                var dictionary = dictionaries.SingleOrDefault(_ => _.ContainsKey(key));
                dictionary?.UpdateWithNotification(key, value);
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) =>
            throw new NotSupportedException();

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            dictionaries.ForEach(_ => _.Clear());

            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(IDictionary.Count)));
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(IDictionary.Keys)));
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(IDictionary.Values)));
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) =>
            dictionaries.Any(_ => ((ICollection<KeyValuePair<TKey, TValue>>)_).Contains(item));

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException();
            if (array.Count() - arrayIndex < ((ICollection<KeyValuePair<TKey, TValue>>)this).Count)
                throw new ArgumentException();

            int currentIndex = arrayIndex;
            foreach (var dictionary in dictionaries)
            {
                ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, currentIndex);
                currentIndex += ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Count;
            }
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count =>
            dictionaries.Sum(_ => ((ICollection<KeyValuePair<TKey, TValue>>)_).Count);

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly =>
            dictionaries.Any(_ => ((ICollection<KeyValuePair<TKey, TValue>>)_).IsReadOnly);

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) =>
            Remove(item.Key);

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
            new ConcatenatedEnumerator<KeyValuePair<TKey, TValue>>(dictionaries.Select(_ => ((IEnumerable<KeyValuePair<TKey, TValue>>)_).GetEnumerator()).ToArray());

        IEnumerator IEnumerable.GetEnumerator() =>
            new ConcatenatedEnumerator<KeyValuePair<TKey, TValue>>(dictionaries.Select(_ => ((IEnumerable<KeyValuePair<TKey, TValue>>) _).GetEnumerator()).ToArray());

        #endregion
    }

    class ConcatenatedEnumerator<T> : IEnumerator<T>
    {
        readonly List<IEnumerator<T>> enumerators;
        IEnumerator<T> currentEnumerator;

        public ConcatenatedEnumerator(params IEnumerator<T>[] enumerators)
        {
            this.enumerators = new List<IEnumerator<T>>(enumerators);
            currentEnumerator = this.enumerators.First();
        }

        public T Current =>
            (T)currentEnumerator.Current;

        object IEnumerator.Current =>
            currentEnumerator.Current;

        public void Dispose()
        {
            foreach (var enumerator in enumerators)
                enumerator.Dispose();
        }

        public bool MoveNext()
        {
            var enumeratorIndex = enumerators.IndexOf(currentEnumerator);
            var result = false;
            while (true)
            {
                currentEnumerator = enumerators[enumeratorIndex];
                result = currentEnumerator.MoveNext();
                if (result)
                    return true;
                if (enumeratorIndex < enumerators.Count)
                    enumeratorIndex++;
                else
                    return false;
            }
        }

        public void Reset()
        {
            foreach (var enumerator in enumerators)
                enumerator.Reset();
            currentEnumerator = enumerators.First();
        }
    }
}
