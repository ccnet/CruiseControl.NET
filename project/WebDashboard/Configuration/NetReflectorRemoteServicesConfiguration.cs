using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
	[ReflectorType("remoteServices")]
	public class NetReflectorRemoteServicesConfiguration : IRemoteServicesConfiguration
	{
		private ServerLocation[] servers = new ServerLocation[0];

		[ReflectorArray("servers")]
		public ServerLocation[] Servers
		{
			get
			{
				return servers;
			}
			set
			{
				servers = value;
			}
		}
	}
}
