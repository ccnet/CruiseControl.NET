namespace CruiseControl.Core.Tasks
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using CruiseControl.Core.Interfaces;
    using Ninject;

    public class GetSource
        : SourceControlTask
    {
        #region Public properties
        #region Revert
        /// <summary>
        /// Gets or sets a value indicating whether the code should be reverted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if revert; otherwise, <c>false</c>.
        /// </value>
        public bool Revert { get; set; }
        #endregion

        #region FileSystem
        /// <summary>
        /// Gets or sets the file system.
        /// </summary>
        /// <value>
        /// The file system.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Inject]
        public IFileSystem FileSystem { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region OnRun()
        /// <summary>
        /// Executes this task.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The child tasks to execute.
        /// </returns>
        protected override IEnumerable<Task> OnRun(TaskExecutionContext context)
        {
            // TODO: Implement this task
            return null;
        }
        #endregion
        #endregion
    }
}
