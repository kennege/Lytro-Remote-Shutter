using System;
using System.Collections;
using System.Collections.Generic;

namespace UAM.InformatiX.Collections
{
    internal class NonGenericDictionaryWrapper<TKey, TValue> : IDictionary
    {
        public class Enumerator : IDictionaryEnumerator
        {
            private IEnumerator<KeyValuePair<TKey, TValue>> _enumerator;

            public Enumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator)
            {
                _enumerator = enumerator;
            }

            public DictionaryEntry Entry
            {
                get { return new DictionaryEntry(_enumerator.Current.Key, _enumerator.Current.Value); }
            }

            public object Key
            {
                get { return _enumerator.Current.Key; }
            }

            public object Value
            {
                get { return _enumerator.Current.Value; }
            }

            public object Current
            {
                get { return Entry; }
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator.Reset();
            }
        }

        private IDictionary<TKey, TValue> _dictionary;

        public NonGenericDictionaryWrapper(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException();

            _dictionary = dictionary;
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }
        public bool IsFixedSize
        {
            get { return false; }
        }
        public bool IsReadOnly
        {
            get { return _dictionary.IsReadOnly; }
        }
        public bool IsSynchronized
        {
            get { return false; }
        }
        public object SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        public void Add(object key, object value)
        {
            _dictionary.Add((TKey)key, (TValue)value);
        }
        public void Remove(object key)
        {
            _dictionary.Remove((TKey)key);
        }
        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(object key)
        {
            return _dictionary.ContainsKey((TKey)key);
        }
        public object this[object key]
        {
            get
            {
                return _dictionary[(TKey)key];
            }
            set
            {
                _dictionary[(TKey)key] = (TValue)value;
            }
        }

        public ICollection Keys
        {
            get { return new NonGenericCollectionWrapper<TKey>(_dictionary.Keys); }
        }
        public ICollection Values
        {
            get { return new NonGenericCollectionWrapper<TValue>(_dictionary.Values); }
        }

        public void CopyTo(Array array, int index)
        {
            KeyValuePair<TKey, TValue>[] genericArray = (KeyValuePair<TKey, TValue>[])array;

            _dictionary.CopyTo(genericArray, index);
        }
        public IDictionaryEnumerator GetEnumerator()
        {
            return new Enumerator(_dictionary.GetEnumerator());
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
