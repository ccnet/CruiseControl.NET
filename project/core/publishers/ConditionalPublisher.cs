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
        : TaskBase, ITask
    {
        #region Private fields
        private Dictionary<string, string> parameters;
        private IEnumerable<ParameterBase> parameterDefinitions;
        #endregion

        #region Public properties
        #region Publishers
        /// <summary>
        /// The publishers to run if the condition is met.
        /// </summary>
        [ReflectorProperty("publishers")]
        public ITask[] Publishers { get; set; }
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

        #region Public methods
        #region Run()
        /// <summary>
        /// Runs the task, given the specified <see cref="IIntegrationResult"/>, in the specified <see cref="IProject"/>.
        /// </summary>
        /// <param name="result"></param>
        public virtual void Run(IIntegrationResult result)
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
                    var publisher = Publishers[loop];
                    logger.Debug("Running publisher #{0}", loop);
                    try
                    {
                        if (publisher is IParamatisedTask)
                        {
                            (publisher as IParamatisedTask).ApplyParameters(parameters, parameterDefinitions);
                        }

                        publisher.Run(result);
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
