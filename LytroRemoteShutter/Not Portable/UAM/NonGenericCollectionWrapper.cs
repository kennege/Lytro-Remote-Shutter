using System;
using System.Collections;
using System.Collections.Generic;

namespace UAM.InformatiX.Collections
{
    internal class NonGenericCollectionWrapper<T> : ICollection
    {
        private ICollection<T> _collection;

        public NonGenericCollectionWrapper(ICollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException();

            _collection = collection;
        }

        public int Count
        {
            get { return _collection.Count; }
        }
        public bool IsSynchronized
        {
            get { return false; }
        }
        public object SyncRoot
        {
            get { throw new NotSupportedException(); }
        }

        public void CopyTo(Array array, int index)
        {
            T[] genericArray = (T[])array;

            _collection.CopyTo(genericArray, index);
        }
        public IEnumerator GetEnumerator()
        {
            return _collection.GetEnumerator();
        }
    }
}
