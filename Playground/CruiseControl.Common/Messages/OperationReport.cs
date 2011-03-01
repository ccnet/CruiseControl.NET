namespace CruiseControl.Common.Messages
{
    using System.ComponentModel;

    /// <summary>
    /// Reports on the result of an operation.
    /// </summary>
    public class OperationReport
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationReport"/> class.
        /// </summary>
        public OperationReport()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationReport"/> class.
        /// </summary>
        /// <param name="success">Whether the operation was a success or not.</param>
        /// <param name="message">The message.</param>
        public OperationReport(bool success, string message)
        {
            this.WasSuccess = success;
            this.Message = message;
        }
        #endregion

        #region Public methods
        #region WasSuccess
        /// <summary>
        /// Gets or sets a value indicating whether the operation was a success.
        /// </summary>
        /// <value>
        /// <c>true</c> if the operation was a success; otherwise, <c>false</c>.
        /// </value>
        public bool WasSuccess { get; set; }
        #endregion

        #region Message
        /// <summary>
        /// Gets or sets any additional message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [DefaultValue(null)]
        public string Message { get; set; }
        #endregion
        #endregion
    }
}
