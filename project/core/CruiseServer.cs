using System;
using System.Collections;
using System.IO;
using System.Threading;
using ThoughtWorks.Core.Log;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public class CruiseServer : ICruiseServer
	{
		private readonly IProjectSerializer projectSerializer;
		private readonly IProjectIntegratorListFactory projectIntegratorListFactory;
		private IConfigurationService configurationService;
		private ICruiseManager _manager;
		private ManualResetEvent _monitor = new ManualResetEvent(true);

		private IProjectIntegratorList projectIntegrators;

		public CruiseServer(IConfigurationService configurationService, IProjectIntegratorListFactory projectIntegratorListFactory, IProjectSerializer projectSerializer)
		{
			this.configurationService = configurationService;
			this.configurationService.AddConfigurationUpdateHandler(new ConfigurationUpdateHandler(Restart));
			this.projectIntegratorListFactory = projectIntegratorListFactory;

			// ToDo - get rid of manager, maybe
			_manager = new CruiseManager(this);

			// By default, no integrators are running
			projectIntegrators = new ProjectIntegratorList();
			this.projectSerializer = projectSerializer;
		}

		public void Start()
		{
			Log.Info("Starting CruiseControl.NET Server");
			_monitor.Reset();
			CreateAndStartIntegrators();
		}

		public void Stop()
		{
			Log.Info("Stopping CruiseControl.NET Server");
			StopIntegrators();
			_monitor.Set();
		}

		public void Abort()
		{
			Log.Info("Aborting CruiseControl.NET Server");
			AbortIntegrators();
			_monitor.Set();
		}

		public void Restart()
		{
			Log.Info("Configuration changed: Restarting CruiseControl.NET Server ");

			StopIntegrators();
			CreateAndStartIntegrators();
		}

		public void WaitForExit()
		{
			_monitor.WaitOne();
		}

		private void CreateAndStartIntegrators()
		{
			IConfiguration configuration;
			try
			{
				configuration = configurationService.Load();
			}
			catch (ConfigurationException ce)
			{
				Log.Error(ce);
				return;
			}

			if (configuration == null)
			{
				Log.Error("Cruise Server Was not able to get a configuration");
				return;
			}

			projectIntegrators = projectIntegratorListFactory.CreateProjectIntegrators(configuration.Projects);
			foreach (IProjectIntegrator integrator in projectIntegrators)
			{
				integrator.Start();
			}

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
			get { return _manager; }
		}

		public ProjectStatus[] GetProjectStatus()
		{
			ArrayList projects = new ArrayList();
			foreach (IProjectIntegrator integrator in projectIntegrators)
			{
				Project project = (Project) integrator.Project;
				projects.Add(new ProjectStatus(integrator.State, 
					project.LatestBuildStatus, 
					project.CurrentActivity, 
					project.Name, 
					project.WebURL, 
					project.LastIntegrationResult.StartTime, 
					project.LastIntegrationResult.Label));
			}

			return (ProjectStatus []) projects.ToArray(typeof(ProjectStatus));
		}

		public void ForceBuild(string projectName)
		{
			GetIntegrator(projectName).ForceBuild();
		}

		public void WaitForExit(string projectName)
		{
			GetIntegrator(projectName).WaitForExit();
		}

		public string GetLatestBuildName(string projectName)
		{
			string[] buildNames = GetBuildNames(projectName);
			if (buildNames.Length > 0)
			{
				return buildNames[0];
			}
			else
			{
				return "";
			}
		}

		public string[] GetBuildNames(string projectName)
		{
			// TODO - this is a hack - I'll tidy it up later - promise! :) MR
			foreach (IProjectIntegrator projectIntegrator in projectIntegrators) 
			{
				if (projectIntegrator.Name == projectName)
				{
					foreach (IIntegrationCompletedEventHandler publisher in ((Project) projectIntegrator.Project).Publishers)
					{
						if (publisher is XmlLogPublisher)
						{
							string[] logFileNames = LogFileUtil.GetLogFileNames(((XmlLogPublisher) publisher).LogDir);
							Array.Reverse(logFileNames);
							return logFileNames;
						}
					}
					throw new CruiseControlException("Unable to find Log Publisher for project so can't find log file");
				}
			}

			throw new NoSuchProjectException(projectName);
		}

		public string[] GetMostRecentBuildNames(string projectName, int buildCount)
		{
			// TODO - this is a hack - I'll tidy it up later - promise! :) MR
			string[] buildNames = GetBuildNames(projectName);
			ArrayList buildNamesToReturn = new ArrayList();
			for (int i = 0; i < ((buildCount < buildNames.Length) ? buildCount : buildNames.Length); i++)
			{
				buildNamesToReturn.Add(buildNames[i]);
			}
			return (string[]) buildNamesToReturn.ToArray(typeof (string));
		}

		public string GetLog(string projectName, string buildName)
		{
			// TODO - this is a hack - I'll tidy it up later - promise! :) MR
			foreach (IProjectIntegrator projectIntegrator in projectIntegrators) 
			{
				if (projectIntegrator.Name == projectName)
				{
					foreach (IIntegrationCompletedEventHandler publisher in ((Project) projectIntegrator.Project).Publishers)
					{
						if (publisher is XmlLogPublisher)
						{
							using (StreamReader sr = new StreamReader(Path.Combine(((XmlLogPublisher) publisher).LogDir, buildName)))
							{
								return sr.ReadToEnd();
							}
						}
					}
					throw new CruiseControlException("Unable to find Log Publisher for project so can't find log file");
				}
			}

			throw new NoSuchProjectException(projectName);
		}

		// ToDo - test
		public string GetServerLog ()
		{
			return new ServerLogFileReader().Read();
		}

		// ToDo - test
		public void AddProject(string serializedProject)
		{
			Log.Info("Adding project - " + serializedProject);
			try
			{
				IConfiguration configuration = configurationService.Load();
				Project project = projectSerializer.Deserialize(serializedProject);
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
		public void DeleteProject(string projectName)
		{
			Log.Info("Deleting project - " + projectName);
			try
			{
				IConfiguration configuration = configurationService.Load();
				configuration.Projects[projectName].Purge();
				configuration.DeleteProject(projectName);
				configurationService.Save(configuration);
			}
			catch (Exception e)
			{
				Log.Warning(e);
				throw new CruiseControlException("Failed to add project. Exception was - " + e.Message);
			}
		}

		private IProjectIntegrator GetIntegrator(string projectName)
		{
			IProjectIntegrator integrator = projectIntegrators[projectName];
			if (integrator == null)
			{
				throw new CruiseControlException("Specified project does not exist: " + projectName);
			}
			return integrator;
		}

		void IDisposable.Dispose()
		{
			Abort();
			WaitForExit();
		}
	}
}
