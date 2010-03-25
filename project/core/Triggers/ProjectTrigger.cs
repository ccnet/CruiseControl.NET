using System;
using System.Net.Sockets;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
    /// <summary>
    /// <para>
    /// The Project Trigger is used to trigger a build when the specified dependent project has completed its build. This trigger can help you split your 
    /// build process across projects and servers. For example, you could have a CCNet project that will trigger the regression test suite once the main 
    /// development build has completed successfully. This dependent build could be running on either a local or a remote CCNet server.
    /// </para>
    /// <para>
    /// The Project Trigger works by using .NET remoting to poll the status of the dependent project. Whenever it detects that the dependent project has 
    /// completed a build, the Project Trigger will fire. The Project Trigger can be configured to fire when the dependent project build succeeded, failed 
    /// or threw an exception. In order to avoid hammering the remote project through polling, the Project Trigger is composed of an <link>Interval Trigger
    /// </link>that will set a polling interval to 5 seconds. This inner trigger can be adjusted through changing the configuration.
    /// </para>
    /// <para type="info">
    /// Like all triggers, the projectTrigger must be enclosed within a triggers element in the appropriate <link>Project Configuration Block</link>.
    /// </para>
    /// </summary>
    /// <title>Project Trigger</title>
    /// <version>1.0</version>
    /// <remarks>
    /// <para type="warning">
    /// There is currently a limitation in the Project Trigger in that it will always trigger a build when the inner trigger fires (at the end of the first 
    /// interval for an Interval Trigger). This is because the Project Trigger has no way to save its state from a previous server run. So the last time that 
    /// the build was triggered is not retrievable when the server restarts.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;projectTrigger project="Core" /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;projectTrigger serverUri="tcp://server:21234/CruiseManager.rem" project="Server"&gt;
    /// &lt;triggerStatus&gt;Success&lt;/triggerStatus&gt;
    /// &lt;innerTrigger type="intervalTrigger" seconds="30" buildCondition="ForceBuild"/&gt;
    /// &lt;/projectTrigger&gt;
    /// </code>
    /// </example>
    [ReflectorType("projectTrigger")]
	public class ProjectTrigger : ITrigger
	{
		public const string DefaultServerUri = RemoteCruiseServer.DefaultManagerUri;
		private const int DefaultIntervalSeconds = 5;

		private readonly ICruiseManagerFactory managerFactory;
		private ProjectStatus lastStatus;
		private ProjectStatus currentStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTrigger"/> class.
        /// </summary>
		public ProjectTrigger() : this(new RemoteCruiseManagerFactory())
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTrigger"/> class.
        /// </summary>
        /// <param name="managerFactory">The manager factory.</param>
		public ProjectTrigger(ICruiseManagerFactory managerFactory)
		{
			this.managerFactory = managerFactory;
            this.ServerUri = DefaultServerUri;
            this.TriggerStatus = IntegrationStatus.Success;
            this.InnerTrigger = ProjectTrigger.NewIntervalTrigger();
            this.TriggerFirstTime = false;
		}

        /// <summary>
        /// The name of the dependent project to trigger a build from.
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
        [ReflectorProperty("project")]
        public string Project { get; set; }

        /// <summary>
        /// The URI for the CCNet server containing the dependent project.
        /// </summary>
        /// <version>1.0</version>
        /// <default>tcp://localhost:21234/CruiseManager.rem</default>
        [ReflectorProperty("serverUri", Required = false)]
        public string ServerUri { get; set; }

        /// <summary>
        /// The status of the dependent project that will be used to trigger the build. For example, if this value is set to Success then a build will 
        /// be triggered when the dependent project completes a successful build.
        /// </summary>
        /// <version>1.0</version>
        /// <default>Success</default>
        [ReflectorProperty("triggerStatus", Required = false)]
        public IntegrationStatus TriggerStatus { get; set; }

        /// <summary>
        /// The trigger used to modulate the polling interval for the ProjectTrigger. By default, this is set to a ForceBuild IntervalTrigger that will cause 
        /// the trigger to check the status of the dependent project every 5 seconds.
        /// </summary>
        /// <version>1.0</version>
        /// <default>5 second ForceBuild intervalTrigger</default>
        [ReflectorProperty("innerTrigger", InstanceTypeKey = "type", Required = false)]
        public ITrigger InnerTrigger { get; set; }

        /// <summary>
        /// Whether to trigger on the first time or not.
        /// </summary>
        /// <version>1.0</version>
        /// <default>false</default>
        [ReflectorProperty("triggerFirstTime", Required = false)]
        public bool TriggerFirstTime { get; set; }

		public void IntegrationCompleted()
		{
			lastStatus = currentStatus;
			InnerTrigger.IntegrationCompleted();
		}

		private ProjectStatus GetCurrentProjectStatus()
		{
			Log.Debug("Retrieving ProjectStatus from server: " + ServerUri);
			ProjectStatus[] currentStatuses = managerFactory.GetCruiseManager(ServerUri).GetProjectStatus();
			foreach (ProjectStatus projectStatus in currentStatuses)
			{
				if (projectStatus.Name == Project)
				{
                    Log.Debug("Found status for dependent project {0} is {1}",projectStatus.Name,projectStatus.BuildStatus);
					return projectStatus;
				}
			}
			throw new NoSuchProjectException(Project);
		}

		public DateTime NextBuild
		{
			get
			{
                return InnerTrigger.NextBuild;
			}
		}

		public IntegrationRequest Fire()
		{
			IntegrationRequest request = InnerTrigger.Fire();
			if (request == null) return null;
			InnerTrigger.IntegrationCompleted(); // reset inner trigger (timer)

            try
            {
                currentStatus = GetCurrentProjectStatus();
                if (lastStatus == null)
                {
                    lastStatus = currentStatus;
                    if (TriggerFirstTime && currentStatus.BuildStatus == TriggerStatus)
                    {
                        return request;
                    }
                    return null;
                }
                if (currentStatus.LastBuildDate > lastStatus.LastBuildDate && currentStatus.BuildStatus == TriggerStatus)
                {
                    return request;
                }
            }
            catch (SocketException)
            {
                Log.Warning("Skipping Fire() because ServerUri " + ServerUri + " was not found.");
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
