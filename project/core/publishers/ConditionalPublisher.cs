//-----------------------------------------------------------------------
// <copyright file="ConditionalPublisher.cs" company="CruiseControl.NET">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// A container publisher that only executes the child publishers when the 
    /// condition (e.g. build status) is met.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Currently the only available condition that can be checked is the state of the build.
    /// </para>
    /// </remarks>
    /// <title>Conditional Publisher</title>
    /// <version>1.5</version>
    /// <example>
    /// <code>
    /// &lt;conditionalPublisher&gt;
    /// &lt;conditions&gt;
    /// &lt;condition&gt;Success&lt;/condition&gt;
    /// &lt;/conditions&gt;
    /// &lt;publishers&gt;
    /// &lt;!-- Add publishers here --&gt;
    /// &lt;/publishers&gt;
    /// &lt;/conditionalPublisher&gt;
    /// </code>
    /// </example>
    [ReflectorType("conditionalPublisher")]
    public class ConditionalPublisher
        : TaskContainerBase
    {
        #region Public properties
        #region Publishers
        /// <summary>
        /// The publishers to run if the conditions are met.
        /// </summary>
        /// <default>n/a</default>
        /// <version>1.5</version>
        [ReflectorProperty("publishers")]
        public override ITask[] Tasks
        {
            get { return base.Tasks; }
            set { base.Tasks = value; }
        }
        #endregion

        #region Conditions
        /// <summary>
        /// A list of conditions of which at least one must be met in order to run the publishers.
        /// </summary>
        /// <default>n/a</default>
        /// <version>1.5</version>
        [ReflectorProperty("conditions", Required = true)]
        public IntegrationStatus[] Conditions { get; set; }
        #endregion

        #region Logger
        /// <summary>
        /// Gets or sets the logger to use.
        /// </summary>
        public ILogger Logger { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region Execute()
        /// <summary>
        /// Runs the task, given the specified <see cref="IIntegrationResult"/>, in the specified <see cref="IProject"/>.
        /// </summary>
        /// <param name="result">The results of the current build.</param>
        /// <returns><c>true</c> if the execution is successful; <c>false</c> otherwise.</returns>
        protected override bool Execute(IIntegrationResult result)
        {
            // Initialise the publisher
            var logger = this.Logger ?? new DefaultLogger();
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description)
                ? Description
                : "Checking conditional publishers");
            logger.Info("Checking conditional publishers");

            // Check the conditions
            var runPublishers = false;
            foreach (var condition in this.Conditions ?? new IntegrationStatus[0])
            {
                runPublishers |= condition == result.Status;
            }

            if (runPublishers)
            {
                // Run each publisher
                logger.Info("Conditions met - running publishers");
                for (var loop = 0; loop < this.Tasks.Length; loop++)
                {
                    var publisher = this.Tasks[loop];
                    logger.Debug("Running publisher #{0}", loop);
                    try
                    {
                        var taskResult = result.Clone();
                        RunTask(publisher, taskResult, new RunningSubTaskDetails(loop, result));
                        result.Merge(taskResult);
                    }
                    catch (Exception e)
                    {
                        logger.Error("Publisher threw exception: {0}", e.Message);
                    }
                }
            }
            else
            {
                logger.Info("Conditions not met - publishers not run");
            }

            // Clean up
            this.CancelTasks();
            return true;
        }
        #endregion

        protected override string GetStatusInformation(RunningSubTaskDetails Details)
        {
            string Value = !string.IsNullOrEmpty(Description)
                            ? Description
                            : string.Format("Running publishers ({0} task(s))", Tasks.Length);

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
