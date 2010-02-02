namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.IO;
    using System.Runtime;

    /// <summary>
    /// A <see cref="ITaskResult"/> that reads the data directly from a file.
    /// </summary>
    public class FileTaskResult
        : ITaskResult
    {
        #region Private fields
        /// <summary>
        /// The file containing the data.
        /// </summary>
        private readonly FileInfo dataSource;
        private bool deleteAfterMerge = true;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FileTaskResult"/> class from a file name.
        /// </summary>
        /// <param name="filename">The name of the file.</param>
        public FileTaskResult(string filename) :
            this(filename, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTaskResult"/> class from a file name.
        /// </summary>
        /// <param name="filename">The name of the file.</param>
        /// <param name="deleteAfterMerge">Delete file after merging.</param>
        public FileTaskResult(string filename, bool deleteAfterMerge) :
            this(new FileInfo(filename), deleteAfterMerge)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTaskResult"/> class from a <see cref="FileInfo"/>.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        public FileTaskResult(FileInfo file) :
            this(file, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTaskResult"/> class from a <see cref="FileInfo"/>.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="deleteAfterMerge">Delete file after merging.</param>
        public FileTaskResult(FileInfo file, bool deleteAfterMerge)
        {
            if (!file.Exists)
            {
                throw new CruiseControlException("File not found: " + file.FullName);
            }

            this.deleteAfterMerge = deleteAfterMerge;
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

        /// <summary>
        /// Gets a value indicating if the file should be deleted after merge.
        /// </summary>
        public bool DeleteAfterMerge
        {
            get { return deleteAfterMerge; }
        }
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
                    return "<![CDATA[" + data + "]]>";
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
                if (this.dataSource.Length > 1048576)
                {
                    // Since the file is over one Mb, check if there is enough free memory to load the data
                    // Note: We are actually checking to see if there is twice the amount of memory required, this is because often the 
                    // data will need to be copied somewhere else, which means the string will exist in memory at least twice (hopefully
                    // GC will clean up if it is needed more than twice)
                    var fileSizeInMB = Convert.ToInt32(this.dataSource.Length / 524288);
                    try
                    {
                        using (new MemoryFailPoint(fileSizeInMB))
                        {
                        }
                    }
                    catch (InsufficientMemoryException error)
                    {
                        // Much nicer to handle an InsufficientMemoryException exception than an OutOfMemoryException - OOM tends to kill
                        // things!
                        throw new CruiseControlException("Insufficient memory to import file results: " + error.Message, error);
                    }
                }

                // Load the data from the file
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
