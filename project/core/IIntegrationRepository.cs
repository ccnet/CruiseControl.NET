namespace ThoughtWorks.CruiseControl.Core
{
    using System.Collections.Generic;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    /// <summary>
    /// 	
    /// </summary>
    public interface IIntegrationRepository
    {
        /// <summary>
        /// Gets the build log.	
        /// </summary>
        /// <param name="buildName">Name of the build.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        string GetBuildLog(string buildName);
        /// <summary>
        /// Gets the build names.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        string[] GetBuildNames();
        /// <summary>
        /// Gets the most recent build names.	
        /// </summary>
        /// <param name="buildCount">The build count.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        string[] GetMostRecentBuildNames(int buildCount);
        /// <summary>
        /// Gets the name of the latest build.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        string GetLatestBuildName();

        /// <summary>
        /// Retrieves some summaries for the project.
        /// </summary>
        /// <param name="start">The first summary to retrieve. This starts from the last summary generated.</param>
        /// <param name="count">The number of summaries to retrieve.</param>
        /// <returns>
        /// The summaries.
        /// </returns>
        IList<BuildSummary> GetSummaries(int start, int count);
    }
}