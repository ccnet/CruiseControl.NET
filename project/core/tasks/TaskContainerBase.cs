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
        : TaskBase, IStatusSnapshotGenerator, ITask, IConfigurationValidation
    {
        #region Private fields
        private Dictionary<string, string> parameters;
        private IEnumerable<ParameterBase> parameterDefinitions;
        private ItemStatus currentStatus;
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
        #region Run()
        /// <summary>
        /// Runs the task, given the specified <see cref="IIntegrationResult"/>, in the specified <see cref="IProject"/>.
        /// </summary>
        /// <param name="result"></param>
        public virtual void Run(IIntegrationResult result)
        {
            // Initialise the task
            if (currentStatus == null) InitialiseStatus(GetType().Name, Tasks);
            currentStatus.Status = ItemBuildStatus.Running;
            currentStatus.TimeStarted = DateTime.Now;

            // Perform the actual run
            try
            {
                RunTasks(result);
            }
            catch (Exception)
            {
                result.Status = IntegrationStatus.Exception;
                throw;
            }
            finally
            {
                // Clean up
                currentStatus.Status = (result.Status == IntegrationStatus.Success) ? ItemBuildStatus.CompletedSuccess : ItemBuildStatus.CompletedFailed;
                currentStatus.TimeCompleted = DateTime.Now;
            }
        }
        #endregion

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

        #region GenerateSnapshot()
        /// <summary>
        /// Generates a snapshot of the current status.
        /// </summary>
        /// <returns></returns>
        public virtual ItemStatus GenerateSnapshot()
        {
            if (currentStatus == null) InitialiseStatus(GetType().Name, Tasks);
            return currentStatus;
        }
        #endregion

        #region Validate()
        /// <summary>
        /// Validates this task.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="parent"></param>
        /// <param name="errorProcesser"></param>
        public virtual void Validate(IConfiguration configuration, object parent, IConfigurationErrorProcesser errorProcesser)
        {
            // Validate all the child tasks
            foreach (var task in Tasks)
            {
                var validatorTask = task as IConfigurationValidation;
                if (validatorTask != null)
                {
                    validatorTask.Validate(configuration, parent, errorProcesser);
                }
            }
        }
        #endregion
        #endregion

        #region Protected methods
        #region RunTasks()
        /// <summary>
        /// Run all the child tasks.
        /// </summary>
        /// <param name="result"></param>
        protected abstract void RunTasks(IIntegrationResult result);
        #endregion

        #region RunTask()
        /// <summary>
        /// Runs a task.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="result"></param>
        protected virtual void RunTask(ITask task, IIntegrationResult result)
        {
            if (task is IParamatisedTask)
            {
                (task as IParamatisedTask).ApplyParameters(parameters, parameterDefinitions);
            }

            task.Run(result);
        }
        #endregion

        #region InitialiseStatus()
        /// <summary>
        /// Initialise an <see cref="ItemStatus"/>.
        /// </summary>
        /// <param name="tasks"></param>
        protected virtual void InitialiseStatus(string name, ITask[] tasks)
        {
            taskStatuses.Clear();
            currentStatus = new ItemStatus
            {
                Name = name,
                Description = Description,
                Status = ItemBuildStatus.Pending,
                TimeCompleted = null,
                TimeOfEstimatedCompletion = null,
                TimeStarted = null
            };

            // Add each status
            foreach (ITask task in tasks)
            {
                ItemStatus taskItem = null;
                if (task is IStatusSnapshotGenerator)
                {
                    taskItem = (task as IStatusSnapshotGenerator).GenerateSnapshot();
                }
                else
                {
                    taskItem = new ItemStatus(task.GetType().Name);
                    taskItem.Status = ItemBuildStatus.Pending;
                }

                // Only add the item if it has been initialised
                if (taskItem != null)
                {
                    currentStatus.AddChild(taskItem);
                    taskStatuses.Add(task, taskItem);
                }
            }
        }
        #endregion
        #endregion
    }
}
