using System;
using System.Collections;

using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// Main CruiseControl engine class.  Responsible for loading and launching CruiseControl projects.
	/// </summary>
	public class CruiseServer : ICruiseControl, ICruiseServer, IDisposable
	{
		private IConfigurationLoader _loader;
		private IConfiguration _configuration;
		// define as ArrayList, not IList, so we have .ToArray(...)
		private ArrayList _projectIntegrators = new ArrayList();
		private bool _stopped = false;

		public CruiseServer(IConfigurationLoader loader)
		{
			_loader = loader;
			_loader.AddConfigurationChangedHandler(new ConfigurationChangedHandler(OnConfigurationChanged));
			_configuration = _loader.Load();

			foreach (IProject project in _configuration)
			{
				AddProjectIntegrator(project);
			}
		}

		public IProject GetProject(string projectName)
		{
			return _configuration.GetProject(projectName);
		}

		public IConfiguration Configuration
		{
			get { return _configuration; }
		}

		public IntegrationResult RunIntegration(string projectName)
		{
			IProject project = GetProject(projectName);
			
			if (project == null) 
				throw new CruiseControlException(String.Format("Cannot execute the specified project: {0}.  Project does not exist.", projectName));
			
			return project.RunIntegration(BuildCondition.ForceBuild);
		}

		public void ForceBuild(string projectName)
		{
			IProject project = GetProject(projectName);

			// tell the project's schedule that we want a build forced
			project.Schedule.ForceBuild();
		}

		private void AddProjectIntegrator(IProject project)
		{
			if (project.Schedule!=null)
				_projectIntegrators.Add(new ProjectIntegrator(project.Schedule, project));
		}

		public IProjectIntegrator[] ProjectIntegrators
		{
			get { return (IProjectIntegrator[])_projectIntegrators.ToArray(typeof(IProjectIntegrator)); }
		}

		public CruiseControlStatus Status
		{
			get { return (_stopped) ? CruiseControlStatus.Stopped : CruiseControlStatus.Running; }
		}

		/// <summary>
		/// Starts each project's integration cycle.
		/// </summary>
		public void Start()
		{
			_stopped = false;

			foreach (IProjectIntegrator projectIntegrator in _projectIntegrators)
			{
				projectIntegrator.Start();
			}
		}

		/// <summary>
		/// Stops each project's integration cycle.
		/// </summary>
		public void Stop()
		{
			_stopped = true;

			foreach (IProjectIntegrator projectIntegrator in _projectIntegrators)
			{
				projectIntegrator.Stop();
			}
		}

		public void Abort()
		{
			foreach (IProjectIntegrator projectIntegrator in _projectIntegrators)
			{
				projectIntegrator.Abort();
			}		
		}

		protected void OnConfigurationChanged()
		{
			ReloadConfiguration();
		}

		private void ReloadConfiguration()
		{
			_configuration = _loader.Load();

			IDictionary schedulerMap = CreateSchedulerMap();
			foreach (IProjectIntegrator scheduler in schedulerMap.Values)
			{
				IProject project = _configuration.GetProject(scheduler.Project.Name);
				if (project == null)
				{
					// project has been removed, so stop scheduler and remove
					scheduler.Stop();
					_projectIntegrators.Remove(scheduler);
				}
				else
				{
					scheduler.Project = project;
					scheduler.Schedule = project.Schedule;
				}
			}

			foreach (IProject project in _configuration)
			{
				IProjectIntegrator scheduler = (IProjectIntegrator)schedulerMap[project.Name];
				if (scheduler == null)
				{
					// create new scheduler
					IProjectIntegrator newScheduler = new ProjectIntegrator(project.Schedule, project);
					_projectIntegrators.Add(newScheduler);
					newScheduler.Start();
				}
			}
		}

		private IDictionary CreateSchedulerMap()
		{
			Hashtable schedulerList = new Hashtable();
			foreach (ProjectIntegrator scheduler in _projectIntegrators)
			{
				schedulerList.Add(scheduler.Project.Name, scheduler);
			}
			return schedulerList;
		}

		/// <summary>
		/// Stops all integration threads when garbage collected.
		/// </summary>
		public void Dispose()
		{
			Abort();
		}

		public void WaitForExit()
		{
			foreach (IProjectIntegrator scheduler in _projectIntegrators)
			{
				scheduler.WaitForExit();
			}
		}

		/// <summary>
		/// Gets the most recent build status across all projects for this CruiseControl.NET
		/// instance.
		/// </summary>
		/// <returns></returns>
		public IntegrationStatus GetLatestBuildStatus()
		{
			// TODO determine the most recent where multiple projects exist, rather than simply returning the first
			foreach (IProject project in _configuration) 
			{
				return project.GetLatestBuildStatus();
			}

			return IntegrationStatus.Unknown;
		}

		public ProjectActivity CurrentProjectActivity()
		{
			// TODO determine the appropriate project where multiples exist, rather than simply returning the first
			foreach (IProject project in _configuration) 
			{
				return project.CurrentActivity;
			}

			return ProjectActivity.Unknown;
		}
	}
}
