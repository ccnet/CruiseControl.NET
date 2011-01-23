namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Provides an interface for performing compression.
    /// </summary>
    public interface ICompressionService
    {
        #region Public methods
        #region CompressString()
        /// <summary>
        /// Compresses a string.
        /// </summary>
        /// <param name="value">The string to compress.</param>
        /// <returns>The compressed string.</returns>
        string CompressString(string value);
        #endregion

        #region ExpandString()
        /// <summary>
        /// Expands (de-compresses) a string.
        /// </summary>
        /// <param name="value">The string to expanded.</param>
        /// <returns>The expanded string.</returns>
        string ExpandString(string value);
        #endregion
        #endregion
    }
}
