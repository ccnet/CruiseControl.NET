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
			LoadConfiguration();
		}

		internal void LoadConfiguration()
		{
			_projects = _loader.LoadProjects();
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

		public void Run()
		{
			IntegrationExceptionEventHandler handler = new IntegrationExceptionEventHandler(HandleException);
			foreach (IProject project in Projects) 
			{
				project.AddIntegrationExceptionEventHandler(handler);
			}
			while (! Stopped)
			{
				RunIntegration();
			}
			Trace.WriteLine("CruiseControl stopped");
		}
		
		public void HandleException(object sender, CruiseControlException ex) 
		{
			// TODO: this method should be tested!
			LogUtil.Log((IProject)sender, String.Format("exception: {0}\n{1}", ex.Message, ex.InnerException));
		}

		internal void RunIntegration()
		{
			foreach (IProject project in Projects)
			{
				project.Run();
				if (!Stopped)
					project.Sleep();
			}
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
			// stop existing schedulers and remove them
			Stop();
			_schedulers = new ArrayList();

			// load new projects
			LoadConfiguration();

			// Start();
		}

		private void RemoveSchedule(IProject project)
		{
			Scheduler[] schedulers = new Scheduler[_schedulers.Count];
			_schedulers.CopyTo(schedulers, 0);
			foreach (IScheduler scheduler in schedulers)
			{
				if (scheduler.Project == project)
				{
					_schedulers.Remove(scheduler);
				}
			}
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
		}		
	}
}
