using System.IO;
using System.Xml;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Builder
{
	public class DevenvTaskResult : ITaskResult
	{
		private string data;

		public DevenvTaskResult(string devenvOutput)
		{
			data = TransformDevenvOutput(devenvOutput);
		}

		private string TransformDevenvOutput(string devenvOutput)
		{
			StringWriter result = new StringWriter();
			XmlWriter writer = new XmlTextWriter(result);
			writer.WriteStartElement("buildresults");
			WriteContent(writer, devenvOutput);
			writer.WriteEndElement();
			return result.ToString();
		}

		private void WriteContent(XmlWriter writer, string devenvOutput)
		{
			StringReader reader = new StringReader(devenvOutput);
			while (reader.Peek() >= 0)
			{
				string line = reader.ReadLine();
				if (! StringUtil.IsBlank(line))
				{
					writer.WriteElementString("message", RemoveNulls(line));
				}
			}
		}

		private string RemoveNulls(string line)
		{
			return line.Replace("\0", string.Empty);
		}

		public string Data
		{
			get { return data; }
		}
	}
}