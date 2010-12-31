namespace CruiseControl.Core.Tasks
{
    using System;
    using System.Linq;

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

        #region Protected methods
        #region GetSourceControlBlock()
        /// <summary>
        /// Attempts to get the source control block for this task.
        /// </summary>
        /// <returns>
        /// The <see cref="SourceControlBlock"/> for this task to use.
        /// </returns>
        protected SourceControlBlock GetSourceControlBlock()
        {
            if (this.Project == null)
            {
                // TODO: Replace with custom exception
                throw new NotSupportedException("This task does not belong to a project");
            }

            if (string.IsNullOrEmpty(this.Use))
            {
                if (this.Project.SourceControl.Count == 1)
                {
                    return this.Project.SourceControl[0];
                }

                // TODO: Replace with custom exception
                throw new NotSupportedException();
            }

            var block = this.Project.SourceControl
                .FirstOrDefault(sc => sc.Name == this.Use);
            if (block == null)
            {
                // TODO: Replace with custom exception
                throw new NotSupportedException();
            }

            return block;
        }
        #endregion
        #endregion
    }
}
