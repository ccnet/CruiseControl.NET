using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultProjectSpecifier : IProjectSpecifier
	{
		private readonly string projectName;
		private readonly IServerSpecifier serverSpecifer;

		public DefaultProjectSpecifier(IServerSpecifier serverSpecifier, string projectName)
		{
			if (serverSpecifier == null)
			{
				throw new CruiseControlException("Project Specifier cannot be instantiated with a null Server Specifier");
			}
			if (projectName == null)
			{
				throw new CruiseControlException("Project Specifier cannot be instantiated with a null project name");
			}
			if (projectName == string.Empty)
			{
				throw new CruiseControlException("Project Specifier cannot be instantiated with an empty project name");
			}
			this.serverSpecifer = serverSpecifier;
			this.projectName = projectName;
		}

		public string ProjectName
		{
			get { return projectName; }
		}

		public IServerSpecifier ServerSpecifier
		{
			get { return serverSpecifer; }
		}

		public override bool Equals(object obj)
		{
			if (obj is DefaultProjectSpecifier)
			{
				DefaultProjectSpecifier other = obj as DefaultProjectSpecifier;
				return (this.ProjectName == other.ProjectName && this.ServerSpecifier.Equals(other.ServerSpecifier));
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			return ServerSpecifier.ToString() + ", " + ProjectName;
		}
	}
}
