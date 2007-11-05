using System;
using System.Reflection;
using System.Threading;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Logging;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class CruiseServer : ICruiseServer
	{
		private readonly IProjectSerializer projectSerializer;
		private readonly IConfigurationService configurationService;
		private readonly ICruiseManager manager;
		private readonly ManualResetEvent monitor = new ManualResetEvent(true);

		private bool disposed;
		private IntegrationQueueManager integrationQueueManager;

		public CruiseServer(IConfigurationService configurationService,
		                    IProjectIntegratorListFactory projectIntegratorListFactory, IProjectSerializer projectSerializer)
		{
			this.configurationService = configurationService;
			this.configurationService.AddConfigurationUpdateHandler(new ConfigurationUpdateHandler(Restart));
			this.projectSerializer = projectSerializer;

			// ToDo - get rid of manager, maybe
			manager = new CruiseManager(this);
			InitializeServerThread();

			IConfiguration configuration = configurationService.Load();
			// TODO - does this need to go through a factory? GD
			integrationQueueManager = new IntegrationQueueManager(projectIntegratorListFactory, configuration);
		}

		public void Start()
		{
			Log.Info("Starting CruiseControl.NET Server");
			monitor.Reset();
			integrationQueueManager.StartAllProjects();
		}

		/// <summary>
		/// Start integrator for specified project. 
		/// </summary>
		public void Start(string project)
		{
			integrationQueueManager.Start(project);
		}

		/// <summary>
		/// Stop all integrators, waiting until each integrator has completely stopped, before releasing any threads blocked by WaitForExit. 
		/// </summary>
		public void Stop()
		{
			Log.Info("Stopping CruiseControl.NET Server");
			integrationQueueManager.StopAllProjects();
			monitor.Set();
		}

		/// <summary>
		/// Stop integrator for specified project. 
		/// </summary>
		public void Stop(string project)
		{
			integrationQueueManager.Stop(project);
		}

		/// <summary>
		/// Abort all integrators, waiting until each integrator has completely stopped, before releasing any threads blocked by WaitForExit. 
		/// </summary>
		public void Abort()
		{
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

		public void ForceBuild(string projectName)
		{
			integrationQueueManager.ForceBuild(projectName);
		}

		public void WaitForExit(string projectName)
		{
			integrationQueueManager.WaitForExit(projectName);
		}

		public void Request(string project, IntegrationRequest request)
		{
			integrationQueueManager.Request(project, request);
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
			Log.Info("New message received: " + message);
			LookupProject(projectName).AddMessage(message);
		}

		public string GetArtifactDirectory(string projectName)
		{
			return LookupProject(projectName).ArtifactDirectory;
		}

		public string GetStatisticsDocument(string projectName)
		{
			return GetIntegrator(projectName).Project.Statistics.OuterXml;
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
	}
}