using System;
using System.Collections;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    /// <summary>
    /// A charting tool for <see cref="Statistic"/>s.
    /// </summary>
    public class StatisticsChartGenerator
    {
        /// <summary>
        /// The statistic names to be included in the chart.
        /// </summary>
        private string[] relevantStats;
        /// <summary>
        /// The plotter that will render the chart.
        /// </summary>
        private IPlotter plotter;

        /// <summary>
        /// Create a chart generator using the default data plotter.
        /// </summary>
        public StatisticsChartGenerator()
        {
        }

        /// <summary>
        /// Create a chart generator using the specified data plotter.
        /// </summary>
        /// <param name="plotter">The plotter that will render the chart.</param>
        public StatisticsChartGenerator(IPlotter plotter)
        {
            this.plotter = plotter;
        }

        /// <summary>
        /// The statistic names to be included in the chart.
        /// </summary>
        public string[] RelevantStats
        {
            get { return relevantStats; }
            set { relevantStats = value; }
        }

        /// <summary>
        /// Get a data plotter for the specified fileid.
        /// </summary>
        /// <param name="savePath">The fileid to store the image at.</param>
        /// <returns>The plotter.</returns>
        private IPlotter GetPlotter(string savePath)
        {
            if (plotter == null)
            {
                plotter = new Plotter(savePath, "png", ImageFormat.Png);
            }
            return plotter;
        }

        /// <summary>
        /// Extract the statistics from the specified XML statistics
        /// document and create a chart image at the specifed fileid.
        /// </summary>
        /// <param name="xmlDocument">The XML document containing the data.</param>
        /// <param name="savePath">The location of the chart image file.</param>
        public void Process(XmlDocument xmlDocument, string savePath)
        {
            foreach (string relevantStat in relevantStats)
            {
                if (!AvailableStatistics(xmlDocument).Contains(relevantStat))
                {
                    throw new UnavailableStatisticsException(relevantStat);
                }
                XPathNavigator navigator = xmlDocument.CreateNavigator();

                string xpath = "/statistics/integration";

                XPathNodeIterator dataList = navigator.Select(xpath);

                ArrayList ordinateData = new ArrayList();
                IList abscissaData = new ArrayList();
                while (dataList.MoveNext())
                {
                    XPathNavigator integration = dataList.Current;
                    XPathNavigator statistic =
                        integration.SelectSingleNode(string.Format("statistic[@name='{0}']", relevantStat));

                    string value = statistic.Value;
                    Log.Debug(string.Format("Relevant Stat: {0}, Raw Value: {1}", relevantStat, value));
                    ordinateData.Add(GetPlottableValue(relevantStat, value));
                    abscissaData.Add(integration.GetAttribute("build-label", ""));
                }

                try
                {
                    GetPlotter(savePath).DrawGraph(ordinateData, abscissaData, relevantStat);
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("Cannot handle value for the statistic {0}", relevantStat));
                }
            }
        }

        /// <summary>
        /// Convert the specified statistic value to a form amenable to plotting.
        /// </summary>
        /// <param name="relevantStat">The statistic name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The plottable value.</returns>
        /// <remarks>
        /// Most values are returned unmodified.  Values for "Duration" statistics
        /// that look like time durations (<i>e.g.</i>, <code>12:34:56</code> are
        /// converted to an integer number of seconds.
        /// </remarks>
        private static string GetPlottableValue(string relevantStat, string value)
        {
            if (relevantStat == "Duration" && Regex.IsMatch(value, "[0-9]+:[0-9]+:[0-9]+"))
            {
                string[] parts = value.Split(':');
                value = Convert.ToInt32(parts[0]) * 3600 + Convert.ToInt32(parts[1]) * 60 + parts[2];
            }
            return value;
        }

        /// <summary>
        /// Determine what statistics are present in the specified XML statistics document.
        /// </summary>
        /// <param name="xmlDocument">The XML statistics document.</param>
        /// <returns>A list of statistic names.</returns>
        private static ArrayList AvailableStatistics(XmlDocument xmlDocument)
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