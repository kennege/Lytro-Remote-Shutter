namespace UAM.Optics.LightField.Lytro.Camera
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents a record in the camera's list of pictures' entry.
    /// </summary>
    public class PictureListRecordDefinition
    {
        internal const int Size = 8;

        private int _index;
        private int _offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureListRecordDefinition"/> class.
        /// </summary>
        public PictureListRecordDefinition() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="PictureListRecordDefinition"/> class from buffer data.
        /// </summary>
        /// <param name="buffer">A buffer containing item data.</param>
        /// <param name="index">An offset into the <paramref name="buffer"/> where this item data begin.</param>
        public PictureListRecordDefinition(byte[] buffer, int index)
        {
            Load(buffer, index);
        }

        /// <summary>
        /// Populates the entry from a buffer data.
        /// </summary>
        /// <param name="buffer">A buffer containing item data.</param>
        /// <param name="index">An offset into the <paramref name="buffer"/> where this item data begin.</param>
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

            _index = BitConverter.ToInt32(buffer, index + 0x10);
            _offset = BitConverter.ToInt32(buffer, index + 0x14);
        }

        /// <summary>
        /// Returns a binary representation of this entry.
        /// </summary>
        /// <returns>a binary representation of this entry.</returns>
        public byte[] ToArray()
        {
            byte[] buffer = new byte[Size];
            CopyTo(buffer, 0);

            return buffer;
        }
        /// <summary>
        /// Copies a binary representation of this entry to an existing buffer.
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
                throw new ArgumentException("Buffer must have at least " + Size + " bytes starting at specified index.");

            Array.Clear(buffer, index, Size);

            BitConverter.GetBytes(_index).CopyTo(buffer, index + 0x00);
            BitConverter.GetBytes(_offset).CopyTo(buffer, index + 0x04);
        }

        /// <summary>
        /// Copies a binary representation of this entry to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream"></param>
        public void CopyTo(Stream stream)
        {
            byte[] buffer = ToArray();
            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Gets the record index.
        /// </summary>
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        /// <summary>
        /// Gets the record offset.
        /// </summary>
        public int Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        /// <summary>
        /// Returns a string that represents current object.
        /// </summary>
        /// <returns>a string that represents current object.</returns>
        public override string ToString()
        {
            return string.Format("{0}: {1:X2}", _index, _offset);
        }
    }
}
