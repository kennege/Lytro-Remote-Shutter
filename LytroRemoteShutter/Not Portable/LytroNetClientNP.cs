namespace UAM.Optics.LightField.Lytro.Net
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// Provides common methods for sending data to and receiving data from a Lytro camera using <see cref="TcpClient" />.
    /// </summary>
    public partial class LytroNetClient : LytroNetClientPortableBase
    {
        /// <summary>
        /// Default Lytro camera network address.
        /// </summary>
        public const long DefaultIPAddress = 0x0101640a;
        /// <summary>
        /// Default Lytro camera command port.
        /// </summary>
        public const int DefaultCommandPort = 5678;
        /// <summary>
        /// Alternative Lytro camera command port.
        /// </summary>
        public const int SecondaryCommandPort = 5679;

        /// <summary>
        /// Gets the default Lytro commands endpoint.
        /// </summary>
        public static IPEndPoint DefaultEndPoint
        {
            get { return new IPEndPoint(DefaultIPAddress, DefaultCommandPort); }
        }

        private IPEndPoint _endpoint = DefaultEndPoint;

        /// <summary>
        /// Gets or sets the callback endpoint.
        /// </summary>
        /// <remarks>The value is effective at the time the processing is started.</remarks>
        public IPEndPoint EndPoint
        {
            get { return _endpoint; }
            set { _endpoint = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LytroNetClient"/> class with default settings.
        /// </summary>
        public LytroNetClient()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LytroNetClient"/> class using given endpoint.
        /// </summary>
        /// <param name="endpoint">The commands endpoint.</param>
        public LytroNetClient(IPEndPoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");

            _endpoint = endpoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LytroNetClient"/> class from given IP address and port.
        /// </summary>
        /// <param name="address">The IP address to connect to.</param>
        /// <param name="port">The command port to use.</param>
        public LytroNetClient(long address = DefaultIPAddress, int port = DefaultCommandPort) : this(new IPEndPoint(address, port))
        {

        }

        private Stream _commandStream;
        private TcpClient _commandClient;

        /// <summary>
        /// Establishes a new connection or returns an available one.
        /// </summary>
        /// <returns>a connected <see cref="Stream"/>.</returns>
        protected override Stream GetStream()
        {
            if (_commandStream == null || !_commandStream.CanRead || !_commandStream.CanWrite)
            {
                _commandClient = new TcpClient();
                _commandClient.Connect(_endpoint);
                _commandStream = _commandClient.GetStream();

                Raise(Connected);
            }

            return _commandStream;
        }

        /// <summary>
        /// Closes existing connection.
        /// </summary>
        /// <param name="e">The exception thrown.</param>
        /// <param name="stream">The stream in use when the exception occurred.</param>
        protected override void OnException(Exception e, Stream stream)
        {
            if (_commandClient != null)
                _commandClient.Close();

            Raise(Disconnected);
        }

        private void Raise(EventHandler handler)
        {
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when a connection to the camera was established.
        /// </summary>
        public event EventHandler Connected;

        /// <summary>
        /// Occurs when a connection to the camera was lost.
        /// </summary>
        public event EventHandler Disconnected;
    }
}
