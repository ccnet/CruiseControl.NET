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
		private static void Main()
		{
			ServiceBase.Run(new ServiceBase[] {new CCService()});
		}

		private ICruiseServer server;

		public CCService()
		{
			this.ServiceName = "CCService";
		}

		private string ConfigFileName
		{
			get { return ConfigurationSettings.AppSettings["ccnet.config"]; }
		}

		private string Remoting
		{
			get { return ConfigurationSettings.AppSettings["remoting"]; }
		}

		protected override void OnStart(string[] args)
		{
			// Set working directory to service executable's home directory.
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

			string configFile = GetConfigFilename();
			VerifyConfigFileExists(configFile);
			CreateAndStartCruiseServer(configFile);
		}

		private string GetConfigFilename()
		{
			string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string defaultConfigFile = appDirectory + @"\ccnet.config";
			string configFile = ConfigFileName;
			if (configFile == null || configFile.Trim().Length == 0) configFile = defaultConfigFile;
			return configFile;
		}
		
		private void VerifyConfigFileExists(string configFile)
		{
			FileInfo configFileInfo = new FileInfo(configFile);
			if (!configFileInfo.Exists)
			{
				throw new Exception(string.Format("CruiseControl.NET configuration file {0} does not exist.", configFileInfo.FullName));
			}			
		}
		
		private void CreateAndStartCruiseServer(string configFile)
		{
			server = new CruiseServerFactory().Create(UseRemoting(), configFile);
			server.Start();
		}

		private bool UseRemoting()
		{
			return (Remoting != null && Remoting.Trim().ToLower() == "on");
		}

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
