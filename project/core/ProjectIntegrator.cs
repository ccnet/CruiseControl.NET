using System;
using System.Threading;
using ThoughtWorks.CruiseControl.Core.Queues;
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
	public class ProjectIntegrator : IProjectIntegrator, IDisposable, IIntegrationQueueNotifier
	{
		private readonly ITrigger trigger;
		private readonly IProject project;
		private readonly IIntegrationQueue integrationQueue;
		private Thread thread;
		private ProjectIntegratorState state = ProjectIntegratorState.Stopped;

		public ProjectIntegrator(IProject project, IIntegrationQueue integrationQueue)
		{
			trigger = project.Triggers;
			this.project = project;
			this.integrationQueue = integrationQueue;
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
			AddToQueue(new IntegrationRequest(BuildCondition.ForceBuild, "intervalTrigger"));
			Start();
		}

		public void Request(IntegrationRequest request)
		{
			AddToQueue(request);
			Start();
		}

		public void CancelPendingRequest()
		{
			integrationQueue.RemovePendingRequest(project);
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
					try
					{
						Integrate();
					}
					catch (Exception ex)
					{
						Log.Error(ex);
					}
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
			IntegrationRequest ir = integrationQueue.GetNextRequest(project);
			if (ir != null)
			{
				Log.Info(string.Format("Project: '{0}' is first in queue: '{1}' and shall start integration.",
				                       project.Name, project.QueueName));
				try
				{
					project.Integrate(ir);
				}
				finally
				{
					RemoveCompletedRequestFromQueue();
				}
			}
			else
			{
				PollTriggers();
				// If a build is queued for this project we need to hang around until either:
				// - the build gets started by reaching it's turn on the queue
				// - the build gets cancelled from the queue
				// - the thread gets killed
				while (IsRunning && integrationQueue.HasItemOnQueue(project) && integrationQueue.GetNextRequest(project) == null)
				{
					Thread.Sleep(200);
				}
			}
		}

		private void PollTriggers()
		{
			IntegrationRequest triggeredRequest = trigger.Fire();
			if (triggeredRequest != null)
			{
				// Add to the queue - if it is able to build straight away
				// then integrationRequest will get set immediately by the callback.
				AddToQueue(triggeredRequest);
			}
		}

		private void AddToQueue(IntegrationRequest request)
		{
			integrationQueue.Enqueue(new IntegrationQueueItem(project, request, this));
		}

		private void RemoveCompletedRequestFromQueue()
		{
			// Free up the queue to kick off the next integration in it if any.
			integrationQueue.Dequeue();
		}

		private void Stopped()
		{
			// the state was set to 'Stopping', so set it to 'Stopped'
			state = ProjectIntegratorState.Stopped;
			thread = null;
			// Ensure that any queued integrations are cleared for this project.
			integrationQueue.RemoveProject(project);
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

		/// <summary>
		/// Notification of entering the integration queue.
		/// </summary>
		public void NotifyEnteringIntegrationQueue()
		{
			project.NotifyPendingState();
		}

		/// <summary>
		/// Notification of exiting the integration queue. This could be due to a single project completing,
		/// a pending integration being cancelled or due to all projects being removed from the queue.
		/// </summary>
		public void NotifyExitingIntegrationQueue(bool isPendingItemCancelled)
		{
			if (!integrationQueue.HasItemOnQueue(project))
			{
				project.NotifySleepingState();
			}
			else if (isPendingItemCancelled)
			{
				// Must have been a queued force build for the same project while we are currently integrating 
				// the first in the queue. Do not touch the state of the project in this scenario.
			}
			else
			{
				// State should go to pending as we still have an item on the queue
				project.NotifyPendingState();
			}
			trigger.IntegrationCompleted();
		}
	}
}