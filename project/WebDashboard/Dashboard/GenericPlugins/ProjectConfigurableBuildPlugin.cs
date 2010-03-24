namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.GenericPlugins
{
    using System.Collections.Generic;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

    public abstract class ProjectConfigurableBuildPlugin : IBuildPlugin
	{
        private List<string> includedProjects = new List<string>();
        private List<string> excludedProjects = new List<string>();

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

        /// <summary>
        /// The projects to include in this plug-in.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        /// <remarks>
        /// This is currently not implemented.
        /// </remarks>
        [ReflectorProperty("includedProjects", Required = false)]
		public string[] IncludedProjects
		{
			get { return includedProjects.ToArray(); }
			set
			{
				CheckOtherPropertyNotAlreadySet(value, excludedProjects);
                includedProjects = new List<string>(value);
			}
		}

        /// <summary>
        /// The projects to exclude from this plug-in.
        /// </summary>
        /// <version>1.0</version>
        /// <default>None</default>
        /// <remarks>
        /// This is currently not implemented.
        /// </remarks>
        [ReflectorProperty("excludedProjects", Required = false)]
		public string[] ExcludedProjects
		{
			get { return excludedProjects.ToArray(); }
			set
			{
				CheckOtherPropertyNotAlreadySet(value, includedProjects);
                excludedProjects = new List<string>(value);
			}
		}

        private void CheckOtherPropertyNotAlreadySet(string[] newList, List<string> otherList)
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
