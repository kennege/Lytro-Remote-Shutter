using System;
using System.IO;
using System.Text;
using UAM.InformatiX;

namespace UAM.Optics.LightField.Lytro.IO
{
    /// <summary>
    /// Represents a component of the light field package.
    /// </summary>
    public class LightFieldComponent
    {
        /// <summary>
        /// The maximum number of bytes that the Reference property can hold.
        /// </summary>
        public const int MaximumReferenceLength = 80;

        private char _componentType;
        /// <summary>
        /// Gets or sets the component type.
        /// </summary>
        public char ComponentType
        {
            get { return _componentType; }
            set { _componentType = value; }
        }

        private int _version;
        /// <summary>
        /// Gets or sets the component schema version.
        /// </summary>
        public int Version
        {
            get { return _version; }
            set { _version = value; }
        }

        private int _streamLength;
        /// <summary>
        /// Gets the length of the component data.
        /// </summary>
        /// <remarks>
        /// Components are not required to contain data. In that case this property returns zero.
        /// </remarks>
        public int Length
        {
            get { return _data == null ? 0 : _data.Length; }
        }

        private string _reference;
        /// <summary>
        /// Gets or sets the component reference identifier.
        /// </summary>
        /// <exception cref="ArgumentException">The <paramref name="value"/> needs more than <see cref="MaximumReferenceLength" /> bytes when encoded using the current <see cref="Encoding"/>.</exception>
        public string Reference
        {
            get { return _reference; }
            set
            {
                if (value != null && Encoding.GetByteCount(value) > MaximumReferenceLength)
                    throw new ArgumentException(string.Format("Reference must take {0} bytes at maximum.", MaximumReferenceLength));

                _reference = value;
            }
        }

        private byte[] _data;
        /// <summary>
        /// Gets or sets the component data.
        /// </summary>
        /// <remarks>
        /// Components are not required to contain data. This property can be null.
        /// </remarks>
        public byte[] Data
        {
            get { return _data; }
            set { _data = value; }
        }

        private Encoding _encoding;
        /// <summary>
        /// Gets or sets the component encoding.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        public Encoding Encoding
        {
            get { return _encoding; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                _encoding = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightFieldComponent" /> class.
        /// </summary>
        public LightFieldComponent()
        {
            _componentType = 'C';
            _version = 0;
            _encoding = Encoding.UTF8;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LightFieldComponent" /> class from a stream of data.
        /// </summary>
        /// <param name="stream">A stream to load the component from.</param>
        /// <exception cref="FormatException"><paramref name="stream"/> does not contain a valid component.</exception>
        public LightFieldComponent(Stream stream) : this(stream, Encoding.UTF8)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LightFieldComponent" /> class from a stream of data with specified encoding.
        /// </summary>
        /// <param name="stream">A stream to load the component from.</param>
        /// <param name="encoding">The encoding of data in the <paramref name="stream"/>.</param>
        /// <exception cref="FormatException"><paramref name="stream"/> does not contain a valid component.</exception>
        public LightFieldComponent(Stream stream, Encoding encoding)
        {
            _encoding = encoding;

            byte[] header = new byte[8];

            if (stream.Read(header, 0, header.Length) != header.Length)
                throw new EndOfStreamException();

            CheckHeader(header);
            _componentType = (char)header[3];

            _version = stream.ReadBigInt32();
            _streamLength = stream.ReadBigInt32();

            if (_streamLength > 0)
            {
                byte[] reference = new byte[MaximumReferenceLength];
                if (stream.Read(reference, 0, reference.Length) != reference.Length)
                    throw new EndOfStreamException();

                _reference = encoding.GetString(reference, 0, reference.Length).TrimEnd('\0');

                _data = new byte[_streamLength];
                if (stream.Read(_data, 0, _data.Length) != _data.Length)
                    throw new EndOfStreamException();

                int padding = _data.Length % 16;
                if (padding > 0)
                    stream.Read(new byte[16 - padding], 0, 16 - padding);
            }
        }

        /// <summary>
        /// Checks whether the supplied byte array is a valid component header.
        /// </summary>
        /// <param name="header">A byte array to check.</param>
        /// <exception cref="ArgumentNullException"><paramref name="header"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="header"/> is not of an expected length.</exception>
        /// <exception cref="FormatException"><paramref name="header"/> does not contain a valid component header.</exception>
        private void CheckHeader(byte[] header)
        {
            if (header == null)
                throw new ArgumentNullException();

            if (header.Length != 8)
                throw new ArgumentException();

            if (header[0] != 0x89 ||
                header[1] != (byte)'L' ||
                header[2] != (byte)'F' ||
                header[4] != 0x0D ||
                header[5] != 0x0A ||
                header[6] != 0x1A ||
                header[7] != 0x0A)
                throw new FormatException();
        }

        /// <summary>
        /// Gets the component data as <see cref="String"/> using the component's <see cref="Encoding"/> or null if the component does not contain any data.
        /// </summary>
        /// <returns>The component data as <see cref="String"/> using the component's <see cref="Encoding"/> or null if the component does not contain any data.</returns>
        public virtual string GetDataAsString()
        {
            if (_data == null)
                return null;

            return _encoding.GetString(_data, 0, _data.Length);
        }

        /// <summary>
        /// Sets the component data to <see cref="String"/> using the component's <see cref="Encoding"/>.
        /// </summary>
        /// <param name="data">The string to set the component data to.</param>
        public virtual void SetDataToString(string data)
        {
            if (data == null)
                _data = null;

            else
                _data = _encoding.GetBytes(data);
        }

        /// <summary>
        /// Writes the LightFieldComponent to a stream.
        /// </summary>
        /// <param name="stream">A stream to write the component to.</param>
        public virtual void WriteTo(Stream stream)
        {
            byte[] header = new byte[8] { 0x89, (byte)'L', (byte)'F', (byte)ComponentType, 0x0D, 0x0A, 0x1A, 0x0A };
            stream.Write(header, 0, header.Length);

            stream.WriteBigInt32(_version);
            stream.WriteBigInt32(Length);

            if (Length > 0)
            {
                byte[] reference = new byte[MaximumReferenceLength];
                if (Reference != null)
                    _encoding.GetBytes(Reference).CopyTo(reference, 0);
                stream.Write(reference, 0, reference.Length);

                stream.Write(_data, 0, _data.Length);

                int padding = Length % 16;
                if (padding > 0)
                    stream.Write(new byte[16 - padding], 0, 16 - padding);
            }
        }

        /// <summary>
        /// Returns a <see cref="String"/> representing the type and name of the current LightFieldComponent. (Overrides <see cref="Object.ToString()"/>.)
        /// </summary>
        /// <returns>a <see cref="String"/> representing the type and name of the current LightFieldComponent. (Overrides <see cref="Object.ToString()"/>.)</returns>
        public override string ToString()
        {
            if (_componentType == '\0')
                return base.ToString();
            else
                if (_reference == null)
                    return "LF" + _componentType;
                else
                    return "LF" + _componentType + ": " + _reference;
        }
    }
}
