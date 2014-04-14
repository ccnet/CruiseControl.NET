using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// <para>
    /// Runs a set of child tasks in order.
    /// This task is primarily designed for scenarios where execution can take more than more path (e.g. <link>Parallel Task</link>). This
    /// is normally not required for tasks directly under the prebuild, tasks or publishers element in a project.
    /// </para>
    /// </summary>
    /// <title> Sequential Task </title>
    /// <version>1.5</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;sequential&gt;
    /// &lt;tasks&gt;
    /// &lt;!-- Tasks defined here --&gt;
    /// &lt;/tasks&gt;
    /// &lt;/sequential&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;sequential continueOnFailure="true"&gt;
    /// &lt;description&gt;Example of how to run multiple tasks in sequence.&lt;/description&gt;
    /// &lt;tasks&gt;
    /// &lt;!-- Tasks defined here --&gt;
    /// &lt;/tasks&gt;
    /// &lt;/sequential&gt;
    /// </code>
    /// </example>
    [ReflectorType("sequential")]
    public class SequentialTask
        : TaskContainerBase
    {
        #region Public properties
        #region Tasks
        /// <summary>
        /// The tasks to run in sequence.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
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
        /// <version>1.5</version>
        /// <default>false</default>
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

        protected override string GetStatusInformation(RunningSubTaskDetails Details)
        {
            string Value = !string.IsNullOrEmpty(Description)
                            ? Description
                            : string.Format("Running sequential tasks ({0} task(s))", Tasks.Length);

            if (Details != null)
                Value += string.Format(": [{0}] {1}",
                                        Details.Index,
                                        !string.IsNullOrEmpty(Details.Information)
                                         ? Details.Information
                                         : "No information");

            return Value;
        }


        #region Protected methods
        #region Execute()
        /// <summary>
        /// Runs the task, given the specified <see cref="IIntegrationResult"/>, in the specified <see cref="IProject"/>.
        /// </summary>
        /// <param name="result"></param>
        protected override bool Execute(IIntegrationResult result)
        {
            // Initialise the task
            var logger = Logger ?? new DefaultLogger();
            var numberOfTasks = Tasks.Length;
            result.BuildProgressInformation.SignalStartRunTask(GetStatusInformation(null));
            logger.Info("Starting sequential task with {0} sub-task(s)", numberOfTasks);

            // Launch each task
            var successCount = 0;
            var failureCount = 0;
            for (var loop = 0; loop < numberOfTasks; loop++)
            {
                var taskName = string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0} [{1}]", Tasks[loop].GetType().Name, loop);
                logger.Debug("Starting task '{0}'", taskName);

                try
                {
                    var taskResult = result.Clone();

                    // must reset the status so that we check for the current task failure and not a previous one
                    taskResult.Status = IntegrationStatus.Unknown;

                    // Start the actual task
                    var task = Tasks[loop];
                    RunTask(task, taskResult, new RunningSubTaskDetails(loop, result));
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
                    if (!ContinueOnFailure) break;
                }
            }

            // Clean up
            this.CancelTasks();
            logger.Info("Sequential task completed: {0} successful, {1} failed", successCount, failureCount);
            return failureCount == 0;
        }
        #endregion
        #endregion
    }
}
