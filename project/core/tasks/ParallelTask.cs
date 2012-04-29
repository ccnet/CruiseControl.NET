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

        private class ParallelRunningSubTaskDetails : RunningSubTaskDetails
        {
            public ParallelRunningSubTaskDetails(int Index, IIntegrationResult ParentResult): base(Index, ParentResult)
            {
                this.Finished = false;
            }

            /// <summary>
            /// true if the task is finished.            
            /// This one has to be updated by you, should you need it
            /// </summary>
            public bool Finished { get; set; }
        }

        private ParallelRunningSubTaskDetails[] tasksDetails;

        protected override string GetStatusInformation(RunningSubTaskDetails Details)
        {
            string Value = !string.IsNullOrEmpty(Description)
                            ? Description
                            : string.Format("Running parallel tasks ({0} task(s))", Tasks.Length);

            if (Details != null)
            {
                Value += ": ";
                for (var loop = 0; loop < Tasks.Length; loop++)
                {
                    var Status = tasksDetails[loop];

                    if (!Status.Finished)
                        Value += string.Format("[{0}] {1} --- ",
                                                loop,
                                                !string.IsNullOrEmpty(Status.Information)
                                                ? Status.Information
                                                : "No information");
                }
            }

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
            tasksDetails = new ParallelRunningSubTaskDetails[numberOfTasks];
            result.BuildProgressInformation.SignalStartRunTask(GetStatusInformation(null));
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
                tasksDetails[loop] = new ParallelRunningSubTaskDetails(loop, result);
                ThreadPool.QueueUserWorkItem((state) =>
                {
                    var taskNumber = (int)state;
                    var taskName = string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0} [{1}]", Tasks[taskNumber].GetType().Name, taskNumber);
                    try
                    {
                        Thread.CurrentThread.Name = string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0} [Parallel-{1}]", result.ProjectName, taskNumber);
                        logger.Debug("Starting task '{0}'", taskName);

                        // Start the actual task
                        var task = Tasks[taskNumber];
                        var taskResult = results[taskNumber];
                        RunTask(task, taskResult, tasksDetails[taskNumber]);
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
                    tasksDetails[taskNumber].Finished = true;
                    tasksDetails[taskNumber].ParentResult.BuildProgressInformation.UpdateStartupInformation(GetStatusInformation(tasksDetails[taskNumber]));
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
