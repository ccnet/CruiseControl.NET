using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class Build
	{
		private readonly string url;
		private readonly string name;
		private readonly string log;

		public Build(string name, string log, string Url)
		{
			this.log = log;
			this.name = name;
			this.url = Url;
		}

		public string Name
		{
			get { return name; }
		}

		public string Log
		{
			get { return log; }
		}

		public string Url
		{
			get { return url; }
		}

		public bool IsSuccessful
		{
			get { return LogFileUtil.IsSuccessful(Name); }
		}
	}
}
