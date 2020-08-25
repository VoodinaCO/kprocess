using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ReencodeTool
{
    public class ObservableDictionary<T_Key, T_Value> : Dictionary<T_Key, T_Value>, IDictionary<T_Key, T_Value>, INotifyCollectionChanged
    {
        public bool IsReadOnly => throw new System.NotImplementedException();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public new void Add(T_Key key, T_Value value)
        {
            base.Add(key, value);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<T_Key, T_Value>(key, value)));
        }

        public void Add(KeyValuePair<T_Key, T_Value> item)
        {
            base.Add(item.Key, item.Value);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public new void Clear()
        {
            base.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(KeyValuePair<T_Key, T_Value> item)
        {
            return ContainsKey(item.Key) && this[item.Key].Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<T_Key, T_Value>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public new bool Remove(T_Key key)
        {
            T_Value value = this[key];
            bool result = base.Remove(key);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
            return result;
        }

        public bool Remove(KeyValuePair<T_Key, T_Value> item)
        {
            T_Value value = this[item.Key];
            bool result = base.Remove(item.Key);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return base.GetEnumerator();
        }
    }
}
