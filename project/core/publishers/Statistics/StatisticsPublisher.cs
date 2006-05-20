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
		private static string csvFileName = "statistics.csv";
		private Statistic[] configuredStatistics = new Statistic[0];

		public void Run(IIntegrationResult integrationResult)
		{
			StatisticsBuilder builder = new StatisticsBuilder();
			for (int i = 0; i < configuredStatistics.Length; i++)
			{
				Statistic statistic = configuredStatistics[i];
				builder.Add(statistic);				
			}
			IList stats = builder.ProcessBuildResults(integrationResult);

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

		[ReflectorArray("statisticList", Required=false)]
		public Statistic[] ConfiguredStatistics
		{
			get{ return configuredStatistics;}
			set{configuredStatistics = value;}
		}

		private XmlDocument UpdateXmlFile(IList stats, IIntegrationResult integrationResult)
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
			root.AppendChild(xml);
	
			Directory.CreateDirectory(integrationResult.ArtifactDirectory);
	
			doc.Save(XmlStatisticsFile(integrationResult));
			return doc;
		}

		private XmlElement toXml(XmlDocument doc, IList stats)
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