namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject
{
	public class DeleteProjectModel
	{
		private readonly bool allowDelete;
		private readonly string message;
		private readonly string projectName;
		private readonly string serverName;

		public DeleteProjectModel(string serverName, string projectName, string message, bool allowDelete)
		{
			this.serverName = serverName;
			this.projectName = projectName;
			this.message = message;
			this.allowDelete = allowDelete;
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
				);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ServerName.GetHashCode() + ProjectName.GetHashCode() + Message.GetHashCode() + AllowDelete.GetHashCode();
		}
	}
}
