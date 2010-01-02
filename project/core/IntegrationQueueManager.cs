
using System.Collections;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Queues;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System;
using ThoughtWorks.CruiseControl.Remote.Events;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core
{
	public class IntegrationQueueManager
        : IQueueManager
	{
		private readonly IProjectIntegratorListFactory projectIntegratorListFactory;
		private IProjectIntegratorList projectIntegrators;

		private readonly IntegrationQueueSet integrationQueues = new IntegrationQueueSet();
        private readonly IProjectStateManager stateManager;

		public IntegrationQueueManager(IProjectIntegratorListFactory projectIntegratorListFactory,
		                               IConfiguration configuration,
                                       IProjectStateManager stateManager)
		{
			this.projectIntegratorListFactory = projectIntegratorListFactory;
			Initialize(configuration);
            this.stateManager = stateManager;
		}

		/// <summary>
		/// Gets the projects and integration queues snapshot from this server.
		/// </summary>
        public CruiseServerSnapshot GetCruiseServerSnapshot()
		{
		    ProjectStatus[] projectStatuses = GetProjectStatuses();
		    QueueSetSnapshot queueSetSnapshot = integrationQueues.GetIntegrationQueueSnapshot();
            return new CruiseServerSnapshot(projectStatuses, queueSetSnapshot);
		}

		public void StartAllProjects()
		{
			foreach (IProjectIntegrator integrator in projectIntegrators)
			{
                bool canStart = (integrator.Project == null) ||
                    ((integrator.Project.StartupMode == ProjectStartupMode.UseLastState) &&
                    stateManager.CheckIfProjectCanStart(integrator.Name)) ||
                    ((integrator.Project.StartupMode == ProjectStartupMode.UseInitialState) &&
                    (integrator.Project.InitialState == ProjectInitialState.Started));
                if (canStart) integrator.Start();
			}
		}

		public void StopAllProjects()
		{
			foreach (IProjectIntegrator integrator in projectIntegrators)
			{
				integrator.Stop();
			}
			WaitForIntegratorsToExit();
			// We should clear the integration queue so the queues can be rebuilt when start again.
			integrationQueues.Clear();
		}

		public void Abort()
		{
			foreach (IProjectIntegrator integrator in projectIntegrators)
			{
				integrator.Abort();
			}
			WaitForIntegratorsToExit();
			// We should clear the integration queue so the queues can be rebuilt when start again.
			integrationQueues.Clear();
		}

		public ProjectStatus[] GetProjectStatuses()
		{
			ArrayList projectStatusList = new ArrayList();
			foreach (IProjectIntegrator integrator in projectIntegrators)
			{
				IProject project = integrator.Project;
				projectStatusList.Add(project.CreateProjectStatus(integrator));
			}
			return (ProjectStatus[]) projectStatusList.ToArray(typeof (ProjectStatus));
		}

		public IProjectIntegrator GetIntegrator(string projectName)
		{
			IProjectIntegrator integrator = projectIntegrators[projectName];
			if (integrator == null) throw new NoSuchProjectException(projectName);
			return integrator;
		}

        public void ForceBuild(string projectName, string enforcerName, Dictionary<string, string> buildValues)
		{
            GetIntegrator(projectName).ForceBuild(enforcerName, buildValues);
		}

		public void WaitForExit(string projectName)
		{
			GetIntegrator(projectName).WaitForExit();
		}

		public void Request(string project, IntegrationRequest request)
		{
			GetIntegrator(project).Request(request);
		}

		public void CancelPendingRequest(string projectName)
		{
			GetIntegrator(projectName).CancelPendingRequest();
		}

		public void Stop(string project)
		{
            stateManager.RecordProjectAsStopped(project);
			GetIntegrator(project).Stop();
		}

		public void Start(string project)
		{
            stateManager.RecordProjectAsStartable(project);
			GetIntegrator(project).Start();
		}

		public void Restart(IConfiguration configuration)
		{
			StopAllProjects();
			Initialize(configuration);
			StartAllProjects();
		}

		private void WaitForIntegratorsToExit()
		{
			foreach (IProjectIntegrator integrator in projectIntegrators)
			{
				integrator.WaitForExit();
			}
		}

		private void Initialize(IConfiguration configuration)
		{
			foreach (IProject project in configuration.Projects)
			{
				// Force the queue to be created if it does not exist already.
                IQueueConfiguration config = configuration.FindQueueConfiguration(project.QueueName);
				integrationQueues.Add(project.QueueName, config);
			}
			projectIntegrators = projectIntegratorListFactory.CreateProjectIntegrators(configuration.Projects, integrationQueues);

			if (projectIntegrators.Count == 0)
			{
				Log.Info("No projects found");
			}
		}

		/// <summary>
		/// Returns an array of the current queue names in usage.
		/// </summary>
		/// <returns>Array of current queue names in use.</returns>
		public string[] GetQueueNames()
		{
			return integrationQueues.GetQueueNames();
		}

        /// <summary>
        /// Associates the integration events.
        /// </summary>
        /// <param name="integrationStarted"></param>
        /// <param name="integrationCompleted"></param>
        public void AssociateIntegrationEvents(EventHandler<IntegrationStartedEventArgs> integrationStarted,
            EventHandler<IntegrationCompletedEventArgs> integrationCompleted)
        {
            foreach (IProjectIntegrator integrator in projectIntegrators)
            {
                integrator.IntegrationStarted += integrationStarted;
                integrator.IntegrationCompleted += integrationCompleted;
            }
        }
	}
}
