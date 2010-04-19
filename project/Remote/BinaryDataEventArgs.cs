namespace ThoughtWorks.CruiseControl.Remote
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Events args that contains binary data.
    /// </summary>
    public class BinaryDataEventArgs
        : AsyncCompletedEventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryDataEventArgs"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="error">The error.</param>
        /// <param name="cancelled">if set to <c>true</c> [cancelled].</param>
        /// <param name="userState">State of the user.</param>
        public BinaryDataEventArgs(byte[] data, Exception error, bool cancelled, object userState)
            : base(error, cancelled, userState)
        {
            this.Data = data;
        }
        #endregion

        #region Public properties
        #region Data
        /// <summary>
        /// Gets the binary data.
        /// </summary>
        /// <value>The data.</value>
        public byte[] Data { get; private set; }
        #endregion
        #endregion
    }
}
