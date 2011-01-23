namespace ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IPhysicalApplicationPathProvider
	{
        /// <summary>
        /// Gets the full path for.	
        /// </summary>
        /// <param name="appRelativePath">The app relative path.</param>
        /// <returns></returns>
        /// <remarks></remarks>
	    string GetFullPathFor(string appRelativePath);
	}
}
