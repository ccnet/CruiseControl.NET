using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// Runs a number of tasks in sequence.
    /// </summary>
    [ReflectorType("sequential")]
    public class SequentialTask
        : TaskContainerBase
    {
        #region Public properties
        #region Tasks
        /// <summary>
        /// The tasks to run in sequence.
        /// </summary>
        [ReflectorProperty("tasks")]
        public override ITask[] Tasks
        {
            get { return base.Tasks; }
            set { base.Tasks = value; }
        }
        #endregion

        #region ContinueOnFailure
        /// <summary>
        /// Should the tasks continue to run, even if there is a failure?
        /// </summary>
        [ReflectorProperty("continueOnFailure", Required = false)]
        public bool ContinueOnFailure { get; set; }
        #endregion

        #region Logger
        /// <summary>
        /// The logger to use.
        /// </summary>
        public ILogger Logger { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region RunTasks()
        /// <summary>
        /// Runs the task, given the specified <see cref="IIntegrationResult"/>, in the specified <see cref="IProject"/>.
        /// </summary>
        /// <param name="result"></param>
        protected override void RunTasks(IIntegrationResult result)
        {
            // Initialise the task
            var logger = Logger ?? new DefaultLogger();
            var numberOfTasks = Tasks.Length;
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description)
                ? Description
                : string.Format("Running sequential tasks ({0} task(s))", numberOfTasks));
            logger.Info("Starting sequential task with {0} sub-task(s)", numberOfTasks);

            // Launch each task
            var successCount = 0;
            var failureCount = 0;
            for (var loop = 0; loop < numberOfTasks; loop++)
            {
                var taskName = string.Format("{0} [{1}]", Tasks[loop].GetType().Name, loop);
                logger.Debug("Starting task '{0}'", taskName);
                try
                {
                    // Start the actual task
                    var taskResult = result.Clone();
                    var task = Tasks[loop];
                    RunTask(task, taskResult);
                    result.Merge(taskResult);
                }
                catch (Exception error)
                {
                    // Handle any error details
                    result.ExceptionResult = error;
                    result.Status = IntegrationStatus.Exception;
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
                    if (!ContinueOnFailure) break;
                }
            }

            logger.Info("Sequential task completed: {0} successful, {1} failed", successCount, failureCount);
        }
        #endregion
        #endregion
    }
}
