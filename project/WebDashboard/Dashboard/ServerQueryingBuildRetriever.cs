using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ServerQueryingBuildRetriever : IBuildRetriever
	{
		private readonly ICruiseManagerWrapper cruiseManagerWrapper;

		public ServerQueryingBuildRetriever(ICruiseManagerWrapper cruiseManagerWrapper)
		{
			this.cruiseManagerWrapper = cruiseManagerWrapper;
		}

		public Build GetBuild(IBuildSpecifier buildSpecifier)
		{
			string log = cruiseManagerWrapper.GetLog(buildSpecifier);

			return new Build(buildSpecifier, log);
		}
	}
}
