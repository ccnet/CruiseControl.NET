using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class Build
	{
		private readonly string projectName;
		private readonly string serverName;
		private readonly string name;
		private readonly string log;

		public Build(string name, string log, string serverName, string projectName)
		{
			this.log = log;
			this.name = name;
			this.serverName = serverName;
			this.projectName = projectName;
		}

		public string Name
		{
			get { return name; }
		}

		public string Log
		{
			get { return log; }
		}

		public string ProjectName
		{
			get { return projectName; }
		}

		public string ServerName
		{
			get { return serverName; }
		}

		public bool IsSuccessful
		{
			get { return LogFileUtil.IsSuccessful(Name); }
		}
	}
}
