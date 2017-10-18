using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UAM.InformatiX.Collections;

namespace UAM.InformatiX.Text.Json
{                                  
    internal class JsonTypeDictionary : JsonTypeResolver, IDictionary<string, Type>
    {
        private Dictionary<string, Type> _types;
        protected IDictionary<string, Type> TypesDictionary { get { return (IDictionary<string, Type>)_types; } }

        public JsonTypeDictionary()
        {
            _types = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        }
        public JsonTypeDictionary(int capacity)
        {
            _types = new Dictionary<string, Type>(capacity, StringComparer.OrdinalIgnoreCase);
        }
        public JsonTypeDictionary(IComparer<string> comparer)
        {
            _types = new Dictionary<string, Type>(EqualityComparer.FromComparer(comparer));
        }
        public JsonTypeDictionary(IDictionary<string, Type> dictionary)
        {
            _types = new Dictionary<string, Type>(dictionary);
        }
        public JsonTypeDictionary(int capacity, IComparer<string> comparer)
        {
            _types = new Dictionary<string, Type>(capacity, EqualityComparer.FromComparer(comparer));
        }
        public JsonTypeDictionary(IDictionary<string, Type> dictionary, IComparer<string> comparer)
        {
            _types = new Dictionary<string, Type>(dictionary, EqualityComparer.FromComparer(comparer));
        }

        public override Type ResolveType(string typeName)
        {
            Type type;
            if (typeName != null && _types.TryGetValue(typeName, out type))
                return type;

            return base.ResolveType(typeName);
        }
        public override string ResolveTypeName(Type type)
        {
            // SortedList not allowed in portable build

            foreach (KeyValuePair<string, Type> pair in _types)
                if (pair.Value == type)
                    return pair.Key;

            return base.ResolveTypeName(type);
        }

        public void LoadAssemblyMappings(Assembly assembly)
        {
            foreach (JsonTypeMappingAttribute attribute in assembly.GetCustomAttributes<JsonTypeMappingAttribute>())
                Add(attribute.Name, attribute.Type);
        }
        public void LoadAssemblyMappings<T>()
        {
            LoadAssemblyMappings(typeof(T).Assembly);
        }

        #region IDictionary wrapper

        public void Add(string key, Type value)
        {
            _types.Add(key, value);
        }
        public void Add(KeyValuePair<string, Type> item)
        {
            TypesDictionary.Add(item);
        }

        public bool Contains(KeyValuePair<string, Type> item)
        {
            return TypesDictionary.Contains(item);
        }
        public bool ContainsKey(string key)
        {
            return _types.ContainsKey(key);
        }

        public int Count
        {
            get { return _types.Count; }
        }
        public bool IsReadOnly
        {
            get { return TypesDictionary.IsReadOnly; }
        }
        
        public ICollection<string> Keys
        {
            get { return _types.Keys; }
        }
        public ICollection<Type> Values
        {
            get { return _types.Values; }
        }
        public void CopyTo(KeyValuePair<string, Type>[] array, int arrayIndex)
        {
            TypesDictionary.CopyTo(array, arrayIndex);
        }

        public void Clear()
        {
            _types.Clear();
        }
        public bool Remove(string key)
        {
            return _types.Remove(key);
        }
        public bool Remove(KeyValuePair<string, Type> item)
        {
            return TypesDictionary.Remove(item);
        }

        public Type this[string key]
        {
            get { return _types[key]; }
            set { _types[key] = value; }
        }
        public bool TryGetValue(string key, out Type value)
        {
            return _types.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, Type>> GetEnumerator()
        {
            return _types.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        #endregion
    }
}
