using System.Collections;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ResourceIntegrationQueueIconProvider : IIntegrationQueueIconProvider
	{
		public static readonly StatusIcon REMOTING_SERVER = new StatusIcon(DefaultQueueIcons.ServerRemoting, false);
        public static readonly StatusIcon HTTP_SERVER = new StatusIcon(DefaultQueueIcons.ServerHttp, false);
        public static readonly StatusIcon QUEUE_EMPTY = new StatusIcon(DefaultQueueIcons.QueueEmpty, false);
        public static readonly StatusIcon QUEUE_POPULATED = new StatusIcon(DefaultQueueIcons.QueuePopulated, false);
        public static readonly StatusIcon CHECKING_MODIFICATIONS = new StatusIcon(DefaultQueueIcons.BuildCheckingModifications, false);
        public static readonly StatusIcon BUILDING = new StatusIcon(DefaultQueueIcons.Yellow, false);
        public static readonly StatusIcon PENDING = new StatusIcon(DefaultQueueIcons.BuildPending, false);

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
