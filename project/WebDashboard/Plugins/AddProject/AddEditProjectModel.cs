using System.Collections;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject
{
	public class AddEditProjectModel
	{
		private string saveActionName;
		private readonly Project project;
		private string selectedServer;
		private readonly IServerSpecifier[] serverSpecifiers;
		private string status;
		private bool isAdd;

		public AddEditProjectModel(Project project, string selectedServer, IServerSpecifier[] serverSpecifiers) 
		{
			this.project = project;
			this.selectedServer = selectedServer;
			this.serverSpecifiers = serverSpecifiers;
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
			set { selectedServer = value; }
		}

		public string[] ServerNames
		{
			get 
			{
				ArrayList serverNames = new ArrayList() ;
				foreach (IServerSpecifier specifier in serverSpecifiers)
				{
					serverNames.Add(specifier.ServerName);
				}
				return (string[]) serverNames.ToArray(typeof (string));
			}
		}
	}
}
