
namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
    /// <summary>
    /// 	
    /// </summary>
	public class DefaultProjectSpecifier : IProjectSpecifier
	{
		private readonly string projectName;
		private readonly IServerSpecifier serverSpecifer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultProjectSpecifier" /> class.	
        /// </summary>
        /// <param name="serverSpecifier">The server specifier.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <remarks></remarks>
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

        /// <summary>
        /// Gets the name of the project.	
        /// </summary>
        /// <value>The name of the project.</value>
        /// <remarks></remarks>
		public string ProjectName
		{
			get { return projectName; }
		}

        /// <summary>
        /// Gets the server specifier.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public IServerSpecifier ServerSpecifier
		{
			get { return serverSpecifer; }
		}

        /// <summary>
        /// Equalses the specified obj.	
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public override bool Equals(object obj)
		{
            DefaultProjectSpecifier other = obj as DefaultProjectSpecifier;
            if (other != null)
			{
				return (this.ProjectName == other.ProjectName && this.ServerSpecifier.Equals(other.ServerSpecifier));
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
			return ServerSpecifier + ", " + ProjectName;
		}
	}
}
