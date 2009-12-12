using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Label
{
    /// <summary>
    /// This labeller retrieves the last successful integration label for a project on a remote server. You can use this labeller if you have
    /// split your build across multiple projects on different servers and you want to use a consistent version across all builds.
    /// </summary>
    /// <title>Remote Project Labeller</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;labeller type="remoteProjectLabeller"&gt;
    /// &lt;project&gt;Common&lt;/project&gt;
    /// &lt;serverUri&gt;tcp://mainbuild:21234/CruiseManager.rem&lt;/serverUri&gt;
    /// &lt;/labeller&gt;	
    /// </code>
    /// </example>
	[ReflectorType("remoteProjectLabeller")]
	public class RemoteProjectLabeller
        : LabellerBase
	{
		private IRemotingService remotingService;

		public RemoteProjectLabeller() : this(new RemotingServiceAdapter())
		{}

		public RemoteProjectLabeller(IRemotingService service)
		{
			remotingService = service;
		}

        /// <summary>
        /// The URI to the remote cruise server containing the project to use (defaults to the local build server).
        /// </summary>
        /// <version>1.0</version>
        /// <default>tcp://localhost:21234/CruiseManager.rem</default>
		[ReflectorProperty("serverUri", Required=false)]
		public string ServerUri = RemoteCruiseServer.DefaultManagerUri;

        /// <summary>
        /// The project to retrieve the label from. 
        /// </summary>
        /// <version>1.0</version>
        /// <default>n/a</default>
		[ReflectorProperty("project")]
		public string ProjectName;

		public override string Generate(IIntegrationResult result)
		{
			ICruiseManager manager = (ICruiseManager) remotingService.Connect(typeof (ICruiseManager), ServerUri);

			ProjectStatus[] statuses = manager.GetProjectStatus();
			foreach (ProjectStatus status in statuses)
			{
				if (status.Name == ProjectName)
				{
					return status.LastSuccessfulBuildLabel;
				}
			}
			throw new NoSuchProjectException(ProjectName);
		}
	}
}