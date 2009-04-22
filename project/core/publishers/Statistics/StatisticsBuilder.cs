using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    public class StatisticsBuilder
    {
        /// <summary>
        /// The statistics this builder works with.
        /// </summary>
        private readonly List<Statistic> logStatistics = new List<Statistic>();

        /// <summary>
        /// Create a StatisticsBuilder with the default set of statistics, all included.
        /// </summary>
        public StatisticsBuilder()
        {
            Add(new FirstMatch("StartTime", "/cruisecontrol/build/@date"));
            Add(new FirstMatch("Duration", "/cruisecontrol/build/@buildtime"));

            Add(new Statistic("TestCount", "sum(//test-results/@total)"));
            Add(new Statistic("TestFailures", "sum(//test-results/@failures)"));
            Add(new Statistic("TestIgnored", "sum(//test-results/@not-run)"));

			Add(new Statistic("GendarmeDefects", "count(//gendarme-output//rule/target/defect)"));

			Add(new Statistic("FxCop Warnings", "count(//FxCopReport//Message/Issue[@Level='Warning' or @Level='CriticalWarning'])"));
			Add(new Statistic("FxCop Errors", "count(//FxCopReport//Message/Issue[@Level='Error' or @Level='CriticalError'])"));

            Add(new FirstMatch("BuildErrorType", "//failure/builderror/type"));
            Add(new FirstMatch("BuildErrorMessage", "//failure/builderror/message"));

        }

        /// <summary>
        /// Extract all the statistics from the specified build results.
        /// </summary>
        /// <param name="result">The results of the build.</param>
        /// <returns>The set of statistic values.</returns>
        internal StatisticsResults ProcessBuildResults(IIntegrationResult result)
        {
            return ProcessBuildResults(ToXml(result));
        }

        /// <summary>
        /// Convert the build results into XML.
        /// </summary>
        /// <param name="result">The build results.</param>
        /// <returns>The XML results.</returns>
        private static string ToXml(IIntegrationResult result)
        {
            StringWriter xmlResultString = new StringWriter();
            XmlIntegrationResultWriter writer = new XmlIntegrationResultWriter(xmlResultString);
            writer.Write(result);
            return xmlResultString.ToString();
        }

        /// <summary>
        /// Extract all the statistics from the specified XML build results.
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        internal StatisticsResults ProcessBuildResults(string xmlString)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(xmlString));
            return ProcessLog(doc);
        }

        /// <summary>
        /// Add a statistic to the build if its Include property is true.
        /// </summary>
        /// <param name="statistic">The name of the statistic.</param>
        /// <remarks>
        /// If the statistic's <see cref="Statistic.Include"/> property
        /// is false, this method may actually remove it from the list!
        /// </remarks>
        internal void Add(Statistic statistic)
        {
            if (!logStatistics.Contains(statistic))
            {
                logStatistics.Add(statistic);
            }
            else
            {
                int indexOf = logStatistics.IndexOf(statistic);
                logStatistics.Remove(statistic);
                if (statistic.Include)
                {
                    logStatistics.Insert(indexOf, statistic);
                }
            }
        }

        /// <summary>
        /// Extract all the statistics from the specified XML build results document.
        /// </summary>
        /// <param name="doc">The build results.</param>
        /// <returns>The set of statistics.</returns>
        private StatisticsResults ProcessLog(IXPathNavigable doc)
        {
            XPathNavigator nav = doc.CreateNavigator();
            StatisticsResults statisticResults = new StatisticsResults();
            foreach (Statistic s in logStatistics)
            {
                statisticResults.Add(s.Apply(nav));
            }
            return statisticResults;
        }

        /// <summary>
        /// The statistics this builder works with.
        /// </summary>
        public List<Statistic> Statistics
        {
            get { return logStatistics; }
        }
    }
}