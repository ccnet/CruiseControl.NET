using System;
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
		private IList stats = new ArrayList();
		private IList logStatistics = new ArrayList();

		public StatisticsBuilder()
		{
			Add(new FirstMatch("BuildErrorType", "//failure/builderror/type"));
			Add(new FirstMatch("BuildErrorMessage", "//failure/builderror/message"));

			Add(new FirstMatch("StartTime", "/cruisecontrol/build/@date"));
			Add(new FirstMatch("Duration", "/cruisecontrol/build/@buildtime"));
			Add(new FirstMatch("ProjectName", "/cruisecontrol/@project"));

			Add(new Statistic("TestCount", "sum(//test-results/@total)"));
			Add(new Statistic("TestFailures", "sum(//test-results/@failures)"));
			Add(new Statistic("TestIgnored", "sum(//test-results/@not-run)"));

			Add(new Statistic("FxCop Warnings", "count(//FxCopReport/Namespaces/Namespace/Messages/Message/Issue[@Level='Warning'])"));
			Add(new Statistic("FxCop Errors", "count(//FxCopReport//Issue[@Level='Error'])"));			
		}

		public IList ProcessBuildResults(IIntegrationResult result)
		{
			return ProcessBuildResults(ToXml(result));
		}

		private string ToXml(IIntegrationResult result)
		{
			StringWriter xmlResultString = new StringWriter();
			XmlIntegrationResultWriter writer = new XmlIntegrationResultWriter(xmlResultString);
			writer.Write(result);
			return xmlResultString.ToString();
		}

		public IList ProcessBuildResults(string xmlString)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(new StringReader(xmlString));
			stats = ProcessLog(doc);
			return stats;
		}

		public void Save(TextWriter outStream)
		{
			XmlTextWriter writer = new XmlTextWriter(outStream);
			writer.Formatting = Formatting.Indented;
			writer.WriteStartElement("statistics");
			foreach (StatisticResult statisticResult in stats)
			{
				writer.WriteStartElement("statistic");
				writer.WriteAttributeString("name", statisticResult.StatName);
				writer.WriteString(Convert.ToString(statisticResult.Value));
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		public void Add(Statistic stat)
		{
			if (!logStatistics.Contains(stat))
			{
				logStatistics.Add(stat);
			}
		}

		private IList ProcessLog(XmlDocument doc)
		{
			XPathNavigator nav = doc.CreateNavigator();
			foreach (Statistic s in logStatistics)
			{
				stats.Add(s.Apply(nav));
			}
			return stats;
		}

		public void WriteHeadings(TextWriter writer)
		{
			for (int i = 0; i < logStatistics.Count; i++)
			{
				Statistic statistic = (Statistic) logStatistics[i];
				if (i > 0) writer.Write(", ");
				writer.Write('"' + statistic.Name + '"');
			}
			writer.WriteLine();
		}

		public void WriteStats(TextWriter writer)
		{
			for (int i = 0; i < stats.Count; i++)
			{
				StatisticResult statistic = (StatisticResult) stats[i];
				if (i > 0) writer.Write(", ");
				writer.Write(statistic.Value);
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
			foreach (StatisticResult statisticResult in stats)
			{
				if(statisticResult.StatName.Equals(name))
				{
					return statisticResult.Value;
				}
			}
			return null;
		}

		public IList Statistics
		{
			get { return logStatistics; }
		}
	}

}