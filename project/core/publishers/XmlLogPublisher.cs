using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

using Exortech.NetReflector;

using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers 
{
	[ReflectorType("xmllogger")]
	public class XmlLogPublisher : PublisherBase
	{
		private string _logDir;
		private string[] _mergeFiles;

		public XmlLogPublisher() : base()
		{
		}

		[ReflectorProperty("logDir")]
		public string LogDir
		{
			get { return _logDir; }
			set { _logDir = value; }
		}

		[ReflectorArray("mergeFiles", Required=false)]
		public string[] MergeFiles 
		{
			get 
			{
				if (_mergeFiles == null)
					_mergeFiles = new string[0];

				return _mergeFiles;
			}

			set { _mergeFiles = value; }
		}

		public override void PublishIntegrationResults(IProject project, IntegrationResult result)
		{
			// only deal with known integration status
			if (result.Status==IntegrationStatus.Unknown)
				return;
			foreach(string mergeFileName in MergeFiles)
			{
				result.TaskResults.Add(new DefaultTaskResult(mergeFileName));			    
			}
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

		/// <summary>
		/// Gets the list of file names, as specified in the MergeFiles property.  Any wildcards
		/// are expanded to include such files.
		/// </summary>
		public ArrayList GetMergeFileList() 
		{
			ArrayList result = new ArrayList();

			foreach (string file in MergeFiles) 
			{
				if (file.IndexOf("*")>-1) 
				{
					// filename has a wildcard
					string dir = Path.GetDirectoryName(file);
					string pattern = Path.GetFileName(file);
					DirectoryInfo info = new DirectoryInfo(dir);
					// add all files that match wildcard
					foreach (FileInfo fileInfo in info.GetFiles(pattern)) 
					{
						result.Add(fileInfo.FullName);
					}
				}
				else 
				{
					// no wildcard, so just add
					result.Add(file);
				}
			}

			return result;
		}
	}
}
