using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.WebDashboard.Configuration
{
	[ReflectorType("server")]
	public class ServerLocation
	{
		private string name = "";
		private string url = "";

		[ReflectorProperty("name")]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[ReflectorProperty("url")]
		public string Url
		{
			get { return url; }
			set { url = value; }
		}
	}
}
