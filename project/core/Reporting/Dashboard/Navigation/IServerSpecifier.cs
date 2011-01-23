namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IServerSpecifier
	{
        /// <summary>
        /// Gets the name of the server.	
        /// </summary>
        /// <value>The name of the server.</value>
        /// <remarks></remarks>
		string ServerName { get; }
        /// <summary>
        /// Gets the allow force build.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		bool AllowForceBuild { get; }
        /// <summary>
        /// Gets the allow start stop build.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		bool AllowStartStopBuild { get; }
	}
}