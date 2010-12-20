namespace CruiseControl.Core.Interfaces
{
    /// <summary>
    /// Exposes functionality for working with the file system.
    /// </summary>
    public interface IFileSystem
    {
        #region Public methods
        #region CheckIfFileExists()
        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>
        /// <c>true</c> if the file exists; <c>false</c> otherwise.
        /// </returns>
        bool CheckIfFileExists(string filename);
        #endregion
        #endregion
    }
}
