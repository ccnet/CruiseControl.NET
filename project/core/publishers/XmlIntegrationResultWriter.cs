using System;
using System.Xml;
using System.Collections;
using System.IO;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Text.RegularExpressions;

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	public class XmlIntegrationResultWriter :IDisposable
	{
		private XmlWriter _writer;

	    public XmlIntegrationResultWriter(XmlWriter writer)
	    {
	        _writer = writer;
	    }

	    public void Write(IntegrationResult result)
		{
	        _writer.WriteStartElement(Elements.CRUISE_ROOT);
	        _writer.WriteStartElement(Elements.MODIFICATIONS);
			Write(result.Modifications);
	        _writer.WriteEndElement();
			WriteBuildElement(result);
			WriteTaskResults(result);
			WriteException(result);
	        _writer.WriteEndElement();
		}

		private void WriteTaskResults(IntegrationResult result)
		{	
		    foreach (ITaskResult taskResult in result.TaskResults)
		    {
		        _writer.WriteRaw(taskResult.Data);
		    }
		}

		public void WriteBuildElement(IntegrationResult result)
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
			
			if (result.Output != null)
			{
				WriteIntegrationResultOutput(result);
			}
			
			_writer.WriteEndElement();
		}

		private void WriteIntegrationResultOutput(IntegrationResult result)
		{
			string nullRemovedOutput = RemoveNulls(result.Output);
			XmlValidatingReader reader = new XmlValidatingReader(nullRemovedOutput, XmlNodeType.Element, null);
			try 
			{ 
				reader.ReadInnerXml();
				_writer.WriteNode(reader, false);
			}
			catch (XmlException) 
			{
				// IF we had a problem with the input xml, wrap it in CDATA and put that in instead
				_writer.WriteCData(XmlUtil.EncodeCDATA(nullRemovedOutput));
			}
			finally 
			{ 
				reader.Close(); 
			}
		}

		public string RemoveNulls(string s)
		{
			Regex nullStringRegex = new Regex("\0");
			return nullStringRegex.Replace(s, "");;
		}

		public void WriteException(IntegrationResult result)
		{
			if (result.ExceptionResult == null)
			{
				return;
			}

			_writer.WriteStartElement(Elements.EXCEPTION);
			_writer.WriteCData(XmlUtil.EncodeCDATA(result.ExceptionResult.ToString()));
			_writer.WriteEndElement();
		}

		public void Dispose()
		{
			_writer.Close();
		}

		public void Write(Modification[] mods)
		{
			if (mods == null)
			{
				return;
			}
			foreach (Modification mod in mods)
			{
				_writer.WriteRaw(mod.ToXml());
			}
		}

		public class Elements 
		{
			public const string BUILD = "build";
			public const string CRUISE_ROOT = "cruisecontrol";
			public const string MODIFICATIONS = "modifications";
			public const string INFO = "info";
			public const string EXCEPTION = "exception";
		}

	}
}
