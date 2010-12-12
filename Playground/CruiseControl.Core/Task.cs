namespace CruiseControl.Core
{
    using System.Collections.Generic;

    public abstract class Task
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Task"/> class.
        /// </summary>
        public Task()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Task"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Task(string name)
        {
            this.Name = name;
        }
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        #endregion

        #region Parent
        /// <summary>
        /// Gets or sets the parent task.
        /// </summary>
        /// <value>The parent.</value>
        public Task Parent { get; set; }
        #endregion

        #region Project
        /// <summary>
        /// Gets or sets the owning project.
        /// </summary>
        /// <value>The project.</value>
        public Project Project { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Execute()
        /// <summary>
        /// Executes this task.
        /// </summary>
        /// <returns>The child tasks to execute.</returns>
        public abstract IEnumerable<Task> Execute();
        #endregion
        #endregion
    }
}
