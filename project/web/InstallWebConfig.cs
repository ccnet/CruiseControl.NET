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
				throw new CruiseControlException(String.Format(
				@"The specified log file folder: {0} does not exist", logFileDir));
			}
			
			XmlDocument document = new XmlDocument();
			document.LoadXml(GetWebConfigXml(logFileDir));
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

		private string GetWebConfigXml(string logFileDir)
		{
			return String.Format(@"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
  <system.web>
    <compilation defaultLanguage=""c#"" debug=""true""/>
    <customErrors mode=""RemoteOnly"" /> 
    <authentication mode=""Windows"" /> 
    <trace enabled=""false"" requestLimit=""10"" pageOutput=""false"" traceMode=""SortByTime"" localOnly=""true""/>
    <sessionState mode=""InProc"" stateConnectionString=""tcpip=127.0.0.1:42424"" sqlConnectionString=""data source=127.0.0.1;user id=sa;password="" cookieless=""false"" timeout=""20"" />
    <globalization requestEncoding=""utf-8"" responseEncoding=""utf-8"" />
  </system.web>

  <appSettings>
	<add key=""logDir"" value=""{0}"" />
  </appSettings>
</configuration>", logFileDir);
		}
	}
}
