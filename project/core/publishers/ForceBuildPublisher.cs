using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	[ReflectorType("forcebuild")]
	public class ForceBuildPublisher : PublisherBase
	{
		private readonly IRemotingService remotingService;

		public ForceBuildPublisher() : this(new RemotingServiceAdapter()) { }

		public ForceBuildPublisher(IRemotingService remotingService)
		{
			this.remotingService = remotingService;
		}

		[ReflectorProperty("project")]
		public string Project;

		[ReflectorProperty("serverUri")]
		public string ServerUri;

		[ReflectorProperty("integrationStatus", Required=false)]
		public IntegrationStatus IntegrationStatus = IntegrationStatus.Success;

		public override void PublishIntegrationResults(IProject project, IIntegrationResult result)
		{
			if (IntegrationStatus != result.Status) return;

			ICruiseManager manager = (ICruiseManager) remotingService.Connect(typeof(ICruiseManager), ServerUri);
			manager.ForceBuild(Project);
			remotingService.Disconnect(manager);
		}
	}
}
