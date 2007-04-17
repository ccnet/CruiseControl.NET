using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public class CruiseServerSnapshotOnServer
	{
		private readonly IServerSpecifier serverSpecifier;
        private readonly CruiseServerSnapshot cruiseServerSnapshot;

        public CruiseServerSnapshotOnServer(CruiseServerSnapshot cruiseServerSnapshot, IServerSpecifier serverSpecifier)
		{
			this.serverSpecifier = serverSpecifier;
            this.cruiseServerSnapshot = cruiseServerSnapshot;
		}

		public IServerSpecifier ServerSpecifier
		{
			get { return serverSpecifier; }
		}

        public CruiseServerSnapshot CruiseServerSnapshot
		{
            get { return cruiseServerSnapshot; }
		}
	}
}