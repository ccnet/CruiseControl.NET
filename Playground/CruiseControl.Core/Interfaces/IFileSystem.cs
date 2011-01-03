namespace CruiseControl.Core.Interfaces
{
    using System.IO;

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

        #region OpenFileForRead()
        /// <summary>
        /// Opens the file for read.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        /// The <see cref="Stream"/> containing the file data.
        /// </returns>
        Stream OpenFileForRead(string filePath);
        #endregion
        #endregion
    }
}
