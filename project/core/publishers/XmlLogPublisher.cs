using System.IO;
using System.Text;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	// TODO - Make Integration Writer a dependency, and make its method take the filename
    [ReflectorType("xmllogger")]
    public class XmlLogPublisher : PublisherBase
    {
		public static readonly string DEFAULT_LOG_SUBDIRECTORY = "buildlogs";

        private string _logDir;

        public XmlLogPublisher() : base()
        {
        }

        [ReflectorProperty("logDir", Required = false)] 
		public string ConfiguredLogDirectory
        {
            get { return _logDir; }
            set { _logDir = value; }
        }

		// This is only public because of a nasty hack which I (MR) put in the code. To be made private later...
		public string LogDirectory(IProject project)
		{
			if (ConfiguredLogDirectory == null || ConfiguredLogDirectory == "")
			{
				return Path.Combine(project.ArtifactDirectory, DEFAULT_LOG_SUBDIRECTORY);
			}
			else if (Path.IsPathRooted(ConfiguredLogDirectory))
			{
				return ConfiguredLogDirectory;
			}
			else
			{
				return Path.Combine(project.ArtifactDirectory, ConfiguredLogDirectory);
			}
		}

		public override void PublishIntegrationResults(IProject project, IIntegrationResult result)
        {
            // only deal with known integration status
            if (result.Status == IntegrationStatus.Unknown)
                return;

            using (XmlIntegrationResultWriter integrationWriter = new XmlIntegrationResultWriter(GetXmlWriter(LogDirectory(project), GetFilename(result))))
            {
                integrationWriter.Write(result);
            }
        }

        private XmlWriter GetXmlWriter(string dirname, string filename)
        {
            // create directory if necessary
            if (!Directory.Exists(dirname))
                Directory.CreateDirectory(dirname);

            string path = Path.Combine(dirname, filename);

			// create XmlWriter using UTF8 encoding
        	XmlTextWriter writer = new XmlTextWriter(path, Encoding.UTF8);
			writer.Formatting = Formatting.Indented;
        	return writer;
        }

        private string GetFilename(IIntegrationResult result)
        {
			return new LogFile(result).Filename;
        }
    }
}