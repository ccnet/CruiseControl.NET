namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// The result from a task.
    /// </summary>
    public interface ITaskResult
    {
        #region Public properties
        #region Data
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data from the result.</value>
        string Data { get; }
        #endregion
        #endregion

        #region Public methods
        #region CheckIfSuccess()
        /// <summary>
        /// Checks whether the result was successful.
        /// </summary>
        /// <returns><c>true</c> if the result was successful, <c>false</c> otherwise.</returns>
        bool CheckIfSuccess();
        #endregion
        #endregion
    }
}