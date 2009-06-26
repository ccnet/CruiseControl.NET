using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	[ReflectorType("forcebuild")]
	public class ForceBuildPublisher 
        : TaskBase
	{
		private readonly ICruiseManagerFactory factory;
        private string BuildForcerName="BuildForcer";

		public ForceBuildPublisher() : this(new RemoteCruiseManagerFactory())
		{}

		public ForceBuildPublisher(ICruiseManagerFactory factory)
		{
			this.factory = factory;
		}

        [ReflectorProperty("project")]
        public string Project;


        [ReflectorProperty("enforcerName", Required = false)]
        public string EnforcerName
        {
            get { return BuildForcerName; }
            set { BuildForcerName = value; }
        }

		[ReflectorProperty("serverUri", Required=false)]
		public string ServerUri = string.Format("tcp://localhost:21234/{0}", RemoteCruiseServer.ManagerUri);

		[ReflectorProperty("integrationStatus", Required=false)]
		public IntegrationStatus IntegrationStatus = IntegrationStatus.Success;

        protected override bool Execute(IIntegrationResult result)
		{
			if (IntegrationStatus != result.Status) return false;

            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Running for build publisher");                

            factory.GetCruiseManager(ServerUri).ForceBuild(Project, BuildForcerName);

            return true;
		}
	}
}