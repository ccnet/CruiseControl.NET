using System;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProjectPlugin
{
	public class AddProjectPageModel
	{
		private readonly IProjectSerializer projectSerializer;
		private readonly ICruiseManagerWrapper cruiseManagerWrapper;
		private string selectedServerName = "";
		private string projectName = "";
		private string repositoryRoot = "";
		private string builderExecutable = "";
		private string builderBaseDirectory = "";
		private string builderBuildArgs = "";

		public AddProjectPageModel(ICruiseManagerWrapper cruiseManagerWrapper, IProjectSerializer projectSerializer)
		{
			this.cruiseManagerWrapper = cruiseManagerWrapper;
			this.projectSerializer = projectSerializer;
		}

		public string[] ServerNames
		{
			get
			{
				return cruiseManagerWrapper.GetServerNames();
			}
		}

		public string Save()
		{
			Project project = new Project();
			project.Name = projectName;

			string message = "";
			try
			{
				cruiseManagerWrapper.AddProject(selectedServerName,  projectSerializer.Serialize(project));	
				message = "Project saved successfully";
			}
			catch (Exception e)
			{
				message = "Failed to Save project - Exception message was : " + e.Message;
			}
			
			return message;
		}

		public string SelectedServerName
		{
			set { selectedServerName = value; }
		}

		public string ProjectName
		{
			set { projectName = value; }
		}

		public string RepositoryRoot
		{
			set { repositoryRoot = value; }
		}

		public string BuilderExecutable
		{
			set { builderExecutable = value; }
		}

		public string BuilderBaseDirectory
		{
			set { builderBaseDirectory = value; }
		}

		public string BuilderBuildArgs
		{
			set { builderBuildArgs = value; }
		}
	}
}
