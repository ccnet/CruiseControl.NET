using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class Build
	{
		private readonly IBuildSpecifier buildSpecifier;
		private readonly string buildLogLocation;
		private readonly string log;

		public Build(IBuildSpecifier buildSpecifier, string log, string buildLogLocation)
		{
			this.log = log;
			this.buildLogLocation = buildLogLocation;
			this.buildSpecifier = buildSpecifier;
		}

		public string Log
		{
			get { return log; }
		}

		public string BuildLogLocation
		{
			get { return buildLogLocation; }
		}

		public IBuildSpecifier BuildSpecifier
		{
			get { return buildSpecifier; }
		}

		public bool IsSuccessful
		{
			get { return new LogFile(buildSpecifier.BuildName).Succeeded; }
		}
	}
}
