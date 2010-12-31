namespace CruiseControl.Core
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using NLog;

    /// <summary>
    /// The base task implementation - provides common functionality for tasks.
    /// </summary>
    public abstract class Task
        : ProjectItem
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Task"/> class.
        /// </summary>
        protected Task()
        {
            this.State = TaskState.Loaded;
            this.Conditions = new List<TaskCondition>();
            this.FailureActions = new List<TaskFailureAction>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Task"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected Task(string name)
            : this()
        {
            this.Name = name;
        }
        #endregion

        #region Public properties
        #region Description
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DefaultValue(null)]
        public string Description { get; set; }
        #endregion

        #region FailureActions
        /// <summary>
        /// Gets the failure actions.
        /// </summary>
        public IList<TaskFailureAction> FailureActions { get; private set; }
        #endregion

        #region Conditions
        /// <summary>
        /// Gets the conditions.
        /// </summary>
        public IList<TaskCondition> Conditions { get; private set; }
        #endregion

        #region Parent
        /// <summary>
        /// Gets or sets the parent task.
        /// </summary>
        /// <value>The parent.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Task Parent { get; set; }
        #endregion

        #region State
        /// <summary>
        /// Gets the current state of the task.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TaskState State { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Validates this task after it has been loaded.
        /// </summary>
        public void Validate()
        {
            logger.Debug("Validating task '{0}'", this.NameOrType);
            this.OnValidate();
            this.State = TaskState.Validated;
        }
        #endregion
        #region Initialise()
        /// <summary>
        /// Initialises this instance.
        /// </summary>
        public void Initialise()
        {
            logger.Debug("Initialising task '{0}'", this.NameOrType);
            this.State = TaskState.Pending;
            this.OnInitialise();
        }
        #endregion

        #region CanRun()
        /// <summary>
        /// Determines whether this instance can run.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance can run; otherwise, <c>false</c>.
        /// </returns>
        public bool CanRun(TaskExecutionContext context)
        {
            logger.Debug("Checking conditions for task '{0}'", this.NameOrType);
            this.State = TaskState.CheckingConditions;
            var canExecute = this.OnCanRun(context);

            return canExecute;
        }
        #endregion

        #region Run()
        /// <summary>
        /// Runs this task.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The child tasks to run.</returns>
        public virtual IEnumerable<Task> Run(TaskExecutionContext context)
        {
            logger.Debug("Running task '{0}'", this.NameOrType);
            this.State = TaskState.Executing;

            foreach (var task in this.OnRun(context) ?? new Task[0])
            {
                yield return task;
            }

            logger.Debug("Task '{0}' has completed", this.NameOrType);
            this.State = TaskState.Completed;
        }
        #endregion

        #region Skip()
        /// <summary>
        /// Skips this instance.
        /// </summary>
        /// <param name="context">The context.</param>
        public virtual void Skip(TaskExecutionContext context)
        {
            logger.Debug("Task '{0}' has been skipped", this.NameOrType);
            this.State = TaskState.Skipped;
            this.OnSkip(context);
        }
        #endregion

        #region CleanUp()
        /// <summary>
        /// Cleans up anything from this task.
        /// </summary>
        public void CleanUp()
        {
            logger.Debug("Cleaning up task '{0}'", this.NameOrType);
            switch (this.State)
            {
                case TaskState.Pending:
                    this.State = TaskState.Skipped;
                    break;

                case TaskState.CheckingConditions:
                case TaskState.Executing:
                    this.State = TaskState.Terminated;
                    break;
            }

            this.OnCleanUp();
        }
        #endregion
        #endregion

        #region Protected methods
        #region OnValidate()
        /// <summary>
        /// Called when this task is validated.
        /// </summary>
        protected virtual void OnValidate()
        {
        }
        #endregion

        #region OnInitialise()
        /// <summary>
        /// Called when this task is being initialised.
        /// </summary>
        protected virtual void OnInitialise()
        {
        }
        #endregion

        #region OnCanRun()
        /// <summary>
        /// Called when the task is being checked whether it can run.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// <c>true</c> if the task can run; <c>false</c> otherwise.
        /// </returns>
        protected virtual bool OnCanRun(TaskExecutionContext context)
        {
            var canExecute = true;
            foreach (var condition in this.Conditions ?? new TaskCondition[0])
            {
                canExecute = condition.Evaluate(context);
                if (!canExecute)
                {
                    break;
                }
            }

            return canExecute;
        }
        #endregion

        #region OnRun()
        /// <summary>
        /// Executes this task.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The child tasks to execute.
        /// </returns>
        protected abstract IEnumerable<Task> OnRun(TaskExecutionContext context);
        #endregion

        #region OnSkip()
        /// <summary>
        /// Called when this task has been skipped.
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void OnSkip(TaskExecutionContext context)
        {
        }
        #endregion

        #region OnCleanUp()
        /// <summary>
        /// Called when this task has been cleaned up.
        /// </summary>
        protected virtual void OnCleanUp()
        {
        }
        #endregion
        #endregion
    }
}
