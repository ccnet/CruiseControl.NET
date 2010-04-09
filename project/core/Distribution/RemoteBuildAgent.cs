namespace ThoughtWorks.CruiseControl.Core.Distribution
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Distribution.Messages;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;
    using System.Xml;

    /// <summary>
    /// A build agent to handle remote builds.
    /// </summary>
    [ReflectorType("remoteMachineAgent")]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single,
        InstanceContextMode = InstanceContextMode.Single)]
    public class RemoteBuildAgent
        : IBuildAgent, IDisposable, IRemoteBuildService
    {
        #region Private fields
        private ServiceHost serviceHost;
        private Dictionary<string, BuildStatusInformation> statusList = new Dictionary<string, BuildStatusInformation>();
        private Dictionary<string, IProject> projects = new Dictionary<string, IProject>();
        private object lockObject = new object();
        private int activeCount = 0;
        private int idCounter = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteBuildAgent"/> class.
        /// </summary>
        public RemoteBuildAgent()
        {
            this.NumberOfBuilds = 5;
        }
        #endregion

        #region Public properties
        #region ConfigurationReader
        /// <summary>
        /// Gets or sets the configuration reader.
        /// </summary>
        /// <value>The configuration reader.</value>
        public INetReflectorConfigurationReader ConfigurationReader { get; set; }
        #endregion
        
        #region Address
        /// <summary>
        /// The address that this agent listens on.
        /// </summary>
        /// <version>1.6</version>
        /// <default>n/a</default>
        [ReflectorProperty("address")]
        public string Address { get; set; }
        #endregion

        #region NumberOfBuilds
        /// <summary>
        /// The number of builds that are allowed to run.
        /// </summary>
        /// <version>1.6</version>
        /// <default>5</default>
        [ReflectorProperty("allowed", Required = false)]
        public int NumberOfBuilds { get; set; }
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
        /// Initialises the agent.
        /// </summary>
        public void Initialise()
        {
            this.RetrieveLogger().Info("Starting build agent at " + this.Address);
            this.serviceHost = new ServiceHost(this);
            var binding = new NetTcpBinding();
            this.serviceHost.AddServiceEndpoint(
                typeof(IRemoteBuildService),
                binding,
                this.Address);
            this.serviceHost.Open();
        }
        #endregion

        #region Terminate()
        /// <summary>
        /// Terminates the agent.
        /// </summary>
        public void Terminate()
        {
            this.Dispose();
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.serviceHost != null)
            {
                this.RetrieveLogger().Info("Stopping build agent at " + this.Address);
                try
                {
                    this.serviceHost.Close();
                }
                catch
                {
                    // Don't care what has happened
                }
                finally
                {
                    this.serviceHost = null;
                }
            }
        }
        #endregion

        #region CheckIfBuildCanRun()
        /// <summary>
        /// Checks if a build can run.
        /// </summary>
        /// <param name="request">The request detailing the project to run.</param>
        /// <returns>The response details.</returns>
        public CheckIfBuildCanRunResponse CheckIfBuildCanRun(CheckIfBuildCanRunRequest request)
        {
            this.RetrieveLogger().Debug(
                "Checking if project '" +
                request.ProjectName +
                "' can run");

            // Check if there is an available slot
            var canBuild = false;
            this.RunInLock(() => canBuild = this.activeCount < this.NumberOfBuilds);

            // Return the response
            var response = new CheckIfBuildCanRunResponse
            {
                CanBuild = canBuild
            };
            return response;
        }
        #endregion

        #region StartBuild()
        /// <summary>
        /// Starts a build.
        /// </summary>
        /// <param name="request">The request detailing the project to run.</param>
        /// <returns>The response details.</returns>
        public StartBuildResponse StartBuild(StartBuildRequest request)
        {
            // Update the counts and ids
            Thread.CurrentThread.Name = "R==>" + request.ProjectName;
            string id = null;
            var info = new BuildStatusInformation();
            this.RunInLock(() =>
            {
                id = this.idCounter.ToString();
                this.idCounter++;
                this.activeCount++;
                this.statusList.Add(id, info);
            });

            // Initialise the project
            IProject project = null;
            var crypto = new DefaultCryptoFunctions();
            var hash = crypto.GenerateHash(request.ProjectDefinition);
            if (!this.projects.TryGetValue(hash, out project))
            {
                this.RetrieveLogger().Debug(
                    "Deserialising project definition for '" + 
                    request.ProjectName +
                    "'");
                var document = new XmlDocument();
                document.LoadXml(request.ProjectDefinition);
                project = this.ConfigurationReader.ParseElement(document.DocumentElement) as Project;
                this.projects.Add(hash, project);
            }

            // Initialise the request
            var projectRequest = new IntegrationRequest(
                request.BuildCondition,
                request.Source,
                request.UserName);
            projectRequest.BuildValues = request.BuildValues ??
                new Dictionary<string, string>();

            // Start the actual run
            this.RetrieveLogger().Debug(
                "Starting project '" +
                request.ProjectName +
                "'");
            ThreadPool.QueueUserWorkItem(s =>
            {
                Thread.CurrentThread.Name = "R==>" + request.ProjectName;
                try
                {
                    info.Result = project.Integrate(projectRequest) ??
                        new IntegrationResult
                        {
                            Status = IntegrationStatus.Success
                        };
                    if (info.Result.Status == IntegrationStatus.Unknown)
                    {
                        info.Result.Status = IntegrationStatus.Success;
                    }
                }
                catch
                {
                    info.Result = new IntegrationResult
                    {
                        Status = IntegrationStatus.Exception
                    };
                }
                this.RetrieveLogger().Debug(
                    "Project '" +
                    request.ProjectName +
                    "' has completed");
                this.RunInLock(() =>
                {
                    this.activeCount--;
                });
            });

            // Generate the response
            var response = new StartBuildResponse
            {
                BuildIdentifier = id
            };
            return response;
        }
        #endregion

        #region CancelBuild()
        /// <summary>
        /// Cancels a build.
        /// </summary>
        /// <param name="request">The request detailing the build to cancel.</param>
        /// <returns>The response details.</returns>
        public CancelBuildResponse CancelBuild(CancelBuildRequest request)
        {
            // TODO: Cancel a build
            throw new NotImplementedException();
        }
        #endregion

        #region RetrieveBuildStatus()
        /// <summary>
        /// Retrieves the status of a build.
        /// </summary>
        /// <param name="request">The request detailing which build to retrieve.</param>
        /// <returns>The current status of the build.</returns>
        public RetrieveBuildStatusResponse RetrieveBuildStatus(RetrieveBuildStatusRequest request)
        {
            var buildStatus = IntegrationStatus.Unknown;
            this.RunInLock(() =>
            {
                BuildStatusInformation info;
                if (this.statusList.TryGetValue(request.BuildIdentifier, out info))
                {
                    buildStatus = info.Result == null ? 
                        IntegrationStatus.Unknown : 
                        info.Result.Status;
                }
            });

            // Return the current status
            var response = new RetrieveBuildStatusResponse
            {
                Status = buildStatus
            };
            return response;
        }
        #endregion
        #endregion

        #region Private methods
        #region RunInLock()
        /// <summary>
        /// Runs an action in an lock.
        /// </summary>
        /// <param name="action">The action to run.</param>
        /// <returns>
        /// <c>true</c> if the action has run; <c>false otherwise.</c>
        /// </returns>
        private bool RunInLock(Action action)
        {
            if (Monitor.TryEnter(this.lockObject, 5000))
            {
                try
                {
                    action();
                    return true;
                }
                finally
                {
                    Monitor.Exit(this.lockObject);
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

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
        #endregion

        #region Private classes
        private class BuildStatusInformation
        {
            public IIntegrationResult Result { get; set; }
        }
        #endregion
    }
}
