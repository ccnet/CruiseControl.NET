using System;
using System.Threading;
using tw.ccnet.remote;
using tw.ccnet.core.util;

namespace tw.ccnet.core
{
	/// <summary>
	/// An object responsible for the continuous integration of a single project.
	/// This integrator, when running, coordinates the top-level life cycle of
	/// a project's integration.
	/// <list type="1">
	///		<item>The <see cref="ISchedule"/> instance is asked whether to build or not.</item>
	///		<item>If a build is required, the <see cref="IProject.RunIntegration()"/>
	///		is called.</item>
	/// </list>
	/// </summary>
	public class ProjectIntegrator : IProjectIntegrator
	{
		ISchedule _schedule;
		IProject _project;
		Thread _thread;
		ProjectIntegratorState _state = ProjectIntegratorState.Stopped;

		public ProjectIntegrator(ISchedule schedule, IProject project)
		{
			_schedule = schedule;
			_project = project;
		}

		#region Properties

		public IProject Project 
		{ 
			get { return _project; }
			set { _project = value; } 
		}

		public ISchedule Schedule
		{
			get { return _schedule; }
			set { _schedule = value; }
		}

		public ProjectIntegratorState State
		{
			get { return _state; }
		}

		#endregion

		public void Start()
		{
			lock (this)
			{
				if (IsRunning)
					return;

				_state = ProjectIntegratorState.Running;
			}

			// multiple thread instances cannot be created
			if (_thread==null || _thread.ThreadState==ThreadState.Stopped)
			{
				_thread = new Thread(new ThreadStart(Run));
				_thread.Name = "ProjectIntegrator for project " + _project.Name;
			}

			// start thread if it's not running yet
			if (_thread.ThreadState!=ThreadState.Running)
				_thread.Start();
		}

		/// <summary>
		/// Main integration loop, intended to be run in its own thread.
		/// </summary>
		void Run()
		{
			// loop, until the integrator is stopped
			while (IsRunning)
			{
				// should we integrate this pass?
				BuildCondition buildCondition = _schedule.ShouldRunIntegration();
				if (buildCondition != BuildCondition.NoBuild)
				{
					try
					{
						IntegrationResult result = _project.RunIntegration(buildCondition);
					}
					catch (Exception ex) 
					{ 
						CruiseControlException cce
							= new CruiseControlException("Project threw an exception while integrating", ex);

						LogUtil.Log(_project, "Project threw an exception while integrating.", cce);
					}

					// notify the schedule whether the build was successful or not
					_schedule.IntegrationCompleted();
				}

				// should we stop the entire continuous integration process for this project?
				if (_schedule.ShouldStopIntegration())
					_state = ProjectIntegratorState.Stopping;

				// sleep for a short while, to avoid hammering CPU
				Thread.Sleep(100);
			}

			// the state was set to 'Stopping', so set it to 'Stopped'
			_state = ProjectIntegratorState.Stopped;
		}

		/// <summary>
		/// Gets a value indicating whether this project integrator is running
		/// and will continue to run.  If the state is Stopping, this returns false.
		/// </summary>
		public bool IsRunning
		{
			get { return _state==ProjectIntegratorState.Running; }
		}

		/// <summary>
		/// Sets the scheduler's state to <see cref="Stopping"/>, telling the scheduler to
		/// stop at the next possible point in time.
		/// </summary>
		public void Stop()
		{
			if (IsRunning)
			{
				_state = ProjectIntegratorState.Stopping;
//				if (_thread != null && _thread.ThreadState == ThreadState.WaitSleepJoin)
//				{
//					_thread.Resume();
//				}
			}
		}

		public void Abort()
		{
			if (_thread != null)
			{
				_thread.Abort();
			}
			_state = ProjectIntegratorState.Stopped;
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
		public void Dispose()
		{
			Abort();
		}
	}
}
