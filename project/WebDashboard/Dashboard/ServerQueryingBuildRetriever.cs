using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using System.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ServerQueryingBuildRetriever : IBuildRetriever
	{
		private readonly ICruiseManagerWrapper cruiseManagerWrapper;

		public ServerQueryingBuildRetriever(ICruiseManagerWrapper cruiseManagerWrapper)
		{
			this.cruiseManagerWrapper = cruiseManagerWrapper;
		}

        public Build GetBuild(IBuildSpecifier buildSpecifier, string sessionToken)
		{
			string log = cruiseManagerWrapper.GetLog(buildSpecifier, sessionToken);

			return new Build(buildSpecifier, log);
		}

        public void GetFile(IBuildSpecifier buildSpecifier, string sessionToken, string fileName, Stream outputStream)
        {
            cruiseManagerWrapper.RetrieveFileTransfer(buildSpecifier, fileName, sessionToken, outputStream);
        }
    }
}
