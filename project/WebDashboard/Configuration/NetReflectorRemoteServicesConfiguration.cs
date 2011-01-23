using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
	[ReflectorType("remoteServices")]
	public class NetReflectorRemoteServicesConfiguration : IRemoteServicesConfiguration
	{
		private ServerLocation[] servers = new ServerLocation[0];

        [ReflectorProperty("servers")]
		public ServerLocation[] Servers
		{
			get { return servers; }
			set { servers = value; }
		}

		
		public ServerLocation GetServerLocation(string serverName)
		{
			foreach (ServerLocation server in servers)
			{
				if (server.ServerName == serverName)
					return server;
			}
			return null;
		}
	}
}
