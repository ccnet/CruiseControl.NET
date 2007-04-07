using System;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class HttpCruiseServerManager : ICruiseServerManager
	{
		private readonly string serverUrl;
		private readonly Uri serverUri;
		private readonly string displayName;
		private readonly BuildServerTransport transport;
		private readonly IWebRetriever webRetriever;
		private readonly IDashboardXmlParser dashboardXmlParser;

		public HttpCruiseServerManager(IWebRetriever webRetriever, IDashboardXmlParser dashboardXmlParser, BuildServer buildServer)
		{
			this.webRetriever = webRetriever;
			this.dashboardXmlParser = dashboardXmlParser;
			this.serverUrl = buildServer.Url;
			this.transport = buildServer.Transport;
			this.serverUri = buildServer.Uri;
			this.displayName = GetDisplayNameFromUri(serverUri);
		}

		public string ServerUrl
		{
			get { return serverUrl; }
		}

		public string DisplayName
		{
			get { return displayName; }
		}

		public BuildServerTransport Transport
		{
			get { return transport; }
		}

		public void CancelPendingRequest(string projectName)
		{
			throw new NotImplementedException("Cancel pending not currently supported on servers monitored via HTTP");
		}

		public IntegrationQueueSnapshot GetIntegrationQueueSnapshot()
		{
			throw new NotImplementedException("GD - Awaiting dashboard changes by Daniel Piessens");
//			IntegrationQueueSnapshot snapshot;
//			string content = webRetriever.Get(serverUri);
//			snapshot = dashboardXmlParser.ExtractAsIntegrationQueueSnapshot(content, serverName);
//			snapshot.ServerUrl = serverUrl;
//			return snapshot;
		}

		private string GetDisplayNameFromUri(Uri uri)
		{
			const int DefaultHttpPort = 80;
			// TODO: The BuildServer.DisplayName property is coded such that it only strips out the server
			// name if it is a remoting connection, not an http one.
			// Rather than changing this behaviour in case it is seen as "desirable" the functionality is
			// implemented within here so that the server queues treeview can just display the server name.
			if (uri.Port == DefaultHttpPort)
				return uri.Host;

			return uri.Host + ":" + uri.Port;
		}
	}
}
