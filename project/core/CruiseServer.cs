using System;
using System.Reflection;
using System.Threading;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Logging;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.Configuration;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Events;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core
{
	public class CruiseServer
        : CruiseServerEventsBase, ICruiseServer
	{
		private readonly IProjectSerializer projectSerializer;
		private readonly IConfigurationService configurationService;
		private readonly ICruiseManager manager;
		// TODO: Why the monitor? What reentrancy do we have? davcamer, dec 24 2008
		private readonly ManualResetEvent monitor = new ManualResetEvent(true);
        private readonly List<ICruiseServerExtension> extensions = new List<ICruiseServerExtension>();

		private bool disposed;
		private IntegrationQueueManager integrationQueueManager;

		public CruiseServer(IConfigurationService configurationService,
		                    IProjectIntegratorListFactory projectIntegratorListFactory, 
                            IProjectSerializer projectSerializer,
                            IProjectStateManager stateManager,
                            List<ExtensionConfiguration> extensionList)
		{
			this.configurationService = configurationService;
			this.projectSerializer = projectSerializer;

			// ToDo - get rid of manager, maybe
			manager = new CruiseManager(this);
			InitializeServerThread();

			IConfiguration configuration = configurationService.Load();
			// TODO - does this need to go through a factory? GD
			integrationQueueManager = new IntegrationQueueManager(projectIntegratorListFactory, configuration, stateManager);
            integrationQueueManager.AssociateIntegrationEvents(OnIntegrationStarted, OnIntegrationCompleted);

            // Load the extensions
            if (extensionList != null)
            {
                InitialiseExtensions(extensionList);
            }
            this.configurationService.AddConfigurationUpdateHandler(new ConfigurationUpdateHandler(Restart));
        }

        #region Integration pass-through events
        /// <summary>
        /// Pass this event onto any listeners.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnIntegrationStarted(object sender, IntegrationStartedEventArgs args)
        {
            FireIntegrationStarted(args.Request, args.ProjectName);
        }

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

        public void Start()
		{
			Log.Info("Starting CruiseControl.NET Server");
			monitor.Reset();
			integrationQueueManager.StartAllProjects();

            // Start the extensions
            Log.Info("Starting extensions");
            foreach (ICruiseServerExtension extension in extensions)
            {
                extension.Start();
            }
        }

		/// <summary>
		/// Start integrator for specified project. 
		/// </summary>
		public void Start(string project)
		{
            if (!FireProjectStarting(project))
            {
                integrationQueueManager.Start(project);
                FireProjectStarted(project);
            }
		}

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
		/// Stop integrator for specified project. 
		/// </summary>
		public void Stop(string project)
		{
            if (!FireProjectStopping(project))
            {
                integrationQueueManager.Stop(project);
                FireProjectStopped(project);
            }
		}

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

		/// <summary>
		/// Restart server by stopping all integrators, creating a new set of integrators from Configuration and then starting them.
		/// </summary>
        public void Restart()
        {
            Log.Info("Configuration changed: Restarting CruiseControl.NET Server ");

            IConfiguration configuration = configurationService.Load();
            integrationQueueManager.Restart(configuration);
        }

		/// <summary>
		/// Block thread until all integrators to have been stopped or aborted.
		/// </summary>
		public void WaitForExit()
		{
			monitor.WaitOne();
		}

		/// <summary>
		/// Cancel a pending project integration request from the integration queue.
		/// </summary>
		public void CancelPendingRequest(string projectName)
		{
			integrationQueueManager.CancelPendingRequest(projectName);
		}

		/// <summary>
		/// Gets the projects and integration queues snapshot from this server.
		/// </summary>
        public CruiseServerSnapshot GetCruiseServerSnapshot()
		{
			return integrationQueueManager.GetCruiseServerSnapshot();
		}

		public ICruiseManager CruiseManager
		{
			get { return manager; }
		}

		public ProjectStatus[] GetProjectStatus()
		{
			return integrationQueueManager.GetProjectStatuses();
		}

		public void ForceBuild(string projectName, string enforcerName)
		{
            if (!FireForceBuildReceived(projectName, enforcerName))
            {
                integrationQueueManager.ForceBuild(projectName, enforcerName);
                FireForceBuildProcessed(projectName, enforcerName);
            }
		}

		public void AbortBuild(string projectName, string enforcerName)
		{
            if (!FireAbortBuildReceived(projectName, enforcerName))
            {
                GetIntegrator(projectName).AbortBuild(enforcerName);
                FireAbortBuildProcessed(projectName, enforcerName);
            }
        }
		
		public void WaitForExit(string projectName)
		{
			integrationQueueManager.WaitForExit(projectName);
		}

        public void Request(string project, IntegrationRequest request)
        {
            if (!FireForceBuildReceived(project, request.Source))
            {
                integrationQueueManager.Request(project, request);
                FireForceBuildProcessed(project, request.Source);
            }
        }

		public string GetLatestBuildName(string projectName)
		{
			return GetIntegrator(projectName).IntegrationRepository.GetLatestBuildName();
		}

		public string[] GetMostRecentBuildNames(string projectName, int buildCount)
		{
			return GetIntegrator(projectName).IntegrationRepository.GetMostRecentBuildNames(buildCount);
		}

		public string[] GetBuildNames(string projectName)
		{
			return GetIntegrator(projectName).IntegrationRepository.GetBuildNames();
		}

		public string GetLog(string projectName, string buildName)
		{
			return GetIntegrator(projectName).IntegrationRepository.GetBuildLog(buildName);
		}

		public string GetServerLog()
		{
			return new ServerLogFileReader().Read();
		}

		public string GetServerLog(string projectName)
		{
			return new ServerLogFileReader().Read(projectName);
		}

		// ToDo - test

		public void AddProject(string serializedProject)
		{
			Log.Info("Adding project - " + serializedProject);
			try
			{
				IConfiguration configuration = configurationService.Load();
				IProject project = projectSerializer.Deserialize(serializedProject);
				configuration.AddProject(project);
				project.Initialize();
				configurationService.Save(configuration);
			}
			catch (ApplicationException e)
			{
				Log.Warning(e);
				throw new CruiseControlException("Failed to add project. Exception was - " + e.Message);
			}
		}

		// ToDo - test
		// ToDo - when we decide how to handle configuration changes, do more here (like stopping/waiting for project, returning asynchronously, etc.)

		public void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory,
		                          bool purgeSourceControlEnvironment)
		{
			Log.Info("Deleting project - " + projectName);
			try
			{
				IConfiguration configuration = configurationService.Load();
				configuration.Projects[projectName].Purge(purgeWorkingDirectory, purgeArtifactDirectory,
				                                          purgeSourceControlEnvironment);
				configuration.DeleteProject(projectName);
				configurationService.Save(configuration);
			}
			catch (Exception e)
			{
				Log.Warning(e);
				throw new CruiseControlException("Failed to add project. Exception was - " + e.Message);
			}
		}

		// ToDo - this done TDD

		public string GetProject(string name)
		{
			Log.Info("Getting project - " + name);
			return new NetReflectorProjectSerializer().Serialize(configurationService.Load().Projects[name]);
		}

		public string GetVersion()
		{
			Log.Info("Returning version number");
			try
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
			catch (ApplicationException e)
			{
				Log.Warning(e);
				throw new CruiseControlException("Failed to get project version . Exception was - " + e.Message);
			}
		}

		// ToDo - this done TDD
		// ToDo - really delete working dir? What if SCM hasn't changed?

		public void UpdateProject(string projectName, string serializedProject)
		{
			Log.Info("Updating project - " + projectName);
			try
			{
				IConfiguration configuration = configurationService.Load();
				configuration.Projects[projectName].Purge(true, false, true);
				configuration.DeleteProject(projectName);
				IProject project = projectSerializer.Deserialize(serializedProject);
				configuration.AddProject(project);
				project.Initialize();
				configurationService.Save(configuration);
			}
			catch (ApplicationException e)
			{
				Log.Warning(e);
				throw new CruiseControlException("Failed to add project. Exception was - " + e.Message);
			}
		}

		public ExternalLink[] GetExternalLinks(string projectName)
		{
			return LookupProject(projectName).ExternalLinks;
		}

		private IProject LookupProject(string projectName)
		{
			return GetIntegrator(projectName).Project;
		}

		public void SendMessage(string projectName, Message message)
		{
            if (!FireSendMessageReceived(projectName, message))
            {
                Log.Info("New message received: " + message);
                LookupProject(projectName).AddMessage(message);
                FireSendMessageProcessed(projectName, message);
            }
		}

		public string GetArtifactDirectory(string projectName)
		{
			return LookupProject(projectName).ArtifactDirectory;
		}

		public string GetStatisticsDocument(string projectName)
		{
			return GetIntegrator(projectName).Project.Statistics;
		}


        public string GetModificationHistoryDocument(string projectName)
        {
            return GetIntegrator(projectName).Project.ModificationHistory;
        }


        public string GetRSSFeed(string projectName)
        {
            return GetIntegrator(projectName).Project.RSSFeed;
        }


		private IProjectIntegrator GetIntegrator(string projectName)
		{
			return integrationQueueManager.GetIntegrator(projectName);
        }

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

		void IDisposable.Dispose()
		{
			lock (this)
			{
				if (disposed) return;
				disposed = true;
			}
			Abort();
		}

		private static void InitializeServerThread()
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

        /// <summary>
        /// Retrieves the amount of free disk space.
        /// </summary>
		/// <returns>The amount of free space in bytes.</returns>
		public long GetFreeDiskSpace()
        {
			//TODO: this is currently a hack
			// this method sould return a collection of drives used by ccnet
			// since each project can be hostet on a different drive.
			// We should determine the drives used by a project
			// (working, artefacs, SCM checkout, build publisher, etc)
			// of each project and return a list of free disc space for every used drive.
        	string drive = ConfigurationManager.AppSettings["DataDrive"];
        	if (string.IsNullOrEmpty(drive))
        	{
        		if (System.IO.Path.DirectorySeparatorChar == '/')
        			drive = "/";
        		else
        			drive = "C:";
        	}

        	IFileSystem fileSystem = new SystemIoFileSystem();
			return fileSystem.GetFreeDiskSpace(drive);
        }

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieve a file transfer object.
        /// </summary>
        /// <param name="project">The project to retrieve the file for.</param>
        /// <param name="fileName">The name of the file.</param>
        public virtual RemotingFileTransfer RetrieveFileTransfer(string project, string fileName)
        {
            // Validate that the path is valid
            var sourceProject = GetIntegrator(project).Project;
            var filePath = Path.Combine(sourceProject.ArtifactDirectory, fileName);
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.FullName.StartsWith(sourceProject.ArtifactDirectory, StringComparison.InvariantCultureIgnoreCase))
            {
                var message = string.Format("Files can only be retrieved from the artefact folder - unable to retrieve {0}", fileName);
                Log.Warning(message);
                throw new CruiseControlException(message);
            }
            else if (fileInfo.FullName.StartsWith(Path.Combine(sourceProject.ArtifactDirectory, "buildlogs"), StringComparison.InvariantCultureIgnoreCase))
            {
                var message = string.Format("Unable to retrieve files from the build logs folder - unable to retrieve {0}", fileName);
                Log.Warning(message);
                throw new CruiseControlException(message);
            }

            RemotingFileTransfer fileTransfer = null;
            if (fileInfo.Exists)
            {
                Log.Debug(string.Format("Retrieving file '{0}' from '{1}'", fileName, project));
                fileTransfer = new RemotingFileTransfer(File.OpenRead(filePath));
            }
            else
            {
                Log.Warning(string.Format("Unable to find file '{0}' in '{1}'", fileName, project));
            }
            return fileTransfer;
        }
        #endregion
	}
}
