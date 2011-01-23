namespace CruiseControl.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Markup;
    using NLog;

    /// <summary>
    /// Merges one or more files into the results.
    /// </summary>
    [ContentProperty("Files")]
    public class MergeFiles
        : Task
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MergeFiles"/> class.
        /// </summary>
        public MergeFiles()
        {
            this.Files = new List<MergeFile>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeFiles"/> class.
        /// </summary>
        /// <param name="files">The files.</param>
        public MergeFiles(params MergeFile[] files)
        {
            this.Files = new List<MergeFile>(files);
        }
        #endregion

        #region Public properties
        #region Files
        /// <summary>
        /// Gets the files.
        /// </summary>
        public IList<MergeFile> Files { get; private set; }
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
            foreach (var file in this.Files)
            {
                logger.Info("Merging file '{0}'", file.File);
                context.ImportFile(file.File, file.Delete);
            }

            return null;
        }
        #endregion
        #endregion
    }
}
