using System;
using System.Threading;
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
	///		<item>If a build is required, the <see cref="IProject.Integrate(IntegrationRequest request)"/>
	///		is called.</item>
	/// </list>
	/// </summary>
	public class ProjectIntegrator : IProjectIntegrator, IDisposable
	{
		private readonly ITrigger trigger;
		private readonly IProject project;
		private Thread thread;
		private ProjectIntegratorState state = ProjectIntegratorState.Stopped;
		public IntegrationRequest request;

		public ProjectIntegrator(IProject project)
		{
			this.trigger = project.Triggers;
			this.project = project;
		}

		public string Name
		{
			get { return project.Name; }
		}

		public IProject Project
		{
			get { return project; }
		}

		public ProjectIntegratorState State
		{
			get { return state; }
		}

		public IIntegrationRepository IntegrationRepository
		{
			get { return project.IntegrationRepository; }
		}

		// TODO: should not start if stopping (ie. not stopped)
		public void Start()
		{
			lock (this)
			{
				if (IsRunning)
					return;

				state = ProjectIntegratorState.Running;
			}

			// multiple thread instances cannot be created
			if (thread == null || thread.ThreadState == ThreadState.Stopped)
			{
				thread = new Thread(new ThreadStart(Run));
				thread.Name = project.Name;
			}

			// start thread if it's not running yet
			if (thread.ThreadState != ThreadState.Running)
			{
				thread.Start();
			}
		}

		public void ForceBuild()
		{
			Log.Info("Force Build for project: " + project.Name);
			this.request = new IntegrationRequest(BuildCondition.ForceBuild, "intervalTrigger");
			Start();
		}

		public void Request(IntegrationRequest request)
		{
			this.request = request;
			Start();
		}

		/// <summary>
		/// Main integration loop, intended to be run in its own thread.
		/// </summary>
		private void Run()
		{
			Log.Info("Starting integrator for project: " + project.Name);
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
			catch (ThreadAbortException)
			{
				// suppress logging of ThreadAbortException
				Thread.ResetAbort();
			}
			finally
			{
				Stopped();
			}
		}

		private void Integrate()
		{
			if (ShouldRunIntegration())
			{
				IntegrationRequest temp = request;
				request = null;
				try
				{
					project.Integrate(temp);
				}
				catch (Exception ex)
				{
					Log.Error(ex);
				}
				trigger.IntegrationCompleted();
			}
		}

		// TODO: threading issues here: request could get overridden by force at this point
		private bool ShouldRunIntegration()
		{
			if (request == null)
			{
				request = trigger.Fire();
			}
			return request != null;
		}

		private void Stopped()
		{
			// the state was set to 'Stopping', so set it to 'Stopped'
			state = ProjectIntegratorState.Stopped;
			thread = null;
			Log.Info("Integrator for project: " + project.Name + " is now stopped.");
		}

		/// <summary>
		/// Gets a value indicating whether this project integrator is running
		/// and will continue to run.  If the state is Stopping, this returns false.
		/// </summary>
		public bool IsRunning
		{
			get { return state == ProjectIntegratorState.Running; }
		}

		/// <summary>
		/// Sets the state to <see cref="ProjectIntegratorState.Stopping"/>, telling the project to
		/// stop at the next possible point in time.
		/// </summary>
		public void Stop()
		{
			if (IsRunning)
			{
				Log.Info("Stopping integrator for project: " + project.Name);
				state = ProjectIntegratorState.Stopping;
			}
		}

		/// <summary>
		/// Asynchronously abort project by aborting the project thread.  This needs to be followed by a call to WaitForExit 
		/// to ensure that the abort has completed.
		/// </summary>
		public void Abort()
		{
			if (thread != null)
			{
				Log.Info("Aborting integrator for project: " + project.Name);
				thread.Abort();
			}
		}

		public void WaitForExit()
		{
			if (thread != null && thread.IsAlive)
			{
				thread.Join();
			}
		}

		/// <summary>
		/// Ensure that the integrator's thread is aborted when this object is disposed.
		/// </summary>
		void IDisposable.Dispose()
		{
			Abort();
		}
	}
}