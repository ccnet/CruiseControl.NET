using System;
using System.Collections;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.publishers.Statistics;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	[ReflectorType("statistics")]
	public class StatisticsPublisher : ITask
	{
		private static string csvFileName = "statistics.csv";

		public void Run(IIntegrationResult integrationResult)
		{
			StatisticsBuilder builder = new StatisticsBuilder();
			Hashtable stats = builder.ProcessBuildResults(integrationResult);

			XmlDocument xmlDocument = UpdateXmlFile(stats, integrationResult);
			ChartGenerator().Process(xmlDocument, integrationResult.ArtifactDirectory);
			UpdateCsvFile(builder, integrationResult.Integration);
		}

		private StatisticsChartGenerator ChartGenerator()
		{
			StatisticsChartGenerator chartGenerator = new StatisticsChartGenerator();
			chartGenerator.RelevantStats = new string[]{"TestCount", "Duration"};
			return chartGenerator;
		}

		private XmlDocument UpdateXmlFile(Hashtable stats, IIntegrationResult integrationResult)
		{
			XmlDocument doc = new XmlDocument();
	
			string lastFile = XmlStatisticsFile(integrationResult);
			XmlElement root = null;
			if (File.Exists(lastFile))
			{
				doc.Load(lastFile);
				root = (XmlElement) doc.FirstChild;
			}
			else
			{
				root = doc.CreateElement("statistics");
				doc.AppendChild(root);
			}

			XmlElement xml = toXml(doc, stats);
			xml.SetAttribute("build-label", integrationResult.Label);
			IntegrationStatus status = integrationResult.Status;
			xml.SetAttribute("status", status.ToString());
			DateTime now = DateTime.Now;
			xml.SetAttribute("day", now.Day.ToString());
			xml.SetAttribute("month", now.ToString("MMM"));
			xml.SetAttribute("year", now.Year.ToString());
			xml.SetAttribute("fileName", integrationResult.StatisticsFile);
			root.AppendChild(xml);
	
			Directory.CreateDirectory(integrationResult.ArtifactDirectory);
	
			doc.Save(XmlStatisticsFile(integrationResult));
			return doc;
		}

		private XmlElement toXml(XmlDocument doc, Hashtable stats)
		{
			XmlElement el = doc.CreateElement("integration");
			foreach (string key in stats.Keys)
			{
				XmlElement stat = doc.CreateElement("statistic");
				stat.SetAttribute("name", key);
				stat.InnerText = stats[key].ToString();
				el.AppendChild(stat);
			}
			return el;
		}


		private string XmlStatisticsFile(IIntegrationResult integrationResult)
		{
			return Path.Combine(integrationResult.ArtifactDirectory, integrationResult.StatisticsFile);
		}

		private string CsvStatisticsFile(IntegrationState integrationState)
		{
			return Path.Combine(integrationState.ArtifactDirectory, csvFileName);
		}

		private void UpdateCsvFile(StatisticsBuilder builder, IntegrationState previousState)
		{
			string csvFile = CsvStatisticsFile(previousState);
			builder.AppendCsv(csvFile);
		}
	}
}