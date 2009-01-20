using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	[ReflectorType("forcebuild")]
	public class ForceBuildPublisher : ITask
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
		public string ServerUri = string.Format("tcp://localhost:21234/{0}", RemoteCruiseServer.URI);

		[ReflectorProperty("integrationStatus", Required=false)]
		public IntegrationStatus IntegrationStatus = IntegrationStatus.Success;


        /// <summary>
        /// Description used for the visualisation of the buildstage, if left empty the process name will be shown
        /// </summary>
        [ReflectorProperty("description", Required = false)]
        public string Description = string.Empty;


		public void Run(IIntegrationResult result)
		{
			if (IntegrationStatus != result.Status) return;

            result.BuildProgressInformation.SignalStartRunTask(Description != string.Empty ? Description : "Running for build publisher");                


            factory.GetCruiseManager(ServerUri).ForceBuild(Project, BuildForcerName);
		}
	}
}