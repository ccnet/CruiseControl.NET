using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class AddProjectModel
	{
		private bool allowSave;
		private readonly Project project;
		private readonly string selectedServer;
		private readonly string[] servers;
		private string status;

		public AddProjectModel(Project project, string selectedServer, string[] servers) : this (project, selectedServer, servers, "", true) { }

		public AddProjectModel(Project project, string selectedServer, string[] servers, string status, bool allowSave)
		{
			this.project = project;
			this.selectedServer = selectedServer;
			this.servers = servers;
			this.status = status;
			this.allowSave = allowSave;
		}

		public string Status
		{
			get { return status; }
			set { status = value; }
		}

		public bool AllowSave
		{
			get { return allowSave; }
			set { allowSave = value; }
		}

		public Project Project
		{
			get { return project; }
		}

		public string SelectedServerName
		{
			get { return selectedServer; }
		}

		public string[] ServerNames
		{
			get { return servers; }
		}
	}
}
