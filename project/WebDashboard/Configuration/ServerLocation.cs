using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
	[ReflectorType("server")]
	public class ServerLocation
	{
		[ReflectorProperty("name")]
		public string Name = string.Empty;

		[ReflectorProperty("url")]
		public string Url = string.Empty;

		[ReflectorProperty("allowForceBuild", Required=false)]
		public bool AllowForceBuild = true;
		
		[ReflectorProperty("allowStartStopBuild", Required=false)]
		public bool AllowStartStopBuild = true;
	}
}