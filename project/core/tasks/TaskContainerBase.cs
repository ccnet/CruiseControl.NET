using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using ThoughtWorks.CruiseControl.Remote;
using System;
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
            // This needs to be called first, otherwise the status is not set up
            taskStatuses.Clear();
            base.InitialiseStatus(newStatus);

            // Add each status
            if (Tasks != null)
            {
                foreach (ITask task in Tasks)
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
                        CurrentStatus.AddChild(taskItem);
                        taskStatuses.Add(task, taskItem);
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
        protected virtual void RunTask(ITask task, IIntegrationResult result)
        {
            if (task is IParamatisedItem)
            {
                (task as IParamatisedItem).ApplyParameters(parameters, parameterDefinitions);
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
        #endregion
    }
}
