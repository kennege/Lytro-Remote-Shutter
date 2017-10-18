namespace UAM.Optics.LightField.Lytro.Net
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using UAM.Optics.LightField.Lytro.Camera;

    /// <summary>
    /// Processes callback messages from the Lytro camera.
    /// </summary>
    public partial class LytroCallbackSink
    {
        private const int EventCount = 12;

        private const string IDAllPicturesDeleted = "[AllPicturesDeleted]";
        private const string IDCreativeModeChanged = "[CreativeModeChanged]";
        private const string IDHeartbeatTick = "[HeartbeatTick]";
        private const string IDIsoSensitivityChanged = "[IsoSensitivityChanged]";
        private const string IDLikedChanged = "[LikedChanged]";
        private const string IDNeutralDensityFilterChanged = "[NeutralDensityFilterChanged]";
        private const string IDNewPictureAvailable = "[NewPictureAvailable]";
        private const string IDPictureDeleted = "[PictureDeleted]";
        private const string IDSelfTimerTick = "[SelfTimerTick]";
        private const string IDShutterPressed = "[ShutterPressed]";
        private const string IDShutterSpeedChanged = "[ShutterSpeedChanged]";
        private const string IDZoomLevelChanged = "[ZoomLevelChanged]";

        private class EventInfo
        {
            public Delegate Handler;
            public readonly CallbackEventDelegate RaiseMethod;

            public EventInfo(Delegate handler, CallbackEventDelegate raiseMethod)
            {
                Handler = handler;
                RaiseMethod = raiseMethod;
            }
        }

        private volatile bool _isProcessing;
        private Dictionary<string, EventInfo> _events;
        private ManualResetEvent _processingDone;
        
        #pragma warning disable 649
        private IDisposable _stopDisposable;
        #pragma warning restore 649

        /// <summary>
        /// Gets whether a callback stream is currently processed.
        /// </summary>
        public bool IsProcessing
        {
            get { return _processingDone.WaitOne(0) == false; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LytroCallbackSink"/> class using default settings. 
        /// </summary>
        public LytroCallbackSink()
        {
            _events = new Dictionary<string, EventInfo>(EventCount, StringComparer.Ordinal);
            _processingDone = new ManualResetEvent(true);
        }

        /// <summary>
        /// Starts processing of a callback stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to be processed.</param>
        /// <remarks>This method is blocking and returns only when the stream is closed, throws an exception or the processing is stopped using the <see cref="StopProcessing"/> method.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="InvalidOperationException">A callback stream is already being processed.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> is not readable.</exception>
        public void Process(Stream stream)
        {
            Process(stream, true);
        }

        private void Process(Stream stream, bool checkProcessing)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            
            if (!stream.CanRead)
                throw new ArgumentException("Stream is not readable.", "stream");

            if (checkProcessing)
                SetProcessingOrThrow();

            byte[] buffer = new byte[80];
            int offset = 0;

            try
            {
                while (_isProcessing)
                {
                    int read = stream.Read(buffer, offset, buffer.Length - offset);

                    if (read == 0)
                        return;

                    Process(ref buffer, ref offset, read);
                }
            }
            finally
            {
                _isProcessing = false;
                _processingDone.Set();
            }
        }

        private void SetProcessingOrThrow()
        {
            if (!_isProcessing && _processingDone.WaitOne(0))
                lock (_processingDone)
                    if (!_isProcessing)
                    {
                        _isProcessing = true;
                        _processingDone.Reset();
                        return;
                    }

            throw new InvalidOperationException("Callbacks are already being processed.");
        }

        private void Process(ref byte[] buffer, ref int offset, int read)
        {
            while (_isProcessing)
            {
                int zeroIndex = Array.IndexOf<byte>(buffer, 0, offset, read);

                if (zeroIndex < 0)
                {
                    offset += read;

                    if (offset == buffer.Length)
                    {
                        byte[] biggerBuffer = new byte[buffer.Length * 2];
                        buffer.CopyTo(biggerBuffer, 0);
                        buffer = biggerBuffer;
                    }
                    break;
                }

                int lineLength = zeroIndex;
                if (lineLength >= 2 && buffer[lineLength - 1] == 10 && buffer[lineLength - 2] == 13)
                    lineLength -= 2;

                string eventLine = Encoding.UTF8.GetString(buffer, 0, lineLength);
                OnEventLineReceived(eventLine);

                read += offset - (zeroIndex + 1);
                offset = 0;
                if (read == 0)
                    break;

                Array.Copy(buffer, zeroIndex + 1, buffer, 0, read);
            }
        }

        /// <summary>
        /// Stops the processing of a callback stream and waits until it is stopped. 
        /// </summary>
        /// <remarks>The processing is stopped co-operatively, i.e. the callback stream must return from the read request in order for the process to be stopped.</remarks>
        public void StopProcessing()
        {
            StopProcessingAsync();
            _processingDone.WaitOne();
        }

        /// <summary>
        /// Stops the processing of a callback stream. This method does not block the calling thread.
        /// </summary>
        /// <remarks>The processing is stopped co-operatively, i.e. the callback stream must return from the read request in order for the process to be stopped. You can check the current status with the <see cref="IsProcessing"/> property.</remarks>
        public void StopProcessingAsync()
        {
            _isProcessing = false;

            if (_stopDisposable != null)
                _stopDisposable.Dispose();
        }

        private void OnEventLineReceived(string line)
        {
            DateTime received = DateTime.Now;
            string[] args = line.Split(' ');

            if (args.Length > 0)
            {
                CallbackEventHandler<RawCallbackArgs> handler = CallbackReceived;
                if (handler != null)
                {
                    RawCallbackArgs c = new RawCallbackArgs(received, args);
                    handler(this, new RawCallbackArgs(received, args));
                    if (c.Handled)
                        return;
                }

                EventInfo info;
                if (_events.TryGetValue(args[0], out info))
                    info.RaiseMethod(info.Handler, received, args);
            }
        }

        private void AddHandler(string eventID, CallbackEventDelegate raiseMethod, Delegate handler)
        {
            EventInfo info;
            if (_events.TryGetValue(eventID, out info))
                info.Handler = MulticastDelegate.Combine(info.Handler, handler);
            else
                _events[eventID] = new EventInfo(handler, raiseMethod);

        }
        private void RemoveHandler(string eventID, Delegate handler)
        {
            EventInfo info;
            if (_events.TryGetValue(eventID, out info))
                info.Handler = MulticastDelegate.Remove(info.Handler, handler);
        }

        private void RaiseCallbackArgs(Delegate handler, DateTime received, params string[] args)
        {
            Raise(handler, new CallbackArgs(received));
        }
        private void RaiseCameraModeChangedCallbackArgs(Delegate handler, DateTime received, params string[] args)
        {
            if (args.Length > 1)
            {
                Raise(handler, new CameraModeChangedCallbackArgs(received, args[1] == "1" ? CameraMode.Creative : CameraMode.Everyday));
            }
        }
        private void RaiseIsoSensitivityChangedCallbackArgs(Delegate handler, DateTime received, params string[] args)
        {
            if (args.Length > 1)
            {
                float sensitivity;
                if (float.TryParse(args[1], NumberStyles.Float, CultureInfo.InvariantCulture, out sensitivity))
                    Raise(handler, new IsoSensitivityChangedCallbackArgs(received, sensitivity * 50f, sensitivity == 0f));
            }
        }
        private void RaiseLikedChangedCallbackArgs(Delegate handler, DateTime received, params string[] args)
        {
            if (args.Length > 2)
            {
                Raise(handler, new LikedChangedCallbackArgs(received, args[1], args[2] == "1"));
            }
        }
        private void RaiseStateChangedCallbackArgs(Delegate handler, DateTime received, params string[] args)
        {
            if (args.Length > 1)
            {
                Raise(handler, new StateChangedCallbackArgs(received, args[1] == "1"));
            }
        }
        private void RaisePictureCallbackArgs(Delegate handler, DateTime received, params string[] args)
        {
            if (args.Length > 1)
            {
                Raise(handler, new PictureCallbackArgs(received, args[1]));
            }
        }
        private void RaiseSelfTimerCallbackArgs(Delegate handler, DateTime received, params string[] args)
        {
            if (args.Length > 1)
            {
                int seconds = 0;
                bool isCancelled = args[1] == "Canceled";

                if (!isCancelled)
                    if (!int.TryParse(args[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out seconds))
                        return;

                Raise(handler, new SelfTimerCallbackArgs(received, seconds, isCancelled));
            }
        }
        private void RaiseShutterSpeedChangedCallbackArgs(Delegate handler, DateTime received, params string[] args)
        {
            if (args.Length > 1)
            {
                float speed;
                if (float.TryParse(args[1], NumberStyles.Float, CultureInfo.InvariantCulture, out speed))
                    Raise(handler, new ShutterSpeedChangedCallbackArgs(received, speed, speed == 0f));
            }
        }
        private void RaiseZoomLevelChangedCallbackArgs(Delegate handler, DateTime received, params string[] args)
        {
            if (args.Length > 1)
            {
                float zoom;
                if (float.TryParse(args[1], NumberStyles.Float, CultureInfo.InvariantCulture, out zoom))
                    Raise(handler, new ZoomLevelChangedCallbackArgs(received, zoom));
            }
        }

        private void Raise<T>(Delegate handler, T args) where T : CallbackArgs
        {
            CallbackEventHandler<T> typedHandler = (CallbackEventHandler<T>)handler;

            if (typedHandler != null)
                typedHandler(this, args);
        }

        /// <summary>
        /// Any callback message was received.
        /// </summary>
        public event CallbackEventHandler<RawCallbackArgs> CallbackReceived;
        /// <summary>
        /// Occurs when all pictures are deleted from the camera at once.
        /// </summary>
        public event CallbackEventHandler<CallbackArgs> AllPicturesDeleted
        {
            add { AddHandler(IDAllPicturesDeleted, RaiseCallbackArgs, value); }
            remove { RemoveHandler(IDAllPicturesDeleted, value); }
        }
        /// <summary>
        /// Occurs when the current shooting mode changes.
        /// </summary>
        public event CallbackEventHandler<CameraModeChangedCallbackArgs> CreativeModeChanged
        {
            add { AddHandler(IDCreativeModeChanged, RaiseCameraModeChangedCallbackArgs, value); }
            remove { RemoveHandler(IDCreativeModeChanged, value); }
        }
        /// <summary>
        /// Generated automatically approximately every 100 ms if no other callback occurs.
        /// </summary>
        public event CallbackEventHandler<CallbackArgs> HeartbeatTick
        {
            add { AddHandler(IDHeartbeatTick, RaiseCallbackArgs, value); }
            remove { RemoveHandler(IDHeartbeatTick, value); }
        }
        /// <summary>
        /// Occurs when the ISO sensitivity setting changes.
        /// </summary>
        public event CallbackEventHandler<IsoSensitivityChangedCallbackArgs> IsoSensitivityChanged
        {
            add { AddHandler(IDIsoSensitivityChanged, RaiseIsoSensitivityChangedCallbackArgs, value); }
            remove { RemoveHandler(IDIsoSensitivityChanged, value); }
        }
        /// <summary>
        /// Occurs when a user marks or unmarks a picture as a favorite.
        /// </summary>
        public event CallbackEventHandler<LikedChangedCallbackArgs> LikedChanged
        {
            add { AddHandler(IDLikedChanged, RaiseLikedChangedCallbackArgs, value); }
            remove { RemoveHandler(IDLikedChanged, value); }
        }
        /// <summary>
        /// Occurs when the neutral density filter is turned on or off.
        /// </summary>
        public event CallbackEventHandler<StateChangedCallbackArgs> NeutralDensityFilterChanged
        {
            add { AddHandler(IDNeutralDensityFilterChanged, RaiseStateChangedCallbackArgs, value); }
            remove { RemoveHandler(IDNeutralDensityFilterChanged, value); }
        }
        /// <summary>
        /// Occurs when a picture taken is rendered and becomes available for download.
        /// </summary>
        public event CallbackEventHandler<PictureCallbackArgs> NewPictureAvailable
        {
            add { AddHandler(IDNewPictureAvailable, RaisePictureCallbackArgs, value); }
            remove { RemoveHandler(IDNewPictureAvailable, value); }
        }
        /// <summary>
        /// Occurs when a picture is deleted from the camera.
        /// </summary>
        public event CallbackEventHandler<PictureCallbackArgs> PictureDeleted
        {
            add { AddHandler(IDPictureDeleted, RaisePictureCallbackArgs, value); }
            remove { RemoveHandler(IDPictureDeleted, value); }
        }
        /// <summary>
        /// Occurs every second during self timer count-down.
        /// </summary>
        public event CallbackEventHandler<SelfTimerCallbackArgs> SelfTimerTick
        {
            add { AddHandler(IDSelfTimerTick, RaiseSelfTimerCallbackArgs, value); }
            remove { RemoveHandler(IDSelfTimerTick, value); }
        }
        /// <summary>
        /// Occurs immediately after shutter is triggered.
        /// </summary>
        public event CallbackEventHandler<CallbackArgs> ShutterPressed
        {
            add { AddHandler(IDShutterPressed, RaiseCallbackArgs, value); }
            remove { RemoveHandler(IDShutterPressed, value); }
        }
        /// <summary>
        /// Occurs when the shutter speed setting changes.
        /// </summary>
        public event CallbackEventHandler<ShutterSpeedChangedCallbackArgs> ShutterSpeedChanged
        {
            add { AddHandler(IDShutterSpeedChanged, RaiseShutterSpeedChangedCallbackArgs, value); }
            remove { RemoveHandler(IDShutterSpeedChanged, value); }
        }
        /// <summary>
        /// Occurs when camera zoom is changed.
        /// </summary>
        public event CallbackEventHandler<ZoomLevelChangedCallbackArgs> ZoomLevelChanged
        {
            add { AddHandler(IDZoomLevelChanged, RaiseZoomLevelChangedCallbackArgs, value); }
            remove { RemoveHandler(IDZoomLevelChanged, value); }
        }

        private delegate void CallbackEventDelegate(Delegate handler, DateTime received, params string[] args);
    }
}