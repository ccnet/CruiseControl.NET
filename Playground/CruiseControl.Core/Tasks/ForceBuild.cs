namespace CruiseControl.Core.Tasks
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using CruiseControl.Common;
    using CruiseControl.Core.Interfaces;
    using Ninject;
    using NLog;
    using Utilities;

    /// <summary>
    /// Forces a build on another project.
    /// </summary>
    public class ForceBuild
        : Task
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Public properties
        #region ProjectName
        /// <summary>
        /// Gets or sets the project name.
        /// </summary>
        /// <value>
        /// The name of the project to force.
        /// </value>
        public string ProjectName { get; set; }
        #endregion

        #region ServerAddress
        /// <summary>
        /// Gets or sets the server address.
        /// </summary>
        /// <value>
        /// The server address.
        /// </value>
        /// <remarks>
        /// If omitted this will be the local server.
        /// </remarks>
        public string ServerAddress { get; set; }
        #endregion

        #region ServerConnectionFactory
        /// <summary>
        /// Gets or sets the server connection factory.
        /// </summary>
        /// <value>
        /// The server connection factory.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Inject]
        public IServerConnectionFactory ServerConnectionFactory { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Called when this task is validated.
        /// </summary>
        /// <param name="validationLog">The validation log.</param>
        protected override void OnValidate(IValidationLog validationLog)
        {
            base.OnValidate(validationLog);
            if (string.IsNullOrEmpty(this.ProjectName))
            {
                validationLog.AddError("ProjectName has not been set");
            }
        }
        #endregion
        #endregion

        #region Protected methods
        #region OnRun()
        /// <summary>
        /// Executes this task.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The child tasks to execute.
        /// </returns>
        protected override IEnumerable<Task> OnRun(TaskExecutionContext context)
        {
            logger.Info("Sending force build to '{0}'", this.ProjectName);
            if (string.IsNullOrEmpty(this.ServerAddress))
            {
                this.SendLocalRequest(context);
            }
            else
            {
                this.SendRemoteRequest(context);
            }

            return null;
        }
        #endregion
        #endregion

        #region Private methods
        #region SendLocalRequest()
        /// <summary>
        /// Sends the request to the local server.
        /// </summary>
        /// <param name="context">The context.</param>
        private void SendLocalRequest(TaskExecutionContext context)
        {
            // Resolve the URN
            var urn = this.ProjectName;
            if (!UrnHelpers.IsCCNetUrn(urn))
            {
                urn = UrnHelpers.GenerateProjectUrn(this.Project.Server, this.ProjectName);
            }

            // Send the actual request
            logger.Debug("Performing local force build on '{0}'", urn);
            var arguments = new InvokeArguments
                                {
                                    Action = "ForceBuild"
                                };
            var result = this.Project.Server.ActionInvoker.Invoke(urn, arguments);

            // Check the result
            if (result.ResultCode == RemoteResultCode.Success)
            {
                var message = "Force build successfully sent to '" + ProjectName + "'";
                logger.Info(message);
                context.AddEntryToBuildLog(message);
            }
            else
            {
                var message = "Force build failed for '" + ProjectName + "' - result code " + result.ResultCode;
                logger.Info(message);
                context.AddEntryToBuildLog(message);
                context.CurrentStatus = IntegrationStatus.Failure;
            }
        }
        #endregion

        #region SendRemoteRequest()
        /// <summary>
        /// Sends the request to a remote server.
        /// </summary>
        /// <param name="context">The context.</param>
        private void SendRemoteRequest(TaskExecutionContext context)
        {
            // Generate the connection
            var connection = this.ServerConnectionFactory.GenerateConnection(this.ServerAddress);

            // Resolve the URN
            var urn = this.ProjectName;
            if (!UrnHelpers.IsCCNetUrn(urn))
            {
                urn = this.ServerConnectionFactory.GenerateUrn(this.ServerAddress, urn);
            }

            // Send the actual request
            logger.Debug("Sending force build on '{0}' to '{1}'", urn, this.ServerAddress);
            try
            {
                connection.Invoke(urn, "ForceBuild");
                var message = "Force build successfully sent to '" + ProjectName + "' at '" + this.ServerAddress + "'";
                logger.Info(message);
                context.AddEntryToBuildLog(message);
            }
            catch (RemoteServerException error)
            {
                var message = "Force build failed for '" + ProjectName + "' at '" + this.ServerAddress + "' - result code " + error.ResultCode;
                logger.Info(message);
                context.AddEntryToBuildLog(message);
                context.CurrentStatus = IntegrationStatus.Failure;
            }
        }
        #endregion
        #endregion
    }
}
