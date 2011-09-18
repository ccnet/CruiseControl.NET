using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
    /// <summary>
    /// Information of remote services that the dashboard must handle.
    /// </summary>
    /// <title>Remote Services</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;remoteServices&gt;
    /// &lt;servers&gt;
    /// &lt;server name="linuxBuilder" url="tcp://Metroplex:21234/CruiseManager.rem" allowForceBuild="true" allowStartStopBuild="true" backwardsCompatible="false" /&gt; 
    /// &lt;server name="macBuilder" url="tcp://Trypticon:21234/CruiseManager.rem" allowForceBuild="true" allowStartStopBuild="true" backwardsCompatible="false" /&gt; 
    /// &lt;server name="winBuilder" url="tcp://OrionPax:21234/CruiseManager.rem" allowForceBuild="true" allowStartStopBuild="true" backwardsCompatible="false" /&gt; 
    /// &lt;/servers&gt;
    /// &lt;/remoteServices&gt;
    /// </code>
    /// </example>
    [ReflectorType("remoteServices")]
	public class NetReflectorRemoteServicesConfiguration : IRemoteServicesConfiguration
	{
		private ServerLocation[] servers = new ServerLocation[0];

        /// <summary>
        /// This section contains all the build servers that the Dashboard will visualize.
        /// </summary>
        /// <version>1.0</version>
        /// <default>&lt;server name="local" url="tcp://localhost:21234/CruiseManager.rem" allowForceBuild="true" allowStartStopBuild="true" backwardsCompatible="false" /&gt;</default>
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
