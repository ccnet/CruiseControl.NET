using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    public class StatisticsBuilder
    {
        private readonly List<Statistic> logStatistics = new List<Statistic>();

        public StatisticsBuilder()
        {
            Add(new FirstMatch("BuildErrorType", "//failure/builderror/type"));
            Add(new FirstMatch("BuildErrorMessage", "//failure/builderror/message"));

            Add(new FirstMatch("StartTime", "/cruisecontrol/build/@date"));
            Add(new FirstMatch("Duration", "/cruisecontrol/build/@buildtime"));
//            Add(new FirstMatch("ProjectName", "/cruisecontrol/@project"));

            Add(new Statistic("TestCount", "sum(//test-results/@total)"));
            Add(new Statistic("TestFailures", "sum(//test-results/@failures)"));
            Add(new Statistic("TestIgnored", "sum(//test-results/@not-run)"));

            Add(
                new Statistic("FxCop Warnings",
                              "count(//FxCopReport/Namespaces/Namespace/Messages/Message/Issue[@Level='Warning'])"));
            Add(new Statistic("FxCop Errors", "count(//FxCopReport//Issue[@Level='Error'])"));
        }

        internal StatisticsResults ProcessBuildResults(IIntegrationResult result)
        {
            return ProcessBuildResults(ToXml(result));
        }

        private static string ToXml(IIntegrationResult result)
        {
            StringWriter xmlResultString = new StringWriter();
            XmlIntegrationResultWriter writer = new XmlIntegrationResultWriter(xmlResultString);
            writer.Write(result);
            return xmlResultString.ToString();
        }

        internal StatisticsResults ProcessBuildResults(string xmlString)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(xmlString));
            return ProcessLog(doc);
        }

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

        public List<Statistic> Statistics
        {
            get { return logStatistics; }
        }
    }
}