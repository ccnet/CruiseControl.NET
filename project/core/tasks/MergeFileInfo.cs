namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using Exortech.NetReflector;

    /// <summary>
    /// Details on a file to merge.
    /// </summary>
    /// <title>Merge File</title>
    /// <version>1.0</version>
    /// <example>
    /// <code title="Minimalist Example">
    /// &lt;file&gt;&lt;!-- path to file --&gt;&lt;/file&gt;
    /// </code>
    /// <code title="Full Example">
    /// &lt;file action="Copy" deleteAfterMerge="false"&gt;
    /// &lt;path&gt;
    /// &lt;!-- path to file --&gt;
    /// &lt;/path&gt;
    /// &lt;/file&gt;
    /// </code>
    /// </example>
    [ReflectorType("fileToMerge")]
    public class MergeFileInfo
    {
        #region Public properties
        #region FileName
        /// <summary>
        /// The name of the file to merge.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("path")]
        public string FileName { get; set; }
        #endregion

        #region MergeAction
        /// <summary>
        /// The type of the file to merge.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Merge</default>
        [ReflectorProperty("action", Required = false)]
        public MergeActionType MergeAction { get; set; }
        #endregion

        /// <summary>
        /// Delete the file after merging if set to true.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        [ReflectorProperty("deleteAfterMerge", Required = false)]
        public bool DeleteAfterMerge { get; set; }
        #endregion

        #region Public enumerations
        /// <summary>
        /// The type of merge to perform.
        /// </summary>
        public enum MergeActionType
        {
            /// <summary>
            /// Merge the files into the report file.
            /// </summary>
            Merge,
            /// <summary>
            /// Copy the files into the target folder.
            /// </summary>
            Copy,
            /// <summary>
            /// Merge the data in a CData section.
            /// </summary>
            CData,
            /// <summary>
            /// The file is an index file for selecting which files to copy.
            /// </summary>
            IndexCopy,
        }
        #endregion
    }
}
