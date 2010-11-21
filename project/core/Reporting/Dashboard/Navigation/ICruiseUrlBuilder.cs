namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
    /// <summary>
    /// 	
    /// </summary>
	public interface ICruiseUrlBuilder
	{
        /// <summary>
        /// Builds the server URL.	
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="serverSpecifier">The server specifier.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string BuildServerUrl(string action, IServerSpecifier serverSpecifier);
        /// <summary>
        /// Builds the server URL.	
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="serverSpecifier">The server specifier.</param>
        /// <param name="queryString">The query string.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string BuildServerUrl(string action, IServerSpecifier serverSpecifier, string queryString);
        /// <summary>
        /// Builds the project URL.	
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="projectSpecifier">The project specifier.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string BuildProjectUrl(string action, IProjectSpecifier projectSpecifier);
        /// <summary>
        /// Builds the build URL.	
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="buildSpecifier">The build specifier.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string BuildBuildUrl(string action, IBuildSpecifier buildSpecifier);
        /// <summary>
        /// Gets or sets the extension.	
        /// </summary>
        /// <value>The extension.</value>
        /// <remarks></remarks>
		string Extension { set; get; }
        /// <summary>
        /// Gets the inner builder.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        IUrlBuilder InnerBuilder { get; }
	}
}
