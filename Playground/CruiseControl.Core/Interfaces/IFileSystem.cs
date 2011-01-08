namespace CruiseControl.Core.Interfaces
{
    using System.IO;
    using System.Xml;

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

        #region CreateXmlWriter()
        /// <summary>
        /// Creates a new XML writer.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>
        /// The new <see cref="XmlWriter"/>.
        /// </returns>
        XmlWriter CreateXmlWriter(string filename);
        #endregion

        #region EnsureFolderExists()
        /// <summary>
        /// Ensures that the folder exists.
        /// </summary>
        /// <param name="folder">The path to the folder.</param>
        void EnsureFolderExists(string folder);
        #endregion

        #region CopyFile()
        /// <summary>
        /// Copies a file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        void CopyFile(string source, string destination);
        #endregion

        #region MoveFile()
        /// <summary>
        /// Moves a file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        void MoveFile(string source, string destination);
        #endregion

        #region DeleteFile()
        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="file">The file.</param>
        void DeleteFile(string file);
        #endregion
        #endregion
    }
}
