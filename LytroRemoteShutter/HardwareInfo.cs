namespace UAM.Optics.LightField.Lytro.Camera
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Represents basic hardware information.
    /// </summary>
    public class HardwareInfo
    {
        internal const int Size = 644;
        /// <summary>
        /// Maximum number of bytes for the <see cref="Manufacturer"/> property.
        /// </summary>
        public const int MaximumManufacturerSize = 256;
        /// <summary>
        /// Maximum number of bytes for the <see cref="SerialNumber"/> property.
        /// </summary>
        public const int MaximumSerialNumberSize = 128;
        /// <summary>
        /// Maximum number of bytes for the <see cref="BuildID"/> property.
        /// </summary>
        public const int MaximumBuildIDSize = 128;
        /// <summary>
        /// Maximum number of bytes for the <see cref="SoftwareVersion"/> property.
        /// </summary>
        public const int MaximumSoftwareVersionSize = 128;

        private string _manufacturer;
        private string _serialNumber;
        private string _buildID;
        private string _softwareVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="HardwareInfo"/> class.
        /// </summary>
        public HardwareInfo()
        {
            _manufacturer = _serialNumber = _buildID = _softwareVersion = string.Empty;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="HardwareInfo"/> class from buffer data.
        /// </summary>
        /// <param name="buffer">A buffer containing the information data.</param>
        /// <param name="index">An offset into the <paramref name="buffer"/> where the information data begin.</param>
        public HardwareInfo(byte[] buffer, int index)
        {
            Load(buffer, index);
        }

        /// <summary>
        /// Populates the information from a buffer data.
        /// </summary>
        /// <param name="buffer">A buffer containing item data.</param>
        /// <param name="index">An offset into the <paramref name="buffer"/> where the information data begin.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is negative or large than the size of <paramref name="buffer"/>.</exception>
        /// <exception cref="ArgumentException">The required amount of data is not available from specified <paramref name="index"/>.</exception>
        public void Load(byte[] buffer, int index)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (index < 0 || index > buffer.Length)
                throw new ArgumentOutOfRangeException("index");

            if (index + Size > buffer.Length)
                throw new ArgumentException("Buffer must contain at least " + Size + " bytes starting at specified index.");

            _manufacturer = Encoding.UTF8.GetString(buffer, index + 0x000, MaximumManufacturerSize).TrimEnd('\0');
            _serialNumber = Encoding.UTF8.GetString(buffer, index + 0x100, MaximumSerialNumberSize).TrimEnd('\0');
            _buildID = Encoding.UTF8.GetString(buffer, index + 0x180, MaximumBuildIDSize).TrimEnd('\0');
            _softwareVersion = Encoding.UTF8.GetString(buffer, index + 0x200, MaximumSoftwareVersionSize).TrimEnd('\0');

            Unknown1 = BitConverter.ToInt32(buffer, index + 0x280);
        }

        /// <summary>
        /// Returns a binary representation of the information.
        /// </summary>
        /// <returns>a binary representation of the information.</returns>
        public byte[] ToArray()
        {
            byte[] buffer = new byte[Size];
            CopyTo(buffer, 0);
            
            return buffer;
        }

        /// <summary>
        /// Copies a binary representation of the information to an existing buffer.
        /// </summary>
        /// <param name="buffer">The buffer to copy the data to.</param>
        /// <param name="index">An offset into the <paramref name="buffer"/> where the copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is negative or large than the size of <paramref name="buffer"/>.</exception>
        /// <exception cref="ArgumentException">The required amount of data is not available from specified <paramref name="index"/>.</exception>
        private void CopyTo(byte[] buffer, int index)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (index < 0 || index > buffer.Length)
                throw new ArgumentOutOfRangeException("index");

            if (index + Size > buffer.Length)
                throw new ArgumentException("Buffer must have at least 128 bytes starting at specified index.");
            
            Array.Clear(buffer, index, Size);

            Encoding.UTF8.GetBytes(_manufacturer).CopyTo(buffer, index + 0x000);
            Encoding.UTF8.GetBytes(_serialNumber).CopyTo(buffer, index + 0x100);
            Encoding.UTF8.GetBytes(_buildID).CopyTo(buffer, index + 0x180);
            Encoding.UTF8.GetBytes(_softwareVersion).CopyTo(buffer, index + 0x200);

            BitConverter.GetBytes(Unknown1).CopyTo(buffer, index + 0x280);
        }

        /// <summary>
        /// Copies a binary representation of this info to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream"></param>
        public void CopyTo(Stream stream)
        {
            byte[] buffer = ToArray();
            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Gets or sets the camera's manufacturer name.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is longer than <see cref="MaximumManufacturerSize"/> bytes.</exception>
        public string Manufacturer
        {
            get { return _manufacturer; }
            set { EnsureLength(value, MaximumManufacturerSize); _manufacturer = value; }
        }

        /// <summary>
        /// Gets or sets the camera's serial number.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is longer than <see cref="MaximumSerialNumberSize"/> bytes.</exception>
        public string SerialNumber
        {
            get { return _serialNumber; }
            set { EnsureLength(value, MaximumSerialNumberSize); _serialNumber = value; }
        }

        /// <summary>
        /// Gets or sets the firmware build identification.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is longer than <see cref="MaximumBuildIDSize"/> bytes.</exception>
        public string BuildID
        {
            get { return _buildID; }
            set { EnsureLength(value, MaximumBuildIDSize); _buildID = value; }
        }

        /// <summary>
        /// Gets or sets the software version.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is longer than <see cref="MaximumSoftwareVersionSize"/> bytes.</exception>
        public string SoftwareVersion
        {
            get { return _softwareVersion; }
            set { EnsureLength(value, MaximumSoftwareVersionSize); _softwareVersion = value; }
        }

        /// <summary>
        /// Unknown value at offset 0x280.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Unknown1;

        private static void EnsureLength(string value, int length)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (Encoding.UTF8.GetByteCount(value) > length)
                throw new ArgumentException(string.Format("String value too long, must be {0} bytes or less.", length), "value");
        }
    }
}
