
namespace ThoughtWorks.CruiseControl.WebDashboard.config
{
	public class ServerSpecification
	{
		private readonly string name;
		private readonly string url;

		public ServerSpecification(string name, string url)
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
