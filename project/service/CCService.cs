using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Service
{
	public class CCService : ServiceBase
	{
		static void Main()
		{
			ServiceBase.Run(new ServiceBase[] { new CCService() } );
		}

		private ICruiseServer server;

		public CCService()
		{
			this.ServiceName = "CCService";
		}

		/// <summary>
		/// Start the service
		/// </summary>
		protected override void OnStart(string[] args)
		{
			// Set working directory to service executable's home directory.
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

			string configFile = GetConfigFilename();
			FileInfo configFileInfo = (new FileInfo(configFile));

			if (! configFileInfo.Exists)
			{
				EventLog.WriteEntry("CCService", string.Format("Config file {0} does not exist - exiting application", configFileInfo.FullName), EventLogEntryType.Error);
				return;
			}

			server = CruiseServerFactory.Create(UseRemoting(), configFile);
			server.Start();
		}

		private string ConfigFileName { get { return ConfigurationSettings.AppSettings["ccnet.config"]; } }
		private string Remoting       { get { return ConfigurationSettings.AppSettings["remoting"]; } }

		private string GetConfigFilename()
		{
			string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string defaultConfigFile = appDirectory + @"\ccnet.config";
			string configFile = ConfigFileName;
			if (configFile == null || configFile.Trim().Length == 0)
				configFile = defaultConfigFile;
			return configFile;
		}

		private bool UseRemoting()
		{
			return (Remoting != null && Remoting.Trim().ToLower() == "on");
		}
 
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			server.Abort();
		}

		protected override void OnPause()
		{
			server.Stop();
		}

		protected override void OnContinue()
		{
			server.Start();
		}
	}
}
