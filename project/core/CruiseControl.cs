using System;
using System.Collections;
using System.Threading;
using System.Runtime.Remoting;
using System.Diagnostics;
using tw.ccnet.core.configuration;
using tw.ccnet.core.schedule;
using tw.ccnet.core.util;
using tw.ccnet.remote;

namespace tw.ccnet.core
{
	/// <summary>
	/// 
	/// </summary>
	public class CruiseControl : ICruiseControl, IDisposable
	{
		IConfigurationLoader _loader;
		IDictionary _projects;
		IList _projectIntegrators = new ArrayList();
		bool _stopped = false;

		#region Constructors

		internal CruiseControl()
		{
			_projects = new Hashtable();
		}

		public CruiseControl(IConfigurationLoader loader)
		{
			_loader = loader;
			_loader.AddConfigurationChangedHandler(new ConfigurationChangedHandler(OnConfigurationChanged));
			_projects = _loader.LoadProjects();

			foreach (IProject project in _projects.Values)
			{
				AddProjectIntegrator(project);
			}
		}


		#endregion

		#region Get / Add projects

		public IProject GetProject(string projectName)
		{
			return (IProject)_projects[projectName];
		}

		public void AddProject(IProject project)
		{
			_projects.Add(project.Name, project);
			AddProjectIntegrator(project);
		}

		void AddProjectIntegrator(IProject project)
		{
			if (project.Schedule!=null)
				_projectIntegrators.Add(new ProjectIntegrator(project.Schedule, project));
		}
		

		#endregion

		#region Properties

		public ICollection Projects
		{
			get { return _projects.Values; }
		}

		public IList ProjectIntegrators
		{
			get { return _projectIntegrators; }
		}

		public virtual bool Stopped
		{
			get { return _stopped; }
//			set { _stopped = value; }
		}


		#endregion

		#region Start & Stop

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


		#endregion

		#region Configuration loading / reloading

		protected void OnConfigurationChanged()
		{
			ReloadConfiguration();
		}

		private void ReloadConfiguration()
		{
			_projects = _loader.LoadProjects();

			IDictionary schedulerMap = CreateSchedulerMap();
			foreach (IProjectIntegrator scheduler in schedulerMap.Values)
			{
				IProject project = (IProject)_projects[scheduler.Project.Name];
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

			foreach (IProject project in _projects.Values)
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

		#endregion

		#region Disposing

		/// <summary>
		/// Stops all integration threads when garbage collected.
		/// </summary>
		public void Dispose()
		{
			Stop();
		}

		#endregion

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
			foreach (IProject project in Projects) 
			{
				return project.GetLatestBuildStatus();
			}

			return IntegrationStatus.Unknown;
		}

		public ProjectActivity CurrentProjectActivity()
		{
			foreach (IProject project in Projects) 
			{
				return project.CurrentActivity;
			}

			return ProjectActivity.Unknown;
		}

	}
}
