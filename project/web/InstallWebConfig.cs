using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Xml;
using tw.ccnet.core;

namespace tw.ccnet.web
{
	[RunInstaller(true)]
	public class InstallWebConfig : Installer
	{
		public InstallWebConfig() : base()
		{
		}

		public override void Install(System.Collections.IDictionary stateSaver)
		{
			base.Install(stateSaver);

			string logFileDir = Context.Parameters["LogFileDir"];
			if (! Directory.Exists(logFileDir))
			{
				throw new CruiseControlException(string.Format(
				@"The specified log file folder: {0} does not exist", logFileDir));
			}	
			
			XmlDocument document = new XmlDocument();
			document.Load(GetWebConfigFilename());
			XmlElement logElement = document.GetElementById("logfile");
			XmlNode logNode = document.SelectSingleNode("//appSettings/add[@key = 'logDir']");
			logNode.Attributes["value"].Value = logFileDir;
			document.Save(GetWebConfigFilename());
		}

		private string GetWebConfigFilename()
		{
			return Path.Combine(Context.Parameters["WebFolder"], "Web.config");
		}

		public override void Uninstall(System.Collections.IDictionary savedState)
		{
			base.Uninstall(savedState);

			if (File.Exists(GetWebConfigFilename()))
			{
				File.Delete(GetWebConfigFilename());
			}
		}
	}
}
