using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class UnknownBuildException : CruiseControlException
	{
		private readonly IBuildSpecifier buildSpecifier;

		public UnknownBuildException(IBuildSpecifier buildSpecifier) : base()
		{
			this.buildSpecifier = buildSpecifier;
		}

		public IBuildSpecifier BuildSpecifier
		{
			get { return buildSpecifier; }
		}
	}
}
