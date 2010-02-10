namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    using System;
    using System.Collections;
    using System.Web;
    using System.Web.Caching;
    using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;
    using System.Text;
    using System.IO;

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
        /// <param name="taskTypes">The task types.</param>
        /// <returns>The transformed content.</returns>
        public string Transform(IBuildSpecifier buildSpecifier, string[] transformerFileNames, Hashtable xsltArgs, string sessionToken, string[] taskTypes)
		{
            var log = this.RetrieveLogData(buildSpecifier, sessionToken);

            if ((taskTypes != null) && (taskTypes.Length > 0))
            {
                // HACK: This is rebuilding the required parts of the log in memory, this could be done by writing to a temporary file on disk - this would
                // also allow for caching the files on the web server and reduce the amount of network traffic
                // HACK: We are currently removing the closing tag and adding it later - this assumes that the build log will always end with the same tag!
                var buildLog = new BuildLog(log);
                var logBuilder = new StringBuilder(log);
                logBuilder.Remove(logBuilder.Length - 17, 16);
                foreach (var taskType in taskTypes)
                {
                    this.RetrieveLogData(buildSpecifier, sessionToken, buildLog, taskType, logBuilder);
                }

                logBuilder.Append("</cruisecontrol>");
                log = logBuilder.ToString();
            }

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
            var logData = cache[logKey] as SynchronisedData<string>;
            if (logData == null)
            {
                // Add the new log data and load it
                logData = new SynchronisedData<string>();
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

            return logData.Data;
        }

        /// <summary>
        /// Retrieves the log data for a specified task type.
        /// </summary>
        /// <param name="buildSpecifier">The build specifier.</param>
        /// <param name="sessionToken">The session token.</param>
        /// <param name="buildLog">The build log.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="builder">The builder to use.</param>
        private void RetrieveLogData(IBuildSpecifier buildSpecifier, string sessionToken, BuildLog buildLog, string taskType, StringBuilder builder)
        {
            // TODO: Cache the output so subsequent fetches are faster
            var outputs = buildLog.FindOutputOfTaskType(taskType);
            foreach (var output in outputs)
            {
                // TODO: Need to figure out a way of reading directly from the stream and putting it into the builder - because unicode characters can 
                // consist of multiple bytes a direct byte-by-byte transfer won't work
                var stream = new MemoryStream();
                var isXml = (output.DataType == "text/xml") || (output.DataType == "data/xml");
                this.buildRetriever.GetFile(buildSpecifier, sessionToken, output.FileName, stream);
                if (!isXml)
                {
                    builder.Append("<data type=\"" + taskType + "\"><![CDATA[");
                }

                // HACK: This will transfer the stream into the builder - however if the file is massive we will still have memory issues!
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(stream))
                {
                    builder.Append(reader.ReadToEnd());
                }

                if (!isXml)
                {
                    builder.Append("]]></data>");
                }
            }
        }
        #endregion
        #endregion
	}
}
