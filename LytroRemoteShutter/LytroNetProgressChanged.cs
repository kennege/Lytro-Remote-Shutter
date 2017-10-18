namespace UAM.Optics.LightField.Lytro.Net
{
    using System;

    /// <summary>
    /// Provides data for the <see cref="LytroNetClientPortableBase.ProgressChanged" /> event.
    /// </summary>
    public class LytroNetProgressChangedEventArgs : EventArgs
    {
        private int _bytesTransferred;
        private int _totalBytesToTransfer;
        private bool _cancel;

        /// <summary>
        /// Gets the number of bytes transferred.
        /// </summary>
        public int BytesTransferred
        {
            get { return _bytesTransferred; }
        }

        /// <summary>
        /// Gets the total number of bytes to be transferred.
        /// </summary>
        public int TotalBytesToTransfer
        {
            get { return _totalBytesToTransfer; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the operation should be canceled.
        /// </summary>
        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LytroNetProgressChangedEventArgs"/> class.
        /// </summary>
        /// <param name="bytesTransferred">The number of bytes transferred.</param>
        /// <param name="totalBytesToTansfer">The total number of bytes to be transferred.</param>
        public LytroNetProgressChangedEventArgs(int bytesTransferred, int totalBytesToTansfer)
        {
            _bytesTransferred = bytesTransferred;
            _totalBytesToTransfer = totalBytesToTansfer;
        }
    }

    /// <summary>
    /// Represents the method that will handle the <see cref="LytroNetClientPortableBase.ProgressChanged"/> event.
    /// </summary>
    /// <param name="sender">The source of the event. </param>
    /// <param name="e">A <see cref="LytroNetProgressChangedEventArgs" /> that contains the event data.</param>
    public delegate void LytroNetProgressChangedHandler(object sender, LytroNetProgressChangedEventArgs e);
}
