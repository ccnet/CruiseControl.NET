using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class Build
	{
		private readonly IBuildSpecifier buildSpecifier;
		private readonly string log;

		public Build(IBuildSpecifier buildSpecifier, string log)
		{
			this.log = log;
			this.buildSpecifier = buildSpecifier;
		}

		public string Log
		{
			get { return log; }
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
