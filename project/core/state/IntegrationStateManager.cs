using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Exortech.NetReflector;

namespace tw.ccnet.core.state
{
	[ReflectorType("state")]
	public class IntegrationStateManager : IStateManager
	{
		private XmlSerializer _serializer;
		private string _directory = System.IO.Directory.GetCurrentDirectory();
		private string _filename = "ccnet.state";

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

		public bool Exists()
		{
			return File.Exists(GetFilePath());
		}

		public IntegrationResult Load()
		{
			TextReader reader = CreateTextReader(GetFilePath());
			try
			{
				return (IntegrationResult)_serializer.Deserialize(reader);
			}
			finally
			{
				reader.Close();
			}
		}
		
		public void Save(IntegrationResult result)
		{
			TextWriter writer = CreateTextWriter(GetFilePath());
			try
			{
				_serializer.Serialize(writer, result);
				writer.Flush();
			}
			finally
			{
				writer.Close();
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
				throw new CruiseControlException(String.Format("Unable to read the specified state file: {0}.  The path may be invalid.", path), ex); 
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
				throw new CruiseControlException(String.Format("Unable to save the IntegrationResult to the specified directory: {0}", path), ex);
			}
		}
	}
}
