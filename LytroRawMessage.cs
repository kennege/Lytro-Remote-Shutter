namespace UAM.Optics.LightField.Lytro.Net
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents a raw message sent between a Lytro camera and its client over network.
    /// </summary>
    public partial class LytroRawMessage
    {
        /// <summary>
        /// When set, the message is a response message. 
        /// When cleared, the message is a request message.
        /// </summary>
        public const int IsResponse = 0x00000002;
        /// <summary>
        /// When set, the message has no content and the <see cref="Length" /> denotes a buffer size (maximum allowed content length for the reply).
        /// When cleared, the <see cref="Length"/> denotes the size of the message's content (payload).
        /// </summary>
        public const int NoContent = 0x00000001;

        private const int HeaderLength = 7 * 4;
        private const int MagicValue = unchecked((int)0xfaaa55af);
        
        private const int MagicOffset = 0;
        private const int LengthOffset = 4;
        private const int FlagsOffset = 8;
        private const int CommandOffset = 12;
        private const int ParametersOffset = 20;

        private readonly byte[] _header;
        private byte[] _content;

        /// <summary>
        /// Initializes a new instance of the <see cref="LytroRawMessage"/> class.
        /// </summary>
        public LytroRawMessage()
        {
            _header = new byte[HeaderLength];
            Write(_header, MagicOffset, MagicValue);

            _content = new byte[0];
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LytroRawMessage"/> class with specified content.
        /// </summary>
        /// <param name="data">The message content.</param>
        public LytroRawMessage(params byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length < HeaderLength)
                throw new ArgumentException("Not enough data.");

            if (MagicValue != ReadInt32(data, MagicOffset))
                throw new ArgumentException("Invalid data.");

            _header = new byte[HeaderLength];
            Array.Copy(data, 0, _header, 0, _header.Length);

            _content = new byte[data.Length - HeaderLength];
            Array.Copy(data, HeaderLength, _content, 0, _content.Length);
        }
        private LytroRawMessage(byte[] header, byte[] content)
        {
            _header = header;
            _content = content;

            if (MagicValue != ReadInt32(header, MagicOffset))
                throw new ArgumentException("Invalid data.");
        }

        /// <summary>
        /// Gets or sets the message or buffer length.
        /// </summary>
        public int Length
        {
            get { return ReadInt32(_header, LengthOffset); }
            set { Write(_header, LengthOffset, value); }
        }
        /// <summary>
        /// Gets or sets the message flags.
        /// </summary>
        public int Flags
        {
            get { return ReadInt32(_header, FlagsOffset); }
            set { Write(_header, FlagsOffset, value); }
        }

        /// <summary>
        /// Gets or sets the first 8 bytes of command and parameters.
        /// </summary>
        public ulong CommandAndParameters
        {
            get { return ReadUInt64(_header, CommandOffset); }
            set { Write(_header, CommandOffset, value); }
        }
        /// <summary>
        /// Gets or sets the second 8 bytes of command and parameters.
        /// </summary>
        public ulong AdditionalParameters
        {
            get { return ReadUInt64(_header, ParametersOffset); }
            set { Write(_header, ParametersOffset, value); }
        }

        /// <summary>
        /// Gets or sets the message content.
        /// </summary>
        public byte[] Content
        {
            get { return _content; }
            set { _content = value; }
        }

        /// <summary>
        /// Reads a <see cref="Int32"/> value from buffer at specified offset.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="index">The offset at which the <see cref="Int32"/> value starts.</param>
        /// <returns>a <see cref="Int32"/> value from buffer at specified offset.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is negative or larger than size of the <paramref name="buffer"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="buffer"/> does not contain enough bytes starting at <paramref name="index"/> to hold an <see cref="Int32"/> value.</exception>
        protected static int ReadInt32(byte[] buffer, int index)
        {
            return (int)Read(buffer, index, sizeof(int));
        }
        /// <summary>
        /// Reads a <see cref="UInt64"/> value from buffer at specified offset.
        /// </summary>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="index">The offset at which the <see cref="Int32"/> value starts.</param>
        /// <returns>a <see cref="UInt64"/> value from buffer at specified offset.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is negative or larger than size of the <paramref name="buffer"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="buffer"/> does not contain enough bytes starting at <paramref name="index"/> to hold an <see cref="UInt64"/> value.</exception>
        protected static ulong ReadUInt64(byte[] buffer, int index)
        {
            return (ulong)Read(buffer, index, sizeof(ulong));
        }
        private static ulong Read(byte[] buffer, int index, int length)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (index < 0 || index > buffer.Length)
                throw new ArgumentOutOfRangeException("index");

            if (buffer.Length - length < 0)
                throw new ArgumentException("Buffer too small.");

            ulong value = 0;
            while (length-- > 0)
            {
                value <<= 8;
                value |= buffer[index + length];
            }

            return value;
        }

        /// <summary>
        /// Writes a <see cref="Int32"/> value to a buffer at specified offset.
        /// </summary>
        /// <param name="buffer">The buffer to write to.</param>
        /// <param name="index">The offset at which the <see cref="Int32"/> value starts.</param>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is negative or larger than size of the <paramref name="buffer"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="buffer"/> does not contain enough bytes starting at <paramref name="index"/> to hold an <see cref="Int32"/> value.</exception>
        protected static void Write(byte[] buffer, int index, int value)
        {
            Write(buffer, index, (ulong)value, sizeof(int));
        }
        /// <summary>
        /// Writes a <see cref="UInt64"/> value to a buffer at specified offset.
        /// </summary>
        /// <param name="buffer">The buffer to write to.</param>
        /// <param name="index">The offset at which the <see cref="Int32"/> value starts.</param>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is negative or larger than size of the <paramref name="buffer"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="buffer"/> does not contain enough bytes starting at <paramref name="index"/> to hold an <see cref="UInt64"/> value.</exception>
        protected static void Write(byte[] buffer, int index, ulong value)
        {
            Write(buffer, index, value, sizeof(ulong));
        }
        private static void Write(byte[] buffer, int index, ulong value, int length)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            if (index < 0 || index > buffer.Length)
                throw new ArgumentOutOfRangeException("index");

            if (buffer.Length - length < 0)
                throw new ArgumentException("Buffer too small.");

            index += length - 1;
            while (length-- > 0)
            {
                buffer[index - length] = (byte)value;
                value >>= 8;
            }
        }

        /// <summary>
        /// Reads a <see cref="LytroRawMessage"/> from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read the message from.</param>
        /// <returns>a new instance of the <see cref="LytroRawMessage"/> class representing the message received.</returns>
        /// <remarks>This is a blocking method.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="EndOfStreamException">End of <paramref name="stream"/> was reached without receiving complete message.</exception>
        public static LytroRawMessage ReadFrom(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            byte[] header = new byte[HeaderLength];
            byte[] payload = new byte[0];
            int offset = 0;

            while (offset < header.Length)
            {
                int read = stream.Read(header, offset, header.Length - offset);

                TraceReadHeader(stream, header, offset, read);

                if (read == 0)
                    throw new EndOfStreamException();
                
                offset += read;
            }

            int length = ReadInt32(header, LengthOffset);
            if (length > 0)
            {
                payload = new byte[length];
                offset = 0;

                while (offset < length)
                {
                    int read = stream.Read(payload, offset, payload.Length - offset);

                    TraceReadData(stream, payload, offset, read);

                    if (read == 0)
                        throw new EndOfStreamException();

                    offset += read;
                }
            }

            return new LytroRawMessage(header, payload);
        }

        /// <summary>
        /// Writes a <see cref="LytroRawMessage"/> to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write to.</param>
        /// <remarks>This is a blocking method.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        public void WriteTo(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (_header != null)
            {
                TraceWriteHeader(stream, _header, 0, _header.Length);
                stream.Write(_header, 0, _header.Length);
            }

            if (_content != null)
            {
                TraceWriteData(stream, _content, 0, _content.Length);
                stream.Write(_content, 0, _content.Length);
            }
        }

        /// <summary>
        /// Gets a byte array representing this <see cref="LytroRawMessage" />.
        /// </summary>
        /// <returns>a byte array representing this <see cref="LytroRawMessage" />.</returns>
        public byte[] ToArray()
        {
            byte[] data = new byte[_header.Length + _content.Length];

            Array.Copy(_header, 0, data, 0, _header.Length);
            Array.Copy(_content, 0, data, _header.Length, _content.Length);

            return data;
        }


        private void TraceWriteHeader(Stream stream, byte[] buffer, int offset, int count)
        {
            Trace(this, stream, buffer, offset, count, TraceHeader, true);
        }
        private static void TraceReadHeader(Stream stream, byte[] buffer, int offset, int count)
        {
            Trace(null, stream, buffer, offset, count, TraceHeader, false);
        }
        private void TraceWriteData(Stream stream, byte[] buffer, int offset, int count)
        {
            Trace(this, stream, buffer, offset, count, TraceData, true);
        }
        private static void TraceReadData(Stream stream, byte[] buffer, int offset, int count)
        {
            Trace(null, stream, buffer, offset, count, TraceData, false);
        }
        private static void Trace(object @this, Stream stream, byte[] buffer, int offset, int count, TraceEventHandler handler, bool isWrite)
        {
            if (handler != null)
                handler(@this, new TraceEventArgs(stream, buffer, offset, count, isWrite));
        }

        /// <summary>
        /// Invoked after read or before write operation for purposes of tracing the headers being transfered.
        /// </summary>
        public static event TraceEventHandler TraceHeader;

        /// <summary>
        /// Called after read or before write opearion for purposes of tracing the data being transfered.
        /// </summary>
        public static event TraceEventHandler TraceData;

        /// <summary>
        /// Represents methods that will handle the trace events.
        /// </summary>
        /// <param name="sender">The source of the event if available.</param>
        /// <param name="e">A <see cref="TraceEventHandler" /> that contains the event data.</param>
        /// <remarks>
        /// The <paramref name="sender"/> is set to an instance of <see cref="LytroRawMessage"/> when writing to the stream.
        /// When called during reading a message, the class is not created yet and thus the <paramref name="sender"/> is null.
        /// </remarks>
        public delegate void TraceEventHandler(object sender, TraceEventArgs e);

        /// <summary>
        /// Provides data for the tracing events.
        /// </summary>
        public class TraceEventArgs : EventArgs
        {
            private readonly bool _isWriteTrace;
            /// <summary>
            /// Gets whether this is a write operation.
            /// </summary>
            public bool IsWriteTrace { get { return _isWriteTrace; } }

            /// <summary>
            /// Read-only. The stream the data is being read from or write to.
            /// </summary>
            public readonly Stream Stream;
            /// <summary>
            /// Read-only. The buffer containing the traced data.
            /// </summary>
            public readonly byte[] Buffer;
            /// <summary>
            /// Read-only. The offset at which the traced data start.
            /// </summary>
            public readonly int Offset;
            /// <summary>
            /// Read-only. The number of bytes traced.
            /// </summary>
            public readonly int Count;

            /// <summary>
            /// Initializes a new instance of the <see cref="TraceEventArgs"/> class.
            /// </summary>
            /// <param name="stream">The stream the data is being read from or write to.</param>
            /// <param name="buffer">The buffer containing the traced data.</param>
            /// <param name="offset">The offset at which the traced data start.</param>
            /// <param name="count">The number of bytes traced.</param>
            /// <param name="isWriteTrace">true if a write operation is being traced; false otherwise.</param>
            public TraceEventArgs(Stream stream, byte[] buffer, int offset, int count, bool isWriteTrace)
            {
                Stream = stream;
                Buffer = buffer;
                Offset = offset;
                Count = count;

                _isWriteTrace = isWriteTrace;
            }
        }
    }
}
