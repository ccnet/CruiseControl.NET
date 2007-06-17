using System;
using System.IO;
using System.Xml;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	public class XmlIntegrationResultWriter : IDisposable
	{
		private XmlFragmentWriter writer;

		public XmlIntegrationResultWriter(TextWriter textWriter)
		{
			writer = new XmlFragmentWriter(textWriter);
		}

		public void Write(IIntegrationResult result)
		{
			writer.WriteStartElement(Elements.CRUISE_ROOT);
			writer.WriteAttributeString("project", result.ProjectName);
			WriteRequest(result.IntegrationRequest);
			WriteModifications(result.Modifications);
			WriteBuildElement(result);
			WriteException(result);
			writer.WriteEndElement();
		}

		private void WriteRequest(IntegrationRequest request)
		{
			if (request == null) return;
			writer.WriteStartElement(Elements.Request);
			writer.WriteAttributeString("source", request.Source);
			writer.WriteAttributeString("buildCondition", request.BuildCondition.ToString());
			writer.WriteString(request.ToString());
			writer.WriteEndElement();
		}

		private void WriteTaskResults(IIntegrationResult result)
		{
			foreach (ITaskResult taskResult in result.TaskResults)
			{
				WriteOutput(taskResult.Data);
			}
		}

		public void WriteBuildElement(IIntegrationResult result)
		{
			writer.WriteStartElement(Elements.BUILD);
			writer.WriteAttributeString("date", DateUtil.FormatDate(result.StartTime));

			// hide the milliseconds
			TimeSpan time = result.TotalIntegrationTime;
			writer.WriteAttributeString("buildtime", string.Format("{0:d2}:{1:d2}:{2:d2}", time.Hours, time.Minutes, time.Seconds));
			if (result.Failed)
			{
				writer.WriteAttributeString("error", "true");
			}
			writer.WriteAttributeString("buildcondition", result.BuildCondition.ToString());
			WriteTaskResults(result);
			writer.WriteEndElement();
		}

		private void WriteOutput(string output)
		{
			writer.WriteNode(output);
		}

		private void WriteException(IIntegrationResult result)
		{
			if (result.ExceptionResult == null)
			{
				return;
			}

			writer.WriteStartElement(Elements.EXCEPTION);
			writer.WriteCData(result.ExceptionResult.ToString());
			writer.WriteEndElement();
		}

		void IDisposable.Dispose()
		{
			writer.Close();
		}

		public void WriteModifications(Modification[] mods)
		{
			writer.WriteStartElement(Elements.MODIFICATIONS);
			if (mods == null)
			{
				return;
			}
			foreach (Modification mod in mods)
			{
				mod.ToXml(writer);
			}
			writer.WriteEndElement();
		}

		private class Elements
		{
			public const string BUILD = "build";
			public const string CRUISE_ROOT = "cruisecontrol";
			public const string MODIFICATIONS = "modifications";
			public const string EXCEPTION = "exception";
			public const string Request = "request";
		}

		public Formatting Formatting
		{
			set { writer.Formatting = value; }
		}
	}
}