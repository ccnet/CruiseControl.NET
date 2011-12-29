using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
    /// <summary>
    /// The Dashboard needs to connect over the network to each of Build Servers you want to report on. 
    /// If you have changed any of the remoting configuration for your servers, you'll need those details now.
    /// To configure the dashboard, add a &lt;server&gt; tag for each CruiseControl.NET Server you want to monitor to the &lt;servers&gt; section.
    /// </summary>
    /// <title>Server</title>
    /// <version>1.0</version>
    /// <example>
    /// <code>
    /// &lt;server name="local" url="tcp://localhost:21234/CruiseManager.rem" allowForceBuild="true" allowStartStopBuild="true" /&gt;
    /// </code>
    /// </example>
	[ReflectorType("server")]
	public class ServerLocation: IServerSpecifier
	{
		private string serverName = string.Empty;
		private string url = string.Empty;
		private bool allowForceBuild = true;
		private bool allowStartStopBuild = true;
		
        /// <summary>
        /// A referential name for the server - this must be unique for each server monitored by the Dashboard
        /// </summary>
        /// <default>local</default>
        /// <version>1.0</version>
		[ReflectorProperty("name")]
		public string Name
		{
			get { return serverName; }
			set { serverName = value; }
		}
 
        /// <summary>
        /// The management URL for the Server 
        /// </summary>
        /// <default>tcp://localhost:21234/CruiseManager.rem</default>
        /// <version>1.0</version>
		[ReflectorProperty("url")]
		public string Url
		{
			get { return url; }
			set { url = value; }
		}
 
        /// <summary>
        /// Displays or hides the Force Build button on the Project Dashboard page 
        /// </summary>
        /// <default>true</default>
        /// <version>1.4</version>
		[ReflectorProperty("allowForceBuild", Required=false)]
		public bool AllowForceBuild
		{
			get { return allowForceBuild; }
			set { allowForceBuild = value; }	
		}
 		
        /// <summary>
        /// Displays or hides the Stop Build button on the Project Dashboard page. 
        /// Stopping a project will stop the project's triggers so that they will not trigger new builds until the project is started again; 
        /// this will not abort a build that is in progress.
        /// </summary>
        /// <default>true</default>
        /// <version>1.4.0</version>
		[ReflectorProperty("allowStartStopBuild", Required=false)]
		public bool AllowStartStopBuild
		{
			get { return allowStartStopBuild; }
			set { allowStartStopBuild = value; }	
		}

        /// <summary>
        /// Should the client handle server versions older than 1.5.0.
        /// beta code, could be removed
        /// </summary>
        /// <default>false</default>
        /// <version>1.5</version>
        [ReflectorProperty("backwardsCompatible", Required = false)]
        public bool BackwardCompatible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the transfer should use compressed logs.
        /// </summary>
        /// <default>false</default>
        /// <version>1.6</version>
        [ReflectorProperty("compressLogs", Required = false)]
        public bool TransferLogsCompressed { get; set; }
		
		public string ServerName		
		{
			get { return serverName; }
			set { serverName = value; }
		}

	}
}