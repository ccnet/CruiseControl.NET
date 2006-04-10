using System;
using System.Collections;
using System.Xml;
using System.Xml.XPath;

namespace ThoughtWorks.CruiseControl.Core.publishers.Statistics
{
	public class StatisticsChartGenerator
	{
		private string[] relevantStats;
		private IPlotter plotter;

		public StatisticsChartGenerator(IPlotter plotter)
		{
			this.plotter = plotter;
			relevantStats = new string[] {"TestCount", "Duration"};
		}

		public StatisticsChartGenerator(string savePath, string fileName) : this(new Plotter(savePath, fileName))
		{
		}

		public StatisticsChartGenerator(string savePath) : this(savePath, "reports.bmp")
		{
		}

		public string[] RelevantStats
		{
			get { return relevantStats; }
			set { relevantStats = value; }
		}

		public void Process(XmlDocument xmlDocument)
		{
			for (int i = 0; i < relevantStats.Length; i++)
			{
				string relevantStat = relevantStats[i];
				if (!AvailableStatistics(xmlDocument).Contains(relevantStat))
				{
					throw new UnavailableStatisticsException("Unavailable statistics. Check your statistics publisher configuration");
				}
				XPathNavigator navigator = xmlDocument.CreateNavigator();

				string xpath = string.Format("//statistic[@name='{0}']", relevantStats[0]);

				XPathNodeIterator dataList = navigator.Select(xpath);

				ArrayList ordinateData = new ArrayList();
				IList abscissaData = new ArrayList();
				StatisticsCalculator statisticsCalculator = new StatisticsCalculator();
				int dataIndex = 1;
				while (dataList.MoveNext())
				{
					ordinateData.Add(Convert.ToDouble(dataList.Current.Value));
					abscissaData.Add(dataIndex ++);
				}

				plotter.DrawGraph(ordinateData, abscissaData, statisticsCalculator.StandardDeviation((double[]) ordinateData.ToArray(typeof (double))));
			}
		}

		private ArrayList AvailableStatistics(XmlDocument xmlDocument)
		{
			XPathNavigator xPathNavigator = xmlDocument.CreateNavigator();
			XPathExpression statsNames = xPathNavigator.Compile("/statistics/integration[1]/statistic/@name");
			XPathNodeIterator nodeIterator = xPathNavigator.Select(statsNames);
			ArrayList stats = new ArrayList();
			while (nodeIterator.MoveNext())
			{
				stats.Add(nodeIterator.Current.Value);
			}
			return stats;
		}
	}

}