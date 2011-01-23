namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IUrlBuilder
	{
        /// <summary>
        /// Builds the URL.	
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string BuildUrl(string action);
        /// <summary>
        /// Builds the URL.	
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="queryString">The query string.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string BuildUrl(string action, string queryString);
        /// <summary>
        /// Builds the URL.	
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="queryString">The query string.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		string BuildUrl(string action, string queryString, string path);

        /// <summary>
        /// Gets or sets the extension.	
        /// </summary>
        /// <value>The extension.</value>
        /// <remarks></remarks>
		string Extension { set; get; }
	}
}
