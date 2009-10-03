namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    using System;
    using System.Collections;
    using System.Web;
    using System.Web.Caching;
    using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
    using ThoughtWorks.CruiseControl.Core.Util;

    public class BuildRequestTransformer 
        : IBuildLogTransformer
	{
		private readonly IMultiTransformer transformer;
		private readonly IBuildRetriever buildRetriever;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildRequestTransformer"/> class.
        /// </summary>
        /// <param name="buildRetriever">The build retriever.</param>
        /// <param name="transformer">The transformer.</param>
		public BuildRequestTransformer(IBuildRetriever buildRetriever, IMultiTransformer transformer)
		{
			this.buildRetriever = buildRetriever;
			this.transformer = transformer;
		}
        #endregion

        #region Public methods
        #region Transform()
        /// <summary>
        /// Transforms the specified build specifier.
        /// </summary>
        /// <param name="buildSpecifier">The build specifier.</param>
        /// <param name="transformerFileNames">The transformer file names.</param>
        /// <param name="xsltArgs">The XSLT args.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns>The transformed content.</returns>
        public string Transform(IBuildSpecifier buildSpecifier, string[] transformerFileNames, Hashtable xsltArgs, string sessionToken)
		{
            var log = this.RetrieveLogData(buildSpecifier, sessionToken);
			return transformer.Transform(log, transformerFileNames, xsltArgs);
		}
        #endregion
        #endregion

        #region Private methods
        #region RetrieveLogData()
        /// <summary>
        /// Retrieves the log data.
        /// </summary>
        /// <param name="buildSpecifier">The build specifier to retrieve the log for.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <returns>The data for the log.</returns>
        /// <exception cref="ApplicationException">Thrown if the data for the log could not be retrieved.</exception>
        private string RetrieveLogData(IBuildSpecifier buildSpecifier, string sessionToken)
        {
            var cache = HttpRuntime.Cache;

            // Generate the log and report keys
            var logKey = buildSpecifier.ProjectSpecifier.ServerSpecifier.ServerName +
                buildSpecifier.ProjectSpecifier.ProjectName +
                buildSpecifier.BuildName +
                (sessionToken ?? string.Empty);

            // Check if the log has already been cached
            var logData = cache[logKey] as SynchronisedData;
            if (logData == null)
            {
                // Add the new log data and load it
                logData = new SynchronisedData();
                cache.Add(
                    logKey,
                    logData,
                    null,
                    Cache.NoAbsoluteExpiration,
                    new TimeSpan(1, 0, 0),
                    CacheItemPriority.AboveNormal,
                    null);
                logData.LoadData(() =>
                {
                    var buildLog = buildRetriever.GetBuild(buildSpecifier, sessionToken).Log;
                    return buildLog;
                });
            }
            else
            {
                // Wait for the data to load
                logData.WaitForLoad(10000);
            }

            // Raise an error if there is no log data
            if (logData.Data == null)
            {
                cache.Remove(logKey);
                throw new ApplicationException("Unable to retrieve log data");
            }

            return logData.Data as string;
        }
        #endregion
        #endregion
	}
}
