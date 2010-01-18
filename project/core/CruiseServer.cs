
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Logging;
using ThoughtWorks.CruiseControl.Core.Queues;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Events;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using ThoughtWorks.CruiseControl.Remote.Security;
using System.Web;
using System.Web.Caching;
using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// The Continuous Integration server.
    /// </summary>
    public class CruiseServer
        : CruiseServerEventsBase, ICruiseServer
    {
        #region Private fields
        private readonly IProjectSerializer projectSerializer;
        private readonly IConfigurationService configurationService;
		private readonly IFileSystem fileSystem;
		private readonly IExecutionEnvironment executionEnvironment;
        private IConfiguration configuration;
        [Obsolete]
        private readonly ICruiseManager manager;
        private readonly ICruiseServerClient serverClient;
        // TODO: Why the monitor? What reentrancy do we have? davcamer, dec 24 2008
        private readonly ManualResetEvent monitor = new ManualResetEvent(true);
        private ISecurityManager securityManager;
        private readonly List<ICruiseServerExtension> extensions = new List<ICruiseServerExtension>();
        private Dictionary<string, DateTime> receivedRequests = new Dictionary<string, DateTime>();
        private bool disposed;
        private IQueueManager integrationQueueManager;
        // TODO: Replace this with a proper IoC container
        private Dictionary<Type, object> services = new Dictionary<Type,object>();
    	private readonly string programmDataFolder;
        private object logCacheLock = new object();
        private TimeSpan cacheTime;
        #endregion

        #region Constructors
        public CruiseServer(IConfigurationService configurationService,
                            IProjectIntegratorListFactory projectIntegratorListFactory,
                            IProjectSerializer projectSerializer,
                            IProjectStateManager stateManager,
							IFileSystem fileSystem,
							IExecutionEnvironment executionEnvironment,
                            List<ExtensionConfiguration> extensionList)
        {
            Log.Trace();

            this.configurationService = configurationService;
            this.projectSerializer = projectSerializer;
			this.fileSystem = fileSystem;
			this.executionEnvironment = executionEnvironment;

            // Leave the manager for backwards compatability - it is marked as obsolete
#pragma warning disable 0618
            manager = new CruiseManager(this);
#pragma warning restore 0618
            serverClient = new CruiseServerClient(this);
            InitializeServerThread();

            // Initialise the configuration
            configuration = configurationService.Load();

            // Initialise the queue manager
            integrationQueueManager = IntegrationQueueManagerFactory.CreateManager(projectIntegratorListFactory, configuration, stateManager);
            integrationQueueManager.AssociateIntegrationEvents(OnIntegrationStarted, OnIntegrationCompleted);

            securityManager = configuration.SecurityManager;

            // Load the extensions
            if (extensionList != null)
            {
                InitialiseExtensions(extensionList);
            }

            this.configurationService.AddConfigurationUpdateHandler(new ConfigurationUpdateHandler(Restart));

        	programmDataFolder = this.executionEnvironment.GetDefaultProgramDataFolder(ApplicationType.Server);

            // Initialise the cache time
            var cacheTimeInConfig = ConfigurationManager.AppSettings["cacheTime"];
            if (string.IsNullOrEmpty(cacheTimeInConfig))
            {
                // Set the default cache time to five minutes
                this.cacheTime = new TimeSpan(0, 5, 0);
                Log.Info("Log cache time set to 5 minutes");
            }
            else
            {
                this.cacheTime = TimeSpan.FromSeconds(Convert.ToDouble(cacheTimeInConfig));
                if (this.cacheTime.TotalSeconds < 10)
                {
                    // Set the minimum cache time to ten seconds to prevent too much cache churn
                    this.cacheTime = new TimeSpan(0, 0, 10);
                }

                Log.Info("Log cache time set to " + this.cacheTime.TotalSeconds.ToString() + " seconds");
            }
        }
        #endregion

        #region Public properties
        #region CruiseManager
        /// <summary>
        /// Retrieve CruiseManager interface for the server
        /// </summary>
        [Obsolete("Use CruiseServerClient instead")]
        public ICruiseManager CruiseManager
        {
            get { return manager; }
        }
        #endregion

        #region CruiseServerClient
        /// <summary>
        /// Client for communicating with the server.
        /// </summary>
        public ICruiseServerClient CruiseServerClient
        {
            get { return serverClient; }
        }
        #endregion

        #region SecurityManager
        /// <summary>
        /// The underlying security manager.
        /// </summary>
        public ISecurityManager SecurityManager
        {
            get { return securityManager; }
        }
        #endregion
        #endregion

        #region Public methods
        #region InitialiseServices()
        /// <summary>
        /// Initialise the default services that are provided.
        /// </summary>
        public void InitialiseServices()
        {
            services.Add(typeof(IFileSystem), new SystemIoFileSystem());
            services.Add(typeof(ILogger), new DefaultLogger());
        }
        #endregion

        #region Start()
        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            Log.Info("Starting CruiseControl.NET Server");
            monitor.Reset();

            // Make sure the default program data folder exists
			if (!fileSystem.DirectoryExists(programmDataFolder))
            {
				Log.Info("Initialising data folder: '{0}'", programmDataFolder);
            	fileSystem.EnsureFolderExists(programmDataFolder);
            }

            integrationQueueManager.StartAllProjects();
            Log.Info("Initialising security");
            securityManager.Initialise();

            // Start the extensions
            Log.Info("Starting extensions");
            foreach (ICruiseServerExtension extension in extensions)
            {
                extension.Start();
            }
        }

        /// <summary>
        /// Attempts to start a project.
        /// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        public Response Start(ProjectRequest request)
        {
            Response response = RunProjectRequest(request,
                SecurityPermission.StartStopProject,
                SecurityEvent.StartProject,
                delegate(ProjectRequest arg, Response resp)
                {
                    // Perform the actual project start
                    if (!FireProjectStarting(arg.ProjectName))
                    {
                        integrationQueueManager.Start(arg.ProjectName);
                        FireProjectStarted(arg.ProjectName);
                    }
                });
            return response;
        }
        #endregion

        #region Stop()
        /// <summary>
        /// Stop all integrators, waiting until each integrator has completely stopped, before releasing any threads blocked by WaitForExit. 
        /// </summary>
        public void Stop()
        {
            // Stop the extensions
            Log.Info("Stopping extensions");
            foreach (ICruiseServerExtension extension in extensions)
            {
                extension.Stop();
            }

            Log.Info("Stopping CruiseControl.NET Server");
            integrationQueueManager.StopAllProjects();
            monitor.Set();
        }

        /// <summary>
        /// Attempts to stop a project.
        /// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        public Response Stop(ProjectRequest request)
        {
            Response response = RunProjectRequest(request,
                SecurityPermission.StartStopProject,
                SecurityEvent.StopProject,
                delegate(ProjectRequest arg, Response resp)
                {
                    // Perform the actual project start
                    if (!FireProjectStopping(arg.ProjectName))
                    {
                        integrationQueueManager.Stop(arg.ProjectName);
                        FireProjectStopped(arg.ProjectName);
                    }
                });
            return response;
        }
        #endregion

        #region Abort()
        /// <summary>
        /// Abort all integrators, waiting until each integrator has completely stopped, before releasing any threads blocked by WaitForExit. 
        /// </summary>
        public void Abort()
        {
            // Abort the extensions
            Log.Info("Aborting extensions");
            foreach (ICruiseServerExtension extension in extensions)
            {
                extension.Abort();
            }

            Log.Info("Aborting CruiseControl.NET Server");
            integrationQueueManager.Abort();
            monitor.Set();
        }
        #endregion

        #region Restart()
        /// <summary>
        /// Restart server by stopping all integrators, creating a new set of integrators from Configuration and then starting them.
        /// </summary>
        public void Restart()
        {
            Log.Info("Configuration changed: Restarting CruiseControl.NET Server ");

            configuration = configurationService.Load();
            integrationQueueManager.Restart(configuration);
            securityManager = configuration.SecurityManager;
            securityManager.Initialise();
        }
        #endregion

        #region WaitForExit()
        /// <summary>
        /// Block thread until all integrators to have been stopped or aborted.
        /// </summary>
        public void WaitForExit()
        {
            monitor.WaitOne();
        }

        /// <summary>
        /// Waits for the project thread to finish processing.
        /// </summary>
        /// <param name="request"></param>
        public virtual Response WaitForExit(ProjectRequest request)
        {
            Response response = RunProjectRequest(request,
                null,
                null,
                delegate(ProjectRequest arg, Response resp)
                {
                    integrationQueueManager.WaitForExit(arg.ProjectName);
                });
            return response;
        }
        #endregion

        #region ForceBuild()
        /// <summary>
        /// Forces a build for the named project.
        /// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        public virtual Response ForceBuild(ProjectRequest request)
        {
            Response response = RunProjectRequest(request,
                SecurityPermission.ForceAbortBuild,
                SecurityEvent.ForceBuild,
                delegate(ProjectRequest arg, Response resp)
                {
                    // Perform the actual force build
                    string userName = securityManager.GetDisplayName(arg.SessionToken);
                    if (!FireForceBuildReceived(arg.ProjectName, userName))
                    {
                        // Build the integration request
                        IntegrationRequest integrationRequest;
                        if (request is BuildIntegrationRequest)
                        {
                            BuildIntegrationRequest actualRequest = arg as BuildIntegrationRequest;
                            integrationRequest = new IntegrationRequest(actualRequest.BuildCondition, request.SourceName, userName);
                            integrationRequest.BuildValues = NameValuePair.ToDictionary(actualRequest.BuildValues);
                        }
                        else
                        {
                            integrationRequest = new IntegrationRequest(BuildCondition.ForceBuild, request.SourceName, userName);
                        }

                        // Send the request on
                        GetIntegrator(arg.ProjectName).Request(integrationRequest);
                        FireForceBuildProcessed(arg.ProjectName, userName);
                    }
                });
            return response;
        }
        #endregion

        #region AbortBuild()
        /// <summary>
        /// Aborts the build of the selected project.
        /// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        public virtual Response AbortBuild(ProjectRequest request)
        {
            Response response = RunProjectRequest(request,
                SecurityPermission.ForceAbortBuild,
                SecurityEvent.AbortBuild,
                delegate(ProjectRequest arg, Response resp)
                {
                    // Perform the actual abort build
                    string userName = securityManager.GetDisplayName(arg.SessionToken);
                    if (!FireAbortBuildReceived(arg.ProjectName, userName))
                    {
                        GetIntegrator(arg.ProjectName).AbortBuild(userName);
                        FireAbortBuildProcessed(arg.ProjectName, userName);
                    }
                });
            return response;
        }
        #endregion

        #region CancelPendingRequest()
        /// <summary>
        /// Cancel a pending project integration request from the integration queue.
        /// </summary>
        public virtual Response CancelPendingRequest(ProjectRequest request)
        {
            Response response = RunProjectRequest(request,
                SecurityPermission.ForceAbortBuild,
                SecurityEvent.CancelRequest,
                delegate(ProjectRequest arg, Response resp)
                {
                    // Perform the actual cancel request
                    integrationQueueManager.CancelPendingRequest(arg.ProjectName);
                });
            return response;
        }
        #endregion

        #region GetCruiseServerSnapshot()
        /// <summary>
        /// Gets the projects and integration queues snapshot from this server.
        /// </summary>
        public virtual SnapshotResponse GetCruiseServerSnapshot(ServerRequest request)
        {
            CruiseServerSnapshot snapshot = null;
            SnapshotResponse response = new SnapshotResponse(RunServerRequest(request,
                null,
                null,
                delegate(ServerRequest arg)
                {
                    snapshot = integrationQueueManager.GetCruiseServerSnapshot();
                    snapshot.ProjectStatuses = FilterProjects(request.SessionToken,
                        snapshot.ProjectStatuses);
                }));
            response.Snapshot = snapshot;
            return response;
        }
        #endregion

        #region GetProjectStatus()
        /// <summary>
        /// Gets information about the last build status, current activity and project name.
        /// for all projects on a cruise server
        /// </summary>
        public virtual ProjectStatusResponse GetProjectStatus(ServerRequest request)
        {
            ProjectStatus[] data = null;
            ProjectStatusResponse response = new ProjectStatusResponse(RunServerRequest(request,
                null,
                null,
                delegate(ServerRequest arg)
                {
                    data = integrationQueueManager.GetProjectStatuses();
                    data = FilterProjects(request.SessionToken, data);
                }));
            if (data != null) response.Projects.AddRange(data);
            return response;
        }
        #endregion

        #region SendMessage()
        /// <summary>
        /// Send a text message to the server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual Response SendMessage(MessageRequest request)
        {
            Response response = RunProjectRequest(request,
                SecurityPermission.SendMessage,
                SecurityEvent.SendMessage,
                delegate(ProjectRequest arg, Response resp)
                {
                    // Perform the actual send message
                    Log.Info("New message received: " + request.Message);
                    Message message = new Message(request.Message,request.Kind );
                    if (!FireSendMessageReceived(arg.ProjectName, message))
                    {
                        LookupProject(arg.ProjectName).AddMessage(message);
                        FireSendMessageProcessed(arg.ProjectName, message);
                    }
                });
            return response;
        }
        #endregion

        #region GetLatestBuildName()
        /// <summary>
        /// Returns the name of the most recent build for the specified project
        /// </summary>
        public DataResponse GetLatestBuildName(ProjectRequest request)
        {
            string data = null;
            DataResponse response = new DataResponse(RunProjectRequest(request,
                SecurityPermission.ViewProject,
                null,
                delegate(ProjectRequest arg, Response resp)
                {
                    data = GetIntegrator(arg.ProjectName).IntegrationRepository.GetLatestBuildName();
                }));
            response.Data = data;
            return response;
        }
        #endregion

        #region GetMostRecentBuildNames()
        /// <summary>
        /// Returns the names of the buildCount most recent builds for the specified project, sorted s.t. the newest build is first in the array
        /// </summary>
        public virtual DataListResponse GetMostRecentBuildNames(BuildListRequest request)
        {
            string[] data = { };
            DataListResponse response = new DataListResponse(RunProjectRequest(request,
                SecurityPermission.ViewProject,
                null,
                delegate(ProjectRequest arg, Response resp)
                {
                    data = GetIntegrator(request.ProjectName)
                        .IntegrationRepository
                        .GetMostRecentBuildNames(request.NumberOfBuilds);
                }));
            if (data != null) response.Data.AddRange(data);
            return response;
        }
        #endregion

        #region GetBuildNames()
        /// <summary>
        /// Returns the names of all builds for the specified project, sorted s.t. the newest build is first in the array
        /// </summary>
        public DataListResponse GetBuildNames(ProjectRequest request)
        {
            List<string> data = new List<string>();
            DataListResponse response = new DataListResponse(RunProjectRequest(request,
                SecurityPermission.ViewProject,
                null,
                delegate(ProjectRequest arg, Response resp)
                {
                    data.AddRange(GetIntegrator(arg.ProjectName).
                        IntegrationRepository.
                        GetBuildNames());
                }));
            response.Data = data;
            return response;
        }
        #endregion

        #region GetLog()
        /// <summary>
        /// Returns the build log contents for requested project and build name
        /// </summary>
        public virtual DataResponse GetLog(BuildRequest request)
        {
            string data = null;
            DataResponse response = new DataResponse(RunProjectRequest(request,
                SecurityPermission.ViewProject,
                null,
                delegate(ProjectRequest arg, Response resp)
                {
                    data = this.RetrieveLogData(request.ProjectName, request.BuildName);
                }));
            response.Data = data;

            // Perform a garbage collection to reduce the amount of memory held
            GC.Collect();
            return response;
        }
        #endregion

        #region GetServerLog()
        /// <summary>
        /// Retrieves the server log.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DataResponse GetServerLog(ServerRequest request)
        {
            string data = null;
            DataResponse response = null;
            if (request is ProjectRequest)
            {
                response = new DataResponse(RunProjectRequest(request as ProjectRequest,
                    SecurityPermission.ViewConfiguration,
                    null,
                    delegate(ProjectRequest arg, Response resp)
                    {
                        data = new ServerLogFileReader().Read((arg as ProjectRequest).ProjectName);
                    }));
            }
            else
            {
                response = new DataResponse(RunServerRequest(request,
                    SecurityPermission.ViewConfiguration,
                    null,
                    delegate(ServerRequest arg)
                    {
                        data = new ServerLogFileReader().Read();
                    }));
            }
            response.Data = data;
            return response;
        }
        #endregion

        #region AddProject()
        /// <summary>
        /// Adds a project to the server
        /// </summary>
        public virtual Response AddProject(ChangeConfigurationRequest request)
        {
            Response response = RunProjectRequest(request,
                SecurityPermission.ChangeProjectConfiguration,
                SecurityEvent.AddProject,
                delegate(ProjectRequest arg, Response resp)
                {
                    Log.Info("Adding project - " + request.ProjectDefinition);
                    try
                    {
                        IConfiguration configuration = configurationService.Load();
                        IProject project = projectSerializer.Deserialize(request.ProjectDefinition);
                        configuration.AddProject(project);
                        project.Initialize();
                        configurationService.Save(configuration);
                    }
                    catch (ApplicationException e)
                    {
                        Log.Warning(e);
                        throw new CruiseControlException("Failed to add project. Exception was - " + e.Message, e);
                    }
                });
            return response;
        }
        #endregion

        #region DeleteProject()
        /// <summary>
        /// Deletes the specified project from the server
        /// </summary>
        public virtual Response DeleteProject(ChangeConfigurationRequest request)
        {
            Response response = RunProjectRequest(request,
                SecurityPermission.ChangeProjectConfiguration,
                SecurityEvent.DeleteProject,
                delegate(ProjectRequest arg, Response resp)
                {
                    Log.Info("Deleting project - " + request.ProjectName);
                    try
                    {
                        IConfiguration configuration = configurationService.Load();
                        configuration.Projects[request.ProjectName]
                            .Purge(request.PurgeWorkingDirectory,
                                request.PurgeArtifactDirectory,
                                request.PurgeSourceControlEnvironment);
                        configuration.DeleteProject(request.ProjectName);
                        configurationService.Save(configuration);
                    }
                    catch (Exception e)
                    {
                        Log.Warning(e);
                        throw new CruiseControlException("Failed to delete project. Exception was - " + e.Message, e);
                    }
                });
            return response;
        }
        #endregion

        #region UpdateProject()
        /// <summary>
        /// Updates the selected project on the server
        /// </summary>
        public virtual Response UpdateProject(ChangeConfigurationRequest request)
        {
            Response response = RunProjectRequest(request,
                SecurityPermission.ChangeProjectConfiguration,
                SecurityEvent.UpdateProject,
                delegate(ProjectRequest arg, Response resp)
                {
                    Log.Info("Updating project - " + request.ProjectName);
                    try
                    {
                        IConfiguration configuration = configurationService.Load();
                        configuration.Projects[request.ProjectName].Purge(true, false, true);
                        configuration.DeleteProject(request.ProjectName);
                        IProject project = projectSerializer.Deserialize(request.ProjectDefinition);
                        configuration.AddProject(project);
                        project.Initialize();
                        configurationService.Save(configuration);
                    }
                    catch (ApplicationException e)
                    {
                        Log.Warning(e);
                        throw new CruiseControlException("Failed to add project. Exception was - " + e.Message, e);
                    }
                });
            return response;
        }
        #endregion

        #region GetProject()
        public DataResponse GetProject(ProjectRequest request)
        {
            string data = null;
            DataResponse response = new DataResponse(RunProjectRequest(request,
                SecurityPermission.ViewConfiguration,
                null,
                delegate(ProjectRequest arg, Response resp)
                {
                    Log.Info("Getting project - " + request.ProjectName);
                    data = new NetReflectorProjectSerializer()
                        .Serialize(configurationService.Load().Projects[arg.ProjectName]);
                }));
            response.Data = data;
            return response;
        }
        #endregion

        #region GetServerVersion()
        public DataResponse GetServerVersion(ServerRequest request)
        {
            string data = null;
            DataResponse response = new DataResponse(RunServerRequest(request,
                null,
                null,
                delegate(ServerRequest arg)
                {
                    Log.Trace("Returning version number");
                    try
                    {
                        data = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    }
                    catch (ApplicationException e)
                    {
                        Log.Warning(e);
                        throw new CruiseControlException("Failed to get project version . Exception was - " + e.Message, e);
                    }
                }));
            response.Data = data;
            return response;
        }
        #endregion

        #region GetExternalLinks()
        public ExternalLinksListResponse GetExternalLinks(ProjectRequest request)
        {
            List<ExternalLink> data = new List<ExternalLink>();
            ExternalLinksListResponse response = new ExternalLinksListResponse(RunProjectRequest(request,
                SecurityPermission.ViewProject,
                null,
                delegate(ProjectRequest arg, Response resp)
                {
                    data.AddRange(LookupProject(arg.ProjectName).ExternalLinks);
                }));
            response.ExternalLinks = data;
            return response;
        }
        #endregion

        #region GetArtifactDirectory()
        public DataResponse GetArtifactDirectory(ProjectRequest request)
        {
            string data = null;
            DataResponse response = new DataResponse(RunProjectRequest(request,
                SecurityPermission.ViewProject,
                null,
                delegate(ProjectRequest arg, Response resp)
                {
                    data = GetIntegrator(arg.ProjectName).Project.ArtifactDirectory;
                }));
            response.Data = data;
            return response;
        }
        #endregion

        #region GetStatisticsDocument()
        public DataResponse GetStatisticsDocument(ProjectRequest request)
        {
            string data = null;
            DataResponse response = new DataResponse(RunProjectRequest(request,
                SecurityPermission.ViewProject,
                null,
                delegate(ProjectRequest arg, Response resp)
                {
                    data = GetIntegrator(arg.ProjectName).Project.Statistics;
                }));
            response.Data = data;
            return response;
        }
        #endregion

        #region GetModificationHistoryDocument()
        public DataResponse GetModificationHistoryDocument(ProjectRequest request)
        {
            string data = null;
            DataResponse response = new DataResponse(RunProjectRequest(request,
                SecurityPermission.ViewProject,
                null,
                delegate(ProjectRequest arg, Response resp)
                {
                    data = GetIntegrator(arg.ProjectName).Project.ModificationHistory;
                }));
            response.Data = data;
            return response;
        }
        #endregion

        #region GetRSSFeed()
        public DataResponse GetRSSFeed(ProjectRequest request)
        {
            string data = null;
            DataResponse response = new DataResponse(RunProjectRequest(request,
                SecurityPermission.ViewProject,
                null,
                delegate(ProjectRequest arg, Response resp)
                {
                    data = GetIntegrator(arg.ProjectName).Project.RSSFeed;
                }));
            response.Data = data;
            return response;
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Dispose this object.
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (disposed) return;
                disposed = true;
            }
            Abort();
        }
        #endregion

        #region GetFreeDiskSpace()
        /// <summary>
        /// Retrieves the amount of free disk space.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The amount of free space in bytes.</returns>
        public DataResponse GetFreeDiskSpace(ServerRequest request)
        {
            string data = null;
            DataResponse response = new DataResponse(RunServerRequest(request,
                null,
                null,
                delegate(ServerRequest arg)
                {
                    //TODO: this is currently a hack
                    // this method sould return a collection of drives used by ccnet
                    // since each project can be hostet on a different drive.
                    // We should determine the drives used by a project
                    // (working, artefacs, SCM checkout, build publisher, etc)
                    // of each project and return a list of free disc space for every used drive.
                    var drive = ConfigurationManager.AppSettings["DataDrive"];
                    if (string.IsNullOrEmpty(drive))
                    {
                        if (System.IO.Path.DirectorySeparatorChar == '/')
                            drive = "/";
                        else
                            drive = "C:";
                    }

                    var fileSystem = new SystemIoFileSystem();
                    data = fileSystem.GetFreeDiskSpace(drive).ToString();
                }));

            response.Data = data;
            return response;
        }
        #endregion

        #region TakeStatusSnapshot()
        /// <summary>
        /// Takes a status snapshot of a project.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The snapshot of the current status.</returns>
        public virtual StatusSnapshotResponse TakeStatusSnapshot(ProjectRequest request)
        {
            ProjectStatusSnapshot snapshot = null;
            StatusSnapshotResponse response = new StatusSnapshotResponse(RunProjectRequest(request,
                SecurityPermission.ViewProject,
                null,
                delegate(ProjectRequest arg, Response resp)
                {
                    IProjectIntegrator integrator = GetIntegrator(request.ProjectName);
                    if (integrator != null)
                    {
                        // First see if the project has its own associated generator
                        if (integrator.Project is IStatusSnapshotGenerator)
                        {
                            snapshot = (integrator.Project as IStatusSnapshotGenerator).GenerateSnapshot()
                                as ProjectStatusSnapshot;
                        }
                        else
                        {
                            // Otherwise generate an overview snapshot (details will not be available)
                            ProjectStatus status = integrator.Project.CreateProjectStatus(integrator);
                            snapshot = new ProjectStatusSnapshot();
                            snapshot.Name = integrator.Project.Name;
                            if (status.Activity.IsBuilding())
                            {
                                snapshot.Status = ItemBuildStatus.Running;
                            }
                            else if (status.Activity.IsPending())
                            {
                                snapshot.Status = ItemBuildStatus.Pending;
                            }
                            else if (status.Activity.IsSleeping())
                            {
                                switch (status.BuildStatus)
                                {
                                    case IntegrationStatus.Success:
                                        snapshot.Status = ItemBuildStatus.CompletedSuccess;
                                        break;
                                    case IntegrationStatus.Exception:
                                    case IntegrationStatus.Failure:
                                        snapshot.Status = ItemBuildStatus.CompletedSuccess;
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new NoSuchProjectException(request.ProjectName);
                    }
                }));
            response.Snapshot = snapshot;
            return response;
        }
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves the latest list of packages for a project.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual ListPackagesResponse RetrievePackageList(ProjectRequest request)
        {
            List<PackageDetails> packages = null;
            ListPackagesResponse response = new ListPackagesResponse(RunProjectRequest(request,
                SecurityPermission.ViewProject,
                null,
                delegate(ProjectRequest arg, Response resp)
                {
                    if (request is BuildRequest)
                    {
                        var actualRequest = request as BuildRequest;
                        packages = GetIntegrator(request.ProjectName).Project.RetrievePackageList(actualRequest.BuildName);
                    }
                    else
                    {
                        packages = GetIntegrator(request.ProjectName).Project.RetrievePackageList();
                    }
                }));
            response.Packages = packages;
            return response;
        }
        #endregion

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieve a file transfer object.
        /// </summary>
        /// <param name="request"></param>
        public virtual FileTransferResponse RetrieveFileTransfer(FileTransferRequest request)
        {
            var response = new FileTransferResponse(request);
            try
            {
                // Validate that the path is valid
                var sourceProject = GetIntegrator(request.ProjectName).Project;
                var filePath = Path.Combine(sourceProject.ArtifactDirectory, request.FileName);
                var fileInfo = new FileInfo(filePath);
                if (!fileInfo.FullName.StartsWith(sourceProject.ArtifactDirectory, StringComparison.InvariantCultureIgnoreCase))
                {
                    var message = string.Format("Files can only be retrieved from the artefact folder - unable to retrieve {0}", request.FileName);
                    Log.Warning(message);
                    throw new CruiseControlException(message);
                }
                else if (fileInfo.FullName.StartsWith(Path.Combine(sourceProject.ArtifactDirectory, "buildlogs"), StringComparison.InvariantCultureIgnoreCase))
                {
                    var message = string.Format("Unable to retrieve files from the build logs folder - unable to retrieve {0}", request.FileName);
                    Log.Warning(message);
                    throw new CruiseControlException(message);
                }

                RemotingFileTransfer fileTransfer = null;
                if (fileInfo.Exists)
                {
                    Log.Debug(string.Format("Retrieving file '{0}' from '{1}'", request.FileName, request.ProjectName));
                    fileTransfer = new RemotingFileTransfer(File.OpenRead(filePath));
                }
                else
                {
                    Log.Warning(string.Format("Unable to find file '{0}' in '{1}'", request.FileName, request.ProjectName));
                }
                response.FileTransfer = fileTransfer;
                response.Result = ResponseResult.Success;
            }
            catch (Exception error)
            {
                response.Result = ResponseResult.Failure;
                response.ErrorMessages.Add(
                    new ErrorMessage(error.Message));
            }
            return response;
        }
        #endregion

        #region Login()
        /// <summary>
        /// Logs a user into the session and generates a session.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual LoginResponse Login(LoginRequest request)
        {
            string sessionToken = null;
            LoginResponse response = new LoginResponse(RunServerRequest(request,
                null,
                null,
                delegate(ServerRequest arg)
                {
                    sessionToken = securityManager.Login(request);
                }));
            response.SessionToken = sessionToken;
            return response;
        }
        #endregion

        #region Logout()
        /// <summary>
        /// Logs a user out of the system and removes their session.
        /// </summary>
        /// <param name="request"></param>
        public virtual Response Logout(ServerRequest request)
        {
            Response response = RunServerRequest(request,
                null,
                null,
                delegate(ServerRequest arg)
                {
                    securityManager.Logout(request.SessionToken);
                });
            return response;
        }
        #endregion

        #region GetSecurityConfiguration()
        /// <summary>
        /// Retrieves the security configuration.
        /// </summary>
        /// <param name="request"></param>
        public virtual DataResponse GetSecurityConfiguration(ServerRequest request)
        {
            Log.Info("GetSecurityConfiguration");
            string configData = null;
            DataResponse response = new DataResponse(RunServerRequest(request,
                SecurityPermission.ViewSecurity,
                SecurityEvent.GetSecurityConfiguration,
                delegate(ServerRequest arg)
                {
                    ServerSecurityConfigurationInformation config = new ServerSecurityConfigurationInformation();
                    config.Manager = securityManager;
                    foreach (IProject project in configuration.Projects)
                    {
                        config.AddProject(project);
                    }
                    configData = config.ToString();
                }));
            return response;
        }
        #endregion

        #region ListUsers()
        /// <summary>
        /// Lists all the users who have been defined in the system.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// A list of <see cref="UserNameCredentials"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        public virtual ListUsersResponse ListUsers(ServerRequest request)
        {
            Log.Info("Listing users");
            List<UserDetails> users = new List<UserDetails>();
            ListUsersResponse response = new ListUsersResponse(RunServerRequest(request,
                SecurityPermission.ViewSecurity,
                SecurityEvent.ListAllUsers,
                delegate(ServerRequest arg)
                {
                    users = securityManager.ListAllUsers();
                }));
            response.Users = users;
            return response;
        }
        #endregion

        #region DiagnoseSecurityPermissions()
        /// <summary>
        /// Checks the security permissions for a user against one or more projects.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A set of diagnostics information.</returns>
        public DiagnoseSecurityResponse DiagnoseSecurityPermissions(DiagnoseSecurityRequest request)
        {
            List<SecurityCheckDiagnostics> diagnoses = new List<SecurityCheckDiagnostics>();
            DiagnoseSecurityResponse response = new DiagnoseSecurityResponse(RunServerRequest(request,
                SecurityPermission.ViewSecurity,
                SecurityEvent.DiagnoseSecurityPermissions,
                delegate(ServerRequest arg)
                {
                    Array permissions = Enum.GetValues(typeof(SecurityPermission));
                    foreach (string projectName in request.Projects)
                    {
                        if (string.IsNullOrEmpty(projectName))
                        {
                            Log.Info(string.Format("DiagnoseServerPermission for user {0}", request.UserName));
                        }
                        else
                        {
                            Log.Info(string.Format("DiagnoseProjectPermission for user {0} project {1}", request.UserName, projectName));
                        }
                        foreach (SecurityPermission permission in permissions)
                        {
                            SecurityCheckDiagnostics diagnostics = new SecurityCheckDiagnostics();
                            diagnostics.Permission = permission.ToString();
                            diagnostics.Project = projectName;
                            diagnostics.User = request.UserName;
                            diagnostics.IsAllowed = DiagnosePermission(request.UserName, projectName, permission);
                            diagnoses.Add(diagnostics);
                        }
                    }
                }));
            response.Diagnostics = diagnoses;
            return response;
        }
        #endregion

        #region ReadAuditRecords()
        /// <summary>
        /// Reads the specified number of filtered audit events.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        public ReadAuditResponse ReadAuditRecords(ReadAuditRequest request)
        {
            List<AuditRecord> records = new List<AuditRecord>();
            ReadAuditResponse response = new ReadAuditResponse(RunServerRequest(request,
                SecurityPermission.ViewSecurity,
                SecurityEvent.ViewAuditLog,
                delegate(ServerRequest arg)
                {
                    records = securityManager.ReadAuditRecords(request.StartRecord,
                        request.NumberOfRecords,
                        request.Filter);
                }));
            response.Records = records;
            return response;
        }
        #endregion

        #region ChangePassword()
        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        /// <param name="request"></param>
        public virtual Response ChangePassword(ChangePasswordRequest request)
        {
            Response response = RunServerRequest(request,
                null,
                null,
                delegate(ServerRequest arg)
                {
                    string displayName = securityManager.GetDisplayName(request.SessionToken);
                    Log.Debug(string.Format("Changing password for '{0}'", displayName));
                    securityManager.ChangePassword(request.SessionToken,
                        request.OldPassword,
                        request.NewPassword);
                });
            return response;
        }
        #endregion

        #region ResetPassword()
        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        /// <param name="request"></param>
        public virtual Response ResetPassword(ChangePasswordRequest request)
        {
            Response response = RunServerRequest(request,
                null,
                null,
                delegate(ServerRequest arg)
                {
                    string displayName = securityManager.GetDisplayName(request.SessionToken);
                    Log.Debug(string.Format("'{0}' is resetting password for '{1}'", displayName, request.UserName));
                    securityManager.ResetPassword(request.SessionToken,
                        request.UserName,
                        request.NewPassword);
                });
            return response;
        }
        #endregion

        #region ListBuildParameters()
        /// <summary>
        /// Lists the build parameters for a project.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The list of parameters (if any).</returns>
        public virtual BuildParametersResponse ListBuildParameters(ProjectRequest request)
        {
            List<ParameterBase> parameters = new List<ParameterBase>();
            BuildParametersResponse response = new BuildParametersResponse(RunProjectRequest(request,
                SecurityPermission.ViewProject,
                null,
                delegate(ProjectRequest arg, Response resp)
                {
                    IProjectIntegrator projectIntegrator = GetIntegrator(arg.ProjectName);
                    if (projectIntegrator == null) throw new NoSuchProjectException(arg.ProjectName);
                    IProject project = projectIntegrator.Project;
                    if (project is IParamatisedProject)
                    {
                        parameters = (project as IParamatisedProject).ListBuildParameters();
                    }
                }));
            response.Parameters = parameters;
            return response;
        }
        #endregion

        #region RetrieveService()
        /// <summary>
        /// Retrieves a service.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <returns>A valid service, if found, null otherwise.</returns>
        public virtual object RetrieveService(Type serviceType)
        {
            if (services.ContainsKey(serviceType))
            {
                return services[serviceType];
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region AddService()
        /// <summary>
        /// Adds a service.
        /// </summary>
        /// <param name="serviceType">The type of service.</param>
        /// <param name="service">The service to add.</param>
        public virtual void AddService(Type serviceType, object service)
        {
            if (service != null)
            {
                services[serviceType] = service;
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region ValidateRequest()
        /// <summary>
        /// Validates an incoming request.
        /// </summary>
        /// <param name="request"></param>
        private void ValidateRequest(ServerRequest request)
        {
            try
            {
                // Validate any channel information
                if (securityManager.Channel != null)
                {
                    securityManager.Channel.Validate(request.ChannelInformation);
                }

                // Validate the time
                if (request.Timestamp < DateTime.Now.AddDays(-1))
                {
                    throw new CruiseControlException("Request is too old");
                }

                // Make sure the message isn't duplicated
                if (receivedRequests.ContainsKey(request.Identifier))
                {
                    if (receivedRequests.ContainsKey(request.Identifier))
                    {
                        if (receivedRequests[request.Identifier] < DateTime.Now.AddDays(-1))
                        {
                            receivedRequests.Remove(request.Identifier);
                        }
                        else
                        {
                            throw new CruiseControlException("This request has already been processed");
                        }
                        receivedRequests.Add(request.Identifier, request.Timestamp);
                    }
                }
            }
            catch (SecurityException error)
            {
                Log.Warning("Message validation failed: {0}", error.Message);
                throw;
            }
        }
        #endregion

        #region RunProjectRequest()
        /// <summary>
        /// Encapsulates the code to process a request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="permission"></param>
        /// <param name="eventType"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private Response RunProjectRequest(ProjectRequest request,
            SecurityPermission? permission,
            SecurityEvent? eventType,
            ProjectRequestAction action)
        {
            var response = new Response(request);
            try
            {
                // Validate the request and check the security token
                ValidateRequest(request);
                if (permission.HasValue)
                {
                    CheckSecurity(request.SessionToken,
                        request.ProjectName,
                        permission.Value,
                        eventType);
                }

                // Perform the actual action
                action(request, response);
                if (response.Result == ResponseResult.Unknown)
                {
                    response.Result = ResponseResult.Success;
                }
            }
            catch (Exception error)
            {
                response.Result = ResponseResult.Failure;
                response.ErrorMessages.Add(
                    new ErrorMessage(
                        error.Message,
                        error.GetType().Name));
            }
            return response;
        }
        #endregion

        #region RunServerRequest()
        /// <summary>
        /// Encapsulates the code to process a request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="permission"></param>
        /// <param name="eventType"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private Response RunServerRequest(ServerRequest request,
            SecurityPermission? permission,
            SecurityEvent? eventType,
            Action<ServerRequest> action)
        {
            Response response = new Response(request);
            try
            {
                // Validate the request and check the security token
                ValidateRequest(request);
                if (permission.HasValue)
                {
                    CheckSecurity(request.SessionToken,
                        null,
                        permission.Value,
                        eventType);
                }

                // Perform the actual action
                action(request);
                response.Result = ResponseResult.Success;
            }
            catch (Exception error)
            {
                // Security exceptions have already been logged, just need to log any other exception
                if (!(error is SecurityException)) Log.Error(error);

                // Tell the caller the request failed and include the error message (but not the stack trace!)
                response.Result = ResponseResult.Failure;
                response.ErrorMessages.Add(
                    new ErrorMessage(
                        error.Message,
                        error.GetType().Name));
            }
            return response;
        }
        #endregion

        #region FilterProjects()
        /// <summary>
        /// Filters a list of projects and only returns the projects that a user is allowed to view.
        /// </summary>
        /// <param name="sessionToken">The session token to use in filtering.</param>
        /// <param name="projects">The projects to filter.</param>
        /// <returns>The filtered projects.</returns>
        private ProjectStatus[] FilterProjects(string sessionToken,
            ProjectStatus[] projects)
        {
            List<ProjectStatus> allowedProjects = new List<ProjectStatus>();
            string userName = securityManager.GetUserName(sessionToken);
            bool defaultIsAllowed = (securityManager.GetDefaultRight(SecurityPermission.ViewProject) == SecurityRight.Allow);
            foreach (ProjectStatus project in projects)
            {
                IProjectIntegrator projectIntegrator = GetIntegrator(project.Name);
                bool isAllowed = true;
                if (projectIntegrator != null)
                {
                    IProjectAuthorisation authorisation = projectIntegrator.Project.Security;
                    if ((authorisation != null) && authorisation.RequiresSession(securityManager))
                    {
                        var thisUserName = userName;
                        if (string.IsNullOrEmpty(thisUserName)) thisUserName = authorisation.GuestAccountName;
                        if (thisUserName == null)
                        {
                            isAllowed = defaultIsAllowed;
                        }
                        else
                        {
                            isAllowed = authorisation.CheckPermission(securityManager,
                                thisUserName,
                                SecurityPermission.ViewProject,
                                SecurityRight.Allow);
                        }
                    }
                }
                if (isAllowed)
                {
                    allowedProjects.Add(project);
                }
            }
            return allowedProjects.ToArray();
        }
        #endregion

        #region OnIntegrationStarted()
        /// <summary>
        /// Pass this event onto any listeners.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnIntegrationStarted(object sender, IntegrationStartedEventArgs args)
        {
            args.Result = FireIntegrationStarted(args.Request, args.ProjectName);
        }
        #endregion

        #region OnIntegrationCompleted()
        /// <summary>
        /// Pass this event onto any listeners.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnIntegrationCompleted(object sender, IntegrationCompletedEventArgs args)
        {
            FireIntegrationCompleted(args.Request, args.ProjectName, args.Status);
        }
        #endregion

        #region LookupProject()
        private IProject LookupProject(string projectName)
        {
            return GetIntegrator(projectName).Project;
        }
        #endregion

        #region GetIntegrator()
        private IProjectIntegrator GetIntegrator(string projectName)
        {
            return integrationQueueManager.GetIntegrator(projectName);
        }
        #endregion

        #region InitialiseExtensions()
        /// <summary>
        /// Initialise all the extensions for the server.
        /// </summary>
        /// <param name="extensionList">The extensions to load.</param>
        private void InitialiseExtensions(List<ExtensionConfiguration> extensionList)
        {
            foreach (ExtensionConfiguration extensionConfig in extensionList)
            {
                // See if we can find the type
                Type extensionType = Type.GetType(extensionConfig.Type);
                if (extensionType == null) throw new NullReferenceException(string.Format("Unable to find extension '{0}'", extensionConfig.Type));

                // Load and initialise the extension
                ICruiseServerExtension extension = Activator.CreateInstance(extensionType) as ICruiseServerExtension;
                if (extension == null) throw new NullReferenceException(string.Format("Unable to create an instance of '{0}'", extensionType.FullName));
                extension.Initialise(this, extensionConfig);

                // Add to the list of extensions
                extensions.Add(extension);
            }
        }
        #endregion

        #region InitializeServerThread()()
        private void InitializeServerThread()
        {
            try
            {
                Thread.CurrentThread.Name = "CCNet Server";
            }
            catch (InvalidOperationException)
            {
                // Thread name has already been set.  This only happens during unit tests.
            }
        }
        #endregion

        #region DiagnosePermission()
        /// <summary>
        /// Checks to see if a session has the required right to perform a permission.
        /// </summary>
        /// <param name="userName">The user to check.</param>
        /// <param name="projectName">The project the permission is for.</param>
        /// <param name="permission">The permission being checked.</param>
        /// <returns>True if the permission is allowed, false otherwise.</returns>
        private bool DiagnosePermission(string userName, string projectName, SecurityPermission permission)
        {
            bool isAllowed = false;
            if (userName != null)
            {
                if (string.IsNullOrEmpty(projectName))
                {
                    isAllowed = securityManager.CheckServerPermission(userName, permission);
                }
                else
                {
                    IProjectIntegrator projectIntegrator = GetIntegrator(projectName);
                    if ((projectIntegrator != null) &&
                        (projectIntegrator.Project != null) &&
                        (projectIntegrator.Project.Security != null))
                    {
                        IProjectAuthorisation authorisation = projectIntegrator.Project.Security;
                        isAllowed = authorisation.CheckPermission(securityManager,
                            userName,
                            permission,
                            securityManager.GetDefaultRight(permission));
                    }
                }
            }
            return isAllowed;
        }
        #endregion

        #region CheckSecurity()
        /// <summary>
        /// Checks to see if a session has the required right to perform a permission.
        /// </summary>
        /// <param name="sessionToken">The session to check.</param>
        /// <param name="projectName">The project the permission is for.</param>
        /// <param name="permission">The permission being checked.</param>
        /// <param name="eventType">The event type for logging.</param>
        /// <returns>The display name of the user if the permission is allowed.</returns>
        private string CheckSecurity(string sessionToken,
            string projectName,
            SecurityPermission permission,
            SecurityEvent? eventType)
        {
            // Retrieve the project authorisation
            IProjectAuthorisation authorisation = null;
            bool requiresSession = securityManager.RequiresSession;
            string userName = securityManager.GetUserName(sessionToken);
            string displayName = securityManager.GetDisplayName(sessionToken) ?? userName;
            if (!string.IsNullOrEmpty(projectName))
            {
                IProjectIntegrator projectIntegrator = GetIntegrator(projectName);
                if ((projectIntegrator != null) &&
                    (projectIntegrator.Project != null) &&
                    (projectIntegrator.Project.Security != null))
                {
                    // The project has been found and it has security
                    authorisation = projectIntegrator.Project.Security;
                    requiresSession = authorisation.RequiresSession(securityManager);
                }
                else if ((projectIntegrator != null) &&
                    (projectIntegrator.Project != null) &&
                    (projectIntegrator.Project.Security == null))
                {
                    // The project is found, but security is missing - application error
                    string errorMessage = string.Format("Security not found for project {0}", projectName);
                    Log.Error(errorMessage);
                    if (eventType.HasValue)
                    {
                        securityManager.LogEvent(projectName,
                            userName,
                            eventType.Value,
                            SecurityRight.Deny,
                            errorMessage);
                    }
                    throw new SecurityException(errorMessage);
                }
                else
                {
                    // Couldn't find the requested project
                    string errorMessage = string.Format("project not found {0}", projectName);
                    Log.Error(errorMessage);
                    if (eventType.HasValue)
                    {
                        securityManager.LogEvent(projectName,
                            userName,
                            eventType.Value,
                            SecurityRight.Deny,
                            errorMessage);
                    }
                    throw new NoSuchProjectException(projectName);
                }
            }

            if (!requiresSession || (userName != null))
            {
                if (string.IsNullOrEmpty(projectName))
                {
                    // Checking server-level security
                    if (!securityManager.CheckServerPermission(userName, permission))
                    {
                        string info = string.Format("{2} [{0}] has been denied {1} permission at the server",
                            userName, permission, displayName);
                        Log.Warning(info);
                        if (eventType.HasValue)
                        {
                            securityManager.LogEvent(projectName,
                                userName,
                                eventType.Value,
                                SecurityRight.Deny,
                                info);
                        }
                        throw new PermissionDeniedException(permission.ToString());
                    }
                    else
                    {
                        string info = string.Format("{2} [{0}] has been granted {1} permission at the server",
                            userName, permission, displayName);
                        Log.Debug(info);
                        if (eventType.HasValue)
                        {
                            securityManager.LogEvent(projectName,
                                userName,
                                eventType.Value,
                                SecurityRight.Allow,
                                info);
                        }
                        return displayName;
                    }
                }
                else
                {
                    // Checking project-level security
                    if (!authorisation.CheckPermission(securityManager,
                        userName,
                        permission,
                        securityManager.GetDefaultRight(permission)))
                    {
                        string info = string.Format("{3} [{0}] has been denied {1} permission on '{2}'",
                            userName, permission, projectName, displayName);
                        Log.Warning(info);
                        if (eventType.HasValue)
                        {
                            securityManager.LogEvent(projectName,
                                userName,
                                eventType.Value,
                                SecurityRight.Deny,
                                info);
                        }
                        throw new PermissionDeniedException(permission.ToString());
                    }
                    else
                    {
                        Log.Debug(string.Format("{3} [{0}] has been granted {1} permission on '{2}'",
                            userName,
                            permission,
                            projectName,
                            displayName));
                        if (eventType.HasValue)
                        {
                            securityManager.LogEvent(projectName,
                                userName,
                                eventType.Value,
                                SecurityRight.Allow,
                                null);
                        }
                        return displayName;
                    }
                }
            }
            else
            {
                SecurityRight defaultRight = securityManager.GetDefaultRight(permission);
                switch (defaultRight)
                {
                    case SecurityRight.Allow:
                        Log.Debug(string.Format("{3} [{0}] has been granted {1} permission on '{2}'",
                            userName,
                            permission,
                            projectName,
                            displayName));
                        return string.Empty;
                    default:
                        // Tell the user that the session is unknown
                        var info = string.Format("Session with token '{0}' is not valid", sessionToken);
                        Log.Warning(info);
                        if (eventType.HasValue)
                        {
                            securityManager.LogEvent(projectName,
                                null,
                                eventType.Value,
                                SecurityRight.Deny,
                                info);
                        }
                        throw new SessionInvalidException();
                }
            }
        }
        #endregion

        #region GetLinkedSiteId()
        /// <summary>
        /// Retrieve the identifer for this project on a linked site.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual DataResponse GetLinkedSiteId(ProjectItemRequest request)
        {
            string data = null;
            DataResponse response = new DataResponse(RunProjectRequest(request,
                SecurityPermission.ViewProject,
                null,
                (arg, resp) => 
                {
                    // Retrieve the project configuration
                    var project = GetIntegrator(arg.ProjectName).Project;

                    // Find the site that has the matching name
                    if (project.LinkedSites != null)
                    {
                        foreach (var siteLink in project.LinkedSites)
                        {
                            if (string.Equals(request.ItemName, siteLink.Name, StringComparison.CurrentCultureIgnoreCase))
                            {
                                data = siteLink.Value;
                                break;
                            }
                        }
                    }
                }));
            response.Data = data;
            return response;
        }
        #endregion

        #region RetrieveLogData()
        /// <summary>
        /// Retrieves the log data.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="buildName">The name of the build.</param>
        /// <returns>The data for the log.</returns>
        /// <exception cref="ApplicationException">Thrown if the data for the log could not be retrieved.</exception>
        private string RetrieveLogData(string projectName, string buildName)
        {
            var cache = HttpRuntime.Cache;

            // Generate the log and report keys
            var logKey = projectName +
                buildName;

            // Check if the log has already been cached
            var loadData = false;
            SynchronisedData logData;
            lock (logCacheLock)
            {
                logData = cache[logKey] as SynchronisedData;
                if (logData == null)
                {
                    Log.Debug("Adding new cache entry, current cache size is " + cache.Count);
                    Log.Debug("Current memory in use by GC is " + GC.GetTotalMemory(false));

                    // Add the new log data and load it
                    logData = new SynchronisedData();
                    cache.Add(
                        logKey,
                        logData,
                        null,
                        Cache.NoAbsoluteExpiration,
                        this.cacheTime,
                        CacheItemPriority.BelowNormal,
                        (key, value, reason) =>
                        {
                            Log.Debug("Log for " + key + " has been removed from the cache - " + reason.ToString());
                        });
                    loadData = true;
                }
            }

            // Load the data if required
            if (loadData)
            {
                Log.Debug("Loading log for " + logKey + " into cache");
                logData.LoadData(() =>
                {
                    var buildLog = this.GetIntegrator(projectName)
                        .IntegrationRepository
                        .GetBuildLog(buildName);
                    return buildLog;
                });
                Log.Debug("Current memory in use by GC is " + GC.GetTotalMemory(false));
            }
            else
            {
                // Wait for the data to load
                Log.Debug("Retrieving log for " + logKey + " from cache");
                logData.WaitForLoad(10000);
            }

            // Raise an error if there is no log data
            if (logData.Data == null)
            {
                cache.Remove(logKey);
                throw new ApplicationException("Unable to retrieve log data");
            }

            return logData.Data as string;
        }
        #endregion
        #endregion

        #region Private delegates
        #region ProjectRequestAction
        /// <summary>
        /// Processes a project request.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <param name="response">The response to use.</param>
        private delegate void ProjectRequestAction(ProjectRequest request, Response response);
        #endregion
        #endregion
    }
}
