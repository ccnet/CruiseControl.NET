using System.IO;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	public class FileTaskResult : ITaskResult
	{
		private FileInfo _file;

		public FileTaskResult(FileInfo file)
		{
			_file = file;
		}

		public string Data 
		{ 
			get
			{
				if (_file.Exists) 
				{
					XmlDocument doc = new XmlDocument();
					using (StreamReader reader = new StreamReader(_file.FullName)) 
					{
						doc.Load(reader);
						return doc.DocumentElement.OuterXml;
					}
				}
				return string.Empty;
			}
		}
	}
}
