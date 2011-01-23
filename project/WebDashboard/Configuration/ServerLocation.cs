using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
	[ReflectorType("server")]
	public class ServerLocation: IServerSpecifier
	{
		private string serverName = string.Empty;
		private string url = string.Empty;
		private bool allowForceBuild = true;
		private bool allowStartStopBuild = true;
		
		[ReflectorProperty("name")]
		public string Name
		{
			get { return serverName; }
			set { serverName = value; }
		}
 
		[ReflectorProperty("url")]
		public string Url
		{
			get { return url; }
			set { url = value; }
		}
 
		[ReflectorProperty("allowForceBuild", Required=false)]
		public bool AllowForceBuild
		{
			get { return allowForceBuild; }
			set { allowForceBuild = value; }	
		}
 		
		[ReflectorProperty("allowStartStopBuild", Required=false)]
		public bool AllowStartStopBuild
		{
			get { return allowStartStopBuild; }
			set { allowStartStopBuild = value; }	
		}

        [ReflectorProperty("backwardsCompatible", Required = false)]
        public bool BackwardCompatible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the transfer should use compressed logs.
        /// </summary>
        /// <value><c>true</c> if logs should be transferred compressed; otherwise, <c>false</c>.</value>
        [ReflectorProperty("compressLogs", Required = false)]
        public bool TransferLogsCompressed { get; set; }
		
		public string ServerName		
		{
			get { return serverName; }
			set { serverName = value; }
		}

	}
}