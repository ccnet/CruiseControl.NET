using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class CruiseManagerBuildRetriever : IBuildRetriever
	{
		private readonly ICruiseManagerWrapper cruiseManagerWrapper;

		public CruiseManagerBuildRetriever(ICruiseManagerWrapper cruiseManagerWrapper)
		{
			this.cruiseManagerWrapper = cruiseManagerWrapper;
		}

		public Build GetBuild(string serverName, string projectName, string buildName)
		{
			return new Build(buildName, cruiseManagerWrapper.GetLog(serverName, projectName, buildName), serverName, projectName);
		}
	}
}
