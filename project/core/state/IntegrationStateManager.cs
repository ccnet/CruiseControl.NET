using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core.State
{
	[ReflectorType("state")]
	public class IntegrationStateManager : IStateManager
	{
		XmlSerializer _serializer;
		string _directory = System.IO.Directory.GetCurrentDirectory(); // default
		string _filename = "ccnet.state"; // default

		public IntegrationStateManager()
		{
			_serializer = new XmlSerializer(typeof(IntegrationResult));
		}

		[ReflectorProperty("directory", Required=false)]
		public string Directory
		{
			get { return _directory; }
			set { _directory = value; }
		}

		[ReflectorProperty("filename", Required=false)]
		public string Filename
		{
			get { return _filename; }
			set { _filename = value; }
		}

		public string GetFilePath()
		{
			return Path.Combine(Directory, Filename);
		}

		public bool StateFileExists()
		{
			return File.Exists(GetFilePath());
		}

		public IntegrationResult LoadState()
		{
			using (TextReader reader = CreateTextReader(GetFilePath()))
			{
				return (IntegrationResult)_serializer.Deserialize(reader);
			}
		}
		
		public void SaveState(IntegrationResult result)
		{
			StringWriter buffer = new StringWriter();
			_serializer.Serialize(buffer, result);

			using (TextWriter writer = CreateTextWriter(GetFilePath()))
			{
				writer.Write(buffer.ToString());
				writer.Flush();
			}
		}

		private TextReader CreateTextReader(string path)
		{
			try 
			{
				return new StreamReader(path);
			}
			catch (IOException ex)
			{
				throw new CruiseControlException(string.Format("Unable to read the specified state file: {0}.  The path may be invalid.", path), ex); 
			}
		}

		private TextWriter CreateTextWriter(string path)
		{
			try
			{
				return new StreamWriter(path);
			}
			catch (SystemException ex)
			{
				throw new CruiseControlException(string.Format("Unable to save the IntegrationResult to the specified directory: {0}", path), ex);
			}
		}
	}
}
