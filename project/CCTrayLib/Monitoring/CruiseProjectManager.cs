using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// Allows access to the state of a single cruise control project 
	/// </summary>
	public class CruiseProjectManager : ICruiseProjectManager
	{
		private readonly ICruiseManager manager;
		private readonly string projectName;

		public CruiseProjectManager(ICruiseManager server, string projectName)
		{
			this.manager = server;
			this.projectName = projectName;
		}

		public void ForceBuild()
		{
			manager.Request(ProjectName, new IntegrationRequest(BuildCondition.ForceBuild, Environment.UserName));
		}

		public void FixBuild()
		{
			manager.SendMessage(ProjectName, new Message(string.Format("{0} is fixing the build.", Environment.UserName)));
		}

		public ProjectStatus ProjectStatus
		{
			get
			{
				ProjectStatus[] statuses = manager.GetProjectStatus();
				foreach (ProjectStatus status in statuses)
				{
					if (status.Name == ProjectName)
						return status;
				}
				throw new ApplicationException("Project '" + projectName + "' not found on server");
			}
		}

		public string ProjectName
		{
			get { return projectName; }
		}
	}
}