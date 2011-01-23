namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// A general task result.
    /// </summary>
    public class GeneralTaskResult
        : ITaskResult
    {
        #region Private fields
        private readonly string data;
        private readonly bool succeeded;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralTaskResult"/> class.
        /// </summary>
        /// <param name="succeeded">if set to <c>true</c> [succeeded].</param>
        /// <param name="data">The data.</param>
        public GeneralTaskResult(bool succeeded, string data)
        {
            this.succeeded = succeeded;
            this.data = data;
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
            get { return this.data; }
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
            return this.succeeded;
        }
        #endregion
        #endregion
    }
}
