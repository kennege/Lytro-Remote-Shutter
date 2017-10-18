using System.IO;

namespace UAM.Optics.LightField.Lytro.IO
{
    /// <summary>
    /// Represents a binary reader wrapper around the <see cref="LightFieldComponent"/>
    /// </summary>
    public class ComponentReader : BinaryReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentReader" /> class.
        /// </summary>
        /// <param name="component">The <see cref="LightFieldComponent"/> to read from.</param>
        public ComponentReader(LightFieldComponent component) : base(new MemoryStream(component.Data), component.Encoding)
        {
            _length = component.Length;
        }

        private int _length;
        /// <summary>
        /// Gets length of the underlaying component data.
        /// </summary>
        public int Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Gets or sets the position within the underlaying component data.
        /// </summary>
        public int Position
        {
            get { return (int)BaseStream.Position; }
            set { BaseStream.Position = value; }
        }
    }
}
