using System;
using System.Threading;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// An object responsible for the continuous integration of a single project.
	/// This integrator, when running, coordinates the top-level life cycle of
	/// a project's integration.
	/// <list type="1">
	///		<item>The <see cref="IIntegrationTrigger"/> instance is asked whether to build or not.</item>
	///		<item>If a build is required, the <see cref="IProject.RunIntegration(BuildCondition buildCondition)"/>
	///		is called.</item>
	/// </list>
	/// </summary>
	public class ProjectIntegrator : IProjectIntegrator, IDisposable
	{
		private readonly IIntegratable integratable;
		private readonly IStopProjectTrigger stopProjectTrigger;
		private IIntegrationTrigger integrationTrigger;
		private IProject _project;
		private bool _forceBuild;
		private Thread _thread;
		private ProjectIntegratorState _state = ProjectIntegratorState.Stopped;

		public ProjectIntegrator(IProject project) : this(project.IntegrationTrigger, project.StopProjectTrigger, project, project) { }

		public ProjectIntegrator(IIntegrationTrigger integrationTrigger, IStopProjectTrigger stopProjectTrigger, IIntegratable integratable, IProject project)
		{
			this.integrationTrigger = integrationTrigger;
			this.stopProjectTrigger = stopProjectTrigger;
			_project = project;
			this.integratable = integratable;
		}

		public string Name
		{
			get { return _project.Name; }
		}

		public IProject Project 
		{ 
			get { return _project; }
		}

		public ProjectIntegratorState State
		{
			get { return _state; }
		}

		public void Start()
		{
			lock (this)
			{
				if (IsRunning)
					return;

				_state = ProjectIntegratorState.Running;
			}

			// multiple thread instances cannot be created
			if (_thread == null || _thread.ThreadState == ThreadState.Stopped)
			{
				_thread = new Thread(new ThreadStart(Run));
				_thread.Name = _project.Name;
			}

			// start thread if it's not running yet
			if (_thread.ThreadState != ThreadState.Running)
			{
				Log.Info("Starting integrator for project: " + _project.Name);
				_thread.Start();
			}
		}

		public void ForceBuild()
		{
			Log.Info("Force Build for project: " + _project.Name);
			_forceBuild = true;
			Start();
		}

		/// <summary>
		/// Main integration loop, intended to be run in its own thread.
		/// </summary>
		private void Run()
		{
			// loop, until the integrator is stopped
			Log.Info("Starting integration for project: " + _project.Name);
			while (IsRunning)
			{
				// should we integrate this pass?
				BuildCondition buildCondition = ShouldRunIntegration();
				if (buildCondition != BuildCondition.NoBuild)
				{
					try
					{
						integratable.RunIntegration(buildCondition);
					}
					catch (Exception ex) 
					{ 
						Log.Error(ex);
					}

					// notify the schedule whether the build was successful or not
					integrationTrigger.IntegrationCompleted();
					stopProjectTrigger.IntegrationCompleted();
				}

				// should we stop the entire continuous integration process for this project?
				if (stopProjectTrigger.ShouldStopProject())
					_state = ProjectIntegratorState.Stopping;

				// sleep for a short while, to avoid hammering CPU
				Thread.Sleep(100);
			}

			// the state was set to 'Stopping', so set it to 'Stopped'
			_state = ProjectIntegratorState.Stopped;
		}

		private BuildCondition ShouldRunIntegration()
		{
			if (_forceBuild)
			{
				_forceBuild = false;
				return BuildCondition.ForceBuild;
			}
			return integrationTrigger.ShouldRunIntegration();
		}

		/// <summary>
		/// Gets a value indicating whether this project integrator is running
		/// and will continue to run.  If the state is Stopping, this returns false.
		/// </summary>
		public bool IsRunning
		{
			get { return _state == ProjectIntegratorState.Running; }
		}

		/// <summary>
		/// Sets the scheduler's state to <see cref="ProjectIntegratorState.Stopping"/>, telling the scheduler to
		/// stop at the next possible point in time.
		/// </summary>
		public void Stop()
		{
			if (IsRunning)
			{
				_state = ProjectIntegratorState.Stopping;
				Log.Info("Stopping integrator for project: " + _project.Name);
			}
		}

		public void Abort()
		{
			if (_thread != null)
			{
				_thread.Abort();
			}
			_state = ProjectIntegratorState.Stopped;
			Log.Info("Integrator for project: " + _project.Name + " is now stopped.");
		}

		public void WaitForExit()
		{
			if (_thread != null && _thread.IsAlive)
			{
				_thread.Join();
			}
		}

		/// <summary>
		/// Ensure that the scheduler's thread is terminated when this object is
		/// garbage collected.
		/// </summary>
		void IDisposable.Dispose()
		{
			Abort();
		}
	}
}
