using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
    public class StatisticsResults : List<StatisticResult>
    {
        internal void WriteStats(TextWriter writer)
        {            
            for (int i = 0; i < Count; i++)
            {
                StatisticResult statistic = this[i];
                if (i > 0) writer.Write(", ");
                writer.Write(statistic.Value);
            }
            writer.WriteLine();
        }

        internal void AppendCsv(string fileName, List<Statistic> statistics)
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

        internal static void WriteHeadings(TextWriter writer, List<Statistic> statistics)
        {
            for (int i = 0; i < statistics.Count; i++)
            {
                Statistic statistic = statistics[i];
                if (i > 0) writer.Write(", ");
                writer.Write('"' + statistic.Name + '"');
            }
            writer.WriteLine();
        }

        internal void Save(TextWriter outStream)
        {
            XmlTextWriter writer = new XmlTextWriter(outStream);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartElement("statistics");
            ForEach(delegate(StatisticResult statisticResult)
                        {
                            writer.WriteStartElement("statistic");
                            writer.WriteAttributeString("name", statisticResult.StatName);
                            writer.WriteString(Convert.ToString(statisticResult.Value));
                            writer.WriteEndElement();
                        });
            writer.WriteEndElement();
        }
    }
}