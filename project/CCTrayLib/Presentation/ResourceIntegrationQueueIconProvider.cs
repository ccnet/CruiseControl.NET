using System.Collections;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ResourceIntegrationQueueIconProvider : IIntegrationQueueIconProvider
	{
		public static readonly StatusIcon REMOTING_SERVER = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.ServerRemoting.ico");
		public static readonly StatusIcon HTTP_SERVER = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.ServerHttp.ico");
		public static readonly StatusIcon QUEUE = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.Queue.ico");
		public static readonly StatusIcon FIRST_IN_QUEUE = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.Yellow.ico");
		public static readonly StatusIcon PENDING = new StatusIcon("ThoughtWorks.CruiseControl.CCTrayLib.Stopwatch.ico");

		private static readonly Hashtable map = new Hashtable();

		static ResourceIntegrationQueueIconProvider()
		{
			map.Add(IntegrationQueueNodeType.RemotingServer, REMOTING_SERVER);
			map.Add(IntegrationQueueNodeType.HttpServer, HTTP_SERVER);
			map.Add(IntegrationQueueNodeType.Queue, QUEUE);
			map.Add(IntegrationQueueNodeType.FirstInQueue, FIRST_IN_QUEUE);
			map.Add(IntegrationQueueNodeType.PendingInQueue, PENDING);
		}

		public StatusIcon GetStatusIconForNodeType( IntegrationQueueNodeType integrationQueueNodeType )
		{
			return (StatusIcon) map[integrationQueueNodeType];
		}
	}
}
