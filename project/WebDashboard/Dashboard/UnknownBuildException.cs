using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class UnknownBuildException : CruiseControlException
	{
		private readonly Build build;

		public UnknownBuildException(Build build) : base()
		{
			this.build = build;
		}

		public Build Build
		{
			get { return build; }
		}
	}
}
