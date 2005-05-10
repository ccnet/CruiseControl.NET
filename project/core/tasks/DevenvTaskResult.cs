using System;
using System.IO;
using System.Xml;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
	public class DevenvTaskResult : ProcessTaskResult
	{
		public DevenvTaskResult(ProcessResult result) : base(result)
		{
		}

		public override string Data
		{
			get { return TransformDevenvOutput(result.StandardOutput); }
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
					writer.WriteElementString("message", StringUtil.RemoveNulls(line));
				}
			}
		}
	}
}