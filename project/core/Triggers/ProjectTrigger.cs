namespace ThoughtWorks.CruiseControl.Core.Triggers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.util;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

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
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const string DefaultServerUri = RemoteCruiseServer.DefaultManagerUri;
		private const int DefaultIntervalSeconds = 5;

        private readonly ICruiseServerClientFactory factory;
		private ProjectStatus lastStatus;
		private ProjectStatus currentStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTrigger"/> class.
        /// </summary>
        public ProjectTrigger()
            : this(new CruiseServerClientFactory())
		{}

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectTrigger"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public ProjectTrigger(ICruiseServerClientFactory factory)
		{
			this.factory = factory;
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

        /// <summary>
        /// The security credentials to pass through to the remote server.
        /// </summary>
        /// <version>1.6</version>
        /// <default>None</default>
        /// <remarks>
        /// These are only needed if the remote project has security applied. If credentials are passed to the remote
        /// server, then the enforcerName will be ignored.
        /// Valid security tokens are: "username" and "password" (this list may be expanded in future).
        /// </remarks>
        [ReflectorProperty("security", Required = false)]
        public NameValuePair[] SecurityCredentials { get; set; }

        /// <summary>
        /// Integrations the completed.	
        /// </summary>
        /// <remarks></remarks>
		public void IntegrationCompleted()
		{
			lastStatus = currentStatus;
			InnerTrigger.IntegrationCompleted();
		}

		private ProjectStatus GetCurrentProjectStatus()
		{
			Log.Debug("Retrieving ProjectStatus from server: " + ServerUri);
            var client = factory.GenerateClient(ServerUri);
            var loggedIn = false;
            if ((SecurityCredentials != null) && (SecurityCredentials.Length > 0))
            {
                Log.Debug("Logging in");
                if (client.Login(new List<NameValuePair>(SecurityCredentials)))
                {
                    loggedIn = true;
                    Log.Debug("Logged on server, session token is " + client.SessionToken);
                }
                else
                {
                    Log.Warning("Unable to login to remote server");
                }
            }
            else if (RemoteServerUri.IsLocal(this.ServerUri))
            {
                Log.Debug("Sending local server bypass");
                client.SessionToken = SecurityOverride.SessionIdentifier;
            }

            try
            {
                var currentStatuses = client.GetProjectStatus();
                var project = currentStatuses.FirstOrDefault(p => p.Name == Project);
                if (project != null)
                {
                    Log.Debug("Found status for dependent project {0} is {1}", project.Name, project.BuildStatus);
                    return project;
                }

                throw new NoSuchProjectException(Project);
            }
            finally
            {
                if (loggedIn)
                {
                    Log.Debug("Logging out");
                    client.Logout();
                }
            }
		}

        /// <summary>
        /// Gets the next build.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public DateTime NextBuild
		{
			get
			{
                return InnerTrigger.NextBuild;
			}
		}

        /// <summary>
        /// Fires this instance.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
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
