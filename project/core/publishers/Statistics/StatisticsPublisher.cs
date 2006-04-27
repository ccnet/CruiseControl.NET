using System.Collections;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.publishers.Statistics;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	[ReflectorType("statistics")]
	public class StatisticsPublisher : ITask
	{
		private static string xmlFileName = "statistics.xml";
		private static string csvFileName = "statistics.csv";

		public void Run(IIntegrationResult integrationResult)
		{
			StatisticsBuilder builder = new StatisticsBuilder();
			Hashtable stats = builder.ProcessBuildResults(integrationResult);

			IntegrationState lastIntegration = integrationResult.LastIntegration;
			IntegrationState integration = integrationResult.Integration;

			XmlDocument xmlDocument = UpdateXmlFile(builder, lastIntegration, integration);
			ChartGenerator().Process(xmlDocument, integrationResult.ArtifactDirectory);
			UpdateCsvFile(builder, integration);
		}

		private StatisticsChartGenerator ChartGenerator()
		{
			StatisticsChartGenerator chartGenerator = new StatisticsChartGenerator();
			chartGenerator.RelevantStats = new string[]{"TestCount", "Duration"};
			return chartGenerator;
		}

		private XmlDocument UpdateXmlFile(StatisticsBuilder builder, IntegrationState previousState, IntegrationState currentState)
		{
			XmlDocument doc = new XmlDocument();
	
			string lastFile = XmlStatisticsFile(previousState);
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
	
			XmlElement xml = builder.ToXml(doc);
			xml.SetAttribute("build-label", currentState.Label);
			root.AppendChild(xml);
	
			Directory.CreateDirectory(currentState.ArtifactDirectory);
	
			doc.Save(XmlStatisticsFile(currentState));
			return doc;
		}

		private string XmlStatisticsFile(IntegrationState integrationState)
		{
			return Path.Combine(integrationState.ArtifactDirectory, xmlFileName);
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