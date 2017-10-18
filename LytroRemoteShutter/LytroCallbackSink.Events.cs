namespace UAM.Optics.LightField.Lytro.Net
{
    using System;
    using UAM.Optics.LightField.Lytro.Camera;

    /// <summary>
    /// Represents the base class for classes that contain callback data.
    /// </summary>
    public class CallbackArgs : EventArgs
    {
        private readonly DateTime _received;

        /// <summary>
        /// Gets the message timestamp.
        /// </summary>
        public DateTime Received { get { return _received; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackArgs"/> class.
        /// </summary>
        /// <param name="received">The message timestamp.</param>
        public CallbackArgs(DateTime received)
        {
            _received = received;
        }
    }

    /// <summary>
    /// Provides data for the <see cref="LytroCallbackSink.CallbackReceived"/> event.
    /// </summary>
    public class RawCallbackArgs : CallbackArgs
    {
        private readonly string[] _arguments;

        /// <summary>
        /// Gets the message name and parameters.
        /// </summary>
        public string[] Arguments { get { return _arguments; } }

        /// <summary>
        /// Gets or sets whether the callback message was handled.
        /// </summary>
        /// <remarks>Setting the message as handled will prevent its further processing.</remarks>
        public bool Handled { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawCallbackArgs"/> class.
        /// </summary>
        /// <param name="received">The callback timestamp.</param>
        /// <param name="args">The message name and parameters.</param>
        public RawCallbackArgs(DateTime received, params string[] args) : base(received)
        {
            _arguments = args;
        }
    }

    /// <summary>
    /// Provides callback data camera mode messages.
    /// </summary>
    public class CameraModeChangedCallbackArgs : CallbackArgs
    {
        private readonly CameraMode _cameraMode;

        /// <summary>
        /// Gets the new camera mode.
        /// </summary>
        public CameraMode CameraMode { get { return _cameraMode; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="CameraModeChangedCallbackArgs"/> class.
        /// </summary>
        /// <param name="received">The callback timestamp.</param>
        /// <param name="cameraMode">The new camera mode.</param>
        public CameraModeChangedCallbackArgs(DateTime received, CameraMode cameraMode) : base(received)
        {
            _cameraMode = cameraMode;
        }
    }

    /// <summary>
    /// Provides callback data for individual picture related messages.
    /// </summary>
    public class PictureCallbackArgs : CallbackArgs
    {
        private readonly string _pictureID;

        /// <summary>
        /// Gets the picture identifier.
        /// </summary>
        public string PictureID { get { return _pictureID; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureCallbackArgs"/> class.
        /// </summary>
        /// <param name="received">The callback timestamp.</param>
        /// <param name="pictureID">The picture identifier.</param>
        public PictureCallbackArgs(DateTime received, string pictureID) : base(received)
        {
            _pictureID = pictureID;
        }
    }

    /// <summary>
    /// Provides data for the <see cref="LytroCallbackSink.LikedChanged"/> event.
    /// </summary>
    public class LikedChangedCallbackArgs : PictureCallbackArgs
    {
        private readonly bool _isFavorite;

        /// <summary>
        /// Gets the new favorite state.
        /// </summary>
        public bool IsFavorite { get { return _isFavorite; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="LikedChangedCallbackArgs"/> class.
        /// </summary>
        /// <param name="received">The callback timestamp.</param>
        /// <param name="pictureID">The picture identifier.</param>
        /// <param name="isFavorite">true if the picture is now favorite; false otherwise.</param>
        public LikedChangedCallbackArgs(DateTime received, string pictureID, bool isFavorite) : base(received, pictureID)
        {
            _isFavorite = isFavorite;
        }
    }

    /// <summary>
    /// Provides data for the <see cref="LytroCallbackSink.IsoSensitivityChanged"/> event.
    /// </summary>
    public class IsoSensitivityChangedCallbackArgs : CallbackArgs
    {
        private readonly float _isoSensitivity;
        private readonly bool _isAutomatic;

        /// <summary>
        /// Gets the new ISO sensitivity.
        /// </summary>
        public float IsoSensitivity { get { return _isoSensitivity; } }

        /// <summary>
        /// Gets whether the ISO sensitivity is determined automatically.
        /// </summary>
        public bool IsAutomatic { get { return _isAutomatic; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsoSensitivityChangedCallbackArgs"/> class.
        /// </summary>
        /// <param name="received">The callback timestamp.</param>
        /// <param name="sensitivity">The new ISO sensitivity.</param>
        /// <param name="isAutomatic">true if the ISO sensitivity is determined automatically; false otherwise.</param>
        public IsoSensitivityChangedCallbackArgs(DateTime received, float sensitivity, bool isAutomatic) : base(received)
        {
            _isoSensitivity = sensitivity;
            _isAutomatic = isAutomatic;
        }
    }

    /// <summary>
    /// Provides data for the <see cref="LytroCallbackSink.ShutterSpeedChanged"/> event.
    /// </summary>
    public class ShutterSpeedChangedCallbackArgs : CallbackArgs
    {
        private readonly float _shutterSpeed;
        private readonly bool _isAutomatic;

        /// <summary>
        /// Gets the new shutter speed in seconds.
        /// </summary>
        public float ShutterSpeed { get { return _shutterSpeed; } }

        /// <summary>
        /// Gets whether the shutter speed is determined automatically.
        /// </summary>
        public bool IsAutomatic { get { return _isAutomatic; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShutterSpeedChangedCallbackArgs"/> class.
        /// </summary>
        /// <param name="received">The callback timestamp.</param>
        /// <param name="speed">The new shutter speed in seconds.</param>
        /// <param name="isAutomatic">true if the shutter speed is determined automatically; false otherwise.</param>
        public ShutterSpeedChangedCallbackArgs(DateTime received, float speed, bool isAutomatic) : base(received)
        {
            _shutterSpeed = speed;
            _isAutomatic = isAutomatic;
        }
    }

    /// <summary>
    /// Provides data for the <see cref="LytroCallbackSink.ZoomLevelChanged"/> event.
    /// </summary>
    public class ZoomLevelChangedCallbackArgs : CallbackArgs
    {
        private readonly float _zoomLevel;

        /// <summary>
        /// Gets the new zoom level.
        /// </summary>
        public float ZoomLevel { get { return _zoomLevel; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomLevelChangedCallbackArgs"/> class.
        /// </summary>
        /// <param name="received">The callback timestamp.</param>
        /// <param name="zoom">The new zoom level.</param>
        public ZoomLevelChangedCallbackArgs(DateTime received, float zoom) : base(received)
        {
            _zoomLevel = zoom;
        }
    }

    /// <summary>
    /// Provides callback data for self-timer messages.
    /// </summary>
    public class SelfTimerCallbackArgs : CallbackArgs
    {
        private readonly int _seconds;
        private readonly bool _isCancelled;

        /// <summary>
        /// Gets the number of seconds remaining.
        /// </summary>
        public int Seconds { get { return _seconds; } }

        /// <summary>
        /// Gets whether the self-timer was cancelled.
        /// </summary>
        public bool IsCancelled { get { return _isCancelled; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelfTimerCallbackArgs"/> class.
        /// </summary>
        /// <param name="received">The callback timestamp.</param>
        /// <param name="seconds">The number of seconds remaining.</param>
        /// <param name="isCancelled">true if the self-timer was cancelled; false otherwise.</param>
        public SelfTimerCallbackArgs(DateTime received, int seconds, bool isCancelled) : base(received)
        {
            _seconds = seconds;
            _isCancelled = isCancelled;
        }
    }

    /// <summary>
    /// Provides callback data for state change messages.
    /// </summary>
    public class StateChangedCallbackArgs : CallbackArgs
    {
        private readonly bool _isEnabled;

        /// <summary>
        /// Gets the new state.
        /// </summary>
        public bool IsEnabled { get { return _isEnabled; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateChangedCallbackArgs"/> class.
        /// </summary>
        /// <param name="received">The callback timestamp.</param>
        /// <param name="isEnabled">The new state.</param>
        public StateChangedCallbackArgs(DateTime received, bool isEnabled) : base(received)
        {
            _isEnabled = isEnabled;
        }
    }
 
    /// <summary>
    /// Represents the method that will handle an callback event when the callback provides data.
    /// </summary>
    /// <typeparam name="TCallbackArgs">The type of the callback data generated by the event.</typeparam>
    /// <param name="sender">The source of the event.</param>
    /// <param name="c">An object that contains the callback data.</param>
    public delegate void CallbackEventHandler<TCallbackArgs>(object sender, TCallbackArgs c) where TCallbackArgs : CallbackArgs;
}
