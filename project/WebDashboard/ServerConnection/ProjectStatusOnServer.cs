using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

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
