using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
	[ReflectorType("projectTrigger")]
	public class ProjectTrigger : ITrigger
	{
		public const string DefaultServerUri = RemoteCruiseServer.DefaultUri;
		private const int DefaultIntervalSeconds = 5;

		private readonly ICruiseManagerFactory managerFactory;
		private ProjectStatus lastStatus;
		private ProjectStatus currentStatus;

		public ProjectTrigger() : this(new RemoteCruiseManagerFactory())
		{}

		public ProjectTrigger(ICruiseManagerFactory managerFactory)
		{
			this.managerFactory = managerFactory;
		}

		[ReflectorProperty("project")]
		public string Project;

		[ReflectorProperty("serverUri", Required=false)]
		public string ServerUri = DefaultServerUri;

		[ReflectorProperty("triggerStatus", Required=false)]
		public IntegrationStatus TriggerStatus = IntegrationStatus.Success;

		[ReflectorProperty("innerTrigger", InstanceTypeKey="type", Required=false)]
		public ITrigger InnerTrigger = NewIntervalTrigger();

		public void IntegrationCompleted()
		{
			lastStatus = currentStatus;
			InnerTrigger.IntegrationCompleted();
		}

		private ProjectStatus GetCurrentProjectStatus()
		{
			Log.Debug("Retrieving ProjectStatus from server: " + ServerUri);
			ProjectStatus[] currentStatuses = managerFactory.GetCruiseManager(ServerUri).GetProjectStatus();
			foreach (ProjectStatus currentStatus in currentStatuses)
			{
				if (currentStatus.Name == Project)
				{
					return currentStatus;
				}
			}
			throw new NoSuchProjectException(Project);
		}

		public DateTime NextBuild
		{
			get
			{
				if (lastStatus == null) return InnerTrigger.NextBuild;
				return lastStatus.NextBuildTime;
			}
		}

		public IntegrationRequest Fire()
		{
			IntegrationRequest request = InnerTrigger.Fire();
			if (request == null) return null;
			InnerTrigger.IntegrationCompleted(); // reset inner trigger (timer)

			currentStatus = GetCurrentProjectStatus();
			if (lastStatus == null)
			{
				lastStatus = currentStatus;
				return null;
			}
			if (currentStatus.LastBuildDate > lastStatus.LastBuildDate && currentStatus.BuildStatus == TriggerStatus)
			{
				return request;
			}
			return null;		
		}

		private static ITrigger NewIntervalTrigger()
		{
			IntervalTrigger trigger = new IntervalTrigger();
			trigger.IntervalSeconds = DefaultIntervalSeconds;
			trigger.BuildCondition = BuildCondition.ForceBuild;
			return trigger;
		}
	}
}