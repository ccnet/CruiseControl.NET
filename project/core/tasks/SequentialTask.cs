using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using System.Threading;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// Runs a number of tasks in sequence.
    /// </summary>
    [ReflectorType("sequential")]
    public class SequentialTask
        : TaskBase, ITask
    {
        #region Private fields
        private Dictionary<string, string> parameters;
        private IEnumerable<ParameterBase> parameterDefinitions;
        #endregion

        #region Public properties
        #region Tasks
        /// <summary>
        /// The tasks to run in sequence.
        /// </summary>
        [ReflectorProperty("tasks")]
        public ITask[] Tasks { get; set; }
        #endregion

        #region Description
        /// <summary>
        /// Description used for the visualisation of the buildstage, if left empty the process name will be shown
        /// </summary>
        [ReflectorProperty("description", Required = false)]
        public string Description { get; set; }
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

        #region Public methods
        #region Run()
        /// <summary>
        /// Runs the task, given the specified <see cref="IIntegrationResult"/>, in the specified <see cref="IProject"/>.
        /// </summary>
        /// <param name="result"></param>
        public virtual void Run(IIntegrationResult result)
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

                    if (task is IParamatisedTask)
                    {
                        (task as IParamatisedTask).ApplyParameters(parameters, parameterDefinitions);
                    }

                    task.Run(taskResult);
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
        #endregion
    }
}
