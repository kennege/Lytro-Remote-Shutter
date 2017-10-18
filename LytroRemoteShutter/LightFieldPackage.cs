using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace UAM.Optics.LightField.Lytro.IO
{
    /// <summary>
    /// Represents a light field package, consisting of one or more components.
    /// </summary>
    public partial class LightFieldPackage : LightFieldComponent
    {
        private ComponentCollection _components = new ComponentCollection();

        /// <summary>
        /// Gets the number of metadata components in the package.
        /// </summary>
        public int MetadataComponentsCount { get { return _components.MetadataCount; } }
        /// <summary>
        /// Gets the number of components in the package.
        /// </summary>
        public int ComponentsCount { get { return _components.Count; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightFieldPackage" /> class.
        /// </summary>
        public LightFieldPackage()
        {
            ComponentType = 'P';
            Version = 1;
            _components.Add(this);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LightFieldPackage" /> class from a stream of components.
        /// </summary>
        /// <param name="stream">A stream to load the components from.</param>
        public LightFieldPackage(Stream stream) : base(stream)
        {
            Load(stream);
        }
        /// <summary>
        /// Loads components from a stream.
        /// </summary>
        /// <param name="stream">A stream to load the components from.</param>
        /// <remarks>The components in <paramref name="stream" /> replace components currently in the package.</remarks>
        public void Load(Stream stream)
        {            
            _components.Clear();
            _components.Add(this); // this component was already read by base's constructor

            using (stream)
            {
                while (stream.Position < stream.Length)
                {
                    OnProgressChanged(stream.Position, stream.Length);

                    LightFieldComponent component = new LightFieldComponent(stream);

                    _components.Add(component);
                }
            }
        }

        /// <summary>
        /// Gets a component of specified index.
        /// </summary>
        /// <param name="index">A zero-based index of the component to get.</param>
        /// <returns>a component of the specified index.</returns>
        public LightFieldComponent GetComponent(int index)
        {
            return _components.GetComponent(index);
        }
        /// <summary>
        /// Enumerates components of specified reference identifier.
        /// </summary>
        /// <param name="reference">The component's reference identifier.</param>
        /// <returns> a sequence of components with the specified reference identifier.</returns>
        /// <remarks>Components are not required to have an unique reference identifier.</remarks>
        public IEnumerable<LightFieldComponent> GetComponent(string reference)
        {
            return _components.GetComponent(reference);
        }
        /// <summary>
        /// Enumerates indices of components of specified reference identifier.
        /// </summary>
        /// <param name="reference">The component's reference identifier.</param>
        /// <returns> a sequence of indices of components with the specified reference identifier.</returns>
        /// <remarks>Components are not required to have an unique reference identifier.</remarks>
        public IEnumerable<int> GetComponentIndices(string reference)
        {
            return _components.GetComponentIndices(reference);
        }

        
        /// <summary>
        /// Enumerates metadata components in the package.
        /// </summary>
        /// <returns>a sequence of metadata components in the package.</returns>
        public IEnumerable<LightFieldComponent> GetMetadata()
        {
            return _components.GetMetadata();
        }
        /// <summary>
        /// Enumerates indices of metadata components in the package.
        /// </summary>
        /// <returns>a sequence of indices of metadata components in the package.</returns>
        public IEnumerable<int> GetMetadataIndices()
        {
            return _components.GetMetadataIndices();
        }

        /// <summary>
        /// Gets a collection of components in the package.
        /// </summary>
        public ComponentCollection Components
        {
            get { return _components; }
        }

        /// <summary>
        /// Writes package header and all the components to a stream. (Overrides <see cref="LightFieldComponent.WriteTo" />.)
        /// </summary>
        /// <param name="stream">A stream to write the components to.</param>
        public override void WriteTo(Stream stream)
        {
            // base.WriteTo(stream); // we currently do not enforce this component's position
                                     // so the component list should be allowed to be written as is

            for (int i = 0; i < _components.Count; i++)
                if (_components[i] == this)
                    base.WriteTo(stream);
                else
                    _components[i].WriteTo(stream);
        }

        /// <summary>
        /// Reports the progress of package loading.
        /// </summary>
        /// <param name="position">Current position in the package stream being loaded.</param>
        /// <param name="length">Total size of the package being loaded.</param>
        protected void OnProgressChanged(long position, long length)
        {
            System.ComponentModel.ProgressChangedEventHandler handler = LoadProgressChanged;
            if (handler != null)
            {
                int percentage = (int)(100 * position / length);

                handler(this, new System.ComponentModel.ProgressChangedEventArgs(percentage, position));
            }
        }

        /// <summary>
        /// Reports progress of the package loading.
        /// </summary>
        public event System.ComponentModel.ProgressChangedEventHandler LoadProgressChanged;
    }
}
