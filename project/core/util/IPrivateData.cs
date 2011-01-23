namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Defines a data item as having both "public" and "private" modes.
    /// </summary>
    public interface IPrivateData
    {
        #region Public methods
        #region ToString()
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance in the specified data mode.
        /// </summary>
        /// <param name="dataMode">The data mode to use.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        string ToString(SecureDataMode dataMode);
        #endregion
        #endregion
    }
}
