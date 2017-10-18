using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UAM.Optics.LightField.Lytro.IO
{
    /// <summary>
    /// Represents a collection of <see cref="LightFieldComponent"/>s.
    /// </summary>
    public class ComponentCollection : Collection<LightFieldComponent>
    {
        private List<int> _metadataIndices = new List<int>();
        private Dictionary<string, List<int>> _referenceIndices = new Dictionary<string, List<int>>();

        /// <summary>
        /// Gets the number of metadata components in the collection.
        /// </summary>
        protected internal int MetadataCount
        {
            get { return _metadataIndices.Count; }
        }

        /// <summary>
        /// Removes all components from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            base.ClearItems();
            _metadataIndices.Clear();
            _referenceIndices.Clear();
        }

        /// <summary>
        /// Removes component at specified index.
        /// </summary>
        /// <param name="index">The index to remove the component at.</param>
        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            RemoveItem(_metadataIndices, index);

            foreach (List<int> indexList in _referenceIndices.Values)
                RemoveItem(indexList, index);
        }

        private static void RemoveItem(List<int> indices, int index)
        {
            for (int i = indices.Count - 1; i >= 0; i--)
            {
                if (indices[i] > index)
                    indices[i]--;

                else if (indices[i] == index)
                    indices.RemoveAt(i);
            }
        }

        /// <summary>
        /// Sets component at specified index.
        /// </summary>
        /// <param name="index">The index to set the component at.</param>
        /// <param name="item">The component to set.</param>
        protected override void SetItem(int index, LightFieldComponent item)
        {
            string oldReference = this[index].Reference;
            string newReference = item.Reference;

            char oldType = this[index].ComponentType;
            char newType = item.ComponentType;

            if (oldType != newType)
            {
                if (oldType == 'M')
                    _metadataIndices.Remove(index);

                else if (newType == 'M')
                    _metadataIndices.Add(index);
            }

            if (oldReference != newReference)
            {
                List<int> indices;

                if (_referenceIndices.TryGetValue(oldReference, out indices))
                    indices.Remove(index);

                if (!_referenceIndices.TryGetValue(newReference, out indices))
                    _referenceIndices[newReference] = indices = new List<int>();

                indices.Add(index);
            }

            base.SetItem(index, item);
        }

        /// <summary>
        /// Inserts component at specified index.
        /// </summary>
        /// <param name="index">The index to insert the component at.</param>
        /// <param name="item">The component to insert.</param>
        protected override void InsertItem(int index, LightFieldComponent item)
        {
            InsertItem(_metadataIndices, index);

            if (item.ComponentType == 'M')
                _metadataIndices.Add(index);

            foreach (List<int> indices in _referenceIndices.Values)
                InsertItem(indices, index);

            if (item.Reference != null)
            {
                List<int> indices = null;
                if (!_referenceIndices.TryGetValue(item.Reference, out indices))
                    _referenceIndices[item.Reference] = indices = new List<int>();

                indices.Add(index);
            }

            base.InsertItem(index, item);
        }

        private static void InsertItem(List<int> indices, int index)
        {
            for (int i = 0; i < indices.Count; i++)
            {
                if (indices[i] >= index)
                    indices[i]++;
            }
        }


        /// <summary>
        /// Gets a component of specified index.
        /// </summary>
        /// <param name="index">A zero-based index of the component to get.</param>
        /// <returns>a component of the specified index.</returns>
        protected internal LightFieldComponent GetComponent(int index)
        {
            return this[index]; // throws if out of range
        }
        /// <summary>
        /// Enumerates components of specified reference identifier.
        /// </summary>
        /// <param name="reference">The component's reference identifier.</param>
        /// <returns> a sequence of components with the specified reference identifier.</returns>
        /// <remarks>Components are not required to have an unique reference identifier.</remarks>
        protected internal IEnumerable<LightFieldComponent> GetComponent(string reference)
        {
            List<int> indices;

            if (reference != null && _referenceIndices.TryGetValue(reference, out indices))
                return indices.Select(i => this[i]);
            else
                return Enumerable.Empty<LightFieldComponent>();
        }
        /// <summary>
        /// Enumerates indices of components of specified reference identifier.
        /// </summary>
        /// <param name="reference">The component's reference identifier.</param>
        /// <returns> a sequence of indices of components with the specified reference identifier.</returns>
        /// <remarks>Components are not required to have an unique reference identifier.</remarks>
        protected internal IEnumerable<int> GetComponentIndices(string reference)
        {
            List<int> indices;

            if (reference != null && _referenceIndices.TryGetValue(reference, out indices))
                return new ReadOnlyCollection<int>(indices);
            else
                return Enumerable.Empty<int>();
        }

        /// <summary>
        /// Enumerates metadata components in the collection.
        /// </summary>
        /// <returns>a sequence of metadata components in the package.</returns>
        protected internal IEnumerable<LightFieldComponent> GetMetadata()
        {
            return _metadataIndices.Select(i => this[i]);
        }
        /// <summary>
        /// Enumerates indices of metadata components in the collection.
        /// </summary>
        /// <returns>a sequence of indices of metadata components in the package.</returns>
        protected internal IEnumerable<int> GetMetadataIndices()
        {
            return new ReadOnlyCollection<int>(_metadataIndices);
        }

    }
}
