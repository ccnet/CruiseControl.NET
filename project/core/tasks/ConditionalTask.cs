namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Util;
    using System.Collections.Generic;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Remote.Parameters;

    /// <title>Conditional Task</title>
    /// <version>1.6</version>
    /// <summary>
    /// Checks to see if a condition is true before the contained tasks run.
    /// </summary>
    [ReflectorType("conditional")]
    public class ConditionalTask
        : TaskBase, IConfigurationValidation
    {
        #region Private fields
        private Dictionary<string, string> parameters;
        private IEnumerable<ParameterBase> parameterDefinitions;
        private ItemStatus mainStatus;
        private ItemStatus elseStatus;
        private Dictionary<ITask, ItemStatus> taskStatuses = new Dictionary<ITask, ItemStatus>();
        private Dictionary<ITask, ItemStatus> elseTaskStatuses = new Dictionary<ITask, ItemStatus>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalTask"/> class.
        /// </summary>
        public ConditionalTask()
        {
            this.Tasks = new ITask[0];
            this.ElseTasks = new ITask[0];
        }
        #endregion

        #region Public properties
        #region TaskConditions
        /// <summary>
        /// Gets or sets the task conditions.
        /// </summary>
        /// <value>The task conditions.</value>
        [ReflectorArray("conditions", Required = true)]
        public ITaskCondition[] TaskConditions { get; set; }
        #endregion

        #region Tasks
        /// <summary>
        /// Gets or sets the tasks to run if conditions evaluates to true.
        /// </summary>
        /// <value>The tasks.</value>
        [ReflectorArray("tasks", Required = false)]
        public ITask[] Tasks { get; set; }
        #endregion

        #region ElseTasks
        /// <summary>
        /// Gets or sets the tasks to run if conditions evaluates to false.
        /// </summary>
        /// <value>The tasks.</value>
        [ReflectorArray("elseTasks", Required = false)]
        public ITask[] ElseTasks { get; set; }
        #endregion

        #region Logger
        /// <summary>
        /// Gets or sets the logger to use.
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region ApplyParameters()
        /// <summary>
        /// Applies the input parameters to the task.
        /// </summary>
        /// <param name="parameters">The parameters to apply.</param>
        /// <param name="parameterDefinitions">The original parameter definitions.</param>
        public override void ApplyParameters(Dictionary<string, string> parameters, 
            IEnumerable<ParameterBase> parameterDefinitions)
        {
            this.parameters = parameters;
            this.parameterDefinitions = parameterDefinitions;
            base.ApplyParameters(parameters, parameterDefinitions);
        }
        #endregion

        #region Validate()
        /// <summary>
        /// Validates this task.
        /// </summary>
        /// <param name="configuration">The entire configuration.</param>
        /// <param name="parent">The parent item for the item being validated.</param>
        /// <param name="errorProcesser">The error processer to use.</param>
        public virtual void Validate(IConfiguration configuration, ConfigurationTrace parent, IConfigurationErrorProcesser errorProcesser)
        {
            this.ValidateTasks(this.Tasks, configuration, parent, errorProcesser);
            this.ValidateTasks(this.ElseTasks, configuration, parent, errorProcesser);
        }
        #endregion

        #region InitialiseStatus()
        /// <summary>
        /// Initialise an <see cref="ItemStatus"/>.
        /// </summary>
        /// <param name="newStatus">The new status.</param>
        public override void InitialiseStatus(ItemBuildStatus newStatus)
        {
            // If the status is already set, do not change it
            if ((this.CurrentStatus == null) || (this.CurrentStatus.Status != newStatus))
            {
                // This needs to be called first, otherwise the status is not set up
                this.taskStatuses.Clear();
                this.elseTaskStatuses.Clear();
                base.InitialiseStatus(newStatus);
                this.mainStatus = this.InitialiseTaskStatuses(newStatus, this.Tasks, this.taskStatuses, "Tasks (Main)");
                this.elseStatus = this.InitialiseTaskStatuses(newStatus, this.ElseTasks, this.elseTaskStatuses, "Tasks (Else)");
            }
        }
        #endregion
        #endregion

        #region Protected methods
        #region Execute()
        /// <summary>
        /// Execute the actual task functionality.
        /// </summary>
        /// <param name="result">The result to use.</param>
        /// <returns>
        /// <c>true</c> if the task was successful; <c>false</c> otherwise.
        /// </returns>
        protected override bool Execute(IIntegrationResult result)
        {
            result.BuildProgressInformation
                .SignalStartRunTask(string.IsNullOrEmpty(this.Description) ? "Running conditional task" : this.Description);

            // Check the conditions
            var logger = this.Logger ?? new DefaultLogger();
            logger.Debug("Checking conditions");
            var conditionsPassed = this.EvaluateConditions(result);
            var successful = true;

            // Run the required tasks
            if (conditionsPassed)
            {
                logger.Info("Conditions passed - running tasks");
                this.elseStatus.Status = ItemBuildStatus.Cancelled;
                this.CancelTasks(this.elseTaskStatuses);
                successful = this.RunTasks(this.Tasks, logger, result);
                this.CancelTasks(this.taskStatuses);
                this.mainStatus.Status = successful ? ItemBuildStatus.CompletedSuccess : ItemBuildStatus.CompletedFailed;
            }
            else
            {
                logger.Info("Conditions did not pass - running else tasks");
                this.mainStatus.Status = ItemBuildStatus.Cancelled;
                this.CancelTasks(this.taskStatuses);
                successful = this.RunTasks(this.ElseTasks, logger, result);
                this.CancelTasks(this.elseTaskStatuses);
                this.elseStatus.Status = successful ? ItemBuildStatus.CompletedSuccess : ItemBuildStatus.CompletedFailed;
            }

            return successful;
        }
        #endregion
        #endregion

        #region Private methods
        #region ValidateTasks()
        /// <summary>
        /// Validates some tasks.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="errorProcesser">The error processer.</param>
        private void ValidateTasks(ITask[] tasks,
            IConfiguration configuration, 
            ConfigurationTrace parent, 
            IConfigurationErrorProcesser errorProcesser)
        {
            if (tasks != null)
            {
                foreach (var task in tasks)
                {
                    var validatorTask = task as IConfigurationValidation;
                    if (validatorTask != null)
                    {
                        validatorTask.Validate(configuration, parent.Wrap(this), errorProcesser);
                    }
                }
            }
        }
        #endregion

        #region EvaluateConditions()
        /// <summary>
        /// Evaluates the conditions.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>
        /// <c>true</c> if the conditions are met; <c>false</c> otherwise.
        /// </returns>
        private bool EvaluateConditions(IIntegrationResult result)
        {
            // Sanity check - this should not be possible
            if (this.TaskConditions == null)
            {
                throw new ArgumentNullException();
            }

            // Check each of the conditions - if any fail then fail the entire check
            var passed = true;
            foreach (ITaskCondition condition in TaskConditions)
            {
                passed = condition.Eval(result);
                if (!passed)
                {
                    break;
                }
            }

            return passed;
        }
        #endregion

        #region InitialiseTaskStatuses()
        /// <summary>
        /// Initialises the task statuses.
        /// </summary>
        /// <param name="newStatus">The new status.</param>
        /// <param name="tasks">The tasks.</param>
        /// <param name="taskStatuses">The task statuses.</param>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        private ItemStatus InitialiseTaskStatuses(
            ItemBuildStatus newStatus,
            ITask[] tasks, 
            Dictionary<ITask, ItemStatus> taskStatuses,
            string title)
        {
            var groupStatus = new ItemStatus
            {
                Name = title,
                Status = newStatus,
                TimeCompleted = null,
                TimeOfEstimatedCompletion = null,
                TimeStarted = null
            };
            this.CurrentStatus.AddChild(groupStatus);
            if (tasks != null)
            {
                foreach (ITask task in tasks)
                {
                    ItemStatus taskItem = null;
                    if (task is TaskBase)
                    {
                        // Reset the status for the task
                        (task as TaskBase).InitialiseStatus(newStatus);
                    }

                    if (task is IStatusSnapshotGenerator)
                    {
                        taskItem = (task as IStatusSnapshotGenerator).GenerateSnapshot();
                    }
                    else
                    {
                        taskItem = new ItemStatus(task.GetType().Name);
                        taskItem.Status = newStatus;
                    }

                    // Only add the item if it has been initialised
                    if (taskItem != null)
                    {
                        groupStatus.AddChild(taskItem);
                        taskStatuses.Add(task, taskItem);
                    }
                }
            }

            return groupStatus;
        }
        #endregion

        #region CancelTasks()
        /// <summary>
        /// Cancels any pending tasks.
        /// </summary>
        /// <param name="taskStatuses">The task statuses.</param>
        private void CancelTasks(Dictionary<ITask, ItemStatus> taskStatuses)
        {
            foreach (var status in taskStatuses)
            {
                if (status.Key is IStatusItem)
                {
                    var item = status.Key as IStatusItem;
                    item.CancelStatus();
                }
                else if (status.Value.Status == ItemBuildStatus.Pending)
                {
                    status.Value.Status = ItemBuildStatus.Cancelled;
                }
            }
        }
        #endregion

        #region RunTask()
        /// <summary>
        /// Runs a task.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="result"></param>
        private void RunTask(ITask task, IIntegrationResult result)
        {
            if (task is IParamatisedItem)
            {
                (task as IParamatisedItem).ApplyParameters(parameters, parameterDefinitions);
            }

            task.Run(result);
        }
        #endregion

        #region RunTasks()
        /// <summary>
        /// Runs some tasks.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="result">The result.</param>
        /// <returns><c>true</c> if all the tasks are successul; <c>false</c> otherwise.</returns>
        private bool RunTasks(ITask[] tasks, ILogger logger, IIntegrationResult result)
        {
            // Launch each task
            var successCount = 0;
            var failureCount = 0;
            for (var loop = 0; loop < tasks.Length; loop++)
            {
                var taskName = string.Format("{0} [{1}]", tasks[loop].GetType().Name, loop);
                logger.Debug("Starting task '{0}'", taskName);
                try
                {
                    // Start the actual task
                    var taskResult = result.Clone();
                    var task = tasks[loop];
                    this.RunTask(task, taskResult);
                    result.Merge(taskResult);
                }
                catch (Exception error)
                {
                    // Handle any error details
                    result.ExceptionResult = error;
                    result.Status = IntegrationStatus.Failure;
                    logger.Warning("Task '{0}' failed!", taskName);
                }

                // Record the results
                if (result.Status == IntegrationStatus.Success)
                {
                    successCount++;
                }
                else
                {
                    failureCount++;
                }
            }

            logger.Info("Tasks completed: {0} successful, {1} failed", successCount, failureCount);
            return failureCount == 0;
        }
        #endregion
        #endregion
    }
}
