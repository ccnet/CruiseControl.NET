using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    [ReflectorType("statistics")]
    public class StatisticsPublisher : ITask
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

            XmlDocument xmlDocument = UpdateXmlFile(stats, integrationResult);
            ChartGenerator(builder.Statistics).Process(xmlDocument, integrationResult.ArtifactDirectory);
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
        /// statistics file, creating it if it does not already exist,
        /// and returning the full set.
        /// </summary>
        /// <param name="stats">The collection of statistics.</param>
        /// <param name="integrationResult">The build for which the
        /// statistics were collected.</param>
        /// <returns>The full XML statistics document.</returns>
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
        private static XmlDocument UpdateXmlFile(IEnumerable<StatisticResult> stats,
                                                 IIntegrationResult integrationResult)
        {
            XmlDocument doc = new XmlDocument();

            string lastFile = XmlStatisticsFile(integrationResult);
            if (File.Exists(lastFile))
            {
                doc.Load(lastFile);
            }
            else
            {
                doc.AppendChild(doc.CreateElement("statistics"));
            }
            XmlElement root = doc.DocumentElement;

            XmlElement xml = ToXml(doc, stats);
            xml.SetAttribute("build-label", integrationResult.Label);
            IntegrationStatus status = integrationResult.Status;
            xml.SetAttribute("status", status.ToString());
            DateTime now = DateTime.Now;
            xml.SetAttribute("day", now.Day.ToString());
            xml.SetAttribute("month", now.ToString("MMM"));
            xml.SetAttribute("year", now.Year.ToString());
            root.AppendChild(xml);

            Directory.CreateDirectory(integrationResult.ArtifactDirectory);

            doc.Save(XmlStatisticsFile(integrationResult));
            return doc;
        }

        /// <summary>
        /// Add the specified collection of statistics to the root of the specified XML document.
        /// </summary>
        /// <param name="doc">the document to add the data to.</param>
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
        private static XmlElement ToXml(XmlDocument doc, IEnumerable<StatisticResult> stats)
        {
            XmlElement el = doc.CreateElement("integration");

            foreach (StatisticResult statisticResult in stats)
            {
                XmlElement stat = doc.CreateElement("statistic");
                stat.SetAttribute("name", statisticResult.StatName);
                stat.InnerText = Convert.ToString(statisticResult.Value);
                el.AppendChild(stat);
            }
            return el;
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

        public static XmlDocument LoadStatistics(string artifactDirectory)
        {
            XmlDocument xmlDocument = new XmlDocument();
            string documentLocation = Path.Combine(artifactDirectory, XmlFileName);
            if (File.Exists(documentLocation))
            {
                xmlDocument.Load(documentLocation);
                AppendCurrentDateElement(xmlDocument);
            }
            return xmlDocument;
        }

        private static void AppendCurrentDateElement(XmlDocument xmlDocument)
        {
            XmlElement timeStamp = xmlDocument.CreateElement("timestamp");
            DateTime now = DateTime.Now;
            timeStamp.SetAttribute("day", now.Day.ToString());
            timeStamp.SetAttribute("month", now.ToString("MMM"));
            timeStamp.SetAttribute("year", now.Year.ToString());
            xmlDocument.DocumentElement.AppendChild(timeStamp);
        }
    }
}