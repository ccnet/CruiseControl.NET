#pragma warning disable 1591
namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
	public class DefaultBuildSpecifier : IBuildSpecifier
	{
		private readonly string buildName;
		private readonly IProjectSpecifier projectSpecifier;

		public DefaultBuildSpecifier(IProjectSpecifier projectSpecifier, string buildName)
		{
			if (projectSpecifier == null)
			{
				throw new CruiseControlException("Build Specifier cannot be instantiated with a null Project Specifier");
			}
			if (buildName == null)
			{
				throw new CruiseControlException("Build Specifier cannot be instantiated with a null build name");
			}
			if (buildName == string.Empty)
			{
				throw new CruiseControlException("Build Specifier cannot be instantiated with an empty build name");
			}
			this.projectSpecifier = projectSpecifier;
			this.buildName = buildName;
		}

		public string BuildName
		{
			get { return buildName; }
		}

		public IProjectSpecifier ProjectSpecifier
		{
			get { return projectSpecifier; }
		}

		public override bool Equals(object obj)
		{
			if (obj is DefaultBuildSpecifier)
			{
				DefaultBuildSpecifier other = obj as DefaultBuildSpecifier;
				return (this.BuildName == other.BuildName && this.ProjectSpecifier.Equals(other.ProjectSpecifier));
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			return projectSpecifier.ToString() + ", " + buildName;
		}
	}
}
