
namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// Details on a file to merge.
    /// </summary>
    public class MergeFileInfo
    {
        #region Public properties
        #region FileName
        /// <summary>
        /// The name of the file to merge.
        /// </summary>
        public string FileName { get; set; }
        #endregion

        #region MergeAction
        /// <summary>
        /// The type of the file to merge.
        /// </summary>
        public MergeActionType MergeAction { get; set; }
        #endregion
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
        }
        #endregion
    }
}
