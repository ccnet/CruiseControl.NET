using System;
using System.Threading;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// An object responsible for the continuous integration of a single project.
	/// This integrator, when running, coordinates the top-level life cycle of
	/// a project's integration.
	/// <list type="1">
	///		<item>The <see cref="ITrigger"/> instance is asked whether to build or not.</item>
	///		<item>If a build is required, the <see cref="IProject.RunIntegration(BuildCondition buildCondition)"/>
	///		is called.</item>
	/// </list>
	/// </summary>
	public class ProjectIntegrator : IProjectIntegrator, IDisposable
	{
		private readonly IIntegratable integratable;
		private ITrigger Trigger;
		private IProject _project;
		private bool _forceBuild;
		private Thread _thread;
		private ProjectIntegratorState _state = ProjectIntegratorState.Stopped;

		public ProjectIntegrator(IProject project) : this(new MultipleTrigger(project.Triggers), project, project)
		{
		}

		public ProjectIntegrator(ITrigger Trigger, IIntegratable integratable, IProject project)
		{
			this.Trigger = Trigger;
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
			Log.Info("Starting integration for project: " + _project.Name);
			try
			{
				// loop, until the integrator is stopped
				while (IsRunning)
				{
					Integrate();

					// sleep for a short while, to avoid hammering CPU
					Thread.Sleep(100);
				}
			}
			catch (ThreadAbortException ex)
			{
				Thread.ResetAbort();
			}
			finally
			{
				Stopped();
			}
		}

		private void Integrate()
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
				Trigger.IntegrationCompleted();
			}
		}

		private BuildCondition ShouldRunIntegration()
		{
			if (_forceBuild)
			{
				_forceBuild = false;
				return BuildCondition.ForceBuild;
			}
			return Trigger.ShouldRunIntegration();
		}

		private void Stopped()
		{
			// the state was set to 'Stopping', so set it to 'Stopped'
			_state = ProjectIntegratorState.Stopped;
			_thread = null;
			Log.Info("Integrator for project: " + _project.Name + " is now stopped.");
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
				Log.Info("Stopping integrator for project: " + _project.Name);
				_state = ProjectIntegratorState.Stopping;
			}
		}

		/// <summary>
		/// Asynchronously abort project by aborting the project thread.  This needs to be followed by a call to WaitForExit 
		/// to ensure that the abort has completed.
		/// </summary>
		public void Abort()
		{
			if (_thread != null)
			{
				Log.Info("Aborting integrator for project: " + _project.Name);
				_thread.Abort();
			}
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