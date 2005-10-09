using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	[ReflectorType("forcebuild")]
	public class ForceBuildPublisher : ITask
	{
		private readonly ICruiseManagerFactory factory;

		public ForceBuildPublisher() : this(new RemoteCruiseManagerFactory())
		{}

		public ForceBuildPublisher(ICruiseManagerFactory factory)
		{
			this.factory = factory;
		}

		[ReflectorProperty("project")]
		public string Project;

		[ReflectorProperty("serverUri", Required=false)]
		public string ServerUri = string.Format("tcp://localhost:21234/{0}", RemoteCruiseServer.URI);

		[ReflectorProperty("integrationStatus", Required=false)]
		public IntegrationStatus IntegrationStatus = IntegrationStatus.Success;

		public void Run(IIntegrationResult result)
		{
			if (IntegrationStatus != result.Status) return;

			factory.GetCruiseManager(ServerUri).ForceBuild(Project);
		}
	}
}