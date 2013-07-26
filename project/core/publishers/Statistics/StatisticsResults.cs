using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    /// <summary>
    /// A collection of <see cref="StatisticResult"/>s, with the
    /// elements in the order of their creation.
    /// </summary>
    public class StatisticsResults : List<StatisticResult>
    {
        /// <summary>
        /// Write the values of the statistics to the specified output writer,
        /// in the order of their creation.
        /// </summary>
        /// <param name="writer">The writer.</param>
        internal void WriteStats(TextWriter writer)
        {            
            for (int i = 0; i < Count; i++)
            {
                StatisticResult statistic = this[i];
                if (i > 0) writer.Write(", ");
                writer.Write(Convert.ToString(statistic.Value, CultureInfo.InvariantCulture));
            }
            writer.WriteLine();
        }

        /// <summary>
        /// Add the specified statistics to the specified CSV statistic file.
        /// </summary>
        /// <param name="fileName">The absolute fileid of the file.</param>
        /// <param name="statistics">The statistics.</param>
        /// <remarks>
        /// Note: This method does not reconcile the specified statistics against
        /// the existing content of the file.  If statistics are added or removed
        /// over time, the headings and values may not match up correctly.
        /// </remarks>
        internal void AppendCsv(string fileName, List<StatisticBase> statistics)
        {
            bool isNew = !File.Exists(fileName);
            StreamWriter text = null;
            try
            {
                if (isNew)
                {
                    text = File.CreateText(fileName);
                    WriteHeadings(text, statistics);
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

        /// <summary>
        /// Write the column headings for the specified statistics to the specified
        /// output writer, in the order of their creation.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="statistics">The statistics.</param>
        internal static void WriteHeadings(TextWriter writer, List<StatisticBase> statistics)
        {
            for (int i = 0; i < statistics.Count; i++)
            {
                var statistic = statistics[i];
                if (i > 0) writer.Write(", ");
                writer.Write('"' + statistic.Name + '"');
            }
            writer.WriteLine();
        }

        /// <summary>
        /// Write the statistics in XML to the specified output writer.
        /// </summary>
        /// <param name="outStream">The output writer.</param>
        /// <remarks>
        /// The output is written in the following format:
        ///     &lt;statistics&gt;
        ///         &lt;statistic name="name"&gt;
        ///             value
        ///         &lt;/statistic&gt;
        ///     &lt;/statistics&gt;
        /// </remarks>
        internal void Save(TextWriter outStream)
        {
            XmlTextWriter writer = new XmlTextWriter(outStream);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartElement("statistics");
            ForEach(delegate(StatisticResult statisticResult)
                        {
                            writer.WriteStartElement("statistic");
                            writer.WriteAttributeString("name", statisticResult.StatName);
                            writer.WriteString(Convert.ToString(statisticResult.Value, CultureInfo.InvariantCulture));
                            writer.WriteEndElement();
                        });
            writer.WriteEndElement();
        }
    }
}