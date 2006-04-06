using System.IO;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	[ReflectorType("statistics")]
	public class StatisticsPublisher : ITask
	{
		private string xmlFileName = "statistics.xml";
		private static string csvFileName = "statistics.csv";

		public void Run(IIntegrationResult iresult)
		{
			StatisticsBuilder builder = new StatisticsBuilder();
			builder.ProcessBuildResults(iresult);

			IntegrationState lastIntegration = iresult.LastIntegration;
			IntegrationState integration = iresult.Integration;

			UpdateXmlFile(builder, lastIntegration, integration);
			UpdateCsvFile(builder, integration, lastIntegration);
		}

		private void UpdateXmlFile(StatisticsBuilder builder, IntegrationState previousState, IntegrationState currentState)
		{
			XmlDocument doc = new XmlDocument();
	
			string lastFile = xmlStatisticsFile(previousState);
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
	
			doc.Save(xmlStatisticsFile(currentState));
		}

		private string xmlStatisticsFile(IntegrationState integrationState)
		{
			return Path.Combine(integrationState.ArtifactDirectory, xmlFileName);
		}

		private string csvStatisticsFile(IntegrationState integrationState)
		{
			return Path.Combine(integrationState.ArtifactDirectory, csvFileName);
		}

		private void UpdateCsvFile(StatisticsBuilder builder, IntegrationState currentState, IntegrationState previousState)
		{
			string newFile = csvStatisticsFile(currentState);
			string lastCsvFile = csvStatisticsFile(previousState);
			if (File.Exists(lastCsvFile))
			{
				File.Copy(lastCsvFile, newFile);
			}
			builder.AppendCsv(newFile);
		}
	}
}