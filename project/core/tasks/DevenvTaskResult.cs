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
			get { return TransformDevenvOutput(result.StandardOutput, result.StandardError); }
		}

        /// <summary>
        /// Transform the devenv output streams into an XML build report fragment and return it.
        /// </summary>
        /// <param name="devenvOutput">devenv's standard output with platform-specific newlines</param>
        /// <param name="devenvError">devenv's standard error with platform-specific newlines</param>
        /// <returns>the resulting build report fragment</returns>
        private static string TransformDevenvOutput(string devenvOutput, string devenvError)
		{
			StringWriter output = new StringWriter();
			XmlWriter writer = new XmlTextWriter(output);
			writer.WriteStartElement("buildresults");
			WriteContent(writer, devenvOutput, "message");
            WriteContent(writer, devenvError, "message level=\"error\"");
            writer.WriteEndElement();
			return output.ToString();
		}

        /// <summary>
        /// Add the lines of output from devenv's standard output and standard error streams
        /// to the build results.
        /// </summary>
        /// <param name="writer">an <c>XmlWriter</c> to receive the output</param>
        /// <param name="messages">the messages, with platform-specific newlines between them</param>
        /// <param name="elementString">the XML element string for this type of output</param>
		private static void WriteContent(XmlWriter writer, string messages, string elementString)
		{
            StringReader reader = new StringReader(messages);
			while (reader.Peek() >= 0)
			{
				string line = reader.ReadLine();
				if (! StringUtil.IsBlank(line))
				{
                    writer.WriteElementString(elementString, StringUtil.RemoveNulls(line));
				}
			}
		}
	}
}