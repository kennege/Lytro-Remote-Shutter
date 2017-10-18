namespace UAM.Optics.LightField.Lytro.Camera
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// Represents the camera's list of pictures.
    /// </summary>
    public class PictureList : Collection<PictureListEntry>
    {
        private int _entryLength;

        private int _recordCount;
        private Collection<PictureListRecordDefinition> _recordDefinitions = new Collection<PictureListRecordDefinition>();

        /// <summary>
        /// Unknown value at offset 0x00.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly int Unknown1;

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureList"/> class.
        /// </summary>
        public PictureList() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureList"/> class from buffer data.
        /// </summary>
        /// <param name="buffer">A buffer containing picture list.</param>
        /// <param name="index">An offset into the <paramref name="buffer"/> where the picture list begins.</param>
        public PictureList(byte[] buffer, int index)
        {
            Unknown1 = BitConverter.ToInt32(buffer, index);
            _entryLength = BitConverter.ToInt32(buffer, index + 0x04);
            _recordCount = BitConverter.ToInt32(buffer, index + 0x08);

            index += 0x0C;

            for (int i = 0; i < _recordCount; i++, index += PictureListRecordDefinition.Size)
            {
                PictureListRecordDefinition definition = new PictureListRecordDefinition(buffer, index);
                _recordDefinitions.Add(definition);
            }

            for (; index + _entryLength <= buffer.Length; index += _entryLength)
            {
                PictureListEntry entry = new PictureListEntry(buffer, index); // pass _entryLength?
                Add(entry);
            }
        }

        // Load?
        // ToArray/CopyTo?
    }
}
