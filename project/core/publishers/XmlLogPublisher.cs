using System;
using System.Collections;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using tw.ccnet.core.util;
using tw.ccnet.remote;

namespace tw.ccnet.core.publishers 
{
	[ReflectorType("xmllogger")]
	public class XmlLogPublisher : PublisherBase
	{
		public class Elements 
		{
			public const string BUILD="build";
			public const string CRUISE_ROOT="cruisecontrol";
			public const string MODIFICATIONS="modifications";
			public const string INFO="info";
			public const string EXCEPTION="exception";
		}

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

		public override void Publish(object source, IntegrationResult result)
		{
			if (result.Status == IntegrationStatus.Unknown) return;

			XmlWriter writer = GetXmlWriter(LogDir, GetFilename(result)); 
			try 
			{
				Write(result, writer);
			}
			finally 
			{
				writer.Close();
			}
		}
		
		public XmlWriter GetXmlWriter(string dirname, string filename)
		{
			if (! Directory.Exists(dirname))
			{
				Directory.CreateDirectory(dirname);
			}
			string path = Path.Combine(dirname, filename);
			return new XmlTextWriter(path, System.Text.Encoding.UTF8);
		}
		
		public string GetFilename(IntegrationResult result)
		{
			DateTime date = result.LastModificationDate;
			if (result.Succeeded)
			{
				return LogFile.CreateFileName(date, result.Label);
			} 
			else 
			{
				return LogFile.CreateFileName(date);
			}
		}
		
		public void Write(IntegrationResult result, XmlWriter writer)
		{
			writer.WriteStartElement(Elements.CRUISE_ROOT);
			writer.WriteStartElement(Elements.MODIFICATIONS);
			Write(result.Modifications, writer);
			writer.WriteEndElement();
			WriteBuildElement(result, writer);
			WriteMergeFiles(writer);
			WriteException(result, writer);
			writer.WriteEndElement();
		}

		public ArrayList getFileList() 
		{
			ArrayList result = new ArrayList();
			foreach(string file in MergeFiles) 
			{
				int index = file.IndexOf("*");
				if (index != -1) 
				{
					string dir = Path.GetDirectoryName(file);
					string pattern = Path.GetFileName(file);
					DirectoryInfo info = new DirectoryInfo(dir);
					foreach (FileInfo fileInfo in info.GetFiles(pattern)) 
					{
						result.Add(fileInfo.FullName);
					}
				}
				else 
				{
					result.Add(file);
				}

			}

			return result;
		}

		private void WriteMergeFiles(XmlWriter writer) 
		{
			ArrayList files = getFileList();
			foreach (string file in files) 
			{
				FileInfo info = new FileInfo(file);
				if (info.Exists) 
				{
					XmlDocument doc = new XmlDocument();
					using (StreamReader reader = new StreamReader(info.FullName)) 
					{
						doc.Load(reader);
					}
					writer.WriteRaw(doc.DocumentElement.OuterXml);
				}
			}
		}
		
		public void WriteBuildElement(IntegrationResult result, XmlWriter writer)
		{
			writer.WriteStartElement(Elements.BUILD);
			writer.WriteAttributeString("date", result.StartTime.ToString());

			// hide the milliseconds
			TimeSpan time = result.TotalIntegrationTime;
			writer.WriteAttributeString("buildtime", string.Format("{0:d2}:{1:d2}:{2:d2}", time.Hours, time.Minutes, time.Seconds));
			if (result.Failed)
			{
				writer.WriteAttributeString("error", "true"); 
			}
			
			if (result.Output != null)
			{
				WriteIntegrationResultOutput(result, writer);
			}
			
			writer.WriteEndElement();
		}

		private void WriteIntegrationResultOutput(IntegrationResult result, XmlWriter writer)
		{
			XmlValidatingReader reader = new XmlValidatingReader(result.Output, XmlNodeType.Element, null);
			try 
			{ 
				reader.ReadInnerXml();
				writer.WriteNode(reader, false);
			}
			catch (XmlException) 
			{
				writer.WriteCData(XmlUtil.EncodeCDATA(result.Output));
			}
			finally 
			{ 
				reader.Close(); 
			}
		}

		public void WriteException(IntegrationResult result, XmlWriter writer)
		{
			if (result.ExceptionResult == null)
			{
				return;
			}

			writer.WriteStartElement(Elements.EXCEPTION);
			writer.WriteCData(XmlUtil.EncodeCDATA(result.ExceptionResult.ToString()));
			writer.WriteEndElement();
		}

		public void Write(Modification[] mods, XmlWriter writer)
		{
			if (mods == null)
			{
				return;
			}
			foreach (Modification mod in mods)
			{
				writer.WriteRaw(mod.ToXml());
			}
		}
	}
}


