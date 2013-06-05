//-----------------------------------------------------------------------
// <copyright file="SynchronisationTask.cs" company="Craig Sutherland">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// <para>
    /// A sychronisation context across multiple tasks or projects.
    /// </para>
    /// <para>
    /// Only one task can be in a synchronisation context at any time. This provides a mechanism for locking, either within a project or
    /// inbetween projects.
    /// </para>
    /// </summary>
    /// <title> Synchronisation Context Task </title>
    /// <version>1.5</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;synchronised&gt;
    /// &lt;tasks&gt;
    /// &lt;!-- Tasks defined here --&gt;
    /// &lt;/tasks&gt;
    /// &lt;/synchronised&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;synchronised continueOnFailure="true" context="thereCanBeOnlyOne" timeout="1200"&gt;
    /// &lt;description&gt;Example of how to run multiple tasks in a synchronisation context.&lt;/description&gt;
    /// &lt;tasks&gt;
    /// &lt;!-- Tasks defined here --&gt;
    /// &lt;/tasks&gt;
    /// &lt;/synchronised&gt;
    /// </code>
    /// </example>
    [ReflectorType("synchronised")]
    public class SynchronisationTask
        : TaskContainerBase
    {
        public SynchronisationTask()
        {
            this.TimeoutPeriod = 300;
        }


        #region Private fields
        #region contexts
        /// <summary>
        /// The synchronisation contexts.
        /// </summary>
        private static Dictionary<string, object> contexts = new Dictionary<string, object>();
        #endregion

        #region lockObject
        /// <summary>
        /// The lock object for accessing the synchronisation contexts.
        /// </summary>
        private static object lockObject = new object();
        #endregion
        #endregion

        #region Public properties
        #region Tasks
        /// <summary>
        /// The tasks to run within the synchronisation context. These tasks will be run in the order they are defined.
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
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; set; }
        #endregion

        #region ContextName
        /// <summary>
        /// The name of the synchronisation context. This is only needed if multiple synchronisation contexts are desired.
        /// </summary>
        /// <version>1.5</version>
        /// <default>DefaultSynchronisationContext</default>
        [ReflectorProperty("context", Required = false)]
        public string ContextName { get; set; }
        #endregion

        #region TimeoutPeriod
        /// <summary>
        /// The timeout period (in seconds).
        /// </summary>
        /// <remarks>
        /// The time-out is only used for attempting to aquire the context. If the task cannot acquire the context within this period, it
        /// will time out and throw an error. Once the context has been acquired, there is no time limit on how long it can be held.
        /// </remarks>
        /// <version>1.5</version>
        /// <default>300</default>
        [ReflectorProperty("timeout", Required = false)]
        public int TimeoutPeriod { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region Execute()
        /// <summary>
        /// Execute the actual task functionality.
        /// </summary>
        /// <param name="result">The result to use.</param>
        /// <returns>
        /// True if the task was successful, false otherwise.
        /// </returns>
        protected override bool Execute(IIntegrationResult result)
        {
            // Initialise the task
            var logger = Logger ?? new DefaultLogger();
            var numberOfTasks = Tasks.Length;
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description)
                ? Description
                : string.Format(System.Globalization.CultureInfo.CurrentCulture, "Running tasks in synchronisation context({0} task(s))", numberOfTasks));
            logger.Info("Starting synchronisation task with {0} sub-task(s)", numberOfTasks);
            var contextToUse = this.ContextName ?? "DefaultSynchronisationContext";

            // Make sure the context has been loaded
            lock (lockObject)
            {
                if (!contexts.ContainsKey(contextToUse))
                {
                    contexts.Add(contextToUse, new object());
                }
            }

            // Attempt to enter the synchronisation context
            if (Monitor.TryEnter(contexts[contextToUse], TimeoutPeriod * 1000))
            {
                try
                {
                    // Launch each task
                    var successCount = 0;
                    var failureCount = 0;
                    for (var loop = 0; loop < numberOfTasks; loop++)
                    {
                        var taskName = string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} [{1}]", Tasks[loop].GetType().Name, loop);
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
                    logger.Info("Parallel task completed: {0} successful, {1} failed", successCount, failureCount);
                }
                finally
                {
                    Monitor.Exit(contexts[contextToUse]);
                }
            }
            else
            {
                // Set the result as failed
                logger.Warning("Unable to enter synchonisation context!");
                result.Status = IntegrationStatus.Failure;
            }

            // Clean up
            this.CancelTasks();
            return (result.Status == IntegrationStatus.Success);
        }
        #endregion

        protected override string GetStatusInformation(RunningSubTaskDetails Details)
        {
            string Value = !string.IsNullOrEmpty(Description)
                            ? Description
                            : string.Format("Running synchronized tasks ({0} task(s))", Tasks.Length);

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
