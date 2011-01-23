namespace CruiseControl.Web.Utilities
{
    using System.IO;

    /// <summary>
    /// Abstracts the interface to the file system to make unit testing easier.
    /// </summary>
    public class FileSystem
    {
        #region Public methods
        #region CheckFileExists()
        /// <summary>
        /// Checks that a file exists.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// <c>true</c> if the file exists; <c>false</c> otherwise.
        /// </returns>
        public virtual bool CheckFileExists(string path)
        {
            return File.Exists(path);
        }
        #endregion

        #region ReadFromFile()
        /// <summary>
        /// Generates a reader for reading from a file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// The <see cref="TextReader"/> to use for reading the file.
        /// </returns>
        public virtual TextReader ReadFromFile(string path)
        {
            var reader = new StreamReader(path);
            return reader;
        }
        #endregion

        #region WriteToFile()
        /// <summary>
        /// Generates a writer for writing to a file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// The <see cref="TextWriter"/> to use for writing the file.
        /// </returns>
        public virtual TextWriter WriteToFile(string path)
        {
            var writer = new StreamWriter(path, false);
            return writer;
        }
        #endregion

        #region DeleteFile()
        /// <summary>
        /// Deletes a file if it exists.
        /// </summary>
        /// <param name="path">The path.</param>
        public virtual void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        #endregion
        #endregion
    }
}