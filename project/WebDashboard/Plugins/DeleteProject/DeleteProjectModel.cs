namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject
{
	public class DeleteProjectModel
	{
		private readonly bool purgeWorkingDirectory;
		private readonly bool purgeArtifactDirectory;
		private readonly bool purgeSourceControlEnvironment;
		private readonly bool allowDelete;
		private readonly string message;
		private readonly string projectName;
		private readonly string serverName;

		public DeleteProjectModel(string serverName, string projectName, string message, bool allowDelete,
			bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
			this.serverName = serverName;
			this.projectName = projectName;
			this.message = message;
			this.allowDelete = allowDelete;
			this.purgeSourceControlEnvironment = purgeSourceControlEnvironment;
			this.purgeArtifactDirectory = purgeArtifactDirectory;
			this.purgeWorkingDirectory = purgeWorkingDirectory;
		}

		public bool AllowDelete
		{
			get { return allowDelete; }
		}

		public string Message
		{
			get { return message; }
		}

		public string ProjectName
		{
			get { return projectName; }
		}

		public string ServerName
		{
			get { return serverName; }
		}

		public bool PurgeWorkingDirectory
		{
			get { return purgeWorkingDirectory; }
		}

		public bool PurgeArtifactDirectory
		{
			get { return purgeArtifactDirectory; }
		}

		public bool PurgeSourceControlEnvironment
		{
			get { return purgeSourceControlEnvironment; }
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is DeleteProjectModel)
			{
				DeleteProjectModel other = (DeleteProjectModel) obj;
				return (
					this.ServerName == other.ServerName
					&& this.ProjectName == other.ProjectName
					&& this.Message == other.Message
					&& this.AllowDelete == other.AllowDelete
					&& this.PurgeArtifactDirectory == other.PurgeArtifactDirectory
					&& this.PurgeWorkingDirectory == other.PurgeWorkingDirectory
					&& this.PurgeSourceControlEnvironment == other.PurgeSourceControlEnvironment
				);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ServerName.GetHashCode() + ProjectName.GetHashCode() + Message.GetHashCode() + AllowDelete.GetHashCode()
				+ PurgeArtifactDirectory.GetHashCode() + PurgeWorkingDirectory.GetHashCode() + PurgeSourceControlEnvironment.GetHashCode();
		}
	}
}
