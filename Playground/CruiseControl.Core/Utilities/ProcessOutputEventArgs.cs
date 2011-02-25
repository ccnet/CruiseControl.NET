namespace CruiseControl.Core.Utilities
{
    using System;

    /// <summary>
    /// Arguments for when output has been received from an external process.
    /// </summary>
    public class ProcessOutputEventArgs
        : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessOutputEventArgs" /> class.	
        /// </summary>
        /// <param name="outputType">Type of the output.</param>
        /// <param name="data">The data.</param>
        /// <remarks></remarks>
        public ProcessOutputEventArgs(ProcessOutputType outputType, string data)
        {
            this.OutputType = outputType;
            this.Data = data;
        }
        #endregion

        #region Public proeprties
        #region OutputType
        /// <summary>
        /// Gets or sets the type of the output.	
        /// </summary>
        /// <value>The type of the output.</value>
        /// <remarks></remarks>
        public ProcessOutputType OutputType { get; private set; }
        #endregion

        #region Data
        /// <summary>
        /// Gets or sets the data.	
        /// </summary>
        /// <value>The data.</value>
        /// <remarks></remarks>
        public string Data { get; private set; }
        #endregion
        #endregion
    }
}
