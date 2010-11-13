namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Tasks;

    /// <summary>
    /// <para>
    /// The publisher can be used to collect and update statistics for each build in a file. Some of the statistics which would be collected
    /// are build durations and test count.
    /// At the minimal, the publisher can be configured with just an empty &lt;statistics /&gt; element in the publishers section. This would
    /// pick up some default statistics for capturing during the build process.
    /// </para>
    /// <para type="info">
    /// Statistics publisher must come after any File Merge tasks in the publishers section, in case you want to collect statistics from
    /// merged files.
    /// </para>
    /// <para>
    /// The task will generate a statistics.csv and report.xml file in the artifact directory.
    /// </para>
    /// </summary>
    /// <title> Statistics Publisher </title>
    /// <version>1.0</version>
    /// <remarks>
    /// <para>
    /// If you want to specify your own, or override the default statistics, it is possible to do so by supplying the name and xpath for the
    /// statistics and the corresponding location in the build log to pick the data from.
    /// </para>
    /// <code>
    /// &lt;statistics&gt;
    /// &lt;statisticList&gt;
    /// &lt;statistic name="metric_name" xpath="xpath expression"/&gt;
    /// &lt;firstMatch name="metric_name" xpath="xpath expression" /&gt;
    /// &lt;/statisticList&gt;
    /// &lt;/statistics&gt;
    /// </code>
    /// <para>
    /// It is also possible to optionally configure the statistics publisher to generate charts for any metric against different builds, and
    /// to even remove them altogether. This feature has been added in version 1.3:
    /// </para>
    /// <code>
    /// &lt;statistics&gt;
    /// &lt;statisticList&gt;
    /// &lt;statistic name="metric_name" xpath="xpath expression" generateGraph="true" include="true"/&gt;
    /// &lt;firstMatch name="metric_name" xpath="xpath expression" include="false"/&gt;
    /// &lt;/statisticList&gt;
    /// &lt;/statistics&gt;
    /// </code>
    /// <para>
    /// For the statistics configured with 'generateGraph="true"', a graph is generated with different builds on x-axis and the configured
    /// metric on y-axis in the artifacts directory named as &lt;statistic name&gt;.png. This chart would still be a very basic representation.
    /// For now at least, exporting the report to Excel for charting/analyis might be a better option.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;statistics /&gt;
    /// </code>
    /// </example>
    [ReflectorType("statistics")]
    public class StatisticsPublisher 
        : TaskBase
    {
        /// <summary>
        /// The location of the CSV statistics file, relative to the project artifacts directory.
        /// </summary>
        public const string CsvFileName = "statistics.csv";
        /// <summary>
        /// The location of the XML statistics file, relative to the project artifacts directory.
        /// </summary>
        public const string XmlFileName = "report.xml";

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsPublisher"/> class.
        /// </summary>
        public StatisticsPublisher()
        {
            this.ConfiguredStatistics = new Statistic[0];
        }

        /// <summary>
        /// The list of statistics to be included in the build.
        /// </summary>
        /// <remarks>
        /// There is a default list of statistics to be included, and this list
        /// adds additional statistics to the build.  Any statistic defined with
        /// <b>include=false</b> will be omitted, even if it is in the
        /// default list.
        /// </remarks>
        /// <default>None</default>
        /// <version>1.0</version>
        [ReflectorProperty("statisticList", Required = false)]
        public StatisticBase[] ConfiguredStatistics { get; set; }

        #region ITask Members

        /// <summary>
        /// Publish the statistics for this build.
        /// </summary>
        /// <param name="integrationResult">The results of the build.</param>
        protected override bool Execute(IIntegrationResult integrationResult)
        {
            StatisticsBuilder builder = new StatisticsBuilder();
            for (int i = 0; i < ConfiguredStatistics.Length; i++)
            {
                StatisticBase statistic = ConfiguredStatistics[i];
                builder.Add(statistic);     // Note: This may actually remove the statistic if include=false.
            }
            StatisticsResults stats = builder.ProcessBuildResults(integrationResult);

            UpdateXmlFile(stats, integrationResult);
            
            //commented out because this does not work
            //ChartGenerator(builder.Statistics).Process(xmlDocument, integrationResult.ArtifactDirectory);
            
            UpdateCsvFile(stats, builder.Statistics, integrationResult);

            return true;
        }

        #endregion

        /// <summary>
        /// Write the specified collection of statistics to the XML
        /// statistics file, creating it if it does not already exist
        /// </summary>
        /// <param name="stats">The collection of statistics.</param>
        /// <param name="integrationResult">The build for which the
        /// statistics were collected.</param>
        /// <remarks>
        /// The XML document takes the following form:
        ///     &lt;statistics&gt;
        ///         &lt;integration build-label="label" status="status"
        ///                 day="day_of_month" month="month_name" year="year"&gt;
        ///             &lt;statistic name="name"&gt;
        ///                 value
        ///             &lt;/statistic&gt;
        ///         &lt;/integration&gt;
        ///     &lt;/statistics&gt;
        /// </remarks>
        private static void UpdateXmlFile(IEnumerable<StatisticResult> stats,
                                                 IIntegrationResult integrationResult)
        {
            Directory.CreateDirectory(integrationResult.ArtifactDirectory);

            // make an xml element of the current integration
            System.Text.StringBuilder integration = new System.Text.StringBuilder();
            DateTime now = DateTime.Now;

			integration.AppendFormat("<integration build-label=\"{0}\" status=\"{1}\" day=\"{2}\" month=\"{3}\" year=\"{4}\">",
										integrationResult.Label,
										integrationResult.Status.ToString(),
										now.Day.ToString(), now.ToString("MMM", CultureInfo.InvariantCulture), now.Year.ToString());

            integration.AppendLine(ToXml(stats));

            integration.Append("</integration>");

            // append to the statistics file
            string lastFile = XmlStatisticsFile(integrationResult);
            System.IO.FileStream fs = new System.IO.FileStream(lastFile, System.IO.FileMode.Append);
            fs.Seek(0, System.IO.SeekOrigin.End);

            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
            sw.WriteLine(integration.ToString());

            sw.Flush();
            fs.Flush();

            sw.Close();
            fs.Close();

        }

        /// <summary>
        /// Add the specified collection of statistics to the root of the specified XML document.
        /// </summary>
        /// <param name="stats">The statistics to add.</param>
        /// <returns>The added child element.</returns>
        /// <remarks>
        /// The XML added to the root of the document takes the following form:
        ///     &lt;integration&gt;
        ///         &lt;statistic name="name"&gt;
        ///             value
        ///         &lt;/statistic&gt;
        ///     &lt;/integration&gt;
        /// </remarks>
        private static string ToXml(IEnumerable<StatisticResult> stats)
        {
            System.Text.StringBuilder el = new System.Text.StringBuilder();

            string result;

            foreach (StatisticResult statisticResult in stats)
            {

                if (statisticResult.Value == null)
                    result = string.Empty;
                else
                    result = System.Security.SecurityElement.Escape(statisticResult.Value.ToString());

                el.AppendLine();
                el.AppendFormat("  <statistic name=\"{0}\">{1}</statistic>",
                                    statisticResult.StatName,
                                    result);
            }

            return el.ToString();
        }


        /// <summary>
        /// Obtain the location of the XML statistics file, relative to the project artifacts directory.
        /// </summary>
        /// <param name="integrationResult">The running build.</param>
        /// <returns>The absolute file location.</returns>
        private static string XmlStatisticsFile(IIntegrationResult integrationResult)
        {
            return Path.Combine(integrationResult.ArtifactDirectory, XmlFileName);
        }

        /// <summary>
        /// Write the specified collection of statistics to the CSV
        /// statistics file, creating it if it does not already exist,
        /// and returning the full set.
        /// </summary>
        /// <param name="statisticsResults"></param>
        /// <param name="statistics"></param>
        /// <param name="integrationResult">The build for which the
        /// statistics were collected.</param>
        /// <returns>The full XML statistics document.</returns>
        /// <remarks>
        /// Note: The <see cref="StatisticsResults.AppendCsv"/> method does not
        /// reconcile the specified statistics against the existing content of
        /// the file.  If statistics are added or removed over time, the headings
        /// and values may not match up correctly.
        /// </remarks>
        private static void UpdateCsvFile(StatisticsResults statisticsResults, List<StatisticBase> statistics,
                                          IIntegrationResult integrationResult)
        {
            string csvFile = CsvStatisticsFile(integrationResult);
            statisticsResults.AppendCsv(csvFile, statistics);
        }

        /// <summary>
        /// Obtain the location of the CSV statistics file, relative to the project artifacts directory.
        /// </summary>
        /// <param name="integrationResult">The running build.</param>
        /// <returns>The absolute file location.</returns>
        private static string CsvStatisticsFile(IIntegrationResult integrationResult)
        {
            return Path.Combine(integrationResult.ArtifactDirectory, CsvFileName);
        }

        public static string LoadStatistics(string artifactDirectory)
        {            
            string documentLocation = Path.Combine(artifactDirectory, XmlFileName);
            System.Text.StringBuilder Result = new System.Text.StringBuilder();

            if (File.Exists(documentLocation))
            {
                System.IO.StreamReader sr = new StreamReader(documentLocation);
                
                Result.AppendLine("<statistics>");
                Result.AppendLine(sr.ReadToEnd());
                sr.Close();
                               
                Result.AppendLine(AppendCurrentDateElement());
               
                Result.AppendLine("</statistics>");

            }
            return Result.ToString();
        }

        private static string AppendCurrentDateElement()
        {
            DateTime now = DateTime.Now;

            return string.Format(System.Globalization.CultureInfo.CurrentCulture,"<timestamp day=\"{0}\" month=\"{1}\" year=\"{2}\" />",
                now.Day.ToString(), now.ToString("MMM"), now.Year.ToString());
        }
    }
}
