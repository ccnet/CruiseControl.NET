namespace CruiseControl.Core.Tasks
{
    using System.ComponentModel;
    using System.Windows.Markup;

    /// <summary>
    /// Defines a file to merge.
    /// </summary>
    [ContentProperty("File")]
    public class MergeFile
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MergeFile"/> class.
        /// </summary>
        public MergeFile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeFile"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        public MergeFile(string file)
        {
            this.File = file;
        }
        #endregion

        #region Public properties
        #region File
        /// <summary>
        /// Gets or sets the file to merge.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        public string File { get; set; }
        #endregion

        #region Delete
        /// <summary>
        /// Gets or sets a value indicating whether the file should be deleted after the merge or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> to delete the file; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool Delete { get; set; }
        #endregion
        #endregion
    }
}
