using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class AddEditProjectModel
	{
		private string saveActionName;
		private readonly Project project;
		private readonly string selectedServer;
		private readonly string[] servers;
		private string status;
		private bool isAdd;

		public AddEditProjectModel(Project project, string selectedServer, string[] servers) 
		{
			this.project = project;
			this.selectedServer = selectedServer;
			this.servers = servers;
			this.status = "";
			this.isAdd = true;
			this.saveActionName = "";
		}

		public string Status
		{
			get { return status; }
			set { status = value; }
		}

		public bool IsAdd
		{
			get { return isAdd; }
			set { isAdd = value; }
		}

		public string SaveActionName
		{
			get { return saveActionName; }
			set { saveActionName = value; }
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
