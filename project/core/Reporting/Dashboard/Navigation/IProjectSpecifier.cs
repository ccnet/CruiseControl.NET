namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IProjectSpecifier
	{
        /// <summary>
        /// Gets the name of the project.	
        /// </summary>
        /// <value>The name of the project.</value>
        /// <remarks></remarks>
		string ProjectName { get; }

        /// <summary>
        /// Gets the server specifier.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		IServerSpecifier ServerSpecifier  { get; }
	}
}
