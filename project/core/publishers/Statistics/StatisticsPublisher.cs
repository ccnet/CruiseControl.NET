using System;
using System.Collections;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	[ReflectorType("statistics")]
	public class StatisticsPublisher : ITask
	{
		public const string CsvFileName = "statistics.csv";
		public const string XmlFileName = "report.xml";

		[ReflectorArray("statisticList", Required=false)]
		public Statistic[] ConfiguredStatistics = new Statistic[0];

		public void Run(IIntegrationResult integrationResult)
		{
			StatisticsBuilder builder = new StatisticsBuilder();
			for (int i = 0; i < ConfiguredStatistics.Length; i++)
			{
				Statistic statistic = ConfiguredStatistics[i];
				builder.Add(statistic);				
			}
			IList stats = builder.ProcessBuildResults(integrationResult);

			XmlDocument xmlDocument = UpdateXmlFile(stats, integrationResult);
			ChartGenerator().Process(xmlDocument, integrationResult.ArtifactDirectory);
			UpdateCsvFile(builder, integrationResult);
		}

		private static StatisticsChartGenerator ChartGenerator()
		{
			StatisticsChartGenerator chartGenerator = new StatisticsChartGenerator();
			chartGenerator.RelevantStats = new string[]{"TestCount", "Duration"};
			return chartGenerator;
		}

		private static XmlDocument UpdateXmlFile(IList stats, IIntegrationResult integrationResult)
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

		private static XmlElement ToXml(XmlDocument doc, IList stats)
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

		private static string XmlStatisticsFile(IIntegrationResult integrationResult)
		{
			return Path.Combine(integrationResult.ArtifactDirectory, XmlFileName);
		}

		private static void UpdateCsvFile(StatisticsBuilder builder, IIntegrationResult integrationResult)
		{
			string csvFile = CsvStatisticsFile(integrationResult);
			builder.AppendCsv(csvFile);
		}

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