namespace ThoughtWorks.CruiseControl.Core.Distribution
{
    using System;
    using System.ServiceModel;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;
    using System.ServiceModel.Channels;
    using ThoughtWorks.CruiseControl.Core.Distribution.Messages;

    /// <title>Remote Build Machine Definition</title>
    /// <version>1.6</version>
    /// <summary>
    /// Defines a remote machine that can perform builds.
    /// </summary>
    [ReflectorType("remoteMachine")]
    public class RemoteBuildMachine
        : IBuildMachine
    {
        #region Private fields
        private ChannelFactory<IRemoteBuildService> factory;
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// The name of the remote machine.
        /// </summary>
        /// <version>1.6</version>
        /// <default>n/a</default>
        [ReflectorProperty("name")]
        public string Name { get; set; }
        #endregion

        #region Address
        /// <summary>
        /// The address of the remote machine.
        /// </summary>
        /// <version>1.6</version>
        /// <default>n/a</default>
        [ReflectorProperty("address")]
        public string Address { get; set; }
        #endregion

        #region Logger
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Initialise()
        /// <summary>
        /// Initialises the machine.
        /// </summary>
        public void Initialise()
        {
            this.RetrieveLogger().Info("Initialising remote build machine connection to " + this.Address);
            var address = new EndpointAddress(this.Address);
            var binding = new NetTcpBinding();
            this.factory = new ChannelFactory<IRemoteBuildService>(binding, address);
        }
        #endregion

        #region Terminate()
        /// <summary>
        /// Terminates the machine.
        /// </summary>
        public void Terminate()
        {
            this.RetrieveLogger().Info("Terminating remote build machine connection to " + this.Address);
        }
        #endregion

        #region CanBuild()
        /// <summary>
        /// Determines whether this machine can build the specified project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>
        /// <c>true</c> if this instance can build the specified project; otherwise, <c>false</c>.
        /// </returns>
        public bool CanBuild(IProject project)
        {
            var request = new CheckIfBuildCanRunRequest
            {
                ProjectName = project.Name
            };
            var response = this.SendMessage(s => s.CheckIfBuildCanRun(request));
            return response.CanBuild;
        }
        #endregion

        #region StartBuild()
        /// <summary>
        /// Starts a build for the specified project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="result">The result.</param>
        /// <param name="buildCompleted">Any processing to perform when the build has completed.</param>
        /// <returns>The request for the build.</returns>
        public RemoteBuildRequest StartBuild(
            IProject project,
            IIntegrationResult result,
            Action<RemoteBuildRequest> buildCompleted)
        {
            // Send the request
            var request = new StartBuildRequest
            {
                ProjectDefinition = project.ConfigurationXml,
                ProjectName = project.Name,
                BuildCondition = result.IntegrationRequest.BuildCondition,
                BuildValues = result.IntegrationRequest.BuildValues,
                Source = result.IntegrationRequest.Source,
                UserName = result.IntegrationRequest.UserName
            };
            var response = this.SendMessage(s => s.StartBuild(request));

            // Build the request wrapper
            var buildRequest = new RemoteBuildRequest(
                this,
                response.BuildIdentifier,
                i =>
                {
                    var statusRequest = new RetrieveBuildStatusRequest
                    {
                        BuildIdentifier = i
                    };
                    return this.SendMessage(s => s.RetrieveBuildStatus(statusRequest));
                },
                r => buildCompleted(r));
            return buildRequest;
        }
        #endregion

        #region CancelBuild()
        /// <summary>
        /// Cancels a build.
        /// </summary>
        /// <param name="identifier">The identifier of the build.</param>
        public void CancelBuild(string identifier)
        {
            var request = new CancelBuildRequest
            {
                BuildIdentifier = identifier
            };
            this.SendMessage(s => s.CancelBuild(request));
        }
        #endregion
        #endregion

        #region Private methods
        #region RetrieveLogger()
        /// <summary>
        /// Retrieves the logger.
        /// </summary>
        /// <returns>A <see cref="ILogger"/> for logging messages.</returns>
        private ILogger RetrieveLogger()
        {
            if (this.Logger == null)
            {
                this.Logger = new DefaultLogger();
            }

            return this.Logger;
        }
        #endregion

        #region SendMessage()
        /// <summary>
        /// Sends a message to the remote machine.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="action">The action to perform.</param>
        /// <returns>The response from the remote machine.</returns>
        private TResponse SendMessage<TResponse>(Func<IRemoteBuildService, TResponse> action)
        {
            var remoteService = factory.CreateChannel();
            var remoteServiceClient = remoteService as IChannel;
            remoteServiceClient.Open();
            try
            {
                return action(remoteService);
            }
            finally
            {
                if (remoteServiceClient.State == CommunicationState.Opened)
                {
                    remoteServiceClient.Close();
                }
            }
        }
        #endregion
        #endregion
    }
}
