
namespace ThoughtWorks.CruiseControl.WebDashboard.Config
{
	public class ServerLocation
	{
		private readonly string name;
		private readonly string url;

		public ServerLocation(string name, string url)
		{
			this.name = name;
			this.url = url;
		}

		public string Name
		{
			get { return name; }
		}

		public string Url
		{
			get { return url; }
		}
	}
}
