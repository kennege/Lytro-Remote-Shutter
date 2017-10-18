namespace UAM.Optics.LightField.Lytro.Net
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Represents a network response from the Lytro camera.
    /// </summary>
    public partial class LytroResponse
    {
        private LytroRawMessage _message;

        internal LytroResponse(LytroRawMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            _message = message;
        }

        /// <summary>
        /// Gets the response content length.
        /// </summary>
        public int ContentLength
        {
            get { return _message.Length; }
        }

        /// <summary>
        /// Gets whether the response has any content data.
        /// </summary>
        public bool HasContent
        {
            get { return _message.Length > 0 && _message.Content != null; }
        }

        /// <summary>
        /// Gets the response content data.
        /// </summary>
        public byte[] Content
        {
            get { return _message.Content; }
        }

        /// <summary>
        /// Gets the response content data as <see cref="Int32"/> value.
        /// </summary>
        /// <returns>an <see cref="Int32"/> value from the beginning of content data.</returns>
        /// <exception cref="InvalidOperationException">The response does not have enough data to contain a <see cref="Int32"/> value.</exception>
        public int GetContentAsInt32()
        {
            if (_message.Content == null || _message.Content.Length < 4)
                throw new InvalidOperationException();

            return
                _message.Content[3] << 24 |
                _message.Content[2] << 16 |
                _message.Content[1] << 8 |
                _message.Content[0];
        }

        /// <summary>
        /// Gets the response content data as a <see cref="String"/>.
        /// </summary>
        /// <returns>a <see cref="String"/> representing the content data.</returns>
        /// <remarks><see cref="Encoding.UTF8"/> is used when converting content data to <see cref="String"/>.</remarks>
        // InvalidOperationException cannot be thrown unless class made publicly constructible.
        public string GetContentAsString()
        {
            if (_message.Content == null)
                throw new InvalidOperationException();

            return Encoding.UTF8.GetString(_message.Content, 0, _message.Content.Length);
        }

        /// <summary>
        /// Gets the response content data as a <see cref="MemoryStream" />.
        /// </summary>
        /// <returns>the content data as a <see cref="MemoryStream"/>.</returns>
        public MemoryStream GetContentStream()
        {
            if (_message.Content == null)
                throw new InvalidOperationException();

            return new MemoryStream(_message.Content);
        }
    }
}
