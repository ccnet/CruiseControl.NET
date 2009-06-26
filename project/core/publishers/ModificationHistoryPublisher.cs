using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <summary>
    /// This publisher logs all modifications detected by the integration to a separate file.
    /// So retrieving the modifications across builds is very easy. (Timeline, ... )
    /// </summary>
    [ReflectorType("modificationHistory")]
    public class ModificationHistoryPublisher 
        : TaskBase
    {
        public const string DataHistoryFileName = "HistoryData.xml";
        private bool onlyLogWhenChangesFound = false;


        /// <summary>
        /// When true, the history file will only be updated when the build contains modifications
        /// This setting is mainly for keeping the file small when there are a lot builds without modifications
        /// For example : like CCNet, there is a public website where everybody can force a build
        /// </summary>
        [ReflectorProperty("onlyLogWhenChangesFound", Required = false)]
        public bool OnlyLogWhenChangesFound
        {
            get { return onlyLogWhenChangesFound; }
            set { onlyLogWhenChangesFound = value; }
        }

        protected override bool Execute(IIntegrationResult result)
        {
            if ((OnlyLogWhenChangesFound) & (result.Modifications.Length == 0)) return true;

            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Saving modification history");                

            string DataHistoryFile = System.IO.Path.Combine(result.ArtifactDirectory, DataHistoryFileName);

            WriteModifications(DataHistoryFile, result);

            return true;
        }


        private void WriteModifications(string dataHistoryFile, IIntegrationResult result)
        {            
            System.IO.FileStream fs = new System.IO.FileStream( dataHistoryFile, System.IO.FileMode.Append);           
            fs.Seek(0, System.IO.SeekOrigin.End);

            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
            System.Xml.XmlTextWriter CurrentBuildInfoWriter = new System.Xml.XmlTextWriter(sw);
            CurrentBuildInfoWriter.Formatting = System.Xml.Formatting.Indented;

            CurrentBuildInfoWriter.WriteStartElement("Build");
            WriteXMLAttributeAndValue(CurrentBuildInfoWriter, "BuildDate", Util.DateUtil.FormatDate(result.EndTime));
            WriteXMLAttributeAndValue(CurrentBuildInfoWriter, "Success", result.Succeeded.ToString());
            WriteXMLAttributeAndValue(CurrentBuildInfoWriter, "Label", result.Label);

            if (result.Modifications.Length > 0)
            {                
                CurrentBuildInfoWriter.WriteStartElement("modifications");

                for (int i = 0; i < result.Modifications.Length; i++)
                {
                    result.Modifications[i].ToXml(CurrentBuildInfoWriter);
                }
                
                CurrentBuildInfoWriter.WriteEndElement();                
            }

            CurrentBuildInfoWriter.WriteEndElement();
            sw.WriteLine();

            sw.Flush();
            fs.Flush();
            
            sw.Close();
            fs.Close();
                    
        }

        private void WriteXMLAttributeAndValue(System.Xml.XmlTextWriter xmlWriter, string attributeName, string attributeValue)
        {
            xmlWriter.WriteStartAttribute(attributeName);
            xmlWriter.WriteValue(attributeValue);
        }
       

        public static string LoadHistory(string artifactDirectory)
        {
            string result = string.Empty;

            string documentLocation = System.IO.Path.Combine(artifactDirectory, DataHistoryFileName);

            if (System.IO.File.Exists(documentLocation))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(documentLocation);
                result = "<History>" + sr.ReadToEnd() + "</History>";
                sr.Close();
            }

            return result;
        }

    }
}
