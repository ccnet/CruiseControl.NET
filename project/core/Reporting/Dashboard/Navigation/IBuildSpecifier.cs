namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IBuildSpecifier
	{
        /// <summary>
        /// Gets the name of the build.	
        /// </summary>
        /// <value>The name of the build.</value>
        /// <remarks></remarks>
		string BuildName { get; }

        /// <summary>
        /// Gets the project specifier.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		IProjectSpecifier ProjectSpecifier  { get; }
	}
}
