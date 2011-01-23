using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public class ProjectStatusOnServer
	{
		private readonly IServerSpecifier serverSpecifier;
		private readonly ProjectStatus projectStatus;

		public IServerSpecifier ServerSpecifier
		{
			get { return serverSpecifier; }
		}

		public ProjectStatus ProjectStatus
		{
			get { return projectStatus; }
		}

		public ProjectStatusOnServer(ProjectStatus projectStatus, IServerSpecifier serverSpecifier)
		{
			this.serverSpecifier = serverSpecifier;
			this.projectStatus = projectStatus;
		}
	}
}