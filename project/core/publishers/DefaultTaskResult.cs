using System;
using System.IO;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	public class DefaultTaskResult : ITaskResult
	{
		private FileInfo _file;

		public DefaultTaskResult(FileInfo file)
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
