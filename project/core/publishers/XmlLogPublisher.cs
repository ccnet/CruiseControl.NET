using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    [ReflectorType("xmllogger")]
    public class XmlLogPublisher : PublisherBase
    {
        private string _logDir;
        private MergeFilesTask _mergeTask = new MergeFilesTask();
        //        private string[] _mergeFiles;

        public XmlLogPublisher() : base()
        {
        }

        [ReflectorProperty("logDir")] public string LogDir
        {
            get { return _logDir; }
            set { _logDir = value; }
        }

        [ReflectorArray("mergeFiles", Required = false)] public string[] MergeFiles
        {
            get { return _mergeTask.MergeFiles; }

            set { _mergeTask.MergeFiles = value; }
        }

        public override void PublishIntegrationResults(IProject project, IntegrationResult result)
        {
            // only deal with known integration status
            if (result.Status == IntegrationStatus.Unknown)
                return;

			_mergeTask.Run(result, project);
            using (XmlIntegrationResultWriter integrationWriter = new XmlIntegrationResultWriter(GetXmlWriter(LogDir, GetFilename(result))))
            {
                integrationWriter.Write(result);
            }
        }

        public XmlWriter GetXmlWriter(string dirname, string filename)
        {
            // create directory if necessary
            if (!Directory.Exists(dirname))
                Directory.CreateDirectory(dirname);

            // create Xml writer using UTF8 encoding
            string path = Path.Combine(dirname, filename);
            return new XmlTextWriter(path, System.Text.Encoding.UTF8);
        }

        public string GetFilename(IntegrationResult result)
        {
            DateTime startTime = result.StartTime;
            if (result.Succeeded)
                return LogFileUtil.CreateSuccessfulBuildLogFileName(startTime, result.Label);
            else
                return LogFileUtil.CreateFailedBuildLogFileName(startTime);
        }
    }
}