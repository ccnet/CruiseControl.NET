using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    [ReflectorType("statistics")]
    public class StatisticsPublisher 
        : TaskBase, ITask
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
        /// The list of statistics to be included in the build.
        /// </summary>
        /// <remarks>
        /// There is a default list of statistics to be included, and this list
        /// adds additional statistics to the build.  Any statistic defined with
        /// <code>include=false</code> will be omitted, even if it is in the
        /// default list.
        /// </remarks>
        [ReflectorArray("statisticList", Required=false)]
        public Statistic[] ConfiguredStatistics = new Statistic[0];

        #region ITask Members

        /// <summary>
        /// Publish the statistics for this build.
        /// </summary>
        /// <param name="integrationResult">The results of the build.</param>
        public void Run(IIntegrationResult integrationResult)
        {
            StatisticsBuilder builder = new StatisticsBuilder();
            for (int i = 0; i < ConfiguredStatistics.Length; i++)
            {
                Statistic statistic = ConfiguredStatistics[i];
                builder.Add(statistic);     // Note: This may actually remove the statistic if include=false.
            }
            StatisticsResults stats = builder.ProcessBuildResults(integrationResult);

            UpdateXmlFile(stats, integrationResult);
            
            //commented out because this does not work
            //ChartGenerator(builder.Statistics).Process(xmlDocument, integrationResult.ArtifactDirectory);
            
            UpdateCsvFile(stats, builder.Statistics, integrationResult);
        }

        #endregion

        /// <summary>
        /// Create a chart generator for the specified statistics using the default plotter.
        /// </summary>
        /// <param name="statistics">The statistics to include, checking their
        /// <see cref="Statistic.GenerateGraph"/> property</param>
        /// <returns>The chart generator.</returns>
        private static StatisticsChartGenerator ChartGenerator(List<Statistic> statistics)
        {
            StatisticsChartGenerator chartGenerator = new StatisticsChartGenerator();
            List<string> list = new List<String>();
            statistics.ForEach(delegate(Statistic statistic)
                                   {
                                       if (statistic.GenerateGraph)
                                       {
                                           list.Add(statistic.Name);
                                       }
                                   });
            chartGenerator.RelevantStats = list.ToArray();
            return chartGenerator;
        }

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
                                        now.Day.ToString(), now.ToString("MMM"), now.Year.ToString());

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
                    result = "";
                else
                    result = statisticResult.Value.ToString();

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
        private static void UpdateCsvFile(StatisticsResults statisticsResults, List<Statistic> statistics,
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

            return string.Format("<timestamp day=\"{0}\" month=\"{1}\" year=\"{2}\" />",
                now.Day.ToString(), now.ToString("MMM"), now.Year.ToString());
        }
    }
}