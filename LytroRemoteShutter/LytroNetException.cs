namespace UAM.Optics.LightField.Lytro.Net
{
    using System;

    /// <summary>
    /// Defines the base class for predefined exceptions in the <see cref="UAM.Optics.LightField.Lytro.Net" /> namespace.
    /// </summary>
    public abstract class LytroNetException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LytroNetException"/> class.
        /// </summary>
        public LytroNetException() { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LytroNetException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public LytroNetException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LytroNetException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        public LytroNetException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// The exception that is thrown when an error is made while using a network protocol.
    /// </summary>
    public class LytroNetProtocolViolationException : LytroNetException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LytroNetProtocolViolationException"/> class.
        /// </summary>
        public LytroNetProtocolViolationException() : base("Unexpected response.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LytroNetProtocolViolationException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public LytroNetProtocolViolationException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LytroNetProtocolViolationException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        public LytroNetProtocolViolationException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// The exception that is thrown upon cancellation of an operation.
    /// </summary>
    public class LytroNetCanceledException : LytroNetException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LytroNetCanceledException"/> class.
        /// </summary>
        public LytroNetCanceledException() : base("The command was canceled.") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LytroNetCanceledException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public LytroNetCanceledException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LytroNetCanceledException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        public LytroNetCanceledException(string message, Exception innerException) : base(message, innerException) { }
    }
}
