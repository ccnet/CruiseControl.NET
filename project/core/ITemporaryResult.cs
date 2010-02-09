namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// A result that is only temporary for the duration of the build.
    /// </summary>
    public interface ITemporaryResult
    {
        #region Public methods
        #region CleanUp()
        /// <summary>
        /// Clean up the result when it is no longer needed.
        /// </summary>
        void CleanUp();
        #endregion
        #endregion
    }
}
