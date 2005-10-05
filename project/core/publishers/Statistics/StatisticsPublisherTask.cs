using System;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers.Statistics
{

	[ReflectorType("statistics")]
	public class StatisticsPublisherTask : ITask
	{	
		
		public void Run(IIntegrationResult iresult)
		{
			StatisticsPublisher publisher = new StatisticsPublisher();
			publisher.ProcessBuildResults(iresult);

			IntegrationState lastIntegration = iresult.LastIntegration;
			IntegrationState integration = iresult.Integration;

			UpdateXmlFile(publisher, lastIntegration, integration);
			UpdateCsvFile(publisher, integration, lastIntegration);
		}

		private static void UpdateXmlFile(StatisticsPublisher publisher, IntegrationState previousState, IntegrationState currentState)
		{
			XmlDocument doc = new XmlDocument();
	
			string lastFile = previousState.ArtifactDirectory + "\\statistics.xml";
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
	
			XmlElement xml = publisher.ToXml(doc);
			xml.SetAttribute("build-label", currentState.Label);
			root.AppendChild(xml);
	
			Directory.CreateDirectory(currentState.ArtifactDirectory);
	
			doc.Save(currentState.ArtifactDirectory + "\\statistics.xml");
		}

		private static void UpdateCsvFile(StatisticsPublisher publisher, IntegrationState currentState, IntegrationState previousState)
		{
			string newFile = currentState.ArtifactDirectory + "\\statistics.csv";
			string lastCsvFile = previousState.ArtifactDirectory + "\\statistics.csv";
			if (File.Exists(lastCsvFile))
			{
				File.Copy(lastCsvFile, newFile);
			}
			publisher.AppendCsv(newFile);
		}
	}
	
}