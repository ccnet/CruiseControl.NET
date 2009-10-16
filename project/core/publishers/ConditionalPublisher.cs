//-----------------------------------------------------------------------
// <copyright file="ConditionalPublisher.cs" company="CruiseControl.NET">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    using System;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// A container publisher that only executes the child publishers when the 
    /// condition (e.g. build status) is met.
    /// </summary>
    [ReflectorType("conditionalPublisher")]
    public class ConditionalPublisher
        : TaskContainerBase
    {
        #region Public properties
        #region Publishers
        /// <summary>
        /// Gets or sets the publishers to run if the condition is met.
        /// </summary>
        [ReflectorProperty("publishers")]
        public override ITask[] Tasks
        {
            get { return base.Tasks; }
            set { base.Tasks = value; }
        }
        #endregion

        #region Conditions
        /// <summary>
        /// Gets or sets the conditions to test for.
        /// </summary>
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
                        RunTask(publisher, result);
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

            return true;
        }
        #endregion
        #endregion
    }
}
