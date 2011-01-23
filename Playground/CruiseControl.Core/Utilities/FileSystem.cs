namespace CruiseControl.Core.Utilities
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using CruiseControl.Core.Interfaces;
    using NLog;

    /// <summary>
    /// Implements the functionality for working with the file system.
    /// </summary>
    public class FileSystem
        : IFileSystem
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Public methods
        #region CheckIfFileExists()
        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>
        /// <c>true</c> if the file exists; <c>false</c> otherwise.
        /// </returns>
        public bool CheckIfFileExists(string filename)
        {
            logger.Trace("Checking if file '" + (filename ?? string.Empty) + "' exists");
            var exists = File.Exists(filename);
            return exists;
        }
        #endregion

        #region OpenFileForRead()
        /// <summary>
        /// Opens the file for read.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        /// The <see cref="Stream"/> containing the file data.
        /// </returns>
        public Stream OpenFileForRead(string filePath)
        {
            return File.Open(filePath, FileMode.Open);
        }
        #endregion

        #region OpenFileForWrite()
        /// <summary>
        /// Opens the file for writing.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        /// The <see cref="Stream"/> to write to.
        /// </returns>
        public Stream OpenFileForWrite(string filePath)
        {
            var stream = File.Open(filePath, FileMode.Create);
            return stream;
        }
        #endregion

        #region CreateXmlWriter()
        /// <summary>
        /// Creates a new XML writer.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>
        /// The new <see cref="XmlWriter"/>.
        /// </returns>
        public XmlWriter CreateXmlWriter(string filename)
        {
            var settings = new XmlWriterSettings
                               {
                                   Encoding = Encoding.UTF8,
                                   CloseOutput = true,
                                   Indent = true,
                                   NewLineOnAttributes = false,
                                   OmitXmlDeclaration = false
                               };
            return XmlWriter.Create(filename, settings);
        }
        #endregion

        #region EnsureFolderExists()
        /// <summary>
        /// Ensures that the folder exists.
        /// </summary>
        /// <param name="folder">The path to the folder.</param>
        public void EnsureFolderExists(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
        #endregion

        #region CopyFile()
        /// <summary>
        /// Copies a file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public void CopyFile(string source, string destination)
        {
            File.Copy(source, destination);
        }
        #endregion

        #region MoveFile()
        /// <summary>
        /// Moves a file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public void MoveFile(string source, string destination)
        {
            File.Move(source, destination);
        }
        #endregion

        #region DeleteFile()
        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void DeleteFile(string file)
        {
            File.Delete(file);
        }
        #endregion
        #endregion
    }
}
