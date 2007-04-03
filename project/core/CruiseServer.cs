using System;
using System.Collections;
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
		private readonly IProjectIntegratorListFactory projectIntegratorListFactory;
		private readonly IConfigurationService configurationService;
		private readonly ICruiseManager manager;
		private readonly ManualResetEvent monitor = new ManualResetEvent(true);

		private IProjectIntegratorList projectIntegrators;
		private bool disposed;

		public CruiseServer(IConfigurationService configurationService, IProjectIntegratorListFactory projectIntegratorListFactory, IProjectSerializer projectSerializer)
		{
			this.configurationService = configurationService;
			this.configurationService.AddConfigurationUpdateHandler(new ConfigurationUpdateHandler(Restart));
			this.projectIntegratorListFactory = projectIntegratorListFactory;
			this.projectSerializer = projectSerializer;

			// ToDo - get rid of manager, maybe
			manager = new CruiseManager(this);
			InitializeServerThread();

			// By default, no integrators are running
			CreateIntegrators();
		}

		public void Start()
		{
			Log.Info("Starting CruiseControl.NET Server");
			monitor.Reset();
			StartIntegrators();
		}

		/// <summary>
		/// Start integrator for specified project. 
		/// </summary>
		public void Start(string project)
		{
			IProjectIntegrator integrator = GetIntegrator(project);
			integrator.Start();
		}

		/// <summary>
		/// Stop all integrators, waiting until each integrator has completely stopped, before releasing any threads blocked by WaitForExit. 
		/// </summary>
		public void Stop()
		{
			Log.Info("Stopping CruiseControl.NET Server");
			StopIntegrators();
			monitor.Set();
		}

		/// <summary>
		/// Stop integrator for specified project. 
		/// </summary>
		public void Stop(string project)
		{
			IProjectIntegrator integrator = GetIntegrator(project);
			integrator.Stop();
		}

		/// <summary>
		/// Abort all integrators, waiting until each integrator has completely stopped, before releasing any threads blocked by WaitForExit. 
		/// </summary>
		public void Abort()
		{
			Log.Info("Aborting CruiseControl.NET Server");
			AbortIntegrators();
			monitor.Set();
		}

		/// <summary>
		/// Restart server by stopping all integrators, creating a new set of integrators from Configuration and then starting them.
		/// </summary>
		public void Restart()
		{
			Log.Info("Configuration changed: Restarting CruiseControl.NET Server ");

			StopIntegrators();
			CreateIntegrators();
			StartIntegrators();
		}

		/// <summary>
		/// Block thread until all integrators to have been stopped or aborted.
		/// </summary>
		public void WaitForExit()
		{
			monitor.WaitOne();
		}

		private void StartIntegrators()
		{
			foreach (IProjectIntegrator integrator in projectIntegrators)
			{
				integrator.Start();
			}
		}

		private void CreateIntegrators()
		{
			IConfiguration configuration = configurationService.Load();
			projectIntegrators = projectIntegratorListFactory.CreateProjectIntegrators(configuration.Projects);

			if (projectIntegrators.Count == 0)
			{
				Log.Info("No projects found");
			}
		}

		private void StopIntegrators()
		{
			foreach (IProjectIntegrator integrator in projectIntegrators)
			{
				integrator.Stop();
			}
			WaitForIntegratorsToExit();
		}

		private void AbortIntegrators()
		{
			foreach (IProjectIntegrator integrator in projectIntegrators)
			{
				integrator.Abort();
			}
			WaitForIntegratorsToExit();
		}

		private void WaitForIntegratorsToExit()
		{
			foreach (IProjectIntegrator integrator in projectIntegrators)
			{
				integrator.WaitForExit();
			}
		}

		public ICruiseManager CruiseManager
		{
			get { return manager; }
		}

		public ProjectStatus[] GetProjectStatus()
		{
			ArrayList projectStatusList = new ArrayList();
			foreach (IProjectIntegrator integrator in projectIntegrators)
			{
				IProject project = integrator.Project;
				projectStatusList.Add(project.CreateProjectStatus(integrator));
			}
			return (ProjectStatus[]) projectStatusList.ToArray(typeof (ProjectStatus));
		}

		public void ForceBuild(string projectName)
		{
			GetIntegrator(projectName).ForceBuild();
		}

		public void WaitForExit(string projectName)
		{
			GetIntegrator(projectName).WaitForExit();
		}

		// TODO - move this out of CruiseServer
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
		public void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
			Log.Info("Deleting project - " + projectName);
			try
			{
				IConfiguration configuration = configurationService.Load();
				configuration.Projects[projectName].Purge(purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
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
			return GetIntegrator(projectName).Project.ExternalLinks;
		}

		public void SendMessage(string projectName, Message message)
		{
			Log.Info("New message received: " + message);
			GetIntegrator(projectName).Project.AddMessage(message);
		}

		public string GetArtifactDirectory(string projectName)
		{
			return GetIntegrator(projectName).Project.ArtifactDirectory;
		}

		public string GetStatisticsDocument(string projectName)
		{
			return GetIntegrator(projectName).Project.Statistics.OuterXml;
		}

		private IProjectIntegrator GetIntegrator(string projectName)
		{
			IProjectIntegrator integrator = projectIntegrators[projectName];
			if (integrator == null) throw new NoSuchProjectException(projectName);
			return integrator;
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

		public void Request(string project, IntegrationRequest request)
		{
			GetIntegrator(project).Request(request);
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