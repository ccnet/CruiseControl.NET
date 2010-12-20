namespace CruiseControl.Core.Tasks
{
    public abstract class SourceControlTask
        : Task
    {
        #region Public properties
        #region Use
        /// <summary>
        /// Gets or sets the name of the source control block to use.
        /// </summary>
        /// <value>
        /// The name of the source control block.
        /// </value>
        public string Use { get; set; }
        #endregion
        #endregion
    }
}
