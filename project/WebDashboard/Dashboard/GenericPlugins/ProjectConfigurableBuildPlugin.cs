using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins
{
	public abstract class ProjectConfigurableBuildPlugin : IBuildPlugin
	{
		private ArrayList includedProjects = new ArrayList();
		private ArrayList excludedProjects = new ArrayList();

		public bool IsDisplayedForProject(IProjectSpecifier project)
		{
			string projectName = project.ProjectName;

			if (includedProjects.Count > 0)
			{
				return includedProjects.Contains(project.ProjectName);
			}
			else if (excludedProjects.Count > 0)
			{
				return !excludedProjects.Contains(projectName);
			}
			else
			{
				return true;
			}
		}

		[ReflectorArray("includedProjects", Required=false)]
		public string[] IncludedProjects
		{
			get { return (string[]) includedProjects.ToArray(typeof (string)); }
			set
			{
				CheckOtherPropertyNotAlreadySet(value, excludedProjects);
				includedProjects = new ArrayList(value);
			}
		}

		[ReflectorArray("excludedProjects", Required=false)]
		public string[] ExcludedProjects
		{
			get { return (string[]) excludedProjects.ToArray(typeof (string)); }
			set
			{
				CheckOtherPropertyNotAlreadySet(value, includedProjects);
				excludedProjects = new ArrayList(value);
			}
		}

		private void CheckOtherPropertyNotAlreadySet(string[] newList, ArrayList otherList)
		{
			if (otherList.Count > 0 && newList.Length > 0)
			{
				throw new CruiseControlException("Invalid configuration - cannot set both Included and Excluded Projects for a Build Plugin");
			}
		}

		public abstract INamedAction[] NamedActions { get; }
		public abstract string LinkDescription { get; }
	}
}
