namespace UAM.Optics.LightField.Lytro.Camera
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Represents an entry in the camera's list of pictures.
    /// </summary>
    public class PictureListEntry
    {
        internal const int Size = 128;
        /// <summary>
        /// Maximum number of bytes for the <see cref="FolderName"/> property.
        /// </summary>
        public const int MaximumFolderNameSize = 8;
        /// <summary>
        /// Maximum number of bytes for the <see cref="FileName"/> property.
        /// </summary>
        public const int MaximumFileNameSize = 8;
        /// <summary>
        /// Maximum number of bytes for the <see cref="PictureID"/> property.
        /// </summary>
        public const int MaximumPictureIDSize = 48;
        /// <summary>
        /// Maximum number of bytes for the <see cref="DateTaken"/> property.
        /// </summary>
        public const int MaximumDateTakenSize = 28;

        private string _folderName;
        private string _fileName;
        private int _folderNumber;
        private int _fileNumber;
        private int _liked;
        private float _lastLambda;
        private string _pictureID;
        private string _dateTaken;
        private int _rotation;

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureListEntry"/> class.
        /// </summary>
        public PictureListEntry()
        {
            _folderName = _fileName = _pictureID = _dateTaken = string.Empty;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PictureListEntry"/> class from buffer data.
        /// </summary>
        /// <param name="buffer">A buffer containing item data.</param>
        /// <param name="index">An offset into the <paramref name="buffer"/> where this item data begin.</param>
        public PictureListEntry(byte[] buffer, int index)
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

            _folderName = Encoding.UTF8.GetString(buffer, index + 0x00, MaximumFolderNameSize).TrimEnd('\0');
            _fileName = Encoding.UTF8.GetString(buffer, index + 0x08, MaximumFileNameSize).TrimEnd('\0');
            _folderNumber = BitConverter.ToInt32(buffer, index + 0x10);
            _fileNumber = BitConverter.ToInt32(buffer, index + 0x14);

            Unknown1 = BitConverter.ToInt32(buffer, index + 0x18);
            Unknown2 = BitConverter.ToInt32(buffer, index + 0x1C);
            Unknown3 = BitConverter.ToInt32(buffer, index + 0x20);
            Unknown4 = BitConverter.ToInt32(buffer, index + 0x24);

            _liked = BitConverter.ToInt32(buffer, index + 0x28);
            _lastLambda = BitConverter.ToSingle(buffer, index + 0x2C);
            _pictureID = Encoding.UTF8.GetString(buffer, index + 0x30, MaximumPictureIDSize).TrimEnd('\0');
            _dateTaken = Encoding.UTF8.GetString(buffer, index + 0x60, MaximumDateTakenSize).TrimEnd('\0');

            _rotation = BitConverter.ToInt32(buffer, index + 0x7C);
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

            Encoding.UTF8.GetBytes(_folderName).CopyTo(buffer, index + 0x00);
            Encoding.UTF8.GetBytes(_fileName).CopyTo(buffer, index + 0x08);
            BitConverter.GetBytes(_folderNumber).CopyTo(buffer, index + 0x10);
            BitConverter.GetBytes(_fileNumber).CopyTo(buffer, index + 0x14);
            
            BitConverter.GetBytes(Unknown1).CopyTo(buffer, index + 0x18);
            BitConverter.GetBytes(Unknown2).CopyTo(buffer, index + 0x1C);
            BitConverter.GetBytes(Unknown3).CopyTo(buffer, index + 0x20);
            BitConverter.GetBytes(Unknown4).CopyTo(buffer, index + 0x24);

            BitConverter.GetBytes(_liked).CopyTo(buffer, index + 0x28);
            BitConverter.GetBytes(_lastLambda).CopyTo(buffer, index + 0x2C);
            Encoding.UTF8.GetBytes(_pictureID).CopyTo(buffer, index + 0x30);
            Encoding.UTF8.GetBytes(_dateTaken).CopyTo(buffer, index + 0x60);

            BitConverter.GetBytes(_rotation).CopyTo(buffer, index + 0x7C);
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
        /// Gets or sets the folder name postfix.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is longer than <see cref="MaximumFolderNameSize"/> bytes.</exception>
        public string FolderName
        {
            get { return _folderName; }
            set { EnsureLength(value, MaximumFolderNameSize); _folderName = value; }
        }

        /// <summary>
        /// Gets or sets the file name prefix.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is longer than <see cref="MaximumFileNameSize"/> bytes.</exception>
        public string FileName
        {
            get { return _fileName; }
            set { EnsureLength(value, MaximumFileNameSize); _fileName = value; }
        }

        /// <summary>
        /// Gets or sets the folder number.
        /// </summary>
        public int FolderNumber
        {
            get { return _folderNumber; }
            set { _folderNumber = value; }
        }

        /// <summary>
        /// Gets or sets the folder number.
        /// </summary>
        public int FileNumber
        {
            get { return _fileNumber; }
            set { _fileNumber = value; }
        }

        /// <summary>
        /// Gets or sets the star status of the picture.
        /// </summary>
        public bool IsLiked
        {
            get { return _liked != 0; }
            set { _liked = value ? 1 : 0; }
        }

        /// <summary>
        /// Gets or sets the last lambda at which user focused the picture in camera.
        /// </summary>
        public float LastLambda
        {
            get { return _lastLambda; }
            set { _lastLambda = value; }
        }

        /// <summary>
        /// Gets or sets the picture ID.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is longer than <see cref="MaximumPictureIDSize"/> bytes.</exception>
        public string PictureID
        {
            get { return _pictureID; }
            set { EnsureLength(value, MaximumPictureIDSize); _pictureID = value; }
        }

        /// <summary>
        /// Gets or sets the date the picture was taken. The value should be in ISO 8601 format.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="value"/> is longer than <see cref="MaximumDateTakenSize"/> bytes.</exception>
        public string DateTaken
        {
            get { return _dateTaken; }
            set { EnsureLength(value, MaximumDateTakenSize); _dateTaken = value; }
        }

        /// <summary>
        /// Gets or sets the picture rotation.
        /// </summary>
        public PictureListRotation Rotation
        {
            get { return (PictureListRotation)_rotation; }
            set { _rotation = (int)value; }
        }

        /// <summary>
        /// Gets a path to the image in camera.
        /// </summary>
        public string Path
        {
            get { return string.Format(@"I:\DCIM\{0:000}{1}\{2}{3:0000}.RAW", _folderNumber, _folderName, _fileName, _fileNumber); }
        }

        /// <summary>
        /// Unknown value at offset 0x18.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Unknown1;
        /// <summary>
        /// Unknown value at offset 0x1C.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Unknown2;
        /// <summary>
        /// Unknown value at offset 0x20.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Unknown3;
        /// <summary>
        /// Unknown value at offset 0x24.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int Unknown4;

        private static void EnsureLength(string value, int length)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (Encoding.UTF8.GetByteCount(value) > length)
                throw new ArgumentException(string.Format("String value too long, must be {0} bytes or less.", length), "value");
        }

        /// <summary>
        /// Returns a string that represents current object.
        /// </summary>
        /// <returns>a string that represents current object.</returns>
        public override string ToString()
        {
            return string.Format("{0} [{1}]", PictureID, Path);
        }
    }
}
