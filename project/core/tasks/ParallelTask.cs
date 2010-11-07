using System;
using Exortech.NetReflector;
using System.Threading;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// <para>
    /// Runs a set of child tasks in parallel. Each task will run at the same time as the other tasks.
    /// </para>
    /// <para>
    /// To run a set of tasks in sequential order within this task, use the <link>Sequential Task</link>.
    /// </para>
    /// </summary>
    /// <title>Parallel Task</title>
    /// <version>1.5</version>
    /// <example>
    /// <code>
    /// &lt;parallel&gt;
    /// &lt;tasks&gt;
    /// &lt;!-- Tasks defined here --&gt;
    /// &lt;/tasks&gt;
    /// &lt;/parallel&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// The following is an example of how to combine this task together to the <link>Sequential Task</link> to
    /// run multiple 'streams' of tasks in parallel:
    /// </para>
    /// <code>
    /// &lt;parallel&gt;
    /// &lt;tasks&gt;
    /// &lt;sequential&gt;
    /// &lt;description&gt;First parallel stream.&lt;/description&gt;
    /// &lt;tasks&gt;
    /// &lt;!-- First sequence of tasks--&gt;
    /// &lt;/tasks&gt;
    /// &lt;/sequential&gt;
    /// &lt;sequential&gt;
    /// &lt;description&gt;First parallel stream.&lt;/description&gt;
    /// &lt;tasks&gt;
    /// &lt;!-- Second sequence of tasks--&gt;
    /// &lt;/tasks&gt;
    /// &lt;/sequential&gt;
    /// &lt;/tasks&gt;
    /// &lt;/parallel&gt;
    /// </code>
    /// </remarks>
    [ReflectorType("parallel")]
    public class ParallelTask
        : TaskContainerBase
    {
        #region Public properties
        #region Tasks
        /// <summary>
        /// The tasks to run in parallel.
        /// </summary>
        /// <default>n/a</default>
        /// <version>1.5</version>
        [ReflectorProperty("tasks")]
        public override ITask[] Tasks
        {
            get { return base.Tasks; }
            set { base.Tasks = value; }
        }
        #endregion

        #region Logger
        /// <summary>
        /// The logger to use.
        /// </summary>
        public ILogger Logger { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Validates this task.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="parent"></param>
        /// <param name="errorProcesser"></param>
        public override void Validate(IConfiguration configuration, ConfigurationTrace parent, IConfigurationErrorProcesser errorProcesser)
        {
            base.Validate(configuration, parent, errorProcesser);
            var project = parent.GetAncestorValue<Project>();

            if (project != null)
            {
                // Check if this task is set in the publishers section
                var isPublisher = false;
                foreach (var publisher in project.Publishers)
                {
                    if (object.ReferenceEquals(publisher, this))
                    {
                        isPublisher = true;
                        break;
                    }
                }

                // Display a warning
                if (isPublisher)
                {
                    errorProcesser.ProcessWarning("Putting the parallel task in the publishers section may cause unpredictable results");
                }
            }
        }
        #endregion
        #endregion

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
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description)
                ? Description
                : string.Format("Running parallel tasks ({0} task(s))", numberOfTasks));
            logger.Info("Starting parallel task with {0} sub-task(s)", numberOfTasks);

            // Initialise the arrays
            var events = new ManualResetEvent[numberOfTasks];
            var results = new IIntegrationResult[numberOfTasks];

            // Launch each task using the ThreadPool
            var countLock = new object();
            var successCount = 0;
            var failureCount = 0;
            for (var loop = 0; loop < numberOfTasks; loop++)
            {
                events[loop] = new ManualResetEvent(false);
                results[loop] = result.Clone();
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    var taskNumber = (int)state;
                    var taskName = string.Format("{0} [{1}]", Tasks[taskNumber].GetType().Name, taskNumber);
                    try
                    {
                        Thread.CurrentThread.Name = string.Format("{0} [Parallel-{1}]", result.ProjectName, taskNumber);
                        logger.Debug("Starting task '{0}'", taskName);

                        // Start the actual task
                        var task = Tasks[taskNumber];
                        RunTask(task, results[taskNumber]);
                    }
                    catch (Exception error)
                    {
                        // Handle any error details
                        results[taskNumber].ExceptionResult = error;
                        results[taskNumber].Status = IntegrationStatus.Failure;
                        logger.Warning("Task '{0}' failed!", taskName);
                    }

                    // Record the results
                    lock (countLock)
                    {
                        if (results[taskNumber].Status == IntegrationStatus.Success)
                        {
                            successCount++;
                        }
                        else
                        {
                            failureCount++;
                        }
                    }

                    // Tell everyone the task is done
                    events[taskNumber].Set();
                }, loop);

            }

            // Wait for all the tasks to complete
            logger.Debug("Waiting for tasks to complete");
            WaitHandle.WaitAll(events);

            // Merge all the results
            logger.Info("Merging task results");
            foreach (var taskResult in results)
            {
                result.Merge(taskResult);
            }

            // Clean up
            this.CancelTasks();
            logger.Info("Parallel task completed: {0} successful, {1} failed", successCount, failureCount);
            return true;
        }
        #endregion
        #endregion
    }
}
