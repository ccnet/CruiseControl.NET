using System;
using System.IO;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	public class FileTaskResult : ITaskResult
	{
		private FileInfo _file;
		private string _data;

		public FileTaskResult(FileInfo file)
		{
			_file = file;
			_data = ReadXmlFile();
		}

		public string Data
		{
			get { return _data; }
		}

		private string ReadXmlFile()
		{
			if (! _file.Exists)
				return string.Empty;

			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(_file.FullName);
				return doc.DocumentElement.OuterXml;
			}
			catch (Exception ex)
			{
				throw new CruiseControlException("Unable to read the contents of the merge file: " + _file.Name, ex);
			}
		}
	}
}