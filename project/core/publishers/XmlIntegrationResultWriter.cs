using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	public class XmlIntegrationResultWriter : IDisposable
	{
		private XmlFragmentWriter _writer;

		public XmlIntegrationResultWriter(TextWriter textWriter)
		{
			_writer = new XmlFragmentWriter(textWriter);
		}

		public void Write(IIntegrationResult result)
		{
			_writer.WriteStartElement(Elements.CRUISE_ROOT);
			_writer.WriteAttributeString("project", result.ProjectName);
			WriteModifications(result.Modifications);
			WriteBuildElement(result);
			WriteException(result);
			_writer.WriteEndElement();
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
			_writer.WriteStartElement(Elements.BUILD);
			_writer.WriteAttributeString("date", result.StartTime.ToString());

			// hide the milliseconds
			TimeSpan time = result.TotalIntegrationTime;
			_writer.WriteAttributeString("buildtime", string.Format("{0:d2}:{1:d2}:{2:d2}", time.Hours, time.Minutes, time.Seconds));
			if (result.Failed)
			{
				_writer.WriteAttributeString("error", "true");
			}
			_writer.WriteAttributeString("buildcondition", result.BuildCondition.ToString());
			WriteTaskResults(result);
			_writer.WriteEndElement();
		}

		private void WriteOutput(string output)
		{
			string xmlRemovedOutput = RemoveNulls(output);
			_writer.WriteNode(xmlRemovedOutput);
		}

		private string RemoveNulls(string s)
		{
			Regex nullStringRegex = new Regex("\0");
			return nullStringRegex.Replace(s, string.Empty);
		}

		private void WriteException(IIntegrationResult result)
		{
			if (result.ExceptionResult == null)
			{
				return;
			}

			_writer.WriteStartElement(Elements.EXCEPTION);
			_writer.WriteCData(XmlUtil.EncodeCDATA(result.ExceptionResult.ToString()));
			_writer.WriteEndElement();
		}

		void IDisposable.Dispose()
		{
			_writer.Close();
		}

		public void WriteModifications(Modification[] mods)
		{
			_writer.WriteStartElement(Elements.MODIFICATIONS);
			if (mods == null)
			{
				return;
			}
			foreach (Modification mod in mods)
			{
				mod.ToXml(_writer);
			}
			_writer.WriteEndElement();
		}

		private class Elements
		{
			public const string BUILD = "build";
			public const string CRUISE_ROOT = "cruisecontrol";
			public const string MODIFICATIONS = "modifications";
			public const string EXCEPTION = "exception";
		}

		public Formatting Formatting
		{
			set { _writer.Formatting = value; }
		}
	}
}