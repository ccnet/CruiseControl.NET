using System;
using System.IO;
using System.Xml;
using Exortech.NetReflector;

namespace tw.ccnet.core.publishers 
{
	[ReflectorType("xmllogger")]
	public class XmlLogPublisher : PublisherBase
	{
		public class Elements 
		{
			public const string BUILD="build";
			public const string CRUISE_ROOT="cruisecontrol";
			public const string MODIFICATIONS="modifications";
			public const string INFO="info";
		}

		private string _logDir;

		public XmlLogPublisher() : base()
		{
		}

		[ReflectorProperty("logDir")]
		public string LogDir
		{
			get { return _logDir; }
			set { _logDir = value; }
		}

		public override void Publish(object source, IntegrationResult result)
		{
			string filename = GetFilename(result);
			XmlWriter writer = GetXmlWriter(LogDir, filename); 
			try 
			{
				Write(result, writer);
			}
			finally 
			{
				writer.Close();
			}
		}
		
		public XmlWriter GetXmlWriter(string dirname, string filename)
		{
			if (! Directory.Exists(dirname))
			{
				Directory.CreateDirectory(dirname);
			}
			string path = Path.Combine(dirname, filename);
			return new XmlTextWriter(path, System.Text.Encoding.UTF8);
		}
		
		public string GetFilename(IntegrationResult result)
		{
			DateTime date = result.LastModificationDate;
			if (result.Succeeded)
			{
				return LogFile.CreateFileName(date, result.Label);
			} 
			else 
			{
				return LogFile.CreateFileName(date);
			}
		}
		
		public void Write(IntegrationResult result, XmlWriter writer)
		{
			writer.WriteStartElement(Elements.CRUISE_ROOT);
			writer.WriteStartElement(Elements.MODIFICATIONS);
			Write(result.Modifications, writer);
			writer.WriteEndElement();
			WriteInfo(result, writer);
			writer.WriteEndElement();
		}
		
		public void WriteInfo(IntegrationResult IntegrationResult, XmlWriter writer)
		{
			writer.WriteStartElement(Elements.BUILD);
			writer.WriteAttributeString("date", IntegrationResult.StartTime.ToString());

			// hide the milliseconds
			TimeSpan time = IntegrationResult.TotalIntegrationTime;
			writer.WriteAttributeString("buildtime", string.Format("{0:d2}:{1:d2}:{2:d2}", time.Hours, time.Minutes, time.Seconds));
			if (IntegrationResult.Failed)
			{
				writer.WriteAttributeString("error", "true"); 
			}
			
			if (IntegrationResult.Output != null)
			{
				writer.WriteRaw(IntegrationResult.Output.ToString());
			}
			
			writer.WriteEndElement();
		}

		public void Write(Modification[] mods, XmlWriter writer)
		{
			if (mods == null)
			{
				return;
			}
			foreach (Modification mod in mods)
			{
				writer.WriteRaw(mod.ToXml());
			}
		}
	}
}


