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
	public class CruiseControl : IDisposable
	{
		private IConfigurationLoader _loader;
		private IDictionary _projects;
		private IList _schedulers = new ArrayList();
		private bool _stopped = false;

		internal CruiseControl() 
		{
			_projects = new Hashtable();
		}

		public CruiseControl(IConfigurationLoader loader)
		{
			_loader = loader;
			_loader.AddConfigurationChangedHandler(new ConfigurationChangedHandler(OnConfigurationChanged));
			_projects = _loader.LoadProjects();

			CreateSchedulers();
		}

		internal void CreateSchedulers()
		{
			foreach (IProject project in Projects)
			{
				if (project.Schedule != null)
				{
					_schedulers.Add(new Scheduler(project.Schedule, project));
				}
			}
		}

		public IProject GetProject(string projectName)
		{
			return (IProject)_projects[projectName];
		}

		public ICollection Projects
		{
			get { return _projects.Values; }
		}

		internal IList Schedulers
		{
			get { return _schedulers; }
		}

		public virtual bool Stopped
		{
			get { return _stopped; }
			set { _stopped = value; }
		}

		public void Start()
		{
			foreach (IScheduler scheduler in _schedulers)
			{
				scheduler.Start();
			}
		}

		public void Stop()
		{
			foreach (IScheduler scheduler in _schedulers)
			{
				scheduler.Stop();
			}
		}

		public void WaitForExit()
		{
			foreach (IScheduler scheduler in _schedulers)
			{
				scheduler.WaitForExit();
			}
		}

		protected void OnConfigurationChanged()
		{
			// reload configuration
			_projects = _loader.LoadProjects();

			IDictionary schedulerMap = CreateSchedulerMap();
			foreach (IScheduler scheduler in schedulerMap.Values)
			{
				IProject project = (IProject)_projects[scheduler.Project.Name];
				if (project == null)
				{
					// project has been removed, so stop scheduler and remove
					scheduler.Stop();
					_schedulers.Remove(scheduler);
				}
				else
				{
					scheduler.Project = project;
					scheduler.Schedule = project.Schedule;
				}
			}

			foreach (IProject project in _projects.Values)
			{
				IScheduler scheduler = (IScheduler)schedulerMap[project.Name];
				if (scheduler == null)
				{
					// create new scheduler
					IScheduler newScheduler = new Scheduler(project.Schedule, project);
					_schedulers.Add(newScheduler);
					newScheduler.Start();
				}
			}
		}

		private IDictionary CreateSchedulerMap()
		{
			Hashtable schedulerList = new Hashtable();
			foreach (Scheduler scheduler in _schedulers)
			{
				schedulerList.Add(scheduler.Project.Name, scheduler);
			}
			return schedulerList;
		}

		public void Dispose()
		{
			Stop();
		}

		public IntegrationStatus GetLastBuildStatus() 
		{
			foreach (IProject p in Projects) 
			{
				return p.GetLastBuildStatus();
			}

			return IntegrationStatus.Unknown;
		}

		public string CurrentProjectActivity() 
		{
			foreach (IProject p in Projects) 
			{
				return ((Project)p).CurrentActivity;
			}

			return "unknown";
		}

		internal void AddProject(IProject project) 
		{
			_projects.Add(project.Name, project);
			_schedulers.Add(new Scheduler(project.Schedule, project));
		}		
	}
}
