using System.Collections;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ResourceIntegrationQueueIconProvider : IIntegrationQueueIconProvider
	{
		public static readonly StatusIcon REMOTING_SERVER = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.ServerRemoting.ico");
		public static readonly StatusIcon HTTP_SERVER = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.ServerHttp.ico");
        public static readonly StatusIcon QUEUE_EMPTY = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.QueueEmpty.ico");
        public static readonly StatusIcon QUEUE_POPULATED = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.QueuePopulated.ico");
        public static readonly StatusIcon CHECKING_MODIFICATIONS = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.BuildCheckingModifications.ico");
		public static readonly StatusIcon BUILDING = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.Yellow.ico");
        public static readonly StatusIcon PENDING = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.BuildPending.ico");

		private static readonly Hashtable map = new Hashtable();

		static ResourceIntegrationQueueIconProvider()
		{
			map.Add(IntegrationQueueNodeType.RemotingServer, REMOTING_SERVER);
			map.Add(IntegrationQueueNodeType.HttpServer, HTTP_SERVER);
            map.Add(IntegrationQueueNodeType.QueueEmpty, QUEUE_EMPTY);
            map.Add(IntegrationQueueNodeType.QueuePopulated, QUEUE_POPULATED);
            map.Add(IntegrationQueueNodeType.CheckingModifications, CHECKING_MODIFICATIONS);
			map.Add(IntegrationQueueNodeType.Building, BUILDING);
            map.Add(IntegrationQueueNodeType.PendingInQueue, PENDING);
		}

		public StatusIcon GetStatusIconForNodeType( IntegrationQueueNodeType integrationQueueNodeType )
		{
			return (StatusIcon) map[integrationQueueNodeType];
		}
	}
}
