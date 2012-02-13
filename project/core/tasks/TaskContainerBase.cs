using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// A base class for tasks that contain other tasks.
    /// </summary>
    public abstract class TaskContainerBase
        : TaskBase, IConfigurationValidation
    {
        #region Private fields
        private Dictionary<string, string> parameters;
        private IEnumerable<ParameterBase> parameterDefinitions;
        private Dictionary<ITask, ItemStatus> taskStatuses = new Dictionary<ITask, ItemStatus>();
        #endregion

        #region Public properties
        #region Tasks
        /// <summary>
        /// The child tasks.
        /// </summary>
        public virtual ITask[] Tasks { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region ApplyParameters()
        /// <summary>
        /// Applies the input parameters to the task.
        /// </summary>
        /// <param name="parameters">The parameters to apply.</param>
        /// <param name="parameterDefinitions">The original parameter definitions.</param>
        public override void ApplyParameters(Dictionary<string, string> parameters, IEnumerable<ParameterBase> parameterDefinitions)
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
            // Validate all the child tasks
            if (Tasks != null)
            {
                foreach (var task in Tasks)
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

        #region InitialiseStatus()
        /// <summary>
        /// Initialise an <see cref="ItemStatus"/>.
        /// </summary>
        /// <param name="newStatus">The new status.</param>
        public override void InitialiseStatus(ItemBuildStatus newStatus)
        {
            // Do not re-initialise if the status is already set
            if ((this.CurrentStatus == null) || (this.CurrentStatus.Status != newStatus))
            {
                // This needs to be called first, otherwise the status is not set up
                taskStatuses.Clear();
                base.InitialiseStatus(newStatus);

                // Add each status
                if (Tasks != null)
                {
                    foreach (ITask task in Tasks)
                    {
                        ItemStatus taskItem = null;
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
                            taskItem = new ItemStatus(task.GetType().Name);
                            taskItem.Status = newStatus;
                        }

                        // Only add the item if it has been initialised
                        if (taskItem != null)
                        {
                            CurrentStatus.AddChild(taskItem);
                            taskStatuses.Add(task, taskItem);
                        }
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Protected methods
        #region RunTask()
        /// <summary>
        /// Runs a task.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="result"></param>
        /// <param name="taskDetails"></param>
        protected virtual void RunTask(ITask task, IIntegrationResult result, RunningSubTaskDetails taskDetails)
        {
            var tsk = task as IParamatisedItem;
            if (tsk != null)
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

        #region CancelTasks()
        /// <summary>
        /// Cancels any pending tasks.
        /// </summary>
        protected void CancelTasks()
        {
            foreach (var status in this.taskStatuses)
            {
                var task = status.Key as IStatusItem;
                if (task != null)
                {
                    task.CancelStatus();
                }
                else if (status.Value.Status == ItemBuildStatus.Pending)
                {
                    status.Value.Status = ItemBuildStatus.Cancelled;
                }
            }
        }
        #endregion

        #region SubTasks status management
        /// <summary>
        /// Use this task as a container for subtask details while running them
        /// </summary>
        protected class RunningSubTaskDetails
        {
            private int index;
            private IIntegrationResult parentResult;

            public RunningSubTaskDetails(int Index, IIntegrationResult ParentResult)
            {
                this.index = Index;
                this.parentResult = ParentResult;
                this.Finished = false;
            }
            /// <summary>
            /// Index of the subtask in the parent's list
            /// </summary>
            public int Index { get { return index; } }
            /// <summary>
            /// The current information for the subtask, as a string
            /// This one is updated by the delegate below, if you use it
            /// </summary>
            public string Information { get; set; }
            /// <summary>
            /// true if the task is finished.            
            /// This one has to be updated by you, should you need it
            /// </summary>
            public bool Finished { get; set; }
            /// <summary>
            /// The parent "result", used by the delegate to update the status while running
            /// </summary>
            public IIntegrationResult ParentResult { get { return parentResult; } }
        }

        /// <summary>
        /// This method builds the current status for the parent task, using the running subtask details
        /// given as a parameter
        /// </summary>
        /// <param name="Details">The currently running subtask details</param>
        /// <returns>The status of the parent task, with the running subtask details if need be</returns>
        protected abstract string GetStatusInformation(RunningSubTaskDetails Details);

        /// <summary>
        /// Use this as a delegate for IIntegrationResult.BuildProgressInformation.OnStartupInformationUpdated 
        /// RunTask above uses it
        /// </summary>
        /// <param name="information">The current information for the task that has changed status</param>
        /// <param name="UserObject">Some user object passed through. In this context, always a RunningSubTaskDetails instance</param>
        protected void SubTaskStartupInformationUpdated(string information, object UserObject)
        {
            var Details = ((RunningSubTaskDetails)UserObject);
            Details.Information = information;
            Details.ParentResult.BuildProgressInformation.UpdateStartupInformation(GetStatusInformation(Details));
        }

        #endregion
        #endregion
    }
}
