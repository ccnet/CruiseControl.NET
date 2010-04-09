namespace ThoughtWorks.CruiseControl.Core.Distribution
{
    using System;
using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// A result from a remote build.
    /// </summary>
    public class RemoteBuildTaskResult
        : ITaskResult
    {
        #region Private fields
        private readonly IntegrationStatus status;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteBuildTaskResult"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        public RemoteBuildTaskResult(IntegrationStatus status)
        {
            this.status = status;
        }
        #endregion

        #region Public properties
        #region Data
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data from the result.</value>
        public string Data
        {
            get { throw new NotImplementedException(); }
        }
        #endregion
        #endregion

        #region Public methods
        #region CheckIfSuccess()
        /// <summary>
        /// Checks whether the result was successful.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the result was successful, <c>false</c> otherwise.
        /// </returns>
        public bool CheckIfSuccess()
        {
            return this.status == IntegrationStatus.Success;
        }
        #endregion
        #endregion
    }
}
