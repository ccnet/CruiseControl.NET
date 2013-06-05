namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Tasks.Conditions;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Parameters;

    /// <title>Conditional Task</title>
    /// <version>1.6</version>
    /// <summary>
    /// Checks to see if a condition is true before the contained tasks run.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// <conditional>
    /// <conditions>
    /// <!-- Conditions -->
    /// </conditions>
    /// <tasks>
    /// <!-- Tasks to run if conditions pass -->
    /// </tasks>
    /// <elseTasks>
    /// <!-- Tasks to run if conditions fail -->
    /// </elseTasks>
    /// </conditional>
    /// ]]>
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// This task has been kindly supplied by Lasse Sjørup. The original project is available from
    /// <link>http://ccnetconditional.codeplex.com/</link>.
    /// </para>
    /// </remarks>
    [ReflectorType("conditional")]
    public class ConditionalTask
        : TaskBase, IConfigurationValidation
    {
        #region Private fields
        private Dictionary<string, string> parameters;
        private IEnumerable<ParameterBase> parameterDefinitions;
        private ItemStatus mainStatus;
        private ItemStatus elseStatus;
        private readonly Dictionary<ITask, ItemStatus> taskStatuses = new Dictionary<ITask, ItemStatus>();
        private readonly Dictionary<ITask, ItemStatus> elseTaskStatuses = new Dictionary<ITask, ItemStatus>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalTask"/> class.
        /// </summary>
        public ConditionalTask()
        {
            this.Tasks = new ITask[0];
            this.ElseTasks = new ITask[0];
            this.ContinueOnFailure = true;
        }
        #endregion

        #region Public properties
        #region TaskConditions
        /// <summary>
        /// The conditions to check.
        /// </summary>
        /// <default>n/a</default>
        /// <version>1.6</version>
        /// <remarks>
        /// These conditions must all pass in order for the main tasks to run. Use an 
        /// <link>Or Condition</link> when only one condition is required.
        /// </remarks>
        [ReflectorProperty("conditions", Required = true)]
        public ITaskCondition[] TaskConditions { get; set; }
        #endregion

        #region Tasks
        /// <summary>
        /// The tasks to run if conditions evaluates to true.
        /// </summary>
        /// <default>None</default>
        /// <version>1.6</version>
        [ReflectorProperty("tasks", Required = false)]
        public ITask[] Tasks { get; set; }
        #endregion

        #region ElseTasks
        /// <summary>
        /// The tasks to run if conditions evaluates to false.
        /// </summary>
        /// <default>None</default>
        /// <version>1.6</version>
        [ReflectorProperty("elseTasks", Required = false)]
        public ITask[] ElseTasks { get; set; }
        #endregion

        #region Logger
        /// <summary>
        /// Gets or sets the logger to use.
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; set; }
        #endregion

        #region ContinueOnFailure
        /// <summary>
        /// Should the tasks continue to run, even if there is a failure?
        /// </summary>
        /// <version>1.9</version>
        /// <default>true</default>
        [ReflectorProperty("continueOnFailure", Required = false)]
        public bool ContinueOnFailure { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region ApplyParameters()
        /// <summary>
        /// Applies the input parameters to the task.
        /// </summary>
        /// <param name="parametersToApply">The parameters to apply.</param>
        /// <param name="parameterDefinitionsToUse">The parameter definitions to use.</param>
        public override void ApplyParameters(Dictionary<string, string> parametersToApply, 
            IEnumerable<ParameterBase> parameterDefinitionsToUse)
        {
            this.parameters = parametersToApply;
            this.parameterDefinitions = parameterDefinitionsToUse;
            base.ApplyParameters(parametersToApply, parameterDefinitionsToUse);
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
            // Validate the conditions
            var trace = parent.Wrap(this);
            foreach (var condition in this.TaskConditions ?? new ITaskCondition[0])
            {
                var validation = condition as IConfigurationValidation;
                if (validation != null)
                {
                    validation.Validate(
                        configuration,
                        trace,
                        errorProcesser);
                }
            }

            // Validate the tasks
            this.ValidateTasks(this.Tasks, configuration, trace, errorProcesser);
            this.ValidateTasks(this.ElseTasks, configuration, trace, errorProcesser);
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
            var conditionsPassed = this.EvaluateConditions(logger, result);
            bool successful;

            // Run the required tasks
            if (conditionsPassed)
            {
                logger.Info("Conditions passed - running tasks");
                this.elseStatus.Status = ItemBuildStatus.Cancelled;
                CancelTasks(this.elseTaskStatuses);
                successful = this.RunTasks(this.Tasks, logger, true, result);
                CancelTasks(this.taskStatuses);
                this.mainStatus.Status = successful ? ItemBuildStatus.CompletedSuccess : ItemBuildStatus.CompletedFailed;
            }
            else
            {
                logger.Info("Conditions did not pass - running else tasks");
                this.mainStatus.Status = ItemBuildStatus.Cancelled;
                CancelTasks(this.taskStatuses);
                successful = this.RunTasks(this.ElseTasks, logger, false, result);
                CancelTasks(this.elseTaskStatuses);
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
                        validatorTask.Validate(configuration, parent, errorProcesser);
                    }
                }
            }
        }
        #endregion

        #region EvaluateConditions()
        /// <summary>
        /// Evaluates the conditions.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="result">The result.</param>
        /// <returns>
        /// <c>true</c> if the conditions are met; <c>false</c> otherwise.
        /// </returns>
        private bool EvaluateConditions(ILogger logger,  IIntegrationResult result)
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
                var commonCondition = condition as ConditionBase;
                if (commonCondition != null)
                {
                    commonCondition.Logger = logger;
                }

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
                foreach (var task in tasks)
                {
                    ItemStatus taskItem;
                    var tbase = task as TaskBase;
                    if (tbase != null)
                    {
                        // Reset the status for the task
                        tbase.InitialiseStatus(newStatus);
                    }

                    var dummy = task as IStatusSnapshotGenerator;
                    if (dummy != null)
                    {
                        taskItem = dummy.GenerateSnapshot();
                    }
                    else
                    {
                        taskItem = new ItemStatus(task.GetType().Name)
                                       {
                                           Status = newStatus
                                       };
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
        private static void CancelTasks(Dictionary<ITask, ItemStatus> taskStatuses)
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
        /// <param name="taskDetails"></param>
        private void RunTask(ITask task, IIntegrationResult result, RunningSubTaskDetails taskDetails)
        {
            var tsk = task as IParamatisedItem;
            if (tsk!= null)
            {
                tsk.ApplyParameters(parameters, parameterDefinitions);
            }

            if (result != null)
            {
                result.BuildProgressInformation.OnStartupInformationUpdatedUserObject = taskDetails;
                result.BuildProgressInformation.OnStartupInformationUpdated = SubTaskStartupInformationUpdated;
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
        /// <param name="runningIfTasks">true if running the "if" tasks.</param>
        /// <param name="result">The result.</param>
        /// <returns><c>true</c> if all the tasks are successul; <c>false</c> otherwise.</returns>
        private bool RunTasks(ITask[] tasks, ILogger logger, bool runningIfTasks, IIntegrationResult result)
        {
            // Launch each task
            var successCount = 0;
            var failureCount = 0;
            for (var loop = 0; loop < tasks.Length; loop++)
            {
                var taskName = string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0} [{1}]", tasks[loop].GetType().Name, loop);
                logger.Debug("Starting task '{0}'", taskName);
                try
                {
                    var taskResult = result.Clone();

                    // must reset the status so that we check for the current task failure and not a previous one
                    taskResult.Status = IntegrationStatus.Unknown;

                    // Start the actual task
                    var task = tasks[loop];
                    this.RunTask(task, taskResult, new RunningSubTaskDetails(loop, runningIfTasks, result));
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
                    if (!ContinueOnFailure)
                        break;
                }
            }

            logger.Info("Tasks completed: {0} successful, {1} failed", successCount, failureCount);
            return failureCount == 0;
        }
        #endregion

        private class RunningSubTaskDetails
        {
            private int index;
            private IIntegrationResult parentResult;
            private bool runningIfTasks;

            public RunningSubTaskDetails(int Index, bool RunningIfTasks, IIntegrationResult ParentResult)
            {
                this.index = Index;
                this.parentResult = ParentResult;
                this.runningIfTasks = RunningIfTasks;
            }
            /// <summary>
            /// Index of the subtask in the parent's list
            /// </summary>
            public int Index { get { return index; } }
            /// <summary>
            /// The current information for the subtask, as a string
            /// </summary>
            public string Information { get; set; }
            /// <summary>
            /// true if the task is running for the "if" part.            
            /// </summary>
            public bool RunningIfTasks { get { return runningIfTasks; } }
            /// <summary>
            /// The parent "result", used by the delegate to update the status while running
            /// </summary>
            public IIntegrationResult ParentResult { get { return parentResult; } }
        }

        private void SubTaskStartupInformationUpdated(string information, object UserObject)
        {
            var Details = ((RunningSubTaskDetails)UserObject);
            Details.Information = information;
            Details.ParentResult.BuildProgressInformation.UpdateStartupInformation(GetStatusInformation(Details));
        }

        private string GetStatusInformation(RunningSubTaskDetails Details)
        {
            string Value = !string.IsNullOrEmpty(Description)
                            ? Description
                            : string.Format("Running {1} tasks ({0} task(s))", Tasks.Length, Details.RunningIfTasks ? "\"if\"" : "\"else\"");

            if (Details != null)
                Value += string.Format(": [{0}] {1}",
                                        Details.Index,
                                        !string.IsNullOrEmpty(Details.Information)
                                         ? Details.Information
                                         : "No information");

            return Value;
        }

        #endregion
    }
}
