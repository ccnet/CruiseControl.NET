namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.IO;

    /// <summary>
    /// A <see cref="ITaskResult"/> that reads the data directly from a file.
    /// </summary>
    public class FileTaskResult
        : ITaskResult
    {
        #region Privte fields
        /// <summary>
        /// The file containing the data.
        /// </summary>
        private readonly FileInfo dataSource;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FileTaskResult"/> class from a file name.
        /// </summary>
        /// <param name="filename">The name of the file.</param>
        public FileTaskResult(string filename) :
            this(new FileInfo(filename))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTaskResult"/> class from a <see cref="FileInfo"/>.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        public FileTaskResult(FileInfo file)
        {
            if (!file.Exists)
            {
                throw new CruiseControlException("File not found: " + file.FullName);
            }

            this.dataSource = file;
        }
        #endregion

        #region Public properties
        #region WrapInCData
        /// <summary>
        /// Gets or sets a value indicating whether the data should be wrapped in a CData section.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the data should be wrapped in a CData section; otherwise, <c>false</c>.
        /// </value>
        public bool WrapInCData { get; set; }
        #endregion

        #region Data
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data from the result.</value>
        public string Data
        {
            get
            {
                var data = this.ReadFileContents();
                if (WrapInCData)
                {
                    return string.Format("<![CDATA[{0}]]>", data);
                }
                else
                {
                    return data;
                }
            }
        }
        #endregion
        #endregion

        #region Public methods
        #region CheckIfSuccess()
        /// <summary>
        /// Checks whether the result was successful.
        /// </summary>
        /// <returns><c>true</c> if the result was successful, <c>false</c> otherwise.</returns>
        public bool CheckIfSuccess()
        {
            return true;
        }
        #endregion
        #endregion

        #region Private methods
        #region ReadFileContents()
        /// <summary>
        /// Reads the contents of the file.
        /// </summary>
        /// <returns>The contents of the file as a <c>string</c>.</returns>
        private string ReadFileContents()
        {
            try
            {
                using (StreamReader reader = this.dataSource.OpenText())
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw new CruiseControlException("Unable to read the contents of the file: " + this.dataSource.FullName, ex);
            }
        }
        #endregion
        #endregion
    }
}
