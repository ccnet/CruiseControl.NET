using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
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
        /// The publishers to run if the condition is met.
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
        /// The conditions to test for.
        /// </summary>
        [ReflectorProperty("conditions", Required = true)]
        public IntegrationStatus[] Conditions { get; set; }
        #endregion

        #region Logger
        /// <summary>
        /// The logger to use.
        /// </summary>
        public ILogger Logger { get; set; }
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
            // Initialise the publisher
            var logger = Logger ?? new DefaultLogger();
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description)
                ? Description
                : "Checking conditional publishers");
            logger.Info("Checking conditional publishers");

            // Check the conditions
            var runPublishers = false;
            foreach (var condition in Conditions ?? new IntegrationStatus[0])
            {
                runPublishers |= (condition == result.Status);
            }

            if (runPublishers)
            {
                // Run each publisher
                logger.Info("Conditions met - running publishers");
                for (var loop = 0; loop < Conditions.Length; loop++)
                {
                    var publisher = Tasks[loop];
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
