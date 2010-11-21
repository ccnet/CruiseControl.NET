using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Tasks;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    /// <title>Modification History Publisher</title>
    /// <version>1.3</version>
    /// <summary>
    /// <para>
    /// This publisher logs all modifications for each build in a file.
    /// </para>
    /// <para>
    /// These modifications can be viewed in the Dashboard with the <link>modificationHistoryProjectPlugin</link> plugin enabled.
    /// </para>
    /// </summary>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;modificationHistory /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;modificationHistory  onlyLogWhenChangesFound="true" /&gt;
    /// </code>
    /// </example>
    [ReflectorType("modificationHistory")]
    public class ModificationHistoryPublisher 
        : TaskBase
    {
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
        public const string DataHistoryFileName = "HistoryData.xml";
        private bool onlyLogWhenChangesFound/* = false*/;


        /// <summary>
        /// When true, the history file will only be updated when the build contains modifications. This setting is mainly for keeping the
        /// file small when there are a lot builds without modifications. For example: like CCNet, there is a public website where everybody
        /// can force a build.
        /// </summary>
        /// <version>1.3</version>
        /// <default>false</default>
        [ReflectorProperty("onlyLogWhenChangesFound", Required = false)]
        public bool OnlyLogWhenChangesFound
        {
            get { return onlyLogWhenChangesFound; }
            set { onlyLogWhenChangesFound = value; }
        }

        /// <summary>
        /// Executes the specified result.	
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override bool Execute(IIntegrationResult result)
        {
            if ((OnlyLogWhenChangesFound) & (result.Modifications.Length == 0)) return true;

            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Saving modification history");                

            string dataHistoryFile = System.IO.Path.Combine(result.ArtifactDirectory, DataHistoryFileName);

            WriteModifications(dataHistoryFile, result);

            return true;
        }


        private void WriteModifications(string dataHistoryFile, IIntegrationResult result)
        {            
            System.IO.FileStream fs = new System.IO.FileStream( dataHistoryFile, System.IO.FileMode.Append);           
            fs.Seek(0, System.IO.SeekOrigin.End);

            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
            System.Xml.XmlTextWriter currentBuildInfoWriter = new System.Xml.XmlTextWriter(sw);
            currentBuildInfoWriter.Formatting = System.Xml.Formatting.Indented;

            currentBuildInfoWriter.WriteStartElement("Build");
            WriteXMLAttributeAndValue(currentBuildInfoWriter, "BuildDate", Util.DateUtil.FormatDate(result.EndTime));
            WriteXMLAttributeAndValue(currentBuildInfoWriter, "Success", result.Succeeded.ToString(CultureInfo.CurrentCulture));
            WriteXMLAttributeAndValue(currentBuildInfoWriter, "Label", result.Label);

            if (result.Modifications.Length > 0)
            {                
                currentBuildInfoWriter.WriteStartElement("modifications");

                for (int i = 0; i < result.Modifications.Length; i++)
                {
                    result.Modifications[i].ToXml(currentBuildInfoWriter);
                }
                
                currentBuildInfoWriter.WriteEndElement();                
            }

            currentBuildInfoWriter.WriteEndElement();
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


        /// <summary>
        /// Loads the history.	
        /// </summary>
        /// <param name="artifactDirectory">The artifact directory.</param>
        /// <returns></returns>
        /// <remarks></remarks>
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
