namespace UAM.Optics.LightField.Lytro.Net
{
    using System.IO;
    using System.Text;

    /// <summary>
    /// Represents a network request to the Lytro camera.
    /// </summary>
    public partial class LytroRequest
    {
        private LytroRawMessage _message;

        private LytroRequest(ulong commandParams, ulong additionalParams, byte[] content)
        {
            _message = new LytroRawMessage();
            _message.CommandAndParameters = commandParams;
            _message.AdditionalParameters = additionalParams;
            _message.Content = content;
        }

        /// <summary>
        /// Gets or sets the request command.
        /// </summary>
        public LytroCommand Command
        {
            get { return (LytroCommand)_message.CommandAndParameters; }
            set { _message.CommandAndParameters = (ulong)value; }
        }

        /// <summary>
        /// Gets or sets the request content.
        /// </summary>
        public byte[] Content
        {
            get { return _message.Content; }
            set { _message.Content = value; }
        }

        /// <summary>
        /// Gets the content length.
        /// </summary>
        public int ContentLength
        {
            get { return _message.Content == null ? 0 : _message.Content.Length; }
        }

        /// <summary>
        /// Returns a response to this request.
        /// </summary>
        /// <param name="stream">The <see cref="Stream" /> to send the request and receive the response over.</param>
        /// <param name="bufferLength">For requests without content, the number of bytes expected in the response.</param>
        /// <returns>a <see cref="LytroResponse"/> that contains the response to this request.</returns>
        /// <remarks>
        /// If a request <see cref="Content"/> is set, the <paramref name="bufferLength"/> parameter is ignored and <see cref="LytroRawMessage.Length"/> is set to the size of the content before sending the request.
        /// If <see cref="Content"/> is null, the <see cref="LytroRawMessage.Length"/> is set to <paramref name="bufferLength"/> value and <see cref="LytroRawMessage.NoContent"/> flag is set before sending the request.
        /// </remarks>
        public LytroResponse GetResponse(Stream stream, int bufferLength = 0)
        {
            if (_message.Content != null)
            {
                _message.Length = _message.Content.Length;
            }
            else
            {
                _message.Flags |= LytroRawMessage.NoContent;
                _message.Length = bufferLength;
            }

            _message.WriteTo(stream);

            LytroRawMessage responseMessage = LytroRawMessage.ReadFrom(stream);
            return new LytroResponse(responseMessage);
        }

        /// <summary>
        /// Initializes a new <see cref="LytroRequest"/> for the specified command with no content.
        /// </summary>
        /// <param name="command">The command of the request.</param>
        /// <returns>a <see cref="LytroRequest"/> with initialized <see cref="Command"/> property.</returns>
        public static LytroRequest Create(LytroCommand command)
        {
            return Create((ulong)command, 0U, null);
        }
        /// <summary>
        /// Initializes a new <see cref="LytroRequest"/> for the specified command with content from string.
        /// </summary>
        /// <param name="command">The command of the request.</param>
        /// <param name="content">The request content.</param>
        /// <returns>a <see cref="LytroRequest"/> with initialized <see cref="Command"/> and <see cref="Content"/> properties.</returns>
        /// <remarks>
        /// <see cref="Encoding.UTF8"/> is used when converting <paramref name="content"/> to a byte array.
        /// </remarks>
        public static LytroRequest Create(LytroCommand command, string content)
        {
            return Create((ulong)command, 0U, Encoding.UTF8.GetBytes(content));
        }
        /// <summary>
        /// Initializes a new <see cref="LytroRequest"/> for the specified command with content.
        /// </summary>
        /// <param name="command">The command of the request.</param>
        /// <param name="content">The request content.</param>
        /// <returns>a <see cref="LytroRequest"/> with initialized <see cref="Command"/> and <see cref="Content"/> properties.</returns>
        public static LytroRequest Create(LytroCommand command, params byte[] content)
        {
            return Create((ulong)command, 0U, content);
        }
        /// <summary>
        /// Initializes a new <see cref="LytroRequest"/> for the specified command with content and additional parameters.
        /// </summary>
        /// <param name="commandParams">The first 8 bytes of command and parameters.</param>
        /// <param name="additionalParams">The second 8 bytes of command and parameters.</param>
        /// <param name="content">The request content.</param>
        /// <returns>a <see cref="LytroRequest"/> with initialized <see cref="Command"/> and <see cref="Content"/> properties.</returns>
        public static LytroRequest Create(ulong commandParams, ulong additionalParams, byte[] content)
        {
            return new LytroRequest(commandParams, additionalParams, content);
        }

        /// <summary>
        /// Initializes a new <see cref="LytroRequest"/> for the <see cref="LytroCommand.Download"/> command additional parameters.
        /// </summary>
        /// <param name="offset">The offset at which downloading starts.</param>
        /// <returns>a <see cref="LytroRequest"/> with initialized <see cref="Command"/> and <see cref="Content"/> properties.</returns>
        public static LytroRequest CreateDownload(int offset)
        {
            ulong commandParams = (ulong)LytroCommand.Download | ((ulong)offset << 24);

            return Create(commandParams, 0, null);
        }
    }
}
