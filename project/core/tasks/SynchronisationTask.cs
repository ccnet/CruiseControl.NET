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
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// A task that provides a synchronisation context across projects.
    /// </summary>
    [ReflectorType("synchonised")]
    public class SynchronisationTask
        : TaskContainerBase
    {
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
        /// Gets or sets the child tasks.
        /// </summary>
        /// <value>An array of <see cref="ITask"/> instances.</value>
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
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; set; }
        #endregion

        #region ContextName
        /// <summary>
        /// Gets or sets the name of the context.
        /// </summary>
        /// <value>The name of the context.</value>
        [ReflectorProperty("context", Required = false)]
        public string ContextName { get; set; }
        #endregion

        #region TimeoutPeriod
        /// <summary>
        /// Gets or sets the timeout period.
        /// </summary>
        /// <value>The timeout period (in seconds).</value>
        [ReflectorProperty("timeout", Required = false)]
        public int? TimeoutPeriod { get; set; }
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
                : string.Format("Running tasks in synchronisation context({0} task(s))", numberOfTasks));
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
            if (Monitor.TryEnter(contexts[contextToUse], TimeoutPeriod.GetValueOrDefault(300) * 1000))
            {
                try
                {
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

            return (result.Status == IntegrationStatus.Success);
        }
        #endregion
        #endregion
    }
}
