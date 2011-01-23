
using System;
namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
    /// <summary>
    /// 	
    /// </summary>
	public class DefaultBuildSpecifier : IBuildSpecifier
	{
		private readonly string buildName;
		private readonly IProjectSpecifier projectSpecifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBuildSpecifier" /> class.	
        /// </summary>
        /// <param name="projectSpecifier">The project specifier.</param>
        /// <param name="buildName">Name of the build.</param>
        /// <remarks></remarks>
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
			if ((buildName != null && buildName.Length == 0))
			{
				throw new CruiseControlException("Build Specifier cannot be instantiated with an empty build name");
			}
			this.projectSpecifier = projectSpecifier;
			this.buildName = buildName;
		}

        /// <summary>
        /// Gets the name of the build.	
        /// </summary>
        /// <value>The name of the build.</value>
        /// <remarks></remarks>
		public string BuildName
		{
			get { return buildName; }
		}

        /// <summary>
        /// Gets the project specifier.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public IProjectSpecifier ProjectSpecifier
		{
			get { return projectSpecifier; }
		}

        /// <summary>
        /// Equalses the specified obj.	
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public override bool Equals(object obj)
		{
            DefaultBuildSpecifier other = obj as DefaultBuildSpecifier;
            if (other != null)
			{
				return (this.BuildName == other.BuildName && this.ProjectSpecifier.Equals(other.ProjectSpecifier));
			}
			return false;
		}

        /// <summary>
        /// Gets the hash code.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

        /// <summary>
        /// Toes the string.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
		public override string ToString()
		{
			return projectSpecifier + ", " + buildName;
		}
	}
}
