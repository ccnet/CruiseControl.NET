using System;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using Exortech.NetReflector;
using tw.ccnet.remote;

namespace tw.ccnet.core.history
{
	[ReflectorType("xmlhistory")]
	public class XmlBuildHistory : IBuildHistory
	{
		private const string FILENAME_FORMAT = "log{0}{1}.xml";
		private const string DATE_FORMAT = "yyyyMMddHHmmss";
		private const string BUILDLABEL_SEPARATOR = "Lbuild.";

		private XmlSerializer _serializer;
		private string _logDir;

		public XmlBuildHistory() 
		{
			_serializer = new XmlSerializer(typeof(IntegrationResult));
		}

		[ReflectorProperty("historyDir", Required=false)]
		public string LogDir
		{
			get 
			{ 
				if (_logDir == null)
				{
					_logDir = Directory.GetCurrentDirectory();
				}
				return _logDir; 
			}
			set { _logDir = value; }
		}

		public bool Exists()
		{
			return Directory.Exists(LogDir) && GetLogFiles().Length > 0;
		}

		public IntegrationResult Load()
		{
			string[] filenames = GetLogFiles();
			if (filenames.Length == 0) return null;

			ArrayList.Adapter(filenames).Sort();
			return Load(filenames[filenames.Length-1]);
		}

		private string[] GetLogFiles()
		{
			return Directory.GetFiles(LogDir, String.Format(FILENAME_FORMAT, "*", null));
		}

		public IntegrationResult Load(string filename)
		{
			TextReader reader = new StreamReader(filename);
			try
			{
				return DeserializeIntegrationResult(reader);
			}
			finally
			{
				reader.Close();
			}
		}

		public void Save(IntegrationResult result)
		{
			TextWriter stream = CreateFileWriter(GetFilePath(result));
			try
			{
				SerializeIntegrationResult(stream, result);
				stream.Flush();
			}
			catch (SystemException ex)
			{
				throw new CruiseControlException("Unable to write IntegrationResult to file.", ex);
			}
			finally
			{
				stream.Close();
			}
		}

		internal void SerializeIntegrationResult(TextWriter stream, IntegrationResult result)
		{
			_serializer.Serialize(stream, result);
		}

		internal IntegrationResult DeserializeIntegrationResult(TextReader stream)
		{
			return (IntegrationResult)_serializer.Deserialize(stream);
		}

		protected TextWriter CreateFileWriter(string filename)
		{
			try
			{
				return new StreamWriter(filename);
			}
			catch (SystemException e)
			{
				throw new CruiseControlException("Unable to write IntegrationResult log file to the specified path: " +
					filename, e);
			}
		}

		public string GetFilename(IntegrationResult result)
		{
			string label = (result.Status == IntegrationStatus.Success) ? 
				BUILDLABEL_SEPARATOR + result.Label : String.Empty;

			return String.Format(FILENAME_FORMAT, result.StartTime.ToString(DATE_FORMAT), label);
		}

		public string GetFilePath(IntegrationResult result)
		{
			string filename = GetFilename(result);
			try
			{
				return Path.Combine(LogDir, filename);
			}
			catch (SystemException ex)
			{
				throw new CruiseControlException("Invalid IntegrationResult file path.  Log directory may be unspecified or invalid.  LogDir: " + 
					_logDir, ex);
			}
		}
	}
}
