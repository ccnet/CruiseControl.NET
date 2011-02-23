namespace CruiseControl.Common
{
    using System;

    /// <summary>
    /// An error occurred on the remote server.
    /// </summary>
    public class RemoteServerException
        : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteServerException"/> class.
        /// </summary>
        /// <param name="resultCode">The result code.</param>
        /// <param name="logId">The log id.</param>
        public RemoteServerException(RemoteResultCode resultCode, Guid? logId)
            : this(resultCode, logId, GenerateMessage(resultCode), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteServerException"/> class.
        /// </summary>
        /// <param name="resultCode">The result code.</param>
        /// <param name="logId">The log id.</param>
        /// <param name="message">The message.</param>
        public RemoteServerException(RemoteResultCode resultCode, Guid? logId, string message)
            : this(resultCode, logId, message, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteServerException"/> class.
        /// </summary>
        /// <param name="resultCode">The result code.</param>
        /// <param name="logId">The log id.</param>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public RemoteServerException(RemoteResultCode resultCode, Guid? logId, string message, Exception inner)
            : base(message, inner)
        {
            this.ResultCode = resultCode;
            this.LogId = logId;
        }
        #endregion

        #region Public properties
        #region ResultCode
        /// <summary>
        /// Gets the result code.
        /// </summary>
        public RemoteResultCode ResultCode { get; private set; }
        #endregion

        #region LogId
        /// <summary>
        /// Gets the log id.
        /// </summary>
        public Guid? LogId { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region GenerateMessage()
        /// <summary>
        /// Generates the message based on the result code.
        /// </summary>
        /// <param name="resultCode">The result code.</param>
        /// <returns>
        /// The result-code based message.
        /// </returns>
        public static string GenerateMessage(RemoteResultCode resultCode)
        {
            switch (resultCode)
            {
                case RemoteResultCode.InvalidInput:
                    return "An invalid request message was passed to the remote server";
                default:
                    return "An unknown error has occurred at the remote server, code: " + resultCode;
            }
        }
        #endregion
        #endregion
    }
}