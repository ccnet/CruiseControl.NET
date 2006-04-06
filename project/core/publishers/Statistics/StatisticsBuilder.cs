using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	public class StatisticsBuilder
	{
		private void Add(Statistic stat)
		{
			logStatistics.Add(stat);
		}

		public void ProcessBuildResults(IIntegrationResult result)
		{
			ProcessBuildResults(toXml(result));
		}

		private string toXml(IIntegrationResult result)
		{
			StringWriter xmlResultString = new StringWriter();
			XmlIntegrationResultWriter writer = new XmlIntegrationResultWriter(xmlResultString);
			writer.Write(result);
			return xmlResultString.ToString();
		}


		public void ProcessBuildResults(string xmlString)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(new StringReader(xmlString));
			stats = ProcessLog(doc);
		}

		public void ProcessFile(string file)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(file);
			stats = ProcessLog(doc);
		}

		public void Save(TextWriter outStream)
		{
			XmlTextWriter writer = new XmlTextWriter(outStream);
			writer.Formatting = Formatting.Indented;
			writer.WriteStartElement("statistics");
			foreach (string key in stats.Keys)
			{
				writer.WriteStartElement("statistic");
				writer.WriteAttributeString("name", key);
				writer.WriteString(stats[key].ToString());
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		public XmlElement ToXml(XmlDocument doc)
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

		private Hashtable stats = new Hashtable();
		private IList logStatistics = new ArrayList();

		public StatisticsBuilder()
		{
			Add(new FirstMatch(new DictionaryEntry("StartTime", "/cruisecontrol/build/@date")));
			Add(new FirstMatch(new DictionaryEntry("Duration", "/cruisecontrol/build/@buildtime")));
			Add(new FirstMatch(new DictionaryEntry("ProjectName", "/cruisecontrol/@project")));

			Add(new Statistic(new DictionaryEntry("TestCount", "sum(//test-results/@total)")));
			Add(new Statistic(new DictionaryEntry("TestFailures", "sum(//test-results/@failures)")));
			Add(new Statistic(new DictionaryEntry("TestIgnored", "sum(//test-results/@not-run)")));

			Add(new Statistic(new DictionaryEntry("FxCop Warnings", "count(//FxCopReport/Namespaces/Namespace/Messages/Message/Issue[@Level='Warning'])")));
			Add(new Statistic(new DictionaryEntry("FxCop Errors", "count(//FxCopReport//Issue[@Level='Error'])")));
		}

		private Hashtable ProcessLog(XmlDocument doc)
		{
			XPathNavigator nav = doc.CreateNavigator();
			foreach (Statistic s in logStatistics)
			{
				stats[s.Name] = s.Apply(nav);
			}
			return stats;
		}

		public void WriteHeadings(TextWriter writer)
		{
			for (int i = 0; i < logStatistics.Count; i++)
			{
				Statistic statistic = (Statistic) logStatistics[i];
				writer.Write(", ");
				writer.Write('"' + statistic.Name + '"');
			}
			writer.WriteLine();
		}

		public void WriteStats(TextWriter writer)
		{
			for (int i = 0; i < logStatistics.Count; i++)
			{
				Statistic statistic = (Statistic) logStatistics[i];
				if (i > 0) writer.Write(", ");
				writer.Write(stats[statistic.Name]);
			}
			writer.WriteLine();
		}

		public void AppendCsv(string fileName)
		{
			bool isNew = !File.Exists(fileName);
			StreamWriter text = null;
			try
			{
				if (isNew)
				{
					text = File.CreateText(fileName);
					WriteHeadings(text);
				}
				else
				{
					text = File.AppendText(fileName);
				}
				WriteStats(text);
			}
			finally
			{
				if (text != null) text.Close();
			}
		}

		public object Statistic(string name)
		{
			return stats[name];
		}
	}

}