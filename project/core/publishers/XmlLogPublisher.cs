using System.IO;
using System.Text;
using System.Xml;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    [ReflectorType("xmllogger")]
    public class XmlLogPublisher : PublisherBase
    {
		private string DEFAULT_LOG_SUBDIRECTORY = "buildlogs";

        private string _logDir;
        private MergeFilesTask _mergeTask = new MergeFilesTask();

        public XmlLogPublisher() : base()
        {
        }

        [ReflectorProperty("logDir", Required = false)] 
		public string ConfiguredLogDirectory
        {
            get { return _logDir; }
            set { _logDir = value; }
        }

		public string LogDirectory(IProject project)
		{
			if (ConfiguredLogDirectory == null || ConfiguredLogDirectory == string.Empty)
			{
				return Path.Combine(project.ArtifactDirectory, DEFAULT_LOG_SUBDIRECTORY);
			}
			else
			{
				return ConfiguredLogDirectory;
			}
		}

    	[ReflectorArray("mergeFiles", Required = false)] 
		public string[] MergeFiles
        {
            get { return _mergeTask.MergeFiles; }

            set { _mergeTask.MergeFiles = value; }
        }

        public override void PublishIntegrationResults(IProject project, IIntegrationResult result)
        {
            // only deal with known integration status
            if (result.Status == IntegrationStatus.Unknown)
                return;

			_mergeTask.Run(result);
            using (XmlIntegrationResultWriter integrationWriter = new XmlIntegrationResultWriter(GetXmlWriter(LogDirectory(project), GetFilename(result))))
            {
                integrationWriter.Write(result);
            }
        }

        public XmlWriter GetXmlWriter(string dirname, string filename)
        {
            // create directory if necessary
            if (!Directory.Exists(dirname))
                Directory.CreateDirectory(dirname);

            string path = Path.Combine(dirname, filename);

			// create XmlWriter using UTF8 encoding
            return new XmlTextWriter(path, Encoding.UTF8);
        }

        public string GetFilename(IIntegrationResult result)
        {
			return new LogFile(result).Filename;
        }
    }
}